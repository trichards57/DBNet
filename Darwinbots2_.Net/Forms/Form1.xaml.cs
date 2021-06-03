using Iersera.Model;
using System;
using System.Collections.Generic;
using System.Windows;
using static BucketManager;
using static Common;
using static DNAManipulations;
using static Globals;
using static Master;
using static Microsoft.VisualBasic.Constants;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.FileSystem;
using static Microsoft.VisualBasic.Information;
using static Microsoft.VisualBasic.Interaction;
using static Microsoft.VisualBasic.Strings;
using static Microsoft.VisualBasic.VBMath;
using static ObstaclesManager;
using static Robots;
using static ShotsManager;
using static SimOpt;
using static stuffcolors;
using static System.Math;
using static Teleport;
using static VBConstants;
using static VBExtension;
using static Vegs;

namespace DBNet.Forms
{
    public partial class Form1 : Window
    {
        public const double PIOVER4 = Math.PI / 4;
        public bool Active = false;
        public string BackPic = "";
        public bool camfix = false;
        public int cyc = 0;

        // cycles/second
        public bool dispskin = false;

        public int DNAMaxConds = 0;
        public bool Flickermode = false;
        public float FortyEightOverTwipHeight = 0;
        public float FortyEightOverTwipWidth = 0;

        //Speed up graphics at the cost of some flicker
        public int MagX = 0;

        public int MagY = 0;
        public bool Newpic = false;

        //Botsareus 2/23/2013 normalizes screen
        public bool pausefix = false;

        public bool PiccyMode = false;

        // TODO: WithEvents not supported on t
        public TrayIcon t = null;

        //Botsareus 3/6/2013 Figure out if simulation must start paused
        public int TotalOffspring = 0;

        public float TwipHeight = 0;

        //display that piccy or not?
        //IIs it a new picture?
        public float twipWidth = 0;

        public int visibleh = 0;

        // skin drawing enabled?
        // sim running?
        public float visiblew = 0;

        public float xDivisor = 0;
        public float yDivisor = 0;
        private static Form1 _instance;

        //Botsareus 5/22/2013 For find best
        private bool Cancer = false;

        // field visible portion (for zoom)
        // max conditions per gene allowed by mutation
        private List<Graph> Charts = new List<Graph>(new Graph[(NUMGRAPHS + 1)]);

        private bool DraggingBot = false;

        //Botsareus 10/21/2016 For zerobot mode ignore cancer familys
        private List<float> edat = new List<float>(new float[11]);

        // TODO: Confirm Array Size By Token// array of graph pointers
        private int GridRef = 0;

        private bool MouseClicked = false;
        private int MouseClickX = 0;

        // mouse pos when clicked
        private int MouseClickY = 0;

        private int p_reclev = 0;
        private Stack QStack = null;
        private List<tmppostyp> tmppos = new List<tmppostyp>(new tmppostyp[51]);
        private byte tmprob_c = 0;
        private bool ZoomFlag = false;

        public Form1()
        {
            InitializeComponent();
        }

        public static Form1 instance { set { _instance = null; } get { return _instance ?? (_instance = new Form1()); } }

        public static void Load()
        {
            if (_instance == null) { dynamic A = Form1.instance; }
        }

        public static void Unload()
        {
            if (_instance != null) instance.Close(); _instance = null;
        }

        public void CloseGraph(int n)
        {
            if (!(Charts[n].graf == null))
            {
                Charts[n].graf.Unload();
            }
        }

        public void cyccaption(float num)
        {
            MDIForm1.instance.infos(num, TotalRobotsDisplayed, totnvegsDisplayed, TotalChlr, SimOpts.TotBorn, SimOpts.TotRunCycle, SimOpts.TotRunTime); //Botsareus 8/25/2013 Mod to send TotalChlr
        }

        public int discendenti(int t, int disce)
        {
            int discendenti_v = 0;
            int k = 0;

            int n = 0;

            int rid = 0;

            rid = rob[t].AbsNum;
            if (disce < 1000)
            {
                for (n = 1; n < MaxRobs; n++)
                {
                    if (rob[n].exist)
                    {
                        if (rob[n].parent == rid)
                        {
                            discendenti_v = discendenti_v + 1;
                            disce = disce + 1;
                            discendenti_v = discendenti_v + discendenti(n, disce);
                        }
                    }
                }
            }
            return discendenti_v;
        }

        public void DrawAllRobs()
        {
            //  int w = 0;

            //  int t = 0;

            //  int s = 0;

            //  bool noeyeskin = false;

            //  float PixelsPerTwip = 0;

            //  int PixRobSize = 0;

            //  int PixBorderSize = 0;

            //  float offset = 0;

            //  float halfeyewidth = 0;

            //  int visibleLeft = 0;

            //  int visibleRight = 0;

            //  int visibleTop = 0;

            //  int visibleBottom = 0;

            //  float low = 0;

            //  float highest = 0;

            //  float hi = 0;

            //  float length = 0;

            //  int a = 0;

            //  float r = 0;

            //  visibleLeft = Form1.ScaleLeft;
            //  visibleRight = Form1.ScaleLeft + Form1.ScaleWidth;
            //  visibleTop = Form1.ScaleTop;
            //  visibleBottom = Form1.ScaleTop + Form1.ScaleHeight;

            //  twipWidth = GetTwipWidth();
            //  TwipHeight = GetTwipHeight();

            //  FortyEightOverTwipWidth = 48 / twipWidth;
            //  FortyEightOverTwipHeight = 48 / TwipHeight;

            //  noeyeskin = false;
            //  w = Int(30 / (Form1.visiblew / RobSize) + 1);
            //  if ((Form1.visiblew / RobSize) > 500) {
            //    noeyeskin = true;
            //  }
            //  DrawMode = 13;
            //  DrawStyle = 0;
            //  DrawWidth = w;

            //  if (robfocus > 0& MDIForm1.instance.showVisionGridToggle) {
            //    length = RobSize * 12;
            //    highest = rob[robfocus].aim + PI / 4;

            //    for(a=0; a<8; a++) {
            //      halfeyewidth = ((rob[robfocus].mem(EYE1WIDTH + a)) % 1256) / 400; // half the eyewidth, thus /400 not /200
            //      While(halfeyewidth > PI - PI / 36);
            //      halfeyewidth = halfeyewidth - PI;
            //      Wend();
            //      While(halfeyewidth < -PI / 36);
            //      halfeyewidth = halfeyewidth + PI;
            //      Wend();
            //      hi = highest - (PI / 18) * a + ((rob[robfocus].mem(EYE1DIR + a) % 1256) / 200) + halfeyewidth; // Display where the eye is looking
            //      low = hi - PI / 18 - (2 * halfeyewidth);

            //      While(hi > PI * 2);
            //      hi = hi - PI * 2;
            //      Wend();
            //      While(low > PI * 2);
            //      low = low - PI * 2;
            //      Wend();
            //      While(low < 0);
            //      low = low + PI * 2;
            //      Wend();
            //      While(hi < 0);
            //      hi = hi + PI * 2;
            //      Wend();

            //      if (rob[robfocus].mem(EyeStart + a + 1) > 0) {
            //        DrawMode = vbNotMergePen;

            //        length = (1 / Sqr(rob[robfocus].mem(EyeStart + a + 1))) * (EyeSightDistance(AbsoluteEyeWidth(rob[robfocus].mem(EYE1WIDTH + a)), robfocus) + rob[robfocus].radius) + rob[robfocus].radius;
            //        if (length < 0) {
            //          length = 0;
            //        }
            //      } else {
            //        DrawMode = vbCopyPen;
            //        length = EyeSightDistance(AbsoluteEyeWidth(rob[robfocus].mem(EYE1WIDTH + a)), robfocus) + rob[robfocus].radius + rob[robfocus].radius;
            //      }

            //      Circle((rob[robfocus].pos.x, rob[robfocus].pos.y), length, vbCyan, -low, -hi);

            //      if ((a == Abs(rob[robfocus].mem(FOCUSEYE) + 4) % 9)) {
            //        Circle((rob[robfocus].pos.x, rob[robfocus].pos.y), length, vbRed, low, hi);
            //      }

            //    }
            //  }

            //  DrawMode = vbCopyPen;
            //  DrawStyle = 0;

            //  if (noeyeskin) {
            //    for(a=1; a<MaxRobs; a++) {
            //      if (rob[a].exist && !(rob[a].FName == "Base.txt" && hidepred)) {
            //        r = rob[a].radius;
            //        if (rob[a].pos.x + r > visibleLeft && rob[a].pos.x - r < visibleRight && rob[a].pos.y + r > visibleTop && rob[a].pos.y - r < visibleBottom) {
            //          DrawRobDistPer(a);
            //        }
            //      }
            //    }
            //  } else {
            //    FillColor = BackColor;
            //    for(a=1; a<MaxRobs; a++) {
            //      if (rob[a].exist && !(rob[a].FName == "Base.txt" && hidepred)) {
            //        r = rob[a].radius;
            //        if (rob[a].pos.x + r > visibleLeft && rob[a].pos.x - r < visibleRight && rob[a].pos.y + r > visibleTop && rob[a].pos.y - r < visibleBottom) {
            //          DrawRobPer(a);
            //        }
            //      }
            //    }
            //  }

            //  DrawStyle = 0;
            //  if (dispskin && !noeyeskin) {
            //    for(a=1; a<MaxRobs; a++) {
            //      if (rob[a].exist && !(rob[a].FName == "Base.txt" && hidepred)) {
            //        if (rob[a].pos.x + r > visibleLeft && rob[a].pos.x - r < visibleRight && rob[a].pos.y + r > visibleTop && rob[a].pos.y - r < visibleBottom) {
            //          DrawRobSkin(a);
            //        }
            //      }
            //    }
            //  }

            //  DrawWidth = w * 2;

            //  if (!noeyeskin) {
            //    for(a=1; a<MaxRobs; a++) {
            //      if (rob[a].exist && !(rob[a].FName == "Base.txt" && hidepred)) {
            //        if (rob[a].pos.x + r > visibleLeft && rob[a].pos.x - r < visibleRight && rob[a].pos.y + r > visibleTop && rob[a].pos.y - r < visibleBottom) {
            //          DrawRobAim(a);
            //        }
            //      }
            //    }
            //  }

            //  FillStyle = 1;
            //  DrawWidth = 1;
            ////draw memory monitor
            //  if (MDIForm1.instance.MonitorOn.Checked) {
            //    for(a=1; a<MaxRobs; a++) {
            //      if (rob[a].exist && !(rob[a].FName == "Base.txt" && hidepred)) {
            //        r = rob[a].radius;
            //        if (rob[a].pos.x + r > visibleLeft && rob[a].pos.x - r < visibleRight && rob[a].pos.y + r > visibleTop && rob[a].pos.y - r < visibleBottom) {
            //          DrawMonitor(a);
            //        }
            //      }
            //    }
            //  }
            //  FillStyle = 0;
        }

        public void DrawAllTies()
        {
            //int t = 0;

            //float PixelsPerTwip = 0;

            //int PixRobSize = 0;

            //int visibleLeft = 0;

            //int visibleRight = 0;

            //int visibleTop = 0;

            //int visibleBottom = 0;

            //visibleLeft = Form1.ScaleLeft;
            //visibleRight = Form1.ScaleLeft + Form1.ScaleWidth;
            //visibleTop = Form1.ScaleTop;
            //visibleBottom = Form1.ScaleTop + Form1.ScaleHeight;

            //PixelsPerTwip = GetTwipWidth();
            //PixRobSize = PixelsPerTwip * RobSize;

            //for(t=1; t<MaxRobs; t++) {
            //  if (rob[t].exist && !(rob[t].FName == "Base.txt" && hidepred)) {
            //    if (rob[t].pos.x > visibleLeft && rob[t].pos.x < visibleRight && rob[t].pos.y > visibleTop && rob[t].pos.y < visibleBottom) {
            //      DrawRobTiesCol(t, PixelsPerTwip * rob[t].radius * 2, rob[t].radius);
            //    }
            //  }
            //}
        }

        public void DrawShots()
        {
            //DrawWidth = Int(50 / (ScaleWidth / RobSize) + 1);
            //if (DrawWidth > 2) {
            //  DrawWidth = 2;
            //}
            //int t = 0;

            //FillStyle = 0;
            //for(t=1; t<maxshotarray; t++) {
            //  if (Shots[t].flash && MDIForm1.instance.displayShotImpactsToggle) {
            //    if (Shots[t].shottype < 0& Shots[t].shottype >= -7) {
            //      FillColor = FlashColor(Shots[t].shottype + 10);
            //      Form1.Circle((Shots[t].opos.x, Shots[t].opos.y), 20, FlashColor(Shots[t].shottype + 10));
            //    } else {
            //      FillColor = vbBlack;
            //      Form1.Circle((Shots[t].opos.x, Shots[t].opos.y), 20, vbBlack);
            //    }
            //  } else if (Shots[t].exist && Shots[t].stored == false) {
            //    PSet((Shots[t].pos.x, Shots[t].pos.y), Shots[t].color);
            //  }
            //}
            //FillColor = BackColor;
        }

