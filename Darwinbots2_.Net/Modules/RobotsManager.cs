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

        void ShareChloroplasts(Robot robot, Tie tie);

        void ShareEnergy(Robot robot, Tie tie);

        void ShareShell(Robot rob, Tie tie);

        void ShareSlime(Robot rob, Tie tie);

        void ShareWaste(Robot rob, Tie tie);
    }

    internal class RobotsManager : IRobotManager
    {
        public const int CubicTwipPerBody = 905;
        public const int GeneticSensitivity = 75;
        public const int MaxMem = 1000;
        public const int RobSize = 120;
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
        public int TotalNotVegs { get; set; }
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
            var robot = new Robot();

            Robots.Add(robot);

            return robot;
        }

        public async Task KillRobot(Robot robot)
        {
            if (SimOpt.SimOpts.DeadRobotSnp && (!robot.IsVegetable || !SimOpt.SimOpts.SnpExcludeVegs))
                await Database.AddRecord(robot);

            robot.IsFixed = false;
            robot.IsVegetable = false;
            robot.SonNumber = 0;
            robot.Age = 0;
            Ties.DeleteAllTies(robot);
            robot.Exists = false;
            _bucketManager.UpdateBotBucket(robot);

            if (robot.VirusShot != null)
            {
                robot.VirusShot.Exist = false;
                _shotManager.Shots.Remove(robot.VirusShot);
                robot.VirusShot = null;
            }

            robot.SpermDna.Clear();

            robot.LastMutationDetail = "";

            Robots.Remove(robot);
        }

        public void ShareChloroplasts(Robot robot, Tie tie)
        {
            ShareResource(robot, tie, MemoryAddresses.sharechlr, r => r.Chloroplasts, (r, s) => r.Chloroplasts = s);
        }

        public void ShareEnergy(Robot robot, Tie tie)
        {
            //This is an order of operation thing.  A bot earlier in the rob array might have taken all your nrg, taking your
            //nrg to 0.  You should still be able to take some back.
            if (robot.Energy < 0 || tie.OtherBot.Energy < 0)
                return;

            //.mem(830) is the percentage of the total nrg this bot wants to receive
            //has to be positive to come here, so no worries about changing the .mem location here
            if (robot.Memory[830] <= 0)
                robot.Memory[830] = 0;
            else
            {
                robot.Memory[830] = robot.Memory[830] % 100;
                if (robot.Memory[830] == 0)
                    robot.Memory[830] = 100;
            }

            //Total nrg of both bots combined
            var totnrg = robot.Energy + tie.OtherBot.Energy;

            var portionThatsMine = totnrg * ((double)robot.Memory[830] / 100); // This is what the bot wants to have of the total
            if (portionThatsMine > 32000)
                portionThatsMine = 32000; // Can't want more than the max a bot can have

            var myChangeInNrg = portionThatsMine - robot.Energy; // This is what the bot's change in nrg would be

            //If the bot is taking nrg, then he can't take more than that represented by his own body.  If giving nrg away, same thing.  The bot
            //can't give away more than that represented by his body.  Should make it so that larger bots win tie feeding battles.
            if (Math.Abs(myChangeInNrg) > robot.Body)
                myChangeInNrg = Math.Sign(myChangeInNrg) * robot.Body;

            if (robot.Energy + myChangeInNrg > 32000)
                myChangeInNrg = 32000 - robot.Energy; //Limit change if it would put bot over the limit

            if (robot.Energy + myChangeInNrg < 0)
                myChangeInNrg = -robot.Energy; // Limit change if it would take the bot below 0

            //Now we have to check the limits on the other bot
            //sign is negative since the negative of myChangeinNrg is what the other bot is going to get/recevie
            if (tie.OtherBot.Energy - myChangeInNrg > 32000)
                myChangeInNrg = -(32000 - tie.OtherBot.Energy); //Limit change if it would put bot over the limit

            if (tie.OtherBot.Energy - myChangeInNrg < 0)
                myChangeInNrg = tie.OtherBot.Energy; // limit change if it would take the bot below 0

            //Do the actual nrg exchange
            robot.Energy += myChangeInNrg;
            tie.OtherBot.Energy -= myChangeInNrg;

            //Transferring nrg costs nrg.  1% of the transfer gets deducted from the bot iniating the transfer
            robot.Energy -= Math.Abs(myChangeInNrg) * 0.01;

            //Bots with 32000 nrg can still take or receive nrg, but everything over 32000 disappears
            if (robot.Energy > 32000)
                robot.Energy = 32000;

            if (tie.OtherBot.Energy > 32000)
                tie.OtherBot.Energy = 32000;
        }

        public void ShareShell(Robot rob, Tie tie)
        {
            ShareResource(rob, tie, 832, r => r.Shell, (r, s) => r.Shell = s);
        }

        public void ShareSlime(Robot rob, Tie tie)
        {
            ShareResource(rob, tie, 833, r => r.Slime, (r, s) => r.Slime = s);
        }

        public void ShareWaste(Robot rob, Tie tie)
        {
            ShareResource(rob, tie, 831, r => r.Waste, (r, s) => r.Waste = s);
        }

        public async Task UpdateBots()
        {
            BotsToKill.Clear();
            BotsToReproduce.Clear();
            BotsToReproduceSexually.Clear();
            TotalNotVegs = 0;
            Vegs.TotalVegsDisplayed = Vegs.TotalVegs;
            Vegs.TotalVegs = 0;

            // Only calculate mass due to fluid displacement if the sim medium has density.
            if (SimOpt.SimOpts.Density != 0)
            {
                foreach (var rob in Robots.Where(r => r.Exists))
                    Physics.AddedMass(rob);
            }

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
                if (!rob.IsCorpse)
                    Upkeep(rob); // No upkeep costs if you are dead!

                if (!rob.IsCorpse && !rob.DnaDisabled)
                    Poisons(rob);

                if (!SimOpt.SimOpts.DisableFixing)
                    ManageFixed(rob);

                Physics.CalcMass(rob);

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

                Ties.TiePortCommunication(rob); //transfer data through ties
                Ties.ReadTie(rob); //reads all of the tref variables from a given tie number
            }

            foreach (var s in SimOpt.SimOpts.Specie)
                s.Population = 0;

            foreach (var rob in Robots.Where(r => r.Exists))
                await UpdateCounters(rob); // Counts the number of bots and decays body...

            foreach (var rob in Robots.Where(r => r.Exists))
            {
                Ties.UpdateTies(this, rob); // Carries all tie routines

                //EricL Transfer genetic meomory locations for newborns through the birth tie during their first 15 cycles
                if (rob.Age < 15)
                    DoGeneticMemory(rob);

                if (!rob.IsCorpse && !rob.DnaDisabled)
                {
                    SetAimFunc(rob); //Setup aiming
                    BotDnaManipulation(rob);
                }

                UpdatePosition(rob); //updates robot's position
                                     //EricL 4/9/2006 Got rid of a loop below by moving these inside this loop.  Should speed things up a little.

                rob.Energy = Math.Clamp(rob.Energy, -32000, 32000);
                rob.Poison = Math.Clamp(rob.Poison, -32000, 32000);
                rob.Venom = Math.Clamp(rob.Venom, -32000, 32000);
                rob.Waste = Math.Clamp(rob.Waste, -32000, 32000);
            }

            foreach (var rob in Robots)
            {
                Ties.UpdateTieAngles(rob); // Updates .tielen and .tieang.  Have to do this here after all bot movement happens above.

                if (!rob.IsCorpse && !rob.DnaDisabled && rob.Exists)
                {
                    NeoMutations.Mutate(this, rob);
                    MakeStuff(rob);
                    HandleWaste(rob);
                    Shooting(rob);

                    if (!rob.ChloroplastsDisabled)
                        ManageChlr(rob);

                    ManageBody(rob);
                    ManageBouyancy(rob);
                    ManageReproduction(rob);
                    Shock(rob);
                    Senses.WriteSenses(this, _bucketManager, rob);
                    FireTies(rob);
                }
                if (!rob.IsCorpse && rob.Exists)
                {
                    Ageing(rob); // Even bots with disabled DNA age...
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

        private void Ageing(Robot rob)
        {
            //aging
            rob.Age++;
            rob.NewAge++; //Age this simulation to be used by tie code
            var tempAge = rob.Age;
            if (tempAge > 32000)
                tempAge = 32000;

            rob.Memory[MemoryAddresses.robage] = tempAge; //line added to copy robots age into a memory location
            rob.Memory[MemoryAddresses.timersys] += 1; //update epigenetic timer

            if (rob.Memory[MemoryAddresses.timersys] > 32000)
                rob.Memory[MemoryAddresses.timersys] = -32000;
        }

        private void Altzheimer(Robot rob)
        {
            // Makes robots with high waste act in a bizarre fashion.
            var loops = (rob.PermanentWaste + rob.Waste - SimOpt.SimOpts.BadWasteLevel) / 4;

            for (var t = 0; t < loops; t++)
            {
                int loc;
                do
                {
                    loc = ThreadSafeRandom.Local.Next(1, 1000);
                } while (!(loc != MemoryAddresses.mkchlr && loc != MemoryAddresses.rmchlr));
                var val = ThreadSafeRandom.Local.Next(-32000, 32000);
                rob.Memory[loc] = val;
            }
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

        private void ChangeChlr(Robot rob)
        {
            var tmpchlr = rob.Chloroplasts;

            // add chloroplasts
            rob.Chloroplasts += rob.Memory[MemoryAddresses.mkchlr];

            // remove chloroplasts
            rob.Chloroplasts -= rob.Memory[MemoryAddresses.rmchlr];

            if (tmpchlr < rob.Chloroplasts)
            {
                var newnrg = rob.Energy - (rob.Chloroplasts - tmpchlr) * SimOpt.SimOpts.Costs.CholorplastCost * SimOpt.SimOpts.Costs.CostMultiplier;

                if (TotalChlr > SimOpt.SimOpts.MaxPopulation && rob.IsVegetable || newnrg < 100)
                    rob.Chloroplasts = tmpchlr;
                else
                    rob.Energy = newnrg; //Botsareus 8/24/2013 only charge energy for adding chloroplasts to prevent robots from cheating by adding and subtracting there chlroplasts in 3 cycles
            }

            rob.Memory[MemoryAddresses.mkchlr] = 0;
            rob.Memory[MemoryAddresses.rmchlr] = 0;
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

        private void DoGeneticMemory(Robot rob)
        {
            if (rob.Ties.Count <= 0 || rob.Ties[0].Last <= 0)
                return;

            var loc = 976 + rob.Age;
            if (rob.Memory[loc] == 0 & rob.EpigeneticMemory[rob.Age] != 0)
            {
                rob.Memory[loc] = rob.EpigeneticMemory[rob.Age];
            }
        }

        private void FeedBody(Robot rob)
        {
            if (rob.Memory[MemoryAddresses.fdbody] > 100)
                rob.Memory[MemoryAddresses.fdbody] = 100;

            rob.Energy += rob.Memory[MemoryAddresses.fdbody];
            rob.Body -= (double)rob.Memory[MemoryAddresses.fdbody] / 10;

            if (rob.Energy > 32000)
                rob.Energy = 32000;

            rob.Memory[MemoryAddresses.fdbody] = 0;
        }

        private void FireTies(Robot rob)
        {
            var resetlastopp = false;

            if (rob.LastSeenObject == null & rob.Age < 2 && rob.Parent is { Exists: true })
            {
                rob.LastSeenObject = rob.Parent;
                resetlastopp = true;
            }

            if (rob.LastSeenObject == null & rob.LastTouched != null && rob.LastTouched.Exists)
            {
                rob.LastSeenObject = rob.LastTouched;
                resetlastopp = true;
            }

            if (rob.Memory[MemoryAddresses.mtie] != 0 && rob.LastSeenObject != null & !SimOpt.SimOpts.DisableTies && rob.LastSeenObject is Robot lastOpp)
            {
                var length = (lastOpp.Position - rob.Position).Magnitude();
                var maxLength = RobSize * 4 + rob.Radius + lastOpp.Radius;

                if (length <= maxLength)
                    Ties.MakeTie(rob, lastOpp, (int)(rob.Radius + lastOpp.Radius + RobSize * 2), -20, rob.Memory[MemoryAddresses.mtie]);

                rob.Memory[MemoryAddresses.mtie] = 0;
            }

            if (resetlastopp)
                rob.LastSeenObject = null;
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

        private void HandleWaste(Robot rob)
        {
            if (rob.Waste > 0 && rob.Chloroplasts > 0)
                Vegs.feedveg2(rob);

            if (SimOpt.SimOpts.BadWasteLevel == 0)
                SimOpt.SimOpts.BadWasteLevel = 400;

            if (SimOpt.SimOpts.BadWasteLevel > 0 & rob.PermanentWaste + rob.Waste > SimOpt.SimOpts.BadWasteLevel)
                Altzheimer(rob);

            if (rob.Waste > 32000)
                _shotManager.Defecate(rob);

            if (rob.PermanentWaste > 32000)
                rob.PermanentWaste = 32000;

            if (rob.Waste < 0)
                rob.Waste = 0;

            rob.Memory[828] = (int)rob.Waste;
            rob.Memory[829] = (int)rob.PermanentWaste;
        }

        private void MakeShell(Robot rob)
        {
            const double shellNrgConvRate = 0.1;

            StoreResource(rob, 822, 823, shellNrgConvRate, SimOpt.SimOpts.Costs.ShellCost, r => r.Shell, (r, s) => r.Shell = s, true);
        }

        private void MakeSlime(Robot rob)
        {
            const double slimeNrgConvRate = 0.1;

            StoreResource(rob, 820, 821, slimeNrgConvRate, SimOpt.SimOpts.Costs.SlimeCost, r => r.Slime, (r, s) => r.Slime = s, true);
        }

        private void MakeStuff(Robot rob)
        {
            if (rob.Memory[824] != 0)
                StoreVenom(rob);

            if (rob.Memory[826] != 0)
                StorePoison(rob);

            if (rob.Memory[822] != 0)
                MakeShell(rob);

            if (rob.Memory[820] != 0)
                MakeSlime(rob);
        }

        private void ManageBody(Robot rob)
        {
            if (rob.Memory[MemoryAddresses.strbody] > 0)
                StoreBody(rob);

            if (rob.Memory[MemoryAddresses.fdbody] > 0)
                FeedBody(rob);

            rob.Body = Math.Clamp(rob.Body, 0, 32000);

            rob.Memory[MemoryAddresses.body] = (int)rob.Body;
        }

        private void ManageBouyancy(Robot rob)
        {
            if (rob.Memory[MemoryAddresses.setboy] == 0)
                return;

            rob.Bouyancy += (double)rob.Memory[MemoryAddresses.setboy] / 32000;
            rob.Bouyancy = Math.Clamp(rob.Bouyancy, 0, 1);

            rob.Memory[MemoryAddresses.rdboy] = (int)(rob.Bouyancy * 32000);
            rob.Memory[MemoryAddresses.setboy] = 0;
        }

        private void ManageChlr(Robot rob)
        {
            if (rob.Memory[MemoryAddresses.mkchlr] > 0 || rob.Memory[MemoryAddresses.rmchlr] > 0)
                ChangeChlr(rob);

            rob.Chloroplasts -= 0.5 / Math.Pow(100, rob.Chloroplasts / 16000);

            rob.Chloroplasts = Math.Clamp(rob.Chloroplasts, 0, 32000);

            rob.Memory[MemoryAddresses.chlr] = (int)rob.Chloroplasts;
            rob.Memory[MemoryAddresses.light] = (int)(32000 - Vegs.LightAval * 32000);
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

        private void ManageFixed(Robot rob)
        {
            rob.IsFixed = rob.Memory[216] > 0;
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

        private void Poisons(Robot rob)
        {
            if (rob.IsParalyzed)
                rob.Memory[rob.VirusLocation] = rob.VirusValue;

            if (rob.IsParalyzed)
            {
                rob.ParalyzedCountdown--;

                if (rob.ParalyzedCountdown < 1)
                {
                    rob.IsParalyzed = false;
                    rob.VirusLocation = 0;
                    rob.VirusValue = 0;
                }
            }

            rob.Memory[837] = rob.ParalyzedCountdown;

            if (rob.IsPoisoned)
                rob.Memory[rob.PoisonLocation] = rob.PoisonValue;

            if (rob.IsPoisoned)
            {
                rob.PoisonCountdown--;
                if (rob.PoisonCountdown < 1)
                {
                    rob.IsPoisoned = false;
                    rob.PoisonLocation = 0;
                    rob.PoisonValue = 0;
                }
            }

            rob.Memory[838] = (int)rob.PoisonCountdown;
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

            var sondist = (int)robot.Radius;

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
            nuovo.LastMutationDetail = robot.LastMutationDetail;

            for (var t = 0; t < 12; t++)
                nuovo.Skin[t] = robot.Skin[t];

            Array.Clear(nuovo.Memory, 0, nuovo.Memory.Length);
            nuovo.Ties.Clear();

            nuovo.Position = new DoubleVector(robot.Position.X + AbsX(robot.Aim, sondist, 0, 0, 0), robot.Position.Y + AbsY(robot.Aim, sondist, 0, 0, 0));
            nuovo.Exists = true;
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

        private void SetAimFunc(Robot rob)
        {
            double aim;
            double diff2 = 0;
            double diff = rob.Memory[MemoryAddresses.aimsx] - rob.Memory[MemoryAddresses.aimdx];

            if (rob.Memory[MemoryAddresses.SetAim] == Physics.RadiansToInt(rob.Aim))
            {
                aim = rob.Aim * 200 + diff;
            }
            else
            {
                // .setaim overrides .aimsx and .aimdx
                aim = rob.Memory[MemoryAddresses.SetAim]; // this is where .aim needs to be
                diff = -Physics.AngDiff(rob.Aim, Physics.NormaliseAngle((double)rob.Memory[MemoryAddresses.SetAim] / 200)) * 200; // this is the diff to get there 'Botsareus 6/18/2016 Added angnorm
                diff2 = Math.Abs(Math.Round((rob.Aim * 200 - rob.Memory[MemoryAddresses.SetAim]) / 1256, 0) * 1256) * Math.Sign(diff); // this is how much we add to momentum
            }

            //diff + diff2 is now the amount, positive or negative to turn.
            rob.Energy -= Math.Abs(Math.Round((diff + diff2) / 200, 3) * SimOpt.SimOpts.Costs.RotationCost * SimOpt.SimOpts.Costs.CostMultiplier);

            aim %= 1256;
            if (aim < 0)
                aim += 1256;

            aim /= 200;

            //Overflow Protection
            while (rob.AngularMomentum > 2 * Math.PI)
                rob.AngularMomentum -= 2 * Math.PI;

            while (rob.AngularMomentum < -2 * Math.PI)
                rob.AngularMomentum += 2 * Math.PI;

            rob.Aim = aim + rob.AngularMomentum; // Add in the angular momentum

            //Voluntary rotation can reduce angular momentum but does not add to it.

            if (rob.AngularMomentum > 0 & diff < 0)
            {
                rob.AngularMomentum += (diff + diff2) / 200;
                if (rob.AngularMomentum < 0)
                    rob.AngularMomentum = 0;
            }
            if (rob.AngularMomentum < 0 & diff > 0)
            {
                rob.AngularMomentum += (diff + diff2) / 200;
                if (rob.AngularMomentum > 0)
                    rob.AngularMomentum = 0;
            }

            rob.Memory[MemoryAddresses.aimsx] = 0;
            rob.Memory[MemoryAddresses.aimdx] = 0;
            rob.Memory[MemoryAddresses.AimSys] = (int)(rob.Aim * 200);
            rob.Memory[MemoryAddresses.SetAim] = rob.Memory[MemoryAddresses.AimSys];
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

            var sondist = female.Radius;

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
            nuovo.LastMutationDetail = female.LastMutationDetail;

            for (var t = 0; t < 12; t++)
            {
                nuovo.Skin[t] = female.Skin[t];
            }

            Array.Clear(nuovo.Memory, 0, nuovo.Memory.Length);
            nuovo.Ties.Clear();

            nuovo.Position += new DoubleVector(AbsX(female.Aim, (int)sondist, 0, 0, 0), AbsY(female.Aim, (int)sondist, 0, 0, 0));
            nuovo.Exists = true;
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

            NeoMutations.LogMutation(this, nuovo, $"Female DNA len {female.Dna.Count} and male DNA len {female.SpermDna.Count} had offspring DNA len {nuovo.Dna.Count} during cycle {SimOpt.SimOpts.TotRunCycle}");

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

        private void ShareResource(Robot rob, Tie tie, int address, Func<Robot, double> getValue, Action<Robot, double> setValue)
        {
            if (rob.Memory[address] > 99)
                rob.Memory[address] = 99;
            if (rob.Memory[address] < 0)
                rob.Memory[address] = 0;

            var total = getValue(rob) + getValue(tie.OtherBot);

            setValue(tie.OtherBot, Math.Min(total * ((100 - (double)rob.Memory[address]) / 100), 32000));
            setValue(rob, Math.Min(total * ((double)rob.Memory[address] / 100), 32000));

            rob.Memory[address] = (int)getValue(rob); // update the .shell sysvar
            tie.OtherBot.Memory[address] = (int)getValue(tie.OtherBot);
        }

        private void Shock(Robot rob)
        {
            if (rob.IsVegetable || rob.Energy <= 3000)
                return;

            var temp = rob.OldEnergy - rob.Energy;

            if (!(temp > rob.OldEnergy / 2))
                return;

            rob.Energy = 0;
            rob.Body += rob.Energy / 10;

            if (rob.Body > 32000)
                rob.Body = 32000;
        }

        private void Shoot(Robot rob)
        {
            var valmode = false;
            double energyLost;

            if (rob.Energy <= 0)
                return;

            var shtype = rob.Memory[MemoryAddresses.shoot];
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

        private void Shooting(Robot rob)
        {
            if (rob.Memory[MemoryAddresses.shoot] != 0)
                Shoot(rob);

            rob.Memory[MemoryAddresses.shoot] = 0;
        }

        private bool SimpleCollision(int x, int y, Robot rob)
        {
            if (Robots.Any(r => r.Exists && r != rob && Math.Abs(r.Position.X - x) < r.Radius + rob.Radius && Math.Abs(r.Position.Y - y) < r.Radius + rob.Radius))
                return true;

            if (_obstacleManager.Obstacles.Any(o => o.Position.X <= Math.Max(rob.Position.X, x) && o.Position.X + o.Width >= Math.Min(rob.Position.X, x) && o.Position.Y <= Math.Max(rob.Position.Y, y) && o.Position.Y + o.Height >= Math.Min(rob.Position.Y, y)))
                return true;

            if (SimOpt.SimOpts.DxSxConnected == false && (x < rob.Radius + Physics.SmudgeFactor || x + rob.Radius + Physics.SmudgeFactor > SimOpt.SimOpts.FieldWidth))
                return true;

            if (SimOpt.SimOpts.UpDnConnected == false && (y < rob.Radius + Physics.SmudgeFactor || y + rob.Radius + Physics.SmudgeFactor > SimOpt.SimOpts.FieldHeight))
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

        private void StoreBody(Robot rob)
        {
            if (rob.Memory[MemoryAddresses.strbody] > 100)
                rob.Memory[MemoryAddresses.strbody] = 100;

            rob.Energy -= rob.Memory[MemoryAddresses.strbody];
            rob.Body += rob.Memory[MemoryAddresses.strbody] / 10.0;

            if (rob.Body > 32000)
                rob.Body = 32000;

            rob.Memory[MemoryAddresses.strbody] = 0;
        }

        private void StorePoison(Robot rob)
        {
            const double poisonNrgConvRate = 0.25; // Make 4 poison for 1 nrg

            StoreResource(rob, 826, 827, poisonNrgConvRate, SimOpt.SimOpts.Costs.PoisonCost, r => r.Poison, (r, s) => r.Poison = s);
        }

        private void StoreResource(Robot rob, int storeAddress, int levelAddress, double conversionRate, double resourceCost, Func<Robot, double> getValue, Action<Robot, double> setValue, bool multiBotDiscount = false)
        {
            if (rob.Energy <= 0)
                return;

            double delta = Math.Clamp(rob.Memory[storeAddress], -32000, 32000);
            delta = Math.Clamp(delta, -rob.Energy / conversionRate, rob.Energy / conversionRate);
            delta = Math.Clamp(delta, -100, 100);

            if (getValue(rob) + delta > 32000)
                delta = 32000 - getValue(rob);

            if (getValue(rob) + delta < 0)
                delta = -getValue(rob);

            setValue(rob, getValue(rob) + delta);

            if (rob.IsMultibot && multiBotDiscount)
                rob.Energy -= Math.Abs(delta) * conversionRate / (rob.Ties.Count + 1);
            else
                rob.Energy -= Math.Abs(delta) * conversionRate;

            //This is the transaction cost
            var cost = Math.Abs(delta) * resourceCost * SimOpt.SimOpts.Costs.CostMultiplier;

            rob.Energy -= cost;
            rob.Waste += cost;

            rob.Memory[storeAddress] = 0;
            rob.Memory[levelAddress] = (int)getValue(rob);
        }

        private void StoreVenom(Robot rob)
        {
            const double venomNrgConvRate = 1.0; // Make 1 venom for 1 nrg

            StoreResource(rob, 824, 825, venomNrgConvRate, SimOpt.SimOpts.Costs.VenomCost, r => r.Venom, (r, s) => r.Venom = s);
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
            else
                TotalNotVegs++;
        }

        private void UpdatePosition(Robot rob)
        {
            //Following line commented since mass is set earlier in CalcMass
            if (rob.Mass + rob.AddedMass < 0.25)
                rob.Mass = 0.25 - rob.AddedMass; // a fudge since Euler approximation can't handle it when mass -> 0

            double vt = 0;

            if (!rob.IsFixed)
            {
                // speed normalization
                rob.Velocity += rob.IndependentImpulse * (1 / (rob.Mass + rob.AddedMass));

                vt = rob.Velocity.MagnitudeSquare();
                if (vt > SimOpt.SimOpts.MaxVelocity * SimOpt.SimOpts.MaxVelocity)
                {
                    rob.Velocity = rob.Velocity.Unit() * SimOpt.SimOpts.MaxVelocity;
                }

                rob.Position += rob.Velocity;
                _bucketManager.UpdateBotBucket(rob);
            }
            else
                rob.Velocity = new DoubleVector(0, 0);

            //Have to do these here for both fixed and unfixed bots to avoid build up of forces in case fixed bots become unfixed.
            rob.IndependentImpulse = new DoubleVector(0, 0);
            rob.ResistiveImpulse = rob.IndependentImpulse;
            rob.StaticImpulse = 0;

            if (SimOpt.SimOpts.ZeroMomentum)
                rob.Velocity = new DoubleVector(0, 0);

            rob.Memory[MemoryAddresses.dirup] = 0;
            rob.Memory[MemoryAddresses.dirdn] = 0;
            rob.Memory[MemoryAddresses.dirdx] = 0;
            rob.Memory[MemoryAddresses.dirsx] = 0;

            rob.Memory[MemoryAddresses.velscalar] = (int)Math.Clamp(Math.Sqrt(vt), -32000, 32000);
            rob.Memory[MemoryAddresses.vel] = (int)Math.Clamp(Math.Cos(rob.Aim) * rob.Velocity.X + Math.Sin(rob.Aim) * rob.Velocity.Y * -1, -32000, 32000);
            rob.Memory[MemoryAddresses.veldn] = rob.Memory[MemoryAddresses.vel] * -1;
            rob.Memory[MemoryAddresses.veldx] = (int)Math.Clamp(Math.Sin(rob.Aim) * rob.Velocity.X + Math.Cos(rob.Aim) * rob.Velocity.Y, -32000, 32000);
            rob.Memory[MemoryAddresses.velsx] = rob.Memory[MemoryAddresses.veldx] * -1;

            rob.Memory[MemoryAddresses.masssys] = (int)rob.Mass;
            rob.Memory[MemoryAddresses.maxvelsys] = (int)SimOpt.SimOpts.MaxVelocity;
        }

        private void Upkeep(Robot rob)
        {
            double cost;

            //Age Cost
            var ageDelta = rob.Age - SimOpt.SimOpts.Costs.AgeCostBeginAge;

            if (ageDelta > 0 & rob.Age > 0)
            {
                if (SimOpt.SimOpts.Costs.EnableAgeCostIncreaseLog)
                    cost = SimOpt.SimOpts.Costs.AgeCost * Math.Log(ageDelta);
                else if (SimOpt.SimOpts.Costs.EnableAgeCostIncreasePerCycle)
                    cost = SimOpt.SimOpts.Costs.AgeCost + ageDelta * SimOpt.SimOpts.Costs.AgeCostIncreasePerCycle;
                else
                    cost = SimOpt.SimOpts.Costs.AgeCost;

                rob.Energy -= cost * SimOpt.SimOpts.Costs.CostMultiplier;
            }

            //BODY UPKEEP
            cost = rob.Body * SimOpt.SimOpts.Costs.BodyUpkeepCost * SimOpt.SimOpts.Costs.CostMultiplier;
            rob.Energy -= cost;

            //DNA upkeep cost
            cost = (rob.Dna.Count - 1) * SimOpt.SimOpts.Costs.DnaUpkeepCost * SimOpt.SimOpts.Costs.CostMultiplier;
            rob.Energy -= cost;

            //degrade slime
            rob.Slime *= 0.98;
            if (rob.Slime < 0.5)
                rob.Slime = 0; // To keep things sane for integer rounding, etc.

            rob.Memory[821] = (int)rob.Slime;

            //degrade poison
            rob.Poison *= 0.98;
            if (rob.Poison < 0.5)
                rob.Poison = 0;

            rob.Memory[827] = (int)rob.Poison;
        }
    }
}
