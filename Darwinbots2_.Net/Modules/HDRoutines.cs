using DBNet.Forms;
using Iersera.DataModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using static BucketManager;
using static Common;
using static DNAManipulations;
using static DNATokenizing;
using static Globals;
using static IntOpts;
using static Master;
using static Microsoft.VisualBasic.FileSystem;
using static Microsoft.VisualBasic.VBMath;
using static Multibots;
using static Obstacles;
using static Physics;
using static Robots;
using static Shots_Module;
using static SimOptModule;
using static Teleport;
using static Ties;
using static varspecie;
using static VBExtension;
using static Vegs;

internal static class HDRoutines
{
    /// <summary>
    /// Adds a record to the species array when a bot with a new species is loaded or teleported in.
    /// </summary>
    public static void AddSpecie(robot rob, bool IsNative)
    {
        if (rob.Corpse || rob.FName == "Corpse" || rob.exist == false)
            return;

        var k = SimOpts.SpeciesNum;

        if (k < MAXNATIVESPECIES)
            SimOpts.SpeciesNum++;

        var d = new datispecie
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
            Native = IsNative,
            path = "robots",
        };

        SetDefaultMutationRates(d.Mutables);
        d.Mutables.Mutations = rob.Mutables.Mutations;

        SimOpts.Specie.Add(d);
    }

    /// <summary>
    /// Used in Tournament to get back original names of the files and move to result folder.
    /// </summary>
    public static async Task deseed(string s)
    {
        var files = Directory.GetFiles(s);

        for (var i = 1; i < files.Length; i++)
        {
            var lastLine = (await File.ReadAllLinesAsync(files[i])).Last();
            lastLine = lastLine.Replace("'#tag:", "");
            File.Copy(files[i], $@"\league\Tournament_Results\{lastLine}");
        }
    }

    public static string GetFilePath(string FileName)
    {
        var fi = new FileInfo(FileName);
        return fi.Name;
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
        var X = Random(60, SimOpts.FieldWidth - 60);
        var Y = Random(60, SimOpts.FieldHeight - 60);
        LoadOrganism(path, X, Y);
    }

    public static async Task<mutationprobs> Load_mrates(string FName)
    {
        var data = JsonSerializer.Deserialize<SavedMutationRates>(await File.ReadAllTextAsync(FName));

        return new mutationprobs
        {
            PointWhatToChange = data.PointWhatToChange,
            CopyErrorWhatToChange = data.CopyErrorWhatToChange,
            mutarray = data.MutationProbabilities,
            Mean = data.MutationMeans,
            StdDev = data.MutationStdDevs
        };
    }

    public static async Task LoadGlobalSettings()
    {
        //defaults
        bodyfix = 32100;
        chseedstartnew = true;
        chseedloadsim = true;
        GraphUp = false;
        HideDB = false;
        UseSafeMode = true; //Botsareus 10/5/2015
        UseEpiGene = false; //Botsareus 10/8/2015
        UseIntRnd = false; //Botsareus 10/8/2015
        intFindBestV2 = 100;
        UseOldColor = true;
        //mutations tab
        epiresetemp = 1.3;
        epiresetOP = 17;
        //Delta2
        Delta2 = false;
        DeltaMainExp = 1;
        DeltaMainLn = 0;
        DeltaDevExp = 7;
        DeltaDevLn = 1;
        DeltaPM = 3000;
        DeltaWTC = 15;
        DeltaMainChance = 100;
        DeltaDevChance = 30;
        //Normailize mutation rates
        NormMut = false;
        valNormMut = 1071;
        valMaxNormMut = 1071;

        y_hidePredCycl = 1500;
        y_LFOR = 10;

        y_zblen = 255;

        leagueSourceDir = "Robots\\F1league";

        //see if eco exsists
        y_eco_im = await EcoMode.Load();

        //see if restartmode exisit

        var restartMode = await Iersera.DataModel.RestartMode.Load();
        x_restartmode = restartMode.Mode;
        x_filenumber = restartMode.FileNumber;

        var globalSettings = await GlobalSettings.Load();

        if (globalSettings != null)
        {
            screenratiofix = globalSettings.ScreenRatioFix;
            bodyfix = globalSettings.BodyFix;
            reprofix = globalSettings.ReproFix;
            chseedstartnew = globalSettings.ChSeedStartNew;
            chseedloadsim = globalSettings.ChSeedLoadSim;
            UseSafeMode = globalSettings.UseSafeMode;
            intFindBestV2 = globalSettings.IntFindBestV2;
            UseOldColor = globalSettings.UseOldColor;
            boylabldisp = globalSettings.BoyLablDisp;
            startnovid = globalSettings.StartNovId;
            epireset = globalSettings.EpiReset;
            epiresetemp = globalSettings.EpiResetTemp;
            epiresetOP = globalSettings.EpiResetOP;
            sunbelt = globalSettings.SunBelt;
            Delta2 = globalSettings.Delta2;
            DeltaMainExp = globalSettings.DeltaMainExp;
            DeltaMainLn = globalSettings.DeltaMainLn;
            DeltaDevExp = globalSettings.DeltaDevExp;
            DeltaDevLn = globalSettings.DeltaDevLn;
            DeltaPM = globalSettings.DeltaPM;
            NormMut = globalSettings.NormMut;
            valNormMut = globalSettings.ValNormMut;
            valMaxNormMut = globalSettings.ValMaxNormMut;
            DeltaWTC = globalSettings.DeltaWTC;
            DeltaMainChance = globalSettings.DeltaMainChance;
            DeltaDevChance = globalSettings.DeltaDevChance;
            leagueSourceDir = globalSettings.LeagueSourceDir;
            UseStepladder = globalSettings.UseStepladder;
            x_fudge = globalSettings.XFudge;
            StartChlr = globalSettings.StartChlr;
            Disqualify = globalSettings.Disqualify;
            y_robdir = globalSettings.YRobDir;
            y_graphs = globalSettings.YGraphs;

            if (x_restartmode < 4 || x_restartmode == 10)
                y_normsize = false;
            else
                y_normsize = globalSettings.YNormSize;

            y_hidePredCycl = globalSettings.YHidePredCycl;
            y_LFOR = globalSettings.YLFOR;
            y_zblen = globalSettings.YZblen;
            x_res_kill_chlr = globalSettings.XResKillChlr;
            x_res_kill_mb = globalSettings.XResKillMb;
            x_res_other = globalSettings.XResOther;
            y_res_kill_chlr = globalSettings.YResKillChlr;
            y_res_kill_mb = globalSettings.YResKillMb;
            y_res_kill_dq = globalSettings.YResKillDq;
            y_res_other = globalSettings.YResOther;
            x_res_kill_mb_veg = globalSettings.XResKillMbVeg;
            x_res_other_veg = globalSettings.XResOtherVeg;
            y_res_kill_mb_veg = globalSettings.YResKillMbVeg;
            y_res_kill_dq_veg = globalSettings.YResKillDqVeg;
            y_res_other_veg = globalSettings.YResOtherVeg;

            GraphUp = globalSettings.GraphUp;
            HideDB = globalSettings.HideDB;
            UseEpiGene = globalSettings.UseEpiGene;
            UseIntRnd = globalSettings.UseIntRnd;
        }

        //some global settings change during simulation (copy is here)
        loadboylabldisp = boylabldisp;
        loadstartnovid = startnovid;

        simalreadyrunning = await SafeMode.Load();
        autosaved = await AutoSaved.Load();

        //If we are not using safe mode assume simulation is not runnin'
        if (UseSafeMode == false)
            simalreadyrunning = false;

        if (simalreadyrunning == false)
            autosaved = false;

        //Botsareus 3/16/2014 If autosaved, we change restartmode, this forces system to run in diagnostic mode
        //The difference between x_restartmode 0 and 5 is that 5 uses hidepred settings
        if (autosaved && x_restartmode == 4)
        {
            x_restartmode = 5;
            MDIForm1.instance.y_info.setVisible(true);
        }
        if (autosaved && x_restartmode == 7)
        {
            x_restartmode = 8; //Botsareus 4/14/2014 same deal for zb evo
            intFindBestV2 = 20 + (int)Rnd(-(x_filenumber + 1)) * 40; //Botsareus 10/26/2015 Value more interesting
        }

        //Botsareus 3/19/2014 Load data for evo mode
        if (x_restartmode == 4 || x_restartmode == 5 || x_restartmode == 6)
        {
            var evoData = await EvoData.Load();

            if (evoData != null)
            {
                LFOR = evoData.LFOR;
                LFORdir = evoData.LFORdir;
                LFORcorr = evoData.LFORcorr;
                hidePredCycl = evoData.hidePredCycl;
                curr_dna_size = evoData.curr_dna_size;
                target_dna_size = evoData.target_dna_size;
                Init_hidePredCycl = evoData.Init_hidePredCycl;
                y_Stgwins = evoData.y_Stgwins;
            }
        }
        else
        {
            y_eco_im = 0;
        }

        //Botsareus 3/22/2014 Initial hidepred offset is normal

        hidePredOffset = hidePredCycl / 6;
    }

    /// <summary>
    /// Loads an organism file.
    /// </summary>
    public static int LoadOrganism(string path, double X, double Y)
    {
        var LoadOrganism = 0;
        var clist = new int[51];

        var OList = new int[51];

        var cnum = 0;

        var foundSpecies = false;

        VBCloseFile(402);
        VBOpenFile(402, path);
        FileGet(402, cnum);
        for (var k = 0; k < cnum - 1; k++)
        {
            var nuovo = posto();
            clist[k] = nuovo;
            LoadRobot(402, nuovo);
            LoadOrganism = nuovo;
            var i = SimOpts.SpeciesNum;
            foundSpecies = false;
            while (i > 0)
            {
                i--;
                if (rob[nuovo].FName == SimOpts.Specie[i].Name)
                {
                    foundSpecies = true;
                    i = 0;
                }
            }

            if (!foundSpecies)
                AddSpecie(rob[nuovo], false);
        }
        VBCloseFile(402);

        if (X > -1 && Y > -1)
            PlaceOrganism(clist, X, Y);

        RemapTies(clist, OList, cnum);

        return LoadOrganism;
    }

    public static async Task LoadSimulation(string path)
    {
        Form1.instance.camfix = false; //Botsareus 2/23/2013 When simulation starts the screen is normailized

        var input = await File.ReadAllTextAsync(path);
        var savedFile = JsonSerializer.Deserialize<SavedSimulation>(input);

        MaxRobs = savedFile.Robots.Count();

        LoadRobots(savedFile.Robots);

        // As of 2.42.8, the sim file is packed.  Every bot stored is guarenteed to exist, yet their bot numbers, when loaded, may be
        // different from the sim they came from.  Thus, we remap all the ties from all the loaded bots.
        RemapAllTies(MaxRobs);

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
        SimOpts.SpeciesNum = savedFile.SpeciesNum;
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
        SimOpts.SpeciesNum = savedFile.SpeciesNum;
        LoadSpecies(savedFile.Species);
        SimOpts.VegFeedingToBody = savedFile.VegFeedingToBody;
        SimOpts.CoefficientStatic = savedFile.CoefficientStatic;
        SimOpts.CoefficientKinetic = savedFile.CoefficientKinetic;
        SimOpts.PlanetEaters = savedFile.PlanetEaters;
        SimOpts.PlanetEatersG = savedFile.PlanetEatersG;
        SimOpts.Viscosity = savedFile.Viscosity;
        SimOpts.Density = savedFile.Density;
        SimOpts.Costs = savedFile.Costs;
        SimOpts.BadWastelevel = savedFile.BadWastelevel;
        SimOpts.chartingInterval = savedFile.ChartingInterval;
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
        LoadTeleporters(savedFile.Teleporters.Where(t => !t.Internet));
        LoadObstacles(savedFile.Obstacles);
        SimOpts.shapesAreVisable = savedFile.ShapesAreVisable;
        SimOpts.allowVerticalShapeDrift = savedFile.AllowVerticalShapeDrift;
        SimOpts.allowHorizontalShapeDrift = savedFile.AllowHorizontalShapeDrift;
        SimOpts.shapesAreSeeThrough = savedFile.ShapesAreSeeThrough;
        SimOpts.shapesAbsorbShots = savedFile.ShapesAbsorbShots;
        SimOpts.shapeDriftRate = savedFile.ShapeDriftRate;
        SimOpts.makeAllShapesTransparent = savedFile.MakeAllShapesTransparent;
        SimOpts.makeAllShapesBlack = savedFile.MakeAllShapesBlack;
        LoadShots(savedFile.Shots);
        RemapAllShots(maxshotarray);
        SimOpts.MaxAbsNum = savedFile.MaxAbsNum;
        SimOpts.EGridWidth = savedFile.EGridWidth;
        SimOpts.EGridEnabled = savedFile.EGridEnabled;
        SimOpts.oldCostX = savedFile.OldCostX;
        SimOpts.DisableMutations = savedFile.DisableMutations;
        SimOpts.SimGUID = savedFile.SimGUID;
        SimOpts.SpeciationGenerationalDistance = savedFile.SpeciationGenerationalDistance;
        SimOpts.SpeciationGeneticDistance = savedFile.SpeciationGeneticDistance;
        SimOpts.EnableAutoSpeciation = savedFile.EnableAutoSpeciation;
        SimOpts.SpeciationMinimumPopulation = savedFile.SpeciationMinimumPopulation;
        SimOpts.SpeciationForkInterval = savedFile.SpeciationForkInterval;
        SimOpts.DisableTypArepro = savedFile.DisableTypArepro;
        strGraphQuery1 = savedFile.StrGraphQuery1;
        strGraphQuery2 = savedFile.StrGraphQuery2;
        strGraphQuery3 = savedFile.StrGraphQuery3;
        strSimStart = savedFile.StrSimStart;
        LoadGraphs(savedFile.Graphs);
        SimOpts.NoWShotDecay = savedFile.NoWShotDecay;

        energydif = savedFile.EnergyDif;
        energydifX = savedFile.EnergyDifX;
        energydifXP = savedFile.EnergyDifXP;
        ModeChangeCycles = savedFile.ModeChangeCycles;
        hidePredOffset = savedFile.HidePredOffset;
        hidepred = savedFile.HidePred;
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

        Form1.instance.lblSaving.Visibility = Visibility.Hidden; //Botsareus 1/14/2014

        //EricL 3/28/2006 This line insures that all the simulation dialog options get set to match the loaded sim
        TmpOpts = SimOpts;
    }

    /// <summary>
    /// top/buttom pattern file move
    /// </summary>
    public static void movefilemulti(string source, string Out, int count)
    {
        var i = 0;

        foreach (var file in Directory.GetFiles(source).OrderBy(s => System.IO.Path.GetFileName(s)))
        {
            if (i >= count)
                return;

            File.Move(file, System.IO.Path.Join(Out, file));
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
    public static void PlaceOrganism(int[] clist, double x, double y)
    {
        var k = 0;
        var dx = x - rob[clist[0]].pos.X;
        var dy = y - rob[clist[0]].pos.Y;

        while (clist[k] > 0)
        {
            rob[clist[k]].pos.X = rob[clist[k]].pos.X + dx;
            rob[clist[k]].pos.Y = rob[clist[k]].pos.Y + dy;
            rob[clist[k]].BucketPos.X = -2;
            rob[clist[k]].BucketPos.Y = -2;
            UpdateBotBucket(clist[k]);
            k++;
        }
    }

    public static void RemapAllShots(int numOfShots)
    {
        for (var i = 1; i < numOfShots; i++)
        {
            if (Shots[i].exist)
            {
                for (var j = 1; j < MaxRobs; j++)
                {
                    if (rob[j].exist)
                    {
                        if (Shots[i].parent == rob[j].oldBotNum)
                        {
                            Shots[i].parent = j;
                            if (Shots[i].stored)
                            {
                                rob[j].virusshot = i;
                            }
                            break;
                        }
                    }
                }
                Shots[i].stored = false; // Could not find parent.  Should probalby never happen but if it does, release the shot
            }
        }
    }

    public static void RemapAllTies(int numOfBots)
    {
        for (var i = 1; i < numOfBots; i++)
        {
            var j = 1;
            while (rob[i].Ties[j].pnt > 0)
            { // Loop through each tie
                for (var k = 1; k < numOfBots; k++)
                {
                    if (rob[i].Ties[j].pnt == rob[k].oldBotNum)
                    {
                        rob[i].Ties[j].pnt = k;
                        break;
                    }
                }

                j++;
            }
        }
    }

    /// <summary>
    /// Remaps ties from the old index numbers - those the robots had when saved to disk- to the new indexes assigned in this simulation
    /// </summary>
    public static void RemapTies(int[] clist, int[] olist, int cnum)
    {
        for (var t = 0; t < cnum - 1; t++)
        { // Loop through each cell
            var ind = rob[clist[t]].oldBotNum;
            for (var k = 0; k < cnum - 1; k++)
            { // Loop through each cell
                var j = 1;
                while (rob[clist[k]].Ties[j].pnt > 0)
                { // Loop through each tie
                    if (rob[clist[k]].Ties[j].pnt == ind)
                        rob[clist[k]].Ties[j].pnt = clist[t];

                    j++;
                }
            }
        }

        for (var k = 0; k < cnum - 1; k++)
        { // All cells
            var j = 1;
            while (rob[clist[k]].Ties[j].pnt > 0)
            { //All Ties
                var TiePointsToNode = false;
                for (var t = 0; t < cnum - 1; t++)
                {
                    if (rob[clist[k]].Ties[j].pnt == clist[t])
                        TiePointsToNode = true;
                }

                if (!TiePointsToNode)
                    rob[clist[k]].Ties[j].pnt = 0;

                j++;
            }
        }
    }

    public static async Task Save_mrates(mutationprobs mut, string FName)
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
    public static void SaveOrganism(string path, int r)
    {
        var clist = new int[51];

        var k = 0;
        var cnum = 0;

        clist[0] = r;
        ListCells(clist);

        while (clist[cnum] > 0)
            cnum++;

        VBCloseFile(401);
        VBOpenFile(401, path); ;
        FilePut(401, cnum);
        for (k = 0; k < cnum - 1; k++)
        {
            rob[clist[k]].LastOwner = IntOpts.IName;
            SaveRobotBody(401, clist[k]);
        }
        VBCloseFile(401);
        return;
    }

    /// <summary>
    /// Saves a small file with per species population information.  Used for aggregating the population stats from multiple connected sims.
    /// </summary>
    public static async Task SaveSimPopulation(string path)
    {
        var data = new PopulationData
        {
            IName = IName,
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
            AllowHorizontalShapeDrift = SimOpts.allowHorizontalShapeDrift,
            AllowVerticalShapeDrift = SimOpts.allowVerticalShapeDrift,
            BadWastelevel = SimOpts.BadWastelevel,
            BlockedVegs = SimOpts.BlockedVegs,
            ChartingInterval = SimOpts.chartingInterval,
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
            Graphs = Enumerable.Range(0, NUMGRAPHS).Select(SaveGraphs),
            HidePred = hidepred,
            HidePredOffset = hidePredOffset,
            KillDistVegs = SimOpts.KillDistVegs,
            LightIntensity = SimOpts.LightIntensity,
            MakeAllShapesBlack = SimOpts.makeAllShapesBlack,
            MakeAllShapesTransparent = SimOpts.makeAllShapesTransparent,
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
            Obstacles = Obstacles.Obstacles.Select(SaveObstacles),
            OldCostX = SimOpts.oldCostX,
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
            Robots = rob.Where(r => r.exist).Select(SaveRobots),
            ShapeDriftRate = SimOpts.shapeDriftRate,
            ShapesAbsorbShots = SimOpts.shapesAbsorbShots,
            ShapesAreSeeThrough = SimOpts.shapesAreSeeThrough,
            ShapesAreVisable = SimOpts.shapesAreVisable,
            Shots = Shots.Select(SaveShots),
            SimGUID = SimOpts.SimGUID,
            SimName = SimOpts.SimName,
            SnpExcludeVegs = SimOpts.SnpExcludeVegs,
            SpeciationForkInterval = SimOpts.SpeciationForkInterval,
            SpeciationGenerationalDistance = SimOpts.SpeciationGenerationalDistance,
            SpeciationGeneticDistance = SimOpts.SpeciationGeneticDistance,
            SpeciationMinimumPopulation = SimOpts.SpeciationMinimumPopulation,
            Species = SimOpts.Specie.Select(SaveSpecies),
            SpeciesNum = SimOpts.SpeciesNum,
            Stagnent = stagnent,
            StrGraphQuery1 = strGraphQuery1,
            StrGraphQuery2 = strGraphQuery2,
            StrGraphQuery3 = strGraphQuery3,
            StrSimStart = strSimStart,
            SunChange = SunChange,
            SunDown = SimOpts.SunDown,
            SunDownThreshold = SimOpts.SunDownThreshold,
            SunOnRnd = SimOpts.SunOnRnd,
            SunPosition = SunPosition,
            SunRange = SunRange,
            SunThresholdMode = SimOpts.SunThresholdMode,
            SunUp = SimOpts.SunUp,
            SunUpThreshold = SimOpts.SunUpThreshold,
            Teleporters = Teleporters.Select(SaveTeleporters),
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

    private static void LoadGraphs(IEnumerable<SavedGraph> graphs)
    {
        var graphsArray = graphs.ToArray();

        for (var i = 0; i < graphsArray.Length; i++)
        {
            var g = graphsArray[i];

            graphfilecounter[i] = g.FileCounter;
            graphvisible[i] = g.Visible;
            graphleft[i] = g.Left;
            graphtop[i] = g.Top;
            graphsave[i] = g.Save;

            if (g.Visible)
            {
                switch (i)
                {
                    case 1:
                        Form1.instance.NewGraph(POPULATION_GRAPH, "Populations");
                        break;

                    case 2:
                        Form1.instance.NewGraph(MUTATIONS_GRAPH, "Average_Mutations");
                        break;

                    case 3:
                        Form1.instance.NewGraph(AVGAGE_GRAPH, "Average_Age");
                        break;

                    case 4:
                        Form1.instance.NewGraph(OFFSPRING_GRAPH, "Average_Offspring");
                        break;

                    case 5:
                        Form1.instance.NewGraph(ENERGY_GRAPH, "Average_Energy");
                        break;

                    case 6:
                        Form1.instance.NewGraph(DNALENGTH_GRAPH, "Average_DNA_length");
                        break;

                    case 7:
                        Form1.instance.NewGraph(DNACOND_GRAPH, "Average_DNA_Cond_statements");
                        break;

                    case 8:
                        Form1.instance.NewGraph(MUT_DNALENGTH_GRAPH, "Average_Mutations_per_DNA_length_x1000-");
                        break;

                    case 9:
                        Form1.instance.NewGraph(ENERGY_SPECIES_GRAPH, "Total_Energy_per_Species_x1000-");
                        break;

                    case 10:
                        Form1.instance.NewGraph(DYNAMICCOSTS_GRAPH, "Dynamic_Costs");
                        break;

                    case 11:
                        Form1.instance.NewGraph(SPECIESDIVERSITY_GRAPH, "Species_Diversity");
                        break;

                    case 12:
                        Form1.instance.NewGraph(AVGCHLR_GRAPH, "Average_Chloroplasts");
                        break;

                    case 13:
                        Form1.instance.NewGraph(GENETIC_DIST_GRAPH, "Genetic_Distance_x1000-");
                        break;

                    case 14:
                        Form1.instance.NewGraph(GENERATION_DIST_GRAPH, "Max_Generational_Distance");
                        break;

                    case 15:
                        Form1.instance.NewGraph(GENETIC_SIMPLE_GRAPH, "Simple_Genetic_Distance_x1000-");
                        break;

                    case 16:
                        Form1.instance.NewGraph(CUSTOM_1_GRAPH, "Customizable_Graph_1-");
                        break;

                    case 17:
                        Form1.instance.NewGraph(CUSTOM_2_GRAPH, "Customizable_Graph_2-");
                        break;

                    case 18:
                        Form1.instance.NewGraph(CUSTOM_3_GRAPH, "Customizable_Graph_3-");
                        break;
                }
            }
        }
    }

    private static void LoadObstacles(IEnumerable<SavedObstacle> obstacles)
    {
        foreach (var o in obstacles)
        {
            Obstacles.Obstacles.Add(new Obstacle
            {
                exist = o.Exist,
                pos = o.Position,
                Width = o.Width,
                Height = o.Height,
                color = o.Color,
                vel = o.Velocity
            });
        }
    }

    private static void LoadRobots(IEnumerable<SavedRobot> robots)
    {
        sunbelt = robots.First().SunBelt;

        rob.AddRange(robots.Select(r => new robot
        {
            Veg = r.Veg,
            wall = r.Wall,
            Fixed = r.Fixed,
            pos = r.Position,
            vel = r.Velocity,
            aim = r.Aim,
            ma = r.AngularMomentum,
            mt = r.Torque,
            Ties = r.Ties.Select(LoadTies).ToArray(),
            nrg = r.Energy,
            vars = r.Variables,
            vnum = r.VariableNumber,
            mem = r.Memory,
            dna = r.Dna,
            Mutables = new mutationprobs
            {
                mutarray = r.MutationArray,
                Mutations = r.MutationsProbs,
                Mean = r.MutationsMeans,
                StdDev = r.MutationsStdDevs,
                CopyErrorWhatToChange = r.CopyErrorWhatToChange,
                PointWhatToChange = r.PointWhatToChange
            },
            SonNumber = r.SonNumber,
            Mutations = r.Mutations,
            LastMut = r.LastMutation,
            parent = r.Parent,
            age = r.Age,
            BirthCycle = r.BirthCycle,
            genenum = r.GeneNumber,
            generation = r.Generation,
            Skin = r.Skin,
            body = r.Body,
            Bouyancy = r.Bouyancy,
            Corpse = r.Corpse,
            Pwaste = r.PermanentWaste,
            Waste = r.Waste,
            poison = r.Poison,
            venom = r.Venom,
            oldBotNum = r.OldBotNumber,
            exist = r.Exist,
            Dead = r.Dead,
            FName = r.SpeciesName,
            LastOwner = r.LastOwner,
            LastMutDetail = r.LastMutationDetail,
            View = r.View,
            NewMove = r.NewMove,
            CantSee = r.CantSee,
            DisableDNA = r.DisableDna,
            DisableMovementSysvars = r.DisableMovementSysvars,
            CantReproduce = r.CantReproduce,
            shell = r.Shell,
            Slime = r.Slime,
            VirusImmune = r.VirusImmune,
            SubSpecies = r.SubSpecies,
            spermDNA = r.SpermDNA,
            spermDNAlen = r.SpermDNA.Length,
            fertilized = r.Fertilized,
            sim = r.Sim,
            AbsNum = r.AbsoluteNumber,
            OldGD = r.OldGeneticDistance,
            epimem = r.EpigeneticMemory,
            tag = r.Tag,
            NoChlr = r.NoChloroplasts,
            multibot_time = r.MultiBotTime,
            Chlr_Share_Delay = r.ChloroplastShareDelay,
            dq = r.Dq,
            OldMutations = r.OldMutations,
            actvel = r.ActualVelocity,
        }));

        for (var n = 0; n < rob.Count; n++)
        {
            if (rob[n].exist)
            {
                GiveAbsNum(n);
                insertsysvars(n);
                ScanUsedVars(n);
                makeoccurrlist(n);
                rob[n].DnaLen = DnaLen(rob[n].dna);
                rob[n].genenum = CountGenes(rob[n].dna);
                rob[n].mem[DnaLenSys] = rob[n].DnaLen;
                rob[n].mem[GenesSys] = rob[n].genenum;
            }
        }
    }

    private static void LoadShots(IEnumerable<SavedShot> savedShots)
    {
        foreach (var s in savedShots)
        {
            Shots.Add(new shot
            {
                exist = s.Exists,
                pos = s.Position,
                opos = s.OPosition,
                velocity = s.Velocity,
                parent = s.Parent,
                age = s.Age,
                nrg = s.Energy,
                Range = s.Range,
                value = s.Value,
                color = s.Color,
                shottype = s.ShotType,
                fromveg = s.FromVeg,
                FromSpecie = s.FromSpecies,
                memloc = s.MemoryLocation,
                Memval = s.MemoryValue,
                dna = s.Dna,
                DnaLen = s.Dna.Length,
                genenum = s.GeneNumber,
                stored = s.Stored,
            });
        }
    }

    private static void LoadSpecies(IEnumerable<SavedSpecies> species)
    {
        SimOpts.Specie.AddRange(species.Select(s => new datispecie
        {
            Colind = s.Colind,
            color = s.Color,
            Fixed = s.Fixed,
            Mutables = new mutationprobs
            {
                mutarray = s.MutArray,
                Mutations = s.Mutations,
                CopyErrorWhatToChange = s.CopyErrorWhatToChange,
                PointWhatToChange = s.PointWhatToChange,
                Mean = s.MutationsMeans,
                StdDev = s.MutationsStdDevs
            },
            Name = s.Name,
            path = s.Path,
            Posdn = s.Posdn,
            Posrg = s.Posrg,
            Poslf = s.Poslf,
            Postp = s.Postp,
            qty = s.Qty,
            Skin = s.Skin,
            Stnrg = s.Stnrg,
            Veg = s.Veg,
            CantSee = s.CantSee,
            DisableDNA = s.DisableDNA,
            DisableMovementSysvars = s.DisableMovementSysvars,
            VirusImmune = s.VirusImmune,
            population = s.Population,
            SubSpeciesCounter = s.SubSpeciesCounter,
            Native = s.Native
        }));
    }

    private static void LoadTeleporters(IEnumerable<SavedTeleporter> teleporters)
    {
        foreach (var t in teleporters)
        {
            Teleporters.Add(new Teleporter
            {
                pos = t.Position,
                Width = t.Width,
                Height = t.Height,
                color = t.Color,
                vel = t.Velocity,
                path = t.Path,
                In = t.In,
                = t.Out,
                local = t.Local,
                driftHorizontal = t.DriftHorizontal,
                driftVertical = t.DriftVertical,
                highlight = t.Highlight,
                teleportVeggies = t.TeleportVeggies,
                teleportCorpses = t.TeleportCorpses,
                RespectShapes = t.RespectShapes,
                teleportHeterotrophs = t.TeleportHeterotrophs,
                InboundPollCycles = t.InboundPollCycles,
                BotsPerPoll = t.BotsPerPoll,
                PollCountDown = t.PollCountDown,
                Internet = t.Internet
            });
        }
    }

    private static tie LoadTies(SavedTie t)
    {
        return new tie
        {
            Port = t.Port,
            pnt = t.TargetBot,
            ptt = t.BackTie,
            ang = t.Angle,
            bend = t.Bend,
            angreg = t.FixedAngle,
            ln = t.Length,
            shrink = t.Shrink,
            stat = t.Stat,
            last = t.Last,
            mem = t.Mem,
            back = t.Back,
            nrgused = t.EnergyUsed,
            infused = t.InfoUsed,
            sharing = t.Sharing,
            type = t.Type,
            b = t.b,
            k = t.k,
            NaturalLength = t.NaturalLength
        };
    }

    /// <summary>
    /// Saves a robot's dna.
    /// </summary>
    private static async Task salvarob(int n, string path)
    {
        var hold = new StringBuilder();

        string hashed = "";

        hold.AppendLine(SaveRobHeader(n));

        //Botsareus 10/8/2015 New code to save epigenetic memory as gene

        if (UseEpiGene)
        {
            var started = false;

            for (var a = 971; a <= 990; a++)
            {
                if (rob[n].mem[a] != 0)
                {
                    if (!started)
                        hold.AppendLine("start");

                    started = true;

                    hold.AppendLine($"{rob[n].mem[a]} {a} store");
                }
            }

            if (started)
                hold.AppendLine("stop");
        }

        savingtofile = true; //Botsareus 2/28/2014 when saving to file the def sysvars should not save
        hold.AppendLine(DetokenizeDNA(n, 0));
        savingtofile = false;

        hashed = Hash(hold.ToString());

        hold.AppendLine();
        hold.AppendLine($"'#hash: {hashed}");

        if (!string.IsNullOrWhiteSpace(rob[n].tag))
            hold.AppendLine($"'#tag: {rob[n].tag.Substring(0, 45)}");

        //Botsareus 12/11/2013 Save mrates file
        await Save_mrates(rob[n].Mutables, System.IO.Path.Join(System.IO.Path.GetDirectoryName(path), System.IO.Path.GetFileNameWithoutExtension(path) + ".mrate"));

        if (x_restartmode > 0)
            return;

        if (MessageBox.Show($"Do you want to change robot's name to {System.IO.Path.GetFileNameWithoutExtension(path)} ?", "Robot DNA saved", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            rob[n].FName = System.IO.Path.GetFileNameWithoutExtension(path);
    }

    private static SavedGraph SaveGraphs(int i)
    {
        return new SavedGraph
        {
            FileCounter = graphfilecounter[i],
            Visible = graphvisible[i],
            Left = graphleft[i],
            Top = graphtop[i],
            Save = graphsave[i]
        };
    }

    private static SavedObstacle SaveObstacles(Obstacle o)
    {
        return new SavedObstacle
        {
            Exist = o.exist,
            Position = o.pos,
            Width = o.Width,
            Height = o.Height,
            Color = o.color,
            Velocity = o.vel
        };
    }

    private static SavedRobot SaveRobots(robot r, int index)
    {
        if (!r.Veg && y_eco_im > 0 && r.dq < 2)
        {
            if (string.IsNullOrWhiteSpace(r.tag))
                r.tag = r.FName;
            r.tag += (r.nrg.ToString() + r.nrg.ToString()).Substring(0, 5);
        }

        return new SavedRobot
        {
            Veg = r.Veg,
            Wall = r.wall,
            Fixed = r.Fixed,
            Position = r.pos,
            Velocity = r.vel,
            Aim = r.aim,
            AngularMomentum = r.ma,
            Torque = r.mt,
            Ties = r.Ties.Select(SaveTies),
            Energy = r.nrg,
            Variables = r.vars,
            VariableNumber = r.vnum,
            Memory = r.mem,
            Dna = r.dna,
            MutationArray = r.Mutables.mutarray,
            SonNumber = r.SonNumber,
            Mutations = r.Mutations,
            LastMutation = r.LastMut,
            Parent = r.parent,
            Age = r.age,
            BirthCycle = r.BirthCycle,
            GeneNumber = r.genenum,
            Generation = r.generation,
            Skin = r.Skin,
            Body = r.body,
            Bouyancy = r.Bouyancy,
            Corpse = r.Corpse,
            PermanentWaste = r.Pwaste,
            Waste = r.Waste,
            Poison = r.poison,
            Venom = r.venom,
            OldBotNumber = index,
            Exist = r.exist,
            Dead = r.Dead,
            SpeciesName = r.FName,
            LastOwner = r.LastOwner,
            LastMutationDetail = r.LastMutDetail,
            MutationsProbs = r.Mutables.Mutations,
            MutationsMeans = r.Mutables.Mean,
            MutationsStdDevs = r.Mutables.StdDev,
            CopyErrorWhatToChange = r.Mutables.CopyErrorWhatToChange,
            PointWhatToChange = r.Mutables.PointWhatToChange,
            View = r.View,
            NewMove = r.NewMove,
            CantSee = r.CantSee,
            DisableDna = r.DisableDNA,
            DisableMovementSysvars = r.DisableMovementSysvars,
            CantReproduce = r.CantReproduce,
            Shell = r.shell,
            Slime = r.Slime,
            VirusImmune = r.VirusImmune,
            SubSpecies = r.SubSpecies,
            SpermDNA = r.spermDNA,
            Fertilized = r.fertilized,
            Sim = r.sim,
            AbsoluteNumber = r.AbsNum,
            OldGeneticDistance = r.OldGD,
            EpigeneticMemory = r.epimem,
            Tag = r.tag,
            SunBelt = sunbelt,
            NoChloroplasts = r.NoChlr,
            MultiBotTime = r.multibot_time,
            ChloroplastShareDelay = r.Chlr_Share_Delay,
            Dq = r.dq,
            OldMutations = r.OldMutations,
            ActualVelocity = r.actvel,
        };
    }

    private static SavedShot SaveShots(shot s)
    {
        return new SavedShot
        {
            Exists = s.exist,
            Position = s.pos,
            OPosition = s.opos,
            Velocity = s.velocity,
            Parent = s.parent,
            Age = s.age,
            Energy = s.nrg,
            Range = s.Range,
            Value = s.value,
            Color = s.color,
            ShotType = s.shottype,
            FromVeg = s.fromveg,
            FromSpecies = s.FromSpecie,
            MemoryLocation = s.memloc,
            MemoryValue = s.Memval,
            Dna = s.dna,
            GeneNumber = s.genenum,
            Stored = s.stored,
        };
    }

    private static SavedSpecies SaveSpecies(datispecie s)
    {
        return new SavedSpecies
        {
            Colind = s.Colind,
            Color = s.color,
            Fixed = s.Fixed,
            MutArray = s.Mutables.mutarray,
            Mutations = s.Mutables.Mutations,
            Name = s.Name,
            Path = s.path,
            Qty = s.qty,
            Skin = s.Skin,
            Stnrg = s.Stnrg,
            Veg = s.Veg,
            CopyErrorWhatToChange = s.Mutables.CopyErrorWhatToChange,
            PointWhatToChange = s.Mutables.PointWhatToChange,
            MutationsMeans = s.Mutables.Mean,
            MutationsStdDevs = s.Mutables.StdDev,
            Poslf = s.Poslf,
            Posrg = s.Posrg,
            Postp = s.Postp,
            Posdn = s.Posdn,
            CantSee = s.CantSee,
            DisableDNA = s.DisableDNA,
            DisableMovementSysvars = s.DisableMovementSysvars,
            CantReproduce = s.CantReproduce,
            VirusImmune = s.VirusImmune,
            Population = s.population,
            SubSpeciesCounter = s.SubSpeciesCounter,
            Native = s.Native
        };
    }

    private static SavedTeleporter SaveTeleporters(Teleporter t)
    {
        return new SavedTeleporter
        {
            Position = t.pos,
            Width = t.Width,
            Height = t.Height,
            Color = t.color,
            Velocity = t.vel,
            Path = t.path,
            In = t.In,
            = t.Out,
            Local = t.local,
            DriftHorizontal = t.driftHorizontal,
            DriftVertical = t.driftVertical,
            Highlight = t.highlight,
            TeleportVeggies = t.teleportVeggies,
            TeleportCorpses = t.teleportCorpses,
            RespectShapes = t.RespectShapes,
            TeleportHeterotrophs = t.teleportHeterotrophs,
            InboundPollCycles = t.InboundPollCycles,
            BotsPerPoll = t.BotsPerPoll,
            PollCountDown = t.PollCountDown,
            Internet = t.Internet
        };
    }

    private static SavedTie SaveTies(tie t)
    {
        return new SavedTie
        {
            Port = t.Port,
            TargetBot = t.pnt,
            BackTie = t.ptt,
            Angle = t.ang,
            Bend = t.bend,
            FixedAngle = t.angreg,
            Length = t.ln,
            Shrink = t.shrink,
            Stat = t.stat,
            Last = t.last,
            Mem = t.mem,
            Back = t.back,
            EnergyUsed = t.nrgused,
            InfoUsed = t.infused,
            Sharing = t.sharing,
            Type = t.type,
            b = t.b,
            k = t.k,
            NaturalLength = t.NaturalLength
        };
    }
}
                                                                                                                                    }
