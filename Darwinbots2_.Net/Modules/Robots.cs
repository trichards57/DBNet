using DarwinBots.Model;
using DarwinBots.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using static DarwinBots.Modules.Common;
using static DarwinBots.Modules.Database;
using static DarwinBots.Modules.DNAManipulations;
using static DarwinBots.Modules.DNATokenizing;
using static DarwinBots.Modules.Globals;
using static DarwinBots.Modules.HDRoutines;
using static DarwinBots.Modules.NeoMutations;
using static DarwinBots.Modules.ObstaclesManager;
using static DarwinBots.Modules.Physics;
using static DarwinBots.Modules.Senses;
using static DarwinBots.Modules.ShotsManager;
using static DarwinBots.Modules.SimOpt;
using static DarwinBots.Modules.Teleport;
using static DarwinBots.Modules.Ties;
using static DarwinBots.Modules.Vegs;

namespace DarwinBots.Modules
{
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
        public const int MaxMem = 1000;
        public const int maxvelsys = 11;
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
        public const int refbody = 688;
        public const int refmulti = 686;
        public const int refshell = 687;
        public const int REFTYPE = 685;
        public const int refveldn = 698;
        public const int refveldx = 697;
        public const int refvelscalar = 695;
        public const int refvelsx = 696;
        public const int refvelup = 699;
        public const int refxpos = 689;
        public const int refypos = 690;
        public const int Repro = 300;
        public const int rmchlr = 922;
        public const int robage = 9;
        public const int ROBARRAYMAX = 32000;
        public const int RobSize = 120;
        public const int SetAim = 19;
        public const int setboy = 314;
        public const int SEXREPRO = 302;
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

        public static int MaxRobs { get; set; }
        public static List<robot> rob { get; set; } = new();
        public static robot robfocus { get; set; }
        public static int TotalRobots { get; set; }
        public static int TotalRobotsDisplayed { get; set; }
        private static List<robot> BotsToKill { get; set; } = new();
        private static List<robot> BotsToReproduce { get; set; } = new();
        private static List<robot> BotsToReproduceSexually { get; set; } = new();

        public static double AbsX(double aim, int up, int dn, int sx, int dx)
        {
            var upTotal = Math.Clamp(up - dn, -32000, 32000);
            var sxTotal = Math.Clamp(sx - dx, -32000, 32000);

            return Math.Cos(aim) * upTotal + Math.Sin(aim) * sxTotal;
        }

        public static double AbsY(double aim, int up, int dn, int sx, int dx)
        {
            var upTotal = Math.Clamp(up - dn, -32000, 32000);
            var sxTotal = Math.Clamp(sx - dx, -32000, 32000);

            return -Math.Sin(aim) * upTotal + Math.Cos(aim) * sxTotal;
        }

        public static double DoGeneticDistance(robot r1, robot r2)
        {
            var ndna1 = new List<block3>();
            var ndna2 = new List<block3>();

            ndna1.AddRange(r1.dna.Select(b => new block3() { nucli = DNAtoInt(b.tipo, b.value) }));
            ndna2.AddRange(r2.dna.Select(b => new block3() { nucli = DNAtoInt(b.tipo, b.value) }));

            //Step3 Figure genetic distance
            SimpleMatch(ndna1, ndna2);

            return GeneticDistance(ndna1, ndna2);
        }

        public static void DoGeneticMemory(robot rob)
        {
            if (rob.Ties.Count <= 0 || rob.Ties[0].Last <= 0)
                return;

            var loc = 976 + rob.age;
            if (rob.mem[loc] == 0 & rob.epimem[rob.age] != 0)
            {
                rob.mem[loc] = rob.epimem[rob.age];
            }
        }

        public static double FindRadius(robot rob, double mult = 1)
        {
            double bodypoints;
            double chlr;

            if (mult == -1)
            {
                bodypoints = 32000;
                chlr = 0;
            }
            else
            {
                bodypoints = rob.body * mult;
                chlr = rob.chloroplasts * mult;
            }

            if (bodypoints < 1)
                bodypoints = 1;

            if (SimOpts.FixedBotRadii)
                return half;
            else
            {
                var radius = Math.Pow(Math.Log(bodypoints) * bodypoints * CubicTwipPerBody * 3 * 0.25 / Math.PI, 1.0 / 3);
                radius += (415 - radius) * chlr / 32000;

                if (radius < 1)
                    radius = 1;

                return radius;
            }
        }

        public static robot GetNewBot()
        {
            var rob = new robot();

            Robots.rob.Add(rob);

            return rob;
        }

        public static async Task KillRobot(robot rob)
        {
            if (SimOpts.DeadRobotSnp && (!rob.Veg || !SimOpts.SnpExcludeVegs))
                await AddRecord(rob);

            rob.Fixed = false;
            rob.Veg = false;
            rob.View = false;
            rob.NewMove = false;
            rob.LastOwner = "";
            rob.SonNumber = 0;
            rob.age = 0;
            DeleteAllTies(rob);
            rob.exist = false; // do this after deleting the ties...
            Globals.BucketManager.UpdateBotBucket(rob);

            //if (!MDIForm1.instance.nopoff)
            //    MakePoff(rob);

            if (rob.virusshot != null)
            {
                rob.virusshot.exist = false; // We have to kill any stored shots for this bot
                Shots.Remove(rob.virusshot);
                rob.virusshot = null;
            }

            rob.spermDNA.Clear();

            rob.LastMutDetail = "";

            Robots.rob.Remove(rob);
        }

