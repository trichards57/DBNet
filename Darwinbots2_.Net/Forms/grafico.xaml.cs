using System.Collections.Generic;
using System.Windows;
using static Common;
using static Globals;
using static IntOpts;
using static Microsoft.VisualBasic.Constants;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.Information;
using static Microsoft.VisualBasic.Interaction;
using static Microsoft.VisualBasic.Strings;
using static SimOptModule;
using static stuffcolors;
using static VBConstants;
using static VBExtension;

namespace DBNet.Forms
{
    public partial class grafico : Window
    {
        // Option Explicit //False
        public const int MaxData = 1000;

        public const int MaxItems = 65;
        private static grafico _instance;
        private List<dynamic> data = new List<dynamic>(new dynamic[1001]);
        private List<dynamic> MaxItems = new List<dynamic>(new dynamic[1001]);
        private byte MaxSeries = 0;
        private decimal maxy = 0;
        private int Pivot = 0;
        private bool secretunloadoverwrite = false;
        private List<int> SerCol = new List<int>(new int[(MaxItems + 1)]);

        // TODO: Confirm Array Size By Token
        private List<string> SerName = new List<string>(new string[(MaxItems + 1)]);

        // TODO: Confirm Array Size By Token
        private List<decimal> Sum = new List<decimal>(new decimal[(MaxItems + 1)]);

        public grafico()
        {
            InitializeComponent();
        }

        public static grafico instance { set { _instance = null; } get { return _instance ?? (_instance = new grafico()); } }

        public static void Load()
        {
            if (_instance == null) { dynamic A = grafico.instance; }
        }

        public static void Unload()
        {
            if (_instance != null) instance.Close(); _instance = null;
        }

        // TODO: Confirm Array Size By Token
        //Botsareus 6/29/2013
        // EricL 4/7/2006

        /*
        'Public Sub SetYLabel(a As String)
        '  Dim b As String
        '  b = ""
        '  For t = 1 To Len(a)
        '    b = b + Mid(a, t, 1) + vbCrLf
        '  Next t
        '  'YLabel.Caption = b
        '  Me.Caption = a + " / Cycles graph"
        'End Sub
        */

        public dynamic AddSeries(string n, int c)
        {
            dynamic AddSeries = null;
            if (MaxSeries >= MaxItems)
            {
                MaxSeries = MaxItems - 1; // maxed the number of series in the graph, so replace the last one with the new
            }
            SerName(MaxSeries) = n;
            if (Right(n, 4) == ".txt")
            {
                n = Left(n, Len(n) - 4);
            }
            if (Len(n) > 28)
            {
                Label1(MaxSeries).Caption = Left(n, 25) + "...";
            }
            else
            {
                Label1(MaxSeries).Caption = n;
            }
            Shape3(MaxSeries).FillColor = c;
            Label1(MaxSeries).Visible = true;
            Shape3(MaxSeries).Visible = true;
            popnum(MaxSeries).Visible = true;
            SerCol(MaxSeries) = Shape3(MaxSeries).FillColor;
            MaxSeries = MaxSeries + 1;
            AddSeries = MaxSeries - 1;
            return AddSeries;
        }

        public void DelSeries(int n)
        {
            int k = 0;

            int t = 0;

            if (n < MaxSeries - 1)
            {
                for (k = n; k < MaxSeries - 1; k++)
                {
                    for (t = 0; t < MaxData; t++)
                    {
                        data(t, k) == data(t, k + 1);
                    }
                    Label1(k).Caption = Label1(k + 1).Caption;
                    Shape3(k).FillColor = Shape3(k + 1).FillColor;
                    popnum(k).Caption = popnum(k + 1).Caption;
                    SerName(k) = SerName(k + 1);
                    SerCol(k) = SerCol(k + 1);
                }
            }
            Label1(MaxSeries - 1).Visible == false;
            Shape3(MaxSeries - 1).Visible == false;
            popnum(MaxSeries - 1).Visible == false;
            SerName(MaxSeries - 1) == "";
            MaxSeries = MaxSeries - 1;
        }

