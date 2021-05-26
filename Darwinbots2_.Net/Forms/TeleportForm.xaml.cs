using System.Windows;
using static Common;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.Interaction;
using static SimOpt;
using static Teleport;
using static VBExtension;

namespace DBNet.Forms
{
    public partial class TeleportForm : Window
    {
        // Copyright (c) 2006 Eric Lockard
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
        public int teleporterFormMode = 0;

        private static TeleportForm _instance;

        public TeleportForm()
        {
            InitializeComponent();
        }

        public static TeleportForm instance { set { _instance = null; } get { return _instance ?? (_instance = new TeleportForm()); } }

        public static void Load()
        {
            if (_instance == null) { dynamic A = TeleportForm.instance; }
        }

        public static void Unload()
        {
            if (_instance != null) instance.Close(); _instance = null;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CancelButton_Click();
        }

        private void CancelButton_Click()
        {
            this
        }

        private void Form_Activate()
        {
            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE + SWP_NOSIZE); //Botsareus 1/28/2014 Teleport form is always on top

            decimal aspectRatio = 0;

            aspectRatio = SimOpts.FieldHeight / SimOpts.FieldWidth;

            if (teleporterFormMode == 0)
            {
                teleporterDefaultWidth = 300;
                TeleporterSizeSlider.value = teleporterDefaultWidth;
                FixedCheck.value = false;
                TeleportOption(2).value = true;
                NetworkPath.IsEnabled = false;
                TeleportVeggiesCheck.value = 1;
                TeleportCorpsesCheck.value = 1;
                TeleportHeterotrophsCheck.value = 1;
                RespectShapesCheck.value = 0;
                InboundCycleCheck.text = "10";
                BotsPerPoll.Text = "10";
                intInText.text = IntOpts.InboundPath;
                intOutText.text = IntOpts.OutboundPath;
            }
            else
            {
                this
            dynamic _WithVar_4859;
                _WithVar_4859 = (Teleporters(teleporterFocus));
                NetworkPath.text = _WithVar_4859.path;
                TeleporterSizeSlider.value = Int(_WithVar_4859.Width / aspectRatio);
                FixedCheck.value = (!.driftHorizontal) * true;
                TeleportOption(2).value = _WithVar_4859.local;
                NetworkPath.IsEnabled = !.local;
                TeleportOption(1).value = _WithVar_4859.Out;
                TeleportOption(0).value = _WithVar_4859.In;
                TeleportOption(3).value = _WithVar_4859.Internet;
                TeleportVeggiesCheck.value = _WithVar_4859.teleportVeggies * true;
                TeleportCorpsesCheck.value = _WithVar_4859.teleportCorpses * true;
                TeleportHeterotrophsCheck.value = _WithVar_4859.teleportHeterotrophs * true;
                RespectShapesCheck.value = _WithVar_4859.RespectShapes * true;
                NumTeleported.Content = Str$(_WithVar_4859.NumTeleported);
                InboundCycleCheck.text = _WithVar_4859.InboundPollCycles;
                BotsPerPoll.text = _WithVar_4859.BotsPerPoll.Text;
                intOutText.text = _WithVar_4859.intOutPath;
                intInText.text = _WithVar_4859.intInPath;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            OKButton_Click();
        }

