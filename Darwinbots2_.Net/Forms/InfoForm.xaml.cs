using System.Windows;
using static Globals;
using static Microsoft.VisualBasic.Strings;
using static VBConstants;
using static VBExtension;

namespace DBNet.Forms
{
    public partial class InfoForm : Window
    {
        private static InfoForm _instance;

        public InfoForm()
        {
            InitializeComponent();
        }

        public static InfoForm instance { set { _instance = null; } get { return _instance ?? (_instance = new InfoForm()); } }

        public static void Load()
        {
            if (_instance == null) { dynamic A = InfoForm.instance; }
        }

        public static void Unload()
        {
            if (_instance != null) instance.Close(); _instance = null;
        }

        // Option Explicit //False
        //Botsareus 3/24/2012 simplified the info form

        private void Command1_Click(object sender, RoutedEventArgs e)
        {
            Command1_Click();
        }

        private void Command1_Click()
        {
            Unload(this);
        }

        private void Form_Load(object sender, RoutedEventArgs e)
        {
            Form_Load();
        }

        private void Form_Load()
        {
            strings(this);
            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE + SWP_NOSIZE);
            InfoForm.instance.Show();
            //Botsareus 6/11/2013 Play music
            mciSendString("Open " + Chr(34) + App.path + "\\DB THEME GOLD.mp3" + Chr(34) + " Alias Mellow", "", 0, 0);
            mciSendString("play Mellow from 1 to 60000", "", 0, 0);
        }

        private void Form_QueryUnload(int Cancel_UNUSED, int UnloadMode_UNUSED)
        {
            //Botsareus 6/11/2013 Stop playing music
            mciSendString("close Mellow", "", 0, 0);
        }

        private void Form_Unload(int Cancel_UNUSED)
        {
            Visible = false;
            frmFirstTimeInfo.Show(vbModal);
            DoEvents();
            OptionsForm.instance.SSTab1.Tab = 0;
            OptionsForm.instance.Show();
            DoEvents();
            OptionsForm.instance.LoadSettings_Click();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            int c = 0, u = 0; Form_QueryUnload(c, u); e.Cancel = c != 0;
        }
    }
}
