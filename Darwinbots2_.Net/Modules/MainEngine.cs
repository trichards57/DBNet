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
        private BucketManager _bucketManager;
        private bool Active;
        private Thread simThread;
        private CancellationTokenSource simThreadCancel;

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

            DNATokenizing.LoadSysVars();

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
                Robots.rob.Clear();
                ShotsManager.Shots.Clear();
                ObstaclesManager.Obstacles.Clear();
            }

            Globals.BucketManager = _bucketManager;

            ObstaclesManager.defaultHeight = 0.2;
            ObstaclesManager.defaultWidth = 0.2;

            if (!startLoaded)
                LoadRobots();

            // TODO : Trigger a screen update

            if (!startLoaded)
            {
                Globals.strSimStart = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);

                foreach (var o in Globals.xObstacle)
                {
                    var newO = ObstaclesManager.NewObstacle(o.pos.X, o.pos.Y, o.Width, o.Height);
                    newO.color = o.color;
                    newO.vel = o.vel;
                }
            }
            else
            {
                Globals.NoDeaths = true;
                Vegs.cooldown = -SimOpt.SimOpts.RepopCooldown;
                Globals.totnvegsDisplayed = -1;
                Vegs.totvegs = -1;
                Globals.totnvegs = SimOpt.SimOpts.Costs.DynamicCostsTargetPopulation;
            }

            Active = true;

            Main();
        }

        // TODO : Graphing statistics

        private void LoadRobots()
        {
            throw new NotImplementedException();
        }

        private void Main()
        {
            simThread = new Thread(MainFunction);
            simThreadCancel = new CancellationTokenSource();

            simThread.Start(simThreadCancel.Token);
        }

        // TODO : Drag and drop robots

        private async void MainFunction(object state)
        {
            var cancelToken = (CancellationToken)state;

            while (!cancelToken.IsCancellationRequested)
            {
                if (Active)
                {
                    await Master.UpdateSim();

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
    }
}
