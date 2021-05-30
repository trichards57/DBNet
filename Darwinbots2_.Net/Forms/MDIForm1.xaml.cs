using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using static Common;
using static Database;
using static DNAExecution;
using static DNATokenizing;
using static Evo;
using static F1Mode;
using static Globals;
using static HDRoutines;
using static IntOpts;
using static Master;
using static Microsoft.VisualBasic.Constants;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.DateAndTime;
using static Microsoft.VisualBasic.FileSystem;
using static Microsoft.VisualBasic.Information;
using static Microsoft.VisualBasic.Interaction;
using static Microsoft.VisualBasic.Strings;
using static Microsoft.VisualBasic.VBMath;
using static Multibots;
using static ObstaclesManager;
using static Physics;
using static Robots;
using static SimOpt;
using static stuffcolors;
using static System.Math;
using static Teleport;
using static varspecie;
using static VBConstants;
using static VBExtension;
using static Vegs;

namespace DBNet.Forms
{
    public partial class MDIForm1 : Window
    {
        public string BaseCaption = "";
        public bool displayMovementVectorsToggle = false;
        public bool displayResourceGuagesToggle = false;
        public bool displayShotImpactsToggle = false;
        public bool exitDB = false;
        public int Gridmode = 0;
        public bool HandlingMenuItem = false;
        public bool ignoreerror = false;
        public decimal imModeHandle = 0;
        public bool insrob = false;
        public bool limitgraphics = false;
        public bool nopoff = false;
        public bool oneonten = false;
        public bool SaveWithoutMutations = false;
        public bool showVisionGridToggle = false;

        // fast mode on/off
        // don't anim deaths with particles
        // Which display to use on the egrid
        public bool stealthmode = false;

        public bool visualize = false;

        // video output on/off
        public int xc = 0;

        public int yc = 0;
        public int zoomval = 0;
        private static MDIForm1 _instance;
        private bool AspettaFlag = false;

        //False
        // DarwinBots - copyright 2003 Carlo Comis
        // Modifications by Purple Youko and Numsgil - 2004, 2005
        // Post V2.42 modifications copyright (c) 2006, 2007 Eric Lockard  eric@sulaadventures.com
        // Post V2.45 modifications copyright (c) 2012, 2013, 2014, 2015, 2016 Paul Kononov
        //a.k.a
        //______________________________________________1$$$___108033_____$$______________________________
        //____1$$$$$$$3________________011_______________$$__$$$$$$$$$$8_1$_________1$$$1__8$$$1_______3$$
        //____3$$811$$$0______________1$$3_______________0___$$$$__1$$$8____________0$$$__1$$$$______0$$8_
        //____1$$__1$$$1______________$$$_______880_________3$$$3__8$$$3____________0$$0__1$$$0____1$$$1__
        //____3$$11$$$0____8$$$8___8$$$$$$$3__38$___________0$$$$_$$$$$_____________$$$8__8$$$1____$$$8___
        //____1$$$$$$1____$$$$$$$3__$$$$88___$$8____________8$$$$$$$$0______________8$$$__$$$$_____3$$$1__
        //____1$$$$$$$0__$$$$_18$$___8$$___1$$$_____________$$$$$$$$8_______________0$$8_0$$$$_______8$$__
        //____3$$$88$$$$_$$$___8$$__3$$8____0$$$___________1$$$$1$$$$_______________8$$$$$$$$3_______$$$1_
        //____1$$____3$$_8$$__8$$8__8$$______0$$1__________3$$$0_1$$$1______________8$$$$$$$$_______$$$0__
        //____3$$____0$$__$$$$$$$1__$$8______$$0___________$$$$___8$$0______________1$$$$$$$3_____1$$$0___
        //_____$$011$$$$__8$$$$$1__1$$1____3$$1___________3$$$3___3$$$_______________8$$$$$3____1$$$0_____
        //____3$$$$$$81____3$80____$$81__188______________8$$$_____8$$0_______________0$$81____8003_______
        // All rights reserved.
        //Redistribution and use in source and binary forms, with or without
        //modification, are permitted provided that:
        //(1) source code distributions retain the above copyright notice and this
        //    paragraph in its entirety,
        //(2) distributions including binary code include the above copyright notice and
        //    this paragraph in its entirety in the documentation or other materials
        //    provided with the distribution, and
        //(3) Without the agreement of the author redistribution of this product is only allowed
        //    in non commercial terms and non profit distributions.
        //THIS SOFTWARE IS PROVIDED ``AS IS'' AND WITHOUT ANY EXPRESS OR IMPLIED
        //WARRANTIES, INCLUDING, WITHOUT LIMITATION, THE IMPLIED WARRANTIES OF
        //MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.
        // Option Explicit
        //Botsareus 5/19/2012 Update to the way the tool menu looks; removed unnecessary pictures from collection
        //Botsareus 3/15/2013 got rid of screen save code (was broken)
        //Botsareus 4/17/2013 Added a bunch of new components
        private byte lockswitch = 0;

        private int picinc = 0;

        public MDIForm1()
        {
            InitializeComponent();
        }

        public static MDIForm1 instance { set { _instance = null; } get { return _instance ?? (_instance = new MDIForm1()); } }

        public static void Load()
        {
            if (_instance == null) { dynamic A = MDIForm1.instance; }
        }

        public static void Unload()
        {
            if (_instance != null) instance.Close(); _instance = null;
        }

        public dynamic DisableRobotsMenu()
        {
            dynamic DisableRobotsMenu = null;

            MDIForm1.instance.robinf.IsEnabled = false;
            MDIForm1.instance.par.IsEnabled = false;
            MDIForm1.instance.mutrat.IsEnabled = false;
            MDIForm1.instance.col.IsEnabled = false;
            MDIForm1.instance.cons.IsEnabled = false;
            MDIForm1.instance.genact.IsEnabled = false;
            MDIForm1.instance.ucci.IsEnabled = false;
            MDIForm1.instance.sdna.IsEnabled = false;
            MDIForm1.instance.selorg.IsEnabled = false;
            MDIForm1.instance.saveorg.IsEnabled = false;
            MDIForm1.instance.killorg.IsEnabled = false;

            return DisableRobotsMenu;
        }

        public void disablesim()
        {
            // edit.Enabled = False
            // popup.Enabled = False
            // czin.Enabled = False
            // czo.Enabled = False
        }

        public dynamic EnableRobotsMenu()
        {
            dynamic EnableRobotsMenu = null;

            MDIForm1.instance.robinf.IsEnabled = true;
            MDIForm1.instance.par.IsEnabled = true;
            MDIForm1.instance.mutrat.IsEnabled = true;
            MDIForm1.instance.col.IsEnabled = true;
            MDIForm1.instance.cons.IsEnabled = true;
            MDIForm1.instance.genact.IsEnabled = true;
            MDIForm1.instance.ucci.IsEnabled = true;
            MDIForm1.instance.sdna.IsEnabled = true;
            MDIForm1.instance.selorg.IsEnabled = true;
            MDIForm1.instance.saveorg.IsEnabled = true;
            MDIForm1.instance.killorg.IsEnabled = true;
            return EnableRobotsMenu;
        }

        public void enablesim()
        {
            Edit.IsEnabled = true;
            popup.IsEnabled = true;
            czin.IsEnabled = true;
            czo.IsEnabled = true;
        }

        public void F1Internet_Click()
        {
            if (Form1.lblSafeMode.Visible)
            {
                MsgBox("Can not enable Internet during safemode.");
                return;
            }
            if (x_restartmode == 1)
            {
                MsgBox("Can not enable Internet during league seeding.");
                return;
            }
            if (x_restartmode == 2)
            {
                MsgBox("Can not enable Internet during tournament league.");
                return;
            }
            if (x_restartmode == 3)
            {
                MsgBox("Can not enable Internet during stepladder league.");
                return;
            }
            if ((x_restartmode == 4 || x_restartmode == 5) && y_eco_im == 0)
            {
                MsgBox("Can not enable Internet during simple survival mode.");
                return;
            }
            if (x_restartmode == 6)
            {
                MsgBox("Can not enable Internet during survival mode contest round.");
                return;
            }
            if (x_restartmode == 7 || x_restartmode == 8)
            {
                MsgBox("Can not enable Internet during zerobot mode.");
                return;
            }
            if (x_restartmode == 9)
            {
                MsgBox("Can not enable Internet during zerobot testing.");
                return;
            }
            if (x_restartmode == 10)
            { //Botsareus 10/6/2015
                MsgBox("Can not enable Internet during robot filter mode.");
                return;
            }

            int i = 0;

            int b = 0;

            int l = 0;

            string s = "";

            string iq = "";

            string oq = "";

            HandlingMenuItem = true;

        Top:
            F1Internet.Checked = !(F1Internet.Checked);
            if (F1Internet.Checked)
            {
                if (IntOpts.IName == "")
                {
                    IntOpts.IName = "Newbie " + CStr(Random(1, 10000)); //Botsareus 2/25/2014 A little bugfix here
                }

                if (IntOpts.IName == "")
                {
                    MsgBox("You must specify an Internet nickname before switching to Internet mode.", vbOKOnly);
                    OptionsForm.instance.SSTab1.Tab = 5;
                    NetEvent.instance.Timer1.IsEnabled = false;
                    NetEvent.instance.Hide();
                    optionsform.Show(vbModal);
                    return;
                }
            tryagain:
                //This section create our new Internet Mode Teleporter
                i = NewTeleporter(false, false, (SimOpts.FieldHeight ^ 0.5m) * 10, true); //Botsareus 5/12/2012 Changed the startup size of teleporter for better robot flow

                Teleporters(i).vel = VectorSet(0, 0);
                Teleporters(i).teleportVeggies = true;
                Teleporters(i).teleportCorpses = false;
                Teleporters(i).teleportHeterotrophs = true;
                Teleporters(i).RespectShapes = false;
                Teleporters(i).InboundPollCycles = 10;
                Teleporters(i).BotsPerPoll = 10;
                Teleporters(i).PollCountDown = 10;

                MDIForm1.instance.F1InternetButton.DownPicture = Form1.ServerGood;
                MDIForm1.instance.F1InternetButton.value = 1; // checked
                MDIForm1.instance.F1InternetButton.Refresh();
                MDIForm1.instance.EditIntTeleporter.IsEnabled = true;

                Form1.InternetMode.Visible = true;
                InternetMode = true;

                MDIForm1.instance.Caption.DefaultProperty = MDIForm1.instance.Caption.DefaultProperty + "    Internet Mode";
                iq = Chr(34) + Teleporters(i).intInPath + Chr(34);
                oq = Chr(34) + Teleporters(i).intOutPath + Chr(34);
                s = App.path + "\\DarwinbotsIM.exe" + " -in " + iq + " -" + oq + " -name " + Chr(34) + IntOpts.IName + Chr(34) + " -port " + Chr(34) + IIf(IntOpts.ServPort == "", "79", IntOpts.ServPort) + Chr(34) + " -server " + Chr(34) + IIf(IntOpts.ServIP == "PeterIM", "198.50.150.51", IntOpts.ServIP) + Chr(34);

                IntOpts.pid = shell(s, vbNormalFocus);
                if (IntOpts.pid == 0)
                {
                    MsgBox(("Could not open DarwinbotsIM.exe"));
                    goto ;
                }
            }
            else
            {
                //Exit DarwinbotsIM
                l = CloseWindow(IntOpts.pid);

                InternetMode = false;
                MDIForm1.instance.F1InternetButton.value = 0; // checked
                MDIForm1.instance.EditIntTeleporter.IsEnabled = false;

                for (i = 1; i < MAXTELEPORTERS; i++)
                {
                    if (Teleporters(i).Internet && Teleporters(i).exist)
                    {
                        DeleteTeleporter((i));
                        i = i - 1;
                    }
                }
                Form1.InternetMode.Visible = false;

                if (Right(MDIForm1.instance.Caption.DefaultProperty, 17) == "    Internet Mode")
                {
                    MDIForm1.instance.Caption.DefaultProperty = Left(MDIForm1.instance.Caption.DefaultProperty, Len(MDIForm1.instance.Caption.DefaultProperty) - 17);
                }

            bypass:
    }
            HandlingMenuItem = false;
            return;
        }

        public void Follow()
        { //Botsareus 11/29/2013 Zoom follow selected robot
            if (robfocus > 0 & Form1.visiblew < 6000 & visualize)
            {
                xc = rob(robfocus).pos.X;
                yc = rob(robfocus).pos.Y;
                Form1.ScaleTop = yc - Form1.ScaleHeight / 2;
                Form1.ScaleLeft = xc - Form1.ScaleWidth / 2;
            }
        }

        public void grabfile()
        {
            string fl = "";

            fl = dir(App.path + "\\*.bin");
            if (fl == "")
            {
                //if no more bin files generate some more and attempt to grab file again

                //new data is memory intensive so while it is running redim rndylist to 0
                List<> rndylist_1869_tmp = new List<>();
                for (int redim_iter_1802 = 0; i < 0; redim_iter_1802++) { rndylist.Add(null); }
                newdata();
                List<> rndylist_2001_tmp = new List<>();
                for (int redim_iter_2027 = 0; i < 3999; redim_iter_2027++) { rndylist.Add(null); }

                grabfile(); //try again
            }
            else
            {
                filemem = fl;
                //compute file to rndylist
                int l = 0;

                byte bt = 0;

                decimal h = 0;

                VBOpenFile(477, App.path + "\\" + filemem); ;
                h = 1; //Starting seed
                for (l = 0; l < 3999; l++)
                {
                    Get(477); //we have 1 byte
                    rndylist(l) = Rnd(-Abs(angle(0, 0, h - 0.5m, Rnd(-bt - 1) - 0.5m))); //seed optimize
                    h = rndylist(l);
                }
                VBCloseFile(477); ();
            }
        }