        public static void Reproduce(robot rob, int per)
        {
            if (rob.body < 5)
                return;

            if (SimOpts.DisableTypArepro && rob.Veg == false)
                return;

            if (rob.body <= 2 || rob.CantReproduce)
                return;

            //attempt to stop veg overpopulation but will it work?
            if (rob.Veg == true && (TotalChlr > SimOpts.MaxPopulation || totvegsDisplayed < 0))
                return;

            // If we got here and it's a veg, then we are below the reproduction threshold.  Let a random 10% of the veggis reproduce
            // so as to avoid all the veggies reproducing on the same cycle.  This adds some randomness
            // so as to avoid giving preference to veggies with lower bot array numbers.  If the veggy population is below 90% of the threshold
            // then let them all reproduce.
            if (rob.Veg == true && (ThreadSafeRandom.Local.Next(0, 10) != 5) && (TotalChlr > (SimOpts.MaxPopulation * 0.9)))
                return;
            if (totvegsDisplayed == -1)
                return;

            per %= 100; // per should never be <=0 as this is checked in ManageReproduction()

            if (reprofix && per < 3)
                rob.Dead = true;

            if (per <= 0)
                return;

            var sondist = (int)(FindRadius(rob, (double)per / 100) + FindRadius(rob, (double)(100 - per) / 100));

            if (rob.nrg <= 0)
                return;

            var nx = (int)(rob.pos.X + AbsX(rob.aim, sondist, 0, 0, 0));
            var ny = (int)(rob.pos.Y + AbsY(rob.aim, sondist, 0, 0, 0));

            if (SimpleCollision(nx, ny, rob) || !rob.exist)
                return;

            var nuovo = GetNewBot();

            SimOpts.TotBorn = SimOpts.TotBorn + 1;

            if (rob.Veg)
                totvegs++;

            nuovo.dna.AddRange(rob.dna);
            nuovo.genenum = rob.genenum;
            nuovo.Mutables = rob.Mutables;

            nuovo.Mutations = rob.Mutations;
            nuovo.OldMutations = rob.OldMutations; //Botsareus 10/8/2015

            nuovo.LastMut = 0;
            nuovo.LastMutDetail = rob.LastMutDetail;

            for (var t = 0; t < 12; t++)
                nuovo.Skin[t] = rob.Skin[t];

            Array.Clear(nuovo.mem, 0, nuovo.mem.Length);
            nuovo.Ties.Clear();

            nuovo.pos = new DoubleVector(rob.pos.X + AbsX(rob.aim, sondist, 0, 0, 0), rob.pos.Y + AbsY(rob.aim, sondist, 0, 0, 0));
            nuovo.exist = true;
            nuovo.BucketPos = new IntVector(-2, -2);
            Globals.BucketManager.UpdateBotBucket(nuovo);
            nuovo.vel = rob.vel;
            nuovo.actvel = rob.actvel;
            nuovo.color = rob.color;
            nuovo.aim = Physics.NormaliseAngle(rob.aim + Math.PI);

            nuovo.aimvector = new DoubleVector(Math.Cos(nuovo.aim), Math.Sin(nuovo.aim));
            nuovo.mem[SetAim] = (int)(nuovo.aim * 200);
            nuovo.mem[468] = 32000;
            nuovo.Corpse = false;
            nuovo.Dead = false;
            nuovo.NewMove = rob.NewMove;
            nuovo.generation = rob.generation + 1;

            if (nuovo.generation > 32000)
                nuovo.generation = 32000;

            nuovo.BirthCycle = SimOpts.TotRunCycle;

            var nnrg = rob.nrg / 100 * per;
            var nbody = rob.body / 100 * per;
            var nwaste = rob.Waste / 100 * per;
            var npwaste = rob.Pwaste / 100 * per;
            var nchloroplasts = rob.chloroplasts / 100 * per;

            rob.nrg -= nnrg + (nnrg * 0.001);
            rob.Waste -= nwaste;
            rob.Pwaste -= npwaste;
            rob.body -= nbody;
            rob.radius = FindRadius(rob);
            rob.chloroplasts -= nchloroplasts;

            nuovo.chloroplasts = nchloroplasts; //Panda 8/23/2013 Distribute the chloroplasts
            nuovo.body = nbody;
            nuovo.radius = FindRadius(nuovo);
            nuovo.Waste = nwaste;
            nuovo.Pwaste = npwaste;
            rob.mem[Energy] = (int)rob.nrg;
            rob.mem[311] = (int)rob.body;
            rob.SonNumber++;
            if (rob.SonNumber > 32000)
                rob.SonNumber = 32000;

            nuovo.nrg = nnrg * 0.999; // Make reproduction cost 1% of nrg transfer
            nuovo.onrg = nnrg * 0.999;
            nuovo.mem[Energy] = (int)nuovo.nrg;
            nuovo.Poisoned = false;
            nuovo.parent = rob;
            nuovo.FName = rob.FName;
            nuovo.LastOwner = rob.LastOwner;
            nuovo.Veg = rob.Veg;
            nuovo.NoChlr = rob.NoChlr; //Botsareus 3/28/2014 Disable chloroplasts
            nuovo.Fixed = rob.Fixed;
            nuovo.CantSee = rob.CantSee;
            nuovo.DisableDNA = rob.DisableDNA;
            nuovo.DisableMovementSysvars = rob.DisableMovementSysvars;
            nuovo.CantReproduce = rob.CantReproduce;
            nuovo.VirusImmune = rob.VirusImmune;
            if (nuovo.Fixed)
                nuovo.mem[Fixed] = 1;

            nuovo.SubSpecies = rob.SubSpecies;
            nuovo.OldGD = rob.OldGD;
            nuovo.GenMut = rob.GenMut;
            nuovo.tag = rob.tag;
            nuovo.Bouyancy = rob.Bouyancy;

            if (rob.multibot_time > 0)
                nuovo.multibot_time = rob.multibot_time / 2 + 2;

            nuovo.Vtimer = 0;
            nuovo.virusshot = null;

            for (var i = 0; i < 4; i++)
                nuovo.mem[971 + i] = rob.mem[971 + i];

            //The other 15 genetic memory locations are stored now but can be used later
            for (var i = 0; i < 14; i++)
                nuovo.epimem[i] = rob.mem[976 + i];

            //Erase parents genetic memory now to prevent him from completing his own transfer by using his kid
            for (var i = 0; i < 14; i++)
                rob.epimem[i] = 0;

            //Botsareus 12/17/2013 Delta2
            if (Delta2)
            {
                var MratesMax = NormMut ? nuovo.dna.Count * valMaxNormMut : 2000000000;
                var dmoc = !y_normsize ? 1 : Math.Max(1 + (nuovo.dna.Count - curr_dna_size) / 500, 0.01);

                //zerobot stabilization
                if ((x_restartmode == 7 || x_restartmode == 8) && nuovo.FName == "Mutate.txt")
                {
                    //normalize child
                    nuovo.Mutables.mutarray[PointUP] *= 1.75;
                    if (nuovo.Mutables.mutarray[PointUP] > MratesMax)
                        nuovo.Mutables.mutarray[PointUP] = MratesMax;

                    nuovo.Mutables.mutarray[P2UP] *= 1.75;
                    if (nuovo.Mutables.mutarray[P2UP] > MratesMax)
                        nuovo.Mutables.mutarray[P2UP] = MratesMax;
                }

                var endItem = ThreadSafeRandom.Local.Next(2, 4);

                for (var mrep = 0; mrep < endItem; mrep++)
                { //2x to 4x
                    for (var t = 1; t < 10; t++)
                    {
                        if (t == 9 || nuovo.Mutables.mutarray[t] < 1)
                            continue; //ignore PM2 mutation here

                        if (ThreadSafeRandom.Local.NextDouble() < DeltaMainChance / 100)
                        {
                            if (DeltaMainExp != 0)
                            {
                                if (t == CopyErrorUP || t == TranslocationUP || t == ReversalUP || t == CE2UP)
                                    nuovo.Mutables.mutarray[t] *= (dmoc + 2) / 3;
                                else if (!(t == MinorDeletionUP || t == MajorDeletionUP))
                                    nuovo.Mutables.mutarray[t] *= dmoc; //dynamic mutation overload correction

                                nuovo.Mutables.mutarray[t] *= Math.Pow(10, ThreadSafeRandom.Local.NextDouble() * 2 - 1) / DeltaMainExp;
                            }
                            nuovo.Mutables.mutarray[t] += (ThreadSafeRandom.Local.NextDouble() * 2 - 1) * DeltaMainLn;
                            if (nuovo.Mutables.mutarray[t] < 1)
                                nuovo.Mutables.mutarray[t] = 1;

                            if (nuovo.Mutables.mutarray[t] > MratesMax)
                                nuovo.Mutables.mutarray[t] = MratesMax;
                        }
                        if (ThreadSafeRandom.Local.NextDouble() < DeltaDevChance / 100)
                        {
                            if (DeltaDevExp != 0)
                            {
                                nuovo.Mutables.StdDev[t] *= Math.Pow(10, (ThreadSafeRandom.Local.NextDouble() * 2 - 1) / DeltaDevExp);
                            }
                            nuovo.Mutables.StdDev[t] += (ThreadSafeRandom.Local.NextDouble() * 2 - 1) * DeltaDevLn;
                            if (DeltaDevExp != 0)
                            {
                                nuovo.Mutables.Mean[t] *= Math.Pow(10, (ThreadSafeRandom.Local.NextDouble() * 2 - 1) / DeltaDevExp);
                            }
                            nuovo.Mutables.Mean[t] += (ThreadSafeRandom.Local.NextDouble() * 2 - 1) * DeltaDevLn;

                            nuovo.Mutables.StdDev[t] = Math.Clamp(nuovo.Mutables.StdDev[t], 0, 200);
                            nuovo.Mutables.Mean[t] = Math.Clamp(nuovo.Mutables.Mean[t], 1, 400);
                        }
                    }
                    nuovo.Mutables.CopyErrorWhatToChange += (int)((ThreadSafeRandom.Local.NextDouble() * 2 - 1) * DeltaWTC);
                    nuovo.Mutables.CopyErrorWhatToChange = Math.Clamp(nuovo.Mutables.CopyErrorWhatToChange, 0, 100);
                    Mutate(nuovo, true);
                }
            }
            else
            {
                if (rob.mem[mrepro] > 0)
                {
                    var temp = nuovo.Mutables;

                    nuovo.Mutables.Mutations = true; // mutate even if mutations disabled for this bot

                    for (var t = 0; t < 20; t++)
                    {
                        nuovo.Mutables.mutarray[t] = nuovo.Mutables.mutarray[t] / 10;
                        if (nuovo.Mutables.mutarray[t] == 0)
                            nuovo.Mutables.mutarray[t] = 1000;
                    }

                    Mutate(nuovo, true);

                    nuovo.Mutables = temp;
                }
                else
                    Mutate(nuovo, true);
            }

            MakeOccurrList(nuovo);
            nuovo.genenum = CountGenes(nuovo.dna);
            nuovo.mem[DnaLenSys] = nuovo.dna.Count;
            nuovo.mem[GenesSys] = nuovo.genenum;

            MakeTie(rob, nuovo, sondist, 100, 0); //birth ties last 100 cycles
            rob.onrg = rob.nrg; //saves parent from dying from shock after giving birth
            nuovo.mass = nbody / 1000 + nuovo.shell / 200;
            nuovo.mem[timersys] = rob.mem[timersys]; //epigenetic timer

            //Successfully reproduced
            rob.mem[Repro] = 0;
            rob.mem[mrepro] = 0;

            //Botsareus 11/29/2013 Reset epigenetic memory
            if (epireset)
            {
                nuovo.MutEpiReset = rob.MutEpiReset + Math.Pow(nuovo.LastMut, epiresetemp);
                if (nuovo.MutEpiReset > epiresetOP && rob.MutEpiReset > 0)
                {
                    nuovo.MutEpiReset = 0;
                    for (var i = 0; i < 4; i++)
                    {
                        nuovo.mem[971 + i] = 0;
                    }
                    for (var i = 0; i < 14; i++)
                    {
                        nuovo.epimem[i] = 0;
                    }
                }
            }

            rob.nrg -= rob.dna.Count * SimOpts.Costs.DnaCopyCost * SimOpts.Costs.CostMultiplier;
            if (rob.nrg < 0)
                rob.nrg = 0;
        }