        public void FeedGraph(int GraphNumber)
        {
            string[] nomi = new string[MAXSPECIES + 1];
            float[,] dati = new float[MAXSPECIES + 1, NUMGRAPHS + 1];

            int t = 0;
            int k = 0;
            int p = 0;
            int i = 0;

            int startingChart = 0;
            int endingChart = 0;

            // This should never be the case.
            if (GraphNumber < 0 || GraphNumber > NUMGRAPHS)
            {
                return;
            }

            CalcStats(nomi, dati, GraphNumber);

            if (GraphNumber == 0)
            {
                // Update all the graphs
                startingChart = 1;
                endingChart = NUMGRAPHS;
            }
            else
            {
                // Only update one graph
                startingChart = GraphNumber;
                endingChart = GraphNumber;
            }

            t = Flex.last(nomi);

            for (k = startingChart; k < endingChart; k++)
            {
                if (k == 10)
                {
                    t = 1;
                }
                for (p = 1; p < t; p++)
                {
                    if (!(Charts[k].graf == null))
                    {
                        if (k == 10)
                        {
                            Charts[k].graf.SetValues("Cost Multiplier", dati[1, k]);
                            Charts[k].graf.SetValues("Population / Target", dati[2, k]);
                            Charts[k].graf.SetValues("Upper Range", dati[3, k]);
                            Charts[k].graf.SetValues("Lower Range", dati[4, k]);
                            Charts[k].graf.SetValues("Zero Level", dati[5, k]);
                            Charts[k].graf.SetValues("Reinstatement Level", dati[6, k]);
                        }
                        else
                        {
                            Charts[k].graf.SetValues(nomi[p], dati[p, k]);
                        }
                    }
                }
            }
            for (k = startingChart; k < endingChart; k++)
            {
                if (!(Charts[k].graf == null))
                {
                    Charts[k].graf.NewPoints();
                }
            }
        }

        public int fittest()
        {
            int fittest = 0;
            //Botsareus 5/22/2013 Lets figure what we are searching for
            double sPopulation = 0;

            double sEnergy = 0;

            sEnergy = (IIf(intFindBestV2 > 100, 100, intFindBestV2)) / 100;
            sPopulation = (IIf(intFindBestV2 < 100, 100, 200 - intFindBestV2)) / 100;

            int t = 0;

            double s = 0;

            double Mx = 0;

            Mx = 0;
            for (t = 1; t < MaxRobs; t++)
            {
                if (rob[t].exist && !rob[t].Veg && rob[t].FName != "Corpse" && !(rob[t].FName == "Base.txt" && hidepred))
                {
                    TotalOffspring = 1;
                    Cancer = false;
                    s = score(t, 1, 10, 0) + rob[t].nrg + rob[t].body * 10; //Botsareus 5/22/2013 Advanced fit test
                    if (s < 0)
                    {
                        s = 0; //Botsareus 9/23/2016 Bug fix
                    }
                    s = Pow(TotalOffspring, sPopulation) * Pow(s, sEnergy);

                    if (x_restartmode == 7 || x_restartmode == 8)
                    {
                        if (Cancer)
                        {
                            s = 0; //Botsareus 10/21/2016 For zerobot mode ignore cancer familys
                        }
                    }

                    if (s >= Mx)
                    {
                        Mx = s;
                        fittest = t;
                    }
                }
            }

            //Z E R O B O T
            //Pass result of fittest back to evo
            if (x_restartmode == 7 || x_restartmode == 8)
            {
                if (rob[fittest].FName == "Mutate.txt")
                {
                    calculateZB(rob[fittest].AbsNum, Mx, fittest);
                    robfocus = fittest;
                }
            }
            return fittest;
        }

        public void NewGraph(int n, string YLab)
        {
            if ((Charts[n].graf == null))
            {
                grafico k = new grafico();

                Charts[n].graf = k;
                Charts[n].graf.ResetGraph();
                Charts[n].graf.Left = graphleft[n];
                Charts[n].graf.Top = graphtop[n];

                //  Charts[n].graf.SetYLabel YLab ' EricL - Don't need this line - dup of line below
            }
            else
            {
                //Botsareus 1/5/2013 reposition graph
                Charts[n].graf.Top = IIf(Charts[n].graf.Top != Screen.Height && Charts[n].graf.Visible, 0, graphtop[n]);
                Charts[n].graf.Left = IIf(Charts[n].graf.Left != Screen.Width && Charts[n].graf.Visible, 0, graphleft[n]);
            }

            Charts[n].graf.chk_GDsave.value = IIf(graphsave[n], 1, 0);
            //Charts[n].graf.SetYLabel YLab ' EricL 4/7/2006 Commented - just no longer need to call SetYLabel

            //EricL 4/7/2006 Just set the caption directly now without adding "/ Cycles..." to teh end of the caption
            Charts[n].graf.Caption = YLab;

            Charts[n].graf.Show();

            //EricL - 3/27/2006 Get the first data point and show the graph key right from the start
            FeedGraph(n);
        }

        public void Redraw()
        {
            //Botsareus 6/23/2016 Need to offset the robots by (actual velocity minus velocity) before drawing them
            //It is a hacky way of doing it, but should be a bit faster since the computation is only preformed twice, other than preforming it in each subsection.
            for (var t = 1; t < MaxRobs; t++)
            {
                if (rob[t].exist && !(rob[t].FName == "Base.txt" && hidepred))
                {
                    rob[t].pos.X = rob[t].pos.X - (rob[t].vel.X - rob[t].actvel.X);
                    rob[t].pos.Y = rob[t].pos.Y - (rob[t].vel.Y - rob[t].actvel.Y);
                }
            }

            if (numObstacles > 0)
            {
                ObstaclesManager.DrawObstacles();
            }

            DrawArena();
            DrawAllTies();
            DrawAllRobs();
            if (numTeleporters > 0)
            {
                Teleport.DrawTeleporters();
            }
            DrawShots();

            //Plase robots back
            for (var t = 1; t < MaxRobs; t++)
            {
                if (rob[t].exist && !(rob[t].FName == "Base.txt" && hidepred))
                {
                    rob[t].pos.X = rob[t].pos.X + (rob[t].vel.X - rob[t].actvel.X);
                    rob[t].pos.Y = rob[t].pos.Y + (rob[t].vel.Y - rob[t].actvel.Y);
                }
            }
        }

        public void ResetGraphs(int i)
        {
            if (i > 0)
            {
                if (!(Charts[i].graf == null))
                {
                    Charts[i].graf.ResetGraph();
                }
            }
            else
            {
                for (var k = 1; k < NUMGRAPHS; k++)
                {
                    if (!(Charts[k].graf == null))
                    {
                        Charts[k].graf.ResetGraph();
                    }
                }
            }
        }

        public void t_MouseDown(int Button)
        {
            if (MDIForm1.instance.stealthmode && Button == 1)
            {
                MDIForm1.instance.Show();
                t.Remove();
                MDIForm1.instance.stealthmode = false;
            }
            else if (MDIForm1.instance.stealthmode && Button == 2)
            {
                MDIForm1.instance.PopupMenu(MDIForm1.instance.TrayIconPopup);
            }
        }

        public void unfocus()
        {
            int t = 0;

            for (t = 1; t < MaxRobs; t++)
            {
                rob[t].highlight = false;
            }
        }

        internal double score(robot rob, int reclev, int maxrec, int tipo)
        {
            double score_v = 0;
            //int al = 0;

            //double dx = 0;

            //double dy = 0;

            //int cr = 0;

            //int ct = 0;

            //int t = 0;

            //if (tipo == 2)
            //{
            //    plines((r));
            //}
            //score_v = 0;
            //for (t = 1; t < MaxRobs; t++)
            //{
            //    if (rob[t].exist)
            //    {
            //        if (rob[t].parent == rob[r].AbsNum)
            //        {
            //            if (reclev < maxrec)
            //            {
            //                score_v = score_v + score(t, reclev + 1, maxrec, tipo);
            //            }
            //            if (tipo == 0)
            //            {
            //                score_v = score_v + InvestedEnergy(t); //Botsareus 8/3/2012 generational distance code
            //            }
            //            if (tipo == 4 && reclev > p_reclev)
            //            {
            //                p_reclev = reclev;
            //            }
            //            if (tipo == 1)
            //            {
            //                rob[t].highlight = true;
            //            }
            //            if (tipo == 3)
            //            {
            //                dx = (rob[r].pos.X - rob[t].pos.X) / 2;
            //                dy = (rob[r].pos.Y - rob[t].pos.Y) / 2;
            //                cr = RGB(128, 128, 128);
            //                ct = vbWhite;
            //                if (rob[r].AbsNum > rob[t].AbsNum)
            //                {
            //                    cr = vbWhite;
            //                    ct = RGB(128, 128, 128);
            //                }
            //                //Line((rob[t].pos.x, rob[t].pos.y)-Step(dx, dy), ct);
            //                //Line(((rob[r].pos.x, rob[r].pos.y, cr);
            //            }
            //        }
            //    }
            //}
            //if (tipo == 1)
            //{
            //    //Form1.Cls();
            //    DrawAllRobs();
            //}
            return score_v;
        }

        private void BoyLabl_Click(object sender, RoutedEventArgs e)
        {
            BoyLabl_Click();
        }

        private void BoyLabl_Click()
        {
            BoyLabl.setVisible(false);
        }

        private string calc_graphs()
        {
            string calc_graphs = "";
            string lg = "";

            byte i = 0;

            for (i = 1; i < NUMGRAPHS; i++)
            {
                if (!(Charts[i].graf == null))
                {
                    if (Charts[i].graf.Visible)
                    {
                        lg = lg + vbCrLf + Charts[i].graf.Caption;
                    }
                }
            }
            calc_graphs = lg;
            return calc_graphs;
        }

