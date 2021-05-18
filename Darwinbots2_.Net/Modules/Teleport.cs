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


static class Teleport
{
    // Copyright (c) 2006, 2007 Eric Lockard
    // eric@sulaadventures.com
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
    //inbound folder for im
    //outbound folder for im
    //Outbound
    // Inbound
    public class Teleporter
    {
        public bool exist = false;
        public vector pos = null;
        public decimal Width = 0;
        public decimal Height = 0;
        public int color = 0;
        public vector vel = null;
        public string path = "";
        public string intInPath = "";
        public string intOutPath = "";
        public bool In = false;
        public bool = false;
        public bool local = false;
        public bool Internet = false;
        public bool driftHorizontal = false;
        public bool driftVertical = false;
        public bool highlight = false;
        public bool teleportVeggies = false;
        public bool teleportCorpses = false;
        public bool RespectShapes = false;
        public int NumTeleported = 0;
        public int NumTeleportedIn = 0;
        public vector center = null;
        public bool teleportHeterotrophs = false;
        public int InboundPollCycles = 0;
        public int BotsPerPoll = 0;
        public int PollCountDown = 0;
        public int BackFlowLimit = 0;
    }
    public const dynamic MAXTELEPORTERS = 10;
    public static int numTeleporters = 0;
    public static int teleporterFocus = 0;
    public static int teleporterDefaultWidth = 0;
    public static List<Teleporter> Teleporters = new List<Teleporter>(new Teleporter[(MAXTELEPORTERS + 1)]);  // TODO: Confirm Array Size By Token


    public static int NewTeleporter(bool PortIn, bool PortOut, decimal Height, bool Internet)
    {
        int NewTeleporter = 0;
        int i = 0;

        dynamic randomX = null;
        dynamic randomy = null;
        decimal aspectRatio = 0;


        if (numTeleporters + 1 > MAXTELEPORTERS)
        {
            NewTeleporter = -1;
        }
        else
        {
            numTeleporters = numTeleporters + 1;
            NewTeleporter = numTeleporters;
            Teleporters(numTeleporters).exist = true;

            aspectRatio = CSng(SimOpts.FieldHeight / SimOpts.FieldWidth);

            randomX = Random(0, SimOpts.FieldWidth - (teleporterDefaultWidth * aspectRatio));
            randomy = Random(0, SimOpts.FieldHeight - teleporterDefaultWidth);

            Teleporters(numTeleporters).pos = VectorSet(randomX, randomy);
            Teleporters(numTeleporters).Width = Height * aspectRatio;
            Teleporters(numTeleporters).Height = Height;
            Teleporters(numTeleporters).vel = VectorSet(0, 0);
            Teleporters(numTeleporters).color = vbWhite;
            // Teleporters(numTeleporters).path = path
            Teleporters(numTeleporters).In = PortIn;
            Teleporters(numTeleporters).= PortOut;
            Teleporters(numTeleporters).Internet = Internet;
            Teleporters(numTeleporters).driftHorizontal = true;
            Teleporters(numTeleporters).driftVertical = true;
            Teleporters(numTeleporters).NumTeleported = 0;
            Teleporters(numTeleporters).NumTeleportedIn = 0;

            if (Internet)
            {
                if (IntOpts.InboundPath != "")
                {
                    Teleporters(numTeleporters).intInPath = IntOpts.InboundPath;
                }
                else
                {
                    Teleporters(numTeleporters).intInPath = MDIForm1.instance.MainDir + "\\IM\\inbound";
                }
                if (IntOpts.OutboundPath != "")
                {
                    Teleporters(numTeleporters).intOutPath = IntOpts.OutboundPath;
                }
                else
                {
                    Teleporters(numTeleporters).intOutPath = MDIForm1.instance.MainDir + "\\IM\\outbound";
                }
            }

        }
        return NewTeleporter;
    }

