using System.Collections.Generic;
using System.Windows;
using static DNATokenizing;
using static Globals;
using static Microsoft.VisualBasic.Constants;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.FileSystem;
using static Microsoft.VisualBasic.Information;
using static Microsoft.VisualBasic.Interaction;
using static Microsoft.VisualBasic.Strings;
using static VBExtension;

namespace DBNet.Forms
{
    public partial class frmPBMode : Window
    {
        private static frmPBMode _instance;

        // Option Explicit //False
        private int listpos = 0;

        public frmPBMode()
        {
            InitializeComponent();
        }

        public static frmPBMode instance { set { _instance = null; } get { return _instance ?? (_instance = new frmPBMode()); } }

        public static void Load()
        {
            if (_instance == null) { dynamic A = frmPBMode.instance; }
        }

        public static void Unload()
        {
            if (_instance != null) instance.Close(); _instance = null;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            btnAdd_Click();
        }

        private void btnAdd_Click()
        {
            ffmSett.setVisible(false);
            lblPress.setVisible(true);
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            btnDel_Click();
        }

        private void btnDel_Click()
        {
            if (listpos == 0)
            {
                return;
            }
            int i = 0;

            for (i = listpos; i < UBound(PB_keys) - 1; i++)
            {
                PB_keys(i) = PB_keys(i + 1);
            }
            List<> PB_keys_8643_tmp = new List<>();
            for (int redim_iter_6267 = 0; i < 0; redim_iter_6267++) { PB_keys.Add(redim_iter_6267 < PB_keys.Count ? PB_keys(redim_iter_6267) : null); }
            relist();
            listpos = 0;
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            btnLoad_Click();
        }

        private void btnLoad_Click()
        {
            string holdint = "";

            int i = 0;

            CommonDialog1.FileName = "";
            CommonDialog1.InitDir = MDIForm1.instance.MainDir;
            CommonDialog1.Filter = "PlayerBot keys preset file(*.pbkp)|*.pbkp";
            CommonDialog1.ShowOpen();
            if (CommonDialog1.FileName != "")
            {
                List<> PB_keys_9822_tmp = new List<>();
                for (int redim_iter_6084 = 0; i < 0; redim_iter_6084++) { PB_keys.Add(null); }
                VBOpenFile(80, CommonDialog1.FileName); ;
                do
                {
                    i = UBound(PB_keys);
                    List<> PB_keys_4643_tmp = new List<>();
                    for (int redim_iter_5685 = 0; i < 0; redim_iter_5685++) { PB_keys.Add(redim_iter_5685 < PB_keys.Count ? PB_keys(redim_iter_5685) : null); }
                    Line(Input(80), holdint);
                    PB_keys(i + 1).key == val(holdint);
                    Line(Input(80), holdint);
                    PB_keys(i + 1).memloc == val(holdint);
                    Line(Input(80), holdint);
                    PB_keys(i + 1).value == val(holdint);
                    Line(Input(80), holdint);
                    PB_keys(i + 1).Invert == holdint;
                } while (!(EOF(80));
                VBCloseFile(80); ();
                relist();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            btnSave_Click();
        }

        private void btnSave_Click()
        {
            int i = 0;

            CommonDialog1.FileName = "";
            CommonDialog1.InitDir = MDIForm1.instance.MainDir;
            CommonDialog1.Filter = "PlayerBot keys preset file(*.pbkp)|*.pbkp";
            CommonDialog1.ShowSave();
            if (CommonDialog1.FileName != "")
            {
                VBOpenFile(80, CommonDialog1.FileName); ;
                for (i = 1; i < UBound(PB_keys); i++)
                {
                    VBWriteFile(80, PB_keys(i).key); ;
                    VBWriteFile(80, PB_keys(i).memloc); ;
                    VBWriteFile(80, PB_keys(i).value); ;
                    VBWriteFile(80, PB_keys(i).Invert); ;
                }
                VBCloseFile(80); ();
            }
        }

        private void Form_KeyDown(int KeyCode, int Shift_UNUSED)
        {
            decimal memloc = 0;

            string strmem = "";

            string strval = "";

            decimal value = 0;

            if (lblPress.Visibility)
            {
                do
                {
                    strmem = InputBox("Note: You must start a simulation before assigning by name. Enter a memory location to assign the key to:");
                    if (strmem == "")
                    {
                        return;
                    }
                    if (IsNumeric(strmem))
                    {
                        memloc = val(strmem);
                    }
                    else
                    {
                        memloc = SysvarTok("." + strmem);
                    }
                    if (memloc == Int(memloc) && memloc > 0 & memloc < 1000)
                    {
                        break;
                    }
                    MsgBox("Invalid memory location: " + memloc, vbCritical);
                }

    do
                {
                    strval = InputBox("Enter the value to assign to memory location " + memloc + ":");
                    if (strval == "")
                    {
                        return;
                    }
                    value = val(strval);
                    if (value == Int(value) && value >= -32000 & value <= 32000)
                    {
                        break;
                    }
                    MsgBox("Invalid value: " + value, vbCritical);
                }

    int i = 0;

                i = UBound(PB_keys);

                List<> PB_keys_6647_tmp = new List<>();
                for (int redim_iter_8382 = 0; i < 0; redim_iter_8382++) { PB_keys.Add(redim_iter_8382 < PB_keys.Count ? PB_keys(redim_iter_8382) : null); }
                PB_keys(i + 1).key == CByte(KeyCode);
                PB_keys(i + 1).memloc == CInt(memloc);
                PB_keys(i + 1).value == CInt(value);
                PB_keys(i + 1).Invert == MsgBox("Would you like to add this key inverted?", vbQuestion | vbYesNo) == vbYes;
                relist();
                ffmSett.setVisible(true);
                lblPress.setVisible(false);
            }
        }

        private void Form_Load(object sender, RoutedEventArgs e)
        {
            Form_Load();
        }

        private void Form_Load()
        {
            relist();
        }

        private void lstkey_Click(object sender, RoutedEventArgs e)
        {
            lstkey_Click();
        }

        private void lstkey_Click()
        {
            listpos = lstkey.SelectedIndex + 1;
        }

        private string mapkey(byte inkey_UNUSED)
        {
            string mapkey = "";
            byte i = 0;

            VBOpenFile(80, App.path + "\\keys.txt"); ;
            for (i = 0; i < inkey; i++)
            {
                Line(Input(80), mapkey);
            }
            VBCloseFile(80); ();
            return mapkey;
        }

        private string mapmemory(int inmem)
        {
            string mapmemory = "";
            // TODO (not supported): On Error GoTo b
            mapmemory = SysvarDetok(inmem);
            if (!IsNumeric(mapmemory))
            {
                mapmemory = Right(mapmemory, Len(mapmemory) - 1);
            }
            return mapmemory;

        b:
            mapmemory = inmem;
            return mapmemory;
        }

        private void relist()
        {
            lstkey.CLEAR();
            int i = 0;

            for (i = 1; i < UBound(PB_keys); i++)
            {
                dynamic _WithVar_5199;
                _WithVar_5199 = PB_keys(i);
                lstkey.additem((IIf(_WithVar_5199.Invert, "Inverted ", "") + "Key: " + mapkey(_WithVar_5199.key) + "     Memory: " + mapmemory(_WithVar_5199.memloc) + "     Value: " + _WithVar_5199.value));
            }
        }
    }
}
