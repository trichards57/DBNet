using System.Windows;
using System.Windows.Controls;
using static Microsoft.VisualBasic.Conversion;
using static SimOpt;

namespace DBNet.Forms
{
    public partial class CostsForm : Window
    {
        private static CostsForm _instance;

        public CostsForm()
        {
            InitializeComponent();
        }

        public static CostsForm instance => _instance ?? (_instance = new CostsForm());

        public static void Load()
        {
            if (_instance == null)
                _instance = new CostsForm();
        }

        public static void Unload()
        {
            if (_instance != null)
            {
                instance.Close();
                _instance = null;
            }
        }

        // Option Explicit //False
        //Botsareus 6/12/2012 form's icon change

        private void AgeCostLog_Click(object sender, RoutedEventArgs e)
        {
            AgeCostLog_Click();
        }

        private void AgeCostLog_Click()
        {
            if (AgeCostLog.IsChecked.Value)
            {
                LinearAgeCostCheck.IsEnabled = false;
                Costs_33.IsEnabled = false;
                Label17.IsEnabled = false;
            }
            else
            {
                LinearAgeCostCheck.IsEnabled = true;
                Costs_33.IsEnabled = true;
                Label17.IsEnabled = true;
            }
            TmpOpts.Costs[AGECOSTMAKELOG] = AgeCostLog.IsChecked.Value ? 1 : 0;
        }

        private void AllowNegativeCostXCheck_Click(object sender, RoutedEventArgs e)
        {
            TmpOpts.Costs[ALLOWNEGATIVECOSTX] = AllowNegativeCostXCheck.IsChecked.Value ? 1 : 0;
        }

        private void BotNoCostThreshold_Change(object sender, TextChangedEventArgs e)
        {
            TmpOpts.Costs[BOTNOCOSTLEVEL] = Val(BotNoCostThreshold.Text);
        }

        private void CostReinstate_Change(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TmpOpts.Costs[COSTXREINSTATEMENTLEVEL] = Val(CostReinstate.Text);
        }

        private void Costs_Change(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var send = sender as TextBox;
            var index = (int)send.Tag;
            TmpOpts.Costs[index] = Val(send.Text);
        }

        private void CostX_Change(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CostX_Change();
        }

        private void CostX_Change()
        {
            TmpOpts.Costs[COSTMULTIPLIER] = Val(CostX.Text);
            SimOpts.Costs[COSTMULTIPLIER] = Val(CostX.Text); // Have to do this here since DispSettings gets called again when the Options dialog repaints...
            TmpOpts.OldCostX = Val(CostX.Text);
        }

        private void Default_Click(object sender, RoutedEventArgs e)
        {
            Default_Click();
        }

        private void Default_Click()
        {
            Costs_0.Text = "0";
            Costs_1.Text = "0";
            Costs_2.Text = "0";
            Costs_3.Text = "0";
            Costs_4.Text = "0";
            Costs_5.Text = ".004";
            Costs_6.Text = "0";
            Costs_7.Text = ".04";
            Costs_8.Text = "0";
            Costs_9.Text = "0";

            Costs_20.Text = ".05";
            Costs_21.Text = "0";
            Costs_22.Text = "2";
            Costs_23.Text = "2";
            Costs_24.Text = "0";
            Costs_25.Text = "0";
            Costs_26.Text = "0.01";
            Costs_27.Text = "0.01";
            Costs_28.Text = "0.1";
            Costs_29.Text = "0.1";

            Costs_8.Text = "0.2";

            Costs_32.Text = "0";
            AgeCostLog.IsChecked = false;
            BotNoCostThreshold.Text = "0";
            CostReinstate.Text = "0";
            DynamicCostTargetPopulation.IsEnabled = false;
            DynamicCosts.IsEnabled = false;
            TmpOpts.DynamicCosts = false;
            CostX.Text = "1";
            Costs_30.Text = "0.00001";
            Costs_31.Text = "0.01";
        }

        private void DynamicCosts_Click(object sender, RoutedEventArgs e)
        {
            TmpOpts.Costs[USEDYNAMICCOSTS] = DynamicCosts.IsChecked.Value ? 1 : 0;
            DynamicCostTargetPopulation.IsEnabled = DynamicCosts.IsChecked.Value;
            DynamicCostsUpDown.IsEnabled = DynamicCosts.IsChecked.Value;
            DynamicCostSensitivitySlider.IsEnabled = DynamicCosts.IsChecked.Value;
            DynamicCostsRangeU.IsEnabled = DynamicCosts.IsChecked.Value;
            DynamicCostsRangeL.IsEnabled = DynamicCosts.IsChecked.Value;
        }

        private void DynamicCostSensitivitySlider_Change(object sender, TextChangedEventArgs e)
        {
            TmpOpts.Costs[DYNAMICCOSTSENSITIVITY] = DynamicCostSensitivitySlider.Value;
        }

        private void DynamicCostsIncludePlantsCheck_Click(object sender, RoutedEventArgs e)
        {
            TmpOpts.Costs[DYNAMICCOSTINCLUDEPLANTS] = DynamicCostsIncludePlantsCheck.IsChecked.Value ? 1 : 0;
        }

        private void DynamicCostsIncludePlantsCheck_Click()
        {
        }

        private void DynamicCostsRangeL_Change(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TmpOpts.Costs[DYNAMICCOSTTARGETLOWERRANGE] = Val(DynamicCostsRangeL.Text);
        }

        private void DynamicCostsRangeU_Change(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TmpOpts.Costs[DYNAMICCOSTTARGETUPPERRANGE] = Val(DynamicCostsRangeU.Text);
        }

