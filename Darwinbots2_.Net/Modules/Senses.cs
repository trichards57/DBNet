using DarwinBots.Model;
using System;
using System.Linq;

namespace DarwinBots.Modules
{
    internal static class Senses
    {
        public static void EraseSenses(robot rob)
        {
            rob.lasttch = null; //Botsareus 11/26/2013 Erase lasttch here
            rob.mem[MemoryAddresses.hitup] = 0;
            rob.mem[MemoryAddresses.hitdn] = 0;
            rob.mem[MemoryAddresses.hitdx] = 0;
            rob.mem[MemoryAddresses.hitsx] = 0;
            rob.mem[MemoryAddresses.hit] = 0;
            rob.mem[MemoryAddresses.shflav] = 0;
            rob.mem[MemoryAddresses.shang] = 0; //.shang
            rob.mem[MemoryAddresses.shup] = 0;
            rob.mem[MemoryAddresses.shdn] = 0;
            rob.mem[MemoryAddresses.shdx] = 0;
            rob.mem[MemoryAddresses.shsx] = 0;
            rob.mem[214] = 0; //edge collision detection
            EraseLookOccurr(rob);
        }

        public static void LookOccurr(robot rob1, robot rob2)
        {
            if (rob1.Corpse)
                return;

            rob1.mem[MemoryAddresses.REFTYPE] = 0;

            for (var t = 1; t < 8; t++)
                rob1.mem[MemoryAddresses.occurrstart + t] = rob2.occurr[t];

            rob1.mem[MemoryAddresses.occurrstart + 9] = (int)Math.Clamp(rob2.nrg, 0, 32000);

            if (rob2.age < 32001)
                rob1.mem[MemoryAddresses.occurrstart + 10] = rob2.age; //.refage
            else
                rob1.mem[MemoryAddresses.occurrstart + 10] = 32000;

            rob1.mem[MemoryAddresses.in1] = rob2.mem[MemoryAddresses.out1];
            rob1.mem[MemoryAddresses.in2] = rob2.mem[MemoryAddresses.out2];
            rob1.mem[MemoryAddresses.in3] = rob2.mem[MemoryAddresses.out3];
            rob1.mem[MemoryAddresses.in4] = rob2.mem[MemoryAddresses.out4];
            rob1.mem[MemoryAddresses.in5] = rob2.mem[MemoryAddresses.out5];
            rob1.mem[MemoryAddresses.in6] = rob2.mem[MemoryAddresses.out6];
            rob1.mem[MemoryAddresses.in7] = rob2.mem[MemoryAddresses.out7];
            rob1.mem[MemoryAddresses.in8] = rob2.mem[MemoryAddresses.out8];
            rob1.mem[MemoryAddresses.in9] = rob2.mem[MemoryAddresses.out9];
            rob1.mem[MemoryAddresses.in10] = rob2.mem[MemoryAddresses.out10];

            rob1.mem[MemoryAddresses.refaim] = rob2.mem[18]; //refaim
            rob1.mem[MemoryAddresses.reftie] = rob2.occurr[9]; //reftie

            rob1.mem[MemoryAddresses.refshell] = (int)rob2.shell;
            rob1.mem[MemoryAddresses.refbody] = (int)rob2.Body;
            rob1.mem[MemoryAddresses.refypos] = rob2.mem[MemoryAddresses.ypos];
            rob1.mem[MemoryAddresses.refxpos] = rob2.mem[MemoryAddresses.xpos];
            //give reference variables from the bots frame of reference
            var x = Math.Clamp(rob2.vel.X * Math.Cos(rob1.aim) + rob2.vel.Y * Math.Sin(rob1.aim) * -1 - rob1.mem[MemoryAddresses.velup], -32000, 32000);
            var y = Math.Clamp(rob2.vel.Y * Math.Cos(rob1.aim) + rob2.vel.X * Math.Sin(rob1.aim) - rob1.mem[MemoryAddresses.veldx], -32000, 32000);

            rob1.mem[MemoryAddresses.refvelup] = (int)x;
            rob1.mem[MemoryAddresses.refveldn] = rob1.mem[MemoryAddresses.refvelup] * -1;
            rob1.mem[MemoryAddresses.refveldx] = (int)y;
            rob1.mem[MemoryAddresses.refvelsx] = rob1.mem[MemoryAddresses.refvelsx] * -1;

            var temp = Math.Sqrt(Math.Pow(rob1.mem[MemoryAddresses.refvelup], 2) + Math.Pow(rob1.mem[MemoryAddresses.refveldx], 2)); // how fast is this robot moving compared to me?
            if (temp > 32000)
                temp = 32000;

            rob1.mem[MemoryAddresses.refvelscalar] = (int)temp;
            rob1.mem[713] = rob2.mem[827]; //refpoison. current value of poison. not poison commands
            rob1.mem[714] = rob2.mem[825]; //refvenom (as with poison)
            rob1.mem[715] = rob2.Kills; //refkills
            rob1.mem[MemoryAddresses.refmulti] = rob2.Multibot ? 1 : 0;

            if (rob1.mem[474] > 0 & rob1.mem[474] <= 1000)
            {
                //readmem and memloc couple used to read a specified memory location of the target robot
                rob1.mem[473] = rob2.mem[rob1.mem[474]];
                if (rob1.mem[474] > MemoryAddresses.EyeStart && rob1.mem[474] < MemoryAddresses.EyeEnd)
                    rob2.View = true;
            }

            rob1.mem[477] = rob2.Fixed ? 1 : 0;
        }

