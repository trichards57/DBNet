using DarwinBots.Model;
using System;
using System.Linq;

namespace DarwinBots.Modules
{
    internal static class Senses
    {
        public static void EraseSenses(Robot rob)
        {
            rob.LastTouched = null; //Botsareus 11/26/2013 Erase lasttch here
            rob.Memory[MemoryAddresses.hitup] = 0;
            rob.Memory[MemoryAddresses.hitdn] = 0;
            rob.Memory[MemoryAddresses.hitdx] = 0;
            rob.Memory[MemoryAddresses.hitsx] = 0;
            rob.Memory[MemoryAddresses.hit] = 0;
            rob.Memory[MemoryAddresses.shflav] = 0;
            rob.Memory[MemoryAddresses.shang] = 0; //.shang
            rob.Memory[MemoryAddresses.shup] = 0;
            rob.Memory[MemoryAddresses.shdn] = 0;
            rob.Memory[MemoryAddresses.shdx] = 0;
            rob.Memory[MemoryAddresses.shsx] = 0;
            rob.Memory[214] = 0; //edge collision detection
            EraseLookOccurr(rob);
        }

        public static void LookOccurr(Robot rob1, Robot rob2)
        {
            if (rob1.IsCorpse)
                return;

            rob1.Memory[MemoryAddresses.REFTYPE] = 0;

            for (var t = 1; t < 8; t++)
                rob1.Memory[MemoryAddresses.occurrstart + t] = rob2.occurr[t];

            rob1.Memory[MemoryAddresses.occurrstart + 9] = (int)Math.Clamp(rob2.Energy, 0, 32000);

            if (rob2.Age < 32001)
                rob1.Memory[MemoryAddresses.occurrstart + 10] = rob2.Age; //.refage
            else
                rob1.Memory[MemoryAddresses.occurrstart + 10] = 32000;

            rob1.Memory[MemoryAddresses.in1] = rob2.Memory[MemoryAddresses.out1];
            rob1.Memory[MemoryAddresses.in2] = rob2.Memory[MemoryAddresses.out2];
            rob1.Memory[MemoryAddresses.in3] = rob2.Memory[MemoryAddresses.out3];
            rob1.Memory[MemoryAddresses.in4] = rob2.Memory[MemoryAddresses.out4];
            rob1.Memory[MemoryAddresses.in5] = rob2.Memory[MemoryAddresses.out5];
            rob1.Memory[MemoryAddresses.in6] = rob2.Memory[MemoryAddresses.out6];
            rob1.Memory[MemoryAddresses.in7] = rob2.Memory[MemoryAddresses.out7];
            rob1.Memory[MemoryAddresses.in8] = rob2.Memory[MemoryAddresses.out8];
            rob1.Memory[MemoryAddresses.in9] = rob2.Memory[MemoryAddresses.out9];
            rob1.Memory[MemoryAddresses.in10] = rob2.Memory[MemoryAddresses.out10];

            rob1.Memory[MemoryAddresses.refaim] = rob2.Memory[18]; //refaim
            rob1.Memory[MemoryAddresses.reftie] = rob2.occurr[9]; //reftie

            rob1.Memory[MemoryAddresses.refshell] = (int)rob2.Shell;
            rob1.Memory[MemoryAddresses.refbody] = (int)rob2.Body;
            rob1.Memory[MemoryAddresses.refypos] = rob2.Memory[MemoryAddresses.ypos];
            rob1.Memory[MemoryAddresses.refxpos] = rob2.Memory[MemoryAddresses.xpos];
            //give reference variables from the bots frame of reference
            var x = Math.Clamp(rob2.Velocity.X * Math.Cos(rob1.Aim) + rob2.Velocity.Y * Math.Sin(rob1.Aim) * -1 - rob1.Memory[MemoryAddresses.velup], -32000, 32000);
            var y = Math.Clamp(rob2.Velocity.Y * Math.Cos(rob1.Aim) + rob2.Velocity.X * Math.Sin(rob1.Aim) - rob1.Memory[MemoryAddresses.veldx], -32000, 32000);

            rob1.Memory[MemoryAddresses.refvelup] = (int)x;
            rob1.Memory[MemoryAddresses.refveldn] = rob1.Memory[MemoryAddresses.refvelup] * -1;
            rob1.Memory[MemoryAddresses.refveldx] = (int)y;
            rob1.Memory[MemoryAddresses.refvelsx] = rob1.Memory[MemoryAddresses.refvelsx] * -1;

            var temp = Math.Sqrt(Math.Pow(rob1.Memory[MemoryAddresses.refvelup], 2) + Math.Pow(rob1.Memory[MemoryAddresses.refveldx], 2)); // how fast is this robot moving compared to me?
            if (temp > 32000)
                temp = 32000;

            rob1.Memory[MemoryAddresses.refvelscalar] = (int)temp;
            rob1.Memory[713] = rob2.Memory[827]; //refpoison. current value of poison. not poison commands
            rob1.Memory[714] = rob2.Memory[825]; //refvenom (as with poison)
            rob1.Memory[715] = rob2.Kills; //refkills
            rob1.Memory[MemoryAddresses.refmulti] = rob2.IsMultibot ? 1 : 0;

            if (rob1.Memory[474] > 0 & rob1.Memory[474] < 1000)
            {
                //readmem and memloc couple used to read a specified memory location of the target robot
                rob1.Memory[473] = rob2.Memory[rob1.Memory[474]];
            }

            rob1.Memory[477] = rob2.IsFixed ? 1 : 0;
        }