        public int GetPosition(string n)
        {
            int GetPosition = 0;
            int k = 0;

            k = 0;
            While(k < MaxSeries && SerName(k) != n);
            k = k + 1;
            Wend();
            if (SerName(k) != n)
            {
                AddSeries(n, RGB(Random(100, 255), Random(100, 255), Random(100, 255)));
                k = MaxSeries - 1;
            }
            GetPosition = k;
            return GetPosition;
        }

        public void IncSeries(string n)
        {
            byte k = 0;

            k = 0;
            While(k < MaxSeries && SerName(k) != n);
            k = k + 1;
            Wend();
            if (SerName(k) != n)
            {
                AddSeries(n, RGB(Random(100, 255), Random(100, 255), Random(100, 255)));
                k = MaxSeries - 1;
            }
            data(Pivot, k) == data(Pivot, k) + 1;
        }

        public void KillZeroValuedSeries(int k)
        {
            int i = 0;

            decimal Sum = 0;

            Sum = 0;

            for (i = 0; i < MaxData; i++)
            {
                Sum = data(i, k) + Sum;
            }

            if (Sum == 0)
            {
                DelSeries((CInt(k)));
            }
        }

        public void NewPoints()
        {
            byte t = 0;

            Pivot = Pivot + 1;
            if (Pivot > MaxData)
            {
                Pivot = 0;
            }
            for (t = 0; t < MaxItems - 1; t++)
            {
                data(Pivot, t) == 0;
            }

            RedrawGraph();
        }

