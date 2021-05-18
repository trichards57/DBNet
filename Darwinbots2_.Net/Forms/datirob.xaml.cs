using System.Windows;
using static Common;
using static Microsoft.VisualBasic.Constants;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.Interaction;
using static Microsoft.VisualBasic.Strings;
using static Robots;
using static System.Math;
using static VBExtension;

namespace DBNet.Forms
{
    public partial class datirob : Window
    {
        public bool enlarged = false;
        public bool Senses = false;
        public bool showingMemory = false;
        public bool ShowMemoryEarlyCycle = false;
        private static datirob _instance;

        public datirob()
        {
            InitializeComponent();
        }

        public static datirob instance { set { _instance = null; } get { return _instance ?? (_instance = new datirob()); } }

        public static void Load()
        {
            if (_instance == null)
                _instance = new datirob();
        }

        public static void Unload()
        {
            if (_instance != null)
            {
                instance.Close();
                _instance = null;
            }
        }

        public string GetRobMemoryString(int n)
        {
            var GetRobMemoryString = "";

            if (!rob[n].exist)
            {
                GetRobMemoryString = "This robot is dead";
                return GetRobMemoryString;
            }

            for (var j = 1; j < 100; j++)
            {
                GetRobMemoryString = GetRobMemoryString + Str((j - 1) * 10 + 1) + ":";
                for (var i = ((j - 1) * 10) + 1; i < (j * 10); i++)
                {
                    GetRobMemoryString = GetRobMemoryString + vbTab + Str(rob[n].mem[i]);
                }
                GetRobMemoryString += vbCrLf;
            }
            return GetRobMemoryString;
        }

        public void infoupdate(int n, double nrg, int par, int mut, int age, int son, double pmut_UNUSED, string FName, int gn, int mo, int gennum_UNUSED, int DnaLen, string lastown, double Waste, double body, double mass, double venom, double shell, double Slime, double ChlrVal)
        {
            robnum.Content = Str(n);
            UniqueBotID.Content = Str((rob[n].AbsNum));
            robnrg.Content = Str(Round(nrg, 2));
            robbody.Content = Str(Round(body, 2)); //EricL 4/14/2006 Removed Int()  Need to see the decimal value
            robmass.Content = Str(Round(mass, 2));
            robvenom.Content = Str(Round(venom, 2));
            robshell.Content = Str(Round(rob[n].shell, 2));
            robslime.Content = Str(Round(rob[n].Slime, 2));
            PoisonLabel.Content = Str(Round(rob[n].poison, 2));
            VTimerLabel.Content = Str(rob[n].Vtimer);
            robparent.Content = Str(par);
            robmutations.Content = Str(mo);
            robage.Content = Str(age); // EricL 4/13/2006 Now reads actual age
            robson.Content = Str(son);
            robfname.Content = FName;
            robgene.Content = Str(gn);
            robover.Content = Str(mut);
            robgener.Content = rob[n].generation;
            totlenlab.Content = Str(DnaLen);
            ChlrLabel.Content = Str(ChlrVal);
            wasteval.Content = Str(Round(Waste, 2));
            VelocityLabel.Content = Str(Round(VectorMagnitude(rob[n].vel), 2));
            RadiusLabel.Content = Str(Round(rob[n].radius, 2));
            if (lastown != "")
                LastOwnLab.Content = lastown;
            else
                LastOwnLab.Content = "Self";

            if (enlarged && showingMemory)
                dnatext.Text = GetRobMemoryString(n);
        }

        public void RefreshDna()
        {
            if (enlarged)
                dnatext.Text = DetokenizeDNA(robfocus);
        }

        public void ShowDna()
        {
            dnashow_Click(dnashow, new RoutedEventArgs()); //Botsareus 1/25/2013 Show dna using the button
        }

        private void btnMark_Click(object sender, RoutedEventArgs e)
        {
            Hide();

            var poz = Val(InputBox("\"[<POSITION MARKER]\" will be displayed next to dna location. Specify position:"));
            poz = Abs(poz);
            if (poz > 32000)
                poz = 32000;

            Show();
            dnatext.Text = DetokenizeDNA(robfocus, CInt(poz));
        }