        private void DynamicCostTargetPopulation_Change(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TmpOpts.Costs[DYNAMICCOSTTARGET] = Val(DynamicCostTargetPopulation.Text);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Form_Load(object sender, RoutedEventArgs e)
        {
            Costs_0.Text = TmpOpts.Costs[0].ToString();
            Costs_1.Text = TmpOpts.Costs[1].ToString();
            Costs_2.Text = TmpOpts.Costs[2].ToString();
            Costs_3.Text = TmpOpts.Costs[3].ToString();
            Costs_4.Text = TmpOpts.Costs[4].ToString();
            Costs_5.Text = TmpOpts.Costs[5].ToString();
            Costs_6.Text = TmpOpts.Costs[6].ToString();
            Costs_7.Text = TmpOpts.Costs[7].ToString();
            Costs_8.Text = TmpOpts.Costs[8].ToString();
            Costs_9.Text = TmpOpts.Costs[9].ToString();
            Costs_20.Text = TmpOpts.Costs[20].ToString();
            Costs_21.Text = TmpOpts.Costs[21].ToString();
            Costs_22.Text = TmpOpts.Costs[22].ToString();
            Costs_23.Text = TmpOpts.Costs[23].ToString();
            Costs_24.Text = TmpOpts.Costs[24].ToString();
            Costs_25.Text = TmpOpts.Costs[25].ToString();
            Costs_26.Text = TmpOpts.Costs[26].ToString();
            Costs_27.Text = TmpOpts.Costs[27].ToString();
            Costs_28.Text = TmpOpts.Costs[28].ToString();
            Costs_29.Text = TmpOpts.Costs[29].ToString();
            Costs_30.Text = TmpOpts.Costs[30].ToString();
            Costs_31.Text = TmpOpts.Costs[31].ToString();
            Costs_32.Text = TmpOpts.Costs[32].ToString();
            Costs_33.Text = TmpOpts.Costs[33].ToString();

            //EricL 4/12/2006 Set the value of the checkboxes.  Do this way to guard against weird values
            if (TmpOpts.Costs[AGECOSTMAKELOG] == 1)
            {
                AgeCostLog.IsChecked = true;
                LinearAgeCostCheck.IsEnabled = false;
                Costs_33.IsEnabled = false;
                Label17.IsEnabled = false;
            }
            else
            {
                AgeCostLog.IsChecked = false;
                LinearAgeCostCheck.IsEnabled = true;
                Costs_33.IsEnabled = true;
                Label17.IsEnabled = true;
            }
            if (TmpOpts.Costs[AGECOSTMAKELINEAR] == 1)
            {
                LinearAgeCostCheck.IsChecked = true;
            }
            else
            {
                LinearAgeCostCheck.IsChecked = false;
            }
            if ((TmpOpts.Costs[AGECOSTMAKELOG] == 1) && (TmpOpts.Costs[AGECOSTMAKELINEAR] == 1))
            {
                // This should never happen...  Set em both to unchecked since something is amiss...
                AgeCostLog.IsChecked = false;
                LinearAgeCostCheck.IsEnabled = true;
                Costs_33.IsEnabled = true;
                Label17.IsEnabled = true;
                LinearAgeCostCheck.IsChecked = false;
            }
            if (TmpOpts.Costs[ALLOWNEGATIVECOSTX] == 1)
            {
                AllowNegativeCostXCheck.IsChecked = true;
            }
            else
            {
                AllowNegativeCostXCheck.IsChecked = false;
            }

            //Need to load this as it changes and will get put back.
            TmpOpts.Costs[COSTMULTIPLIER] = SimOpts.Costs[COSTMULTIPLIER];

            CostX.Text = TmpOpts.Costs[COSTMULTIPLIER].ToString();

            BotNoCostThreshold.Text = TmpOpts.Costs[BOTNOCOSTLEVEL].ToString();
            CostReinstate.Text = TmpOpts.Costs[COSTXREINSTATEMENTLEVEL].ToString();
            DynamicCosts.IsChecked = TmpOpts.Costs[USEDYNAMICCOSTS] == 1;
            DynamicCostTargetPopulation.Text = TmpOpts.Costs[DYNAMICCOSTTARGET].ToString();
            DynamicCostTargetPopulation.IsEnabled = DynamicCosts.IsChecked.Value;
            DynamicCostsUpDown.IsEnabled = DynamicCosts.IsChecked.Value;
            DynamicCostSensitivitySlider.Value = TmpOpts.Costs[DYNAMICCOSTSENSITIVITY];
            DynamicCostSensitivitySlider.IsEnabled = DynamicCosts.IsChecked.Value;
            DynamicCostsRangeU.Text = TmpOpts.Costs[DYNAMICCOSTTARGETUPPERRANGE].ToString();
            DynamicCostsRangeU.IsEnabled = DynamicCosts.IsChecked.Value;
            DynamicCostsRangeL.Text = TmpOpts.Costs[DYNAMICCOSTTARGETLOWERRANGE].ToString();
            DynamicCostsRangeL.IsEnabled = DynamicCosts.IsChecked.Value;
            DynamicCostsIncludePlantsCheck.IsChecked = TmpOpts.Costs[DYNAMICCOSTINCLUDEPLANTS] != 0;
        }

        private void LinearAgeCostCheck_Click(object sender, RoutedEventArgs e)
        {
            AgeCostLog.IsEnabled = !LinearAgeCostCheck.IsChecked.Value;

            TmpOpts.Costs[AGECOSTMAKELINEAR] = LinearAgeCostCheck.IsChecked.Value ? 1 : 0;
        }
    }
}
