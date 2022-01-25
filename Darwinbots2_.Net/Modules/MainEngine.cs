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
        private bool _active;
        private BucketManager _bucketManager;
        private RobotsManager _robotsManager;
        private ShotsManager _shotsManager;
        private Thread _simThread;
        private CancellationTokenSource _simThreadCancel;

        // TODO : Save last run settings
        // TODO : Graphing statistics
        // TODO : Handle showing data for the selected robot
        // TODO : Handle selected robot logic
        // TODO : Handle saving the sim on error
        // TODO : Drag and drop robots

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
            _shotsManager.Shots.Clear();
            _shotsManager.Shots.AddRange(savedFile.Shots);

            SimOpt.SimOpts.Costs = savedFile.Costs;
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
            SimOpt.SimOpts.PhysBrown = savedFile.PhysBrown;
            SimOpt.SimOpts.YGravity = savedFile.Ygravity;
            SimOpt.SimOpts.ZGravity = savedFile.Zgravity;
            SimOpt.SimOpts.PhysMoving = savedFile.PhysMoving;
            SimOpt.SimOpts.TotBorn = savedFile.TotBorn;
            SimOpt.SimOpts.TotRunCycle = savedFile.TotRunCycle;
            SimOpt.SimOpts.CorpseEnabled = savedFile.CorpseEnabled;
            SimOpt.SimOpts.Decay = savedFile.Decay;
            SimOpt.SimOpts.DecayDelay = savedFile.DecayDelay;
            SimOpt.SimOpts.DecayType = savedFile.DecayType;
            SimOpt.SimOpts.RepopAmount = savedFile.RepopAmount;
            SimOpt.SimOpts.RepopCooldown = savedFile.RepopCooldown;
            SimOpt.SimOpts.ZeroMomentum = savedFile.ZeroMomentum;
            SimOpt.SimOpts.VegFeedingToBody = savedFile.VegFeedingToBody;
            SimOpt.SimOpts.CoefficientStatic = savedFile.CoefficientStatic;
            SimOpt.SimOpts.CoefficientKinetic = savedFile.CoefficientKinetic;
            SimOpt.SimOpts.Viscosity = savedFile.Viscosity;
            SimOpt.SimOpts.Density = savedFile.Density;
            SimOpt.SimOpts.BadWasteLevel = savedFile.BadWastelevel;
            SimOpt.SimOpts.CoefficientElasticity = savedFile.CoefficientElasticity;
            SimOpt.SimOpts.FluidSolidCustom = savedFile.FluidSolidCustom;
            SimOpt.SimOpts.MaxVelocity = savedFile.MaxVelocity;
            SimOpt.SimOpts.NoShotDecay = savedFile.NoShotDecay;
            SimOpt.SimOpts.FixedBotRadii = savedFile.FixedBotRadii;
            SimOpt.SimOpts.DisableMutations = savedFile.DisableMutations;
            SimOpt.SimOpts.SpeciationGeneticDistance = savedFile.SpeciationGeneticDistance;
            SimOpt.SimOpts.EnableAutoSpeciation = savedFile.EnableAutoSpeciation;
            SimOpt.SimOpts.SpeciationForkInterval = savedFile.SpeciationForkInterval;
            SimOpt.SimOpts.DisableTypArepro = savedFile.DisableTypArepro;
            SimOpt.SimOpts.NoWShotDecay = savedFile.NoWShotDecay;
            SimOpt.SimOpts.DisableFixing = savedFile.DisableFixing;
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
                BadWastelevel = SimOpt.SimOpts.BadWasteLevel,
                CoefficientElasticity = SimOpt.SimOpts.CoefficientElasticity,
                CoefficientKinetic = SimOpt.SimOpts.CoefficientKinetic,
                CoefficientStatic = SimOpt.SimOpts.CoefficientStatic,
                CorpseEnabled = SimOpt.SimOpts.CorpseEnabled,
                Costs = SimOpt.SimOpts.Costs,
                Decay = SimOpt.SimOpts.Decay,
                DecayDelay = SimOpt.SimOpts.DecayDelay,
                DecayType = SimOpt.SimOpts.DecayType,
                Density = SimOpt.SimOpts.Density,
                DisableFixing = SimOpt.SimOpts.DisableFixing,
                DisableMutations = SimOpt.SimOpts.DisableMutations,
                DisableTies = SimOpt.SimOpts.DisableTies,
                DisableTypArepro = SimOpt.SimOpts.DisableTypArepro,
                EnableAutoSpeciation = SimOpt.SimOpts.EnableAutoSpeciation,
                EnergyExType = SimOpt.SimOpts.EnergyExType,
                EnergyFix = SimOpt.SimOpts.EnergyFix,
                EnergyProp = SimOpt.SimOpts.EnergyProp,
                FieldHeight = SimOpt.SimOpts.FieldHeight,
                FieldWidth = SimOpt.SimOpts.FieldWidth,
                FixedBotRadii = SimOpt.SimOpts.FixedBotRadii,
                FluidSolidCustom = SimOpt.SimOpts.FluidSolidCustom,
                MaxEnergy = SimOpt.SimOpts.MaxEnergy,
                MaxPopulation = SimOpt.SimOpts.MaxPopulation,
                MaxVelocity = SimOpt.SimOpts.MaxVelocity,
                MinVegs = SimOpt.SimOpts.MinVegs,
                MutCurrMult = SimOpt.SimOpts.MutCurrMult,
                NoShotDecay = SimOpt.SimOpts.NoShotDecay,
                NoWShotDecay = SimOpt.SimOpts.NoWShotDecay,
                PhysBrown = SimOpt.SimOpts.PhysBrown,
                PhysMoving = SimOpt.SimOpts.PhysMoving,
                RepopAmount = SimOpt.SimOpts.RepopAmount,
                RepopCooldown = SimOpt.SimOpts.RepopCooldown,
                Robots = _robotsManager.Robots.Where(r => r.Exists),
                Shots = _shotsManager.Shots,
                SpeciationForkInterval = SimOpt.SimOpts.SpeciationForkInterval,
                SpeciationGeneticDistance = SimOpt.SimOpts.SpeciationGeneticDistance,
                Species = SimOpt.SimOpts.Specie,
                TotBorn = SimOpt.SimOpts.TotBorn,
                TotRunCycle = SimOpt.SimOpts.TotRunCycle,
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

        public void StartSimulation(bool startLoaded = false)
        {
            DnaEngine.LoadSysVars();

            if (!startLoaded)
            {
                SimOpt.SimOpts.TotBorn = 0;
            }

            if (!startLoaded)
            {
                _shotsManager = new ShotsManager();
                _bucketManager = new BucketManager(SimOpt.SimOpts);
                _robotsManager = new RobotsManager(_bucketManager, _shotsManager);
            }

            var standardRadius = SimOpt.SimOpts.FixedBotRadii ? Robot.RobSize / 2.0 : 415.475;
            _shotsManager.MaxBotShotSeparation = Math.Pow(standardRadius, 2) +
                                     Math.Pow(SimOpt.SimOpts.MaxVelocity * 2 + Robot.RobSize / 3.0, 2);

            if (!startLoaded)
                LoadRobots();
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

        private void LoadRobots()
        {
            foreach (var species in SimOpt.SimOpts.Specie)
            {
                for (var t = 0; t < species.Quantity; t++)
                {
                    var rob = DnaManipulations.RobScriptLoad(_robotsManager, _bucketManager, Path.Join(species.Path, species.Name));

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

                    rob.Position = new DoubleVector(ThreadSafeRandom.Local.Next(Robot.RobSize / 2, SimOpt.SimOpts.FieldWidth - Robot.RobSize / 2), ThreadSafeRandom.Local.Next(Robot.RobSize / 2, SimOpt.SimOpts.FieldHeight - Robot.RobSize / 2));
                    rob.Energy = species.Stnrg;
                    rob.Body = 1000;

                    rob.Memory[MemoryAddresses.SetAim] = Physics.RadiansToInt(rob.Aim * 200);
                    if (rob.IsVegetable)
                        rob.Chloroplasts = SimOptions.StartChlr;

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

        private void MainFunction(object state)
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
                        UpdateSim();
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
                }).ToList().AsReadOnly(),
                TieUpdates = _robotsManager.Robots.SelectMany(r => r.Ties.Where(t => !t.BackTie).Select(t => new TieUpdate
                {
                    Color = t.Color ?? r.Color,
                    StartPoint = r.OffsetPosition,
                    EndPoint = t.OtherBot.OffsetPosition,
                    Width = Math.Max(10, t.Last > 0 ? r.GetRadius(SimOpt.SimOpts.FixedBotRadii) / 20 : r.GetRadius(SimOpt.SimOpts.FixedBotRadii) / 40),
                })).ToList().AsReadOnly(),
                ShotUpdates = _shotsManager.Shots.Select(r => new ShotUpdate
                {
                    Position = r.Position,
                    Color = r.Color
                }).ToList().AsReadOnly()
            };

            UpdateAvailable?.Invoke(this, new UpdateAvailableArgs { Update = update });
        }

        private void UpdateSim()
        {
            SimOpt.SimOpts.TotRunCycle++;

            DnaEngine.ExecRobs(SimOpt.SimOpts.Costs, _robotsManager.Robots);

            //updateshots can write to bot sense, so we need to clear bot senses before updating shots
            foreach (var rob in _robotsManager.Robots.Where(r => r.Exists && r.DnaDisabled == false))
                Senses.EraseSenses(rob);

            _shotsManager.UpdateShots(_robotsManager);

            //Botsareus 6/22/2016 to figure actual velocity of the bot incase there is a collision event
            foreach (var rob in _robotsManager.Robots.Where(r => r.Exists))
                rob.OldPosition = rob.Position;

            _robotsManager.UpdateBots();

            //to figure actual velocity of the bot incase there is a collision event
            foreach (var rob in _robotsManager.Robots.Where(r => r.Exists && !(r.OldPosition.X == 0 & r.OldPosition.Y == 0)))
                rob.ActualVelocity = rob.Position - rob.OldPosition;

            var allChlr = (int)_robotsManager.Robots.Where(r => r.Exists).Sum(r => r.Chloroplasts);

            _robotsManager.TotalChlr = allChlr / 16000; //Panda 8/23/2013 Calculate total unit chloroplasts

            if (_robotsManager.TotalChlr < SimOpt.SimOpts.MinVegs && Vegs.TotalVegs != -1)
                Vegs.VegsRepopulate(_robotsManager, _bucketManager); //Will be -1 first cycle after loading a sim.  Prevents spikes.

            Vegs.feedvegs(_robotsManager, SimOpt.SimOpts.MaxEnergy);

            //Kill some robots to prevent of memory
            var totlen = _robotsManager.Robots.Where(r => r.Exists).Sum(r => r.Dna.Count);

            if (totlen > 4000000)
            {
                var maxdel = 1500 * (_robotsManager.TotalRobots * 425 / totlen);

                foreach (var rob in _robotsManager.Robots.Where(r => r.Exists).OrderByDescending(r => r.Energy + r.Body * 10).Take(maxdel).ToArray())
                    _robotsManager.KillRobot(rob);
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