        private void Command1_Click(object sender, RoutedEventArgs e)
        {
            Label2.Content = Str(Form1.instance.discendenti(robfocus, 0));
        }

        private void Command2_Click(object sender, RoutedEventArgs e)
        {
            ActivForm.instance.Show();
        }

        private void Command3_Click(object sender, RoutedEventArgs e)
        {
            Consoleform.instance.openconsole();
        }

        //Botsareus 2/25/2013 Makes the program easy to debug
        private void dnashow_Click(object sender, RoutedEventArgs e)
        {
            showingMemory = false;
            MemoryStateCheck.setVisible(false);
            Width = 12645;
            dnatext.Width = 9050;
            Frame2.Width = 4695 + 8055;
            enlarged = true;
            if (rob[robfocus].exist)
                dnatext.Text = DetokenizeDNA(robfocus);
            else
                dnatext.Text = "This Robot is dead.  No DNA available.";

            btnMark.setVisible(true); //Botsareus 3/15/2013 Makes dna easyer to debug
        }

        private void dnatext_Change(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            robtag.Content = Left(rob[robfocus].tag, 45); //Botsareus 1/28/2014 New short description feature
        }

        private void Form_Load(object sender, RoutedEventArgs e)
        {
            Form_Load();
        }

        private void Form_Load()
        {
            rage.Content = "Age (cycles)"; //EricL 4/13/2006 Override resource file because I don't have a resource editor handy :)
            Width = 3255;
            enlarged = false;
            ShowMemoryEarlyCycle = false;
            MemoryStateCheck.IsChecked = false;
        }

        private void Form_Unload(int Cancel)
        {
            if (Cancel == 0)
                Hide();
        }

        private string GiveMutationDetails(int robfocus)
        {
            var GiveMutationDetails = rob[robfocus].LastMutDetail;
            if (GiveMutationDetails == "")
            {
                GiveMutationDetails = "No mutations";
            }
            return GiveMutationDetails;
        }

        private void MemoryCommand_Click(object sender, RoutedEventArgs e)
        {
            showingMemory = true;
            MemoryStateCheck.setVisible(true);
            Width = 12645;
            dnatext.Width = 9050;
            Frame2.Width = 4695 + 8055;
            enlarged = true;
            if (rob[robfocus].exist)
            {
                dnatext.Text = GetRobMemoryString(robfocus);
            }
            else
            {
                dnatext.Text = "This Robot is dead.  No DNA available.";
            }
            btnMark.setVisible(false); //Botsareus 3/15/2013 Makes dna easyer to debug
        }

        private void MemoryStateCheck_Click(object sender, RoutedEventArgs e)
        {
            ShowMemoryEarlyCycle = MemoryStateCheck.IsChecked.Value;
        }

        private void MutDetails_Click(object sender, RoutedEventArgs e)
        {
            showingMemory = false;
            MemoryStateCheck.setVisible(false);
            Width = 12645;
            dnatext.Width = 9050;
            Frame2.Width = 4695 + 8055;
            enlarged = true;
            dnatext.Text = GiveMutationDetails(robfocus);
            btnMark.setVisible(false); //Botsareus 3/15/2013 Makes dna easyer to debug
        }

        private void repro_Click(object sender, RoutedEventArgs e)
        {
            if ((string)robnum.Content == "0")
            {
                return;
            }
            Reproduce(robnum.Content, 50);
            Form1.instance.Redraw();
        }

        private void robfname_DblClick(object sender, RoutedEventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetText(robfname.Content.ToString());
        }

        private void robtag_DblClick(object sender, RoutedEventArgs e)
        {
            robtag_DblClick();
        }

        private void robtag_DblClick()
        {
            rob[robfocus].tag = InputBox("Enter short description for robot. Can not be more then 45 characters long.", DefaultResponse: Left(rob[robfocus].tag, 45));
            rob[robfocus].tag = Left(replacechars(rob[robfocus].tag), 45);
            robtag.Content = rob[robfocus].tag;
        }

        private void ShrinkWin_Click(object sender, RoutedEventArgs e)
        {
            ShrinkWin_Click();
        }

        private void ShrinkWin_Click()
        {
            Width = 3255;
            MutDetails.Content = "Mutation details->";
            enlarged = false;
            showingMemory = false;
        }
    }
}