        public void RedrawGraph()
        {
            BackColor = chartcolor; //Botsareus 4/37/2013 Set Chart Skin
            chk_GDsave.BackColor = chartcolor;
            //Botsareus 5/31/2013 Special graph info
            if (Left != Screen.Width)
            {
                graphleft(WhichGraphAmI) = Left; //A little mod here not to update graph position if invisible mode
            }
            if (Top != Screen.Height)
            {
                graphtop(WhichGraphAmI) = Top;
            }

            dynamic k = null;
            int p = 0;

            dynamic t = null;
            int x = 0;

            decimal maxv = 0;

            decimal xunit = 0;
            decimal yunit = 0;

            maxv = -1000;
            int an = 0;

            decimal inc = 0;
            decimal xo = 0;
            decimal yo = 0;

            List<dynamic> lp = new List<dynamic>(new dynamic[(MaxItems + 1)]);  // TODO: Confirm Array Size By Token
            List<decimal> As = new List<decimal>(new decimal[(MaxItems + 1)]);  // TODO: Confirm Array Size By Token

            // TODO (not supported):   On Error GoTo bypass //EricL in case chart window gets closed just at the right moment...
            if (maxy < 1)
            {
                maxy = 1;
            }

            xunit = (Riquadro.Width - 200) / (MaxData + 1);
            yunit = (Riquadro.Height - 200) / maxy; // EricL - Multithread divide by zero bug here...
            xo = Riquadro.Left;
            yo = Riquadro.Top + Riquadro.Height - 50;
            this
          DrawAxes(maxy);
            k = Pivot + 1;
            if (k > MaxData)
            {
                k = 0;
            }

            ReorderSeries();

            for (t = 0; t < MaxSeries - 1; t++)
            {
                this
              lp[t, 0] == xo;
                lp[t, 1] == yo - yunit * data(k, t);
                Sum(t) = 0;
            }
            an = 2;

            While(k != Pivot);
            for (t = 0; t < MaxSeries - 1; t++)
            {
                this
              lp[t, 0] == xo + xunit * an;
                lp[t, 1] == yo - yunit * data(k, t);
                if (data(k, t) > maxv)
                {
                    maxv = data(k, t);
                }
                Sum(t) = data(k, t) + Sum(t);
                Label1(t).ToolTipText = Str(data(k, t)); // EricL 4/6/2006 - Updates the tooltip to display that last value
                popnum(t).Caption = Str(data(k, t)); // EricL 8/2007 - Display the actual population
            }
            an = an + 1;
            k = k + 1;
            if (k > MaxData)
            {
                k = 0;
            }
            Wend();

            if (t > 10)
            {
                for (x = (MaxSeries - 1); x < 0 Step - 1; x++) {
                    if (Sum(x) == 0)
                    {
                        DelSeries((x));
                    }
                }
            }

            p = Pivot - 1;
            if (p < 0)
            {
                p = MaxData;
            }

            if (t > 50 || SimOpts.EnableAutoSpeciation)
            { //Botsareus attempt to fix forking issue, may lead to graph instability
                for (x = (MaxSeries - 1); x < 0 Step - 1; x++) {
                    if (data(p, x) == 0)
                    {
                        DelSeries((x));
                    }
                }
            }

            maxy = maxv;

            //Botsareus 6/1/2013 Graph saves
            byte whatgraph = 0;

            string strCGraph = "";

            if (k == 999)
            {
                if (chk_GDsave.value == 1)
                {
                    //figure what graph am I
                    whatgraph = WhichGraphAmI;
                    //figure string od custom graph
                    strCGraph = "normal";
                    if (whatgraph == CUSTOM_1_GRAPH)
                    {
                        strCGraph = strGraphQuery1;
                    }
                    if (whatgraph == CUSTOM_2_GRAPH)
                    {
                        strCGraph = strGraphQuery2;
                    }
                    if (whatgraph == CUSTOM_3_GRAPH)
                    {
                        strCGraph = strGraphQuery3;
                    }
                    //update counter
                    graphfilecounter(whatgraph) = graphfilecounter(whatgraph) + 1;
                    //write folder if non exisits
                    RecursiveMkDir((MDIForm1.instance.MainDir + "\\" + strSimStart));
                    //write data
                    VBOpenFile(100, MDIForm1.MainDir + "\\" + strSimStart + "\\" + Caption + graphfilecounter(whatgraph) + ".gsave"); ;
                    //write custom graph data
                    VBWriteFile(100, strCGraph); ;
                    //write headers
                    strCGraph = "";
                    for (x = 0; x < MaxSeries - 1; x++)
                    {
                        strCGraph = strCGraph + Shape3(x).FillColor + ":" + Label1(x).Caption + ",";
                    }
                    VBWriteFile(100, strCGraph); ;
                    dynamic k2 = null;
                    int t2 = 0;

                    for (k2 = 0; k2 < 998; k2++)
                    {
                        strCGraph = "";
                        for (t2 = 0; t2 < MaxSeries - 1; t2++)
                        {
                            strCGraph = strCGraph + data(k2, t2) + vbTab;
                        }
                        VBWriteFile(100, strCGraph); ;
                    }
                    VBCloseFile(100); ();
                }
            }

            XLabel.Content = Str(SimOpts.ChartingInterval) + " cycles per data point. " + Str(k) + " data points.";

        bypass:
          }

        public void ReorderSeries()
        {
            int i = 0;

            int t = 0;

            for (i = 0; i < MaxSeries - 1; i++)
            {
                for (t = i; t < MaxSeries - 1; t++)
                {
                    if (data(Pivot - 1, i) < data(Pivot - 1, t))
                    {
                        SwapSeries(i, t);
                    }
                }
            }
        }

        public void ResetGraph()
        {
            int t = 0;

            Erase(data);
            Erase(SerCol);
            Erase(SerName);
            Pivot = 0;
            MaxSeries = 0;
            for (t = 0; t < MaxItems; t++)
            {
                Label1(t).Visible = false;
                Shape3(t).Visible = false;
                popnum(t).Visible = false;
            }
        }

        public void setcolor(string n, int c)
        {
            int k = 0;

            k = 0;
            While(k < MaxSeries && SerName(k) != n);
            k = k + 1;
            Wend();
            if (SerName(k) == n)
            {
                Shape3(k).FillColor = c;
                SerCol(k) = c;
            }
        }

