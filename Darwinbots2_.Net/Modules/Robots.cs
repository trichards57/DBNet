using DBNet.Forms;
using Iersera.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using static BucketManager;
using static Common;
using static Database;
using static DNAExecution;
using static DNAManipulations;
using static DNATokenizing;
using static F1Mode;
using static Globals;
using static HDRoutines;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.Information;
using static Microsoft.VisualBasic.Interaction;
using static NeoMutations;
using static Obstacles;
using static Physics;
using static Senses;
using static Shots_Module;
using static SimOptModule;
using static System.Math;
using static Teleport;
using static Ties;
using static varspecie;
using static VBConstants;
using static VBExtension;
using static Vegs;

internal static class Robots
{
    public const int aimdx = 5;

    public const int aimshoot = 901;

    public const int aimsx = 6;

    public const int AimSys = 18;

    public const int backshot = 900;

    public const int bodgain = 194;

    public const int bodloss = 195;

    public const int body = 311;

    public const int chlr = 920;

    public const int CubicTwipPerBody = 905;

    public const int DelgeneSys = 340;

    public const int DELTIE = 467;

    public const int dirdn = 2;

    public const int dirdx = 3;

    public const int dirsx = 4;

    //Botsareus 4/2/2013 Removed old cross over code and replaced it with a working one
    // Option Explicit
    // robot system locations constants
    public const int dirup = 1;

    public const int DnaLenSys = 336;
    public const int Energy = 310;
    public const int EYE1DIR = 521;
    public const int EYE1WIDTH = 531;
    public const int EYE2DIR = 522;
    public const int EYE2WIDTH = 532;
    public const int EYE3DIR = 523;
    public const int EYE3WIDTH = 533;
    public const int EYE4DIR = 524;
    public const int EYE4WIDTH = 534;
    public const int EYE5DIR = 525;
    public const int EYE5WIDTH = 535;
    public const int EYE6DIR = 526;
    public const int EYE6WIDTH = 536;
    public const int EYE7DIR = 527;
    public const int EYE7WIDTH = 537;
    public const int EYE8DIR = 528;
    public const int EYE8WIDTH = 538;
    public const int EYE9DIR = 529;
    public const int EYE9WIDTH = 539;
    public const int EyeEnd = 510;
    public const int EYEF = 510;
    public const int EyeStart = 500;
    public const int fdbody = 312;
    public const int FIXANG = 468;
    public const int Fixed = 216;
    public const int FIXLEN = 469;
    public const int FOCUSEYE = 511;
    public const int GenesSys = 339;

    //Botsareus 4/3/2013 crossover section
    //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    public const int GeneticSensitivity = 75;

    public const int half = 60;
    public const int hit = 201;
    public const int hitdn = 206;
    public const int hitdx = 207;
    public const int hitsx = 208;
    public const int hitup = 205;
    public const int in1 = 810;
    public const int in10 = 819;
    public const int in2 = 811;
    public const int in3 = 812;
    public const int in4 = 813;
    public const int in5 = 814;
    public const int in6 = 815;
    public const int in7 = 816;
    public const int in8 = 817;
    public const int in9 = 818;
    public const int Kills = 220;
    public const int LandM = 400;
    public const int light = 923;
    public const int masssys = 10;
    public const int maxvelsys = 11;

    //Panda 8/13/2013 The Chloroplast variable
    public const int mkchlr = 921;

    public const int mkvirus = 335;
    public const int mrepro = 301;
    public const int mtie = 330;
    public const int multi = 470;
    public const int numties = 466;
    public const int occurrstart = 700;
    public const int out1 = 800;
    public const int out10 = 809;
    public const int out2 = 801;
    public const int out3 = 802;
    public const int out4 = 803;
    public const int out5 = 804;
    public const int out6 = 805;
    public const int out7 = 806;
    public const int out8 = 807;
    public const int out9 = 808;
    public const int pain = 203;
    public const int pleas = 204;
    public const int poison = 827;
    public const int rdboy = 315;
    public const int readtiesys = 471;
    public const dynamic refbody = 688;
    public const dynamic refmulti = 686;
    public const dynamic refshell = 687;
    public const int REFTYPE = 685;
    public const int refveldn = 698;
    public const int refveldx = 697;
    public const int refvelscalar = 695;
    public const int refvelsx = 696;
    public const int refvelup = 699;
    public const dynamic refxpos = 689;
    public const dynamic refypos = 690;
    public const int Repro = 300;

    //Panda 8/15/2013 The add chloroplast variable
    public const int rmchlr = 922;

    public const int robage = 9;

    // and so on...
    //seems like a random number, I know.
    //It's cube root of volume * some constants to give
    //radius of 60 for a bot of 1000 body
    public const int ROBARRAYMAX = 32000;

    public const int RobSize = 120;
    public const int SetAim = 19;
    public const int setboy = 314;
    public const int SEXREPRO = 302;

    //Panda 8/15/2013 The remove chloroplast variable
    //Botsareus 8/14/2013 A variable to let robots know how much light is available
    public const int sharechlr = 924;

    public const int shdn = 211;
    public const int shdx = 212;
    public const int shflav = 202;
    public const int shoot = 7;
    public const int shootval = 8;
    public const int shsx = 213;
    public const int shup = 210;
    public const int stifftie = 331;
    public const int strbody = 313;
    public const int SYSFERTILIZED = 303;
    public const int thisgene = 341;
    public const int TIEANG = 450;
    public const int TIELEN = 451;
    public const int tieloc = 452;
    public const int TIENUM = 455;
    public const int tieport1 = 450;
    public const int TIEPRES = 454;
    public const int tieval = 453;
    public const int timersys = 12;
    public const int TotalBots = 401;
    public const int TOTALMYSPECIES = 402;
    public const int trefbody = 437;
    public const int TREFDNSYS = 457;
    public const int TREFDXSYS = 459;
    public const int trefnrg = 464;
    public const int trefshell = 449;
    public const int TREFSXSYS = 458;
    public const int TREFUPSYS = 456;
    public const int trefvelmydn = 442;
    public const int trefvelmydx = 441;
    public const int trefvelmysx = 440;
    public const int trefvelmyup = 443;
    public const int trefvelscalar = 444;
    public const int trefvelyourdn = 447;
    public const int trefvelyourdx = 446;
    public const int trefvelyoursx = 445;
    public const int trefvelyourup = 448;
    public const int trefxpos = 438;
    public const int trefypos = 439;
    public const int vel = 200;
    public const int veldn = 199;
    public const int veldx = 198;
    public const int velscalar = 196;
    public const int velsx = 197;
    public const int velup = 200;
    public const int VshootSys = 338;
    public const int Vtimer = 337;
    public static List<int> kil = new List<int>(new int[(ROBARRAYMAX + 1)]);

    //Public MaxAbsNum As Long                   ' robots born (used to assign unique code)
    public static int MaxMem = 0;

    public static int MaxRobs = 0;

    public static List<int> rep = new List<int>(new int[(ROBARRAYMAX + 1)]);

    // rob diameter in fixed diameter sims
    //robot array must be an array for swift retrieval times.
    public static List<robot> rob = new List<robot> { };

    // max used robots array index
    public static int robfocus = 0;

    // the robot which has the focus (selected)
    public static int TotalRobots = 0;

    // total robots in the sim
    public static int TotalRobotsDisplayed = 0;

    // TODO: Confirm Array Size By Token// array of robots to kill
    private static int kl = 0;

    // TODO - Specified Minimum Array Boundary Not Supported: Public rob() As robot// array of robots  start at 500 and up dynamically in chunks of 500 as needed
    // TODO: Confirm Array Size By Token// array for pointing to robots attempting reproduction
    private static int rp = 0;

    private layer As


private As block3)


private As Integer, ByRef


private ByVal n


private As Single