        public static void MakeOccurrList(Robot rob)
        {
            for (var t = 1; t < 12; t++)
                rob.occurr[t] = 0;

            for (var i = 0; i < rob.Dna.Count; i++)
            {
                if (rob.Dna[i].Type == 10 && rob.Dna[i].Value == 1)
                    break;

                switch (rob.Dna[i].Type)
                {
                    case 0:
                        //number
                        if (rob.Dna[i + 1].Type == 7)
                        {
                            //DNA is going to store to this value, so it's probably a sysvar
                            switch (rob.Dna[i].Value)
                            {
                                case > 0 and < 9:
                                    //if we are dealing with one of the first 8 sysvars
                                    rob.occurr[rob.Dna[i].Value]++; //then the occur listing for this fxn is incremented
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
                        else if (rob.Dna[i].Value == 330)
                        {
                            //the bot is referencing .tie 'Botsareus 11/29/2013 Moved to "." list
                            rob.occurr[9]++; //ties
                        }
                        break;

                    case 1:
                        //*number
                        if (rob.Dna[i].Value > 500 & rob.Dna[i].Value < 510)
                        {
                            //the bot is referencing an eye
                            rob.occurr[8]++; //eyes
                        }
                        break;
                }
            }

            for (var t = 1; t < 12; t++)
                rob.Memory[720 + t] = rob.occurr[t];
        }

        public static Species SpeciesFromBot(Robot rob)
        {
            return SimOpt.SimOpts.Specie.FirstOrDefault(s => s.Name == rob.FName);
        }

        public static void Taste(Robot rob, double x, double y, int value)
        {
            var aim = 6.28 - rob.Aim;
            var xc = rob.Position.X;
            var yc = rob.Position.Y;
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

            rob.Memory[addr] = value;
            rob.Memory[209] = (int)(dang * 200);
            rob.Memory[MemoryAddresses.shflav] = value;
        }

        public static void Touch(Robot rob, double x, double y)
        {
            var aim = 6.28 - rob.Aim;
            var xc = rob.Position.X;
            var yc = rob.Position.Y;
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
            rob.Memory[addr] = 1;
            rob.Memory[MemoryAddresses.hit] = 1;
        }

        public static void WriteSenses(IRobotManager robotManager, IBucketManager bucketManager, Robot rob)
        {
            LandMark(rob);

            rob.Memory[MemoryAddresses.TotalBots] = robotManager.TotalRobots;
            rob.Memory[MemoryAddresses.TOTALMYSPECIES] = SpeciesFromBot(rob).Population;

            if (!rob.CantSee && !rob.IsCorpse && bucketManager.BucketsProximity(rob, SimOpt.SimOpts.FixedBotRadii) != null)
            {
                switch (rob.LastSeenObject)
                {
                    case Robot r:
                        LookOccurr(rob, r); // It's a bot.  Populate the refvar sysvars
                        break;
                }
            }

            if (rob.Energy > 32000)
                rob.Energy = 32000;

            if (rob.OldEnergy < 0)
                rob.OldEnergy = 0;

            if (rob.OldBody < 0)
                rob.OldBody = 0;

            if (rob.Energy < 0)
                rob.Energy = 0;

            rob.Memory[MemoryAddresses.pain] = (int)(rob.OldEnergy - rob.Energy);
            rob.Memory[MemoryAddresses.pleas] = -rob.Memory[MemoryAddresses.pain];
            rob.Memory[MemoryAddresses.bodloss] = (int)(rob.OldBody - rob.Body);
            rob.Memory[MemoryAddresses.bodgain] = -rob.Memory[MemoryAddresses.bodloss];

            rob.OldEnergy = rob.Energy;
            rob.OldBody = rob.Body;
            rob.Memory[MemoryAddresses.Energy] = (int)rob.Energy;

            if (rob.Age == 0 & rob.Memory[MemoryAddresses.body] == 0)
                rob.Memory[MemoryAddresses.body] = (int)rob.Body;

            rob.Memory[215] = rob.IsFixed ? 1 : 0;

            rob.Position = DoubleVector.Max(rob.Position, 0);

            var temp = DoubleVector.Floor(rob.Position / 32000) * 32000;
            temp = rob.Position - temp;

            rob.Memory[217] = (int)(temp.Y % 32000);
            rob.Memory[219] = (int)(temp.X % 32000);
        }

        private static void EraseLookOccurr(Robot rob)
        {
            if (rob.IsCorpse)
                return;

            rob.Memory[MemoryAddresses.REFTYPE] = 0;

            for (var t = 1; t < 10; t++)
            {
                rob.Memory[MemoryAddresses.occurrstart + t] = 0;
            }

            rob.Memory[MemoryAddresses.in1] = 0;
            rob.Memory[MemoryAddresses.in2] = 0;
            rob.Memory[MemoryAddresses.in3] = 0;
            rob.Memory[MemoryAddresses.in4] = 0;
            rob.Memory[MemoryAddresses.in5] = 0;
            rob.Memory[MemoryAddresses.in6] = 0;
            rob.Memory[MemoryAddresses.in7] = 0;
            rob.Memory[MemoryAddresses.in8] = 0;
            rob.Memory[MemoryAddresses.in9] = 0;
            rob.Memory[MemoryAddresses.in10] = 0;

            rob.Memory[711] = 0;
            rob.Memory[712] = 0;
            rob.Memory[MemoryAddresses.refshell] = 0;
            rob.Memory[MemoryAddresses.refbody] = 0;
            rob.Memory[MemoryAddresses.refypos] = 0;
            rob.Memory[MemoryAddresses.refxpos] = 0;
            rob.Memory[MemoryAddresses.refvelup] = 0;
            rob.Memory[MemoryAddresses.refveldn] = 0;
            rob.Memory[MemoryAddresses.refveldx] = 0;
            rob.Memory[MemoryAddresses.refvelsx] = 0;
            rob.Memory[MemoryAddresses.refvelscalar] = 0;
            rob.Memory[713] = 0;
            rob.Memory[714] = 0;
            rob.Memory[715] = 0;
            rob.Memory[MemoryAddresses.refmulti] = 0;
            rob.Memory[473] = 0;
            rob.Memory[477] = 0;
        }

        private static void LandMark(Robot rob)
        {
            rob.Memory[MemoryAddresses.LandM] = 0;
            if (rob.Aim is > 1.39 and < 1.75)
            {
                rob.Memory[MemoryAddresses.LandM] = 1;
            }
        }
    }
}