        private void CalcStats(dynamic nomi, dynamic dati, int graphNum)
        { //Botsareus 8/3/2012 use names for graph id mod
            int p = 0;
            int t = 0;
            int i = 0;
            int x = 0;

            int population = 0;

            int[,] ListOSubSpecies = new int[501, 10001];

            int[] speciesListIndex = new int[501];

            int SubSpeciesNumber = 0;

            dynamic l = null;
            int ll = 0;

            int sim = 0;

            // Dim numbots As Integer

            for (t = 0; t < SimOpts.SpeciesNum; t++)
            {
                speciesListIndex[t] = 0;
            }

            population = TotalRobotsDisplayed;

            //EricL - Modified in 2.42.5 to handle each graph separatly for perf reasons
            // numbots = 0
            switch (graphNum)
            {
                case 0:  // Do all the graphs

                    for (t = 1; t < MaxRobs; t++)
                    {
                        if (rob[t].exist)
                        {
                            p = Flex.Position(rob[t].FName, nomi);
                            dati[p, POPULATION_GRAPH] = dati[p, POPULATION_GRAPH] + 1;
                            dati[p, MUTATIONS_GRAPH] = dati[p, MUTATIONS_GRAPH] + rob[t].LastMut + rob[t].Mutations;
                            dati[p, AVGAGE_GRAPH] = dati[p, AVGAGE_GRAPH] + (rob[t].age / 100); // EricL 4/7/2006 Graph age in 100's of cycles
                            dati[p, OFFSPRING_GRAPH] = dati[p, OFFSPRING_GRAPH] + rob[t].SonNumber;
                            dati[p, ENERGY_GRAPH] = dati[p, ENERGY_GRAPH] + rob[t].nrg;
                            dati[p, DNALENGTH_GRAPH] = dati[p, DNALENGTH_GRAPH] + rob[t].DnaLen;
                            dati[p, DNACOND_GRAPH] = dati[p, DNACOND_GRAPH] + rob[t].condnum;
                            dati[p, MUT_DNALENGTH_GRAPH] = dati[p, MUT_DNALENGTH_GRAPH] + (rob[t].LastMut + rob[t].Mutations) / rob[t].DnaLen * 1000;
                            dati[p, ENERGY_SPECIES_GRAPH] = Round(dati[p, ENERGY_SPECIES_GRAPH] + (rob[t].nrg + rob[t].body * 10) * 0.001m, 2);

                            //Look through the subspecies we have seen so far and see if this bot has the same as any of them
                            i = 0;
                            while (i < speciesListIndex[p] && rob[t].SubSpecies != ListOSubSpecies[p, i])
                            {
                                i = i + 1;
                            }

                            if (i == speciesListIndex[p])
                            { // New sub species
                                ListOSubSpecies[p, i] = rob[t].SubSpecies;
                                speciesListIndex[p] = speciesListIndex[p] + 1;
                                dati[p, SPECIESDIVERSITY_GRAPH] = dati[p, SPECIESDIVERSITY_GRAPH] + 1;
                            }
                            if (!rob[t].Corpse)
                            {
                                if (rob[t].SubSpecies < 0)
                                {
                                    SubSpeciesNumber = 32000 + CInt(Abs(rob[t].SubSpecies));
                                }
                                else
                                {
                                    SubSpeciesNumber = rob[t].SubSpecies;
                                }

                                //Botsareus 8/3/2012 Generational Distance Graph
                                ll = FindGenerationalDistance(t);
                                if (ll > dati[p, GENERATION_DIST_GRAPH])
                                {
                                    dati[p, GENERATION_DIST_GRAPH] = ll;
                                }
                            }
                        }
                    }

                    t = Flex.last(nomi);
                    if (dati[p, POPULATION_GRAPH] != 0)
                    {
                        for (p = 1; p < t; p++)
                        {
                            dati[p, MUTATIONS_GRAPH] = Round(dati[p, MUTATIONS_GRAPH] / dati[p, POPULATION_GRAPH], 1);
                            dati[p, AVGAGE_GRAPH] = Round(dati[p, AVGAGE_GRAPH] / dati[p, POPULATION_GRAPH], 1);
                            dati[p, OFFSPRING_GRAPH] = Round(dati[p, OFFSPRING_GRAPH] / dati[p, POPULATION_GRAPH], 1);
                            dati[p, ENERGY_GRAPH] = Round(dati[p, ENERGY_GRAPH] / dati[p, POPULATION_GRAPH], 1);
                            dati[p, DNALENGTH_GRAPH] = Round(dati[p, DNALENGTH_GRAPH] / dati[p, POPULATION_GRAPH], 1);
                            dati[p, DNACOND_GRAPH] = Round(dati[p, DNACOND_GRAPH] / dati[p, POPULATION_GRAPH], 1);
                            dati[p, MUT_DNALENGTH_GRAPH] = Round(dati[p, MUT_DNALENGTH_GRAPH] / dati[p, POPULATION_GRAPH], 1);
                        }
                    }
                    dati[1, DYNAMICCOSTS_GRAPH] = SimOpts.Costs[COSTMULTIPLIER];

                    if (SimOpts.Costs[DYNAMICCOSTTARGET] = 0)
                    { // Divide by zero protection
                        dati[2, DYNAMICCOSTS_GRAPH] = population;
                    }
                    else
                    {
                        dati[2, DYNAMICCOSTS_GRAPH] = population / SimOpts.Costs[DYNAMICCOSTTARGET];
                    }

                    dati[3, DYNAMICCOSTS_GRAPH] = 1 + (SimOpts.Costs[DYNAMICCOSTTARGETUPPERRANGE] * 0.01);
                    dati[4, DYNAMICCOSTS_GRAPH] = 1 - (SimOpts.Costs[DYNAMICCOSTTARGETLOWERRANGE] * 0.01);
                    if (SimOpts.Costs[DYNAMICCOSTTARGET] == 0)
                    { // Divide by zero protection
                        dati[5, DYNAMICCOSTS_GRAPH] = SimOpts.Costs[BOTNOCOSTLEVEL];
                        dati[6, DYNAMICCOSTS_GRAPH] = SimOpts.Costs[COSTXREINSTATEMENTLEVEL];
                    }
                    else
                    {
                        dati[5, DYNAMICCOSTS_GRAPH] = SimOpts.Costs[BOTNOCOSTLEVEL] / SimOpts.Costs[DYNAMICCOSTTARGET];
                        dati[6, DYNAMICCOSTS_GRAPH] = SimOpts.Costs[COSTXREINSTATEMENTLEVEL] / SimOpts.Costs[DYNAMICCOSTTARGET];
                    }

                    //Botsareus 5/25/2013 Logic for custom graph
                    string myquery = "";

                    switch (graphNum)
                    {
                        case CUSTOM_1_GRAPH:
                            myquery = strGraphQuery1;
                            break;

                        case CUSTOM_2_GRAPH:
                            myquery = strGraphQuery2;
                            break;

                        case CUSTOM_3_GRAPH:
                            myquery = strGraphQuery3;
                            break;
                    }

                    //Botsareus 5/25/2013 Very simple genetic distance is calculated when necessary
                    if (myquery == "*simpgenetic*")
                    {
                        t = Flex.last(nomi);
                        for (p = 1; p < t; p++)
                        {
                            dati[p, GENETIC_DIST_GRAPH] = 0;
                        }

                        for (t = 1; t < MaxRobs; t++)
                        {
                            if (rob[t].exist && !rob[t].Corpse)
                            {
                                p = Flex.Position(rob[t].FName, nomi);

                                if (rob[t].GenMut > 0)
                                { //If there is not enough mutations for a graph check, skip it
                                    l = rob[t].OldGD;
                                    if (l > dati[p, GENETIC_DIST_GRAPH])
                                    {
                                        dati[p, GENETIC_DIST_GRAPH] = l;
                                    }
                                }
                                else
                                {
                                    rob[t].GenMut = rob[t].DnaLen / GeneticSensitivity; //we have enough mutations, reset counter

                                    float copyl = 0;

                                    copyl = 0;

                                    for (x = t + 1; x < MaxRobs; x++)
                                    { //search trough all robots and figure genetic distance for the once that have enough mutations
                                        if (rob[x].exist && !rob[x].Corpse && rob[x].FName == rob[t].FName && rob[x].GenMut == 0)
                                        { // Must exist, have enugh mutations, and be of same species
                                            l = DoGeneticDistance(t, x) * 1000;
                                            if (l > copyl)
                                            {
                                                copyl = l; //here we store the max genetic distance for a given robot
                                            }
                                        }

                                        if (x == UBound(rob))
                                        {
                                            break;
                                        }
                                    }

                                    if (copyl > dati[p, GENETIC_DIST_GRAPH])
                                    {
                                        dati[p, GENETIC_DIST_GRAPH] = copyl; //now we write this max distance
                                    }
                                    rob[t].OldGD = copyl; //since this robot will not checked for a while, we need to store it's genetic distance to be used later
                                }
                            }

                            if (t == UBound(rob))
                            {
                                break;
                            }
                        }
                    }

                    if (graphNum > 0)
                    {
                        for (p = 1; p < t; p++)
                        {
                            //calculate query
                            ClearQStack();
                            //query logic

                            var splt = Split(myquery, " ");
                            int q = 0;

                            for (q = 0; q < UBound(splt); q++)
                            {
                                //make sure data is lower case
                                splt[q] = LCase(splt[q]);
                                //loop trough each element and compute it as nessisary
                                if (splt[q] == CStr(Val(splt[q])))
                                {
                                    //push ze number
                                    PushQStack((Val(splt[q])));
                                }
                                else
                                {
                                    switch (splt[q])
                                    {
                                        case "pop":
                                            PushQStack(dati[p, POPULATION_GRAPH]);
                                            break;

                                        case "avgmut":
                                            PushQStack(dati[p, MUTATIONS_GRAPH]);
                                            break;

                                        case "avgage":
                                            PushQStack(dati[p, AVGAGE_GRAPH]);
                                            break;

                                        case "avgsons":
                                            PushQStack(dati[p, OFFSPRING_GRAPH]);
                                            break;

                                        case "avgnrg":
                                            PushQStack(dati[p, ENERGY_GRAPH]);
                                            break;

                                        case "avglen":
                                            PushQStack(dati[p, DNALENGTH_GRAPH]);
                                            break;

                                        case "avgcond":
                                            PushQStack(dati[p, DNACOND_GRAPH]);
                                            break;

                                        case "simnrg":
                                            PushQStack(dati[p, ENERGY_SPECIES_GRAPH]);
                                            break;

                                        case "specidiv":
                                            PushQStack(dati[p, SPECIESDIVERSITY_GRAPH]);
                                            break;

                                        case "maxgd":
                                            PushQStack(dati[p, GENERATION_DIST_GRAPH]);
                                            break;

                                        case "simpgenetic":
                                            PushQStack(dati[p, GENETIC_DIST_GRAPH]);
                                            break;

                                        case "add":
                                            Qadd();
                                            break;

                                        case "sub":
                                            QSub();
                                            break;

                                        case "mult":
                                            Qmult();
                                            break;

                                        case "div":
                                            Qdiv();
                                            break;

                                        case "pow":
                                            Qpow();
                                            break;
                                    }
                                }
                            }
                            //end query logic

                            //make sure graph is greater then zero
                            double holdqstack = 0;

                            holdqstack = PopQStack();
                            if (holdqstack < 0)
                            {
                                holdqstack = 0;
                            }

                            dati[p, graphNum] = holdqstack;
                        }
                    }

                getout2:

                    break;

                case POPULATION_GRAPH:
                    for (t = 1; t < MaxRobs; t++)
                    {
                        if (rob[t].exist)
                        {
                            p = Flex.Position(rob[t].FName, nomi);
                            dati[p, POPULATION_GRAPH] = dati[p, POPULATION_GRAPH] + 1;
                        }
                    }

                    break;

                case MUTATIONS_GRAPH:
                    for (t = 1; t < MaxRobs; t++)
                    {
                        if (rob[t].exist)
                        {
                            p = Flex.Position(rob[t].FName, nomi);
                            dati[p, POPULATION_GRAPH] = dati[p, POPULATION_GRAPH] + 1;
                            dati[p, MUTATIONS_GRAPH] = dati[p, MUTATIONS_GRAPH] + rob[t].LastMut + rob[t].Mutations;
                        }
                    }
                    t = Flex.last(nomi);
                    for (p = 1; p < t; p++)
                    {
                        if (dati[p, POPULATION_GRAPH] != 0)
                        {
                            dati[p, MUTATIONS_GRAPH] = Round(dati[p, MUTATIONS_GRAPH] / dati[p, POPULATION_GRAPH], 1);
                        }
                    }

                    break;

                case AVGAGE_GRAPH:
                    for (t = 1; t < MaxRobs; t++)
                    {
                        if (rob[t].exist)
                        {
                            p = Flex.Position(rob[t].FName, nomi);
                            dati[p, POPULATION_GRAPH] = dati[p, POPULATION_GRAPH] + 1;
                            dati[p, AVGAGE_GRAPH] = dati[p, AVGAGE_GRAPH] + (rob[t].age / 100); // EricL 4/7/2006 Graph age in 100's of cycles
                        }
                    }
                    t = Flex.last(nomi);
                    for (p = 1; p < t; p++)
                    {
                        if (dati[p, POPULATION_GRAPH] != 0)
                        {
                            dati[p, AVGAGE_GRAPH] = Round(dati[p, AVGAGE_GRAPH] / dati[p, POPULATION_GRAPH], 1);
                        }
                    }

                    break;

                case OFFSPRING_GRAPH:
                    for (t = 1; t < MaxRobs; t++)
                    {
                        if (rob[t].exist)
                        {
                            p = Flex.Position(rob[t].FName, nomi);
                            dati[p, POPULATION_GRAPH] = dati[p, POPULATION_GRAPH] + 1;
                            dati[p, OFFSPRING_GRAPH] = dati[p, OFFSPRING_GRAPH] + rob[t].SonNumber;
                        }
                    }
                    t = Flex.last(nomi);
                    for (p = 1; p < t; p++)
                    {
                        if (dati[p, POPULATION_GRAPH] != 0)
                        {
                            dati[p, OFFSPRING_GRAPH] = Round(dati[p, OFFSPRING_GRAPH] / dati[p, POPULATION_GRAPH], 1);
                        }
                    }

                    break;

                case ENERGY_GRAPH:
                    for (t = 1; t < MaxRobs; t++)
                    {
                        if (rob[t].exist)
                        {
                            // numbots = numbots + 1
                            p = Flex.Position(rob[t].FName, nomi);
                            dati[p, POPULATION_GRAPH] = dati[p, POPULATION_GRAPH] + 1;
                            dati[p, ENERGY_GRAPH] = dati[p, ENERGY_GRAPH] + rob[t].nrg;
                        }
                    }
                    t = Flex.last(nomi);
                    for (p = 1; p < t; p++)
                    {
                        if (dati[p, POPULATION_GRAPH] != 0)
                        {
                            dati[p, ENERGY_GRAPH] = Round(dati[p, ENERGY_GRAPH] / dati[p, POPULATION_GRAPH], 1);
                        }
                    }

                    break;

                case DNALENGTH_GRAPH:
                    for (t = 1; t < MaxRobs; t++)
                    {
                        //If Not .wall And .exist Then
                        if (rob[t].exist)
                        {
                            // numbots = numbots + 1
                            p = Flex.Position(rob[t].FName, nomi);
                            dati[p, POPULATION_GRAPH] = dati[p, POPULATION_GRAPH] + 1;
                            dati[p, DNALENGTH_GRAPH] = dati[p, DNALENGTH_GRAPH] + rob[t].DnaLen;
                        }
                    }
                    t = Flex.last(nomi);
                    for (p = 1; p < t; p++)
                    {
                        if (dati[p, POPULATION_GRAPH] != 0)
                        {
                            dati[p, DNALENGTH_GRAPH] = Round(dati[p, DNALENGTH_GRAPH] / dati[p, POPULATION_GRAPH], 1);
                        }
                    }

                    break;

                case DNACOND_GRAPH:
                    for (t = 1; t < MaxRobs; t++)
                    {
                        //If Not .wall And .exist Then
                        if (rob[t].exist)
                        {
                            //  numbots = numbots + 1
                            p = Flex.Position(rob[t].FName, nomi);
                            dati[p, POPULATION_GRAPH] = dati[p, POPULATION_GRAPH] + 1;
                            dati[p, DNACOND_GRAPH] = dati[p, DNACOND_GRAPH] + rob[t].condnum;
                        }
                    }
                    t = Flex.last(nomi);
                    for (p = 1; p < t; p++)
                    {
                        if (dati[p, POPULATION_GRAPH] != 0)
                        {
                            dati[p, DNACOND_GRAPH] = Round(dati[p, DNACOND_GRAPH] / dati[p, POPULATION_GRAPH], 1);
                        }
                    }

                    break;

                case MUT_DNALENGTH_GRAPH:
                    for (t = 1; t < MaxRobs; t++)
                    {
                        if (rob[t].exist)
                        {
                            p = Flex.Position(rob[t].FName, nomi);
                            dati[p, POPULATION_GRAPH] = dati[p, POPULATION_GRAPH] + 1;
                            dati[p, MUT_DNALENGTH_GRAPH] = dati[p, MUT_DNALENGTH_GRAPH] + (rob[t].LastMut + rob[t].Mutations) / rob[t].DnaLen * 1000;
                        }
                    }
                    t = Flex.last(nomi);
                    for (p = 1; p < t; p++)
                    {
                        if (dati[p, POPULATION_GRAPH] != 0)
                        {
                            dati[p, MUT_DNALENGTH_GRAPH] = Round(dati[p, MUT_DNALENGTH_GRAPH] / dati[p, POPULATION_GRAPH], 1);
                        }
                    }

                    break;

                case ENERGY_SPECIES_GRAPH:
                    for (t = 1; t < MaxRobs; t++)
                    {
                        if (rob[t].exist)
                        {
                            p = Flex.Position(rob[t].FName, nomi);
                            dati[p, ENERGY_SPECIES_GRAPH] = dati[p, ENERGY_SPECIES_GRAPH] + (rob[t].nrg + rob[t].body * 10) * 0.001m;
                        }
                    }

                    break;

                case DYNAMICCOSTS_GRAPH:
                    dati[1, DYNAMICCOSTS_GRAPH] = Round(SimOpts.Costs(COSTMULTIPLIER), 4);
                    if (SimOpts.Costs(DYNAMICCOSTTARGET) == 0)
                    { // Divide by zero protection
                        dati[2, DYNAMICCOSTS_GRAPH] = population;
                    }
                    else
                    {
                        dati[2, DYNAMICCOSTS_GRAPH] = population / SimOpts.Costs(DYNAMICCOSTTARGET);
                    }

                    dati[3, DYNAMICCOSTS_GRAPH] = 1 + (SimOpts.Costs(DYNAMICCOSTTARGETUPPERRANGE) * 0.01m);
                    dati[4, DYNAMICCOSTS_GRAPH] = 1 - (SimOpts.Costs(DYNAMICCOSTTARGETLOWERRANGE) * 0.01m);
                    if (SimOpts.Costs(DYNAMICCOSTTARGET) == 0)
                    { // Divide by zero protection
                        dati[5, DYNAMICCOSTS_GRAPH] = SimOpts.Costs(BOTNOCOSTLEVEL);
                        dati[6, DYNAMICCOSTS_GRAPH] = SimOpts.Costs(COSTXREINSTATEMENTLEVEL);
                    }
                    else
                    {
                        dati[5, DYNAMICCOSTS_GRAPH] = SimOpts.Costs(BOTNOCOSTLEVEL) / SimOpts.Costs(DYNAMICCOSTTARGET);
                        dati[6, DYNAMICCOSTS_GRAPH] = SimOpts.Costs(COSTXREINSTATEMENTLEVEL) / SimOpts.Costs(DYNAMICCOSTTARGET);
                    }

                    break;

                case SPECIESDIVERSITY_GRAPH:
                    for (t = 1; t < MaxRobs; t++)
                    {
                        if (rob[t].exist)
                        {
                            p = Flex.Position(rob[t].FName, nomi);

                            //Look through the subspecies we have seen so far and see if this bot has the same as any of them
                            i = 0;
                            while (i < speciesListIndex[p] && rob[t].SubSpecies != ListOSubSpecies[p, i])
                            {
                                i = i + 1;
                            }

                            if (i == speciesListIndex[p])
                            { // New sub species
                                ListOSubSpecies[p, i] = rob[t].SubSpecies;
                                speciesListIndex[p] = speciesListIndex[p] + 1;
                                dati[p, SPECIESDIVERSITY_GRAPH] = dati[p, SPECIESDIVERSITY_GRAPH] + 1;
                            }
                        }
                    }

                    break;//Botsareus 8/31/2013 The new chloroplast graph
                case AVGCHLR_GRAPH:
                    for (t = 1; t < MaxRobs; t++)
                    {
                        if (rob[t].exist)
                        {
                            p = Flex.Position(rob[t].FName, nomi);
                            dati[p, POPULATION_GRAPH] = dati[p, POPULATION_GRAPH] + 1;
                            dati[p, AVGCHLR_GRAPH] = dati[p, AVGCHLR_GRAPH] + rob[t].chloroplasts;
                        }
                    }
                    t = Flex.last(nomi);
                    for (p = 1; p < t; p++)
                    {
                        if (dati[p, POPULATION_GRAPH] != 0)
                        {
                            dati[p, AVGCHLR_GRAPH] = Round(dati[p, AVGCHLR_GRAPH] / dati[p, POPULATION_GRAPH], 1);
                        }
                    }

                getout3:

                    break;

                case GENETIC_DIST_GRAPH:
                    //Botsareus 4/9/2013 Genetic Distance Graph uses the new GenMut and OldGD variables

                    t = Flex.last(nomi);
                    for (p = 1; p < t; p++)
                    {
                        dati[p, GENETIC_DIST_GRAPH] = 0;
                    }

                    //show the graph update label and set value to zero
                    GraphLab.Content = "Updating Graph: 0%";
                    GraphLab.setVisible(true);

                    for (t = 1; t < MaxRobs; t++)
                    {
                        if (rob[t].exist && !rob[t].Corpse)
                        {
                            p = Flex.Position(rob[t].FName, nomi);

                            if (rob[t].GenMut > 0)
                            { //If there is not enough mutations for a graph check, skip it
                                l = rob[t].OldGD;
                                if (l > dati[p, GENETIC_DIST_GRAPH])
                                {
                                    dati[p, GENETIC_DIST_GRAPH] = l;
                                }
                            }
                            else
                            {
                                rob[t].GenMut = rob[t].DnaLen / GeneticSensitivity; //we have enough mutations, reset counter

                                //Dim copyl As Single
                                copyl = 0;

                                for (x = t + 1; x < MaxRobs; x++)
                                { //search trough all robots and figure genetic distance for the once that have enough mutations
                                    if (rob[x].exist && !rob[x].Corpse && rob[x].FName == rob[t].FName && rob[x].GenMut == 0)
                                    { // Must exist, have enugh mutations, and be of same species
                                        l = DoGeneticDistance(t, x) * 1000;
                                        if (l > copyl)
                                        {
                                            copyl = l; //here we store the max genetic distance for a given robot
                                        }

                                        //update the graph label
                                        GraphLab.Content = "Updating Graph: " + Int(t / MaxRobs * 100) + "." + Int(x / MaxRobs * 99) + "%";
                                        DoEvents();
                                    }

                                    if (x == UBound(rob))
                                    {
                                        break;
                                    }
                                }

                                if (copyl > dati[p, GENETIC_DIST_GRAPH])
                                {
                                    dati[p, GENETIC_DIST_GRAPH] = copyl; //now we write this max distance
                                }
                                rob[t].OldGD = copyl; //since this robot will not checked for a while, we need to store it's genetic distance to be used later
                                DoEvents();
                            }
                        }

                        if (t == UBound(rob))
                        {
                            break;
                        }
                    }

                    //hide the graph update label
                    GraphLab.setVisible(false);

                    break;

                case GENERATION_DIST_GRAPH:
                    t = Flex.last(nomi);

                    for (p = 1; p < t; p++)
                    {
                        dati[p, GENERATION_DIST_GRAPH] = 0;
                    }

                    for (t = 1; t < MaxRobs; t++)
                    {
                        if (rob[t].exist && !rob[t].Corpse)
                        {
                            p = Flex.Position(rob[t].FName, nomi);
                            //Botsareus 8/3/2012 Generational Distance Graph
                            ll = FindGenerationalDistance(t);
                            if (ll > dati[p, GENERATION_DIST_GRAPH])
                            {
                                dati[p, GENERATION_DIST_GRAPH] = ll;
                            }
                        }
                    }

                    break;

                case GENETIC_SIMPLE_GRAPH:
                    //show the graph update label and set value to zero
                    GraphLab.Content = "Updating Graph: 0%";
                    GraphLab.setVisible(true);

                    t = Flex.last(nomi);
                    for (p = 1; p < t; p++)
                    {
                        dati[p, GENETIC_SIMPLE_GRAPH] = 0;
                    }

                    for (t = 1; t < MaxRobs; t++)
                    {
                        if (rob[t].exist && !rob[t].Corpse)
                        {
                            p = Flex.Position(rob[t].FName, nomi);
                            for (x = t + 1; x < MaxRobs; x++)
                            {
                                if (rob[x].exist && !rob[x].Corpse && rob[x].FName == rob[t].FName)
                                { // Must exist, and be of same species
                                    l = DoGeneticDistance(t, x) * 1000;
                                    if (l > dati[p, GENETIC_SIMPLE_GRAPH])
                                    {
                                        dati[p, GENETIC_SIMPLE_GRAPH] = l; //here we store the max generational distance for a given robot
                                    }
                                }
                            }
                            GraphLab.Content = "Updating Graph: " + Int(t / MaxRobs * 100) + "%";
                            DoEvents();
                        }
                    }

                    //hide the graph update label
                    GraphLab.setVisible(false);
                    break;
            }
        }