private GeneticDistance(dynamic rob1(_UNUSED) {
        block3, ByRef rob2() As block3) As Single GeneticDistance = null;
        int diffcount = 0;

        int a = 0;

        for (a = 0; a < UBound(rob1); a++)
        {
            if (rob1(a).match == 0)
            {
                diffcount = diffcount + 1;
            }
        }

        for (a = 0; a < UBound(rob2); a++)
        {
            if (rob2(a).match == 0)
            {
                diffcount = diffcount + 1;
            }
        }

        GeneticDistance = diffcount / (UBound(rob1) + UBound(rob2) + 2);
        return GeneticDistance;
    }

    public static double absx(double aim, int up, int dn, int sx, int dx)
    {
        decimal absx = 0;
        decimal upTotal = 0;

        decimal sxTotal = 0;

        //  On Error Resume Next
        //  up = up - dn
        //  sx = sx - dx
        upTotal = up - dn;
        sxTotal = sx - dx;
        if (upTotal > 32000)
        {
            upTotal = 32000;
        }
        if (upTotal < -32000)
        {
            upTotal = -32000;
        }
        if (sxTotal > 32000)
        {
            sxTotal = 32000;
        }
        if (sxTotal < -32000)
        {
            sxTotal = -32000;
        }
        absx = Cos(aim) * upTotal + Sin(aim) * sxTotal;
        return absx;
    }

    public static double absy(decimal aim, int up, int dn, int sx, int dx)
    {
        decimal absy = 0;
        decimal upTotal = 0;

        decimal sxTotal = 0;

        //On Error Resume Next
        //up = up - dn
        //sx = sx - dx
        upTotal = up - dn;
        sxTotal = sx - dx;
        if (upTotal > 32000)
        {
            upTotal = 32000;
        }
        if (upTotal < -32000)
        {
            upTotal = -32000;
        }
        if (sxTotal > 32000)
        {
            sxTotal = 32000;
        }
        if (sxTotal < -32000)
        {
            sxTotal = -32000;
        }
        absy = -Sin(aim) * upTotal + Cos(aim) * sxTotal;
        return absy;
    }

    public static double DoGeneticDistance(int r1, int r2)
    {
        decimal DoGeneticDistance = 0;
        int t = 0;

        List<block3> ndna1 = new List<block3> { }; // TODO - Specified Minimum Array Boundary Not Supported: Dim ndna1() As block3

        List<block3> ndna2 = new List<block3> { }; // TODO - Specified Minimum Array Boundary Not Supported: Dim ndna2() As block3

        int length1 = 0;

        int length2 = 0;

        length1 = UBound(rob(r1).dna);
        length2 = UBound(rob(r2).dna);
        List<block3> ndna1_5221_tmp = new List<block3>();
        for (int redim_iter_6355 = 0; i < 0; redim_iter_6355++) { ndna1.Add(null); }
        List<block3> ndna2_5608_tmp = new List<block3>();
        for (int redim_iter_7851 = 0; i < 0; redim_iter_7851++) { ndna2.Add(null); }

        //map to nucli

        //if step is 1 then normal nucli
        for (t = 0; t < UBound(rob(r1).dna); t++)
        {
            ndna1(t).nucli = DNAtoInt(rob(r1).dna(t).tipo, rob(r1).dna(t).value);
        }
        for (t = 0; t < UBound(rob(r2).dna); t++)
        {
            ndna2(t).nucli = DNAtoInt(rob(r2).dna(t).tipo, rob(r2).dna(t).value);
        }

        //Step3 Figure genetic distance
        simplematch(ndna1, ndna2);

        DoGeneticDistance = GeneticDistance(ndna1, ndna2);
        return DoGeneticDistance;
    }

    public static void DoGeneticMemory(int t)
    {
        int loc = 0;// memory location to copy from parent to offspring

        //Make sure the bot has a tie
        if (rob(t).numties > 0)
        {
            //Make sure it really is the birth tie and not some other tie
            if (rob(t).Ties(1).last > 0)
            {
                //Copy the memory locations 976 to 990 from parent to child. One per cycle.
                loc = 976 + rob(t).age; // the location to copy
                                        //only copy the value if the location is 0 in the child and the parent has something to copy
                if (rob(t).mem(loc) == 0 & rob(t).epimem(rob(t).age) != 0)
                {
                    rob(t).mem(loc) = rob(t).epimem(rob(t).age);
                }
            }
        }
    }

    public static double FindRadius(int n, decimal mult = 1)
    {
        decimal FindRadius = 0;

        //Botsareus 9/30/2014 Redo of find radius to make it faster
        decimal bodypoints = 0;

        decimal chlr = 0;

        if (mult == -1)
        {
            bodypoints = 32000;
            chlr = 0;
        }
        else
        {
            bodypoints = rob[n].body * mult;
            chlr = rob[n].chloroplasts * mult;
        }

        if (bodypoints < 1)
        {
            bodypoints = 1;
        }
        if (SimOpts.FixedBotRadii)
        {
            FindRadius = half;
        }
        else
        {
            // EricL 10/20/2007 Added log(bodypoints) to increase the size variation in bots.
            FindRadius = (Log(bodypoints) * bodypoints * CubicTwipPerBody * 3 * 0.25m / PI) ^ (1 / 3);
            //Botsareus 9/30/2014 added a rule to count chloroplasts in robot size
            FindRadius = FindRadius + (415 - FindRadius) * chlr / 32000;
            if (FindRadius < 1)
            {
                FindRadius = 1;
            }
        }
        return FindRadius;
    }

    public static int genelength(int n, int p)
    {
        int genelength = 0;
        //measures the length of gene p in robot n
        int pos = 0;

        pos = genepos(rob[n].dna(), p);
        genelength = GeneEnd(rob[n].dna(), pos) - pos + 1;

        return genelength;
    }

    public static void KillRobot(int n)
    {
        if (SimOpts.DeadRobotSnp)
        {
            if (rob[n].Veg && SimOpts.SnpExcludeVegs)
            {
            }
            else
            {
                AddRecord(n);
            }
        }

        //robfocus to next highlighted robot on kill robot for playerbot mode
        if (n == robfocus && MDIForm1.instance.pbOn.Checked)
        {
            int t = 0;

            for (t = 1; t < MaxRobs; t++)
            {
                dynamic _WithVar_9051;
                _WithVar_9051 = rob(t);
                if (_WithVar_9051.exist && _WithVar_9051.highlight && t != n)
                {
                    robfocus = t;
                }
            }
        }

        int newsize = 0;

        int X = 0;

        //If n = -1 Then n = robfocus

        rob[n].Fixed = false;
        rob[n].Veg = false;
        rob[n].View = false;
        rob[n].NewMove = false;
        rob[n].LastOwner = "";
        rob[n].SonNumber = 0;
        rob[n].age = 0;
        delallties(n);
        rob[n].exist = false; // do this after deleting the ties...
        UpdateBotBucket(n);
        if (!MDIForm1.nopoff)
        {
            makepoff(n);
        }
        if (!(rob[n].console == null))
        {
            rob[n].console.textout "Robot has died."; //EricL 3/19/2006 Indicate robot has died in console
        }
        if (robfocus == n)
        {
            robfocus = 0; // EricL 6/9/2006 get rid of the eye viewer thingy now that the bot is dead.
            MDIForm1.instance.DisableRobotsMenu();
        }

        if (rob[n].virusshot > 0 & rob[n].virusshot <= maxshotarray)
        {
            Shots(rob[n].virusshot).exist = false; // We have to kill any stored shots for this bot
            rob[n].virusshot = 0;
        }

        rob[n].spermDNAlen = 0;
        List<> rob_2445_tmp = new List<>();
        for (int redim_iter_4088 = 0; i < 0; redim_iter_4088++) { rob.Add(null); }

        rob[n].LastMutDetail = "";

        if (n == MaxRobs)
        {
            int b = 0;

            b = MaxRobs - 1;
            While(!rob(b).exist && b > 1); // EricL Loop now counts down, not up and works correctly.
            b = b - 1;
            Wend();
            MaxRobs = b; //b is now the last actual array element

            //If the number of bots is 250 less than the array size, shrink the array.  The array is still potentially sparse
            //since this only happens if the highest numbr bot happened to die.  There are probably still open slots to put new bots
            //so hopefully we shouldn't be redimming up and down all the time.
            //We take the array up in increments of 100 and down in increments of 250 so as not to grow and shrink the array in the same cycle
            newsize = UBound(rob());
            if (MaxRobs + 250 < newsize && MaxRobs > 500)
            {
                // MsgBox "About to shrink the rob array"
                // Form1.Timer2.Enabled = False
                // While Form1.InTimer2
                //   'Do nothing
                // Wend
                // Form1.SecTimer.Enabled = False
                // Form1.Enabled = False
                //        For x = 1 To 10000000
                // Next x
                List<> rob_3225_tmp = new List<>();
                for (int redim_iter_8632 = 0; i < 0; redim_iter_8632++) { rob.Add(redim_iter_8632 < rob.Count ? rob(redim_iter_8632) : null); }
                // Form1.Enabled = True
                // Form1.SecTimer.Enabled = True
                // Form1.Timer2.Enabled = True
            }
        }
    }

    public static int posto()
    {
        int posto = 0;
        int newsize = 0;

        int t = 0;

        bool foundone = false;

        int X = 0;

        t = 1;
        foundone = false;
        While(!foundone && t <= MaxRobs);
        if (!rob(t).exist)
        {
            foundone = true;
        }
        else
        {
            t = t + 1;
        }
        Wend();

        // t could be MaxRobs + 1
        if (t > MaxRobs)
        {
            MaxRobs = t; // The array is fully packed.  Every slot is taken.
        }

        newsize = UBound(rob());
        if (MaxRobs > newsize)
        { //the array is fully packed and we need more space
            newsize = newsize + 100;
            //  Form1.Timer2.Enabled = False
            //  While Form1.InTimer2
            //    'Do nothing
            //  Wend
            //  Form1.SecTimer.Enabled = False
            // Form1.Enabled = False
            //  For x = 1 To 10000000
            //  Next x

            //DoEvents
            // MsgBox "About to Redim the bot array"
            List<> rob_8994_tmp = new List<>();// Should bump the array up in increments of 500
            for (int redim_iter_9874 = 0; i < 0; redim_iter_9874++) { rob.Add(redim_iter_9874 < rob.Count ? rob(redim_iter_9874) : null); }
            //  Form1.Enabled = True
            //  Form1.SecTimer.Enabled = True
            //  Form1.Timer2.Enabled = True
            //MaxRobs = t
        }

        //At some point should add logic to keep the rob array below RobArrayMax for the day when bots reference other bot numbers
        //Shouldn't cause problems at the moment though.

        //If t = UBound(rob()) Then
        //  MaxRobs = MaxRobs - 1
        //  t = t - 1
        //End If

        posto = t;

        //potential memory leak:  I'm not sure if VB will catch and release the dereferenced memory or not
        robot blank = null;

        rob(posto) = blank;

        // MaxAbsNum = MaxAbsNum + 1
        GiveAbsNum(posto);
        return posto;
    }

    public static void Reproduce(int n, int per)
    {
        if (rob[n].body < 5)
        {
            return;//Botsareus 3/27/2014 An attempt to prevent 'robot bursts'
        }

        if (SimOpts.DisableTypArepro && rob[n].Veg == false)
        {
            return;
        }
        int sondist = 0;

        int nuovo = 0;

        decimal nnrg = 0;//Botsareus 8/24/2013 nchloroplasts
        decimal nwaste = 0;
        decimal npwaste = 0;
        decimal nchloroplasts = 0;

        int nbody = 0;

        int nx = 0;

        int ny = 0;

        int t = 0;

        bool tests = false;

        tests = false;
        int i = 0;

        decimal tempnrg = 0;

        //If n = -1 Then n = robfocus 'Botsareus 11/3/2015 this should be done on user side.

        if (rob[n].body <= 2 || rob[n].CantReproduce)
        {
            goto getout; //bot is too small to reproduce
        }

        //attempt to stop veg overpopulation but will it work?
        if (rob[n].Veg == true && (TotalChlr > SimOpts.MaxPopulation || totvegsDisplayed < 0))
        {
            goto getout; //Panda 8/23/2013 Based on TotalChlr now
        }

        // If we got here and it's a veg, then we are below the reproduction threshold.  Let a random 10% of the veggis reproduce
        // so as to avoid all the veggies reproducing on the same cycle.  This adds some randomness
        // so as to avoid giving preference to veggies with lower bot array numbers.  If the veggy population is below 90% of the threshold
        // then let them all reproduce.
        if (rob[n].Veg == true && (Random(0, 10) != 5) && (TotalChlr > (SimOpts.MaxPopulation * 0.9m)))
        {
            goto getout; //Panda 8/23/2013 Based on TotalChlr now
        }
        if (totvegsDisplayed == -1)
        {
            goto getout; // no veggies can reproduce on the first cycle after the sim is restarted.
        }

        per = per % 100; // per should never be <=0 as this is checked in ManageReproduction()

        if (reprofix)
        {
            if (per < 3 Then rob[n].Dead =) { //Botsareus 4/27/2013 kill 8/26/2014 greedy robots
            }

            if (per <= 0)
            {
                goto getout;
            }
            sondist = FindRadius(n, (per / 100)) + FindRadius(n, ((100 - per) / 100));

            nnrg = (rob[n].nrg / 100) * CSng(per);
            nbody = (rob[n].body / 100) * CSng(per);
            //rob[n].nrg = rob[n].nrg - DNALength(n) * 3

            tempnrg = rob[n].nrg;
            if (tempnrg > 0)
            {
                nx = rob[n].pos.X + absx(rob[n].aim, sondist, 0, 0, 0);
                ny = rob[n].pos.Y + absy(rob[n].aim, sondist, 0, 0, 0);
                tests = tests || simplecoll(nx, ny, n);
                tests = tests || !rob[n].exist; //Botsareus 6/4/2014 Can not reproduce from a dead robot
                                                //tests = tests Or (rob[n].Fixed And IsInSpawnArea(nx, ny))
                if (!tests)
                {
                    //    If MaxRobs = 500 Then MsgBox "Maxrobs = 500 in Reproduce, about to call posto"
                    nuovo = posto();

                    SimOpts.TotBorn = SimOpts.TotBorn + 1;
                    if (rob[n].Veg)
                    {
                        totvegs = totvegs + 1;
                    }
                    List<> rob_5128_tmp = new List<>();
                    for (int redim_iter_9253 = 0; i < 0; redim_iter_9253++) { rob.Add(null); }
                    for (t = 1; t < UBound(rob(nuovo).dna); t++)
                    {
                        rob(nuovo).dna(t) = rob[n].dna(t);
                    }

                    rob(nuovo).DnaLen = rob[n].DnaLen;
                    rob(nuovo).genenum = rob[n].genenum;
                    rob(nuovo).Mutables = rob[n].Mutables;

                    rob(nuovo).Mutations = rob[n].Mutations;
                    rob(nuovo).OldMutations = rob[n].OldMutations; //Botsareus 10/8/2015

                    rob(nuovo).LastMut = 0;
                    rob(nuovo).LastMutDetail = rob[n].LastMutDetail;

                    for (t = 1; t < rob[n].maxusedvars; t++)
                    {
                        rob(nuovo).usedvars(t) = rob[n].usedvars(t);
                    }

                    for (t = 0; t < 12; t++)
                    {
                        rob(nuovo).Skin(t) = rob[n].Skin(t);
                    }

                    rob(nuovo).maxusedvars = rob[n].maxusedvars;
                    Erase(rob(nuovo).mem);
                    Erase(rob(nuovo).Ties);

                    rob(nuovo).pos.X = rob[n].pos.X + absx(rob[n].aim, sondist, 0, 0, 0);
                    rob(nuovo).pos.Y = rob[n].pos.Y + absy(rob[n].aim, sondist, 0, 0, 0);
                    rob(nuovo).exist = true;
                    rob(nuovo).BucketPos.X = -2;
                    rob(nuovo).BucketPos.Y = -2;
                    UpdateBotBucket(nuovo);
                    rob(nuovo).vel = rob[n].vel;
                    rob(nuovo).actvel = rob[n].actvel; //Botsareus 7/1/2016 Bugfix
                    rob(nuovo).color = rob[n].color;
                    rob(nuovo).aim = rob[n].aim + PI;
                    if (rob(nuovo).aim > 6.28m)
                    {
                        rob(nuovo).aim = rob(nuovo).aim - 2 * PI;
                    }
                    rob(nuovo).aimvector = VectorSet(Cos(rob(nuovo).aim), Sin(rob(nuovo).aim));
                    rob(nuovo).mem(SetAim) = rob(nuovo).aim * 200;
                    rob(nuovo).mem(468) = 32000;
                    //      rob(nuovo).mem(480) = 32000 Botsareus 2/21/2013 Broken
                    //      rob(nuovo).mem(481) = 32000
                    //      rob(nuovo).mem(482) = 32000
                    //      rob(nuovo).mem(483) = 32000
                    rob(nuovo).Corpse = false;
                    rob(nuovo).Dead = false;
                    rob(nuovo).NewMove = rob[n].NewMove;
                    rob(nuovo).generation = rob[n].generation + 1;
                    if (rob(nuovo).generation > 32000)
                    {
                        rob(nuovo).generation = 32000; //Botsareus 10/9/2015 Overflow protection
                    }
                    rob(nuovo).BirthCycle = SimOpts.TotRunCycle;
                    rob(nuovo).vnum = 1;

                    nnrg = (rob[n].nrg / 100) * CSng(per);
                    nwaste = rob[n].Waste / 100 * CSng(per);
                    npwaste = rob[n].Pwaste / 100 * CSng(per);
                    nchloroplasts = (rob[n].chloroplasts / 100) * CSng(per); //Panda 8/23/2013 Distribute the chloroplasts

                    rob[n].nrg = rob[n].nrg - nnrg - (nnrg * 0.001m); // Make reproduction cost 0.1% of nrg transfer
                    rob[n].Waste = rob[n].Waste - nwaste;
                    rob[n].Pwaste = rob[n].Pwaste - npwaste;
                    rob[n].body = rob[n].body - nbody;
                    rob[n].radius = FindRadius(n);
                    rob[n].chloroplasts = rob[n].chloroplasts - nchloroplasts; //Panda 8/23/2013 Distribute the chloroplasts

                    rob(nuovo).chloroplasts = nchloroplasts; //Panda 8/23/2013 Distribute the chloroplasts
                    rob(nuovo).body = nbody;
                    rob(nuovo).radius = FindRadius(nuovo);
                    rob(nuovo).Waste = nwaste;
                    rob(nuovo).Pwaste = npwaste;
                    rob[n].mem(Energy) = CInt(rob[n].nrg);
                    rob[n].mem(311) = rob[n].body;
                    rob[n].SonNumber = rob[n].SonNumber + 1;
                    if (rob[n].SonNumber > 32000)
                    {
                        rob[n].SonNumber = 32000; // EricL Overflow protection.  Should change to Long at some point.
                    }
                    rob(nuovo).nrg = nnrg * 0.999m; // Make reproduction cost 1% of nrg transfer
                    rob(nuovo).onrg = nnrg * 0.999m;
                    rob(nuovo).mem(Energy) = CInt(rob(nuovo).nrg);
                    rob(nuovo).Poisoned = false;
                    rob(nuovo).parent = rob[n].AbsNum;
                    rob(nuovo).FName = rob[n].FName;
                    rob(nuovo).LastOwner = rob[n].LastOwner;
                    rob(nuovo).Veg = rob[n].Veg;
                    rob(nuovo).NoChlr = rob[n].NoChlr; //Botsareus 3/28/2014 Disable chloroplasts
                    rob(nuovo).Fixed = rob[n].Fixed;
                    rob(nuovo).CantSee = rob[n].CantSee;
                    rob(nuovo).DisableDNA = rob[n].DisableDNA;
                    rob(nuovo).DisableMovementSysvars = rob[n].DisableMovementSysvars;
                    rob(nuovo).CantReproduce = rob[n].CantReproduce;
                    rob(nuovo).VirusImmune = rob[n].VirusImmune;
                    if (rob(nuovo).Fixed)
                    {
                        rob(nuovo).mem(Fixed) = 1;
                    }
                    rob(nuovo).SubSpecies = rob[n].SubSpecies;

                    //Botsareus 4/9/2013 we need to copy some variables for genetic distance
                    rob(nuovo).OldGD = rob[n].OldGD;
                    rob(nuovo).GenMut = rob[n].GenMut;
                    //Botsareus 1/29/2014 Copy the tag
                    rob(nuovo).tag = rob[n].tag;
                    //Botsareus 7/22/2014 Both robots should have same boyancy
                    rob(nuovo).Bouyancy = rob[n].Bouyancy;

                    //Botsareus 7/29/2014 New kill restrictions
                    if (rob[n].multibot_time > 0)
                    {
                        rob(nuovo).multibot_time = rob[n].multibot_time / 2 + 2;
                    }
                    rob(nuovo).dq = rob[n].dq;

                    //Botsareus 10/5/2015 freeing up memory from Eric's obsolete ancestors code
                    //      For i = 0 To 500
                    //        rob(nuovo).Ancestors(i) = rob[n].Ancestors(i)  ' copy the parents ancestor list
                    //      Next i
                    //      rob(nuovo).AncestorIndex = rob[n].AncestorIndex + 1  ' increment the ancestor index
                    //      If rob(nuovo).AncestorIndex > 500 Then rob(nuovo).AncestorIndex = 0  ' wrap it
                    //      rob(nuovo).Ancestors(rob(nuovo).AncestorIndex).num = rob[n].AbsNum  ' add the parent as the most recent ancestor
                    //      rob(nuovo).Ancestors(rob(nuovo).AncestorIndex).mut = rob[n].LastMut ' add the number of mutations the parent has had up until now.
                    //      rob(nuovo).Ancestors(rob(nuovo).AncestorIndex).sim = SimOpts.SimGUID ' Use this seed to uniqufiy this ancestor in Internet mode

                    //BucketsProximity n, 12
                    //BucketsProximity nuovo, 12

                    rob(nuovo).Vtimer = 0;
                    rob(nuovo).virusshot = 0;

                    //First 5 genetic memory locations happen instantly
                    for (i = 0; i < 4; i++)
                    {
                        rob(nuovo).mem(971 + i) == rob[n].mem(971 + i);
                    }
                    //The other 15 genetic memory locations are stored now but can be used later
                    for (i = 0; i < 14; i++)
                    {
                        rob(nuovo).epimem(i) = rob[n].mem(976 + i);
                    }
                    //Erase parents genetic memory now to prevent him from completing his own transfer by using his kid
                    for (i = 0; i < 14; i++)
                    {
                        rob[n].epimem(i) = 0;
                    }

                    //Botsareus 12/17/2013 Delta2
                    if (Delta2)
                    {
                        dynamic _WithVar_855;
                        _WithVar_855 = rob(nuovo);
                        int MratesMax = 0;

                        MratesMax = IIf(NormMut, CLng(_WithVar_855.DnaLen) * CLng(valMaxNormMut), 2000000000);
                        //dynamic mutation overload correction
                        decimal dmoc = 0;

                        dmoc = 1 + (rob(nuovo).DnaLen - curr_dna_size) / 500;
                        if (dmoc < 0.01m)
                        {
                            dmoc = 0.01m; //Botsareus 1/16/2016 Bug fix
                        }
                        if (!y_normsize)
                        {
                            dmoc = 1;
                        }
                        //zerobot stabilization
                        if (x_restartmode == 7 || x_restartmode == 8)
                        {
                            if (_WithVar_855.FName == "Mutate.txt")
                            {
                                //normalize child
                                _WithVar_855.Mutables.mutarray(PointUP) = _WithVar_855.Mutables.mutarray(PointUP) * 1.75m;
                                if (_WithVar_855.Mutables.mutarray(PointUP) > MratesMax)
                                {
                                    _WithVar_855.Mutables.mutarray(PointUP) = MratesMax;
                                }
                                _WithVar_855.Mutables.mutarray(P2UP) = _WithVar_855.Mutables.mutarray(P2UP) * 1.75m;
                                if (_WithVar_855.Mutables.mutarray(P2UP) > MratesMax)
                                {
                                    _WithVar_855.Mutables.mutarray(P2UP) = MratesMax;
                                }
                            }
                        }

                        byte mrep = 0;

                        for (mrep = 0; mrep < (Int(3 * rndy()) + 1(*-(rob[n].mem(mrepro,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,, + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + +) > 0); mrep++)
                        { //2x to 4x
                            for (t = 1; t < 10; t++)
                            {
                                if (t == 9)
                                {
                                    goto skip; //ignore PM2 mutation here
                                }
                                if (_WithVar_855.Mutables.mutarray(t) < 1)
                                {
                                    goto skip; //Botsareus 1/3/2014 if mutation off then skip it
                                }
                                if (rndy() < DeltaMainChance / 100)
                                {
                                    if (DeltaMainExp != 0)
                                    {
                                        if ((t == CopyErrorUP || t == TranslocationUP || t == ReversalUP || t == CE2UP))
                                        {
                                            _WithVar_855.Mutables.mutarray(t) = _WithVar_855.Mutables.mutarray(t) * (dmoc + 2) / 3;
                                        }
                                        else
                                        {
                                            if (!(t == MinorDeletionUP || t == MajorDeletionUP))
                                            {
                                                _WithVar_855.Mutables.mutarray(t) = _WithVar_855.Mutables.mutarray(t) * dmoc; //dynamic mutation overload correction
                                            }
                                        }

                                        _WithVar_855.Mutables.mutarray(t) = _WithVar_855.Mutables.mutarray(t) * 10 ^ ((rndy() * 2 - 1) / DeltaMainExp);
                                    }
                                    _WithVar_855.Mutables.mutarray(t) = _WithVar_855.Mutables.mutarray(t) + (rndy() * 2 - 1) * DeltaMainLn;
                                    if (_WithVar_855.Mutables.mutarray(t) < 1)
                                    {
                                        _WithVar_855.Mutables.mutarray(t) = 1;
                                    }
                                    if (_WithVar_855.Mutables.mutarray(t) > MratesMax)
                                    {
                                        _WithVar_855.Mutables.mutarray(t) = MratesMax;
                                    }
                                }
                                if (rndy() < DeltaDevChance / 100)
                                {
                                    if (DeltaDevExp != 0)
                                    {
                                        _WithVar_855.Mutables.StdDev(t) = _WithVar_855.Mutables.StdDev(t) * 10 ^ ((rndy() * 2 - 1) / DeltaDevExp);
                                    }
                                    _WithVar_855.Mutables.StdDev(t) = _WithVar_855.Mutables.StdDev(t) + (rndy() * 2 - 1) * DeltaDevLn;
                                    if (DeltaDevExp != 0)
                                    {
                                        _WithVar_855.Mutables.Mean(t) = _WithVar_855.Mutables.Mean(t) * 10 ^ ((rndy() * 2 - 1) / DeltaDevExp);
                                    }
                                    _WithVar_855.Mutables.Mean(t) = _WithVar_855.Mutables.Mean(t) + (rndy() * 2 - 1) * DeltaDevLn;
                                    //Max range is always 0 to 800
                                    if (_WithVar_855.Mutables.StdDev(t) < 0)
                                    {
                                        _WithVar_855.Mutables.StdDev(t) = 0;
                                    }
                                    if (_WithVar_855.Mutables.StdDev(t) > 200)
                                    {
                                        _WithVar_855.Mutables.StdDev(t) = 200;
                                    }
                                    if (_WithVar_855.Mutables.Mean(t) < 1)
                                    {
                                        _WithVar_855.Mutables.Mean(t) = 1;
                                    }
                                    if (_WithVar_855.Mutables.Mean(t) > 400)
                                    {
                                        _WithVar_855.Mutables.Mean(t) = 400;
                                    }
                                }
                            skip:
                          }
                            _WithVar_855.Mutables.CopyErrorWhatToChange = _WithVar_855.Mutables.CopyErrorWhatToChange + (rndy() * 2 - 1) * DeltaWTC;
                            if (_WithVar_855.Mutables.CopyErrorWhatToChange < 0)
                            {
                                _WithVar_855.Mutables.CopyErrorWhatToChange = 0;
                            }
                            if (_WithVar_855.Mutables.CopyErrorWhatToChange > 100)
                            {
                                _WithVar_855.Mutables.CopyErrorWhatToChange = 100;
                            }
                            mutate(nuovo, true);
                        }
                    }
                    else
                    {
                        if (rob[n].mem(mrepro) > 0)
                        {
                            mutationprobs temp = null;

                            temp = rob(nuovo).Mutables;

                            rob(nuovo).Mutables.Mutations = true; // mutate even if mutations disabled for this bot

                            for (t = 0; t < 20; t++)
                            {
                                rob(nuovo).Mutables.mutarray(t) = rob(nuovo).Mutables.mutarray(t) / 10;
                                if (rob(nuovo).Mutables.mutarray(t) == 0)
                                {
                                    rob(nuovo).Mutables.mutarray(t) = 1000;
                                }
                            }

                            mutate(nuovo, true);

                            rob(nuovo).Mutables = temp;
                        }
                        else
                        {
                            //Mutate n, True 'mutate parent and child, note that these mutations are independant of each other.
                            mutate(nuovo, true);
                        }
                    }

                    makeoccurrlist(nuovo);
                    rob(nuovo).DnaLen = DnaLen(rob(nuovo).dna());
                    rob(nuovo).genenum = CountGenes(rob(nuovo).dna());
                    rob(nuovo).mem(DnaLenSys) = rob(nuovo).DnaLen;
                    rob(nuovo).mem(GenesSys) = rob(nuovo).genenum;

                    maketie(n, nuovo, sondist, 100, 0); //birth ties last 100 cycles
                    rob[n].onrg = rob[n].nrg; //saves parent from dying from shock after giving birth
                    rob(nuovo).mass = nbody / 1000 + rob(nuovo).shell / 200;
                    rob(nuovo).mem(timersys) = rob[n].mem(timersys); //epigenetic timer

                    //A little hack here to remain in control of reproduced robots
                    if (MDIForm1.instance.pbOn.Checked)
                    {
                        if (n == robfocus || rob[n].highlight)
                        {
                            rob(nuovo).highlight = true;
                        }
                    }

                    //Successfully reproduced
                    rob[n].mem(Repro) = 0;
                    rob[n].mem(mrepro) = 0;

                    //Botsareus 11/29/2013 Reset epigenetic memory
                    if (epireset)
                    {
                        rob(nuovo).MutEpiReset = rob[n].MutEpiReset + rob(nuovo).LastMut ^ epiresetemp;
                        if (rob(nuovo).MutEpiReset > epiresetOP && rob[n].MutEpiReset > 0)
                        {
                            rob(nuovo).MutEpiReset = 0;
                            for (i = 0; i < 4; i++)
                            {
                                rob(nuovo).mem(971 + i) == 0;
                            }
                            for (i = 0; i < 14; i++)
                            {
                                rob(nuovo).epimem(i) = 0;
                            }
                        }
                    }

                    rob[n].nrg = rob[n].nrg - rob[n].DnaLen * SimOpts.Costs(DNACOPYCOST) * SimOpts.Costs(COSTMULTIPLIER); //Botsareus 7/7/2013 Reproduction DNACOPY cost
                    if (rob[n].nrg < 0)
                    {
                        rob[n].nrg = 0;
                    }
                }
            }
        getout:;
        }

        /*
        ' New Sexual Reproduction routine from EricL Jan 2008  -Botsareus 4/2/2013 Sexrepro fix
        */
        public static dynamic SexReproduce(int female)
        {
            dynamic SexReproduce = null;

            if (rob(female).body < 5)
            {
                return SexReproduce;//Botsareus 3/27/2014 An attempt to prevent 'robot bursts'
            }

            int sondist = 0;

            int nuovo = 0;

            decimal nnrg = 0;//Botsareus 8/24/2013 nchloroplasts
            decimal nwaste = 0;
            decimal npwaste = 0;
            decimal nchloroplasts = 0;

            int nbody = 0;

            int nx = 0;

            int ny = 0;

            int t = 0;

            bool tests = false;

            int i = 0;

            decimal per = 0;

            decimal tempnrg = 0;

            tests = false;

            if (!rob(female).exist)
            {
                goto getout; // bot must exist
            }
            if (rob(female).Corpse)
            {
                goto getout; // no sex with corpses
            }
            if (rob(female).CantReproduce)
            {
                goto getout; // bot must be able to reproduce
            }
            if (rob(female).body <= 2)
            {
                goto getout; // female must be large enough to have sex
            }
            if (!IsRobDNABounded(rob(female).spermDNA))
            {
                goto getout; // sperm dna must exist
            }

            //The percent of resources given to the offspring comes exclusivly from the mother
            //Perhaps this will lead to sexual selection since sex is expensive for females but not for males
            per = rob(female).mem(SEXREPRO);

            //veggies can reproduce sexually, but we still have to test for veggy population controls
            //we let male veggies fertilize nonveggie females all they want since the offspring's "species" and thus vegginess
            //will be determined by their mother.  Perhaps a strategy will emerge where plants compete to reproduce
            //with nonveggies so as to bypass the popualtion limtis?  Who knows.
            if (rob(female).Veg == true && (TotalChlr > SimOpts.MaxPopulation || totvegsDisplayed < 0))
            {
                return SexReproduce;//Panda 8/23/2013 Based on TotalChlr now
            }

            // If we got here and the female is a veg, then we are below the reproduction threshold.  Let a random 10% of the veggis reproduce
            // so as to avoid all the veggies reproducing on the same cycle.  This adds some randomness
            // so as to avoid giving preference to veggies with lower bot array numbers.  If the veggy population is below 90% of the threshold
            // then let them all reproduce.
            if (rob(female).Veg == true && (Random(0, 9) != 5) && (TotalChlr > (SimOpts.MaxPopulation * 0.9m)))
            {
                return SexReproduce;//Panda 8/23/2013 Based on TotalChlr now
            }
            if (totvegsDisplayed == -1)
            {
                return SexReproduce;// no veggies can reproduce on the first cycle after the sim is restarted.
            }

            per = per % 100; // per should never be <=0 as this is checked in ManageReproduction()

            if (reprofix)
            {
                if (per < 3 Then rob(female).Dead =) { //Botsareus 4/27/2013 kill 8/26/2014 greedy robots
        }

        if (per <= 0)
        {
            return SexReproduce;// Can't give 100% or 0% of resources to offspring
        }
        sondist = FindRadius(female, (per / 100)) + FindRadius(female, ((100 - per) / 100));

        nnrg = (rob(female).nrg / 100) * CSng(per);
        nbody = (rob(female).body / 100) * CSng(per);

        tempnrg = rob(female).nrg;
        if (tempnrg > 0)
        {
            nx = rob(female).pos.X + absx(rob(female).aim, sondist, 0, 0, 0);
            ny = rob(female).pos.Y + absy(rob(female).aim, sondist, 0, 0, 0);
            tests = tests || simplecoll(nx, ny, female);
            tests = tests || !rob(female).exist; //Botsareus 6/4/2014 Can not reproduce from a dead robot
                                                 //tests = tests Or (rob[n].Fixed And IsInSpawnArea(nx, ny))
            if (!tests)
            {
                //Botsareus 3/14/2014 Disqualify
                if ((SimOpts.F1 || x_restartmode == 1) && Disqualify == 2)
                {
                    dreason(rob(female).FName, rob(female).tag, "attempting to reproduce sexually");
                }
                if (!SimOpts.F1 && rob(female).dq == 1 && Disqualify == 2)
                {
                    rob(female).Dead = true; //safe kill robot
                    goto ;
                }
                //Do the crossover.  The sperm DNA is on the mom's bot structure
                //Botsareus 4/2/2013 Crossover fix
                //Botsareus 5/24/2014 Crossover fix

                //Step1 Copy both dnas into block2

                List<block2> dna1 = new List<block2> { }; // TODO - Specified Minimum Array Boundary Not Supported:       Dim dna1() As block2

                List<block2> dna2 = new List<block2> { }; // TODO - Specified Minimum Array Boundary Not Supported:       Dim dna2() As block2

                List<block2> dna1_9688_tmp = new List<block2>();
                for (int redim_iter_995 = 0; i < 0; redim_iter_995++) { dna1.Add(null); }
                for (t = 0; t < UBound(dna1); t++)
                {
                    dna1(t).tipo = rob(female).dna(t).tipo;
                    dna1(t).value = rob(female).dna(t).value;
                }

                List<block2> dna2_9728_tmp = new List<block2>();
                for (int redim_iter_5562 = 0; i < 0; redim_iter_5562++) { dna2.Add(null); }
                for (t = 0; t < UBound(dna2); t++)
                {
                    dna2(t).tipo = rob(female).spermDNA(t).tipo;
                    dna2(t).value = rob(female).spermDNA(t).value;
                }

                //Step2 map nucli

                List<block3> ndna1 = new List<block3> { }; // TODO - Specified Minimum Array Boundary Not Supported:         Dim ndna1() As block3

                List<block3> ndna2 = new List<block3> { }; // TODO - Specified Minimum Array Boundary Not Supported:         Dim ndna2() As block3

                int length1 = 0;

                int length2 = 0;

                length1 = UBound(dna1);
                length2 = UBound(dna2);
                List<block3> ndna1_2125_tmp = new List<block3>();
                for (int redim_iter_1978 = 0; i < 0; redim_iter_1978++) { ndna1.Add(null); }
                List<block3> ndna2_7254_tmp = new List<block3>();
                for (int redim_iter_7961 = 0; i < 0; redim_iter_7961++) { ndna2.Add(null); }

                //map to nucli

                for (t = 0; t < UBound(dna1); t++)
                {
                    ndna1(t).nucli = DNAtoInt(dna1(t).tipo, dna1(t).value);
                }
                for (t = 0; t < UBound(dna2); t++)
                {
                    ndna2(t).nucli = DNAtoInt(dna2(t).tipo, dna2(t).value);
                }

                //Step3 Check longest sequences

                simplematch(ndna1, ndna2);

                //If robot is too unsimiler then do not reproduce and block sex reproduction for 8 cycles

                if (GeneticDistance(ndna1, ndna2) > 0.6m)
                {
                    rob(female).fertilized = -18;
                    return SexReproduce;
                }

                //Step4 map back

                for (t = 0; t < UBound(dna1); t++)
                {
                    dna1(t).match = ndna1(t).match;
                }

                for (t = 0; t < UBound(dna2); t++)
                {
                    dna2(t).match = ndna2(t).match;
                }

                //      'debug
                //      Dim k As String
                //      Dim temp As String
                //      Dim bp As block
                //      Dim converttosysvar As Boolean
                //      k = ""
                //      For t = 0 To UBound(dna1)

                //        If t = UBound(dna1) Then converttosysvar = False Else converttosysvar = IIf(dna1(t + 1).tipo = 7, True, False)
                //        bp.tipo = dna1(t).tipo
                //        bp.value = dna1(t).value
                //        temp = ""
                //        Parse temp, bp, 1, converttosysvar

                //      k = k & dna1(t).match & vbTab & temp & vbCrLf
                //      Next

                //      Clipboard.CLEAR
                //      Clipboard.SetText k
                //      MsgBox "---", , UBound(dna1) & " " & UBound(dna2)
                //      k = ""
                //      For t = 0 To UBound(dna2)

                //        If t = UBound(dna2) Then converttosysvar = False Else converttosysvar = IIf(dna2(t + 1).tipo = 7, True, False)
                //        bp.tipo = dna2(t).tipo
                //        bp.value = dna2(t).value
                //        temp = ""
                //        Parse temp, bp, 2, converttosysvar

                //      k = k & dna2(t).match & vbTab & temp & vbCrLf

                //      Next
                //      Clipboard.CLEAR
                //      Clipboard.SetText k
                //      MsgBox "done"

                //Step5 do crossover

                List<block> Outdna = new List<block> { }; // TODO - Specified Minimum Array Boundary Not Supported:       Dim Outdna() As block

                List<block> Outdna_6626_tmp = new List<block>();
                for (int redim_iter_5024 = 0; i < 0; redim_iter_5024++) { Outdna.Add(null); }
                crossover(dna1, dna2, Outdna);

                //Bug fix remove starting zero
                if (Outdna(0).value == 0 & Outdna(0).tipo == 0)
                {
                    for (t = 1; t < UBound(Outdna); t++)
                    {
                        Outdna[t - 1] == Outdna[t];
                    }
                    List<block> Outdna_9207_tmp = new List<block>();
                    for (int redim_iter_2132 = 0; i < 0; redim_iter_2132++) { Outdna.Add(redim_iter_2132 < Outdna.Count ? Outdna(redim_iter_2132) : null); }
                }

                nuovo = posto();
                SimOpts.TotBorn = SimOpts.TotBorn + 1;
                if (rob(female).Veg)
                {
                    totvegs = totvegs + 1;
                }

                //Step4 after robot is created store the dna

                rob(nuovo).dna = Outdna;

                rob(nuovo).DnaLen = DnaLen(rob(nuovo).dna()); // Set the DNA length of the offspring

                //Bugfix actual length = virtual length
                List<> rob_2735_tmp = new List<>();
                for (int redim_iter_9678 = 0; i < 0; redim_iter_9678++) { rob.Add(redim_iter_9678 < rob.Count ? rob(redim_iter_9678) : null); }

                rob(nuovo).genenum = CountGenes(rob(nuovo).dna());
                rob(nuovo).Mutables = rob(female).Mutables;

                rob(nuovo).Mutations = rob(female).Mutations;
                rob(nuovo).OldMutations = rob(female).OldMutations; //Botsareus 10/8/2015

                rob(nuovo).LastMut = 0;
                rob(nuovo).LastMutDetail = rob(female).LastMutDetail;

                for (t = 1; t < rob(female).maxusedvars; t++)
                {
                    rob(nuovo).usedvars(t) = rob(female).usedvars(t);
                }

                for (t = 0; t < 12; t++)
                {
                    rob(nuovo).Skin(t) = rob(female).Skin(t);
                }

                rob(nuovo).maxusedvars = rob(female).maxusedvars;
                Erase(rob(nuovo).mem);
                Erase(rob(nuovo).Ties);

                rob(nuovo).pos.X = rob(female).pos.X + absx(rob(female).aim, sondist, 0, 0, 0);
                rob(nuovo).pos.Y = rob(female).pos.Y + absy(rob(female).aim, sondist, 0, 0, 0);
                rob(nuovo).exist = true;
                rob(nuovo).BucketPos.X = -2;
                rob(nuovo).BucketPos.Y = -2;
                UpdateBotBucket(nuovo);

                rob(nuovo).vel = rob(female).vel;
                rob(nuovo).actvel = rob(female).actvel; //Botsareus 7/1/2016 Bugfix
                rob(nuovo).color = rob(female).color;
                rob(nuovo).aim = rob(female).aim + PI;
                if (rob(nuovo).aim > 6.28m)
                {
                    rob(nuovo).aim = rob(nuovo).aim - 2 * PI;
                }
                rob(nuovo).aimvector = VectorSet(Cos(rob(nuovo).aim), Sin(rob(nuovo).aim));
                rob(nuovo).mem(SetAim) = rob(nuovo).aim * 200;
                rob(nuovo).mem(468) = 32000;
                //      rob(nuovo).mem(480) = 32000 Botsareus 2/21/2013 Broken
                //      rob(nuovo).mem(481) = 32000
                //      rob(nuovo).mem(482) = 32000
                //      rob(nuovo).mem(483) = 32000
                rob(nuovo).Corpse = false;
                rob(nuovo).Dead = false;
                rob(nuovo).NewMove = rob(female).NewMove;
                rob(nuovo).generation = rob(female).generation + 1;
                if (rob(nuovo).generation > 32000)
                {
                    rob(nuovo).generation = 32000; //Botsareus 10/9/2015 Overflow protection
                }
                rob(nuovo).BirthCycle = SimOpts.TotRunCycle;
                rob(nuovo).vnum = 1;

                nnrg = (rob(female).nrg / 100) * CSng(per);
                nwaste = rob(female).Waste / 100 * CSng(per);
                npwaste = rob(female).Pwaste / 100 * CSng(per);
                nchloroplasts = (rob(female).chloroplasts / 100) * CSng(per); //Panda 8/23/2013 Distribute the chloroplasts

                rob(female).nrg = rob(female).nrg - nnrg - (nnrg * 0.001m); // Make reproduction cost 0.1% of nrg transfer for females
                                                                            //The male paid a cost to shoot the sperm but nothing more.

                rob(female).Waste = rob(female).Waste - nwaste;
                rob(female).Pwaste = rob(female).Pwaste - npwaste;
                rob(female).body = rob(female).body - nbody;
                rob(female).radius = FindRadius(female);
                rob(female).chloroplasts = rob(female).chloroplasts - nchloroplasts; //Panda 8/23/2013 Distribute the chloroplasts

                rob(nuovo).chloroplasts = nchloroplasts; //Botsareus 8/24/2013 Distribute the chloroplasts
                rob(nuovo).body = nbody;
                rob(nuovo).radius = FindRadius(nuovo);
                rob(nuovo).Waste = nwaste;
                rob(nuovo).Pwaste = npwaste;
                rob(female).mem(Energy) = CInt(rob(female).nrg);
                rob(female).mem(311) = rob(female).body;
                rob(female).SonNumber = rob(female).SonNumber + 1;

                // Need to track the absnum of shot parents before we can do this...
                // rob(male).SonNumber = rob(male).SonNumber + 1

                if (rob(female).SonNumber > 32000)
                {
                    rob(female).SonNumber = 32000; // EricL Overflow protection.  Should change to Long at some point.
                }
                // Need to track the absnum of shot parents before we can do this...
                // If rob(male).SonNumber > 32000 Then rob(male).SonNumber = 32000 ' EricL Overflow protection.  Should change to Long at some point.

                rob(nuovo).nrg = nnrg * 0.999m; // Make reproduction cost 1% of nrg transfer for offspring
                rob(nuovo).onrg = nnrg * 0.999m;
                rob(nuovo).mem(Energy) = CInt(rob(nuovo).nrg);
                rob(nuovo).Poisoned = false;
                rob(nuovo).parent = rob(female).AbsNum;
                rob(nuovo).FName = rob(female).FName;
                rob(nuovo).LastOwner = rob(female).LastOwner;
                rob(nuovo).Veg = rob(female).Veg;
                rob(nuovo).NoChlr = rob(female).NoChlr; //Botsareus 3/28/2014 Disable chloroplasts
                rob(nuovo).Fixed = rob(female).Fixed;
                rob(nuovo).CantSee = rob(female).CantSee;
                rob(nuovo).DisableDNA = rob(female).DisableDNA;
                rob(nuovo).DisableMovementSysvars = rob(female).DisableMovementSysvars;
                rob(nuovo).CantReproduce = rob(female).CantReproduce;
                rob(nuovo).VirusImmune = rob(female).VirusImmune;
                if (rob(nuovo).Fixed)
                {
                    rob(nuovo).mem(Fixed) = 1;
                }
                rob(nuovo).SubSpecies = rob(female).SubSpecies;

                //Botsareus 4/9/2013 we need to copy some variables for genetic distance
                rob(nuovo).OldGD = rob(female).OldGD;
                rob(nuovo).GenMut = rob(female).GenMut;
                //Botsareus 1/29/2014 Copy the tag
                rob(nuovo).tag = rob(female).tag;
                //Botsareus 7/22/2014 Both robots should have same boyancy
                rob(nuovo).Bouyancy = rob(female).Bouyancy;

                //Botsareus 7/29/2014 New kill restrictions
                if (rob(female).multibot_time > 0)
                {
                    rob(nuovo).multibot_time = rob(female).multibot_time / 2 + 2;
                }
                rob(nuovo).dq = rob(female).dq;

                //Botsareus 10/5/2015 freeing up memory from Eric's obsolete ancestors code
                //      For i = 0 To 500
                //        rob(nuovo).Ancestors(i) = rob(female).Ancestors(i)  ' copy the parents ancestor list
                //      Next i
                //      rob(nuovo).AncestorIndex = rob(female).AncestorIndex + 1  ' increment the ancestor index
                //      If rob(nuovo).AncestorIndex > 500 Then rob(nuovo).AncestorIndex = 0  ' wrap it
                //      rob(nuovo).Ancestors(rob(nuovo).AncestorIndex).num = rob(female).AbsNum  ' add the parent as the most recent ancestor
                //      rob(nuovo).Ancestors(rob(nuovo).AncestorIndex).mut = rob(female).LastMut ' add the number of mutations the parent has had up until now.
                //      rob(nuovo).Ancestors(rob(nuovo).AncestorIndex).sim = SimOpts.SimGUID ' Use this seed to uniqufiy this ancestor in Internet mode

                rob(nuovo).Vtimer = 0;
                rob(nuovo).virusshot = 0;

                //First 5 genetic memory locations happen instantly
                for (i = 0; i < 4; i++)
                {
                    rob(nuovo).mem(971 + i) == rob(female).mem(971 + i);
                }
                //The other 15 genetic memory locations are stored now but can be used later
                for (i = 0; i < 14; i++)
                {
                    rob(nuovo).epimem(i) = rob(female).mem(976 + i);
                }
                //Erase parents genetic memory now to prevent him from completing his own transfer by using his kid
                for (i = 0; i < 14; i++)
                {
                    rob(female).epimem(i) = 0;
                }

                logmutation(nuovo, "Female DNA len " + Str(rob(female).DnaLen) + " and male DNA len " + Str(UBound(rob(female).spermDNA)) + " had offspring DNA len " + Str(rob(nuovo).DnaLen) + " during cycle " + Str(SimOpts.TotRunCycle));

                if (Delta2)
                {
                    dynamic _WithVar_8390;
                    _WithVar_8390 = rob(nuovo);
                    int MratesMax = 0;

                    MratesMax = IIf(NormMut, CLng(_WithVar_8390.DnaLen) * CLng(valMaxNormMut), 2000000000);
                    //dynamic mutation overload correction
                    decimal dmoc = 0;

                    dmoc = 1 + (rob(nuovo).DnaLen - curr_dna_size) / 500;
                    if (dmoc < 0.01m)
                    {
                        dmoc = 0.01m; //Botsareus 1/16/2016 Bug fix
                    }
                    if (!y_normsize)
                    {
                        dmoc = 1;
                    }
                    //zerobot stabilization
                    if (x_restartmode == 7 || x_restartmode == 8)
                    {
                        if (_WithVar_8390.FName == "Mutate.txt")
                        {
                            //normalize child
                            _WithVar_8390.Mutables.mutarray(PointUP) = _WithVar_8390.Mutables.mutarray(PointUP) * 1.75m;
                            if (_WithVar_8390.Mutables.mutarray(PointUP) > MratesMax)
                            {
                                _WithVar_8390.Mutables.mutarray(PointUP) = MratesMax;
                            }
                            _WithVar_8390.Mutables.mutarray(P2UP) = _WithVar_8390.Mutables.mutarray(P2UP) * 1.75m;
                            if (_WithVar_8390.Mutables.mutarray(P2UP) > MratesMax)
                            {
                                _WithVar_8390.Mutables.mutarray(P2UP) = MratesMax;
                            }
                        }
                    }

                    for (t = 1; t < 10; t++)
                    {
                        if (t == 9)
                        {
                            goto skip; //ignore PM2 mutation here
                        }
                        if (_WithVar_8390.Mutables.mutarray(t) < 1)
                        {
                            goto skip; //Botsareus 1/3/2014 if mutation off then skip it
                        }
                        if (rndy() < DeltaMainChance / 100)
                        {
                            if (DeltaMainExp != 0)
                            {
                                if ((t == CopyErrorUP || t == TranslocationUP || t == ReversalUP || t == CE2UP))
                                {
                                    _WithVar_8390.Mutables.mutarray(t) = _WithVar_8390.Mutables.mutarray(t) * (dmoc + 2) / 3;
                                }
                                else
                                {
                                    if (!(t == MinorDeletionUP || t == MajorDeletionUP))
                                    {
                                        _WithVar_8390.Mutables.mutarray(t) = _WithVar_8390.Mutables.mutarray(t) * dmoc; //dynamic mutation overload correction
                                    }
                                }
                                _WithVar_8390.Mutables.mutarray(t) = _WithVar_8390.Mutables.mutarray(t) * 10 ^ ((rndy() * 2 - 1) / DeltaMainExp);
                            }
                            _WithVar_8390.Mutables.mutarray(t) = _WithVar_8390.Mutables.mutarray(t) + (rndy() * 2 - 1) * DeltaMainLn;
                            if (_WithVar_8390.Mutables.mutarray(t) < 1)
                            {
                                _WithVar_8390.Mutables.mutarray(t) = 1;
                            }
                            if (_WithVar_8390.Mutables.mutarray(t) > MratesMax)
                            {
                                _WithVar_8390.Mutables.mutarray(t) = MratesMax;
                            }
                        }
                        if (rndy() < DeltaDevChance / 100)
                        {
                            if (DeltaDevExp != 0)
                            {
                                _WithVar_8390.Mutables.StdDev(t) = _WithVar_8390.Mutables.StdDev(t) * 10 ^ ((rndy() * 2 - 1) / DeltaDevExp);
                            }
                            _WithVar_8390.Mutables.StdDev(t) = _WithVar_8390.Mutables.StdDev(t) + (rndy() * 2 - 1) * DeltaDevLn;
                            if (DeltaDevExp != 0)
                            {
                                _WithVar_8390.Mutables.Mean(t) = _WithVar_8390.Mutables.Mean(t) * 10 ^ ((rndy() * 2 - 1) / DeltaDevExp);
                            }
                            _WithVar_8390.Mutables.Mean(t) = _WithVar_8390.Mutables.Mean(t) + (rndy() * 2 - 1) * DeltaDevLn;
                            //Max range is always 0 to 800
                            if (_WithVar_8390.Mutables.StdDev(t) < 0)
                            {
                                _WithVar_8390.Mutables.StdDev(t) = 0;
                            }
                            if (_WithVar_8390.Mutables.StdDev(t) > 200)
                            {
                                _WithVar_8390.Mutables.StdDev(t) = 200;
                            }
                            if (_WithVar_8390.Mutables.Mean(t) < 1)
                            {
                                _WithVar_8390.Mutables.Mean(t) = 1;
                            }
                            if (_WithVar_8390.Mutables.Mean(t) > 400)
                            {
                                _WithVar_8390.Mutables.Mean(t) = 400;
                            }
                        }
                    skip:
                      }
                    _WithVar_8390.Mutables.CopyErrorWhatToChange = _WithVar_8390.Mutables.CopyErrorWhatToChange + (rndy() * 2 - 1) * DeltaWTC;
                    if (_WithVar_8390.Mutables.CopyErrorWhatToChange < 0)
                    {
                        _WithVar_8390.Mutables.CopyErrorWhatToChange = 0;
                    }
                    if (_WithVar_8390.Mutables.CopyErrorWhatToChange > 100)
                    {
                        _WithVar_8390.Mutables.CopyErrorWhatToChange = 100;
                    }
                    mutate(nuovo, true);
                }
                else
                {
                    mutate(nuovo, true);
                }

                makeoccurrlist(nuovo);
                rob(nuovo).DnaLen = DnaLen(rob(nuovo).dna());
                rob(nuovo).genenum = CountGenes(rob(nuovo).dna());
                rob(nuovo).mem(DnaLenSys) = rob(nuovo).DnaLen;
                rob(nuovo).mem(GenesSys) = rob(nuovo).genenum;

                maketie(female, nuovo, sondist, 100, 0); //birth ties last 100 cycles
                rob(female).onrg = rob(female).nrg; //saves mother from dying from shock after giving birth
                rob(nuovo).mass = nbody / 1000 + rob(nuovo).shell / 200;
                rob(nuovo).mem(timersys) = rob(female).mem(timersys); //epigenetic timer

                //A little hack here to remain in control of reproduced robots
                if (MDIForm1.instance.pbOn.Checked)
                {
                    if (female == robfocus || rob(female).highlight)
                    {
                        rob(nuovo).highlight = true;
                    }
                }

                rob(female).mem(SEXREPRO) = 0; // sucessfully reproduced, so reset .sexrepro
                rob(female).fertilized = -1; // Set to -1 so spermDNA space gets reclaimed next cycle
                rob(female).mem(SYSFERTILIZED) = 0; // Sperm is only good for one birth presently

                //Botsareus 11/29/2013 Reset epigenetic memory
                if (epireset)
                {
                    rob(nuovo).MutEpiReset = rob(female).MutEpiReset + rob(nuovo).LastMut ^ epiresetemp;
                    if (rob(nuovo).MutEpiReset > epiresetOP && rob(female).MutEpiReset > 0)
                    {
                        rob(nuovo).MutEpiReset = 0;
                        for (i = 0; i < 4; i++)
                        {
                            rob(nuovo).mem(971 + i) == 0;
                        }
                        for (i = 0; i < 14; i++)
                        {
                            rob(nuovo).epimem(i) = 0;
                        }
                    }
                }

                rob(female).nrg = rob(female).nrg - rob(female).DnaLen * SimOpts.Costs(DNACOPYCOST) * SimOpts.Costs(COSTMULTIPLIER); //Botsareus 7/7/2013 Reproduction DNACOPY cost
                if (rob(female).nrg < 0)
                {
                    rob(female).nrg = 0;
                }
            }
        }
    getout:
        return SexReproduce;
    }

    public static void sharechloroplasts(int t, int k)
    { //Panda 8/31/2013 code to share chloroplasts
        decimal totchlr = 0;

        dynamic _WithVar_6639;
        _WithVar_6639 = rob(t);

        if (DoGeneticDistance(t, _WithVar_6639.Ties(k).pnt) > 0.25m)
        {
            _WithVar_6639.Chlr_Share_Delay = 8;
            return;
        }

        if (_WithVar_6639.mem(sharechlr) > 99)
        {
            _WithVar_6639.mem(sharechlr) = 99;
        }
        if (_WithVar_6639.mem(sharechlr) < 0)
        {
            _WithVar_6639.mem(sharechlr) = 0;
        }
        totchlr = _WithVar_6639.chloroplasts + rob(_WithVar_6639.Ties(k).pnt).chloroplasts;

        if (totchlr * (CSng(_WithVar_6639.mem(sharechlr)) / 100) < 32000)
        {
            _WithVar_6639.chloroplasts = totchlr * (CSng(_WithVar_6639.mem(sharechlr)) / 100);
        }
        else
        {
            _WithVar_6639.chloroplasts = 32000;
        }

        if (totchlr * ((100 - CSng(_WithVar_6639.mem(sharechlr))) / 100) < 32000)
        {
            rob(_WithVar_6639.Ties(k).pnt).chloroplasts = totchlr * ((100 - CSng(_WithVar_6639.mem(sharechlr))) / 100);
        }
        else
        {
            rob(_WithVar_6639.Ties(k).pnt).chloroplasts = 32000;
        }
    }

    public static void sharenrg(int t, int k)
    {
        decimal totnrg = 0;

        decimal portionThatsMine = 0;

        decimal myChangeInNrg = 0;

        dynamic _WithVar_9611;
        _WithVar_9611 = rob(t);

        //This is an order of operation thing.  A bot earlier in the rob array might have taken all your nrg, taking your
        //nrg to 0.  You should still be able to take some back.
        if (rob(t).nrg < 0 || rob(_WithVar_9611.Ties(k).pnt).nrg < 0)
        {
            goto getout; // Can't transfer nrg if nrg is negative
        }

        //.mem(830) is the percentage of the total nrg this bot wants to receive
        //has to be positive to come here, so no worries about changing the .mem location here
        if (rob(t).mem(830) <= 0)
        {
            rob(t).mem(830) = 0;
        }
        else
        {
            rob(t).mem(830) = rob(t).mem(830) % 100;
            if (rob(t).mem(830) == 0)
            {
                rob(t).mem(830) = 100;
            }
        }

        //Total nrg of both bots combined
        totnrg = rob(t).nrg + rob(_WithVar_9611.Ties(k).pnt).nrg;

        portionThatsMine = totnrg * (CSng(rob(t).mem(830)) / 100); // This is what the bot wants to have of the total
        if (portionThatsMine > 32000)
        {
            portionThatsMine = 32000; // Can't want more than the max a bot can have
        }
        myChangeInNrg = portionThatsMine - rob(t).nrg; // This is what the bot's change in nrg would be

        //If the bot is taking nrg, then he can't take more than that represented by his own body.  If giving nrg away, same thing.  The bot
        //can't give away more than that represented by his body.  Should make it so that larger bots win tie feeding battles.
        if (Abs(myChangeInNrg) > (_WithVar_9611.body))
        {
            myChangeInNrg = Sgn(myChangeInNrg) * (_WithVar_9611.body);
        }

        if (_WithVar_9611.nrg + myChangeInNrg > 32000)
        {
            myChangeInNrg = 32000 - _WithVar_9611.nrg; //Limit change if it would put bot over the limit
        }
        if (_WithVar_9611.nrg + myChangeInNrg < 0)
        {
            myChangeInNrg = -.nrg; //Limit change if it would take the bot below 0
        }

        //Now we have to check the limits on the other bot
        //sign is negative since the negative of myChangeinNrg is what the other bot is going to get/recevie
        if (rob(_WithVar_9611.Ties(k).pnt).nrg - myChangeInNrg > 32000)
        {
            myChangeInNrg = ((32000 - rob(_WithVar_9611.Ties(k).pnt).nrg,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,, - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -); //Limit change if it would put bot over the limit
        }
        if (rob(_WithVar_9611.Ties(k).pnt).nrg - myChangeInNrg < 0)
        {
            myChangeInNrg = rob(_WithVar_9611.Ties(k).pnt).nrg; // limit change if it would take the bot below 0
        }

        //Do the actual nrg exchange
        _WithVar_9611.nrg = _WithVar_9611.nrg + myChangeInNrg;
        rob(_WithVar_9611.Ties(k).pnt).nrg = rob(_WithVar_9611.Ties(k).pnt).nrg - myChangeInNrg;

        //Transferring nrg costs nrg.  1% of the transfer gets deducted from the bot iniating the transfer
        _WithVar_9611.nrg = _WithVar_9611.nrg - (Abs(myChangeInNrg) * 0.01m);

        //Bots with 32000 nrg can still take or receive nrg, but everything over 32000 disappears
        if (_WithVar_9611.nrg > 32000)
        {
            _WithVar_9611.nrg = 32000;
        }
        if (rob(_WithVar_9611.Ties(k).pnt).nrg > 32000)
        {
            rob(_WithVar_9611.Ties(k).pnt).nrg = 32000;
        }
    getout:
}

    public static void shareshell(int t, int k)
    {
        decimal totshell = 0;

        dynamic _WithVar_1662;
        _WithVar_1662 = rob(t);
        if (_WithVar_1662.mem(832) > 99)
        {
            _WithVar_1662.mem(832) = 99;
        }
        if (_WithVar_1662.mem(832) < 0)
        {
            _WithVar_1662.mem(832) = 0;
        }
        totshell = _WithVar_1662.shell + rob(_WithVar_1662.Ties(k).pnt).shell;

        if (totshell * ((100 - CSng(_WithVar_1662.mem(832))) / 100) < 32000)
        {
            rob(_WithVar_1662.Ties(k).pnt).shell = totshell * ((100 - CSng(_WithVar_1662.mem(832))) / 100);
        }
        else
        {
            rob(_WithVar_1662.Ties(k).pnt).shell = 32000;
        }
        if (totshell * (CSng(_WithVar_1662.mem(832)) / 100) < 32000)
        {
            _WithVar_1662.shell = totshell * (CSng(_WithVar_1662.mem(832)) / 100);
        }
        else
        {
            _WithVar_1662.shell = 32000;
        }
        _WithVar_1662.mem(823) = CInt(_WithVar_1662.shell); // update the .shell sysvar
        rob(_WithVar_1662.Ties(k).pnt).mem(823) = rob(_WithVar_1662.Ties(k).pnt).shell;
    }

    public static void shareslime(int t, int k)
    { //robot shares slime with others in the same multibot structure
        decimal totslime = 0;

        dynamic _WithVar_5800;
        _WithVar_5800 = rob(t);
        if (_WithVar_5800.mem(833) > 99)
        {
            _WithVar_5800.mem(833) = 99;
        }
        if (_WithVar_5800.mem(833) < 0)
        {
            _WithVar_5800.mem(833) = 0;
        }
        totslime = _WithVar_5800.Slime + rob(_WithVar_5800.Ties(k).pnt).Slime;

        if (totslime * (CSng(_WithVar_5800.mem(833)) / 100) < 32000)
        {
            _WithVar_5800.Slime = totslime * (CSng(_WithVar_5800.mem(833)) / 100);
        }
        else
        {
            _WithVar_5800.Slime = 32000;
        }
        if (totslime * ((100 - CSng(_WithVar_5800.mem(833))) / 100) < 32000)
        {
            rob(_WithVar_5800.Ties(k).pnt).Slime = totslime * ((100 - CSng(_WithVar_5800.mem(833))) / 100);
        }
        else
        {
            rob(_WithVar_5800.Ties(k).pnt).Slime = 32000;
        }
    }

    public static void sharewaste(int t, int k)
    {
        decimal totwaste = 0;

        dynamic _WithVar_1001;
        _WithVar_1001 = rob(t);
        if (_WithVar_1001.mem(831) > 99)
        {
            _WithVar_1001.mem(831) = 99;
        }
        if (_WithVar_1001.mem(831) < 0)
        {
            _WithVar_1001.mem(831) = 0;
        }
        totwaste = _WithVar_1001.Waste + rob(_WithVar_1001.Ties(k).pnt).Waste;

        if (totwaste * (CSng(_WithVar_1001.mem(831)) / 100) < 32000)
        {
            _WithVar_1001.Waste = totwaste * (CSng(_WithVar_1001.mem(831)) / 100);
        }
        else
        {
            _WithVar_1001.Waste = 32000;
        }
        if (totwaste * ((100 - CSng(_WithVar_1001.mem(831))) / 100) < 32000)
        {
            rob(_WithVar_1001.Ties(k).pnt).Waste = totwaste * ((100 - CSng(_WithVar_1001.mem(831))) / 100);
        }
        else
        {
            rob(_WithVar_1001.Ties(k).pnt).Waste = 32000;
        }
    }

    public static bool simplecoll(int X, int Y, int k)
    {
        bool simplecoll = false;
        int t = 0;

        int radius = 0;

        simplecoll = false;

        for (t = 1; t < MaxRobs; t++)
        {
            if (rob(t).exist && !(rob(t).FName == "Base.txt" && hidepred))
            {
                if (Abs(rob(t).pos.X - X) < rob(t).radius + rob(k).radius && Abs(rob(t).pos.Y - Y) < rob(t).radius + rob(k).radius)
                {
                    if (k != t)
                    {
                        simplecoll = true;
                        goto ;
                    }
                }
            }
        }

        //EricL Can't reproduce into or across a shape
        for (t = 1; t < numObstacles; t++)
        {
            if (!((Obstacles.Obstacles(t).pos.X > Max(rob(k).pos.X, X)) || (Obstacles.Obstacles(t).pos.X + Obstacles.Obstacles(t).Width < Min(rob(k).pos.X, X)) || (Obstacles.Obstacles(t).pos.Y > Max(rob(k).pos.Y, Y)) || (Obstacles.Obstacles(t).pos.Y + Obstacles.Obstacles(t).Height < Min(rob(k).pos.Y, Y))))
            {
                simplecoll = true;
                goto ;
            }
        }

        if (SimOpts.Dxsxconnected == false)
        {
            if (X < rob(k).radius + smudgefactor || X + rob(k).radius + smudgefactor > SimOpts.FieldWidth)
            {
                simplecoll = true;
            }
        }

        if (SimOpts.Updnconnected == false)
        {
            if (Y < rob(k).radius + smudgefactor || Y + rob(k).radius + smudgefactor > SimOpts.FieldHeight)
            {
                simplecoll = true;
            }
        }
    getout:
        return simplecoll;
    }

    public static void storepoison(int n)
    {
        decimal Cost = 0;

        decimal Delta = 0;

        decimal poisonNrgConvRate = 0;

        poisonNrgConvRate = 0.25m; //Botsareus 6/23/2016 Make 4 poison for 1 nrg

        dynamic _WithVar_4249;
        _WithVar_4249 = rob[n];
        if (_WithVar_4249.nrg <= 0)
        {
            goto getout; // Can't make or unmake poison if nrg is negative
        }

        if (_WithVar_4249.mem(826) > 32000)
        {
            _WithVar_4249.mem(826) = 32000;
        }
        if (_WithVar_4249.mem(826) < -32000)
        {
            _WithVar_4249.mem(826) = -32000;
        }

        Delta = _WithVar_4249.mem(826); // This is what the bot wants to do to his poison, up or down

        if (Abs(Delta) > _WithVar_4249.nrg / poisonNrgConvRate)
        {
            Delta = Sgn(Delta) * _WithVar_4249.nrg / poisonNrgConvRate; // Can't make or unmake more poison than you have nrg
        }

        if (Abs(Delta) > 100)
        {
            Delta = Sgn(Delta) * 100; // Can't make or unmake more than 100 poison at a time
        }
        if (_WithVar_4249.poison + Delta > 32000)
        {
            Delta = 32000 - _WithVar_4249.poison; // poison can't go above 32000
        }
        if (_WithVar_4249.poison + Delta < 0)
        {
            Delta = -.poison; // poison can't go below 0
        }

        _WithVar_4249.poison = _WithVar_4249.poison + Delta; // Make the change in poison
        _WithVar_4249.nrg = _WithVar_4249.nrg - (Abs(Delta) * poisonNrgConvRate); // Making or unmaking poison takes nrg

        //This is the transaction cost
        Cost = Abs(Delta) * SimOpts.Costs(POISONCOST) * SimOpts.Costs(COSTMULTIPLIER);

        _WithVar_4249.nrg = _WithVar_4249.nrg - Cost;

        _WithVar_4249.Waste = _WithVar_4249.Waste + Cost; // waste is created proportional to the transaction cost

        _WithVar_4249.mem(826) = 0; // reset the .mkpoison sysvar
        _WithVar_4249.mem(827) = CInt(_WithVar_4249.poison); // update the .poison sysvar
    getout:
        //Botsareus 3/14/2014 Disqualify
        if ((SimOpts.F1 || x_restartmode == 1) && Disqualify == 2)
        {
            dreason(_WithVar_4249.FName, _WithVar_4249.tag, "making poison");
        }
        if (!SimOpts.F1 && _WithVar_4249.dq == 1 && Disqualify == 2)
        {
            rob[n].Dead = true; //safe kill robot
        }
    }

    public static void storevenom(int n)
    {
        decimal Cost = 0;

        decimal Delta = 0;

        decimal venomNrgConvRate = 0;

        venomNrgConvRate = 1; // Make 1 venom for 1 nrg

        dynamic _WithVar_9234;
        _WithVar_9234 = rob[n];
        if (_WithVar_9234.nrg <= 0)
        {
            goto getout; // Can't make or unmake venom if nrg is negative
        }

        if (_WithVar_9234.mem(824) > 32000)
        {
            _WithVar_9234.mem(824) = 32000;
        }
        if (_WithVar_9234.mem(824) < -32000)
        {
            _WithVar_9234.mem(824) = -32000;
        }

        Delta = _WithVar_9234.mem(824); // This is what the bot wants to do to his venom, up or down

        if (Abs(Delta) > _WithVar_9234.nrg / venomNrgConvRate)
        {
            Delta = Sgn(Delta) * _WithVar_9234.nrg / venomNrgConvRate; // Can't make or unmake more venom than you have nrg
        }

        if (Abs(Delta) > 100)
        {
            Delta = Sgn(Delta) * 100; // Can't make or unmake more than 100 venom at a time
        }
        if (_WithVar_9234.venom + Delta > 32000)
        {
            Delta = 32000 - _WithVar_9234.venom; // venom can't go above 32000
        }
        if (_WithVar_9234.venom + Delta < 0)
        {
            Delta = -.venom; // venom can't go below 0
        }

        _WithVar_9234.venom = _WithVar_9234.venom + Delta; // Make the change in venom
        _WithVar_9234.nrg = _WithVar_9234.nrg - (Abs(Delta) * venomNrgConvRate); // Making or unmaking venom takes nrg

        //This is the transaction cost
        Cost = Abs(Delta) * SimOpts.Costs(VENOMCOST) * SimOpts.Costs(COSTMULTIPLIER);

        _WithVar_9234.nrg = _WithVar_9234.nrg - Cost;

        _WithVar_9234.Waste = _WithVar_9234.Waste + Cost; // waste is created proportional to the transaction cost

        _WithVar_9234.mem(824) = 0; // reset the .mkvenom sysvar
        _WithVar_9234.mem(825) = Int(_WithVar_9234.venom); // update the .venom sysvar
    getout:
        //Botsareus 3/14/2014 Disqualify
        if ((SimOpts.F1 || x_restartmode == 1) && Disqualify == 2)
        {
            dreason(_WithVar_9234.FName, _WithVar_9234.tag, "making venom");
        }
        if (!SimOpts.F1 && _WithVar_9234.dq == 1 && Disqualify == 2)
        {
            rob[n].Dead = true; //safe kill robot
        }
    }

    public static void UpdateBots()
    {
        int t = 0;

        int i = 0;

        int k = 0;

        int c = 0;

        int z = 0;

        int q = 0;

        decimal ti = 0;

        int X = 0;

        decimal staticV = 0;

        rp = 1;
        kl = 1;
        kil(1) = 0;
        rep(1) = 0;
        TotalEnergy = 0;
        totwalls = 0;
        totcorpse = 0;
        //PopulationLastCycle = totnvegsDisplayed Botsareus 8/4/2014 Trying to save on memory by removing pointless defenitions
        TotalRobotsDisplayed = TotalRobots;
        TotalRobots = 0;
        totnvegsDisplayed = totnvegs;
        totnvegs = 0;
        totvegsDisplayed = totvegs;
        totvegs = 0;

        if (ContestMode)
        {
            F1count = F1count + 1;
            if (F1count == SampFreq)
            {
                Countpop();
            }
        }

        //Need to do this first as NetForces can update bots later in the loop
        for (t = 1; t < MaxRobs; t++)
        {
            if (rob(t).exist && !(rob(t).FName == "Base.txt" && hidepred))
            {
                if (numTeleporters > 0)
                {
                    CheckTeleporters(t);
                }
            }
        }

        //Only calculate mass due to fuild displacement if the sim medium has density.
        if (SimOpts.Density != 0)
        {
            for (t = 1; t < MaxRobs; t++)
            {
                if (rob(t).exist && !(rob(t).FName == "Base.txt" && hidepred))
                {
                    AddedMass(t);
                }
            }
        }

        //Botsareus 8/23/2014 Lets figure tidal force
        if (TmpOpts.Tides == 0)
        {
            BouyancyScaling = 1;
        }
        else
        {
            BouyancyScaling = (1 + Sin(((SimOpts.TotRunCycle + TmpOpts.TidesOf) % TmpOpts.Tides) / SimOpts.Tides * PI * 2)) / 2;
            BouyancyScaling = Sqr(BouyancyScaling);
            SimOpts.Ygravity = (1 - BouyancyScaling) * 4;
            SimOpts.PhysBrown = IIf(BouyancyScaling > 0.8m, 10, 0);
        }

        //this loops is for pre update
        for (t = 1; t < MaxRobs; t++)
        {
            if (t % 250 == 0)
            {
                DoEvents();
            }
            if (rob(t).exist && !(rob(t).FName == "Base.txt" && hidepred))
            {
                if ((rob(t).Corpse == false))
                {
                    Upkeep(t); // No upkeep costs if you are dead!
                }
                if (((rob(t).Corpse == false) && (rob(t).DisableDNA == false)))
                {
                    Poisons(t);
                }
                if (!SimOpts.DisableFixing)
                {
                    ManageFixed(t); //Botsareus 8/5/2014 Call function only if allowed
                }
                CalcMass(t);
                if (numObstacles > 0)
                {
                    DoObstacleCollisions(t);
                }
                bordercolls(t);
                TieHooke(t); // Handles tie lengths, tie hardening and compressive, elastic tie forces
                if (!rob(t).Corpse && !rob(t).DisableDNA)
                {
                    TieTorque(t); //EricL 4/21/2006 Handles tie angles
                }
                if (!rob(t).Fixed)
                {
                    NetForces(t); //calculate forces on all robots
                }
                BucketsCollision(t);
                //Botsareus 6/17/2016 Static friction fix
                if (rob(t).ImpulseStatic > 0 & (rob(t).ImpulseInd.X != 0 || rob(t).ImpulseInd.Y != 0))
                {
                    if (rob(t).vel.X == 0 & rob(t).vel.Y == 0)
                    {
                        staticV = rob(t).ImpulseStatic;
                    }
                    else
                    {
                        //Takes into account the fact that the robot may be moving along the same vector
                        staticV = rob(t).ImpulseStatic * Abs(Cross(VectorUnit(rob(t).vel), VectorUnit(rob(t).ImpulseInd)));
                    }
                    if (staticV > VectorMagnitude(rob(t).ImpulseInd))
                    {
                        rob(t).ImpulseInd = VectorSet(0, 0); //If static vector is greater then impulse vector, reset impulse vector
                    }
                }
                rob(t).ImpulseInd = VectorSub(rob(t).ImpulseInd, rob(t).ImpulseRes);

                if (!rob(t).Corpse && !rob(t).DisableDNA)
                {
                    tieportcom(t); //transfer data through ties
                }
                if (!rob(t).Corpse && !rob(t).DisableDNA)
                {
                    readtie(t); //reads all of the tref variables from a given tie number
                }
            }
        }

        DoEvents();

        // Don't handle events durign this next section cause we are updating species population numbers...
        i = 0;
        While(i < SimOpts.SpeciesNum);
        SimOpts.Specie(i).population = 0;
        i = i + 1;
        Wend();
        for (t = 1; t < MaxRobs; t++)
        {
            if (rob(t).exist && !(rob(t).FName == "Base.txt" && hidepred))
            {
                UpdateCounters(t); // Counts the number of bots and decays body...
            }
        }

        DoEvents();

        for (t = 1; t < MaxRobs; t++)
        {
            if (t % 250 == 0)
            {
                DoEvents();
            }

            if (rob(t).exist && !(rob(t).FName == "Base.txt" && hidepred))
            {
                Update_Ties(t); // Carries all tie routines

                //EricL Transfer genetic meomory locations for newborns through the birth tie during their first 15 cycles
                if (rob(t).age < 15)
                {
                    DoGeneticMemory(t);
                }

                if (!rob(t).Corpse && !rob(t).DisableDNA)
                {
                    SetAimFunc(t); //Setup aiming
                }
                if (!rob(t).Corpse && !rob(t).DisableDNA)
                {
                    BotDNAManipulation(t);
                }
                UpdatePosition(t); //updates robot's position
                                   //EricL 4/9/2006 Got rid of a loop below by moving these inside this loop.  Should speed things up a little.
                if (rob(t).nrg > 32000)
                {
                    rob(t).nrg = 32000;
                }

                //EricL 4/14/2006 Allow energy to continue to be negative to address loophole
                //where bots energy goes neagative above, gets reset to 0 here and then they only have to feed a tiny bit
                //from body.
                if (rob(t).nrg < -32000)
                {
                    rob(t).nrg = -32000;
                }

                if (rob(t).poison > 32000)
                {
                    rob(t).poison = 32000;
                }
                if (rob(t).poison < 0)
                {
                    rob(t).poison = 0;
                }

                if (rob(t).venom > 32000)
                {
                    rob(t).venom = 32000;
                }
                if (rob(t).venom < 0)
                {
                    rob(t).venom = 0;
                }

                if (rob(t).Waste > 32000)
                {
                    rob(t).Waste = 32000;
                }
                if (rob(t).Waste < 0)
                {
                    rob(t).Waste = 0;
                }
            }
        }
        DoEvents();

        //Botsareus 4/17/2013 Prevent big birthas Replaced with chloroplasts check later, chloroplasts must be less then 1/2 of body for check to happen
        for (t = 1; t < MaxRobs; t++)
        {
            if (rob(t).chloroplasts < rob(t).body / 2 || rob(t).Kills > 5)
            { //Bug fix here to prevent huge killer vegys
                if (rob(t).exist && rob(t).body > bodyfix)
                {
                    KillRobot(t);
                }
            }
        }

        for (t = 1; t < MaxRobs; t++)
        {
            if (t % 250 == 0)
            {
                DoEvents();
            }
            UpdateTieAngles(t); // Updates .tielen and .tieang.  Have to do this here after all bot movement happens above.

            if (!rob(t).Corpse && !rob(t).DisableDNA && rob(t).exist && !(rob(t).FName == "Base.txt" & hidepred))
            {
                mutate(t);
                MakeStuff(t);
                HandleWaste(t);
                Shooting(t);
                if (!rob(t).NoChlr)
                {
                    ManageChlr(t); //Botsareus 3/28/2014 Disable Chloroplasts
                }
                ManageBody(t);
                ManageBouyancy(t);
                ManageReproduction(t);
                Shock(t);
                WriteSenses(t);
                FireTies(t);
            }
            if (!rob(t).Corpse && rob(t).exist && !(rob(t).FName == "Base.txt" && hidepred))
            {
                Ageing(t); // Even bots with disabled DNA age...
                ManageDeath(t); // Even bots with disabled DNA can die...
            }
            if (rob(t).exist)
            {
                TotalSimEnergy(CurrentEnergyCycle) = TotalSimEnergy(CurrentEnergyCycle) + rob(t).nrg + rob(t).body * 10;
            }
        }
        //DoEvents
        ReproduceAndKill();
        RemoveExtinctSpecies();

        //Restart
        //Leaguemode handles restarts differently so only restart here if not in leaguemode
        if (totnvegs == 0 & SimOpts.Restart && !SimOpts.F1)
        { //Botsareus 6/11/2013 Using SimOpts instead of raw RestartMode
          // totnvegs = 1
          // Contests = Contests + 1
            ReStarts = ReStarts + 1;
            // Form1.StartSimul
            StartAnotherRound = true;
        }
    }

    public static void UpdatePosition(int n)
    {
        int t = 0;

        decimal vt = 0;

        dynamic _WithVar_8620;
        _WithVar_8620 = rob[n];

        //Following line commented since mass is set earlier in CalcMass
        //.mass = (.body / 1000) + (.shell / 200) 'set value for mass
        if (_WithVar_8620.mass + _WithVar_8620.AddedMass < 0.25m)
        {
            _WithVar_8620.mass = 0.25m - _WithVar_8620.AddedMass; // a fudge since Euler approximation can't handle it when mass -> 0
        }

        if (!.Fixed)
        {
            // speed normalization

            _WithVar_8620.vel = VectorAdd(_WithVar_8620.vel, VectorScalar(_WithVar_8620.ImpulseInd, 1 / (_WithVar_8620.mass + _WithVar_8620.AddedMass)));

            vt = VectorMagnitudeSquare(_WithVar_8620.vel);
            if (vt > SimOpts.MaxVelocity * SimOpts.MaxVelocity)
            {
                _WithVar_8620.vel = VectorScalar(VectorUnit(_WithVar_8620.vel), SimOpts.MaxVelocity);
                vt = SimOpts.MaxVelocity * SimOpts.MaxVelocity;
            }

            _WithVar_8620.pos = VectorAdd(_WithVar_8620.pos, _WithVar_8620.vel);
            UpdateBotBucket(n);
            // If .pos.x > 10000000 Then t = 1 / 0 ' Crash inducing line for debugging
        }
        else
        {
            _WithVar_8620.vel = VectorSet(0, 0);
        }

        //Have to do these here for both fixed and unfixed bots to avoid build up of forces in case fixed bots become unfixed.
        _WithVar_8620.ImpulseInd = VectorSet(0, 0);
        _WithVar_8620.ImpulseRes = _WithVar_8620.ImpulseInd;
        _WithVar_8620.ImpulseStatic = 0;

        if (SimOpts.ZeroMomentum == true)
        {
            _WithVar_8620.vel = VectorSet(0, 0);
        }

        _WithVar_8620.lastup = _WithVar_8620.mem(dirup);
        _WithVar_8620.lastdown = _WithVar_8620.mem(dirdn);
        _WithVar_8620.lastleft = _WithVar_8620.mem(dirsx);
        _WithVar_8620.lastright = _WithVar_8620.mem(dirdx);
        _WithVar_8620.mem(dirup) = 0;
        _WithVar_8620.mem(dirdn) = 0;
        _WithVar_8620.mem(dirdx) = 0;
        _WithVar_8620.mem(dirsx) = 0;

        _WithVar_8620.mem(velscalar) = iceil(Sqr(vt));
        _WithVar_8620.mem(vel) = iceil(Cos(_WithVar_8620.aim) * _WithVar_8620.vel.X + Sin(_WithVar_8620.aim) * _WithVar_8620.vel.Y * -1);
        _WithVar_8620.mem(veldn) = _WithVar_8620.mem(vel) * -1;
        _WithVar_8620.mem(veldx) = iceil(Sin(_WithVar_8620.aim) * _WithVar_8620.vel.X + Cos(_WithVar_8620.aim) * _WithVar_8620.vel.Y);
        _WithVar_8620.mem(velsx) = _WithVar_8620.mem(veldx) * -1;

        _WithVar_8620.mem(masssys) = _WithVar_8620.mass;
        _WithVar_8620.mem(maxvelsys) = SimOpts.MaxVelocity;
    }

    private static void Ageing(int n)
    {
        int tempAge = 0;// EricL 4/13/2006 Added this to allow age to grow beyond 32000

        //aging
        rob[n].age = rob[n].age + 1;
        rob[n].newage = rob[n].newage + 1; //Age this simulation to be used by tie code
        tempAge = rob[n].age;
        if (tempAge > 32000)
        {
            tempAge = 32000;
        }
        rob[n].mem(robage) = CInt(tempAge); //line added to copy robots age into a memory location
        rob[n].mem(timersys) = rob[n].mem(timersys) + 1; //update epigenetic timer
        if (rob[n].mem(timersys) > 32000)
        {
            rob[n].mem(timersys) = -32000;
        }
    }

    private static void altzheimer(int n)
    {
        //makes robots with high waste act in a bizarre fashion.
        int loc = 0;
        int val = 0;

        int loops = 0;

        int t = 0;

        loops = (rob[n].Pwaste + rob[n].Waste - SimOpts.BadWastelevel) / 4;

        for (t = 1; t < loops; t++)
        {
            do
            { //Botsareus 9/12/2014 From Testlund waste can not change chloroplasts
                loc = Random(1, 1000);
            } while (!(loc != mkchlr && loc != rmchlr);
            val = Random(-32000, 32000);
            rob[n].mem(loc) = val;
        }
    }

    private static void BotDNAManipulation(int n)
    {
        int length = 0;

        dynamic _WithVar_5306;
        _WithVar_5306 = rob[n];

        //count down
        if (_WithVar_5306.Vtimer > 1)
        {
            _WithVar_5306.Vtimer = _WithVar_5306.Vtimer - 1;
        }
        _WithVar_5306.mem(Vtimer) = _WithVar_5306.Vtimer;

        //Viruses
        if (_WithVar_5306.mem(mkvirus) > 0 & _WithVar_5306.Vtimer == 0)
        {
            //Botsareus 9/30/2014 Chloroplasts and viruses do not mix very well anymore
            if (_WithVar_5306.chloroplasts == 0)
            {
                //make the virus
                if (MakeVirus(n, _WithVar_5306.mem(mkvirus)))
                {
                    length = genelength(n, _WithVar_5306.mem(mkvirus)) * 2;
                    rob[n].nrg = rob[n].nrg - length / 2 * SimOpts.Costs(DNACOPYCOST) * SimOpts.Costs(COSTMULTIPLIER); //Botsareus 7/20/2013 Creating a virus costs a copy cost
                    if (length < 32000)
                    {
                        _WithVar_5306.Vtimer = length;
                    }
                    else
                    {
                        _WithVar_5306.Vtimer = 32000;
                    }
                }
                else
                {
                    _WithVar_5306.Vtimer = 0;
                    _WithVar_5306.virusshot = 0;
                }
            }
            else
            {
                _WithVar_5306.chloroplasts = 0;
                _WithVar_5306.radius = FindRadius(n);
            }
        }

        //shoot it!
        if (_WithVar_5306.mem(VshootSys) != 0 & _WithVar_5306.Vtimer == 1)
        { //Botsareus 10/5/2015 Bugfix for negative values in vshoot
            if (_WithVar_5306.virusshot <= maxshotarray && _WithVar_5306.virusshot > 0)
            {
                Vshoot(n, rob[n].virusshot);
            }

            _WithVar_5306.mem(VshootSys) = 0;
            _WithVar_5306.mem(Vtimer) = 0;
            _WithVar_5306.mem(mkvirus) = 0;
            _WithVar_5306.Vtimer = 0;
            _WithVar_5306.virusshot = 0;
        }

        //Other stuff

        if (_WithVar_5306.mem(DelgeneSys) > 0)
        {
            delgene(n, _WithVar_5306.mem(DelgeneSys));
            _WithVar_5306.mem(DelgeneSys) = 0;
        }

        _WithVar_5306.mem(DnaLenSys) = _WithVar_5306.DnaLen;
        _WithVar_5306.mem(GenesSys) = rob[n].genenum;
    }

    private static void ChangeChlr(int t)
    { //Panda 8/15/2013 change the number of chloroplasts
        decimal newnrg = 0;//Botsareus 10/6/2015 This will prevent robots from killing themselfs using chloroplasts

        dynamic _WithVar_8425;
        _WithVar_8425 = rob(t);

        decimal tmpchlr = 0;//Botsareus 8/24/2013 used to charge energy for adding chloroplasts

        tmpchlr = _WithVar_8425.chloroplasts;

        //add chloroplasts
        _WithVar_8425.chloroplasts = _WithVar_8425.chloroplasts + _WithVar_8425.mem(mkchlr);

        //remove chloroplasts
        _WithVar_8425.chloroplasts = _WithVar_8425.chloroplasts - _WithVar_8425.mem(rmchlr);

        if (tmpchlr < _WithVar_8425.chloroplasts)
        {
            newnrg = _WithVar_8425.nrg - (_WithVar_8425.chloroplasts - tmpchlr) * SimOpts.Costs(CHLRCOST) * SimOpts.Costs(COSTMULTIPLIER);

            if ((TotalChlr > SimOpts.MaxPopulation && _WithVar_8425.Veg == true) || newnrg < 100)
            { //Botsareus 12/3/2013 Attempt to stop vegy spikes
                _WithVar_8425.chloroplasts = tmpchlr;
            }
            else
            {
                _WithVar_8425.nrg = newnrg; //Botsareus 8/24/2013 only charge energy for adding chloroplasts to prevent robots from cheating by adding and subtracting there chlroplasts in 3 cycles
            }
        }
        rob(t).mem(mkchlr) = 0;
        rob(t).mem(rmchlr) = 0;
    }

    private static void crossover(dynamic rob1(_UNUSED) {
        int i = 0;//layer

        int n1 = 0;//start pos

        int n2 = 0;

        int nn = 0;

        int res1 = 0;//result1

        int res2 = 0;

        int resn = 0;

        int upperbound = 0;

        int a = 0;//looper

        bool nfirst = false;//is it not the first loop

        do
        {
            //diff search

            n1 = res1 + resn - nn;
            n2 = res2 + resn - nn;

            //presets
            i = 0;
            if (nfirst)
            {
                upperbound = UBound(Outdna);
            }
            else
            {
                nfirst = true;
                upperbound = -1;
            }

            res1 = scanfromn(rob1, n1, 0);
            res2 = scanfromn(rob2, n2, i);

            //subloop
            if (res1 - n1 > 0 & res2 - n2 > 0)
            { //run both sides
                if (Int(rndy() * 2) == 0)
                { //which side?
                    List<> Outdna_1242_tmp = new List<>();
                    for (int redim_iter_4940 = 0; i < 0; redim_iter_4940++) { Outdna.Add(redim_iter_4940 < Outdna.Count ? Outdna(redim_iter_4940) : null); }
                    for (a = n1; a < res1 - 1; a++)
                    {
                        Outdna(upperbound + 1 + a - n1).tipo == rob1(a).tipo;
                        Outdna(upperbound + 1 + a - n1).value == rob1(a).value;
                    }
                }
                else
                {
                    List<> Outdna_567_tmp = new List<>();
                    for (int redim_iter_8538 = 0; i < 0; redim_iter_8538++) { Outdna.Add(redim_iter_8538 < Outdna.Count ? Outdna(redim_iter_8538) : null); }
                    for (a = n2; a < res2 - 1; a++)
                    {
                        Outdna(upperbound + 1 + a - n2).tipo == rob2(a).tipo;
                        Outdna(upperbound + 1 + a - n2).value == rob2(a).value;
                    }
                }
            }
            else if (res1 - n1 > 0)
            { //run one side
                if (Int(rndy() * 2) == 0)
                {
                    List<> Outdna_4003_tmp = new List<>();
                    for (int redim_iter_5738 = 0; i < 0; redim_iter_5738++) { Outdna.Add(redim_iter_5738 < Outdna.Count ? Outdna(redim_iter_5738) : null); }
                    for (a = n1; a < res1 - 1; a++)
                    {
                        Outdna(upperbound + 1 + a - n1).tipo == rob1(a).tipo;
                        Outdna(upperbound + 1 + a - n1).value == rob1(a).value;
                    }
                }
            }
            else if (res2 - n2 > 0)
            { //run other side
                if (Int(rndy() * 2) == 0)
                {
                    List<> Outdna_5222_tmp = new List<>();
                    for (int redim_iter_4901 = 0; i < 0; redim_iter_4901++) { Outdna.Add(redim_iter_4901 < Outdna.Count ? Outdna(redim_iter_4901) : null); }
                    for (a = n2; a < res2 - 1; a++)
                    {
                        Outdna(upperbound + 1 + a - n2).tipo == rob2(a).tipo;
                        Outdna(upperbound + 1 + a - n2).value == rob2(a).value;
                    }
                }
            }

            //same search
            bool whatside = false;

            if (i == 0)
            {
                return;
            }
            upperbound = UBound(Outdna);
            nn = res1;
            resn = scanfromn(rob1(), nn, i);
            List<> Outdna_9928_tmp = new List<>();
            for (int redim_iter_7044 = 0; i < 0; redim_iter_7044++) { Outdna.Add(redim_iter_7044 < Outdna.Count ? Outdna(redim_iter_7044) : null); }

            whatside = Int(rndy() * 2) == 0;

            //'''debug
            //Dim debugme As Boolean
            //debugme = False
            //Dim k As String
            //Dim temp As String
            //Dim bp As block
            //Dim converttosysvar As Boolean
            //'''debug

            for (a = nn; a < resn - 1; a++)
            {
                Outdna(upperbound + 1 + a - nn).tipo == IIf(whatside, rob1(a).tipo, rob2(a - nn + res2).tipo); //left hand side or right hand?
                Outdna(upperbound + 1 + a - nn).value == IIf(IIf(rob1(a).tipo == rob2(a - nn + res2).tipo && Abs(rob1(a).value) > 999 && Abs(rob2(a - nn + res2).value) > 999, Int(rndy() * 2) == 0, whatside), rob1(a).value, rob2(a - nn + res2).value); //if typo is different or in var range then all left/right hand, else choose a random side
                                                                                                                                                                                                                                                           //If rob1(a).tipo = rob2(a - nn + res2).tipo And Abs(rob1(a).value) > 999 And Abs(rob2(a - nn + res2).value) > 999 And rob1(a).value <> rob2(a - nn + res2).value Then debugme = True 'debug
            }

            //If debugme Then
            //Dim a2 As Integer
            //Dim a3 As Integer
            //k = ""
            //      For a = nn To resn - 1

            //        If a = UBound(rob1) Then converttosysvar = False Else converttosysvar = IIf(rob1(a + 1).tipo = 7, True, False)
            //        bp.tipo = rob1(a).tipo
            //        bp.value = rob1(a).value
            //        temp = ""
            //        Parse temp, bp, 1, converttosysvar

            //      k = k & temp & vbTab

            //        a2 = a - nn + res2
            //        If a2 = UBound(rob2) Then converttosysvar = False Else converttosysvar = IIf(rob2(a2 + 1).tipo = 7, True, False)
            //        bp.tipo = rob2(a2).tipo
            //        bp.value = rob2(a2).value
            //        temp = ""
            //        Parse temp, bp, 1, converttosysvar

            //      k = k & temp & vbTab

            //        a3 = upperbound + 1 + a - nn
            //        If a3 = UBound(Outdna) Then converttosysvar = False Else converttosysvar = IIf(Outdna(a3 + 1).tipo = 7, True, False)
            //        bp.tipo = Outdna(a3).tipo
            //        bp.value = Outdna(a3).value
            //        temp = ""
            //        Parse temp, bp, 1, converttosysvar

            //      k = k & temp & vbCrLf

            //      Next

            //      MsgBox k
            //End If
        }
}

    private static void DeleteSpecies(int i_UNUSED)
    {
        int X = 0;

        for (X = i; X < SimOpts.SpeciesNum - 1; X++)
        {
            SimOpts.Specie(X) = SimOpts.Specie(X + 1);
        }
        SimOpts.Specie(SimOpts.SpeciesNum - 1).Native == false; // Do this just in case
        SimOpts.SpeciesNum = SimOpts.SpeciesNum - 1;
    }

    private static void feedbody(int t)
    {
        if (rob(t).mem(fdbody) > 100)
        {
            rob(t).mem(fdbody) = 100;
        }
        rob(t).nrg = rob(t).nrg + rob(t).mem(fdbody);
        rob(t).body = rob(t).body - CSng(rob(t).mem(fdbody)) / 10;
        if (rob(t).nrg > 32000)
        {
            rob(t).nrg = 32000;
        }
        rob(t).radius = FindRadius(t);
        rob(t).mem(fdbody) = 0;
    }

    private static void FireTies(int n)
    {
        decimal length = 0;
        decimal maxLength = 0;

        bool resetlastopp = false;//Botsareus 8/26/2012 only if lastopp is zero, this will reset it back to zero

        dynamic _WithVar_3310;
        _WithVar_3310 = rob[n];

        if (_WithVar_3310.lastopp == 0 & (_WithVar_3310.age < 2) && _WithVar_3310.parent <= UBound(rob))
        { //Botsareus 8/31/2012 new way to calculate lastopp overwrite: blind ties to parent
            if (rob(_WithVar_3310.parent).exist)
            {
                _WithVar_3310.lastopp = _WithVar_3310.parent;
                resetlastopp = true;
            }
        }

        //Botsareus 11/26/2013 The tie by touch code
        if (_WithVar_3310.lastopp == 0 & _WithVar_3310.lasttch != 0 & _WithVar_3310.lasttch <= UBound(rob))
        {
            if (rob(_WithVar_3310.lasttch).exist)
            {
                _WithVar_3310.lastopp = _WithVar_3310.lasttch;
                resetlastopp = true;
            }
        }

        if (_WithVar_3310.mem(mtie) != 0)
        {
            if (_WithVar_3310.lastopp > 0 & !SimOpts.DisableTies && (_WithVar_3310.lastopptype == 0))
            {
                //2 robot lengths
                length = VectorMagnitude(VectorSub(rob(_WithVar_3310.lastopp).pos, _WithVar_3310.pos));
                maxLength = RobSize * 4 + rob[n].radius + rob(rob[n].lastopp).radius;

                if (length <= maxLength)
                {
                    //maketie auto deletes existing ties for you
                    maketie(n, rob[n].lastopp, rob[n].radius + rob(rob[n].lastopp).radius + RobSize * 2, -20, rob[n].mem(mtie));
                    //Botsareus 3/14/2014 Disqualify
                    if ((SimOpts.F1 || x_restartmode == 1) && Disqualify == 2)
                    {
                        dreason(rob[n].FName, rob[n].tag, "making a tie");
                    }
                    if (!SimOpts.F1 && rob[n].dq == 1 && Disqualify == 2)
                    {
                        rob[n].Dead = true; //safe kill robot
                    }
                }
            }
            _WithVar_3310.mem(mtie) = 0;
        }

        if (resetlastopp)
        {
            _WithVar_3310.lastopp = 0; //Botsareus 8/26/2012 reset lastopp to zero
        }
    }

    private static void HandleWaste(int n)
    {
        if (rob[n].Waste > 0 & rob[n].chloroplasts > 0)
        {
            feedveg2(n); //Botsareus 8/25/2013 Mod to effect all robots
        }
        if (SimOpts.BadWastelevel == 0)
        {
            SimOpts.BadWastelevel = 400;
        }
        if (SimOpts.BadWastelevel > 0 & rob[n].Pwaste + rob[n].Waste > SimOpts.BadWastelevel)
        {
            altzheimer(n);
        }
        if (rob[n].Waste > 32000)
        {
            defacate(n);
        }
        if (rob[n].Pwaste > 32000)
        {
            rob[n].Pwaste = 32000;
        }
        if (rob[n].Waste < 0)
        {
            rob[n].Waste = 0;
        }
        rob[n].mem(828) = rob[n].Waste;
        rob[n].mem(829) = rob[n].Pwaste;
    }

    private static int iceil(decimal X)
    {
        int iceil = 0;
        if ((Abs(X) > 32000))
        {
            X = Sgn(X) * 32000;
        }
        iceil = X;
        return iceil;
    }

    private static void makeshell(int n)
    {
        decimal oldshell = 0;

        decimal Cost = 0;

        decimal Delta = 0;

        decimal shellNrgConvRate = 0;

        shellNrgConvRate = 0.1m; // Make 10 shell for 1 nrg

        dynamic _WithVar_5508;
        _WithVar_5508 = rob[n];
        if (_WithVar_5508.nrg <= 0)
        {
            goto getout; // Can't make or unmake shell if nrg is negative
        }
        oldshell = _WithVar_5508.shell;

        if (_WithVar_5508.mem(822) > 32000)
        {
            _WithVar_5508.mem(822) = 32000;
        }
        if (_WithVar_5508.mem(822) < -32000)
        {
            _WithVar_5508.mem(822) = -32000;
        }

        Delta = _WithVar_5508.mem(822); // This is what the bot wants to do to his shell, up or down

        if (Abs(Delta) > _WithVar_5508.nrg / shellNrgConvRate)
        {
            Delta = Sgn(Delta) * _WithVar_5508.nrg / shellNrgConvRate; // Can't make or unmake more shell than you have nrg
        }

        if (Abs(Delta) > 100)
        {
            Delta = Sgn(Delta) * 100; // Can't make or unmake more than 100 shell at a time
        }
        if (_WithVar_5508.shell + Delta > 32000)
        {
            Delta = 32000 - _WithVar_5508.shell; // shell can't go above 32000
        }
        if (_WithVar_5508.shell + Delta < 0)
        {
            Delta = -.shell; // shell can't go below 0
        }

        _WithVar_5508.shell = _WithVar_5508.shell + Delta; // Make the change in shell

        _WithVar_5508.nrg = _WithVar_5508.nrg - (Abs(Delta) * shellNrgConvRate); // Making or unmaking shell takes nrg

        //This is the transaction cost
        Cost = Abs(Delta) * SimOpts.Costs(SHELLCOST) * SimOpts.Costs(COSTMULTIPLIER);

        if (_WithVar_5508.Multibot)
        {
            _WithVar_5508.nrg = _WithVar_5508.nrg - Cost / (IIf(_WithVar_5508.numties < 0, 0, _WithVar_5508.numties) + 1); //lower cost for multibot
        }
        else
        {
            _WithVar_5508.nrg = _WithVar_5508.nrg - Cost;
        }

        _WithVar_5508.Waste = _WithVar_5508.Waste + Cost; // waste is created proportional to the transaction cost

        _WithVar_5508.mem(822) = 0; // reset the .mkshell sysvar
        _WithVar_5508.mem(823) = CInt(_WithVar_5508.shell); // update the .shell sysvar
    getout:
        //Botsareus 3/14/2014 Disqualify
        if ((SimOpts.F1 || x_restartmode == 1) && Disqualify == 2)
        {
            dreason(_WithVar_5508.FName, _WithVar_5508.tag, "making shell");
        }
        if (!SimOpts.F1 && _WithVar_5508.dq == 1 && Disqualify == 2)
        {
            rob[n].Dead = true; //safe kill robot
        }
    }

    private static void makeslime(int n)
    {
        decimal oldslime = 0;

        decimal Cost = 0;

        decimal Delta = 0;

        decimal slimeNrgConvRate = 0;

        slimeNrgConvRate = 0.1m; // Make 10 slime for 1 nrg

        dynamic _WithVar_9435;
        _WithVar_9435 = rob[n];
        if (_WithVar_9435.nrg <= 0)
        {
            goto getout; // Can't make or unmake slime if nrg is negative
        }
        oldslime = _WithVar_9435.Slime;

        if (_WithVar_9435.mem(820) > 32000)
        {
            _WithVar_9435.mem(820) = 32000;
        }
        if (_WithVar_9435.mem(820) < -32000)
        {
            _WithVar_9435.mem(820) = -32000;
        }

        Delta = _WithVar_9435.mem(820); // This is what the bot wants to do to his slime, up or down

        if (Abs(Delta) > _WithVar_9435.nrg / slimeNrgConvRate)
        {
            Delta = Sgn(Delta) * _WithVar_9435.nrg / slimeNrgConvRate; // Can't make or unmake more slime than you have nrg
        }

        if (Abs(Delta) > 200)
        {
            Delta = Sgn(Delta) * 200; //Botsareus 6/23/2016 Can't make or unmake more than 200 slime at a time
        }
        if (_WithVar_9435.Slime + Delta > 32000)
        {
            Delta = 32000 - _WithVar_9435.Slime; // Slime can't go above 32000
        }
        if (_WithVar_9435.Slime + Delta < 0)
        {
            Delta = -.Slime; // Slime can't go below 0
        }

        _WithVar_9435.Slime = _WithVar_9435.Slime + Delta; // Make the change in slime

        _WithVar_9435.nrg = _WithVar_9435.nrg - (Abs(Delta) * slimeNrgConvRate); // Making or unmaking slime takes nrg

        //This is the transaction cost
        Cost = Abs(Delta) * SimOpts.Costs(SLIMECOST) * SimOpts.Costs(COSTMULTIPLIER);

        if (_WithVar_9435.Multibot)
        {
            _WithVar_9435.nrg = _WithVar_9435.nrg - Cost / (IIf(_WithVar_9435.numties < 0, 0, _WithVar_9435.numties) + 1); //lower cost for multibot
        }
        else
        {
            _WithVar_9435.nrg = _WithVar_9435.nrg - Cost;
        }

        _WithVar_9435.Waste = _WithVar_9435.Waste + Cost; // waste is created proportional to the transaction cost

        _WithVar_9435.mem(820) = 0; // reset the .mkslime sysvar
        _WithVar_9435.mem(821) = CInt(_WithVar_9435.Slime); // update the .slime sysvar

    getout:
        //Botsareus 3/14/2014 Disqualify
        if (!.Veg)
        {
            if ((SimOpts.F1 || x_restartmode == 1) && Disqualify == 2)
            {
                dreason(_WithVar_9435.FName, _WithVar_9435.tag, "making slime");
            }
            if (!SimOpts.F1 && _WithVar_9435.dq == 1 && Disqualify == 2)
            {
                rob[n].Dead = true; //safe kill robot
            }
        }
    }

    private static void MakeStuff(int n)
    {
        if (rob[n].mem(824) != 0)
        {
            storevenom(n);
        }
        if (rob[n].mem(826) != 0)
        {
            storepoison(n);
        }
        if (rob[n].mem(822) != 0)
        {
            makeshell(n);
        }
        if (rob[n].mem(820) != 0)
        {
            makeslime(n);
        }
    }

    private static void ManageBody(int n)
    {
        //body management
        //rob[n].obody = rob[n].body      'replaces routine above 'Botsareus 7/4/2016 Bug fix -bodgain and bodloss work now

        if (rob[n].mem(strbody) > 0)
        {
            storebody(n);
        }
        if (rob[n].mem(fdbody) > 0)
        {
            feedbody(n);
        }

        if (rob[n].body > 32000)
        {
            rob[n].body = 32000;
        }
        if (rob[n].body < 0)
        {
            rob[n].body = 0; //Ericl 4/6/2006 Overflow protection.
        }
        rob[n].mem(body) = CInt(rob[n].body);
    }

    private static void ManageBouyancy(int n)
    { //Botsareus 2/2/2013 Bouyancy fix 'Botsareus 11/23/2013 More mods, more old school now
        dynamic _WithVar_2183;
        _WithVar_2183 = rob[n];
        if (_WithVar_2183.mem(setboy) != 0)
        {
            _WithVar_2183.Bouyancy = _WithVar_2183.Bouyancy + _WithVar_2183.mem(setboy) / 32000;
            if (_WithVar_2183.Bouyancy < 0)
            {
                _WithVar_2183.Bouyancy = 0;
            }
            if (_WithVar_2183.Bouyancy > 1)
            {
                _WithVar_2183.Bouyancy = 1;
            }
            _WithVar_2183.mem(rdboy) = _WithVar_2183.Bouyancy * 32000;
            _WithVar_2183.mem(setboy) = 0;
        }
    }

    private static void ManageChlr(int n)
    { //Panda 8/15/2013 The new chloroplast function
        dynamic _WithVar_4357;
        _WithVar_4357 = rob[n];

        if (_WithVar_4357.mem(mkchlr) > 0 || _WithVar_4357.mem(rmchlr) > 0)
        {
            ChangeChlr(n);
        }

        _WithVar_4357.chloroplasts = _WithVar_4357.chloroplasts - 0.5m / (100 ^ (_WithVar_4357.chloroplasts / 16000)); //Redo from Botsareus robots with less chloroplasts lose chloroplasts much faster

        if (_WithVar_4357.chloroplasts > 32000)
        {
            _WithVar_4357.chloroplasts = 32000;
        }
        if (_WithVar_4357.chloroplasts < 0)
        {
            _WithVar_4357.chloroplasts = 0; //Panda 9/5/2013 Bug fix
        }

        _WithVar_4357.mem(chlr) = _WithVar_4357.chloroplasts;

        _WithVar_4357.mem(light) = 32000 - (LightAval * 32000); //Botsareus 8/24/2013 Tells the robot how much light is aval. (I want this here because it is chloroplast related)

        _WithVar_4357.radius = FindRadius(n);
    }

    private static void ManageDeath(int n)
    {
        int i = 0;

        dynamic _WithVar_2608;
        _WithVar_2608 = rob[n];

        //We kill bots with nrg of body less than 0.5 rather than 0 to avoid rounding issues with refvars and such
        //showing extant bots with nrg or body of 0.

        if (SimOpts.CorpseEnabled)
        {
            if (!.Corpse)
            {
                if ((_WithVar_2608.nrg < 15) && _WithVar_2608.age > 0)
                { //Botsareus 1/5/2013 Corpse forms more often
                    _WithVar_2608.Corpse = true;
                    _WithVar_2608.FName = "Corpse";
                    //  delallties n
                    Erase(_WithVar_2608.occurr);
                    _WithVar_2608.color = vbWhite;
                    _WithVar_2608.Veg = false;
                    _WithVar_2608.Fixed = false;
                    _WithVar_2608.nrg = 0;
                    _WithVar_2608.DisableDNA = true;
                    _WithVar_2608.DisableMovementSysvars = true;
                    _WithVar_2608.CantSee = true;
                    _WithVar_2608.VirusImmune = true;
                    _WithVar_2608.chloroplasts = 0; //Botsareus 11/10/2013 Reset chloroplasts for corpse

                    //Zero the eyes
                    for (i = (EyeStart + 1); i < (EyeEnd - 1); i++)
                    {
                        _WithVar_2608.mem(i) = 0;
                    }
                    _WithVar_2608.Bouyancy = 0; //Botsareus 2/2/2013 dead robot no bouy.
                }
            }
            if (_WithVar_2608.body < 0.5m)
            {
                _WithVar_2608.Dead = true; //Botsareus 6/9/2013 Small bug fix to kill robots with zero body
            }
        }
        else if ((_WithVar_2608.nrg < 0.5 || _WithVar_2608.body < 0.5) Then _WithVar_2608.Dead == true) {
        }

        if (_WithVar_2608.Dead)
        {
            kil(kl) = n;
            kl = kl + 1;
        }
    }

    private static void ManageFixed(int n)
    {
        //Fixed/ not fixed
        if (rob[n].mem(216) > 0)
        {
            rob[n].Fixed = true;
        }
        else
        {
            rob[n].Fixed = false;
        }
    }

    private static void ManageReproduction(int n)
    {
        //Decrement the fertilization counter
        if (rob[n].fertilized >= 0)
        { // This is >= 0 so that we decrement it to -1 the cycle after the last birth is possible
            rob[n].fertilized = rob[n].fertilized - 1;
            if (rob[n].fertilized >= 0)
            {
                rob[n].mem(SYSFERTILIZED) = rob[n].fertilized;
            }
            else
            {
                rob[n].mem(SYSFERTILIZED) = 0;
            }
        }
        else
        {
            //new code here to block sex repro
            if (rob[n].fertilized < -10)
            {
                rob[n].fertilized = rob[n].fertilized + 1;
            }
            else
            {
                if (rob[n].fertilized == -1)
                { // Safe now to delete the spermDNA
                    List<> rob_164_tmp = new List<>();
                    for (int redim_iter_4462 = 0; i < 0; redim_iter_4462++) { rob.Add(null); }
                    rob[n].spermDNAlen = 0;
                }
                rob[n].fertilized = -2; //This is so we don't keep reDiming every time through
            }
        }

        //Asexual reproduction
        if ((rob[n].mem(Repro) > 0 || rob[n].mem(mrepro) > 0) && !rob[n].CantReproduce)
        {
            rep(rp) = n; // positive value indicates asexual reproduction
            rp = rp + 1;
        }

        //Sexual Reproduction
        if (rob[n].mem(SEXREPRO) > 0 & rob[n].fertilized >= 0 & !rob[n].CantReproduce)
        {
            rep(rp) = -n; //negative value indicates sexual reproduction
            rp = rp + 1;
        }
    }

    private static void Poisons(int n)
    {
        dynamic _WithVar_6568;
        _WithVar_6568 = rob[n];
        //Paralyzed means venomized

        if (_WithVar_6568.Paralyzed)
        {
            _WithVar_6568.mem(_WithVar_6568.Vloc) = _WithVar_6568.Vval;
        }

        if (_WithVar_6568.Paralyzed)
        {
            _WithVar_6568.Paracount = _WithVar_6568.Paracount - 1;
            if (_WithVar_6568.Paracount < 1)
            {
                _WithVar_6568.Paralyzed = false;
                _WithVar_6568.Vloc = 0;
                _WithVar_6568.Vval = 0;
            }
        }

        _WithVar_6568.mem(837) = Int(_WithVar_6568.Paracount); //Botsareus 7/13/2016 Bug fix

        if (_WithVar_6568.Poisoned)
        {
            _WithVar_6568.mem(_WithVar_6568.Ploc) = _WithVar_6568.Pval;
        }

        if (_WithVar_6568.Poisoned)
        {
            _WithVar_6568.Poisoncount = _WithVar_6568.Poisoncount - 1;
            if (_WithVar_6568.Poisoncount < 1)
            {
                _WithVar_6568.Poisoned = false;
                _WithVar_6568.Ploc = 0;
                _WithVar_6568.Pval = 0;
            }
        }

        _WithVar_6568.mem(838) = Int(_WithVar_6568.Poisoncount); //Botsareus 7/13/2016 Bug fix
    }

    private static void RemoveExtinctSpecies()
    {
        dynamic i = null;
        int j = 0;

        i = 0;
        While(i < SimOpts.SpeciesNum);
        if (SimOpts.Specie(i).population == 0 & !SimOpts.Specie(i).Native)
        {
            DeleteSpecies((i));
            // Don't increment i since we need to recheck the i postion after deleting the species
        }
        else
        {
            i = i + 1;
        }
        Wend();
    }

    private static void ReproduceAndKill()
    {
        int t = 0;

        int temp = 0;

        int temp2 = 0;

        t = 1;
        While(t < rp);
        if (rep(t) > 0)
        {
            if (rob(rep(t)).mem(mrepro) > 0 & rob(rep(t)).mem(Repro) > 0)
            {
                if (rndy() > 0.5m)
                {
                    temp = rob(rep(t)).mem(Repro);
                }
                else
                {
                    temp = rob(rep(t)).mem(mrepro);
                }
            }
            else
            {
                if (rob(rep(t)).mem(mrepro) > 0)
                {
                    temp = rob(rep(t)).mem(mrepro);
                }
                if (rob(rep(t)).mem(Repro) > 0)
                {
                    temp = rob(rep(t)).mem(Repro);
                }
            }
            temp2 = rep(t);
            Reproduce(temp2, temp);
        }
        else if (rep(t) < 0)
        {
            // negative values in the rep array indicate sexual reproduction
            SexReproduce(-rep(t));
            // rob(-rep(t)).fertilized = 0 ' sperm shots only work for one birth for now
            // rob(-rep(t)).mem(SYSFERTILIZED) = 0
        }

        t = t + 1;
        Wend();
        t = 1;

        //kill robots
        While(t < kl);
        KillRobot(kil(t));
        t = t + 1;
        Wend();
    }

    private static void robshoot(int n)
    {
        int shtype = 0;

        decimal value = 0;

        decimal multiplier = 0;

        decimal Cost = 0;

        decimal rngmultiplier = 0;

        bool valmode = false;

        decimal EnergyLost = 0;

        if (rob[n].nrg <= 0)
        {
            goto CantShoot;
        }

        shtype = rob[n].mem(shoot);
        value = rob[n].mem(shootval);

        if (shtype == -1 || shtype == -6)
        { //Botsareus 6/20/2016 Only for nrg/body feed shots
            //Negative value for .shootval
            if (value < 0)
            { // negative values of .shootval impact shot range?
                multiplier = 1; // no impact on shot power
                rngmultiplier = -value; // set the range multplier equal to .shootval
            }

            if (value > 0)
            { // postive values of .shootval impact shot power?
                multiplier = value;
                rngmultiplier = 1;
                valmode = true;
            }
            if (value == 0)
            {
                multiplier = 1;
                rngmultiplier = 1;
            }

            if (rngmultiplier > 4)
            {
                Cost = rngmultiplier * SimOpts.Costs(SHOTCOST) * SimOpts.Costs(COSTMULTIPLIER);
                rngmultiplier = Log(rngmultiplier / 2) / Log(2);
            }
            else if (valmode == false)
            {
                rngmultiplier = 1;
                Cost = (SimOpts.Costs(SHOTCOST) * SimOpts.Costs(COSTMULTIPLIER) / (IIf(rob[n].numties < 0, 0, rob[n].numties) + 1));
            }

            if (multiplier > 4)
            {
                Cost = multiplier * SimOpts.Costs(SHOTCOST) * SimOpts.Costs(COSTMULTIPLIER);
                multiplier = Log(multiplier / 2) / Log(2);
            }
            else if (valmode == true)
            {
                multiplier = 1;
                Cost = (SimOpts.Costs(SHOTCOST) * SimOpts.Costs(COSTMULTIPLIER) / (IIf(rob[n].numties < 0, 0, rob[n].numties) + 1)); //Botsareus 6/12/2014 Bug fix
            }

            if (Cost > rob[n].nrg && Cost > 2 && rob[n].nrg > 2 && valmode)
            {
                Cost = rob[n].nrg;
                multiplier = Log(rob[n].nrg / (SimOpts.Costs(SHOTCOST) * SimOpts.Costs(COSTMULTIPLIER))) / Log(2);
            }

            if (Cost > rob[n].nrg && Cost > 2 && rob[n].nrg > 2 && !valmode)
            {
                Cost = rob[n].nrg;
                rngmultiplier = Log(rob[n].nrg / (SimOpts.Costs(SHOTCOST) * SimOpts.Costs(COSTMULTIPLIER))) / Log(2);
            }
        }

        //'''''''''''''''''''''''''''''''''''''''''''''
        //'''''''''''''''''''''''''''''''''''''''''''''
        //'''''''''''''''''''''''''''''''''''''''''''''

        switch (shtype)
        {
            // TODO: Cannot convert case: Is >= 0// Memory Shot
            case 0:
                shtype = shtype % MaxMem;
                Cost = SimOpts.Costs(SHOTCOST) * SimOpts.Costs(COSTMULTIPLIER);
                if (rob[n].nrg < Cost)
                {
                    Cost = rob[n].nrg;
                }
                rob[n].nrg = rob[n].nrg - Cost; // EricL - postive shots should cost the shotcost
                newshot(n, shtype, value, 1, true);
                //Botsareus 3/14/2014 Disqualify
                if ((SimOpts.F1 || x_restartmode == 1) && Disqualify == 2)
                {
                    dreason(rob[n].FName, rob[n].tag, "firing an info shot");
                }
                if (!SimOpts.F1 && rob[n].dq == 1 && Disqualify == 2)
                {
                    rob[n].Dead = true; //safe kill robot
                }
                break;// Nrg request Feeding Shot
            case -1:
                if (rob[n].Multibot)
                {
                    value = 20 + (rob[n].body / 5) * (IIf(rob[n].numties < 0, 0, rob[n].numties) + 1); //Botsareus 6/22/2016 Bugfix
                }
                else
                {
                    value = 20 + (rob[n].body / 5);
                }
                value = value * multiplier;
                if (rob[n].nrg < Cost)
                {
                    Cost = rob[n].nrg;
                }
                rob[n].nrg = rob[n].nrg - Cost;
                newshot(n, shtype, value, rngmultiplier, true);
                break;// Nrg shot
            case -2:
                value = Abs(value);
                if (rob[n].nrg < value)
                {
                    value = rob[n].nrg;
                }
                if (value == 0)
                {
                    value = rob[n].nrg / 100; //default energy shot.  Very small.
                }
                EnergyLost = value + SimOpts.Costs(SHOTCOST) * SimOpts.Costs(COSTMULTIPLIER) / (IIf(rob[n].numties < 0, 0, rob[n].numties) + 1);
                if (EnergyLost > rob[n].nrg)
                {
                    rob[n].nrg = 0;
                }
                else
                {
                    rob[n].nrg = rob[n].nrg - EnergyLost;
                }
                newshot(n, shtype, value, 1, true);
                break;//shoot venom
            case -3:
                value = Abs(value);
                if (value > rob[n].venom)
                {
                    value = rob[n].venom;
                }
                if (value == 0)
                {
                    value = rob[n].venom / 20; //default venom shot.  Not too small.
                }
                rob[n].venom = rob[n].venom - value;
                rob[n].mem(825) = rob[n].venom;
                EnergyLost = SimOpts.Costs(SHOTCOST) * SimOpts.Costs(COSTMULTIPLIER) / (IIf(rob[n].numties < 0, 0, rob[n].numties) + 1);
                if (EnergyLost > rob[n].nrg)
                {
                    //  EnergyLostPerCycle = EnergyLostPerCycle - rob[n].nrg
                    rob[n].nrg = 0;
                }
                else
                {
                    rob[n].nrg = rob[n].nrg - EnergyLost;
                    // EnergyLostPerCycle = EnergyLostPerCycle - EnergyLost
                }
                newshot(n, shtype, value, 1, true);
                break;//shoot waste 'Botsareus 4/22/2016 Bugfix
            case -4:
                value = Abs(value);
                if (value == 0)
                {
                    value = rob[n].Waste / 20; //default waste shot. 'Botsareus 10/5/2015 Fix for waste
                }
                if (value > rob[n].Waste)
                {
                    value = rob[n].Waste;
                }
                rob[n].Waste = rob[n].Waste - value * 0.99m; //Botsareus 10/5/2015 Fix for waste
                rob[n].Pwaste = rob[n].Pwaste + value / 100;
                EnergyLost = SimOpts.Costs(SHOTCOST) * SimOpts.Costs(COSTMULTIPLIER) / (IIf(rob[n].numties < 0, 0, rob[n].numties) + 1);
                if (EnergyLost > rob[n].nrg)
                {
                    // EnergyLostPerCycle = EnergyLostPerCycle - rob[n].nrg
                    rob[n].nrg = 0;
                }
                else
                {
                    rob[n].nrg = rob[n].nrg - EnergyLost;
                }
                newshot(n, shtype, value, 1, true);
                // no -5 shot here as poison can only be shot in response to an attack
                break;//shoot body
            case -6:
                if (rob[n].Multibot)
                {
                    value = 10 + (rob[n].body / 2) * (IIf(rob[n].numties < 0, 0, rob[n].numties) + 1);
                }
                else
                {
                    value = 10 + Abs(rob[n].body) / 2;
                }
                if (rob[n].nrg < Cost)
                {
                    Cost = rob[n].nrg;
                }
                rob[n].nrg = rob[n].nrg - Cost;
                value = value * multiplier;
                newshot(n, shtype, value, rngmultiplier, true);
                break;// shoot sperm
            case -8:
                Cost = SimOpts.Costs(SHOTCOST) * SimOpts.Costs(COSTMULTIPLIER);
                if (rob[n].nrg < Cost)
                {
                    Cost = rob[n].nrg;
                }
                rob[n].nrg = rob[n].nrg - Cost; // EricL - postive shots should cost the shotcost
                newshot(n, shtype, value, 1, true);
                break;
        }
    CantShoot:
        rob[n].mem(shoot) = 0;
        rob[n].mem(shootval) = 0;
    }

    private static decimal SetAimFunc(int t)
    {//Botsareus 6/29/2013 Turn costs and ma more accurate
        decimal SetAimFunc = 0;
        decimal diff = 0;

        decimal diff2 = 0;

        dynamic _WithVar_2082;
        _WithVar_2082 = rob(t);

        diff = CSng(_WithVar_2082.mem(aimsx)) - CSng(_WithVar_2082.mem(aimdx));

        if (_WithVar_2082.mem(SetAim) == Round(_WithVar_2082.aim * 200, 0))
        {
            //Setaim is the same as .aim so nothing set into .setaim this cycle
            SetAimFunc = (_WithVar_2082.aim * 200 + diff);
        }
        else
        {
            // .setaim overrides .aimsx and .aimdx
            SetAimFunc = _WithVar_2082.mem(SetAim); // this is where .aim needs to be
            diff = -AngDiff(_WithVar_2082.aim, angnorm(CSng(_WithVar_2082.mem(SetAim) / 200))) * 200; // this is the diff to get there 'Botsareus 6/18/2016 Added angnorm
            diff2 = Abs(Round((_WithVar_2082.aim * 200 - _WithVar_2082.mem(SetAim)) / 1256, 0) * 1256) * Sgn(diff); // this is how much we add to momentum
        }

        //diff + diff2 is now the amount, positive or negative to turn.
        _WithVar_2082.nrg = _WithVar_2082.nrg - Abs((Round((diff + diff2) / 200, 3) * SimOpts.Costs(TURNCOST) * SimOpts.Costs(COSTMULTIPLIER)));

        SetAimFunc = SetAimFunc % (1256);
        if (SetAimFunc < 0)
        {
            SetAimFunc = SetAimFunc + 1256;
        }
        SetAimFunc = SetAimFunc / 200;

        //Overflow Protection
        While(_WithVar_2082.ma > 2 * PI);
        _WithVar_2082.ma = _WithVar_2082.ma - 2 * PI;
        Wend();
        While(_WithVar_2082.ma < -2 * PI);
        _WithVar_2082.ma = _WithVar_2082.ma + 2 * PI;
        Wend();

        _WithVar_2082.aim = SetAimFunc + _WithVar_2082.ma; // Add in the angular momentum

        //Voluntary rotation can reduce angular momentum but does not add to it.

        if (_WithVar_2082.ma > 0 & diff < 0)
        {
            _WithVar_2082.ma = _WithVar_2082.ma + (diff + diff2) / 200;
            if (_WithVar_2082.ma < 0)
            {
                _WithVar_2082.ma = 0;
            }
        }
        if (_WithVar_2082.ma < 0 & diff > 0)
        {
            _WithVar_2082.ma = _WithVar_2082.ma + (diff + diff2) / 200;
            if (_WithVar_2082.ma > 0)
            {
                _WithVar_2082.ma = 0;
            }
        }

        _WithVar_2082.aimvector = VectorSet(Cos(_WithVar_2082.aim), Sin(_WithVar_2082.aim));

        _WithVar_2082.mem(aimsx) = 0;
        _WithVar_2082.mem(aimdx) = 0;
        _WithVar_2082.mem(AimSys) = CInt(_WithVar_2082.aim * 200);
        _WithVar_2082.mem(SetAim) = _WithVar_2082.mem(AimSys);
        return SetAimFunc;
    }

    private static void Shock(int n)
    {
        //This code here forces a robot to die instantly from getting an overload based on energy

        if (!rob[n].Veg && rob[n].nrg > 3000)
        {
            decimal temp = 0;

            temp = rob[n].onrg - rob[n].nrg;
            if (temp > (rob[n].onrg / 2))
            {
                rob[n].nrg = 0;
                rob[n].body = rob[n].body + (rob[n].nrg / 10);
                if (rob[n].body > 32000)
                {
                    rob[n].body = 32000;
                }
                rob[n].radius = FindRadius(n);
            }
        }
    }

    private static void Shooting(int n)
    {
        //shooting
        if (rob[n].mem(shoot))
        {
            robshoot(n);
        }
        rob[n].mem(shoot) = 0;
    }

    private static void simplematch(dynamic r1(_UNUSED) {
        int patch = 0;//Botsareus 4/18/2016 Temporary fix to prevent infinate loop

        bool newmatch = false;

        int inc = 0;

        int ei1 = 0;

        int ei2 = 0;

        ei1 = UBound(r1);
        ei2 = UBound(r2);

        //the list of variables in r1
        List<int> matchlist1 = new List<int> { }; // TODO - Specified Minimum Array Boundary Not Supported: Dim matchlist1() As Integer

        List<int> matchlist1_6077_tmp = new List<int>();
        for (int redim_iter_375 = 0; i < 0; redim_iter_375++) { matchlist1.Add(0); }

        //the list of variables in r2
        List<int> matchlist2 = new List<int> { }; // TODO - Specified Minimum Array Boundary Not Supported: Dim matchlist2() As Integer

        List<int> matchlist2_2791_tmp = new List<int>();
        for (int redim_iter_5543 = 0; i < 0; redim_iter_5543++) { matchlist2.Add(0); }

        int count = 0;

        count = 0;

        //add data to match list until letters match to each other on opposite sides
        int loopr1 = 0;

        int loopr2 = 0;

        int loopold = 0;

        int laststartmatch1 = 0;

        int laststartmatch2 = 0;

        loopr1 = 0;
        loopr2 = 0;
        laststartmatch1 = 0;
        laststartmatch2 = 0;

        do
        {
            //keep building until both sides max out
            if (loopr1 > ei1)
            {
                loopr1 = ei1;
            }
            if (loopr2 > ei2)
            {
                loopr2 = ei2;
            }

            matchlist1[count] = r1(loopr1).nucli;
            matchlist2[count] = r2(loopr2).nucli;

            count = count + 1;
            List<int> matchlist1_3387_tmp = new List<int>();
            for (int redim_iter_6738 = 0; i < 0; redim_iter_6738++) { matchlist1.Add(redim_iter_6738 < matchlist1.Count ? matchlist1(redim_iter_6738) : 0); }
            List<int> matchlist2_4028_tmp = new List<int>();
            for (int redim_iter_4816 = 0; i < 0; redim_iter_4816++) { matchlist2.Add(redim_iter_4816 < matchlist2.Count ? matchlist2(redim_iter_4816) : 0); }

            //does anything match
            bool match = false;

            bool matchr2 = false;

            match = false;

            for (loopold = 0; loopold < count - 1; loopold++)
            {
                if (r2(loopr2).nucli == matchlist1[loopold])
                {
                    matchr2 = true;
                    match = true;
                    break;
                }
                if (r1(loopr1).nucli == matchlist2[loopold])
                {
                    matchr2 = false;
                    match = true;
                    break;
                }
                patch = patch + 1;
            }

            if (match)
            {
                if (matchr2)
                {
                    loopr1 = loopold + laststartmatch1;
                }
                else
                {
                    loopr2 = loopold + laststartmatch2;
                }

                //start matching

                do
                {
                    if (r2(loopr2).nucli == r1(loopr1).nucli)
                    {
                        //increment only in newmatch
                        if (newmatch == false)
                        {
                            inc = inc + 1;
                        }
                        newmatch = true;
                        r1(loopr1).match = inc;
                        r2(loopr2).match = inc;
                    }
                    else
                    {
                        newmatch = false;
                        //no more match
                        laststartmatch1 = loopr1;
                        laststartmatch2 = loopr2;
                        loopr1 = loopr1 - 1;
                        loopr2 = loopr2 - 1;
                        break;
                    }
                    loopr1 = loopr1 + 1;
                    loopr2 = loopr2 + 1;
                    patch = patch + 1;
                } while (!(loopr1 > ei1 || loopr2 > ei2);

                //reset match list so it will not get too long
                List<int> matchlist1_2614_tmp = new List<int>();
                for (int redim_iter_331 = 0; i < 0; redim_iter_331++) { matchlist1.Add(0); }
                List<int> matchlist2_9837_tmp = new List<int>();
                for (int redim_iter_8662 = 0; i < 0; redim_iter_8662++) { matchlist2.Add(0); }
                count = 0;
            }

            loopr1 = loopr1 + 1;
            loopr2 = loopr2 + 1;
            patch = patch + 1;
        } while (!((loopr1 > ei1 && loopr2 > ei2) || patch > (16000 ^ 2));
    }

    private static void storebody(int t)
    {
        if (rob(t).mem(strbody) > 100)
        {
            rob(t).mem(strbody) = 100;
        }
        rob(t).nrg = rob(t).nrg - rob(t).mem(strbody);
        rob(t).body = rob(t).body + rob(t).mem(strbody) / 10;
        if (rob(t).body > 32000)
        {
            rob(t).body = 32000;
        }
        rob(t).radius = FindRadius(t);
        rob(t).mem(strbody) = 0;
    }

    private static void UpdateCounters(int n)
    {
        int i = 0;

        i = 0;

        TotalRobots = TotalRobots + 1;

        //Update the number of bots in each species
        While(SimOpts.Specie(i).Name != rob[n].FName && i < SimOpts.SpeciesNum);
        i = i + 1;
        Wend();

        //If no species structure for the bot, then create one
        if (!rob[n].Corpse)
        {
            if (i == SimOpts.SpeciesNum && SimOpts.SpeciesNum < MAXNATIVESPECIES)
            {
                AddSpecie(n, false);
            }
            else if (SimOpts.SpeciesNum < MAXNATIVESPECIES)
            {
                SimOpts.Specie(i).population = SimOpts.Specie(i).population + 1;
            }
        }
        //Overflow protection.  Need to make sure teleported in species grow the species array correctly.
        if (SimOpts.Specie(i).population > 32000)
        {
            SimOpts.Specie(i).population = 32000;
        }

        if (rob[n].Veg)
        {
            totvegs = totvegs + 1;
        }
        else if (rob[n].Corpse)
        {
            totcorpse = totcorpse + 1;
            if (rob[n].body > 0)
            {
                Decay(n);
            }
            else
            {
                KillRobot(n);
            }
        }
        else
        {
            totnvegs = totnvegs + 1;
        }
    }

    private static void Upkeep(int n)
    {
        decimal Cost = 0;

        int ageDelta = 0;

        dynamic _WithVar_8783;
        _WithVar_8783 = rob[n];

        //EricL 4/12/2006 Growing old is a bitch
        //Age Cost
        ageDelta = _WithVar_8783.age - CLng(SimOpts.Costs(AGECOSTSTART));
        if (ageDelta > 0 & _WithVar_8783.age > 0)
        {
            if (SimOpts.Costs(AGECOSTMAKELOG) == 1)
            {
                Cost = SimOpts.Costs(AGECOST) * Math.Log(ageDelta);
            }
            else if (SimOpts.Costs(AGECOSTMAKELINEAR) == 1)
            {
                Cost = SimOpts.Costs(AGECOST) + (ageDelta * SimOpts.Costs(AGECOSTLINEARFRACTION));
            }
            else
            {
                Cost = SimOpts.Costs(AGECOST);
            }
            _WithVar_8783.nrg = _WithVar_8783.nrg - (Cost * SimOpts.Costs(COSTMULTIPLIER));
        }

        //BODY UPKEEP
        Cost = _WithVar_8783.body * SimOpts.Costs(BODYUPKEEP) * SimOpts.Costs(COSTMULTIPLIER);
        _WithVar_8783.nrg = _WithVar_8783.nrg - Cost;

        //DNA upkeep cost
        Cost = (_WithVar_8783.DnaLen - 1) * SimOpts.Costs(DNACYCCOST) * SimOpts.Costs(COSTMULTIPLIER);
        _WithVar_8783.nrg = _WithVar_8783.nrg - Cost;

        //degrade slime
        _WithVar_8783.Slime = _WithVar_8783.Slime * 0.98m;
        if (_WithVar_8783.Slime < 0.5m)
        {
            _WithVar_8783.Slime = 0; // To keep things sane for integer rounding, etc.
        }
        _WithVar_8783.mem(821) = CInt(_WithVar_8783.Slime);

        //degrade poison
        _WithVar_8783.poison = _WithVar_8783.poison * 0.98m;
        if (_WithVar_8783.poison < 0.5m)
        {
            _WithVar_8783.poison = 0; //Botsareus 3/15/2013 bug fix for poison so it does not change slime
        }
        _WithVar_8783.mem(827) = CInt(_WithVar_8783.poison);
    }

    private ByRef rob2()


private Integer scanfromn(dynamic rob(_UNUSED) {
        block2, ByVal n As Integer, ByRef layer As Integer scanfromn = null;
        int a = 0;

        for (a = n; a < UBound(rob); a++)
        {
            if (rob(a).match != layer)
            {
                scanfromn = a;
                layer = rob(a).match;
                return scanfromn;
            }
        }
        scanfromn = UBound(rob) + 1;
        return scanfromn;
    }

    //Panda 08/26/2013 Share Chloroplasts between ties variable
    //Botsareus 10/5/2015 freeing up memory from Eric's obsolete ancestors code
    //Private Type ancestorType
    //  num As Long ' unique ID of ancestor
    //  mut As Long ' mutations this ancestor had at time next descendent was born
    //  sim As Long ' the sim this ancestor was born in
    //End Type
    //Botsareus 10/5/2015 Replaced with something better
    //Private Type delgenerestore 'Botsareus 9/16/2014 A new bug fix from Billy
    //position As Integer
    //dna() As block
    //End Type
    // robot structure
    // the robot exists?
    // is it a vegetable?
    // no chloroplasts?
    // is it a wall?
    // is it blocked?
    // has this bot ever tried to see?
    // is the bot using the new physics paradigms?
    // physics
    //Botsareus 6/22/2016 Robots actual velocity if it hits something
    //Used to calculate actvel
    // independant forces vector
    // Resistive forces vector
    // static force scalar (always opposes current forces)
    //From fluid displacement
    // aim angle
    // the unit vector for aim
    // old aim angle
    // angular momentum
    // torch
    // array of ties
    //order in the roborder array
    // array with the ref* values
    // energy
    // old energy
    //Panda 8/11/2013 number of chloroplasts
    // Body points. A corpse still has a body after all
    // old body points, for use with pain pleas versions for body
    // Virtual Body used to calculate body functions of MBs
    // mass of robot
    // Hard shell. protection from shots 1-100 reduces each cycle
    // slime layer. protection from ties 1-100 reduces each cycle
    // waste buildup in a robot. Too much and he dies. some can be removed by various methods
    // Permanent waste. cannot be removed. Builds up as waste is removed.
    // How poisonous is robot
    // How venomous is robot
    // true when robot is paralyzed
    // countdown until paralyzed robot is free to move again
    // the number of ties attached to a robot
    // Is robot part of a multi-bot
    //Botsareus 3/22/2013 allowes program to handle tieang...tielen 1...4 as input
    // Is robot poisoned and confused
    // Countdown till poison is of his system
    // Does robot float or sink
    // controls decay cycle
    // How many other robots has it killed? Might not work properly
    // Allows program to define a robot as dead after a certain operation
    // Location for custom poison to strike
    // Value to insert into venom location
    // Location for custom venom to strike
    // Value to insert into venom location
    // Count down timer to produce a virus
    //| about private variables
    //|
    //| used memory cells
    // virtual machine
    // memory array
    // program array
    // Index of last object in the focus eye.  Could be a bot or shape or something else.
    // Indicates the type of lastopp.
    // 0 - bot
    // 1 - shape
    // 2 - edge of the playing field
    // the position of the closest portion of the viewed object
    // Botsareus 11/26/2013 The robot who is touching our robot.
    // absolute robot number
    // GUID of sim in which this bot was born
    //Mutation related
    // Next cycle to point mutate (expressed in cycles since birth.  ie: age)
    // the base pair to mutate
    // Botsareus 12/10/2013 The new point2 cycle
    // number of conditions (used for cost calculations)
    // console object associated to the robot
    // informative
    // number of sons
    // total mutations
    // total mutations from dna file
    // figure how many mutations before the next genetic test
    // our old genetic distance
    // last mutations
    // how many mutations until epigenetic reset
    // parent absolute number
    // age in cycles
    // age this simulation
    // birth cycle
    // genes number
    // generation
    //'TODO Look at this
    // last internet owner's name
    // species name
    // dna length
    // description of last mutations
    // aspetto
    // skin definition
    // Old skin definition
    // colour
    // is it highlighted?
    // EricL - used for blinking the entire bot a specific color for 1 cycle when various things happen
    //These store the last direction values the bot stored for voluntary movement.  Used to display movement vectors.
    // the viral shot being stored
    // EricL March 15, 2006  Used to store gene activation state
    // EricL New for 2.42.8 - used only for remapping ties when loading multi-cell organisms
    // New for 2.42.9 - the time in cycles before the act of reproduction is free
    // Indicates whether bot's eyes get populated
    // Indicates whether bot's DNA should be executed
    // Indicates whether movement sysvars for this bot should be disabled.
    // Indicates whether reproduction for this robot has been disabled
    // Indicates whether this robot is immune to viruses
    // Indicates this bot's subspecies.  Changed when mutation or virus infection occurs
    //  Ancestors(500) As ancestorType    ' Orderred list of ancestor bot numbers.
    //  AncestorIndex As Integer          ' Index into the Ancestors array.  Points to the bot's immediate parent.  Older ancestors have lower numbers then wrap.
    // If non-zero, indicates this bot has been fertilized via a sperm shot.  This bot can choose to sexually reproduce
    // with this DNA until the counter hits 0.  Will be zero if unfertilized.
    // Contains the DNA this bot has been fertilized with.
    public class robot
    {
        public int AbsNum = 0;
        public vector actvel = null;
        public decimal AddedMass = 0;
        public int age = 0;
        public double aim = 0;
        public vector aimvector = null;
        public int BirthCycle = 0;
        public double body = 0;
        public decimal Bouyancy = 0;
        public IntVector BucketPos = null;
        public bool CantReproduce = false;
        public bool CantSee = false;
        public double chloroplasts = 0;
        public byte Chlr_Share_Delay = 0;
        public int color = 0;
        public int condnum = 0;
        public Consoleform console = null;
        public bool Corpse = false;
        public string dbgstring = "";
        public bool Dead = false;
        public int DecayTimer = 0;
        public bool DisableDNA = false;
        public bool DisableMovementSysvars = false;
        public List<block> dna = new();
        public int DnaLen = 0;
        public int dq = 0;
        public int[] epimem = new int[14];
        public bool exist = false;
        public int fertilized = 0;
        public bool Fixed = false;
        public int flash = 0;
        public string FName = "";
        public bool[] ga = new bool[];
        public int genenum = 0;
        public int generation = 0;
        public decimal GenMut = 0;
        public bool highlight = false;
        public vector ImpulseInd = null;
        public vector ImpulseRes = null;
        public decimal ImpulseStatic = 0;
        public int Kills = 0;
        public int lastdown = 0;
        public int lastleft = 0;
        public int LastMut = 0;
        public string LastMutDetail = "";
        public int lastopp = 0;
        public vector lastopppos = null;
        public int lastopptype = 0;
        public string LastOwner = "";
        public int lastright = 0;
        public int lasttch = 0;
        public int lastup = 0;
        public decimal ma = 0;
        public double mass = 0;
        public int maxusedvars = 0;
        public int[] mem = new int[1000];
        public int monitor_b = 0;
        public int monitor_g = 0;
        public int monitor_r = 0;
        public decimal mt = 0;
        public bool Multibot = false;
        public int multibot_time = 0;
        public mutationprobs Mutables = null;
        public int Mutations = 0;
        public double MutEpiReset = 0;
        public int newage = 0;
        public bool NewMove = false;
        public bool NoChlr = false;
        public double nrg = 0;
        public decimal numties = 0;
        public decimal oaim = 0;
        public decimal obody = 0;
        public int[] occurr = new int[20];
        public int oldBotNum = 0;
        public float OldGD = 0;
        public int OldMutations = 0;
        public decimal onrg = 0;
        public vector opos = null;
        public int order = 0;
        public int[] OSkin = new int[13];
        public decimal Paracount = 0;
        public bool Paralyzed = false;
        public int parent = 0;
        public int Ploc = 0;
        public int Point2MutCycle = 0;
        public int PointMutBP = 0;
        public int PointMutCycle = 0;
        public decimal poison = 0;
        public decimal Poisoncount = 0;
        public bool Poisoned = false;
        public vector pos = null;
        public int Pval = 0;
        public decimal Pwaste = 0;
        public double radius = 0;
        public int reproTimer = 0;
        public double shell = 0;
        public int sim = 0;
        public int[] Skin = new int[13];
        public double Slime = 0;
        public int SonNumber = 0;
        public block[] spermDNA = new block[];
        public int spermDNAlen = 0;
        public int SubSpecies = 0;
        public string tag = "";
        public bool[] TieAngOverwrite = new bool[3];
        public bool[] TieLenOverwrite = new bool[3];
        public tie[] Ties = new tie[10];
        public int[] usedvars = new int[1000];
        public var_[] vars = new var_[1000];
        public decimal vbody = 0;
        public bool Veg = false;
        public vector vel = null;
        public double venom = 0;
        public bool View = false;
        public bool VirusImmune = false;
        public int virusshot = 0;
        public int Vloc = 0;
        public int vnum = 0;
        public int Vtimer = 0;
        public int Vval = 0;
        public bool wall = false;
        public double Waste = 0;
        //TODO: Fixed Length Strings Not Supported: * 50
    }

    // Display value to avoid displaying half updated numbers
    //Botsareus 4/9/2013 used by genetic distance graph. The higher this number, the more the robot is checked
    private class block2
    {
        public int match = 0;
        public int tipo = 0;
        public int value = 0;
    }

    private class block3
    {
        public int match = 0;
        public int nucli = 0;
    }

    private static block2,

    private static block3,     /*
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'End crossover section

    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

    '  R O B O T S    M A N A G E M E N T

    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    */
    /*
    ' returns an absolute acceleration, given up-down,
    ' left-right values and aim
    */
    /*
    ' updates positions, transforming calculated accelerations
    ' in velocities, and velocities in new positions
    */
    /*
    'Add bots reproducing this cycle to the rep array
    'Currently possible to reproduce both sexually and asexually in the same cycle!
    */
    /*
    'The heart of the robots to simulation interfacing
    */
    /*
    ' here we catch the attempt of a robot to shoot,
    ' and actually build the shot
    */
    /*
    'Robot n converts some of his energy to venom
    */
    /*
    ' Robot n converts some of his energy to poison
    */
    /*
    ' Reproduction
    ' makes some tests regarding the available space for
    ' spawning a new robot, the position (not off the field, nor
    ' on the internet d/l gate), the energy of the parent,
    ' then finally copies the dna and most of the two data
    ' structures (with some modif. - for example generation),
    ' sends the newborn rob to the mutation division,
    ' reanalizes the resulting dna (usedvars, condlist, and so on)
    ' ties parent and son, and the miracle of birth is accomplished
    */
    /*
    'Botsareus 12/1/2013 Redone to work in all cases
    */
    /*
    ' verifies rapidly if a field position is already occupied
    */
    /*
    ' searches a free slot in the robots array, to store a new rob
    */
    /*
    ' Kills the robot and writes a snapshot record
    */
}