        public void SetValues(string n, decimal v)
        {
            byte k = 0;

            int i = 0;

            int GraphNumber = 0;

            if (v == 0)
            {
                return;
            }

            i = 0;
            k = 0;

            // see if the series exists in the graph
            While(k < MaxSeries && SerName(k) != n && k < MaxItems);
            k = k + 1;
            Wend();

            // SerName(k) will <> n if this is a new series

            if (SerName(k) != n || k == MaxItems)
            {
                GraphNumber = WhichGraphAmI;

                if (GraphNumber == 10)
                { // Autocosts graph
                    if (n == "Cost Multiplier")
                    {
                        AddSeries(n, RGB(Random(100, 255), Random(100, 255), Random(100, 255)));
                    }
                    else if (n == "Population / Target")
                    {
                        AddSeries(n, RGB(Random(100, 255), Random(100, 255), Random(100, 255)));
                    }
                    else if (n == "Total Bots")
                    {
                        AddSeries(n, RGB(Random(100, 255), Random(100, 255), Random(100, 255)));
                    }
                    else if (n == "Upper Range")
                    {
                        AddSeries(n, RGB(Random(100, 255), Random(100, 255), Random(100, 255)));
                    }
                    else if (n == "Lower Range")
                    {
                        AddSeries(n, RGB(Random(100, 255), Random(100, 255), Random(100, 255)));
                    }
                    else if (n == "Zero Level")
                    {
                        AddSeries(n, RGB(Random(100, 255), Random(100, 255), Random(100, 255)));
                    }
                    else if (n == "Reinstatement Level")
                    {
                        AddSeries(n, RGB(Random(100, 255), Random(100, 255), Random(100, 255)));
                    }
                }
                else
                { // all other graphs uses species as series labels
                  //Check if the name matches a species.  Might be a new species from the internet
                    While(SimOpts.Specie(i).Name != n && i < SimOpts.SpeciesNum && i <= MAXNATIVESPECIES);
                    i = i + 1;
                    Wend();
                    if (i == SimOpts.SpeciesNum)
                    { // Internet Species not in this sim yet
                        i = 0;
                        While(InternetSpecies(i).Name != n && i < numInternetSpecies);
                        i = i + 1;
                        if (i > MAXINTERNETSPECIES)
                        {
                            return;
                        }
                        Wend();
                        AddSeries(n, InternetSpecies(i).color);
                    }
                    else
                    { // species already in the species list
                        AddSeries(n, SimOpts.Specie(i).color);
                    }
                }

                k = MaxSeries - 1;
            }

            data(Pivot, k) == v;
        }

        public void SetValuesP(int k, decimal v)
        {
            data(Pivot, k) == v;
        }

        public dynamic SwapSeries(int i, int t)
        {
            dynamic SwapSeries = null;
            int x = 0;

            for (x = 0; x < MaxData; x++)
            {
                data(x, MaxItems) == data(x, i);
                data(x, i) == data(x, t);
                data(x, t) == data(x, MaxItems);
            }
            Label1(MaxItems).Caption = Label1(i).Caption;
            Shape3(MaxItems).FillColor = Shape3(i).FillColor;
            popnum(MaxItems).Caption = popnum(i).Caption;
            Label1(i).Caption = Label1(t).Caption;
            Shape3(i).FillColor = Shape3(t).FillColor;
            popnum(i).Caption = popnum(t).Caption;
            Label1(t).Caption = Label1(MaxItems).Caption;
            Shape3(t).FillColor = Shape3(MaxItems).FillColor;
            popnum(t).Caption = popnum(MaxItems).Caption;
            SerName(MaxItems) = SerName(i);
            SerName(i) = SerName(t);
            SerName(t) = SerName(MaxItems);
            SerName(MaxItems) = "";
            SerCol(MaxItems) = SerCol(i);
            SerCol(i) = SerCol(t);
            SerCol(t) = SerCol(MaxItems);
            return SwapSeries;
        }

        private void chk_GDsave_Click()
        {
            graphsave(WhichGraphAmI) = chk_GDsave.value == 1;
        }

