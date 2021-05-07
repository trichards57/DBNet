using System.Windows;
using System.Windows.Controls;
using static F1Mode;
using static Microsoft.VisualBasic.Strings;
using static Robots;

namespace DBNet.Forms
{
    public partial class Contest_Form : Window
    {
        private static Contest_Form _instance;

        public Contest_Form()
        {
            InitializeComponent();
        }

        public static Contest_Form instance => _instance ?? (_instance = new Contest_Form());

        public static void Load()
        {
            if (_instance == null) _instance = new Contest_Form();
        }

        public static void Unload()
        {
            if (_instance != null) { instance.Close(); _instance = null; }
        }

        private void Option_Click(object sender, RoutedEventArgs e)
        {
            var send = sender as RadioButton;

            var index = (int)send.Tag;

            if (index == 0)
            {
                return;
            }

            for (var t = 1; t < MaxRobs; t++)
            {
                if (!rob[t].Veg && !rob[t].Corpse && rob[t].exist)
                {
                    var realname = Left(rob[t].FName, Len(rob[t].FName) - 4);
                    if (realname != PopArray[index].SpName)
                    {
                        KillRobot(t);
                    }
                }
            }
            send.IsChecked = false;
        }
    }
}
