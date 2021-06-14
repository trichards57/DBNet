using VB6 = Microsoft.VisualBasic.Compatibility.VB6;
using System.Runtime.InteropServices;
using static VBExtension;
using static VBConstants;
using Microsoft.VisualBasic;
using System;
using System.Windows;
using System.Windows.Controls;
using static System.DateTime;
using static System.Math;
using static Microsoft.VisualBasic.Globals;
using static Microsoft.VisualBasic.Collection;
using static Microsoft.VisualBasic.Constants;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.DateAndTime;
using static Microsoft.VisualBasic.ErrObject;
using static Microsoft.VisualBasic.FileSystem;
using static Microsoft.VisualBasic.Financial;
using static Microsoft.VisualBasic.Information;
using static Microsoft.VisualBasic.Interaction;
using static Microsoft.VisualBasic.Strings;
using static Microsoft.VisualBasic.VBMath;
using System.Collections.Generic;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.ColorConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.DrawStyleConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.FillStyleConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.GlobalModule;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.Printer;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.PrinterCollection;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.PrinterObjectConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.ScaleModeConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.SystemColorConstants;
using ADODB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using DBNet.Forms;
using static stringops;
using static varspecie;
using static stayontop;
using static localizzazione;
using static SimOpt;
using static Common;
using static Flex;
using static Robots;
using static Ties;
using static ShotsManager;
using static Globals;
using static Physics;
using static F1Mode;
using static DNAExecution;
using static Vegs;
using static Senses;
using static Multibots;
using static HDRoutines;
using static Scripts;
using static Database;
using static BucketManager;
using static NeoMutations;
using static Master;
using static DNAManipulations;
using static DNATokenizing;
using static Bitwise;
using static ObstaclesManager;
using static Teleport;
using static IntOpts;
using static stuffcolors;
using static Evo;
using static DBNet.Forms.MDIForm1;
using static DBNet.Forms.datirob;
using static DBNet.Forms.InfoForm;
using static DBNet.Forms.ColorForm;
using static DBNet.Forms.parentele;
using static DBNet.Forms.Consoleform;
using static DBNet.Forms.frmAbout;
using static DBNet.Forms.optionsform;
using static DBNet.Forms.NetEvent;
using static DBNet.Forms.grafico;
using static DBNet.Forms.ActivForm;
using static DBNet.Forms.Form1;
using static DBNet.Forms.Contest_Form;
using static DBNet.Forms.DNA_Help;
using static DBNet.Forms.MutationsProbability;
using static DBNet.Forms.PhysicsOptions;
using static DBNet.Forms.CostsForm;
using static DBNet.Forms.EnergyForm;
using static DBNet.Forms.ObstacleForm;
using static DBNet.Forms.TeleportForm;
using static DBNet.Forms.frmGset;
using static DBNet.Forms.frmMonitorSet;
using static DBNet.Forms.frmPBMode;
using static DBNet.Forms.frmRestriOps;
using static DBNet.Forms.frmEYE;
using static DBNet.Forms.frmFirstTimeInfo;
using Iersera.ViewModels;

namespace DBNet.Forms
{
    public partial class OptionsForm : Window
    {
        internal OptionsViewModel ViewModel { get; set; } = new OptionsViewModel();

        public OptionsForm() { InitializeComponent(); }

        private bool validate = false;
        private bool pass = false;
        private bool follow1 = false;
        private bool follow2 = false;
        private bool follow3 = false;
        private bool follow4 = false;
        private string lastsettings = "";
        private int contrmethod = 0;
        public int CurrSpec = 0;
        public int col1 = 0;
        public bool Canc = false;
        public int IPBWidth = 0;
        public int IPBHeight = 0;
        private int multx = 0;
        private int multy = 0;
        //Windows declarations
        [DllImport("user32.dll")] private static extern int SetCapture(int hWnd);
        [DllImport("user32.dll")] private static extern int ClipCursor(dynamic lpRect);
        [DllImport("user32.dll")] private static extern int ReleaseCapture();
        [DllImport("user32.dll")] private static extern int GetWindowRect(int hWnd, RECT lpRect);
        [DllImport("user32.dll")] private static extern int GetCursorPos(POINTAPI lpPoint);
        [DllImport("user32.dll")] private static extern int GetDC(int hWnd);
        [DllImport("user32.dll")] private static extern int ReleaseDC(int hWnd, int hdc);
        [DllImport("gdi32.dll")] private static extern int SelectObject(int hdc, int hObject);
        [DllImport("gdi32.dll")] private static extern int DeleteObject(int hObject);
        [DllImport("gdi32.dll")] private static extern int GetStockObject(int nIndex);
        [DllImport("gdi32.dll")] private static extern int CreatePen(int nPenStyle, int nWidth, int crColor);
        [DllImport("gdi32.dll")] private static extern int SetROP2(int hdc, int nDrawMode);
        [DllImport("gdi32.dll")] private static extern int Rectangle(int hdc, int x1, int y1, int x2, int y2);
        private class POINTAPI
        {
            public int X = 0;
            public int Y = 0;
        }
        private class RECT
        {
            public int Left = 0;
            public int Top = 0;
            public int Right = 0;
            public int Bottom = 0;
        }
        private const byte NULL_BRUSH = 5;
        private const byte PS_SOLID = 0;
        private const byte R2_NOT = 6;
        public enum ControlState
        {
            StateNothing = 0,
            StateDragging,
            StateSizing
        }
        private Window m_CurrCtl = null;
        private ControlState m_DragState = ControlState.StateNothing;
        private int m_DragHandle = 0;
        private CRect m_DragRect = new CRect();
        private POINTAPI m_DragPoint = null;
        private bool speclistchecked = false;

        // TODO : Set up the gradient display

        private void LightBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ViewModel.StartLightTimer();
        }