        private void OKButton_Click()
        {
            int i = 0;

            decimal randomX = 0;

            decimal randomy = 0;

            vector v = null;

            decimal aspectRatio = 0;

            decimal realWidth = 0;

            FileSystemObject fso = null;

            fso = new FileSystemObject(); ;

            //Check to make sure interent paths are good
            if (TeleportOption(3).value)
            {
                if (!(fso.FolderExists(intInText.text) && fso.FolderExists(intOutText.text)))
                {
                    MsgBox(("Internet paths must be set to a vaild directory."));
                    return;
                }
            }

            aspectRatio = SimOpts.FieldHeight / SimOpts.FieldWidth;
            realWidth = teleporterDefaultWidth * aspectRatio;

            if (teleporterFormMode == 0)
            {
                i = Teleport.NewTeleporter(TeleportOption(0).value, TeleportOption(1).value, CSng(teleporterDefaultWidth), TeleportOption(3).value);
            }
            else
            {
                i = teleporterFocus;
            }
            if (i < 0)
            {
                MsgBox(("Could not create Teleporter."));
            }
            else
            {
                Teleporters(i).path = NetworkPath.text;
                Teleporters(i).driftHorizontal = !CBool(FixedCheck.value);
                Teleporters(i).driftVertical = !CBool(FixedCheck.value);
                if (FixedCheck.value)
                {
                    Teleporters(i).vel = VectorSet(0, 0);
                }
                Teleporters(i).local = TeleportOption(2).value;
                Teleporters(i).In = TeleportOption(0).value;
                Teleporters(i).Out = TeleportOption(1).value;
                Teleporters(i).Internet = TeleportOption(3).value;
                Teleporters(i).teleportVeggies = CBool(TeleportVeggiesCheck.value);
                Teleporters(i).teleportCorpses = CBool(TeleportCorpsesCheck.value);
                Teleporters(i).teleportHeterotrophs = CBool(TeleportHeterotrophsCheck.value);
                Teleporters(i).RespectShapes = CBool(RespectShapesCheck.value);
                Teleporters(i).Height = CSng(TeleporterSizeSlider.value);
                Teleporters(i).Width = CSng(TeleporterSizeSlider.value) * aspectRatio;
                Teleporters(i).InboundPollCycles = CInt(val(InboundCycleCheck.text) % 32000);
                Teleporters(i).BotsPerPoll = CInt(val(BotsPerPoll.text) % 32000);
                Teleporters(i).PollCountDown = Teleporters(i).BotsPerPoll;
                Teleporters(i).intInPath = intInText.text;
                Teleporters(i).intOutPath = intOutText.text;
            }
            this
}

        private void TeleporterSizeSlider_Change(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TeleporterSizeSlider_Change();
        }

        private void TeleporterSizeSlider_Change()
        {
            teleporterDefaultWidth = TeleporterSizeSlider.value;
        }

        private void TeleportOption_Click(object sender, RoutedEventArgs e)
        {
            TeleportOption_Click();
        }

        private void TeleportOption_Click(int Index)
        {
            if (Index == 2 || Index == 3)
            {
                NetworkPath.IsEnabled = false;
            }
            else
            {
                NetworkPath.IsEnabled = true;
            }

            if (Index == 1 || Index == 2 || Index == 3)
            {
                TeleportHeterotrophsCheck.IsEnabled = true;
                TeleportVeggiesCheck.IsEnabled = true;
                TeleportCorpsesCheck.IsEnabled = true;
            }
            else
            {
                TeleportHeterotrophsCheck.IsEnabled = false;
                TeleportVeggiesCheck.IsEnabled = false;
                TeleportCorpsesCheck.IsEnabled = false;
            }

            if (Index == 0 || Index == 3)
            { // Outbound or Internet
                InboundLabel1.IsEnabled = true;
                InboundLabel2.IsEnabled = true;
                InboundLabel3.IsEnabled = true;
                InboundLabel4.IsEnabled = true;
                InboundCycleCheck.IsEnabled = true;
                BotsPerPoll.IsEnabled = true;
            }
            else
            { //Inbound
                InboundLabel1.IsEnabled = false;
                InboundLabel2.IsEnabled = false;
                InboundLabel3.IsEnabled = false;
                InboundLabel4.IsEnabled = false;
                InboundCycleCheck.IsEnabled = false;
                BotsPerPoll.IsEnabled = false;
            }

            if (Index == 3)
            { //Enable / Disable Internet controls
                TeleportOption(0).IsEnabled = false;
                TeleportOption(1).IsEnabled = false;
                TeleportOption(2).IsEnabled = false;
                intInText.IsEnabled = true;
                intOutText.IsEnabled = true;
                inboundLabel.IsEnabled = true;
                outboundLabel.IsEnabled = true;
                TeleportCorpsesCheck.value = 0;
                TeleportCorpsesCheck.IsEnabled = false;
            }
            else
            {
                TeleportOption(0).IsEnabled = true;
                TeleportOption(1).IsEnabled = true;
                TeleportOption(2).IsEnabled = true;
                TeleportOption(3).IsEnabled = false;
                intInText.IsEnabled = false;
                intOutText.IsEnabled = false;
                inboundLabel.IsEnabled = false;
                outboundLabel.IsEnabled = false;
            }
        }
    }
}