        private void DrawAxes(decimal Max)
        {
            decimal d = 0;

            decimal yunit = 0;

            int o = 0;

            int xo = 0;

            int yo = 0;

            xo = Riquadro.Left;
            yo = Riquadro.Top + Riquadro.Height;
            yunit = Riquadro.Height / Max();
            //Midline
            Line((xo, yo - yunit * Max() / 2) - (Riquadro.Left + Riquadro.Width, yo - yunit * Max() / 2), vbBlack);
            YLab(0).Content = CStr(Max() / 2);
            YLab(0).Left = xo;
            YLab(0).Top = (yo - yunit * Max() / 2);
            //Top
            Line((xo, Riquadro.Top) - (xo + Riquadro.Width, Riquadro.Top), vbBlack);
            YLab(1).Content = CStr(Max());
            YLab(1).Left = xo;
            YLab(1).Top = Riquadro.Top;
        }

        private void Form_Activate()
        {
            string s = "";

            int t = 0;

            for (t = 0; t < MaxItems; t++)
            {
                if (SerName(t) != "")
                {
                    Label1(t).Visible = true;
                    Shape3(t).Visible = true;
                    popnum(t).Visible = true;
                    s = SerName(t);
                    if (Right(s, 4) == ".txt")
                    {
                        s = Left(s, Len(s) - 4);
                    }
                    if (Len(s) > 32)
                    {
                        Label1(t).Caption = Left(s, 25) + "..." + Right(s, 4);
                    }
                    else
                    {
                        Label1(t).Caption = s;
                    }
                    Shape3(t).FillColor = SerCol(t);
                }
            }

            //Botsareus 5/31/2013 Special graph info
            graphvisible(WhichGraphAmI) = true;

            //Botsareus 3/19/2014 Auto save
            if (y_graphs && (x_restartmode == 4 || x_restartmode == 5))
            {
                graphsave(WhichGraphAmI) = true;
                chk_GDsave.value = 1;
            }
        }

        private void Form_Load(object sender, RoutedEventArgs e)
        {
            Form_Load();
        }

        private void Form_Load()
        {
            int t = 0;

            for (t = 0; t < MaxItems; t++)
            {
                Label1(t).Visible = false;
                Shape3(t).Visible = false;
                popnum(t).Visible = false;
                popnum(t).Height = 255;
            }
            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE + SWP_NOSIZE);
            this
            this
            XLabel.Content = Str(SimOpts.ChartingInterval) + " cycles per data point";
        }

        private void Form_QueryUnload(int Cancel, int UnloadMode_UNUSED)
        {
            //Botsareus 6/14/2013 Fix to keep updating graph
            Visible = false;
            if (secretunloadoverwrite)
            { //Botsareus 6/29/2013
                Visible = true;
                graphvisible(WhichGraphAmI) = false;
                secretunloadoverwrite = false;
            }
            else
            {
                if (GraphUp)
                {
                    goto skipmsg; //Botsareus 9/26/2014 From Shvarz, make UI more frindly
                }
                if (MsgBox("Would you like to keep updating this graph?", vbYesNo | vbQuestion) == vbYes)
                {
                skipmsg:
                    Cancel = true;
                    Left = Screen.Width;
                    Top = Screen.Height;
                    Visible = true;
                }
                else
                {
                    //Botsareus 5/31/2013 Special graph info
                    Visible = true;
                    graphvisible(WhichGraphAmI) = false;
                }
            }
        }

        private void Form_Resize()
        {
            // TODO (not supported): On Error GoTo patch //Botsareus 10/12/2013 Attempt to fix a 380 error viea patch

            int t = 0;

            if (this > 900 & this > 3000)
            {
                Riquadro.Height = this - 850;
                Riquadro.Width = this - 3400;
                Riquadro.Top = 20;
                for (t = 0; t < MaxItems - 1; t++)
                {
                    Shape3(t).Left = Riquadro.Left + Riquadro.Width + 50;
                    popnum(t).Width = 600; // should be enough for five digits left and one right of the decimal
                    popnum(t).Left = Shape3(t).Left + Shape3(t).Width + 30;
                    Label1(t).Left = popnum(t).Left + popnum(t).Width + 30;
                    Shape3(t).Top = 45 + (t * 200);
                    popnum(t).Top = Shape3(t).Top;
                    Label1(t).Top = Shape3(t).Top;
                }
                UpdateNow.Left = Riquadro.Left + Riquadro.Width - 2000;
                UpdateNow.Top = Riquadro.Height + 30;
                ResetButton.Left = UpdateNow.Left + UpdateNow.Width + 30;
                ResetButton.Top = Riquadro.Height + 30;
                chk_GDsave.Top = Riquadro.Height + 30; //Botsareus 8/3/2012 reposition the save graph data checkbox
                chk_GDsave.Left = ResetButton.Left + ResetButton.Width + 30;
                XLabel.Top = this - XLabel.Height - 550;
                RedrawGraph();
            }
        patch:
  }

