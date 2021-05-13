using VB6 = Microsoft.VisualBasic.Compatibility.VB6;
using System.Runtime.InteropServices;
using static VBExtension;
using static VBConstants;
using Microsoft.VisualBasic;
using System;
using System.Windows;
using System.Windows.Controls;
using static System.DateTime;
using static System.Math;
using static Microsoft.VisualBasic.Globals;
using static Microsoft.VisualBasic.Collection;
using static Microsoft.VisualBasic.Constants;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.DateAndTime;
using static Microsoft.VisualBasic.ErrObject;
using static Microsoft.VisualBasic.FileSystem;
using static Microsoft.VisualBasic.Financial;
using static Microsoft.VisualBasic.Information;
using static Microsoft.VisualBasic.Interaction;
using static Microsoft.VisualBasic.Strings;
using static Microsoft.VisualBasic.VBMath;
using System.Collections.Generic;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.ColorConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.DrawStyleConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.FillStyleConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.GlobalModule;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.Printer;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.PrinterCollection;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.PrinterObjectConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.ScaleModeConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.SystemColorConstants;
using ADODB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using DBNet.Forms;
using static stringops;
using static varspecie;
using static stayontop;
using static localizzazione;
using static SimOptModule;
using static Common;
using static Flex;
using static Robots;
using static Ties;
using static Shots_Module;
using static Globals;
using static Physics;
using static F1Mode;
using static DNAExecution;
using static Vegs;
using static Senses;
using static Multibots;
using static HDRoutines;
using static Scripts;
using static Database;
using static Buckets_Module;
using static NeoMutations;
using static Master;
using static DNAManipulations;
using static DNATokenizing;
using static Bitwise;
using static Obstacles;
using static Teleport;
using static IntOpts;
using static stuffcolors;
using static Evo;
using static DBNet.Forms.MDIForm1;
using static DBNet.Forms.datirob;
using static DBNet.Forms.InfoForm;
using static DBNet.Forms.ColorForm;
using static DBNet.Forms.parentele;
using static DBNet.Forms.Consoleform;
using static DBNet.Forms.frmAbout;
using static DBNet.Forms.optionsform;
using static DBNet.Forms.NetEvent;
using static DBNet.Forms.grafico;
using static DBNet.Forms.ActivForm;
using static DBNet.Forms.Form1;
using static DBNet.Forms.Contest_Form;
using static DBNet.Forms.DNA_Help;
using static DBNet.Forms.MutationsProbability;
using static DBNet.Forms.PhysicsOptions;
using static DBNet.Forms.CostsForm;
using static DBNet.Forms.EnergyForm;
using static DBNet.Forms.ObstacleForm;
using static DBNet.Forms.TeleportForm;
using static DBNet.Forms.frmGset;
using static DBNet.Forms.frmMonitorSet;
using static DBNet.Forms.frmPBMode;
using static DBNet.Forms.frmRestriOps;
using static DBNet.Forms.frmEYE;
using static DBNet.Forms.frmFirstTimeInfo;
using System.IO;
using Iersera.DataModel;
using System.Text.Json;
using System.Reflection;

static class HDRoutines
{
    // Option Explicit
    //   D I S K    O P E R A T I O N S


