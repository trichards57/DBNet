using DBNet.Forms;
using static BucketManager;
using static Common;
using static Globals;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.Information;
using static Robots;
using static SimOptModule;
using static System.Math;
using static VBExtension;

internal static class Senses
{
    //     S E N S E S
    //This module is the most processor intensive.
    // Option Explicit
    // Sets .sun to 1 if robot.aim is within 0.18 radians of 1.57 (Basically up)
    // new version with less clutter.

    public static void EraseLookOccurr(int n)
    {
        if (rob[n].Corpse)
        {
            goto getout;
        }

        byte t = 0;

        rob[n].mem(REFTYPE) = 0;

        for (t = 1; t < 10; t++)
        {
            rob[n].mem(occurrstart + t) == 0;
        }

        rob[n].mem(in1) = 0;
        rob[n].mem(in2) = 0;
        rob[n].mem(in3) = 0;
        rob[n].mem(in4) = 0;
        rob[n].mem(in5) = 0;
        rob[n].mem(in6) = 0;
        rob[n].mem(in7) = 0;
        rob[n].mem(in8) = 0;
        rob[n].mem(in9) = 0;
        rob[n].mem(in10) = 0;

        rob[n].mem(711) = 0; //refaim
        rob[n].mem(712) = 0; //reftie
        rob[n].mem(refshell) = 0;
        rob[n].mem(refbody) = 0;
        rob[n].mem(refypos) = 0;
        rob[n].mem(refxpos) = 0;
        rob[n].mem(refvelup) = 0;
        rob[n].mem(refveldn) = 0;
        rob[n].mem(refveldx) = 0;
        rob[n].mem(refvelsx) = 0;
        rob[n].mem(refvelscalar) = 0;
        rob[n].mem(713) = 0; //refpoison. current value of poison. not poison commands
        rob[n].mem(714) = 0; //refvenom (as with poison)
        rob[n].mem(715) = 0; //refkills
        rob[n].mem(refmulti) = 0;
        rob[n].mem(473) = 0;
        rob[n].mem(477) = 0;
    getout:
  }

    public static void EraseSenses(int n)
    {
        int l = 0;

        dynamic _WithVar_6504;
        _WithVar_6504 = rob[n];
        _WithVar_6504.lasttch = 0; //Botsareus 11/26/2013 Erase lasttch here
        _WithVar_6504.mem(hitup) = 0;
        _WithVar_6504.mem(hitdn) = 0;
        _WithVar_6504.mem(hitdx) = 0;
        _WithVar_6504.mem(hitsx) = 0;
        _WithVar_6504.mem(hit) = 0;
        _WithVar_6504.mem(shflav) = 0;
        _WithVar_6504.mem(209) = 0; //.shang
        _WithVar_6504.mem(shup) = 0;
        _WithVar_6504.mem(shdn) = 0;
        _WithVar_6504.mem(shdx) = 0;
        _WithVar_6504.mem(shsx) = 0;
        _WithVar_6504.mem(214) = 0; //edge collision detection
        EraseLookOccurr((n));

        //EricL - *trefvars now persist across cycles
        // For l = 1 To 10 ' resets *trefvars
        //   .mem(455 + l) = 0
        // Next l
        // For l = 0 To 10     'resets
        //     .mem(trefxpos + l) = 0
        // Next l
        // .mem(472) = 0
    }

    public static void LandMark(int iRobID)
    {
        rob(iRobID).mem(LandM) = 0;
        if (rob(iRobID).aim > 1.39m && rob(iRobID).aim < 1.75m)
        {
            rob(iRobID).mem(LandM) = 1;
        }
    }

    /*
    ' touch: tells a robot whether it has been hit by another one
    ' and where: up, dn dx, sx
    */