        public static void SexReproduce(robot female)
        {
            if (female.body < 5 || !female.exist || female.Corpse || female.CantReproduce || female.body <= 2 || !female.spermDNA.Any())
                return;

            //The percent of resources given to the offspring comes exclusivly from the mother
            //Perhaps this will lead to sexual selection since sex is expensive for females but not for males
            double per = female.mem[SEXREPRO];

            //veggies can reproduce sexually, but we still have to test for veggy population controls
            //we let male veggies fertilize nonveggie females all they want since the offspring's "species" and thus vegginess
            //will be determined by their mother.  Perhaps a strategy will emerge where plants compete to reproduce
            //with nonveggies so as to bypass the popualtion limtis?  Who knows.
            if (female.Veg == true && (TotalChlr > SimOpts.MaxPopulation || totvegsDisplayed < 0))
                return;

            // If we got here and the female is a veg, then we are below the reproduction threshold.  Let a random 10% of the veggis reproduce
            // so as to avoid all the veggies reproducing on the same cycle.  This adds some randomness
            // so as to avoid giving preference to veggies with lower bot array numbers.  If the veggy population is below 90% of the threshold
            // then let them all reproduce.
            if (female.Veg == true && (ThreadSafeRandom.Local.Next(0, 9) != 5) && (TotalChlr > (SimOpts.MaxPopulation * 0.9)))
                return;

            if (totvegsDisplayed == -1)
                return;// no veggies can reproduce on the first cycle after the sim is restarted.

            per %= 100; // per should never be <=0 as this is checked in ManageReproduction()

            if (reprofix && per < 3)
                female.Dead = true;

            if (per <= 0)
                return;

            var sondist = FindRadius(female, per / 100) + FindRadius(female, (100 - per) / 100);

            var tempnrg = female.nrg;

            if (tempnrg <= 0)
                return;

            var nx = female.pos.X + AbsX(female.aim, (int)sondist, 0, 0, 0);
            var ny = female.pos.Y + AbsY(female.aim, (int)sondist, 0, 0, 0);

            if (SimpleCollision((int)nx, (int)ny, female) || !female.exist)
                return;

            //Step1 Copy both dnas into block2

            var dna1 = female.dna.Select(d => new block2 { tipo = d.tipo, value = d.value }).ToList();
            var dna2 = female.spermDNA.Select(d => new block2 { tipo = d.tipo, value = d.value }).ToList();

            //Step2 map nucli

            var ndna1 = dna1.Select(d => new block3 { nucli = DNAtoInt(d.tipo, d.value) }).ToList();
            var ndna2 = dna2.Select(d => new block3 { nucli = DNAtoInt(d.tipo, d.value) }).ToList();

            //Step3 Check longest sequences

            SimpleMatch(ndna1, ndna2);

            //If robot is too unsimiler then do not reproduce and block sex reproduction for 8 cycles

            if (GeneticDistance(ndna1, ndna2) > 0.6)
            {
                female.fertilized = -18;
                return;
            }

            //Step4 map back

            for (var t = 0; t < dna1.Count; t++)
                dna1[t].match = ndna1[t].match;

            for (var t = 0; t < dna2.Count; t++)
                dna2[t].match = ndna2[t].match;

            //Step5 do crossover

            var Outdna = Crossover(dna1, dna2);
            var nuovo = GetNewBot();

            SimOpts.TotBorn++;
            if (female.Veg)
                totvegs++;

            //Step4 after robot is created store the dna

            nuovo.dna = Outdna;
            nuovo.genenum = CountGenes(nuovo.dna);
            nuovo.Mutables = female.Mutables;

            nuovo.Mutations = female.Mutations;
            nuovo.OldMutations = female.OldMutations; //Botsareus 10/8/2015

            nuovo.LastMut = 0;
            nuovo.LastMutDetail = female.LastMutDetail;

            for (var t = 0; t < 12; t++)
            {
                nuovo.Skin[t] = female.Skin[t];
            }

            Array.Clear(nuovo.mem, 0, nuovo.mem.Length);
            nuovo.Ties.Clear();

            nuovo.pos += new DoubleVector(AbsX(female.aim, (int)sondist, 0, 0, 0), AbsY(female.aim, (int)sondist, 0, 0, 0));
            nuovo.exist = true;
            nuovo.BucketPos = new IntVector(-2, -2);
            Globals.BucketManager.UpdateBotBucket(nuovo);

            nuovo.vel = female.vel;
            nuovo.actvel = female.actvel; //Botsareus 7/1/2016 Bugfix
            nuovo.color = female.color;
            nuovo.aim = Physics.NormaliseAngle(female.aim + Math.PI);
            nuovo.aimvector = new DoubleVector(Math.Cos(nuovo.aim), Math.Sin(nuovo.aim));
            nuovo.mem[SetAim] = (int)(nuovo.aim * 200);
            nuovo.mem[468] = 32000;
            nuovo.Corpse = false;
            nuovo.Dead = false;
            nuovo.NewMove = female.NewMove;
            nuovo.generation++;
            if (nuovo.generation > 32000)
                nuovo.generation = 32000; //Botsareus 10/9/2015 Overflow protection

            nuovo.BirthCycle = SimOpts.TotRunCycle;

            var nnrg = female.nrg / 100 * per;
            var nbody = female.body / 100 * per;
            var nwaste = female.Waste / 100 * per;
            var npwaste = female.Pwaste / 100 * per;
            var nchloroplasts = female.chloroplasts / 100 * per; //Panda 8/23/2013 Distribute the chloroplasts

            female.nrg -= nnrg + (nnrg * 0.001);
            female.Waste -= nwaste;
            female.Pwaste -= npwaste;
            female.body -= nbody;
            female.radius = FindRadius(female);
            female.chloroplasts -= nchloroplasts; //Panda 8/23/2013 Distribute the chloroplasts

            nuovo.chloroplasts = nchloroplasts; //Botsareus 8/24/2013 Distribute the chloroplasts
            nuovo.body = nbody;
            nuovo.radius = FindRadius(nuovo);
            nuovo.Waste = nwaste;
            nuovo.Pwaste = npwaste;
            female.mem[Energy] = (int)female.nrg;
            female.mem[311] = (int)female.body;
            female.SonNumber++;

            if (female.SonNumber > 32000)
                female.SonNumber = 32000; // EricL Overflow protection.  Should change to Long at some point.

            nuovo.nrg = nnrg * 0.999; // Make reproduction cost 1% of nrg transfer for offspring
            nuovo.onrg = nnrg * 0.999;
            nuovo.mem[Energy] = (int)nuovo.nrg;
            nuovo.Poisoned = false;
            nuovo.parent = female;
            nuovo.FName = female.FName;
            nuovo.LastOwner = female.LastOwner;
            nuovo.Veg = female.Veg;
            nuovo.NoChlr = female.NoChlr; //Botsareus 3/28/2014 Disable chloroplasts
            nuovo.Fixed = female.Fixed;
            nuovo.CantSee = female.CantSee;
            nuovo.DisableDNA = female.DisableDNA;
            nuovo.DisableMovementSysvars = female.DisableMovementSysvars;
            nuovo.CantReproduce = female.CantReproduce;
            nuovo.VirusImmune = female.VirusImmune;
            if (nuovo.Fixed)
                nuovo.mem[Fixed] = 1;

            nuovo.SubSpecies = female.SubSpecies;

            nuovo.OldGD = female.OldGD;
            nuovo.GenMut = female.GenMut;
            nuovo.tag = female.tag;
            nuovo.Bouyancy = female.Bouyancy;

            if (female.multibot_time > 0)
                nuovo.multibot_time = female.multibot_time / 2 + 2;

            nuovo.Vtimer = 0;
            nuovo.virusshot = null;

            //First 5 genetic memory locations happen instantly
            for (var i = 0; i < 5; i++)
                nuovo.mem[971 + i] = female.mem[971 + i];

            //The other 15 genetic memory locations are stored now but can be used later
            for (var i = 0; i < 15; i++)
                nuovo.epimem[i] = female.mem[976 + i];

            //Erase parents genetic memory now to prevent him from completing his own transfer by using his kid
            for (var i = 0; i < 14; i++)
                female.epimem[i] = 0;

            LogMutation(nuovo, $"Female DNA len {female.dna.Count} and male DNA len {female.spermDNA.Count} had offspring DNA len {nuovo.dna.Count} during cycle {SimOpts.TotRunCycle}");

            if (Delta2)
            {
                var MratesMax = NormMut ? nuovo.dna.Count * valMaxNormMut : 2000000000;

                double dmoc;
                if (!y_normsize)
                    dmoc = 1;
                else
                {
                    dmoc = 1 + (double)(nuovo.dna.Count - curr_dna_size) / 500;
                    if (dmoc < 0.01)
                        dmoc = 0.01; //Botsareus 1/16/2016 Bug fix
                }

                for (var t = 1; t < 10; t++)
                {
                    if (t == 9 || nuovo.Mutables.mutarray[t] < 1)
                        continue;

                    if (ThreadSafeRandom.Local.NextDouble() < (double)DeltaMainChance / 100)
                    {
                        if (DeltaMainExp != 0)
                        {
                            if (t == CopyErrorUP || t == TranslocationUP || t == ReversalUP || t == CE2UP)
                                nuovo.Mutables.mutarray[t] *= (dmoc + 2) / 3;
                            else if (!(t == MinorDeletionUP || t == MajorDeletionUP))
                                nuovo.Mutables.mutarray[t] *= dmoc; //dynamic mutation overload correction

                            nuovo.Mutables.mutarray[t] *= Math.Pow(10, (ThreadSafeRandom.Local.NextDouble() * 2 - 1) / DeltaMainExp);
                        }
                        nuovo.Mutables.mutarray[t] += (ThreadSafeRandom.Local.NextDouble() * 2 - 1) * DeltaMainLn;
                        if (nuovo.Mutables.mutarray[t] < 1)
                            nuovo.Mutables.mutarray[t] = 1;

                        if (nuovo.Mutables.mutarray[t] > MratesMax)
                            nuovo.Mutables.mutarray[t] = MratesMax;
                    }
                    if (ThreadSafeRandom.Local.NextDouble() < (double)DeltaDevChance / 100)
                    {
                        if (DeltaDevExp != 0)
                            nuovo.Mutables.StdDev[t] *= Math.Pow(10, (ThreadSafeRandom.Local.NextDouble() * 2 - 1) / DeltaDevExp);

                        nuovo.Mutables.StdDev[t] += (ThreadSafeRandom.Local.NextDouble() * 2 - 1) * DeltaDevLn;
                        if (DeltaDevExp != 0)
                            nuovo.Mutables.Mean[t] *= Math.Pow(10, (ThreadSafeRandom.Local.NextDouble() * 2 - 1) / DeltaDevExp);

                        nuovo.Mutables.Mean[t] += (ThreadSafeRandom.Local.NextDouble() * 2 - 1) * DeltaDevLn;

                        nuovo.Mutables.StdDev[t] = Math.Clamp(nuovo.Mutables.StdDev[t], 0, 200);
                        nuovo.Mutables.Mean[t] = Math.Clamp(nuovo.Mutables.Mean[t], 1, 400);
                    }
                }
                nuovo.Mutables.CopyErrorWhatToChange += (int)((ThreadSafeRandom.Local.NextDouble() * 2 - 1) * DeltaWTC);

                if (nuovo.Mutables.CopyErrorWhatToChange < 0)
                    nuovo.Mutables.CopyErrorWhatToChange = 0;

                if (nuovo.Mutables.CopyErrorWhatToChange > 100)
                    nuovo.Mutables.CopyErrorWhatToChange = 100;

                Mutate(nuovo, true);
            }
            else
            {
                Mutate(nuovo, true);
            }

            MakeOccurrList(nuovo);
            nuovo.genenum = CountGenes(nuovo.dna);
            nuovo.mem[DnaLenSys] = nuovo.dna.Count;
            nuovo.mem[GenesSys] = nuovo.genenum;

            MakeTie(female, nuovo, (int)sondist, 100, 0); //birth ties last 100 cycles
            female.onrg = female.nrg; //saves mother from dying from shock after giving birth
            nuovo.mass = nbody / 1000 + nuovo.shell / 200;
            nuovo.mem[timersys] = female.mem[timersys]; //epigenetic timer

            female.mem[SEXREPRO] = 0; // sucessfully reproduced, so reset .sexrepro
            female.fertilized = -1; // Set to -1 so spermDNA space gets reclaimed next cycle
            female.mem[SYSFERTILIZED] = 0; // Sperm is only good for one birth presently

            //Botsareus 11/29/2013 Reset epigenetic memory
            if (epireset)
            {
                nuovo.MutEpiReset = female.MutEpiReset + Math.Pow(nuovo.LastMut, epiresetemp);
                if (nuovo.MutEpiReset > epiresetOP && female.MutEpiReset > 0)
                {
                    nuovo.MutEpiReset = 0;

                    for (var i = 0; i < 4; i++)
                        nuovo.mem[971 + i] = 0;

                    for (var i = 0; i < 14; i++)
                        nuovo.epimem[i] = 0;
                }
            }

            female.nrg -= female.dna.Count * SimOpts.Costs.DnaCopyCost * SimOpts.Costs.CostMultiplier; //Botsareus 7/7/2013 Reproduction DNACOPY cost

            if (female.nrg < 0)
                female.nrg = 0;
        }

