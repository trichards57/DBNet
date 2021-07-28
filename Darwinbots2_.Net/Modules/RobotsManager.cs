using DarwinBots.Model;
using DarwinBots.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DarwinBots.Modules
{
    internal interface IRobotManager
    {
        List<Robot> Robots { get; }

        int TotalRobots => Robots.Count(r => r.Exists);

        Robot GetNewBot();
    }

    internal class RobotsManager : IRobotManager
    {
        public const int CubicTwipPerBody = 905;
        public const int GeneticSensitivity = 75;
        public const int MaxMem = 1000;
        private readonly IBucketManager _bucketManager;
        private readonly IObstacleManager _obstacleManager;
        private readonly IShotManager _shotManager;

        public RobotsManager(IBucketManager bucketManager, IObstacleManager obstacleManager, IShotManager shotManager)
        {
            _bucketManager = bucketManager;
            _obstacleManager = obstacleManager;
            _shotManager = shotManager;
        }

        public List<Robot> Robots { get; } = new();
        public int TotalChlr { get; set; }

        public int TotalNotVegs => Robots.Count(r => r.Exists && !r.IsCorpse && !r.IsVegetable);
        public int TotalRobots => Robots.Count(r => r.Exists);

        private List<Robot> BotsToKill { get; } = new();
        private List<Robot> BotsToReproduce { get; } = new();
        private List<Robot> BotsToReproduceSexually { get; } = new();

        public static double AbsX(double aim, int up, int dn, int sx, int dx)
        {
            var upTotal = Math.Clamp(up - dn, -32000, 32000);
            var sxTotal = Math.Clamp(sx - dx, -32000, 32000);

            return Math.Cos(aim) * upTotal + Math.Sin(aim) * sxTotal;
        }

        public static double AbsY(double aim, int up, int dn, int sx, int dx)
        {
            var upTotal = Math.Clamp(up - dn, -32000, 32000);
            var sxTotal = Math.Clamp(sx - dx, -32000, 32000);

            return -Math.Sin(aim) * upTotal + Math.Cos(aim) * sxTotal;
        }

        public Robot GetNewBot()
        {
            var robot = new Robot(_bucketManager);

            Robots.Add(robot);

            return robot;
        }

        public async Task KillRobot(Robot robot)
        {
            if (SimOpt.SimOpts.DeadRobotSnp && (!robot.IsVegetable || !SimOpt.SimOpts.SnpExcludeVegs))
                await Database.AddRecord(robot);

            robot.CleanUp(_shotManager, _bucketManager);
            Robots.Remove(robot);
        }

        public async Task UpdateBots()
        {
            BotsToKill.Clear();
            BotsToReproduce.Clear();
            BotsToReproduceSexually.Clear();
            Vegs.TotalVegsDisplayed = Vegs.TotalVegs;
            Vegs.TotalVegs = 0;

            if (SimOpt.TmpOpts.Tides == 0)
            {
                Physics.BouyancyScaling = 1;
            }
            else
            {
                Physics.BouyancyScaling = (1 + Math.Sin((float)(SimOpt.SimOpts.TotRunCycle + SimOpt.TmpOpts.TidesOf) % SimOpt.TmpOpts.Tides / SimOpt.SimOpts.Tides * Math.PI * 2)) / 2;
                Physics.BouyancyScaling = Math.Sqrt(Physics.BouyancyScaling);
                SimOpt.SimOpts.YGravity = (1 - Physics.BouyancyScaling) * 4;
                SimOpt.SimOpts.PhysBrown = Physics.BouyancyScaling > 0.8 ? 10 : 0;
            }

            //this loops is for pre update
            foreach (var rob in Robots.Where(r => r.Exists))
            {
                rob.Upkeep(SimOpt.SimOpts.Costs);
                rob.Poisons();
                if (!SimOpt.SimOpts.DisableFixing)
                    rob.ManageFixed();
                rob.UpdateMass();

                if (_obstacleManager.Obstacles.Count > 0)
                    _obstacleManager.DoObstacleCollisions(rob);

                Physics.BorderCollision(_bucketManager, rob);

                Physics.TieHooke(rob); // Handles tie lengths, tie hardening and compressive, elastic tie forces

                if (!rob.IsCorpse && !rob.DnaDisabled)
                    Physics.TieTorque(rob);

                if (!rob.IsFixed)
                    Physics.NetForces(this, rob); //calculate forces on all robots

                _bucketManager.BucketsCollision(rob);

                if (rob.StaticImpulse > 0 & (rob.IndependentImpulse.X != 0 || rob.IndependentImpulse.Y != 0))
                {
                    double staticV;
                    if (rob.Velocity.X == 0 & rob.Velocity.Y == 0)
                        staticV = rob.StaticImpulse;
                    else
                        staticV = rob.StaticImpulse * Math.Abs(DoubleVector.Cross(rob.Velocity.Unit(), rob.IndependentImpulse.Unit())); // Takes into account the fact that the robot may be moving along the same vector

                    if (staticV > rob.IndependentImpulse.Magnitude())
                        rob.IndependentImpulse = new DoubleVector(0, 0); // If static vector is greater then impulse vector, reset impulse vector
                }

                rob.IndependentImpulse -= rob.ResistiveImpulse;

                if (rob.IsCorpse || rob.DnaDisabled) continue;

                rob.TiePortCommunication(); //transfer data through ties
                rob.ReadTie(); //reads all of the tref variables from a given tie number
            }

            foreach (var s in SimOpt.SimOpts.Specie)
                s.Population = 0;

            foreach (var rob in Robots.Where(r => r.Exists))
                await UpdateCounters(rob); // Counts the number of bots and decays body...

            foreach (var rob in Robots.Where(r => r.Exists))
            {
                Ties.UpdateTies(rob); // Carries all tie routines

                rob.DoGeneticMemory();

                if (!rob.IsCorpse && !rob.DnaDisabled)
                {
                    rob.SetAim(); //Setup aiming
                    BotDnaManipulation(rob);
                }

                rob.UpdatePosition(SimOpt.SimOpts.MaxVelocity, SimOpt.SimOpts.ZeroMomentum, SimOpt.SimOpts.Density, SimOpt.SimOpts.FixedBotRadii);
            }

            foreach (var rob in Robots)
            {
                rob.UpdateTieAngles(); // Updates .tielen and .tieang.  Have to do this here after all bot movement happens above.

                if (!rob.IsCorpse && !rob.DnaDisabled && rob.Exists)
                {
                    NeoMutations.Mutate(this, rob);
                    rob.MakeStuff(SimOpt.SimOpts.Costs);
                    rob.HandleWaste(_shotManager);
                    Shoot(rob);
                    rob.ManageChlr(TotalChlr, SimOpt.SimOpts.Costs.CholorplastCost * SimOpt.SimOpts.Costs.CostMultiplier, SimOpt.SimOpts.MaxPopulation);
                    rob.ManageBody();
                    rob.ManageBouyancy();
                    ManageReproduction(rob);
                    rob.Shock();
                    Senses.WriteSenses(this, _bucketManager, rob);
                    rob.FireTies();
                }
                rob.Ageing(); // Even bots with disabled DNA age...

                if (!rob.IsCorpse && rob.Exists)
                {
                    ManageDeath(rob); // Even bots with disabled DNA can die...
                }
                if (rob.Exists)
                {
                    Vegs.TotalSimEnergy[Vegs.CurrentEnergyCycle] = (int)(Vegs.TotalSimEnergy[Vegs.CurrentEnergyCycle] + rob.Energy + rob.Body * 10);
                }
            }

            await ReproduceAndKill();
            RemoveExtinctSpecies();
        }

        private void BotDnaManipulation(Robot rob)
        {
            // count down
            if (rob.VirusTimer > 1)
                rob.VirusTimer--;

            rob.Memory[MemoryAddresses.Vtimer] = rob.VirusTimer;

            // Viruses
            if (rob.Memory[MemoryAddresses.mkvirus] > 0 & rob.VirusTimer == 0)
            {
                if (rob.Chloroplasts == 0)
                {
                    if (_shotManager.MakeVirus(rob, rob.Memory[MemoryAddresses.mkvirus]))
                    {
                        var length = GeneLength(rob, rob.Memory[MemoryAddresses.mkvirus]) * 2;
                        rob.Energy -= length / 2.0 * SimOpt.SimOpts.Costs.DnaCopyCost * SimOpt.SimOpts.Costs.CostMultiplier;
                        rob.VirusTimer = Math.Min(length, 32000);
                    }
                    else
                    {
                        rob.VirusTimer = 0;
                        rob.VirusShot = null;
                    }
                }
                else
                {
                    rob.Chloroplasts = 0;
                }
            }

            // shoot it!
            if (rob.Memory[MemoryAddresses.VshootSys] != 0 & rob.VirusTimer == 1)
            {
                // Botsareus 10/5/2015 Bugfix for negative values in vshoot
                _shotManager.ShootVirus(rob, rob.VirusShot);

                rob.Memory[MemoryAddresses.VshootSys] = 0;
                rob.Memory[MemoryAddresses.Vtimer] = 0;
                rob.Memory[MemoryAddresses.mkvirus] = 0;
                rob.VirusTimer = 0;
                rob.VirusShot = null;
            }

            // Other stuff

            if (rob.Memory[MemoryAddresses.DelgeneSys] > 0)
            {
                NeoMutations.DeleteGene(rob, rob.Memory[MemoryAddresses.DelgeneSys]);
                rob.Memory[MemoryAddresses.DelgeneSys] = 0;
            }

            rob.Memory[MemoryAddresses.DnaLenSys] = rob.Dna.Count;
            rob.Memory[MemoryAddresses.GenesSys] = rob.NumberOfGenes;
        }

        private List<DnaBlock> Crossover(List<block2> dna1, List<block2> dna2)
        {
            var nn = 0;
            var res1 = 0;//result1
            var res2 = 0;
            var resn = 0;
            var i = 0;//layer

            while (true)
            {
                //diff search

                var n1 = res1 + resn - nn;//start pos
                var n2 = res2 + resn - nn;

                //presets

                var outDna = new List<DnaBlock>();

                var temp = 0;
                res1 = ScanFromN(dna1, n1, ref temp);
                res2 = ScanFromN(dna2, n2, ref i);

                if (res1 - n1 > 0 & res2 - n2 > 0)
                {
                    if (ThreadSafeRandom.Local.Next(0, 2) == 0)
                    {
                        for (var a = n1; a < res1 - 1; a++)
                            outDna.Add(new DnaBlock { Type = dna1[a].tipo, Value = dna1[a].value });
                    }
                    else
                    {
                        for (var a = n2; a < res2 - 1; a++)
                            outDna.Add(new DnaBlock { Type = dna2[a].tipo, Value = dna2[a].value });
                    }
                }
                else if (res1 - n1 > 0)
                {
                    if (ThreadSafeRandom.Local.Next(0, 2) == 0)
                    {
                        for (var a = n1; a < res1 - 1; a++)
                            outDna.Add(new DnaBlock { Type = dna1[a].tipo, Value = dna1[a].value });
                    }
                }
                else if (res2 - n2 > 0)
                {
                    if (ThreadSafeRandom.Local.Next(0, 2) == 0)
                    {
                        for (var a = n2; a < res2 - 1; a++)
                            outDna.Add(new DnaBlock { Type = dna2[a].tipo, Value = dna2[a].value });
                    }
                }

                //same search

                if (i == 0)
                    return outDna;

                nn = res1;
                resn = ScanFromN(dna1, nn, ref i);

                var whatside = ThreadSafeRandom.Local.Next(0, 2) == 0;

                for (var a = nn; a < resn - 1; a++)
                {
                    var block = new DnaBlock
                    {
                        Type = whatside ? dna1[a].tipo : dna2[a - nn + res2].tipo,
                        Value = (dna1[a].tipo == dna2[a - nn + res2].tipo && Math.Abs(dna2[a].value) > 999 && Math.Abs(dna2[a - nn + res2].value) > 999 ? ThreadSafeRandom.Local.Next(0, 2) == 0 : whatside) ? dna1[a].value : dna2[a - nn + res2].value
                    };
                    outDna.Add(block);
                }
            }
        }

        private int GeneLength(Robot rob, int p)
        {
            var pos = DnaManipulations.GenePosition(rob.Dna, p);
            return DnaManipulations.GeneEnd(rob.Dna, pos) - pos + 1;
        }

        private double GeneticDistance(List<block3> rob1, List<block3> rob2)
        {
            return rob1.Count(b => b.match == 0) + rob2.Count(b => b.match == 0) / (rob1.Count + rob2.Count);
        }

        private void ManageDeath(Robot rob)
        {
            if (SimOpt.SimOpts.CorpseEnabled)
            {
                if (!rob.IsCorpse && rob.Energy < 15 && rob.Age > 0)
                {
                    rob.IsCorpse = true;
                    rob.FName = "Corpse";
                    Array.Clear(rob.occurr, 0, rob.occurr.Length);
                    rob.Color = Colors.White;
                    rob.IsVegetable = false;
                    rob.IsFixed = false;
                    rob.Energy = 0;
                    rob.DnaDisabled = true;
                    rob.MovementSysvarsDisabled = true;
                    rob.CantSee = true;
                    rob.IsVirusImmune = true;
                    rob.Chloroplasts = 0;

                    for (var i = MemoryAddresses.EyeStart + 1; i < MemoryAddresses.EyeEnd; i++)
                        rob.Memory[i] = 0;

                    rob.Bouyancy = 0;
                }
                if (rob.Body < 0.5)
                    rob.IsDead = true;
            }
            else if (rob.Energy < 0.5 || rob.Body < 0.5)
                rob.IsDead = true;

            if (rob.IsDead)
                BotsToKill.Add(rob);
        }

        private void ManageReproduction(Robot rob)
        {
            switch (rob.Fertilized)
            {
                case >= 0:
                    rob.Fertilized--;
                    rob.Memory[MemoryAddresses.SYSFERTILIZED] = rob.Fertilized >= 0 ? rob.Fertilized : 0;
                    break;

                case < -10:
                    rob.Fertilized++;
                    break;

                default:
                    if (rob.Fertilized == -1)
                        rob.SpermDna.Clear();
                    rob.Fertilized = -2;
                    break;
            }

            // Asexual reproduction
            if ((rob.Memory[MemoryAddresses.Repro] > 0 || rob.Memory[MemoryAddresses.mrepro] > 0) && !rob.CantReproduce)
                BotsToReproduce.Add(rob);

            // Sexual Reproduction
            if (rob.Memory[MemoryAddresses.SEXREPRO] > 0 & rob.Fertilized >= 0 & !rob.CantReproduce)
                BotsToReproduceSexually.Add(rob);
        }

        private MutationProbability MutateProbability(MutationProbability probability)
        {
            var p = probability.Probability;
            p /= 10;
            if (p == 0)
                p = 1000;

            return probability with { Probability = p };
        }

        private void RemoveExtinctSpecies()
        {
            foreach (var s in SimOpt.SimOpts.Specie.Where(s => s.Population == 0 && !s.Native).ToArray())
                SimOpt.SimOpts.Specie.Remove(s);
        }

        private void Reproduce(Robot robot, int per)
        {
            if (robot.Body < 5)
                return;

            if (SimOpt.SimOpts.DisableTypArepro && robot.IsVegetable == false)
                return;

            if (robot.Body <= 2 || robot.CantReproduce)
                return;

            // Attempt to stop veg overpopulation but will it work?
            if (robot.IsVegetable && (TotalChlr > SimOpt.SimOpts.MaxPopulation || Vegs.TotalVegsDisplayed < 0))
                return;

            // If we got here and it's a veg, then we are below the reproduction threshold.  Let a random 10% of the veggis reproduce
            // so as to avoid all the veggies reproducing on the same cycle.  This adds some randomness
            // so as to avoid giving preference to veggies with lower bot array numbers.  If the veggy population is below 90% of the threshold
            // then let them all reproduce.
            if (robot.IsVegetable && ThreadSafeRandom.Local.Next(0, 10) != 5 && TotalChlr > SimOpt.SimOpts.MaxPopulation * 0.9)
                return;
            if (Vegs.TotalVegsDisplayed == -1)
                return;

            per %= 100; // per should never be <=0 as this is checked in ManageReproduction()

            if (per <= 0)
                return;

            var sondist = (int)robot.GetRadius(SimOpt.SimOpts.FixedBotRadii);

            if (robot.Energy <= 0)
                return;

            var nx = (int)(robot.Position.X + AbsX(robot.Aim, sondist, 0, 0, 0));
            var ny = (int)(robot.Position.Y + AbsY(robot.Aim, sondist, 0, 0, 0));

            if (SimpleCollision(nx, ny, robot) || !robot.Exists)
                return;

            var nuovo = GetNewBot();

            SimOpt.SimOpts.TotBorn = SimOpt.SimOpts.TotBorn + 1;

            if (robot.IsVegetable)
                Vegs.TotalVegs++;

            nuovo.Dna.AddRange(robot.Dna);
            nuovo.NumberOfGenes = robot.NumberOfGenes;
            nuovo.MutationProbabilities = robot.MutationProbabilities;

            nuovo.Mutations = robot.Mutations;
            nuovo.OldMutations = robot.OldMutations;

            nuovo.LastMutation = 0;
            nuovo.LastMutationDetail.AddRange(robot.LastMutationDetail);

            for (var t = 0; t < 12; t++)
                nuovo.Skin[t] = robot.Skin[t];

            Array.Clear(nuovo.Memory, 0, nuovo.Memory.Length);
            nuovo.Ties.Clear();

            nuovo.Position = new DoubleVector(robot.Position.X + AbsX(robot.Aim, sondist, 0, 0, 0), robot.Position.Y + AbsY(robot.Aim, sondist, 0, 0, 0));
            nuovo.BucketPosition = new IntVector(-2, -2);
            _bucketManager.UpdateBotBucket(nuovo);
            nuovo.Velocity = robot.Velocity;
            nuovo.ActualVelocity = robot.ActualVelocity;
            nuovo.Color = robot.Color;
            nuovo.Aim = Physics.NormaliseAngle(robot.Aim + Math.PI);

            nuovo.Memory[MemoryAddresses.SetAim] = (int)(nuovo.Aim * 200);
            nuovo.Memory[468] = 32000;
            nuovo.IsCorpse = false;
            nuovo.IsDead = false;
            nuovo.Generation = robot.Generation + 1;

            if (nuovo.Generation > 32000)
                nuovo.Generation = 32000;

            nuovo.BirthCycle = SimOpt.SimOpts.TotRunCycle;

            var nnrg = robot.Energy / 100 * per;
            var nbody = robot.Body / 100 * per;
            var nwaste = robot.Waste / 100 * per;
            var npwaste = robot.PermanentWaste / 100 * per;
            var nchloroplasts = robot.Chloroplasts / 100 * per;

            robot.Energy -= nnrg + nnrg * 0.001;
            robot.Waste -= nwaste;
            robot.PermanentWaste -= npwaste;
            robot.Body -= nbody;
            robot.Chloroplasts -= nchloroplasts;

            nuovo.Chloroplasts = nchloroplasts; //Panda 8/23/2013 Distribute the chloroplasts
            nuovo.Body = nbody;
            nuovo.Waste = nwaste;
            nuovo.PermanentWaste = npwaste;
            robot.Memory[MemoryAddresses.Energy] = (int)robot.Energy;
            robot.Memory[MemoryAddresses.body] = (int)robot.Body;
            robot.SonNumber++;

            if (robot.SonNumber > 32000)
                robot.SonNumber = 32000;

            nuovo.Energy = nnrg * 0.999; // Make reproduction cost 1% of nrg transfer
            nuovo.OldEnergy = nnrg * 0.999;
            nuovo.Memory[MemoryAddresses.Energy] = (int)nuovo.Energy;
            nuovo.IsPoisoned = false;
            nuovo.Parent = robot;
            nuovo.FName = robot.FName;
            nuovo.IsVegetable = robot.IsVegetable;
            nuovo.ChloroplastsDisabled = robot.ChloroplastsDisabled; //Botsareus 3/28/2014 Disable chloroplasts
            nuovo.IsFixed = robot.IsFixed;
            nuovo.CantSee = robot.CantSee;
            nuovo.DnaDisabled = robot.DnaDisabled;
            nuovo.MovementSysvarsDisabled = robot.MovementSysvarsDisabled;
            nuovo.CantReproduce = robot.CantReproduce;
            nuovo.IsVirusImmune = robot.IsVirusImmune;
            if (nuovo.IsFixed)
                nuovo.Memory[MemoryAddresses.Fixed] = 1;

            nuovo.SubSpecies = robot.SubSpecies;
            nuovo.GenMut = robot.GenMut;
            nuovo.Bouyancy = robot.Bouyancy;

            if (robot.MultibotTimer > 0)
                nuovo.MultibotTimer = robot.MultibotTimer / 2 + 2;

            nuovo.VirusTimer = 0;
            nuovo.VirusShot = null;

            for (var i = 0; i < 4; i++)
                nuovo.Memory[971 + i] = robot.Memory[971 + i];

            //The other 15 genetic memory locations are stored now but can be used later
            for (var i = 0; i < 14; i++)
                nuovo.EpigeneticMemory[i] = robot.Memory[976 + i];

            //Erase parents genetic memory now to prevent him from completing his own transfer by using his kid
            for (var i = 0; i < 14; i++)
                robot.EpigeneticMemory[i] = 0;

            //Botsareus 12/17/2013 Delta2
            if (robot.Memory[MemoryAddresses.mrepro] > 0)
            {
                var temp = nuovo.MutationProbabilities;

                nuovo.MutationProbabilities.EnableMutations = true; // mutate even if mutations disabled for this bot

                nuovo.MutationProbabilities.CopyError = MutateProbability(nuovo.MutationProbabilities.CopyError);
                nuovo.MutationProbabilities.Delta = MutateProbability(nuovo.MutationProbabilities.Delta);
                nuovo.MutationProbabilities.Insertion = MutateProbability(nuovo.MutationProbabilities.Insertion);
                nuovo.MutationProbabilities.MajorDeletion = MutateProbability(nuovo.MutationProbabilities.MajorDeletion);
                nuovo.MutationProbabilities.MinorDeletion = MutateProbability(nuovo.MutationProbabilities.MinorDeletion);
                nuovo.MutationProbabilities.PointMutation = MutateProbability(nuovo.MutationProbabilities.PointMutation);
                nuovo.MutationProbabilities.Reversal = MutateProbability(nuovo.MutationProbabilities.Reversal);

                NeoMutations.Mutate(this, nuovo, true);

                nuovo.MutationProbabilities = temp;
            }
            else
                NeoMutations.Mutate(this, nuovo, true);

            Senses.MakeOccurrList(nuovo);
            nuovo.NumberOfGenes = DnaManipulations.CountGenes(nuovo.Dna);
            nuovo.Memory[MemoryAddresses.DnaLenSys] = nuovo.Dna.Count;
            nuovo.Memory[MemoryAddresses.GenesSys] = nuovo.NumberOfGenes;

            Ties.MakeTie(robot, nuovo, sondist, 100, 0); //birth ties last 100 cycles
            robot.OldEnergy = robot.Energy; //saves parent from dying from shock after giving birth
            nuovo.Mass = nbody / 1000 + nuovo.Shell / 200;
            nuovo.Memory[MemoryAddresses.timersys] = robot.Memory[MemoryAddresses.timersys]; //epigenetic timer

            //Successfully reproduced
            robot.Memory[MemoryAddresses.Repro] = 0;
            robot.Memory[MemoryAddresses.mrepro] = 0;

            robot.Energy -= robot.Dna.Count * SimOpt.SimOpts.Costs.DnaCopyCost * SimOpt.SimOpts.Costs.CostMultiplier;
            if (robot.Energy < 0)
                robot.Energy = 0;
        }

        private async Task ReproduceAndKill()
        {
            foreach (var r in BotsToReproduce)
            {
                var per = 0;

                if (r.Memory[MemoryAddresses.mrepro] > 0 && r.Memory[MemoryAddresses.Repro] > 0)
                {
                    per = ThreadSafeRandom.Local.NextDouble() > 0.5 ? r.Memory[MemoryAddresses.Repro] : r.Memory[MemoryAddresses.mrepro];
                }
                else
                {
                    if (r.Memory[MemoryAddresses.mrepro] > 0)
                    {
                        per = r.Memory[MemoryAddresses.mrepro];
                    }
                    if (r.Memory[MemoryAddresses.Repro] > 0)
                    {
                        per = r.Memory[MemoryAddresses.Repro];
                    }
                }

                Reproduce(r, per);
            }

            foreach (var r in BotsToReproduceSexually)
            {
                SexReproduce(r);
            }

            foreach (var r in BotsToKill)
                await KillRobot(r);
        }

        private int ScanFromN(IList<block2> rob, int n, ref int layer)
        {
            for (var a = n; a < rob.Count; a++)
            {
                if (rob[a].match == layer)
                    continue;

                layer = rob[a].match;
                return a;
            }
            return rob.Count;
        }

        private void SexReproduce(Robot female)
        {
            if (female.Body < 5 || !female.Exists || female.IsCorpse || female.CantReproduce || female.Body <= 2 || !female.SpermDna.Any())
                return;

            //The percent of resources given to the offspring comes exclusivly from the mother
            //Perhaps this will lead to sexual selection since sex is expensive for females but not for males
            double per = female.Memory[MemoryAddresses.SEXREPRO];

            //veggies can reproduce sexually, but we still have to test for veggy population controls
            //we let male veggies fertilize nonveggie females all they want since the offspring's "species" and thus vegginess
            //will be determined by their mother.  Perhaps a strategy will emerge where plants compete to reproduce
            //with nonveggies so as to bypass the popualtion limtis?  Who knows.
            if (female.IsVegetable && (TotalChlr > SimOpt.SimOpts.MaxPopulation || Vegs.TotalVegsDisplayed < 0))
                return;

            // If we got here and the female is a veg, then we are below the reproduction threshold.  Let a random 10% of the veggis reproduce
            // so as to avoid all the veggies reproducing on the same cycle.  This adds some randomness
            // so as to avoid giving preference to veggies with lower bot array numbers.  If the veggy population is below 90% of the threshold
            // then let them all reproduce.
            if (female.IsVegetable && ThreadSafeRandom.Local.Next(0, 9) != 5 && TotalChlr > SimOpt.SimOpts.MaxPopulation * 0.9)
                return;

            if (Vegs.TotalVegsDisplayed == -1)
                return;// no veggies can reproduce on the first cycle after the sim is restarted.

            per %= 100; // per should never be <=0 as this is checked in ManageReproduction()

            if (per <= 0)
                return;

            var sondist = female.GetRadius(SimOpt.SimOpts.FixedBotRadii);

            var tempnrg = female.Energy;

            if (tempnrg <= 0)
                return;

            var nx = female.Position.X + AbsX(female.Aim, (int)sondist, 0, 0, 0);
            var ny = female.Position.Y + AbsY(female.Aim, (int)sondist, 0, 0, 0);

            if (SimpleCollision((int)nx, (int)ny, female) || !female.Exists)
                return;

            //Step1 Copy both dnas into block2

            var dna1 = female.Dna.Select(d => new block2 { tipo = d.Type, value = d.Value }).ToList();
            var dna2 = female.SpermDna.Select(d => new block2 { tipo = d.Type, value = d.Value }).ToList();

            //Step2 map nucli

            var ndna1 = dna1.Select(d => new block3 { nucli = DnaTokenizing.DnaToInt(d.tipo, d.value) }).ToList();
            var ndna2 = dna2.Select(d => new block3 { nucli = DnaTokenizing.DnaToInt(d.tipo, d.value) }).ToList();

            //Step3 Check longest sequences

            SimpleMatch(ndna1, ndna2);

            //If robot is too unsimiler then do not reproduce and block sex reproduction for 8 cycles

            if (GeneticDistance(ndna1, ndna2) > 0.6)
            {
                female.Fertilized = -18;
                return;
            }

            //Step4 map back

            for (var t = 0; t < dna1.Count; t++)
                dna1[t].match = ndna1[t].match;

            for (var t = 0; t < dna2.Count; t++)
                dna2[t].match = ndna2[t].match;

            //Step5 do crossover

            var outDna = Crossover(dna1, dna2);
            var nuovo = GetNewBot();

            SimOpt.SimOpts.TotBorn++;
            if (female.IsVegetable)
                Vegs.TotalVegs++;

            //Step4 after robot is created store the dna

            nuovo.Dna = outDna;
            nuovo.NumberOfGenes = DnaManipulations.CountGenes(nuovo.Dna);
            nuovo.MutationProbabilities = female.MutationProbabilities;

            nuovo.Mutations = female.Mutations;
            nuovo.OldMutations = female.OldMutations; //Botsareus 10/8/2015

            nuovo.LastMutation = 0;
            nuovo.LastMutationDetail.AddRange(female.LastMutationDetail);

            for (var t = 0; t < 12; t++)
            {
                nuovo.Skin[t] = female.Skin[t];
            }

            Array.Clear(nuovo.Memory, 0, nuovo.Memory.Length);
            nuovo.Ties.Clear();

            nuovo.Position += new DoubleVector(AbsX(female.Aim, (int)sondist, 0, 0, 0), AbsY(female.Aim, (int)sondist, 0, 0, 0));
            nuovo.BucketPosition = new IntVector(-2, -2);
            _bucketManager.UpdateBotBucket(nuovo);

            nuovo.Velocity = female.Velocity;
            nuovo.ActualVelocity = female.ActualVelocity; //Botsareus 7/1/2016 Bugfix
            nuovo.Color = female.Color;
            nuovo.Aim = Physics.NormaliseAngle(female.Aim + Math.PI);
            nuovo.Memory[MemoryAddresses.SetAim] = (int)(nuovo.Aim * 200);
            nuovo.Memory[MemoryAddresses.FIXANG] = 32000;
            nuovo.IsCorpse = false;
            nuovo.IsDead = false;
            nuovo.Generation++;
            if (nuovo.Generation > 32000)
                nuovo.Generation = 32000; //Botsareus 10/9/2015 Overflow protection

            nuovo.BirthCycle = SimOpt.SimOpts.TotRunCycle;

            var nnrg = female.Energy / 100 * per;
            var nbody = female.Body / 100 * per;
            var nwaste = female.Waste / 100 * per;
            var npwaste = female.PermanentWaste / 100 * per;
            var nchloroplasts = female.Chloroplasts / 100 * per; //Panda 8/23/2013 Distribute the chloroplasts

            female.Energy -= nnrg + nnrg * 0.001;
            female.Waste -= nwaste;
            female.PermanentWaste -= npwaste;
            female.Body -= nbody;
            female.Chloroplasts -= nchloroplasts; //Panda 8/23/2013 Distribute the chloroplasts

            nuovo.Chloroplasts = nchloroplasts; //Botsareus 8/24/2013 Distribute the chloroplasts
            nuovo.Body = nbody;
            nuovo.Waste = nwaste;
            nuovo.PermanentWaste = npwaste;
            female.Memory[MemoryAddresses.Energy] = (int)female.Energy;
            female.Memory[MemoryAddresses.body] = (int)female.Body;
            female.SonNumber++;

            if (female.SonNumber > 32000)
                female.SonNumber = 32000; // EricL Overflow protection.  Should change to Long at some point.

            nuovo.Energy = nnrg * 0.999; // Make reproduction cost 1% of nrg transfer for offspring
            nuovo.OldEnergy = nnrg * 0.999;
            nuovo.Memory[MemoryAddresses.Energy] = (int)nuovo.Energy;
            nuovo.IsPoisoned = false;
            nuovo.Parent = female;
            nuovo.FName = female.FName;
            nuovo.IsVegetable = female.IsVegetable;
            nuovo.ChloroplastsDisabled = female.ChloroplastsDisabled; //Botsareus 3/28/2014 Disable chloroplasts
            nuovo.IsFixed = female.IsFixed;
            nuovo.CantSee = female.CantSee;
            nuovo.DnaDisabled = female.DnaDisabled;
            nuovo.MovementSysvarsDisabled = female.MovementSysvarsDisabled;
            nuovo.CantReproduce = female.CantReproduce;
            nuovo.IsVirusImmune = female.IsVirusImmune;
            if (nuovo.IsFixed)
                nuovo.Memory[MemoryAddresses.Fixed] = 1;

            nuovo.SubSpecies = female.SubSpecies;

            nuovo.GenMut = female.GenMut;
            nuovo.Bouyancy = female.Bouyancy;

            if (female.MultibotTimer > 0)
                nuovo.MultibotTimer = female.MultibotTimer / 2 + 2;

            nuovo.VirusTimer = 0;
            nuovo.VirusShot = null;

            //First 5 genetic memory locations happen instantly
            for (var i = 0; i < 5; i++)
                nuovo.Memory[971 + i] = female.Memory[971 + i];

            //The other 15 genetic memory locations are stored now but can be used later
            for (var i = 0; i < 15; i++)
                nuovo.EpigeneticMemory[i] = female.Memory[976 + i];

            //Erase parents genetic memory now to prevent him from completing his own transfer by using his kid
            for (var i = 0; i < 14; i++)
                female.EpigeneticMemory[i] = 0;

            nuovo.LogMutation($"Female DNA len {female.Dna.Count} and male DNA len {female.SpermDna.Count} had offspring DNA len {nuovo.Dna.Count} during cycle {SimOpt.SimOpts.TotRunCycle}");

            NeoMutations.Mutate(this, nuovo, true);

            Senses.MakeOccurrList(nuovo);
            nuovo.NumberOfGenes = DnaManipulations.CountGenes(nuovo.Dna);
            nuovo.Memory[MemoryAddresses.DnaLenSys] = nuovo.Dna.Count;
            nuovo.Memory[MemoryAddresses.GenesSys] = nuovo.NumberOfGenes;

            Ties.MakeTie(female, nuovo, (int)sondist, 100, 0); //birth ties last 100 cycles
            female.OldEnergy = female.Energy; //saves mother from dying from shock after giving birth
            nuovo.Mass = nbody / 1000 + nuovo.Shell / 200;
            nuovo.Memory[MemoryAddresses.timersys] = female.Memory[MemoryAddresses.timersys]; //epigenetic timer

            female.Memory[MemoryAddresses.SEXREPRO] = 0; // sucessfully reproduced, so reset .sexrepro
            female.Fertilized = -1; // Set to -1 so spermDNA space gets reclaimed next cycle
            female.Memory[MemoryAddresses.SYSFERTILIZED] = 0; // Sperm is only good for one birth presently

            female.Energy -= female.Dna.Count * SimOpt.SimOpts.Costs.DnaCopyCost * SimOpt.SimOpts.Costs.CostMultiplier; //Botsareus 7/7/2013 Reproduction DNACOPY cost

            if (female.Energy < 0)
                female.Energy = 0;
        }

        private void Shoot(Robot rob)
        {
            var valmode = false;
            double energyLost;

            if (rob.Energy <= 0)
                return;

            var shtype = rob.Memory[MemoryAddresses.shoot];

            if (shtype == 0)
                return;

            double value = rob.Memory[MemoryAddresses.shootval];
            var multiplier = 0.0;
            var rngmultiplier = 0.0;
            var cost = 0.0;

            if (shtype is -1 or -6)
            {
                switch (value)
                {
                    case < 0:
                        // negative values of .shootval impact shot range?
                        multiplier = 1; // no impact on shot power
                        rngmultiplier = -value; // set the range multplier equal to .shootval
                        break;

                    case > 0:
                        // postive values of .shootval impact shot power?
                        multiplier = value;
                        rngmultiplier = 1;
                        valmode = true;
                        break;

                    case 0:
                        multiplier = 1;
                        rngmultiplier = 1;
                        break;
                }

                if (rngmultiplier > 4)
                {
                    cost = rngmultiplier * SimOpt.SimOpts.Costs.ShotFormationCost * SimOpt.SimOpts.Costs.CostMultiplier;
                    rngmultiplier = Math.Log(rngmultiplier / 2, 2);
                }
                else if (valmode == false)
                {
                    rngmultiplier = 1;
                    cost = SimOpt.SimOpts.Costs.ShotFormationCost * SimOpt.SimOpts.Costs.CostMultiplier / (rob.Ties.Count + 1);
                }

                if (multiplier > 4)
                {
                    cost = multiplier * SimOpt.SimOpts.Costs.ShotFormationCost * SimOpt.SimOpts.Costs.CostMultiplier;
                    multiplier = Math.Log(multiplier / 2, 2);
                }
                else if (valmode)
                {
                    multiplier = 1;
                    cost = SimOpt.SimOpts.Costs.ShotFormationCost * SimOpt.SimOpts.Costs.CostMultiplier / (rob.Ties.Count + 1);
                }

                if (cost > rob.Energy && cost > 2 && rob.Energy > 2 && valmode)
                {
                    cost = rob.Energy;
                    multiplier = Math.Log(rob.Energy / (SimOpt.SimOpts.Costs.ShotFormationCost * SimOpt.SimOpts.Costs.CostMultiplier), 2);
                }

                if (cost > rob.Energy && cost > 2 && rob.Energy > 2 && !valmode)
                {
                    cost = rob.Energy;
                    rngmultiplier = Math.Log(rob.Energy / (SimOpt.SimOpts.Costs.ShotFormationCost * SimOpt.SimOpts.Costs.CostMultiplier), 2);
                }
            }

            switch (shtype)
            {
                case > 0:
                    shtype %= MaxMem;
                    cost = SimOpt.SimOpts.Costs.ShotFormationCost * SimOpt.SimOpts.Costs.CostMultiplier;
                    if (rob.Energy < cost)
                        cost = rob.Energy;

                    rob.Energy -= cost; // EricL - postive shots should cost the shotcost
                    _shotManager.NewShot(rob, shtype, value, 1, true);

                    break;// Nrg request Feeding Shot
                case -1:
                    if (rob.IsMultibot)
                        value = 20 + rob.Body / 5 * rob.Ties.Count; //Botsareus 6/22/2016 Bugfix
                    else
                        value = 20 + rob.Body / 5;

                    value *= multiplier;
                    if (rob.Energy < cost)
                        cost = rob.Energy;

                    rob.Energy -= cost;
                    _shotManager.NewShot(rob, shtype, value, rngmultiplier, true);
                    break;// Nrg shot
                case -2:
                    value = Math.Abs(value);
                    if (rob.Energy < value)
                        value = rob.Energy;

                    if (value == 0)
                        value = rob.Energy / 100; //default energy shot.  Very small.

                    energyLost = value + SimOpt.SimOpts.Costs.ShotFormationCost * SimOpt.SimOpts.Costs.CostMultiplier / (rob.Ties.Count + 1);
                    if (energyLost > rob.Energy)
                        rob.Energy = 0;
                    else
                        rob.Energy -= energyLost;

                    _shotManager.NewShot(rob, shtype, value, 1, true);
                    break;//shoot venom
                case -3:
                    value = Math.Abs(value);
                    if (value > rob.Venom)
                        value = rob.Venom;

                    if (value == 0)
                        value = rob.Venom / 20; //default venom shot.  Not too small.

                    rob.Venom -= value;
                    rob.Memory[825] = (int)rob.Venom;
                    energyLost = SimOpt.SimOpts.Costs.ShotFormationCost * SimOpt.SimOpts.Costs.CostMultiplier / (rob.Ties.Count + 1);

                    rob.Energy = energyLost > rob.Energy ? 0 : rob.Energy - energyLost;

                    _shotManager.NewShot(rob, shtype, value, 1, true);
                    break;//shoot waste 'Botsareus 4/22/2016 Bugfix
                case -4:
                    value = Math.Abs(value);
                    if (value == 0)
                        value = rob.Waste / 20; //default waste shot. 'Botsareus 10/5/2015 Fix for waste

                    if (value > rob.Waste)
                        value = rob.Waste;

                    rob.Waste -= value * 0.99; //Botsareus 10/5/2015 Fix for waste
                    rob.PermanentWaste += value / 100;
                    energyLost = SimOpt.SimOpts.Costs.ShotFormationCost * SimOpt.SimOpts.Costs.CostMultiplier / (rob.Ties.Count + 1);
                    rob.Energy = energyLost > rob.Energy ? 0 : rob.Energy - energyLost;

                    _shotManager.NewShot(rob, shtype, value, 1, true);
                    // no -5 shot here as poison can only be shot in response to an attack
                    break;//shoot body
                case -6:
                    if (rob.IsMultibot)
                        value = 10 + rob.Body / 2 * (rob.Ties.Count + 1);
                    else
                        value = 10 + Math.Abs(rob.Body) / 2;

                    if (rob.Energy < cost)
                        cost = rob.Energy;

                    rob.Energy -= cost;
                    value *= multiplier;
                    _shotManager.NewShot(rob, shtype, value, rngmultiplier, true);
                    break;// shoot sperm
                case -8:
                    cost = SimOpt.SimOpts.Costs.ShotFormationCost * SimOpt.SimOpts.Costs.CostMultiplier;

                    if (rob.Energy < cost)
                        cost = rob.Energy;

                    rob.Energy -= cost; // EricL - postive shots should cost the shotcost
                    _shotManager.NewShot(rob, shtype, value, 1, true);
                    break;
            }
            rob.Memory[MemoryAddresses.shoot] = 0;
            rob.Memory[MemoryAddresses.shootval] = 0;
        }

        private bool SimpleCollision(int x, int y, Robot rob)
        {
            if (Robots.Any(r => r.Exists && r != rob && Math.Abs(r.Position.X - x) < r.GetRadius(SimOpt.SimOpts.FixedBotRadii) + rob.GetRadius(SimOpt.SimOpts.FixedBotRadii) && Math.Abs(r.Position.Y - y) < r.GetRadius(SimOpt.SimOpts.FixedBotRadii) + rob.GetRadius(SimOpt.SimOpts.FixedBotRadii)))
                return true;

            if (_obstacleManager.Obstacles.Any(o => o.Position.X <= Math.Max(rob.Position.X, x) && o.Position.X + o.Width >= Math.Min(rob.Position.X, x) && o.Position.Y <= Math.Max(rob.Position.Y, y) && o.Position.Y + o.Height >= Math.Min(rob.Position.Y, y)))
                return true;

            if (SimOpt.SimOpts.DxSxConnected == false && (x < rob.GetRadius(SimOpt.SimOpts.FixedBotRadii) + Physics.SmudgeFactor || x + rob.GetRadius(SimOpt.SimOpts.FixedBotRadii) + Physics.SmudgeFactor > SimOpt.SimOpts.FieldWidth))
                return true;

            if (SimOpt.SimOpts.UpDnConnected == false && (y < rob.GetRadius(SimOpt.SimOpts.FixedBotRadii) + Physics.SmudgeFactor || y + rob.GetRadius(SimOpt.SimOpts.FixedBotRadii) + Physics.SmudgeFactor > SimOpt.SimOpts.FieldHeight))
                return true;

            return false;
        }

        private void SimpleMatch(List<block3> r1, List<block3> r2)
        {
            var ei1 = r1.Count;
            var ei2 = r2.Count;
            var matchlist1 = new List<int>();
            var matchlist2 = new List<int>();
            var loopr1 = 0;
            var loopr2 = 0;
            var laststartmatch1 = 0;
            var laststartmatch2 = 0;
            var patch = 0;//Botsareus 4/18/2016 Temporary fix to prevent infinate loop

            do
            {
                //keep building until both sides max out
                if (loopr1 >= ei1)
                    loopr1 = ei1 - 1;

                if (loopr2 >= ei2)
                    loopr2 = ei2 - 1;

                matchlist1.Add(r1[loopr1].nucli);
                matchlist2.Add(r2[loopr2].nucli);

                //does anything match
                var matchr2 = false;

                var match = false;

                int loopold;

                for (loopold = 0; loopold < matchlist1.Count; loopold++)
                {
                    if (r2[loopr2].nucli == matchlist1[loopold])
                    {
                        matchr2 = true;
                        match = true;
                        break;
                    }
                    if (r1[loopr1].nucli == matchlist2[loopold])
                    {
                        match = true;
                        break;
                    }
                    patch++;
                }

                if (match)
                {
                    if (matchr2)
                    {
                        loopr1 = loopold + laststartmatch1;
                    }
                    else
                    {
                        loopr2 = loopold + laststartmatch2;
                    }

                    //start matching

                    var inc = 0;
                    var newmatch = false;

                    do
                    {
                        if (r2[loopr2].nucli == r1[loopr1].nucli)
                        {
                            if (newmatch == false)
                                inc++;

                            newmatch = true;
                            r1[loopr1].match = inc;
                            r2[loopr2].match = inc;
                        }
                        else
                        {
                            laststartmatch1 = loopr1;
                            laststartmatch2 = loopr2;
                            loopr1--;
                            loopr2--;
                            break;
                        }
                        loopr1++;
                        loopr2++;
                        patch++;
                    } while (loopr1 <= ei1 && loopr2 <= ei2);
                }

                loopr1++;
                loopr2++;
                patch++;
            } while (loopr1 <= ei1 || loopr2 <= ei2 || patch > (16000 ^ 2));
        }

        private async Task UpdateCounters(Robot rob)
        {
            var species = SimOpt.SimOpts.Specie.FirstOrDefault(s => s.Name == rob.FName);

            //If no species structure for the bot, then create one
            if (!rob.IsCorpse)
            {
                if (species == null)
                    MainEngine.AddSpecie(rob, false);
                else
                {
                    species.Population++;
                    species.Population = Math.Min(species.Population, 32000);
                }
            }

            if (rob.IsVegetable)
                Vegs.TotalVegs++;
            else if (rob.IsCorpse)
            {
                if (rob.Body > 0)
                    _shotManager.Decay(rob);
                else
                    await KillRobot(rob);
            }
        }
    }
}
