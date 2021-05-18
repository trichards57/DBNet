using VB6 = Microsoft.VisualBasic.Compatibility.VB6;
using System.Runtime.InteropServices;
using static VBExtension;
using static VBConstants;
using Microsoft.VisualBasic;
using System;
using System.Windows;
using System.Windows.Controls;
using static System.DateTime;
using static System.Math;
using static Microsoft.VisualBasic.Globals;
using static Microsoft.VisualBasic.Collection;
using static Microsoft.VisualBasic.Constants;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.DateAndTime;
using static Microsoft.VisualBasic.ErrObject;
using static Microsoft.VisualBasic.FileSystem;
using static Microsoft.VisualBasic.Financial;
using static Microsoft.VisualBasic.Information;
using static Microsoft.VisualBasic.Interaction;
using static Microsoft.VisualBasic.Strings;
using static Microsoft.VisualBasic.VBMath;
using System.Collections.Generic;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.ColorConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.DrawStyleConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.FillStyleConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.GlobalModule;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.Printer;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.PrinterCollection;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.PrinterObjectConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.ScaleModeConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.SystemColorConstants;
using ADODB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using DBNet.Forms;
using static stringops;
using static varspecie;
using static stayontop;
using static localizzazione;
using static SimOptModule;
using static Common;
using static Flex;
using static Robots;
using static Ties;
using static Shots_Module;
using static Globals;
using static Physics;
using static F1Mode;
using static DNAExecution;
using static Vegs;
using static Senses;
using static Multibots;
using static HDRoutines;
using static Scripts;
using static Database;
using static BucketManager;
using static NeoMutations;
using static Master;
using static DNAManipulations;
using static DNATokenizing;
using static Bitwise;
using static Obstacles;
using static Teleport;
using static IntOpts;
using static stuffcolors;
using static Evo;
using static DBNet.Forms.MDIForm1;
using static DBNet.Forms.datirob;
using static DBNet.Forms.InfoForm;
using static DBNet.Forms.ColorForm;
using static DBNet.Forms.parentele;
using static DBNet.Forms.Consoleform;
using static DBNet.Forms.frmAbout;
using static DBNet.Forms.optionsform;
using static DBNet.Forms.NetEvent;
using static DBNet.Forms.grafico;
using static DBNet.Forms.ActivForm;
using static DBNet.Forms.Form1;
using static DBNet.Forms.Contest_Form;
using static DBNet.Forms.DNA_Help;
using static DBNet.Forms.MutationsProbability;
using static DBNet.Forms.PhysicsOptions;
using static DBNet.Forms.CostsForm;
using static DBNet.Forms.EnergyForm;
using static DBNet.Forms.ObstacleForm;
using static DBNet.Forms.TeleportForm;
using static DBNet.Forms.frmGset;
using static DBNet.Forms.frmMonitorSet;
using static DBNet.Forms.frmPBMode;
using static DBNet.Forms.frmRestriOps;
using static DBNet.Forms.frmEYE;
using static DBNet.Forms.frmFirstTimeInfo;


static class Ties
{
    // Option Explicit
    // tie structure, used to represent robot ties
    //booleans = integers in space taken up.
    // the tie port, i.e. the value one must give to .tienum var to access the tie
    // the robot the tie points to
    // the back tie of the pointed robot
    // current tie angle (relative to aim)
    // angle bend value
    // the angle is fixed?
    // tie len
    // tie shrink value
    // apparently unused
    // tie timing: <-1: cycles to angle fixing; >1: cycles to tie destruction
    // Reads nrg of tied robot
    // is it a back tie?
    // for colouring the tie red in case of energy transfer
    // for colouring white in case of information transfer
    // for coloring yellow in case of sharing
    //New:
    //Spring Force = -k*displacement - velocty * b
    //b and k are constants between 0 and 1
    //for storing what forces have been applied to us.  Erase when Finished!
    //0 = damped spring, lower b and k values, birth ties here
    //1 = string (only applies force if longer than
    //"natural length", not if too short) b and k values high
    //2 = bone (very high b and k values).  (Or perhaps something better?)
    //3 = anti-rope - only applies force if shorter than
    //"natural length", not if too long) b and k values high
    public class tie
    {
        public int Port = 0;
        public int pnt = 0;
        public int ptt = 0;
        public decimal ang = 0;
        public decimal bend = 0;
        public bool angreg = false;
        public int ln = 0;
        public int shrink = 0;
        public bool stat = false;
        public int last = 0;
        public int mem = 0;
        public bool back = false;
        public bool nrgused = false;
        public bool infused = false;
        public bool sharing = false;
        public decimal Fx = 0;
        public decimal Fy = 0;
        public decimal NaturalLength = 0;
        public decimal k = 0;
        public decimal b = 0;
        public byte type = 0;
    }
    public const int MAXTIES = 10;
    //This routine deals with information transfer only. Added in to fix a major bug
    //in which older robots could transfer information to younger bots OK but
    //young bots could not transfer information to older bots in time for the information
    //to do any good


    public static void tieportcom(int t)
    {
        int k = 0;

        int tn = 0;

        //Dim tp As Integer
        //tp = tieport1

        dynamic _WithVar_8167;
        _WithVar_8167 = rob(t);
        if (!(_WithVar_8167.mem(455) != 0 & _WithVar_8167.numties > 0 & _WithVar_8167.mem(tieloc) > 0))
        {
            goto getout;
        }
        tn = _WithVar_8167.mem(TIENUM);
        k = 1;
        if (_WithVar_8167.mem(tieloc) > 0 & _WithVar_8167.mem(tieloc) < 1001)
        { //.tieloc value
            While(_WithVar_8167.Ties(k).pnt > 0);
            if (_WithVar_8167.Ties(k).Port == tn)
            {
                rob(_WithVar_8167.Ties(k).pnt).mem(_WithVar_8167.mem(tieloc)) = _WithVar_8167.mem(tieval); //stores a value in tied robot memory location (.tieloc) specified in .tieval
                if (!.Ties(k).back)
                { //forward information transfer
                    _WithVar_8167.Ties(k).infused = true; //draws tie white
                }
                else
                { //backward information transfer
                    rob(_WithVar_8167.Ties(k).pnt).Ties(_WithVar_8167.Ties(k).ptt).infused = true; //draws tie white
                }
                // .mem(tienum) = 0 ' EricL 4/24/2006 Commented out
                _WithVar_8167.mem(tieval) = 0;
                _WithVar_8167.mem(tieloc) = 0;
            }
            k = k + 1;
            Wend();
        }
    getout:
}

