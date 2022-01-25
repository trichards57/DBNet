using System.Windows;
using static DNATokenizing;
using static Microsoft.VisualBasic.Information;
using static VBExtension;

namespace DBNet.Forms
{
    public partial class frmMonitorSet : Window
    {
        public int Monitor_ceil_b = 0;
        public int Monitor_ceil_g = 0;
        public int Monitor_ceil_r = 0;
        public int Monitor_floor_b = 0;
        public int Monitor_floor_g = 0;
        public int Monitor_floor_r = 0;
        public int Monitor_mem_b = 0;
        public int Monitor_mem_g = 0;

        // Option Explicit //Windows Default
        public int Monitor_mem_r = 0;

        public bool overwrite = false;
        private static frmMonitorSet _instance;
        private bool okclick = false;

        public frmMonitorSet()
        {
            InitializeComponent();
        }

        public static frmMonitorSet instance { set { _instance = null; } get { return _instance ?? (_instance = new frmMonitorSet()); } }

        public static void Load()
        {
            if (_instance == null) { dynamic A = frmMonitorSet.instance; }
        }

        public static void Unload()
        {
            if (_instance != null) instance.Close(); _instance = null;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            btnCancel_Click();
        }

        private void btnCancel_Click()
        {
            Unload(this);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            btnOK_Click();
        }

        private void btnOK_Click()
        {
            Monitor_mem_r = txtMem(0).Text;
            Monitor_mem_g = txtMem(1).Text;
            Monitor_mem_b = txtMem(2).Text;

            Monitor_floor_r = txtFloor(0).Text;
            Monitor_floor_g = txtFloor(1).Text;
            Monitor_floor_b = txtFloor(2).Text;

            Monitor_ceil_r = txtCeil(0).Text;
            Monitor_ceil_g = txtCeil(1).Text;
            Monitor_ceil_b = txtCeil(2).Text;

            okclick = true;
            Unload(this);
        }

        private void Command1_Click(object sender, RoutedEventArgs e)
        {
            Command1_Click();
        }//load a preset

        private void Command1_Click()
        {
            int holdint = 0;

            CommonDialog1.FileName = "";
            CommonDialog1.InitDir = MDIForm1.instance.MainDir;
            CommonDialog1.Filter = "Monitor preset file(*.mtrp)|*.mtrp";
            CommonDialog1.ShowOpen();
            if (CommonDialog1.FileName != "")
            {
                VBOpenFile(80, CommonDialog1.FileName); ;
                Get(80);
                txtMem(0).text = holdint;
                Get(80);
                txtMem(1).text = holdint;
                Get(80);
                txtMem(2).text = holdint;

                Get(80);
                txtFloor(0).text = holdint;
                Get(80);
                txtFloor(1).text = holdint;
                Get(80);
                txtFloor(2).text = holdint;

                Get(80);
                txtCeil(0).text = holdint;
                Get(80);
                txtCeil(1).text = holdint;
                Get(80);
                txtCeil(2).text = holdint;
                VBCloseFile(80); ();
            }
        }

        private void Command2_Click(object sender, RoutedEventArgs e)
        {
            Command2_Click();
        }//save a preset

        private void Command2_Click()
        {
            CommonDialog1.FileName = "";
            CommonDialog1.InitDir = MDIForm1.instance.MainDir;
            CommonDialog1.Filter = "Monitor preset file(*.mtrp)|*.mtrp";
            CommonDialog1.ShowSave();
            if (CommonDialog1.FileName != "")
            {
                VBOpenFile(80, CommonDialog1.FileName); ;
                Put(80);
                Put(80);
                Put(80);

                Put(80);
                Put(80);
                Put(80);

                Put(80);
                Put(80);
                Put(80);
                VBCloseFile(80); ();
            }
        }

        private void Form_Load(object sender, RoutedEventArgs e)
        {
            Form_Load();
        }

        private void Form_Load()
        {
            if (overwrite)
            {
                txtMem(0).Text = Monitor_mem_r;
                txtMem(1).Text = Monitor_mem_g;
                txtMem(2).Text = Monitor_mem_b;

                txtFloor(0).Text = Monitor_floor_r;
                txtFloor(1).Text = Monitor_floor_g;
                txtFloor(2).Text = Monitor_floor_b;

                txtCeil(0).Text = Monitor_ceil_r;
                txtCeil(1).Text = Monitor_ceil_g;
                txtCeil(2).Text = Monitor_ceil_b;
            }
            else
            {
                btnOK.Left = btnCancel.Left;
                btnCancel.setVisible(false);
            }
            overwrite = true;
        }

        private void Form_Unload(int Cancel_UNUSED)
        {
            if (!okclick)
            {
                overwrite = btnCancel.Visibility;
            }
        }

        private void txtCeil_LostFocus(object sender, RoutedEventArgs e)
        {
            txtCeil_LostFocus();
        }

        private void txtCeil_LostFocus(int Index)
        {
            decimal v = 0;

            v = val(txtCeil(Index));
            if (v < -32000)
            {
                v = -32000;
            }
            if (v > 32000)
            {
                v = 32000;
            }
            if (v < val(txtFloor(Index)) + 1)
            {
                v = val(txtFloor(Index)) + 1;
            }
            v = CInt(v);
            txtCeil(Index) = v;
        }

        private void txtFloor_LostFocus(object sender, RoutedEventArgs e)
        {
            txtFloor_LostFocus();
        }

        private void txtFloor_LostFocus(int Index)
        {
            decimal v = 0;

            v = val(txtFloor(Index));
            if (v < -32000)
            {
                v = -32000;
            }
            if (v > 32000)
            {
                v = 32000;
            }
            if (v > val(txtCeil(Index)) - 1)
            {
                v = val(txtCeil(Index)) - 1;
            }
            v = CInt(v);
            txtFloor(Index) = v;
        }

        private void txtMem_LostFocus(object sender, RoutedEventArgs e)
        {
            txtMem_LostFocus();
        }

        private void txtMem_LostFocus(int Index)
        {
            if (!IsNumeric(txtMem(Index)))
            {
                txtMem(Index) = SysvarTok("." + txtMem(Index));
            }

            int v = 0;

            v = val(txtMem(Index));
            if (v < 1)
            {
                v = 1;
            }
            if (v > 999)
            {
                v = 999;
            }
            txtMem(Index) = v;
        }
    }
}
