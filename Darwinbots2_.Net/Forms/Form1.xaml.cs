using DarwinBots;
using DarwinBots.Modules;
using System;
using System.Collections.Generic;
using System.Windows;
using static Microsoft.VisualBasic.Constants;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.Interaction;

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

        private void Form_Load(object sender, RoutedEventArgs e)
        {
            MaxMem = 1000;
            for (var i = 0; i < maxshotarray; i++) { ShotsManager.Shots.Add(null); }
            dispskin = true;
            InitObstacles();

            MDIForm1.instance.F1Piccy.setVisible(false);
            ContestMode = false;
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

        private class tmppostyp
        {
            public int n = 0;
            public double x = 0;
            public double y = 0;
        }
    }
}
