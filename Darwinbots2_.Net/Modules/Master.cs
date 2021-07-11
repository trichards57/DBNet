using System;
using System.Linq;
using System.Threading.Tasks;
using static DarwinBots.Modules.Globals;
using static DarwinBots.Modules.ObstaclesManager;
using static DarwinBots.Modules.Robots;
using static DarwinBots.Modules.Senses;
using static DarwinBots.Modules.ShotsManager;
using static DarwinBots.Modules.SimOpt;
using static DarwinBots.Modules.Teleport;
using static DarwinBots.Modules.Vegs;

namespace DarwinBots.Modules
{
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

        public static async Task UpdateSim()
        {
            ModeChangeCycles++;
            SimOpts.TotRunCycle++;

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

            var CurrentPopulation = TotalNotVegsDisplayed;

            if (SimOpts.Costs.DynamicCostsIncludePlants)
                CurrentPopulation += totvegsDisplayed; //Include Plants in target population

            //If (SimOpts.TotRunCycle + 200) Mod 2000 = 0 Then MsgBox "sup" & SimOpts.TotRunCycle 'debug only

            if (SimOpts.TotRunCycle % 10 == 0)
            {
                for (var i = 10; i >= 2; i--)
                    PopulationLast10Cycles[i] = PopulationLast10Cycles[i - 1];

                PopulationLast10Cycles[1] = CurrentPopulation;
            }

            if (SimOpts.Costs.EnableDynamicCosts)
            {
                var AmountOff = CurrentPopulation - SimOpts.Costs.DynamicCostsTargetPopulation;

                //If we are more than X% off of our target population either way AND the population isn't moving in the
                //the direction we want or hasn't moved at all in the past 10 cycles then adjust the cost multiplier
                var UpperRange = TmpOpts.Costs.DynamicCostsUpperRangeTarget * 0.01 * SimOpts.Costs.DynamicCostsTargetPopulation;
                var LowerRange = TmpOpts.Costs.DynamicCostsLowerRangeTarget * 0.01 * SimOpts.Costs.DynamicCostsTargetPopulation;

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
                    SimOpts.Costs = SimOpts.Costs with { CostMultiplier = SimOpts.Costs.CostMultiplier + (0.0000001 * CorrectionAmount * Math.Sign(AmountOff) * SimOpts.Costs.DynamicCostsSensitivity) };

                    //Don't let the costs go negative if the user doesn't want them to
                    if (!SimOpts.Costs.AllowMultiplerToGoNegative && SimOpts.Costs.CostMultiplier < 0)
                        SimOpts.Costs = SimOpts.Costs with { CostMultiplier = 0 };

                    DynamicCountdown = 10; // Reset the countdown timer
                }
            }

            if (CurrentPopulation < SimOpts.Costs.ZeroCostPopulationLimit && SimOpts.Costs.CostMultiplier != 0)
            {
                CostsWereZeroed = true;
                SimOpts.OldCostX = SimOpts.Costs.CostMultiplier;
                SimOpts.Costs = SimOpts.Costs with { CostMultiplier = 0 }; // The population has fallen below the threshold to 0 all costs
            }
            else if (CurrentPopulation > SimOpts.Costs.ReinstateCostPopulationLimit && CostsWereZeroed)
            {
                CostsWereZeroed = false; // Set the flag so we don't do this again unless they get zeored again
                SimOpts.Costs = SimOpts.Costs with { CostMultiplier = SimOpts.OldCostX };
            }

            DnaEngine.ExecRobs(SimOpts.Costs, rob);

            //updateshots can write to bot sense, so we need to clear bot senses before updating shots
            foreach (var rob in rob.Where(r => r.exist && r.DisableDNA == false))
                EraseSenses(rob);

            UpdateShots();

            //Botsareus 6/22/2016 to figure actual velocity of the bot incase there is a collision event
            foreach (var rob in rob.Where(r => r.exist))
                rob.opos = rob.pos;

            await UpdateBots();

            //to figure actual velocity of the bot incase there is a collision event
            foreach (var rob in rob.Where(r => r.exist && !(r.opos.X == 0 & r.opos.Y == 0)))
                rob.actvel = rob.pos - rob.opos;

            if (Obstacles.Count > 0)
                MoveObstacles();

            if (Teleporters.Count > 0)
                UpdateTeleporters();

            var AllChlr = (int)rob.Where(r => r.exist).Sum(r => r.chloroplasts);

            TotalChlr = AllChlr / 16000; //Panda 8/23/2013 Calculate total unit chloroplasts

            if (TotalChlr < SimOpts.MinVegs && totvegsDisplayed != -1)
                await VegsRepopulate(); //Will be -1 first cycle after loading a sim.  Prevents spikes.

            feedvegs(SimOpts.MaxEnergy);

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
        }
    }
}
