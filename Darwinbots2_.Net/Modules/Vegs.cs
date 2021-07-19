using DarwinBots.Model;
using DarwinBots.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DarwinBots.Modules
{
    internal static class Vegs
    {
        public static int CoolDown { get; set; }
        public static int CurrentEnergyCycle { get; set; }
        public static double LightAval { get; private set; }
        public static double SunChange { get; set; }
        public static double SunPosition { get; set; }
        public static double SunRange { get; set; }
        public static List<int> TotalSimEnergy { get; } = new(new int[101]);
        public static int TotalSimEnergyDisplayed { get; set; }
        public static int TotalVegs { get; set; }
        public static int TotalVegsDisplayed { get; set; }

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

        public static void feedvegs(int totnrg)
        {
            if (SimOpt.SimOpts.SunOnRnd)
            {
                var Sposition = (int)(SunChange % 10);
                var Srange = (int)SunChange / 10;

                if (ThreadSafeRandom.Local.Next(0, 2000) == 0)
                    Srange = Srange == 0 ? 1 : 0;

                if (ThreadSafeRandom.Local.Next(0, 2000) == 0)
                    Sposition = ThreadSafeRandom.Local.Next(0, 3);

                if (Srange == 1)
                    SunRange += 0.0005;

                if (Srange == 0)
                    SunRange -= 0.0005;

                if (SunRange >= 1)
                    Srange = 0;

                if (SunRange <= 0)
                    Srange = 1;

                if (Sposition == 0)
                    SunPosition -= 0.0005;

                if (Sposition == 2)
                    SunPosition += 0.0005;

                if (SunPosition >= 1)
                    Sposition = 0;

                if (SunPosition <= 0)
                    Sposition = 2;

                SunChange = Sposition + Srange * 10;
            }

            var FeedThisCycle = SimOpt.SimOpts.Daytime;
            var OverrideDayNight = false;

            if (TotalSimEnergyDisplayed < SimOpt.SimOpts.SunUpThreshold && SimOpt.SimOpts.SunUp)
            {
                //Sim Energy has fallen below the threshold.  Let the sun shine!
                switch (SimOpt.SimOpts.SunThresholdMode)
                {
                    case SunThresholdMode.TemporarilySuspend:
                        // We only suspend the sun cycles for this cycle.  We want to feed this cycle, but not
                        // advance the sun or disable day/night cycles
                        FeedThisCycle = true;
                        OverrideDayNight = true;
                        break;

                    case SunThresholdMode.AdvanceToDawnDusk:
                        //Speed up time until Dawn.  No need to override the day night cycles as we want them to take over.
                        //Note that the real dawn won't actually start until the nrg climbs above the threshold since
                        //we will keep coming in here and zeroing the counter, but that's probably okay.
                        SimOpt.SimOpts.DayNightCycleCounter = 0;
                        SimOpt.SimOpts.Daytime = true;
                        FeedThisCycle = true;
                        break;

                    case SunThresholdMode.PermanentlyToggle:
                        //We don't care about cycles.  We are just bouncing back and forth between the thresholds.
                        //We want to feed this cycle.
                        //We also want to turn on the sun.  The test below should avoid trying to execute day/night cycles.
                        FeedThisCycle = true;
                        SimOpt.SimOpts.Daytime = true;
                        break;
                }
            }
            else if (TotalSimEnergyDisplayed > SimOpt.SimOpts.SunDownThreshold && SimOpt.SimOpts.SunDown)
            {
                switch (SimOpt.SimOpts.SunThresholdMode)
                {
                    case SunThresholdMode.TemporarilySuspend:
                        // We only suspend the sun cycles for this cycle.  We do not want to feed this cycle, nor do we
                        // advance the sun or disable day/night cycles
                        FeedThisCycle = false;
                        OverrideDayNight = true;
                        break;

                    case SunThresholdMode.AdvanceToDawnDusk:
                        //Speed up time until Dusk.  No need to override the day night cycles as we want them to take over.
                        //Note that the real night time won't actually start until the nrg falls below the threshold since
                        //we will keep coming in here and zeroing the counter, but that's probably okay.
                        SimOpt.SimOpts.DayNightCycleCounter = 0;
                        SimOpt.SimOpts.Daytime = false;
                        FeedThisCycle = false;
                        break;

                    case SunThresholdMode.PermanentlyToggle:
                        //We don't care about cycles.  We are just bouncing back and forth between the thresholds.
                        //We do not want to feed this cycle.
                        //We also want to turn off the sun.  The test below should avoid trying to execute day/night cycles
                        FeedThisCycle = false;
                        SimOpt.SimOpts.Daytime = false;
                        break;
                }
            }

            //In this mode, we ignore sun cycles and just bounce between thresholds.  I don't really want to add another
            //feature enable checkbox, so we will just test to make sure the user is using both thresholds.  If not, we
            //don't override the cycles even if one of the thresholds is set.
            if (SimOpt.SimOpts.SunThresholdMode == SunThresholdMode.PermanentlyToggle && SimOpt.SimOpts.SunDown && SimOpt.SimOpts.SunUp)
                OverrideDayNight = true;

            if (SimOpt.SimOpts.DayNight && !OverrideDayNight)
            {
                //Well, we are neither above nor below the thresholds or we arn't using thresholds so lets see if it's time to rise and shine
                SimOpt.SimOpts.DayNightCycleCounter++;
                if (SimOpt.SimOpts.DayNightCycleCounter > SimOpt.SimOpts.CycleLength)
                {
                    SimOpt.SimOpts.Daytime = !SimOpt.SimOpts.Daytime;
                    SimOpt.SimOpts.DayNightCycleCounter = 0;
                }

                FeedThisCycle = SimOpt.SimOpts.Daytime;
            }

            //Botsareus 8/16/2014 All robots are set to think there is no sun, sun is calculated later
            foreach (var rob in Globals.RobotsManager.Robots.Where(r => r.Energy > 0 && r.Exists))
            {
                rob.Memory[218] = 0;
            }

            if (!FeedThisCycle)
                return;

            double ScreenArea = SimOpt.SimOpts.FieldWidth * SimOpt.SimOpts.FieldHeight;

            ScreenArea -= Globals.ObstacleManager.Obstacles.Where(o => o.Exist).Sum(o => o.Width * o.Height);

            var TotalRobotArea = Globals.RobotsManager.Robots.Where(r => r.Exists).Sum(r => Math.Pow(r.Radius, 2) * Math.PI);

            if (ScreenArea < 1)
            {
                ScreenArea = 1;
            }

            LightAval = TotalRobotArea / ScreenArea; //Panda 8/14/2013 Figure AreaInverted a.k.a. available light
            if (LightAval > 1)
                LightAval = 1; //Botsareus make sure LighAval never goes negative

            var AreaCorrection = Math.Pow((1 - LightAval), 2) * 4;

            var sunstart = (SunPosition - (0.25 + Math.Pow(SunRange, 3) * 0.75) / 2) * SimOpt.SimOpts.FieldWidth;
            var sunstop = (SunPosition + (0.25 + Math.Pow(SunRange, 3) * 0.75) / 2) * SimOpt.SimOpts.FieldWidth;

            var sunstop2 = sunstop;
            var sunstart2 = sunstart; //Do not delete, bug fix!

            if (sunstart < 0)
            {
                sunstart2 = SimOpt.SimOpts.FieldWidth + sunstart;
                sunstop2 = SimOpt.SimOpts.FieldWidth;
            }
            if (sunstop > SimOpt.SimOpts.FieldWidth)
            {
                sunstop2 = sunstop - SimOpt.SimOpts.FieldWidth;
                sunstart2 = 0;
            }

            foreach (var rob in Globals.RobotsManager.Robots.Where(r => r.Energy > 0 && r.Exists))
            {
                double acttok = 0;
                //Botsareus 8/16/2014 Allow robots to share chloroplasts again
                if (rob.Chloroplasts > 0)
                {
                    if (rob.ChloroplastsShareDelay > 0)
                        rob.ChloroplastsShareDelay--;

                    if ((rob.Position.X < sunstart2 || rob.Position.X > sunstop2) && (rob.Position.X < sunstart || rob.Position.X > sunstop))
                        continue;

                    double tok = 0;
                    if (SimOpt.SimOpts.PondMode)
                    {
                        var depth = (rob.Position.Y / 2000) + 1;
                        if (depth < 1)
                            depth = 1;

                        tok = SimOpt.SimOpts.LightIntensity / Math.Pow(depth, SimOpt.SimOpts.Gradient); //Botsareus 3/26/2013 No longer add one, robots get fed more accuratly
                    }
                    else
                        tok = totnrg;

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

                if (SimOpt.TmpOpts.Tides > 0)
                    acttok *= (1 - Physics.BouyancyScaling); //Botsareus 10/6/2015 Cancer effect corrected for

                rob.Energy += acttok * (1 - SimOpt.SimOpts.VegFeedingToBody);
                rob.Body += acttok * SimOpt.SimOpts.VegFeedingToBody / 10;

                if (rob.Energy > 32000)
                    rob.Energy = 32000;

                if (rob.Body > 32000)
                    rob.Body = 32000;
            }
        }

        public static async Task VegsRepopulate()
        {
            CoolDown++;
            if (CoolDown >= SimOpt.SimOpts.RepopCooldown)
            {
                for (var t = 1; t < SimOpt.SimOpts.RepopAmount; t++)
                {
                    await aggiungirob();
                    TotalVegs++;
                }
                CoolDown -= SimOpt.SimOpts.RepopCooldown;
            }
        }

        private static async Task aggiungirob()
        {
            if (!SimOpt.SimOpts.Specie.Any(CheckVegStatus))
                return;

            int r;

            do
            {
                r = ThreadSafeRandom.Local.Next(0, SimOpt.SimOpts.Specie.Count); // start randomly in the list of species
            } while (!CheckVegStatus(SimOpt.SimOpts.Specie[r]));

            var x = ThreadSafeRandom.Local.Next((int)(SimOpt.SimOpts.Specie[r].Poslf * (SimOpt.SimOpts.FieldWidth - 60)), (int)(SimOpt.SimOpts.Specie[r].Posrg * (SimOpt.SimOpts.FieldWidth - 60)));
            var y = ThreadSafeRandom.Local.Next((int)(SimOpt.SimOpts.Specie[r].Postp * (SimOpt.SimOpts.FieldHeight - 60)), (int)(SimOpt.SimOpts.Specie[r].Posdn * (SimOpt.SimOpts.FieldHeight - 60)));

            if (SimOpt.SimOpts.Specie[r].Name == "" || SimOpt.SimOpts.Specie[r].Path == "Invalid Path")
                return;

            var a = await DnaManipulations.RobScriptLoad(System.IO.Path.Join(SimOpt.SimOpts.Specie[r].Path,
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
                a.Chloroplasts = Globals.StartChlr;

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
            Globals.BucketManager.UpdateBotBucket(a);
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

        private static bool CheckVegStatus(Species species)
        {
            if (!species.Veg || !species.Native)
                return false;

            //see if any active robots have chloroplasts
            if (Globals.RobotsManager.Robots.Where(r => r.Exists && r.Chloroplasts > 0)
                .Select(rob => new { rob, splitname = rob.FName.Split(")") })
                .Select(t =>
                    t.splitname[0].StartsWith("(") && int.TryParse(t.splitname[0][1..], out _)
                        ? t.splitname[1]
                        : t.rob.FName).Any(robname => species.Name == robname))
            {
                return true;
            }

            //If there is no robots at all with chlr then repop everything
            return !Globals.RobotsManager.Robots.Any(r => r.Exists && r.IsVegetable && r.Age > 0);
        }
    }
}
