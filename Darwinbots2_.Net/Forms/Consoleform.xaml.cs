using Microsoft.VisualBasic;
using System.Windows;
using static DNAExecution;
using static IntOpts;
using static Master;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.Strings;
using static Robots;
using static System.Math;
using static VBExtension;

namespace DBNet.Forms
{
    public partial class Consoleform : Window
    {
        public int wcount = 0;
        private static Consoleform _instance;
        private readonly string[] hist = new string[101];
        private readonly string[] words = new string[101];
        private cevent _evnt;

        private int cnum = 0;

        private int hcurr = 0;

        private int hpos = 0;

        private float lasttim = 0;

        public Consoleform()
        {
            InitializeComponent();
        }

        public static Consoleform instance => _instance ?? (_instance = new Consoleform());

        public cevent evnt
        {
            get => _evnt;
            set
            {
                if (_evnt != null)
                    _evnt.eventtextentered -= evnt_textentered;

                _evnt = value;
                _evnt.eventtextentered += evnt_textentered;
            }
        }

        public static void Load()
        {
            if (_instance == null)
                _instance = new Consoleform();
        }

        public static void Unload()
        {
            if (_instance != null)
            {
                instance.Close();
                _instance = null;
            }
        }

        public void cycle(int num)
        {
            for (var k = 1; k < num; k++)
            {
                Form1.instance.cyc = Form1.instance.cyc + 1;

                UpdateSim();
                Form1.instance.Redraw();

                if (datirob.instance.IsVisible && !datirob.instance.ShowMemoryEarlyCycle)
                {
                    var rob = Robots.rob[robfocus];
                    datirob.instance.infoupdate(robfocus, rob.nrg, rob.parent, rob.Mutations, rob.age, rob.SonNumber, 1, rob.FName, rob.genenum, rob.LastMut, rob.generation, rob.DnaLen, rob.LastOwner, rob.Waste, rob.body, rob.mass, rob.venom, rob.shell, rob.Slime, rob.chloroplasts);
                }

                if (lasttim > Int(DateAndTime.Timer))
                    lasttim = (int)Int(DateAndTime.Timer);

                if (lasttim < Int(DateAndTime.Timer))
                {
                    Form1.instance.cyccaption(Form1.instance.cyc);
                    lasttim = (int)Int(DateAndTime.Timer);
                    Form1.instance.cyc = 0;
                }
                DoEvents();
            }
        }

        public void endconsole(int c)
        {
            rob[c].console = null;
        }

        public void newconsole(int ind, string title, string welc)
        {
            hpos = 0;
            hcurr = 0;
            cnum = ind;
            settitle(title);
            Text1.Text = welc;
            Show();
        }

        public void openconsole()
        {
            if (rob[robfocus].console == null)
            {
                rob[robfocus].console = new Consoleform();
                rob[robfocus].console.newconsole(robfocus, "Robot " + Str(rob[robfocus].AbsNum) + " console", "Robot " + Str(rob[robfocus].AbsNum) + " - " + rob[robfocus].FName + " console");
                rob[robfocus].console.textout("Type 'help' for commands");
                Active = false;
            }
        }

        public void settitle(string title)
        {
            Title = title;
        }

        public string text(int ind)
        {
            var text = "";
            if (ind >= 0 & ind <= wcount)
            {
                text = words[ind];
            }
            return text;
        }

        public void textout(string txt)
        {
            Text1.Text = Text1.Text + Chr(13) + Chr(10) + txt;
            if (Len(Text1.Text) > 2500)
            {
                Text1.Text = Mid(Text1.Text, InStr(Text1.Text, Chr(13).ToString()) + 2);
            }
            Text1.SelectionStart = Len(Text1.Text);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearButton_Click();
        }

        private void ClearButton_Click()
        {
            Text1.Text = "";
            Text1.SelectionStart = 0;
        }

        private void Command1_Click(object sender, RoutedEventArgs e)
        {
            Command1_Click();
        }

        private void Command1_Click()
        {
            robfocus = cnum;
            MDIForm1.instance.EnableRobotsMenu();
            textout("showdna");
            Text2.Text = "showdna";
            Parse();
            evnt.fire(cnum, "showdna");
            hist[hcurr] = "showdna";
            hcurr++;
            if (hcurr > 100)
            {
                hcurr = 0;
            }
            hpos = hcurr;
            Form1.instance.Redraw();
        }