    public static bool ResizeTeleporter(int i, decimal Height)
    {
        bool ResizeTeleporter = false;
        decimal aspectRatio = 0;


        ResizeTeleporter = false;

        if (!Teleporters(i).exist)
        {
            return ResizeTeleporter;

        }
        aspectRatio = CSng(SimOpts.FieldHeight / SimOpts.FieldWidth);
        Teleporters(i).Width = Height * aspectRatio;
        Teleporters(i).Height = Height;

        ResizeTeleporter = true;

        return ResizeTeleporter;
    }

    public static bool ResizeInternetTeleporter(decimal Height)
    {
        bool ResizeInternetTeleporter = false;
        int i = 0;


        ResizeInternetTeleporter = false;
        if (!InternetMode)
        {
            return ResizeInternetTeleporter;

        }

        for (i = 1; i < numTeleporters; i++)
        {
            if (Teleporters(i).exist && Teleporters(i).Internet)
            {
                ResizeInternetTeleporter = ResizeTeleporter(i, Height);
                return ResizeInternetTeleporter;

            }
        }

        return ResizeInternetTeleporter;
    }

    public static dynamic DeleteAllTeleporters()
    {
        dynamic DeleteAllTeleporters = null;
        int i = 0;


        for (i = 1; i < numTeleporters; i++)
        {
            Teleporters(i).exist = false;
        }
        numTeleporters = 0;
        MDIForm1.instance.DeleteTeleporterMenu.IsEnabled = false;
        return DeleteAllTeleporters;
    }

    public static dynamic DeleteTeleporter(int i)
    {
        dynamic DeleteTeleporter = null;
        int x = 0;

        if (numTeleporters <= 0)
        {
            return DeleteTeleporter;

        }
        for (x = i + 1; x < numTeleporters; x++)
        {
            Teleporters(x - 1) == Teleporters(x);
        }
        Teleporters(numTeleporters).exist = false;
        numTeleporters = numTeleporters - 1;
        if (teleporterFocus == i)
        {
            MDIForm1.instance.DeleteTeleporterMenu.IsEnabled = false;
        }

        return DeleteTeleporter;
    }

    public static dynamic CheckTeleporters(int n)
    {
        dynamic CheckTeleporters = null;
        int i = 0;

        string Name = "";

        vector randomV = null;


        for (i = 1; i < numTeleporters; i++)
        {
            if (Teleporters(i).|| Teleporters(i).local || (Teleporters(i).Internet && Teleporters(i).PollCountDown <= 0))
            {
                if ((TeleportCollision(n, i) || rob[n].dq > 1) && rob[n].exist)
                {
                    if (Teleporters(i).|| Teleporters(i).Internet)
                    {
                        if (rob[n].dq > 1)
                        {
                            goto forceteleport;
                        }
                        if ((rob[n].Veg && !Teleporters(i).teleportVeggies) || (rob[n].Corpse && !Teleporters(i).teleportCorpses) || ((!rob[n].Veg) && (!Teleporters(i).teleportHeterotrophs)))
                        {
                            //Don't Teleport
                        }
                        else
                        {
                        forceteleport:
                            Teleporters(i).NumTeleported = Teleporters(i).NumTeleported + 1;
                            Name = "\\" + (Format(DateTime.Today, "yymmdd")) + Format(Time, "hhmmss") + rob[n].FName + CStr(i) + CStr(Teleporters(i).NumTeleported) + ".dbo";
                            if (Teleporters(i).Out)
                            {
                                SaveOrganism(Teleporters(i).path + Name, n);
                            }
                            if (Teleporters(i).Internet)
                            {
                                SaveOrganism(Teleporters(i).intOutPath + Name, n);
                            }
                            KillOrganism(n);
                        }
                    }
                    else if (Teleporters(i).local)
                    {
                        if ((rob[n].Veg && !Teleporters(i).teleportVeggies) || (rob[n].Corpse && !Teleporters(i).teleportCorpses) || ((!rob[n].Veg) && (!Teleporters(i).teleportHeterotrophs)))
                        {
                            //Don't Teleport
                        }
                        else
                        {
                            if (Teleporters(i).local)
                            {
                                Teleporters(i).NumTeleported = Teleporters(i).NumTeleported + 1; // Don't update the counter for Internet Mode teleporters
                            }
                            randomV = VectorSet(SimOpts.FieldWidth * Rndy(), SimOpts.FieldHeight * Rndy());
                            if (MDIForm1.instance.visualize)
                            {
                                //Form1.Line(rob[n].pos.X, rob[n].pos.Y) - (randomV.x, randomV.y), vbWhite);
                            }
                            ReSpawn(n, CLng(randomV.x), CLng(randomV.y));
                        }
                    }
                }
            }
        }
        return CheckTeleporters;
    }

