using DarwinBots.Model;
using DarwinBots.Support;
using System;
using System.Linq;

namespace DarwinBots.Modules
{
    internal static class Vegs
    {
        public static int CoolDown { get; set; }
        public static double LightAval { get; private set; }
        public static int TotalVegs { get; set; }

        public static void feedveg2(Robot rob)
        {
            var energy = rob.Chloroplasts / 64000 * (1 - SimOpt.SimOpts.VegFeedingToBody);
            var body = (rob.Chloroplasts / 64000 * SimOpt.SimOpts.VegFeedingToBody) / 10;

            if (ThreadSafeRandom.Local.Next(0, 2) == 0)
            {
                if (rob.Waste > 0)
                {
                    if (rob.Energy + energy < 32000)
                    {
                        rob.Energy += energy;
                        rob.Waste -= rob.Chloroplasts / 32000 * (1 - SimOpt.SimOpts.VegFeedingToBody);
                    }

                    if (rob.Waste < 0)
                        rob.Waste = 0;
                }

                if (rob.Waste > 0)
                {
                    if (rob.Body + body < 32000)
                    {
                        rob.Body += body;
                        rob.Waste -= rob.Chloroplasts / 32000 * SimOpt.SimOpts.VegFeedingToBody;
                    }

                    if (rob.Waste < 0)
                        rob.Waste = 0;
                }
            }
            else
            {
                if (rob.Waste > 0)
                {
                    if (rob.Body + body < 32000)
                    {
                        rob.Body += body;
                        rob.Waste -= rob.Chloroplasts / 32000 * SimOpt.SimOpts.VegFeedingToBody;
                    }

                    if (rob.Waste < 0)
                        rob.Waste = 0;
                }

                if (rob.Waste > 0)
                {
                    if (rob.Energy + energy < 32000)
                    {
                        rob.Energy += energy;
                        rob.Waste -= rob.Chloroplasts / 32000 * (1 - SimOpt.SimOpts.VegFeedingToBody);
                    }

                    if (rob.Waste < 0)
                        rob.Waste = 0;
                }
            }
        }

        public static void feedvegs(IRobotManager robotManager, int totnrg)
        {
            //Botsareus 8/16/2014 All robots are set to think there is no sun, sun is calculated later
            foreach (var rob in robotManager.Robots.Where(r => r.Energy > 0 && r.Exists))
            {
                rob.Memory[218] = 0;
            }

            double ScreenArea = SimOpt.SimOpts.FieldWidth * SimOpt.SimOpts.FieldHeight;

            var TotalRobotArea = robotManager.Robots.Where(r => r.Exists).Sum(r => Math.Pow(r.GetRadius(SimOpt.SimOpts.FixedBotRadii), 2) * Math.PI);

            if (ScreenArea < 1)
            {
                ScreenArea = 1;
            }

            LightAval = TotalRobotArea / ScreenArea; //Panda 8/14/2013 Figure AreaInverted a.k.a. available light
            if (LightAval > 1)
                LightAval = 1; //Botsareus make sure LighAval never goes negative

            var AreaCorrection = Math.Pow((1 - LightAval), 2) * 4;

            foreach (var rob in robotManager.Robots.Where(r => r.Energy > 0 && r.Exists))
            {
                double acttok = 0;
                //Botsareus 8/16/2014 Allow robots to share chloroplasts again
                if (rob.Chloroplasts > 0)
                {
                    if (rob.ChloroplastsShareDelay > 0)
                        rob.ChloroplastsShareDelay--;

                    double tok = totnrg;

                    if (tok < 0)
                    {
                        tok = 0;
                    }

                    tok /= 3.5; //Botsareus 2/25/2014 A little mod for PhinotPi

                    //Panda 8/14/2013 New chloroplast codez
                    var ChloroplastCorrection = rob.Chloroplasts / 16000;
                    var AddEnergyRate = (AreaCorrection * ChloroplastCorrection) * 1.25;
                    var SubtractEnergyRate = Math.Pow(rob.Chloroplasts / 32000, 2);

                    acttok = (AddEnergyRate - SubtractEnergyRate) * tok;
                }
                rob.Memory[218] = 1; //Botsareus 8/16/2014 Now it is time view the sun

                if (!(rob.Chloroplasts > 0))
                    continue;

                acttok -= rob.Age * rob.Chloroplasts / 1000000000; //Botsareus 10/6/2015 Robots should start losing body at around 32000 cycles

                rob.Energy += acttok * (1 - SimOpt.SimOpts.VegFeedingToBody);
                rob.Body += acttok * SimOpt.SimOpts.VegFeedingToBody / 10;

                if (rob.Energy > 32000)
                    rob.Energy = 32000;

                if (rob.Body > 32000)
                    rob.Body = 32000;
            }
        }

        public static void VegsRepopulate(IRobotManager robotManager, IBucketManager bucketManager)
        {
            CoolDown++;
            if (CoolDown >= SimOpt.SimOpts.RepopCooldown)
            {
                for (var t = 1; t < SimOpt.SimOpts.RepopAmount; t++)
                {
                    aggiungirob(robotManager, bucketManager);
                    TotalVegs++;
                }
                CoolDown -= SimOpt.SimOpts.RepopCooldown;
            }
        }