        public static void MakeOccurrList(robot rob)
        {
            for (var t = 1; t < 12; t++)
                rob.occurr[t] = 0;

            for (var i = 0; i < rob.dna.Count; i++)
            {
                if (rob.dna[i].Type == 10 && rob.dna[i].Value == 1)
                    break;

                switch (rob.dna[i].Type)
                {
                    case 0:
                        //number
                        if (rob.dna[i + 1].Type == 7)
                        {
                            //DNA is going to store to this value, so it's probably a sysvar
                            switch (rob.dna[i].Value)
                            {
                                case > 0 and < 9:
                                    //if we are dealing with one of the first 8 sysvars
                                    rob.occurr[rob.dna[i].Value]++; //then the occur listing for this fxn is incremented
                                    break;

                                case 826:
                                    //referencing .strpoison
                                    rob.occurr[10]++;
                                    break;

                                case 824:
                                    //refencing .strvenom
                                    rob.occurr[11]++;
                                    break;
                            }
                        }
                        else if (rob.dna[i].Value == 330)
                        {
                            //the bot is referencing .tie 'Botsareus 11/29/2013 Moved to "." list
                            rob.occurr[9]++; //ties
                        }
                        break;

                    case 1:
                        //*number
                        if (rob.dna[i].Value > 500 & rob.dna[i].Value < 510)
                        {
                            //the bot is referencing an eye
                            rob.occurr[8]++; //eyes
                        }
                        break;
                }
            }

            for (var t = 1; t < 12; t++)
                rob.mem[720 + t] = rob.occurr[t];
        }

        public static Species SpeciesFromBot(robot rob)
        {
            return SimOpt.SimOpts.Specie.FirstOrDefault(s => s.Name == rob.FName);
        }

        public static void Taste(robot rob, double x, double y, int value)
        {
            var aim = 6.28 - rob.aim;
            var xc = rob.pos.X;
            var yc = rob.pos.Y;
            var dx = x - xc;
            var dy = y - yc;
            var ang = Math.Atan2(dy, dx);
            var dang = Physics.NormaliseAngle(ang - aim);
            var addr = dang switch
            {
                > 5.49 or <= 0.78 => MemoryAddresses.shup,
                > 0.78 and <= 2.36 => MemoryAddresses.shdx,
                > 2.36 and <= 3.92 => MemoryAddresses.shdn,
                _ => MemoryAddresses.shsx,
            };

            rob.mem[addr] = value;
            rob.mem[209] = (int)(dang * 200);
            rob.mem[MemoryAddresses.shflav] = value;
        }

