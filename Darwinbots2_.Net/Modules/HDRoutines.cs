using DarwinBots.DataModel;
using DarwinBots.Model;
using DarwinBots.Support;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

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

        /// <summary>
        /// Assigns a robot his unique code.
        /// </summary>
        public static void GiveAbsNum(int k)
        {
            if (Robots.rob[k].AbsNum == 0)
            {
                SimOpt.SimOpts.MaxAbsNum++;
                Robots.rob[k].AbsNum = SimOpt.SimOpts.MaxAbsNum;
            }
        }

        /// <summary>
        /// Inserts organism file in the simulation
        /// </summary>
        /// <remarks>
        /// Organisms could be made of more than one robot
        /// </remarks>
        public static void InsertOrganism(string path)
        {
            var x = ThreadSafeRandom.Local.Next(60, SimOpt.SimOpts.FieldWidth - 60);
            var y = ThreadSafeRandom.Local.Next(60, SimOpt.SimOpts.FieldHeight - 60);
            LoadOrganism(path, x, y);
        }

        public static async Task LoadGlobalSettings()
        {
            //defaults
            Robots.BodyFix = 32100;
            Globals.UseSafeMode = true; //Botsareus 10/5/2015
            Globals.UseEpiGene = false; //Botsareus 10/8/2015
            //mutations tab
            Globals.EpiResetEmp = 1.3;
            Globals.EpiResetOp = 17;
            //Delta2
            Globals.Delta2 = false;
            Globals.DeltaMainExp = 1;
            Globals.DeltaMainLn = 0;
            Globals.DeltaDevExp = 7;
            Globals.DeltaDevLn = 1;
            Globals.DeltaPm = 3000;
            Globals.DeltaWtc = 15;
            Globals.DeltaMainChance = 100;
            Globals.DeltaDevChance = 30;
            //Normailize mutation rates
            Globals.NormMut = false;
            Globals.ValNormMut = 1071;
            Globals.ValMaxNormMut = 1071;

            var globalSettings = await GlobalSettings.Load();

            if (globalSettings != null)
            {
                Robots.BodyFix = globalSettings.BodyFix;
                Globals.ReproFix = globalSettings.ReproFix;
                Globals.UseSafeMode = globalSettings.UseSafeMode;
                Physics.boylabldisp = globalSettings.BoyLablDisp;
                Globals.EpiReset = globalSettings.EpiReset;
                Globals.EpiResetEmp = globalSettings.EpiResetTemp;
                Globals.EpiResetOp = globalSettings.EpiResetOP;
                Globals.SunBelt = globalSettings.SunBelt;
                Globals.Delta2 = globalSettings.Delta2;
                Globals.DeltaMainExp = globalSettings.DeltaMainExp;
                Globals.DeltaMainLn = globalSettings.DeltaMainLn;
                Globals.DeltaDevExp = globalSettings.DeltaDevExp;
                Globals.DeltaDevLn = globalSettings.DeltaDevLn;
                Globals.DeltaPm = globalSettings.DeltaPM;
                Globals.NormMut = globalSettings.NormMut;
                Globals.ValNormMut = globalSettings.ValNormMut;
                Globals.ValMaxNormMut = globalSettings.ValMaxNormMut;
                Globals.DeltaWtc = globalSettings.DeltaWTC;
                Globals.DeltaMainChance = globalSettings.DeltaMainChance;
                Globals.DeltaDevChance = globalSettings.DeltaDevChance;
                Globals.StartChlr = globalSettings.StartChlr;

                Globals.y_normsize = globalSettings.YNormSize;

                Globals.UseEpiGene = globalSettings.UseEpiGene;
            }

            Globals.SimAlreadyRunning = await SafeMode.Load();
            Globals.AutoSaved = await AutoSaved.Load();

            //If we are not using safe mode assume simulation is not runnin'
            if (Globals.UseSafeMode == false)
                Globals.SimAlreadyRunning = false;

            if (Globals.SimAlreadyRunning == false)
                Globals.AutoSaved = false;
        }

        public static async Task<MutationProbabilities> LoadMutationRates(string filename)
        {
            var data = JsonSerializer.Deserialize<SavedMutationRates>(await File.ReadAllTextAsync(filename));

            return new MutationProbabilities
            {
                PointWhatToChange = data.PointWhatToChange,
                CopyErrorWhatToChange = data.CopyErrorWhatToChange,
                mutarray = data.MutationProbabilities,
                Mean = data.MutationMeans,
                StdDev = data.MutationStdDevs
            };
        }

        /// <summary>
        /// Loads an organism file.
        /// </summary>
        public static int LoadOrganism(string path, double X, double Y)
        {
            throw new NotImplementedException();
            //var LoadOrganism = 0;
            //var clist = new int[51];

            //var OList = new int[51];

            //var cnum = 0;

            //var foundSpecies = false;

            //VBCloseFile(402);
            //VBOpenFile(402, path);
            //FileGet(402, cnum);
            //for (var k = 0; k < cnum - 1; k++)
            //{
            //    var nuovo = GetNewBot();
            //    clist[k] = nuovo;
            //    LoadRobot(402, nuovo);
            //    LoadOrganism = nuovo;
            //    var i = SimOpts.SpeciesNum;
            //    foundSpecies = false;
            //    while (i > 0)
            //    {
            //        i--;
            //        if (rob[nuovo].FName == SimOpts.Specie[i].Name)
            //        {
            //            foundSpecies = true;
            //            i = 0;
            //        }
            //    }

            //    if (!foundSpecies)
            //        AddSpecie(rob[nuovo], false);
            //}
            //VBCloseFile(402);

            //if (X > -1 && Y > -1)
            //    PlaceOrganism(clist, X, Y);

            //RemapTies(clist, OList, cnum);

            //return LoadOrganism;
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
            Teleport.Teleporters.Clear();
            Teleport.Teleporters.AddRange(savedFile.Teleporters);
            ObstaclesManager.Obstacles.Clear();
            ObstaclesManager.Obstacles.AddRange(savedFile.Obstacles);
            ShotsManager.Shots.Clear();
            ShotsManager.Shots.AddRange(savedFile.Shots);

            SimOpt.SimOpts.BlockedVegs = savedFile.BlockedVegs;
            SimOpt.SimOpts.Costs = savedFile.Costs;
            SimOpt.SimOpts.CostExecCond = savedFile.CostExecCond;
            SimOpt.SimOpts.DeadRobotSnp = savedFile.DeadRobotSnp;
            SimOpt.SimOpts.SnpExcludeVegs = savedFile.SnpExcludeVegs;
            SimOpt.SimOpts.DisableTies = savedFile.DisableTies;
            SimOpt.SimOpts.EnergyExType = savedFile.EnergyExType;
            SimOpt.SimOpts.EnergyFix = savedFile.EnergyFix;
            SimOpt.SimOpts.EnergyProp = savedFile.EnergyProp;
            SimOpt.SimOpts.FieldHeight = savedFile.FieldHeight;
            SimOpt.SimOpts.FieldSize = savedFile.FieldSize;
            SimOpt.SimOpts.FieldWidth = savedFile.FieldWidth;
            SimOpt.SimOpts.KillDistVegs = savedFile.KillDistVegs;
            SimOpt.SimOpts.MaxEnergy = savedFile.MaxEnergy;
            SimOpt.SimOpts.MaxPopulation = savedFile.MaxPopulation;
            SimOpt.SimOpts.MinVegs = savedFile.MinVegs;
            SimOpt.SimOpts.MutCurrMult = savedFile.MutCurrMult;
            SimOpt.SimOpts.MutCycMax = savedFile.MutCycMax;
            SimOpt.SimOpts.MutCycMin = savedFile.MutCycMin;
            SimOpt.SimOpts.MutOscill = savedFile.MutOscill;
            SimOpt.SimOpts.PhysBrown = savedFile.PhysBrown;
            SimOpt.SimOpts.Ygravity = savedFile.Ygravity;
            SimOpt.SimOpts.Zgravity = savedFile.Zgravity;
            SimOpt.SimOpts.PhysMoving = savedFile.PhysMoving;
            SimOpt.SimOpts.PhysSwim = savedFile.PhysSwim;
            SimOpt.SimOpts.PopLimMethod = savedFile.PopLimMethod;
            SimOpt.SimOpts.SimName = savedFile.SimName;
            SimOpt.SimOpts.Toroidal = savedFile.Toroidal;
            SimOpt.SimOpts.TotBorn = savedFile.TotBorn;
            SimOpt.SimOpts.TotRunCycle = savedFile.TotRunCycle;
            SimOpt.SimOpts.TotRunTime = savedFile.TotRunTime;
            SimOpt.SimOpts.Pondmode = savedFile.Pondmode;
            SimOpt.SimOpts.CorpseEnabled = savedFile.CorpseEnabled;
            SimOpt.SimOpts.LightIntensity = savedFile.LightIntensity;
            SimOpt.SimOpts.Decay = savedFile.Decay;
            SimOpt.SimOpts.Gradient = savedFile.Gradient;
            SimOpt.SimOpts.DayNight = savedFile.DayNight;
            SimOpt.SimOpts.CycleLength = savedFile.CycleLength;
            SimOpt.SimOpts.Decaydelay = savedFile.DecayDelay;
            SimOpt.SimOpts.DecayType = savedFile.DecayType;
            SimOpt.SimOpts.F1 = savedFile.F1;
            SimOpt.SimOpts.Restart = savedFile.Restart;
            SimOpt.SimOpts.Dxsxconnected = savedFile.DxSxConnected;
            SimOpt.SimOpts.Updnconnected = savedFile.UpDnConnected;
            SimOpt.SimOpts.RepopAmount = savedFile.RepopAmount;
            SimOpt.SimOpts.RepopCooldown = savedFile.RepopCooldown;
            SimOpt.SimOpts.ZeroMomentum = savedFile.ZeroMomentum;
            SimOpt.SimOpts.UserSeedNumber = savedFile.UserSeedNumber;
            SimOpt.SimOpts.VegFeedingToBody = savedFile.VegFeedingToBody;
            SimOpt.SimOpts.CoefficientStatic = savedFile.CoefficientStatic;
            SimOpt.SimOpts.CoefficientKinetic = savedFile.CoefficientKinetic;
            SimOpt.SimOpts.PlanetEaters = savedFile.PlanetEaters;
            SimOpt.SimOpts.PlanetEatersG = savedFile.PlanetEatersG;
            SimOpt.SimOpts.Viscosity = savedFile.Viscosity;
            SimOpt.SimOpts.Density = savedFile.Density;
            SimOpt.SimOpts.Costs = savedFile.Costs;
            SimOpt.SimOpts.BadWastelevel = savedFile.BadWastelevel;
            SimOpt.SimOpts.ChartingInterval = savedFile.ChartingInterval;
            SimOpt.SimOpts.CoefficientElasticity = savedFile.CoefficientElasticity;
            SimOpt.SimOpts.FluidSolidCustom = savedFile.FluidSolidCustom;
            SimOpt.SimOpts.CostRadioSetting = savedFile.CostRadioSetting;
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
            SimOpt.SimOpts.MakeAllShapesTransparent = savedFile.MakeAllShapesTransparent;
            SimOpt.SimOpts.MakeAllShapesBlack = savedFile.MakeAllShapesBlack;
            SimOpt.SimOpts.MaxAbsNum = savedFile.MaxAbsNum;
            SimOpt.SimOpts.EGridWidth = savedFile.EGridWidth;
            SimOpt.SimOpts.EGridEnabled = savedFile.EGridEnabled;
            SimOpt.SimOpts.OldCostX = savedFile.OldCostX;
            SimOpt.SimOpts.DisableMutations = savedFile.DisableMutations;
            SimOpt.SimOpts.SimGUID = savedFile.SimGUID;
            SimOpt.SimOpts.SpeciationGenerationalDistance = savedFile.SpeciationGenerationalDistance;
            SimOpt.SimOpts.SpeciationGeneticDistance = savedFile.SpeciationGeneticDistance;
            SimOpt.SimOpts.EnableAutoSpeciation = savedFile.EnableAutoSpeciation;
            SimOpt.SimOpts.SpeciationMinimumPopulation = savedFile.SpeciationMinimumPopulation;
            SimOpt.SimOpts.SpeciationForkInterval = savedFile.SpeciationForkInterval;
            SimOpt.SimOpts.DisableTypArepro = savedFile.DisableTypArepro;
            Globals.SimStart = savedFile.StrSimStart;
            SimOpt.SimOpts.NoWShotDecay = savedFile.NoWShotDecay;
            Master.energydif = savedFile.EnergyDif;
            Master.energydifX = savedFile.EnergyDifX;
            Master.energydifXP = savedFile.EnergyDifXP;
            Globals.ModeChangeCycles = savedFile.ModeChangeCycles;
            Master.energydif2 = savedFile.EnergyDif2;
            Master.energydifX2 = savedFile.EnergyDifX2;
            Master.energydifXP2 = savedFile.EnergyDifXP2;
            SimOpt.SimOpts.SunOnRnd = savedFile.SunOnRnd;
            SimOpt.SimOpts.DisableFixing = savedFile.DisableFixing;
            Vegs.SunPosition = savedFile.SunPosition;
            Vegs.SunRange = savedFile.SunRange;
            Vegs.SunChange = savedFile.SunChange;
            SimOpt.SimOpts.Tides = savedFile.Tides;
            SimOpt.SimOpts.TidesOf = savedFile.TidesOf;
            SimOpt.SimOpts.MutOscillSine = savedFile.MutOscillSine;
            Master.stagnent = savedFile.Stagnent;

            SimOpt.TmpOpts = SimOpt.SimOpts;
        }

        /// <summary>
        /// Places an organism (made of robots listed in clist()) in the specified x,y position.
        /// </summary>
        public static void PlaceOrganism(List<robot> clist, double x, double y)
        {
            throw new NotImplementedException();
            //var dx = x - clist[0].pos.X;
            //var dy = y - clist[0].pos.Y;

            //for (var k = 1; k < clist.Count; k++)
            //{
            //    clist[k].pos.X = clist[k].pos.X + dx;
            //    clist[k].pos.Y = clist[k].pos.Y + dy;
            //    clist[k].BucketPos.X = -2;
            //    clist[k].BucketPos.Y = -2;
            //    UpdateBotBucket(clist[k]);
            //    k++;
            //}
        }

        public static void RemapAllShots(int numOfShots)
        {
            throw new NotImplementedException();
            //foreach (var shot in Shots.Where(s => s.exist))
            //{
            //    for (var j = 1; j < MaxRobs; j++)
            //    {
            //        if (rob[j].exist)
            //        {
            //            if (ShotsManager.Shots[i].parent == rob[j].oldBotNum)
            //            {
            //                ShotsManager.Shots[i].parent = j;
            //                if (ShotsManager.Shots[i].stored)
            //                {
            //                    rob[j].virusshot = i;
            //                }
            //                break;
            //            }
            //        }
            //    }
            //    ShotsManager.Shots[i].stored = false; // Could not find parent.  Should probalby never happen but if it does, release the shot
            //}
        }

        public static void RemapAllTies(int numOfBots)
        {
            throw new NotImplementedException();

            //for (var i = 1; i < numOfBots; i++)
            //{
            //    var j = 1;
            //    while (rob[i].Ties[j].OtherBot > 0)
            //    { // Loop through each tie
            //        for (var k = 1; k < numOfBots; k++)
            //        {
            //            if (rob[i].Ties[j].OtherBot == rob[k].oldBotNum)
            //            {
            //                rob[i].Ties[j].OtherBot = k;
            //                break;
            //            }
            //        }

            //        j++;
            //    }
            //}
        }

        /// <summary>
        /// Remaps ties from the old index numbers - those the robots had when saved to disk- to the new indexes assigned in this simulation
        /// </summary>
        public static void RemapTies(int[] clist, int[] olist, int cnum)
        {
            throw new NotImplementedException();

            //for (var t = 0; t < cnum - 1; t++)
            //{ // Loop through each cell
            //    var ind = rob[clist[t]].oldBotNum;
            //    for (var k = 0; k < cnum - 1; k++)
            //    { // Loop through each cell
            //        var j = 1;
            //        while (rob[clist[k]].Ties[j].OtherBot > 0)
            //        { // Loop through each tie
            //            if (rob[clist[k]].Ties[j].OtherBot == ind)
            //                rob[clist[k]].Ties[j].OtherBot = clist[t];

            //            j++;
            //        }
            //    }
            //}

            //for (var k = 0; k < cnum - 1; k++)
            //{ // All cells
            //    var j = 1;
            //    while (rob[clist[k]].Ties[j].OtherBot > 0)
            //    { //All Ties
            //        var TiePointsToNode = false;
            //        for (var t = 0; t < cnum - 1; t++)
            //        {
            //            if (rob[clist[k]].Ties[j].OtherBot == clist[t])
            //                TiePointsToNode = true;
            //        }

            //        if (!TiePointsToNode)
            //            rob[clist[k]].Ties[j].OtherBot = 0;

            //        j++;
            //    }
            //}
        }

        /// <summary>
        /// Saves a robot's dna.
        /// </summary>
        public static async Task salvarob(robot rob, string path)
        {
            var hold = new StringBuilder();

            hold.AppendLine(DnaTokenizing.SaveRobHeader(rob));

            //Botsareus 10/8/2015 New code to save epigenetic memory as gene

            if (Globals.UseEpiGene)
            {
                var started = false;

                for (var a = 971; a <= 990; a++)
                {
                    if (rob.mem[a] != 0)
                    {
                        if (!started)
                            hold.AppendLine("start");

                        started = true;

                        hold.AppendLine($"{rob.mem[a]} {a} store");
                    }
                }

                if (started)
                    hold.AppendLine("stop");
            }

            hold.AppendLine(DnaTokenizing.DetokenizeDna(rob));

            var hashed = DnaTokenizing.Hash(hold.ToString());

            hold.AppendLine();
            hold.AppendLine($"'#hash: {hashed}");

            if (!string.IsNullOrWhiteSpace(rob.tag))
                hold.AppendLine($"'#tag: {rob.tag.Substring(0, 45)}");

            //Botsareus 12/11/2013 Save mrates file
            await Save_mrates(rob.Mutables, Path.Join(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + ".mrate"));

            if (MessageBox.Show($"Do you want to change robot's name to {Path.GetFileNameWithoutExtension(path)} ?", "Robot DNA saved", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                rob.FName = Path.GetFileNameWithoutExtension(path);
        }

        public static async Task Save_mrates(MutationProbabilities mut, string FName)
        {
            var data = new SavedMutationRates
            {
                PointWhatToChange = mut.PointWhatToChange,
                CopyErrorWhatToChange = mut.CopyErrorWhatToChange,
                MutationProbabilities = mut.mutarray,
                MutationMeans = mut.Mean,
                MutationStdDevs = mut.StdDev
            };

            await File.WriteAllTextAsync(FName, JsonSerializer.Serialize(data));
        }

        /// <summary>
        /// Saves the organism file.
        /// </summary>
        public static void SaveOrganism(string path, robot rob)
        {
            throw new NotImplementedException();
            //var clist = new int[51];

            //var k = 0;
            //var cnum = 0;

            //clist[0] = r;
            //ListCells(clist);

            //while (clist[cnum] > 0)
            //    cnum++;

            //VBCloseFile(401);
            //VBOpenFile(401, path); ;
            //FilePut(401, cnum);
            //for (k = 0; k < cnum - 1; k++)
            //{
            //    rob[clist[k]].LastOwner = IntOpts.IName;
            //    SaveRobotBody(401, clist[k]);
            //}
            //VBCloseFile(401);
            //return;
        }

        /// <summary>
        /// Saves a small file with per species population information.  Used for aggregating the population stats from multiple connected sims.
        /// </summary>
        public static async Task SaveSimPopulation(string path)
        {
            var data = new PopulationData
            {
                Species = SimOpt.SimOpts.Specie.Select(s => new SpeciesPopulationData
                {
                    Name = s.Name,
                    Population = s.population,
                    Veg = s.Veg,
                    Color = s.color
                })
            };

            await File.WriteAllTextAsync(path, JsonSerializer.Serialize(data));
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
                BadWastelevel = SimOpt.SimOpts.BadWastelevel,
                BlockedVegs = SimOpt.SimOpts.BlockedVegs,
                ChartingInterval = SimOpt.SimOpts.ChartingInterval,
                CoefficientElasticity = SimOpt.SimOpts.CoefficientElasticity,
                CoefficientKinetic = SimOpt.SimOpts.CoefficientKinetic,
                CoefficientStatic = SimOpt.SimOpts.CoefficientStatic,
                CorpseEnabled = SimOpt.SimOpts.CorpseEnabled,
                CostExecCond = SimOpt.SimOpts.CostExecCond,
                CostRadioSetting = SimOpt.SimOpts.CostRadioSetting,
                Costs = SimOpt.SimOpts.Costs,
                CycleLength = SimOpt.SimOpts.CycleLength,
                DayNight = SimOpt.SimOpts.DayNight,
                DayNightCycleCounter = SimOpt.SimOpts.DayNightCycleCounter,
                Daytime = SimOpt.SimOpts.Daytime,
                DeadRobotSnp = SimOpt.SimOpts.DeadRobotSnp,
                Decay = SimOpt.SimOpts.Decay,
                DecayDelay = SimOpt.SimOpts.Decaydelay,
                DecayType = SimOpt.SimOpts.DecayType,
                Density = SimOpt.SimOpts.Density,
                DisableFixing = SimOpt.SimOpts.DisableFixing,
                DisableMutations = SimOpt.SimOpts.DisableMutations,
                DisableTies = SimOpt.SimOpts.DisableTies,
                DisableTypArepro = SimOpt.SimOpts.DisableTypArepro,
                DxSxConnected = SimOpt.SimOpts.Dxsxconnected,
                EGridEnabled = SimOpt.SimOpts.EGridEnabled,
                EGridWidth = SimOpt.SimOpts.EGridWidth,
                EnableAutoSpeciation = SimOpt.SimOpts.EnableAutoSpeciation,
                EnergyDif = Master.energydif,
                EnergyDif2 = Master.energydif2,
                EnergyDifX = Master.energydifX,
                EnergyDifX2 = Master.energydifX2,
                EnergyDifXP = Master.energydifXP,
                EnergyDifXP2 = Master.energydifXP2,
                EnergyExType = SimOpt.SimOpts.EnergyExType,
                EnergyFix = SimOpt.SimOpts.EnergyFix,
                EnergyProp = SimOpt.SimOpts.EnergyProp,
                F1 = SimOpt.SimOpts.F1,
                FieldHeight = SimOpt.SimOpts.FieldHeight,
                FieldSize = SimOpt.SimOpts.FieldSize,
                FieldWidth = SimOpt.SimOpts.FieldWidth,
                FixedBotRadii = SimOpt.SimOpts.FixedBotRadii,
                FluidSolidCustom = SimOpt.SimOpts.FluidSolidCustom,
                Gradient = SimOpt.SimOpts.Gradient,
                KillDistVegs = SimOpt.SimOpts.KillDistVegs,
                LightIntensity = SimOpt.SimOpts.LightIntensity,
                MakeAllShapesBlack = SimOpt.SimOpts.MakeAllShapesBlack,
                MakeAllShapesTransparent = SimOpt.SimOpts.MakeAllShapesTransparent,
                MaxAbsNum = SimOpt.SimOpts.MaxAbsNum,
                MaxEnergy = SimOpt.SimOpts.MaxEnergy,
                MaxPopulation = SimOpt.SimOpts.MaxPopulation,
                MaxVelocity = SimOpt.SimOpts.MaxVelocity,
                MinVegs = SimOpt.SimOpts.MinVegs,
                ModeChangeCycles = Globals.ModeChangeCycles,
                MutCurrMult = SimOpt.SimOpts.MutCurrMult,
                MutCycMax = SimOpt.SimOpts.MutCycMax,
                MutCycMin = SimOpt.SimOpts.MutCycMin,
                MutOscill = SimOpt.SimOpts.MutOscill,
                MutOscillSine = SimOpt.SimOpts.MutOscillSine,
                NoShotDecay = SimOpt.SimOpts.NoShotDecay,
                NoWShotDecay = SimOpt.SimOpts.NoWShotDecay,
                Obstacles = ObstaclesManager.Obstacles,
                OldCostX = SimOpt.SimOpts.OldCostX,
                PhysBrown = SimOpt.SimOpts.PhysBrown,
                PhysMoving = SimOpt.SimOpts.PhysMoving,
                PhysSwim = SimOpt.SimOpts.PhysSwim,
                PlanetEaters = SimOpt.SimOpts.PlanetEaters,
                PlanetEatersG = SimOpt.SimOpts.PlanetEatersG,
                Pondmode = SimOpt.SimOpts.Pondmode,
                PopLimMethod = SimOpt.SimOpts.PopLimMethod,
                RepopAmount = SimOpt.SimOpts.RepopAmount,
                RepopCooldown = SimOpt.SimOpts.RepopCooldown,
                Restart = SimOpt.SimOpts.Restart,
                Robots = Robots.rob.Where(r => r.exist),
                ShapeDriftRate = SimOpt.SimOpts.ShapeDriftRate,
                ShapesAbsorbShots = SimOpt.SimOpts.ShapesAbsorbShots,
                ShapesAreSeeThrough = SimOpt.SimOpts.ShapesAreSeeThrough,
                ShapesAreVisable = SimOpt.SimOpts.ShapesAreVisable,
                Shots = ShotsManager.Shots,
                SimGUID = SimOpt.SimOpts.SimGUID,
                SimName = SimOpt.SimOpts.SimName,
                SnpExcludeVegs = SimOpt.SimOpts.SnpExcludeVegs,
                SpeciationForkInterval = SimOpt.SimOpts.SpeciationForkInterval,
                SpeciationGenerationalDistance = SimOpt.SimOpts.SpeciationGenerationalDistance,
                SpeciationGeneticDistance = SimOpt.SimOpts.SpeciationGeneticDistance,
                SpeciationMinimumPopulation = SimOpt.SimOpts.SpeciationMinimumPopulation,
                Species = SimOpt.SimOpts.Specie,
                Stagnent = Master.stagnent,
                StrSimStart = Globals.SimStart,
                SunChange = Vegs.SunChange,
                SunDown = SimOpt.SimOpts.SunDown,
                SunDownThreshold = SimOpt.SimOpts.SunDownThreshold,
                SunOnRnd = SimOpt.SimOpts.SunOnRnd,
                SunPosition = Vegs.SunPosition,
                SunRange = Vegs.SunRange,
                SunThresholdMode = SimOpt.SimOpts.SunThresholdMode,
                SunUp = SimOpt.SimOpts.SunUp,
                SunUpThreshold = SimOpt.SimOpts.SunUpThreshold,
                Teleporters = Teleport.Teleporters,
                Tides = SimOpt.SimOpts.Tides,
                TidesOf = SimOpt.SimOpts.TidesOf,
                Toroidal = SimOpt.SimOpts.Toroidal,
                TotBorn = SimOpt.SimOpts.TotBorn,
                TotRunCycle = SimOpt.SimOpts.TotRunCycle,
                TotRunTime = SimOpt.SimOpts.TotRunTime,
                UpDnConnected = SimOpt.SimOpts.Updnconnected,
                UserSeedNumber = SimOpt.SimOpts.UserSeedNumber,
                VegFeedingToBody = SimOpt.SimOpts.VegFeedingToBody,
                Viscosity = SimOpt.SimOpts.Viscosity,
                Ygravity = SimOpt.SimOpts.Ygravity,
                ZeroMomentum = SimOpt.SimOpts.ZeroMomentum,
                Zgravity = SimOpt.SimOpts.Zgravity,
            };

            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
            File.WriteAllText(path, JsonSerializer.Serialize(sim));
        }
    }
}