        private void changerobcol()
        {
            ColorForm.instance.setcolor(rob[robfocus].color);
            rob[robfocus].color = ColorForm.instance.color;
        }

        private void ClearQStack()
        {
            QStack.pos = 0;
            QStack.val[0] = 0;
        }

        private void deletemark()
        {
            for (var t = 1; t < MaxRobs; t++)
            {
                rob[t].highlight = false;
            }
        }

        private void DrawArena()
        {
            ////Botsareus 8/16/2014 Draw Sun
            //  int sunstart = 0;

            //  int sunstop = 0;

            //  int sunadd = 0;

            //  int colr = 0;

            //  colr = vbYellow;
            //  if (TmpOpts.Tides > 0) {
            //    colr = RGB(255 - 255 * BouyancyScaling, 255 - 255 * BouyancyScaling, 0);
            //  }
            //  if (SimOpts.Daytime) {
            ////Range calculation 0.25 + ~.75 pow 3
            //    sunstart = (SunPosition - (0.25m + (SunRange ^ 3) * 0.75m) / 2) * SimOpts.FieldWidth;
            //    sunstop = (SunPosition + (0.25m + (SunRange ^ 3) * 0.75m) / 2) * SimOpts.FieldWidth;
            //    if (sunstart < 0) {
            //      sunadd = SimOpts.FieldWidth + sunstart;
            //      Line((sunadd, 0)-(SimOpts.FieldWidth, SimOpts.FieldHeight / 100), colr, BF);
            //      Line((sunadd, 0)-(sunadd, SimOpts.FieldHeight), colr);
            //      sunstart = 0;
            //    }
            //    if (sunstop > SimOpts.FieldWidth) {
            //      sunadd = sunstop - SimOpts.FieldWidth;
            //      Line((sunadd, 0)-(0, SimOpts.FieldHeight / 100), colr, BF);
            //      Line((sunadd, 0)-(sunadd, SimOpts.FieldHeight), colr);
            //      sunstop = SimOpts.FieldWidth;
            //    }
            //    Line((sunstart, 0)-(sunstop, SimOpts.FieldHeight / 100), colr, BF);
            //    Line((sunstart, 0)-(sunstart, SimOpts.FieldHeight), colr);
            //    Line((sunstop, 0)-(sunstop, SimOpts.FieldHeight), colr);
            //  }

            //  if (MDIForm1.instance.ZoomLock.IsChecked == 0) {
            //return;//no need to draw boundaries if we aren't going to see them

            //  }
            //  Line((0, 0)-(0, 0 + SimOpts.FieldHeight), vbWhite);
            //  Line(-(SimOpts.FieldWidth - 0, SimOpts.FieldHeight), vbWhite);
            //  Line(-(0 + SimOpts.FieldWidth, 0), vbWhite);
            //  Line(-(0, -0), vbWhite);
        }

        private void DrawMonitor(int n)
        {
            //double rangered = 0;

            //rangered = (frmMonitorSet.instance.Monitor_ceil_r - frmMonitorSet.instance.Monitor_floor_r) / 255;
            //double rangestart_r = 0;

            //rangestart_r = rob[n].monitor_r - frmMonitorSet.instance.Monitor_floor_r;
            //double valred = 0;

            //valred = rangestart_r / rangered;
            //if (valred > 255) {
            //  valred = 255;
            //}
            //if (valred < 0) {
            //  valred = 0;
            //}

            //double rangegreen = 0;

            //rangegreen = (frmMonitorSet.instance.Monitor_ceil_g - frmMonitorSet.instance.Monitor_floor_g) / 255;
            //double rangestart_g = 0;

            //rangestart_g = rob[n].monitor_g - frmMonitorSet.instance.Monitor_floor_g;
            //double valgreen = 0;

            //valgreen = rangestart_g / rangegreen;
            //if (valgreen > 255) {
            //  valgreen = 255;
            //}
            //if (valgreen < 0) {
            //  valgreen = 0;
            //}

            //double rangeblue = 0;

            //rangeblue = (frmMonitorSet.instance.Monitor_ceil_b - frmMonitorSet.instance.Monitor_floor_b) / 255;
            //double rangestart_b = 0;

            //rangestart_b = rob[n].monitor_b - frmMonitorSet.instance.Monitor_floor_b;
            //double valblue = 0;

            //valblue = rangestart_b / rangeblue;
            //if (valblue > 255) {
            //  valblue = 255;
            //}
            //if (valblue < 0) {
            //  valblue = 0;
            //}

            //double aspectmod = 0;

            //aspectmod = TwipHeight / twipWidth;
            //Line((rob[n].pos.x - rob[n].radius * 1.1, rob[n].pos.y - rob[n].radius * 1.1m / aspectmod)-(rob[n].pos.x + rob[n].radius * 1.1, rob[n].pos.y + rob[n].radius * 1.1m / aspectmod), RGB(valred, valgreen, valblue), B);
        }