    public static bool TeleportCollision(int n, int t)
    {
        bool TeleportCollision = false;
        decimal botrightedge = 0;

        decimal botleftedge = 0;

        decimal bottopedge = 0;

        decimal botbottomedge = 0;


        TeleportCollision = false;

        if (VectorMagnitude(VectorSub(rob[n].pos, Teleporters(t).center)) < Teleporters(t).Width / 2 + rob[n].radius)
        {
            TeleportCollision = true;
        }

        return TeleportCollision;
    }

    public static dynamic DrawTeleporters()
    {
        dynamic DrawTeleporters = null;
        int i = 0;

        int sm = 0;

        decimal telewidth = 0;

        decimal zoomRatio = 0;

        decimal aspectRatio = 0;

        decimal twipWidth = 0;

        int scw = 0;
        int sch = 0;
        int scm = 0;

        int sct = 0;
        int scl = 0;

        decimal pictwidth = 0;

        decimal pictmod = 0;

        int hilightcolor = 0;

        int visibleLeft = 0;

        int visibleRight = 0;

        int visibleTop = 0;

        int visibleBottom = 0;


        visibleLeft = Form1.ScaleLeft;
        visibleRight = Form1.ScaleLeft + Form1.ScaleWidth;
        visibleTop = Form1.ScaleTop;
        visibleBottom = Form1.ScaleTop + Form1.ScaleHeight;

        zoomRatio = Form1.ScaleWidth / SimOpts.FieldWidth;
        aspectRatio = SimOpts.FieldHeight / SimOpts.FieldWidth;

        Form1.FillStyle = 1;

        for (i = 1; i < numTeleporters; i++)
        {
            if (SimOpts.TotRunCycle >= 0)
            {
                if ((Form1.visiblew / RobSize) < 1000 & Teleporters(i).pos.x > visibleLeft && Teleporters(i).pos.x < visibleRight && Teleporters(i).pos.y > visibleTop && Teleporters(i).pos.y < visibleBottom)
                {
                    pictwidth = (Form1.Teleporter.Picture.Height) * zoomRatio * SimOpts.FieldWidth / (2 * Form1.Width);
                    pictmod = (SimOpts.TotRunCycle % 16) * pictwidth * 1.134m + Form1.ScaleLeft;

                    Form1.PaintPicture(Form1.TeleporterMask.Picture, Teleporters(i).pos.x, Teleporters(i).pos.y, Teleporters(i).Width, Teleporters(i).Height, pictmod, Form1.ScaleTop, pictwidth);

                    Form1.PaintPicture(Form1.Teleporter.Picture, Teleporters(i).pos.x, Teleporters(i).pos.y, Teleporters(i).Width, Teleporters(i).Height, pictmod, Form1.ScaleTop, pictwidth);
                }

                if (Teleporters(i).highlight && Teleporters(i).pos.x > visibleLeft && Teleporters(i).pos.x < visibleRight && Teleporters(i).pos.y > visibleTop && Teleporters(i).pos.y < visibleBottom)
                {
                    if (Teleporters(i).In)
                    {
                        hilightcolor = vbGreen;
                    }
                    if (Teleporters(i).Out)
                    {
                        hilightcolor = vbRed;
                    }
                    if (Teleporters(i).local)
                    {
                        hilightcolor = vbYellow;
                    }
                    if (Teleporters(i).Internet)
                    {
                        hilightcolor = vbBlue;
                    }
                    Form1.Circle((Teleporters(i).pos.x + (Teleporters(i).Width / 2), Teleporters(i).pos.y + (Teleporters(i).Height / 3)), Teleporters(i).Width * 0.6m, hilightcolor);
                }

                if (i == teleporterFocus && Teleporters(i).pos.x > visibleLeft && Teleporters(i).pos.x < visibleRight && Teleporters(i).pos.y > visibleTop && Teleporters(i).pos.y < visibleBottom)
                {
                    Form1.Circle((Teleporters(i).pos.x + (Teleporters(i).Width / 2), Teleporters(i).pos.y + (Teleporters(i).Height / 3)), Teleporters(i).Width * 0.7m, vbWhite);
                }

            }
        }

        Form1.FillStyle = 0;
        // Form1.ScaleMode = sm     (SimOpts.TotRunCycle Mod 16) * (telewidth) * zoomRatio * SimOpts.FieldSize * aspectRatio * Teleporters(i).Height / Form1.Teleporter.Picture.Height + Form1.ScaleLeft,
        return DrawTeleporters;
    }