    public static void movetopos(string s, int pos)
    { //Botsareus 3/7/2014 Used in Stepladder to move files in specific order

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

    public static async Task deseed(string s)
    { //Botsareus 2/25/2014 Used in Tournament to get back original names of the files and move to result folder
        var files = Directory.GetFiles(s);

        for (var i = 1; i < files.Length; i++)
        {
            var lastLine = (await File.ReadAllLinesAsync(files[i])).Last();
            lastLine = lastLine.Replace("'#tag:", "");
            File.Copy(files[i], $@"\league\Tournament_Results\{lastLine}");
        }
    }

    public static string NamefileRecursive(string s)
    {
        //Botsareus 1/31/2014 .txt files only

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

    public static void movefilemulti(string source, string Out, int count)
    { //Botsareus 2/18/2014 top/buttom pattern file move

        var last = false;

        var i = 0;

        foreach (var file in Directory.GetFiles(source).OrderBy(s => System.IO.Path.GetFileName(s)))
        {
            if (i >= count)
                return;

            File.Move(file, System.IO.Path.Join(Out, file));
        }
    }

    /*
    ' inserts organism file in the simulation
    ' remember that organisms could be made of more than one robot
    */
    public static void InsertOrganism(string path)
    {

        var X = Random(60, SimOpts.FieldWidth - 60); //Botsareus 2/24/2013 bug fix: robots location within screen limits
        var Y = Random(60, SimOpts.FieldHeight - 60);
        LoadOrganism(path, X, Y);
    }

    /*
    ' saves organism file
    */
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

    /*
    'Adds a record to the species array when a bot with a new species is loaded or teleported in
    */
    public static int AddSpecie(ref int n, ref bool IsNative)
    {
        if (rob[n].Corpse || rob[n].FName == "Corpse" || rob[n].exist == false)
        {
            AddSpecie = 0;
            return AddSpecie;
        }

        var k = SimOpts.SpeciesNum;

        if (k < MAXNATIVESPECIES)
            SimOpts.SpeciesNum = SimOpts.SpeciesNum + 1;

        SimOpts.Specie[k].Name = rob[n].FName;
        SimOpts.Specie[k].Veg = rob[n].Veg;
        SimOpts.Specie[k].CantSee = rob[n].CantSee;
        SimOpts.Specie[k].DisableMovementSysvars = rob[n].DisableMovementSysvars;
        SimOpts.Specie[k].DisableDNA = rob[n].DisableDNA;
        SimOpts.Specie[k].CantReproduce = rob[n].CantReproduce;
        SimOpts.Specie[k].VirusImmune = rob[n].VirusImmune;
        SimOpts.Specie[k].population = 1;
        SimOpts.Specie[k].SubSpeciesCounter = 0;
        SimOpts.Specie[k].color = rob[n].color;
        SimOpts.Specie[k].Comment = "Species arrived from the Internet";
        SimOpts.Specie[k].Posrg = 1;
        SimOpts.Specie[k].Posdn = 1;
        SimOpts.Specie[k].Poslf = 0;
        SimOpts.Specie[k].Postp = 0;

        SetDefaultMutationRates(SimOpts.Specie[k].Mutables);
        SimOpts.Specie[k].Mutables.Mutations = rob[n].Mutables.Mutations;
        SimOpts.Specie[k].qty = 5;
        SimOpts.Specie[k].Stnrg = 3000;
        SimOpts.Specie[k].Native = IsNative;
        SimOpts.Specie[k].path = MDIForm1.instance.MainDir + "\\robots";

        return k;
    }

    /*
    ' loads an organism file
    */
    public static int LoadOrganism(string path, double X, double Y)
    {
        var LoadOrganism = 0;
        var clist = new int[51];

        var OList = new int[51];

        var cnum = 0;

        var foundSpecies = false;

        VBCloseFile(402);
        VBOpenFile(402, path);
        FileGet(402, ref cnum);
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
            {
                AddSpecie(nuovo, false);
            }

        }
        VBCloseFile(402);

        if (X > -1 && Y > -1)
            PlaceOrganism(clist, X, Y);

        RemapTies(clist, OList, cnum);

        return LoadOrganism;
    }

    /*
    ' places an organism (made of robots listed in clist())
    ' in the specified x,y position
    */
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

    /*
    ' remaps ties from the old index numbers - those the robots had
    ' when saved to disk- to the new indexes assigned in this simulation
    */
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

    /*
    'Saves a small file with per species population informaton
    'Used for aggregating the population stats from multiple connected sims
    */
    public static void SaveSimPopulation(string path)
    {
        int X = 0;

        int numSpecies = 0;

        const byte Fe = 254;


        Form1.instance.MousePointer = vbHourglass;
        // TODO (not supported):   On Error GoTo bypass

        VBOpenFile(10, path); ;

        FilePut(10, Len(IntOpts.IName));
        FilePut(10, IntOpts.IName);

        numSpecies = 0;
        for (X = 0; X < SimOpts.SpeciesNum - 1; X++)
        {
            if (SimOpts.Specie[x].population > 0)
            {
                numSpecies = numSpecies + 1;
            }
        }

        FilePut(10, numSpecies); // Only save non-zero populations


        for (X = 0; X < SimOpts.SpeciesNum - 1; X++)
        {
            if (SimOpts.Specie[x].population > 0)
            {
                FilePut(10, Len(SimOpts.Specie[x].Name));
                FilePut(10, SimOpts.Specie[x].Name);
                FilePut(10, SimOpts.Specie[x].population);
                FilePut(10, SimOpts.Specie[x].Veg);
                FilePut(10, SimOpts.Specie[x].color);

                //write any future data here

                //Record ending bytes
                FilePut(10, Fe);
                FilePut(10, Fe);
                FilePut(10, Fe);
            }

        }


        VBCloseFile(10);
        Form1.instance.MousePointer = vbArrow;

    }

    public static string GetFilePath(string FileName)
    {
        var fi = new FileInfo(FileName);
        return fi.Name;
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
            Out = t.Out,
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

    private static SavedShot SaveRobots(robot r)
    {
        throw new NotImplementedException();
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

    /*
    ' saves a whole simulation
    */
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
            CostExecCond = (double)SimOpts.CostExecCond,
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
            Robots = rob.Where(r => r.exist).Select(SaveRobots),
        };

        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
        File.WriteAllText(path, JsonSerializer.Serialize(sim));
    }

    /*
    'Botsareus 3/15/2013 load global settings
    */
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

    /*
    ' loads a whole simulation
    */
    public static async Task LoadSimulation(string path)
    {
        Form1.instance.camfix = false; //Botsareus 2/23/2013 When simulation starts the screen is normailized

        var input = await File.ReadAllTextAsync(path);
        var savedFile = JsonSerializer.Deserialize<SavedSimulation>(input);


        Form1.MousePointer = vbHourglass;

        MaxRobs = savedFile.Robots.Count();

        Form1.instance.lblSaving.Visible = true; //Botsareus 1/14/2014 New code to display load status
        Form1.instance.Visible = true;

        await LoadRobots(savedFile.Robots);

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

        Form1.instance.MousePointer = vbArrow;
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
                Out = t.Out,
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
            DisableMovementSysvars = s.DisableMovem,
            VirusImmune = s.VirusImmune,
            population = s.Population,
            SubSpeciesCounter = s.SubSpeciesCounter,
            Native = s.Native
        }));
    }

    /*
    ' loads a single robot
    */
    public static void LoadRobot(ref int fnum, int n)
    {
        LoadRobotBody(fnum, n);
        if (rob[n].exist)
        {
            GiveAbsNum(n);
            insertsysvars(n);
            ScanUsedVars(n);
            makeoccurrlist(n);
            rob[n].DnaLen = DnaLen(ref rob[n].dna());
            rob[n].genenum = CountGenes(ref rob[n].dna());
            rob[n].mem(DnaLenSys) = rob[n].DnaLen;
            rob[n].mem(GenesSys) = rob[n].genenum;
            // UpdateBotBucket n
        }
    }

    /*
    ' assignes a robot his unique code
    */
    public static void GiveAbsNum(ref int k)
    {
        // Dim n As Integer, Max As Long
        //For n = 1 To MaxRobs
        //  If Max < rob[n].AbsNum Then
        //    Max = rob[n].AbsNum
        //  End If
        //Next n
        //rob[k].AbsNum = Max + 1
        if (rob[k].AbsNum == 0)
        {
            SimOpts.MaxAbsNum = SimOpts.MaxAbsNum + 1;
            rob[k].AbsNum = SimOpts.MaxAbsNum;
        }
    }

    /*
    ' loads the body of the robot
    */
    private static void LoadRobotBody(ref int n, ref int r)
    {
        //robot r
        //file #n,
        int t = 0;
        int k = 0;
        int ind = 0;
        byte Fe = 0;
        int L1 = 0;
        int inttmp = 0;

        bool MessedUpMutations = false;

        int longtmp = 0;//Botsareus 10/5/2015 freeing up memory from Eric's obsolete ancestors code


        MessedUpMutations = false;
        dynamic _WithVar_2822;
        _WithVar_2822 = rob(r);
        Get(n);
        Get(n);
        Get(n);

        Get(n);
        Get(n);
        Get(n);
        Get(n);
        Get(n);
        Get(n); //momento angolare
        Get(n); //momento torcente

        _WithVar_2822.BucketPos.X = -2;
        _WithVar_2822.BucketPos.Y = -2;

        //ties
        for (t = 0; t < MAXTIES; t++)
        {
            Get(n);
            Get(n);
            Get(n);
            Get(n);
            Get(n);
            Get(n);
            Get(n);
            Get(n);
            Get(n);
            Get(n);
            Get(n);
            Get(n);
            Get(n);
            Get(n);
            Get(n);
        }

        Get(n);

        for (t = 1; t < 50; t++)
        {
            Get(n);
            _WithVar_2822.vars(t).Name = Space(k);
            Get(n); //|
            Get(n);
        }
        Get(n); //| variabili private

        // macchina virtuale
        Get(n); // memoria dati
        Get(n);
        List<> dna_4061_tmp = new List<>();
        for (int redim_iter_8013 = 0; i < 0; redim_iter_8013++) { dna.Add(null); }

        for (t = 1; t < k; t++)
        {
            Get(n);
            Get(n);
        }

        //Force an end base pair to protect against DNA corruption
        _WithVar_2822.dna(k).tipo = 10;
        _WithVar_2822.dna(k).value = 1;


        //EricL Set reasonable default values to protect against corrupted sims that don't read these values
        SetDefaultMutationRates(_WithVar_2822.Mutables, true);

        for (t = 0; t < 20; t++)
        {
            Get(n);
        }

        // informative
        Get(n);
        Get(n);
        _WithVar_2822.Mutations = inttmp;
        Get(n);
        _WithVar_2822.LastMut = inttmp;
        Get(n);
        Get(n);
        Get(n);
        Get(n);
        Get(n);
        Get(n);

        // aspetto
        Get(n);
        Get(n);

        //new stuff using FileContinue conditions for backward and forward compatability
        if (FileContinue(n))
        {
            Get(n);
            _WithVar_2822.radius = FindRadius(r);
        }
        if (FileContinue(n))
        {
            Get(n);
        }
        if (FileContinue(n))
        {
            Get(n);
        }
        if (FileContinue(n))
        {
            Get(n);
        }
        if (FileContinue(n))
        {
            Get(n);
        }
        if (FileContinue(n))
        {
            Get(n);
        }
        if (FileContinue(n))
        {
            Get(n);
        }
        if (FileContinue(n))
        {
            Get(n);
        }
        if (FileContinue(n))
        {
            Get(n);
        }
        if (FileContinue(n))
        {
            Get(n);
        }

        if (FileContinue(n))
        {
            Get(n);
            _WithVar_2822.FName = Space(k);
        }
        if (FileContinue(n))
        {
            Get(n);
        }

        if (FileContinue(n))
        {
            Get(n);
            _WithVar_2822.LastOwner = Space(k);
        }
        if (FileContinue(n))
        {
            Get(n);
        }
        if (_WithVar_2822.LastOwner == "")
        {
            _WithVar_2822.LastOwner = "Local";
        }

        if (FileContinue(n))
        {
            Get(n);
        }

        //EricL 5/2/2006  This needs some explaining.  The length of the mutation details can exceed 2^15 -1 for bots with lots
        //of mutations.  If we are reading an old file, the length could be negative in which case we read what we can and then punt and skip the
        //rest of the bot.  We will miss some stuff, like the mutation settings, but at least the sim will load.
        //If the sim file was stored with 2.42.4 or later and this bot has a ton of mutation details, then an Int value of 1
        //indicates the actual length of the mutation details is stored as a Long in which case we read that and continue.
        if (k < 0)
        {
            //Its an old corrupted file with > 2^15 worth of mutation details.  Bail.
            _WithVar_2822.LastMutDetail = "Problem reading mutation details.  May be a very old sim.  Please tell the developers.  Mutation Details deleted.";

            //EricL Set reasonable default values for everything read from this point on.
            _WithVar_2822.Mutables.Mutations = true;

            SetDefaultMutationRates(_WithVar_2822.Mutables, true);

            _WithVar_2822.View = true;
            _WithVar_2822.NewMove = false;
            _WithVar_2822.oldBotNum = 0;
            _WithVar_2822.CantSee = false;
            _WithVar_2822.DisableDNA = false;
            _WithVar_2822.DisableMovementSysvars = false;
            _WithVar_2822.CantReproduce = false;
            _WithVar_2822.VirusImmune = false;
            _WithVar_2822.shell = 0;
            _WithVar_2822.Slime = 0;

            goto ;
        }
        if (k == 1)
        {
            //Its a new file with lots of mutations.  Read the actual length stored as a Long
            Get(n);
        }
        else
        {
            //Not that many mutations for this bot (It's possible its an old file with lots of mutations and the len wrapped.
            //If so, we just read the postiive len and keep going.  Everything following this will be wrong, but the sim should
            //still load.  It's a corner case.  The alternative is to try to parse the mutation details strings directly.  No thanks.
            L1 = CLng(k);
        }

        if (Form1.lblSaving.Visible)
        { //Botsareus 4/18/2016 Bug fix to prevent string buffer overflow
            _WithVar_2822.LastMutDetail = Space(L1);
            if (FileContinue(n))
            {
                Get(n);
            }
        }
        else
        {
            if (L1 > (100000000 / TotalRobotsDisplayed))
            {
                Seek(#n, L1 + Seek(n));
              }
            else
            {
                _WithVar_2822.LastMutDetail = Space(L1);
                if (FileContinue(n))
                {
                    Get(n);
                }
            }
        }

        if (FileContinue(n))
        {
            Get(n);
        }

        for (t = 0; t < 20; t++)
        {
            if (FileContinue(n))
            {
                Get(n);
            }
            if (FileContinue(n))
            {
                Get(n);
            }
        }

        for (t = 0; t < 20; t++)
        {
            if (_WithVar_2822.Mutables.Mean(t) < 0 || _WithVar_2822.Mutables.Mean(t) > 32000 || _WithVar_2822.Mutables.StdDev(t) < 0 || _WithVar_2822.Mutables.StdDev(t) > 32000)
            {
                MessedUpMutations = true;
            }
        }

        if (FileContinue(n))
        {
            Get(n);
        }
        if (FileContinue(n))
        {
            Get(n);
        }

        if (_WithVar_2822.Mutables.CopyErrorWhatToChange < 0 || _WithVar_2822.Mutables.CopyErrorWhatToChange > 32000 || _WithVar_2822.Mutables.PointWhatToChange < 0 || _WithVar_2822.Mutables.PointWhatToChange > 32000)
        {
            MessedUpMutations = true;
        }

        //If we read wacky values, the file was saved with an older version which messed these up.  Set the defaults.
        if (MessedUpMutations)
        {
            SetDefaultMutationRates(_WithVar_2822.Mutables, true);
        }

        if (FileContinue(n))
        {
            Get(n);
        }
        if (FileContinue(n))
        {
            Get(n);
        }

        _WithVar_2822.oldBotNum = 0;
        if (FileContinue(n))
        {
            Get(n);
        }

        _WithVar_2822.CantSee = false;
        if (FileContinue(n))
        {
            Get(n);
        }
        if (CInt(_WithVar_2822.CantSee) > 0 || CInt(_WithVar_2822.CantSee) < -1)
        {
            _WithVar_2822.CantSee = false; // Protection against corrpt sim files.
        }

        _WithVar_2822.DisableDNA = false;
        if (FileContinue(n))
        {
            Get(n);
        }
        if (CInt(_WithVar_2822.DisableDNA) > 0 || CInt(_WithVar_2822.DisableDNA) < -1)
        {
            _WithVar_2822.DisableDNA = false; // Protection against corrpt sim files.
        }

        _WithVar_2822.DisableMovementSysvars = false;
        if (FileContinue(n))
        {
            Get(n);
        }
        if (CInt(_WithVar_2822.DisableMovementSysvars) > 0 || CInt(_WithVar_2822.DisableMovementSysvars) < -1)
        {
            _WithVar_2822.DisableMovementSysvars = false; // Protection against corrpt sim files.
        }

        _WithVar_2822.CantReproduce = false;
        if (FileContinue(n))
        {
            Get(n);
        }
        if (CInt(_WithVar_2822.CantReproduce) > 0 || CInt(_WithVar_2822.CantReproduce) < -1)
        {
            _WithVar_2822.CantReproduce = false; // Protection against corrpt sim files.
        }

        _WithVar_2822.shell = 0;
        if (FileContinue(n))
        {
            Get(n);
        }

        if (_WithVar_2822.shell > 32000)
        {
            _WithVar_2822.shell = 32000;
        }
        if (_WithVar_2822.shell < 0)
        {
            _WithVar_2822.shell = 0;
        }

        _WithVar_2822.Slime = 0;
        if (FileContinue(n))
        {
            Get(n);
        }

        if (_WithVar_2822.Slime > 32000)
        {
            _WithVar_2822.Slime = 32000;
        }
        if (_WithVar_2822.Slime < 0)
        {
            _WithVar_2822.Slime = 0;
        }

        _WithVar_2822.VirusImmune = false;
        if (FileContinue(n))
        {
            Get(n);
        }
        if (CInt(_WithVar_2822.VirusImmune) > 0 || CInt(_WithVar_2822.VirusImmune) < -1)
        {
            _WithVar_2822.VirusImmune = false; // Protection against corrpt sim files.
        }

        _WithVar_2822.SubSpecies = 0; // For older sims saved before this was implemented, set the sup species to be the bot's number.  Every bot is a sub species.
        if (FileContinue(n))
        {
            Get(n);
        }

        _WithVar_2822.spermDNAlen = 0;
        if (FileContinue(n))
        {
            Get(n);
            List<> spermDNA_1155_tmp = new List<>();
            for (int redim_iter_9379 = 0; i < 0; redim_iter_9379++) { spermDNA.Add(null); }
        }
        for (t = 1; t < _WithVar_2822.spermDNAlen; t++)
        {
            if (FileContinue(n))
            {
                Get(n);
            }
            if (FileContinue(n))
            {
                Get(n);
            }
        }

        _WithVar_2822.fertilized = -1;
        if (FileContinue(n))
        {
            Get(n);
        }

        //Botsareus 10/5/2015 freeing up memory from Eric's obsolete ancestors code
        if (FileContinue(n))
        {
            Get(n);
        }
        for (t = 0; t < 500; t++)
        {
            if (FileContinue(n))
            {
                Get(n);
            }
            if (FileContinue(n))
            {
                Get(n);
            }
            if (FileContinue(n))
            {
                Get(n);
            }
        }

        _WithVar_2822.sim = 0;
        if (FileContinue(n))
        {
            Get(n);
        }
        if (FileContinue(n))
        {
            Get(n);
        }

        //Botsareus 2/23/2013 Rest of tie data
        if (FileContinue(n))
        {
            Get(n);
        }
        for (t = 0; t < MAXTIES; t++)
        {
            if (FileContinue(n))
            {
                Get(n);
            }
            if (FileContinue(n))
            {
                Get(n);
            }
            if (FileContinue(n))
            {
                Get(n);
            }
            if (FileContinue(n))
            {
                Get(n);
            }
            //Botsareus 4/18/2016 Protection against currupt file
            if (_WithVar_2822.Ties(t).NaturalLength < 0)
            {
                _WithVar_2822.Ties(t).NaturalLength = 0;
            }
            if (_WithVar_2822.Ties(t).NaturalLength > 1500)
            {
                _WithVar_2822.Ties(t).NaturalLength = 1500;
            }
        }

        //Botsareus 4/9/2013 For genetic distance graph
        if (FileContinue(n))
        {
            Get(n);
        }
        _WithVar_2822.GenMut = _WithVar_2822.DnaLen / GeneticSensitivity;

        //Panda 2013/08/11 chloroplasts
        if (FileContinue(n))
        {
            Get(n);
        }
        //Botsareus 4/18/2016 Protection against currupt file
        if (_WithVar_2822.chloroplasts < 0)
        {
            _WithVar_2822.chloroplasts = 0;
        }
        if (_WithVar_2822.chloroplasts > 32000)
        {
            _WithVar_2822.chloroplasts = 32000;
        }

        //Botsareus 12/3/2013 Read epigenetic information

        for (t = 0; t < 14; t++)
        {
            if (FileContinue(n))
            {
                Get(n);
            }
        }

        //Botsareus 1/28/2014 Read robot tag

        if (FileContinue(n))
        {
            Get(n);
        }

        //Read if robot is using sunbelt

        bool usesunbelt = false;//sunbelt mutations


        if (FileContinue(n))
        {
            Get(n);
        }

        //Botsareus 3/28/2014 Read if disable chloroplasts

        if (FileContinue(n))
        {
            Get(n);
        }

        //Botsareus 3/28/2014 Read kill resrictions

        if (FileContinue(n))
        {
            Get(n);
        }
        if (FileContinue(n))
        {
            Get(n);
        }
        if (_WithVar_2822.Chlr_Share_Delay > 8)
        {
            _WithVar_2822.Chlr_Share_Delay = 8; //Botsareus 4/18/2016 Protection against currupt file
        }
        if (FileContinue(n))
        {
            Get(n);
        }
        if (_WithVar_2822.dq > 3)
        {
            _WithVar_2822.dq = 3; //Botsareus 4/18/2016 Protection against currupt file
        }

        //Botsareus 10/8/2015 Keep track of mutations from old dna file
        if (FileContinue(n))
        {
            Get(n);
        }

        //Botsareus 6/22/2016 Actual velocity

        if (FileContinue(n))
        {
            Get(n);
        }
        if (FileContinue(n))
        {
            Get(n);
        }

        _WithVar_2822.dq = _WithVar_2822.dq - IIf(_WithVar_2822.dq > 1, 2, 0);

        if (!.Veg)
        {
            if (y_eco_im > 0 & Form1.lblSaving.Visible == false)
            {
                if (Trim(Right(_WithVar_2822.tag, 5)) != Trim(Left(_WithVar_2822.nrg + _WithVar_2822.nrg, 5)))
                {
                    _WithVar_2822.dq = 2 + (_WithVar_2822.dq == 1) * true;
                }
                if (_WithVar_2822.FName != "Mutate.txt" && _WithVar_2822.FName != "Base.txt" && _WithVar_2822.FName != "Corpse")
                {
                    _WithVar_2822.dq = 2 + (_WithVar_2822.dq == 1) * true;
                }
            }
        }
        else
        {
            if (y_eco_im > 0 & _WithVar_2822.chloroplasts < 2000)
            {
                _WithVar_2822.Dead = true;
            }
            if (TotalChlr > SimOpts.MaxPopulation)
            {
                _WithVar_2822.Dead = true;
            }
        }
        if (_WithVar_2822.FName == "Corpse")
        {
            _WithVar_2822.nrg = 0;
        }

    //Botsareus 10/5/2015 Replaced with something better
    //Botsareus 9/16/2014 Read gene kill resrictions
    //    ReDim .delgenes(0)
    //    ReDim .delgenes(0).dna(0)
    //    Dim x As Integer
    //    Dim y As Integer
    //    Dim poz As Long
    //    poz = Seek(n)
    //    Get #n, , x
    //    If x < 0 Then
    //        Get #n, poz - 1, Fe
    //        If y_eco_im > 0 And Form1.lblSaving.Visible = False Then
    //            .dq = 2 + (.dq = 1) * True
    //        End If
    //        GoTo OldFile
    //    End If
    //    ReDim .delgenes(x)
    //    For y = 0 To x
    //        Get #n, , .delgenes(y).position
    //        Get #n, , k
    //        ReDim .delgenes(y).dna(k)
    //        For t = 0 To k
    //          Get #n, , .delgenes(y).dna(t).tipo
    //          Get #n, , .delgenes(y).dna(t).value
    //        Next t
    //    Next

    //read in any future data here

    OldFile:
        //burn through any new data from a different version
        While(FileContinue(n));
        Get(n);
        Wend();

        //grab these three FE codes
        Get(n);
        Get(n);
        Get(n);

        //don't you dare put anything after this!
        //except some initialization stuff
        _WithVar_2822.Vtimer = 0;
        _WithVar_2822.virusshot = 0;

        //Botsareus 2/21/2014 Special case reset sunbelt mutations

        if (!usesunbelt)
        {
            _WithVar_2822.Mutables.mutarray(P2UP) = 0;
            _WithVar_2822.Mutables.mutarray(CE2UP) = 0;
            _WithVar_2822.Mutables.mutarray(AmplificationUP) = 0;
            _WithVar_2822.Mutables.mutarray(TranslocationUP) = 0;
        }
    }

    private static bool FileContinue(ref int filenumber)
    {
        bool FileContinue = false;
        //three FE bytes (ie: 254) means we are at the end of the record

        byte Fe = 0;

        int Position = 0;

        int k = 0;


        FileContinue = false;
        Position() = Seek(filenumber);

        do
        {
            if (!EOF(filenumber))
            {
                Get(#filenumber);
    }
            else
            {
                FileContinue = false;
                Fe = 254;
            }

            k = k + 1;

            if (Fe != 254)
            {
                FileContinue = true;
                //exit immediatly, we are done
            }
        } while (!(!FileContinue && k < 3);

        //reset position
        Get(#filenumber, Position() - 1, Fe);
  return FileContinue;
    }

    /*
    ' saves the body of the robot
    */
    private static void SaveRobotBody(ref int n, ref int r)
    {
        int t = 0;
        int k = 0;

        string s = "";

        string s2 = "";

        string temp = "";

        int longtmp = 0;


        const byte Fe = 254;
        // Dim space As Integer

        s = "Mutation Details removed in last save.";

        dynamic _WithVar_7882;
        _WithVar_7882 = rob(r);

        Put(#n);
    Put(#n);
    Put(#n);

// fisiche
    Put(#n);
    Put(#n);
    Put(#n);
    Put(#n);
    Put(#n);
    Put(#n); //momento angolare
    Put(#n); //momento torcente

    for (t = 0; t < MAXTIES; t++)
        {
            Put(#n);
      Put(#n);
      Put(#n);
      Put(#n);
      Put(#n);
      Put(#n);
      Put(#n);
      Put(#n);
      Put(#n);
      Put(#n);
      Put(#n);
      Put(#n);
      Put(#n);
      Put(#n);
      Put(#n);
      }

        // biologiche
        Put(#n);

//custom variables we're saving
      for (t = 1; t < 50; t++)
        {
            Put(#n);
        Put(#n); //|
        Put(#n);
        }

        Put(#n); //| variabili private

// macchina virtuale
        Put(#n);
        k = DnaLen(ref rob(r).dna());
        Put(#n);
        for (t = 1; t < k; t++)
        {
            Put(#n);
          Put(#n);
          }

        for (t = 0; t < 20; t++)
        {
            Put(#n);
            }

        // informative
        Put(#n);
            Put(#n);
            Put(#n);
            Put(#n);
            Put(#n);
            Put(#n);
            Put(#n);
            Put(#n);
            Put(#n);

// aspetto

            Put(#n);
            Put(#n);

// new features
            Put(#n);
            Put(#n);
            Put(#n);
            Put(#n);
            Put(#n);
            Put(#n);
            Put(#n);
            Put(#n);
            Put(#n);
            Put(#n);

            Put(#n);
            Put(#n);

            Put(#n);
            Put(#n);

//EricL 5/8/2006 New feature allows for saving sims without all the mutations details
            if (MDIForm1.instance.SaveWithoutMutations)
        {
            Put(#n);
              Put(#n);
            }
        else
        {
            //EricL 5/3/2006  This needs some explaining.  It's all about backward compatability.  The length of the mutation details
            //was stored as an Integer in older sim file versions.  It can overflow and go negative or even wrap positive
            //again in sims with lots of mutations.  So, we test to see if it would have overflowed and it so, we write
            //the interger 1 there instead of the actual length.  Since the actual details, being string descriptions,
            //should never have length 1, this is a signal to the sim file read routine that the real length is a Long
            //stored right after the Int.
            if (CLng(Len(_WithVar_7882.LastMutDetail)) > CLng((2 ^ 15 - 1)))
            {
                // Lots of mutations.  Tell the read routine that the real length is Long valued and coming up next.
                Put(#n);
                Put(#n); // The real length
              }
            else
            {
                //Not so many mutation details.  Leave the length as an Int for backward compatability
                Put(#n);
              }
            Put(#n);
            }

        //EricL 3/30/2006 Added the following line.  Looks like it was just missing.  Mutations were turned off after loading save...
        Put(#n);

            for (t = 0; t < 20; t++)
        {
            Put(#n);
              Put(#n);
             }

        Put(#n);
              Put(#n);

              Put(#n);
              Put(#n);
              Put(#n); //EricL  New for 2.42.8.  Save Robot number for use in re-mapping ties and shots when re-loaded

              Put(#n);
              Put(#n);
              Put(#n);
              Put(#n);
              Put(#n);
              Put(#n);
              Put(#n);
              Put(#n);

              if (_WithVar_7882.fertilized < 0)
        {
            _WithVar_7882.spermDNAlen = 0;
        }

        Put(#n);
              for (t = 1; t < _WithVar_7882.spermDNAlen; t++)
        {
            Put(#n);
                Put(#n);
                }
        Put(#n);

//Botsareus 10/5/2015 freeing up memory from Eric's obsolete ancestors code
                Put(#n);
                for (t = 0; t < 500; t++)
        {
            Put(#n);
                  Put(#n);
                  Put(#n);
                  }

        Put(#n);
                  Put(#n);

//Botsareus 2/23/2013 Rest of tie data
                  Put(#n);
                  for (t = 0; t < MAXTIES; t++)
        {
            Put(#n);
                    Put(#n);
                    Put(#n);
                    Put(#n);
                  }

        //Botsareus 4/9/2013 For genetic distance graph
        Put(#n);

//Panda 8/13/2013 Write chloroplasts
                  Put(#n);

//Botsareus 12/3/2013 Write epigenetic information
                  for (t = 0; t < 14; t++)
        {
            Put(#n);
                  }

        //Botsareus 1/28/2014 Write robot tag

        string blank = "";


        if (!.Veg)
        {
            if (y_eco_im > 0 & Form1.lblSaving.Visible == false && _WithVar_7882.dq < 2)
            {
                if (Left(_WithVar_7882.tag, 45) == Left(blank, 45))
                {
                    _WithVar_7882.tag = _WithVar_7882.FName;
                }
                _WithVar_7882.tag = Left(_WithVar_7882.tag, 45) + Left(_WithVar_7882.nrg + _WithVar_7882.nrg, 5);
            }
        }

        Put(#n);

//Botsareus 1/28/2014 Write if robot is using sunbelt

                  Put(#n);

//Botsareus 3/28/2014 Write if disable chloroplasts

                  Put(#n);

//Botsareus 3/28/2014 Read kill resrictions

                  Put(#n);
                  Put(#n);
                  Put(#n);

//Botsareus 10/8/2015 Keep track of mutations from old dna file

                  Put(#n);

//Botsareus 6/22/2016 Actual velocity

                  Put(#n);
                  Put(#n);


//Botsareus 10/5/2015 Replaced with something better
//    'Botsareus 9/16/2014 Write gene kill resrictions

//    Dim x As Integer
//    Dim y As Integer
//    x = UBound(.delgenes): Put #n, , x
//    For y = 0 To x
//        Put #n, , .delgenes(y).position
//        k = UBound(.delgenes(y).dna): Put #n, , k
//        For t = 0 To k
//          Put #n, , .delgenes(y).dna(t).tipo
//          Put #n, , .delgenes(y).dna(t).value
//        Next t
//    Next

//write any future data here

                  Put(#n);
                  Put(#n);
                  Put(#n);
              }

    /*
    ' saves a robot dna     !!!New routine from Carlo!!!
    'Botsareus 10/8/2015 Code simplification
    */
    static void salvarob(ref int n, ref string path)
    {
        string hold = "";

        string hashed = "";


        int a = 0;

        string epigene = "";


        VBCloseFile(1); ();
        VBOpenFile(1, path); ;
        hold = SaveRobHeader(ref n);

        //Botsareus 10/8/2015 New code to save epigenetic memory as gene

        if (UseEpiGene)
        {
            for (a = 971; a < 990; a++)
            {
                if (rob[n].mem(a) != 0)
                {
                    epigene = epigene + rob[n].mem(a) + " " + a + " store" + vbCrLf;
                }
            }

            if (epigene != "")
            {
                epigene = "start" + vbCrLf + epigene + "*.thisgene .delgene store" + vbCrLf + "stop";

                hold = hold + epigene;

            }

        }

        savingtofile = true; //Botsareus 2/28/2014 when saving to file the def sysvars should not save
        hold = hold + DetokenizeDNA(ref n, ref 0);
        savingtofile = false;
        hashed = Hash(ref hold, ref 20);
        VBWriteFile(1, hold); ;
        VBWriteFile(1, ""); ;
        VBWriteFile(1, "'#hash: " + hashed); ;
        string blank = "";

        if (Left(rob[n].tag, 45) != Left(blank, 45))
        {
            VBWriteFile(1, "'#tag:" + Left(rob[n].tag, 45) + vbCrLf); ;
        }
        VBCloseFile(1); ();

        //Botsareus 12/11/2013 Save mrates file
        Save_mrates(rob[n].Mutables, extractpath(ref path) + "\\" + extractexactname(ref extractname(ref path)) + ".mrate");

        if (x_restartmode > 0)
        {
            return;//Botsareus 10/8/2015 Can not rename robot in any special restart mode

        }

        if (MsgBox("Do you want to change robot's name to " + extractname(ref path) + " ?", vbYesNo, "Robot DNA saved") == vbYes)
        {
            rob[n].FName = extractname(ref path);
        }
    }

    /*
    'M U T A T I O N  F I L E Botsareus 12/11/2013

    'generate mrates file
    */
    static void Save_mrates(ref mutationprobs mut, ref string FName)
    {
        byte m = 0;

        VBOpenFile(1, FName); ;
        mutationprobs _WithVar_mut;
        _WithVar_mut = mut;
        Write(1, _WithVar_mut.PointWhatToChange);
        Write(1, _WithVar_mut.CopyErrorWhatToChange);
        for (m = 0; m < 10; m++)
        { //Need to change this if adding more mutation types (Trying to keep some backword compatability here)
            Write(1, _WithVar_mut.mutarray(m));
            Write(1, _WithVar_mut.Mean(m));
            Write(1, _WithVar_mut.StdDev(m));
        }
        mut = _WithVar_mut;
        VBCloseFile(1); ();
    }

    /*
    'load mrates file
    */
    public static mutationprobs Load_mrates(ref string FName)
    {
        mutationprobs Load_mrates = null;
        byte m = 0;

        VBOpenFile(1, FName); ;
        mutationprobs _WithVar_Load_mrates;
        _WithVar_Load_mrates = Load_mrates;
        Input(1, _WithVar_Load_mrates.PointWhatToChange);
        Input(1, _WithVar_Load_mrates.CopyErrorWhatToChange);
        for (m = 0; m < 10; m++)
        { //Need to change this if adding more mutation types (needs to have eofs if more then 10 for backword compatability)
            Input(1, _WithVar_Load_mrates.mutarray(m));
            Input(1, _WithVar_Load_mrates.Mean(m));
            Input(1, _WithVar_Load_mrates.StdDev(m));
        }
        Load_mrates = _WithVar_Load_mrates;
        VBCloseFile(1); ();
        return Load_mrates;
    }

    /*
    'D A T A C O N V E R S I O N S Botsareus 12/18/2013
    */
    private static int sint(int lval)
    {
        int sint = 0;
        lval = lval % 32000;
        sint = lval;
        return sint;
    }
}

                                                                                                                                    }
