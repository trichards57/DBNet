using DarwinBots.DataModel;
using DarwinBots.Model;
using DarwinBots.Support;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DarwinBots.Modules
{
    internal class MainEngine
    {
        private readonly int[] _populationLast10Cycles = new int[10];
        private bool _active;
        private BucketManager _bucketManager;
        private bool _costsWereZeroed;
        private int _dynamicCountdown;
        private ObstaclesManager _obstacleManager;
        private Thread _simThread;
        private CancellationTokenSource _simThreadCancel;

        public robot RobotAtPoint(DoubleVector point)
        {
            return Robots.rob
                 .Where(r => r.exist)
                 .Select(r => new
                 {
                     Robot = r,
                     Distance = (r.pos - r.vel + r.actvel - point).MagnitudeSquare()
                 })
                 .Where(r => r.Distance < r.Robot.radius * r.Robot.radius)
                 .OrderByDescending(r => r.Distance)
                 .FirstOrDefault()?.Robot;
        }

        public async Task StartSimulation(bool startLoaded = false)
        {
            // TODO : Save last run settings

            DnaEngine.LoadSysVars();

            await AutoSaved.Save(false);
            await SafeMode.Save(true);

            if (SimOpt.SimOpts.SunOnRnd)
            {
                Vegs.SunRange = 0.5;
                Vegs.SunChange = ThreadSafeRandom.Local.Next(3) + ThreadSafeRandom.Local.Next(2) * 10;
                Vegs.SunPosition = ThreadSafeRandom.Local.NextDouble();
            }
            else
            {
                Vegs.SunPosition = 0.5;
                Vegs.SunRange = 1;
            }

            if (!startLoaded)
            {
                SimOpt.SimOpts.SimGUID = Guid.NewGuid();
                SimOpt.SimOpts.TotBorn = 0;
            }

            ShotsManager.MaxBotShotSeperation = Math.Pow(Robots.FindRadius(null, -1), 2) +
                                                Math.Pow(SimOpt.SimOpts.MaxVelocity * 2 + Robots.RobSize / 3.0, 2);

            if (!startLoaded)
            {
                _bucketManager = new BucketManager(SimOpt.SimOpts);
                _obstacleManager = new ObstaclesManager();
                Robots.rob.Clear();
                ShotsManager.Shots.Clear();
            }

            Globals.BucketManager = _bucketManager;
            Globals.ObstacleManager = _obstacleManager;

            _obstacleManager.DefaultHeight = 0.2;
            _obstacleManager.DefaultWidth = 0.2;

            if (!startLoaded)
                LoadRobots();

            // TODO : Trigger a screen update

            if (!startLoaded)
            {
                Globals.SimStart = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);

                foreach (var o in Globals.xObstacle)
                {
                    var newO = _obstacleManager.NewObstacle(o.pos.X, o.pos.Y, o.Width, o.Height);
                    newO.color = o.color;
                    newO.vel = o.vel;
                }
            }
            else
            {
                Vegs.cooldown = -SimOpt.SimOpts.RepopCooldown;
                Globals.TotalNotVegsDisplayed = -1;
                Vegs.totvegs = -1;
                Globals.TotalNotVegs = SimOpt.SimOpts.Costs.DynamicCostsTargetPopulation;
            }

            _active = true;

            Main();
        }

        private void LoadRobots()
        {
            throw new NotImplementedException();
        }

        // TODO : Graphing statistics
        private void Main()
        {
            _simThread = new Thread(MainFunction);
            _simThreadCancel = new CancellationTokenSource();

            _simThread.Start(_simThreadCancel.Token);
        }

        private async void MainFunction(object state)
        {
            var cancelToken = (CancellationToken)state;

            while (!cancelToken.IsCancellationRequested)
            {
                if (_active)
                {
                    await UpdateSim();

                    // TODO : Handle showing data for the selected robot
                    // TODO : Handle signalling the window to draw
                    // TODO : Handle selected robot logic
                    // TODO : Handle saving the sim on error
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        private async Task UpdateSim()
        {
            Globals.ModeChangeCycles++;
            SimOpt.SimOpts.TotRunCycle++;

            if (SimOpt.SimOpts.MutOscill && (SimOpt.SimOpts.MutCycMax + SimOpt.SimOpts.MutCycMin) > 0)
            {
                if (SimOpt.SimOpts.MutOscillSine)
                {
                    var fullRange = SimOpt.SimOpts.TotRunCycle % (SimOpt.SimOpts.MutCycMax + SimOpt.SimOpts.MutCycMin);

                    SimOpt.SimOpts.MutCurrMult = fullRange < SimOpt.SimOpts.MutCycMax
                        ? Math.Pow(20, Math.Sin((double)fullRange / SimOpt.SimOpts.MutCycMax * Math.PI))
                        : Math.Pow(20, Math.Sin((double)(fullRange - SimOpt.SimOpts.MutCycMax) / SimOpt.SimOpts.MutCycMin * Math.PI));
                }
                else
                    SimOpt.SimOpts.MutCurrMult = SimOpt.SimOpts.TotRunCycle % (SimOpt.SimOpts.MutCycMax + SimOpt.SimOpts.MutCycMin) < SimOpt.SimOpts.MutCycMax ? 16 : 1 / 16;
            }

            Vegs.TotalSimEnergyDisplayed = Vegs.TotalSimEnergy[Vegs.CurrentEnergyCycle];
            Vegs.CurrentEnergyCycle = SimOpt.SimOpts.TotRunCycle % 100;
            Vegs.TotalSimEnergy[Vegs.CurrentEnergyCycle] = 0;

            var currentPopulation = Globals.TotalNotVegsDisplayed;

            if (SimOpt.SimOpts.Costs.DynamicCostsIncludePlants)
                currentPopulation += Vegs.totvegsDisplayed; //Include Plants in target population

            //If (SimOpts.TotRunCycle + 200) Mod 2000 = 0 Then MsgBox "sup" & SimOpts.TotRunCycle 'debug only

            if (SimOpt.SimOpts.TotRunCycle % 10 == 0)
            {
                for (var i = 10; i >= 2; i--)
                    _populationLast10Cycles[i] = _populationLast10Cycles[i - 1];

                _populationLast10Cycles[1] = currentPopulation;
            }

            if (SimOpt.SimOpts.Costs.EnableDynamicCosts)
            {
                var amountOff = currentPopulation - SimOpt.SimOpts.Costs.DynamicCostsTargetPopulation;

                //If we are more than X% off of our target population either way AND the population isn't moving in the
                //the direction we want or hasn't moved at all in the past 10 cycles then adjust the cost multiplier
                var upperRange = SimOpt.TmpOpts.Costs.DynamicCostsUpperRangeTarget * 0.01 * SimOpt.SimOpts.Costs.DynamicCostsTargetPopulation;
                var lowerRange = SimOpt.TmpOpts.Costs.DynamicCostsLowerRangeTarget * 0.01 * SimOpt.SimOpts.Costs.DynamicCostsTargetPopulation;

                if (currentPopulation == _populationLast10Cycles[10])
                {
                    _dynamicCountdown--;
                    if (_dynamicCountdown < -10)
                        _dynamicCountdown = -10;
                }
                else
                    _dynamicCountdown = 10;

                if ((amountOff > upperRange && (_populationLast10Cycles[10] < currentPopulation || _dynamicCountdown <= 0)) || (amountOff < -lowerRange && (_populationLast10Cycles[10] > currentPopulation || _dynamicCountdown <= 0)))
                {
                    var correctionAmount = amountOff > upperRange ? amountOff - upperRange : Math.Abs(amountOff) - lowerRange;

                    //Adjust the multiplier. The idea is to rachet this over time as bots evolve to be more effecient.
                    //We don't muck with it if the bots are within X% of the target.  If they are outside the target, then
                    //we adjust only if the populatiuon isn't heading towards the range and then we do it my an amount that is a function
                    //of how far of the range we are (not how far from the target itself) and the sensitivity set in the sim
                    SimOpt.SimOpts.Costs = SimOpt.SimOpts.Costs with { CostMultiplier = SimOpt.SimOpts.Costs.CostMultiplier + (0.0000001 * correctionAmount * Math.Sign(amountOff) * SimOpt.SimOpts.Costs.DynamicCostsSensitivity) };

                    //Don't let the costs go negative if the user doesn't want them to
                    if (!SimOpt.SimOpts.Costs.AllowMultiplerToGoNegative && SimOpt.SimOpts.Costs.CostMultiplier < 0)
                        SimOpt.SimOpts.Costs = SimOpt.SimOpts.Costs with { CostMultiplier = 0 };

                    _dynamicCountdown = 10; // Reset the countdown timer
                }
            }

            if (currentPopulation < SimOpt.SimOpts.Costs.ZeroCostPopulationLimit && SimOpt.SimOpts.Costs.CostMultiplier != 0)
            {
                _costsWereZeroed = true;
                SimOpt.SimOpts.OldCostX = SimOpt.SimOpts.Costs.CostMultiplier;
                SimOpt.SimOpts.Costs = SimOpt.SimOpts.Costs with { CostMultiplier = 0 }; // The population has fallen below the threshold to 0 all costs
            }
            else if (currentPopulation > SimOpt.SimOpts.Costs.ReinstateCostPopulationLimit && _costsWereZeroed)
            {
                _costsWereZeroed = false; // Set the flag so we don't do this again unless they get zeored again
                SimOpt.SimOpts.Costs = SimOpt.SimOpts.Costs with { CostMultiplier = SimOpt.SimOpts.OldCostX };
            }

            DnaEngine.ExecRobs(SimOpt.SimOpts.Costs, Robots.rob);

            //updateshots can write to bot sense, so we need to clear bot senses before updating shots
            foreach (var rob in Robots.rob.Where(r => r.exist && r.DisableDNA == false))
                Senses.EraseSenses(rob);

            ShotsManager.UpdateShots();

            //Botsareus 6/22/2016 to figure actual velocity of the bot incase there is a collision event
            foreach (var rob in Robots.rob.Where(r => r.exist))
                rob.opos = rob.pos;

            await Robots.UpdateBots();

            //to figure actual velocity of the bot incase there is a collision event
            foreach (var rob in Robots.rob.Where(r => r.exist && !(r.opos.X == 0 & r.opos.Y == 0)))
                rob.actvel = rob.pos - rob.opos;

            if (_obstacleManager.Obstacles.Count > 0)
                _obstacleManager.MoveObstacles();

            if (Teleport.Teleporters.Count > 0)
                Teleport.UpdateTeleporters();

            var allChlr = (int)Robots.rob.Where(r => r.exist).Sum(r => r.chloroplasts);

            Globals.TotalChlr = allChlr / 16000; //Panda 8/23/2013 Calculate total unit chloroplasts

            if (Globals.TotalChlr < SimOpt.SimOpts.MinVegs && Vegs.totvegsDisplayed != -1)
                await Vegs.VegsRepopulate(); //Will be -1 first cycle after loading a sim.  Prevents spikes.

            Vegs.feedvegs(SimOpt.SimOpts.MaxEnergy);

            //Kill some robots to prevent of memory
            var totlen = Robots.rob.Where(r => r.exist).Sum(r => r.dna.Count);

            if (totlen > 4000000)
            {
                var maxdel = 1500 * (Robots.TotalRobotsDisplayed * 425 / totlen);

                foreach (var rob in Robots.rob.Where(r => r.exist).OrderByDescending(r => r.nrg + r.body * 10).Take(maxdel).ToArray())
                    await Robots.KillRobot(rob);
            }
            if (totlen > 3000000)
            {
                foreach (var rob in Robots.rob)
                    rob.LastMutDetail = "";
            }
        }

        // TODO : Drag and drop robots
    }
}