        public static void Touch(robot rob, double x, double y)
        {
            var aim = 6.28 - rob.aim;
            var xc = rob.pos.X;
            var yc = rob.pos.Y;
            var dx = x - xc;
            var dy = y - yc;
            var ang = Math.Atan2(dy, dx);
            var dang = Physics.NormaliseAngle(ang - aim);
            var addr = dang switch
            {
                > 5.49 or <= 0.78 => MemoryAddresses.hitup,
                > 0.78 and <= 2.36 => MemoryAddresses.hitdn,
                > 2.36 and <= 3.92 => MemoryAddresses.hitdx,
                _ => MemoryAddresses.hitsx,
            };
            rob.mem[addr] = 1;
            rob.mem[MemoryAddresses.hit] = 1;
        }

        public static void WriteSenses(robot rob)
        {
            LandMark(rob);

            rob.mem[MemoryAddresses.TotalBots] = Globals.RobotsManager.TotalRobots;
            rob.mem[MemoryAddresses.TOTALMYSPECIES] = SpeciesFromBot(rob).population;

            if (!rob.CantSee && !rob.Corpse && Globals.BucketManager.BucketsProximity(rob) != null)
            {
                switch (rob.lastopp)
                {
                    case robot r:
                        LookOccurr(rob, r); // It's a bot.  Populate the refvar sysvars
                        break;

                    case Obstacle o:
                        LookOccurrShape(rob, o);
                        break;
                }
            }

            if (rob.nrg > 32000)
                rob.nrg = 32000;

            if (rob.onrg < 0)
                rob.onrg = 0;

            if (rob.obody < 0)
                rob.obody = 0;

            if (rob.nrg < 0)
                rob.nrg = 0;

            rob.mem[MemoryAddresses.pain] = (int)(rob.onrg - rob.nrg);
            rob.mem[MemoryAddresses.pleas] = -rob.mem[MemoryAddresses.pain];
            rob.mem[MemoryAddresses.bodloss] = (int)(rob.obody - rob.Body);
            rob.mem[MemoryAddresses.bodgain] = -rob.mem[MemoryAddresses.bodloss];

            rob.onrg = rob.nrg;
            rob.obody = rob.Body;
            rob.mem[MemoryAddresses.Energy] = (int)rob.nrg;

            if (rob.age == 0 & rob.mem[MemoryAddresses.body] == 0)
                rob.mem[MemoryAddresses.body] = (int)rob.Body;

            rob.mem[215] = rob.Fixed ? 1 : 0;

            rob.pos = DoubleVector.Max(rob.pos, 0);

            var temp = DoubleVector.Floor(rob.pos / 32000) * 32000;
            temp = rob.pos - temp;

            rob.mem[217] = (int)(temp.Y % 32000);
            rob.mem[219] = (int)(temp.X % 32000);
        }

        private static void EraseLookOccurr(robot rob)
        {
            if (rob.Corpse)
                return;

            rob.mem[MemoryAddresses.REFTYPE] = 0;

            for (var t = 1; t < 10; t++)
            {
                rob.mem[MemoryAddresses.occurrstart + t] = 0;
            }

            rob.mem[MemoryAddresses.in1] = 0;
            rob.mem[MemoryAddresses.in2] = 0;
            rob.mem[MemoryAddresses.in3] = 0;
            rob.mem[MemoryAddresses.in4] = 0;
            rob.mem[MemoryAddresses.in5] = 0;
            rob.mem[MemoryAddresses.in6] = 0;
            rob.mem[MemoryAddresses.in7] = 0;
            rob.mem[MemoryAddresses.in8] = 0;
            rob.mem[MemoryAddresses.in9] = 0;
            rob.mem[MemoryAddresses.in10] = 0;

            rob.mem[711] = 0;
            rob.mem[712] = 0;
            rob.mem[MemoryAddresses.refshell] = 0;
            rob.mem[MemoryAddresses.refbody] = 0;
            rob.mem[MemoryAddresses.refypos] = 0;
            rob.mem[MemoryAddresses.refxpos] = 0;
            rob.mem[MemoryAddresses.refvelup] = 0;
            rob.mem[MemoryAddresses.refveldn] = 0;
            rob.mem[MemoryAddresses.refveldx] = 0;
            rob.mem[MemoryAddresses.refvelsx] = 0;
            rob.mem[MemoryAddresses.refvelscalar] = 0;
            rob.mem[713] = 0;
            rob.mem[714] = 0;
            rob.mem[715] = 0;
            rob.mem[MemoryAddresses.refmulti] = 0;
            rob.mem[473] = 0;
            rob.mem[477] = 0;
        }

