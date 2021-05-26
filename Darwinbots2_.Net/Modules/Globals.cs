using Iersera.Model;
using Iersera.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BucketManager;
using static DNAManipulations;
using static Obstacles;
using static Robots;
using static Shots;
using static SimOpt;

internal static class Globals
{
    public const int AVGAGE_GRAPH = 3;
    public const int AVGCHLR_GRAPH = 12;
    public const int CUSTOM_1_GRAPH = 16;
    public const int CUSTOM_2_GRAPH = 17;
    public const int CUSTOM_3_GRAPH = 18;
    public const int DNACOND_GRAPH = 7;
    public const int DNALENGTH_GRAPH = 6;
    public const int DYNAMICCOSTS_GRAPH = 10;
    public const int ENERGY_GRAPH = 5;
    public const int ENERGY_SPECIES_GRAPH = 9;
    public const int GENERATION_DIST_GRAPH = 14;
    public const int GENETIC_DIST_GRAPH = 13;
    public const int GENETIC_SIMPLE_GRAPH = 15;
    public const uint INFINITE = 0xFFFFFFFF;
    public const int MUT_DNALENGTH_GRAPH = 8;
    public const int MUTATIONS_GRAPH = 2;
    public const int NUMGRAPHS = 18;
    public const int OFFSPRING_GRAPH = 4;
    public const int POPULATION_GRAPH = 1;
    public const int SPECIESDIVERSITY_GRAPH = 11;
    public const int SYNCHRONIZE = 0x100000;
    public const int WM_CLOSE = 0x10;
    public static bool autosaved = false;
    public static int bodyfix = 0;
    public static bool chseedloadsim = false;
    public static bool chseedstartnew = false;
    public static int curr_dna_size = 0;
    public static bool Delta2 = false;
    public static int DeltaDevChance = 0;
    public static double DeltaDevExp = 0;
    public static double DeltaDevLn = 0;
    public static int DeltaMainChance = 0;
    public static double DeltaMainExp = 0;
    public static double DeltaMainLn = 0;
    public static int DeltaPM = 0;
    public static int DeltaWTC = 0;
    public static int Disqualify = 0;
    public static bool epireset = false;
    public static double epiresetemp = 0;
    public static int epiresetOP = 0;
    public static bool FudgeAll = false;
    public static bool FudgeEyes = false;
    public static int[] graphfilecounter = new int[(NUMGRAPHS + 1)];
    public static int[] graphleft = new int[(NUMGRAPHS + 1)];
    public static bool[] graphsave = new bool[(NUMGRAPHS + 1)];
    public static int[] graphtop = new int[(NUMGRAPHS + 1)];
    public static bool GraphUp = false;
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
    public static bool loadboylabldisp = false;
    public static bool loadstartnovid = false;
    public static int maxfieldsize = 0;
    public static int ModeChangeCycles = 0;
    public static vector Mouse_loc = null;
    public static bool NoDeaths = false;
    public static bool NormMut = false;
    public static List<KeyData> PB_keys = new() { };
    public static bool reprofix = false;
    public static bool screenratiofix = false;
    public static bool simalreadyrunning = false;
    public static int StartChlr = 0;
    public static bool startnovid = false;
    public static string strGraphQuery1 = "";
    public static string strGraphQuery2 = "";
    public static string strGraphQuery3 = "";
    public static string strSimStart = "";
    public static bool sunbelt = false;
    public static int target_dna_size = 0;
    public static int tmpseed = 0;
    public static int TotalChlr = 0;
    public static int TotalEnergy = 0;
    public static int totcorpse = 0;
    public static int totnvegs = 0;
    public static int totnvegsDisplayed = 0;
    public static int totwalls = 0;
    public static bool UseEpiGene = false;
    public static bool UseIntRnd = false;
    public static bool UseOldColor = false;
    public static bool UseSafeMode = false;
    public static bool UseStepladder = false;
    public static int valMaxNormMut = 0;
    public static int valNormMut = 0;
    public static int x_filenumber = 0;
    public static int x_fudge = 0;
    public static int x_res_kill_chlr = 0;
    public static bool x_res_kill_mb = false;
    public static bool x_res_kill_mb_veg = false;
    public static int x_res_other = 0;
    public static int x_res_other_veg = 0;
    public static int x_restartmode = 0;
    public static List<Obstacle> xObstacle = new();
    public static int y_eco_im = 0;
    public static bool y_graphs = false;
    public static int y_hidePredCycl = 0;
    public static double y_LFOR = 0;
    public static bool y_normsize = false;
    public static int y_res_kill_chlr = 0;
    public static bool y_res_kill_dq = false;
    public static bool y_res_kill_dq_veg = false;
    public static bool y_res_kill_mb = false;
    public static bool y_res_kill_mb_veg = false;
    public static int y_res_other = 0;
    public static int y_res_other_veg = 0;
    public static string y_robdir = "";
    public static int y_Stgwins = 0;
    public static int y_zblen = 0;