    public static dynamic HighLightAllTeleporters()
    {
        dynamic HighLightAllTeleporters = null;
        int i = 0;

        for (i = 1; i < MAXTELEPORTERS; i++)
        {
            Teleporters(i).highlight = true;
        }
        return HighLightAllTeleporters;
    }

    public static dynamic UnHighLightAllTeleporters()
    {
        dynamic UnHighLightAllTeleporters = null;
        int i = 0;

        for (i = 1; i < MAXTELEPORTERS; i++)
        {
            Teleporters(i).highlight = false;
        }
        return UnHighLightAllTeleporters;
    }

    public static dynamic DriftTeleporter(int i)
    {
        dynamic DriftTeleporter = null;
        decimal vel = 0;


        vel = SimOpts.MaxVelocity / 4;
        if (Teleporters(i).driftHorizontal)
        {
            Teleporters(i).vel.x = Teleporters(i).vel.x + (Rndy() - 0.5m);
        }
        if (Teleporters(i).driftVertical)
        {
            Teleporters(i).vel.y = Teleporters(i).vel.y + (Rndy() - 0.5m);
        }
        if (VectorMagnitude(Teleporters(i).vel) > vel)
        {
            Teleporters(i).vel = VectorScalar(Teleporters(i).vel, vel / VectorMagnitude(Teleporters(i).vel));
        }
        return DriftTeleporter;
    }