        private static void LandMark(robot rob)
        {
            rob.mem[MemoryAddresses.LandM] = 0;
            if (rob.aim is > 1.39 and < 1.75)
            {
                rob.mem[MemoryAddresses.LandM] = 1;
            }
        }

        private static void LookOccurrShape(robot rob, Obstacle obstacle)
        {
            if (rob.Corpse)
                return;

            rob.mem[MemoryAddresses.REFTYPE] = 1;

            for (var t = 1; t < 8; t++)
                rob.mem[MemoryAddresses.occurrstart + t] = 0;

            rob.mem[MemoryAddresses.occurrstart + 9] = 0; // refnrg
            rob.mem[MemoryAddresses.occurrstart + 10] = 0; //refage

            rob.mem[MemoryAddresses.in1] = 0;
            rob.mem[MemoryAddresses.in2] = 0;
            rob.mem[MemoryAddresses.in3] = 0;
            rob.mem[MemoryAddresses.in4] = 0;
            rob.mem[MemoryAddresses.in5] = 0;
            rob.mem[MemoryAddresses.in6] = 0;
            rob.mem[MemoryAddresses.in7] = 0;
            rob.mem[MemoryAddresses.in8] = 0;
            rob.mem[MemoryAddresses.in9] = 0;
            rob.mem[MemoryAddresses.in10] = 0;

            rob.mem[711] = 0; //refaim
            rob.mem[712] = 0; //reftie
            rob.mem[MemoryAddresses.refshell] = 0;
            rob.mem[MemoryAddresses.refbody] = 0;

            rob.mem[MemoryAddresses.refxpos] = (int)rob.lastopppos.X % 32000;
            rob.mem[MemoryAddresses.refypos] = (int)rob.lastopppos.Y % 32000;

            //give reference variables from the bots frame of reference
            rob.mem[MemoryAddresses.refvelup] = (int)(obstacle.vel.X * Math.Cos(rob.aim) + obstacle.vel.Y * Math.Sin(rob.aim) * -1) - rob.mem[MemoryAddresses.velup];
            rob.mem[MemoryAddresses.refveldn] = rob.mem[MemoryAddresses.refvelup] * -1;
            rob.mem[MemoryAddresses.refveldx] = (int)(obstacle.vel.Y * Math.Cos(rob.aim) + obstacle.vel.X * Math.Sin(rob.aim)) - rob.mem[MemoryAddresses.veldx];
            rob.mem[MemoryAddresses.refvelsx] = rob.mem[MemoryAddresses.refvelsx] * -1;

            var temp = Math.Sqrt(Math.Pow(rob.mem[MemoryAddresses.refvelup], 2) + Math.Pow(rob.mem[MemoryAddresses.refveldx], 2)); // how fast is this shape moving compared to me?
            if (temp > 32000)
                temp = 32000;

            rob.mem[MemoryAddresses.refvelscalar] = (int)temp;
            rob.mem[713] = 0; //refpoison. current value of poison. not poison commands
            rob.mem[714] = 0; //refvenom (as with poison)
            rob.mem[715] = 0; //refkills
            rob.mem[MemoryAddresses.refmulti] = 0;

            rob.mem[473] = 0;
            rob.mem[477] = obstacle.vel.X == 0 && obstacle.vel.Y == 0 ? 1 : 0;
        }
    }
}