        public dynamic MakeNewSpeciesFromBot(int n)
        {
            dynamic MakeNewSpeciesFromBot = null;
            int i = 0;

            string OldSpeciesName = "";

            if (!rob[n].exist || rob[n].Corpse)
            {
                return MakeNewSpeciesFromBot;
            }
            //Change the species of bot n
            OldSpeciesName = rob[n].FName;
            if (Right(rob[n].FName, 4) == ".txt")
            {
                rob[n].FName = Left(rob[n].FName, Len(rob[n].FName) - 4);
            }

            rob[n].FName = Left(rob[n].FName, 28) + Str(Random(1, 10000));

            AddSpecie(n, false); // Species is forked in this sim so it's native
            ChangeNameOfAllChildren(n, OldSpeciesName);
            ChangeNameOfAllCloselyRelated(n, 10, OldSpeciesName);
            return MakeNewSpeciesFromBot;
        }

        public void menuupdate()
        { //Botsareus 7/13/2012 The menu handler
            if (limitgraphics)
            {
                Toolbar1.buttons(9).value = tbrPressed;
            }
            else
            {
                Toolbar1.buttons(9).value = tbrUnpressed;
            }
            if (oneonten)
            {
                Toolbar1.buttons(10).value = tbrPressed;
            }
            else
            {
                Toolbar1.buttons(10).value = tbrUnpressed;
            }
            if (Form1.Flickermode)
            {
                Toolbar1.buttons(11).value = tbrPressed;
            }
            else
            {
                Toolbar1.buttons(11).value = tbrUnpressed;
            }
            if (!Form1.dispskin)
            {
                Toolbar1.buttons(12).value = tbrPressed;
            }
            else
            {
                Toolbar1.buttons(12).value = tbrUnpressed;
            }
            if (nopoff)
            {
                Toolbar1.buttons(13).value = tbrPressed;
            }
            else
            {
                Toolbar1.buttons(13).value = tbrUnpressed;
            }
            if (!visualize)
            {
                Toolbar1.buttons(14).value = tbrPressed;
            }
            else
            {
                Toolbar1.buttons(14).value = tbrUnpressed;
            }
            Toolbar1.Refresh(); //Botsareus 1/11/2013 Force toolbar to refresh
        }

        public void robmutchange()
        {
            //''''''''''''''''''''''''''''''''''''''''''''''''''''''
            int t = 0;

            Species Specie = null;

            t = OptionsForm.instance.CurrSpec;
            Specie = TmpOpts.Specie(50);

            TmpOpts.Specie(50).Mutables = rob(robfocus).Mutables;
            OptionsForm.instance.CurrSpec = 50;

            MutationsProbability.Show(vbModal);

            rob(robfocus).Mutables = TmpOpts.Specie(50).Mutables;
            TmpOpts.Specie(50) = Specie;
            OptionsForm.instance.CurrSpec = t;
        }

        public void ZoomIn()
        {
            if (Form1.visiblew > RobSize * 4)
            {
                if (robfocus > 0)
                {
                    xc = rob(robfocus).pos.X;
                    yc = rob(robfocus).pos.Y;
                }
                else
                {
                    xc = Form1.visiblew / 2 + Form1.ScaleLeft;
                    yc = Form1.visibleh / 2 + Form1.ScaleTop;
                }
                Form1.visiblew = Form1.visiblew / 1.05m;
                Form1.visibleh = Form1.visibleh / 1.05m;
                Form1.ScaleHeight = Form1.visibleh;
                Form1.ScaleWidth = Form1.visiblew;
                Form1.ScaleTop = yc - Form1.ScaleHeight / 2;
                Form1.ScaleLeft = xc - Form1.ScaleWidth / 2;
                Form1.Redraw();
            }

            if (ZoomLock.value == 1)
            {
                if (lockswitch == 0)
                {
                    decimal ratio = 0;

                    ratio = Form1.TwipHeight / Form1.twipWidth;
                    decimal expectedscreenratio = 0;

                    expectedscreenratio = 9645 / 15300;
                    decimal actualscreenratio = 0;

                    actualscreenratio = Form1.Height / Form1.Width;
                    Form1.visiblew = Form1.visiblew * ratio * expectedscreenratio / actualscreenratio * 1.065m;
                    lockswitch = 1;
                }
            }
        }

        public void ZoomOut()
        {
            int tvv = 0;

            int thv = 0;

            //EricL Prevents zooming too far from causing an overflow
            if (Form1.visiblew >= 10000000)
            {
                return;
            }
            if (Form1.visibleh >= 10000000)
            {
                return;
            }

            xc = Form1.visiblew / 2 + Form1.ScaleLeft;
            yc = Form1.visibleh / 2 + Form1.ScaleTop;

            Form1.visiblew = Form1.visiblew / 0.95m;
            Form1.visibleh = Form1.visibleh / 0.95m;

            if (Form1.visiblew > SimOpts.FieldWidth && ZoomLock.value == 0)
            {
                Form1.visiblew = SimOpts.FieldWidth;
                Form1.visibleh = SimOpts.FieldHeight;
            }

            if (Form1.visibleh > SimOpts.FieldHeight && ZoomLock.value == 0)
            {
                Form1.visiblew = SimOpts.FieldWidth;
                Form1.visibleh = SimOpts.FieldHeight;
            }

            Form1.ScaleTop = yc - Form1.visibleh / 2;
            Form1.ScaleLeft = xc - Form1.visiblew / 2;

            if (Form1.visiblew + Form1.ScaleLeft > SimOpts.FieldWidth && ZoomLock.value == 0)
            {
                Form1.ScaleLeft = SimOpts.FieldWidth - Form1.visiblew;
                Form1.ScaleTop = SimOpts.FieldHeight - Form1.visibleh;
            }

            if (Form1.ScaleLeft < 0 & ZoomLock.value == 0)
            {
                Form1.ScaleLeft = 0;
            }

            if (Form1.ScaleTop < 0 & ZoomLock.value == 0)
            {
                Form1.ScaleTop = 0;
            }

            Form1.ScaleHeight = Form1.visibleh;
            Form1.ScaleWidth = Form1.visiblew;

            if (ZoomLock.value == 1)
            {
                if (lockswitch == 0)
                {
                    decimal ratio = 0;

                    ratio = Form1.TwipHeight / Form1.twipWidth;
                    decimal expectedscreenratio = 0;

                    expectedscreenratio = 9645 / 15300;
                    decimal actualscreenratio = 0;

                    actualscreenratio = Form1.Height / Form1.Width;
                    Form1.visiblew = Form1.visiblew * ratio * expectedscreenratio / actualscreenratio * 1.065m;
                    lockswitch = 1;
                }
            }
            Form1.Redraw();
        }

        // global used to prevent recursion between internet mode button and menu
        //USE INTERNET AS RANDOMIZER SECTION
        [DllImport("urlmon.dll", EntryPoint = "URLDownloadToFileA")] private static extern int URLDownloadToFile(int pCaller, string szURL, string szFileName, int dwReserved, int lpfnCB);

        private void about_Click(object sender, RoutedEventArgs e)
        {
            about_Click();
        }

        private void about_Click()
        {
            frmAbout.instance.Show();
        }

        private void AddTenObstacles_Click(object sender, RoutedEventArgs e)
        {
            AddTenObstacles_Click();
        }

        private void AddTenObstacles_Click()
        {
            AddRandomObstacles((10));
        }

        private void AutoFork_Click(object sender, RoutedEventArgs e)
        {
            AutoFork_Click();
        }

        //Botsareus 3/23/2014 auto forking
        private void AutoFork_Click()
        {
            // TODO (not supported): On Error GoTo b
            AutoFork.Checked = !(AutoFork.Checked);
            if (AutoFork.Checked)
            {
                SimOpts.SpeciationGeneticDistance = InputBox("Enter % of mutations to DNA length that constitutes forking", "Automatic Forking", SimOpts.SpeciationGeneticDistance);
            }
            SimOpts.EnableAutoSpeciation = AutoFork.Checked;
            return;

        b:
            AutoFork.Checked = false;
        }

        private void AutoSpeciationMenu_Click(object sender, RoutedEventArgs e)
        {
            AutoSpeciationMenu_Click();
        }

        private void AutoSpeciationMenu_Click()
        {
            //Speciation.Show 'Commented to remove error.
        }

        private void AutoTag_Click(object sender, RoutedEventArgs e)
        {
            AutoTag_Click();
        }

        private void AutoTag_Click()
        {
            int t = 0;

            string robname = "";

            string robtag = "";

            robname = InputBox("Please enter the exact name of the robot you wish append with a new tag.");
            if (robname == "")
            {
                goto fine;
            }
            robtag = InputBox("Please enter the new tag. It can not be more then 45 characters long.");
            robtag = Left(replacechars(robtag), 45);

            for (t = 1; t < MaxRobs; t++)
            {
                if (rob(t).exist)
                {
                    if (rob(t).FName == robname)
                    {
                        rob(t).tag = robtag;
                    }
                }
            }
            return;

        fine:
            MsgBox("Cancel or blank entry", vbCritical);
        }

        private void ChangeNameOfAllChildren(int n, string OldSpeciesName)
        {
            int t = 0;

            if (rob[n].SonNumber == 0)
            {
                return;
            }
            for (t = 1; t < MaxRobs; t++)
            {
                if (rob(t).exist && !rob(t).Corpse && t != n)
                {
                    if (rob(t).parent == rob[n].AbsNum)
                    {
                        rob(t).FName = rob[n].FName;
                        ChangeNameOfAllChildren(t, OldSpeciesName);
                    }
                }
            }
        }

        private void ChangeNameOfAllCloselyRelated(int n, int d_UNUSED, string OldSpeciesName)
        {
            int t = 0;

            dynamic l = null;
            int ll = 0;

            int simNum = 0;

            int closestAncestor = 0;

            for (t = 1; t < MaxRobs; t++)
            {
                if (rob(t).exist && !rob(t).Corpse && t != n)
                {
                    if (rob(t).FName == OldSpeciesName)
                    {
                        //closestAncestor = FindClosestCommonAncestor(t, n, simNum)
                        //If closestAncestor <> 0 Then
                        //  l = FindGeneticDistance(t, n, closestAncestor, simNum)
                        //  ll = FindGenerationalDistance(t, n, closestAncestor, simNum)
                        //  If (l < SimOpts.SpeciationGeneticDistance / 3) And (ll < SimOpts.SpeciationGenerationalDistance / 3) Then
                        //    rob(t).FName = rob[n].FName
                        //  End If
                        //End If
                    }
                }
            }
        }

        private void CheckerMaze_Click(object sender, RoutedEventArgs e)
        {
            CheckerMaze_Click();
        }

        private void CheckerMaze_Click()
        {
            ObstaclesManager.DrawCheckerboardMaze();
        }

        private void col_Click(object sender, RoutedEventArgs e)
        {
            col_Click();
        }

        private void col_Click()
        {
            Form1.changerobcol();
        }

        private void cons_Click(object sender, RoutedEventArgs e)
        {
            cons_Click();
        }

        private void cons_Click()
        {
            Consoleform.openconsole();
        }

        private void costi_Click(object sender, RoutedEventArgs e)
        {
            costi_Click();
        }

        private void costi_Click()
        {
            OptionsForm.instance.SSTab1.Tab = 2;
            NetEvent.instance.Timer1.IsEnabled = false;
            NetEvent.instance.Hide();
            optionsform.Show(vbModal);
        }

        private void czin_MouseDown(int Button_UNUSED, int Shift_UNUSED, decimal X_UNUSED, decimal Y_UNUSED)
        {
            AspettaFlag = true;
            ZoomInPremuto();
        }

        private void czin_MouseUp(int Button_UNUSED, int Shift_UNUSED, decimal X_UNUSED, decimal Y_UNUSED)
        {
            AspettaFlag = false;
        }

        private void czo_MouseDown(int Button_UNUSED, int Shift_UNUSED, decimal X_UNUSED, decimal Y_UNUSED)
        {
            AspettaFlag = true;
            ZoomOutPremuto();
        }

        private void czo_MouseUp(int Button_UNUSED, int Shift_UNUSED, decimal X_UNUSED, decimal Y_UNUSED)
        {
            AspettaFlag = false;
        }

        private void DeleteAllShapes_Click(object sender, RoutedEventArgs e)
        {
            DeleteAllShapes_Click();
        }

        private void DeleteAllShapes_Click()
        {
            DeleteAllObstacles();
        }

        private void DeleteShape_Click(object sender, RoutedEventArgs e)
        {
            DeleteShape_Click();
        }

        private void DeleteShape_Click()
        {
            if (obstaclefocus != 0)
            {
                DeleteObstacle((obstaclefocus));
                obstaclefocus = 0;
                DeleteShape.IsEnabled = false;
            }
        }

        private void DeleteTeleporterMenu_Click(object sender, RoutedEventArgs e)
        {
            DeleteTeleporterMenu_Click();
        }

        private void DeleteTeleporterMenu_Click()
        {
            if (teleporterFocus != 0)
            {
                DeleteTeleporter((teleporterFocus));
                teleporterFocus = 0;
                DeleteTeleporterMenu.IsEnabled = false;
            }
        }

        private void DeleteTeleportersMenu_Click(object sender, RoutedEventArgs e)
        {
            DeleteTeleportersMenu_Click();
        }

        private void DeleteTeleportersMenu_Click()
        {
            DeleteAllTeleporters();
        }

        private void DeleteTenObstacles_Click(object sender, RoutedEventArgs e)
        {
            DeleteTenObstacles_Click();
        }

        private void DeleteTenObstacles_Click()
        {
            DeleteTenRandomObstacles();
        }

        private void DisableArep_Click(object sender, RoutedEventArgs e)
        {
            DisableArep_Click();
        }

        //Botsareus 4/17/2013 The new disable asexrepro button
        private void DisableArep_Click()
        {
            DisableArep.Checked = !(DisableArep.Checked);
            SimOpts.DisableTypArepro = DisableArep.Checked;
            TmpOpts.DisableTypArepro = DisableArep.Checked;
        }