        private void DrawRobAim(int n)
        {
            //  int x = 0;
            //  int y = 0;

            //  vector pos = null;

            //  vector pos2 = null;

            //  vector vol = null;

            //  vector arrow1 = null;

            //  vector arrow2 = null;

            //  vector arrow3 = null;

            //  vector temp = null;

            //  if (!rob[n].Corpse) {
            ////We have to remember that the upper left corner is (0,0)
            //    pos.x = rob[n].aimvector.x;
            //    pos.y = -rob[n].aimvector.y;

            //    pos2 = VectorAdd(rob[n].pos, VectorScalar(VectorUnit(pos), rob[n].radius));
            //    PSet((pos2.x, pos2.y), vbWhite);

            //    if (MDIForm1.instance.displayMovementVectorsToggle) {
            ////Draw the voluntary movement vectors
            //      if (.lastup != 0) {
            //        if (.lastup < -1000) {
            //          rob[n].lastup = -1000;
            //        }
            //        if (.lastup > 1000) {
            //          rob[n].lastup = 1000;
            //        }
            //        vol = VectorAdd(pos2, VectorScalar(pos, CSng(rob[n].lastup)));
            //        Line((pos2.x, pos2.y)-(vol.x, vol.y), rob[n].color);

            //        arrow3 = VectorAdd(vol, VectorScalar(pos, 15)); // point of the arrowhead
            //        temp = VectorSet(Cos(rob[n].aim - PI / 2), Sin(rob[n].aim - PI / 2));
            //        temp.y = -temp.y;
            //        pos2 = VectorScalar(temp, 10);
            //        arrow1 = VectorAdd(vol, pos2); // left side of arrowhead
            //        arrow2 = VectorSub(vol, pos2); // right side of arrowhead
            //        Line((arrow1.x, arrow1.y)-(arrow3.x, arrow3.y), rob[n].color);
            //        Line((arrow2.x, arrow2.y)-(arrow3.x, arrow3.y), rob[n].color);
            //        Line((arrow1.x, arrow1.y)-(arrow2.x, arrow2.y), rob[n].color);
            //      }
            //      if (rob[n].lastdown != 0) {
            //        if (rob[n].lastdown < -1000) {
            //          rob[n].lastdown = -1000;
            //        }
            //        if (rob[n].lastdown > 1000) {
            //          rob[n].lastdown = 1000;
            //        }
            //        pos2 = VectorSub(rob[n].pos, VectorScalar(pos, rob[n].radius));
            //        vol = VectorSub(pos2, VectorScalar(pos, CSng(rob[n].lastdown)));
            //        Line((pos2.x, pos2.y)-(vol.x, vol.y), rob[n].color);

            //        arrow3 = VectorAdd(vol, VectorScalar(pos, -15)); // point of the arrowhead
            //        temp = VectorSet(Cos(rob[n].aim - PI / 2), Sin(rob[n].aim - PI / 2));
            //        temp.y = -temp.y;
            //        pos2 = VectorScalar(temp, 10);
            //        arrow1 = VectorAdd(vol, pos2); // left side of arrowhead
            //        arrow2 = VectorSub(vol, pos2); // right side of arrowhead
            //        Line((arrow1.x, arrow1.y)-(arrow3.x, arrow3.y), rob[n].color);
            //        Line((arrow2.x, arrow2.y)-(arrow3.x, arrow3.y), rob[n].color);
            //        Line((arrow1.x, arrow1.y)-(arrow2.x, arrow2.y), rob[n].color);
            //      }
            //      if (rob[n].lastleft != 0) {
            //        if (rob[n].lastleft < -1000) {
            //          rob[n].lastleft = -1000;
            //        }
            //        if (rob[n].lastleft > 1000) {
            //          rob[n].lastleft = 1000;
            //        }
            //        pos = VectorSet(Cos(rob[n].aim - PI / 2), Sin(rob[n].aim - PI / 2));
            //        pos.y = -pos.y;
            //        pos2 = VectorAdd(rob[n].pos, VectorScalar(pos, rob[n].radius));
            //        vol = VectorAdd(pos2, VectorScalar(pos, CSng(rob[n].lastleft)));
            //        Line((pos2.x, pos2.y)-(vol.x, vol.y), rob[n].color);

            //        arrow3 = VectorAdd(vol, VectorScalar(pos, 15)); // point of the arrowhead
            //        temp = rob[n].aimvector;
            //        temp.y = -temp.y;
            //        pos2 = VectorScalar(temp, 10);
            //        arrow1 = VectorAdd(vol, pos2); // left side of arrowhead
            //        arrow2 = VectorSub(vol, pos2); // right side of arrowhead
            //        Line((arrow1.x, arrow1.y)-(arrow3.x, arrow3.y), rob[n].color);
            //        Line((arrow2.x, arrow2.y)-(arrow3.x, arrow3.y), rob[n].color);
            //        Line((arrow1.x, arrow1.y)-(arrow2.x, arrow2.y), rob[n].color);
            //      }
            //      if (rob[n].lastright != 0) {
            //        if (rob[n].lastright < -1000) {
            //          rob[n].lastright = -1000;
            //        }
            //        if (rob[n].lastright > 1000) {
            //          rob[n].lastright = 1000;
            //        }
            //        pos = VectorSet(Cos(rob[n].aim + PI / 2), Sin(rob[n].aim + PI / 2));
            //        pos.y = -pos.y;
            //        pos2 = VectorAdd(rob[n].pos, VectorScalar(pos, rob[n].radius));
            //        vol = VectorAdd(pos2, VectorScalar(pos, CSng(rob[n].lastright)));
            //        Line((pos2.x, pos2.y)-(vol.x, vol.y), rob[n].color);

            //        arrow3 = VectorAdd(vol, VectorScalar(pos, 15)); // point of the arrowhead
            //        temp = rob[n].aimvector;
            //        temp.y = -temp.y;
            //        pos2 = VectorScalar(temp, 10);
            //        arrow1 = VectorAdd(vol, pos2); // left side of arrowhead
            //        arrow2 = VectorSub(vol, pos2); // right side of arrowhead
            //        Line((arrow1.x, arrow1.y)-(arrow3.x, arrow3.y), rob[n].color);
            //        Line((arrow2.x, arrow2.y)-(arrow3.x, arrow3.y), rob[n].color);
            //        Line((arrow1.x, arrow1.y)-(arrow2.x, arrow2.y), rob[n].color);
            //      }
            //    }
            //  }
        }

        private void DrawRobDistPer(int n)
        {
            //int CentreX = 0;
            //int CentreY = 0;

            //float nrgPercent = 0;

            //float bodyPercent = 0;

            //CentreX = rob[n].pos.x;
            //CentreY = rob[n].pos.y;

            //if (rob[n].highlight) {
            //  Circle((CentreX, CentreY), RobSize * 2, vbYellow); //new line
            //}
            //if (n == robfocus) {
            //  Circle((CentreX, CentreY), RobSize * 2, vbWhite);
            //}

            //Form1.FillColor = rob[n].color;

            //Circle((CentreX, CentreY), rob[n].radius, rob[n].color);
        }

        private void DrawRobPer(int n)
        {
            //int Sides = 0;

            //float t = 0;

            //float Sdlen = 0;

            //int CentreX = 0;

            //int CentreY = 0;

            //int realX = 0;

            //int realY = 0;

            //int xc = 0;

            //int yc = 0;

            //float radius = 0;

            //float Percent = 0;

            //int botDirection = 0;

            //float Diameter = 0;

            //float topX = 0;

            //float topY = 0;

            //CentreX = rob[n].pos.x;
            //CentreY = rob[n].pos.y;
            //radius = rob[n].radius;

            //if (rob[n].highlight) {
            //  Circle((CentreX, CentreY), radius * 1.2m, vbYellow);
            //}
            //if (n == robfocus) {
            //  Circle((CentreX, CentreY), radius * 1.2m, vbWhite);
            //}

            //FillColor = BackColor;

            //Circle((CentreX, CentreY), rob[n].radius, rob[n].color); //new line

            //if (MDIForm1.instance.displayResourceGuagesToggle == true) {
            //  if (rob[n].nrg > 0.5m) {
            //    if (rob[n].nrg < 32000) {
            //      Percent = rob[n].nrg / 32000;
            //    } else {
            //      Percent = 1;
            //    }
            //    if (Percent > 0.99m) {
            //      Percent = 0.99m; // Do this because the cirlce won't draw if it goes all the way around for some reason...
            //    }
            //    Circle((CentreX, CentreY), rob[n].radius * 0.95m, vbWhite, 0, (Percent * PI * 2));
            //  }

            //  if (rob[n].body > 0.5m) {
            //    if (rob[n].body < 32000) {
            //      Percent = rob[n].body / 32000;
            //    } else {
            //      Percent = 1;
            //    }
            //    if (Percent > 0.99m) {
            //      Percent = 0.99m; // Do this because the cirlce won't draw if it goes all the way around for some reason...
            //    }
            //    Circle((CentreX, CentreY), rob[n].radius * 0.9m, vbMagenta, 0, (Percent * PI * 2));
            //  }

            //  if (rob[n].Waste > 0.5m) {
            //    if (rob[n].Waste < 32000) {
            //      Percent = rob[n].Waste / 32000;
            //    } else {
            //      Percent = 1;
            //    }
            //    if (Percent > 0.99m) {
            //      Percent = 0.99m; // Do this because the cirlce won't draw if it goes all the way around for some reason...
            //    }
            //    Circle((CentreX, CentreY), rob[n].radius * 0.85m, vbGreen, 0, (Percent * PI * 2));
            //  }

            //  if (rob[n].venom > 0.5m) {
            //    if (rob[n].venom < 32000) {
            //      Percent = rob[n].venom / 32000;
            //    } else {
            //      Percent = 1;
            //    }
            //    if (Percent > 0.99m) {
            //      Percent = 0.99m; // Do this because the cirlce won't draw if it goes all the way around for some reason...
            //    }
            //    Circle((CentreX, CentreY), rob[n].radius * 0.8m, vbBlue, 0, (Percent * PI * 2));
            //  }

            //  if (rob[n].shell > 0.5m) {
            //    if (rob[n].shell < 32000) {
            //      Percent = rob[n].shell / 32000;
            //    } else {
            //      Percent = 1;
            //    }
            //    if (Percent > 0.99m) {
            //      Percent = 0.99m; // Do this because the cirlce won't draw if it goes all the way around for some reason...
            //    }
            //    Circle((CentreX, CentreY), rob[n].radius * 0.75m, vbRed, 0, (Percent * PI * 2));
            //  }

            //  if (rob[n].Slime > 0.5m) {
            //    if (rob[n].Slime < 32000) {
            //      Percent = rob[n].Slime / 32000;
            //    } else {
            //      Percent = 1;
            //    }
            //    if (Percent > 0.99m) {
            //      Percent = 0.99m; // Do this because the cirlce won't draw if it goes all the way around for some reason...
            //    }
            //    Circle((CentreX, CentreY), rob[n].radius * 0.7m, vbBlack, 0, (Percent * PI * 2));
            //  }

            //  if (rob[n].poison > 0.5m) {
            //    if (rob[n].poison < 32000) {
            //      Percent = rob[n].poison / 32000;
            //    } else {
            //      Percent = 1;
            //    }
            //    if (Percent > 0.99m) {
            //      Percent = 0.99m; // Do this because the cirlce won't draw if it goes all the way around for some reason...
            //    }
            //    Circle((CentreX, CentreY), rob[n].radius * 0.65m, vbYellow, 0, (Percent * PI * 2));
            //  }

            //  if (rob[n].Vtimer > 0) {
            //    Percent = rob[n].Vtimer / 100;
            //    if (Percent > 0.99m) {
            //      Percent = 0.99m; // Do this because the cirlce won't draw if it goes all the way around for some reason...
            //    }
            //    Circle((CentreX, CentreY), rob[n].radius * 0.6m, vbCyan, 0, (Percent * PI * 2));
            //  }

            //  if (rob[n].chloroplasts > 0) { //Panda 8/13/2013 Show how much chloroplasts a robot has
            //    Percent = rob[n].chloroplasts / 32000;
            //    if (Percent > 0.98m) {
            //      Percent = 0.98m;
            //    }
            //    Circle((CentreX, CentreY), rob[n].radius * 0.55m, vbGreen, 0, (Percent * PI * 2));
            //  }
            //}
        }

        private void DrawRobSkin(int n)
        {
            //  int x1 = 0;

            //  int x2 = 0;

            //  int y1 = 0;

            //  int y2 = 0;

            //  if (rob[n].Corpse) {
            //return;

            //  }
            //  if (rob[n].oaim != rob[n].aim) {
            //    int t = 0;

            //    rob[n].OSkin(0) = (Cos(rob[n].Skin(1) / 100 - rob[n].aim) * rob[n].Skin(0)) * rob[n].radius / 60;
            //    rob[n].OSkin(1) = (Sin(rob[n].Skin(1) / 100 - rob[n].aim) * rob[n].Skin(0)) * rob[n].radius / 60;
            //    PSet((rob[n].OSkin(0) + rob[n].pos.x, rob[n].OSkin(1) + rob[n].pos.y));
            //    for(t=2; t<6 Step 2; t++) {
            //      rob[n].OSkin(t) = (Cos(rob[n].Skin(t + 1) / 100 - rob[n].aim) * rob[n].Skin(t)) * rob[n].radius / 60;
            //      rob[n].OSkin(t + 1) == (Sin(rob[n].Skin(t + 1) / 100 - rob[n].aim) * rob[n].Skin(t)) * rob[n].radius / 60;
            //      Line(((rob[n].OSkin(t) + .pos.x, rob[n].OSkin(t + 1) + rob[n].pos.y,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,, +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +), rob[n].color);
            //    }
            //    rob[n].oaim = rob[n].aim;
            //  } else {
            //    PSet((rob[n].OSkin(0) + rob[n].pos.x, rob[n].OSkin(1) + rob[n].pos.y));
            //    for(t=2; t<6 Step 2; t++) {
            //      Line(((rob[n].OSkin(t) + rob[n].pos.x, rob[n].OSkin(t + 1) + rob[n].pos.y,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,, +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +  +), rob[n].color);
            //    }
            //  }
        }

        private void DrawRobTies(int t, int w, int s_UNUSED)
        {
            //byte k = 0;

            //int rp = 0;

            //int drawsmall = 0;

            //float CentreX = 0;

            //float CentreY = 0;

            //float CentreX1 = 0;

            //float CentreY1 = 0;

            //drawsmall = w / 4;
            //if (drawsmall == 0) {
            //  drawsmall = 1;
            //}

            //k = 1;
            //CentreX = rob[t].pos.x;
            //CentreY = rob[t].pos.y;
            //While(rob[t].Ties(k).pnt > 0);
            //if (!rob[t].Ties(k).back) {
            //  rp = rob[t].Ties(k).pnt;
            //  CentreX1 = rob(rp).pos.x;
            //  CentreY1 = rob(rp).pos.y;
            //  DrawWidth = drawsmall;
            //  if (rob[t].Ties(k).last > 0) {
            //    if (w > 2) {
            //      DrawWidth = w;
            //    } else {
            //      DrawWidth = 2;
            //    }
            //    Line((CentreX, CentreY)-(CentreX1, CentreY1), rob[t].color);
            //  }
            //}
            //k = k + 1;
        }

        private void DrawRobTiesCol(int t, int w, int s_UNUSED)
        {
            //byte k = 0;

            //int col = 0;

            //int rp = 0;

            //int drawsmall = 0;

            //float CentreX = 0;

            //float CentreY = 0;

            //float CentreX1 = 0;

            //float CentreY1 = 0;

            //drawsmall = w / 4;
            //if (drawsmall == 0) {
            //  drawsmall = 1;
            //}
            //k = 1;
            //CentreX = rob[t].pos.x;
            //CentreY = rob[t].pos.y;
            //While(rob[t].Ties(k).pnt > 0);
            //if (!rob[t].Ties(k).back) {
            //  rp = rob[t].Ties(k).pnt;
            //  CentreX1 = rob(rp).pos.x;
            //  CentreY1 = rob(rp).pos.y;
            //  DrawWidth = drawsmall;
            //  col = rob[t].color;
            //  if (w < 2) {
            //    w = 2;
            //  }
            //  if (rob[t].Ties(k).last > 0) {
            //    DrawWidth = w / 2; //size of birth ties
            //  }
            //  if (rob[t].Ties(k).infused) {
            //    col = vbWhite;
            //    rob[t].Ties(k).infused = false;
            //  }
            //  if (rob[t].Ties(k).nrgused) {
            //    col = vbRed;
            //    rob[t].Ties(k).nrgused = false;
            //  }
            //  if (rob[t].Ties(k).sharing) {
            //    col = vbYellow;
            //    rob[t].Ties(k).sharing = false;
            //  }

            //  Line((CentreX, CentreY)-(CentreX1, CentreY1), col);
            //}
            //k = k + 1;
            //Wend();
        }