        private void LightBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ViewModel.StopLightTimer();
        }

        private void clearall()
        {
            int k = 0;

            k = 0;
            //  SpecList.CLEAR
            While(TmpOpts.Specie(k).Name != "");
            TmpOpts.Specie(k).Name = "";
            k = k + 1;
            Wend();

            TmpOpts.SunOnRnd = false;
            TmpOpts.Tides = 0;
            TmpOpts.TidesOf = 0;
        }

        private void SkinChange_Click(object sender, RoutedEventArgs e) { SkinChange_Click(); }
        private void SkinChange_Click()
        {
            if (SpecList.SelectedIndex >= 0)
            {
                TmpOpts.Specie(SpecList.SelectedIndex).Skin(6) = (TmpOpts.Specie(SpecList.SelectedIndex).Skin(6) + Random(0, half) * 2) / 3;
                ShowSkin(SpecList.SelectedIndex);
            }
        }

        private void ShowSkin(int k)
        {
            int t = 0;

            int X = 0;

            int Y = 0;

            X = Cerchio.Left + Cerchio.Width / 2;
            Y = Cerchio.Top + Cerchio.Height / 2;
            multx = Cerchio.Width / 120;
            multy = Cerchio.Height / 120;
            this
          Line7.x1 = TmpOpts.Specie(k).Skin(0) * multx * Cos(TmpOpts.Specie(k).Skin(1) / 100) + X;
            Line7.y1 = TmpOpts.Specie(k).Skin(0) * multy * Sin(TmpOpts.Specie(k).Skin(1) / 100) + Y;
            Line7.x2 = TmpOpts.Specie(k).Skin(2) * multx * Cos(TmpOpts.Specie(k).Skin(3) / 100) + X;
            Line7.y2 = TmpOpts.Specie(k).Skin(2) * multy * Sin(TmpOpts.Specie(k).Skin(3) / 100) + Y;
            Line8.x1 = Line7.x2;
            Line8.y1 = Line7.y2;
            Line8.x2 = TmpOpts.Specie(k).Skin(4) * multx * Cos(TmpOpts.Specie(k).Skin(5) / 100) + X;
            Line8.y2 = TmpOpts.Specie(k).Skin(4) * multy * Sin(TmpOpts.Specie(k).Skin(5) / 100) + Y;
            Line9.x1 = Line8.x2;
            Line9.y1 = Line8.y2;
            Line9.x2 = TmpOpts.Specie(k).Skin(6) * multx * Cos(TmpOpts.Specie(k).Skin(7) / 100) + X;
            Line9.y2 = TmpOpts.Specie(k).Skin(6) * multy * Sin(TmpOpts.Specie(k).Skin(7) / 100) + Y;
        }
       
        private void MutRatesBut_Click(object sender, RoutedEventArgs e) { MutRatesBut_Click(); }
        private void MutRatesBut_Click()
        {
            //EricL 4/9/2006 Catches crash when no species is selected
            if (OptionsForm.instance.CurrSpec < 0)
            {
                MsgBox(("Please select a species."));
            }
            else
            {
                MutationsProbability.Show(vbModal);
            }
        }

        public void SwapSpecies(int a, int b)
        {
            Species c = null;


            c = TmpOpts.Specie(a);
            TmpOpts.Specie(a) = TmpOpts.Specie(b);
            TmpOpts.Specie(b) = c;

        }

        public void SortSpecies()
        {
            int i = 0;

            int j = 0;


            for (i = 0; i < TmpOpts.SpeciesNum - 2; i++)
            {
                for (j = i + 1; j < TmpOpts.SpeciesNum - 1; j++)
                {
                    if (UCase(TmpOpts.Specie(i).Name) > UCase(TmpOpts.Specie(j).Name))
                    {
                        SwapSpecies(i, j);
                    }
                }
            }

            //Botsareus 2/23/2013 Remove nonnative species
            for (i = 0; i < TmpOpts.SpeciesNum - 1; i++)
            {
                if (TmpOpts.Specie(i).Native == false && TmpOpts.Specie(i).Name != "")
                {
                    for (j = i; j < TmpOpts.SpeciesNum - 1; j++)
                    {
                        TmpOpts.Specie(j) = TmpOpts.Specie(j + 1);
                    }
                    i = i - 1;
                }
            }

            j = TmpOpts.SpeciesNum - 1;
            for (i = 0; i < j; i++)
            {
                if (TmpOpts.Specie(i).Native == false)
                {
                    TmpOpts.SpeciesNum = TmpOpts.SpeciesNum - 1;
                }
            }

        }

        public void datatolist()
        { //datatolist
            int i = 0;

            SpecList.CLEAR();

            SortSpecies();

            for (i = 0; i < TmpOpts.SpeciesNum - 1; i++)
            {
                SpecList.additem((TmpOpts.Specie(i).Name));
                ExtractComment(TmpOpts.Specie(i).path + "\\" + TmpOpts.Specie(i).Name, i);
            }


        }

        private void enprop()
        {
            Frame1.IsEnabled = true;
        }

        private void disprop()
        {
            Frame1.IsEnabled = false;
        }

        private void IndNum_Click(object sender, RoutedEventArgs e) { IndNum_Click(); }
        private void IndNum_Click(int Index)
        {
            int qty = 0;

            switch (Index)
            {
                case 0:
                    qty = 5;
                    break;
                case 1:
                    qty = 10;
                    break;
                case 2:
                    qty = 15;
                    break;
                case 3:
                    qty = 30;
                    break;
                case 4:
                    qty = 0;
                    break;
            }
            SpecQty.text = qty;
        }

        private void SpecQty_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { SpecQty_Change(); }
        private void SpecQty_Change()
        {
            if (CurrSpec > -1)
            {
                TmpOpts.Specie(CurrSpec).qty = val(SpecQty.text);
            }
        }

        private void InNrg_Click(object sender, RoutedEventArgs e) { InNrg_Click(); }
        private void InNrg_Click(int Index)
        {
            int qty = 0;

            switch (Index)
            {
                case 0:
                    qty = 3000;
                    break;
                case 1:
                    qty = 4000;
                    break;
                case 2:
                    qty = 5000;
                    break;
                case 3:
                    qty = 30000;
                    break;
            }
            SpecNrg.text = qty;
        }

        private void SpecNrg_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { SpecNrg_Change(); }
        private void SpecNrg_Change()
        {
            if (CurrSpec >= 0)
            {
                TmpOpts.Specie(CurrSpec).Stnrg = val(SpecNrg.text) % 32000;
            }
        }

        private void SpecVeg_Click(object sender, RoutedEventArgs e) { SpecVeg_Click(); }
        private void SpecVeg_Click()
        {
            if (CurrSpec > -1)
            {
                TmpOpts.Specie(CurrSpec).Veg = false;
                if (SpecVeg.value == 1)
                {
                    TmpOpts.Specie(CurrSpec).Veg = true;
                    TmpOpts.Specie(CurrSpec).NoChlr = false;
                    chkNoChlr.value = 0;
                }
            }
        }

        private void BlockSpec_Click(object sender, RoutedEventArgs e) { BlockSpec_Click(); }
        private void BlockSpec_Click()
        {
            if (CurrSpec > -1)
            {
                TmpOpts.Specie(CurrSpec).Fixed = false;
                if (BlockSpec.value == 1)
                {
                    TmpOpts.Specie(CurrSpec).Fixed = true;
                }
            }
        }

        private void SpecCol_click()
        {
            if (CurrSpec == -1)
            {
                return;//Botsareus 2/3/2013 bug fix when no robot selected

            }

            string c = "";

            decimal r = 0;

            decimal g = 0;

            decimal b = 0;

            int k = 0;

            int col = 0;


            c = SpecCol.text;
            k = CurrSpec;
            if (speclistchecked)
            {
                goto bypass;
            }
            switch (c)
            {
                case "Red":
                    r = 200;
                    g = 60;
                    b = 60;
                    break;
                case "Blue":
                    r = 60;
                    g = 60;
                    b = 200;
                    break;
                case "Green":
                    r = 60;
                    g = 200;
                    b = 60;
                    break;
                case "Yellow":
                    r = 220;
                    g = 220;
                    b = 60;
                    break;
                case "Pink":
                    r = 210;
                    g = 160;
                    b = 210;
                    break;
                case "Brown":
                    r = 140;
                    g = 110;
                    b = 70;
                    break;
                case "Purple":
                    r = 150;
                    g = 56;
                    b = 180;
                    break;
                case "Orange":
                    r = 220;
                    g = 150; // EricL
                    b = 60;
                    break;
                case "Cyan":
                    r = 58;
                    g = 207;
                    b = 228;
                    break;//Botsareus 4/27/2013 This was very broken
                case "Random":
                    Randomize();
                    r = IIf(UseOldColor, Rnd * 200 + 55, Rnd * 255);
                    g = IIf(UseOldColor, Rnd * 200 + 55, Rnd * 255);
                    b = Rnd * 255;
                    break;
                case "Custom":
                    col = TmpOpts.Specie(k).color;
                    MakeColor(col, respath(TmpOpts.Specie(k).path) + "\\" + TmpOpts.Specie(k).Name);
                    if (colorform.instance.SelectColor)
                    {
                        TmpOpts.Specie(k).color = colorform.instance.color;
                    }
                    else
                    {
                        TmpOpts.Specie(k).color = colorform.instance.OldColor;
                    }
                    TmpOpts.Specie(k).Colind = SpecCol.SelectedIndex;
                    goto ;
                    break;
            }
            TmpOpts.Specie(k).color = (65536 * b) + (256 * g) + r;
            TmpOpts.Specie(k).Colind = SpecCol.SelectedIndex;
        bypass:
            Cerchio.FillColor = TmpOpts.Specie(k).color;
            Cerchio.BorderColor = TmpOpts.Specie(k).color;
            Line7.BorderColor = TmpOpts.Specie(k).color;
            Line8.BorderColor = TmpOpts.Specie(k).color;
            Line9.BorderColor = TmpOpts.Specie(k).color;
        }

        private void MakeColor(int col, string path)
        {
            colorform.instance.color = col;
            colorform.instance.SelectColor = false;
            colorform.instance.path = path;
            ColorForm.Show(vbModal);
        }

        /*
        ''''''''''''''''''''''''''''''''''''
        ' Position control '''''''''''''''''
        ''''''''''''''''''''''''''''''''''''
        '=========================== Sample controls ===========================
        'To drag a control, simply call the DragBegin function with
        'the control to be dragged
        '=======================================================================
        */
        private void PosReset_Click(object sender, RoutedEventArgs e) { PosReset_Click(); }
        private void PosReset_Click()
        {
            DragEnd();

            Initial_Position.Left = 0;
            Initial_Position.Top = 0;

            Initial_Position.Width = IPB.Width;
            Initial_Position.Height = IPB.Height;

            //EricL Hitting the reset button is sufficient to set the bots starting position
            //without having to click on the Initial Position control
            //These are percentages of the field width and height
            if (CurrSpec > -1)
            {
                TmpOpts.Specie(CurrSpec).Posrg = 1;
                TmpOpts.Specie(CurrSpec).Posdn = 1;
                TmpOpts.Specie(CurrSpec).Poslf = 0;
                TmpOpts.Specie(CurrSpec).Postp = 0;
            }

        }

        private void Initial_Position_MouseDown(int Button, int Shift_UNUSED, decimal X_UNUSED, decimal Y_UNUSED)
        {
            if (Button == vbLeftButton)
            {
                DragBegin(Initial_Position.Source);
                PaintObstacles();
            }
        }

        private void IPB_MouseDown(int Button, int Shift_UNUSED, decimal X_UNUSED, decimal Y_UNUSED)
        {
            if (Button == vbLeftButton)
            {
                DragBegin(Initial_Position.Source);
                PaintObstacles();
            }
        }

        /*
        '========================== Dragging Code ================================

        'Initialization -- Do not call more than once
        */
        public void DragInit()
        {
            int i = 0;
            decimal xHandle = 0;
            decimal yHandle = 0;


            //Use black Picture box controls for 8 sizing handles
            //Calculate size of each handle
            xHandle = 8 * Screen.TwipsPerPixelX;
            yHandle = 8 * Screen.TwipsPerPixelY;
            //Load array of handles until we have 8
            for (i = 0; i < 7; i++)
            {
                if (i != 0)
                {
                    Load(picHandle(i));
                }
                picHandle(i).Width = xHandle;
                picHandle(i).Height = yHandle;
                //Must be in front of other controls
                picHandle(i).ZOrder();
            }

            for (i = 0; i < 3; i++)
            {
                if (i != 0)
                {
                    Load(RobPlacLine(i));
                    RobPlacLine(i) = RobPlacLine(0).Source;
                }
                //Must be in front of other controls
                RobPlacLine(i).ZOrder();
            }
            //Set mousepointers for each sizing handle
            picHandle(0).MousePointer = vbSizeNWSE;
            picHandle(2).MousePointer = vbSizeNESW;
            picHandle(4).MousePointer = vbSizeNWSE;
            picHandle(6).MousePointer = vbSizeNESW;

            RobPlacLine(0).MousePointer = vbSizeNS;
            RobPlacLine(1).MousePointer = vbSizeNS;

            RobPlacLine(2).MousePointer = vbSizeWE;
            RobPlacLine(3).MousePointer = vbSizeWE;

            //Initialize current control
            m_CurrCtl = null;
        }

        /*
        'Drags the specified control
        */
        public void DragBegin(Window ctl)
        {
            RECT rc = null;


            //Hide any visible handles
            ShowHandles(false);

            //Save reference to control being dragged
            m_CurrCtl = ctl;

            //Store initial mouse position
            GetCursorPos(m_DragPoint);

            //Save control position (in screen coordinates)
            //Note: control might not have a window handle
            m_DragRect.SetRectToCtrl(m_CurrCtl, IPB.Source, Frame1.Content, SSTab1.DefaultProperty);
            m_DragRect.TwipsToScreen(m_CurrCtl);

            //Make initial mouse position relative to control
            m_DragPoint.X = m_DragPoint.X - m_DragRect.Left;
            m_DragPoint.Y = m_DragPoint.Y - m_DragRect.Top;

            //Force redraw of form without sizing handles
            //before drawing dragging rectangle
            Refresh();

            //Show dragging rectangle
            DrawDragRect();

            //Indicate dragging under way
            m_DragState = ControlState.StateDragging;

            //In order to detect mouse movement over any part of the form,
            //we set the mouse capture to the form and will process mouse
            //movement from the applicable form events
            ReleaseCapture(); //This appears needed before calling SetCapture
            SetCapture(hWnd);

            //Limit cursor movement within form
            GetWindowRect(hWnd, rc);
            ClipCursor(rc);
        }

        /*
        'Clears any current drag mode and hides sizing handles
        */
        public void DragEnd()
        {
            m_CurrCtl = null;
            ShowHandles(false);
            m_DragState = ControlState.StateNothing;
        }

        /*
        'To handle all mouse message anywhere on the form, we set the mouse
        'capture to the form. Mouse movement is processed here
        */
        private void Form_MouseMove(int Button_UNUSED, int Shift_UNUSED, decimal X, decimal Y)
        {
            if (CurrSpec == -1)
            {
                return;//Botsareus 2/3/2013 bug fix when no robot selected

            }

            decimal nWidth = 0;
            decimal nHeight = 0;

            POINTAPI pt = null;


            if (m_DragState == ControlState.StateDragging)
            {
                //Save dimensions before modifying rectangle
                nWidth = m_DragRect.Right - m_DragRect.Left;
                nHeight = m_DragRect.Bottom - m_DragRect.Top;
                //Get current mouse position in screen coordinates
                GetCursorPos(pt);
                //Hide existing rectangle
                DrawDragRect();
                //Update drag rectangle coordinates
                m_DragRect.Left = pt.X - m_DragPoint.X;
                m_DragRect.Top = pt.Y - m_DragPoint.Y;
                m_DragRect.Right = m_DragRect.Left + nWidth;
                m_DragRect.Bottom = m_DragRect.Top + nHeight;
                //Draw new rectangle
                DrawDragRect();
            }
            else if (m_DragState == ControlState.StateSizing)
            {
                //Get current mouse position in screen coordinates
                GetCursorPos(pt);
                //Hide existing rectangle
                DrawDragRect();
                //Action depends on handle being dragged
                switch (m_DragHandle)
                {
                    case 0:
                        m_DragRect.Left = pt.X;
                        m_DragRect.Top = pt.Y;
                        break;
                    case 2:
                        m_DragRect.Right = pt.X;
                        m_DragRect.Top = pt.Y;
                        break;
                    case 4:
                        m_DragRect.Right = pt.X;
                        m_DragRect.Bottom = pt.Y;
                        break;
                    case 6:
                        m_DragRect.Left = pt.X;
                        m_DragRect.Bottom = pt.Y;
                        break;
                    case 9:
                        m_DragRect.Top = pt.Y;
                        break;
                    case 10:
                        m_DragRect.Bottom = pt.Y;
                        break;
                    case 11:
                        m_DragRect.Left = pt.X;
                        break;
                    case 12:
                        m_DragRect.Right = pt.X;
                        break;
                }
                //Draw new rectangle
                DrawDragRect();
            }
        }

        /*
        'To handle all mouse message anywhere on the form, we set the mouse
        'capture to the form. Mouse up is processed here
        */
        private void Form_MouseUp(int Button, int Shift_UNUSED, decimal X_UNUSED, decimal Y_UNUSED)
        {
            if (CurrSpec == -1)
            {
                return;//Botsareus 2/3/2013 bug fix when no robot selected

            }

            if (Button == vbLeftButton)
            {
                if (m_DragState == ControlState.StateDragging || m_DragState == ControlState.StateSizing)
                {
                    //Hide drag rectangle
                    DrawDragRect();
                    //Move control to new location
                    m_DragRect.ScreenToTwips(m_CurrCtl);
                    m_DragRect.SetCtrlToRect(m_CurrCtl, IPB.Source, Frame1.Content, SSTab1.DefaultProperty);
                    //Restore sizing handles
                    ShowHandles(true);
                    //Free mouse movement
                    ClipCursor(ByVal(0 &));
                    //Release mouse capture
                    ReleaseCapture();
                    //Reset drag state
                    m_DragState = ControlState.StateNothing;

                    int w = 0;
                    int h = 0;


                    w = IPB.Width;
                    h = IPB.Height;

                    TmpOpts.Specie(CurrSpec).Posrg = (Initial_Position.Left + Initial_Position.Width) / w;
                    TmpOpts.Specie(CurrSpec).Posdn = (Initial_Position.Top + Initial_Position.Height) / h;
                    TmpOpts.Specie(CurrSpec).Poslf = (Initial_Position.Left) / w;
                    TmpOpts.Specie(CurrSpec).Postp = (Initial_Position.Top) / h;

                    if ((TmpOpts.Specie(CurrSpec).Posrg > 1))
                    {
                        TmpOpts.Specie(CurrSpec).Posrg = 1;
                    }
                    if ((TmpOpts.Specie(CurrSpec).Posrg < 0))
                    {
                        TmpOpts.Specie(CurrSpec).Posrg = 0;
                    }

                    if ((TmpOpts.Specie(CurrSpec).Posdn > 1))
                    {
                        TmpOpts.Specie(CurrSpec).Posdn = 1;
                    }
                    if ((TmpOpts.Specie(CurrSpec).Posdn < 0))
                    {
                        TmpOpts.Specie(CurrSpec).Posdn = 0;
                    }

                    if ((TmpOpts.Specie(CurrSpec).Poslf > 1))
                    {
                        TmpOpts.Specie(CurrSpec).Poslf = 1;
                    }
                    if ((TmpOpts.Specie(CurrSpec).Poslf < 0))
                    {
                        TmpOpts.Specie(CurrSpec).Poslf = 0;
                    }

                    if ((TmpOpts.Specie(CurrSpec).Postp > 1))
                    {
                        TmpOpts.Specie(CurrSpec).Postp = 1;
                    }
                    if ((TmpOpts.Specie(CurrSpec).Postp < 0))
                    {
                        TmpOpts.Specie(CurrSpec).Postp = 0;
                    }
                }
            }
        }

        /*
        'Process MouseDown over handles
        */
        private void picHandle_MouseDown(int Index, int Button_UNUSED, int Shift_UNUSED, decimal X_UNUSED, decimal Y_UNUSED)
        {
            int i = 0;

            RECT rc = null;


            //Handles should only be visible when a control is selected
            if (picHandle(Index).Visible == false)
            {
                return;

            }

            //NOTE: m_DragPoint not used for sizing
            //Save control position in screen coordinates
            m_DragRect.SetRectToCtrl(m_CurrCtl, IPB.Source, Frame1.Content, SSTab1.DefaultProperty);
            m_DragRect.TwipsToScreen(m_CurrCtl);
            //Track index handle
            m_DragHandle = Index;
            //Hide sizing handles
            ShowHandles(false);
            //We need to force handles to hide themselves before drawing drag rectangle
            Refresh();
            //Indicate sizing is under way
            m_DragState = ControlState.StateSizing;
            //Show sizing rectangle
            DrawDragRect();
            //In order to detect mouse movement over any part of the form,
            //we set the mouse capture to the form and will process mouse
            //movement from the applicable form events
            SetCapture(hWnd);
            //Limit cursor movement within form
            GetWindowRect(hWnd, rc);
            ClipCursor(rc);
        }

        private void Robplacline_MouseDown(int Index, int Button_UNUSED, int Shift_UNUSED, decimal X_UNUSED, decimal Y_UNUSED)
        {
            int i = 0;

            RECT rc = null;


            //Handles should only be visible when a control is selected
            if (RobPlacLine(Index).Visible == false)
            {
                return;

            }

            //NOTE: m_DragPoint not used for sizing
            //Save control position in screen coordinates
            m_DragRect.SetRectToCtrl(m_CurrCtl, IPB.Source, Frame1.Content, SSTab1.DefaultProperty);
            m_DragRect.TwipsToScreen(m_CurrCtl);
            //Track index handle
            m_DragHandle = Index + 9;
            //Hide sizing handles
            ShowHandles(false);
            //We need to force handles to hide themselves before drawing drag rectangle
            Refresh();
            //Indicate sizing is under way
            m_DragState = ControlState.StateSizing;
            //Show sizing rectangle
            DrawDragRect();
            //In order to detect mouse movement over any part of the form,
            //we set the mouse capture to the form and will process mouse
            //movement from the applicable form events
            SetCapture(hWnd);
            //Limit cursor movement within form
            GetWindowRect(hWnd, rc);
            ClipCursor(rc);
        }

        /*
        'Display or hide the sizing handles and arrange them for the current rectangld
        */
        private void ShowHandles(bool bShowHandles)
        {
            //int i = 0;

            //int xFudge = 0;
            //int yFudge = 0;


            //bool ShowHandlesBool = false;


            //ShowHandlesBool = true;

            //if (bShowHandles && !m_CurrCtl == null)
            //{
            //    dynamic _WithVar_871;
            //    _WithVar_871 = m_DragRect;
            //    if (!(_WithVar_871.Width < 250 || _WithVar_871.Height < 250))
            //    {
            //        //Save some calculations in variables for speed
            //        xFudge = (2 * Screen.TwipsPerPixelX);
            //        yFudge = (2 * Screen.TwipsPerPixelY);

            //        //Top Left
            //        picHandle(0).Move(_WithVar_871.Left - IPB.Left - Frame1.Left - SSTab1.Left) + xFudge,(_WithVar_871.Top - IPB.Top - Frame1.Top) + yFudge - SSTab1.Top;
            //        //Top right
            //        picHandle(2).Move _WithVar_871.Left - IPB.Left - Frame1.Left - SSTab1.Left + _WithVar_871.Width - picHandle(0).Width - xFudge, _WithVar_871.Top - IPB.Top - Frame1.Top + yFudge - SSTab1.Top;
            //        //Bottom left
            //        picHandle(6).Move _WithVar_871.Left - IPB.Left - Frame1.Left - SSTab1.Left + xFudge, _WithVar_871.Top + _WithVar_871.Height - picHandle(0).Height - yFudge - IPB.Top - Frame1.Top - SSTab1.Top;
            //        //Bottom right
            //        picHandle(4).Move(_WithVar_871.Left - IPB.Left - Frame1.Left - SSTab1.Left + _WithVar_871.Width) - picHandle(0).Width - xFudge, _WithVar_871.Top + _WithVar_871.Height - picHandle(0).Height - yFudge - IPB.Top - Frame1.Top - SSTab1.Top;
            //    }
            //    else
            //    {
            //        ShowHandlesBool = false;
            //    }

            //    RobPlacLine(0).Move _WithVar_871.Left - IPB.Left - Frame1.Left - SSTab1.Left, _WithVar_871.Top - IPB.Top - Frame1.Top - SSTab1.Top, _WithVar_871.Width, 60;
            //    RobPlacLine(1).Move _WithVar_871.Left - IPB.Left - Frame1.Left - SSTab1.Left, _WithVar_871.Top + _WithVar_871.Height - IPB.Top - Frame1.Top - SSTab1.Top - 60, _WithVar_871.Width, 60;

            //    RobPlacLine(2).Move _WithVar_871.Left - IPB.Left - Frame1.Left - SSTab1.Left, _WithVar_871.Top - IPB.Top - Frame1.Top - SSTab1.Top, 60, _WithVar_871.Height;
            //    RobPlacLine(3).Move _WithVar_871.Left + _WithVar_871.Width - IPB.Left - Frame1.Left - SSTab1.Left - 60, _WithVar_871.Top - IPB.Top - Frame1.Top - SSTab1.Top, 60, _WithVar_871.Height;
            //}
            ////Show or hide each handle
            //picHandle(0).Visibility = bShowHandles && ShowHandlesBool;
            //picHandle(2).Visibility = bShowHandles && ShowHandlesBool;
            //picHandle(6).Visibility = bShowHandles && ShowHandlesBool;
            //picHandle(4).Visibility = bShowHandles && ShowHandlesBool;

            //RobPlacLine(0).Visibility = bShowHandles;
            //RobPlacLine(1).Visibility = bShowHandles;
            //RobPlacLine(2).Visibility = bShowHandles;
            //RobPlacLine(3).Visibility = bShowHandles;
        }

        /*
        'Draw drag rectangle. The API is used for efficiency and also
        'because drag rectangle must be drawn on the screen DC in
        'order to appear on top of all controls
        */
        private void DrawDragRect()
        {
            int hPen = 0;
            int hOldPen = 0;

            int hBrush = 0;
            int hOldBrush = 0;

            int hScreenDC = 0;
            int nDrawMode = 0;


            //Get DC of entire screen in order to
            //draw on top of all controls
            hScreenDC = GetDC(0);
            //Select GDI object
            hPen = CreatePen(PS_SOLID, 2, 0);
            hOldPen = SelectObject(hScreenDC, hPen);
            hBrush = GetStockObject(NULL_BRUSH);
            hOldBrush = SelectObject(hScreenDC, hBrush);
            nDrawMode = SetROP2(hScreenDC, R2_NOT);
            //Draw rectangle
            Rectangle(hScreenDC, m_DragRect.Left, m_DragRect.Top, m_DragRect.Right, m_DragRect.Bottom);
            //Restore DC
            SetROP2(hScreenDC, nDrawMode);
            SelectObject(hScreenDC, hOldBrush);
            SelectObject(hScreenDC, hOldPen);
            ReleaseDC(0, hScreenDC);
            //Delete GDI objects
            DeleteObject(hPen);
        }

        /*
        ''''''''''''''''''''''''''''''''''''''''''''

        '''General Panel'''''''''''''''''''''''''''''

        '''''''''''''''''''''''''''''''''''''''''''''

        '''''Start dimensions control
        */
        private void FieldSizeSlide_Scroll()
        {
            int t = 0;

            decimal oldsw = 0;
            decimal oldsh = 0;


            TmpOpts.FieldSize = FieldSizeSlide.value;

            oldsw = TmpOpts.FieldWidth;
            oldsh = TmpOpts.FieldHeight;

            if (TmpOpts.FieldSize == 1)
            { //F1 mode
                TmpOpts.FieldWidth = 9237;
                TmpOpts.FieldHeight = 6928;
            }
            else
            {
                TmpOpts.FieldWidth = 8000;
                TmpOpts.FieldHeight = 6000;

                if (TmpOpts.FieldSize <= 12)
                {
                    TmpOpts.FieldWidth = TmpOpts.FieldWidth * TmpOpts.FieldSize;
                    TmpOpts.FieldHeight = TmpOpts.FieldHeight * TmpOpts.FieldSize;
                }
                else
                { // Field sizes larger than size 12 get big fast...
                    TmpOpts.FieldWidth = (TmpOpts.FieldWidth * 12) * (TmpOpts.FieldSize - 12) * 2;
                    TmpOpts.FieldHeight = (TmpOpts.FieldHeight * 12) * (TmpOpts.FieldSize - 12) * 2;
                }
            }

            FWidthLab.Content = TmpOpts.FieldWidth;
            FHeightLab.Content = TmpOpts.FieldHeight;
        }

        /*
        '''''End Dimensions Control

        '''''Start Seeded Random Control

        'Private Sub UserSeed_Click() 'Botsareus 5/3/2013 replaced by safemode
        '  TmpOpts.UserSeedToggle = UserSeed.value * True
        '  UserSeedText.Enabled = UserSeed.value * True
        'End Sub
        */
        private void UserSeedText_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { UserSeedText_Change(); }
        private void UserSeedText_Change()
        {
            TmpOpts.UserSeedNumber = val(UserSeedText.text);
            SimOpts.UserSeedNumber = val(UserSeedText.text);
        }

        /*
        ''''''End Seeded Random Control

        ''''''Start Torroidal, etc.
        */
        private void TopDownCheck_Click(object sender, RoutedEventArgs e) { TopDownCheck_Click(); }
        private void TopDownCheck_Click()
        {
            TmpOpts.Updnconnected = TopDownCheck.value * true;
            TmpOpts.Toroidal = TopDownCheck.value * true && RightLeftCheck.value * true;
        }

        private void RightLeftCheck_Click(object sender, RoutedEventArgs e) { RightLeftCheck_Click(); }
        private void RightLeftCheck_Click()
        {
            TmpOpts.Dxsxconnected = RightLeftCheck.value * true;
            TmpOpts.Toroidal = TopDownCheck.value * true && RightLeftCheck.value * true;
        }

        /*
        '''''End Torroidal, etc.

        ''''''Waste Control
        */
        private void CustomWaste_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { CustomWaste_Change(); }
        private void CustomWaste_Change()
        {
            if (val(CustomWaste.text) == 0)
            {
                TmpOpts.BadWastelevel = -1;
            }
            else
            {
                TmpOpts.BadWastelevel = val(CustomWaste.text);
            }
        }

        private void VirusImmuneCheck_Click(object sender, RoutedEventArgs e) { VirusImmuneCheck_Click(); }
        private void VirusImmuneCheck_Click()
        {
            if (CurrSpec > -1)
            {
                TmpOpts.Specie(CurrSpec).VirusImmune = false;
                if (VirusImmuneCheck.value == 1)
                {
                    TmpOpts.Specie(CurrSpec).VirusImmune = true;
                }
            }
        }

        private void WasteThresholdUpDown_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { WasteThresholdUpDown_Change(); }
        private void WasteThresholdUpDown_Change()
        {
            //Buddied to CustomWaste, and synchronized,
            //so we don't have to do anything here
        }

        /*
        '''''''End Waste Control

        '''''''Start Misc
        'Private Sub DisableTiesCheck_Click()
        'TmpOpts.DisableTies = DisableTiesCheck_Click.value * True
        'End Sub
        '''''''End Misc

        '''''''Start Corpse Controls
        */
        private void CorpseCheck_Click(object sender, RoutedEventArgs e) { CorpseCheck_Click(); }
        private void CorpseCheck_Click()
        {
            TmpOpts.CorpseEnabled = CorpseCheck.value * true;
            DecayText.IsEnabled = CorpseCheck.value * true;
            DecayUpDn(0).IsEnabled = CorpseCheck.value * true;
            DecayUpDn(1).IsEnabled = CorpseCheck.value * true;
            FrequencyText.IsEnabled = CorpseCheck.value * true;
            DecayOption(0).IsEnabled = CorpseCheck.value * true;
            DecayOption(1).IsEnabled = CorpseCheck.value * true;
            DecayOption(2).IsEnabled = CorpseCheck.value * true;
        }

        private void DecayOption_Click(object sender, RoutedEventArgs e) { DecayOption_Click(); }
        private void DecayOption_Click(int Index)
        {
            TmpOpts.DecayType = Index + 1;
        }

        private void DecayText_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { DecayText_Change(); }
        private void DecayText_Change()
        {
            TmpOpts.Decay = val(DecayText.text);
        }

        private void FrequencyText_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { FrequencyText_Change(); }
        private void FrequencyText_Change()
        {
            TmpOpts.Decaydelay = val(FrequencyText.text);
        }

        private void DecayUpDn_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { DecayUpDn_Change(); }
        private void DecayUpDn_Change(int Index_UNUSED)
        {
            //Buddied to DecayText amd Frequency Text, so do nothing.
        }

        /*
        '''''End Corpse Controls

        '''''Start Veg Controls

        '''StartPond Mode
        */
        private void Pondcheck_Click(object sender, RoutedEventArgs e) { Pondcheck_Click(); }
        private void Pondcheck_Click()
        {
            TmpOpts.Pondmode = Pondcheck.value * true;
            LightText.IsEnabled = Pondcheck.value * true;
            LightUpDn.IsEnabled = Pondcheck.value * true;
            Gradient.IsEnabled = Pondcheck.value * true;
            GradientUpDn.IsEnabled = Pondcheck.value * true;
            GradientLabel.IsEnabled = Pondcheck.value * true;
            Frame30.Visibility = !(Pondcheck.value) * true; //Botsareus 8/23/2014 A little mod here to make UI less confusing

            if (validate)
            {
                return;

            }

            if (Pondcheck.value == 1)
            {
                validate = true;

                if (MsgBox("Turning on Pond Mode will greatly alter the physics of your simulation. Most importantly, in Pond Mode the light providing energy to bots with chloroplasts comes from the top and gradually disappears toward the bottom. Also, Pond Mode includes Earth-like gravity and top/down sides of the screen will not wrap-around. Are you sure?", vbExclamation | vbYesNo, "Darwinbots Settings") == vbNo)
                {
                    Pondcheck.value = 0;
                    goto ;
                }


                TmpOpts.Updnconnected = false;
                TmpOpts.Ygravity = 6.2m;
                TmpOpts.LightIntensity = 100;
                TmpOpts.Gradient = 2;
                DispSettings();

            fin:

                validate = false;

            }


        }

        private void LightText_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { LightText_Change(); }
        private void LightText_Change()
        {
            //Botsareus 5/18/2013 Overflow prevention
            if (val(LightText.text) > 1000)
            {
                LightText.text = 1000;
            }

            TmpOpts.LightIntensity = val(LightText.text);
            color_lightlines();
        }

        private void LightUpDn_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { LightUpDn_Change(); }
        private void LightUpDn_Change()
        {
            //Buddied to LightText, and synchronized, so we don't have to do anything here
        }

        private void Gradient_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { Gradient_Change(); }
        private void Gradient_Change()
        {
            //Botsareus 5/18/2013 Overflow prevention
            if (val(Gradient.text) > 200)
            {
                Gradient.text = 200;
            }

            TmpOpts.Gradient = val(Gradient.text) / 10 + 1;
            color_lightlines();
        }

        private void GradientUpDn_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { GradientUpDn_Change(); }
        private void GradientUpDn_Change()
        {
            decimal a = 0;

            a = GradientUpDn.value;
            Gradient.text = (a / 5);
            //TmpOpts.Gradient = a / 5 Botsareus 12/12/2012 No need to store gradient here, use text conversion
            //color_lightlines Botsareus 12/12/2012 No need for checking light mods twise
        }

        private void EnergyScalingFactor_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { EnergyScalingFactor_Change(); }
        private void EnergyScalingFactor_Change()
        {
            //this just controls the little graph to the left of this control,
            //doesn't effect the simulation
            if (val(EnergyScalingFactor.text) == 0)
            {
                EnergyScalingFactor.text = "1";
            }
            Gradient_Change(); //Botsareus 12/12/2012 Update the graph by updating the gradient
        }

        private void color_lightlines()
        {
            int Index = 0;

            decimal fxn = 0;

            decimal depth = 0;
            decimal depth2 = 0;

            decimal greyform = 0;

            decimal toplevel = 0;

            decimal Gradient = 0;

            int color = 0;


            Gradient.Text = TmpOpts.Gradient;

            depth = 1.1m;
            if (EnergyScalingFactor.Text == 0)
            {
                EnergyScalingFactor.Text = 1;
            }
            toplevel = 2 * (val(EnergyScalingFactor.text) / (depth ^ Gradient.Text));

            depth = TmpOpts.FieldHeight / 16 / 2000 + 1;
            fxn = 2 * (TmpOpts.LightIntensity / (depth ^ Gradient.Text)) / toplevel;
            greyform = Convert_PercentageColor(Ceil(fxn, 1));
            greyform = Abs(greyform);
            color = (65536 * Ceil(greyform, 255)) + (256 * Ceil(greyform, 255));
            LightStrata(Index).BorderColor = color;

            depth = 1;
            fxn = 2 * (TmpOpts.LightIntensity / (depth ^ Gradient.Text)) / toplevel;
            greyform = Convert_PercentageColor(fxn);
            greyform = Abs(greyform);
            color = (65536 * Ceil(greyform, 255)) + (256 * Ceil(greyform, 255));
            LightStrata(15).BorderColor = color;

            for (Index = 1; Index < 14; Index++)
            {
                depth = (Index * TmpOpts.FieldHeight / 16) / 2000 + 1;
                depth2 = ((Index + 1) * TmpOpts.FieldHeight / 16 / 2000) + 1;
                fxn = 2 * (TmpOpts.LightIntensity / (depth ^ Gradient.Text)) / toplevel;
                fxn = fxn + 2 * (TmpOpts.LightIntensity / (depth2 ^ Gradient.Text)) / toplevel;
                fxn = Ceil(fxn / 2, 1);
                greyform = Convert_PercentageColor(fxn);
                greyform = Abs(greyform);
                LightStrata(Index).BorderColor = (65536 * greyform) + (256 * greyform);
            }
            Frame17.setVisible(false);
            Frame17.setVisible(true);
        }

        private int Convert_PercentageColor(decimal sgl)
        {
            int Convert_PercentageColor = 0;
            decimal temp = 0;


            temp = 255 * sgl;
            if (temp > 255)
            {
                temp = 255;
            }

            Convert_PercentageColor = temp;
            return Convert_PercentageColor;
        }

        private decimal Ceil(decimal a, decimal b)
        {
            decimal Ceil = 0;
            if (a > b)
            {
                a = b;
            }
            Ceil = a;
            return Ceil;
        }

        /*
        '''''''''''''''''''''''''''''''''''''''

        '''End Pond Mode

        '''Start Rest of Veg controls

        '''''''''''''''''''''''''''''''''''''''
        */
        private void MaxPopText_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { MaxPopText_Change(); }
        private void MaxPopText_Change()
        {
            TmpOpts.MaxPopulation = val(MaxPopText.text) % 32000;
        }

        private void MaxPopUpDn_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { MaxPopUpDn_Change(); }
        private void MaxPopUpDn_Change()
        {
            //Buddied to MaxPopText, and synchronized, so we
            //do nothing
        }

        private void MinVegText_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { MinVegText_Change(); }
        private void MinVegText_Change()
        {
            TmpOpts.MinVegs = val(MinVegText.text) % 32000;
        }

        private void MinVegUpDn_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { MinVegUpDn_Change(); }
        private void MinVegUpDn_Change()
        {
            //Buddied to MaxPopText, and synchronized, so we
            //do nothing
        }

        private void RepopAmountText_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { RepopAmountText_Change(); }
        private void RepopAmountText_Change()
        {
            TmpOpts.RepopAmount = val(RepopAmountText.text) % 32000;
        }

        private void RepopAmountUpDown_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { RepopAmountUpDown_Change(); }
        private void RepopAmountUpDown_Change(int Index_UNUSED)
        {
            //Buddied to RepopAmountText, and synchronized, so we
            //do nothing
        }

        private void RepopCooldownText_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { RepopCooldownText_Change(); }
        private void RepopCooldownText_Change()
        {
            TmpOpts.RepopCooldown = val(RepopCooldownText.text) % 32000;
        }

        private void RepopCooldownUpDown_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { RepopCooldownUpDown_Change(); }
        private void RepopCooldownUpDown_Change(int Index_UNUSED)
        {
            //Buddied to RepopCooldownText, and synchronized, so we
            //do nothing
        }

        private void KillDistVegsCheck_Click(object sender, RoutedEventArgs e) { KillDistVegsCheck_Click(); }
        private void KillDistVegsCheck_Click()
        {
            TmpOpts.KillDistVegs = KillDistVegsCheck.value * true;
        }

        private void MaxNRGText_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { MaxNRGText_Change(); }
        private void MaxNRGText_Change()
        {
            TmpOpts.MaxEnergy = val(MaxNRGText.text) % 32000;
        }

        private void BodyNrgDist_change()
        {
            TmpOpts.VegFeedingToBody = BodyNrgDist.value / 100;
        }

        /*
        ''''''''''''''''''''''''''''''''''

        '''''End Veg Controls and END GENERAL

        ''''''''''''''''''''''''''''''''''

        ''''''''''''''''''''''''''''''''''''''''''''

        ''''Physics Panel'''''''''''''''''''''''''''

        ''''''''''''''''''''''''''''''''''''''''''''

        'Private Sub SubstanceOption_Click(Index As Integer)
        '  Select Case Index
        '    Case 0 'liquid
        '      FrictionCombo.Enabled = False
        '      DragCombo.Enabled = True
        '    Case 1 'solid
        '      FrictionCombo.Enabled = True
        '      DragCombo.Enabled = False
        '  End Select
        'End Sub

        'needs work
        */
        private void DragCombo_Click(object sender, RoutedEventArgs e) { DragCombo_Click(); }
        private void DragCombo_Click()
        {
            switch (DragCombo.text)
            {
                case DragCombo.list(0):
                    TmpOpts.Viscosity = 0.01m;
                    TmpOpts.Density = 0.0000001m;
                    break;
                case DragCombo.list(1):
                    TmpOpts.Viscosity = 0.0005m;
                    TmpOpts.Density = 0.0000001m;
                    break;
                case DragCombo.list(2):
                    TmpOpts.Viscosity = 0.000025m;
                    TmpOpts.Density = 0.0000001m;
                    break;
                case DragCombo.list(3):
                    TmpOpts.Viscosity = 0;
                    TmpOpts.Density = 0;
                    break;
            }
        }

        private void EfficiencyCombo_Click(object sender, RoutedEventArgs e) { EfficiencyCombo_Click(); }
        private void EfficiencyCombo_Click()
        {
            switch (EfficiencyCombo.text)
            {
                case EfficiencyCombo.list(0):
                    TmpOpts.PhysMoving = 1;
                    break;
                case EfficiencyCombo.list(1):
                    TmpOpts.PhysMoving = 0.66m;
                    break;
                case EfficiencyCombo.list(2):
                    TmpOpts.PhysMoving = 0.33m;
                    break;
            }
        }

        private void FrictionCombo_Click(object sender, RoutedEventArgs e) { FrictionCombo_Click(); }
        private void FrictionCombo_Click()
        {
            switch (FrictionCombo.text)
            {
                case FrictionCombo.list(0):
                    TmpOpts.Zgravity = 4;
                    TmpOpts.CoefficientStatic = 0.9m;
                    TmpOpts.CoefficientKinetic = 0.75m;
                    break;
                case FrictionCombo.list(1):
                    TmpOpts.Zgravity = 2;
                    TmpOpts.CoefficientStatic = 0.6m;
                    TmpOpts.CoefficientKinetic = 0.4m;
                    break;
                case FrictionCombo.list(2):
                    TmpOpts.Zgravity = 1;
                    TmpOpts.CoefficientStatic = 0.05m;
                    TmpOpts.CoefficientKinetic = 0.05m;
                    break;
                case FrictionCombo.list(3):
                    TmpOpts.Zgravity = 0;
                    TmpOpts.CoefficientStatic = 0;
                    TmpOpts.CoefficientKinetic = 0;
                    break;
            }
        }

        private void GravityCombo_Click(object sender, RoutedEventArgs e) { GravityCombo_Click(); }
        private void GravityCombo_Click()
        {
            switch (GravityCombo.text)
            {
                case GravityCombo.list(0):
                    TmpOpts.Ygravity = 0;
                    break;
                case GravityCombo.list(1):
                    TmpOpts.Ygravity = 0.1m;
                    break;
                case GravityCombo.list(2):
                    TmpOpts.Ygravity = 0.3m;
                    break;
                case GravityCombo.list(3):
                    TmpOpts.Ygravity = 0.9m;
                    break;
                case GravityCombo.list(4):
                    TmpOpts.Ygravity = 6;
                    break;
            }
        }

        private void BrownianCombo_Click(object sender, RoutedEventArgs e) { BrownianCombo_Click(); }
        private void BrownianCombo_Click()
        {
            switch (BrownianCombo.text)
            {
                case BrownianCombo.list(0):
                    TmpOpts.PhysBrown = 7;
                    break;
                case BrownianCombo.list(1):
                    TmpOpts.PhysBrown = 0.5m;
                    break;
                case BrownianCombo.list(2):
                    TmpOpts.PhysBrown = 0;
                    break;
            }
        }

        /*
        '''''''''''''''''''''''''''''''''''''''''''
        */
        private void ToPhysics_Click(object sender, RoutedEventArgs e) { ToPhysics_Click(); }
        private void ToPhysics_Click()
        {
            PhysicsOptions.Show(vbModal);
            Update();
        }

        /*
        'Private Sub Cancel_Click()
        '  Canc = True
        '  If Form1.Visible Then Form1.SecTimer.Enabled = True
        '  'Me.Hide
        '  Unload Me
        'End Sub


        '  All settings
        */
        private void txtMinRounds_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { txtMinRounds_Change(); }
        private void txtMinRounds_Change()
        {
            MinRounds = val(txtMinRounds.text);
            if (MinRounds < 1)
            {
                MinRounds = 1;
            }
            optMinRounds = MinRounds;
        }

        private void ContestsUpDn_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { ContestsUpDn_Change(); }
        private void ContestsUpDn_Change()
        {
            txtMinRounds.text = ContestsUpDn.value;
            MinRounds = val(txtMinRounds.text);
        }

        /*
        ' sets all conditions to F1 specs
        */
        private void F1check_Click(object sender, RoutedEventArgs e) { F1check_Click(); }
        private void F1check_Click()
        {
            if (F1Check.value == 1)
            {
                TmpOpts.F1 = true;
            }
            else
            {
                TmpOpts.F1 = false;
            }

        }

        /*
        'Private Sub Form_Unload(Cancel As Integer) 'unload
        '  Form1.Timer2.Enabled = True
        'End Sub
        */
        private void Form_Activate()
        {
            Form1.hide_graphs();
            // Form1.SecTimer.Enabled = False
            // TmpOpts = SimOpts
            // Dim i As Long

            // SpecList.CLEAR
            // For i = 0 To TmpOpts.SpeciesNum - 1
            //   SpecList.additem (TmpOpts.Specie(i).Name)
            // Next i

            //  DispSettings

            //Botsareus 12/12/2012 Hide always on top forms for easy readability
            datirob.Visible = false;
            ActivForm.Instance.Visible = false;
            Shape2.FillColor = IIf(UseOldColor, 0x511206, vbBlack);

            //Botsareus 1/5/2014 Repopulate obstacle array
            if (Form1.Visible)
            {
                ObsRepop();
            }
            PaintObstacles();
        }

        void ObsRepop()
        {
            xObstacle = ObstaclesManager.Obstacles;
            int o = 0;

            for (o = 1; o < UBound(xObstacle); o++)
            {
                if (xObstacle(o).exist)
                {
                    dynamic _WithVar_2606;
                    _WithVar_2606 = xObstacle(o);
                    _WithVar_2606.pos.X = _WithVar_2606.pos.X / SimOpts.FieldWidth;
                    _WithVar_2606.pos.Y = _WithVar_2606.pos.Y / SimOpts.FieldHeight;
                    _WithVar_2606.Width = _WithVar_2606.Width / SimOpts.FieldWidth;
                    _WithVar_2606.Height = _WithVar_2606.Height / SimOpts.FieldHeight;
                }
            }
        }

        private void PaintObstacles()
        { //Botsareus 1/5/2014 The obstacle paint code
          // TODO (not supported): On Error GoTo fine //Bug fix for no obstacles
          //            IPB.Cls();
          //            int o = 0;

            //            for (o = 1; o < UBound(xObstacle); o++)
            //            {
            //                if (xObstacle(o).exist)
            //                {
            //                    dynamic _WithVar_1759;
            //                    _WithVar_1759 = xObstacle(o);
            //                    IPB.Line(_WithVar_1759.pos.X * IPB.ScaleWidth, _WithVar_1759.pos.Y * IPB.ScaleHeight) - ((_WithVar_1759.pos.X + _WithVar_1759.Width) * IPB.ScaleWidth, (_WithVar_1759.pos.Y + _WithVar_1759.Height) * IPB.ScaleHeight), _WithVar_1759.color, BF);
            //        }
            //    }

            //return;

            //fine:

            //  List<> xObstacle_4753_tmp = new List<>();
            //for (int redim_iter_1869=0;i<0;redim_iter_1869++) {xObstacle.Add(null);}
        }

        private void Form_Load(object sender, RoutedEventArgs e) { Form_Load(); }
        private void Form_Load()
        {
            TmpOpts = SimOpts;

            CurrSpec = -1; // EricL 4/1/2006 Initialize that no species is selected
            DragInit(); //Initialize drag code
            strings(this);

            Form1.ScaleWidth = SimOpts.FieldWidth;
            Form1.ScaleHeight = SimOpts.FieldHeight;
            if (TmpOpts.FieldWidth == 0)
            {
                TmpOpts.FieldWidth = 16000;
            }
            if (TmpOpts.FieldHeight == 0)
            {
                TmpOpts.FieldHeight = 12000;
            }
            datatolist();
            validate = true;
            DispSettings();
            validate = false;
        }


        private void OKButton_Click(object sender, RoutedEventArgs e) { OKButton_Click(); }
        private void OKButton_Click()
        {
            //Botsareus 5/13/2013 Safemode restrictions
            if (Form1.lblSafeMode.Visible)
            {
                MsgBox("Can not change settings during safemode");
                return;

            }

            savesett(MDIForm1.instance.MainDir + "\\settings\\lastran.set");

            //Botsareus 8/16/2014 Normalize the sun
            if (TmpOpts.SunOnRnd == false)
            {
                SunPosition = 0.5m;
                SunRange = 1;
            }

            Form1.camfix = false; //Botsareus 2/23/2013 When simulation starts the screen is normailized

            int i = 0;

            int k = 0;

            int a = 0;

            FileSystemObject fso = null;

            fso = new FileSystemObject(); ;

            DragEnd();

            Contests = 0;
            ReStarts = 0;
            MDIForm1.instance.Combo1.CLEAR();
            MDIForm1.Combo1.additem(WSnone);

            for (i = 0; i < TmpOpts.SpeciesNum - 1; i++)
            {
                if (i > MAXNATIVESPECIES)
                {
                    MsgBox("Exceeded number of native species.");
                }
                else
                {
                    if (TmpOpts.Specie(i).Native)
                    {
                        MDIForm1.Combo1.additem(TmpOpts.Specie(i).Name);
                    }
                }
            }

            if (TmpOpts.F1)
            {
                a = MsgBox("You cannot change settings in the middle of a contest. Please restart the SIM or disable F1 mode if you wish to continue.", vbOKOnly);
                return;

            }

            if (!(fso.FolderExists(inboundPathText.text) && fso.FolderExists(outboundPathText.text)))
            {
                MsgBox(("Internet paths must be set to a vaild directory."));
                return;

            }

            Canc = false;

            // EricL 4/2/2006 - moved these here from control change routine to fix init bugs
            TmpOpts.ChartingInterval = val(ChartInterval.text);
            TmpOpts.Restart = RestartMode; // EricL 4/2/2006 Added this to pick up any changes to restart mode

            IntOpts.IName = Trim(IntName.text);
            IntOpts.InboundPath = inboundPathText.text;
            IntOpts.OutboundPath = outboundPathText.text;
            IntOpts.ServIP = txtInttServ.text;
            IntOpts.ServPort = txtInttPort.text;
            SaveInternetSett();

            //These change values while a sim is running.
            //Copy them so the correct value will be put back into SimOpts
            TmpOpts.TotRunCycle = SimOpts.TotRunCycle;
            TmpOpts.TotBorn = SimOpts.TotBorn;
            TmpOpts.TotRunTime = SimOpts.TotRunTime;
            TmpOpts.DayNightCycleCounter = SimOpts.DayNightCycleCounter;
            TmpOpts.Daytime = SimOpts.Daytime;
            if (TmpOpts.DayNight == false)
            {
                TmpOpts.Daytime = true;
            }


            SimOpts = TmpOpts;

            if (InternetMode)
            {
                ResizeInternetTeleporter((SimOpts.FieldHeight / 150));
            }

            Form1.ScaleWidth = SimOpts.FieldWidth;
            Form1.ScaleHeight = SimOpts.FieldHeight;
            Form1.visiblew = SimOpts.FieldWidth;
            Form1.visibleh = SimOpts.FieldHeight;
            Form1.xDivisor = 1;
            Form1.yDivisor = 1;
            if (SimOpts.FieldWidth > 32000)
            {
                Form1.xDivisor = SimOpts.FieldWidth / 32000;
            }
            if (SimOpts.FieldHeight > 32000)
            {
                Form1.yDivisor = SimOpts.FieldHeight / 32000;
            }
            Form1.SecTimer.Enabled = true;
            Form1.Active = true;

            //Botsareus 1/5/2013 more fix for n-vedio button
            MDIForm1.instance.visualize = true; //Botsareus 1/11/2013 set to true

            //Me.Hide
            Unload(this);
        }

        private void RestartSimCheck_Click(object sender, RoutedEventArgs e) { RestartSimCheck_Click(); }
        private void RestartSimCheck_Click()
        {
            if (RestartSimCheck.IsChecked == 1)
            {
                RestartMode = true;
            }
            else
            {
                RestartMode = false;
            }
        }

        private void StartNew_Click(object sender, RoutedEventArgs e) { StartNew_Click(); }//startnew
        void StartNew_Click()
        {
            if (Form1.Visible)
            {
                if (MsgBox("Are you sure?", vbYesNo, "About to start a new simulation") == vbNo)
                {
                    return;

                }
            }

            if (chseedstartnew)
            {
                TmpOpts.UserSeedNumber = Timer * 100; //Botsareus 5/3/2013 Change seed on start new
            }

            //Botsareus 5/7/2013 Safemode component
            Form1.lblSafeMode.Visible = false;

            if (x_restartmode == 0 || x_restartmode == 4 || x_restartmode == 7)
            {
                MDIForm1.instance.Objects.IsEnabled = true;
                MDIForm1.instance.inssp.IsEnabled = true;
                MDIForm1.instance.DisableArep.IsEnabled = true;
            }
            else
            {
                MDIForm1.instance.Objects.IsEnabled = false;
                MDIForm1.instance.inssp.IsEnabled = false;
                MDIForm1.instance.DisableArep.IsEnabled = false;
            }
            MDIForm1.instance.AutoFork.IsEnabled = x_restartmode == 0;

            if (x_restartmode == 4 || x_restartmode == 5 || x_restartmode == 6)
            {
                MDIForm1.instance.y_info.setVisible(true);
            }

            if (dir(MDIForm1.instance.MainDir + "\\saves\\localcopy.sim") != "")
            {
                File.Delete((MDIForm1.MainDir + "\\saves\\localcopy.sim")); ;
            }
            if (dir(MDIForm1.instance.MainDir + "\\saves\\lastautosave.sim") != "")
            {
                File.Delete((MDIForm1.MainDir + "\\saves\\lastautosave.sim")); ;
            }

            int i = 0;

            int k = 0;

            int t = 0;


            DragEnd();

            Contests = 0;
            ReStarts = 0;

            // EricL Moved here from StartSimul() so that cycles counter isn't reset when sim settings
            // are changed.  Only reset when a new simulation is started.
            TmpOpts.TotRunCycle = -1;

            MDIForm1.instance.visualize = true;
            Form1.Label1.Visible = false;

            MDIForm1.instance.Combo1.CLEAR();
            MDIForm1.Combo1.additem(WSnone);

            for (i = 0; i < TmpOpts.SpeciesNum - 1; i++)
            {
                if (i > MAXNATIVESPECIES)
                {
                    MsgBox("Exceeded number of native species.");
                }
                else
                {
                    if (TmpOpts.Specie(i).Native)
                    {
                        MDIForm1.Combo1.additem(TmpOpts.Specie(i).Name);
                    }
                }
            }

            for (t = 0; t < TmpOpts.SpeciesNum - 1; t++)
            {
                TmpOpts.Specie(t).population = TmpOpts.Specie(t).qty;
                TmpOpts.Specie(t).SubSpeciesCounter = 0;
                TmpOpts.Specie(t).Native = true;
                //  If TmpOpts.Specie(t).Name = "plant.txt" Then
                //    TmpOpts.Specie(t).DisplayImage.Picture = Form1.Plant.Picture

                //  End If
            }

            ContestMode = TmpOpts.F1;
            MDIForm1.instance.F1Piccy.Visibility = TmpOpts.F1 * true;
            TmpOpts.Restart = RestartMode; // EricL 4/2/2006 This was backwards.  Changed from RestartMode = TmpOpts.Restart

            // EricL 4/2/2006 - moved these here from control's change routine to fix init bugs
            TmpOpts.ChartingInterval = val(ChartInterval.text);

            TmpOpts.SpeciesNum = SpecList.Items.Count;
            Canc = false;
            IntOpts.IName = IntName.text;
            IntOpts.InboundPath = inboundPathText.text;
            IntOpts.OutboundPath = outboundPathText.text;
            SaveInternetSett();
            //Me.Hide

            //Botsareus 1/5/2013 more fix for n-vedio button
            MDIForm1.instance.visualize = false;
            MDIForm1.instance.menuupdate();

            Unload(this);

            SimOpts = TmpOpts;

            if (Form1.Active)
            {
                Form1.SecTimer.Enabled = true;
            }
            if (InternetMode)
            {
                MDIForm1.instance.F1Internet_Click();
            }


            StartAnotherRound = true; // Set true for first simulation.  Will get set true if running leagues or using auto-restart mode
            While(StartAnotherRound);
            StartAnotherRound = false;
            Form1.StartSimul();
            SimOpts.UserSeedNumber = Rnd * 2147483647; //Botsareus 6/11/2013 Randomize seed on restart, moved to after first sim
            Wend();

        }

        private void LoadSettings_Click(object sender, RoutedEventArgs e) { LoadSettings_Click(); }//opensettings
        void LoadSettings_Click()
        {
            //On Error GoTo fine
            CommonDialog1.FileName = "";
            CommonDialog1.InitDir = MDIForm1.instance.MainDir + "\\settings";
            CommonDialog1.Filter = "Settings file(*.set)|*.set";
            CommonDialog1.ShowOpen();
            if (CommonDialog1.FileName != "")
            {
                ReadSett(CommonDialog1.FileName);
                datatolist();
                int i = 0;


                SpecList.CLEAR();
                for (i = 0; i < TmpOpts.SpeciesNum - 1; i++)
                {
                    if (TmpOpts.Specie(i).Native)
                    {
                        SpecList.additem((TmpOpts.Specie(i).Name));
                    }
                }

                validate = true;
                DispSettings();
                validate = false;

                PaintObstacles(); //Botsareus 11/15/2015 Minor bug fix to paint obstacles

            }
            return;

        fine:
            MsgBox("Error loading");
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e) { SaveSettings_Click(); }//savesettings
        private void SaveSettings_Click()
        {
            //On Error GoTo fine
            CommonDialog1.InitDir = MDIForm1.instance.MainDir + "\\settings";
            CommonDialog1.FileName = "";
            CommonDialog1.Filter = "Settings file(*.set)|*.set";
            CommonDialog1.ShowSave();
            if (CommonDialog1.FileName != "")
            {
                savesett(CommonDialog1.FileName);
            }
        fine:;
        }

        public void savesett(string path)
        {
            // TODO (not supported): On Error GoTo fine
            int t = 0;

            int k = 0;

            int numSpecies = 0;

            VBOpenFile(1, path); ;

            //EricL 4/13/2006 using this for version information
            //-2 is pre 2.42.2 versions
            //-1 is 2.42.2 and beyond VB fork
            //-3 is the C++ version
            t = -1;
            Write(1, t);

            numSpecies = 0;
            for (t = 0; t < TmpOpts.SpeciesNum - 1; t++)
            {
                if (TmpOpts.Specie(t).Native)
                {
                    numSpecies = numSpecies + 1;
                }
            }


            Write(1, numSpecies - 1); // done this way becuase of the busted way the read routine loops through species...
            for (t = 0; t < TmpOpts.SpeciesNum - 1; t++)
            {
                if (!TmpOpts.Specie(t).Native)
                {
                    goto skipthisspecie;
                }

                //Write 1, TmpOpts.Specie(t).Posrg
                //Write 1, TmpOpts.Specie(t).Posdn
                //Write 1, TmpOpts.Specie(t).Poslf
                //Write 1, TmpOpts.Specie(t).Postp

                Write(1, TmpOpts.FieldWidth);
                Write(1, TmpOpts.FieldHeight);
                Write(1, 0);
                Write(1, 0);

                Write(1, TmpOpts.Specie(t).Mutables.mutarray(0));
                Write(1, TmpOpts.Specie(t).path);
                Write(1, TmpOpts.Specie(t).qty);
                Write(1, TmpOpts.Specie(t).Name);
                Write(1, TmpOpts.Specie(t).Veg);
                Write(1, TmpOpts.Specie(t).Fixed);
                Write(1, TmpOpts.Specie(t).color);
                Write(1, TmpOpts.Specie(t).Colind);
                Write(1, TmpOpts.Specie(t).Stnrg);
                for (k = 0; k < 13; k++)
                {
                    Write(1, TmpOpts.Specie(t).Mutables.mutarray(k));
                }
                for (k = 0; k < 12; k++)
                {
                    Write(1, TmpOpts.Specie(t).Skin(k));
                }
            skipthisspecie:;
            }
            Write(1, TmpOpts.SimName);
            //TmpOpts.SpeciesNum = SpecList.ListCount

            //generali
            Write(1, numSpecies); //Botsareus 1/21/2013 No more non-native species -bug fixed
            Write(1, TmpOpts.FieldSize);
            Write(1, TmpOpts.FieldWidth);
            Write(1, TmpOpts.FieldHeight);
            Write(1, TmpOpts.MaxPopulation);
            Write(1, TmpOpts.BlockedVegs);
            Write(1, TmpOpts.DisableTies);
            Write(1, TmpOpts.PopLimMethod);
            Write(1, TmpOpts.Toroidal);
            Write(1, TmpOpts.MaxEnergy);
            Write(1, TmpOpts.MinVegs);

            //costi
            Write(1, TmpOpts.CostExecCond);
            Write(1, TmpOpts.Costs(COSTSTORE));
            Write(1, TmpOpts.Costs(SHOTCOST));
            Write(1, TmpOpts.EnergyProp);
            Write(1, TmpOpts.EnergyFix);
            Write(1, TmpOpts.EnergyExType);

            //fisica
            Write(1, TmpOpts.Ygravity);
            Write(1, TmpOpts.Zgravity);
            Write(1, TmpOpts.PhysBrown);
            Write(1, TmpOpts.PhysMoving);
            Write(1, TmpOpts.PhysSwim);

            Write(1, "null");
            Write(1, 0);
            Write(1, "null");
            Write(1, 0);

            Write(1, TmpOpts.MutCurrMult);
            Write(1, TmpOpts.MutOscill);
            Write(1, TmpOpts.MutCycMax);
            Write(1, TmpOpts.MutCycMin);

            Write(1, "null");
            Write(1, TmpOpts.DeadRobotSnp);
            Write(1, TmpOpts.SnpExcludeVegs);

            Write(1, TmpOpts.Pondmode);
            Write(1, false);
            Write(1, TmpOpts.LightIntensity);
            Write(1, TmpOpts.CorpseEnabled);
            Write(1, TmpOpts.Decay);
            Write(1, TmpOpts.Gradient);
            Write(1, TmpOpts.DayNight);
            Write(1, TmpOpts.CycleLength);

            Write(1, TmpOpts.DecayType);
            Write(1, TmpOpts.Decaydelay);

            //obsolete
            Write(1, TmpOpts.Costs(MOVECOST));

            Write(1, TmpOpts.F1);
            Write(1, TmpOpts.Restart);

            SaveScripts();

            Write(1, TmpOpts.Dxsxconnected);
            Write(1, TmpOpts.Updnconnected);

            Write(1, TmpOpts.RepopAmount);
            Write(1, TmpOpts.RepopCooldown);

            Write(1, TmpOpts.ZeroMomentum);
            Write(1, true); //Botsareus 5/3/2013 Replaced by safemode
            Write(1, TmpOpts.UserSeedNumber);

            for (t = 0; t < TmpOpts.SpeciesNum - 1; t++)
            {
                if (!TmpOpts.Specie(t).Native)
                {
                    goto skipthisspecie2;
                }
                for (k = 14; k < 20; k++)
                {
                    Write(1, TmpOpts.Specie(t).Mutables.mutarray(k));
                }
                Write(1, TmpOpts.Specie(t).Mutables.Mutations);
            skipthisspecie2:
            }

            Write(1, CInt(0));
            Write(1, TmpOpts.VegFeedingToBody);

            //New for 2.4:
            Write(1, TmpOpts.CoefficientStatic);
            Write(1, TmpOpts.CoefficientKinetic);
            Write(1, TmpOpts.PlanetEaters);
            Write(1, TmpOpts.PlanetEatersG);
            Write(1, TmpOpts.Viscosity);
            Write(1, TmpOpts.Density);


            for (k = 0; k < TmpOpts.SpeciesNum - 1; k++)
            {
                if (!TmpOpts.Specie(k).Native)
                {
                    goto skipthisspecie3;
                }
                Write(1, TmpOpts.Specie(k).Mutables.CopyErrorWhatToChange);
                Write(1, TmpOpts.Specie(k).Mutables.PointWhatToChange);

                int h = 0;


                for (h = 0; h < 20; h++)
                {
                    Write(1, TmpOpts.Specie(k).Mutables.Mean(h));
                    Write(1, TmpOpts.Specie(k).Mutables.StdDev(h));
                }
            skipthisspecie3:;
            }

            for (k = 0; k < 70; k++)
            {
                Write(1, TmpOpts.Costs(k));
            }

            for (k = 0; k < TmpOpts.SpeciesNum - 1; k++)
            {
                if (!TmpOpts.Specie(k).Native)
                {
                    goto skipthisspecie4;
                }
                Write(1, TmpOpts.Specie(k).Poslf);
                Write(1, TmpOpts.Specie(k).Posrg);
                Write(1, TmpOpts.Specie(k).Postp);
                Write(1, TmpOpts.Specie(k).Posdn);
            skipthisspecie4:;
            }

            Write(1, TmpOpts.MaxVelocity);
            Write(1, TmpOpts.BadWastelevel); //EricL 4/1/2006 Added this
            Write(1, TmpOpts.ChartingInterval); //EricL 4/1/2006 Added this
            Write(1, TmpOpts.FluidSolidCustom); //EricL 5/7/2006
            Write(1, TmpOpts.CostRadioSetting); // EricL 5/7/2006
            Write(1, TmpOpts.CoefficientElasticity); // EricL 5/7/2006
            Write(1, TmpOpts.MaxVelocity); // EricL 5/15/2006
            Write(1, TmpOpts.NoShotDecay); // EricL 6/8/2006
            Write(1, TmpOpts.SunUpThreshold); //EricL 6/8/2006 Added this
            Write(1, TmpOpts.SunUp); //EricL 6/8/2006 Added this
            Write(1, TmpOpts.SunDownThreshold); //EricL 6/8/2006 Added this
            Write(1, TmpOpts.SunDown); //EricL 6/8/2006 Added this
            Write(1, false);
            Write(1, false);
            Write(1, TmpOpts.FixedBotRadii);
            Write(1, TmpOpts.SunThresholdMode);

            //Botsareus 4/17/2013
            Write(1, TmpOpts.DisableTypArepro);

            //Botsareus 4/27/2013 Rest of species data
            for (t = 0; t < numSpecies - 1; t++)
            {
                Write(1, TmpOpts.Specie(t).CantSee);
                Write(1, TmpOpts.Specie(t).DisableDNA);
                Write(1, TmpOpts.Specie(t).DisableMovementSysvars);
                Write(1, TmpOpts.Specie(t).CantReproduce);
                Write(1, TmpOpts.Specie(t).VirusImmune);
            }

            Write(1, TmpOpts.NoWShotDecay); //Botsareus 9/28/2013

            //Botsareus 1/5/2014 Save obstacle data
            Write(1, TmpOpts.MakeAllShapesTransparent);
            Write(1, TmpOpts.MakeAllShapesBlack);
            Write(1, TmpOpts.ShapeDriftRate);
            Write(1, TmpOpts.AllowHorizontalShapeDrift);
            Write(1, TmpOpts.AllowVerticalShapeDrift);
            Write(1, TmpOpts.ShapesAreSeeThrough);
            Write(1, TmpOpts.ShapesAbsorbShots);
            Write(1, TmpOpts.ShapesAreVisable);

            int o = 0;


            //count walls first
            int numXobs = 0;


            for (o = 1; o < UBound(xObstacle); o++)
            {
                if (xObstacle(o).exist)
                {
                    dynamic _WithVar_2516;
                    _WithVar_2516 = xObstacle(o);
                    numXobs = numXobs + 1;
                }
            }

            Write(1, numXobs);

            for (o = 1; o < UBound(xObstacle); o++)
            {
                if (xObstacle(o).exist)
                {
                    dynamic _WithVar_7046;
                    _WithVar_7046 = xObstacle(o);
                    Write(1, _WithVar_7046.color);
                    Write(1, _WithVar_7046.Width);
                    Write(1, _WithVar_7046.Height);
                    Write(1, _WithVar_7046.vel.X);
                    Write(1, _WithVar_7046.vel.Y);
                    Write(1, _WithVar_7046.pos.X);
                    Write(1, _WithVar_7046.pos.Y);
                }
            }

            Write(1, optMinRounds);
            Write(1, Maxrounds);
            Write(1, MaxCycles);
            Write(1, MaxPop);

            //Botsareus 3/28/2014 Some more species data
            for (t = 0; t < numSpecies - 1; t++)
            {
                Write(1, TmpOpts.Specie(t).NoChlr);
            }

            //Botsareus 7/15/2014 Some more variables
            Write(1, TmpOpts.SunOnRnd);


            //Botsareus 7/30/2014 Some UI exposed to settings
            Write(1, MDIForm1.instance.displayResourceGuagesToggle);
            Write(1, MDIForm1.instance.displayMovementVectorsToggle);
            Write(1, MDIForm1.instance.displayShotImpactsToggle);
            Write(1, MDIForm1.instance.showVisionGridToggle);

            //Botsareus 7/29/2014 Some more species data
            for (t = 0; t < numSpecies - 1; t++)
            {
                Write(1, TmpOpts.Specie(t).kill_mb);
                Write(1, TmpOpts.Specie(t).dq_kill);
            }

            //Botsareus 8/5/2014 Disable fixing
            Write(1, TmpOpts.DisableFixing);

            //Botsareus 8/23/2014 Tides
            Write(1, TmpOpts.Tides);
            Write(1, TmpOpts.TidesOf);

            //Botsareus 8/10/2015
            Write(1, TmpOpts.MutOscillSine);

            VBCloseFile(1); ();
            return;

        fine:
            MsgBox(("Unable to save settings: some error occurred"));
        }

        public void ReadSett(string path)
        {
            // TODO (not supported): On Error GoTo aiuto
            int t = 0;

            decimal col = 0;

            int m = 0;

            int k = 0;

            int maxs = 0;

            int longv = 0;

            int intv = 0;

            decimal singv = 0;

            bool boolv = false;

            int b = 0;


        carica:
            // TODO (not supported):   On Error GoTo aiuto
            VBOpenFile(1, path); ;
            Input(1, maxs); //we can actually use this for version info

            //EricL 4/13/2006 Check for older settings files.  2.42.1 fixed bugs that introduced incomptabilities...
            if (maxs != -1)
            {
                if (Right(path, 12) == "lastexit.set")
                {
                    MsgBox(("The settings from your last exit are incomptable with this version.  Last exit settings not loaded.  When you exit the program, a new lastexit settings file will be created automatically."));
                }
                else
                {
                    if (Right(path, 11) == "default.set")
                    {
                        MsgBox(("The default settings file is incomptable with this version.  You can save your settings to default.set to create a new one.  Settings not loaded."));
                    }
                    else
                    {
                        MsgBox(("The settings file is incomptable with this version.  Settings not loaded."));
                    }
                }
            }
            else
            {
                clearall();
                ReadSettFromFile();
            }
            VBCloseFile(1); ();
            //EricL 3/21/2006 Added following three lines to work around problem with default settings file
            if ((TmpOpts.MaxEnergy == 500000) && (Right(path, 11) == "default.set"))
            {
                TmpOpts.MaxEnergy = 50;
            }
            lastsettings = path;
            return;

        aiuto:
            VBCloseFile(1); ();

            //EricL 3/22/2006 Added following section to work around case where there is no settings directory
            if (Err().Number == 76)
            {
                b = MsgBox("Cannot find the Settings Directory.  " + vbCrLf + "Would you like me to create one?   " + vbCrLf + vbCrLf + "If this is a new install, choose OK.", vbOKCancel | vbQuestion);
                if (b == vbOK)
                {
                    RecursiveMkDir((MDIForm1.instance.MainDir + "\\settings"));
                    InfoForm.instance.Show(); //Botsareus 5/8/2012 Show the info form if no settings where found
                }
                else
                {
                    MsgBox(("Darwinbots cannot continue.  Program will exit."));
                    End(); //Botsareus 7/12/2012 force DB to exit
                }
            }
            else if (Err().Number == 53 && Right(path, 12) == "lastexit.set")
            {
                MsgBox(("Cannot find the settings file from your last exit.  " + vbCrLf + "Using the internal default settings. " + vbCrLf + vbCrLf + "If this is a new install, this is normal."));
                InfoForm.instance.Show(); //Botsareus 3/24/2012 Show the info form if no settings where found
            }
            else
            {
                MsgBox(MBcannotfindI);
                CommonDialog1.FileName = path;
                CommonDialog1.ShowOpen();
                path = CommonDialog1.FileName;
                if (path != "")
                {
                    goto carica;
                }
            }

        }

        public void ReadSettFromFile()
        {
            int maxsp = 0;

            string strvar = "";

            bool check = false;

            decimal sinvar = 0;

            int t = 0;

            int k = 0;

            int obsoleteLong = 0;// EricL 3/28/2006 Added to read obsolete long values from the saved settings file

            int obsoleteInt = 0;

            string obsoleteString = "";

            bool obsoleteBool = false;

            Input(1, maxsp);

            for (t = 0; t < maxsp; t++)
            {
                TmpOpts.Specie(t).Posrg = 1;
                TmpOpts.Specie(t).Posdn = 1;
                TmpOpts.Specie(t).Poslf = 0;
                TmpOpts.Specie(t).Postp = 0;

                //Obsolete
                Input(1, obsoleteLong); //EricL 3/28/2006 Changed to Long from k to fix bug with reloading settings with large fields
                Input(1, obsoleteLong); //EricL 3/28/2006 Changed to Long from k to fix bug with reloading settings with large fields
                Input(1, k);
                Input(1, k);

                Input(1, TmpOpts.Specie(t).Mutables.mutarray(0));
                Input(1, TmpOpts.Specie(t).path);
                Input(1, TmpOpts.Specie(t).qty);
                Input(1, TmpOpts.Specie(t).Name);
                Input(1, TmpOpts.Specie(t).Veg);
                Input(1, TmpOpts.Specie(t).Fixed);
                Input(1, TmpOpts.Specie(t).color);
                Input(1, TmpOpts.Specie(t).Colind);
                Input(1, TmpOpts.Specie(t).Stnrg);
                for (k = 0; k < 13; k++)
                {
                    Input(1, TmpOpts.Specie(t).Mutables.mutarray(k));
                }
                for (k = 0; k < 12; k++)
                {
                    Input(1, TmpOpts.Specie(t).Skin(k));
                }
            }

            //generali
            Input(1, TmpOpts.SimName);
            Input(1, TmpOpts.SpeciesNum);
            Input(1, TmpOpts.FieldSize);
            Input(1, TmpOpts.FieldWidth);
            Input(1, TmpOpts.FieldHeight);
            Input(1, TmpOpts.MaxPopulation);
            Input(1, TmpOpts.BlockedVegs);
            Input(1, TmpOpts.DisableTies);
            Input(1, TmpOpts.PopLimMethod);
            Input(1, TmpOpts.Toroidal);
            Input(1, TmpOpts.MaxEnergy);
            Input(1, TmpOpts.MinVegs);

            //costi
            Input(1, TmpOpts.CostExecCond);
            Input(1, TmpOpts.Costs(COSTSTORE));
            Input(1, TmpOpts.Costs(SHOTCOST));
            Input(1, TmpOpts.EnergyProp);
            Input(1, TmpOpts.EnergyFix);
            Input(1, TmpOpts.EnergyExType);

            //fisica
            Input(1, TmpOpts.Ygravity);
            Input(1, TmpOpts.Zgravity);
            Input(1, TmpOpts.PhysBrown);
            Input(1, TmpOpts.PhysMoving);
            Input(1, TmpOpts.PhysSwim);

            Input(1, obsoleteString);
            Input(1, obsoleteInt);
            Input(1, obsoleteString);
            Input(1, obsoleteInt);
            Input(1, TmpOpts.MutCurrMult);
            Input(1, TmpOpts.MutOscill);
            Input(1, TmpOpts.MutCycMax);
            Input(1, TmpOpts.MutCycMin);
            Input(1, obsoleteString);
            Input(1, TmpOpts.DeadRobotSnp);
            Input(1, TmpOpts.SnpExcludeVegs);

            if (!EOF(1))
            {
                Input(1, TmpOpts.Pondmode);
            }
            if (!EOF(1))
            {
                Input(1, TmpOpts.CorpseEnabled); //dummy variable
            }
            if (!EOF(1))
            {
                Input(1, TmpOpts.LightIntensity);
            }
            if (!EOF(1))
            {
                Input(1, TmpOpts.CorpseEnabled);
            }
            if (!EOF(1))
            {
                Input(1, TmpOpts.Decay);
            }
            if (!EOF(1))
            {
                Input(1, TmpOpts.Gradient);
            }
            if (!EOF(1))
            {
                Input(1, TmpOpts.DayNight);
            }
            if (!EOF(1))
            {
                Input(1, TmpOpts.CycleLength);
            }
            if (!EOF(1))
            {
                Input(1, TmpOpts.DecayType);
            }
            if (!EOF(1))
            {
                Input(1, TmpOpts.Decaydelay);
            }

            //obsolete
            if (!EOF(1))
            {
                Input(1, TmpOpts.Costs(MOVECOST));
            }

            if (!EOF(1))
            {
                Input(1, TmpOpts.F1);
            }
            if (!EOF(1))
            {
                Input(1, TmpOpts.Restart);
            }

            LoadScripts(); //load up the scripts. Only available in form1. Can't access them from here.

            //even even newer newer stuff
            if (!EOF(1))
            {
                Input(1, TmpOpts.Dxsxconnected);
            }
            if (!EOF(1))
            {
                Input(1, TmpOpts.Updnconnected);
            }
            if (!EOF(1))
            {
                Input(1, TmpOpts.RepopAmount);
            }
            if (!EOF(1))
            {
                Input(1, TmpOpts.RepopCooldown);
            }
            if (!EOF(1))
            {
                Input(1, TmpOpts.ZeroMomentum);
            }
            if (!EOF(1))
            {
                Input(1, check); //Botsareus 5/3/2013 Replaced by safemode
            }
            if (!EOF(1))
            {
                Input(1, TmpOpts.UserSeedNumber);
            }

            for (t = 0; t < maxsp; t++)
            {
                for (k = 14; k < 20; k++)
                {
                    if (!EOF(1))
                    {
                        Input(1, TmpOpts.Specie(t).Mutables.mutarray(k));
                    }
                }

                if (!EOF(1))
                {
                    Input(1, TmpOpts.Specie(t).Mutables.Mutations);
                }
            }

            if (!EOF(1))
            {
                Input(1, obsoleteInt);
            }
            if (!EOF(1))
            {
                Input(1, TmpOpts.VegFeedingToBody);
            }
            else
            {
                TmpOpts.VegFeedingToBody == 0.1m;
            }

            //New for 2.4
            if (!EOF(1))
            {
                Input(1, TmpOpts.CoefficientStatic);
            }
            if (!EOF(1))
            {
                Input(1, TmpOpts.CoefficientKinetic);
            }
            if (!EOF(1))
            {
                Input(1, TmpOpts.PlanetEaters);
            }
            if (!EOF(1))
            {
                Input(1, TmpOpts.PlanetEatersG);
            }
            if (!EOF(1))
            {
                Input(1, TmpOpts.Viscosity);
            }
            if (!EOF(1))
            {
                Input(1, TmpOpts.Density);
            }

            for (k = 0; k < maxsp; k++)
            {
                if (!EOF(1))
                {
                    Input(1, TmpOpts.Specie(k).Mutables.CopyErrorWhatToChange);
                }
                if (!EOF(1))
                {
                    Input(1, TmpOpts.Specie(k).Mutables.PointWhatToChange);
                }

                int h = 0;


                for (h = 0; h < 20; h++)
                {
                    if (!EOF(1))
                    {
                        Input(1, TmpOpts.Specie(k).Mutables.Mean(h));
                    }
                    if (!EOF(1))
                    {
                        Input(1, TmpOpts.Specie(k).Mutables.StdDev(h));
                    }
                }
            }

            for (k = 0; k < 70; k++)
            {
                decimal temp = 0;

                if (!EOF(1))
                {
                    Input(1, temp);
                }
                TmpOpts.Costs(k) = temp;
            }

            for (k = 0; k < maxsp; k++)
            {
                if (!EOF(1))
                {
                    Input(1, TmpOpts.Specie(k).Poslf);
                }
                if (!EOF(1))
                {
                    Input(1, TmpOpts.Specie(k).Posrg);
                }
                if (!EOF(1))
                {
                    Input(1, TmpOpts.Specie(k).Postp);
                }
                if (!EOF(1))
                {
                    Input(1, TmpOpts.Specie(k).Posdn);
                }
                TmpOpts.Specie(k).Native = true;
            }

            if (!EOF(1))
            {
                Input(1, TmpOpts.MaxVelocity);
            }

            TmpOpts.BadWastelevel = -1; // Default
            if (!EOF(1))
            {
                Input(1, TmpOpts.BadWastelevel); //EricL 4/1/2006 Added this
            }

            TmpOpts.ChartingInterval = 200; // Default
            if (!EOF(1))
            {
                Input(1, TmpOpts.ChartingInterval); //EricL 4/1/2006 Added this
            }

            TmpOpts.FluidSolidCustom = 2; // Default to custom for older settings files
            if (!EOF(1))
            {
                Input(1, TmpOpts.FluidSolidCustom); //EricL 5/7/2006 Added this
            }

            TmpOpts.CostRadioSetting = 2; // Default to custom for older settings files
            if (!EOF(1))
            {
                Input(1, TmpOpts.CostRadioSetting); //EricL 5/7/2006 Added this
            }

            TmpOpts.CoefficientElasticity = 0; // Default for older settings files
            if (!EOF(1))
            {
                Input(1, TmpOpts.CoefficientElasticity); //EricL 5/7/2006 Added this
            }

            TmpOpts.MaxVelocity = 40; // Default for older settings files
            if (!EOF(1))
            {
                Input(1, TmpOpts.MaxVelocity); //EricL 5/15/2006 Added this
            }

            TmpOpts.NoShotDecay = false; // Default for older settings files
            if (!EOF(1))
            {
                Input(1, TmpOpts.NoShotDecay); //EricL 6/8/2006 Added this
            }

            TmpOpts.SunUpThreshold = 500000; //Set to a reasonable default value
            if (!EOF(1))
            {
                Input(1, TmpOpts.SunUpThreshold); //EricL 6/8/2006 Added this
            }

            TmpOpts.SunUp = false; //Set to a reasonable default value
            if (!EOF(1))
            {
                Input(1, TmpOpts.SunUp); //EricL 6/8/2006 Added this
            }

            TmpOpts.SunDownThreshold = 1000000; //Set to a reasonable default value
            if (!EOF(1))
            {
                Input(1, TmpOpts.SunDownThreshold); //EricL 6/8/2006 Added this
            }

            TmpOpts.SunDown = false; //Set to a reasonable default value
            if (!EOF(1))
            {
                Input(1, TmpOpts.SunDown); //EricL 6/8/2006 Added this
            }

            if (!EOF(1))
            {
                Input(1, obsoleteBool);
            }
            if (!EOF(1))
            {
                Input(1, obsoleteBool);
            }

            TmpOpts.FixedBotRadii = false;
            if (!EOF(1))
            {
                Input(1, TmpOpts.FixedBotRadii);
            }

            TmpOpts.SunThresholdMode = 0;
            if (!EOF(1))
            {
                Input(1, TmpOpts.SunThresholdMode);
            }

            TmpOpts.DisableTypArepro = 0;
            if (!EOF(1))
            {
                Input(1, TmpOpts.DisableTypArepro);
            }

            //Botsareus 4/37/2013 Rest of species data
            for (t = 0; t < maxsp; t++)
            {
                if (!EOF(1))
                {
                    Input(1, TmpOpts.Specie(t).CantSee);
                }
                if (!EOF(1))
                {
                    Input(1, TmpOpts.Specie(t).DisableDNA);
                }
                if (!EOF(1))
                {
                    Input(1, TmpOpts.Specie(t).DisableMovementSysvars);
                }
                if (!EOF(1))
                {
                    Input(1, TmpOpts.Specie(t).CantReproduce);
                }
                if (!EOF(1))
                {
                    Input(1, TmpOpts.Specie(t).VirusImmune);
                }
            }

            //Botsareus 9/28/2013 Do not decay waste shots
            TmpOpts.NoWShotDecay = false;
            if (!EOF(1))
            {
                Input(1, TmpOpts.NoWShotDecay);
            }

            //Botsareus 1/5/2014 Obstecle settings
            if (!EOF(1))
            {
                Input(1, TmpOpts.MakeAllShapesTransparent);
            }
            if (!EOF(1))
            {
                Input(1, TmpOpts.MakeAllShapesBlack);
            }
            if (!EOF(1))
            {
                Input(1, TmpOpts.ShapeDriftRate);
            }
            if (!EOF(1))
            {
                Input(1, TmpOpts.AllowHorizontalShapeDrift);
            }
            if (!EOF(1))
            {
                Input(1, TmpOpts.AllowVerticalShapeDrift);
            }
            if (!EOF(1))
            {
                Input(1, TmpOpts.ShapesAreSeeThrough);
            }
            if (!EOF(1))
            {
                Input(1, TmpOpts.ShapesAbsorbShots);
            }
            if (!EOF(1))
            {
                Input(1, TmpOpts.ShapesAreVisable);
            }

            int numXobs = 0;


            if (!EOF(1))
            {
                Input(1, numXobs);
            }

            List<> xObstacle_9648_tmp = new List<>();
            for (int redim_iter_8046 = 0; i < 0; redim_iter_8046++) { xObstacle.Add(null); }
            int o = 0;

            for (o = 1; o < numXobs; o++)
            {
                dynamic _WithVar_4885;
                _WithVar_4885 = xObstacle(o);
                if (!EOF(1))
                {
                    Input(1, _WithVar_4885.color);
                }
                if (!EOF(1))
                {
                    Input(1, _WithVar_4885.Width);
                }
                if (!EOF(1))
                {
                    Input(1, _WithVar_4885.Height);
                }
                if (!EOF(1))
                {
                    Input(1, _WithVar_4885.vel.X);
                }
                if (!EOF(1))
                {
                    Input(1, _WithVar_4885.vel.Y);
                }
                if (!EOF(1))
                {
                    Input(1, _WithVar_4885.pos.X);
                }
                if (!EOF(1))
                {
                    Input(1, _WithVar_4885.pos.Y);
                }
                _WithVar_4885.exist = true;
            }

            if (!EOF(1))
            {
                Input(1, optMinRounds);
            }
            MinRounds = optMinRounds;
            if (!EOF(1))
            {
                Input(1, Maxrounds);
            }
            if (!EOF(1))
            {
                Input(1, MaxCycles);
            }
            if (!EOF(1))
            {
                Input(1, MaxPop);
            }

            //Botsareus 3/28/2014 Some more species data
            for (t = 0; t < maxsp; t++)
            {
                if (!EOF(1))
                {
                    Input(1, TmpOpts.Specie(t).NoChlr);
                }
            }

            //Botsareus 7/15/2014 Some more variables
            if (!EOF(1))
            {
                Input(1, TmpOpts.SunOnRnd);
            }

            //Botsareus 7/18/2014 Some more settings exposed for Testlund
            if (!EOF(1))
            {
                Input(1, check);
            }
            MDIForm1.instance.displayResourceGuagesToggle = check;
            MDIForm1.instance.DisplayResourceGuages.Checked = check;
            if (!EOF(1))
            {
                Input(1, check);
            }
            MDIForm1.instance.displayMovementVectorsToggle = check;
            MDIForm1.instance.DisplayMovementVectors.Checked = check;
            if (!EOF(1))
            {
                Input(1, check);
            }
            MDIForm1.instance.displayShotImpactsToggle = check;
            MDIForm1.instance.DisplayShotImpacts.Checked = check;
            if (!EOF(1))
            {
                Input(1, check);
            }
            MDIForm1.instance.showVisionGridToggle = check;
            MDIForm1.instance.ShowVisionGrid.Checked = check;

            //Botsareus 7/29/2014 Some more species data
            for (t = 0; t < maxsp; t++)
            {
                if (!EOF(1))
                {
                    Input(1, TmpOpts.Specie(t).kill_mb);
                }
                if (!EOF(1))
                {
                    Input(1, TmpOpts.Specie(t).dq_kill);
                }
            }

            //Botsareus 8/5/2014 Disable fixing
            TmpOpts.DisableFixing = 0;
            if (!EOF(1))
            {
                Input(1, TmpOpts.DisableFixing);
            }

            //Botsareus 8/23/2014 Tides
            if (!EOF(1))
            {
                Input(1, TmpOpts.Tides);
            }
            if (!EOF(1))
            {
                Input(1, TmpOpts.TidesOf);
            }

            //Botsareus 10/8/2015
            if (!EOF(1))
            {
                Input(1, TmpOpts.MutOscillSine);
            }

            if ((!EOF(1)))
            {
                MsgBox("This settings file is a newer version than this version can read.  " + vbCrLf + "Not all the information it contains can be " + vbCrLf + "transfered.");
            }

            if (TmpOpts.FieldWidth == 0)
            {
                TmpOpts.FieldWidth = 16000;
            }
            if (TmpOpts.FieldHeight == 0)
            {
                TmpOpts.FieldHeight = 12000;
            }
            if (TmpOpts.MaxVelocity == 0)
            {
                TmpOpts.MaxVelocity = 60;
            }
            if (TmpOpts.Costs(DYNAMICCOSTSENSITIVITY) == 0)
            {
                TmpOpts.Costs(DYNAMICCOSTSENSITIVITY) = 50;
            }


            //EricL 4/13/2006 divide by zero protection for older settings files.
            if (TmpOpts.ChartingInterval == 0)
            {
                TmpOpts.ChartingInterval = 200;
            }

            TmpOpts.DayNightCycleCounter = 0; // When you load settings, you don't get the state from the last sim
            TmpOpts.Daytime = true; // EricL 3/21/2006 - this is a bettter place for this than in MDIForm_Load

        }

        private void Prop_Lostfocus()
        {
            TmpOpts.EnergyProp = val(Prop.text) / 100;
        }

        private void PropUpDn_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { PropUpDn_Change(); }
        private void PropUpDn_Change()
        {
            decimal a = 0;

            a = PropUpDn.value;
            Prop.text = Str$(a / 100);
            TmpOpts.EnergyProp = a / 100;
        }

        void Fixed_Lostfocus()
        {
            decimal a = 0;

            a = val(Fixed.text);
            if (a < FixUpDn.Min)
            {
                a = FixUpDn.Min;
            }
            if (a > FixUpDn.Max)
            {
                a = FixUpDn.Max;
            }
            TmpOpts.EnergyFix = a;
        }

        private void FixUpDn_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { FixUpDn_Change(); }
        private void FixUpDn_Change()
        {
            decimal a = 0;

            a = FixUpDn.value;
            TmpOpts.EnergyFix = a;
        }

        private void ExchangeProp_Click(object sender, RoutedEventArgs e) { ExchangeProp_Click(); }
        private void ExchangeProp_Click()
        {
            TmpOpts.EnergyExType = ExchangeProp.value;
        }

        private void ExchangeFix_Click(object sender, RoutedEventArgs e) { ExchangeFix_Click(); }
        private void ExchangeFix_Click()
        {
            TmpOpts.EnergyExType = !(ExchangeFix.value);
        }

        /*
        '  M U T R A T E   O P T I O N S
        */
        private void MutSlide_Scroll()
        {
            TmpOpts.MutCurrMult = 2 ^ MutSlide.value;
            if (TmpOpts.MutCurrMult > 1)
            {
                MutLab.Content = Str$(TmpOpts.MutCurrMult) + " X";
            }
            else
            {
                MutLab.Content = "1/" + Str$(2 ^ -MutSlide.value) + " X";
            }
        }

        private void CyclesHi_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { CyclesHi_Change(); }
        private void CyclesHi_Change()
        {
            TmpOpts.MutCycMax = val(CyclesHi.text);
        }

        private void CyclesLo_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { CyclesLo_Change(); }
        private void CyclesLo_Change()
        {
            TmpOpts.MutCycMin = val(CyclesLo.text);
        }

        private void MutOscill_Click(object sender, RoutedEventArgs e) { MutOscill_Click(); }
        private void MutOscill_Click()
        {
            TmpOpts.MutOscill = false;
            if (MutOscill.value == 1)
            {
                TmpOpts.MutOscill = true;
            }
            MutOscillSin.Visibility = MutOscill.value == 1;
            Label3.Visibility = MutOscill.value == 1;
            Label9.Visibility = MutOscill.value == 1;
            CyclesHi.Visibility = MutOscill.value == 1;
            CyclesLo.Visibility = MutOscill.value == 1;
            CycHiUpDn.Visibility = MutOscill.value == 1;
            CycLoUpDn.Visibility = MutOscill.value == 1;
        }
    }
}