    public static dynamic MoveTeleporter(int i)
    {
        dynamic MoveTeleporter = null;

        if (Teleporters(i).driftHorizontal && Teleporters(i).driftVertical)
        {
            Teleporters(i).pos = VectorAdd(Teleporters(i).pos, Teleporters(i).vel);
        }
        Teleporters(i).center = VectorSet(Teleporters(i).pos.x + (Teleporters(i).Width * 0.5m), Teleporters(i).pos.y + (Teleporters(i).Height * 0.3m));

        //Keep teleporters from drifting off into space.
        dynamic _WithVar_6271;
        _WithVar_6271 = Teleporters(i);
        if (_WithVar_6271.pos.x < 0)
        {
            if (_WithVar_6271.pos.x + _WithVar_6271.Width < 0)
            {
                _WithVar_6271.pos.x = 0;
            }
            if (SimOpts.Dxsxconnected == true)
            {
                _WithVar_6271.pos.x = _WithVar_6271.pos.x + SimOpts.FieldWidth - _WithVar_6271.Width;
            }
            else
            {
                _WithVar_6271.vel.x = SimOpts.MaxVelocity * 0.1m;
            }
        }
        if (_WithVar_6271.pos.y < 0)
        {
            if (_WithVar_6271.pos.y + _WithVar_6271.Height < 0)
            {
                _WithVar_6271.pos.y = 0;
            }
            if (SimOpts.Updnconnected == true)
            {
                _WithVar_6271.pos.y = _WithVar_6271.pos.y + SimOpts.FieldHeight - _WithVar_6271.Height;
            }
            else
            {
                _WithVar_6271.vel.y = SimOpts.MaxVelocity * 0.1m;
            }
        }
        if (_WithVar_6271.pos.x + _WithVar_6271.Width > SimOpts.FieldWidth)
        {
            if (_WithVar_6271.pos.x > SimOpts.FieldWidth)
            {
                _WithVar_6271.pos.x = SimOpts.FieldWidth - _WithVar_6271.Width;
            }
            if (SimOpts.Dxsxconnected == true)
            {
                _WithVar_6271.pos.x = _WithVar_6271.pos.x - (SimOpts.FieldWidth - _WithVar_6271.Width);
            }
            else
            {
                _WithVar_6271.vel.x = -SimOpts.MaxVelocity * 0.1m;
            }
        }
        if (_WithVar_6271.pos.y + _WithVar_6271.Height > SimOpts.FieldHeight)
        {
            if (_WithVar_6271.pos.y > SimOpts.FieldHeight)
            {
                _WithVar_6271.pos.y = SimOpts.FieldHeight - _WithVar_6271.Height;
            }
            if (SimOpts.Updnconnected == true)
            {
                _WithVar_6271.pos.y = _WithVar_6271.pos.y - (SimOpts.FieldHeight - _WithVar_6271.Height);
            }
            else
            {
                _WithVar_6271.vel.y = -SimOpts.MaxVelocity * 0.1m;
            }
        }

        return MoveTeleporter;
    }

