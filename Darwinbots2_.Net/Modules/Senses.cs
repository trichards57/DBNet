using DBNet.Forms;
using Iersera.Model;
using Iersera.Support;
using System;
using System.Linq;
using static BucketManager;
using static Globals;
using static Robots;
using static SimOpt;

internal static class Senses
{
    public static void EraseSenses(robot rob)
    {
        rob.lasttch = 0; //Botsareus 11/26/2013 Erase lasttch here
        rob.mem[hitup] = 0;
        rob.mem[hitdn] = 0;
        rob.mem[hitdx] = 0;
        rob.mem[hitsx] = 0;
        rob.mem[hit] = 0;
        rob.mem[shflav] = 0;
        rob.mem[209] = 0; //.shang
        rob.mem[shup] = 0;
        rob.mem[shdn] = 0;
        rob.mem[shdx] = 0;
        rob.mem[shsx] = 0;
        rob.mem[214] = 0; //edge collision detection
        EraseLookOccurr(rob);
    }

    public static void LookOccurr(robot rob1, robot rob2)
    {
        if (rob1.Corpse)
            return;

        rob1.mem[REFTYPE] = 0;

        for (var t = 1; t < 8; t++)
            rob1.mem[occurrstart + t] = rob2.occurr[t];

        if (!rob2.Veg && rob2.FName != rob1.FName)
        {
            //Botsareus 2/5/2014 Eye Fudge
            if (FudgeEyes || FudgeAll)
            {
                if (rob1.mem[occurrstart + 8] < 2)
                    rob1.mem[occurrstart + 8] = ThreadSafeRandom.Local.Next(1, 3);
                else
                    rob1.mem[occurrstart + 8] = rob1.mem[occurrstart + 8] + ThreadSafeRandom.Local.Next(-1, 3);
            }
            //Fudge the rest of look occurr
            if (FudgeAll)
            {
                for (var t = 1; t < 7; t++)
                {
                    if (rob1.mem[occurrstart + t] < 2)
                        rob1.mem[occurrstart + t] = ThreadSafeRandom.Local.Next(1, 3);
                    else
                        rob1.mem[occurrstart + t] = rob1.mem[occurrstart + t] + ThreadSafeRandom.Local.Next(-1, 3);
                }
            }
        }

        rob1.mem[occurrstart + 9] = (int)Math.Clamp(rob2.nrg, 0, 32000);

        if (rob2.age < 32001)
            rob1.mem[occurrstart + 10] = rob2.age; //.refage
        else
            rob1.mem[occurrstart + 10] = 32000;

        rob1.mem[in1] = rob2.mem[out1];
        rob1.mem[in2] = rob2.mem[out2];
        rob1.mem[in3] = rob2.mem[out3];
        rob1.mem[in4] = rob2.mem[out4];
        rob1.mem[in5] = rob2.mem[out5];
        rob1.mem[in6] = rob2.mem[out6];
        rob1.mem[in7] = rob2.mem[out7];
        rob1.mem[in8] = rob2.mem[out8];
        rob1.mem[in9] = rob2.mem[out9];
        rob1.mem[in10] = rob2.mem[out10];

        if (!rob2.Veg && FudgeAll && rob2.FName != rob1.FName)
        {
            if (rob2.mem[out1] != 0)
                rob1.mem[in1] = rob2.mem[out1] + ThreadSafeRandom.Local.Next(-1, 3);

            if (rob2.mem[out2] != 0)
                rob1.mem[in2] = rob2.mem[out2] + ThreadSafeRandom.Local.Next(-1, 3);

            if (rob2.mem[out3] != 0)
                rob1.mem[in3] = rob2.mem[out3] + ThreadSafeRandom.Local.Next(-1, 3);

            if (rob2.mem[out4] != 0)
                rob1.mem[in4] = rob2.mem[out4] + ThreadSafeRandom.Local.Next(-1, 3);

            if (rob2.mem[out5] != 0)
                rob1.mem[in5] = rob2.mem[out5] + ThreadSafeRandom.Local.Next(-1, 3);

            if (rob2.mem[out6] != 0)
                rob1.mem[in6] = rob2.mem[out6] + ThreadSafeRandom.Local.Next(-1, 3);

            if (rob2.mem[out7] != 0)
                rob1.mem[in7] = rob2.mem[out7] + ThreadSafeRandom.Local.Next(-1, 3);

            if (rob2.mem[out8] != 0)
                rob1.mem[in8] = rob2.mem[out8] + ThreadSafeRandom.Local.Next(-1, 3);

            if (rob2.mem[out9] != 0)
                rob1.mem[in9] = rob2.mem[out9] + ThreadSafeRandom.Local.Next(-1, 3);

            if (rob2.mem[out10] != 0)
                rob1.mem[in10] = rob2.mem[out10] + ThreadSafeRandom.Local.Next(-1, 3);
        }

        rob1.mem[711] = rob2.mem[18]; //refaim
        rob1.mem[712] = rob2.occurr[9]; //reftie

        if (!rob2.Veg && FudgeAll && rob2.FName != rob1.FName)
        {
            if (rob1.mem[712] < 2)
            {
                rob1.mem[712] = ThreadSafeRandom.Local.Next(1, 3);
            }
            else
            {
                rob1.mem[712] = ThreadSafeRandom.Local.Next(-1, 3);
            }
        }

        rob1.mem[refshell] = (int)rob2.shell;
        rob1.mem[refbody] = (int)rob2.body;
        rob1.mem[refypos] = rob2.mem[217];
        rob1.mem[refxpos] = rob2.mem[219];
        //give reference variables from the bots frame of reference
        var x = Math.Clamp(rob2.vel.X * Math.Cos(rob1.aim) + rob2.vel.Y * Math.Sin(rob1.aim) * -1 - rob1.mem[velup], -32000, 32000);
        var y = Math.Clamp(rob2.vel.Y * Math.Cos(rob1.aim) + rob2.vel.X * Math.Sin(rob1.aim) - rob1.mem[veldx], -32000, 32000);

        rob1.mem[refvelup] = (int)x;
        rob1.mem[refveldn] = rob1.mem[refvelup] * -1;
        rob1.mem[refveldx] = (int)y;
        rob1.mem[refvelsx] = rob1.mem[refvelsx] * -1;

        var temp = Math.Sqrt(Math.Pow(rob1.mem[refvelup], 2) + Math.Pow(rob1.mem[refveldx], 2)); // how fast is this robot moving compared to me?
        if (temp > 32000)
            temp = 32000;

        rob1.mem[refvelscalar] = (int)temp;
        rob1.mem[713] = rob2.mem[827]; //refpoison. current value of poison. not poison commands
        rob1.mem[714] = rob2.mem[825]; //refvenom (as with poison)
        rob1.mem[715] = rob2.Kills; //refkills
        rob1.mem[refmulti] = rob2.Multibot == true ? 1 : 0;

        if (rob1.mem[474] > 0 & rob1.mem[474] <= 1000)
        {
            //readmem and memloc couple used to read a specified memory location of the target robot
            rob1.mem[473] = rob2.mem[rob1.mem[474]];
            if (rob1.mem[474] > EyeStart && rob1.mem[474] < EyeEnd)
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
            if (rob.dna[i].tipo == 10 && rob.dna[i].value == 1)
                break;

            switch (rob.dna[i].tipo)
            {
                case 0:
                    //number
                    if (rob.dna[i + 1].tipo == 7)
                    {
                        //DNA is going to store to this value, so it's probably a sysvar
                        switch (rob.dna[i].value)
                        {
                            case > 0 and < 9:
                                //if we are dealing with one of the first 8 sysvars
                                rob.occurr[rob.dna[i].value]++; //then the occur listing for this fxn is incremented
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
                    else if (rob.dna[i].value == 330)
                    {
                        //the bot is referencing .tie 'Botsareus 11/29/2013 Moved to "." list
                        rob.occurr[9]++; //ties
                    }
                    break;

                case 1:
                    //*number
                    if (rob.dna[i].value > 500 & rob.dna[i].value < 510)
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

    public static Iersera.Model.Species SpeciesFromBot(robot rob)
    {
        return SimOpts.Specie.FirstOrDefault(s => s.Name == rob.FName);
    }

    public static void Taste(robot rob, double X, double Y, int value)
    {
        var aim = 6.28 - rob.aim;
        var xc = rob.pos.X;
        var yc = rob.pos.Y;
        var dx = X - xc;
        var dy = Y - yc;
        var ang = Math.Atan2(dy, dx);
        var dang = Physics.angnorm(ang - aim);
        var addr = dang switch
        {
            > 5.49 or <= 0.78 => shup,
            > 0.78 and <= 2.36 => shdx,
            > 2.36 and <= 3.92 => shdn,
            _ => shsx,
        };

        rob.mem[addr] = value;
        rob.mem[209] = (int)(dang * 200);
        rob.mem[shflav] = value;
    }

    public static void Touch(robot rob, int X, int Y)
    {
        var aim = 6.28 - rob.aim;
        var xc = rob.pos.X;
        var yc = rob.pos.Y;
        var dx = X - xc;
        var dy = Y - yc;
        var ang = Math.Atan2(dy, dx);
        var dang = Physics.angnorm(ang - aim);
        var addr = dang switch
        {
            > 5.49 or <= 0.78 => hitup,
            > 0.78 and <= 2.36 => hitdn,
            > 2.36 and <= 3.92 => hitdx,
            _ => hitsx,
        };
        rob.mem[addr] = 1;
        rob.mem[hit] = 1;
    }

    public static void WriteSenses(robot rob)
    {
        LandMark(rob);

        rob.mem[TotalBots] = TotalRobots;
        rob.mem[TOTALMYSPECIES] = SpeciesFromBot(rob).population;

        if (!rob.CantSee && !rob.Corpse)
        {
            if (BucketsProximity(rob) > 0)
            {
                switch (rob.lastopptype)
                {
                    case 0:
                        LookOccurr(rob, Robots.rob[rob.lastopp]); // It's a bot.  Populate the refvar sysvars
                        break;

                    case 1:
                        LookOccurrShape(rob, Obstacles.Obstacles[rob.lastopp]);
                        break;
                }
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

        rob.mem[pain] = (int)(rob.onrg - rob.nrg);
        rob.mem[pleas] = -rob.mem[pain];
        rob.mem[bodloss] = (int)(rob.obody - rob.body);
        rob.mem[bodgain] = -rob.mem[bodloss];

        rob.onrg = rob.nrg;
        rob.obody = rob.body;
        rob.mem[Energy] = (int)rob.nrg;

        if (rob.age == 0 & rob.mem[body] == 0)
            rob.mem[body] = (int)rob.body; //to stop an odd bug in birth.  Don't ask

        rob.mem[215] = rob.Fixed ? 1 : 0;

        if (rob.pos.Y < 0)
            rob.pos.Y = 0;

        var temp = Math.Floor(rob.pos.Y / Form1.instance.yDivisor / 32000) * 32000;
        temp = (rob.pos.Y / Form1.instance.yDivisor) - temp;
        rob.mem[217] = (int)(temp % 32000);

        if (rob.pos.X < 0)
            rob.pos.X = 0;

        temp = Math.Floor(rob.pos.X / Form1.instance.xDivisor / 32000) * 32000;
        temp = (rob.pos.X / Form1.instance.xDivisor) - temp;
        rob.mem[219] = (int)(temp % 32000);
    }

    private static void EraseLookOccurr(robot rob)
    {
        if (rob.Corpse)
            return;

        rob.mem[REFTYPE] = 0;

        for (var t = 1; t < 10; t++)
        {
            rob.mem[occurrstart + t] = 0;
        }

        rob.mem[in1] = 0;
        rob.mem[in2] = 0;
        rob.mem[in3] = 0;
        rob.mem[in4] = 0;
        rob.mem[in5] = 0;
        rob.mem[in6] = 0;
        rob.mem[in7] = 0;
        rob.mem[in8] = 0;
        rob.mem[in9] = 0;
        rob.mem[in10] = 0;

        rob.mem[711] = 0;
        rob.mem[712] = 0;
        rob.mem[refshell] = 0;
        rob.mem[refbody] = 0;
        rob.mem[refypos] = 0;
        rob.mem[refxpos] = 0;
        rob.mem[refvelup] = 0;
        rob.mem[refveldn] = 0;
        rob.mem[refveldx] = 0;
        rob.mem[refvelsx] = 0;
        rob.mem[refvelscalar] = 0;
        rob.mem[713] = 0;
        rob.mem[714] = 0;
        rob.mem[715] = 0;
        rob.mem[refmulti] = 0;
        rob.mem[473] = 0;
        rob.mem[477] = 0;
    }

    private static void LandMark(robot rob)
    {
        rob.mem[LandM] = 0;
        if (rob.aim > 1.39 && rob.aim < 1.75)
        {
            rob.mem[LandM] = 1;
        }
    }

    private static void LookOccurrShape(robot rob, Obstacles.Obstacle obstacle)
    {
        if (rob.Corpse)
            return;

        rob.mem[REFTYPE] = 1;

        for (var t = 1; t < 8; t++)
            rob.mem[occurrstart + t] = 0;

        rob.mem[occurrstart + 9] = 0; // refnrg
        rob.mem[occurrstart + 10] = 0; //refage

        rob.mem[in1] = 0;
        rob.mem[in2] = 0;
        rob.mem[in3] = 0;
        rob.mem[in4] = 0;
        rob.mem[in5] = 0;
        rob.mem[in6] = 0;
        rob.mem[in7] = 0;
        rob.mem[in8] = 0;
        rob.mem[in9] = 0;
        rob.mem[in10] = 0;

        rob.mem[711] = 0; //refaim
        rob.mem[712] = 0; //reftie
        rob.mem[refshell] = 0;
        rob.mem[refbody] = 0;

        rob.mem[refxpos] = (int)(rob.lastopppos.X / Form1.instance.xDivisor) % 32000;
        rob.mem[refypos] = (int)(rob.lastopppos.Y / Form1.instance.yDivisor) % 32000;

        //give reference variables from the bots frame of reference
        rob.mem[refvelup] = (int)(obstacle.vel.X * Math.Cos(rob.aim) + obstacle.vel.Y * Math.Sin(rob.aim) * -1) - rob.mem[velup];
        rob.mem[refveldn] = rob.mem[refvelup] * -1;
        rob.mem[refveldx] = (int)(obstacle.vel.Y * Math.Cos(rob.aim) + obstacle.vel.X * Math.Sin(rob.aim)) - rob.mem[veldx];
        rob.mem[refvelsx] = rob.mem[refvelsx] * -1;

        var temp = Math.Sqrt(Math.Pow(rob.mem[refvelup], 2) + Math.Pow(rob.mem[refveldx], 2)); // how fast is this shape moving compared to me?
        if (temp > 32000)
            temp = 32000;

        rob.mem[refvelscalar] = (int)temp;
        rob.mem[713] = 0; //refpoison. current value of poison. not poison commands
        rob.mem[714] = 0; //refvenom (as with poison)
        rob.mem[715] = 0; //refkills
        rob.mem[refmulti] = 0;

        rob.mem[473] = 0;
        rob.mem[477] = obstacle.vel.X == 0 && obstacle.vel.Y == 0 ? 1 : 0;
    }
}