        private static void aggiungirob(IRobotManager robotManager, IBucketManager bucketManager)
        {
            if (!SimOpt.SimOpts.Specie.Any(s => CheckVegStatus(robotManager, s)))
                return;

            int r;

            do
            {
                r = ThreadSafeRandom.Local.Next(0, SimOpt.SimOpts.Specie.Count); // start randomly in the list of species
            } while (!CheckVegStatus(robotManager, SimOpt.SimOpts.Specie[r]));

            var x = ThreadSafeRandom.Local.Next((int)(SimOpt.SimOpts.Specie[r].Poslf * (SimOpt.SimOpts.FieldWidth - 60)), (int)(SimOpt.SimOpts.Specie[r].Posrg * (SimOpt.SimOpts.FieldWidth - 60)));
            var y = ThreadSafeRandom.Local.Next((int)(SimOpt.SimOpts.Specie[r].Postp * (SimOpt.SimOpts.FieldHeight - 60)), (int)(SimOpt.SimOpts.Specie[r].Posdn * (SimOpt.SimOpts.FieldHeight - 60)));

            if (SimOpt.SimOpts.Specie[r].Name == "" || SimOpt.SimOpts.Specie[r].Path == "Invalid Path")
                return;

            var a = DnaManipulations.RobScriptLoad(robotManager, bucketManager, System.IO.Path.Join(SimOpt.SimOpts.Specie[r].Path,
                    SimOpt.SimOpts.Specie[r].Name));

            if (a == null)
            {
                SimOpt.SimOpts.Specie[r].Native = false;
                return;
            }

            //Check to see if we were able to load the bot.  If we can't, the path may be wrong, the sim may have
            //come from another machine with a different install path.  Set the species path to an empty string to
            //prevent endless looping of error dialogs.
            if (!a.Exists)
            {
                SimOpt.SimOpts.Specie[r].Path = "Invalid Path";
                return;
            }

            a.IsVegetable = SimOpt.SimOpts.Specie[r].Veg;
            if (a.IsVegetable)
                a.Chloroplasts = SimOptions.StartChlr;

            a.IsFixed = SimOpt.SimOpts.Specie[r].Fixed;
            a.CantSee = SimOpt.SimOpts.Specie[r].CantSee;
            a.DnaDisabled = SimOpt.SimOpts.Specie[r].DisableDna;
            a.MovementSysvarsDisabled = SimOpt.SimOpts.Specie[r].DisableMovementSysvars;
            a.CantReproduce = SimOpt.SimOpts.Specie[r].CantReproduce;
            a.IsVirusImmune = SimOpt.SimOpts.Specie[r].VirusImmune;
            a.IsCorpse = false;
            a.IsDead = false;
            a.Body = 1000;
            a.Mutations = 0;
            a.OldMutations = 0;
            a.LastMutation = 0;
            a.Generation = 0;
            a.SonNumber = 0;
            a.Parent = null;
            Array.Clear(a.Memory, 0, a.Memory.Length);

            if (a.IsFixed)
                a.Memory[216] = 1;

            a.Position = new DoubleVector(x, y);

            a.Aim = ThreadSafeRandom.Local.NextDouble() * Math.PI * 2;
            a.Memory[MemoryAddresses.SetAim] = (int)a.Aim * 200;

            //Bot is already in a bucket due to the prepare routine
            bucketManager.UpdateBotBucket(a);
            a.Energy = SimOpt.SimOpts.Specie[r].Stnrg;
            a.MutationProbabilities = SimOpt.SimOpts.Specie[r].Mutables;

            a.VirusTimer = 0;
            a.VirusShot = null;
            a.NumberOfGenes = DnaManipulations.CountGenes(a.Dna);

            a.GenMut = (double)a.Dna.Count / RobotsManager.GeneticSensitivity;

            a.Memory[MemoryAddresses.DnaLenSys] = a.Dna.Count;
            a.Memory[MemoryAddresses.GenesSys] = a.NumberOfGenes;

            a.MultibotTimer = SimOpt.SimOpts.Specie[r].kill_mb ? 210 : 0;
            a.ChloroplastsDisabled = SimOpt.SimOpts.Specie[r].NoChlr;

            for (var i = 0; i < 7; i++)
                a.Skin[i] = SimOpt.SimOpts.Specie[r].Skin[i];

            a.Color = SimOpt.SimOpts.Specie[r].Color;
            Senses.MakeOccurrList(a);
        }

        private static bool CheckVegStatus(IRobotManager robotManager, Species species)
        {
            if (!species.Veg || !species.Native)
                return false;

            //see if any active robots have chloroplasts
            if (robotManager.Robots.Where(r => r.Exists && r.Chloroplasts > 0)
                .Select(rob => new { rob, splitname = rob.FName.Split(")") })
                .Select(t =>
                    t.splitname[0].StartsWith("(") && int.TryParse(t.splitname[0][1..], out _)
                        ? t.splitname[1]
                        : t.rob.FName).Any(robname => species.Name == robname))
            {
                return true;
            }

            //If there is no robots at all with chlr then repop everything
            return !robotManager.Robots.Any(r => r.Exists && r.IsVegetable && r.Age > 0);
        }
    }
}