        private void Command2_Click(object sender, RoutedEventArgs e)
        {
            words[1] = "printtouch";
            wcount = 1;
            evnt.fire(cnum, "printtouch");
        }

        private void Command3_Click(object sender, RoutedEventArgs e)
        {
            words[1] = "printtaste";
            wcount = 1;
            evnt.fire(cnum, "printtaste");
        }

        private void cyclebut_Click(object sender, RoutedEventArgs e)
        {
            words[1] = "cycle";
            words[2] = "1";
            wcount = 2;
            evnt.fire(cnum, "cycle");
        }

        private void debug_Click(object sender, RoutedEventArgs e)
        {
            debug_Click();
        }

        private void debug_Click()
        {
            words[1] = "debug";
            wcount = 1;
            evnt.fire(cnum, "debug");
        }

        private void evnt_textentered(int ind, string text)
        {
            if (rob[ind].console == null)
                return;

            text = rob[ind].console.text(1);
            switch (text)
            {
                case "debug":
                    rob[ind].console.textout(PrintDebug(ind));
                    break;

                case "printeye":
                    rob[ind].console.textout(PrintEye(ind));
                    break;

                case "printtouch":
                    rob[ind].console.textout(PrintTouch(ind));
                    break;

                case "printtaste":
                    rob[ind].console.textout(PrintTaste(ind));
                    break;

                case "cycle":
                    DisplayActivations = true;
                    cycle((int)Val(rob[ind].console.text(2)));
                    DisplayActivations = false;
                    break;

                case "energy":
                    rob[ind].nrg = Val(rob[ind].console.text(2));
                    break;

                case "play":
                    DisplayActivations = true;
                    Form1.instance.Active = true;
                    break;

                case "pause":
                    DisplayActivations = false;
                    Form1.instance.Active = false;
                    break;

                case "set":
                    if (Abs(Val(rob[ind].console.text(3))) < 32001)
                    {
                        rob[ind].mem[SysvarTok(rob[ind].console.text(2), ind)] = Val(rob[ind].console.text(3));
                        PrintMem(ind, rob[ind].console.text(2));
                    }
                    else
                        rob[ind].console.textout("Value of range.  Memory values must be between -32000 and 32000.");

                    break;

                case "printmem":
                    PrintMem(ind, rob[ind].console.text(2));
                    break;

                case "?":
                    PrintMem(ind, rob[ind].console.text(2));
                    break;

                case "execrob":
                    ExecRobs();
                    break;

                case "showdna":
                    datirob.instance.Show();
                    datirob.instance.RefreshDna();
                    datirob.instance.infoupdate(ind, rob[ind].nrg, rob[ind].parent, rob[ind].Mutations, rob[ind].age, rob[ind].SonNumber, 1, rob[ind].FName, rob[ind].genenum, rob[ind].LastMut, rob[ind].generation, rob[ind].DnaLen, rob[ind].LastOwner, rob[ind].Waste, rob[ind].body, rob[ind].mass, rob[ind].venom, rob[ind].shell, rob[ind].Slime, rob[ind].chloroplasts);
                    datirob.instance.ShowDna();
                    break;

                case "help":
                    rob[ind].console.textout("");
                    rob[ind].console.textout("This console works as an input/output interface for a single robot.");
                    rob[ind].console.textout("It could be used for robot debugging and manipulation.");
                    rob[ind].console.textout("One of the most useful features of the r.c. is that it shows");
                    rob[ind].console.textout("which parts of the dna are executed in each cycle. Just press the single");
                    rob[ind].console.textout("cycle button to try. To watch the entire dna, just click the button at");
                    rob[ind].console.textout("the extreme right in the console.");
                    rob[ind].console.textout("");
                    rob[ind].console.textout("Other commands are:");
                    rob[ind].console.textout("printeye : prints the eye cells status");
                    rob[ind].console.textout("printtouch : prints the touch cells status");
                    rob[ind].console.textout("printtaste : prints the taste (hit) cells status");
                    rob[ind].console.textout("printmem (or ?) (.var|n): prints value of .var or location n");
                    rob[ind].console.textout("set (.var|n) value : stores value in variable .var or location n");
                    rob[ind].console.textout("energy e : sets the robot's energy at e");
                    rob[ind].console.textout("cycle n : executes n cycles");
                    rob[ind].console.textout("execrob : executes all robots without doing a cycle");
                    rob[ind].console.textout("showdna : brings up the robot details window showing the robot's dna");
                    rob[ind].console.textout("debug : fires one cycle with debugger enabled");
                    break;
            }
        }

