using Iersera.ViewModels;
using System.Windows;

namespace DBNet.Forms
{
    public partial class RestrictionOptionsForm : Window
    {
        public byte res_state = 0;

        public RestrictionOptionsForm()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        internal RestrictionOptionsViewModel ViewModel { get; } = new RestrictionOptionsViewModel();

        private void ApplyClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnOK_Click()
        {
            //int t = 0;

            ////save based on state
            //switch (res_state)
            //{
            //    case 0:
            //        TmpOpts.Specie(optionsform.CurrSpec).kill_mb = chkMBKillChlr.value * true;
            //        TmpOpts.Specie(optionsform.CurrSpec).dq_kill = chkDQKillChlr.value * true;
            //        break;

            //    case 1:
            //        TmpOpts.Specie(optionsform.CurrSpec).kill_mb = chkMBKillNChlr.value * true;
            //        TmpOpts.Specie(optionsform.CurrSpec).dq_kill = chkDQKillNChlr.value * true;
            //        break;

            //    case 3:
            //        //overwrite current simulation with given rules
            //        for (t = 1; t < MaxRobs; t++)
            //        {
            //            if (rob(t).exist)
            //            {
            //                if (rob(t).Veg)
            //                {
            //                    rob(t).multibot_time = IIf(chkMBKillChlr.value * true, 210, 0);
            //                    rob(t).dq = chkDQKillChlr.value;

            //                    rob(t).Fixed = BlockSpecVeg.value * true;
            //                    if (rob(t).Fixed)
            //                    {
            //                        rob(t).mem(216) = 1;
            //                        rob(t).vel.x = 0;
            //                        rob(t).vel.y = 0;
            //                    }
            //                    rob(t).CantSee = DisableVisionCheckVeg.value * true;
            //                    rob(t).DisableDNA = DisableDNACheckVeg.value * true;
            //                    rob(t).CantReproduce = DisableReproductionCheckChlr.value * true;
            //                    rob(t).VirusImmune = VirusImmuneCheckVeg.value * true;
            //                    if (x_restartmode == 0)
            //                    {
            //                        rob(t).Mutables.Mutations = MutEnabledCheckVeg.value == 0;
            //                    }
            //                    rob(t).DisableMovementSysvars = DisableMovementSysvarsCheckVeg.value * true;
            //                }
            //                else
            //                {
            //                    rob(t).multibot_time = IIf(chkMBKillNChlr.value * true, 210, 0);
            //                    if (rob(t).dq < 2)
            //                    {
            //                        rob(t).dq = chkDQKillNChlr.value;
            //                    }

            //                    rob(t).NoChlr = chkNoChlr.value * true;
            //                    rob(t).Fixed = BlockSpec.value * true;
            //                    if (rob(t).Fixed)
            //                    {
            //                        rob(t).mem(216) = 1;
            //                        rob(t).vel.x = 0;
            //                        rob(t).vel.y = 0;
            //                    }
            //                    rob(t).CantSee = DisableVisionCheck.value * true;
            //                    rob(t).DisableDNA = DisableDNACheck.value * true;
            //                    rob(t).CantReproduce = DisableReproductionCheck.value * true;
            //                    rob(t).VirusImmune = VirusImmuneCheck.value * true;
            //                    if (x_restartmode == 0)
            //                    {
            //                        rob(t).Mutables.Mutations = MutEnabledCheck.value == 0;
            //                    }
            //                    rob(t).DisableMovementSysvars = DisableMovementSysvarsCheck.value * true;
            //                }
            //            }
            //        }
            //        break;
            //}
            //Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Form_Activate()
        {
            //switch (res_state)
            //{
            //    case 0:  //just kills for veg
            //        ffmNChlr.setVisible(false);
            //        ffmPChlr.setVisible(false);
            //        Caption = "Restriction Options: " + TmpOpts.Specie(optionsform.CurrSpec).Name;
            //        chkMBKillChlr.value = TmpOpts.Specie(optionsform.CurrSpec).kill_mb * true;
            //        chkDQKillChlr.value = TmpOpts.Specie(optionsform.CurrSpec).dq_kill * true;
            //        break;//just kills for Nveg
            //    case 1:
            //        ffmChlr.setVisible(false);
            //        ffmP.setVisible(false);
            //        Caption = "Restriction Options: " + TmpOpts.Specie(optionsform.CurrSpec).Name;
            //        chkMBKillNChlr.value = TmpOpts.Specie(optionsform.CurrSpec).kill_mb * true;
            //        chkDQKillNChlr.value = TmpOpts.Specie(optionsform.CurrSpec).dq_kill * true;
            //        break;
            //    case 3:
            //        Caption = "Restriction Options: Active simulation";
            //        btnLoad.setVisible(true);
            //        btnSave.setVisible(true);
            //        break;

            //}
        }
    }
}