        public static void ShareChloroplasts(robot rob, Tie tie)
        {
            ShareResource(rob, tie, sharechlr, r => r.chloroplasts, (r, s) => r.chloroplasts = s);
        }

        public static void ShareEnergy(robot rob, Tie tie)
        {
            //This is an order of operation thing.  A bot earlier in the rob array might have taken all your nrg, taking your
            //nrg to 0.  You should still be able to take some back.
            if (rob.nrg < 0 || tie.OtherBot.nrg < 0)
                return;

            //.mem(830) is the percentage of the total nrg this bot wants to receive
            //has to be positive to come here, so no worries about changing the .mem location here
            if (rob.mem[830] <= 0)
                rob.mem[830] = 0;
            else
            {
                rob.mem[830] = rob.mem[830] % 100;
                if (rob.mem[830] == 0)
                    rob.mem[830] = 100;
            }

            //Total nrg of both bots combined
            var totnrg = rob.nrg + tie.OtherBot.nrg;

            var portionThatsMine = totnrg * ((double)rob.mem[830] / 100); // This is what the bot wants to have of the total
            if (portionThatsMine > 32000)
                portionThatsMine = 32000; // Can't want more than the max a bot can have

            var myChangeInNrg = portionThatsMine - rob.nrg; // This is what the bot's change in nrg would be

            //If the bot is taking nrg, then he can't take more than that represented by his own body.  If giving nrg away, same thing.  The bot
            //can't give away more than that represented by his body.  Should make it so that larger bots win tie feeding battles.
            if (Math.Abs(myChangeInNrg) > rob.body)
                myChangeInNrg = Math.Sign(myChangeInNrg) * rob.body;

            if (rob.nrg + myChangeInNrg > 32000)
                myChangeInNrg = 32000 - rob.nrg; //Limit change if it would put bot over the limit

            if (rob.nrg + myChangeInNrg < 0)
                myChangeInNrg = -rob.nrg; // Limit change if it would take the bot below 0

            //Now we have to check the limits on the other bot
            //sign is negative since the negative of myChangeinNrg is what the other bot is going to get/recevie
            if (tie.OtherBot.nrg - myChangeInNrg > 32000)
                myChangeInNrg = -(32000 - tie.OtherBot.nrg); //Limit change if it would put bot over the limit

            if (tie.OtherBot.nrg - myChangeInNrg < 0)
                myChangeInNrg = tie.OtherBot.nrg; // limit change if it would take the bot below 0

            //Do the actual nrg exchange
            rob.nrg += myChangeInNrg;
            tie.OtherBot.nrg -= myChangeInNrg;

            //Transferring nrg costs nrg.  1% of the transfer gets deducted from the bot iniating the transfer
            rob.nrg -= Math.Abs(myChangeInNrg) * 0.01;

            //Bots with 32000 nrg can still take or receive nrg, but everything over 32000 disappears
            if (rob.nrg > 32000)
                rob.nrg = 32000;

            if (tie.OtherBot.nrg > 32000)
                tie.OtherBot.nrg = 32000;
        }

        public static void ShareShell(robot rob, Tie tie)
        {
            ShareResource(rob, tie, 832, r => r.shell, (r, s) => r.shell = s);
        }

        public static void ShareSlime(robot rob, Tie tie)
        {
            ShareResource(rob, tie, 833, r => r.Slime, (r, s) => r.Slime = s);
        }

        public static void ShareWaste(robot rob, Tie tie)
        {
            ShareResource(rob, tie, 831, r => r.Waste, (r, s) => r.Waste = s);
        }

        public static bool SimpleCollision(int X, int Y, robot rob)
        {
            if (Robots.rob.Any(r => r.exist && !(r.FName == "Base.txt" && hidepred) && r != rob && Math.Abs(r.pos.X - X) < r.radius + rob.radius && Math.Abs(r.pos.Y - Y) < r.radius + rob.radius))
                return true;

            if (ObstaclesManager.Obstacles.Any(o => o.pos.X <= Math.Max(rob.pos.X, X) && o.pos.X + o.Width >= Math.Min(rob.pos.X, X) && o.pos.Y <= Math.Max(rob.pos.Y, Y) && o.pos.Y + o.Height >= Math.Min(rob.pos.Y, Y)))
                return true;

            if (SimOpts.Dxsxconnected == false && (X < rob.radius + SmudgeFactor || X + rob.radius + SmudgeFactor > SimOpts.FieldWidth))
                return true;

            if (SimOpts.Updnconnected == false && (Y < rob.radius + SmudgeFactor || Y + rob.radius + SmudgeFactor > SimOpts.FieldHeight))
                return true;

            return false;
        }

        public static void StorePoison(robot rob)
        {
            const double poisonNrgConvRate = 0.25; // Make 4 poison for 1 nrg

            StoreResource(rob, 826, 827, poisonNrgConvRate, SimOpts.Costs.PoisonCost, r => r.poison, (r, s) => r.poison = s);
        }

        public static void StoreVenom(robot rob)
        {
            const double venomNrgConvRate = 1.0; // Make 1 venom for 1 nrg

            StoreResource(rob, 824, 825, venomNrgConvRate, SimOpts.Costs.VenomCost, r => r.venom, (r, s) => r.venom = s);
        }