        private void DisableFixing_Click(object sender, RoutedEventArgs e)
        {
            DisableFixing_Click();
        }

        private void DisableFixing_Click()
        {
            DisableFixing.Checked = !(DisableFixing.Checked);
            SimOpts.DisableFixing = DisableFixing.Checked;
            TmpOpts.DisableFixing = DisableFixing.Checked;
        }

        private void DisableTies_Click(object sender, RoutedEventArgs e)
        {
            DisableTies_Click();
        }

        private void DisableTies_Click()
        {
            DisableTies.Checked = !(DisableTies.Checked);
            SimOpts.DisableTies = DisableTies.Checked;
            TmpOpts.DisableTies = DisableTies.Checked;
        }

        private void DisplayMovementVectors_Click(object sender, RoutedEventArgs e)
        {
            DisplayMovementVectors_Click();
        }

        private void DisplayMovementVectors_Click()
        {
            DisplayMovementVectors.Checked = !(DisplayMovementVectors.Checked);
            displayMovementVectorsToggle = DisplayMovementVectors.Checked;
        }

        private void DisplayResourceGuages_Click(object sender, RoutedEventArgs e)
        {
            DisplayResourceGuages_Click();
        }

        private void DisplayResourceGuages_Click()
        {
            DisplayResourceGuages.Checked = !(DisplayResourceGuages.Checked);
            displayResourceGuagesToggle = DisplayResourceGuages.Checked;
        }

        private void DisplayShotImpacts_Click(object sender, RoutedEventArgs e)
        {
            DisplayShotImpacts_Click();
        }

        private void DisplayShotImpacts_Click()
        {
            DisplayShotImpacts.Checked = !(DisplayShotImpacts.Checked);
            displayShotImpactsToggle = DisplayShotImpacts.Checked;
        }

        private void DNAexp_Click(object sender, RoutedEventArgs e)
        {
            DNAexp_Click();
        }

        //Botsareus 8/7/2012 help loads a while, added a hourglass
        private void DNAexp_Click()
        {
            MousePointer = vbHourglass;
            DNA_Help.Show();
            MousePointer = vbDefault;
        }

        private void DontDecayNrgShots_Click(object sender, RoutedEventArgs e)
        {
            DontDecayNrgShots_Click();
        }

        private void DontDecayNrgShots_Click()
        {
            DontDecayNrgShots.Checked = !(DontDecayNrgShots.Checked);
            SimOpts.NoShotDecay = DontDecayNrgShots.Checked;
            TmpOpts.NoShotDecay = DontDecayNrgShots.Checked;
        }

        private void DontDecayWstShots_Click(object sender, RoutedEventArgs e)
        {
            DontDecayWstShots_Click();
        }

        //Botsareus 9/28/2013 Don't decay waste shots
        private void DontDecayWstShots_Click()
        {
            DontDecayWstShots.Checked = !(DontDecayWstShots.Checked);
            SimOpts.NoWShotDecay = DontDecayWstShots.Checked;
            TmpOpts.NoWShotDecay = DontDecayWstShots.Checked;
        }

        private void DrawSpiral_Click(object sender, RoutedEventArgs e)
        {
            DrawSpiral_Click();
        }

        private void DrawSpiral_Click()
        {
            ObstaclesManager.DrawSpiral();
        }

        private void EditIntTeleporter_Click(object sender, RoutedEventArgs e)
        {
            EditIntTeleporter_Click();
        }

        private void EditIntTeleporter_Click()
        {
            int i = 0;

            for (i = 1; i < numTeleporters; i++)
            {
                if (Teleporters(i).exist && Teleporters(i).Internet)
                {
                    Teleport.teleporterFocus = i;
                    break;
                }
            }
            TeleportForm.instance.teleporterFormMode = 1;
            TeleportForm.instance.Show();
        }

        private void extract(string url)
        { //outputs picture data to hard drive
            HTMLDocument iDoc = null;

            dynamic Element = null;

            iDoc = new HTMLDocument(); ;
            iDoc.body.innerHTML = Inet1.OpenURL(url);
            foreach (var Element in iDoc.All)
            {
                if (Element.tagName == "IMG")
                {
                    if (Element.href != "")
                    {
                        URLDownloadToFile(0, Element.href, App.path + "\\" + picinc + ".bmp", 0, 0);
                        picinc = picinc + 1;
                    }
                }
            }
        }

        private void F1Internet_Click(object sender, RoutedEventArgs e)
        {
            F1Internet_Click();
        }

        private void F1InternetButton_Click(object sender, RoutedEventArgs e)
        {
            F1InternetButton_Click();
        }

        private void F1InternetButton_Click()
        {
            if (!HandlingMenuItem)
            {
                MDIForm1.instance.F1Internet_Click().DefaultProperty;
            }
        }

        private void fisica_Click(object sender, RoutedEventArgs e)
        {
            fisica_Click();
        }

        private void fisica_Click()
        {
            OptionsForm.instance.SSTab1.Tab = 1;
            NetEvent.instance.Timer1.IsEnabled = false;
            NetEvent.instance.Hide();
            optionsform.Show(vbModal);
        }

        private void fittest_Click(object sender, RoutedEventArgs e)
        {
            fittest_Click();
        }

        private void fittest_Click()
        {
            robfocus = Form1.fittest;
        }

        private void fixcam()
        { //Botsareus 2/23/2013 When simulation starts the screen is normailized
            if (x_restartmode > 0 & HideDB)
            {
                Form1.t.Add();
                stealthmode = true;
                this
              if (SimOpts.F1)
                {
                    Contest_Form.instance.WindowState = vbMinimized;
                }
            }

            //Botsareus 9/12/2014 Simulation now always starts with ignoreerror on
            if (UseSafeMode == false)
            {
                ignoreerror = true;
                Toolbar1.buttons(24).value = tbrPressed;
            }
            //Botsareus 2/9/2014 Based on collected data we need to figure fudging here
            if (SimOpts.F1 || (x_restartmode != 1 && x_restartmode != 9 && x_restartmode != 0))
            { //Botsareus 10/6/2015 Do not fudge under sertain modes
                switch (x_fudge)
                {
                    case 1:
                        FudgeEyes = true;
                        break;

                    case 2:
                        FudgeAll = true;
                        break;
                }
            }
            pbOn.IsEnabled = !SimOpts.F1 && !y_eco_im == 2;
            showEyeDesign.IsEnabled = !SimOpts.F1 && !y_eco_im == 2;
            inssp.IsEnabled = y_eco_im == 0;
            if (y_eco_im == 2 && !(F1Internet.Checked) && !SimOpts.F1)
            {
                F1Internet_Click(); //Botsareus 7/12/2014 For eco evo this activates the internet
            }
            Form1.BackColor = backgcolor; //Botsareus 4/27/2013 Set back ground skin color
            if (startnovid)
            { //turn off vedio as requested
                visualize = false;
                Form1.Label1.Visible = true;
                startnovid = false;
            }
            //Botsareus 3/19/2014  auto. load some graphs for evo mode
            if (y_graphs && (x_restartmode == 4 || x_restartmode == 5))
            {
                Form1.NewGraph(POPULATION_GRAPH, "Populations");
                Form1.NewGraph(MUTATIONS_GRAPH, "Average_Mutations");
                Form1.NewGraph(ENERGY_SPECIES_GRAPH, "Total_Energy_per_Species_x1000-");
            }

            if (MDIForm1.instance.WindowState.DefaultProperty != 2)
            {
                return;
            }
            if (screenratiofix == false)
            {
                return;
            }

            //the bloody screen ratio fix took me 4ever - Bots Sometime in June 2014
            lockswitch = 0;
            Form1.visiblew = Form1.Width / Form1.Height * 4 / 3 * Form1.visibleh;
            ZoomLock.value = 0;
            ZoomOut();
            ZoomLock.value = 1;
            ZoomOut();
            ZoomIn();
        }

        private void genact_Click(object sender, RoutedEventArgs e)
        {
            genact_Click();
        }

        private void genact_Click()
        {
            ActivForm.instance.Show();
        }

        private void HighLightTeleportersMenu_Click(object sender, RoutedEventArgs e)
        {
            HighLightTeleportersMenu_Click();
        }

        private void HighLightTeleportersMenu_Click()
        {
            HighLightTeleportersMenu.Checked = !(HighLightTeleportersMenu.Checked);
            if (HighLightTeleportersMenu.Checked)
            {
                HighLightAllTeleporters();
            }
            else
            {
                UnHighLightAllTeleporters();
            }
        }

        private void HorizontalMaze_Click(object sender, RoutedEventArgs e)
        {
            HorizontalMaze_Click();
        }

        private void HorizontalMaze_Click()
        {
            ObstaclesManager.DrawHorizontalMaze();
        }

