using System.Windows;
using System.Windows.Controls;
using static Microsoft.VisualBasic.Conversion;
using static SimOptModule;

namespace DBNet.Forms
{
    public partial class EnergyForm : Window
    {
        private static EnergyForm _instance;

        public EnergyForm()
        {
            InitializeComponent();
        }

        public static EnergyForm instance => _instance ?? (_instance = new EnergyForm());

        public static void Load()
        {
            if (_instance == null)
                _instance = new EnergyForm();
        }

        public static void Unload()
        {
            if (_instance != null)
            {
                instance.Close();
                _instance = null;
            }
        }


        private void chkRnd_Click(object sender, RoutedEventArgs e)
        {
            TmpOpts.SunOnRnd = chkRnd.IsChecked.Value;
        }

        private void DNCheck_Click(object sender, RoutedEventArgs e)
        {
            TmpOpts.DayNight = DNCheck.IsChecked.Value;
            if (TmpOpts.DayNight == false)
                TmpOpts.Daytime = true;

            DNLength.IsEnabled = DNCheck.IsChecked.Value;
            DNCycleUpDn.IsEnabled = DNCheck.IsChecked.Value;
        }

        private void DNLength_Change(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            DNLength_Change();
        }

        private void DNLength_Change()
        {
            if (Val(DNLength.Text) > 32000)
                DNLength.Text = "32000";

            TmpOpts.CycleLength = (int)Val(DNLength.Text);
        }

        private void Form_Load(object sender, RoutedEventArgs e)
        {
            DNLength.Text = TmpOpts.CycleLength.ToString();
            DNLength.IsEnabled = TmpOpts.DayNight;
            DNCycleUpDn.IsEnabled = TmpOpts.DayNight;
            DNCheck.IsChecked = TmpOpts.DayNight;

            SunUpThreshold.Text = TmpOpts.SunUpThreshold.ToString();
            SunUpThreshold.IsEnabled = TmpOpts.SunUp;
            SunUpUpDn.IsEnabled = TmpOpts.SunUp;
            SunUp.IsChecked = TmpOpts.SunUp;

            SunDownThreshold.Text = TmpOpts.SunDownThreshold.ToString();
            SunDownThreshold.IsEnabled = TmpOpts.SunDown;
            SunDownUpDn.IsEnabled = TmpOpts.SunDown;
            SunDown.IsChecked = TmpOpts.SunDown;
            ThresholdMode_0.IsChecked = TmpOpts.SunThresholdMode == 0;
            ThresholdMode_1.IsChecked = TmpOpts.SunThresholdMode == 1;
            ThresholdMode_2.IsChecked = TmpOpts.SunThresholdMode == 2;
            chkRnd.IsChecked = TmpOpts.SunOnRnd;

            txtTide.Text = TmpOpts.Tides.ToString();
            txtTideOf.Text = TmpOpts.TidesOf.ToString();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        { 
            Unload();
        }

        private void SunDown_Click(object sender, RoutedEventArgs e)
        {
            TmpOpts.SunDown = SunDown.IsChecked.Value;
            SunDownThreshold.IsEnabled = SunDown.IsChecked.Value;
            SunDownUpDn.IsEnabled = SunDown.IsChecked.Value;
        }

        private void SunDownThreshold_Change(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TmpOpts.SunDownThreshold = (int)Val(SunDownThreshold.Text);
        }

        private void SunUp_Click(object sender, RoutedEventArgs e)
        {
            TmpOpts.SunUp = SunUp.IsChecked.Value;
            SunUpThreshold.IsEnabled = SunUp.IsChecked.Value;
            SunUpUpDn.IsEnabled = SunUp.IsChecked.Value;
        }

        private void SunUpThreshold_Change(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TmpOpts.SunUpThreshold = (int)Val(SunUpThreshold.Text);
        }

        private void ThresholdMode_Click(object sender, RoutedEventArgs e)
        {
            var send = sender as RadioButton;
            TmpOpts.SunThresholdMode =(int)send.Tag;
        }


        private void txtTide_Change(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
           
            txtTide.Text = Int(Val(txtTide.Text)).ToString();
            if (Val(txtTide.Text) < 0)
                txtTide.Text = "0";

            if (Val(txtTide.Text) > 32000)
                txtTide.Text = "32000";

            if (Val(txtTide.Text) == 0)
                lblTides.Content = "cycles (off)";
            else
                lblTides.Content = "cycles";

            TmpOpts.Tides = (int)Val(txtTide.Text);
        }

        private void txtTideOf_Change(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            txtTideOf.Text = Int(Val(txtTideOf.Text)).ToString();

            if (Val(txtTideOf.Text) < 0)
                txtTideOf.Text = "0";
            if (Val(txtTideOf.Text) > 32000)
                txtTideOf.Text = "32000";

            TmpOpts.TidesOf = (int)Val(txtTideOf.Text);
        }
    }
}
