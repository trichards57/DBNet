using Iersera.Model;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static Buckets_Module;
using static Common;
using static DNAManipulations;
using static Microsoft.VisualBasic.Information;
using static Microsoft.VisualBasic.Interaction;
using static Microsoft.VisualBasic.Strings;
using static Obstacles;
using static Robots;
using static Shots_Module;
using static SimOptModule;
using static System.Math;
using static VBExtension;

internal static class Globals
{
    public const int AVGAGE_GRAPH = 3;

    public const int AVGCHLR_GRAPH = 12;

    //Botsareus 5/24/2013 Customizable graphs
    public const int CUSTOM_1_GRAPH = 16;

    public const int CUSTOM_2_GRAPH = 17;

    public const int CUSTOM_3_GRAPH = 18;

    public const int DNACOND_GRAPH = 7;

    public const int DNALENGTH_GRAPH = 6;

    public const int DYNAMICCOSTS_GRAPH = 10;

    public const int ENERGY_GRAPH = 5;

    public const int ENERGY_SPECIES_GRAPH = 9;

    public const int GENERATION_DIST_GRAPH = 14;

    //Botsareus 8/31/2013 Average chloroplasts graph
    public const int GENETIC_DIST_GRAPH = 13;

    public const int GENETIC_SIMPLE_GRAPH = 15;

    public const uint INFINITE = 0xFFFFFFFF;

    public const int MUT_DNALENGTH_GRAPH = 8;

    public const int MUTATIONS_GRAPH = 2;

    public const int NUMGRAPHS = 18;

    public const int OFFSPRING_GRAPH = 4;

    //Constants for the graphs, which are used all over the place unfortunately -Botsareus 8/3/2012 reimplemented
    public const int POPULATION_GRAPH = 1;

    public const int SPECIESDIVERSITY_GRAPH = 11;

    public const int SYNCHRONIZE = 0x100000;

    //For args to the IM client
    //Botsareus 10/21/2015 No longer a required feature
    //Public Declare Function GetCurrentProcessId Lib "kernel32" () As Long
    public const int WM_CLOSE = 0x10;

    public static bool autosaved = false;

    public static int bodyfix = 0;

    public static bool chseedloadsim = false;

    public static bool chseedstartnew = false;

    //actual evolution globals
    public static int curr_dna_size = 0;

    //Botsareus 12/16/2013 Delta2 on mutations tab
    public static bool Delta2 = false;

    public static byte DeltaDevChance = 0;

    public static decimal DeltaDevExp = 0;

    public static decimal DeltaDevLn = 0;

    public static byte DeltaMainChance = 0;

    public static decimal DeltaMainExp = 0;

    public static decimal DeltaMainLn = 0;

    public static int DeltaPM = 0;

    public static byte DeltaWTC = 0;

    public static byte Disqualify = 0;

    //Botsareus 11/29/2013 The mutations tab
    public static bool epireset = false;

    public static decimal epiresetemp = 0;

    public static int epiresetOP = 0;

    public static bool FudgeAll = false;

    public static bool FudgeEyes = false;

    //Botsareus 5/25/2013 Two more graphs, moved to globals
    public static int[] graphfilecounter = new int[(NUMGRAPHS + 1)];

    // TODO: Confirm Array Size By Token
    public static int[] graphleft = new int[(NUMGRAPHS + 1)];

    public static bool[] graphsave = new bool[(NUMGRAPHS + 1)];

    // TODO: Confirm Array Size By Token
    public static int[] graphtop = new int[(NUMGRAPHS + 1)];

    public static bool GraphUp = false;

    // TODO: Confirm Array Size By Token
    public static bool[] graphvisible = new bool[(NUMGRAPHS + 1)];

    public static bool HideDB = false;

    public static bool hidepred = false;

    public static int hidePredCycl = 0;

    public static int hidePredOffset = 0;

    public static int Init_hidePredCycl = 0;

    public static int intFindBestV2 = 0;