    public static dynamic TeleportInBots()
    {
        dynamic TeleportInBots = null;
        int i = 0;

        int n = 0;

        string sFile = "";

        int lElement = 0;

        List<string> sAns = new List<string> { }; // TODO - Specified Minimum Array Boundary Not Supported: Dim sAns() As String

        List<string> sAns_1403_tmp = new List<string>();
        for (int redim_iter_1683 = 0; i < 0; redim_iter_1683++) { sAns.Add(""); }
        vector randomV = null;

        int MaxBotsPerCyclePerTeleporter = 0;

        bool temp = false;


        if (SimOpts.SpeciesNum > 45)
        {
            return TeleportInBots;//Botsareus 3/25/2014 keeps amount of species below 50

        }

        //MaxBotsPerCyclePerTeleporter = 10

        //Form1.SecTimer.Enabled = False
        for (i = 1; i < numTeleporters; i++)
        {
            if (Teleporters(i).In)
            {
                if (Teleporters(i).PollCountDown <= 0)
                {
                    Teleporters(i).PollCountDown = Teleporters(i).InboundPollCycles;
                    MaxBotsPerCyclePerTeleporter = Teleporters(i).BotsPerPoll;
                    // TODO (not supported):         On Error GoTo abandonthiscycle
                    sFile = dir(Teleporters(i).path + "\\", vbNormal + vbHidden + vbReadOnly + vbSystem + vbArchive);
                    While(sFile != "" & MaxBotsPerCyclePerTeleporter > 0);
                    sAns[0] = sFile;
                    lElement = IIf(sAns[0] == "", 0, UBound(sAns) + 1);
                    List<string> sAns_8321_tmp = new List<string>();
                    for (int redim_iter_2874 = 0; i < 0; redim_iter_2874++) { sAns.Add(redim_iter_2874 < sAns.Count ? sAns(redim_iter_2874) : ""); }
                    sAns[lElement] = sFile;
                    if (Right(sFile, 3) == "dbo")
                    {
                        n = LoadOrganism(Teleporters(i).path + "\\" + sAns[lElement], Teleporters(i).pos.x + Teleporters(i).Width / 2, Teleporters(i).pos.y + Teleporters(i).Height / 3);
                        Teleporters(i).NumTeleportedIn = Teleporters(i).NumTeleportedIn + 1;
                        File.Delete((Teleporters(i).path + "\\" + sAns(lElement))); ;
                        MaxBotsPerCyclePerTeleporter = MaxBotsPerCyclePerTeleporter - 1;
                        sFile = dir;
                    }
                    else if (Right(sFile, 4) == "temp")
                    { //Botsareus 2/21/2014 Added code to ignore temp files
                        sFile = dir;
                    }
                    else
                    {
                        MsgBox(("Non dbo file " + sFile + "found in " + Teleporters(i).path + ".  Inbound Teleporter Deleted."));
                        Teleporters(i).exist = false;
                        sFile = "";
                    }
                    Wend();
                }
                else
                {
                    Teleporters(i).PollCountDown = Teleporters(i).PollCountDown - 1;
                }
            }
            if (Teleporters(i).Internet)
            {
                if (Teleporters(i).PollCountDown <= 0)
                {
                    Teleporters(i).PollCountDown = Teleporters(i).InboundPollCycles;
                    MaxBotsPerCyclePerTeleporter = Teleporters(i).BotsPerPoll;
                    // TODO (not supported):         On Error GoTo abandonthiscycle
                    sFile = dir(Teleporters(i).intInPath + "\\", vbNormal + vbHidden + vbReadOnly + vbSystem + vbArchive);
                    While(sFile != "" && MaxBotsPerCyclePerTeleporter > 0);
                    sAns[0] = sFile;
                    lElement = IIf(sAns[0] == "", 0, UBound(sAns) + 1);
                    List<string> sAns_2345_tmp = new List<string>();
                    for (int redim_iter_8006 = 0; i < 0; redim_iter_8006++) { sAns.Add(redim_iter_8006 < sAns.Count ? sAns(redim_iter_8006) : ""); }
                    sAns[lElement] = sFile;
                    if (Right(sFile, 3) == "dbo")
                    {
                        randomV = VectorSet(SimOpts.FieldWidth * Rndy(), SimOpts.FieldHeight * Rndy());
                        n = LoadOrganism(Teleporters(i).intInPath + "\\" + sAns[lElement], randomV.X, randomV.Y);
                        Teleporters(i).NumTeleportedIn = Teleporters(i).NumTeleportedIn + 1;
                        File.Delete((Teleporters(i).intInPath + "\\" + sAns(lElement))); ;
                        MaxBotsPerCyclePerTeleporter = MaxBotsPerCyclePerTeleporter - 1;
                        sFile = dir;
                    }
                    else if (Right(sFile, 4) == "temp")
                    { //Botsareus 2/21/2014 Added code to ignore temp files
                        sFile = dir;
                    }
                    else
                    {
                        MsgBox(("Non dbo file " + sFile + "found in " + Teleporters(i).intInPath + ".  Inbound Teleporter Deleted."));
                        Teleporters(i).exist = false;
                        sFile = "";
                    }
                    Wend();
                }
                else
                {
                    Teleporters(i).PollCountDown = Teleporters(i).PollCountDown - 1;
                }
            }
        abandonthiscycle:;
        }

        //Form1.SecTimer.Enabled = True
        return TeleportInBots;
    }

    public static dynamic UpdateTeleporters()
    {
        dynamic UpdateTeleporters = null;
        int i = 0;

        for (i = 1; i < numTeleporters; i++)
        {
            if (SimOpts.TotRunCycle >= 0)
            {
                DriftTeleporter(i);
                MoveTeleporter(i);
            }
        }

        TeleportInBots();
        return UpdateTeleporters;
    }

    public static int whichTeleporter(decimal x, decimal y)
    {
        int whichTeleporter = 0;
        int t = 0;

        whichTeleporter = 0;
        for (t = 1; t < numTeleporters; t++)
        {
            if (x >= Teleporters(t).pos.x && x <= Teleporters(t).pos.x + Teleporters(t).Width && y >= Teleporters(t).pos.y && y <= Teleporters(t).pos.y + Teleporters(t).Height)
            {
                whichTeleporter = t;
                return whichTeleporter;

            }
        }
        return whichTeleporter;
    }
}