        private dynamic FindGenerationalDistance(int id)
        {//Botsareus 8/3/2012 code to find generational distance
            dynamic FindGenerationalDistance = null;
            p_reclev = 0;
            score(id, 1, 500, 4);
            FindGenerationalDistance = p_reclev;
            return FindGenerationalDistance;
        }

        private void Form_Click(object sender, RoutedEventArgs e)
        {
            if (lblSafeMode.Visibility == Visibility.Visible)
            {
                return;//Botsareus 5/13/2013 Safemode restrictions
            }

            if (robfocus != 0 & MDIForm1.instance.pbOn.Checked)
            {
                return;
            }

            int n = 0;

            int m = 0;

            n = whichrob(CSng(MouseClickX), CSng(MouseClickY));

            //Click on teleporter unless its the internet mode teleporter.
            if (n == 0)
            {
                m = WhichTeleporter(MouseClickX, MouseClickY);
                if (m != 0 & Teleporters(m).Internet == false)
                {
                    teleporterFocus = m;
                }
            }

            if (n == 0 & m == 0)
            {
                m = WhichObstacle(MouseClickX, MouseClickY);
                if (m != 0)
                {
                    obstaclefocus = m;
                    MDIForm1.instance.DeleteShape.IsEnabled = true;
                }
            }

            if (n == 0 & m == 0)
            {
                robfocus = 0;
                MDIForm1.instance.DisableRobotsMenu();
                if (ActivForm.Instance.Visible)
                {
                    ActivForm.Instance.NoFocus();
                }
            }
        }

        private void Form_DblClick(object sender, RoutedEventArgs e)
        {
            int n = 0;

            int m = 0;

            n = whichrob(MouseClickX, MouseClickY);
            if (n == 0)
            {
                m = whichTeleporter(MouseClickX), MouseClickY);
            }
            if (n > 0)
            {
                robfocus = n;
                MDIForm1.instance.EnableRobotsMenu();
                if (!rob[n].highlight)
                {
                    deletemark();
                }
                datirob.instance.Visible = true;
                datirob.instance.RefreshDna();
                DoEvents(); //fixo?
                datirob.instance.infoupdate(n, rob[n].nrg, rob[n].parent, rob[n].Mutations, rob[n].age, rob[n].SonNumber, 1, rob[n].FName, rob[n].genenum, rob[n].LastMut, rob[n].generation, rob[n].DnaLen, rob[n].LastOwner, rob[n].Waste, rob[n].body, rob[n].mass, rob[n].venom, rob[n].shell, rob[n].Slime, rob[n].chloroplasts);
            }
            else if (m > 0)
            {
                TeleportForm.instance.teleporterFormMode = 1;
                TeleportForm.instance.Show();
            }
            else
            {
                datirob.instance.Form_Unload(0);
                robfocus = 0;
                MDIForm1.instance.DisableRobotsMenu();
                if (ActivForm.Instance.Visible)
                {
                    ActivForm.Instance.NoFocus();
                }
            }
        }

        private void Form_KeyDown(int KeyCode, int Shift_UNUSED)
        {
            if (PlayerBot.Visibility == Visibility.Visible)
            {
                for (var i = 1; i < UBound(PB_keys); i++)
                {
                    if (PB_keys[i].key == KeyCode)
                    {
                        PB_keys[i].Active = true;
                    }
                }
            }
        }

        private void Form_KeyUp(int KeyCode, int Shift_UNUSED)
        {
            if (PlayerBot.Visibility == Visibility.Visible)
            {
                for (var i = 1; i < UBound(PB_keys); i++)
                {
                    if (PB_keys[i].key == KeyCode)
                    {
                        PB_keys[i].Active = false;
                    }
                }
            }
        }

        private void Form_Load(object sender, RoutedEventArgs e)
        {
            Consoleform.instance.evnt = new cevent();

            LoadSysVars();
            MDIForm1.instance.visualize = true;
            MaxMem = 1000;
            maxfieldsize = SimOpts.FieldWidth * 2;
            TotalRobots = 0;
            robfocus = 0;
            MDIForm1.instance.DisableRobotsMenu();
            maxshotarray = 50;
            for (var i = 0; i < maxshotarray; i++) { ShotsManager.Shots.Add(null); }
            dispskin = true;

            FlashColor[1] = vbBlack; // Hit with memory shot
            FlashColor[-1 + 10] = vbRed; // Hit with Nrg feeding shot
            FlashColor[-2 + 10] = vbWhite; // Hit with Nrg Shot
            FlashColor[-3 + 10] = vbBlue; // Hit with venom shot
            FlashColor[-4 + 10] = vbGreen; // Hit with waste shot
            FlashColor[-5 + 10] = vbYellow; // Hit with poison Shot
            FlashColor[-6 + 10] = vbMagenta; // Hit with body feeding shot
            FlashColor[-7 + 10] = vbCyan; // Hit with virus shot
            InitObstacles();

            MDIForm1.instance.F1Piccy.setVisible(false);
            ContestMode = false;
        }

        private void Form_MouseDown(int Button, int Shift, float x, float y)
        {
            if (lblSafeMode.Visibility == Visibility.Visible)
            {
                return;//Botsareus 5/13/2013 Safemode restrictions
            }

            int n = 0;

            int k = 0;

            int m = 0;

            n = whichrob(x, y);

            //Botsareus 7/2/2014 Multiselect for pb mode
            if (n > 0 & MDIForm1.instance.pbOn.Checked && Shift == 1)
            {
                if (n != robfocus)
                {
                    rob[n].highlight = true;
                    Redraw();
                    return;
                }
            }

            if (n == 0)
            {
                DraggingBot = false;
                if (!SecTimer.Enabled)
                {
                    datirob.instance.Visible = false; //Botsareus 1/5/2013 Small fix to do with wrong data displayed in robot info, auto hide the window
                }
            }
            else
            {
                DraggingBot = true;
                gen_tmp_pos_lst();
            }

            if (n == 0)
            {
                teleporterFocus = WhichTeleporter(x, y);
                if (teleporterFocus != 0)
                {
                    //MDIForm1.DeleteTeleporter.Enabled = True
                    mousepos = VectorSet(x, y);
                }
                else
                {
                    // MDIForm1.DeleteTeleporter.Enabled = False
                }
            }

            if (n == 0 & teleporterFocus == 0)
            {
                obstaclefocus = WhichObstacle(x, y);
                if (obstaclefocus != 0)
                {
                    MDIForm1.instance.DeleteShape.IsEnabled = true;
                    mousepos = VectorSet(x, y);
                }
                else
                {
                    MDIForm1.instance.DeleteShape.IsEnabled = false;
                }
            }

            if (Button == 2)
            {
                robfocus = n;
                if (n > 0)
                {
                    MDIForm1.instance.PopupMenu(MDIForm1.instance.popup.DefaultProperty);
                    MDIForm1.instance.EnableRobotsMenu();
                }
            }

            if (n > 0)
            {
                robfocus = n;
                MDIForm1.instance.EnableRobotsMenu();
                if (!rob[n].highlight && (!(MDIForm1.pbOn.Checked) || Shift != 1))
                {
                    deletemark();
                }
            }

            if (n == 0 & Button == 1 && MDIForm1.instance.insrob)
            {
                if (lblSaving.Visibility != Visibility.Visible)
                { //Botsareus 9/6/2014 Bug fix
                    k = 0;
                    while (SimOpts.Specie[k].Name != "" && SimOpts.Specie[k].Name != MDIForm1.instance.Combo1.text)
                    {
                        k = k + 1;
                    }

                    if (SimOpts.Specie[k].path == "Invalid Path")
                    {
                        MsgBox(("The path for this robot is invalid."));
                    }
                    else
                    {
                        aggiungirob(k, x, y);
                    }
                }
            }

            if (Button == 1 && !MDIForm1.instance.insrob && n == 0)
            {
                MouseClicked = true;
            }

            Redraw();

            if (n == 0 & Button == 1 && MDIForm1.instance.pbOn.Checked)
            {
                MousePointer = vbCrosshair;
            }
        }

        private void Form_MouseMove(int Button, int Shift_UNUSED, float x, float y)
        {
            //Botsareus 7/2/2014 Added exitsub to awoid bugs
            int st = 0;

            int sl = 0;

            int vsv = 0;

            int hsv = 0;

            byte t = 0;

            vector vel = null;

            //visibleh = Int(ScaleHeight);
            if (Button == 0)
            {
                MouseClickX = x;
                MouseClickY = y;
            }

            //  if (Button == 1 && !MDIForm1.instance.insrob && obstaclefocus == 0& teleporterFocus == 0& !(MDIForm1.instance.pbOn.Checked)) {
            //    if (MouseClicked) {
            //      st = ScaleTop + MouseClickY - y;
            //      sl = ScaleLeft + MouseClickX - x;
            //      if (st < 0& MDIForm1.instance.ZoomLock.value == 0) {
            //        st = 0;
            //        MouseClickY = y;
            //      }
            //      if (sl < 0& MDIForm1.instance.ZoomLock.value == 0) {
            //        sl = 0;
            //        MouseClickX = x;
            //      }
            //      if (st > SimOpts.FieldHeight - visibleh && MDIForm1.instance.ZoomLock.value == 0) {
            //        st = SimOpts.FieldHeight - visibleh;
            //      }
            //      if (sl > SimOpts.FieldWidth - visiblew && MDIForm1.instance.ZoomLock.value == 0) {
            //        sl = SimOpts.FieldWidth - visiblew;
            //      }
            //      ScaleTop = st;
            //      ScaleLeft = sl;
            //      Refresh();
            //      Redraw();
            //      Refresh();
            //return;

            //    }
            //  }

            if (Button == 1 && robfocus > 0 & DraggingBot)
            {
                vel = VectorSub(rob[robfocus].pos, VectorSet(x, y));
                rob[robfocus].pos = VectorSet(x, y);
                rob[robfocus].vel = VectorSet(0, 0);
                rob[robfocus].actvel = VectorSet(0, 0); //Botsareus 6/24/2016 Bug fix
                byte a = 0;

                for (a = 1; a < tmprob_c; a++)
                {
                    rob[tmppos[a].n].pos = VectorSet(x - tmppos[a].x, y - tmppos[a].y);
                    rob[tmppos[a].n].vel = VectorSet(0, 0);
                    rob[tmppos[a].n].actvel = VectorSet(0, 0); //Botsareus 6/24/2016 Bug fix
                }
                if (!Active)
                {
                    Redraw();
                }
                return;
            }

            if (Button == 1 && obstaclefocus > 0)
            {
                ObstaclesManager.Obstacles[obstaclefocus].pos = VectorSet(x - (ObstaclesManager.Obstacles[obstaclefocus].Width / 2), y - (ObstaclesManager.Obstacles[obstaclefocus].Height / 2));
                if (!Active)
                {
                    Redraw();
                }
                return;
            }

            if (Button == 1 && teleporterFocus > 0)
            {
                Teleport.Teleporters(teleporterFocus).pos = VectorSet(x - (Teleport.Teleporters(teleporterFocus).Width / 2), y - (Teleport.Teleporters(teleporterFocus).Height / 2));
                if (!Active)
                {
                    Redraw();
                }
                return;
            }

            //Botsareus 7/2/2014 Overwrite for PlayerBot mode
            if (Button == 1 && MDIForm1.instance.pbOn.Checked)
            {
                Mouse_loc.X = x;
                Mouse_loc.Y = y;
            }
        }

        private void Form_MouseUp(int Button_UNUSED, int Shift_UNUSED, float x, float y)
        {
            if (lblSafeMode.Visibility == Visibility.Visible)
            {
                return;//Botsareus 5/13/2013 Safemode restrictions
            }

            MouseClicked = false;
            ZoomFlag = false; // EricL - stop zooming in!
            DraggingBot = false;

            if (MDIForm1.instance.pbOn.Checked && !MDIForm1.instance.insrob)
            {
                MousePointer = vbDefault;
                Mouse_loc.X = 0;
                Mouse_loc.Y = 0;
            }
        }

        private void gen_tmp_pos_lst()
        {
            int a = 0;

            bool rst = false;

            rst = true;
            tmprob_c = 0;
            for (a = 1; a < MaxRobs; a++)
            {
                if (rob[a].exist && rob[a].highlight && !(rob[a].FName == "Base.txt" && hidepred))
                {
                    if (a == robfocus)
                    {
                        rst = false;
                    }
                    tmprob_c++;
                    if (tmprob_c < 51)
                    {
                        tmppos[tmprob_c].n = a;
                        tmppos[tmprob_c].x = rob[robfocus].pos.X - rob[a].pos.X;
                        tmppos[tmprob_c].y = rob[robfocus].pos.Y - rob[a].pos.Y;
                    }
                    else
                    {
                        tmprob_c = 50;
                    }
                }
            }
            if (rst)
            {
                tmprob_c = 0;
            }
        }

        //   IM STUFF
        //'''''''''''''''''''''''''''''''''''''''''
        private dynamic IMgetname(int i)
        {
            dynamic IMgetname = null;
            IMgetname = extractexactname(rob[i].FName) + IIf(y_eco_im > 0, "(" + Trim(Left(rob[i].tag, 45)) + ")", "");

            string blank = "";

            if (Left(rob[i].tag, 45) == Left(blank, 45))
            {
                IMgetname = extractexactname(rob[i].FName);
            }

            IMgetname = Replace(IMgetname, "[", "");
            IMgetname = Replace(IMgetname, "]", "");
            IMgetname = Replace(IMgetname, "{", "");
            IMgetname = Replace(IMgetname, "}", "");
            IMgetname = Replace(IMgetname, ",", "");
            return IMgetname;
        }