    public static void UpdateTieAngles(int t)
    {
        int k = 0;

        int n = 0;

        decimal tieAngle = 0;

        decimal dist = 0;

        int whichTie = 0;


        // Zero these incase no ties or tienum is non-zero, but does not refer to a tieport, etc.
        rob(t).mem(TIEANG) = 0;
        rob(t).mem(TIELEN) = 0;

        //No point in setting the length and angle if no ties!
        if (rob(t).numties <= 0)
        {
            goto getout;
        }

        //Figure if .tienum has a value.  If it's zero, use .tiepres
        if (rob(t).mem(TIENUM) != 0)
        {
            whichTie = rob(t).mem(TIENUM);
        }
        else
        {
            whichTie = rob(t).mem(TIEPRES);
        }

        if (whichTie == 0)
        {
            return;

        }

        //Now find the tie that corrosponds to either .tienum or .tiepres and set .tieang and .tielen accordingly
        //We count down through the ties to find the most recent tie with the specified tieport since more than one tie
        //can potentially have the same tieport and we want the most recent, which will be the one with the highest k.
        k = rob(t).numties;
        While(k > 0);
        if (rob(t).Ties(k).Port == whichTie)
        {
            n = rob(t).Ties(k).pnt; // The bot number of the robot on the other end of the tie
            tieAngle = angle(rob(t).pos.X, rob(t).pos.Y, rob[n].pos.X, rob[n].pos.Y);
            dist = Sqr((rob(t).pos.X - rob[n].pos.X) ^ 2 + (rob(t).pos.Y - rob[n].pos.Y) ^ 2);
            //Overflow prevention.  Very long ties can happen for one cycle when bots wrap in torridal fields
            if (dist > 32000)
            {
                dist = 32000;
            }
            rob(t).mem(TIEANG) = -CInt(AngDiff(angnorm(tieAngle), angnorm(rob(t).aim)) * 200);
            rob(t).mem(TIELEN) = CInt(dist - rob(t).radius - rob[n].radius);
            goto ;
        }
        k = k - 1;
        Wend();
    getout:
}

