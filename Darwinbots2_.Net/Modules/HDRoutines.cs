using DarwinBots.DataModel;
using DarwinBots.Model;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DarwinBots.Modules
{
    internal static class HDRoutines
    {
        /// <summary>
        /// Adds a record to the species array when a bot with a new species is loaded or teleported in.
        /// </summary>
        public static void AddSpecie(robot rob, bool isNative)
        {
            if (rob.Corpse || rob.FName == "Corpse" || rob.exist == false)
                return;

            if (SimOpt.SimOpts.Specie.Count >= SimOpt.MAXNATIVESPECIES)
                return;

            var d = new Species
            {
                Name = rob.FName,
                Veg = rob.Veg,
                CantSee = rob.CantSee,
                DisableMovementSysvars = rob.DisableMovementSysvars,
                DisableDNA = rob.DisableDNA,
                CantReproduce = rob.CantReproduce,
                VirusImmune = rob.VirusImmune,
                population = 1,
                SubSpeciesCounter = 0,
                color = rob.color,
                Comment = "Species arrived from the Internet",
                Posrg = 1,
                Posdn = 1,
                Poslf = 0,
                Postp = 0,
                qty = 5,
                Stnrg = 3000,
                Native = isNative,
                path = "robots",
            };

            d.Mutables.ResetToDefault();
            d.Mutables.Mutations = rob.Mutables.Mutations;

            SimOpt.SimOpts.Specie.Add(d);
        }

        public static async Task LoadSimulation(string path)
        {
            var input = await File.ReadAllTextAsync(path);
            var savedFile = JsonSerializer.Deserialize<SavedSimulation>(input);

            if (savedFile == null)
                return;

            Robots.MaxRobs = savedFile.Robots.Count();

            Robots.rob.Clear();
            Robots.rob.AddRange(savedFile.Robots);
            SimOpt.Species.Clear();
            SimOpt.Species.AddRange(savedFile.Species);
            Globals.ObstacleManager.Obstacles.Clear();
            Globals.ObstacleManager.Obstacles.AddRange(savedFile.Obstacles);
            ShotsManager.Shots.Clear();
            ShotsManager.Shots.AddRange(savedFile.Shots);

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
            SimOpt.SimOpts.Costs = savedFile.Costs;
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
            SimOpt.SimOpts.SimGuid = savedFile.SimGUID;
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

            SimOpt.TmpOpts = SimOpt.SimOpts;
        }

        /// <summary>
        /// Saves a whole simulation.
        /// </summary>
        public static void SaveSimulation(string path)
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
                Obstacles = Globals.ObstacleManager.Obstacles,
                OldCostX = SimOpt.SimOpts.OldCostX,
                PhysBrown = SimOpt.SimOpts.PhysBrown,
                PhysMoving = SimOpt.SimOpts.PhysMoving,
                PlanetEaters = SimOpt.SimOpts.PlanetEaters,
                PlanetEatersG = SimOpt.SimOpts.PlanetEatersG,
                Pondmode = SimOpt.SimOpts.PondMode,
                RepopAmount = SimOpt.SimOpts.RepopAmount,
                RepopCooldown = SimOpt.SimOpts.RepopCooldown,
                Robots = Robots.rob.Where(r => r.exist),
                ShapeDriftRate = SimOpt.SimOpts.ShapeDriftRate,
                ShapesAbsorbShots = SimOpt.SimOpts.ShapesAbsorbShots,
                ShapesAreSeeThrough = SimOpt.SimOpts.ShapesAreSeeThrough,
                ShapesAreVisable = SimOpt.SimOpts.ShapesAreVisable,
                Shots = ShotsManager.Shots,
                SimGUID = SimOpt.SimOpts.SimGuid,
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

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, JsonSerializer.Serialize(sim));
        }
    }
}
