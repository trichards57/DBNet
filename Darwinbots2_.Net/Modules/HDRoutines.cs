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
using static DarwinBots.Modules.DnaTokenizing;
using static DarwinBots.Modules.Globals;
using static DarwinBots.Modules.Master;
using static DarwinBots.Modules.Physics;
using static DarwinBots.Modules.Robots;
using static DarwinBots.Modules.ShotsManager;
using static DarwinBots.Modules.SimOpt;
using static DarwinBots.Modules.Teleport;
using static DarwinBots.Modules.Vegs;
using AutoSaved = DarwinBots.DataModel.AutoSaved;

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

            if (SimOpts.Specie.Count >= MAXNATIVESPECIES)
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

            SimOpts.Specie.Add(d);
        }

        /// <summary>
        /// Used in Tournament to get back original names of the files and move to result folder.
        /// </summary>
        public static async Task deseed(string s)
        {
            foreach (var file in Directory.GetFiles(s))
            {
                var lastLine = (await File.ReadAllLinesAsync(file)).Last();
                lastLine = lastLine.Replace("'#tag:", "");
                File.Copy(file, $@"league\Tournament_Results\{lastLine}");
            }
        }

        /// <summary>
        /// Assigns a robot his unique code.
        /// </summary>
        public static void GiveAbsNum(int k)
        {
            if (rob[k].AbsNum == 0)
            {
                SimOpts.MaxAbsNum++;
                rob[k].AbsNum = SimOpts.MaxAbsNum;
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
            var X = ThreadSafeRandom.Local.Next(60, SimOpts.FieldWidth - 60);
            var Y = ThreadSafeRandom.Local.Next(60, SimOpts.FieldHeight - 60);
            LoadOrganism(path, X, Y);
        }

        public static async Task LoadGlobalSettings()
        {
            //defaults
            BodyFix = 32100;
            UseSafeMode = true; //Botsareus 10/5/2015
            UseEpiGene = false; //Botsareus 10/8/2015
            //mutations tab
            EpiResetEmp = 1.3;
            EpiResetOp = 17;
            //Delta2
            Delta2 = false;
            DeltaMainExp = 1;
            DeltaMainLn = 0;
            DeltaDevExp = 7;
            DeltaDevLn = 1;
            DeltaPm = 3000;
            DeltaWtc = 15;
            DeltaMainChance = 100;
            DeltaDevChance = 30;
            //Normailize mutation rates
            NormMut = false;
            ValNormMut = 1071;
            ValMaxNormMut = 1071;

            var globalSettings = await GlobalSettings.Load();

            if (globalSettings != null)
            {
                BodyFix = globalSettings.BodyFix;
                ReproFix = globalSettings.ReproFix;
                UseSafeMode = globalSettings.UseSafeMode;
                boylabldisp = globalSettings.BoyLablDisp;
                EpiReset = globalSettings.EpiReset;
                EpiResetEmp = globalSettings.EpiResetTemp;
                EpiResetOp = globalSettings.EpiResetOP;
                SunBelt = globalSettings.SunBelt;
                Delta2 = globalSettings.Delta2;
                DeltaMainExp = globalSettings.DeltaMainExp;
                DeltaMainLn = globalSettings.DeltaMainLn;
                DeltaDevExp = globalSettings.DeltaDevExp;
                DeltaDevLn = globalSettings.DeltaDevLn;
                DeltaPm = globalSettings.DeltaPM;
                NormMut = globalSettings.NormMut;
                ValNormMut = globalSettings.ValNormMut;
                ValMaxNormMut = globalSettings.ValMaxNormMut;
                DeltaWtc = globalSettings.DeltaWTC;
                DeltaMainChance = globalSettings.DeltaMainChance;
                DeltaDevChance = globalSettings.DeltaDevChance;
                StartChlr = globalSettings.StartChlr;

                y_normsize = globalSettings.YNormSize;

                UseEpiGene = globalSettings.UseEpiGene;
            }

            SimAlreadyRunning = await SafeMode.Load();
            Globals.AutoSaved = await AutoSaved.Load();

            //If we are not using safe mode assume simulation is not runnin'
            if (UseSafeMode == false)
                SimAlreadyRunning = false;

            if (SimAlreadyRunning == false)
                Globals.AutoSaved = false;
        }

        public static async Task<MutationProbabilities> LoadMutationRates(string FName)
        {
            var data = JsonSerializer.Deserialize<SavedMutationRates>(await File.ReadAllTextAsync(FName));

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
            //Form1.instance.camfix = false; //Botsareus 2/23/2013 When simulation starts the screen is normailized

            var input = await File.ReadAllTextAsync(path);
            var savedFile = JsonSerializer.Deserialize<SavedSimulation>(input);

            if (savedFile == null)
                return;

            MaxRobs = savedFile.Robots.Count();

            rob.Clear();
            rob.AddRange(savedFile.Robots);
            SimOpt.Species.Clear();
            SimOpt.Species.AddRange(savedFile.Species);
            Teleporters.Clear();
            Teleporters.AddRange(savedFile.Teleporters);
            ObstaclesManager.Obstacles.Clear();
            ObstaclesManager.Obstacles.AddRange(savedFile.Obstacles);
            Shots.Clear();
            Shots.AddRange(savedFile.Shots);

            SimOpts.BlockedVegs = savedFile.BlockedVegs;
            SimOpts.Costs = savedFile.Costs;
            SimOpts.CostExecCond = savedFile.CostExecCond;
            SimOpts.DeadRobotSnp = savedFile.DeadRobotSnp;
            SimOpts.SnpExcludeVegs = savedFile.SnpExcludeVegs;
            SimOpts.DisableTies = savedFile.DisableTies;
            SimOpts.EnergyExType = savedFile.EnergyExType;
            SimOpts.EnergyFix = savedFile.EnergyFix;
            SimOpts.EnergyProp = savedFile.EnergyProp;
            SimOpts.FieldHeight = savedFile.FieldHeight;
            SimOpts.FieldSize = savedFile.FieldSize;
            SimOpts.FieldWidth = savedFile.FieldWidth;
            SimOpts.KillDistVegs = savedFile.KillDistVegs;
            SimOpts.MaxEnergy = savedFile.MaxEnergy;
            SimOpts.MaxPopulation = savedFile.MaxPopulation;
            SimOpts.MinVegs = savedFile.MinVegs;
            SimOpts.MutCurrMult = savedFile.MutCurrMult;
            SimOpts.MutCycMax = savedFile.MutCycMax;
            SimOpts.MutCycMin = savedFile.MutCycMin;
            SimOpts.MutOscill = savedFile.MutOscill;
            SimOpts.PhysBrown = savedFile.PhysBrown;
            SimOpts.Ygravity = savedFile.Ygravity;
            SimOpts.Zgravity = savedFile.Zgravity;
            SimOpts.PhysMoving = savedFile.PhysMoving;
            SimOpts.PhysSwim = savedFile.PhysSwim;
            SimOpts.PopLimMethod = savedFile.PopLimMethod;
            SimOpts.SimName = savedFile.SimName;
            SimOpts.Toroidal = savedFile.Toroidal;
            SimOpts.TotBorn = savedFile.TotBorn;
            SimOpts.TotRunCycle = savedFile.TotRunCycle;
            SimOpts.TotRunTime = savedFile.TotRunTime;
            SimOpts.Pondmode = savedFile.Pondmode;
            SimOpts.CorpseEnabled = savedFile.CorpseEnabled;
            SimOpts.LightIntensity = savedFile.LightIntensity;
            SimOpts.Decay = savedFile.Decay;
            SimOpts.Gradient = savedFile.Gradient;
            SimOpts.DayNight = savedFile.DayNight;
            SimOpts.CycleLength = savedFile.CycleLength;
            SimOpts.Decaydelay = savedFile.DecayDelay;
            SimOpts.DecayType = savedFile.DecayType;
            SimOpts.F1 = savedFile.F1;
            SimOpts.Restart = savedFile.Restart;
            SimOpts.Dxsxconnected = savedFile.DxSxConnected;
            SimOpts.Updnconnected = savedFile.UpDnConnected;
            SimOpts.RepopAmount = savedFile.RepopAmount;
            SimOpts.RepopCooldown = savedFile.RepopCooldown;
            SimOpts.ZeroMomentum = savedFile.ZeroMomentum;
            SimOpts.UserSeedNumber = savedFile.UserSeedNumber;
            SimOpts.VegFeedingToBody = savedFile.VegFeedingToBody;
            SimOpts.CoefficientStatic = savedFile.CoefficientStatic;
            SimOpts.CoefficientKinetic = savedFile.CoefficientKinetic;
            SimOpts.PlanetEaters = savedFile.PlanetEaters;
            SimOpts.PlanetEatersG = savedFile.PlanetEatersG;
            SimOpts.Viscosity = savedFile.Viscosity;
            SimOpts.Density = savedFile.Density;
            SimOpts.Costs = savedFile.Costs;
            SimOpts.BadWastelevel = savedFile.BadWastelevel;
            SimOpts.ChartingInterval = savedFile.ChartingInterval;
            SimOpts.CoefficientElasticity = savedFile.CoefficientElasticity;
            SimOpts.FluidSolidCustom = savedFile.FluidSolidCustom;
            SimOpts.CostRadioSetting = savedFile.CostRadioSetting;
            SimOpts.MaxVelocity = savedFile.MaxVelocity;
            SimOpts.NoShotDecay = savedFile.NoShotDecay;
            SimOpts.SunUpThreshold = savedFile.SunUpThreshold;
            SimOpts.SunUp = savedFile.SunUp;
            SimOpts.SunDownThreshold = savedFile.SunDownThreshold;
            SimOpts.SunDown = savedFile.SunDown;
            SimOpts.FixedBotRadii = savedFile.FixedBotRadii;
            SimOpts.DayNightCycleCounter = savedFile.DayNightCycleCounter;
            SimOpts.Daytime = savedFile.Daytime;
            SimOpts.SunThresholdMode = savedFile.SunThresholdMode;
            SimOpts.ShapesAreVisable = savedFile.ShapesAreVisable;
            SimOpts.AllowVerticalShapeDrift = savedFile.AllowVerticalShapeDrift;
            SimOpts.AllowHorizontalShapeDrift = savedFile.AllowHorizontalShapeDrift;
            SimOpts.ShapesAreSeeThrough = savedFile.ShapesAreSeeThrough;
            SimOpts.ShapesAbsorbShots = savedFile.ShapesAbsorbShots;
            SimOpts.ShapeDriftRate = savedFile.ShapeDriftRate;
            SimOpts.MakeAllShapesTransparent = savedFile.MakeAllShapesTransparent;
            SimOpts.MakeAllShapesBlack = savedFile.MakeAllShapesBlack;
            SimOpts.MaxAbsNum = savedFile.MaxAbsNum;
            SimOpts.EGridWidth = savedFile.EGridWidth;
            SimOpts.EGridEnabled = savedFile.EGridEnabled;
            SimOpts.OldCostX = savedFile.OldCostX;
            SimOpts.DisableMutations = savedFile.DisableMutations;
            SimOpts.SimGUID = savedFile.SimGUID;
            SimOpts.SpeciationGenerationalDistance = savedFile.SpeciationGenerationalDistance;
            SimOpts.SpeciationGeneticDistance = savedFile.SpeciationGeneticDistance;
            SimOpts.EnableAutoSpeciation = savedFile.EnableAutoSpeciation;
            SimOpts.SpeciationMinimumPopulation = savedFile.SpeciationMinimumPopulation;
            SimOpts.SpeciationForkInterval = savedFile.SpeciationForkInterval;
            SimOpts.DisableTypArepro = savedFile.DisableTypArepro;
            SimStart = savedFile.StrSimStart;
            SimOpts.NoWShotDecay = savedFile.NoWShotDecay;
            energydif = savedFile.EnergyDif;
            energydifX = savedFile.EnergyDifX;
            energydifXP = savedFile.EnergyDifXP;
            ModeChangeCycles = savedFile.ModeChangeCycles;
            energydif2 = savedFile.EnergyDif2;
            energydifX2 = savedFile.EnergyDifX2;
            energydifXP2 = savedFile.EnergyDifXP2;
            SimOpts.SunOnRnd = savedFile.SunOnRnd;
            SimOpts.DisableFixing = savedFile.DisableFixing;
            SunPosition = savedFile.SunPosition;
            SunRange = savedFile.SunRange;
            SunChange = savedFile.SunChange;
            SimOpts.Tides = savedFile.Tides;
            SimOpts.TidesOf = savedFile.TidesOf;
            SimOpts.MutOscillSine = savedFile.MutOscillSine;
            stagnent = savedFile.Stagnent;

            //Form1.instance.lblSaving.Visibility = Visibility.Hidden; //Botsareus 1/14/2014

            TmpOpts = SimOpts;
        }

        /// <summary>
        /// top/buttom pattern file move
        /// </summary>
        public static void movefilemulti(string source, string Out, int count)
        {
            var i = 0;

            foreach (var file in Directory.GetFiles(source).OrderBy(s => Path.GetFileName(s)))
            {
                if (i >= count)
                    return;

                File.Move(file, Path.Join(Out, file));
            }
        }

        /// <summary>
        /// Used in Stepladder to move files in a specific order.
        /// </summary>
        public static void movetopos(string s, int pos)
        {
            var files = Directory.GetFiles(@"league\stepladder");
            if (pos > files.Length)
            {
                //just put at end
                File.Copy(s, $@"league\stepladder\{files.Length + 1}-{System.IO.Path.GetFileName(s)}");
            }
            else
            {
                //move files first
                for (var i = files.Length; i > pos; i--)
                {
                    //find a file prefixed i
                    for (var j = 1; j < files.Length; j++)
                    {
                        var parts = System.IO.Path.GetFileName(files[j]).Split("-", 2);
                        if (parts[0] == i.ToString())
                        {
                            File.Move(files[j], $@"league\stepladder\{i + 1}-{parts[1]}");
                            break;
                        }
                    }
                }
                File.Copy(s, $@"league\stepladder\{pos}-" + System.IO.Path.GetFileName(s));
            }

            File.Delete(s);
        }

        public static string NamefileRecursive(string s)
        {
            var i = 'a' - 1;
            var NamefileRecursive = s;
            while (File.Exists(NamefileRecursive))
            {
                i++;
                if ('z' < i)
                    break;

                NamefileRecursive = s.Replace(".txt", $"{(char)i}.txt");
            }
            return NamefileRecursive;
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

            hold.AppendLine(SaveRobHeader(rob));

            //Botsareus 10/8/2015 New code to save epigenetic memory as gene

            if (UseEpiGene)
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

            hold.AppendLine(DetokenizeDna(rob));

            var hashed = Hash(hold.ToString());

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
                Species = SimOpts.Specie.Select(s => new SpeciesPopulationData
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
                AllowHorizontalShapeDrift = SimOpts.AllowHorizontalShapeDrift,
                AllowVerticalShapeDrift = SimOpts.AllowVerticalShapeDrift,
                BadWastelevel = SimOpts.BadWastelevel,
                BlockedVegs = SimOpts.BlockedVegs,
                ChartingInterval = SimOpts.ChartingInterval,
                CoefficientElasticity = SimOpts.CoefficientElasticity,
                CoefficientKinetic = SimOpts.CoefficientKinetic,
                CoefficientStatic = SimOpts.CoefficientStatic,
                CorpseEnabled = SimOpts.CorpseEnabled,
                CostExecCond = SimOpts.CostExecCond,
                CostRadioSetting = SimOpts.CostRadioSetting,
                Costs = SimOpts.Costs,
                CycleLength = SimOpts.CycleLength,
                DayNight = SimOpts.DayNight,
                DayNightCycleCounter = SimOpts.DayNightCycleCounter,
                Daytime = SimOpts.Daytime,
                DeadRobotSnp = SimOpts.DeadRobotSnp,
                Decay = SimOpts.Decay,
                DecayDelay = SimOpts.Decaydelay,
                DecayType = SimOpts.DecayType,
                Density = SimOpts.Density,
                DisableFixing = SimOpts.DisableFixing,
                DisableMutations = SimOpts.DisableMutations,
                DisableTies = SimOpts.DisableTies,
                DisableTypArepro = SimOpts.DisableTypArepro,
                DxSxConnected = SimOpts.Dxsxconnected,
                EGridEnabled = SimOpts.EGridEnabled,
                EGridWidth = SimOpts.EGridWidth,
                EnableAutoSpeciation = SimOpts.EnableAutoSpeciation,
                EnergyDif = energydif,
                EnergyDif2 = energydif2,
                EnergyDifX = energydifX,
                EnergyDifX2 = energydifX2,
                EnergyDifXP = energydifXP,
                EnergyDifXP2 = energydifXP2,
                EnergyExType = SimOpts.EnergyExType,
                EnergyFix = SimOpts.EnergyFix,
                EnergyProp = SimOpts.EnergyProp,
                F1 = SimOpts.F1,
                FieldHeight = SimOpts.FieldHeight,
                FieldSize = SimOpts.FieldSize,
                FieldWidth = SimOpts.FieldWidth,
                FixedBotRadii = SimOpts.FixedBotRadii,
                FluidSolidCustom = SimOpts.FluidSolidCustom,
                Gradient = SimOpts.Gradient,
                KillDistVegs = SimOpts.KillDistVegs,
                LightIntensity = SimOpts.LightIntensity,
                MakeAllShapesBlack = SimOpts.MakeAllShapesBlack,
                MakeAllShapesTransparent = SimOpts.MakeAllShapesTransparent,
                MaxAbsNum = SimOpts.MaxAbsNum,
                MaxEnergy = SimOpts.MaxEnergy,
                MaxPopulation = SimOpts.MaxPopulation,
                MaxVelocity = SimOpts.MaxVelocity,
                MinVegs = SimOpts.MinVegs,
                ModeChangeCycles = ModeChangeCycles,
                MutCurrMult = SimOpts.MutCurrMult,
                MutCycMax = SimOpts.MutCycMax,
                MutCycMin = SimOpts.MutCycMin,
                MutOscill = SimOpts.MutOscill,
                MutOscillSine = SimOpts.MutOscillSine,
                NoShotDecay = SimOpts.NoShotDecay,
                NoWShotDecay = SimOpts.NoWShotDecay,
                Obstacles = ObstaclesManager.Obstacles,
                OldCostX = SimOpts.OldCostX,
                PhysBrown = SimOpts.PhysBrown,
                PhysMoving = SimOpts.PhysMoving,
                PhysSwim = SimOpts.PhysSwim,
                PlanetEaters = SimOpts.PlanetEaters,
                PlanetEatersG = SimOpts.PlanetEatersG,
                Pondmode = SimOpts.Pondmode,
                PopLimMethod = SimOpts.PopLimMethod,
                RepopAmount = SimOpts.RepopAmount,
                RepopCooldown = SimOpts.RepopCooldown,
                Restart = SimOpts.Restart,
                Robots = rob.Where(r => r.exist),
                ShapeDriftRate = SimOpts.ShapeDriftRate,
                ShapesAbsorbShots = SimOpts.ShapesAbsorbShots,
                ShapesAreSeeThrough = SimOpts.ShapesAreSeeThrough,
                ShapesAreVisable = SimOpts.ShapesAreVisable,
                Shots = Shots,
                SimGUID = SimOpts.SimGUID,
                SimName = SimOpts.SimName,
                SnpExcludeVegs = SimOpts.SnpExcludeVegs,
                SpeciationForkInterval = SimOpts.SpeciationForkInterval,
                SpeciationGenerationalDistance = SimOpts.SpeciationGenerationalDistance,
                SpeciationGeneticDistance = SimOpts.SpeciationGeneticDistance,
                SpeciationMinimumPopulation = SimOpts.SpeciationMinimumPopulation,
                Species = SimOpts.Specie,
                Stagnent = stagnent,
                StrSimStart = SimStart,
                SunChange = SunChange,
                SunDown = SimOpts.SunDown,
                SunDownThreshold = SimOpts.SunDownThreshold,
                SunOnRnd = SimOpts.SunOnRnd,
                SunPosition = SunPosition,
                SunRange = SunRange,
                SunThresholdMode = SimOpts.SunThresholdMode,
                SunUp = SimOpts.SunUp,
                SunUpThreshold = SimOpts.SunUpThreshold,
                Teleporters = Teleporters,
                Tides = SimOpts.Tides,
                TidesOf = SimOpts.TidesOf,
                Toroidal = SimOpts.Toroidal,
                TotBorn = SimOpts.TotBorn,
                TotRunCycle = SimOpts.TotRunCycle,
                TotRunTime = SimOpts.TotRunTime,
                UpDnConnected = SimOpts.Updnconnected,
                UserSeedNumber = SimOpts.UserSeedNumber,
                VegFeedingToBody = SimOpts.VegFeedingToBody,
                Viscosity = SimOpts.Viscosity,
                Ygravity = SimOpts.Ygravity,
                ZeroMomentum = SimOpts.ZeroMomentum,
                Zgravity = SimOpts.Zgravity,
            };

            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
            File.WriteAllText(path, JsonSerializer.Serialize(sim));
        }
    }
}