    /*
    ' this procedure takes care of parsing and addressing
    ' ties commands: bend, shrink, communications
    */
    public static void Update_Ties(int t)
    {
        int tp = 0;

        int tn = 0;

        int k = 0;

        decimal l = 0;

        decimal ptag = 0;

        int Length = 0;


        dynamic _WithVar_620;
        _WithVar_620 = rob(t);
        tp = tieport1;
        tn = _WithVar_620.mem(TIENUM); //.tienum value

        //this routine addresses all ties. not just ones that match .tienum
        k = 1;
        _WithVar_620.vbody = _WithVar_620.body;
        bool atleast1tie = false;

        atleast1tie = false;
        While(_WithVar_620.Ties(k).pnt > 0); //while there is a tie that points to another robot that this bot created.
        if (_WithVar_620.Multibot)
        {
            if (!.Ties(k).back)
            {
                if (_WithVar_620.mem(830) > 0)
                {
                    sharenrg(t, k);
                    _WithVar_620.Ties(k).sharing = true; //yellow ties
                }
                if (_WithVar_620.mem(831) > 0)
                {
                    sharewaste(t, k);
                    _WithVar_620.Ties(k).sharing = true; //yellow ties
                }
                if (_WithVar_620.mem(832) > 0)
                {
                    shareshell(t, k);
                    _WithVar_620.Ties(k).sharing = true; //yellow ties
                }
                if (_WithVar_620.mem(833) > 0)
                {
                    shareslime(t, k);
                    _WithVar_620.Ties(k).sharing = true; //yellow ties
                }
                if (_WithVar_620.mem(sharechlr) > 0 & _WithVar_620.Chlr_Share_Delay == 0 & !rob(t).NoChlr)
                { //Panda 8/31/2013 code to share chloroplasts 'Botsareus 8/16/2014 chloroplast sharing disable
                    sharechloroplasts(t, k);
                    _WithVar_620.Ties(k).sharing = true; //yellow ties
                }
            }
            _WithVar_620.vbody = _WithVar_620.vbody + rob(_WithVar_620.Ties(k).pnt).body;
            if (_WithVar_620.FName == rob(_WithVar_620.Ties(k).pnt).FName)
            {
                atleast1tie = true;
            }
        }
        k = k + 1;
        Wend();
        if (_WithVar_620.multibot_time > 0)
        {
            if (atleast1tie)
            {
                _WithVar_620.multibot_time = _WithVar_620.multibot_time + 1;
            }
            if (!atleast1tie)
            {
                _WithVar_620.multibot_time = _WithVar_620.multibot_time - 1;
            }
            if (_WithVar_620.multibot_time > 210)
            {
                _WithVar_620.multibot_time = 210;
            }
            if (_WithVar_620.multibot_time < 10)
            {
                rob(t).Dead = true; //safe kill robot
            }
        }

        // Zero the sharing sysvars
        _WithVar_620.mem(830) = 0;
        _WithVar_620.mem(831) = 0;
        _WithVar_620.mem(832) = 0;
        _WithVar_620.mem(833) = 0;
        _WithVar_620.mem(sharechlr) = 0;

        _WithVar_620.numties = k - 1; // Set the number of ties.
        _WithVar_620.mem(numties) = _WithVar_620.numties; //places a value in the memory cell .numties for number of ties attached to a robot

        if (_WithVar_620.numties == 0)
        {
            _WithVar_620.Multibot = false;
            _WithVar_620.mem(multi) = 0;
            goto ;
        }

        // Handle the deltie sysvar.  Bot is trying to delete one or more ties
        if (_WithVar_620.mem(DELTIE) != 0)
        {
            for (k = 1; k < _WithVar_620.numties; k++)
            {
                if (_WithVar_620.Ties(k).pnt > 0 & _WithVar_620.Ties(k).Port == _WithVar_620.mem(tp + 17))
                {
                    DeleteTie(t, _WithVar_620.Ties(k).pnt);
                }
            }
            _WithVar_620.mem(DELTIE) = 0; //resets .deltie command
        }

        //Botsareus 2/21/2013 Broken
        //    If .mem(480) <> 32000 Then
        //      .Ties(1).ang = .mem(480) / 200
        //    End If
        //    If .mem(481) <> 32000 Then
        //      .Ties(2).ang = .mem(481) / 200
        //    End If
        //    If .mem(482) <> 32000 Then
        //      .Ties(3).ang = .mem(482) / 200
        //    End If
        //    If .mem(483) <> 32000 Then
        //      .Ties(4).ang = .mem(483) / 200
        //    End If
        //    If .mem(484) > RobSize And .mem(484) > RobSize Then 'set tie 1 length
        //      .Ties(1).ln = .mem(484)
        //      rob(.Ties(1).pnt).Ties(srctie((.Ties(1).pnt), t)).ln = .mem(484)
        //    End If
        //    If .mem(485) > RobSize And .mem(485) > RobSize Then 'set tie 2 length
        //      .Ties(2).ln = .mem(485)
        //      rob(.Ties(2).pnt).Ties(srctie((.Ties(2).pnt), t)).ln = .mem(485)
        //    End If
        //    If .mem(486) > RobSize And .mem(486) > RobSize Then 'set tie 3 length
        //      .Ties(3).ln = .mem(485)
        //      rob(.Ties(3).pnt).Ties(srctie((.Ties(3).pnt), t)).ln = .mem(486)
        //    End If
        //    If .mem(487) > RobSize And .mem(487) > RobSize Then 'set tie 4 length
        //      .Ties(4).ln = .mem(487)
        //      rob(.Ties(4).pnt).Ties(srctie((.Ties(4).pnt), t)).ln = .mem(487)
        //    End If

        if (tn == 0)
        {
            tn = _WithVar_620.mem(TIEPRES);
        }
        if (tn == 0)
        {
            goto getout;
        }
        // If tn Then  'routines only carried if .tienum has a value

        k = 1;
        While(_WithVar_620.Ties(k).pnt > 0 & k < MAXTIES);
        if (_WithVar_620.Multibot && _WithVar_620.Ties(k).type == 3)
        { // Has to be a multibot and tie has to have hardened

            //FixAng - fixes tie angle
            //Positive values fix the tie angle
            //Negative values allow the tie to pivot freely
            if (_WithVar_620.mem(FIXANG) != 32000 & _WithVar_620.Ties(k).Port == tn)
            {
                if (_WithVar_620.mem(FIXANG) >= 0)
                {
                    _WithVar_620.Ties(k).ang = (_WithVar_620.mem(FIXANG) % 1256) / 200;
                    _WithVar_620.Ties(k).angreg = true; //EricL 4/24/2006
                                                        //If .mem(FIXANG) > 628 Then .mem(FIXANG) = -627
                                                        //If .mem(FIXANG) < -628 Then .mem(FIXANG) = 627
                }
                else
                {
                    _WithVar_620.Ties(k).angreg = false; //EricL 4/24/2006
                }
            }

            //TieLen Section
            if (_WithVar_620.mem(FIXLEN) != 0 & _WithVar_620.Ties(k).Port == tn)
            { //fixes tie length
              //length = Abs(.mem(FIXLEN))
                Length = Abs(_WithVar_620.mem(FIXLEN)) + _WithVar_620.radius + rob(_WithVar_620.Ties(k).pnt).radius; // include the radius of the tied bots in the length
                if (Length > 32000)
                {
                    Length = 32000; // Can happen for very big bots with very long ties.
                }
                _WithVar_620.Ties(k).NaturalLength = CInt(Length); //for first robot
                rob(_WithVar_620.Ties(k).pnt).Ties(srctie((_WithVar_620.Ties(k).pnt), t)).NaturalLength == CInt(Length); //for second robot. What a messed up formula
            }

            //EricL 5/7/2006 Added Stifftie section.  This never made it into the 2.4 code
            if (_WithVar_620.mem(stifftie) != 0 & _WithVar_620.Ties(k).Port == tn)
            {
                _WithVar_620.mem(stifftie) = _WithVar_620.mem(stifftie) % 100;
                if (_WithVar_620.mem(stifftie) == 0)
                {
                    _WithVar_620.mem(stifftie) = 100;
                }
                if (_WithVar_620.mem(stifftie) < 0)
                {
                    _WithVar_620.mem(stifftie) = 1;
                }
                _WithVar_620.Ties(k).b = 0.005m * _WithVar_620.mem(stifftie); // May need to tweak the multiplier here vares from 0.0025 to .1
                rob(_WithVar_620.Ties(k).pnt).Ties(srctie((_WithVar_620.Ties(k).pnt), t)).b == 0.005m * _WithVar_620.mem(stifftie); // May need to tweak the multiplier here
                _WithVar_620.Ties(k).k = 0.0025m * _WithVar_620.mem(stifftie); //May need to tweak the multiplier here:  varies from 0.00125 to 0.05
                rob(_WithVar_620.Ties(k).pnt).Ties(srctie((_WithVar_620.Ties(k).pnt), t)).k == 0.0025m * _WithVar_620.mem(stifftie); // May need to tweak the multiplier here: varies from 0.00125 to 0.05
            }
        }
        k = k + 1;
        Wend();

        _WithVar_620.mem(FIXANG) = 32000;
        _WithVar_620.mem(FIXLEN) = 0;
        _WithVar_620.mem(stifftie) = 0;
        k = 1;


        //Botsareus 3/22/2013 Complete fix for tielen...tieang 1...4
        if (_WithVar_620.Multibot)
        { //check for multibot first

            for (k = 1; k < 4; k++)
            {
                if (_WithVar_620.Ties(k).pnt > 0 & _WithVar_620.Ties(k).type == 3)
                {
                    //input
                    if (_WithVar_620.TieLenOverwrite(k - 1))
                    {
                        Length = _WithVar_620.mem(483 + k) + _WithVar_620.radius + rob(_WithVar_620.Ties(k).pnt).radius; // include the radius of the tied bots in the length
                        if (Length > 32000)
                        {
                            Length = 32000; // Can happen for very big bots with very long ties.
                        }
                        _WithVar_620.Ties(k).NaturalLength = CInt(Length); //for first robot
                        rob(_WithVar_620.Ties(k).pnt).Ties(srctie((_WithVar_620.Ties(k).pnt), t)).NaturalLength == CInt(Length); //for second robot. What a messed up formula
                    }
                    if (_WithVar_620.TieAngOverwrite(k - 1))
                    {
                        _WithVar_620.Ties(k).ang = angnorm(_WithVar_620.mem(479 + k) / 200);
                        _WithVar_620.Ties(k).angreg = true; //EricL 4/24/2006
                    }
                    //clear input
                    _WithVar_620.TieAngOverwrite(k - 1) == false;
                    _WithVar_620.TieLenOverwrite(k - 1) == false;
                    //output
                    int n = 0;

                    decimal dist = 0;

                    decimal tieAngle = 0;

                    n = _WithVar_620.Ties(k).pnt;
                    tieAngle = angle(_WithVar_620.pos.X, _WithVar_620.pos.Y, rob[n].pos.X, rob[n].pos.Y);
                    dist = Sqr((_WithVar_620.pos.X - rob[n].pos.X) ^ 2 + (_WithVar_620.pos.Y - rob[n].pos.Y) ^ 2);
                    if (dist > 32000)
                    {
                        dist = 32000; //Botsareus 1/24/2014 Bug fix here
                    }
                    _WithVar_620.mem(483 + k) == CInt(dist - _WithVar_620.radius - rob[n].radius);
                    _WithVar_620.mem(479 + k) == angnorm(angnorm(tieAngle) - angnorm(_WithVar_620.aim)) * 200;
                }
            }

        }


        k = 1;

        //      If .mem(tp) Then  '.tieang value
        //        k = 1
        //        While .Ties(k).pnt > 0
        //          If .Ties(k).Port = tn Then bend t, k, .mem(tp) 'bend a tie
        //          k = k + 1
        //        Wend
        //      End If

        //      If .mem(tp + 1) Then  '.tielen value
        //        k = 1
        //        While .Ties(k).pnt > 0
        //          If .Ties(k).Port = tn Then shrink t, k, .mem(FIXLEN) 'set tie length to value specified in mem location 451 (tp+1)
        //          k = k + 1
        //        Wend
        //      End If


        //Botsareus 7/22/2015 Code more coherent
        if (_WithVar_620.mem(tp + 2) < 0)
        { //we are checking values that are negative such as -1 or -6

            if (_WithVar_620.mem(tp + 2) == -1 && _WithVar_620.mem(tp + 3) != 0)
            { //tieloc = -1 and .tieval not zero

                l = _WithVar_620.mem(tp + 3); // l is amount of energy to exchange, positive to give nrg away, negative to take it

                //Limits on Tie feeding as a function of body attempting to do the feeding (or sharing)
                if (_WithVar_620.body < 0)
                {
                    l = 0; // If your body has gone negative, you can't take or give nrg.
                }
                if (_WithVar_620.nrg < 0)
                {
                    l = 0; // If you nrg has gone negative, you can't take or give nrg.
                }
                if (_WithVar_620.age == 0)
                {
                    l = 0; // The just born can't trasnfer nrg
                }
                if (l > 1000)
                {
                    l = 1000; // Upper limt on sharing
                }
                if (l < -3000)
                {
                    l = -3000; // Upper limit on tie feeding
                }

                k = 1;
                While(_WithVar_620.Ties(k).pnt > 0); //tie actually points at something
                if (_WithVar_620.Ties(k).Port == tn)
                { //tienum indicates this tie

                    //Giving nrg away
                    if (l > 0)
                    {
                        if (l > _WithVar_620.nrg)
                        {
                            l = _WithVar_620.nrg; // Can't give away more nrg than you have
                        }

                        rob(_WithVar_620.Ties(k).pnt).nrg = rob(_WithVar_620.Ties(k).pnt).nrg + l * 0.7m; //tied robot receives energy
                        if (rob(_WithVar_620.Ties(k).pnt).nrg > 32000)
                        {
                            rob(_WithVar_620.Ties(k).pnt).nrg = 32000;
                        }
                        rob(_WithVar_620.Ties(k).pnt).body = rob(_WithVar_620.Ties(k).pnt).body + l * 0.029m; //tied robot stores some fat
                        if (rob(_WithVar_620.Ties(k).pnt).body > 32000)
                        {
                            rob(_WithVar_620.Ties(k).pnt).body = 32000;
                        }
                        rob(_WithVar_620.Ties(k).pnt).Waste = rob(_WithVar_620.Ties(k).pnt).Waste + l * 0.01m; //tied robot receives waste
                        rob(_WithVar_620.Ties(k).pnt).radius = FindRadius(_WithVar_620.Ties(k).pnt);

                        _WithVar_620.nrg = _WithVar_620.nrg - l; //tying robot gives up energy

                        if ((SimOpts.F1 || x_restartmode == 1) && Disqualify == 1 && _WithVar_620.FName != rob(_WithVar_620.Ties(k).pnt).FName)
                        {
                            dreason(_WithVar_620.FName, _WithVar_620.tag, "giving energy to opponent");
                        }
                        if (!SimOpts.F1 && _WithVar_620.dq == 1 && Disqualify == 1 && _WithVar_620.FName != rob(_WithVar_620.Ties(k).pnt).FName)
                        {
                            rob(t).Dead = true; //safe kill robot
                        }

                    }

                    //Taking nrg
                    if (l < 0)
                    {
                        if (l < -rob(_WithVar_620.Ties(k).pnt).nrg)
                        {
                            l = -rob(_WithVar_620.Ties(k).pnt).nrg; // Can't give away more nrg than you have
                        }

                        //Poison
                        ptag = Abs(l / 4);
                        if (rob(_WithVar_620.Ties(k).pnt).poison > ptag)
                        { //target robot has poison
                            if (rob(_WithVar_620.Ties(k).pnt).FName != _WithVar_620.FName)
                            { //can't poison your brother

                                _WithVar_620.Poisoned = true;
                                _WithVar_620.Poisoncount = _WithVar_620.Poisoncount + ptag;
                                if (_WithVar_620.Poisoncount > 32000)
                                {
                                    _WithVar_620.Poisoncount = 32000;
                                }

                                l = 0;

                                rob(_WithVar_620.Ties(k).pnt).poison = rob(_WithVar_620.Ties(k).pnt).poison - ptag;
                                rob(_WithVar_620.Ties(k).pnt).mem(827) = rob(_WithVar_620.Ties(k).pnt).poison;

                                if (rob(_WithVar_620.Ties(k).pnt).mem(834) > 0)
                                {
                                    _WithVar_620.Ploc = ((rob(_WithVar_620.Ties(k).pnt).mem(834) - 1) % 1000) + 1; //sets .Ploc to targets .mem(ploc) EricL - 3/29/2006 Added Mod to fix overflow
                                    if (_WithVar_620.Ploc == 340)
                                    {
                                        _WithVar_620.Ploc = 0;
                                    }
                                }
                                else
                                {
                                    do
                                    {
                                        _WithVar_620.Ploc = Random(1, 1000);
                                    } while (!(_WithVar_620.Ploc != 340);
                                }

                                _WithVar_620.Pval = rob(_WithVar_620.Ties(k).pnt).mem(839);

                            }
                        }

                        _WithVar_620.nrg = _WithVar_620.nrg - l * 0.7m; //tying robot receives energy
                        if (_WithVar_620.nrg > 32000)
                        {
                            _WithVar_620.nrg = 32000;
                        }
                        _WithVar_620.body = _WithVar_620.body - l * 0.029m; //tying robot stores some fat
                        if (_WithVar_620.body > 32000)
                        {
                            _WithVar_620.body = 32000;
                        }
                        _WithVar_620.Waste = _WithVar_620.Waste - l * 0.01m; //tying robot adds waste
                        _WithVar_620.radius = FindRadius(t);

                        rob(_WithVar_620.Ties(k).pnt).nrg = rob(_WithVar_620.Ties(k).pnt).nrg + l; //Take the nrg

                        if (rob(_WithVar_620.Ties(k).pnt).nrg <= 0 & rob(_WithVar_620.Ties(k).pnt).Dead == false)
                        { //Botsareus 3/11/2014 Tie feeding kills
                            if (!rob(_WithVar_620.Ties(k).pnt).Corpse)
                            { //Botsareus 7/17/2016 Bug fix to prevent logging infinate kills against a corpse
                                _WithVar_620.Kills = _WithVar_620.Kills + 1;
                                if (_WithVar_620.Kills > 32000)
                                {
                                    _WithVar_620.Kills = 32000;
                                }
                                _WithVar_620.mem(220) = _WithVar_620.Kills;
                            }
                        }

                        if ((SimOpts.F1 || x_restartmode == 1) && Disqualify == 1 && _WithVar_620.FName != rob(_WithVar_620.Ties(k).pnt).FName)
                        {
                            dreason(_WithVar_620.FName, _WithVar_620.tag, "taking energy from opponent");
                        }
                        if (!SimOpts.F1 && _WithVar_620.dq == 1 && Disqualify == 1 && _WithVar_620.FName != rob(_WithVar_620.Ties(k).pnt).FName)
                        {
                            rob(t).Dead = true; //safe kill robot
                        }

                    }

                    if (!.Ties(k).back)
                    { //forward ties
                        _WithVar_620.Ties(k).nrgused = true; //red ties
                    }
                    else
                    { //backward ties
                        rob(_WithVar_620.Ties(k).pnt).Ties(_WithVar_620.Ties(k).ptt).nrgused = true; //red ties
                    }

                }

                k = k + 1;
                Wend();
            }

            if (_WithVar_620.mem(tp + 2) == -3 && _WithVar_620.mem(tp + 3) != 0)
            { //inject or steal venom

                l = _WithVar_620.mem(tp + 3); //amount of venom to take or inject

                //limits on injecting or taking venum
                if (l > 100)
                {
                    l = 100;
                }
                if (l < -100)
                {
                    l = -100;
                }

                k = 1;
                While(_WithVar_620.Ties(k).pnt > 0);
                if (_WithVar_620.Ties(k).Port == tn)
                {
                    if (l > _WithVar_620.venom)
                    {
                        l = _WithVar_620.venom;
                    }

                    if (l > 0)
                    { //works the same as a venom injection

                        rob(_WithVar_620.Ties(k).pnt).Paracount = rob(_WithVar_620.Ties(k).pnt).Paracount + l; //paralysis counter set
                        if (rob(_WithVar_620.Ties(k).pnt).Paracount > 32000)
                        {
                            rob(_WithVar_620.Ties(k).pnt).Paracount = 32000;
                        }
                        rob(_WithVar_620.Ties(k).pnt).Paralyzed = true; //robot paralyzed

                        if (_WithVar_620.mem(835) > 0)
                        {
                            rob(_WithVar_620.Ties(k).pnt).Vloc = ((_WithVar_620.mem(835) - 1) % 1000) + 1;
                            if (rob(_WithVar_620.Ties(k).pnt).Vloc == 340)
                            {
                                rob(_WithVar_620.Ties(k).pnt).Vloc = 0;
                            }
                        }
                        else
                        {
                            do
                            {
                                rob(_WithVar_620.Ties(k).pnt).Vloc = Random(1, 1000);
                            } while (!(rob(_WithVar_620.Ties(k).pnt).Vloc != 340);
                        }

                        rob(_WithVar_620.Ties(k).pnt).Vval = _WithVar_620.mem(836);
                        _WithVar_620.venom = _WithVar_620.venom - l;
                        _WithVar_620.mem(825) = _WithVar_620.venom;

                    }

                    if (l < 0)
                    { //Taking venom

                        if (l < -rob(_WithVar_620.Ties(k).pnt).venom)
                        {
                            l = -rob(_WithVar_620.Ties(k).pnt).venom; // Can't give away more venom than you have
                        }

                        //robot steals venom from tied target
                        rob(_WithVar_620.Ties(k).pnt).venom = rob(_WithVar_620.Ties(k).pnt).venom + l;
                        _WithVar_620.venom = _WithVar_620.venom - l;
                        if (_WithVar_620.venom > 32000)
                        {
                            _WithVar_620.venom = 32000;
                        }
                        _WithVar_620.mem(825) = _WithVar_620.venom;

                    }

                    if (!.Ties(k).back)
                    { //forward ties
                        _WithVar_620.Ties(k).nrgused = true; //red ties
                    }
                    else
                    { //backward ties
                        rob(_WithVar_620.Ties(k).pnt).Ties(_WithVar_620.Ties(k).ptt).nrgused = true; //red ties
                    }

                }

                k = k + 1;
                Wend();
            }

            if (_WithVar_620.mem(tp + 2) == -4 && _WithVar_620.mem(tp + 3) != 0)
            { //trade waste via ties

                l = _WithVar_620.mem(tp + 3); // l is amount of waste to exchange, positive to give waste away, negative to take it

                //limits on giving or taking waste
                if (l > 1000)
                {
                    l = 1000;
                }
                if (l < -1000)
                {
                    l = -1000;
                }

                k = 1;
                While(_WithVar_620.Ties(k).pnt > 0);
                if (_WithVar_620.Ties(k).Port == tn)
                {
                    //giving waste away
                    if (l > 0)
                    {
                        if (l > _WithVar_620.Waste)
                        {
                            l = _WithVar_620.Waste;
                        }

                        rob(_WithVar_620.Ties(k).pnt).Waste = rob(_WithVar_620.Ties(k).pnt).Waste + l * 0.99m;
                        _WithVar_620.Waste = _WithVar_620.Waste - l;
                        _WithVar_620.Pwaste = _WithVar_620.Pwaste + l * 0.01m; //some waste is converted into perminent waste rather then given away

                    }

                    //taking waste
                    if (l < 0)
                    {
                        if (l < -rob(_WithVar_620.Ties(k).pnt).Waste)
                        {
                            l = -rob(_WithVar_620.Ties(k).pnt).Waste;
                        }

                        _WithVar_620.Waste = _WithVar_620.Waste - l * 0.99m; //robot reseaves waste from opponent
                        rob(_WithVar_620.Ties(k).pnt).Waste = rob(_WithVar_620.Ties(k).pnt).Waste + l; //opponent losing some waste
                        rob(_WithVar_620.Ties(k).pnt).Pwaste = rob(_WithVar_620.Ties(k).pnt).Pwaste - l * 0.01m; //some waste is converted into perminent waste rather then given away

                    }

                    if (!.Ties(k).back)
                    { //forward ties
                        _WithVar_620.Ties(k).nrgused = true; //red ties
                    }
                    else
                    { //backward ties
                        rob(_WithVar_620.Ties(k).pnt).Ties(_WithVar_620.Ties(k).ptt).nrgused = true; //red ties
                    }

                }

                k = k + 1;
                Wend();
            }

            if (_WithVar_620.mem(tp + 2) == -6 && _WithVar_620.mem(tp + 3) != 0)
            { //tieloc = -6 and .tieval not zero

                l = _WithVar_620.mem(tp + 3); // l is amount of body to exchange, positive to give body away, negative to take it

                //Limits on Tie feeding as a function of body attempting to do the feeding (or sharing)
                if (_WithVar_620.body < 0)
                {
                    l = 0; // If your body has gone negative, you can't take or give body.
                }
                if (_WithVar_620.nrg < 0)
                {
                    l = 0; // If you nrg has gone negative, you can't take or give body.
                }
                if (_WithVar_620.age == 0)
                {
                    l = 0; // The just born can't trasnfer body
                }
                if (l > 100)
                {
                    l = 100; // Upper limt on sharing
                }
                if (l < -300)
                {
                    l = -300; // Upper limit on tie feeding
                }

                k = 1;
                While(_WithVar_620.Ties(k).pnt > 0); //tie actually points at something
                if (_WithVar_620.Ties(k).Port == tn)
                { //tienum indicates this tie

                    //Giving body away
                    if (l > 0)
                    {
                        if (l > _WithVar_620.body)
                        {
                            l = _WithVar_620.body; // Can't give away more body than you have
                        }

                        rob(_WithVar_620.Ties(k).pnt).nrg = rob(_WithVar_620.Ties(k).pnt).nrg + l * 0.03m; //tied robot receives energy
                        if (rob(_WithVar_620.Ties(k).pnt).nrg > 32000)
                        {
                            rob(_WithVar_620.Ties(k).pnt).nrg = 32000;
                        }
                        rob(_WithVar_620.Ties(k).pnt).body = rob(_WithVar_620.Ties(k).pnt).body + l * 0.987m; //tied robot stores some fat 'Botsareus 3/23/2016 Bugfix
                        if (rob(_WithVar_620.Ties(k).pnt).body > 32000)
                        {
                            rob(_WithVar_620.Ties(k).pnt).body = 32000;
                        }
                        rob(_WithVar_620.Ties(k).pnt).Waste = rob(_WithVar_620.Ties(k).pnt).Waste + l * 0.01m; //tied robot receives waste
                        rob(_WithVar_620.Ties(k).pnt).radius = FindRadius(_WithVar_620.Ties(k).pnt);

                        _WithVar_620.body = _WithVar_620.body - l; //tying robot gives up energy

                        if ((SimOpts.F1 || x_restartmode == 1) && Disqualify == 1 && _WithVar_620.FName != rob(_WithVar_620.Ties(k).pnt).FName)
                        {
                            dreason(_WithVar_620.FName, _WithVar_620.tag, "giving body to opponent");
                        }
                        if (!SimOpts.F1 && _WithVar_620.dq == 1 && Disqualify == 1 && _WithVar_620.FName != rob(_WithVar_620.Ties(k).pnt).FName)
                        {
                            rob(t).Dead = true; //safe kill robot
                        }

                    }

                    //Taking body
                    if (l < 0)
                    {
                        if (l < -rob(_WithVar_620.Ties(k).pnt).body)
                        {
                            l = -rob(_WithVar_620.Ties(k).pnt).body; // Can't give away more body than you have
                        }

                        //Poison (Yes tiefeeding body is a reason enough to get poisoned)
                        ptag = Abs(l / 4);
                        if (rob(_WithVar_620.Ties(k).pnt).poison > ptag)
                        { //target robot has poison
                            if (rob(_WithVar_620.Ties(k).pnt).FName != _WithVar_620.FName)
                            { //can't poison your brother

                                _WithVar_620.Poisoned = true;
                                _WithVar_620.Poisoncount = _WithVar_620.Poisoncount + ptag;
                                if (_WithVar_620.Poisoncount > 32000)
                                {
                                    _WithVar_620.Poisoncount = 32000;
                                }

                                l = 0;

                                rob(_WithVar_620.Ties(k).pnt).poison = rob(_WithVar_620.Ties(k).pnt).poison - ptag;
                                rob(_WithVar_620.Ties(k).pnt).mem(827) = rob(_WithVar_620.Ties(k).pnt).poison;

                                if (rob(_WithVar_620.Ties(k).pnt).mem(834) > 0)
                                {
                                    _WithVar_620.Ploc = ((rob(_WithVar_620.Ties(k).pnt).mem(834) - 1) % 1000) + 1; //sets .Ploc to targets .mem(ploc) EricL - 3/29/2006 Added Mod to fix overflow
                                    if (_WithVar_620.Ploc == 340)
                                    {
                                        _WithVar_620.Ploc = 0;
                                    }
                                }
                                else
                                {
                                    do
                                    {
                                        _WithVar_620.Ploc = Random(1, 1000);
                                    } while (!(_WithVar_620.Ploc != 340);
                                }

                                _WithVar_620.Pval = rob(_WithVar_620.Ties(k).pnt).mem(839);

                            }
                        }

                        _WithVar_620.nrg = _WithVar_620.nrg - l * 0.03m; //tying robot receives energy
                        if (_WithVar_620.nrg > 32000)
                        {
                            _WithVar_620.nrg = 32000;
                        }
                        _WithVar_620.body = _WithVar_620.body - l * 0.987m; //tying robot stores some fat 'Botsareus 3/23/2016 Bugfix
                        if (_WithVar_620.body > 32000)
                        {
                            _WithVar_620.body = 32000;
                        }
                        _WithVar_620.Waste = _WithVar_620.Waste - l * 0.01m; //tying robot adds waste
                        _WithVar_620.radius = FindRadius(t);

                        rob(_WithVar_620.Ties(k).pnt).body = rob(_WithVar_620.Ties(k).pnt).body + l; //Take the body

                        if (rob(_WithVar_620.Ties(k).pnt).body <= 0 & rob(_WithVar_620.Ties(k).pnt).Dead == false)
                        { //Botsareus 3/11/2014 Tie feeding kills
                            _WithVar_620.Kills = _WithVar_620.Kills + 1;
                            if (_WithVar_620.Kills > 32000)
                            {
                                _WithVar_620.Kills = 32000;
                            }
                            _WithVar_620.mem(220) = _WithVar_620.Kills;
                        }

                        if ((SimOpts.F1 || x_restartmode == 1) && Disqualify == 1 && _WithVar_620.FName != rob(_WithVar_620.Ties(k).pnt).FName)
                        {
                            dreason(_WithVar_620.FName, _WithVar_620.tag, "taking body from opponent");
                        }
                        if (!SimOpts.F1 && _WithVar_620.dq == 1 && Disqualify == 1 && _WithVar_620.FName != rob(_WithVar_620.Ties(k).pnt).FName)
                        {
                            rob(t).Dead = true; //safe kill robot
                        }

                    }

                    if (!.Ties(k).back)
                    { //forward ties
                        _WithVar_620.Ties(k).nrgused = true; //red ties
                    }
                    else
                    { //backward ties
                        rob(_WithVar_620.Ties(k).pnt).Ties(_WithVar_620.Ties(k).ptt).nrgused = true; //red ties
                    }

                }

                k = k + 1;
                Wend();
            }

            _WithVar_620.mem(tp + 2) == 0;
            _WithVar_620.mem(tp + 3) == 0;
        }


        _WithVar_620.mem(tp + 5) == 0; // .tienum should be reset every cycle
    getout:
  }

    public static void EraseTRefVars(int t)
    {
        int counter = 0;


        dynamic _WithVar_5599;
        _WithVar_5599 = rob(t);
        // Zero the trefvars as all ties have gone.  Perf -> Could set a flag to not do this everytime
        for (counter = 456; counter < 465; counter++)
        {
            _WithVar_5599.mem(counter) = 0;
        }
        _WithVar_5599.mem(trefbody) = 0; //trefbody
        _WithVar_5599.mem(475) = 0; //tmemval
                                    // .mem(476) = 0       'tmemloc EricL 4/20/2006 Commented out.  Should persist even when tie goes away or no tienum is specified
        _WithVar_5599.mem(478) = 0; //treffixed
        _WithVar_5599.mem(479) = 0; //trefaim
        for (counter = 0; counter < 10; counter++)
        { //For trefvelX functions.
            _WithVar_5599.mem(trefxpos + counter) == 0;
        }

        //These are .tin trefvars
        for (counter = 420; counter < 429; counter++)
        {
            _WithVar_5599.mem(counter) = 0;
        }
    }

    public static void readtie(int t)
    { //Botsareus 2/11/2014 Bug fix

        //reads all of the tref variables from a given tie number
        int k = 0;

        int tn = 0;

        dynamic counter = null;


        dynamic _WithVar_6750;
        _WithVar_6750 = rob(t);
        if (rob(t).newage < 2)
        {
            return;//Botsareus 3/6/2013 Bug fix: Robot must be fully loaded before checking ties

        }

        if (_WithVar_6750.numties == 0)
        {
            EraseTRefVars((t));
            goto ;
        }
        else
        {
            // If there is a value in .readtie then get the trefvars from that tie else read the trefvars from the last tie created
            if (_WithVar_6750.mem(471) != 0)
            {
                tn = _WithVar_6750.mem(471); // .readtie
            }
            else
            {
                tn = _WithVar_6750.mem(454); // .tiepres
            }
            k = 1;
            While(_WithVar_6750.Ties(k).pnt > 0);
            if (_WithVar_6750.Ties(k).Port == tn)
            {
                ReadTRefVars(t, k);
                goto ;
            }
            k = k + 1;
            Wend();
            //If we got here, no tie exists with this number.
            EraseTRefVars((t)); // Zero the trefvars.  The bot might be checking if the tie still exists.
        }
    getout:
}

    /*
    'EricL 4/20/2006  This was the heart of readtie.  Seperated it so Trefvars can be loaded when tie is formed.
    'Reads the Tie Refvars for tie k into the mem of bot t
    */
    public static void ReadTRefVars(int t, int k)
    {
        int l = 0;// just a loop counter


        dynamic _WithVar_7541;
        _WithVar_7541 = rob(t);
        if (rob(_WithVar_7541.Ties(k).pnt).nrg < 32000 & rob(_WithVar_7541.Ties(k).pnt).nrg > -32000)
        {
            _WithVar_7541.mem(464) = CInt(rob(_WithVar_7541.Ties(k).pnt).nrg); //copies tied robot's energy into memory cell *trefnrg
        }
        if (rob(_WithVar_7541.Ties(k).pnt).age < 32000)
        {
            _WithVar_7541.mem(465) = rob(_WithVar_7541.Ties(k).pnt).age + 1; //copies age of tied robot into *refvar
        }
        else
        {
            _WithVar_7541.mem(465) = 32000;
        }
        if (rob(_WithVar_7541.Ties(k).pnt).body < 32000 & rob(_WithVar_7541.Ties(k).pnt).body > -32000)
        {
            _WithVar_7541.mem(trefbody) = CInt(rob(_WithVar_7541.Ties(k).pnt).body); //copies tied robot's body value
        }
        else
        {
            _WithVar_7541.mem(trefbody) = 32000;
        }

        for (l = 1; l < 8; l++)
        { //copies all vars from tied robot
            _WithVar_7541.mem(455 + l) == rob(_WithVar_7541.Ties(k).pnt).occurr(l);
        }

        if (!rob(_WithVar_7541.Ties(k).pnt).Veg)
        {
            if (_WithVar_7541.FName != rob(_WithVar_7541.Ties(k).pnt).FName)
            {
                //Botsareus 2/11/2014 Tie Eye Fudge
                if (FudgeEyes || FudgeAll)
                {
                    if (_WithVar_7541.mem(455 + 8) < 2)
                    {
                        _WithVar_7541.mem(455 + 8) == Int(rndy() * 2) + 1;
                    }
                    else
                    {
                        _WithVar_7541.mem(455 + 8) == _WithVar_7541.mem(455 + 8) + Int(rndy() * 2) * 2 - 1;
                    }
                }
                //Fudge the rest of Tie occurr
                if (FudgeAll)
                {
                    for (l = 1; l < 7; l++)
                    {
                        if (_WithVar_7541.mem(455 + l) < 2)
                        {
                            _WithVar_7541.mem(455 + l) == Int(rndy() * 2) + 1;
                        }
                        else
                        {
                            _WithVar_7541.mem(455 + l) == _WithVar_7541.mem(455 + l) + Int(rndy() * 2) * 2 - 1;
                        }
                    }
                }
            }
        }

        if (_WithVar_7541.mem(476) > 0 & _WithVar_7541.mem(476) <= 1000)
        { //tmemval and tmemloc couple used to read a specific memory value from tied robot.
            _WithVar_7541.mem(475) = rob(_WithVar_7541.Ties(k).pnt).mem(_WithVar_7541.mem(476));
            if (_WithVar_7541.mem(479) > EyeStart && _WithVar_7541.mem(479) < EyeEnd)
            {
                rob(_WithVar_7541.Ties(k).pnt).View = true;
            }
        }
        if (rob(_WithVar_7541.Ties(k).pnt).Fixed)
        {
            _WithVar_7541.mem(478) = 1;
        }
        else
        {
            _WithVar_7541.mem(478) = 0;
        }

        _WithVar_7541.mem(479) = rob(_WithVar_7541.Ties(k).pnt).mem(AimSys);

        _WithVar_7541.mem(trefxpos) = rob(_WithVar_7541.Ties(k).pnt).mem(219);
        _WithVar_7541.mem(trefypos) = rob(_WithVar_7541.Ties(k).pnt).mem(217);
        _WithVar_7541.mem(trefvelyourup) = rob(_WithVar_7541.Ties(k).pnt).mem(velup);
        _WithVar_7541.mem(trefvelyourdn) = rob(_WithVar_7541.Ties(k).pnt).mem(veldn);
        _WithVar_7541.mem(trefvelyoursx) = rob(_WithVar_7541.Ties(k).pnt).mem(velsx);
        _WithVar_7541.mem(trefvelyourdx) = rob(_WithVar_7541.Ties(k).pnt).mem(veldx);

        //Botsareus 9/27/2014 I was thinking long and hard where to place this bug fix, probebly best to place it at the source
        if (Abs(rob(_WithVar_7541.Ties(k).pnt).vel.Y) > 16000)
        {
            rob(_WithVar_7541.Ties(k).pnt).vel.Y = 16000 * Sgn(rob(_WithVar_7541.Ties(k).pnt).vel.Y);
        }
        if (Abs(rob(_WithVar_7541.Ties(k).pnt).vel.X) > 16000)
        {
            rob(_WithVar_7541.Ties(k).pnt).vel.X = 16000 * Sgn(rob(_WithVar_7541.Ties(k).pnt).vel.X);
        }

        _WithVar_7541.mem(trefvelmyup) = rob(_WithVar_7541.Ties(k).pnt).vel.X * Cos(_WithVar_7541.aim) + Sin(_WithVar_7541.aim) * rob(_WithVar_7541.Ties(k).pnt).vel.Y * -1 - _WithVar_7541.mem(velup); //gives velocity from mybots frame of reference
        _WithVar_7541.mem(trefvelmydn) = _WithVar_7541.mem(trefvelmyup) * -1;
        _WithVar_7541.mem(trefvelmydx) = rob(_WithVar_7541.Ties(k).pnt).vel.Y * Cos(_WithVar_7541.aim) + Sin(_WithVar_7541.aim) * rob(_WithVar_7541.Ties(k).pnt).vel.X - _WithVar_7541.mem(veldx);
        _WithVar_7541.mem(trefvelmysx) = _WithVar_7541.mem(trefvelmydx) * -1;
        _WithVar_7541.mem(trefvelscalar) = rob(_WithVar_7541.Ties(k).pnt).mem(velscalar);
        // .mem(trefbody) = rob(.Ties(k).pnt).body
        _WithVar_7541.mem(trefshell) = rob(_WithVar_7541.Ties(k).pnt).shell;

        //These are the tie in/pairs
        for (l = 410; l < 419; l++)
        {
            _WithVar_7541.mem(l + 10) == rob(_WithVar_7541.Ties(k).pnt).mem(l);
        }

        if (!rob(_WithVar_7541.Ties(k).pnt).Veg)
        {
            //Fudge tin/tout
            if (FudgeAll && _WithVar_7541.FName != rob(_WithVar_7541.Ties(k).pnt).FName)
            {
                for (l = 410; l < 419; l++)
                {
                    if (_WithVar_7541.mem(l + 10) != 0)
                    {
                        _WithVar_7541.mem(l + 10) == _WithVar_7541.mem(l + 10) + Int(rndy() * 2) * 2 - 1;
                    }
                }
            }
        }

    }

    /*
    ' deletes all ties of a robot a
    */
    public static void delallties(int a)
    {
        int i = 0;

        i = 1;
        While(rob(a).Ties(1).pnt != 0 & i <= MAXTIES);
        DeleteTie(a, rob(a).Ties(1).pnt);
        i = i + 1;
        Wend();
    }

    /*
    ' deletes a tie between robots a and b
    */
    public static void DeleteTie(int a, int b)
    {
        int k = 0;

        int j = 0;

        int t = 0;


        //Quick tests to rule whether a tie can't exist between the bots.
        if ((!rob(a).exist) || (!rob(b).exist))
        {
            goto getout;
        }
        if (rob(a).numties == 0 || rob(b).numties == 0)
        {
            goto getout;
        }

        k = 1;
        j = 1;

        //Only allows 9 ties at present.  Change this?

        While(rob(a).Ties(k).pnt != b && k < MAXTIES);
        k = k + 1;
        Wend();

        While(rob(b).Ties(j).pnt != a && j < MAXTIES);
        j = j + 1;
        Wend();

        if (k < MAXTIES)
        {
            rob(a).numties = rob(a).numties - 1; // Decrement numties
            rob(a).mem(numties) = rob(a).numties;
            if (rob(a).mem(TIEPRES) == rob(a).Ties(k).Port)
            { // we are deleting the last tie created.  Have to update .tiepres.
                if (k > 1)
                {
                    rob(a).mem(TIEPRES) = rob(a).Ties(k - 1).Port;
                }
                else
                {
                    rob(a).mem(TIEPRES) = 0; // no more ties
                }
            }
        }

        if (j < MAXTIES)
        {
            rob(b).numties = rob(b).numties - 1; // Decrement numties
            rob(b).mem(numties) = rob(b).numties;
            if (rob(b).mem(TIEPRES) == rob(b).Ties(j).Port)
            { // we are deleting the last tie created.  Have to update .tiepres.
                if (j > 1)
                {
                    rob(b).mem(TIEPRES) = rob(b).Ties(j - 1).Port;
                }
                else
                {
                    rob(b).mem(TIEPRES) = 0; // no more ties
                }
            }
        }


        for (t = k; t < MAXTIES - 1; t++)
        {
            rob(a).Ties(t) = rob(a).Ties(t + 1);
        }

        rob(a).Ties(MAXTIES).pnt = 0;

        for (t = j; t < MAXTIES - 1; t++)
        {
            rob(b).Ties(t) = rob(b).Ties(t + 1);
        }

        rob(b).Ties(MAXTIES).pnt = 0;
    getout:
    }

    /*
    ' T I E S


    ' creates a tie between rob a and b,of len c, lasting last cycles
    ' (or waiting -last cycles before consolidating)
    ' tie is addressed with index mem (putting mem in .tienum)
    */
    public static bool maketie(int a, int b, int c, int last, int mem)
    {
        bool maketie = false;
        //returns true on success
        //Ties and slime need to be reworked at some point
        int k = 0;

        int j = 0;

        bool OK = false;

        int Max = 0;

        int deflect = 0;

        int Length = 0;

        bool deletedtie = false;


        maketie = false;

        if (rob(a).exist == false)
        {
            goto getout;
        }

        deflect = Random(2, 92); //random number which allows for the effect of slime on the target robot. If slime is greater then no tie is formed
        Max() = MAXTIES;
        OK = true;
        k = 1;
        j = 1;

        Length = VectorMagnitude(VectorSub(rob(a).pos, rob(b).pos));

        if (Length <= c * 1.5m)
        { //And deflect > rob(b).slime Then
            if (deflect < rob(b).Slime)
            {
                OK = false; //should stop ties forming when slime is high
            }

            if (OK == true)
            {
                DeleteTie(a, b);
            }

            While(rob(a).Ties(k).pnt > 0 & k <= Max() && OK);
            k = k + 1;
            Wend();
            While(rob(b).Ties(j).pnt > 0 & j <= Max() && OK);
            j = j + 1;
            Wend();

            if (k < Max() && j < Max() && OK)
            {
                rob(a).Ties(k).pnt = b;
                rob(a).Ties(k).ptt = j;
                rob(a).Ties(k).NaturalLength = Length;
                rob(a).Ties(k).stat = false;
                rob(a).Ties(k).last = last();
                rob(a).Ties(k).Port = mem;
                rob(a).Ties(k).back = false;
                rob(a).numties = k;
                rob(a).mem(466) = rob(a).numties; //EricL 3/22/2006 Increment numties in the bot's memory
                rob(a).mem(TIEPRES) = mem;
                ReadTRefVars(a, k); // EricL 4./20/2006  Load up the trefvars for the bot that created the tie.

                //EricL 5/7/2006 All ties are springs when first created
                rob(a).Ties(k).b = 0.02m;
                rob(a).Ties(k).k = 0.01m;
                rob(a).Ties(k).type = 0;

                rob(b).Ties(j).pnt = a;
                rob(b).Ties(j).ptt = k;
                rob(b).Ties(j).NaturalLength = Length;
                rob(b).Ties(j).stat = false;
                rob(b).Ties(j).last = last();
                rob(b).Ties(j).back = true;
                rob(b).numties = j;
                rob(b).Ties(j).Port = rob(b).numties; // The port of the tie from the point of view of the tied bot
                rob(b).mem(466) = rob(b).numties; //EricL 3/22/2006 Increment numties in the bot's memory
                rob(b).mem(TIEPRES) = j;

                //EricL 5/7/2006 All ties are springs when first created
                rob(b).Ties(j).b = 0.02m;
                rob(b).Ties(j).k = 0.01m;
                rob(b).Ties(j).type = 0;
            }
        }

        if (rob(b).Slime > 0)
        {
            rob(b).Slime = rob(b).Slime - 20;
        }
        if (rob(b).Slime < 0)
        {
            rob(b).Slime = 0; //cost to slime layer when attacked
        }
        rob(a).nrg = rob(a).nrg - (SimOpts.Costs(TIECOST) * SimOpts.Costs(COSTMULTIPLIER)) / (IIf(rob(a).numties < 0, 0, rob(a).numties) + 1); //Tie cost to form tie
    getout:
        return maketie;
    }

    /*
    ' searches a tie in rob t pointing to rob p
    ' returns tie number (j) of the tie pointing to the specified robot
    */
    public static int srctie(int t, int p)
    {
        int srctie = 0;
        int j = 0;

        j = 1;
        srctie = 0;
        dynamic _WithVar_3098;
        _WithVar_3098 = rob(t);
        While(_WithVar_3098.Ties(j).pnt > 0 & srctie == 0);
        if (_WithVar_3098.Ties(j).pnt == p && _WithVar_3098.Ties(j).last < 1)
        {
            srctie = j;
        }
        j = j + 1;
        Wend();
        return srctie;
    }

    /*
    'fixes tie angle and length at whatever it currently is
    */
    public static void regang(int t, int j)
    {
        int n = 0;

        decimal angl = 0;

        decimal dist = 0;

        dynamic _WithVar_8469;
        _WithVar_8469 = rob(t);
        _WithVar_8469.Multibot = true;
        _WithVar_8469.mem(multi) = 1;
        _WithVar_8469.Ties(j).b = 0.1m; // was 0.1
        _WithVar_8469.Ties(j).k = 0.05m; // was 0.05
        _WithVar_8469.Ties(j).type = 3;
        n = _WithVar_8469.Ties(j).pnt;
        angl = angle(_WithVar_8469.pos.X, _WithVar_8469.pos.Y, rob[n].pos.X, rob[n].pos.Y);
        //  angl = angnorm(angl)
        dist = Sqr((_WithVar_8469.pos.X - rob[n].pos.X) ^ 2 + (_WithVar_8469.pos.Y - rob[n].pos.Y) ^ 2);
        if (_WithVar_8469.Ties(j).back == false)
        {
            _WithVar_8469.Ties(j).ang = AngDiff(angnorm(angl), angnorm(rob(t).aim)); // only fix the angle of the bot that created the tie
            _WithVar_8469.Ties(j).angreg = true;
        }
        _WithVar_8469.Ties(j).NaturalLength = dist;
        //If .Ties(j).NaturalLength < 200 Then .Ties(j).NaturalLength = 200
        //   If .mem(468) <> 32000 Then 'And .mem(468) <> 0 Then          'replaces .ang calculated value with .fixang value
        //     If .mem(468) > 628 Then .mem(468) = 628
        //     If .mem(468) < -628 Then .mem(468) = -628
        //     .Ties(j).ang = .mem(468) / 200 'should it be 100 or 200?
        //   End If
        //   If .mem(469) <> 0 Then            'replaces .ln with .fixlen value
        //    .Ties(j).NaturalLength = .mem(469)
        //   End If
        //  .Ties(j).angreg = True
        //.mem(10) = .Ties(j).ang       'temporary test locations
        //.mem(11) = .Ties(j).ln
    }

    /*
    ' bends a tie
    */
    public static void bend(int t, int lnk, int ang)
    {
        decimal an = 0;

        if (Abs(ang) > 100)
        {
            ang = 100 * Sgn(ang);
        }
        an = ang / 100;
        dynamic _WithVar_9534;
        _WithVar_9534 = rob(t).Ties(lnk);
        _WithVar_9534.bend = an;
        rob(_WithVar_9534.pnt).Ties(_WithVar_9534.ptt).bend = -an;
        ang = 0;
    }

    /*
    ' shrinks a tie
    */
    public static void shrink(int t, int lnk, int ln)
    {
        if (Abs(ln) > 100)
        {
            ln = 1000 * Sgn(ln); // EricL 5/7/2006 Changed from 100 to 1000
        }
        dynamic _WithVar_4402;
        _WithVar_4402 = rob(t).Ties(lnk);
        _WithVar_4402.shrink = ln;
        rob(_WithVar_4402.pnt).Ties(_WithVar_4402.ptt).shrink = ln;
        ln = 0;
    }
}