        private double InvestedEnergy(int t)
        {//Botsareus 5/22/2013 Calculate both population and energy
            double InvestedEnergy = 0;
            InvestedEnergy = rob[t].nrg + rob[t].body * 10; //botschange fittest
            TotalOffspring++; //botschange fittest

            if (InvestedEnergy < 1000)
            {
                Cancer = true; //Botsareus 10/21/2016 For zerobot mode ignore cancer familys
            }
            return InvestedEnergy;
        }

        private void loadrobs()
        {
            int k = 0;

            int a = 0;

            int i = 0;

            k = 0;
            for (var cc = 1; cc < SimOpts.SpeciesNum; cc++)
            {
                for (var t = 1; t < SimOpts.Specie[k].qty; t++)
                {
                    a = RobScriptLoad(respath(SimOpts.Specie[k].path) + "\\" + SimOpts.Specie[k].Name);
                    if (a < 0)
                    {
                        t = SimOpts.Specie[k].qty;
                        SimOpts.Specie[k].Native = false;
                        break;
                    }
                    else
                    {
                        SimOpts.Specie[k].Native = true;
                    }
                    rob[a].Veg = SimOpts.Specie[k].Veg;
                    rob[a].NoChlr = SimOpts.Specie[k].NoChlr;
                    rob[a].Fixed = SimOpts.Specie[k].Fixed;
                    if (rob[a].Fixed)
                    {
                        rob[a].mem[216] = 1;
                    }
                    rob[a].pos.X = Random(SimOpts.Specie[k].Poslf * CSng(SimOpts.FieldWidth - 60), SimOpts.Specie[k].Posrg * CSng(SimOpts.FieldWidth - 60));
                    rob[a].pos.Y = Random(SimOpts.Specie[k].Postp * CSng(SimOpts.FieldHeight - 60), SimOpts.Specie[k].Posdn * CSng(SimOpts.FieldHeight - 60));

                    rob[a].nrg = SimOpts.Specie[k].Stnrg;
                    rob[a].body = 1000;

                    rob[a].radius = FindRadius(a);

                    rob[a].mem[SetAim] = (int)rob[a].aim * 200;
                    if (rob[a].Veg)
                    {
                        rob[a].chloroplasts = StartChlr; //Botsareus 2/12/2014 Start a robot with chloroplasts
                    }
                    rob[a].Dead = false;

                    rob[a].Mutables = SimOpts.Specie[k].Mutables;

                    for (i = 0; i < 7; i++)
                    { //Botsareus 5/20/2012 fix for skin engine
                        rob[a].Skin[i] = SimOpts.Specie[k].Skin[i];
                    }

                    rob[a].color = SimOpts.Specie[k].color;
                    rob[a].mem[timersys] = Random(-32000, 32000);
                    rob[a].CantSee = SimOpts.Specie[k].CantSee;
                    rob[a].DisableDNA = SimOpts.Specie[k].DisableDNA;
                    rob[a].DisableMovementSysvars = SimOpts.Specie[k].DisableMovementSysvars;
                    rob[a].CantReproduce = SimOpts.Specie[k].CantReproduce;
                    rob[a].VirusImmune = SimOpts.Specie[k].VirusImmune;
                    rob[a].virusshot = 0;
                    rob[a].Vtimer = 0;
                    rob[a].genenum = CountGenes(rob[a].dna);

                    rob[a].DnaLen = DnaLen(rob[a].dna);
                    rob[a].GenMut = rob[a].DnaLen / GeneticSensitivity; //Botsareus 4/9/2013 automatically apply genetic to inserted robots

                    rob[a].mem[DnaLenSys] = rob[a].DnaLen;
                    rob[a].mem[GenesSys] = rob[a].genenum;

                    //Botsareus 7/29/2014 New kill restrictions
                    rob[a].multibot_time = IIf(SimOpts.Specie[k].kill_mb, 210, 0);
                    rob[a].dq = IIf(SimOpts.Specie[k].dq_kill, 1, 0);
                }
                k++;
                MDIForm1.instance.Title = "Loading... " + Int((cc - 1) * 100 / SimOpts.SpeciesNum) + "% Please wait...";
            }
            MDIForm1.instance.Title = MDIForm1.instance.BaseCaption;
        }

        // main procedure. Oh yes!
        private void main()
        {
            int clocks = 0;

            int i = 0;

            do
            {
                if (Active)
                {
                    if (MDIForm1.instance.ignoreerror == true)
                    {
                        // TODO (not supported):         On Error Resume Next
                    }

                    SecTimer.Enabled = true;

                    UpdateSim();
                    MDIForm1.instance.Follow(); // 11/29/2013 zoom follow selected robot

                    if (StartAnotherRound)
                    {
                        return;
                    }

                    // redraws all:
                    if (MDIForm1.instance.visualize)
                    {
                        Label1.Visible = false;
                        if (!MDIForm1.instance.oneonten)
                        {
                            Redraw();
                        }
                        else
                        {
                            if (SimOpts.TotRunCycle % 10 == 0)
                            {
                                Redraw();
                            }
                        }
                    }

                    if (datirob.instance.Visible && !datirob.instance.ShowMemoryEarlyCycle)
                    {
                        datirob.instance.infoupdate(robfocus, rob[robfocus].nrg, rob[robfocus].parent, rob[robfocus].Mutations, rob[robfocus].age, rob[robfocus].SonNumber, 1, rob[robfocus].FName, rob[robfocus].genenum, rob[robfocus].LastMut, rob[robfocus].generation, rob[robfocus].DnaLen, rob[robfocus].LastOwner, rob[robfocus].Waste, rob[robfocus].body, rob[robfocus].mass, rob[robfocus].venom, rob[robfocus].shell, rob[robfocus].Slime, rob[robfocus].chloroplasts);
                    }

                    // feeds graphs with data:
                    if (SimOpts.TotRunCycle % SimOpts.ChartingInterval == 0)
                    {
                        for (i = 1; i < NUMGRAPHS; i++)
                        {
                            if (!(Charts[i].graf == null))
                            {
                                if (Charts[i].graf.Visible)
                                { //Botsareus 2/23/2013 Do not update chart if invisable
                                    FeedGraph(i);
                                }
                            }
                        }
                    }
                    if (SimOpts.TotRunCycle % 200 == 0)
                    {
                        if (InternetMode.Visibility == Visibility.Visible)
                        {
                            writeIMdata(); //Botsareus 9/6/2014 calculate stats for IM
                        }
                    }
                }
                DoEvents();
                if (MDIForm1.instance.limitgraphics == true)
                {
                    clocks = GetTickCount();
                    if ((GetTickCount() - clocks) < 67)
                    {
                        while ((GetTickCount() - clocks < 67)) { }
                    }
                }

                if (!camfix)
                {
                    MDIForm1.instance.fixcam(); //Botsareus 2/23/2013 normalizes screen
                    camfix = true;
                    Active = !pausefix; //Botsareus 3/6/2013 allowes starting a simulation paused
                    SecTimer.Enabled = !pausefix;
                }
            }
return;

        SaveError:
            MsgBox("Error. " + Err().Description + ".  Saving sim in saves directory as error.sim");

        tryagain:
            // TODO (not supported):   On Error GoTo missingdir
            SaveSimulation(MDIForm1.instance.MainDir + "\\saves\\error.sim");
        missingdir:
            if (Err().Number == 76)
            {
                var b = MsgBox("Cannot find the Saves Directory to save error.sim.  " + vbCrLf + "Would you like me to create one?   " + vbCrLf + vbCrLf + "If this is a new install, choose OK.", vbOKCancel | vbQuestion);
                if (b == vbOK)
                {
                    MkDir((MDIForm1.instance.MainDir + "\\saves"));
                    goto tryagain;
                }
                else
                {
                    MsgBox(("Exiting without saving error.sim."));
                }
            }

            End();
        }

        private int parent(int r)
        {
            int parent = 0;
            int t = 0;

            parent = 0;
            for (t = 1; t < MaxRobs; t++)
            {
                if (rob[t].AbsNum == rob[r].parent && rob[t].exist)
                {
                    parent = t;
                }
            }
            return parent;
        }

        private void plines(int t)
        {
            int p = 0;

            p = parent(t);
            while (p > 0)
            {
                t = p;
                p = parent(t);
            }
            if (p == 0)
            {
                p = t;
            }
            t = score(p, 1, 1000, 3);
        }

        private double PopQStack()
        {
            double PopQStack = 0;
            QStack.pos--;

            if (QStack.pos == -1)
            {
                QStack.pos = 0;
                QStack.val[0] = 0;
            }

            PopQStack = QStack.val[QStack.pos];
            return PopQStack;
        }

        private void PushQStack(double value)
        {
            int a = 0;

            if (QStack.pos >= 101)
            { //next push will overfill
                for (a = 0; a < 99; a++)
                {
                    QStack.val[a] = QStack.val[a + 1];
                }
                QStack.val[100] = 0;
                QStack.pos = 100;
            }

            QStack.val[QStack.pos] = value;
            QStack.pos++;
        }

        private void Qadd()
        {
            double a = 0;

            double b = 0;

            double c = 0;

            b = PopQStack();
            a = PopQStack();

            if (a > 2000000000)
            {
                a %= 2000000000;
            }
            if (b > 2000000000)
            {
                b %= 2000000000;
            }

            c = a + b;

            if (Abs(c) > 2000000000)
            {
                c -= Sign(c) * 2000000000;
            }
            PushQStack(c);
        }

        private void Qdiv()
        {
            double a = 0;

            double b = 0;

            b = PopQStack();
            a = PopQStack();
            if (b != 0)
            {
                PushQStack(a / b);
            }
            else
            {
                PushQStack(0);
            }
        }

        private void Qmult()
        {
            double a = 0;

            double b = 0;

            double c = 0;

            b = PopQStack();
            a = PopQStack();
            c = CDbl(a) * CDbl(b);
            if (Abs(c) > 2000000000)
            {
                c = Sgn(c) * 2000000000;
            }
            PushQStack(CDbl(c));
        }

        private void Qpow()
        {
            double a = 0;

            double b = 0;

            double c = 0;

            b = PopQStack();
            a = PopQStack();

            if (Abs(b) > 10)
            {
                b = 10 * Sgn(b);
            }

            if (a == 0)
            {
                c = 0;
            }
            else
            {
                c = Pow(a, b);
            }
            if (Abs(c) > 2000000000)
            {
                c = Sign(c) * 2000000000;
            }
            PushQStack(c);
        }

        private void QSub()
        { //Botsareus 5/20/2012 new code to stop overflow
            double a = 0;

            double b = 0;

            double c = 0;

            b = PopQStack();
            a = PopQStack();

            if (a > 2000000000)
            {
                a %= 2000000000;
            }
            if (b > 2000000000)
            {
                b %= 2000000000;
            }

            c = a - b;

            if (Abs(c) > 2000000000)
            {
                c -= Sign(c) * 2000000000;
            }
            PushQStack(c);
        }

        private void startloaded()
        {
            if (tmpseed != 0)
            {
                SimOpts.UserSeedNumber = tmpseed;
                TmpOpts.UserSeedNumber = tmpseed;
                //Botsareus 5/8/2013 save the safemode for 'load sim'
                optionsform.savesett(MDIForm1.instance.MainDir + "\\settings\\lastran.set"); //Botsareus 5/3/2013 Save the lastran setting
            }

            //lets reset the autosafe data
            VBOpenFile(1, App.path + "\\autosaved.gset"); ;
            Write(1, false);
            VBCloseFile(1);

            Rnd(-1);
            Randomize(SimOpts.UserSeedNumber / 100);

            //Botsareus 5/5/2013 Update the system that the sim is running

            VBOpenFile(1, App.path + "\\Safemode.gset"); ;
            Write(1, true);
            VBCloseFile(1);

            //Botsareus 4/27/2013 Create Simulation's skin
            H_S_L tmphsl = null;

            tmphsl.h = Int(Rnd() * 240);
            tmphsl.s = Int(Rnd() * 60) + 180;
            tmphsl.l = 222 + Int(Rnd() * 2) * 6;
            R_G_B tmprgb = null;

            tmprgb = hsltorgb(tmphsl);
            chartcolor = RGB(tmprgb.r, tmprgb.g, tmprgb.b);
            tmphsl.l -= 195;
            tmprgb = hsltorgb(tmphsl);
            backgcolor = RGB(tmprgb.r, tmprgb.g, tmprgb.b);
            //Botsareus 6/8/2013 Overwrite skin as nessisary
            if (UseOldColor)
            {
                chartcolor = vbWhite;
                backgcolor = 0x400000;
            }

            InitialiseBuckets();

            visiblew = SimOpts.FieldWidth;
            visibleh = SimOpts.FieldHeight;

            xDivisor = 1;
            yDivisor = 1;
            if (SimOpts.FieldWidth > 32000)
            {
                xDivisor = SimOpts.FieldWidth / 32000;
            }
            if (SimOpts.FieldHeight > 32000)
            {
                yDivisor = SimOpts.FieldHeight / 32000;
            }

            MDIForm1.instance.visualize = true;
            Active = true;
            MaxMem = 1000;
            maxfieldsize = SimOpts.FieldWidth * 2;
            robfocus = 0;
            MDIForm1.instance.DisableRobotsMenu();
            nlink = RobSize;
            klink = 0.01m;
            plink = 0.1m;
            mlink = RobSize * 1.5m;

            //EricL - This is used in Shot collision as a fast way to weed bots that could not possibily have collided with a shot
            //this cycle.  It is the maximum possible distance a bot center can be from a shot and still have had the shot impact.
            //This is the case where a bot with 32000 body and a shot are traveling at maximum velocity in opposite directions and the shot just
            //grazes the edge of the bot.  If the shot was just about to hit the bot at the end of the last cycle, then it's distance at
            //the end of this cycle will be the hypotinoose (sp?) of a right triangle ABC where side A is the maximum possible bot radius, and
            //side B is the sum of the maximum bot velocity and the maximum shot velocity, the latter of which can be robsize/3 + the bot
            //max velocity since bot velocity is added to shot velocity.
            MaxBotShotSeperation = Sqrt(Pow(FindRadius(0, -1), 2) + Pow(SimOpts.MaxVelocity * 2 + RobSize / 3, 2));

            defaultWidth = 0.2m;
            defaultHeight = 0.2m;

            MDIForm1.instance.DontDecayNrgShots.Checked = SimOpts.NoShotDecay;
            MDIForm1.instance.DontDecayWstShots.Checked = SimOpts.NoWShotDecay;

            MDIForm1.instance.DisableTies.Checked = SimOpts.DisableTies;
            MDIForm1.instance.DisableArep.Checked = SimOpts.DisableTypArepro;
            MDIForm1.instance.DisableFixing.Checked = SimOpts.DisableFixing;

            //Botsareus 4/18/2016 recording menu
            MDIForm1.instance.SnpDeadEnable.Checked = SimOpts.DeadRobotSnp;
            MDIForm1.instance.SnpDeadExRep.Checked = SimOpts.SnpExcludeVegs;

            MDIForm1.instance.AutoFork.Checked = SimOpts.EnableAutoSpeciation;

            SecTimer.IsEnabled = true;
            //setfeed
            if (MDIForm1.instance.visualize)
            {
                DrawAllRobs();
            }
            MDIForm1.instance.enablesim();

            NoDeaths = true;

            Vegs.cooldown = -SimOpts.RepopCooldown;
            totnvegsDisplayed = -1; // Just set this to -1 for the first cycle so the cost low water mark doesn't trigger.
            totvegs = -1; // Set to -1 to avoid veggy reproduction on first cycle
            totnvegs = (int)SimOpts.Costs[DYNAMICCOSTTARGET]; // Just set this high for the first cycle so the cost low water mark doesn't trigger.

            main();
        }