        public static async Task UpdateBots()
        {
            BotsToKill.Clear();
            BotsToReproduce.Clear();
            BotsToReproduceSexually.Clear();
            TotalEnergy = 0;
            totwalls = 0;
            totcorpse = 0;
            TotalRobotsDisplayed = TotalRobots;
            TotalRobots = 0;
            totnvegsDisplayed = totnvegs;
            totnvegs = 0;
            totvegsDisplayed = totvegs;
            totvegs = 0;

            if (Teleporters.Any())
            {
                // Need to do this first as NetForces can update bots later in the loop
                foreach (var rob in rob.Where(r => r.exist && !(r.FName == "Base.txt" && hidepred)))
                    CheckTeleporters(rob);
            }

            //Only calculate mass due to fuild displacement if the sim medium has density.
            if (SimOpts.Density != 0)
            {
                foreach (var rob in rob.Where(r => r.exist && !(r.FName == "Base.txt" && hidepred)))
                    AddedMass(rob);
            }

            if (TmpOpts.Tides == 0)
            {
                BouyancyScaling = 1;
            }
            else
            {
                BouyancyScaling = (1 + Math.Sin((SimOpts.TotRunCycle + TmpOpts.TidesOf) % TmpOpts.Tides / SimOpts.Tides * Math.PI * 2)) / 2;
                BouyancyScaling = Math.Sqrt(BouyancyScaling);
                SimOpts.Ygravity = (1 - BouyancyScaling) * 4;
                SimOpts.PhysBrown = BouyancyScaling > 0.8 ? 10 : 0;
            }

            //this loops is for pre update
            foreach (var rob in rob.Where(r => r.exist && !(r.FName == "Base.txt" && hidepred)))
            {
                if (!rob.Corpse)
                    Upkeep(rob); // No upkeep costs if you are dead!

                if (!rob.Corpse && !rob.DisableDNA)
                    Poisons(rob);

                if (!SimOpts.DisableFixing)
                    ManageFixed(rob);

                CalcMass(rob);

                if (Obstacles.Count > 0)
                    DoObstacleCollisions(rob);

                BorderCollision(rob);

                TieHooke(rob); // Handles tie lengths, tie hardening and compressive, elastic tie forces

                if (!rob.Corpse && !rob.DisableDNA)
                    TieTorque(rob);

                if (!rob.Fixed)
                    NetForces(rob); //calculate forces on all robots

                Globals.BucketManager.BucketsCollision(rob);

                if (rob.ImpulseStatic > 0 & (rob.ImpulseInd.X != 0 || rob.ImpulseInd.Y != 0))
                {
                    double staticV;
                    if (rob.vel.X == 0 & rob.vel.Y == 0)
                        staticV = rob.ImpulseStatic;
                    else
                        staticV = rob.ImpulseStatic * Math.Abs(Cross(rob.vel.Unit(), rob.ImpulseInd.Unit())); // Takes into account the fact that the robot may be moving along the same vector

                    if (staticV > rob.ImpulseInd.Magnitude())
                        rob.ImpulseInd = new DoubleVector(0, 0); // If static vector is greater then impulse vector, reset impulse vector
                }

                rob.ImpulseInd -= rob.ImpulseRes;

                if (!rob.Corpse && !rob.DisableDNA)
                {
                    Ties.TiePortCommunication(rob); //transfer data through ties
                    ReadTie(rob); //reads all of the tref variables from a given tie number
                }
            }

            foreach (var s in SimOpts.Specie)
                s.population = 0;

            foreach (var rob in rob.Where(r => r.exist && !(r.FName == "Base.txt" && hidepred)))
                await UpdateCounters(rob); // Counts the number of bots and decays body...

            foreach (var rob in rob.Where(r => r.exist && !(r.FName == "Base.txt" && hidepred)))
            {
                UpdateTies(rob); // Carries all tie routines

                //EricL Transfer genetic meomory locations for newborns through the birth tie during their first 15 cycles
                if (rob.age < 15)
                    DoGeneticMemory(rob);

                if (!rob.Corpse && !rob.DisableDNA)
                {
                    SetAimFunc(rob); //Setup aiming
                    BotDNAManipulation(rob);
                }

                UpdatePosition(rob); //updates robot's position
                                     //EricL 4/9/2006 Got rid of a loop below by moving these inside this loop.  Should speed things up a little.

                rob.nrg = Math.Clamp(rob.nrg, -32000, 32000);
                rob.poison = Math.Clamp(rob.poison, -32000, 32000);
                rob.venom = Math.Clamp(rob.venom, -32000, 32000);
                rob.Waste = Math.Clamp(rob.Waste, -32000, 32000);
            }

            foreach (var rob in rob.Where(r => (r.chloroplasts < r.body / 2 || r.Kills > 5) && r.exist && r.body > bodyfix))
                await KillRobot(rob);

            foreach (var rob in rob)
            {
                UpdateTieAngles(rob); // Updates .tielen and .tieang.  Have to do this here after all bot movement happens above.

                if (!rob.Corpse && !rob.DisableDNA && rob.exist && !(rob.FName == "Base.txt" & hidepred))
                {
                    Mutate(rob);
                    MakeStuff(rob);
                    HandleWaste(rob);
                    Shooting(rob);
                    if (!rob.NoChlr)
                    {
                        ManageChlr(rob); //Botsareus 3/28/2014 Disable Chloroplasts
                    }
                    ManageBody(rob);
                    ManageBouyancy(rob);
                    ManageReproduction(rob);
                    Shock(rob);
                    WriteSenses(rob);
                    FireTies(rob);
                }
                if (!rob.Corpse && rob.exist && !(rob.FName == "Base.txt" && hidepred))
                {
                    Ageing(rob); // Even bots with disabled DNA age...
                    ManageDeath(rob); // Even bots with disabled DNA can die...
                }
                if (rob.exist)
                {
                    TotalSimEnergy[CurrentEnergyCycle] = (int)(TotalSimEnergy[CurrentEnergyCycle] + rob.nrg + rob.body * 10);
                }
            }

            await ReproduceAndKill();
            RemoveExtinctSpecies();
        }

        public static void UpdatePosition(robot rob)
        {
            //Following line commented since mass is set earlier in CalcMass
            if (rob.mass + rob.AddedMass < 0.25)
                rob.mass = 0.25 - rob.AddedMass; // a fudge since Euler approximation can't handle it when mass -> 0

            double vt = 0;

            if (!rob.Fixed)
            {
                // speed normalization
                rob.vel += rob.ImpulseInd * (1 / (rob.mass + rob.AddedMass));

                vt = rob.vel.MagnitudeSquare();
                if (vt > SimOpts.MaxVelocity * SimOpts.MaxVelocity)
                {
                    rob.vel = rob.vel.Unit() * SimOpts.MaxVelocity;
                }

                rob.pos += rob.vel;
                Globals.BucketManager.UpdateBotBucket(rob);
            }
            else
                rob.vel = new DoubleVector(0, 0);

            //Have to do these here for both fixed and unfixed bots to avoid build up of forces in case fixed bots become unfixed.
            rob.ImpulseInd = new DoubleVector(0, 0);
            rob.ImpulseRes = rob.ImpulseInd;
            rob.ImpulseStatic = 0;

            if (SimOpts.ZeroMomentum == true)
                rob.vel = new DoubleVector(0, 0);

            rob.lastup = rob.mem[dirup];
            rob.lastdown = rob.mem[dirdn];
            rob.lastleft = rob.mem[dirsx];
            rob.lastright = rob.mem[dirdx];
            rob.mem[dirup] = 0;
            rob.mem[dirdn] = 0;
            rob.mem[dirdx] = 0;
            rob.mem[dirsx] = 0;

            rob.mem[velscalar] = (int)Math.Clamp(Math.Sqrt(vt), -32000, 32000);
            rob.mem[vel] = (int)Math.Clamp(Math.Cos(rob.aim) * rob.vel.X + Math.Sin(rob.aim) * rob.vel.Y * -1, -32000, 32000);
            rob.mem[veldn] = rob.mem[vel] * -1;
            rob.mem[veldx] = (int)Math.Clamp(Math.Sin(rob.aim) * rob.vel.X + Math.Cos(rob.aim) * rob.vel.Y, -32000, 32000);
            rob.mem[velsx] = rob.mem[veldx] * -1;

            rob.mem[masssys] = (int)rob.mass;
            rob.mem[maxvelsys] = (int)SimOpts.MaxVelocity;
        }

        private static void Ageing(robot rob)
        {
            //aging
            rob.age++;
            rob.newage++; //Age this simulation to be used by tie code
            var tempAge = rob.age;
            if (tempAge > 32000)
                tempAge = 32000;

            rob.mem[robage] = tempAge; //line added to copy robots age into a memory location
            rob.mem[timersys] += 1; //update epigenetic timer

            if (rob.mem[timersys] > 32000)
                rob.mem[timersys] = -32000;
        }

        private static void Altzheimer(robot rob)
        {
            //makes robots with high waste act in a bizarre fashion.
            var loops = (rob.Pwaste + rob.Waste - SimOpts.BadWastelevel) / 4;

            for (var t = 0; t < loops; t++)
            {
                int loc;
                do
                {
                    loc = ThreadSafeRandom.Local.Next(1, 1000);
                } while (!(loc != mkchlr && loc != rmchlr));
                var val = ThreadSafeRandom.Local.Next(-32000, 32000);
                rob.mem[loc] = val;
            }
        }

        private static void BotDNAManipulation(robot rob)
        {
            //count down
            if (rob.Vtimer > 1)
                rob.Vtimer--;

            rob.mem[Vtimer] = rob.Vtimer;

            //Viruses
            if (rob.mem[mkvirus] > 0 & rob.Vtimer == 0)
            {
                //Botsareus 9/30/2014 Chloroplasts and viruses do not mix very well anymore
                if (rob.chloroplasts == 0)
                {
                    //make the virus
                    if (MakeVirus(rob, rob.mem[mkvirus]))
                    {
                        var length = GeneLength(rob, rob.mem[mkvirus]) * 2;
                        rob.nrg -= length / 2 * SimOpts.Costs.DnaCopyCost * SimOpts.Costs.CostMultiplier;
                        rob.Vtimer = Math.Min(length, 32000);
                    }
                    else
                    {
                        rob.Vtimer = 0;
                        rob.virusshot = null;
                    }
                }
                else
                {
                    rob.chloroplasts = 0;
                    rob.radius = FindRadius(rob);
                }
            }

            //shoot it!
            if (rob.mem[VshootSys] != 0 & rob.Vtimer == 1)
            {
                // Botsareus 10/5/2015 Bugfix for negative values in vshoot
                ShootVirus(rob, rob.virusshot);

                rob.mem[VshootSys] = 0;
                rob.mem[Vtimer] = 0;
                rob.mem[mkvirus] = 0;
                rob.Vtimer = 0;
                rob.virusshot = null;
            }

            //Other stuff

            if (rob.mem[DelgeneSys] > 0)
            {
                DeleteGene(rob, rob.mem[DelgeneSys]);
                rob.mem[DelgeneSys] = 0;
            }

            rob.mem[DnaLenSys] = rob.dna.Count;
            rob.mem[GenesSys] = rob.genenum;
        }