    public static bool ismutating = false;

    public static string leagueSourceDir = "";

    public static double LFOR = 0;

    public static double LFORcorr = 0;

    public static bool LFORdir = false;

    //some global settings change within simulation
    public static bool loadboylabldisp = false;

    public static bool loadstartnovid = false;

    //later used in conjunction with a routine to give robs a bit of energy back after loading up.
    public static int maxfieldsize = 0;

    public static int ModeChangeCycles = 0;

    public static vector Mouse_loc = null;

    public static bool NoDeaths = false;

    //Botsareus 12/16/2013 Normalize DNA length
    public static bool NormMut = false;

    public static List<keydata> PB_keys = new() { };

    public static bool reprofix = false;

    // TODO - Specified Minimum Array Boundary Not Supported: Public PB_keys() As keydata
    //G L O B A L  S E T T I N G S Botsareus 3/15/2013
    public static bool screenratiofix = false;

    public static bool simalreadyrunning = false;

    public static int StartChlr = 0;

    public static bool startnovid = false;

    public static string strGraphQuery1 = "";

    public static string strGraphQuery2 = "";

    public static string strGraphQuery3 = "";

    //Botsareus 5/31/2013 Special graph info
    public static string strSimStart = "";

    public static bool sunbelt = false;

    //direction
    //correction
    public static int target_dna_size = 0;

    public static int tmpseed = 0;

    public static int TotalChlr = 0;

    // TODO: Confirm Array Size By Token
    // TODO: Confirm Array Size By Token
    public static int TotalEnergy = 0;

    public static int totcorpse = 0;

    // total energy in the sim
    public static int totnvegs = 0;

    // total non vegs in sim
    public static int totnvegsDisplayed = 0;

    // Toggle for display purposes, so the display doesn't catch half calculated value
    public static int totwalls = 0;

    public static bool UseEpiGene = false;

    public static bool UseIntRnd = false;

    public static bool UseOldColor = false;

    public static bool UseSafeMode = false;

    public static bool UseStepladder = false;

    public static int valMaxNormMut = 0;

    public static int valNormMut = 0;

    public static int x_filenumber = 0;

    public static byte x_fudge = 0;

    //Variables below prefixed x_ are used for league and evolution, y_ are used only for evolution
    //Variables prefixed _res_ are used for restriction overwrites
    public static byte x_res_kill_chlr = 0;

    public static bool x_res_kill_mb = false;

    public static bool x_res_kill_mb_veg = false;

    public static byte x_res_other = 0;

    public static byte x_res_other_veg = 0;

    //Botsareus 1/31/2014 Restart modes
    public static byte x_restartmode = 0;

    //used only by "load simulation"
    //Botsareus 1/5/2014 Copy of Obstacle array
    public static List<Obstacle> xObstacle = new() { };

    public static byte y_eco_im = 0;

    public static bool y_graphs = false;

    public static int y_hidePredCycl = 0;

    public static decimal y_LFOR = 0;

    public static bool y_normsize = false;

    // TODO - Specified Minimum Array Boundary Not Supported: Public xObstacle() As Obstacle
    public static byte y_res_kill_chlr = 0;

    public static bool y_res_kill_dq = false;

    public static bool y_res_kill_dq_veg = false;

    public static bool y_res_kill_mb = false;

    public static bool y_res_kill_mb_veg = false;

    public static byte y_res_other = 0;

    public static byte y_res_other_veg = 0;

    //Botsareus 2/12/2014 Start repopulating robots with chloroplasts
    //Botsareus 2/14/2014 Used to calculate time difference and mode change for survival
    public static string y_robdir = "";

    public static int y_Stgwins = 0;

    public static int y_zblen = 0;

    private static int MSWHEEL_ROLLMSG = 0;