        private void eyebut_Click(object sender, RoutedEventArgs e)
        {
            words[1] = "printeye";
            wcount = 1;
            evnt.fire(cnum, "printeye");
        }

        private void Form_Resize()
        {
            //if (WindowState != WindowState.Minimized) {
            //  Text1.Width = Width - 120;
            //  Text1.Height = Height - Text2.Height - 620;
            //  Text2.Top = Height - Text2.Height - 400;
            //  Text2.Width = Text1.Width;
            //  Picture1.Width = Text2.Width;
            //}
        }

        private void Form_Unload()
        {
            endconsole(cnum);
        }

        private void Parse()
        {
            var c = 0;
            var a = Text2.Text;
            while (InStr(1, a, " ") > 0)
            {
                c++;
                words[c] = Left(a, InStr(1, a, " ") - 1);
                a = Right(a, Len(a) - 1 - Len(words[c]));
            }
            words[c + 1] = a;
            wcount = c + 1;
        }

        private void pausebut_Click(object sender, RoutedEventArgs e)
        {
            words[1] = "pause";
            wcount = 1;
            evnt.fire(cnum, "pause");
        }

        private void playbut_Click(object sender, RoutedEventArgs e)
        {
            words[1] = "play";
            wcount = 1;
            evnt.fire(cnum, "play");
        }

        private string PrintDebug(int ind)
        {
            return "***ROBOT DEBUG***" + rob[ind].dbgstring;
        }

        private string PrintEye(int ind)
        {
            var printeye = "EyeN: ";
            for (var t = 1; t < 9; t++)
            {
                printeye += Str(rob[ind].mem[EyeStart + t]);
            }
            printeye = printeye + " .eyef:" + Str(rob[ind].mem[EYEF]) + " .focuseye:" + Str(rob[ind].mem[FOCUSEYE]);
            printeye = printeye + Chr(13) + Chr(10) + "EyeNDir: ";
            for (var t = 0; t < 8; t++)
            {
                printeye += Str(rob[ind].mem[EYE1DIR + t]);
            }
            printeye = printeye + Chr(13) + Chr(10) + "EyeNWidth: ";
            for (var t = 0; t < 8; t++)
            {
                printeye += Str(rob[ind].mem[EYE1WIDTH + t]);
            }
            return printeye;
        }

        private void PrintMem(int ind, string w)
        {
            var v = (int)Val(w);
            if (v == 0)
            {
                v = SysvarTok(w, ind);
            }
            if (v > 0 & v < 1000)
            {
                rob[ind].console.textout(Str(v) + "->" + Str(rob[ind].mem[v]));
            }
        }

        private string PrintTaste(int ind)
        {
            var a = "Up:" + Str(rob[ind].mem[shup]);
            a = a + " Dn:" + Str(rob[ind].mem[shdn]);
            a = a + " Sx:" + Str(rob[ind].mem[shsx]);
            a = a + " Dx:" + Str(rob[ind].mem[shdx]);
            return a;
        }

        private string PrintTouch(int ind)
        {
            var a = "Up:" + Str(rob[ind].mem[hitup]);
            a = a + " Dn:" + Str(rob[ind].mem[hitdn]);
            a = a + " Sx:" + Str(rob[ind].mem[hitsx]);
            a = a + " Dx:" + Str(rob[ind].mem[hitdx]);
            a = a + " ID:" + Str(rob[ind].lasttch);
            return a;
        }

        private void Text2_KeyDown(int KeyCode)
        {
            if (KeyCode == 13 && Text2.Text != "")
            {
                textout(Text2.Text);
                Parse();
                evnt.fire(cnum, Text2.Text);
                hist[hcurr] = Text2.Text;
                hcurr++;
                if (hcurr > 100)
                {
                    hcurr = 0;
                }
                hpos = hcurr;
                Text2.Text = "";
            }
            if (KeyCode == 38)
            {
                hpos--;
                if (hpos == -1)
                {
                    hpos = 100;
                }
                Text2.Text = hist[hpos];
            }
            if (KeyCode == 40)
            {
                hpos++;
                if (hpos == 101)
                {
                    hpos = 0;
                }
                Text2.Text = hist[hpos];
            }
        }
    }
}
