using System.Windows;
using static Robots;
using static SimOptModule;

namespace DBNet.Forms
{
    public partial class frmEYE : Window
    {
        private static frmEYE _instance;

        public frmEYE()
        {
            InitializeComponent();
        }

        public static frmEYE instance { set { _instance = null; } get { return _instance ?? (_instance = new frmEYE()); } }

        public static void Load()
        {
            if (_instance == null) { dynamic A = frmEYE.instance; }
        }

        public static void Unload()
        {
            if (_instance != null) instance.Close(); _instance = null;
        }

        // Option Explicit //Windows Default

        private void btnBoff_Click(object sender, RoutedEventArgs e)
        {
            SimOpts.PhysBrown = 0;
        }

        private void btnCosts_Click(object sender, RoutedEventArgs e)
        {
            SimOpts.Costs[COSTMULTIPLIER] = 0;
        }

        private void btnOut_Click(object sender, RoutedEventArgs e)
        {
            //  byte i = 0;

            //  // TODO (not supported):   On Error GoTo fine
            //  CommonDialog1.DialogTitle = MBSaveDNA;
            //  CommonDialog1.FileName = "";
            //  CommonDialog1.Filter = "DNA file(*.txt)|*.txt";
            //  CommonDialog1.InitDir = MDIForm1.instance.MainDir + "\\robots";
            //  CommonDialog1.ShowSave();
            //  if (CommonDialog1.FileName != "") {
            //    VBOpenFile(437, CommonDialog1.FileName);;
            //    VBWriteFile(437, "Cond");;
            //    VBWriteFile(437, "*.robage 0 =");;
            //    VBWriteFile(437, "Start");;
            //    for(i=0; i<8; i++) {
            //      VBWriteFile(437, txtDir(i).text + " .eye" + (i + 1) + "dir store");;
            //      VBWriteFile(437, txtWth(i).text + " .eye" + (i + 1) + "width store");;
            //      VBWriteFile(437, "'");;
            //    }
            //    VBWriteFile(437, "Stop");;
            //    VBCloseFile();
            //  }
            //return;

            //fine:
            //  MsgBox(MBDNANotSaved);
        }

        private void btnRaim_Click(object sender, RoutedEventArgs e)
        {
            // TODO (not supported): On Error Resume Next
            rob(robfocus).mem(SetAim) = 0;
        }

        private void txtDir_Change(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // TODO (not supported): On Error Resume Next
            rob(robfocus).mem(Index + EYE1DIR) == txtDir(Index).text;
        }

        private void txtWth_Change(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // TODO (not supported): On Error Resume Next
            rob(robfocus).mem(Index + EYE1WIDTH) == txtWth(Index).text;
        }
    }
}
