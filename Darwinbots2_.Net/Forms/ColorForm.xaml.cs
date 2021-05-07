using System;
using System.Windows;
using static Microsoft.VisualBasic.Constants;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.FileSystem;
using static Microsoft.VisualBasic.Information;
using static Microsoft.VisualBasic.Interaction;
using static Microsoft.VisualBasic.Strings;
using static VBExtension;

namespace DBNet.Forms
{
    public partial class ColorForm : Window
    {
        public int color = 0;
        public int OldColor = 0;
        public string path = "";
        public bool SelectColor = false;
        public bool UseThisColor = false;
        private static ColorForm _instance;
        private int bval = 0;
        private int gval = 0;
        private int rval = 0;

        public ColorForm()
        {
            InitializeComponent();
        }

        public static ColorForm instance => _instance ?? (_instance = new ColorForm());

        public static void Load()
        {
            if (_instance == null)
                _instance = new ColorForm();
        }

        public static void Unload()
        {
            if (_instance != null)
            {
                instance.Close();
                _instance = null;
            }
        }

        private void btnWrite_Click(object sender, RoutedEventArgs e)
        {
            //Step 1, where is the dna file
            if (Dir(path) == "")
            {
                var splt = Split(path, "\\");
                var namepart = splt[UBound(splt)];
                path = MDIForm1.instance.MainDir + "\\Robots\\" + namepart;
                if (Dir(path) == "")
                {
                    MsgBox("Robot not found!", vbCritical);
                    return;
                }
            }

            //Step 2, load Dna (ignore lines that def red, green , or blue) (initial lines that have ' will be moved)

            var robot = "";//Whole robot
            var cmtrob = "";
            var endofcmt = false;

            VBOpenFile(1, path);

            while (!EOF(1))
            {
                var dtl = Trim(LineInput(1));

                if (dtl == "" || dtl == "'*" && !endofcmt)
                {
                    //initial comments move to top
                    cmtrob = cmtrob + dtl + vbCrLf;
                    continue;
                }
                else
                    endofcmt = true;

                if (dtl == "def red*")
                    continue;
                if (dtl == "def green*")
                    continue;
                if (dtl == "def blue*")
                    continue;
                if (dtl == "@")
                    continue;

                robot = robot + dtl + vbCrLf;
            }
            VBCloseFile(1);

            robot = Left(robot, Len(robot) - 2);
            if (cmtrob != "")
            {
                cmtrob = Left(cmtrob, Len(cmtrob) - 2); //trim back comments only if there where comments
            }

            //Step 3 add back new values for red, green, and blue, and comments
            robot = "def blue " + bval + vbNewLine + robot;
            robot = "def green " + gval + vbNewLine + robot;
            robot = "def red " + rval + vbNewLine + robot;
            robot = "@" + vbNewLine + robot; //Botsareus 11/29/2013 bug fix
            robot = cmtrob + vbNewLine + robot;

            //Step 4 write back to dna file
            VBOpenFile(1, path);
            VBWriteFile(1, robot);
            VBCloseFile(1);

            //Step5 use the color
            UseThisColor = true;
            SelectColor = true;
            Hide();
        }

        private void DispColor()
        {
            //color = bval * 65536 + gval * 256 + rval;
            //Shape1.BackColor = color;
        }

        private void Form_Load(object sender, RoutedEventArgs e)
        {
            OldColor = color;
            SetColor(color);
        }

        private void SetColor(int col)
        {
            bval = Int(col / 65536);
            col -= bval * 65536;
            gval = Int(col / 256);
            rval = col - gval * 256;
            SliderB.Value = bval;
            SliderR.Value = rval;
            SliderG.Value = gval;
            LabelG.Content = Str(gval);
            LabelB.Content = Str(bval);
            LabelR.Content = Str(rval);
            DispColor();
        }

        private void SliderB_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            bval = (int)SliderB.Value;
            LabelB.Content = Str(bval);
            DispColor();
        }

        private void SliderG_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            gval = (int)SliderG.Value;
            LabelG.Content = Str(gval);
            DispColor();
        }

        private void SliderR_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            rval = (int)SliderR.Value;
            LabelR.Content = Str(rval);
            DispColor();
        }

        private void UseColor_Click(object sender, RoutedEventArgs e)
        {
            UseThisColor = true;
            SelectColor = true;
            Hide();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            UseThisColor = true;
        }
    }
}