        private static void ChangeChlr(robot rob)
        {
            var tmpchlr = rob.chloroplasts;

            //add chloroplasts
            rob.chloroplasts += rob.mem[mkchlr];

            //remove chloroplasts
            rob.chloroplasts -= rob.mem[rmchlr];

            if (tmpchlr < rob.chloroplasts)
            {
                var newnrg = rob.nrg - (rob.chloroplasts - tmpchlr) * SimOpts.Costs.CholorplastCost * SimOpts.Costs.CostMultiplier;

                if ((TotalChlr > SimOpts.MaxPopulation && rob.Veg == true) || newnrg < 100)
                    rob.chloroplasts = tmpchlr;
                else
                    rob.nrg = newnrg; //Botsareus 8/24/2013 only charge energy for adding chloroplasts to prevent robots from cheating by adding and subtracting there chlroplasts in 3 cycles
            }

            rob.mem[mkchlr] = 0;
            rob.mem[rmchlr] = 0;
        }

        private static List<DNABlock> Crossover(List<block2> dna1, List<block2> dna2)
        {
            var nn = 0;
            var res1 = 0;//result1
            var res2 = 0;
            var resn = 0;
            var i = 0;//layer

            while (true)
            {
                //diff search

                var n1 = res1 + resn - nn;//start pos
                var n2 = res2 + resn - nn;

                //presets

                var outDna = new List<DNABlock>();

                var temp = 0;
                res1 = ScanFromN(dna1, n1, ref temp);
                res2 = ScanFromN(dna2, n2, ref i);

                if (res1 - n1 > 0 & res2 - n2 > 0)
                {
                    if (ThreadSafeRandom.Local.Next(0, 2) == 0)
                    {
                        for (var a = n1; a < res1 - 1; a++)
                            outDna.Add(new DNABlock { tipo = dna1[a].tipo, value = dna1[a].value });
                    }
                    else
                    {
                        for (var a = n2; a < res2 - 1; a++)
                            outDna.Add(new DNABlock { tipo = dna2[a].tipo, value = dna2[a].value });
                    }
                }
                else if (res1 - n1 > 0)
                {
                    if (ThreadSafeRandom.Local.Next(0, 2) == 0)
                    {
                        for (var a = n1; a < res1 - 1; a++)
                            outDna.Add(new DNABlock { tipo = dna1[a].tipo, value = dna1[a].value });
                    }
                }
                else if (res2 - n2 > 0)
                {
                    if (ThreadSafeRandom.Local.Next(0, 2) == 0)
                    {
                        for (var a = n2; a < res2 - 1; a++)
                            outDna.Add(new DNABlock { tipo = dna2[a].tipo, value = dna2[a].value });
                    }
                }

                //same search

                if (i == 0)
                    return outDna;

                nn = res1;
                resn = ScanFromN(dna1, nn, ref i);

                var whatside = ThreadSafeRandom.Local.Next(0, 2) == 0;

                for (var a = nn; a < resn - 1; a++)
                {
                    var block = new DNABlock
                    {
                        tipo = whatside ? dna1[a].tipo : dna2[a - nn + res2].tipo,
                        value = ((dna1[a].tipo == dna2[a - nn + res2].tipo && Math.Abs(dna2[a].value) > 999 && Math.Abs(dna2[a - nn + res2].value) > 999 ? ThreadSafeRandom.Local.Next(0, 2) == 0 : whatside) ? dna1[a].value : dna2[a - nn + res2].value)
                    };
                    outDna.Add(block);
                }
            }
        }

        private static void DeleteSpecies(Species species)
        {
            SimOpts.Specie.Remove(species);
        }

        private static void FeedBody(robot rob)
        {
            if (rob.mem[fdbody] > 100)
                rob.mem[fdbody] = 100;

            rob.nrg += rob.mem[fdbody];
            rob.body -= (double)rob.mem[fdbody] / 10;

            if (rob.nrg > 32000)
                rob.nrg = 32000;

            rob.radius = FindRadius(rob);
            rob.mem[fdbody] = 0;
        }

        private static void FireTies(robot rob)
        {
            var resetlastopp = false;

            if (rob.lastopp == null & rob.age < 2 && rob.parent != null && rob.parent.exist)
            {
                rob.lastopp = rob.parent;
                resetlastopp = true;
            }

            //Botsareus 11/26/2013 The tie by touch code
            if (rob.lastopp == null & rob.lasttch != null && rob.lasttch.exist)
            {
                rob.lastopp = rob.lasttch;
                resetlastopp = true;
            }

            if (rob.mem[mtie] != 0 && rob.lastopp != null & !SimOpts.DisableTies && (rob.lastopp is robot lastOpp))
            {
                //2 robot lengths
                var length = (lastOpp.pos - rob.pos).Magnitude();
                var maxLength = RobSize * 4 + rob.radius + lastOpp.radius;

                if (length <= maxLength)
                {
                    MakeTie(rob, lastOpp, (int)(rob.radius + lastOpp.radius + RobSize * 2), -20, rob.mem[mtie]);
                }
                rob.mem[mtie] = 0;
            }

            if (resetlastopp)
                rob.lastopp = null;
        }

        private static int GeneLength(robot rob, int p)
        {
            var pos = genepos(rob.dna, p);
            return GeneEnd(rob.dna, pos) - pos + 1;
        }

        private static double GeneticDistance(List<block3> rob1, List<block3> rob2)
        {
            return rob1.Count(b => b.match == 0) + rob2.Count(b => b.match == 0) / (rob1.Count + rob2.Count);
        }

        private static void HandleWaste(robot rob)
        {
            if (rob.Waste > 0 && rob.chloroplasts > 0)
                feedveg2(rob);

            if (SimOpts.BadWastelevel == 0)
                SimOpts.BadWastelevel = 400;

            if (SimOpts.BadWastelevel > 0 & rob.Pwaste + rob.Waste > SimOpts.BadWastelevel)
                Altzheimer(rob);

            if (rob.Waste > 32000)
                Defacate(rob);

            if (rob.Pwaste > 32000)
                rob.Pwaste = 32000;

            if (rob.Waste < 0)
                rob.Waste = 0;

            rob.mem[828] = (int)rob.Waste;
            rob.mem[829] = (int)rob.Pwaste;
        }

        private static void MakeShell(robot rob)
        {
            const double shellNrgConvRate = 0.1; // Make 10 shell for 1 nrg

            StoreResource(rob, 822, 823, shellNrgConvRate, SimOpts.Costs.ShellCost, r => r.shell, (r, s) => r.shell = s, true);
        }

        private static void MakeSlime(robot rob)
        {
            const double slimeNrgConvRate = 0.1; // Make 10 slime for 1 nrg

            StoreResource(rob, 820, 821, slimeNrgConvRate, SimOpts.Costs.SlimeCost, r => r.Slime, (r, s) => r.Slime = s, true);
        }

        private static void MakeStuff(robot rob)
        {
            if (rob.mem[824] != 0)
                StoreVenom(rob);

            if (rob.mem[826] != 0)
                StorePoison(rob);

            if (rob.mem[822] != 0)
                MakeShell(rob);

            if (rob.mem[820] != 0)
                MakeSlime(rob);
        }

        private static void ManageBody(robot rob)
        {
            if (rob.mem[strbody] > 0)
                StoreBody(rob);

            if (rob.mem[fdbody] > 0)
                FeedBody(rob);

            rob.body = Math.Clamp(rob.body, 0, 32000);

            rob.mem[body] = (int)rob.body;
        }

        private static void ManageBouyancy(robot rob)
        {
            if (rob.mem[setboy] == 0)
                return;

            rob.Bouyancy += (double)rob.mem[setboy] / 32000;
            rob.Bouyancy = Math.Clamp(rob.Bouyancy, 0, 1);

            rob.mem[rdboy] = (int)(rob.Bouyancy * 32000);
            rob.mem[setboy] = 0;
        }

        private static void ManageChlr(robot rob)
        {
            if (rob.mem[mkchlr] > 0 || rob.mem[rmchlr] > 0)
                ChangeChlr(rob);

            rob.chloroplasts -= 0.5 / Math.Pow(100, rob.chloroplasts / 16000);

            rob.chloroplasts = Math.Clamp(rob.chloroplasts, 0, 32000);

            rob.mem[chlr] = (int)rob.chloroplasts;
            rob.mem[light] = (int)(32000 - (LightAval * 32000));

            rob.radius = FindRadius(rob);
        }

        private static void ManageDeath(robot rob)
        {
            if (SimOpts.CorpseEnabled)
            {
                if (!rob.Corpse && rob.nrg < 15 && rob.age > 0)
                {
                    rob.Corpse = true;
                    rob.FName = "Corpse";
                    Array.Clear(rob.occurr, 0, rob.occurr.Length);
                    rob.color = Colors.White;
                    rob.Veg = false;
                    rob.Fixed = false;
                    rob.nrg = 0;
                    rob.DisableDNA = true;
                    rob.DisableMovementSysvars = true;
                    rob.CantSee = true;
                    rob.VirusImmune = true;
                    rob.chloroplasts = 0;

                    for (var i = EyeStart + 1; i < EyeEnd; i++)
                        rob.mem[i] = 0;

                    rob.Bouyancy = 0;
                }
                if (rob.body < 0.5)
                    rob.Dead = true;
            }
            else if (rob.nrg < 0.5 || rob.body < 0.5)
                rob.Dead = true;

            if (rob.Dead)
                BotsToKill.Add(rob);
        }

