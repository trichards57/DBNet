using System.Windows;

namespace DBNet.Forms
{
    public partial class frmFirstTimeInfo : Window
    {
        private static frmFirstTimeInfo _instance;

        public frmFirstTimeInfo()
        {
            InitializeComponent();
        }

        public static frmFirstTimeInfo instance { set { _instance = null; } get { return _instance ?? (_instance = new frmFirstTimeInfo()); } }

        public static void Load()
        {
            if (_instance == null) { dynamic A = frmFirstTimeInfo.instance; }
        }

        public static void Unload()
        {
            if (_instance != null) instance.Close(); _instance = null;
        }
    }
}
