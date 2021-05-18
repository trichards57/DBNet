using System.Windows;
using static Globals;
using static Microsoft.VisualBasic.FileSystem;
using static Microsoft.VisualBasic.Interaction;
using static Robots;
using static SimOptModule;
using static VBExtension;

namespace DBNet.Forms
{
    public partial class frmRestriOps : Window
    {
        // Option Explicit //Windows Default
        public byte res_state = 0;

        private static frmRestriOps _instance;

        public frmRestriOps()
        {
            InitializeComponent();
        }

        public static frmRestriOps instance { set { _instance = null; } get { return _instance ?? (_instance = new frmRestriOps()); } }

        public static void Load()
        {
            if (_instance == null) { dynamic A = frmRestriOps.instance; }
        }

        public static void Unload()
        {
            if (_instance != null) instance.Close(); _instance = null;
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            btnLoad_Click();
        }

        private void btnLoad_Click()
        {
            string val = "";

            CommonDialog1.FileName = "";
            CommonDialog1.InitDir = MDIForm1.instance.MainDir;
            CommonDialog1.Filter = "Restriction preset file(*.resp)|*.resp";
            CommonDialog1.ShowOpen();
            if (CommonDialog1.FileName != "")
            {
                VBOpenFile(80, CommonDialog1.FileName); ;
                Line(Input(80), val);
                chkMBKillNChlr.value = val;
                Line(Input(80), val);
                chkDQKillNChlr.value = val;
                Line(Input(80), val);
                chkNoChlr.value = val;
                Line(Input(80), val);
                BlockSpec.value = val;
                Line(Input(80), val);
                DisableVisionCheck.value = val;
                Line(Input(80), val);
                DisableDNACheck.value = val;
                Line(Input(80), val);
                DisableReproductionCheck.value = val;
                Line(Input(80), val);
                VirusImmuneCheck.value = val;
                Line(Input(80), val);
                MutEnabledCheck.value = val;
                Line(Input(80), val);
                DisableMovementSysvarsCheck.value = val;

                Line(Input(80), val);
                chkMBKillChlr.value = val;
                Line(Input(80), val);
                chkDQKillChlr.value = val;
                Line(Input(80), val);
                BlockSpecVeg.value = val;
                Line(Input(80), val);
                DisableVisionCheckVeg.value = val;
                Line(Input(80), val);
                DisableDNACheckVeg.value = val;
                Line(Input(80), val);
                DisableReproductionCheckChlr.value = val;
                Line(Input(80), val);
                VirusImmuneCheckVeg.value = val;
                Line(Input(80), val);
                MutEnabledCheckVeg.value = val;
                Line(Input(80), val);
                DisableMovementSysvarsCheckVeg.value = val;
                VBCloseFile(80); ();
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            btnOK_Click();
        }

        private void btnOK_Click()
        {
            int t = 0;

            //save based on state
            switch (res_state)
            {
                case 0:
                    TmpOpts.Specie(optionsform.CurrSpec).kill_mb = chkMBKillChlr.value * true;
                    TmpOpts.Specie(optionsform.CurrSpec).dq_kill = chkDQKillChlr.value * true;
                    break;

                case 1:
                    TmpOpts.Specie(optionsform.CurrSpec).kill_mb = chkMBKillNChlr.value * true;
                    TmpOpts.Specie(optionsform.CurrSpec).dq_kill = chkDQKillNChlr.value * true;
                    break;

                case 2:
                    x_res_kill_mb = chkMBKillNChlr.value * true;
                    x_res_other = chkNoChlr.value + BlockSpec.value * 2 + DisableVisionCheck.value * 4 + DisableDNACheck.value * 8 + DisableReproductionCheck.value * 16 + VirusImmuneCheck.value * 32 + DisableMovementSysvarsCheck.value * 64;
                    x_res_kill_mb_veg = chkMBKillChlr.value * true;
                    x_res_other_veg = BlockSpecVeg.value + DisableVisionCheckVeg.value * 2 + DisableDNACheckVeg.value * 4 + DisableReproductionCheckChlr.value * 8 + VirusImmuneCheckVeg.value * 16 + DisableMovementSysvarsCheckVeg.value * 32;
                    break;

                case 3:
                    //overwrite current simulation with given rules
                    for (t = 1; t < MaxRobs; t++)
                    {
                        if (rob(t).exist)
                        {
                            if (rob(t).Veg)
                            {
                                rob(t).multibot_time = IIf(chkMBKillChlr.value * true, 210, 0);
                                rob(t).dq = chkDQKillChlr.value;

                                rob(t).Fixed = BlockSpecVeg.value * true;
                                if (rob(t).Fixed)
                                {
                                    rob(t).mem(216) = 1;
                                    rob(t).vel.x = 0;
                                    rob(t).vel.y = 0;
                                }
                                rob(t).CantSee = DisableVisionCheckVeg.value * true;
                                rob(t).DisableDNA = DisableDNACheckVeg.value * true;
                                rob(t).CantReproduce = DisableReproductionCheckChlr.value * true;
                                rob(t).VirusImmune = VirusImmuneCheckVeg.value * true;
                                if (x_restartmode == 0)
                                {
                                    rob(t).Mutables.Mutations = MutEnabledCheckVeg.value == 0;
                                }
                                rob(t).DisableMovementSysvars = DisableMovementSysvarsCheckVeg.value * true;
                            }
                            else
                            {
                                rob(t).multibot_time = IIf(chkMBKillNChlr.value * true, 210, 0);
                                if (rob(t).dq < 2)
                                {
                                    rob(t).dq = chkDQKillNChlr.value;
                                }

                                rob(t).NoChlr = chkNoChlr.value * true;
                                rob(t).Fixed = BlockSpec.value * true;
                                if (rob(t).Fixed)
                                {
                                    rob(t).mem(216) = 1;
                                    rob(t).vel.x = 0;
                                    rob(t).vel.y = 0;
                                }
                                rob(t).CantSee = DisableVisionCheck.value * true;
                                rob(t).DisableDNA = DisableDNACheck.value * true;
                                rob(t).CantReproduce = DisableReproductionCheck.value * true;
                                rob(t).VirusImmune = VirusImmuneCheck.value * true;
                                if (x_restartmode == 0)
                                {
                                    rob(t).Mutables.Mutations = MutEnabledCheck.value == 0;
                                }
                                rob(t).DisableMovementSysvars = DisableMovementSysvarsCheck.value * true;
                            }
                        }
                    }
                    break;

                case 4:
                    y_res_kill_mb = chkMBKillNChlr.value * true;
                    y_res_other = chkNoChlr.value + BlockSpec.value * 2 + DisableVisionCheck.value * 4 + DisableDNACheck.value * 8 + DisableReproductionCheck.value * 16 + VirusImmuneCheck.value * 32 + DisableMovementSysvarsCheck.value * 64;
                    y_res_kill_mb_veg = chkMBKillChlr.value * true;
                    y_res_other_veg = BlockSpecVeg.value + DisableVisionCheckVeg.value * 2 + DisableDNACheckVeg.value * 4 + DisableReproductionCheckChlr.value * 8 + VirusImmuneCheckVeg.value * 16 + DisableMovementSysvarsCheckVeg.value * 32;
                    y_res_kill_dq = chkDQKillNChlr.value * true;
                    y_res_kill_dq_veg = chkDQKillChlr.value * true;
                    break;
            }
            Unload(this);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            btnSave_Click();
        }

        private void btnSave_Click()
        {
            CommonDialog1.FileName = "";
            CommonDialog1.InitDir = MDIForm1.instance.MainDir;
            CommonDialog1.Filter = "Restriction preset file(*.resp)|*.resp";
            CommonDialog1.ShowSave();
            if (CommonDialog1.FileName != "")
            {
                VBOpenFile(80, CommonDialog1.FileName); ;
                VBWriteFile(80, chkMBKillNChlr.value); ;
                VBWriteFile(80, chkDQKillNChlr.value); ;
                VBWriteFile(80, chkNoChlr.value); ;
                VBWriteFile(80, BlockSpec.value); ;
                VBWriteFile(80, DisableVisionCheck.value); ;
                VBWriteFile(80, DisableDNACheck.value); ;
                VBWriteFile(80, DisableReproductionCheck.value); ;
                VBWriteFile(80, VirusImmuneCheck.value); ;
                VBWriteFile(80, MutEnabledCheck.value); ;
                VBWriteFile(80, DisableMovementSysvarsCheck.value); ;

                VBWriteFile(80, chkMBKillChlr.value); ;
                VBWriteFile(80, chkDQKillChlr.value); ;
                VBWriteFile(80, BlockSpecVeg.value); ;
                VBWriteFile(80, DisableVisionCheckVeg.value); ;
                VBWriteFile(80, DisableDNACheckVeg.value); ;
                VBWriteFile(80, DisableReproductionCheckChlr.value); ;
                VBWriteFile(80, VirusImmuneCheckVeg.value); ;
                VBWriteFile(80, MutEnabledCheckVeg.value); ;
                VBWriteFile(80, DisableMovementSysvarsCheckVeg.value); ;
                VBCloseFile(80); ();
            }
        }

        private void Form_Activate()
        {
            byte lastmod = 0;

            byte holdother = 0;

            //configure form based on possible states
            //step1 reset everything
            ffmChlr.setVisible(true);
            ffmNChlr.setVisible(true);
            ffmP.setVisible(true);
            ffmPChlr.setVisible(true);
            ffmkillChlr.setVisible(true);
            ffmKillNChlr.setVisible(true);
            chkDQKillChlr.setVisible(true);
            chkDQKillNChlr.setVisible(true);
            btnLoad.setVisible(false);
            btnSave.setVisible(false);
            if (x_restartmode == 0)
            {
                MutEnabledCheck.setVisible(true);
                MutEnabledCheckVeg.setVisible(true);
            }
            else
            {
                MutEnabledCheck.setVisible(false);
                MutEnabledCheckVeg.setVisible(false);
            }
            //step2 reconfigure
            switch (res_state)
            {
                case 0:  //just kills for veg
                    ffmNChlr.setVisible(false);
                    ffmPChlr.setVisible(false);
                    Caption = "Restriction Options: " + TmpOpts.Specie(optionsform.CurrSpec).Name;

                    chkMBKillChlr.value = TmpOpts.Specie(optionsform.CurrSpec).kill_mb * true;
                    chkDQKillChlr.value = TmpOpts.Specie(optionsform.CurrSpec).dq_kill * true;

                    break;//just kills for Nveg
                case 1:
                    ffmChlr.setVisible(false);
                    ffmP.setVisible(false);
                    Caption = "Restriction Options: " + TmpOpts.Specie(optionsform.CurrSpec).Name;

                    chkMBKillNChlr.value = TmpOpts.Specie(optionsform.CurrSpec).kill_mb * true;
                    chkDQKillNChlr.value = TmpOpts.Specie(optionsform.CurrSpec).dq_kill * true;

                    break;//league
                case 2:
                    MutEnabledCheck.setVisible(false);
                    MutEnabledCheckVeg.setVisible(false);

                    chkDQKillChlr.setVisible(false);
                    chkDQKillNChlr.setVisible(false);
                    Caption = "League Restriction Options";

                    chkMBKillNChlr.value = x_res_kill_mb * true;

                    holdother = x_res_other;

                    lastmod = holdother % 2;
                    chkNoChlr.value = lastmod;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    BlockSpec.value = lastmod;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    DisableVisionCheck.value = lastmod;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    DisableDNACheck.value = lastmod;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    DisableReproductionCheck.value = lastmod;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    VirusImmuneCheck.value = lastmod;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    DisableMovementSysvarsCheck.value = lastmod;

                    chkMBKillChlr.value = x_res_kill_mb_veg * true;

                    holdother = x_res_other_veg;

                    lastmod = holdother % 2;
                    BlockSpecVeg.value = lastmod;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    DisableVisionCheckVeg.value = lastmod;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    DisableDNACheckVeg.value = lastmod;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    DisableReproductionCheckChlr.value = lastmod;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    VirusImmuneCheckVeg.value = lastmod;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    DisableMovementSysvarsCheckVeg.value = lastmod;
                    break;

                case 3:
                    Caption = "Restriction Options: Active simulation";
                    btnLoad.setVisible(true);
                    btnSave.setVisible(true);
                    break;

                case 4:
                    MutEnabledCheck.setVisible(false);
                    MutEnabledCheckVeg.setVisible(false);

                    Caption = "Evolution Restriction Options";

                    chkMBKillNChlr.value = y_res_kill_mb * true;

                    holdother = y_res_other;

                    lastmod = holdother % 2;
                    chkNoChlr.value = lastmod;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    BlockSpec.value = lastmod;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    DisableVisionCheck.value = lastmod;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    DisableDNACheck.value = lastmod;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    DisableReproductionCheck.value = lastmod;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    VirusImmuneCheck.value = lastmod;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    DisableMovementSysvarsCheck.value = lastmod;

                    chkMBKillChlr.value = y_res_kill_mb_veg * true;

                    holdother = y_res_other_veg;

                    lastmod = holdother % 2;
                    BlockSpecVeg.value = lastmod;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    DisableVisionCheckVeg.value = lastmod;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    DisableDNACheckVeg.value = lastmod;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    DisableReproductionCheckChlr.value = lastmod;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    VirusImmuneCheckVeg.value = lastmod;
                    holdother = (holdother - lastmod) / 2;
                    lastmod = holdother % 2;
                    DisableMovementSysvarsCheckVeg.value = lastmod;

                    chkDQKillNChlr.value = y_res_kill_dq * true;

                    chkDQKillChlr.value = y_res_kill_dq_veg * true;
                    break;
            }
        }
    }
}