    public static void aggiungirob(int r, double x, double y)
    { //Botsareus 5/22/2014 Bugfix by adding byval
        bool anyvegy = false;

        if (r == -1)
        {
            //run one loop to check vegy status
            for (var i = 0; i < SimOpts.SpeciesNum - 1; i++)
            {
                if (checkvegstatus(i))
                {
                    anyvegy = true;
                    break;
                }
            }
            if (!anyvegy)
            {
                return;
            }

            do
            {
                r = Random(0, SimOpts.SpeciesNum - 1); // start randomly in the list of species
            } while (!checkvegstatus(r));

            x = fRnd((int)(SimOpts.Specie[r].Poslf * (SimOpts.FieldWidth - 60)), (int)(SimOpts.Specie[r].Posrg * (SimOpts.FieldWidth - 60)));
            y = fRnd((int)(SimOpts.Specie[r].Postp * (SimOpts.FieldHeight - 60)), (int)(SimOpts.Specie[r].Posdn * (SimOpts.FieldHeight - 60)));
        }

        if (SimOpts.Specie[r].Name != "" && SimOpts.Specie[r].path != "Invalid Path")
        {
            var a = RobScriptLoad(respath(ref SimOpts.Specie[r].path) + "\\" + SimOpts.Specie[r].Name);
            if (a < 0)
            {
                SimOpts.Specie[r].Native = false;
                return;
            }

            //Check to see if we were able to load the bot.  If we can't, the path may be wrong, the sim may have
            //come from another machine with a different install path.  Set the species path to an empty string to
            //prevent endless looping of error dialogs.
            if (!rob[a].exist)
            {
                SimOpts.Specie[r].path = "Invalid Path";
                return;
            }

            rob[a].Veg = SimOpts.Specie[r].Veg;
            if (rob[a].Veg)
            {
                rob[a].chloroplasts = StartChlr; //Botsareus 2/12/2014 Start a robot with chloroplasts
            }
            //NewMove loaded via robscriptload
            rob[a].Fixed = SimOpts.Specie[r].Fixed;
            rob[a].CantSee = SimOpts.Specie[r].CantSee;
            rob[a].DisableDNA = SimOpts.Specie[r].DisableDNA;
            rob[a].DisableMovementSysvars = SimOpts.Specie[r].DisableMovementSysvars;
            rob[a].CantReproduce = SimOpts.Specie[r].CantReproduce;
            rob[a].VirusImmune = SimOpts.Specie[r].VirusImmune;
            rob[a].Corpse = false;
            rob[a].Dead = false;
            rob[a].body = 1000;
            //  EnergyAddedPerCycle = EnergyAddedPerCycle + 10000
            rob[a].radius = FindRadius(a);
            rob[a].Mutations = 0;
            rob[a].OldMutations = 0; //Botsareus 10/8/2015
            rob[a].LastMut = 0;
            rob[a].generation = 0;
            rob[a].SonNumber = 0;
            rob[a].parent = 0;
            Array.Clear(rob[a].mem, 0, rob[a].mem.Length);
            if (rob[a].Fixed)
            {
                rob[a].mem[216] = 1;
            }
            rob[a].pos.X = x;
            rob[a].pos.Y = y;

            rob[a].aim = rndy() * PI * 2; //Botsareus 5/30/2012 Added code to rotate the robot on placment
            rob[a].mem[SetAim] = (int)rob[a].aim * 200;

            //Bot is already in a bucket due to the prepare routine
            UpdateBotBucket(a);
            rob[a].nrg = SimOpts.Specie[r].Stnrg;
            rob[a].Mutables = SimOpts.Specie[r].Mutables;

            rob[a].Vtimer = 0;
            rob[a].virusshot = 0;
            rob[a].genenum = CountGenes(rob[a].dna);

            rob[a].DnaLen = DnaLen(rob[a].dna);
            rob[a].GenMut = rob[a].DnaLen / GeneticSensitivity; //Botsareus 4/9/2013 automatically apply genetic to inserted robots

            rob[a].mem[DnaLenSys] = rob[a].DnaLen;
            rob[a].mem[GenesSys] = rob[a].genenum;

            //Botsareus 10/8/2015 New kill restrictions
            rob[a].multibot_time = IIf(SimOpts.Specie[r].kill_mb, 210, 0);
            rob[a].dq = IIf(SimOpts.Specie[r].dq_kill, 1, 0);
            rob[a].NoChlr = SimOpts.Specie[r].NoChlr; //Botsareus 11/1/2015 Bug fix

            for (var i = 0; i < 7; i++)
            { //Botsareus 5/20/2012 fix for skin engine
                rob[a].Skin[i] = SimOpts.Specie[r].Skin[i];
            }

            rob[a].color = SimOpts.Specie[r].color;
            makeoccurrlist(a);
        }
    }