    public static void lookoccurr(int n, int o)
    {
        if (rob[n].Corpse)
        {
            goto getout;
        }
        byte t = 0;

        decimal X = 0;

        decimal Y = 0;

        rob[n].mem(REFTYPE) = 0;

        for (t = 1; t < 8; t++)
        {
            rob[n].mem(occurrstart + t) == rob(o).occurr(t);
        }

        if (!rob(o).Veg)
        { //Botsareus 6/23/2016 Bug fix - fudging does not apply to repopulating robots
            if (rob(o).FName != rob[n].FName)
            {
                //Botsareus 2/5/2014 Eye Fudge
                if (FudgeEyes || FudgeAll)
                {
                    if (rob[n].mem(occurrstart + 8) < 2)
                    {
                        rob[n].mem(occurrstart + 8) == Int(rndy() * 2) + 1;
                    }
                    else
                    {
                        rob[n].mem(occurrstart + 8) == rob[n].mem(occurrstart + 8) + Int(rndy() * 2) * 2 - 1;
                    }
                }
                //Fudge the rest of look occurr
                if (FudgeAll)
                {
                    for (t = 1; t < 7; t++)
                    {
                        if (rob[n].mem(occurrstart + t) < 2)
                        {
                            rob[n].mem(occurrstart + t) == Int(rndy() * 2) + 1;
                        }
                        else
                        {
                            rob[n].mem(occurrstart + t) == rob[n].mem(occurrstart + t) + Int(rndy() * 2) * 2 - 1;
                        }
                    }
                }
            }
        }

        if (rob(o).nrg < 0)
        {
            rob[n].mem(occurrstart + 9) == 0;
        }
        else if (rob(o).nrg < 32001)
        {
            rob[n].mem(occurrstart + 9) == rob(o).nrg;
        }
        else
        {
            rob[n].mem(occurrstart + 9) == 32000;
        }
        //EricL 4/13/2006 Added If Then now that age can exceed 32000
        if (rob(o).age < 32001)
        {
            rob[n].mem(occurrstart + 10) == rob(o).age; //.refage
        }
        else
        {
            rob[n].mem(occurrstart + 10) == 32000;
        }

        rob[n].mem(in1) = rob(o).mem(out1);
        rob[n].mem(in2) = rob(o).mem(out2);
        rob[n].mem(in3) = rob(o).mem(out3);
        rob[n].mem(in4) = rob(o).mem(out4);
        rob[n].mem(in5) = rob(o).mem(out5);
        rob[n].mem(in6) = rob(o).mem(out6);
        rob[n].mem(in7) = rob(o).mem(out7);
        rob[n].mem(in8) = rob(o).mem(out8);
        rob[n].mem(in9) = rob(o).mem(out9);
        rob[n].mem(in10) = rob(o).mem(out10);

        if (!rob(o).Veg)
        {
            //fudge in/out
            if (FudgeAll)
            {
                if (rob(o).FName != rob[n].FName)
                {
                    if (rob(o).mem(out1) != 0)
                    {
                        rob[n].mem(in1) = rob(o).mem(out1) + Int(rndy() * 2) * 2 - 1;
                    }
                    if (rob(o).mem(out2) != 0)
                    {
                        rob[n].mem(in2) = rob(o).mem(out2) + Int(rndy() * 2) * 2 - 1;
                    }
                    if (rob(o).mem(out3) != 0)
                    {
                        rob[n].mem(in3) = rob(o).mem(out3) + Int(rndy() * 2) * 2 - 1;
                    }
                    if (rob(o).mem(out4) != 0)
                    {
                        rob[n].mem(in4) = rob(o).mem(out4) + Int(rndy() * 2) * 2 - 1;
                    }
                    if (rob(o).mem(out5) != 0)
                    {
                        rob[n].mem(in5) = rob(o).mem(out5) + Int(rndy() * 2) * 2 - 1;
                    }
                    if (rob(o).mem(out6) != 0)
                    {
                        rob[n].mem(in6) = rob(o).mem(out6) + Int(rndy() * 2) * 2 - 1;
                    }
                    if (rob(o).mem(out7) != 0)
                    {
                        rob[n].mem(in7) = rob(o).mem(out7) + Int(rndy() * 2) * 2 - 1;
                    }
                    if (rob(o).mem(out8) != 0)
                    {
                        rob[n].mem(in8) = rob(o).mem(out8) + Int(rndy() * 2) * 2 - 1;
                    }
                    if (rob(o).mem(out9) != 0)
                    {
                        rob[n].mem(in9) = rob(o).mem(out9) + Int(rndy() * 2) * 2 - 1;
                    }
                    if (rob(o).mem(out10) != 0)
                    {
                        rob[n].mem(in10) = rob(o).mem(out10) + Int(rndy() * 2) * 2 - 1;
                    }
                }
            }
        }

        rob[n].mem(711) = rob(o).mem(18); //refaim
        rob[n].mem(712) = rob(o).occurr(9); //reftie

        if (!rob(o).Veg)
        {
            //Fudge the ties
            if (FudgeAll)
            {
                if (rob(o).FName != rob[n].FName)
                {
                    if (rob[n].mem(712) < 2)
                    {
                        rob[n].mem(712) = Int(rndy() * 2) + 1;
                    }
                    else
                    {
                        rob[n].mem(712) = rob[n].mem(712) + Int(rndy() * 2) * 2 - 1;
                    }
                }
            }
        }

        rob[n].mem(refshell) = rob(o).shell;
        rob[n].mem(refbody) = rob(o).body;
        rob[n].mem(refypos) = rob(o).mem(217);
        rob[n].mem(refxpos) = rob(o).mem(219);
        //give reference variables from the bots frame of reference
        X = (rob(o).vel.X * Cos(rob[n].aim) + rob(o).vel.Y * Sin(rob[n].aim) * -1) - rob[n].mem(velup);
        Y = (rob(o).vel.Y * Cos(rob[n].aim) + rob(o).vel.X * Sin(rob[n].aim)) - rob[n].mem(veldx);
        if (X > 32000)
        {
            X = 32000;
        }
        if (X < -32000)
        {
            X = -32000;
        }
        if (Y > 32000)
        {
            Y = 32000;
        }
        if (Y < -32000)
        {
            Y = -32000;
        }

        rob[n].mem(refvelup) = X;
        rob[n].mem(refveldn) = rob[n].mem(refvelup) * -1;
        rob[n].mem(refveldx) = Y;
        rob[n].mem(refvelsx) = rob[n].mem(refvelsx) * -1;
        decimal temp = 0;

        temp = Sqr(CLng(rob[n].mem(refvelup) ^ 2) + CLng(rob[n].mem(refveldx) ^ 2)); // how fast is this robot moving compared to me?
        if (temp > 32000)
        {
            temp = 32000;
        }

        rob[n].mem(refvelscalar) = temp;
        rob[n].mem(713) = rob(o).mem(827); //refpoison. current value of poison. not poison commands
        rob[n].mem(714) = rob(o).mem(825); //refvenom (as with poison)
        rob[n].mem(715) = rob(o).Kills; //refkills
        if (rob(o).Multibot == true)
        {
            rob[n].mem(refmulti) = 1;
        }
        else
        {
            rob[n].mem(refmulti) = 0;
        }
        if (rob[n].mem(474) > 0 & rob[n].mem(474) <= 1000)
        { //readmem and memloc couple used to read a specified memory location of the target robot
            rob[n].mem(473) = rob(o[.mem(rob[n].mem(474 + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + +]);
            if (rob[n].mem(474) > EyeStart && rob[n].mem(474) < EyeEnd)
            {
                rob(o).View = true;
            }
        }
        if (rob(o).Fixed)
        { //reffixed. Tells if a viewed robot is fixed by .fixpos.
            rob[n].mem(477) = 1;
        }
        else
        {
            rob[n].mem(477) = 0;
        }
    // rob[n].mem(825) = Int(rob(o).venom)
    //rob[n].mem(827) = Int(rob(o).poison)
    getout:
    }

    public static void lookoccurrShape(int n, int o)
    {
        // bot n has shape o in it's focus eye

        if (rob[n].Corpse)
        {
            goto getout;
        }
        byte t = 0;

        rob[n].mem(REFTYPE) = 1;

        for (t = 1; t < 8; t++)
        {
            rob[n].mem(occurrstart + t) == 0;
        }

        rob[n].mem(occurrstart + 9) == 0; // refnrg

        rob[n].mem(occurrstart + 10) == 0; //refage

        rob[n].mem(in1) = 0;
        rob[n].mem(in2) = 0;
        rob[n].mem(in3) = 0;
        rob[n].mem(in4) = 0;
        rob[n].mem(in5) = 0;
        rob[n].mem(in6) = 0;
        rob[n].mem(in7) = 0;
        rob[n].mem(in8) = 0;
        rob[n].mem(in9) = 0;
        rob[n].mem(in10) = 0;

        rob[n].mem(711) = 0; //refaim
        rob[n].mem(712) = 0; //reftie
        rob[n].mem(refshell) = 0;
        rob[n].mem(refbody) = 0;

        rob[n].mem(refxpos) = CInt((rob[n].lastopppos.X / Form1.xDivisor) % 32000);
        rob[n].mem(refypos) = CInt((rob[n].lastopppos.Y / Form1.yDivisor) % 32000);

        //give reference variables from the bots frame of reference
        rob[n].mem(refvelup) = (Obstacles.Obstacles(o).vel.X * Cos(rob[n].aim) + Obstacles.Obstacles(o).vel.Y * Sin(rob[n].aim) * -1) - rob[n].mem(velup);
        rob[n].mem(refveldn) = rob[n].mem(refvelup) * -1;
        rob[n].mem(refveldx) = (Obstacles.Obstacles(o).vel.Y * Cos(rob[n].aim) + Obstacles.Obstacles(o).vel.X * Sin(rob[n].aim)) - rob[n].mem(veldx);
        rob[n].mem(refvelsx) = rob[n].mem(refvelsx) * -1;

        decimal temp = 0;

        temp = Sqr(CLng(rob[n].mem(refvelup) ^ 2) + CLng(rob[n].mem(refveldx) ^ 2)); // how fast is this shape moving compared to me?
        if (temp > 32000)
        {
            temp = 32000;
        }

        rob[n].mem(refvelscalar) = temp;
        rob[n].mem(713) = 0; //refpoison. current value of poison. not poison commands
        rob[n].mem(714) = 0; //refvenom (as with poison)
        rob[n].mem(715) = 0; //refkills
        rob[n].mem(refmulti) = 0;

        //readmem and memloc couple used to read a specified memory location of the target robot
        rob[n].mem(473) = 0;

        if (Obstacles.Obstacles(o).vel.X == 0 & Obstacles.Obstacles(o).vel.Y == 0)
        { //reffixed. Tells if a viewed robot is fixed by .fixpos.
            rob[n].mem(477) = 1;
        }
        else
        {
            rob[n].mem(477) = 0;
        }

    //  rob[n].mem(825) = 0 ' venom
    //  rob[n].mem(827) = 0 ' poison
    getout:
  }

    public static void makeoccurrlist(robot rob)
    {
        int t = 0;

        int k = 0;

        dynamic _WithVar_7514;
        _WithVar_7514 = rob[n];

        for (t = 1; t < 12; t++)
        {
            _WithVar_7514.occurr(t) = 0;
        }
        t = 1;
        k = 1;
        While(!(_WithVar_7514.dna(t).tipo == 10 & _WithVar_7514.dna(t).value == 1) && t <= 32000 & t < UBound(_WithVar_7514.dna)); //Botsareus 6/16/2012 Added code to check upper bounds

        if (_WithVar_7514.dna(t).tipo == 0)
        { //number
            if (_WithVar_7514.dna(t + 1).tipo == 7)
            { //DNA is going to store to this value, so it's probably a sysvar
                if (_WithVar_7514.dna(t).value < 8 && _WithVar_7514.dna(t).value > 0)
                { //if we are dealing with one of the first 8 sysvars
                    _WithVar_7514.occurr(_WithVar_7514.dna(t).value) = _WithVar_7514.occurr(_WithVar_7514.dna(t).value) + 1; //then the occur listing for this fxn is incremented
                }

                if (_WithVar_7514.dna(t).value == 826)
                { //referencing .strpoison
                    _WithVar_7514.occurr(10) = _WithVar_7514.occurr(10) + 1;
                }

                if (_WithVar_7514.dna(t).value == 824)
                { //refencing .strvenom
                    _WithVar_7514.occurr(11) = _WithVar_7514.occurr(11) + 1;
                }
            }

            if (_WithVar_7514.dna(t).value == 330)
            { //the bot is referencing .tie 'Botsareus 11/29/2013 Moved to "." list
                _WithVar_7514.occurr(9) = _WithVar_7514.occurr(9) + 1; //ties
            }
        }

        if (_WithVar_7514.dna(t).tipo == 1)
        { //*number
            if (_WithVar_7514.dna(t).value > 500 & _WithVar_7514.dna(t).value < 510)
            { //the bot is referencing an eye
                _WithVar_7514.occurr(8) = _WithVar_7514.occurr(8) + 1; //eyes
            }
        }

        t = t + 1;
        Wend();

        //creates the "ownvars" our own readbacks as versions of the refvars seen by others
        for (t = 1; t < 8; t++)
        {
            _WithVar_7514.mem(720 + t) == _WithVar_7514.occurr(t);
        }
        _WithVar_7514.mem(728) = _WithVar_7514.occurr(8);
        _WithVar_7514.mem(729) = _WithVar_7514.occurr(9);
        _WithVar_7514.mem(730) = _WithVar_7514.occurr(10);
        _WithVar_7514.mem(731) = _WithVar_7514.occurr(11);
    }

    public static int SpeciesFromBot(int n)
    {
        int SpeciesFromBot = 0;
        int i = 0;

        i = 0;
        While(SimOpts.Specie(i).Name != rob[n].FName && i < SimOpts.SpeciesNum);
        i = i + 1;
        Wend();
        SpeciesFromBot = i;
        return SpeciesFromBot;
    }

    public static void taste(robot rob, double X, double Y, int value)
    {
        decimal xc = 0;

        decimal yc = 0;

        decimal dx = 0;

        decimal dy = 0;

        decimal tn = 0;

        decimal ang = 0;

        decimal aim = 0;

        decimal dang = 0;

        aim = 6.28m - rob(a).aim;
        xc = rob(a).pos.X;
        yc = rob(a).pos.Y;
        dx = X - xc;
        dy = Y - yc;
        if (dx != 0)
        {
            tn = dy / dx;
            ang = Atn(tn);
            if (dx < 0)
            {
                ang = ang - 3.14m;
            }
        }
        else
        {
            ang = 1.57m * Sgn(dy);
        }
        dang = ang - aim;
        While(dang < 0);
        dang = dang + 6.28m;
        Wend();
        While(dang > 6.28m);
        dang = dang - 6.28m;
        Wend();
        if (dang > 5.49m || dang < 0.78m)
        {
            rob(a).mem(shup) = value;
        }
        if (dang > 2.36m && dang < 3.92m)
        {
            rob(a).mem(shdn) = value;
        }
        if (dang > 0.78m && dang < 2.36m)
        {
            rob(a).mem(shdx) = value;
        }
        if (dang > 3.92m && dang < 5.49m)
        {
            rob(a).mem(shsx) = value;
        }
        rob(a).mem(209) = dang * 200; //sysvar = .shang just returns the angle of the shot without the flavor
        rob(a).mem(shflav) = value; //sysvar = .shflav returns the flavor without the angle
    }

    public static void touch(int a, int X, int Y)
    {
        decimal xc = 0;

        decimal yc = 0;

        decimal dx = 0;

        decimal dy = 0;

        decimal tn = 0;

        decimal ang = 0;

        decimal aim = 0;

        decimal dang = 0;

        aim = 6.28m - rob(a).aim;
        xc = rob(a).pos.X;
        yc = rob(a).pos.Y;
        dx = X - xc;
        dy = Y - yc;

        if (dx != 0)
        {
            tn = dy / dx;
            ang = Atn(tn);
            if (dx < 0)
            {
                ang = ang - 3.14m;
            }
        }
        else
        {
            ang = 1.57m * Sgn(dy);
        }

        dang = ang - aim;
        While(dang < 0);
        dang = dang + 6.28m;
        Wend();
        While(dang > 6.28m);
        dang = dang - 6.28m;
        Wend();
        if (dang > 5.49m || dang < 0.78m)
        {
            rob(a).mem(hitup) = 1;
        }
        if (dang > 2.36m && dang < 3.92m)
        {
            rob(a).mem(hitdn) = 1;
        }
        if (dang > 0.78m && dang < 2.36m)
        {
            rob(a).mem(hitdx) = 1;
        }
        if (dang > 3.92m && dang < 5.49m)
        {
            rob(a).mem(hitsx) = 1;
        }
        rob(a).mem(hit) = 1;
    }

    /*
    ' taste: same as for touch, but for shots, and gives back
    ' also the flavour of the shot, that is, its shottype
    ' value
    */
    /*
    ' erases some senses
    */
    /*
    'Public Function BasicProximity(n As Integer, Optional force As Boolean = False) As Integer 'returns .lastopp
    '  Dim counter As Integer
    '  Dim u As vector
    '  Dim dotty As Long, crossy As Long
    '  Dim x As Integer

    '  'until I get some better data structures, this will ahve to do

    '  rob[n].lastopp = 0
    '  rob[n].lastopptype = 0 ' set the default type of object seen to a bot.
    '  rob[n].mem(EYEF) = 0
    '  For x = EyeStart + 1 To EyeEnd - 1
    '    rob[n].mem(x) = 0
    '  Next x

    '  'We have to populate eyes for every bot, even for those without .eye sysvars
    '  'since they could evolve indirect addressing of the eye sysvars.
    '  For counter = 1 To MaxRobs
    '    If n <> counter And rob(counter).exist Then
    '       CompareRobots3 n, counter
    '    End If
    '  Next counter

    '  If SimOpts.shapesAreVisable And rob[n].exist Then CompareShapes n, 12

    '  BasicProximity = rob[n].lastopp ' return the index of the last viewed object
    'End Function

    'Returns the index into the Specie array to which a given bot conforms
    */
    /*
    ' writes some senses: view, .ref* vars, absvel
    ' pain, pleas, nrg
    */

    public static void WriteSenses(int n)
    {
        int t = 0;

        int i = 0;

        decimal temp = 0;

        LandMark(n);
        dynamic _WithVar_7127;
        _WithVar_7127 = rob[n];

        _WithVar_7127.mem(TotalBots) = TotalRobots;
        _WithVar_7127.mem(TOTALMYSPECIES) = SimOpts.Specie(SpeciesFromBot(n)).population;

        if (!.CantSee && !.Corpse)
        {
            if (BucketsProximity(n) > 0)
            {
                //If BasicProximity(n) > 0 Then
                //There is somethign visable in the focus eye
                if (_WithVar_7127.lastopptype == 0)
                {
                    lookoccurr(n, _WithVar_7127.lastopp); // It's a bot.  Populate the refvar sysvars
                }
                if (_WithVar_7127.lastopptype == 1)
                {
                    lookoccurrShape(n, _WithVar_7127.lastopp);
                }
            }
        }

        //If Abs(.vel.x) > 1000 Then .vel.x = 1000 * Sgn(.vel.x) '2 new lines added to stop weird crashes
        //If Abs(.vel.y) > 1000 Then .vel.y = 1000 * Sgn(.vel.y)

        if (_WithVar_7127.nrg > 32000)
        {
            _WithVar_7127.nrg = 32000;
        }
        if (_WithVar_7127.onrg < 0)
        {
            _WithVar_7127.onrg = 0;
        }
        if (_WithVar_7127.obody < 0)
        {
            _WithVar_7127.obody = 0;
        }
        if (_WithVar_7127.nrg < 0)
        {
            _WithVar_7127.nrg = 0;
        }

        _WithVar_7127.mem(pain) = CInt(_WithVar_7127.onrg - _WithVar_7127.nrg);
        _WithVar_7127.mem(pleas) = CInt(_WithVar_7127.nrg - _WithVar_7127.onrg);
        _WithVar_7127.mem(bodloss) = CInt(_WithVar_7127.obody - _WithVar_7127.body);
        _WithVar_7127.mem(bodgain) = CInt(_WithVar_7127.body - _WithVar_7127.obody);

        _WithVar_7127.onrg = _WithVar_7127.nrg;
        _WithVar_7127.obody = _WithVar_7127.body;
        _WithVar_7127.mem(Energy) = CInt(_WithVar_7127.nrg);
        if (_WithVar_7127.age == 0 & _WithVar_7127.mem(body) == 0)
        {
            _WithVar_7127.mem(body) = _WithVar_7127.body; //to stop an odd bug in birth.  Don't ask
        }
        if (_WithVar_7127.Fixed)
        {
            _WithVar_7127.mem(215) = 1;
        }
        else
        {
            _WithVar_7127.mem(215) = 0;
        }
        if (_WithVar_7127.pos.Y < 0)
        {
            _WithVar_7127.pos.Y = 0;
        }
        temp = Int((_WithVar_7127.pos.Y / Form1.yDivisor) / 32000);
        temp = (_WithVar_7127.pos.Y / Form1.yDivisor) - (temp * 32000);
        _WithVar_7127.mem(217) = CInt(temp % 32000);
        if (_WithVar_7127.pos.X < 0)
        {
            _WithVar_7127.pos.X = 0;
        }
        temp = Int((_WithVar_7127.pos.X / Form1.xDivisor) / 32000);
        temp = (_WithVar_7127.pos.X / Form1.xDivisor) - (temp * 32000);
        _WithVar_7127.mem(219) = CInt(temp % 32000);
    }

    /*
    ' copies the occurr array of a viewed robot
    ' in the ref* vars of the viewing one
    */
    /*
    ' Erases the occurr array
    */
    /*
    ' sets up the refvars for a viewed shape
    ' in the ref* vars of the viewing one
    */
    /*
    ' creates the array which is copied to the ref* variables
    ' of any opponent looking at us
    */
}