        private void StartSimul()
        {
            //Botsareus 5/8/2013 save the safemode for 'start new'
            optionsform.instance.savesett(MDIForm1.instance.MainDir + "\\settings\\lastran.set"); //Botsareus 5/3/2013 Save the lastran setting

            //lets reset the autosafe data
            VBOpenFile(1, App.path + "\\autosaved.gset"); ;
            Write(1, false);
            VBCloseFile(1);

            camfix = false; //Botsareus 2/23/2013 When simulation starts the scren is normailized

            MDIForm1.instance.visualize = true; //Botsareus 8/31/2012 reset vedio tuggle button
            MDIForm1.instance.menuupdate();

            Rnd(-1);
            Randomize(SimOpts.UserSeedNumber / 100);

            //Botsareus 5/5/2013 Update the system that the sim is running

            VBOpenFile(1, App.path + "\\Safemode.gset"); ;
            Write(1, true);
            VBCloseFile(1);

            //Botsareus 4/27/2013 Create Simulation's skin
            H_S_L tmphsl = null;

            tmphsl.h = Int(Rnd() * 240);
            tmphsl.s = Int(Rnd() * 60) + 180;
            tmphsl.l = 222 + Int(Rnd() * 2) * 6;
            R_G_B tmprgb = null;

            tmprgb = hsltorgb(tmphsl);
            chartcolor = RGB(tmprgb.r, tmprgb.g, tmprgb.b);
            tmphsl.l = tmphsl.l - 195;
            tmprgb = hsltorgb(tmphsl);
            backgcolor = RGB(tmprgb.r, tmprgb.g, tmprgb.b);
            //Botsareus 6/8/2013 Overwrite skin as nessisary
            if (UseOldColor)
            {
                chartcolor = vbWhite;
                backgcolor = 0x400000;
            }

            //Botsareus 8/16/2014 Lets initate the sun
            if (SimOpts.SunOnRnd)
            {
                SunRange = 0.5m;
                SunChange = Int(Rnd() * 3) + Int(Rnd() * 2) * 10;
                SunPosition = Rnd();
            }
            else
            {
                SunPosition = 0.5;
                SunRange = 1;
            }

            SimOpts.SimGUID = CInt(Rnd());
            Over = false;

            //if (BackPic != "") {
            //  Form1.Picture = LoadPicture(BackPic);
            //} else {
            //  Form1.Picture = null;
            //}

            Show();
            visiblew = SimOpts.FieldWidth;
            visibleh = SimOpts.FieldHeight;
            xDivisor = 1;
            yDivisor = 1;

            if (SimOpts.FieldWidth > 32000)
            {
                xDivisor = SimOpts.FieldWidth / 32000;
            }
            if (SimOpts.FieldHeight > 32000)
            {
                yDivisor = SimOpts.FieldHeight / 32000;
            }

            MDIForm1.instance.DontDecayNrgShots.Checked = SimOpts.NoShotDecay;
            MDIForm1.instance.DontDecayWstShots.Checked = SimOpts.NoWShotDecay;

            MDIForm1.instance.DisableTies.Checked = SimOpts.DisableTies;
            MDIForm1.instance.DisableArep.Checked = SimOpts.DisableTypArepro;
            MDIForm1.instance.DisableFixing.Checked = SimOpts.DisableFixing;

            //Botsareus 4/18/2016 recording menu
            MDIForm1.instance.SnpDeadEnable.Checked = SimOpts.DeadRobotSnp;
            MDIForm1.instance.SnpDeadExRep.Checked = SimOpts.SnpExcludeVegs;

            SimOpts.TotBorn = 0;
            grafico.instance.ResetGraph();
            MaxMem = 1000;
            maxfieldsize = SimOpts.FieldWidth * 2;
            robfocus = 0;
            MDIForm1.instance.DisableRobotsMenu();
            nlink = RobSize;
            klink = 0.01m;
            plink = 0.1m;
            mlink = RobSize * 1.5m;

            //EricL - This is used in Shot collision as a fast way to weed bots that could not possibily have collided with a shot
            //this cycle.  It is the maximum possible distance a bot center can be from a shot and still have had the shot impact.
            //This is the case where the bot and shot are traveling at maximum velocity in opposite directions and the shot just
            //grazes the edge of the bot.  If the shot was just about to hit the bot at the end of the last cycle, then it's distance at
            //the end of this cycle will be the hypotinose (sp?) of a right triangle C where side A is the miximum possible bot radius, and
            //side B is the sum of the maximum bot velocity and the maximum shot velocity, the latter of which can be robsize/3 + the bot
            //max velocity since bot velocity is added to shot velocity.
            MaxBotShotSeperation = Sqrt(Pow(FindRadius(0, -1), 2) + Pow(SimOpts.MaxVelocity * 2 + RobSize / 3, 2));

            for (var i = 0; i < 500; i++) { rob.Add(null); }

            for (var t = 1; t < 500; t++)
            {
                rob[t].exist = false;
                rob[t].virusshot = 0;
            }
            MaxRobs = 0;
            InitialiseBuckets();
            for (var i = 0; i < 50; i++) { ShotsManager.Shots.Add(null); }
            maxshotarray = 50;

            for (var t = 1; t < maxshotarray; t++)
            {
                ShotsManager.Shots[t].exist = false;
                ShotsManager.Shots[t].flash = false;
                ShotsManager.Shots[t].stored = false;
            }

            for (var t = 1; t < MAXOBSTACLES; t++)
            {
                ObstaclesManager.Obstacles[t].exist = false;
            }
            numObstacles = 0;

            defaultWidth = 0.2m;
            defaultHeight = 0.2m;

            MaxRobs = 0;
            loadrobs();
            if (Active)
            {
                SecTimer.IsEnabled = true;
            }
            SimOpts.TotRunTime = 0;
            //setfeed
            if (MDIForm1.instance.visualize)
            {
                DrawAllRobs();
            }
            MDIForm1.instance.enablesim();

            if (ContestMode)
            {
                FindSpecies();
                F1count = 0;
            }

            if (SimOpts.MaxEnergy > 5000)
            {
                if (MsgBox("Your nrg allotment is set to" + Str(SimOpts.MaxEnergy) + ".  A correct value is in the neighborhood of about 10 or so.  Do you want to change your energy allotment to 10?", vbYesNo, "Energy allotment suspicously high.") == vbYes)
                {
                    SimOpts.MaxEnergy = 10;
                }
            }

            strSimStart = Replace(Replace(DateTime.Now.ToString(), ":", "-"), "/", "-");

            //Botsareus 1/5/2014 The Obstacle Regeneration code

            for (var o = 1; o < UBound(xObstacle); o++)
            {
                if (xObstacle[o].exist)
                {
                    var oo = NewObstacle(xObstacle[o].pos.X * SimOpts.FieldWidth, xObstacle[o].pos.Y * SimOpts.FieldHeight, xObstacle[o].Width * SimOpts.FieldWidth, xObstacle[o].Height * SimOpts.FieldHeight);
                    ObstaclesManager.Obstacles[oo].color = xObstacle[o].color;
                    ObstaclesManager.Obstacles[oo].vel = xObstacle[o].vel;
                }
            }

            //sim running

            main();
        }

        private int whichrob(float x, float y)
        {
            int whichrob = 0;
            double dist = 0;
            double pist = 0;

            //Botsareus 6/23/2016 Need to offset the robots by (actual velocity minus velocity) before drawing them
            for (var t = 1; t < MaxRobs; t++)
            {
                if (rob[t].exist && !(rob[t].FName == "Base.txt" && hidepred))
                {
                    rob[t].pos.X = rob[t].pos.X - (rob[t].vel.X - rob[t].actvel.X);
                    rob[t].pos.Y = rob[t].pos.Y - (rob[t].vel.Y - rob[t].actvel.Y);
                }
            }

            whichrob = 0;
            dist = 10000;
            for (var t = 1; t < MaxRobs; t++)
            {
                if (rob[t].exist && !(rob[t].FName == "Base.txt" && hidepred))
                {
                    pist = Pow(rob[t].pos.X - x, 2) + Pow(rob[t].pos.Y - y, 2);
                    if (Abs(rob[t].pos.X - x) < rob[t].radius && Abs(rob[t].pos.Y - y) < rob[t].radius && pist < dist && rob[t].exist)
                    {
                        whichrob = t;
                        dist = pist;
                    }
                }
            }

            for (var t = 1; t < MaxRobs; t++)
            {
                if (rob[t].exist && !(rob[t].FName == "Base.txt" & hidepred))
                {
                    rob[t].pos.X = rob[t].pos.X + (rob[t].vel.X - rob[t].actvel.X);
                    rob[t].pos.Y = rob[t].pos.Y + (rob[t].vel.Y - rob[t].actvel.Y);
                }
            }
            return whichrob;
        }

        private void writeIMdata()
        {
            int b = 0;

            bool datehit = false;

            string simdata = "";

            List<IMbots> simpopulations = new List<IMbots> { }; // TODO - Specified Minimum Array Boundary Not Supported:     Dim simpopulations() As IMbots

            int upperbound = 0;

            List<IMbots> simpopulations_119_tmp = new List<IMbots>();
            for (int i = 0; i < 0; i++) { simpopulations.Add(null); }
            simdata = "{\"cycle\":" + SimOpts.TotRunCycle + ",\"simId\":\"" + strSimStart + "\",\"width\":" + SimOpts.FieldWidth + ",\"height\":" + SimOpts.FieldHeight + ",\"population\":["; //bug fix

            //calculate species
            for (i = 1; i < MaxRobs; i++)
            {
                if (rob[i].exist)
                {
                    datehit = false;
                    for (b = 1; b < upperbound; b++)
                    {
                        if (simpopulations[b].Name == IMgetname[i] && simpopulations[b].vegy == rob[i].Veg)
                        {
                            datehit = true;
                            break;
                        }
                    }
                    if (!datehit)
                    {
                        upperbound = upperbound + 1;
                        List<IMbots> simpopulations_8564_tmp = new List<IMbots>();
                        for (var i = 0; i < 0; i++) { simpopulations.Add(i < simpopulations.Count ? simpopulations[i] : null); }
                        simpopulations[upperbound].Name = IMgetname[i];
                        simpopulations[upperbound].vegy = rob[i].Veg;
                    }
                }
            }
            //calculate populations
            for (i = 1; i < MaxRobs; i++)
            {
                if (rob[i].exist)
                {
                    for (b = 1; b < upperbound; b++)
                    {
                        if (simpopulations[b].Name == IMgetname[i] && simpopulations[b].vegy == rob[i].Veg)
                        {
                            simpopulations[b].pop = simpopulations[b].pop + 1;
                        }
                    }
                }
            }

            for (b = 1; b < upperbound; b++)
            {
                simdata = simdata + "{\"botName\":\"" + simpopulations[b].Name + "\",\"count\":" + simpopulations[b].pop + IIf(simpopulations[b].vegy, ",\"repopulating\":true", "") + "}";
                if (b < upperbound)
                {
                    simdata = simdata + ",";
                }
            }

            simdata = simdata + "]}";

            VBOpenFile(299, OutboundPath + "\\" + SimOpts.TotRunCycle + SimOpts.UserSeedNumber + ".stats"); ;
            VBWriteFile(299, simdata); ;
            VBCloseFile(299);
        }

        // for graphs
        private class Graph
        {
            public grafico graf = null;
            public bool opened = false;
        }

        //Botsareus 8/3/2012 for generational distance
        private class IMbots
        {
            public string Name = "";
            public int pop = 0;
            public bool vegy = false;
        }

        private class Stack
        {
            public int pos = 0;
            public double[] val = new double[100];
        }

        // EricL True while mouse button four is held down - for zooming
        // EricL True while mouse is down dragging bot around
        //Botsareus 11/29/2013 Allows for moving whole organism
        private class tmppostyp
        {
            public int n = 0;
            public double x = 0;
            public double y = 0;
        }
    }
}