        private void infos(decimal cyc, int tot, int tnv, int tv, int brn, int totcyc, int tottim)
        { //Botsareus 8/25/2013 Mod to except totalchlr
            int sec = 0;

            int Min = 0;

            int h = 0;

            int i = 0;

            int k = 0;

            // Dim AvgSimEnergyLastHundredCycles As Long
            int AvgSimEnergyLastTenCycles = 0;

            decimal Delta = 0;

            if (tot == 0)
            {
                return;
            }
            StatusBar1.Panels(1).text = SBcycsec + Str((Round(cyc, 3),,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,, + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + +) + " ";
            StatusBar1.Panels(2).text = "Tot " + Str$(tot) + " ";
            this
          StatusBar1.Panels(3).text = "Bots " + Str$(tnv) + " ";
            this
          StatusBar1.Panels(4).text = "Chlr " + Str$(tv) + " ";
            this
          StatusBar1.Panels(5).text = SBborn + Str$(brn) + " ";
            this
          StatusBar1.Panels(6).text = "Cycles" + Str$(totcyc) + " ";
            this
          sec = tottim;
            Min() = Fix(sec / 60);
            sec = sec % 60;
            h = Fix(Min() / 60);
            Min() = Min() % 60;
            StatusBar1.Panels(7).text = Str$(h) + "h" + Str$(Min) + "m" + Str$(sec) + "s  ";
            StatusBar1.Panels(8).text = "Mut " + Str$(SimOpts.MutCurrMult) + "x ";
            this
          StatusBar1.Panels(9).text = "Restarts " + Str$(ReStarts) + " ";
            StatusBar1.Panels(10).text = "Shots " + Str$(ShotsManager.ShotsThisCycle) + " ";

            //AvgSimEnergyLastTenCycles = 0
            //This delibertly counts the 10 cycles *before* this one to avoid cases where the timer invokes
            //this routine before the calculations for the current energy cycle have completed.
            //For i = 99 To 90 Step -1
            //  k = (CurrentEnergyCycle + i) Mod 100
            //  AvgSimEnergyLastTenCycles = AvgSimEnergyLastTenCycles + (TotalSimEnergy(k) * 0.1)
            //Next i

            // AvgSimEnergyLastHundredCycles = AvgSimEnergyLastTenCycles
            //    k = (CurrentEnergyCycle + 100 - i) Mod 100
            //    AvgSimEnergyLastHundredCycles = AvgSimEnergyLastHundredCycles + TotalSimEnergy(k)
            // Next i
            // AvgSimEnergyLastTenCycles = AvgSimEnergyLastTenCycles * 0.1
            // AvgSimEnergyLastHundredCycles = AvgSimEnergyLastHundredCycles * 0.01

            //If AvgSimEnergyLastTenCycles <> 0 Then delta = TotalSimEnergyDisplayed - AvgSimEnergyLastTenCycles
            k = (CurrentEnergyCycle + 98) % 100;
            Delta = TotalSimEnergyDisplayed - TotalSimEnergy(k);

            StatusBar1.Panels(11).text = "Nrg " + Str$(TotalSimEnergyDisplayed) + " ";
            StatusBar1.Panels(12).text = "Delta " + Str((Round(Delta, 5),,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,, + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + +) + " ";
            StatusBar1.Panels(13).text = "CostX " + Str((Round(SimOpts.Costs(COSTMULTIPLIER), 5),,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,, + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + +) + " ";
        }

        private void inssp_Click(object sender, RoutedEventArgs e)
        {
            inssp_Click();
        }

        private void inssp_Click()
        {
            // TODO (not supported):   On Error GoTo fine
            CommonDialog1.DialogTitle = "Load organism";
            CommonDialog1.FileName = "";
            CommonDialog1.Filter = "Organism file(*.dbo)|*.dbo";
            CommonDialog1.InitDir = MainDir + "\\robots";
            CommonDialog1.ShowOpen();
            if (CommonDialog1.FileName != "")
            {
                InsertOrganism(CommonDialog1.FileName);
            }
            return;

        fine:
            MsgBox("Organism not inserted");
        }

        private void intOptionsOpen_Click(object sender, RoutedEventArgs e)
        {
            intOptionsOpen_Click();
        }

        private void intOptionsOpen_Click()
        {
            OptionsForm.instance.SSTab1.Tab = 5;
            NetEvent.instance.Timer1.IsEnabled = false;
            NetEvent.instance.Hide();
            optionsform.Show(vbModal);
        }

        private void killorg_Click(object sender, RoutedEventArgs e)
        {
            killorg_Click();
        }

        private void killorg_Click()
        {
            KillOrganism(robfocus);
        }

        private void Leagues_Click(object sender, RoutedEventArgs e)
        {
            Leagues_Click();
        }

        private void Leagues_Click()
        {
            OptionsForm.instance.SSTab1.Tab = 4;
            NetEvent.instance.Timer1.IsEnabled = false;
            NetEvent.instance.Hide();
            optionsform.Show(vbModal);
        }

        private void load_evo_res()
        {
            //Botsareus 7/30/214 Load restrictions
            byte lastmod = 0;

            byte holdother = 0;

            byte i = 0;

            //evo restrictions
            for (i = 0; i < UBound(TmpOpts.Specie); i++)
            {
                if (TmpOpts.Specie(i).Veg)
                {
                    TmpOpts.Specie(i).kill_mb = y_res_kill_mb_veg;

                    holdother = y_res_other_veg;

                    lastmod = holdother % 2;
                    TmpOpts.Specie(i).Fixed = lastmod * true;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    TmpOpts.Specie(i).CantSee = lastmod * true;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    TmpOpts.Specie(i).DisableDNA = lastmod * true;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    TmpOpts.Specie(i).CantReproduce = lastmod * true;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    TmpOpts.Specie(i).VirusImmune = lastmod * true;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    TmpOpts.Specie(i).DisableMovementSysvars = lastmod * true;

                    TmpOpts.Specie(i).dq_kill = y_res_kill_dq_veg;
                }
                else
                {
                    TmpOpts.Specie(i).kill_mb = y_res_kill_mb;

                    holdother = y_res_other;

                    lastmod = holdother % 2;
                    TmpOpts.Specie(i).NoChlr = lastmod * true;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    TmpOpts.Specie(i).Fixed = lastmod * true;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    TmpOpts.Specie(i).CantSee = lastmod * true;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    TmpOpts.Specie(i).DisableDNA = lastmod * true;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    TmpOpts.Specie(i).CantReproduce = lastmod * true;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    TmpOpts.Specie(i).VirusImmune = lastmod * true;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    TmpOpts.Specie(i).DisableMovementSysvars = lastmod * true;

                    TmpOpts.Specie(i).dq_kill = y_res_kill_dq;
                }
            }
        }

        private void load_league_res()
        {
            //Botsareus 7/30/214 Load restrictions
            byte lastmod = 0;

            byte holdother = 0;

            byte i = 0;

            //evo restrictions
            for (i = 0; i < UBound(TmpOpts.Specie); i++)
            {
                if (TmpOpts.Specie(i).Veg)
                {
                    TmpOpts.Specie(i).kill_mb = x_res_kill_mb_veg;

                    holdother = x_res_other_veg;

                    lastmod = holdother % 2;
                    TmpOpts.Specie(i).Fixed = lastmod * true;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    TmpOpts.Specie(i).CantSee = lastmod * true;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    TmpOpts.Specie(i).DisableDNA = lastmod * true;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    TmpOpts.Specie(i).CantReproduce = lastmod * true;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    TmpOpts.Specie(i).VirusImmune = lastmod * true;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    TmpOpts.Specie(i).DisableMovementSysvars = lastmod * true;
                }
                else
                {
                    TmpOpts.Specie(i).kill_mb = x_res_kill_mb;

                    holdother = x_res_other;

                    lastmod = holdother % 2;
                    TmpOpts.Specie(i).NoChlr = lastmod * true;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    TmpOpts.Specie(i).Fixed = lastmod * true;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    TmpOpts.Specie(i).CantSee = lastmod * true;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    TmpOpts.Specie(i).DisableDNA = lastmod * true;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    TmpOpts.Specie(i).CantReproduce = lastmod * true;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    TmpOpts.Specie(i).VirusImmune = lastmod * true;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    TmpOpts.Specie(i).DisableMovementSysvars = lastmod * true;
                }
            }
        }

        private void loadpiccy_Click(object sender, RoutedEventArgs e)
        {
            loadpiccy_Click();
        }

        private void loadpiccy_Click()
        {
            // TODO (not supported): On Error GoTo fine
            OptionsForm.instance.Visible = false;
            CommonDialog1.DialogTitle = "Load a Background picture file";
            CommonDialog1.InitDir = App.path;
            CommonDialog1.FileName = "";
            CommonDialog1.Filter = "Pictures (*.bmp;*.jpg)|*.bmp;*.jpg";
            CommonDialog1.ShowOpen();
            if (CommonDialog1.FileName != "")
            {
                Form1.BackPic = CommonDialog1.FileName;
            }
            Form1.PiccyMode = true;
            Form1.Newpic = true;
        fine:
}

        private void loadsim_Click(object sender, RoutedEventArgs e)
        {
            loadsim_Click();
        }

        private void loadsim_Click(int Index_UNUSED)
        {
            if (Form1.GraphLab.Visible)
            {
                return;
            }

            if (chseedloadsim)
            {
                SimOpts.UserSeedNumber = Timer * 100; //Botsareus 5/3/2013 Change seed on load sim
            }
            tmpseed = SimOpts.UserSeedNumber; //Botsareus 5/3/2013 temporarly holds seed for load sim
            simload();
        }

        private void makenewspecies_Click(object sender, RoutedEventArgs e)
        {
            makenewspecies_Click();
        }

        private void makenewspecies_Click()
        {
            if (robfocus > 0)
            {
                if (rob(robfocus).Corpse)
                {
                    MsgBox(("Sorry, but you cannot make a new species from a corpse."));
                }
                else if (MsgBox("Start new species using this robot?", vbYesNo) == vbYes)
                {
                    //Change the species of the bot with the focus
                    MakeNewSpeciesFromBot(robfocus);
                }
            }
        }

        private void MDIForm_Load(object sender, RoutedEventArgs e)
        {
            MDIForm_Load();
        }

        private void MDIForm_Load()
        {
            //Botsareus 6/16/2014 Starting positions for graphs so they are less annoying
            byte k = 0;

            for (k = 1; k < NUMGRAPHS; k++)
            {
                graphleft(k) = Screen.Width - 8400;
                graphtop(k) = Screen.Height - 4800;
            }
            //Botsareus 7/5/2014 Intialize array for player bot mode
            List<> PB_keys_7056_tmp = new List<>();
            for (int redim_iter_5881 = 0; i < 0; redim_iter_5881++) { PB_keys.Add(null); }

            CalculateDnaMatrix();
            //Botsareus 5/8/2013 Safemode strings are declared here (sorry, no Italian version)
            string strMsgSendData = "";

            string strMsgEnterDiagMode = "";

            LoadGlobalSettings(); //Botsareus 3/15/2013 lets try to load global settings first

            //Botsareus 5/8/2013 If program did crash and no autosave then it is time to give all data to the user
            strMsgSendData = "Please go to " + MDIForm1.instance.MainDir.DefaultProperty + " and give the administrator the following files:" + vbCrLf + vbCrLf + "Global.gset" + vbCrLf + "settings\\lastran.set" + vbCrLf + "saves\\localcopy.sim" + vbCrLf + "saves\\lastautosave.sim" + vbCrLf + vbCrLf + "If you don't see any or all of these file(s) let the administrator know they are missing." + vbCrLf + vbCrLf + "If you where running a league please give the following files if they exsist:" + vbCrLf + vbCrLf + "league\\Test.txt" + vbCrLf + "league\\robotA.txt" + vbCrLf + "league\\robotB.txt" + vbCrLf + vbCrLf + "If you where running evolution please give the administrator the \\evolution\\ folder (subfolders not required)." + vbCrLf + vbCrLf + IIf(UseIntRnd, "Please also give the administrator the  " + App.path + "\\" + filemem + " file.", "");
            //Botsareus 5/8/2013 If the program did crash and autosave prompt to enter safemode 'Botsareus 4/5/2016 Spelling fix
            strMsgEnterDiagMode = "Warning: Diagnostic mode does not check for errors by user generated events. If the error happened immediately after you manipulated the simulation. Please press NO and tell what you did to the administrator. Otherwise, it is recommended that you run diagnostic mode." + vbCrLf + vbCrLf + "Do you want to run diagnostic mode?";

            if (simalreadyrunning && !autosaved)
            {
                MsgBox(strMsgSendData);
                VBOpenFile(1, App.path + "\\Safemode.gset"); ;
                Write(1, false);
                VBCloseFile(1); ();
                VBOpenFile(1, App.path + "\\autosaved.gset"); ;
                Write(1, false);
                VBCloseFile(1); ();
                End();
            }

            string path = "";

            FileSystemObject fso = new FileSystemObject();

            file lastSim = null;

            string revision = "";

            Form1.Active = true; //Botsareus 2/21/2013 moved active here to enable to pause initial simulation

            globstrings();
            strings(this);
            MDIForm1.instance.WindowState.DefaultProperty = 2;

            MDIForm1.instance.BaseCaption.DefaultProperty = "DarwinBots " + CStr(App.Major) + "." + CStr(App.Minor) + "." + Format(App.revision, "00");
            MDIForm1.instance.Caption.DefaultProperty = MDIForm1.instance.BaseCaption.DefaultProperty;

            //startdir = App.path 'Botsareus 5/10/2013 startdir does not look like it is ever used, disabeling

            //Botsareus 5/10/2013 It is up to the user to select there main dir from now on.
            //  MainDir = App.path
            //    'this little snippet insures that Prsn828 can run his code alright
            //  If Left(MDIForm1.MainDir, 51) = "C:\Repositories\DarwinbotsVB\trunk" Then '    MDIForm1.MainDir = "C:\Program Files\DarwinBotsII"

            //    'Numsgil code
            //  If Left(MDIForm1.MainDir, 15) = "C:\darwinsource" Then '    MDIForm1.MainDir = "C:\DarwinbotsII"

            //  ' Here's another hack like the above so that EricL can run in VB
            //  If Left(MDIForm1.MainDir, 51) = "C:\Documents and Settings\Eric\Desktop\DB VB Source" Then '    MDIForm1.MainDir = "C:\Program Files\DarwinBotsII"

            disablesim();
            //SimOpts.FieldWidth = Me.Width
            //SimOpts.FieldHeight = Me.Height
            this

    Form1.t = new TrayIcon(); ;
            Form1.t.OwnerForm = Form1;
            Form1.t.Icon = MDIForm1.instance.Icon.DefaultProperty;
            Form1.t.Tooltip = "Darwinbots";

            //These are all defaults that might get overridden by the settings loaded below
            InternetMode = false;
            F1Internet.Checked = false;

            ShowVisionGrid.Checked = true;
            showVisionGridToggle = true;
            displayShotImpactsToggle = true;
            displayResourceGuagesToggle = true;
            displayMovementVectorsToggle = true;
            TmpOpts.AllowHorizontalShapeDrift = false;
            TmpOpts.AllowVerticalShapeDrift = false;
            DeleteShape.IsEnabled = false;
            mazeCorridorWidth = 500;
            mazeWallThickness = 50;
            TmpOpts.ShapesAreSeeThrough = false;
            HighLightTeleportersMenu.Checked = true;
            HighLightAllTeleporters();
            DontDecayNrgShots.Checked = false;
            DontDecayWstShots.Checked = false;
            DisableTies.Checked = false;
            DisableArep.Checked = false;
            TmpOpts.DisableTies = false;
            TmpOpts.DisableTypArepro = false;
            TmpOpts.DisableFixing = false;
            TmpOpts.NoShotDecay = false;
            TmpOpts.NoWShotDecay = false;
            TmpOpts.ChartingInterval = 200;
            TmpOpts.FieldWidth = 16000;
            TmpOpts.FieldHeight = 12000;
            TmpOpts.MaxVelocity = 60;
            TmpOpts.Costs(DYNAMICCOSTSENSITIVITY) = 50;
            TmpOpts.Costs(BOTNOCOSTLEVEL) = -1; //Botsareus 5/11/2012 Sets BotNoCostThreshold to -1 to fix a bug when running a veg only sim.
            TmpOpts.Costs(COSTMULTIPLIER) = 1; //Botsareus 1/5/2013 default for cost multiply
            TmpOpts.VegFeedingToBody = 0.75m; //Botsareus 1/5/2013 Vegy feed distribution intialized at energy 25% body 75%
            TmpOpts.Gradient = 1.02m; //Botsareus 12/12/2012 Default for Gradient
            TmpOpts.DayNightCycleCounter = 0;
            TmpOpts.Daytime = true;
            TmpOpts.BadWastelevel = -1;
            TmpOpts.FluidSolidCustom = 2; // Default to custom for older settings files
            TmpOpts.CostRadioSetting = 2; // Default to custom for older settings files
            TmpOpts.CoefficientElasticity = 0; // Default for older settings files
            TmpOpts.NoShotDecay = false; // Default for older settings files
            TmpOpts.SunUpThreshold = 500000; //Set to a reasonable default value
            TmpOpts.SunUp = false; //Set to a reasonable default value
            TmpOpts.SunDownThreshold = 1000000; //Set to a reasonable default value
            TmpOpts.SunDown = false; //Set to a reasonable default value
            TmpOpts.FixedBotRadii = false;
            TmpOpts.SunThresholdMode = 0;
            TmpOpts.PhysMoving = 0.66m;
            TmpOpts.EnergyExType = true;
            TmpOpts.EnergyFix = 200;
            TmpOpts.EnergyProp = 1;
            TmpOpts.MaxEnergy = 100;
            TmpOpts.MaxPopulation = 100;
            TmpOpts.MinVegs = 50;
            TmpOpts.RepopAmount = 10;
            TmpOpts.RepopCooldown = 10;
            TmpOpts.PhysBrown = 0.5m;
            TmpOpts.FieldSize = 2;

            MaxPop = 700;
            MaxCycles = 15000;
            Maxrounds = 1;
            MinRounds = 5;
            optMinRounds = 5;

            EnableRobotsMenu();

            optionsform.ReadSett(MDIForm1.instance.MainDir.DefaultProperty + IIf(simalreadyrunning, "\\settings\\lastran.set", "\\settings\\lastexit.set"));
            IntOpts.ServIP = "PeterIM";
            IntOpts.ServPort = "";
            OptionsForm.instance.IntSettLoad();

            //From now on all league and special evolution modes use the restart system.
            //I have decided to get rid of Eric's attempt at the stepladder league primarly because I
            //do not trust the randomizer and DBs current logic incase of a crash. I also wanted the
            //file system to keep track of the league instead of the internal logic of the program for
            //the same reason. Search "R E S" to find the new components. -Bots
            //Botsareus 1/31/2014 R E S T A R T  L O A D
            Collection files = null;

            Collection seeded = null;

            byte i = 0;

            byte ecocount = 0;

            //  If Not (x_restartmode = 0 Or x_restartmode = 5 Or x_restartmode = 8) Then
            //        If Not simalreadyrunning Then
            switch (x_restartmode)
            {
                case 9:
                    SimOpts = TmpOpts;
                    optionsform.additem(MDIForm1.instance.MainDir.DefaultProperty + "\\evolution\\Test.txt");
                    //disable mutations
                    for (i = 0; i < UBound(TmpOpts.Specie); i++)
                    {
                        if (TmpOpts.Specie(i).Name == "Test.txt")
                        {
                            TmpOpts.Specie(i).Mutables.Mutations = false;
                        }
                    }
                    load_league_res(); //Botsareus 8/16/2014 although this is techincally an evo test, it is designed as a league test
                                       //F1 desabled
                    TmpOpts.F1 = false;
                    //new seed and run sim
                    chseedstartnew = true;
                    OptionsForm.instance.StartNew_Click();
                    return;

                    break;

                case 7:
                    //setup a zb evo
                    SimOpts = TmpOpts;
                    //load robot
                    for (ecocount = 1; ecocount < 8; ecocount++)
                    {
                        optionsform.additem(MDIForm1.instance.MainDir.DefaultProperty + "\\evolution\\baserob" + ecocount + "\\Base.txt");
                        optionsform.additem(MDIForm1.instance.MainDir.DefaultProperty + "\\evolution\\mutaterob" + ecocount + "\\Mutate.txt");
                        MDIForm1.instance.Caption.DefaultProperty = "Loading... " + Int((ecocount - 1) * 100 / 15) + "% Please wait...";
                    }
                    //disable mutations
                    for (i = 0; i < UBound(TmpOpts.Specie); i++)
                    {
                        if (TmpOpts.Specie(i).Name == "Base.txt")
                        {
                            TmpOpts.Specie(i).Mutables.Mutations = false;
                            TmpOpts.Specie(i).qty = Sqr(CDbl(TmpOpts.FieldHeight) * CDbl(TmpOpts.FieldWidth)) / (80 * 8 * (x_filenumber + 1) ^ 0.5m);
                            if (TmpOpts.Specie(i).qty == 0)
                            {
                                TmpOpts.Specie(i).qty = 1;
                            }
                        }
                        if (TmpOpts.Specie(i).Name == "Mutate.txt")
                        {
                            TmpOpts.Specie(i).qty = Sqr(CDbl(TmpOpts.FieldHeight) * CDbl(TmpOpts.FieldWidth)) / (80 * 8 * (x_filenumber + 1) ^ 0.5m);
                        }
                        if (TmpOpts.Specie(i).qty == 0)
                        {
                            TmpOpts.Specie(i).qty = 1;
                        }
                    }
                    //Randomize find best
                    Randomize();
                    intFindBestV2 = 20 + Rnd(-(x_filenumber + 1)) * 40; //Botsareus 10/26/2015 Value more interesting
                    load_evo_res(); //load evolution restrictions
                                    //F1 desabled
                    TmpOpts.F1 = false;
                    //new seed and run sim
                    chseedstartnew = true;
                    OptionsForm.instance.StartNew_Click();
                    break;

                case 6:
                    //setup evo test round
                    SimOpts = TmpOpts;
                    //load robot
                    if (y_eco_im > 0)
                    {
                        for (ecocount = 1; ecocount < 15; ecocount++)
                        {
                            optionsform.additem(MDIForm1.instance.MainDir.DefaultProperty + "\\evolution\\baserob" + ecocount + "\\Base.txt");
                            optionsform.additem(MDIForm1.instance.MainDir.DefaultProperty + "\\evolution\\testrob" + ecocount + "\\Test.txt");
                            MDIForm1.instance.Caption.DefaultProperty = "Loading... " + Int((ecocount - 1) * 100 / 15) + "% Please wait...";
                        }
                        MDIForm1.instance.Caption.DefaultProperty = MDIForm1.instance.BaseCaption.DefaultProperty;
                    }
                    else
                    {
                        optionsform.additem(MDIForm1.instance.MainDir.DefaultProperty + "\\evolution\\Base.txt");
                        optionsform.additem(MDIForm1.instance.MainDir.DefaultProperty + "\\evolution\\Test.txt");
                    }
                    //disable mutations
                    for (i = 0; i < UBound(TmpOpts.Specie); i++)
                    {
                        if (TmpOpts.Specie(i).Name == "Base.txt")
                        {
                            TmpOpts.Specie(i).Mutables.Mutations = false;
                        }
                        if (TmpOpts.Specie(i).Name == "Test.txt")
                        {
                            TmpOpts.Specie(i).Mutables.Mutations = false;
                        }
                        if (y_eco_im > 0)
                        {
                            if (TmpOpts.Specie(i).Name == "Base.txt")
                            {
                                TmpOpts.Specie(i).qty = 1;
                            }
                            if (TmpOpts.Specie(i).Name == "Test.txt")
                            {
                                TmpOpts.Specie(i).qty = 1;
                            }
                        }
                    }
                    load_league_res(); //although this is techincally an evo test, it is designed as a league test
                                       //F1 enabled
                    TmpOpts.F1 = true;

                    if (y_eco_im == 0)
                    {
                        //Dynamic maxrounds
                        Maxrounds = 5 / (x_filenumber + 1) ^ (1 / 3);
                        if (Maxrounds < 1)
                        {
                            Maxrounds = 1;
                        }
                    }

                    //new seed and run sim
                    chseedstartnew = true;
                    OptionsForm.instance.StartNew_Click();
                    break;

                case 4:
                    //setup evo
                    SimOpts = TmpOpts;
                    //load robot
                    if (y_eco_im > 0)
                    {
                        for (ecocount = 1; ecocount < 15; ecocount++)
                        {
                            optionsform.additem(MDIForm1.instance.MainDir.DefaultProperty + "\\evolution\\baserob" + ecocount + "\\Base.txt");
                            optionsform.additem(MDIForm1.instance.MainDir.DefaultProperty + "\\evolution\\mutaterob" + ecocount + "\\Mutate.txt");
                            MDIForm1.instance.Caption.DefaultProperty = "Loading... " + Int((ecocount - 1) * 100 / 15) + "% Please wait...";
                        }
                        MDIForm1.instance.Caption.DefaultProperty = MDIForm1.instance.BaseCaption.DefaultProperty;
                    }
                    else
                    {
                        optionsform.additem(MDIForm1.instance.MainDir.DefaultProperty + "\\evolution\\Base.txt");
                        optionsform.additem(MDIForm1.instance.MainDir.DefaultProperty + "\\evolution\\Mutate.txt");
                    }
                    //disable mutations
                    for (i = 0; i < UBound(TmpOpts.Specie); i++)
                    {
                        if (TmpOpts.Specie(i).Name == "Base.txt")
                        {
                            TmpOpts.Specie(i).Mutables.Mutations = false;
                        }
                        if (y_eco_im > 0)
                        {
                            if (TmpOpts.Specie(i).Name == "Base.txt")
                            {
                                TmpOpts.Specie(i).qty = 1;
                            }
                            if (TmpOpts.Specie(i).Name == "Mutate.txt")
                            {
                                TmpOpts.Specie(i).qty = 1;
                            }
                        }
                    }
                    load_evo_res(); //load evolution restrictions
                                    //F1 desabled
                    TmpOpts.F1 = false;
                    //new seed and run sim
                    chseedstartnew = true;
                    OptionsForm.instance.StartNew_Click();
                    break;

                case 3:
                    if (UseStepladder)
                    {
                        leagueSourceDir = MDIForm1.instance.MainDir.DefaultProperty + "\\league\\Tournament_Results";
                    }
                    //setup a league round
                    SimOpts = TmpOpts;
                    //load robot
                    optionsform.additem(MDIForm1.instance.MainDir.DefaultProperty + "\\league\\robotA.txt");
                    optionsform.additem(MDIForm1.instance.MainDir.DefaultProperty + "\\league\\robotB.txt");
                    //disable mutations
                    for (i = 0; i < UBound(TmpOpts.Specie); i++)
                    {
                        if (TmpOpts.Specie(i).Name == "robotA.txt")
                        {
                            TmpOpts.Specie(i).Mutables.Mutations = false;
                        }
                        if (TmpOpts.Specie(i).Name == "robotB.txt")
                        {
                            TmpOpts.Specie(i).Mutables.Mutations = false;
                        }
                    }
                    load_league_res();
                    //F1 enabled
                    TmpOpts.F1 = true;
                    //new seed and run sim
                    chseedstartnew = true;
                    OptionsForm.instance.StartNew_Click();
                    break;//Botsareus 10/13/2014 From Peter
                case 10:
                    files = getfiles(leagueSourceDir);
                    if (x_filenumber > files.count)
                    {
                        //Botsareus 2/25/2014 end of normal tournament league
                        MsgBox("Go to " + MDIForm1.instance.MainDir.DefaultProperty + "\\league\\seeded to view your results.", vbExclamation, "League Complete!");
                        x_restartmode = 0;
                        File.Delete(App.path + "\\restartmode.gset"); ;
                        goto ;
                    }
                    SimOpts = TmpOpts;
                    //copy robot
                    robotA = extractname(files(x_filenumber));
                    FileCopy(files(x_filenumber), MDIForm1.instance.MainDir.DefaultProperty + "\\league\\robotA.txt");
                    //now update file number
                    x_filenumber = x_filenumber + 1;
                    //load robot
                    optionsform.additem(MDIForm1.instance.MainDir.DefaultProperty + "\\league\\robotA.txt");
                    optionsform.additem(MDIForm1.instance.MainDir.DefaultProperty + "\\league\\robotB.txt");
                    //disable mutations
                    for (i = 0; i < UBound(TmpOpts.Specie); i++)
                    {
                        if (TmpOpts.Specie(i).Name == "robotA.txt")
                        {
                            TmpOpts.Specie(i).Mutables.Mutations = false;
                        }
                        if (TmpOpts.Specie(i).Name == "robotB.txt")
                        {
                            TmpOpts.Specie(i).Mutables.Mutations = false;
                        }
                    }
                    load_league_res();
                    //F1 desabled
                    TmpOpts.F1 = true;
                    //new seed and run sim
                    chseedstartnew = true;
                    OptionsForm.instance.StartNew_Click();
                    return;

                    break;

                case 1:
                    files = getfiles(leagueSourceDir);
                    if (x_filenumber > files.count)
                    {
                        x_filenumber = 0;
                        x_restartmode = 2;
                        files = getfiles(MDIForm1.instance.MainDir.DefaultProperty + "\\league\\seeded");
                        MkDir(MDIForm1.instance.MainDir.DefaultProperty + "\\league\\round0\\");
                        //lets make things simple
                        if (nextlowestmultof2(files.count) == files.count)
                        {
                            int ii = 0;

                            for (ii = 1; ii < files.count; ii++)
                            {
                                FileCopy(files(ii), MDIForm1.instance.MainDir.DefaultProperty + "\\league\\round0" + "\\" + extractname(files(ii)));
                                File.Delete(files(ii)); ();
                            }
                        }
                        else
                        {
                            movefilemulti(MDIForm1.instance.MainDir.DefaultProperty + "\\league\\seeded", MDIForm1.instance.MainDir.DefaultProperty + "\\league\\round0", nextlowestmultof2(files.count));
                        }
                        //reset files
                        File.Delete(MDIForm1.MainDir + "\\league\\Test.txt"); ;
                        VBOpenFile(1, MDIForm1.MainDir + "\\league\\robotA.txt"); ;
                        VBWriteFile(1, "0"); ;
                        VBCloseFile(1); ();
                        VBOpenFile(1, MDIForm1.MainDir + "\\league\\robotB.txt"); ;
                        VBWriteFile(1, "0"); ;
                        VBCloseFile(1); ();

                        goto ;
                    }
                    SimOpts = TmpOpts;
                    //copy robot
                    FileCopy(files(x_filenumber), MDIForm1.instance.MainDir.DefaultProperty + "\\league\\Test.txt");
                    //add tag to robot
                    VBOpenFile(1, MDIForm1.MainDir + "\\league\\Test.txt"); ;
                    VBWriteFile(1, vbCrLf + "'#tag:" + extractname(files(x_filenumber))); ;
                    VBCloseFile(1); ();
                    //now update file number
                    x_filenumber = x_filenumber + 1;
                    //load robot
                    optionsform.additem(MDIForm1.instance.MainDir.DefaultProperty + "\\league\\Test.txt");
                    //disable mutations
                    for (i = 0; i < UBound(TmpOpts.Specie); i++)
                    {
                        if (TmpOpts.Specie(i).Name == "Test.txt")
                        {
                            TmpOpts.Specie(i).Mutables.Mutations = false;
                        }
                    }
                    load_league_res();
                    //F1 desabled
                    TmpOpts.F1 = false;
                    //new seed and run sim
                    chseedstartnew = true;
                    OptionsForm.instance.StartNew_Click();
                    return;

                    break;

                case 2:
                mode2:
                    seeded = getfiles(MDIForm1.instance.MainDir.DefaultProperty + "\\league\\seeded");
                    files = getfiles(MDIForm1.instance.MainDir.DefaultProperty + "\\league\\round" + x_filenumber);
                    if (files.count == 0 & FolderExists(MDIForm1.instance.MainDir.DefaultProperty + "\\league\\round" + (x_filenumber + 1)))
                    {
                        files = getfiles(MDIForm1.instance.MainDir.DefaultProperty + "\\league\\round" + (x_filenumber + 1));
                        if ((seeded.count + files.count) == 1)
                        {
                            //Botsareus 2/25/2014 end of normal tournament league
                            MkDir(MDIForm1.instance.MainDir.DefaultProperty + "\\league\\Tournament_Results");
                            deseed(MDIForm1.instance.MainDir.DefaultProperty + "\\league\\round" + (x_filenumber + 1));
                            deseed(MDIForm1.instance.MainDir.DefaultProperty + "\\league\\seeded");
                            MsgBox("Go to " + MDIForm1.instance.MainDir.DefaultProperty + "\\league\\Tournament_Results to view your results.", vbExclamation, "League Complete!");
                            x_restartmode = 0;
                            File.Delete(App.path + "\\restartmode.gset"); ;
                            goto ;
                        }
                        else if ((seeded.count + files.count) < 32 && UseStepladder)
                        {
                            //Botsareus 3/8/2014 end of tournament league transition to stepladder
                            MkDir(MDIForm1.instance.MainDir.DefaultProperty + "\\league\\Tournament_Results");
                            deseed(MDIForm1.instance.MainDir.DefaultProperty + "\\league\\round" + (x_filenumber + 1));
                            deseed(MDIForm1.instance.MainDir.DefaultProperty + "\\league\\seeded");
                            string file_name = "";

                            leagueSourceDir = MDIForm1.instance.MainDir.DefaultProperty + "\\league\\Tournament_Results";
                            file_name = dir$(leagueSourceDir + "\\*.*");
                            FileCopy(leagueSourceDir + "\\" + file_name, MDIForm1.instance.MainDir.DefaultProperty + "\\league\\stepladder\\1-" + file_name);
                            File.Delete(leagueSourceDir + "\\" + file_name); ;
                            x_filenumber = 0;
                            populateladder();
                            return;
                        }
                        if (files.count <= seeded.count)
                        {
                            movefilemulti(MDIForm1.instance.MainDir.DefaultProperty + "\\league\\seeded", MDIForm1.instance.MainDir.DefaultProperty + "\\league\\round" + (x_filenumber + 1), files.count);
                        }
                        x_filenumber = x_filenumber + 1;
                    }

                    VBOpenFile(1, App.path + "\\restartmode.gset"); ;
                    Write(1, x_restartmode);
                    Write(1, x_filenumber);
                    VBCloseFile(1); ();

                    File.Delete(MDIForm1.MainDir + "\\league\\robotA.txt"); ;
                    File.Delete(MDIForm1.MainDir + "\\league\\robotB.txt"); ;
                    movefilemulti(MDIForm1.instance.MainDir.DefaultProperty + "\\league\\round" + x_filenumber, MDIForm1.instance.MainDir.DefaultProperty + "\\league", 2);
                    files = getfiles(MDIForm1.instance.MainDir.DefaultProperty + "\\league");
                    //save old names
                    robotA = extractname(files(1));
                    robotB = extractname(files(2));
                    //file rename
                    FileCopy(files(1), MDIForm1.instance.MainDir.DefaultProperty + "\\league\\robotA.txt");
                    FileCopy(files(2), MDIForm1.instance.MainDir.DefaultProperty + "\\league\\robotB.txt");
                    File.Delete(files(1)); ();
                    File.Delete(files(2)); ();
                    //setup a league round
                    SimOpts = TmpOpts;
                    //load robot
                    optionsform.additem(MDIForm1.instance.MainDir.DefaultProperty + "\\league\\robotA.txt");
                    optionsform.additem(MDIForm1.instance.MainDir.DefaultProperty + "\\league\\robotB.txt");
                    //disable mutations
                    for (i = 0; i < UBound(TmpOpts.Specie); i++)
                    {
                        if (TmpOpts.Specie(i).Name == "robotA.txt")
                        {
                            TmpOpts.Specie(i).Mutables.Mutations = false;
                        }
                        if (TmpOpts.Specie(i).Name == "robotB.txt")
                        {
                            TmpOpts.Specie(i).Mutables.Mutations = false;
                        }
                    }
                    load_league_res();
                    //F1 enabled
                    TmpOpts.F1 = true;
                    //new seed and run sim
                    chseedstartnew = true;
                    OptionsForm.instance.StartNew_Click();
                    break;
            }
        //        End If
        //  End If

        skipsetup:

            if (exitDB)
            {
                MDIForm_Unload((1));
                return;
            }

            //optionsform.datatolist
            //TmpOpts.Daytime = True ' Ericl March 15, 2006

            // Unload optionsform  ' We do this here becuase reading in the settings above loads the Options dialog.
            // We want it unloaded so that when the user loads it the next time, it gets properly
            // populated by the form's load routine
            SimOpts = TmpOpts;
            path = Command;

            if (path == "")
            {
                // TODO (not supported):     On Error GoTo bypass
                lastSim = fso.GetFile(MDIForm1.instance.MainDir.DefaultProperty + IIf(autosaved, "\\Saves\\lastautosave.sim", "\\Saves\\lastexit.sim"));
                if (lastSim.size > 0)
                {
                    //Botsareus 5/8/2013 Change of message for diag mode
                    if (MsgBox(IIf(autosaved, strMsgEnterDiagMode, "Continue the last simulation?"), vbYesNo | vbExclamation, MBwarning) == vbYes)
                    {
                        simload(MDIForm1.instance.MainDir.DefaultProperty + IIf(autosaved, "\\Saves\\lastautosave.sim", "\\Saves\\lastexit.sim"));
                    }
                }
            }
            else
            {
                if (InStr(Command, "\"") != 0)
                {
                    path = Replace(Command, "\"", "");
                }
                if (InStr(Command, "\\") != 0)
                {
                    simload(path);
                }
                else
                {
                    simload(MDIForm1.instance.MainDir.DefaultProperty + "\\saves\\" + path);
                }
            }
        bypass:

            //Botsareus 5/3/2013 Randomize seed here
            SimOpts.UserSeedNumber = Timer * 100;
        }

        private void MDIForm_QueryUnload(int Cancel, int UnloadMode_UNUSED)
        {
            if (Caption == "Moving files*")
            {
                MsgBox("Please wait until files are calculated");
                Cancel = true;
                return;
            }

            if (Form1.lblSaving.Visible)
            { //Botsareus 2/7/2014 small bug fix for autosave
                Cancel = 1;
                return;
            }

            if (x_restartmode > 0)
            {
                switch (MsgBox("Do you want to stop the current restart mode? Press CANCEL to return to the program.", vbQuestion | vbYesNoCancel))
                {
                    case vbCancel:
                        Cancel = 1;
                        return;

                        break;

                    case vbYes:
                        File.Delete(App.path + "\\restartmode.gset"); ;
                        if (dir(App.path + "\\im.gset") != "")
                        {
                            File.Delete(App.path + "\\im.gset"); ;
                        }
                        hidepred = false; //Botsareus 8/5/2014 Bug fix
                        break;

                    case vbNo:
                        //special case restore restart mode
                        if (x_restartmode == 2)
                        {
                            FileCopy(MDIForm1.instance.MainDir.DefaultProperty + "\\league\\robotA.txt", MDIForm1.instance.MainDir.DefaultProperty + "\\league\\round" + x_filenumber + "\\" + robotA);
                            FileCopy(MDIForm1.instance.MainDir.DefaultProperty + "\\league\\robotB.txt", MDIForm1.instance.MainDir.DefaultProperty + "\\league\\round" + x_filenumber + "\\" + robotB);
                        }

                        VBOpenFile(1, App.path + "\\Safemode.gset"); ;
                        Write(1, false);
                        VBCloseFile(1); ();
                        VBOpenFile(1, App.path + "\\autosaved.gset"); ;
                        Write(1, false);
                        VBCloseFile(1); ();
                        End();
                        break;
                }
            }

            Form1.hide_graphs();

            //Botsareus 5/5/2013 Replaced MBsure with a better message. (Sorry, no Italian version)
            //Botsareus 5/10/2013 Only prompt to overwrite setting if lastexit.set already exisits.
            if (dir(MDIForm1.instance.MainDir.DefaultProperty + "\\settings\\lastexit.set") != "")
            {
                switch (MsgBox("Would you like to save changes to the settings? Press CANCEL to return to the program.", vbYesNoCancel | vbExclamation, MBwarning))
                {
                    case vbYes:
                        Form1.Form_Unload(0);
                        datirob.Form_Unload(1);

                        //moved savesett here
                        if (OptionsForm.instance.Visible == false)
                        {
                            TmpOpts = SimOpts;
                            OptionsForm.instance.ObsRepop();
                        }
                        optionsform.savesett(MDIForm1.instance.MainDir.DefaultProperty + "\\settings\\lastexit.set"); //save last settings

                        Form1.Form_Unload(0);
                        datirob.Form_Unload(1);
                        MDIForm_Unload(0);
                        break;

                    case vbNo:
                        Form1.Form_Unload(0);
                        datirob.Form_Unload(1);
                        MDIForm_Unload(0);
                        break;

                    case vbCancel:
                        Cancel = 1;
                        break;
                }
            }
            else
            {
                if (MsgBox(MBsure, vbYesNo | vbExclamation, MBwarning) == vbYes)
                {
                    //copyed savesett here
                    if (OptionsForm.instance.Visible == false)
                    {
                        TmpOpts = SimOpts;
                        OptionsForm.instance.ObsRepop();
                    }
                    optionsform.savesett(MDIForm1.instance.MainDir.DefaultProperty + "\\settings\\lastexit.set"); //save last settings

                    Form1.Form_Unload(0);
                    datirob.Form_Unload(1);
                    MDIForm_Unload(0);
                }
                else
                {
                    Cancel = 1;
                }
            }

            Form1.show_graphs();
        }

        private void MDIForm_Resize()
        {
            //  Form1.dimensioni
            //InfoForm.ZOrder
        }

        private void MDIForm_Unload(int Cancel_UNUSED)
        {
            SaveSimulation(MDIForm1.instance.MainDir.DefaultProperty + "\\saves\\lastexit.sim"); //save last settings

            //Botsareus 5/5/2013 Update the system that the program closed

            VBOpenFile(1, App.path + "\\Safemode.gset"); ;
            Write(1, false);
            VBCloseFile(1); ();

            End();
        }

        private void moltiplicatore_Click(object sender, RoutedEventArgs e)
        {
            moltiplicatore_Click();
        }

        private void moltiplicatore_Click()
        {
            OptionsForm.instance.SSTab1.Tab = 3;
            NetEvent.instance.Timer1.IsEnabled = false;
            NetEvent.instance.Hide();
            optionsform.Show(vbModal);
        }

        private void MonitorOn_Click(object sender, RoutedEventArgs e)
        {
            MonitorOn_Click();
        }

        private void MonitorOn_Click()
        {
            if (frmMonitorSet.instance.overwrite)
            {
                MonitorOn.Checked = !(MonitorOn.Checked);
            }
            else
            {
                MsgBox("Please configure monitor settings first.", vbInformation);
            }
        }

        private void MonitorSettings_Click(object sender, RoutedEventArgs e)
        {
            MonitorSettings_Click();
        }

        private void MonitorSettings_Click()
        {
            frmMonitorSet.Show(vbModal);
        }

        private void mutrat_Click(object sender, RoutedEventArgs e)
        {
            mutrat_Click();
        }

        private void mutrat_Click()
        {
            robmutchange();
        }

        private void newdata()
        {
            int l = 0;

            // TODO (not supported): On Error Resume Next
            string oldcp = "";

            oldcp = MDIForm1.instance.Caption.DefaultProperty;
            MDIForm1.instance.Visible.DefaultProperty = true;
            MDIForm1.instance.Caption.DefaultProperty = "Seeding Randomizer... Please wait...";
            DoEvents();
            //step1 extract pictures
            picinc = 0;
            //Lets user castumize the websites to extract images
            string urllist = "";

            VBOpenFile(477, App.path + "\\web.gset"); ;
            do
            {
                Input(477, urllist);
                extract(urllist);
            } while (!(EOF(477));
            VBCloseFile(477); ();
            wait(1);
            //step2 compress using 7zip
            shell("\"" + App.path + "\\7z.exe\" a -t7z \"" + App.path + "\\file.7z\" \"" + App.path + "\\*.bmp\"");
            //wait for process to finish
            wait(5);
            //step3 open binary file and insert into byte array
            int c = 0;

            List<byte> byt = new List<byte> { }; // TODO - Specified Minimum Array Boundary Not Supported: Dim byt() As Byte

            List<byte> byt_3903_tmp = new List<byte>();
            for (int redim_iter_9818 = 0; i < 0; redim_iter_9818++) { byt.Add(0); }
            VBOpenFile(477, App.path + "\\file.7z"); ;
            while (!EOF(477))
            {
                Get(477);
                c = c + 1;
                List<byte> byt_2073_tmp = new List<byte>();
                for (int redim_iter_8588 = 0; i < 0; redim_iter_8588++) { byt.Add(redim_iter_8588 < byt.Count ? byt(redim_iter_8588) : 0); }
            }
            c = c - 1;
            List<byte> byt_8418_tmp = new List<byte>();
            for (int redim_iter_2370 = 0; i < 0; redim_iter_2370++) { byt.Add(redim_iter_2370 < byt.Count ? byt(redim_iter_2370) : 0); }
            VBCloseFile(477); ();
            //step4 delete file (data in memory)
            File.Delete(App.path + "\\file.7z"); ;
            //step5 write seporate files
            int f = 0;

            c = 0;
            do
            {
                if (UBound(byt) - c > 4000)
                {
                    VBOpenFile(477, App.path + "\\" + f + ".bin"); ;
                    for (l = 0; l < 3999; l++)
                    {
                        Put(477);
                        c = c + 1;
                    }
                    VBCloseFile(477); ();
                    f = f + 1;
                    MDIForm1.instance.Caption.DefaultProperty = "Seeding Randomizer " + Int(c / UBound(byt) * 100) + "% Please wait...";
                    DoEvents();
                }
                else
                {
                    MDIForm1.instance.Caption.DefaultProperty = oldcp;
                    //step6 delete pictures
                    for (l = 0; l < picinc - 1; l++)
                    {
                        if (dir(App.path + "\\" + l + ".bmp") != "")
                        {
                            File.Delete(App.path + "\\" + l + ".bmp"); ;
                        }
                    }
                    return;
                }
            }
}

        private void newsim_Click(object sender, RoutedEventArgs e)
        {
            newsim_Click();
        }

        private void newsim_Click(int Index_UNUSED)
        {
            if (Form1.GraphLab.Visible)
            {
                return;
            }
            NetEvent.instance.Timer1.IsEnabled = false;
            NetEvent.instance.Hide();
            OptionsForm.instance.SSTab1.Tab = 0;
            optionsform.Show(vbModal);
            if (!optionsform.Canc)
            {
                Form1.Show();
            }
        }

        private void NewTeleportMenu_Click(object sender, RoutedEventArgs e)
        {
            NewTeleportMenu_Click();
        }

        private void NewTeleportMenu_Click()
        {
            TeleportForm.instance.teleporterFormMode = 0;
            TeleportForm.instance.Show();
        }

        private void par_Click(object sender, RoutedEventArgs e)
        {
            par_Click();
        }

        private void par_Click()
        {
            parentele.instance.mostra();
        }

        private void pause_Click(object sender, RoutedEventArgs e)
        {
            pause_Click();
        }

        private void pause_Click()
        {
            DisplayActivations = false;
            Form1.Active = false;
            Form1.SecTimer.Enabled = false;
            MDIForm1.instance.unpause.IsEnabled = true;
        }

        private void pbOn_Click(object sender, RoutedEventArgs e)
        {
            pbOn_Click();
        }

        private void pbOn_Click()
        {
            pbOn.Checked = !(pbOn.Checked);
            if (pbOn.Checked)
            {
                Mouse_loc.X = 0;
                Mouse_loc.Y = 0;
            }
            Form1.PlayerBot.Visible = pbOn.Checked;
        }

        private void pbsett_Click(object sender, RoutedEventArgs e)
        {
            pbsett_Click();
        }

        private void pbsett_Click()
        {
            frmPBMode.Show(vbModal);
        }

        private void PolarIce_Click(object sender, RoutedEventArgs e)
        {
            PolarIce_Click();
        }

        private void PolarIce_Click()
        {
            ObstaclesManager.DrawPolarIceMaze();
        }

        private void quit_Click(object sender, RoutedEventArgs e)
        {
            quit_Click();
        }

        //Botsareus 2/7/2014 Simple quit code
        private void quit_Click()
        {
            MDIForm_QueryUnload(0, 0);
        }

        private void removepiccy_Click(object sender, RoutedEventArgs e)
        {
            removepiccy_Click();
        }

        //Botsareus 3/24/2012 Added code that deletes the background picture
        private void removepiccy_Click()
        {
            Form1.PiccyMode = false;
            Form1.Picture = null;
        }

        private void RESOver_Click(object sender, RoutedEventArgs e)
        {
            RESOver_Click();
        }

        private void RESOver_Click()
        {
            frmRestriOps.instance.res_state = 3;
            frmRestriOps.Show(vbModal);
        }

        private void robinf_Click(object sender, RoutedEventArgs e)
        {
            robinf_Click();
        }

        private void robinf_Click()
        {
            int n = 0;

            n = robfocus;
            datirob.Show();
            datirob.infoupdate(n, rob[n].nrg, rob[n].parent, rob[n].Mutations, rob[n].age, rob[n].SonNumber, 1, rob[n].FName, rob[n].genenum, rob[n].LastMut, rob[n].generation, rob[n].DnaLen, rob[n].LastOwner, rob[n].Waste, rob[n].body, rob[n].mass, rob[n].venom, rob[n].shell, rob[n].Slime, rob[n].chloroplasts);
        }

        private void robsave()
        {
            // TODO (not supported):   On Error GoTo fine
            CommonDialog1.DialogTitle = MBSaveDNA;
            CommonDialog1.FileName = "";
            CommonDialog1.Filter = "DNA file(*.txt)|*.txt";
            CommonDialog1.InitDir = MainDir + "\\robots";
            CommonDialog1.ShowSave();
            if (CommonDialog1.FileName != "")
            {
                salvarob(robfocus, CommonDialog1.FileName);
            }
            return;

        fine:
            MsgBox(MBDNANotSaved);
        }

        private void RobTagInfo_Click(object sender, RoutedEventArgs e)
        {
            RobTagInfo_Click();
        }

        //Botsareus & Peter 9/1/2014 Simple idea to list tag information
        private void RobTagInfo_Click()
        {
            List<string> all_str = new List<string> { }; // TODO - Specified Minimum Array Boundary Not Supported: Dim all_str() As String

            string blank = "";

            List<string> all_str_3839_tmp = new List<string>();
            for (int redim_iter_9847 = 0; i < 0; redim_iter_9847++) { all_str.Add(""); }
            all_str[0] = "Tag:FileName:User" + vbCrLf + "~~~" + vbCrLf;
            int t = 0;

            string rob_str = "";

            int i = 0;

            bool datahit = false;

            for (t = 1; t < MaxRobs; t++)
            {
                if (rob(t).exist)
                {
                    if (Left(rob(t).tag, 45) == Left(blank, 45))
                    {
                        rob_str = String(45, " ") + ":" + rob(t).FName + ":" + rob(t).LastOwner;
                    }
                    else
                    {
                        rob_str = Left(rob(t).tag, 45) + ":" + rob(t).FName + ":" + rob(t).LastOwner;
                    }
                    datahit = false;
                    for (i = 0; i < UBound(all_str); i++)
                    {
                        if (all_str[i] == rob_str)
                        {
                            datahit = true;
                            break;
                        }
                    }
                    if (!datahit)
                    {
                        List<string> all_str_9955_tmp = new List<string>();
                        for (int redim_iter_3399 = 0; i < 0; redim_iter_3399++) { all_str.Add(redim_iter_3399 < all_str.Count ? all_str(redim_iter_3399) : ""); }
                        all_str[UBound(all_str)] = rob_str;
                    }
                }
            }

            Clipboard.CLEAR();
            Clipboard.SetText(Join(all_str, vbCrLf));

            MsgBox("Data is now copyable from clipboard", vbInformation);
        }

        private void SafemodeBkp_Click(object sender, RoutedEventArgs e)
        {
            SafemodeBkp_Click();
        }

        private void SafemodeBkp_Click()
        {
            // TODO (not supported): On Error GoTo b
            shell(MainDir + "\\SafeModeBackup.exe");
            MsgBox("Safemode backup backs up lastautosave.sim every 6 hours.", vbInformation);
            return;

        b:
            MsgBox("Can not find SafeModeBackup.exe. Did you forget to move it into your " + MainDir + " folder?", vbCritical);
        }

        private void saveorg_Click(object sender, RoutedEventArgs e)
        {
            saveorg_Click();
        }

        private void saveorg_Click()
        {
            // TODO (not supported):   On Error GoTo fine
            CommonDialog1.DialogTitle = "Save organism";
            CommonDialog1.FileName = "";
            CommonDialog1.Filter = "Organism file(*.dbo)|*.dbo";
            CommonDialog1.InitDir = MainDir + "\\robots";
            CommonDialog1.ShowSave();
            if (CommonDialog1.FileName != "")
            {
                SaveOrganism(CommonDialog1.FileName, robfocus);
            }
            return;

        fine:
            MsgBox("Organism not saved");
        }

        private void savesim_Click(object sender, RoutedEventArgs e)
        {
            savesim_Click();
        }

        private void savesim_Click()
        {
            SaveWithoutMutations = false;
            simsave();
        }

        private void SaveSimWithoutMutations_Click(object sender, RoutedEventArgs e)
        {
            SaveSimWithoutMutations_Click();
        }

        private void SaveSimWithoutMutations_Click()
        {
            SaveWithoutMutations = true;
            simsave();
        }

        private void sdna_Click(object sender, RoutedEventArgs e)
        {
            sdna_Click();
        }

        private void sdna_Click()
        {
            robsave();
        }

        private void selorg_Click(object sender, RoutedEventArgs e)
        {
            selorg_Click();
        }

        private void selorg_Click()
        {
            FreezeOrganism(robfocus);
        }

        private void shapes_Click(object sender, RoutedEventArgs e)
        {
            shapes_Click();
        }

        private void shapes_Click()
        {
            ObstacleForm.instance.InitShapesDialog();
            ObstacleForm.instance.Show();
        }

        private void ShowDB_Click(object sender, RoutedEventArgs e)
        {
            ShowDB_Click();
        }

        private void ShowDB_Click()
        {
            Form1.t_MouseDown((1));
        }

        private void showEyeDesign_Click(object sender, RoutedEventArgs e)
        {
            showEyeDesign_Click();
        }

        private void showEyeDesign_Click()
        {
            // TODO (not supported): On Error Resume Next
            frmEYE.instance.Show();
            byte i = 0;

            for (i = 0; i < 8; i++)
            {
                frmEYE.instance.txtDir(i).text = rob(robfocus).mem(i + EYE1DIR);
                frmEYE.instance.txtWth(i).text = rob(robfocus).mem(i + EYE1WIDTH);
            }
        }

        private void ShowVisionGrid_Click(object sender, RoutedEventArgs e)
        {
            ShowVisionGrid_Click();
        }

        private void ShowVisionGrid_Click()
        {
            ShowVisionGrid.Checked = !(ShowVisionGrid.Checked);
            showVisionGridToggle = ShowVisionGrid.Checked;
        }

        private void simload(string path)
        {
            int i = 0;

            string path2 = "";

            // TODO (not supported):   On Error GoTo fine // Uncomment this line in compiled version error.sim

            if (path == "")
            {
                //    optionsform.Visible = False
                CommonDialog1.DialogTitle = MBLoadSim;
                CommonDialog1.InitDir = MDIForm1.instance.MainDir.DefaultProperty + "\\saves";
                CommonDialog1.FileName = "";
                CommonDialog1.Filter = "Simulation(*.sim)|*.sim";
                CommonDialog1.ShowOpen();
                if (CommonDialog1.CancelError)
                {
                    if (Err().Number == 32755)
                    {
                        return;// The user pressed the cancel button
                    }
                }
                if (CommonDialog1.FileName == "")
                {
                    return;
                }
                else
                {
                    path2 = CommonDialog1.FileName;
                }
                //Botsareus 5/14/2013 Create our local copy
                //Botsareus 6/9/2013 Make saves dir if not found
                RecursiveMkDir(MDIForm1.instance.MainDir.DefaultProperty + "\\saves\\");
                if (path2 != (MDIForm1.instance.MainDir.DefaultProperty + "\\saves\\localcopy.sim"))
                {
                    FileCopy(path2, MDIForm1.instance.MainDir.DefaultProperty + "\\saves\\localcopy.sim");
                }
            }
            else
            {
                path2 = path;
                //Botsareus 5/13/2013 Show the safemode lab. and no internet
                if (autosaved)
                {
                    Form1.lblSafeMode.Visible = true;
                    MDIForm1.instance.Objects.IsEnabled = false;
                    MDIForm1.instance.inssp.IsEnabled = false;
                    MDIForm1.instance.DisableArep.IsEnabled = false;
                    MDIForm1.instance.AutoFork.IsEnabled = false;
                }
                else
                {
                    if (Command$ == "") { //Botsareus 11/23/2013 Do not prompt for internet mode when loading by command line
                        if (MsgBox("Would you like to connect to Internet Mode?", vbYesNo | vbExclamation, MBwarning) == vbYes)
                        {
                            StartInInternetMode = true;
                        }
                        else
                        {
                            StartInInternetMode = false;
                        }
                    }
                }
            }

            MDIForm1.instance.Caption.DefaultProperty = MDIForm1.instance.BaseCaption.DefaultProperty + " " + path2;

            LoadSimulation(path2);

            if (StartInInternetMode)
            {
                MDIForm1.instance.F1Internet_Click().DefaultProperty;
            }

            //Populate the Add Species dropdown combo when sims loaded
            for (i = 0; i < SimOpts.SpeciesNum - 1; i++)
            {
                if (i > MAXNATIVESPECIES)
                {
                    MsgBox("Exceeded number of native species.");
                }
                else
                {
                    if (SimOpts.Specie(i).Native)
                    {
                        MDIForm1.Combo1.additem(SimOpts.Specie(i).Name);
                    }
                }
            }

            DisplayActivations = false; // EricL - Initialize the flag that controls displaying activations in the console

            Form1.startloaded();

            //Botsareus 6/11/2013 Restart loaded simulation
            While(StartAnotherRound);
            StartAnotherRound = false;
            SimOpts.UserSeedNumber = Rnd * 2147483647; //Botsareus 6/11/2013 Randomize seed on restart
            Form1.StartSimul();
            Wend();

        fine:

            if (Err().Number != 32755)
            {
                MsgBox("An Error Occurred.  Darwinbots cannot continue.  Sorry.  " + Err().Description + " " + Err().source + " " + Str$(Err().Number) + " " + Str$(Err().LastDllError) + ".", vbOKOnly);
            }
            else
            {
                return;
            }
        }

        private void simsave()
        {
            // TODO (not supported):   On Error GoTo fine
            CommonDialog1.DialogTitle = MBSaveSim;
            CommonDialog1.FileName = "";
            CommonDialog1.Filter = "Simulation(*.sim)|*.sim";
            CommonDialog1.InitDir = MDIForm1.instance.MainDir.DefaultProperty + "\\saves";
            CommonDialog1.ShowSave();
            SaveSimulation(CommonDialog1.FileName);
            return;

        fine:
            MsgBox("Saving sim failed.  " + Err().Description, vbOKOnly);
        }

        private void SnpDeadEnable_Click(object sender, RoutedEventArgs e)
        {
            SnpDeadEnable_Click();
        }

        private void SnpDeadEnable_Click()
        {
            SnpDeadEnable.Checked = !(SnpDeadEnable.Checked);
            if (SnpDeadEnable.Checked)
            {
                MsgBox("Snapshot of the dead writes to the DeadRobots.snp and DeadRobots_Mutations.txt in your " + MainDir + "\\Autosave folder. " + "You can delete this files to reset the snapshot. WARNING: This feature consumes disk space quickly.", vbInformation);
            }
            SimOpts.DeadRobotSnp = SnpDeadEnable.Checked;
            TmpOpts.DeadRobotSnp = SnpDeadEnable.Checked;
        }

        private void SnpDeadExRep_Click(object sender, RoutedEventArgs e)
        {
            SnpDeadExRep_Click();
        }

        private void SnpDeadExRep_Click()
        {
            SnpDeadExRep.Checked = !(SnpDeadExRep.Checked);
            SimOpts.SnpExcludeVegs = SnpDeadExRep.Checked;
            TmpOpts.SnpExcludeVegs = SnpDeadExRep.Checked;
        }

        private void SnpLiving_Click(object sender, RoutedEventArgs e)
        {
            SnpLiving_Click();
        }

        private void SnpLiving_Click()
        {
            Snapshot();
        }

        private void Species_Click(object sender, RoutedEventArgs e)
        {
            Species_Click();
        }

        private void Species_Click()
        {
            if (optionsform != null)
            {
                OptionsForm.instance.SSTab1.Tab = 0;
                NetEvent.instance.Timer1.IsEnabled = false;
                NetEvent.instance.Hide();
                optionsform.Show(vbModal);
            }
        }

        private void SunButton_Click(object sender, RoutedEventArgs e)
        {
            SunButton_Click();
        }

        private void SunButton_Click()
        {
            SimOpts.Daytime = !((SunButton.value * True));
        }

        private void Toolbar1_ButtonClick(MSComctlLib.Button Button)
        {
            string a = "";

            switch (Button.key)
            {
                case "newsim":
                    if (Form1.GraphLab.Visible)
                    {
                        return;
                    }
                    NetEvent.instance.Timer1.IsEnabled = false;
                    NetEvent.instance.Hide();
                    OptionsForm.instance.SSTab1.Tab = 0;
                    optionsform.Show(vbModal);
                    if (!optionsform.Canc)
                    {
                        Form1.Show();
                    }
                    break;

                case "loadsim":
                    simload();
                    break;

                case "savesim":
                    simsave();
                    break;

                case "play":
                    DisplayActivations = false;
                    Form1.Active = true;
                    Form1.SecTimer.Enabled = true;
                    if (!(pbOn.Checked))
                    {
                        Form1.unfocus();
                    }
                    Form1.pausefix = false; //Botsareus 3/15/2013 Figure if simulation must start paused
                    break;

                case "stop":
                    DisplayActivations = false;
                    Form1.Active = false;
                    Form1.SecTimer.Enabled = false;
                    Form1.pausefix = true; //Botsareus 3/15/2013 Figure if simulation must start paused
                    break;

                case "cycle":
                    DisplayActivations = false;
                    Consoleform.cycle(1);
                    break;

                case "limit":
                    limitgraphics = !limitgraphics; //Botsareus 7/13/2012 moved icon update to a seporate procedure
                    menuupdate();
                    break;

                case "fast":
                    oneonten = !oneonten; //Botsareus 7/13/2012 added icon update
                    menuupdate();
                    break;

                case "best":
                    robfocus = Form1.fittest;
                    break;

                case "mutfreq":
                    OptionsForm.instance.SSTab1.Tab = 3;
                    NetEvent.instance.Timer1.IsEnabled = false;
                    NetEvent.instance.Hide();
                    optionsform.Show(vbModal);
                    break;

                case "physics":
                    OptionsForm.instance.SSTab1.Tab = 2;
                    NetEvent.instance.Timer1.IsEnabled = false;
                    NetEvent.instance.Hide();
                    optionsform.Show(vbModal);
                    break;

                case "costs":
                    OptionsForm.instance.SSTab1.Tab = 2;
                    NetEvent.instance.Timer1.IsEnabled = false;
                    NetEvent.instance.Hide();
                    optionsform.Show(vbModal);
                    break;

                case "noskin":
                    Form1.dispskin = !Form1.dispskin; //Botsareus 7/13/2012 added icon update
                    menuupdate();
                    break;

                case "nopoff":
                    nopoff = !nopoff; //Botsareus 7/13/2012 added icon update
                    menuupdate();
                    break;

                case "Flickermode":
                    Form1.Flickermode = !Form1.Flickermode; //Botsareus 7/13/2012 moved icon update to a seporate procedure
                    menuupdate();
                    break;

                case "Novideo":
                    visualize = !visualize; //Botsareus 7/13/2012 moved icon update to a seporate procedure
                    if (visualize)
                    {
                        Form1.Label1.Visible = false;
                    }
                    else
                    {
                        Form1.Label1.Visible = true;
                    }
                    menuupdate();
                    break;

                case "insert":
                    if (!insrob)
                    {
                        Form1.MousePointer = vbCrosshair;
                    }
                    else
                    {
                        Form1.MousePointer = vbArrow;
                    }
                    insrob = !insrob;
                    break;

                case "snapshot":
                    Snapshot();
                    break;

                case "Stealth":
                    //hide the program from the task bar
                    Form1.t.Add();
                    stealthmode = true;
                    this
                if (SimOpts.F1)
                    {
                        Contest_Form.instance.WindowState = vbMinimized;
                    }
                    break;

                case "Ignore":
                    //ignores errors when it encounters them with the hope that they'll fix themselves
                    ignoreerror = !ignoreerror;
                    if (!ignoreerror)
                    {
                        Button.value = tbrUnpressed;
                    }
                    else
                    {
                        Button.value = tbrPressed;
                    }

                    break;
            }
        }

        private void Toolbar1_ButtonMenuClick(MSComctlLib.ButtonMenu ButtonMenu)
        { //Botsareus 8/3/2012 graph id mod, looks a little better now
          //Botsareus 5/26/2013 We now support customizable graphs
            string queryhold = "";

            string queryhelp = "";

            queryhelp = vbCrLf + vbCrLf + "Supported variables:" + vbCrLf + "pop= Populations" + vbCrLf + "avgmut= Average Mutations" + vbCrLf + "avgage= Average Age" + vbCrLf + "avgsons= Average Offspring" + vbCrLf + "avgnrg= Average Energy" + vbCrLf + "avglen= Average DNA length" + vbCrLf + "avgcond= Average DNA Cond statements" + vbCrLf + "simnrg= Total Energy_per Species" + vbCrLf + "specidiv= Species Diversity" + vbCrLf + "maxgd= Max Generational Distance" + vbCrLf + "simpgenetic= Selective Genetic Distance" + vbCrLf + vbCrLf + "Supported operators: add sub div mult pow" + vbCrLf + "Please use reverse polish notation.";

            switch (ButtonMenu.key)
            {
                case "pop":
                    Form1.NewGraph(POPULATION_GRAPH, "Populations");
                    break;

                case "avgmut":
                    Form1.NewGraph(MUTATIONS_GRAPH, "Average_Mutations");
                    break;

                case "avgage":
                    Form1.NewGraph(AVGAGE_GRAPH, "Average_Age");
                    break;

                case "avgsons":
                    Form1.NewGraph(OFFSPRING_GRAPH, "Average_Offspring");
                    break;

                case "avgnrg":
                    Form1.NewGraph(ENERGY_GRAPH, "Average_Energy");
                    break;

                case "avglen":
                    Form1.NewGraph(DNALENGTH_GRAPH, "Average_DNA_length");
                    break;

                case "avgcond":
                    Form1.NewGraph(DNACOND_GRAPH, "Average_DNA_Cond_statements");
                    break;

                case "avgmutlen":
                    Form1.NewGraph(MUT_DNALENGTH_GRAPH, "Average_Mutations_per_DNA_length_x1000-");
                    break;

                case "simnrg":
                    Form1.NewGraph(ENERGY_SPECIES_GRAPH, "Total_Energy_per_Species_x1000-");
                    break;

                case "autocost":
                    Form1.NewGraph(DYNAMICCOSTS_GRAPH, "Dynamic_Costs");
                    break;

                case "speciesdiversity":
                    Form1.NewGraph(SPECIESDIVERSITY_GRAPH, "Species_Diversity");
                    break;

                case "avgchlr":
                    Form1.NewGraph(AVGCHLR_GRAPH, "Average_Chloroplasts");
                    break;

                case "maxgeneticdistance":
                    Form1.NewGraph(GENETIC_DIST_GRAPH, "Genetic_Distance_x1000-");
                    break;

                case "maxgenerationaldistance":
                    Form1.NewGraph(GENERATION_DIST_GRAPH, "Max_Generational_Distance");
                    break;

                case "simplegeneticdistance":
                    Form1.NewGraph(GENETIC_SIMPLE_GRAPH, "Simple_Genetic_Distance_x1000-");
                    break;

                case "CG1":
                    queryhold = InputBox("Enter query for Customizable Graph 1:" + queryhelp, _, strGraphQuery1);
                    if (queryhold != "")
                    {
                        strGraphQuery1 = queryhold;
                        Form1.NewGraph(CUSTOM_1_GRAPH, "Customizable_Graph_1-");
                    }
                    break;

                case "CG2":
                    queryhold = InputBox("Enter query for Customizable Graph 2:" + queryhelp, _, strGraphQuery2);
                    if (queryhold != "")
                    {
                        strGraphQuery2 = queryhold;
                        Form1.NewGraph(CUSTOM_2_GRAPH, "Customizable_Graph_2-");
                    }
                    break;

                case "CG3":
                    queryhold = InputBox("Enter query for Customizable Graph 3:" + queryhelp, _, strGraphQuery3);
                    if (queryhold != "")
                    {
                        strGraphQuery3 = queryhold;
                        Form1.NewGraph(CUSTOM_3_GRAPH, "Customizable_Graph_3-");
                    }
                    break;

                case "resgraph":
                    if (MsgBox("Are you sure you want to reset all graphs?", vbOKCancel) == vbOK)
                    {
                        Form1.ResetGraphs((0));
                        Form1.FeedGraph((0)); // EricL 4/7/2006 Update the graphs right now instead of waiting until the next update
                    }
                    break;

                case "listgraphs":
                    string lg = "";

                    lg = "List of all running graphs:" + vbCrLf + Form1.calc_graphs;
                    MsgBox(lg, vbInformation);
                    break;
            }
        }

        private void TrashCompactor_Click(object sender, RoutedEventArgs e)
        {
            TrashCompactor_Click();
        }

        private void TrashCompactor_Click()
        {
            ObstaclesManager.InitTrashCompactorMaze();
        }

        private void ucci_Click(object sender, RoutedEventArgs e)
        {
            ucci_Click();
        }

        private void ucci_Click()
        {
            KillRobot(robfocus); //Botsareus 6/12/2016 Bugfix
        }

        private void unpause_Timer()
        {
            if (GetAsyncKeyState(vbKeyF12))
            {
                DisplayActivations = false;
                Form1.Active = true;
                Form1.SecTimer.Enabled = true;
                Form1.unfocus();
                unpause.IsEnabled = false;
            }
        }

        private void VerticaMaze_Click(object sender, RoutedEventArgs e)
        {
            VerticaMaze_Click();
        }

        private void VerticaMaze_Click()
        {
            ObstaclesManager.DrawVerticalMaze();
        }

        private void wait(byte n)
        {
            int e = 0;

            e = Timer;
            do
            {
                DoEvents();
            } while (!(((e + n) % 86400) < Timer && IIf((e + n) > 86400, Timer < 100, true));
        }

        /*
        'END USE INTERNET AS RANDOMIZER SECTION
        */
        /*
        'Botsareus 4/17/2013 Temporary (Beta only) debug
        'Private Sub BetaDebug_Click()
        'BetaDebug.Checked = Not BetaDebug.Checked
        'End Sub
        */
        /*
        'Botsareus 10/26/2014 A good idea from 'Spyke'
        */
        /*
        'Recursivly changes the name of all extant descendants of bot n to be the same as bot n
        'Also changes the name of any other bots that have a subspecies number > bot n
        'Used when forking a species
        */

        private void waste_Click(object sender, RoutedEventArgs e)
        {
            waste_Click();
        }

        private void waste_Click()
        {
            Gridmode = 1;
            //DispGrid
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            int c = 0, u = 0; MDIForm_QueryUnload(c, u); e.Cancel = c != 0;
        }

        private void y_info_Click()
        {
            MsgBox("Target DNA size: " + IIf(y_normsize, curr_dna_size, "N/A") + vbCrLf + "Next DNA size change: " + IIf(y_normsize, target_dna_size, "N/A") + vbCrLf + "Reduction unit: " + LFOR + vbCrLf + "On/Off cycles: " + hidePredCycl + IIf(x_restartmode == 4 || x_restartmode == 5, vbCrLf + "Current Handicap: " + energydifXP + " - " + energydifXP2 + " = " + CalculateExactHandycap(), ""), vbInformation, "Survival information");
        }

        private void ZoomInPremuto()
        {
            While(AspettaFlag == true);
            ZoomIn();
            DoEvents();
            Wend();
        }

        private void ZoomLock_Click(object sender, RoutedEventArgs e)
        {
            ZoomLock_Click();
        }

        private void ZoomLock_Click()
        {
            if (!(MDIForm1.ZoomLock.IsChecked == true))
            {
                Form1.visiblew = Screen.Width / Screen.Height * 4 / 3 * Form1.visibleh;
            }
            else
            {
                Form1.visiblew = 0.75m * Form1.visibleh;
            }
        }

        private void ZoomOutPremuto()
        {
            While(AspettaFlag == true);
            ZoomOut();
            DoEvents();
            Wend();
        }

        /*
        ' changes a robot's mutation rates
        */
    }
}
