using DarwinBots.Model;
using DarwinBots.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DarwinBots.Modules
{
    internal static class Globals
    {
        public static bool AutoSaved { get; set; }
        public static BucketManager BucketManager { get; set; }
        public static bool Delta2 { get; set; }
        public static int DeltaDevChance { get; set; }
        public static double DeltaDevExp { get; set; }
        public static double DeltaDevLn { get; set; }
        public static int DeltaMainChance { get; set; }
        public static double DeltaMainExp { get; set; }
        public static double DeltaMainLn { get; set; }
        public static int DeltaPm { get; set; }
        public static int DeltaWtc { get; set; }
        public static bool EpiReset { get; set; }
        public static double EpiResetEmp { get; set; }
        public static int EpiResetOp { get; set; }
        public static bool FudgeAll { get; set; }
        public static bool FudgeEyes { get; set; }
        public static int ModeChangeCycles { get; set; }
        public static bool NormMut { get; set; }
        public static bool ReproFix { get; set; }
        public static bool SimAlreadyRunning { get; set; }
        public static string SimStart { get; set; }
        public static int StartChlr { get; set; }
        public static bool SunBelt { get; set; }
        public static int TotalChlr { get; set; }
        public static int TotalNotVegs { get; set; }
        public static int TotalNotVegsDisplayed { get; set; }
        public static bool UseEpiGene { get; set; }
        public static bool UseSafeMode { get; set; }
        public static int ValMaxNormMut { get; set; }
        public static int ValNormMut { get; set; }
        public static List<Obstacle> xObstacle { get; set; } = new();
        public static bool y_normsize { get; set; }

        public static async Task aggiungirob(int r, double x, double y)
        {
            if (r == -1)
            {
                if (!SimOpt.SimOpts.Specie.Any(CheckVegStatus))
                    return;

                do
                {
                    r = ThreadSafeRandom.Local.Next(0, SimOpt.SimOpts.Specie.Count); // start randomly in the list of species
                } while (!CheckVegStatus(SimOpt.SimOpts.Specie[r]));

                x = ThreadSafeRandom.Local.Next((int)(SimOpt.SimOpts.Specie[r].Poslf * (SimOpt.SimOpts.FieldWidth - 60)), (int)(SimOpt.SimOpts.Specie[r].Posrg * (SimOpt.SimOpts.FieldWidth - 60)));
                y = ThreadSafeRandom.Local.Next((int)(SimOpt.SimOpts.Specie[r].Postp * (SimOpt.SimOpts.FieldHeight - 60)), (int)(SimOpt.SimOpts.Specie[r].Posdn * (SimOpt.SimOpts.FieldHeight - 60)));
            }

            if (SimOpt.SimOpts.Specie[r].Name != "" && SimOpt.SimOpts.Specie[r].path != "Invalid Path")
            {
                var a = await DnaManipulations.RobScriptLoad(System.IO.Path.Join(SimOpt.SimOpts.Specie[r].path, SimOpt.SimOpts.Specie[r].Name));

                if (a == null)
                {
                    SimOpt.SimOpts.Specie[r].Native = false;
                    return;
                }

                //Check to see if we were able to load the bot.  If we can't, the path may be wrong, the sim may have
                //come from another machine with a different install path.  Set the species path to an empty string to
                //prevent endless looping of error dialogs.
                if (!a.exist)
                {
                    SimOpt.SimOpts.Specie[r].path = "Invalid Path";
                    return;
                }

                a.Veg = SimOpt.SimOpts.Specie[r].Veg;
                if (a.Veg)
                    a.chloroplasts = StartChlr;

                a.Fixed = SimOpt.SimOpts.Specie[r].Fixed;
                a.CantSee = SimOpt.SimOpts.Specie[r].CantSee;
                a.DisableDNA = SimOpt.SimOpts.Specie[r].DisableDNA;
                a.DisableMovementSysvars = SimOpt.SimOpts.Specie[r].DisableMovementSysvars;
                a.CantReproduce = SimOpt.SimOpts.Specie[r].CantReproduce;
                a.VirusImmune = SimOpt.SimOpts.Specie[r].VirusImmune;
                a.Corpse = false;
                a.Dead = false;
                a.body = 1000;
                a.radius = Robots.FindRadius(a);
                a.Mutations = 0;
                a.OldMutations = 0;
                a.LastMut = 0;
                a.generation = 0;
                a.SonNumber = 0;
                a.parent = null;
                Array.Clear(a.mem, 0, a.mem.Length);

                if (a.Fixed)
                    a.mem[216] = 1;

                a.pos = new DoubleVector(x, y);

                a.aim = ThreadSafeRandom.Local.NextDouble() * Math.PI * 2;
                a.mem[Robots.SetAim] = (int)a.aim * 200;

                //Bot is already in a bucket due to the prepare routine
                BucketManager.UpdateBotBucket(a);
                a.nrg = SimOpt.SimOpts.Specie[r].Stnrg;
                a.Mutables = SimOpt.SimOpts.Specie[r].Mutables;

                a.Vtimer = 0;
                a.virusshot = null;
                a.genenum = DnaManipulations.CountGenes(a.dna);

                a.GenMut = (double)a.dna.Count / Robots.GeneticSensitivity;

                a.mem[Robots.DnaLenSys] = a.dna.Count;
                a.mem[Robots.GenesSys] = a.genenum;

                a.multibot_time = SimOpt.SimOpts.Specie[r].kill_mb ? 210 : 0;
                a.NoChlr = SimOpt.SimOpts.Specie[r].NoChlr;

                for (var i = 0; i < 7; i++)
                    a.Skin[i] = SimOpt.SimOpts.Specie[r].Skin[i];

                a.color = SimOpt.SimOpts.Specie[r].color;
                Senses.MakeOccurrList(a);
            }
        }

        public static void MakePoff(robot rob)
        {
            for (var t = 1; t < 20; t++)
            {
                var an = 640 / 20 * t;
                var vs = ThreadSafeRandom.Local.Next(Robots.RobSize / 40, Robots.RobSize / 30);
                var vx = (int)(rob.vel.X + Robots.AbsX(an / 100.0, vs, 0, 0, 0));
                var vy = (int)(rob.vel.Y + Robots.AbsY(an / 100.0, vs, 0, 0, 0));
                var x = ThreadSafeRandom.Local.Next((int)(rob.pos.X - rob.radius), (int)(rob.pos.X + rob.radius));
                var y = ThreadSafeRandom.Local.Next((int)(rob.pos.Y - rob.radius), (int)(rob.pos.Y + rob.radius));
                ShotsManager.CreateShot(x, y, vx, vy, -100, null, 0, Robots.RobSize * 2, ThreadSafeRandom.Local.Next(1, 3) == 1 ? rob.color : ShotsManager.DBrite(rob.color));
            }
        }

        private static bool CheckVegStatus(Species species)
        {
            if (!species.Veg || !species.Native)
                return false;

            //see if any active robots have chloroplasts
            if (Robots.rob.Where(r => r.exist && r.chloroplasts > 0)
                .Select(rob => new { rob, splitname = rob.FName.Split(")") })
                .Select(t =>
                    t.splitname[0].StartsWith("(") && int.TryParse(t.splitname[0][1..], out _)
                        ? t.splitname[1]
                        : t.rob.FName).Any(robname => species.Name == robname))
            {
                return true;
            }

            //If there is no robots at all with chlr then repop everything
            return !Robots.rob.Any(r => r.exist && r.Veg && r.age > 0);
        }
    }
}
