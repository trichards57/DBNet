using DarwinBots.Model;
using DarwinBots.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DarwinBots.Modules.Globals;
using static DarwinBots.Modules.Physics;
using static DarwinBots.Modules.Robots;
using static DarwinBots.Modules.SimOpt;

namespace DarwinBots.Modules
{
    internal static class Vegs
    {
        public static int cooldown { get; set; }
        public static int CurrentEnergyCycle { get; set; }
        public static double LightAval { get; set; }
        public static double SunChange { get; set; }
        public static double SunPosition { get; set; }
        public static double SunRange { get; set; }
        public static List<int> TotalSimEnergy { get; set; } = new List<int>(new int[101]);
        public static int TotalSimEnergyDisplayed { get; set; }
        public static int totvegs { get; set; }
        public static int totvegsDisplayed { get; set; }

        public static void feedveg2(robot rob)
        {
            var Energy = rob.chloroplasts / 64000 * (1 - SimOpts.VegFeedingToBody);
            var body = (rob.chloroplasts / 64000 * SimOpts.VegFeedingToBody) / 10;

            if (ThreadSafeRandom.Local.Next(0, 2) == 0)
            {
                if (rob.Waste > 0)
                {
                    if (rob.nrg + Energy < 32000)
                    {
                        rob.nrg += Energy;
                        rob.Waste -= rob.chloroplasts / 32000 * (1 - SimOpts.VegFeedingToBody);
                    }

                    if (rob.Waste < 0)
                        rob.Waste = 0;
                }

                if (rob.Waste > 0)
                {
                    if (rob.body + body < 32000)
                    {
                        rob.body += body;
                        rob.Waste -= rob.chloroplasts / 32000 * SimOpts.VegFeedingToBody;
                    }

                    if (rob.Waste < 0)
                        rob.Waste = 0;
                }
            }
            else
            {
                if (rob.Waste > 0)
                {
                    if (rob.body + body < 32000)
                    {
                        rob.body += body;
                        rob.Waste -= rob.chloroplasts / 32000 * SimOpts.VegFeedingToBody;
                    }

                    if (rob.Waste < 0)
                        rob.Waste = 0;
                }

                if (rob.Waste > 0)
                {
                    if (rob.nrg + Energy < 32000)
                    {
                        rob.nrg += Energy;
                        rob.Waste -= rob.chloroplasts / 32000 * (1 - SimOpts.VegFeedingToBody);
                    }

                    if (rob.Waste < 0)
                        rob.Waste = 0;
                }
            }
        }

        public static void feedvegs(int totnrg)
        {
            if (SimOpts.SunOnRnd)
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

            var FeedThisCycle = SimOpts.Daytime;
            var OverrideDayNight = false;

            if (TotalSimEnergyDisplayed < SimOpts.SunUpThreshold && SimOpts.SunUp)
            {
                //Sim Energy has fallen below the threshold.  Let the sun shine!
                switch (SimOpts.SunThresholdMode)
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
                        SimOpts.DayNightCycleCounter = 0;
                        SimOpts.Daytime = true;
                        FeedThisCycle = true;
                        break;

                    case SunThresholdMode.PermanentlyToggle:
                        //We don't care about cycles.  We are just bouncing back and forth between the thresholds.
                        //We want to feed this cycle.
                        //We also want to turn on the sun.  The test below should avoid trying to execute day/night cycles.
                        FeedThisCycle = true;
                        SimOpts.Daytime = true;
                        break;
                }
            }
            else if (TotalSimEnergyDisplayed > SimOpts.SunDownThreshold && SimOpts.SunDown)
            {
                switch (SimOpts.SunThresholdMode)
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
                        SimOpts.DayNightCycleCounter = 0;
                        SimOpts.Daytime = false;
                        FeedThisCycle = false;
                        break;

                    case SunThresholdMode.PermanentlyToggle:
                        //We don't care about cycles.  We are just bouncing back and forth between the thresholds.
                        //We do not want to feed this cycle.
                        //We also want to turn off the sun.  The test below should avoid trying to execute day/night cycles
                        FeedThisCycle = false;
                        SimOpts.Daytime = false;
                        break;
                }
            }

            //In this mode, we ignore sun cycles and just bounce between thresholds.  I don't really want to add another
            //feature enable checkbox, so we will just test to make sure the user is using both thresholds.  If not, we
            //don't override the cycles even if one of the thresholds is set.
            if (SimOpts.SunThresholdMode == SunThresholdMode.PermanentlyToggle && SimOpts.SunDown && SimOpts.SunUp)
                OverrideDayNight = true;

            if (SimOpts.DayNight && !OverrideDayNight)
            {
                //Well, we are neither above nor below the thresholds or we arn't using thresholds so lets see if it's time to rise and shine
                SimOpts.DayNightCycleCounter++;
                if (SimOpts.DayNightCycleCounter > SimOpts.CycleLength)
                {
                    SimOpts.Daytime = !SimOpts.Daytime;
                    SimOpts.DayNightCycleCounter = 0;
                }

                FeedThisCycle = SimOpts.Daytime;
            }

