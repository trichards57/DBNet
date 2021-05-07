using System.Windows;
using static Microsoft.VisualBasic.Information;
using static VBExtension;

namespace DBNet.Forms
{
    public partial class ActivForm : Window
    {
        private static ActivForm _instance;
        private int[] gb = new int[1];

        public ActivForm()
        {
            InitializeComponent();
        }

        public static ActivForm instance => _instance ?? (_instance = new ActivForm());

        public static void Load()
        {
            if (_instance == null)
            {
                _instance = new ActivForm();
            }
        }

        public static void Unload()
        {
            if (_instance != null)
            {
                instance.Close();
                _instance = null;
            }
        }

        public void DrawGrid(bool[] ga)
        {
            DrawStat(ga);
            if (UBound(gb) != UBound(ga))
            {
                gb = new int[UBound(ga)];

                SetLab(ga);
            }
            DrawDyn(ga);
        }

        public void NoFocus()
        {
            //FillStyle = 0;
            //FillColor = vbBlack;
            //Line((cornice.Left, cornice.Top)-(cornice.Left + cornice.Width, cornice.Top + cornice.Height));
            //Line((cornice2.Left, cornice2.Top)-(cornice2.Left + cornice2.Width, cornice2.Top + cornice2.Height));
        }

        private void DrawDyn(bool[] ga)
        {
            //  float stp = 0;
            //  int t = 0;
            //  int gn = 0;

            //  float GrH = 0;
            //  float ReH = 0;

            //  gn = UBound(ga);
            //  if (gn == 0) {
            //return;

            //  }
            //  stp = cornice2.Width / gn;
            //  FillStyle = 0;
            //  for(t=1; t<gn; t++) {
            //    if (ga(t)) {
            //      gb[t] = gb[t] + (100 - gb[t]) / 5;
            //      if (gb[t] > 100) {
            //        gb[t] = 100;
            //      }
            //    } else {
            //      gb[t] = gb[t] - gb[t] / 5;
            //      if (gb[t] < 0) {
            //        gb[t] = 0;
            //      }
            //    }
            //    GrH = gb[t] / 100 * cornice2.Height;
            //    ReH = cornice2.Height - GrH;
            //    FillColor = vbBlue;
            //    Line((cornice2.Left + stp * (t - 1), cornice2.Top)-(cornice2.Left + stp * (t), cornice2.Top + ReH));
            //    FillColor = vbCyan;
            //    Line((cornice2.Left + stp * (t - 1), cornice2.Top + ReH)-(cornice2.Left + stp * (t), cornice2.Top + ReH + GrH));
            //  }
        }

        private void DrawStat(bool[] ga)
        {
            //  float stp = 0;
            //  int t = 0;
            //  int gn = 0;

            //  gn = UBound(ga);
            //  if (gn == 0) {
            //return;// EricL to prevent overflow

            //  }
            //  stp = cornice.Width / gn;
            //  FillStyle = 0;
            //  for(t=1; t<gn; t++) {
            //    if (ga(t)) {
            //      FillColor = vbGreen;
            //    } else {
            //      FillColor = vbRed;
            //    }
            //    Line((cornice.Left + stp * (t - 1), cornice.Top)-(cornice.Left + stp * (t), cornice.Top + cornice.Height));
            //  }
        }

        private void Form_Load(object sender, RoutedEventArgs e)
        {
            gb = new int[1];
        }

        private void SetLab(bool[] ga)
        {
            //int t = 0;
            //int d = 0;
            //int da = 0;
            //float stp = 0;

            //int cy = 0;

            //da = UBound(ga);
            //if (da == 0) {
            //  da = 1;
            //}
            //stp = pbox.Width / da;
            //pbox.Cls();
            //if (da < 25) {
            //  for(t=0; t<da - 1; t++) {
            //    pbox.CurrentX = pbox.Left + (pbox.Width / da) * t + (pbox.Width / da) / 2 - Len(CStr(t + 1)) * 40;
            //    pbox.PrintNNL(CStr(t + 1));
            //  }
            //} else {
            //  cy = pbox.CurrentY;
            //  for(t=5; t<da - 1 Step 5; t++) {
            //    pbox.CurrentY = cy;
            //    pbox.CurrentX = (pbox.ScaleWidth / da) * t + (pbox.ScaleWidth / da) / 2 - Len(CStr(t + 1)) * 40;
            //    pbox.PrintNNL(CStr(t));
            //    pbox.Line(stp * (t), 0)-Step(0, pbox.ScaleHeight / 2), vbBlack);
            //  }
            //}
        }
    }
}
