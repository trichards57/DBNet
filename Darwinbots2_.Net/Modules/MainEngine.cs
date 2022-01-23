﻿using DarwinBots.DataModel;
using DarwinBots.Forms;
using DarwinBots.Model;
using DarwinBots.Model.Display;
using DarwinBots.Support;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
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
        private RobotsManager _robotsManager;
        private ShotsManager _shotsManager;
        private Thread _simThread;
        private CancellationTokenSource _simThreadCancel;

        // TODO : Save last run settings
        // TODO : Graphing statistics
        // TODO : Handle showing data for the selected robot
        // TODO : Handle signalling the window to draw
        // TODO : Handle selected robot logic
        // TODO : Handle saving the sim on error
        // TODO : Drag and drop robots
        // TODO : Triggering screen updates

        public event EventHandler<UpdateAvailableArgs> UpdateAvailable;

        /// <summary>
        /// Adds a record to the species array when a bot with a new species is loaded or teleported in.
        /// </summary>
        public static void AddSpecie(Robot rob, bool isNative)
        {
            if (rob.IsCorpse || rob.FName == "Corpse" || rob.Exists == false)
                return;

            if (SimOpt.SimOpts.Specie.Count >= SimOpt.MaxNativeSpecies)
                return;

            var d = new Species
            {
                Name = rob.FName,
                Veg = rob.IsVegetable,
                CantSee = rob.CantSee,
                DisableMovementSysvars = rob.MovementSysvarsDisabled,
                DisableDna = rob.DnaDisabled,
                CantReproduce = rob.CantReproduce,
                VirusImmune = rob.IsVirusImmune,
                Population = 1,
                SubSpeciesCounter = 0,
                Color = rob.Color,
                Comment = "Species arrived from the Internet",
                Posrg = 1,
                Posdn = 1,
                Poslf = 0,
                Postp = 0,
                Quantity = 5,
                Stnrg = 3000,
                Native = isNative,
                Path = "robots",
            };

            d.Mutables.ResetToDefault();
            d.Mutables.EnableMutations = rob.MutationProbabilities.EnableMutations;

            SimOpt.SimOpts.Specie.Add(d);
        }

        public async Task LoadSimulation(string path)
        {
            var input = await File.ReadAllTextAsync(path);
            var savedFile = JsonSerializer.Deserialize<SavedSimulation>(input);

            if (savedFile == null)
                return;

            _robotsManager.Robots.Clear();
            _robotsManager.Robots.AddRange(savedFile.Robots);
            SimOpt.Species.Clear();
            SimOpt.Species.AddRange(savedFile.Species);
            _obstacleManager.Obstacles.Clear();
            _obstacleManager.Obstacles.AddRange(savedFile.Obstacles);
            _shotsManager.Shots.Clear();
            _shotsManager.Shots.AddRange(savedFile.Shots);

            SimOpt.SimOpts.Costs = savedFile.Costs;
            SimOpt.SimOpts.DeadRobotSnp = savedFile.DeadRobotSnp;
            SimOpt.SimOpts.SnpExcludeVegs = savedFile.SnpExcludeVegs;
            SimOpt.SimOpts.DisableTies = savedFile.DisableTies;
            SimOpt.SimOpts.EnergyExType = savedFile.EnergyExType;
            SimOpt.SimOpts.EnergyFix = savedFile.EnergyFix;
            SimOpt.SimOpts.EnergyProp = savedFile.EnergyProp;
            SimOpt.SimOpts.FieldHeight = savedFile.FieldHeight;
            SimOpt.SimOpts.FieldWidth = savedFile.FieldWidth;
            SimOpt.SimOpts.MaxEnergy = savedFile.MaxEnergy;
            SimOpt.SimOpts.MaxPopulation = savedFile.MaxPopulation;
            SimOpt.SimOpts.MinVegs = savedFile.MinVegs;
            SimOpt.SimOpts.MutCurrMult = savedFile.MutCurrMult;
            SimOpt.SimOpts.MutCycMax = savedFile.MutCycMax;
            SimOpt.SimOpts.MutCycMin = savedFile.MutCycMin;
            SimOpt.SimOpts.MutOscill = savedFile.MutOscill;
            SimOpt.SimOpts.PhysBrown = savedFile.PhysBrown;
            SimOpt.SimOpts.YGravity = savedFile.Ygravity;
            SimOpt.SimOpts.ZGravity = savedFile.Zgravity;
            SimOpt.SimOpts.PhysMoving = savedFile.PhysMoving;
            SimOpt.SimOpts.TotBorn = savedFile.TotBorn;
            SimOpt.SimOpts.TotRunCycle = savedFile.TotRunCycle;
            SimOpt.SimOpts.PondMode = savedFile.Pondmode;
            SimOpt.SimOpts.CorpseEnabled = savedFile.CorpseEnabled;
            SimOpt.SimOpts.LightIntensity = savedFile.LightIntensity;
            SimOpt.SimOpts.Decay = savedFile.Decay;
            SimOpt.SimOpts.Gradient = savedFile.Gradient;
            SimOpt.SimOpts.DayNight = savedFile.DayNight;
            SimOpt.SimOpts.CycleLength = savedFile.CycleLength;
            SimOpt.SimOpts.DecayDelay = savedFile.DecayDelay;
            SimOpt.SimOpts.DecayType = savedFile.DecayType;
            SimOpt.SimOpts.DxSxConnected = savedFile.DxSxConnected;
            SimOpt.SimOpts.UpDnConnected = savedFile.UpDnConnected;
            SimOpt.SimOpts.RepopAmount = savedFile.RepopAmount;
            SimOpt.SimOpts.RepopCooldown = savedFile.RepopCooldown;
            SimOpt.SimOpts.ZeroMomentum = savedFile.ZeroMomentum;
            SimOpt.SimOpts.VegFeedingToBody = savedFile.VegFeedingToBody;
            SimOpt.SimOpts.CoefficientStatic = savedFile.CoefficientStatic;
            SimOpt.SimOpts.CoefficientKinetic = savedFile.CoefficientKinetic;
            SimOpt.SimOpts.PlanetEaters = savedFile.PlanetEaters;
            SimOpt.SimOpts.PlanetEatersG = savedFile.PlanetEatersG;
            SimOpt.SimOpts.Viscosity = savedFile.Viscosity;
            SimOpt.SimOpts.Density = savedFile.Density;
            SimOpt.SimOpts.BadWasteLevel = savedFile.BadWastelevel;
            SimOpt.SimOpts.CoefficientElasticity = savedFile.CoefficientElasticity;
            SimOpt.SimOpts.FluidSolidCustom = savedFile.FluidSolidCustom;
            SimOpt.SimOpts.MaxVelocity = savedFile.MaxVelocity;
            SimOpt.SimOpts.NoShotDecay = savedFile.NoShotDecay;
            SimOpt.SimOpts.SunUpThreshold = savedFile.SunUpThreshold;
            SimOpt.SimOpts.SunUp = savedFile.SunUp;
            SimOpt.SimOpts.SunDownThreshold = savedFile.SunDownThreshold;
            SimOpt.SimOpts.SunDown = savedFile.SunDown;
            SimOpt.SimOpts.FixedBotRadii = savedFile.FixedBotRadii;
            SimOpt.SimOpts.DayNightCycleCounter = savedFile.DayNightCycleCounter;
            SimOpt.SimOpts.Daytime = savedFile.Daytime;
            SimOpt.SimOpts.SunThresholdMode = savedFile.SunThresholdMode;
            SimOpt.SimOpts.ShapesAreVisable = savedFile.ShapesAreVisable;
            SimOpt.SimOpts.AllowVerticalShapeDrift = savedFile.AllowVerticalShapeDrift;
            SimOpt.SimOpts.AllowHorizontalShapeDrift = savedFile.AllowHorizontalShapeDrift;
            SimOpt.SimOpts.ShapesAreSeeThrough = savedFile.ShapesAreSeeThrough;
            SimOpt.SimOpts.ShapesAbsorbShots = savedFile.ShapesAbsorbShots;
            SimOpt.SimOpts.ShapeDriftRate = savedFile.ShapeDriftRate;
            SimOpt.SimOpts.MakeAllShapesBlack = savedFile.MakeAllShapesBlack;
            SimOpt.SimOpts.MaxAbsNum = savedFile.MaxAbsNum;
            SimOpt.SimOpts.OldCostX = savedFile.OldCostX;
            SimOpt.SimOpts.DisableMutations = savedFile.DisableMutations;
            SimOpt.SimOpts.SimGuid = savedFile.SimGuid;
            SimOpt.SimOpts.SpeciationGeneticDistance = savedFile.SpeciationGeneticDistance;
            SimOpt.SimOpts.EnableAutoSpeciation = savedFile.EnableAutoSpeciation;
            SimOpt.SimOpts.SpeciationForkInterval = savedFile.SpeciationForkInterval;
            SimOpt.SimOpts.DisableTypArepro = savedFile.DisableTypArepro;
            SimOpt.SimOpts.NoWShotDecay = savedFile.NoWShotDecay;
            SimOpt.SimOpts.SunOnRnd = savedFile.SunOnRnd;
            SimOpt.SimOpts.DisableFixing = savedFile.DisableFixing;
            Vegs.SunPosition = savedFile.SunPosition;
            Vegs.SunRange = savedFile.SunRange;
            Vegs.SunChange = savedFile.SunChange;
            SimOpt.SimOpts.Tides = savedFile.Tides;
            SimOpt.SimOpts.TidesOf = savedFile.TidesOf;
            SimOpt.SimOpts.MutOscillSine = savedFile.MutOscillSine;
        }

        public Robot RobotAtPoint(DoubleVector point)
        {
            return _robotsManager.Robots
                 .Where(r => r.Exists)
                 .Select(r => new
                 {
                     Robot = r,
                     Distance = (r.OffsetPosition - point).MagnitudeSquare()
                 })
                 .Where(r => r.Distance < r.Robot.GetRadius(SimOpt.SimOpts.FixedBotRadii) * r.Robot.GetRadius(SimOpt.SimOpts.FixedBotRadii))
                 .OrderByDescending(r => r.Distance)
                 .FirstOrDefault()?.Robot;
        }

        public void SaveSimulation(string path)
        {
            var sim = new SavedSimulation
            {
                AllowHorizontalShapeDrift = SimOpt.SimOpts.AllowHorizontalShapeDrift,
                AllowVerticalShapeDrift = SimOpt.SimOpts.AllowVerticalShapeDrift,
                BadWastelevel = SimOpt.SimOpts.BadWasteLevel,
                CoefficientElasticity = SimOpt.SimOpts.CoefficientElasticity,
                CoefficientKinetic = SimOpt.SimOpts.CoefficientKinetic,
                CoefficientStatic = SimOpt.SimOpts.CoefficientStatic,
                CorpseEnabled = SimOpt.SimOpts.CorpseEnabled,
                Costs = SimOpt.SimOpts.Costs,
                CycleLength = SimOpt.SimOpts.CycleLength,
                DayNight = SimOpt.SimOpts.DayNight,
                DayNightCycleCounter = SimOpt.SimOpts.DayNightCycleCounter,
                Daytime = SimOpt.SimOpts.Daytime,
                DeadRobotSnp = SimOpt.SimOpts.DeadRobotSnp,
                Decay = SimOpt.SimOpts.Decay,
                DecayDelay = SimOpt.SimOpts.DecayDelay,
                DecayType = SimOpt.SimOpts.DecayType,
                Density = SimOpt.SimOpts.Density,
                DisableFixing = SimOpt.SimOpts.DisableFixing,
                DisableMutations = SimOpt.SimOpts.DisableMutations,
                DisableTies = SimOpt.SimOpts.DisableTies,
                DisableTypArepro = SimOpt.SimOpts.DisableTypArepro,
                DxSxConnected = SimOpt.SimOpts.DxSxConnected,
                EnableAutoSpeciation = SimOpt.SimOpts.EnableAutoSpeciation,
                EnergyExType = SimOpt.SimOpts.EnergyExType,
                EnergyFix = SimOpt.SimOpts.EnergyFix,
                EnergyProp = SimOpt.SimOpts.EnergyProp,
                FieldHeight = SimOpt.SimOpts.FieldHeight,
                FieldWidth = SimOpt.SimOpts.FieldWidth,
                FixedBotRadii = SimOpt.SimOpts.FixedBotRadii,
                FluidSolidCustom = SimOpt.SimOpts.FluidSolidCustom,
                Gradient = SimOpt.SimOpts.Gradient,
                LightIntensity = SimOpt.SimOpts.LightIntensity,
                MakeAllShapesBlack = SimOpt.SimOpts.MakeAllShapesBlack,
                MaxAbsNum = SimOpt.SimOpts.MaxAbsNum,
                MaxEnergy = SimOpt.SimOpts.MaxEnergy,
                MaxPopulation = SimOpt.SimOpts.MaxPopulation,
                MaxVelocity = SimOpt.SimOpts.MaxVelocity,
                MinVegs = SimOpt.SimOpts.MinVegs,
                MutCurrMult = SimOpt.SimOpts.MutCurrMult,
                MutCycMax = SimOpt.SimOpts.MutCycMax,
                MutCycMin = SimOpt.SimOpts.MutCycMin,
                MutOscill = SimOpt.SimOpts.MutOscill,
                MutOscillSine = SimOpt.SimOpts.MutOscillSine,
                NoShotDecay = SimOpt.SimOpts.NoShotDecay,
                NoWShotDecay = SimOpt.SimOpts.NoWShotDecay,
                Obstacles = _obstacleManager.Obstacles,
                OldCostX = SimOpt.SimOpts.OldCostX,
                PhysBrown = SimOpt.SimOpts.PhysBrown,
                PhysMoving = SimOpt.SimOpts.PhysMoving,
                PlanetEaters = SimOpt.SimOpts.PlanetEaters,
                PlanetEatersG = SimOpt.SimOpts.PlanetEatersG,
                Pondmode = SimOpt.SimOpts.PondMode,
                RepopAmount = SimOpt.SimOpts.RepopAmount,
                RepopCooldown = SimOpt.SimOpts.RepopCooldown,
                Robots = _robotsManager.Robots.Where(r => r.Exists),
                ShapeDriftRate = SimOpt.SimOpts.ShapeDriftRate,
                ShapesAbsorbShots = SimOpt.SimOpts.ShapesAbsorbShots,
                ShapesAreSeeThrough = SimOpt.SimOpts.ShapesAreSeeThrough,
                ShapesAreVisable = SimOpt.SimOpts.ShapesAreVisable,
                Shots = _shotsManager.Shots,
                SimGuid = SimOpt.SimOpts.SimGuid,
                SnpExcludeVegs = SimOpt.SimOpts.SnpExcludeVegs,
                SpeciationForkInterval = SimOpt.SimOpts.SpeciationForkInterval,
                SpeciationGeneticDistance = SimOpt.SimOpts.SpeciationGeneticDistance,
                Species = SimOpt.SimOpts.Specie,
                SunChange = Vegs.SunChange,
                SunDown = SimOpt.SimOpts.SunDown,
                SunDownThreshold = SimOpt.SimOpts.SunDownThreshold,
                SunOnRnd = SimOpt.SimOpts.SunOnRnd,
                SunPosition = Vegs.SunPosition,
                SunRange = Vegs.SunRange,
                SunThresholdMode = SimOpt.SimOpts.SunThresholdMode,
                SunUp = SimOpt.SimOpts.SunUp,
                SunUpThreshold = SimOpt.SimOpts.SunUpThreshold,
                Tides = SimOpt.SimOpts.Tides,
                TidesOf = SimOpt.SimOpts.TidesOf,
                TotBorn = SimOpt.SimOpts.TotBorn,
                TotRunCycle = SimOpt.SimOpts.TotRunCycle,
                UpDnConnected = SimOpt.SimOpts.UpDnConnected,
                VegFeedingToBody = SimOpt.SimOpts.VegFeedingToBody,
                Viscosity = SimOpt.SimOpts.Viscosity,
                Ygravity = SimOpt.SimOpts.YGravity,
                ZeroMomentum = SimOpt.SimOpts.ZeroMomentum,
                Zgravity = SimOpt.SimOpts.ZGravity,
            };

            var dName = Path.GetDirectoryName(path);
            if (dName != null)
                Directory.CreateDirectory(dName);

            File.WriteAllText(path, JsonSerializer.Serialize(sim));
        }

        public async Task StartSimulation(bool startLoaded = false)
        {
            DnaEngine.LoadSysVars();

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
                SimOpt.SimOpts.SimGuid = Guid.NewGuid();
                SimOpt.SimOpts.TotBorn = 0;
            }

            if (!startLoaded)
            {
                _obstacleManager = new ObstaclesManager();
                _shotsManager = new ShotsManager(_obstacleManager);
                _bucketManager = new BucketManager(SimOpt.SimOpts, _obstacleManager);
                _robotsManager = new RobotsManager(_bucketManager, _obstacleManager, _shotsManager);
            }

            var standardRadius = SimOpt.SimOpts.FixedBotRadii ? Robot.RobSize / 2.0 : 415.475;
            _shotsManager.MaxBotShotSeparation = Math.Pow(standardRadius, 2) +
                                     Math.Pow(SimOpt.SimOpts.MaxVelocity * 2 + Robot.RobSize / 3.0, 2);

            if (!startLoaded)
                await LoadRobots();
            else
            {
                Vegs.CoolDown = -SimOpt.SimOpts.RepopCooldown;
                Vegs.TotalVegs = -1;
            }

            _active = true;

            Main();
        }

        public void Stop()
        {
            _simThreadCancel.Cancel();
        }

        //private void DrawAllTies(Graphics graphics)
        //{
        //    foreach (var r in _robotsManager.Robots.Where(r => r.Exists))
        //    {
        //        DrawTobTiesCol(graphics, r, r.GetRadius(SimOpt.SimOpts.FixedBotRadii) * 2, r.GetRadius(SimOpt.SimOpts.FixedBotRadii));
        //    }
        //}

        //private void DrawTobTiesCol(Graphics graphics, Robot t, double w, double s)
        //{
        //    var drawSmall = Math.Max((int)(w / 4), 1);
        //    var centre = t.Position;

        //    foreach (var tie in t.Ties.Where(i => !i.BackTie))
        //    {
        //        var otherRob = tie.OtherBot;
        //        var centre1 = otherRob.Position;
        //        var drawWidth = drawSmall;
        //        var col = t.Color;
        //        w = Math.Max(w, 2);

        //        if (tie.Last > 0)
        //            drawWidth = (int)(w / 2);
        //        if (tie.InfoUsed)
        //        {
        //            col = Colors.White;
        //            tie.InfoUsed = false;
        //        }
        //        if (tie.EnergyUsed)
        //        {
        //            col = Colors.Red;
        //            tie.EnergyUsed = false;
        //        }
        //        if (tie.Sharing)
        //        {
        //            col = Colors.Yellow;
        //            tie.Sharing = false;
        //        }

        //        graphics.DrawLine(new Pen(col), centre, centre1);
        //    }
        //}

        private void Initialise()
        {
            _obstacleManager.InitObstacles();
        }

        private async Task LoadRobots()
        {
            foreach (var species in SimOpt.SimOpts.Specie)
            {
                for (var t = 0; t < species.Quantity; t++)
                {
                    var rob = await DnaManipulations.RobScriptLoad(_robotsManager, _bucketManager, Path.Join(species.Path, species.Name));

                    if (rob == null)
                    {
                        species.Native = false;
                        break;
                    }

                    species.Native = true;

                    rob.IsVegetable = species.Veg;
                    rob.ChloroplastsDisabled = species.NoChlr;
                    rob.IsFixed = species.Fixed;

                    if (rob.IsFixed)
                    {
                        rob.Memory[216] = 1;
                    }

                    rob.Position = new DoubleVector(ThreadSafeRandom.Local.Next((int)(species.Poslf * (SimOpt.SimOpts.FieldWidth - 60)), (int)(species.Posrg * (SimOpt.SimOpts.FieldWidth - 60))), ThreadSafeRandom.Local.Next((int)(species.Postp * (SimOpt.SimOpts.FieldHeight - 60)), (int)(species.Posdn * (SimOpt.SimOpts.FieldHeight - 60))));

                    rob.Energy = species.Stnrg;
                    rob.Body = 1000;

                    rob.Memory[MemoryAddresses.SetAim] = Physics.RadiansToInt(rob.Aim * 200);
                    if (rob.IsVegetable)
                        rob.Chloroplasts = SimOpt.SimOpts.StartChlr;

                    rob.IsDead = false;

                    rob.MutationProbabilities = species.Mutables;

                    for (var i = 0; i < 7; i++)
                    {
                        rob.Skin[i] = species.Skin[i];
                    }

                    rob.Color = species.Color;
                    rob.Memory[MemoryAddresses.timersys] = ThreadSafeRandom.Local.Next(-32000, 32000);
                    rob.CantSee = species.CantSee;
                    rob.DnaDisabled = species.DisableDna;
                    rob.MovementSysvarsDisabled = species.DisableMovementSysvars;
                    rob.CantReproduce = species.CantReproduce;
                    rob.IsVirusImmune = species.VirusImmune;
                    rob.VirusShot = null;
                    rob.VirusTimer = 0;
                    rob.NumberOfGenes = DnaManipulations.CountGenes(rob.Dna);

                    rob.GenMut = (double)rob.Dna.Count / RobotsManager.GeneticSensitivity; //Botsareus 4/9/2013 automatically apply genetic to inserted robots

                    rob.Memory[MemoryAddresses.DnaLenSys] = rob.Dna.Count;
                    rob.Memory[MemoryAddresses.GenesSys] = rob.NumberOfGenes;
                }
            }
        }

        private void Main()
        {
            _simThread = new Thread(MainFunction);
            _simThreadCancel = new CancellationTokenSource();

            _simThread.Start(_simThreadCancel.Token);
        }

        private async void MainFunction(object state)
        {
            const int MinFrameLength = 1000 / 50; // 50 FPS
            var cancelToken = (CancellationToken)state;

            while (!cancelToken.IsCancellationRequested)
            {
                var lastCycle = DateTime.Now;

                if (_active)
                {
                    try
                    {
                        await UpdateSim();
                        var currentTime = DateTime.Now;
                        Thread.Sleep(Math.Max(0, MinFrameLength - (int)(currentTime - lastCycle).TotalMilliseconds));
                    }
                    catch (TaskCanceledException)
                    { }
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        private void RenderField()
        {
            var update = new DisplayUpdate
            {
                FieldSize = new System.Windows.Size(SimOpt.SimOpts.FieldWidth, SimOpt.SimOpts.FieldHeight),
                RobotUpdates = _robotsManager.Robots.Select(r => new RobotUpdate
                {
                    Position = r.Position,
                    Radius = r.GetRadius(SimOpt.SimOpts.FixedBotRadii),
                    Color = r.Color
                }).ToList().AsReadOnly()
            };

            UpdateAvailable?.Invoke(this, new UpdateAvailableArgs { Update = update });
        }

        private async Task UpdateSim()
        {
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

            var currentPopulation = _robotsManager.TotalNotVegs;

            if (SimOpt.SimOpts.Costs.DynamicCostsIncludePlants)
                currentPopulation += Vegs.TotalVegsDisplayed; //Include Plants in target population

            //If (SimOpts.TotRunCycle + 200) Mod 2000 = 0 Then MsgBox "sup" & SimOpts.TotRunCycle 'debug only

            if (SimOpt.SimOpts.TotRunCycle % 10 == 0)
            {
                for (var i = 9; i >= 1; i--)
                    _populationLast10Cycles[i] = _populationLast10Cycles[i - 1];

                _populationLast10Cycles[9] = currentPopulation;
            }

            if (SimOpt.SimOpts.Costs.EnableDynamicCosts)
            {
                var amountOff = currentPopulation - SimOpt.SimOpts.Costs.DynamicCostsTargetPopulation;

                //If we are more than X% off of our target population either way AND the population isn't moving in the
                //the direction we want or hasn't moved at all in the past 10 cycles then adjust the cost multiplier
                var upperRange = SimOpt.SimOpts.Costs.DynamicCostsUpperRangeTarget * 0.01 * SimOpt.SimOpts.Costs.DynamicCostsTargetPopulation;
                var lowerRange = SimOpt.SimOpts.Costs.DynamicCostsLowerRangeTarget * 0.01 * SimOpt.SimOpts.Costs.DynamicCostsTargetPopulation;

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

            DnaEngine.ExecRobs(SimOpt.SimOpts.Costs, _robotsManager.Robots);

            //updateshots can write to bot sense, so we need to clear bot senses before updating shots
            foreach (var rob in _robotsManager.Robots.Where(r => r.Exists && r.DnaDisabled == false))
                Senses.EraseSenses(rob);

            _shotsManager.UpdateShots(_robotsManager);

            //Botsareus 6/22/2016 to figure actual velocity of the bot incase there is a collision event
            foreach (var rob in _robotsManager.Robots.Where(r => r.Exists))
                rob.OldPosition = rob.Position;

            await _robotsManager.UpdateBots();

            //to figure actual velocity of the bot incase there is a collision event
            foreach (var rob in _robotsManager.Robots.Where(r => r.Exists && !(r.OldPosition.X == 0 & r.OldPosition.Y == 0)))
                rob.ActualVelocity = rob.Position - rob.OldPosition;

            if (_obstacleManager.Obstacles.Count > 0)
                _obstacleManager.MoveObstacles();

            var allChlr = (int)_robotsManager.Robots.Where(r => r.Exists).Sum(r => r.Chloroplasts);

            _robotsManager.TotalChlr = allChlr / 16000; //Panda 8/23/2013 Calculate total unit chloroplasts

            if (_robotsManager.TotalChlr < SimOpt.SimOpts.MinVegs && Vegs.TotalVegsDisplayed != -1)
                await Vegs.VegsRepopulate(_robotsManager, _bucketManager); //Will be -1 first cycle after loading a sim.  Prevents spikes.

            Vegs.feedvegs(_robotsManager, _obstacleManager, SimOpt.SimOpts.MaxEnergy);

            //Kill some robots to prevent of memory
            var totlen = _robotsManager.Robots.Where(r => r.Exists).Sum(r => r.Dna.Count);

            if (totlen > 4000000)
            {
                var maxdel = 1500 * (_robotsManager.TotalRobots * 425 / totlen);

                foreach (var rob in _robotsManager.Robots.Where(r => r.Exists).OrderByDescending(r => r.Energy + r.Body * 10).Take(maxdel).ToArray())
                    await _robotsManager.KillRobot(rob);
            }
            if (totlen > 3000000)
            {
                foreach (var rob in _robotsManager.Robots)
                    rob.LastMutationDetail.Clear();
            }

            RenderField();
        }
    }
}
