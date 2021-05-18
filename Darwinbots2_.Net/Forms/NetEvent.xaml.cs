using System.Windows;

namespace DBNet.Forms
{
    public partial class NetEvent : Window
    {
        private static NetEvent _instance;
        private decimal Mx = 0;

        //False
        private decimal My = 0;

        public NetEvent()
        {
            InitializeComponent();
        }

        public static NetEvent instance { set { _instance = null; } get { return _instance ?? (_instance = new NetEvent()); } }

        public static void Load()
        {
            if (_instance == null) { dynamic A = NetEvent.instance; }
        }

        public static void Unload()
        {
            if (_instance != null) instance.Close(); _instance = null;
        }

        public void stayontop()
        {
            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE + SWP_NOSIZE);
        }

        private void Appear(string txt)
        {
            this
          this
          NetLab.Content = txt;
            Netlab2.Content = txt;
            if (OptionsForm.instance.Visible == false)
            {
                Timer1.Interval = 5000;
                Timer1.IsEnabled = true;
                this
            }
        }

        private void Form_Load(object sender, RoutedEventArgs e)
        {
            Form_Load();
        }

        private void Form_Load()
        {
            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE + SWP_NOSIZE);
        }

        private void Form_MouseDown(int button_UNUSED, int Shift_UNUSED, decimal x, decimal y)
        {
            Mx = x;
            My = y;
        }

        private void Form_MouseMove(int button_UNUSED, int Shift_UNUSED, decimal x, decimal y)
        {
            if (Mx > 0 || My > 0)
            {
                dx = x - Mx;
                dy = y - My;
                this
            }
        }

        private void Form_MouseUp(int button_UNUSED, int Shift_UNUSED, decimal x_UNUSED, decimal y_UNUSED)
        {
            Mx = 0;
            My = 0;
        }

        private void Label1_Click(object sender, RoutedEventArgs e)
        {
            Label1_Click();
        }

        private void Label1_Click()
        {
            Timer1.IsEnabled = false;
            this
        }

        private void NetLab_Change(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            NetLab_Change();
        }

        private void NetLab_Change()
        {
            this
        }

        private void NetLab_MouseDown(int button_UNUSED, int Shift_UNUSED, decimal x, decimal y)
        {
            Mx = x;
            My = y;
        }

        private void NetLab_MouseMove(int button_UNUSED, int Shift_UNUSED, decimal x, decimal y)
        {
            if (Mx > 0 || My > 0)
            {
                dx = x - Mx;
                dy = y - My;
                this
              if (this < 0)
                {
                    this
    }
            }
        }

        private void NetLab_MouseUp(int button_UNUSED, int Shift_UNUSED, decimal x_UNUSED, decimal y_UNUSED)
        {
            Mx = 0;
            My = 0;
        }

        private void NetLab2_MouseDown(int button_UNUSED, int Shift_UNUSED, decimal x, decimal y)
        {
            Mx = x;
            My = y;
        }

        private void NetLab2_MouseMove(int button_UNUSED, int Shift_UNUSED, decimal x, decimal y)
        {
            if (Mx > 0 || My > 0)
            {
                dx = x - Mx;
                dy = y - My;
                this
              if (this < 0)
                {
                    this
    }
            }
        }

        private void NetLab2_MouseUp(int button_UNUSED, int Shift_UNUSED, decimal x_UNUSED, decimal y_UNUSED)
        {
            Mx = 0;
            My = 0;
        }

        private void Timer1_Timer()
        {
            Timer1.IsEnabled = false;
            this
        }
    }
}