            //Botsareus 8/16/2014 All robots are set to think there is no sun, sun is calculated later
            foreach (var rob in rob.Where(r => r.nrg > 0 && r.exist))
            {
                rob.mem[218] = 0;
            }

            if (!FeedThisCycle)
                return;

            double ScreenArea = SimOpt.SimOpts.FieldWidth * SimOpt.SimOpts.FieldHeight;

            ScreenArea -= Globals.ObstacleManager.Obstacles.Where(o => o.exist).Sum(o => o.Width * o.Height);

            var TotalRobotArea = rob.Where(r => r.exist).Sum(r => Math.Pow(r.radius, 2) * Math.PI);

            if (ScreenArea < 1)
            {
                ScreenArea = 1;
            }

            LightAval = TotalRobotArea / ScreenArea; //Panda 8/14/2013 Figure AreaInverted a.k.a. available light
            if (LightAval > 1)
                LightAval = 1; //Botsareus make sure LighAval never goes negative

            var AreaCorrection = Math.Pow((1 - LightAval), 2) * 4;

            var sunstart = (SunPosition - (0.25 + Math.Pow(SunRange, 3) * 0.75) / 2) * SimOpts.FieldWidth;
            var sunstop = (SunPosition + (0.25 + Math.Pow(SunRange, 3) * 0.75) / 2) * SimOpts.FieldWidth;

            var sunstop2 = sunstop;
            var sunstart2 = sunstart; //Do not delete, bug fix!

            if (sunstart < 0)
            {
                sunstart2 = SimOpts.FieldWidth + sunstart;
                sunstop2 = SimOpts.FieldWidth;
            }
            if (sunstop > SimOpts.FieldWidth)
            {
                sunstop2 = sunstop - SimOpts.FieldWidth;
                sunstart2 = 0;
            }

            foreach (var rob in Robots.rob.Where(r => r.nrg > 0 && r.exist))
            {
                double acttok = 0;
                //Botsareus 8/16/2014 Allow robots to share chloroplasts again
                if (rob.chloroplasts > 0)
                {
                    if (rob.Chlr_Share_Delay > 0)
                        rob.Chlr_Share_Delay--;

                    if ((rob.pos.X < sunstart2 || rob.pos.X > sunstop2) && (rob.pos.X < sunstart || rob.pos.X > sunstop))
                        continue;

                    double tok = 0;
                    if (SimOpts.PondMode)
                    {
                        var depth = (rob.pos.Y / 2000) + 1;
                        if (depth < 1)
                            depth = 1;

                        tok = SimOpts.LightIntensity / Math.Pow(depth, SimOpts.Gradient); //Botsareus 3/26/2013 No longer add one, robots get fed more accuratly
                    }
                    else
                        tok = totnrg;

                    if (tok < 0)
                    {
                        tok = 0;
                    }

                    tok /= 3.5; //Botsareus 2/25/2014 A little mod for PhinotPi

                    //Panda 8/14/2013 New chloroplast codez
                    var ChloroplastCorrection = rob.chloroplasts / 16000;
                    var AddEnergyRate = (AreaCorrection * ChloroplastCorrection) * 1.25;
                    var SubtractEnergyRate = Math.Pow(rob.chloroplasts / 32000, 2);

                    acttok = (AddEnergyRate - SubtractEnergyRate) * tok;
                }
                rob.mem[218] = 1; //Botsareus 8/16/2014 Now it is time view the sun

                if (rob.chloroplasts > 0)
                {
                    acttok -= rob.age * rob.chloroplasts / 1000000000; //Botsareus 10/6/2015 Robots should start losing body at around 32000 cycles

                    if (TmpOpts.Tides > 0)
                    {
                        acttok *= (1 - BouyancyScaling); //Botsareus 10/6/2015 Cancer effect corrected for
                    }

                    rob.nrg += acttok * (1 - SimOpts.VegFeedingToBody);
                    rob.body += acttok * SimOpts.VegFeedingToBody / 10;

                    if (rob.nrg > 32000)
                        rob.nrg = 32000;

                    if (rob.body > 32000)
                        rob.body = 32000;

                    rob.radius = FindRadius(rob);
                }
            }
        }

        public static async Task VegsRepopulate()
        {
            cooldown++;
            if (cooldown >= SimOpts.RepopCooldown)
            {
                for (var t = 1; t < SimOpts.RepopAmount; t++)
                {
                    await aggiungirob(-1, ThreadSafeRandom.Local.Next(60, SimOpts.FieldWidth - 60), ThreadSafeRandom.Local.Next(60, SimOpts.FieldHeight - 60));
                    totvegs++;
                }
                cooldown -= SimOpts.RepopCooldown;
            }
        }
    }
}