        private void ResetBUtton_Click(object sender, RoutedEventArgs e)
        {
            ResetBUtton_Click();
        }

        private void ResetBUtton_Click()
        {
            Form1.ResetGraphs((WhichGraphAmI));
        }

        private void secret_exit_Click()
        { //Botsareus 6/29/2013
            secretunloadoverwrite = true;
            Unload(this);
        }

        private void UpdateNow_Click(object sender, RoutedEventArgs e)
        {
            UpdateNow_Click();
        }

        private void UpdateNow_Click()
        {
            Form1.FeedGraph((WhichGraphAmI));
        }

        private int WhichGraphAmI()
        {//Botsareus 8/3/2012 use names for graph id mod
            int WhichGraphAmI = 0;
            int chartNumber = 0;

            chartNumber = 0;

            //EricL Figuring which graph I am this way is a total hack, but it works
            switch (this)
            {
                case "Populations":
                    chartNumber = POPULATION_GRAPH;
                    break;

                case "Average_Mutations":
                    chartNumber = MUTATIONS_GRAPH;
                    break;

                case "Average_Age":
                    chartNumber = AVGAGE_GRAPH;
                    break;

                case "Average_Offspring":
                    chartNumber = OFFSPRING_GRAPH;
                    break;

                case "Average_Energy":
                    chartNumber = ENERGY_GRAPH;
                    break;

                case "Average_DNA_length":
                    chartNumber = DNALENGTH_GRAPH;
                    break;

                case "Average_DNA_Cond_statements":
                    chartNumber = DNACOND_GRAPH;
                    break;

                case "Average_Mutations_per_DNA_length_x1000-":
                    chartNumber = MUT_DNALENGTH_GRAPH;
                    break;

                case "Total_Energy_per_Species_x1000-":
                    chartNumber = ENERGY_SPECIES_GRAPH;
                    break;

                case "Dynamic_Costs":
                    chartNumber = DYNAMICCOSTS_GRAPH;
                    break;

                case "Species_Diversity":
                    chartNumber = SPECIESDIVERSITY_GRAPH;
                    break;

                case "Average_Chloroplasts":
                    chartNumber = AVGCHLR_GRAPH;
                    break;

                case "Genetic_Distance_x1000-":
                    chartNumber = GENETIC_DIST_GRAPH;
                    break;

                case "Max_Generational_Distance":
                    chartNumber = GENERATION_DIST_GRAPH;
                    break;

                case "Simple_Genetic_Distance_x1000-":
                    chartNumber = GENETIC_SIMPLE_GRAPH;
                    break;

                case "Customizable_Graph_1-":
                    chartNumber = CUSTOM_1_GRAPH;
                    break;

                case "Customizable_Graph_2-":
                    chartNumber = CUSTOM_2_GRAPH;
                    break;

                case "Customizable_Graph_3-":
                    chartNumber = CUSTOM_3_GRAPH;
                    break;
            }

            WhichGraphAmI = chartNumber;
            return WhichGraphAmI;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            int c = 0, u = 0; Form_QueryUnload(c, u); e.Cancel = c != 0;
        }

        /*
        'Public Sub AddVal(n As String, x As Single, s As Integer)
        '  Dim k As Byte
        '  k = 0
        '  While k < MaxSeries And SerName(k) <> n
        '    k = k + 1
        '  Wend
        '  If SerName(k) <> n Then
        '    AddSeries n, RGB(Random(0, 255), Random(0, 255), Random(0, 255))
        '    s = MaxSeries - 1
        '  End If
        '  data(Pivot, s) = x
        'End Sub
        */
    }
}