        private static void ManageFixed(robot rob)
        {
            rob.Fixed = rob.mem[216] > 0;
        }

        private static void ManageReproduction(robot rob)
        {
            //Decrement the fertilization counter
            if (rob.fertilized >= 0)
            {
                // This is >= 0 so that we decrement it to -1 the cycle after the last birth is possible
                rob.fertilized--;
                rob.mem[SYSFERTILIZED] = rob.fertilized >= 0 ? rob.fertilized : 0;
            }
            else
            {
                //new code here to block sex repro
                if (rob.fertilized < -10)
                {
                    rob.fertilized++;
                }
                else
                {
                    if (rob.fertilized == -1)
                        rob.spermDNA.Clear();
                    rob.fertilized = -2; //This is so we don't keep reDiming every time through
                }
            }

            //Asexual reproduction
            if ((rob.mem[Repro] > 0 || rob.mem[mrepro] > 0) && !rob.CantReproduce)
                BotsToReproduce.Add(rob);

            //Sexual Reproduction
            if (rob.mem[SEXREPRO] > 0 & rob.fertilized >= 0 & !rob.CantReproduce)
                BotsToReproduceSexually.Add(rob);
        }

        private static void Poisons(robot rob)
        {
            if (rob.Paralyzed)
                rob.mem[rob.Vloc] = rob.Vval;

            if (rob.Paralyzed)
            {
                rob.Paracount--;

                if (rob.Paracount < 1)
                {
                    rob.Paralyzed = false;
                    rob.Vloc = 0;
                    rob.Vval = 0;
                }
            }

            rob.mem[837] = (int)rob.Paracount;

            if (rob.Poisoned)
                rob.mem[rob.Ploc] = rob.Pval;

            if (rob.Poisoned)
            {
                rob.Poisoncount--;
                if (rob.Poisoncount < 1)
                {
                    rob.Poisoned = false;
                    rob.Ploc = 0;
                    rob.Pval = 0;
                }
            }

            rob.mem[838] = (int)rob.Poisoncount;
        }

        private static void RemoveExtinctSpecies()
        {
            foreach (var s in SimOpts.Specie.Where(s => s.population == 0 && !s.Native).ToArray())
                DeleteSpecies(s);
        }

        private static async Task ReproduceAndKill()
        {
            foreach (var r in BotsToReproduce)
            {
                var per = 0;

                if (r.mem[mrepro] > 0 & r.mem[Repro] > 0)
                {
                    if (ThreadSafeRandom.Local.NextDouble() > 0.5)
                    {
                        per = r.mem[Repro];
                    }
                    else
                    {
                        per = r.mem[mrepro];
                    }
                }
                else
                {
                    if (r.mem[mrepro] > 0)
                    {
                        per = r.mem[mrepro];
                    }
                    if (r.mem[Repro] > 0)
                    {
                        per = r.mem[Repro];
                    }
                }

                Reproduce(r, per);
            }

            foreach (var r in BotsToReproduceSexually)
            {
                SexReproduce(r);
            }

            foreach (var r in BotsToKill)
                await KillRobot(r);
        }

        private static int ScanFromN(IList<block2> rob, int n, ref int layer)
        {
            for (var a = n; a < rob.Count; a++)
            {
                if (rob[a].match != layer)
                {
                    layer = rob[a].match;
                    return a;
                }
            }
            return rob.Count;
        }

        private static double SetAimFunc(robot rob)
        {
            double SetAimFunc;
            double diff2 = 0;
            double diff = rob.mem[aimsx] - rob.mem[aimdx];

            if (rob.mem[SetAim] == Math.Round(rob.aim * 200, 0))
            {
                SetAimFunc = rob.aim * 200 + diff;
            }
            else
            {
                // .setaim overrides .aimsx and .aimdx
                SetAimFunc = rob.mem[SetAim]; // this is where .aim needs to be
                diff = -AngDiff(rob.aim, NormaliseAngle((double)rob.mem[SetAim] / 200)) * 200; // this is the diff to get there 'Botsareus 6/18/2016 Added angnorm
                diff2 = Math.Abs(Math.Round((rob.aim * 200 - rob.mem[SetAim]) / 1256, 0) * 1256) * Math.Sign(diff); // this is how much we add to momentum
            }

            //diff + diff2 is now the amount, positive or negative to turn.
            rob.nrg -= Math.Abs(Math.Round((diff + diff2) / 200, 3) * SimOpts.Costs.RotationCost * SimOpts.Costs.CostMultiplier);

            SetAimFunc %= (1256);
            if (SetAimFunc < 0)
                SetAimFunc += 1256;

            SetAimFunc /= 200;

            //Overflow Protection
            while (rob.ma > 2 * Math.PI)
                rob.ma -= 2 * Math.PI;

            while (rob.ma < -2 * Math.PI)
                rob.ma += 2 * Math.PI;

            rob.aim = SetAimFunc + rob.ma; // Add in the angular momentum

            //Voluntary rotation can reduce angular momentum but does not add to it.

            if (rob.ma > 0 & diff < 0)
            {
                rob.ma += (diff + diff2) / 200;
                if (rob.ma < 0)
                    rob.ma = 0;
            }
            if (rob.ma < 0 & diff > 0)
            {
                rob.ma += (diff + diff2) / 200;
                if (rob.ma > 0)
                    rob.ma = 0;
            }

            rob.aimvector = new DoubleVector(Math.Cos(rob.aim), Math.Sin(rob.aim));

            rob.mem[aimsx] = 0;
            rob.mem[aimdx] = 0;
            rob.mem[AimSys] = (int)(rob.aim * 200);
            rob.mem[SetAim] = rob.mem[AimSys];
            return SetAimFunc;
        }

        private static void ShareResource(robot rob, Tie tie, int address, Func<robot, double> getValue, Action<robot, double> setValue)
        {
            if (rob.mem[address] > 99)
                rob.mem[address] = 99;
            if (rob.mem[address] < 0)
                rob.mem[address] = 0;

            var total = getValue(rob) + getValue(tie.OtherBot);

            setValue(tie.OtherBot, Math.Min(total * ((100 - (double)rob.mem[address]) / 100), 32000));
            setValue(rob, Math.Min(total * ((double)rob.mem[address] / 100), 32000));

            rob.mem[address] = (int)getValue(rob); // update the .shell sysvar
            tie.OtherBot.mem[address] = (int)getValue(tie.OtherBot);
        }

        private static void Shock(robot rob)
        {
            if (rob.Veg || rob.nrg <= 3000)
                return;

            var temp = rob.onrg - rob.nrg;
            if (temp > (rob.onrg / 2))
            {
                rob.nrg = 0;
                rob.body += rob.nrg / 10;

                if (rob.body > 32000)
                    rob.body = 32000;

                rob.radius = FindRadius(rob);
            }
        }

