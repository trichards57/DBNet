using DBNet.Forms;
using Iersera.DataModel;
using Iersera.Support;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Common;
using static DNAExecution;
using static Evo;
using static Globals;
using static HDRoutines;
using static Multibots;
using static ObstaclesManager;
using static Robots;
using static Senses;
using static ShotsManager;
using static SimOpt;
using static Teleport;
using static Vegs;

internal static class Master
{
    public static bool CostsWereZeroed { get; set; }
    public static int DynamicCountdown { get; set; }
    public static double energydif { get; set; }
    public static double energydif2 { get; set; }
    public static double energydifX { get; set; }
    public static double energydifX2 { get; set; }
    public static double energydifXP { get; set; }
    public static double energydifXP2 { get; set; }
    public static int[] PopulationLast10Cycles { get; set; } = new int[11];
    public static bool savenow { get; set; }
    public static bool stagnent { get; set; }
    public static bool stopflag { get; set; }
    private static double totnrgnvegs { get; set; }

    public static async Task UpdateSim()
    {
        ModeChangeCycles++;
        SimOpts.TotRunCycle++;

        var usehidepred = x_restartmode == 4 || x_restartmode == 5;

        if (usehidepred)
        {
            var Base_count = rob.Count(r => r.exist && r.FName == "Base.txt");
            var Mutate_count = rob.Count(r => r.exist && r.FName == "Mutate.txt");

            if (Base_count > Mutate_count)
                stagnent = false;

            if (Mutate_count == 0)
            {
                stopflag = true; //Botsareus 9/2/2014 A bug fix from Spork22
                await UpdateLostEvo();
            }

            if (Base_count == 0 & !stopflag)
                await UpdateWonEvo(Form1.instance.fittest());

            if (SimOpts.TotRunCycle == 1000000)
                stagnent = true; //Set the stagnent flag now and see what happens

            while (ModeChangeCycles > (hidePredCycl / 1.2m + hidePredOffset))
            {
                if (LFOR == 150 & Mutate_count < Base_count && hidepred)
                {
                    ModeChangeCycles -= 100;
                    continue;
                }

                energydif2 += energydif / ModeChangeCycles;

                if (hidepred)
                {
                    double holdXP = 0;

                    holdXP = (energydifX - (energydif / ModeChangeCycles)) / LFOR;
                    energydifXP = holdXP < energydifXP ? holdXP : (energydifXP * 9 + holdXP) / 10;

                    //inverse handycap
                    energydifXP2 = (energydifX2 - energydif2) / LFOR;
                    if (energydifXP2 > 0)
                        energydifXP2 = 0;

                    if ((energydifXP - energydifXP2) > 0.1)
                        energydifXP2 = energydifXP - 0.1;

                    energydifX2 = energydif2;
                    energydif2 = 0;
                }
                energydifX = energydif / ModeChangeCycles;
                energydif = 0;

                if (hidepred)
                {
                    //Erase offensive shots
                    foreach (var shot in Shots.Where(s => s.shottype == -1 || s.shottype == -6))
                    {
                        shot.exist = false;
                        shot.flash = false;
                    }

                    //Reposition robots the safe way
                    var k2 = 0; //robots moved total
                    int k;

                    do
                    {
                        k = 0;
                        foreach (var tRob in rob.Where(r => r.exist && r.FName == "Base.txt"))
                        {
                            foreach (var iRob in rob.Where(r => r.exist && r.FName == "Mutate.txt"))
                            {
                                double ingdist = 0;

                                if (tRob.body > iRob.body)
                                    ingdist = tRob.body > 10 ? Math.Log(tRob.body) * 60 + 41 : 40;
                                else
                                    ingdist = iRob.body > 10 ? Math.Log(iRob.body) * 60 + 41 : 40;

                                ingdist += tRob.radius + iRob.radius + 40; //both radii plus shot dist plus offset 1 shot travel dist

                                var posdif = tRob.pos - iRob.pos;
                                if (posdif.Magnitude() < ingdist)
                                {
                                    //if the distance between the robots is less then ingagment distance
                                    ingdist -= posdif.Magnitude(); //ingdist becomes offset dist
                                    var newpoz = iRob.pos - posdif.Unit() * ingdist;
                                    var pozdif = newpoz - iRob.pos;

                                    if (iRob.Ties.Count > 0 && iRob.FName == "Mutate.txt")
                                    {
                                        foreach (var cell in ListCells(iRob))
                                        {
                                            //Botsareus 7/15/2016 Only own species
                                            iRob.pos.X += pozdif.X;
                                            iRob.pos.Y += pozdif.Y;
                                        }
                                    }

                                    iRob.pos += pozdif;
                                    k++;
                                    k2++;
                                }
                            }
                        }
                    } while (!(k == 0 || k2 > (3200 + Mutate_count * 0.9m))); //Scales as mutate_count scales

                    //change hide pred
                    hidepred = !hidepred;
                    hidePredOffset = (int)(hidePredCycl / 3 * ThreadSafeRandom.Local.NextDouble());
                    ModeChangeCycles = 0;
                }
            }

            if (SimOpts.MutOscill && (SimOpts.MutCycMax + SimOpts.MutCycMin) > 0)
            {
                if (SimOpts.MutOscillSine)
                {
                    var fullrange = SimOpts.TotRunCycle % (SimOpts.MutCycMax + SimOpts.MutCycMin);

                    SimOpts.MutCurrMult = fullrange < SimOpts.MutCycMax
                        ? Math.Pow(20, Math.Sin(fullrange / SimOpts.MutCycMax * Math.PI))
                        : Math.Pow(20, Math.Sin((fullrange - SimOpts.MutCycMax) / SimOpts.MutCycMin * Math.PI));
                }
                else
                    SimOpts.MutCurrMult = SimOpts.TotRunCycle % (SimOpts.MutCycMax + SimOpts.MutCycMin) < SimOpts.MutCycMax ? 16 : 1 / 16;
            }

            TotalSimEnergyDisplayed = TotalSimEnergy[CurrentEnergyCycle];
            CurrentEnergyCycle = SimOpts.TotRunCycle % 100;
            TotalSimEnergy[CurrentEnergyCycle] = 0;

            var CurrentPopulation = totnvegsDisplayed;

            if (SimOpts.Costs[DYNAMICCOSTINCLUDEPLANTS] != 0)
                CurrentPopulation += totvegsDisplayed; //Include Plants in target population

            //If (SimOpts.TotRunCycle + 200) Mod 2000 = 0 Then MsgBox "sup" & SimOpts.TotRunCycle 'debug only

            if (SimOpts.TotRunCycle % 10 == 0)
            {
                for (var i = 10; i >= 2; i--)
                    PopulationLast10Cycles[i] = PopulationLast10Cycles[i - 1];

                PopulationLast10Cycles[1] = CurrentPopulation;
            }

            if (SimOpts.Costs[USEDYNAMICCOSTS] > 0)
            {
                var AmountOff = CurrentPopulation - SimOpts.Costs[DYNAMICCOSTTARGET];

                //If we are more than X% off of our target population either way AND the population isn't moving in the
                //the direction we want or hasn't moved at all in the past 10 cycles then adjust the cost multiplier
                var UpperRange = TmpOpts.Costs[DYNAMICCOSTTARGETUPPERRANGE] * 0.01 * SimOpts.Costs[DYNAMICCOSTTARGET];
                var LowerRange = TmpOpts.Costs[DYNAMICCOSTTARGETLOWERRANGE] * 0.01 * SimOpts.Costs[DYNAMICCOSTTARGET];

                if (CurrentPopulation == PopulationLast10Cycles[10])
                {
                    DynamicCountdown--;
                    if (DynamicCountdown < -10)
                        DynamicCountdown = -10;
                }
                else
                    DynamicCountdown = 10;

                if ((AmountOff > UpperRange && (PopulationLast10Cycles[10] < CurrentPopulation || DynamicCountdown <= 0)) || (AmountOff < -LowerRange && (PopulationLast10Cycles[10] > CurrentPopulation || DynamicCountdown <= 0)))
                {
                    var CorrectionAmount = AmountOff > UpperRange ? AmountOff - UpperRange : Math.Abs(AmountOff) - LowerRange;

                    //Adjust the multiplier. The idea is to rachet this over time as bots evolve to be more effecient.
                    //We don't muck with it if the bots are within X% of the target.  If they are outside the target, then
                    //we adjust only if the populatiuon isn't heading towards the range and then we do it my an amount that is a function
                    //of how far of the range we are (not how far from the target itself) and the sensitivity set in the sim
                    SimOpts.Costs[COSTMULTIPLIER] = SimOpts.Costs[COSTMULTIPLIER] + (0.0000001 * CorrectionAmount * Math.Sign(AmountOff) * SimOpts.Costs[DYNAMICCOSTSENSITIVITY]);

                    //Don't let the costs go negative if the user doesn't want them to
                    if ((SimOpts.Costs[ALLOWNEGATIVECOSTX] != 1) && SimOpts.Costs[COSTMULTIPLIER] < 0)
                        SimOpts.Costs[COSTMULTIPLIER] = 0;

                    DynamicCountdown = 10; // Reset the countdown timer
                }
            }

            if ((CurrentPopulation < SimOpts.Costs[BOTNOCOSTLEVEL]) && (SimOpts.Costs[COSTMULTIPLIER] != 0))
            {
                CostsWereZeroed = true;
                SimOpts.OldCostX = SimOpts.Costs[COSTMULTIPLIER];
                SimOpts.Costs[COSTMULTIPLIER] = 0; // The population has fallen below the threshold to 0 all costs
            }
            else if ((CurrentPopulation > SimOpts.Costs[COSTXREINSTATEMENTLEVEL]) && CostsWereZeroed)
            {
                CostsWereZeroed = false; // Set the flag so we don't do this again unless they get zeored again
                SimOpts.Costs[COSTMULTIPLIER] = SimOpts.OldCostX;
            }

            if (hidepred)
            {
                //Store new energy handycap
                foreach (var rob in rob.Where(r => r.exist && r.FName == "Mutate.txt"))
                    rob.nrg += rob.LastMut > 0 ? CalculateHandycap() : CalculateHandycap() / 2;
            }

            //core evo
            var avrnrgStart = 0.0;

            if (usehidepred)
                avrnrgStart = rob.Where(r => r.exist && r.FName == "Mutate.txt" && r.LastMut > 0).Average(s => s.nrg);

            ExecRobs();

            //updateshots can write to bot sense, so we need to clear bot senses before updating shots
            foreach (var rob in rob.Where(r => r.exist && r.DisableDNA == false))
                EraseSenses(rob);

            UpdateShots();

            //Botsareus 6/22/2016 to figure actual velocity of the bot incase there is a collision event
            foreach (var rob in rob.Where(r => r.exist && !(r.FName == "Base.txt" && hidepred)))
                rob.opos = rob.pos;

            await UpdateBots();

            //to figure actual velocity of the bot incase there is a collision event
            foreach (var rob in rob.Where(r => r.exist && !(r.FName == "Base.txt" && hidepred) && !(r.opos.X == 0 & r.opos.Y == 0)))
                rob.actvel = rob.pos - rob.opos;

            if (Obstacles.Count > 0)
                MoveObstacles();

            if (Teleporters.Count > 0)
                UpdateTeleporters();

            var AllChlr = (int)rob.Where(r => r.exist && !(r.FName == "Base.txt" && hidepred)).Sum(r => r.chloroplasts);

            TotalChlr = AllChlr / 16000; //Panda 8/23/2013 Calculate total unit chloroplasts

            if (TotalChlr < SimOpts.MinVegs && totvegsDisplayed != -1)
                await VegsRepopulate(); //Will be -1 first cycle after loading a sim.  Prevents spikes.

            feedvegs(SimOpts.MaxEnergy);

            if (usehidepred)
            {
                var avrnrgEnd = rob.Where(r => r.exist && r.FName == "Mutate.txt" && r.LastMut > 0).Average(s => s.nrg);
                energydif = energydif - avrnrgStart + avrnrgEnd;
            }

            //Kill some robots to prevent of memory
            var totlen = rob.Where(r => r.exist).Sum(r => r.dna.Count);

            if (totlen > 4000000)
            {
                var maxdel = 1500 * (TotalRobotsDisplayed * 425 / totlen);

                foreach (var rob in rob.Where(r => r.exist).OrderByDescending(r => r.nrg + r.body * 10).Take(maxdel).ToArray())
                    await KillRobot(rob);
            }
            if (totlen > 3000000)
            {
                foreach (var rob in rob)
                    rob.LastMutDetail = "";
            }

            if (UseSafeMode
                && (UseIntRnd ? savenow : SimOpts.TotRunCycle % 2000 == 0 && SimOpts.TotRunCycle > 0)
                && (x_restartmode == 0 || x_restartmode == 4 || x_restartmode == 5 || x_restartmode == 7 || x_restartmode == 8))
            {
                SaveSimulation(@"saves\lastautosave.sim");

                if (File.Exists(@"saves\localcopy.sim"))
                    File.Delete(@"saves\localcopy.sim");

                await AutoSaved.Save(true);

                savenow = false;
            }

            if (x_restartmode == 1 && SimOpts.TotRunCycle == 2000)
            {
                File.Copy("league\\Test.txt", NamefileRecursive($"league\\seeded\\{totnvegsDisplayed}.txt"));
                await RestartMode.Save(x_restartmode, x_filenumber);
                Restarter();
            }

            if (x_restartmode == 7 || x_restartmode == 8)
            {
                if (SimOpts.TotRunCycle % 50 == 0 & SimOpts.TotRunCycle > 0)
                    Form1.instance.fittest();

                if (rob.Any(r => r.exist && r.FName == "Mutate.txt"))
                {
                    //Restart
                    await LogEvolution("A restart is needed.");

                    await SafeMode.Save(false);
                    await AutoSaved.Save(false);

                    Restarter();
                }
            }

            double cmptotnrgnvegs = 0;

            //test mode
            if (x_restartmode == 9 && SimOpts.TotRunCycle == 1)
                totnrgnvegs = rob.Where(r => r.exist && r.FName == "Test.txt").Sum(r => r.nrg + 10 * r.body);

            if (SimOpts.TotRunCycle == 8000)
            {
                cmptotnrgnvegs = rob.Where(r => r.exist && r.FName == "Test.txt").Sum(r => r.nrg + 10 * r.body);

                if (totnvegsDisplayed > 10 & cmptotnrgnvegs > totnrgnvegs * 2)
                    ZBPassedTest();
                else
                    await ZBFailedTest();
            }
        }
    }
}