    // Not sure where to put this function, so it's going here
    // makes poff. that is, creates that explosion effect with
    // some fake shots...
    public static void makepoff(int n)
    {
        for (var t = 1; t < 20; t++)
        {
            var an = (640 / 20) * t;
            var vs = Random(RobSize / 40, RobSize / 30);
            var vx = (int)(Robots.rob[n].vel.X + absx(an / 100, vs, 0, 0, 0));
            var vy = (int)(Robots.rob[n].vel.Y + absy(an / 100, vs, 0, 0, 0));
            var rob = Robots.rob[n];
            var x = Random((int)(rob.pos.X - rob.radius), (int)(rob.pos.X + rob.radius));
            var y = Random((int)(rob.pos.Y - rob.radius), (int)(rob.pos.Y + rob.radius));
            if (Random(1, 2) == 1)
                createshot(x, y, vx, vy, -100, 0, 0, RobSize * 2, Robots.rob[n].color);
            else
                createshot(x, y, vx, vy, -100, 0, 0, RobSize * 2, DBrite(Robots.rob[n].color));
        }
    }

    // total walls count
    // Total corpses
    //Panda 8/24/2013 total number of chlroroplasts
    //Attempt to stop robots dying during the first cycle of a loaded sim
    //Botsareus 2/2/2013 Tells the parseor to ignore debugint and debugbool while the robot is mutating
    //Botsareus 6/11/2013 For music
    [DllImport("winmm.dll", EntryPoint = "mciSendStringA")] public static extern dynamic mciSendString(dynamic _);

    private static bool checkvegstatus(int r)
    {
        var checkvegstatus = false;

        if (SimOpts.Specie[r].Veg == true && SimOpts.Specie[r].Native)
        {
            //see if any active robots have chloroplasts
            for (var t = 1; t < MaxRobs; t++)
            {
                var rob = Robots.rob[t];
                if (rob.exist && rob.chloroplasts > 0)
                {
                    //remove old nick name
                    var splitname = Split(rob.FName, ")");
                    //if it is a nick name only
                    var robname = Left(splitname[0], 1) == "(" && IsNumeric(Right(splitname[0], Len(splitname[0]) - 1)) ? splitname[1] : rob.FName;
                    if (SimOpts.Specie[r].Name == robname)
                    {
                        checkvegstatus = true;
                        return checkvegstatus;
                    }
                }
            }

            //If there is no robots at all with chlr then repop everything

            checkvegstatus = true;

            for (var t = 1; t < MaxRobs; t++)
            {
                if (Robots.rob[t].exist && Robots.rob[t].Veg && Robots.rob[t].age > 0)
                { //Botsareus 11/4/2015 age test makes sure all robots spawn
                    checkvegstatus = false;
                    return checkvegstatus;
                }
            }
        }
        return checkvegstatus;
    }

    //A temporary module for everything without a home.
    // Option Explicit
    //Botsareus 7/2/2014 PlayerBot settings
    public class keydata
    {
        public bool Active = false;
        public bool Invert = false;
        public byte key = 0;
        public int memloc = 0;
        public int value = 0;
    }

    // var structure, to store the correspondance name<->value
    public class var_
    {
        public string Name = "";
        public int value = 0;
    }
}