        private static void Shoot(robot rob)
        {
            var valmode = false;
            double energyLost;

            if (rob.nrg <= 0)
                return;

            var shtype = rob.mem[shoot];
            double value = rob.mem[shootval];
            var multiplier = 0.0;
            var rngmultiplier = 0.0;
            var cost = 0.0;

            if (shtype == -1 || shtype == -6)
            {
                if (value < 0)
                {
                    // negative values of .shootval impact shot range?
                    multiplier = 1; // no impact on shot power
                    rngmultiplier = -value; // set the range multplier equal to .shootval
                }

                if (value > 0)
                {
                    // postive values of .shootval impact shot power?
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
                    cost = rngmultiplier * SimOpts.Costs.ShotFormationCost * SimOpts.Costs.CostMultiplier;
                    rngmultiplier = Math.Log(rngmultiplier / 2, 2);
                }
                else if (valmode == false)
                {
                    rngmultiplier = 1;
                    cost = SimOpts.Costs.ShotFormationCost * SimOpts.Costs.CostMultiplier / (rob.Ties.Count + 1);
                }

                if (multiplier > 4)
                {
                    cost = multiplier * SimOpts.Costs.ShotFormationCost * SimOpts.Costs.CostMultiplier;
                    multiplier = Math.Log(multiplier / 2, 2);
                }
                else if (valmode == true)
                {
                    multiplier = 1;
                    cost = SimOpts.Costs.ShotFormationCost * SimOpts.Costs.CostMultiplier / (rob.Ties.Count + 1); //Botsareus 6/12/2014 Bug fix
                }

                if (cost > rob.nrg && cost > 2 && rob.nrg > 2 && valmode)
                {
                    cost = rob.nrg;
                    multiplier = Math.Log(rob.nrg / (SimOpts.Costs.ShotFormationCost * SimOpts.Costs.CostMultiplier), 2);
                }

                if (cost > rob.nrg && cost > 2 && rob.nrg > 2 && !valmode)
                {
                    cost = rob.nrg;
                    rngmultiplier = Math.Log(rob.nrg / (SimOpts.Costs.ShotFormationCost * SimOpts.Costs.CostMultiplier), 2);
                }
            }

            //'''''''''''''''''''''''''''''''''''''''''''''
            //'''''''''''''''''''''''''''''''''''''''''''''
            //'''''''''''''''''''''''''''''''''''''''''''''

            switch (shtype)
            {
                case > 0:
                    shtype %= MaxMem;
                    cost = SimOpts.Costs.ShotFormationCost * SimOpts.Costs.CostMultiplier;
                    if (rob.nrg < cost)
                        cost = rob.nrg;

                    rob.nrg -= cost; // EricL - postive shots should cost the shotcost
                    NewShot(rob, shtype, value, 1, true);

                    break;// Nrg request Feeding Shot
                case -1:
                    if (rob.Multibot)
                        value = 20 + (rob.body / 5) * rob.Ties.Count; //Botsareus 6/22/2016 Bugfix
                    else
                        value = 20 + (rob.body / 5);

                    value *= multiplier;
                    if (rob.nrg < cost)
                        cost = rob.nrg;

                    rob.nrg -= cost;
                    NewShot(rob, shtype, value, rngmultiplier, true);
                    break;// Nrg shot
                case -2:
                    value = Math.Abs(value);
                    if (rob.nrg < value)
                        value = rob.nrg;

                    if (value == 0)
                        value = rob.nrg / 100; //default energy shot.  Very small.

                    energyLost = value + SimOpts.Costs.ShotFormationCost * SimOpts.Costs.CostMultiplier / (rob.Ties.Count + 1);
                    if (energyLost > rob.nrg)
                        rob.nrg = 0;
                    else
                        rob.nrg -= energyLost;

                    NewShot(rob, shtype, value, 1, true);
                    break;//shoot venom
                case -3:
                    value = Math.Abs(value);
                    if (value > rob.venom)
                        value = rob.venom;

                    if (value == 0)
                        value = rob.venom / 20; //default venom shot.  Not too small.

                    rob.venom -= value;
                    rob.mem[825] = (int)rob.venom;
                    energyLost = SimOpts.Costs.ShotFormationCost * SimOpts.Costs.CostMultiplier / (rob.Ties.Count + 1);

                    rob.nrg = energyLost > rob.nrg ? 0 : rob.nrg - energyLost;

                    NewShot(rob, shtype, value, 1, true);
                    break;//shoot waste 'Botsareus 4/22/2016 Bugfix
                case -4:
                    value = Math.Abs(value);
                    if (value == 0)
                        value = rob.Waste / 20; //default waste shot. 'Botsareus 10/5/2015 Fix for waste

                    if (value > rob.Waste)
                        value = rob.Waste;

                    rob.Waste -= value * 0.99; //Botsareus 10/5/2015 Fix for waste
                    rob.Pwaste += value / 100;
                    energyLost = SimOpts.Costs.ShotFormationCost * SimOpts.Costs.CostMultiplier / (rob.Ties.Count + 1);
                    rob.nrg = energyLost > rob.nrg ? 0 : rob.nrg - energyLost;

                    NewShot(rob, shtype, value, 1, true);
                    // no -5 shot here as poison can only be shot in response to an attack
                    break;//shoot body
                case -6:
                    if (rob.Multibot)
                        value = 10 + rob.body / 2 * (rob.Ties.Count + 1);
                    else
                        value = 10 + Math.Abs(rob.body) / 2;

                    if (rob.nrg < cost)
                        cost = rob.nrg;

                    rob.nrg -= cost;
                    value *= multiplier;
                    NewShot(rob, shtype, value, rngmultiplier, true);
                    break;// shoot sperm
                case -8:
                    cost = SimOpts.Costs.ShotFormationCost * SimOpts.Costs.CostMultiplier;

                    if (rob.nrg < cost)
                        cost = rob.nrg;

                    rob.nrg -= cost; // EricL - postive shots should cost the shotcost
                    NewShot(rob, shtype, value, 1, true);
                    break;
            }
            rob.mem[shoot] = 0;
            rob.mem[shootval] = 0;
        }

        private static void Shooting(robot rob)
        {
            if (rob.mem[shoot] != 0)
                Shoot(rob);

            rob.mem[shoot] = 0;
        }

        private static void SimpleMatch(List<block3> r1, List<block3> r2)
        {
            var ei1 = r1.Count;
            var ei2 = r2.Count;
            var matchlist1 = new List<int>();
            var matchlist2 = new List<int>();
            var loopr1 = 0;
            var loopr2 = 0;
            var laststartmatch1 = 0;
            var laststartmatch2 = 0;
            var patch = 0;//Botsareus 4/18/2016 Temporary fix to prevent infinate loop

            do
            {
                //keep building until both sides max out
                if (loopr1 >= ei1)
                    loopr1 = ei1 - 1;

                if (loopr2 >= ei2)
                    loopr2 = ei2 - 1;

                matchlist1.Add(r1[loopr1].nucli);
                matchlist2.Add(r2[loopr2].nucli);

                //does anything match
                var matchr2 = false;

                var match = false;

                int loopold;

                for (loopold = 0; loopold < matchlist1.Count; loopold++)
                {
                    if (r2[loopr2].nucli == matchlist1[loopold])
                    {
                        matchr2 = true;
                        match = true;
                        break;
                    }
                    if (r1[loopr1].nucli == matchlist2[loopold])
                    {
                        matchr2 = false;
                        match = true;
                        break;
                    }
                    patch++;
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

                    var inc = 0;
                    var newmatch = false;

                    do
                    {
                        if (r2[loopr2].nucli == r1[loopr1].nucli)
                        {
                            if (newmatch == false)
                                inc++;

                            newmatch = true;
                            r1[loopr1].match = inc;
                            r2[loopr2].match = inc;
                        }
                        else
                        {
                            laststartmatch1 = loopr1;
                            laststartmatch2 = loopr2;
                            loopr1--;
                            loopr2--;
                            break;
                        }
                        loopr1++;
                        loopr2++;
                        patch++;
                    } while (loopr1 <= ei1 && loopr2 <= ei2);
                }

                loopr1++;
                loopr2++;
                patch++;
            } while (loopr1 <= ei1 || loopr2 <= ei2 || patch > (16000 ^ 2));
        }

        private static void StoreBody(robot rob)
        {
            if (rob.mem[strbody] > 100)
                rob.mem[strbody] = 100;

            rob.nrg -= rob.mem[strbody];
            rob.body += (double)rob.mem[strbody] / 10;

            if (rob.body > 32000)
                rob.body = 32000;

            rob.radius = FindRadius(rob);
            rob.mem[strbody] = 0;
        }

        private static void StoreResource(robot rob, int storeAddress, int levelAddress, double conversionRate, double resourceCost, Func<robot, double> getValue, Action<robot, double> setValue, bool multiBotDiscount = false)
        {
            if (rob.nrg <= 0)
                return;

            double delta = Math.Clamp(rob.mem[storeAddress], -32000, 32000);
            delta = Math.Clamp(delta, -rob.nrg / conversionRate, rob.nrg / conversionRate);
            delta = Math.Clamp(delta, -100, 100);

            if (getValue(rob) + delta > 32000)
                delta = 32000 - getValue(rob);

            if (getValue(rob) + delta < 0)
                delta = -getValue(rob);

            setValue(rob, getValue(rob) + delta);

            if (rob.Multibot && multiBotDiscount)
                rob.nrg -= Math.Abs(delta) * conversionRate / (rob.Ties.Count + 1);
            else
                rob.nrg -= Math.Abs(delta) * conversionRate;

            //This is the transaction cost
            var cost = Math.Abs(delta) * resourceCost * SimOpts.Costs.CostMultiplier;

            rob.nrg -= cost;
            rob.Waste += cost;

            rob.mem[storeAddress] = 0;
            rob.mem[levelAddress] = (int)getValue(rob);
        }

        private static async Task UpdateCounters(robot rob)
        {
            TotalRobots++;

            var species = SimOpts.Specie.FirstOrDefault(s => s.Name == rob.FName);

            //If no species structure for the bot, then create one
            if (!rob.Corpse)
            {
                if (species == null)
                    AddSpecie(rob, false);
                else
                {
                    species.population++;
                    species.population = Math.Min(species.population, 32000);
                }
            }

            if (rob.Veg)
                totvegs++;
            else if (rob.Corpse)
            {
                totcorpse++;
                if (rob.body > 0)
                    Decay(rob);
                else
                    await KillRobot(rob);
            }
            else
                totnvegs++;
        }

        private static void Upkeep(robot rob)
        {
            double Cost;

            //Age Cost
            var ageDelta = rob.age - SimOpts.Costs.AgeCostBeginAge;

            if (ageDelta > 0 & rob.age > 0)
            {
                if (SimOpts.Costs.EnableAgeCostIncreaseLog)
                    Cost = SimOpts.Costs.AgeCost * Math.Log(ageDelta);
                else if (SimOpts.Costs.EnableAgeCostIncreasePerCycle)
                    Cost = SimOpts.Costs.AgeCost + (ageDelta * SimOpts.Costs.AgeCostIncreasePerCycle);
                else
                    Cost = SimOpts.Costs.AgeCost;

                rob.nrg -= Cost * SimOpts.Costs.CostMultiplier;
            }

            //BODY UPKEEP
            Cost = rob.body * SimOpts.Costs.BodyUpkeepCost * SimOpts.Costs.CostMultiplier;
            rob.nrg -= Cost;

            //DNA upkeep cost
            Cost = (rob.dna.Count - 1) * SimOpts.Costs.DnaUpkeepCost * SimOpts.Costs.CostMultiplier;
            rob.nrg -= Cost;

            //degrade slime
            rob.Slime *= 0.98;
            if (rob.Slime < 0.5)
                rob.Slime = 0; // To keep things sane for integer rounding, etc.

            rob.mem[821] = (int)rob.Slime;

            //degrade poison
            rob.poison *= 0.98;
            if (rob.poison < 0.5)
                rob.poison = 0; //Botsareus 3/15/2013 bug fix for poison so it does not change slime

            rob.mem[827] = (int)rob.poison;
        }
    }
}
