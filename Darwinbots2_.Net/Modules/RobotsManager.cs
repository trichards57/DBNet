using DarwinBots.Model;
using DarwinBots.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DarwinBots.Modules
{
    internal class RobotsManager
    {
        public const int CubicTwipPerBody = 905;
        public const int GeneticSensitivity = 75;
        public const int MaxMem = 1000;
        public const int RobSize = 120;
        public List<robot> Robots { get; } = new();
        public int TotalRobots => Robots.Count(r => r.exist);
        private List<robot> BotsToKill { get; } = new();
        private List<robot> BotsToReproduce { get; } = new();
        private List<robot> BotsToReproduceSexually { get; } = new();

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

        public robot GetNewBot()
        {
            var robot = new robot();

            Robots.Add(robot);

            return robot;
        }

        public async Task KillRobot(robot robot)
        {
            if (SimOpt.SimOpts.DeadRobotSnp && (!robot.Veg || !SimOpt.SimOpts.SnpExcludeVegs))
                await Database.AddRecord(robot);

            robot.Fixed = false;
            robot.Veg = false;
            robot.View = false;
            robot.NewMove = false;
            robot.LastOwner = "";
            robot.SonNumber = 0;
            robot.age = 0;
            Ties.DeleteAllTies(robot);
            robot.exist = false;
            Globals.BucketManager.UpdateBotBucket(robot);

            if (robot.virusshot != null)
            {
                robot.virusshot.Exist = false;
                Globals.ShotsManager.Shots.Remove(robot.virusshot);
                robot.virusshot = null;
            }

            robot.spermDNA.Clear();

            robot.LastMutDetail = "";

            Robots.Remove(robot);
        }

        public void ShareChloroplasts(robot robot, Tie tie)
        {
            ShareResource(robot, tie, MemoryAddresses.sharechlr, r => r.chloroplasts, (r, s) => r.chloroplasts = s);
        }

        public void ShareEnergy(robot robot, Tie tie)
        {
            //This is an order of operation thing.  A bot earlier in the rob array might have taken all your nrg, taking your
            //nrg to 0.  You should still be able to take some back.
            if (robot.nrg < 0 || tie.OtherBot.nrg < 0)
                return;

            //.mem(830) is the percentage of the total nrg this bot wants to receive
            //has to be positive to come here, so no worries about changing the .mem location here
            if (robot.mem[830] <= 0)
                robot.mem[830] = 0;
            else
            {
                robot.mem[830] = robot.mem[830] % 100;
                if (robot.mem[830] == 0)
                    robot.mem[830] = 100;
            }

            //Total nrg of both bots combined
            var totnrg = robot.nrg + tie.OtherBot.nrg;

            var portionThatsMine = totnrg * ((double)robot.mem[830] / 100); // This is what the bot wants to have of the total
            if (portionThatsMine > 32000)
                portionThatsMine = 32000; // Can't want more than the max a bot can have

            var myChangeInNrg = portionThatsMine - robot.nrg; // This is what the bot's change in nrg would be

            //If the bot is taking nrg, then he can't take more than that represented by his own body.  If giving nrg away, same thing.  The bot
            //can't give away more than that represented by his body.  Should make it so that larger bots win tie feeding battles.
            if (Math.Abs(myChangeInNrg) > robot.Body)
                myChangeInNrg = Math.Sign(myChangeInNrg) * robot.Body;

            if (robot.nrg + myChangeInNrg > 32000)
                myChangeInNrg = 32000 - robot.nrg; //Limit change if it would put bot over the limit

            if (robot.nrg + myChangeInNrg < 0)
                myChangeInNrg = -robot.nrg; // Limit change if it would take the bot below 0

            //Now we have to check the limits on the other bot
            //sign is negative since the negative of myChangeinNrg is what the other bot is going to get/recevie
            if (tie.OtherBot.nrg - myChangeInNrg > 32000)
                myChangeInNrg = -(32000 - tie.OtherBot.nrg); //Limit change if it would put bot over the limit

            if (tie.OtherBot.nrg - myChangeInNrg < 0)
                myChangeInNrg = tie.OtherBot.nrg; // limit change if it would take the bot below 0

            //Do the actual nrg exchange
            robot.nrg += myChangeInNrg;
            tie.OtherBot.nrg -= myChangeInNrg;

            //Transferring nrg costs nrg.  1% of the transfer gets deducted from the bot iniating the transfer
            robot.nrg -= Math.Abs(myChangeInNrg) * 0.01;

            //Bots with 32000 nrg can still take or receive nrg, but everything over 32000 disappears
            if (robot.nrg > 32000)
                robot.nrg = 32000;

            if (tie.OtherBot.nrg > 32000)
                tie.OtherBot.nrg = 32000;
        }

        public void ShareShell(robot rob, Tie tie)
        {
            ShareResource(rob, tie, 832, r => r.shell, (r, s) => r.shell = s);
        }

        public void ShareSlime(robot rob, Tie tie)
        {
            ShareResource(rob, tie, 833, r => r.Slime, (r, s) => r.Slime = s);
        }

        public void ShareWaste(robot rob, Tie tie)
        {
            ShareResource(rob, tie, 831, r => r.Waste, (r, s) => r.Waste = s);
        }

        public async Task UpdateBots()
        {
            BotsToKill.Clear();
            BotsToReproduce.Clear();
            BotsToReproduceSexually.Clear();
            Globals.TotalNotVegsDisplayed = Globals.TotalNotVegs;
            Globals.TotalNotVegs = 0;
            Vegs.TotalVegsDisplayed = Vegs.TotalVegs;
            Vegs.TotalVegs = 0;

            // Only calculate mass due to fluid displacement if the sim medium has density.
            if (SimOpt.SimOpts.Density != 0)
            {
                foreach (var rob in Robots.Where(r => r.exist))
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
            foreach (var rob in Robots.Where(r => r.exist))
            {
                if (!rob.Corpse)
                    Upkeep(rob); // No upkeep costs if you are dead!

                if (!rob.Corpse && !rob.DisableDNA)
                    Poisons(rob);

                if (!SimOpt.SimOpts.DisableFixing)
                    ManageFixed(rob);

                Physics.CalcMass(rob);

                if (Globals.ObstacleManager.Obstacles.Count > 0)
                    Globals.ObstacleManager.DoObstacleCollisions(rob);

                Physics.BorderCollision(rob);

                Physics.TieHooke(rob); // Handles tie lengths, tie hardening and compressive, elastic tie forces

                if (!rob.Corpse && !rob.DisableDNA)
                    Physics.TieTorque(rob);

                if (!rob.Fixed)
                    Physics.NetForces(rob); //calculate forces on all robots

                Globals.BucketManager.BucketsCollision(rob);

                if (rob.ImpulseStatic > 0 & (rob.ImpulseInd.X != 0 || rob.ImpulseInd.Y != 0))
                {
                    double staticV;
                    if (rob.vel.X == 0 & rob.vel.Y == 0)
                        staticV = rob.ImpulseStatic;
                    else
                        staticV = rob.ImpulseStatic * Math.Abs(DoubleVector.Cross(rob.vel.Unit(), rob.ImpulseInd.Unit())); // Takes into account the fact that the robot may be moving along the same vector

                    if (staticV > rob.ImpulseInd.Magnitude())
                        rob.ImpulseInd = new DoubleVector(0, 0); // If static vector is greater then impulse vector, reset impulse vector
                }

                rob.ImpulseInd -= rob.ImpulseRes;

                if (rob.Corpse || rob.DisableDNA) continue;

                Ties.TiePortCommunication(rob); //transfer data through ties
                Ties.ReadTie(rob); //reads all of the tref variables from a given tie number
            }

            foreach (var s in SimOpt.SimOpts.Specie)
                s.population = 0;

            foreach (var rob in Robots.Where(r => r.exist))
                await UpdateCounters(rob); // Counts the number of bots and decays body...

            foreach (var rob in Robots.Where(r => r.exist))
            {
                Ties.UpdateTies(rob); // Carries all tie routines

                //EricL Transfer genetic meomory locations for newborns through the birth tie during their first 15 cycles
                if (rob.age < 15)
                    DoGeneticMemory(rob);

                if (!rob.Corpse && !rob.DisableDNA)
                {
                    SetAimFunc(rob); //Setup aiming
                    BotDnaManipulation(rob);
                }

                UpdatePosition(rob); //updates robot's position
                                     //EricL 4/9/2006 Got rid of a loop below by moving these inside this loop.  Should speed things up a little.

                rob.nrg = Math.Clamp(rob.nrg, -32000, 32000);
                rob.poison = Math.Clamp(rob.poison, -32000, 32000);
                rob.venom = Math.Clamp(rob.venom, -32000, 32000);
                rob.Waste = Math.Clamp(rob.Waste, -32000, 32000);
            }

            foreach (var rob in Robots)
            {
                Ties.UpdateTieAngles(rob); // Updates .tielen and .tieang.  Have to do this here after all bot movement happens above.

                if (!rob.Corpse && !rob.DisableDNA && rob.exist)
                {
                    NeoMutations.Mutate(rob);
                    MakeStuff(rob);
                    HandleWaste(rob);
                    Shooting(rob);

                    if (!rob.NoChlr)
                        ManageChlr(rob);

                    ManageBody(rob);
                    ManageBouyancy(rob);
                    ManageReproduction(rob);
                    Shock(rob);
                    Senses.WriteSenses(rob);
                    FireTies(rob);
                }
                if (!rob.Corpse && rob.exist)
                {
                    Ageing(rob); // Even bots with disabled DNA age...
                    ManageDeath(rob); // Even bots with disabled DNA can die...
                }
                if (rob.exist)
                {
                    Vegs.TotalSimEnergy[Vegs.CurrentEnergyCycle] = (int)(Vegs.TotalSimEnergy[Vegs.CurrentEnergyCycle] + rob.nrg + rob.Body * 10);
                }
            }

            await ReproduceAndKill();
            RemoveExtinctSpecies();
        }

        private void Ageing(robot rob)
        {
            //aging
            rob.age++;
            rob.newage++; //Age this simulation to be used by tie code
            var tempAge = rob.age;
            if (tempAge > 32000)
                tempAge = 32000;

            rob.mem[MemoryAddresses.robage] = tempAge; //line added to copy robots age into a memory location
            rob.mem[MemoryAddresses.timersys] += 1; //update epigenetic timer

            if (rob.mem[MemoryAddresses.timersys] > 32000)
                rob.mem[MemoryAddresses.timersys] = -32000;
        }

        private void Altzheimer(robot rob)
        {
            // Makes robots with high waste act in a bizarre fashion.
            var loops = (rob.Pwaste + rob.Waste - SimOpt.SimOpts.BadWasteLevel) / 4;

            for (var t = 0; t < loops; t++)
            {
                int loc;
                do
                {
                    loc = ThreadSafeRandom.Local.Next(1, 1000);
                } while (!(loc != MemoryAddresses.mkchlr && loc != MemoryAddresses.rmchlr));
                var val = ThreadSafeRandom.Local.Next(-32000, 32000);
                rob.mem[loc] = val;
            }
        }

        private void BotDnaManipulation(robot rob)
        {
            // count down
            if (rob.Vtimer > 1)
                rob.Vtimer--;

            rob.mem[MemoryAddresses.Vtimer] = rob.Vtimer;

            // Viruses
            if (rob.mem[MemoryAddresses.mkvirus] > 0 & rob.Vtimer == 0)
            {
                if (rob.chloroplasts == 0)
                {
                    if (Globals.ShotsManager.MakeVirus(rob, rob.mem[MemoryAddresses.mkvirus]))
                    {
                        var length = GeneLength(rob, rob.mem[MemoryAddresses.mkvirus]) * 2;
                        rob.nrg -= length / 2.0 * SimOpt.SimOpts.Costs.DnaCopyCost * SimOpt.SimOpts.Costs.CostMultiplier;
                        rob.Vtimer = Math.Min(length, 32000);
                    }
                    else
                    {
                        rob.Vtimer = 0;
                        rob.virusshot = null;
                    }
                }
                else
                {
                    rob.chloroplasts = 0;
                }
            }

            // shoot it!
            if (rob.mem[MemoryAddresses.VshootSys] != 0 & rob.Vtimer == 1)
            {
                // Botsareus 10/5/2015 Bugfix for negative values in vshoot
                Globals.ShotsManager.ShootVirus(rob, rob.virusshot);

                rob.mem[MemoryAddresses.VshootSys] = 0;
                rob.mem[MemoryAddresses.Vtimer] = 0;
                rob.mem[MemoryAddresses.mkvirus] = 0;
                rob.Vtimer = 0;
                rob.virusshot = null;
            }

            // Other stuff

            if (rob.mem[MemoryAddresses.DelgeneSys] > 0)
            {
                NeoMutations.DeleteGene(rob, rob.mem[MemoryAddresses.DelgeneSys]);
                rob.mem[MemoryAddresses.DelgeneSys] = 0;
            }

            rob.mem[MemoryAddresses.DnaLenSys] = rob.dna.Count;
            rob.mem[MemoryAddresses.GenesSys] = rob.genenum;
        }

        private void ChangeChlr(robot rob)
        {
            var tmpchlr = rob.chloroplasts;

            // add chloroplasts
            rob.chloroplasts += rob.mem[MemoryAddresses.mkchlr];

            // remove chloroplasts
            rob.chloroplasts -= rob.mem[MemoryAddresses.rmchlr];

            if (tmpchlr < rob.chloroplasts)
            {
                var newnrg = rob.nrg - (rob.chloroplasts - tmpchlr) * SimOpt.SimOpts.Costs.CholorplastCost * SimOpt.SimOpts.Costs.CostMultiplier;

                if (Globals.TotalChlr > SimOpt.SimOpts.MaxPopulation && rob.Veg || newnrg < 100)
                    rob.chloroplasts = tmpchlr;
                else
                    rob.nrg = newnrg; //Botsareus 8/24/2013 only charge energy for adding chloroplasts to prevent robots from cheating by adding and subtracting there chlroplasts in 3 cycles
            }

            rob.mem[MemoryAddresses.mkchlr] = 0;
            rob.mem[MemoryAddresses.rmchlr] = 0;
        }

        private List<DNABlock> Crossover(List<block2> dna1, List<block2> dna2)
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

                var outDna = new List<DNABlock>();

                var temp = 0;
                res1 = ScanFromN(dna1, n1, ref temp);
                res2 = ScanFromN(dna2, n2, ref i);

                if (res1 - n1 > 0 & res2 - n2 > 0)
                {
                    if (ThreadSafeRandom.Local.Next(0, 2) == 0)
                    {
                        for (var a = n1; a < res1 - 1; a++)
                            outDna.Add(new DNABlock { Type = dna1[a].tipo, Value = dna1[a].value });
                    }
                    else
                    {
                        for (var a = n2; a < res2 - 1; a++)
                            outDna.Add(new DNABlock { Type = dna2[a].tipo, Value = dna2[a].value });
                    }
                }
                else if (res1 - n1 > 0)
                {
                    if (ThreadSafeRandom.Local.Next(0, 2) == 0)
                    {
                        for (var a = n1; a < res1 - 1; a++)
                            outDna.Add(new DNABlock { Type = dna1[a].tipo, Value = dna1[a].value });
                    }
                }
                else if (res2 - n2 > 0)
                {
                    if (ThreadSafeRandom.Local.Next(0, 2) == 0)
                    {
                        for (var a = n2; a < res2 - 1; a++)
                            outDna.Add(new DNABlock { Type = dna2[a].tipo, Value = dna2[a].value });
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
                    var block = new DNABlock
                    {
                        Type = whatside ? dna1[a].tipo : dna2[a - nn + res2].tipo,
                        Value = (dna1[a].tipo == dna2[a - nn + res2].tipo && Math.Abs(dna2[a].value) > 999 && Math.Abs(dna2[a - nn + res2].value) > 999 ? ThreadSafeRandom.Local.Next(0, 2) == 0 : whatside) ? dna1[a].value : dna2[a - nn + res2].value
                    };
                    outDna.Add(block);
                }
            }
        }

        private void DoGeneticMemory(robot rob)
        {
            if (rob.Ties.Count <= 0 || rob.Ties[0].Last <= 0)
                return;

            var loc = 976 + rob.age;
            if (rob.mem[loc] == 0 & rob.epimem[rob.age] != 0)
            {
                rob.mem[loc] = rob.epimem[rob.age];
            }
        }

        private void FeedBody(robot rob)
        {
            if (rob.mem[MemoryAddresses.fdbody] > 100)
                rob.mem[MemoryAddresses.fdbody] = 100;

            rob.nrg += rob.mem[MemoryAddresses.fdbody];
            rob.Body -= (double)rob.mem[MemoryAddresses.fdbody] / 10;

            if (rob.nrg > 32000)
                rob.nrg = 32000;

            rob.mem[MemoryAddresses.fdbody] = 0;
        }

        private void FireTies(robot rob)
        {
            var resetlastopp = false;

            if (rob.lastopp == null & rob.age < 2 && rob.parent is { exist: true })
            {
                rob.lastopp = rob.parent;
                resetlastopp = true;
            }

            if (rob.lastopp == null & rob.lasttch != null && rob.lasttch.exist)
            {
                rob.lastopp = rob.lasttch;
                resetlastopp = true;
            }

            if (rob.mem[MemoryAddresses.mtie] != 0 && rob.lastopp != null & !SimOpt.SimOpts.DisableTies && rob.lastopp is robot lastOpp)
            {
                var length = (lastOpp.pos - rob.pos).Magnitude();
                var maxLength = RobSize * 4 + rob.Radius + lastOpp.Radius;

                if (length <= maxLength)
                    Ties.MakeTie(rob, lastOpp, (int)(rob.Radius + lastOpp.Radius + RobSize * 2), -20, rob.mem[MemoryAddresses.mtie]);

                rob.mem[MemoryAddresses.mtie] = 0;
            }

            if (resetlastopp)
                rob.lastopp = null;
        }

        private int GeneLength(robot rob, int p)
        {
            var pos = DnaManipulations.GenePosition(rob.dna, p);
            return DnaManipulations.GeneEnd(rob.dna, pos) - pos + 1;
        }

        private double GeneticDistance(List<block3> rob1, List<block3> rob2)
        {
            return rob1.Count(b => b.match == 0) + rob2.Count(b => b.match == 0) / (rob1.Count + rob2.Count);
        }

        private void HandleWaste(robot rob)
        {
            if (rob.Waste > 0 && rob.chloroplasts > 0)
                Vegs.feedveg2(rob);

            if (SimOpt.SimOpts.BadWasteLevel == 0)
                SimOpt.SimOpts.BadWasteLevel = 400;

            if (SimOpt.SimOpts.BadWasteLevel > 0 & rob.Pwaste + rob.Waste > SimOpt.SimOpts.BadWasteLevel)
                Altzheimer(rob);

            if (rob.Waste > 32000)
                Globals.ShotsManager.Defecate(rob);

            if (rob.Pwaste > 32000)
                rob.Pwaste = 32000;

            if (rob.Waste < 0)
                rob.Waste = 0;

            rob.mem[828] = (int)rob.Waste;
            rob.mem[829] = (int)rob.Pwaste;
        }

        private void MakeShell(robot rob)
        {
            const double shellNrgConvRate = 0.1;

            StoreResource(rob, 822, 823, shellNrgConvRate, SimOpt.SimOpts.Costs.ShellCost, r => r.shell, (r, s) => r.shell = s, true);
        }

        private void MakeSlime(robot rob)
        {
            const double slimeNrgConvRate = 0.1;

            StoreResource(rob, 820, 821, slimeNrgConvRate, SimOpt.SimOpts.Costs.SlimeCost, r => r.Slime, (r, s) => r.Slime = s, true);
        }

        private void MakeStuff(robot rob)
        {
            if (rob.mem[824] != 0)
                StoreVenom(rob);

            if (rob.mem[826] != 0)
                StorePoison(rob);

            if (rob.mem[822] != 0)
                MakeShell(rob);

            if (rob.mem[820] != 0)
                MakeSlime(rob);
        }

        private void ManageBody(robot rob)
        {
            if (rob.mem[MemoryAddresses.strbody] > 0)
                StoreBody(rob);

            if (rob.mem[MemoryAddresses.fdbody] > 0)
                FeedBody(rob);

            rob.Body = Math.Clamp(rob.Body, 0, 32000);

            rob.mem[MemoryAddresses.body] = (int)rob.Body;
        }

        private void ManageBouyancy(robot rob)
        {
            if (rob.mem[MemoryAddresses.setboy] == 0)
                return;

            rob.Bouyancy += (double)rob.mem[MemoryAddresses.setboy] / 32000;
            rob.Bouyancy = Math.Clamp(rob.Bouyancy, 0, 1);

            rob.mem[MemoryAddresses.rdboy] = (int)(rob.Bouyancy * 32000);
            rob.mem[MemoryAddresses.setboy] = 0;
        }

        private void ManageChlr(robot rob)
        {
            if (rob.mem[MemoryAddresses.mkchlr] > 0 || rob.mem[MemoryAddresses.rmchlr] > 0)
                ChangeChlr(rob);

            rob.chloroplasts -= 0.5 / Math.Pow(100, rob.chloroplasts / 16000);

            rob.chloroplasts = Math.Clamp(rob.chloroplasts, 0, 32000);

            rob.mem[MemoryAddresses.chlr] = (int)rob.chloroplasts;
            rob.mem[MemoryAddresses.light] = (int)(32000 - Vegs.LightAval * 32000);
        }

        private void ManageDeath(robot rob)
        {
            if (SimOpt.SimOpts.CorpseEnabled)
            {
                if (!rob.Corpse && rob.nrg < 15 && rob.age > 0)
                {
                    rob.Corpse = true;
                    rob.FName = "Corpse";
                    Array.Clear(rob.occurr, 0, rob.occurr.Length);
                    rob.color = Colors.White;
                    rob.Veg = false;
                    rob.Fixed = false;
                    rob.nrg = 0;
                    rob.DisableDNA = true;
                    rob.DisableMovementSysvars = true;
                    rob.CantSee = true;
                    rob.VirusImmune = true;
                    rob.chloroplasts = 0;

                    for (var i = MemoryAddresses.EyeStart + 1; i < MemoryAddresses.EyeEnd; i++)
                        rob.mem[i] = 0;

                    rob.Bouyancy = 0;
                }
                if (rob.Body < 0.5)
                    rob.Dead = true;
            }
            else if (rob.nrg < 0.5 || rob.Body < 0.5)
                rob.Dead = true;

            if (rob.Dead)
                BotsToKill.Add(rob);
        }

        private void ManageFixed(robot rob)
        {
            rob.Fixed = rob.mem[216] > 0;
        }

        private void ManageReproduction(robot rob)
        {
            switch (rob.fertilized)
            {
                case >= 0:
                    rob.fertilized--;
                    rob.mem[MemoryAddresses.SYSFERTILIZED] = rob.fertilized >= 0 ? rob.fertilized : 0;
                    break;

                case < -10:
                    rob.fertilized++;
                    break;

                default:
                    if (rob.fertilized == -1)
                        rob.spermDNA.Clear();
                    rob.fertilized = -2;
                    break;
            }

            // Asexual reproduction
            if ((rob.mem[MemoryAddresses.Repro] > 0 || rob.mem[MemoryAddresses.mrepro] > 0) && !rob.CantReproduce)
                BotsToReproduce.Add(rob);

            // Sexual Reproduction
            if (rob.mem[MemoryAddresses.SEXREPRO] > 0 & rob.fertilized >= 0 & !rob.CantReproduce)
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

        private void Poisons(robot rob)
        {
            if (rob.Paralyzed)
                rob.mem[rob.Vloc] = rob.Vval;

            if (rob.Paralyzed)
            {
                rob.Paracount--;

                if (rob.Paracount < 1)
                {
                    rob.Paralyzed = false;
                    rob.Vloc = 0;
                    rob.Vval = 0;
                }
            }

            rob.mem[837] = (int)rob.Paracount;

            if (rob.Poisoned)
                rob.mem[rob.Ploc] = rob.Pval;

            if (rob.Poisoned)
            {
                rob.Poisoncount--;
                if (rob.Poisoncount < 1)
                {
                    rob.Poisoned = false;
                    rob.Ploc = 0;
                    rob.Pval = 0;
                }
            }

            rob.mem[838] = (int)rob.Poisoncount;
        }

        private void RemoveExtinctSpecies()
        {
            foreach (var s in SimOpt.SimOpts.Specie.Where(s => s.population == 0 && !s.Native).ToArray())
                SimOpt.SimOpts.Specie.Remove(s);
        }

        private void Reproduce(robot robot, int per)
        {
            if (robot.Body < 5)
                return;

            if (SimOpt.SimOpts.DisableTypArepro && robot.Veg == false)
                return;

            if (robot.Body <= 2 || robot.CantReproduce)
                return;

            // Attempt to stop veg overpopulation but will it work?
            if (robot.Veg && (Globals.TotalChlr > SimOpt.SimOpts.MaxPopulation || Vegs.TotalVegsDisplayed < 0))
                return;

            // If we got here and it's a veg, then we are below the reproduction threshold.  Let a random 10% of the veggis reproduce
            // so as to avoid all the veggies reproducing on the same cycle.  This adds some randomness
            // so as to avoid giving preference to veggies with lower bot array numbers.  If the veggy population is below 90% of the threshold
            // then let them all reproduce.
            if (robot.Veg && ThreadSafeRandom.Local.Next(0, 10) != 5 && Globals.TotalChlr > SimOpt.SimOpts.MaxPopulation * 0.9)
                return;
            if (Vegs.TotalVegsDisplayed == -1)
                return;

            per %= 100; // per should never be <=0 as this is checked in ManageReproduction()

            if (per <= 0)
                return;

            var sondist = (int)robot.Radius;

            if (robot.nrg <= 0)
                return;

            var nx = (int)(robot.pos.X + AbsX(robot.aim, sondist, 0, 0, 0));
            var ny = (int)(robot.pos.Y + AbsY(robot.aim, sondist, 0, 0, 0));

            if (SimpleCollision(nx, ny, robot) || !robot.exist)
                return;

            var nuovo = GetNewBot();

            SimOpt.SimOpts.TotBorn = SimOpt.SimOpts.TotBorn + 1;

            if (robot.Veg)
                Vegs.TotalVegs++;

            nuovo.dna.AddRange(robot.dna);
            nuovo.genenum = robot.genenum;
            nuovo.Mutables = robot.Mutables;

            nuovo.Mutations = robot.Mutations;
            nuovo.OldMutations = robot.OldMutations;

            nuovo.LastMut = 0;
            nuovo.LastMutDetail = robot.LastMutDetail;

            for (var t = 0; t < 12; t++)
                nuovo.Skin[t] = robot.Skin[t];

            Array.Clear(nuovo.mem, 0, nuovo.mem.Length);
            nuovo.Ties.Clear();

            nuovo.pos = new DoubleVector(robot.pos.X + AbsX(robot.aim, sondist, 0, 0, 0), robot.pos.Y + AbsY(robot.aim, sondist, 0, 0, 0));
            nuovo.exist = true;
            nuovo.BucketPos = new IntVector(-2, -2);
            Globals.BucketManager.UpdateBotBucket(nuovo);
            nuovo.vel = robot.vel;
            nuovo.actvel = robot.actvel;
            nuovo.color = robot.color;
            nuovo.aim = Physics.NormaliseAngle(robot.aim + Math.PI);

            nuovo.aimvector = new DoubleVector(Math.Cos(nuovo.aim), Math.Sin(nuovo.aim));
            nuovo.mem[MemoryAddresses.SetAim] = (int)(nuovo.aim * 200);
            nuovo.mem[468] = 32000;
            nuovo.Corpse = false;
            nuovo.Dead = false;
            nuovo.NewMove = robot.NewMove;
            nuovo.generation = robot.generation + 1;

            if (nuovo.generation > 32000)
                nuovo.generation = 32000;

            nuovo.BirthCycle = SimOpt.SimOpts.TotRunCycle;

            var nnrg = robot.nrg / 100 * per;
            var nbody = robot.Body / 100 * per;
            var nwaste = robot.Waste / 100 * per;
            var npwaste = robot.Pwaste / 100 * per;
            var nchloroplasts = robot.chloroplasts / 100 * per;

            robot.nrg -= nnrg + nnrg * 0.001;
            robot.Waste -= nwaste;
            robot.Pwaste -= npwaste;
            robot.Body -= nbody;
            robot.chloroplasts -= nchloroplasts;

            nuovo.chloroplasts = nchloroplasts; //Panda 8/23/2013 Distribute the chloroplasts
            nuovo.Body = nbody;
            nuovo.Waste = nwaste;
            nuovo.Pwaste = npwaste;
            robot.mem[MemoryAddresses.Energy] = (int)robot.nrg;
            robot.mem[MemoryAddresses.body] = (int)robot.Body;
            robot.SonNumber++;

            if (robot.SonNumber > 32000)
                robot.SonNumber = 32000;

            nuovo.nrg = nnrg * 0.999; // Make reproduction cost 1% of nrg transfer
            nuovo.onrg = nnrg * 0.999;
            nuovo.mem[MemoryAddresses.Energy] = (int)nuovo.nrg;
            nuovo.Poisoned = false;
            nuovo.parent = robot;
            nuovo.FName = robot.FName;
            nuovo.LastOwner = robot.LastOwner;
            nuovo.Veg = robot.Veg;
            nuovo.NoChlr = robot.NoChlr; //Botsareus 3/28/2014 Disable chloroplasts
            nuovo.Fixed = robot.Fixed;
            nuovo.CantSee = robot.CantSee;
            nuovo.DisableDNA = robot.DisableDNA;
            nuovo.DisableMovementSysvars = robot.DisableMovementSysvars;
            nuovo.CantReproduce = robot.CantReproduce;
            nuovo.VirusImmune = robot.VirusImmune;
            if (nuovo.Fixed)
                nuovo.mem[MemoryAddresses.Fixed] = 1;

            nuovo.SubSpecies = robot.SubSpecies;
            nuovo.OldGD = robot.OldGD;
            nuovo.GenMut = robot.GenMut;
            nuovo.tag = robot.tag;
            nuovo.Bouyancy = robot.Bouyancy;

            if (robot.multibot_time > 0)
                nuovo.multibot_time = robot.multibot_time / 2 + 2;

            nuovo.Vtimer = 0;
            nuovo.virusshot = null;

            for (var i = 0; i < 4; i++)
                nuovo.mem[971 + i] = robot.mem[971 + i];

            //The other 15 genetic memory locations are stored now but can be used later
            for (var i = 0; i < 14; i++)
                nuovo.epimem[i] = robot.mem[976 + i];

            //Erase parents genetic memory now to prevent him from completing his own transfer by using his kid
            for (var i = 0; i < 14; i++)
                robot.epimem[i] = 0;

            //Botsareus 12/17/2013 Delta2
            if (robot.mem[MemoryAddresses.mrepro] > 0)
            {
                var temp = nuovo.Mutables;

                nuovo.Mutables.EnableMutations = true; // mutate even if mutations disabled for this bot

                nuovo.Mutables.CopyError = MutateProbability(nuovo.Mutables.CopyError);
                nuovo.Mutables.Delta = MutateProbability(nuovo.Mutables.Delta);
                nuovo.Mutables.Insertion = MutateProbability(nuovo.Mutables.Insertion);
                nuovo.Mutables.MajorDeletion = MutateProbability(nuovo.Mutables.MajorDeletion);
                nuovo.Mutables.MinorDeletion = MutateProbability(nuovo.Mutables.MinorDeletion);
                nuovo.Mutables.PointMutation = MutateProbability(nuovo.Mutables.PointMutation);
                nuovo.Mutables.Reversal = MutateProbability(nuovo.Mutables.Reversal);

                NeoMutations.Mutate(nuovo, true);

                nuovo.Mutables = temp;
            }
            else
                NeoMutations.Mutate(nuovo, true);

            Senses.MakeOccurrList(nuovo);
            nuovo.genenum = DnaManipulations.CountGenes(nuovo.dna);
            nuovo.mem[MemoryAddresses.DnaLenSys] = nuovo.dna.Count;
            nuovo.mem[MemoryAddresses.GenesSys] = nuovo.genenum;

            Ties.MakeTie(robot, nuovo, sondist, 100, 0); //birth ties last 100 cycles
            robot.onrg = robot.nrg; //saves parent from dying from shock after giving birth
            nuovo.mass = nbody / 1000 + nuovo.shell / 200;
            nuovo.mem[MemoryAddresses.timersys] = robot.mem[MemoryAddresses.timersys]; //epigenetic timer

            //Successfully reproduced
            robot.mem[MemoryAddresses.Repro] = 0;
            robot.mem[MemoryAddresses.mrepro] = 0;

            robot.nrg -= robot.dna.Count * SimOpt.SimOpts.Costs.DnaCopyCost * SimOpt.SimOpts.Costs.CostMultiplier;
            if (robot.nrg < 0)
                robot.nrg = 0;
        }

        private async Task ReproduceAndKill()
        {
            foreach (var r in BotsToReproduce)
            {
                var per = 0;

                if (r.mem[MemoryAddresses.mrepro] > 0 && r.mem[MemoryAddresses.Repro] > 0)
                {
                    per = ThreadSafeRandom.Local.NextDouble() > 0.5 ? r.mem[MemoryAddresses.Repro] : r.mem[MemoryAddresses.mrepro];
                }
                else
                {
                    if (r.mem[MemoryAddresses.mrepro] > 0)
                    {
                        per = r.mem[MemoryAddresses.mrepro];
                    }
                    if (r.mem[MemoryAddresses.Repro] > 0)
                    {
                        per = r.mem[MemoryAddresses.Repro];
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

        private void SetAimFunc(robot rob)
        {
            double aim;
            double diff2 = 0;
            double diff = rob.mem[MemoryAddresses.aimsx] - rob.mem[MemoryAddresses.aimdx];

            if (rob.mem[MemoryAddresses.SetAim] == Physics.RadiansToInt(rob.aim))
            {
                aim = rob.aim * 200 + diff;
            }
            else
            {
                // .setaim overrides .aimsx and .aimdx
                aim = rob.mem[MemoryAddresses.SetAim]; // this is where .aim needs to be
                diff = -Physics.AngDiff(rob.aim, Physics.NormaliseAngle((double)rob.mem[MemoryAddresses.SetAim] / 200)) * 200; // this is the diff to get there 'Botsareus 6/18/2016 Added angnorm
                diff2 = Math.Abs(Math.Round((rob.aim * 200 - rob.mem[MemoryAddresses.SetAim]) / 1256, 0) * 1256) * Math.Sign(diff); // this is how much we add to momentum
            }

            //diff + diff2 is now the amount, positive or negative to turn.
            rob.nrg -= Math.Abs(Math.Round((diff + diff2) / 200, 3) * SimOpt.SimOpts.Costs.RotationCost * SimOpt.SimOpts.Costs.CostMultiplier);

            aim %= 1256;
            if (aim < 0)
                aim += 1256;

            aim /= 200;

            //Overflow Protection
            while (rob.ma > 2 * Math.PI)
                rob.ma -= 2 * Math.PI;

            while (rob.ma < -2 * Math.PI)
                rob.ma += 2 * Math.PI;

            rob.aim = aim + rob.ma; // Add in the angular momentum

            //Voluntary rotation can reduce angular momentum but does not add to it.

            if (rob.ma > 0 & diff < 0)
            {
                rob.ma += (diff + diff2) / 200;
                if (rob.ma < 0)
                    rob.ma = 0;
            }
            if (rob.ma < 0 & diff > 0)
            {
                rob.ma += (diff + diff2) / 200;
                if (rob.ma > 0)
                    rob.ma = 0;
            }

            rob.aimvector = new DoubleVector(Math.Cos(rob.aim), Math.Sin(rob.aim));

            rob.mem[MemoryAddresses.aimsx] = 0;
            rob.mem[MemoryAddresses.aimdx] = 0;
            rob.mem[MemoryAddresses.AimSys] = (int)(rob.aim * 200);
            rob.mem[MemoryAddresses.SetAim] = rob.mem[MemoryAddresses.AimSys];
        }

        private void SexReproduce(robot female)
        {
            if (female.Body < 5 || !female.exist || female.Corpse || female.CantReproduce || female.Body <= 2 || !female.spermDNA.Any())
                return;

            //The percent of resources given to the offspring comes exclusivly from the mother
            //Perhaps this will lead to sexual selection since sex is expensive for females but not for males
            double per = female.mem[MemoryAddresses.SEXREPRO];

            //veggies can reproduce sexually, but we still have to test for veggy population controls
            //we let male veggies fertilize nonveggie females all they want since the offspring's "species" and thus vegginess
            //will be determined by their mother.  Perhaps a strategy will emerge where plants compete to reproduce
            //with nonveggies so as to bypass the popualtion limtis?  Who knows.
            if (female.Veg && (Globals.TotalChlr > SimOpt.SimOpts.MaxPopulation || Vegs.TotalVegsDisplayed < 0))
                return;

            // If we got here and the female is a veg, then we are below the reproduction threshold.  Let a random 10% of the veggis reproduce
            // so as to avoid all the veggies reproducing on the same cycle.  This adds some randomness
            // so as to avoid giving preference to veggies with lower bot array numbers.  If the veggy population is below 90% of the threshold
            // then let them all reproduce.
            if (female.Veg && ThreadSafeRandom.Local.Next(0, 9) != 5 && Globals.TotalChlr > SimOpt.SimOpts.MaxPopulation * 0.9)
                return;

            if (Vegs.TotalVegsDisplayed == -1)
                return;// no veggies can reproduce on the first cycle after the sim is restarted.

            per %= 100; // per should never be <=0 as this is checked in ManageReproduction()

            if (per <= 0)
                return;

            var sondist = female.Radius;

            var tempnrg = female.nrg;

            if (tempnrg <= 0)
                return;

            var nx = female.pos.X + AbsX(female.aim, (int)sondist, 0, 0, 0);
            var ny = female.pos.Y + AbsY(female.aim, (int)sondist, 0, 0, 0);

            if (SimpleCollision((int)nx, (int)ny, female) || !female.exist)
                return;

            //Step1 Copy both dnas into block2

            var dna1 = female.dna.Select(d => new block2 { tipo = d.Type, value = d.Value }).ToList();
            var dna2 = female.spermDNA.Select(d => new block2 { tipo = d.Type, value = d.Value }).ToList();

            //Step2 map nucli

            var ndna1 = dna1.Select(d => new block3 { nucli = DnaTokenizing.DnaToInt(d.tipo, d.value) }).ToList();
            var ndna2 = dna2.Select(d => new block3 { nucli = DnaTokenizing.DnaToInt(d.tipo, d.value) }).ToList();

            //Step3 Check longest sequences

            SimpleMatch(ndna1, ndna2);

            //If robot is too unsimiler then do not reproduce and block sex reproduction for 8 cycles

            if (GeneticDistance(ndna1, ndna2) > 0.6)
            {
                female.fertilized = -18;
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
            if (female.Veg)
                Vegs.TotalVegs++;

            //Step4 after robot is created store the dna

            nuovo.dna = outDna;
            nuovo.genenum = DnaManipulations.CountGenes(nuovo.dna);
            nuovo.Mutables = female.Mutables;

            nuovo.Mutations = female.Mutations;
            nuovo.OldMutations = female.OldMutations; //Botsareus 10/8/2015

            nuovo.LastMut = 0;
            nuovo.LastMutDetail = female.LastMutDetail;

            for (var t = 0; t < 12; t++)
            {
                nuovo.Skin[t] = female.Skin[t];
            }

            Array.Clear(nuovo.mem, 0, nuovo.mem.Length);
            nuovo.Ties.Clear();

            nuovo.pos += new DoubleVector(AbsX(female.aim, (int)sondist, 0, 0, 0), AbsY(female.aim, (int)sondist, 0, 0, 0));
            nuovo.exist = true;
            nuovo.BucketPos = new IntVector(-2, -2);
            Globals.BucketManager.UpdateBotBucket(nuovo);

            nuovo.vel = female.vel;
            nuovo.actvel = female.actvel; //Botsareus 7/1/2016 Bugfix
            nuovo.color = female.color;
            nuovo.aim = Physics.NormaliseAngle(female.aim + Math.PI);
            nuovo.aimvector = new DoubleVector(Math.Cos(nuovo.aim), Math.Sin(nuovo.aim));
            nuovo.mem[MemoryAddresses.SetAim] = (int)(nuovo.aim * 200);
            nuovo.mem[MemoryAddresses.FIXANG] = 32000;
            nuovo.Corpse = false;
            nuovo.Dead = false;
            nuovo.NewMove = female.NewMove;
            nuovo.generation++;
            if (nuovo.generation > 32000)
                nuovo.generation = 32000; //Botsareus 10/9/2015 Overflow protection

            nuovo.BirthCycle = SimOpt.SimOpts.TotRunCycle;

            var nnrg = female.nrg / 100 * per;
            var nbody = female.Body / 100 * per;
            var nwaste = female.Waste / 100 * per;
            var npwaste = female.Pwaste / 100 * per;
            var nchloroplasts = female.chloroplasts / 100 * per; //Panda 8/23/2013 Distribute the chloroplasts

            female.nrg -= nnrg + nnrg * 0.001;
            female.Waste -= nwaste;
            female.Pwaste -= npwaste;
            female.Body -= nbody;
            female.chloroplasts -= nchloroplasts; //Panda 8/23/2013 Distribute the chloroplasts

            nuovo.chloroplasts = nchloroplasts; //Botsareus 8/24/2013 Distribute the chloroplasts
            nuovo.Body = nbody;
            nuovo.Waste = nwaste;
            nuovo.Pwaste = npwaste;
            female.mem[MemoryAddresses.Energy] = (int)female.nrg;
            female.mem[MemoryAddresses.body] = (int)female.Body;
            female.SonNumber++;

            if (female.SonNumber > 32000)
                female.SonNumber = 32000; // EricL Overflow protection.  Should change to Long at some point.

            nuovo.nrg = nnrg * 0.999; // Make reproduction cost 1% of nrg transfer for offspring
            nuovo.onrg = nnrg * 0.999;
            nuovo.mem[MemoryAddresses.Energy] = (int)nuovo.nrg;
            nuovo.Poisoned = false;
            nuovo.parent = female;
            nuovo.FName = female.FName;
            nuovo.LastOwner = female.LastOwner;
            nuovo.Veg = female.Veg;
            nuovo.NoChlr = female.NoChlr; //Botsareus 3/28/2014 Disable chloroplasts
            nuovo.Fixed = female.Fixed;
            nuovo.CantSee = female.CantSee;
            nuovo.DisableDNA = female.DisableDNA;
            nuovo.DisableMovementSysvars = female.DisableMovementSysvars;
            nuovo.CantReproduce = female.CantReproduce;
            nuovo.VirusImmune = female.VirusImmune;
            if (nuovo.Fixed)
                nuovo.mem[MemoryAddresses.Fixed] = 1;

            nuovo.SubSpecies = female.SubSpecies;

            nuovo.OldGD = female.OldGD;
            nuovo.GenMut = female.GenMut;
            nuovo.tag = female.tag;
            nuovo.Bouyancy = female.Bouyancy;

            if (female.multibot_time > 0)
                nuovo.multibot_time = female.multibot_time / 2 + 2;

            nuovo.Vtimer = 0;
            nuovo.virusshot = null;

            //First 5 genetic memory locations happen instantly
            for (var i = 0; i < 5; i++)
                nuovo.mem[971 + i] = female.mem[971 + i];

            //The other 15 genetic memory locations are stored now but can be used later
            for (var i = 0; i < 15; i++)
                nuovo.epimem[i] = female.mem[976 + i];

            //Erase parents genetic memory now to prevent him from completing his own transfer by using his kid
            for (var i = 0; i < 14; i++)
                female.epimem[i] = 0;

            NeoMutations.LogMutation(nuovo, $"Female DNA len {female.dna.Count} and male DNA len {female.spermDNA.Count} had offspring DNA len {nuovo.dna.Count} during cycle {SimOpt.SimOpts.TotRunCycle}");

            NeoMutations.Mutate(nuovo, true);

            Senses.MakeOccurrList(nuovo);
            nuovo.genenum = DnaManipulations.CountGenes(nuovo.dna);
            nuovo.mem[MemoryAddresses.DnaLenSys] = nuovo.dna.Count;
            nuovo.mem[MemoryAddresses.GenesSys] = nuovo.genenum;

            Ties.MakeTie(female, nuovo, (int)sondist, 100, 0); //birth ties last 100 cycles
            female.onrg = female.nrg; //saves mother from dying from shock after giving birth
            nuovo.mass = nbody / 1000 + nuovo.shell / 200;
            nuovo.mem[MemoryAddresses.timersys] = female.mem[MemoryAddresses.timersys]; //epigenetic timer

            female.mem[MemoryAddresses.SEXREPRO] = 0; // sucessfully reproduced, so reset .sexrepro
            female.fertilized = -1; // Set to -1 so spermDNA space gets reclaimed next cycle
            female.mem[MemoryAddresses.SYSFERTILIZED] = 0; // Sperm is only good for one birth presently

            female.nrg -= female.dna.Count * SimOpt.SimOpts.Costs.DnaCopyCost * SimOpt.SimOpts.Costs.CostMultiplier; //Botsareus 7/7/2013 Reproduction DNACOPY cost

            if (female.nrg < 0)
                female.nrg = 0;
        }

        private void ShareResource(robot rob, Tie tie, int address, Func<robot, double> getValue, Action<robot, double> setValue)
        {
            if (rob.mem[address] > 99)
                rob.mem[address] = 99;
            if (rob.mem[address] < 0)
                rob.mem[address] = 0;

            var total = getValue(rob) + getValue(tie.OtherBot);

            setValue(tie.OtherBot, Math.Min(total * ((100 - (double)rob.mem[address]) / 100), 32000));
            setValue(rob, Math.Min(total * ((double)rob.mem[address] / 100), 32000));

            rob.mem[address] = (int)getValue(rob); // update the .shell sysvar
            tie.OtherBot.mem[address] = (int)getValue(tie.OtherBot);
        }

        private void Shock(robot rob)
        {
            if (rob.Veg || rob.nrg <= 3000)
                return;

            var temp = rob.onrg - rob.nrg;

            if (!(temp > rob.onrg / 2))
                return;

            rob.nrg = 0;
            rob.Body += rob.nrg / 10;

            if (rob.Body > 32000)
                rob.Body = 32000;
        }

        private void Shoot(robot rob)
        {
            var valmode = false;
            double energyLost;

            if (rob.nrg <= 0)
                return;

            var shtype = rob.mem[MemoryAddresses.shoot];
            double value = rob.mem[MemoryAddresses.shootval];
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

                if (cost > rob.nrg && cost > 2 && rob.nrg > 2 && valmode)
                {
                    cost = rob.nrg;
                    multiplier = Math.Log(rob.nrg / (SimOpt.SimOpts.Costs.ShotFormationCost * SimOpt.SimOpts.Costs.CostMultiplier), 2);
                }

                if (cost > rob.nrg && cost > 2 && rob.nrg > 2 && !valmode)
                {
                    cost = rob.nrg;
                    rngmultiplier = Math.Log(rob.nrg / (SimOpt.SimOpts.Costs.ShotFormationCost * SimOpt.SimOpts.Costs.CostMultiplier), 2);
                }
            }

            switch (shtype)
            {
                case > 0:
                    shtype %= MaxMem;
                    cost = SimOpt.SimOpts.Costs.ShotFormationCost * SimOpt.SimOpts.Costs.CostMultiplier;
                    if (rob.nrg < cost)
                        cost = rob.nrg;

                    rob.nrg -= cost; // EricL - postive shots should cost the shotcost
                    Globals.ShotsManager.NewShot(rob, shtype, value, 1, true);

                    break;// Nrg request Feeding Shot
                case -1:
                    if (rob.Multibot)
                        value = 20 + rob.Body / 5 * rob.Ties.Count; //Botsareus 6/22/2016 Bugfix
                    else
                        value = 20 + rob.Body / 5;

                    value *= multiplier;
                    if (rob.nrg < cost)
                        cost = rob.nrg;

                    rob.nrg -= cost;
                    Globals.ShotsManager.NewShot(rob, shtype, value, rngmultiplier, true);
                    break;// Nrg shot
                case -2:
                    value = Math.Abs(value);
                    if (rob.nrg < value)
                        value = rob.nrg;

                    if (value == 0)
                        value = rob.nrg / 100; //default energy shot.  Very small.

                    energyLost = value + SimOpt.SimOpts.Costs.ShotFormationCost * SimOpt.SimOpts.Costs.CostMultiplier / (rob.Ties.Count + 1);
                    if (energyLost > rob.nrg)
                        rob.nrg = 0;
                    else
                        rob.nrg -= energyLost;

                    Globals.ShotsManager.NewShot(rob, shtype, value, 1, true);
                    break;//shoot venom
                case -3:
                    value = Math.Abs(value);
                    if (value > rob.venom)
                        value = rob.venom;

                    if (value == 0)
                        value = rob.venom / 20; //default venom shot.  Not too small.

                    rob.venom -= value;
                    rob.mem[825] = (int)rob.venom;
                    energyLost = SimOpt.SimOpts.Costs.ShotFormationCost * SimOpt.SimOpts.Costs.CostMultiplier / (rob.Ties.Count + 1);

                    rob.nrg = energyLost > rob.nrg ? 0 : rob.nrg - energyLost;

                    Globals.ShotsManager.NewShot(rob, shtype, value, 1, true);
                    break;//shoot waste 'Botsareus 4/22/2016 Bugfix
                case -4:
                    value = Math.Abs(value);
                    if (value == 0)
                        value = rob.Waste / 20; //default waste shot. 'Botsareus 10/5/2015 Fix for waste

                    if (value > rob.Waste)
                        value = rob.Waste;

                    rob.Waste -= value * 0.99; //Botsareus 10/5/2015 Fix for waste
                    rob.Pwaste += value / 100;
                    energyLost = SimOpt.SimOpts.Costs.ShotFormationCost * SimOpt.SimOpts.Costs.CostMultiplier / (rob.Ties.Count + 1);
                    rob.nrg = energyLost > rob.nrg ? 0 : rob.nrg - energyLost;

                    Globals.ShotsManager.NewShot(rob, shtype, value, 1, true);
                    // no -5 shot here as poison can only be shot in response to an attack
                    break;//shoot body
                case -6:
                    if (rob.Multibot)
                        value = 10 + rob.Body / 2 * (rob.Ties.Count + 1);
                    else
                        value = 10 + Math.Abs(rob.Body) / 2;

                    if (rob.nrg < cost)
                        cost = rob.nrg;

                    rob.nrg -= cost;
                    value *= multiplier;
                    Globals.ShotsManager.NewShot(rob, shtype, value, rngmultiplier, true);
                    break;// shoot sperm
                case -8:
                    cost = SimOpt.SimOpts.Costs.ShotFormationCost * SimOpt.SimOpts.Costs.CostMultiplier;

                    if (rob.nrg < cost)
                        cost = rob.nrg;

                    rob.nrg -= cost; // EricL - postive shots should cost the shotcost
                    Globals.ShotsManager.NewShot(rob, shtype, value, 1, true);
                    break;
            }
            rob.mem[MemoryAddresses.shoot] = 0;
            rob.mem[MemoryAddresses.shootval] = 0;
        }

        private void Shooting(robot rob)
        {
            if (rob.mem[MemoryAddresses.shoot] != 0)
                Shoot(rob);

            rob.mem[MemoryAddresses.shoot] = 0;
        }

        private bool SimpleCollision(int x, int y, robot rob)
        {
            if (Robots.Any(r => r.exist && r != rob && Math.Abs(r.pos.X - x) < r.Radius + rob.Radius && Math.Abs(r.pos.Y - y) < r.Radius + rob.Radius))
                return true;

            if (Globals.ObstacleManager.Obstacles.Any(o => o.pos.X <= Math.Max(rob.pos.X, x) && o.pos.X + o.Width >= Math.Min(rob.pos.X, x) && o.pos.Y <= Math.Max(rob.pos.Y, y) && o.pos.Y + o.Height >= Math.Min(rob.pos.Y, y)))
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

        private void StoreBody(robot rob)
        {
            if (rob.mem[MemoryAddresses.strbody] > 100)
                rob.mem[MemoryAddresses.strbody] = 100;

            rob.nrg -= rob.mem[MemoryAddresses.strbody];
            rob.Body += rob.mem[MemoryAddresses.strbody] / 10.0;

            if (rob.Body > 32000)
                rob.Body = 32000;

            rob.mem[MemoryAddresses.strbody] = 0;
        }

        private void StorePoison(robot rob)
        {
            const double poisonNrgConvRate = 0.25; // Make 4 poison for 1 nrg

            StoreResource(rob, 826, 827, poisonNrgConvRate, SimOpt.SimOpts.Costs.PoisonCost, r => r.poison, (r, s) => r.poison = s);
        }

        private void StoreResource(robot rob, int storeAddress, int levelAddress, double conversionRate, double resourceCost, Func<robot, double> getValue, Action<robot, double> setValue, bool multiBotDiscount = false)
        {
            if (rob.nrg <= 0)
                return;

            double delta = Math.Clamp(rob.mem[storeAddress], -32000, 32000);
            delta = Math.Clamp(delta, -rob.nrg / conversionRate, rob.nrg / conversionRate);
            delta = Math.Clamp(delta, -100, 100);

            if (getValue(rob) + delta > 32000)
                delta = 32000 - getValue(rob);

            if (getValue(rob) + delta < 0)
                delta = -getValue(rob);

            setValue(rob, getValue(rob) + delta);

            if (rob.Multibot && multiBotDiscount)
                rob.nrg -= Math.Abs(delta) * conversionRate / (rob.Ties.Count + 1);
            else
                rob.nrg -= Math.Abs(delta) * conversionRate;

            //This is the transaction cost
            var cost = Math.Abs(delta) * resourceCost * SimOpt.SimOpts.Costs.CostMultiplier;

            rob.nrg -= cost;
            rob.Waste += cost;

            rob.mem[storeAddress] = 0;
            rob.mem[levelAddress] = (int)getValue(rob);
        }

        private void StoreVenom(robot rob)
        {
            const double venomNrgConvRate = 1.0; // Make 1 venom for 1 nrg

            StoreResource(rob, 824, 825, venomNrgConvRate, SimOpt.SimOpts.Costs.VenomCost, r => r.venom, (r, s) => r.venom = s);
        }

        private async Task UpdateCounters(robot rob)
        {
            var species = SimOpt.SimOpts.Specie.FirstOrDefault(s => s.Name == rob.FName);

            //If no species structure for the bot, then create one
            if (!rob.Corpse)
            {
                if (species == null)
                    MainEngine.AddSpecie(rob, false);
                else
                {
                    species.population++;
                    species.population = Math.Min(species.population, 32000);
                }
            }

            if (rob.Veg)
                Vegs.TotalVegs++;
            else if (rob.Corpse)
            {
                if (rob.Body > 0)
                    Globals.ShotsManager.Decay(rob);
                else
                    await KillRobot(rob);
            }
            else
                Globals.TotalNotVegs++;
        }

        private void UpdatePosition(robot rob)
        {
            //Following line commented since mass is set earlier in CalcMass
            if (rob.mass + rob.AddedMass < 0.25)
                rob.mass = 0.25 - rob.AddedMass; // a fudge since Euler approximation can't handle it when mass -> 0

            double vt = 0;

            if (!rob.Fixed)
            {
                // speed normalization
                rob.vel += rob.ImpulseInd * (1 / (rob.mass + rob.AddedMass));

                vt = rob.vel.MagnitudeSquare();
                if (vt > SimOpt.SimOpts.MaxVelocity * SimOpt.SimOpts.MaxVelocity)
                {
                    rob.vel = rob.vel.Unit() * SimOpt.SimOpts.MaxVelocity;
                }

                rob.pos += rob.vel;
                Globals.BucketManager.UpdateBotBucket(rob);
            }
            else
                rob.vel = new DoubleVector(0, 0);

            //Have to do these here for both fixed and unfixed bots to avoid build up of forces in case fixed bots become unfixed.
            rob.ImpulseInd = new DoubleVector(0, 0);
            rob.ImpulseRes = rob.ImpulseInd;
            rob.ImpulseStatic = 0;

            if (SimOpt.SimOpts.ZeroMomentum)
                rob.vel = new DoubleVector(0, 0);

            rob.lastup = rob.mem[MemoryAddresses.dirup];
            rob.lastdown = rob.mem[MemoryAddresses.dirdn];
            rob.lastleft = rob.mem[MemoryAddresses.dirsx];
            rob.lastright = rob.mem[MemoryAddresses.dirdx];
            rob.mem[MemoryAddresses.dirup] = 0;
            rob.mem[MemoryAddresses.dirdn] = 0;
            rob.mem[MemoryAddresses.dirdx] = 0;
            rob.mem[MemoryAddresses.dirsx] = 0;

            rob.mem[MemoryAddresses.velscalar] = (int)Math.Clamp(Math.Sqrt(vt), -32000, 32000);
            rob.mem[MemoryAddresses.vel] = (int)Math.Clamp(Math.Cos(rob.aim) * rob.vel.X + Math.Sin(rob.aim) * rob.vel.Y * -1, -32000, 32000);
            rob.mem[MemoryAddresses.veldn] = rob.mem[MemoryAddresses.vel] * -1;
            rob.mem[MemoryAddresses.veldx] = (int)Math.Clamp(Math.Sin(rob.aim) * rob.vel.X + Math.Cos(rob.aim) * rob.vel.Y, -32000, 32000);
            rob.mem[MemoryAddresses.velsx] = rob.mem[MemoryAddresses.veldx] * -1;

            rob.mem[MemoryAddresses.masssys] = (int)rob.mass;
            rob.mem[MemoryAddresses.maxvelsys] = (int)SimOpt.SimOpts.MaxVelocity;
        }

        private void Upkeep(robot rob)
        {
            double cost;

            //Age Cost
            var ageDelta = rob.age - SimOpt.SimOpts.Costs.AgeCostBeginAge;

            if (ageDelta > 0 & rob.age > 0)
            {
                if (SimOpt.SimOpts.Costs.EnableAgeCostIncreaseLog)
                    cost = SimOpt.SimOpts.Costs.AgeCost * Math.Log(ageDelta);
                else if (SimOpt.SimOpts.Costs.EnableAgeCostIncreasePerCycle)
                    cost = SimOpt.SimOpts.Costs.AgeCost + ageDelta * SimOpt.SimOpts.Costs.AgeCostIncreasePerCycle;
                else
                    cost = SimOpt.SimOpts.Costs.AgeCost;

                rob.nrg -= cost * SimOpt.SimOpts.Costs.CostMultiplier;
            }

            //BODY UPKEEP
            cost = rob.Body * SimOpt.SimOpts.Costs.BodyUpkeepCost * SimOpt.SimOpts.Costs.CostMultiplier;
            rob.nrg -= cost;

            //DNA upkeep cost
            cost = (rob.dna.Count - 1) * SimOpt.SimOpts.Costs.DnaUpkeepCost * SimOpt.SimOpts.Costs.CostMultiplier;
            rob.nrg -= cost;

            //degrade slime
            rob.Slime *= 0.98;
            if (rob.Slime < 0.5)
                rob.Slime = 0; // To keep things sane for integer rounding, etc.

            rob.mem[821] = (int)rob.Slime;

            //degrade poison
            rob.poison *= 0.98;
            if (rob.poison < 0.5)
                rob.poison = 0;

            rob.mem[827] = (int)rob.poison;
        }
    }
}