    public static async Task aggiungirob(int r, double x, double y)
    {
        if (r == -1)
        {
            if (!Species.Any(s => CheckVegStatus(s)))
                return;

            do
            {
                r = ThreadSafeRandom.Local.Next(0, SimOpts.SpeciesNum); // start randomly in the list of species
            } while (!CheckVegStatus(Species[r]));

            x = ThreadSafeRandom.Local.Next((int)(SimOpts.Specie[r].Poslf * (SimOpts.FieldWidth - 60)), (int)(SimOpts.Specie[r].Posrg * (SimOpts.FieldWidth - 60)));
            y = ThreadSafeRandom.Local.Next((int)(SimOpts.Specie[r].Postp * (SimOpts.FieldHeight - 60)), (int)(SimOpts.Specie[r].Posdn * (SimOpts.FieldHeight - 60)));
        }

        if (SimOpts.Specie[r].Name != "" && SimOpts.Specie[r].path != "Invalid Path")
        {
            var a = await RobScriptLoad(System.IO.Path.Join(SimOpts.Specie[r].path, SimOpts.Specie[r].Name));

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
                rob[a].chloroplasts = StartChlr;

            rob[a].Fixed = SimOpts.Specie[r].Fixed;
            rob[a].CantSee = SimOpts.Specie[r].CantSee;
            rob[a].DisableDNA = SimOpts.Specie[r].DisableDNA;
            rob[a].DisableMovementSysvars = SimOpts.Specie[r].DisableMovementSysvars;
            rob[a].CantReproduce = SimOpts.Specie[r].CantReproduce;
            rob[a].VirusImmune = SimOpts.Specie[r].VirusImmune;
            rob[a].Corpse = false;
            rob[a].Dead = false;
            rob[a].body = 1000;
            rob[a].radius = FindRadius(a);
            rob[a].Mutations = 0;
            rob[a].OldMutations = 0;
            rob[a].LastMut = 0;
            rob[a].generation = 0;
            rob[a].SonNumber = 0;
            rob[a].parent = 0;
            Array.Clear(rob[a].mem, 0, rob[a].mem.Length);

            if (rob[a].Fixed)
                rob[a].mem[216] = 1;

            rob[a].pos.X = x;
            rob[a].pos.Y = y;

            rob[a].aim = ThreadSafeRandom.Local.NextDouble() * Math.PI * 2;
            rob[a].mem[SetAim] = (int)rob[a].aim * 200;

            //Bot is already in a bucket due to the prepare routine
            UpdateBotBucket(a);
            rob[a].nrg = SimOpts.Specie[r].Stnrg;
            rob[a].Mutables = SimOpts.Specie[r].Mutables;

            rob[a].Vtimer = 0;
            rob[a].virusshot = 0;
            rob[a].genenum = CountGenes(rob[a].dna);

            rob[a].DnaLen = DnaLen(rob[a].dna);
            rob[a].GenMut = rob[a].DnaLen / GeneticSensitivity;

            rob[a].mem[DnaLenSys] = rob[a].DnaLen;
            rob[a].mem[GenesSys] = rob[a].genenum;

            rob[a].multibot_time = SimOpts.Specie[r].kill_mb ? 210 : 0;
            rob[a].dq = SimOpts.Specie[r].dq_kill ? 1 : 0;
            rob[a].NoChlr = SimOpts.Specie[r].NoChlr;

            for (var i = 0; i < 7; i++)
            {
                rob[a].Skin[i] = SimOpts.Specie[r].Skin[i];
            }

            rob[a].color = SimOpts.Specie[r].color;
            Senses.makeoccurrlist(a);
        }
    }

    public static void MakePoff(robot rob)
    {
        for (var t = 1; t < 20; t++)
        {
            var an = 640 / 20 * t;
            var vs = ThreadSafeRandom.Local.Next(RobSize / 40, RobSize / 30);
            var vx = (int)(rob.vel.X + absx(an / 100, vs, 0, 0, 0));
            var vy = (int)(rob.vel.Y + absy(an / 100, vs, 0, 0, 0));
            var x = ThreadSafeRandom.Local.Next((int)(rob.pos.X - rob.radius), (int)(rob.pos.X + rob.radius));
            var y = ThreadSafeRandom.Local.Next((int)(rob.pos.Y - rob.radius), (int)(rob.pos.Y + rob.radius));
            if (ThreadSafeRandom.Local.Next(1, 3) == 1)
                createshot(x, y, vx, vy, -100, 0, 0, RobSize * 2, rob.color);
            else
                createshot(x, y, vx, vy, -100, 0, 0, RobSize * 2, DBrite(rob.color));
        }
    }

    private static bool CheckVegStatus(varspecie.Species species)
    {
        if (!species.Veg || !species.Native)
            return false;

        //see if any active robots have chloroplasts
        foreach (var rob in Robots.rob.Where(r => r.exist && r.chloroplasts > 0))
        {
            //remove old nick name
            var splitname = rob.FName.Split(")");
            //if it is a nick name only
            var robname = (splitname[0].StartsWith("(") && int.TryParse(splitname[0][1..], out var _)) ? splitname[1] : rob.FName;
            if (species.Name == robname)
                return true;
        }

        //If there is no robots at all with chlr then repop everything
        return !Robots.rob.Any(r => r.exist && r.Veg && r.age > 0);
    }
}
