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
using static SimOptModule;
using static Common;
using static Flex;
using static Robots;
using static Ties;
using static Shots_Module;
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
using static Obstacles;
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


namespace DBNet.Forms
{
    public partial class optionsform : Window
    {
        private static optionsform _instance;
        public static optionsform instance { set { _instance = null; } get { return _instance ?? (_instance = new optionsform()); } }
        public static void Load() { if (_instance == null) { dynamic A = optionsform.instance; } }
        public static void Unload() { if (_instance != null) instance.Close(); _instance = null; }
        public optionsform() { InitializeComponent(); }


        //Botsareus 5/8/2012 Selected the first tab to be the default when window first opens (vb generated code above may have changed)
        // boring stuff. this is the sim options module, so
        // there's much graphical interface (and a big mess).
        // simulation options are saved in a structure
        // called SimOptions. When the options window is opened
        // the current settings SimOpts are copied to TmpOpts
        // which is modified. On exit, if ok has been clicked
        // TmpOpts is copied to SimOpts again
        // SimOpts struct is defined in SimOptModule
        // Option Explicit
        private bool validate = false;
        private bool pass = false;
        private bool follow1 = false;
        private bool follow2 = false;
        private bool follow3 = false;
        private bool follow4 = false;
        //Dim SpeciesToggle As Boolean 'Botsareus 1/21/2013 no more need for speices toggle
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
        private const dynamic NULL_BRUSH = 5;
        private const dynamic PS_SOLID = 0;
        private const dynamic R2_NOT = 6;
        public enum ControlState
        {
            StateNothing = 0,
            StateDragging,
            StateSizing
        }
        private Window m_CurrCtl = null;
        private ControlState m_DragState = null;
        private int m_DragHandle = 0;
        private CRect m_DragRect = new CRect();
        private POINTAPI m_DragPoint = null;
        private bool speclistchecked = false;


        private void btnFatalRES_Click(object sender, RoutedEventArgs e) { btnFatalRES_Click(); }
        private void btnFatalRES_Click()
        {
            //Botsareus 7/29/2014 Show restrictions
            if (OptionsForm.instance.CurrSpec < 0)
            {
                MsgBox(("Please select a species."));
            }
            else
            {
                frmRestriOps.instance.res_state = IIf(TmpOpts.Specie(optionsform.CurrSpec).Veg, 0, 1);
                frmRestriOps.Show(vbModal);
            }
        }

        private void btnSetF1_Click(object sender, RoutedEventArgs e) { btnSetF1_Click(); }//Botsareus 2/5/2014 New way to set league settings
        private void btnSetF1_Click()
        {
            if (!pass)
            {
                if (MsgBox("Enabling F1 mode settings will greatly alter the physics of your simulation. Are you sure?", vbExclamation | vbYesNo, "Darwinbots Settings") == vbNo)
                {
                    return;

                }
            }
            pass = false;

            int t = 0;

            //Zero all Costs
            for (t = 1; t < 70; t++)
            {
                TmpOpts.Costs(t) = 0;
            }

            //Now set the ones that matter
            TmpOpts.Costs(SHOTCOST) = 2;
            TmpOpts.Costs(COSTSTORE) = 0.04m;
            TmpOpts.Costs(CONDCOST) = 0.004m;
            TmpOpts.Costs(MOVECOST) = 0.05m;
            TmpOpts.Costs(TIECOST) = 2;
            TmpOpts.Costs(SHOTCOST) = 2;
            TmpOpts.Costs(VENOMCOST) = 0.01m;
            TmpOpts.Costs(POISONCOST) = 0.01m;
            TmpOpts.Costs(SLIMECOST) = 0.1m;
            TmpOpts.Costs(SHELLCOST) = 0.1m;
            TmpOpts.Costs(COSTMULTIPLIER) = 1;
            TmpOpts.Costs(BODYUPKEEP) = 0.00001m;
            TmpOpts.Costs(AGECOST) = 0.01m;

            TmpOpts.Costs(CHLRCOST) = 0.2m;

            TmpOpts.DynamicCosts = false;

            TmpOpts.CorpseEnabled = false; // No Corpses
            TmpOpts.DayNight = false; // Sun never sets
            TmpOpts.SunOnRnd = true; // There is weather
            TmpOpts.FieldWidth = 9237;
            TmpOpts.FieldHeight = 6928;
            TmpOpts.FieldSize = 1;
            TmpOpts.MaxEnergy = 40; // Veggy nrg per cycle
            TmpOpts.MaxPopulation = 25; // Veggy max population
            TmpOpts.MinVegs = 10;
            TmpOpts.Pondmode = false;
            TmpOpts.PhysBrown = 0; // Animal Motion
            TmpOpts.Toroidal = true;
            TmpOpts.Updnconnected = true;
            TmpOpts.Dxsxconnected = true;

            TmpOpts.BadWastelevel = 10000; // Pretty high Waste Threshold

            for (t = 0; t < TmpOpts.SpeciesNum - 1; t++)
            {
                TmpOpts.Specie(t).Fixed = false; //Nobody is fixed
                TmpOpts.Specie(t).Mutables.Mutations = false; //Nobody can mutate
                TmpOpts.Specie(t).CantSee = false;
                TmpOpts.Specie(t).DisableDNA = false;
                TmpOpts.Specie(t).CantReproduce = false;
                TmpOpts.Specie(t).DisableMovementSysvars = false;
                TmpOpts.Specie(t).VirusImmune = false;
                TmpOpts.Specie(t).qty = 5;
                TmpOpts.Specie(t).Native = true;
            }

            TmpOpts.FixedBotRadii = false;
            TmpOpts.NoShotDecay = false;
            TmpOpts.NoWShotDecay = false;
            TmpOpts.DisableTies = false;
            TmpOpts.DisableTypArepro = false;
            TmpOpts.DisableFixing = false;
            TmpOpts.RepopAmount = 10;
            TmpOpts.RepopCooldown = 25;
            TmpOpts.MaxVelocity = 180;
            TmpOpts.VegFeedingToBody = 0.5m; // 50/50 nrg/body veggy feeding ratio
            TmpOpts.SunUp = false; // Turn off bringing the sun up due to a threshold
            TmpOpts.SunDown = false; // Turn off setting the sun due to a threshold
            TmpOpts.CoefficientElasticity = 0; // Collisions are soft.
            TmpOpts.Ygravity = 0;

            // Surface Friction - Metal Option
            TmpOpts.Zgravity = 2;
            TmpOpts.CoefficientStatic = 0.6m;
            TmpOpts.CoefficientKinetic = 0.4m;

            //No Fluid Resistance
            TmpOpts.Viscosity = 0;
            TmpOpts.Density = 0;

            //Shot Energy Physics
            TmpOpts.EnergyProp = 1; // 100% normal shot nrg
            TmpOpts.EnergyExType = true; // Use Proportional shot nrg exchange method

            DispSettings();
        }

        private void btnSetF2_Click(object sender, RoutedEventArgs e) { btnSetF2_Click(); }//Botsareus 2/5/2014 New way to set league settings
        private void btnSetF2_Click()
        {
            if (MsgBox("Enabling F2 mode settings will greatly alter the physics of your simulation. Are you sure?", vbExclamation | vbYesNo, "Darwinbots Settings") == vbNo)
            {
                return;

            }

            pass = true;

            btnSetF1_Click();
            TmpOpts.MaxEnergy = 30; // Veggy nrg per cycle
            MaxNRGText.text = 30;
        }

        private void btnSetSB_Click(object sender, RoutedEventArgs e) { btnSetSB_Click(); }
        private void btnSetSB_Click()
        {
            if (MsgBox("Enabling SB mode settings will greatly alter the physics of your simulation. Are you sure?", vbExclamation | vbYesNo, "Darwinbots Settings") == vbNo)
            {
                return;

            }

            pass = true;

            btnSetF1_Click();
            TmpOpts.FieldWidth = 16000;
            TmpOpts.FieldHeight = 12000;
            FieldSizeSlide.value = 2;
            FWidthLab.DefaultProperty = 16000;
            FHeightLab.DefaultProperty = 12000;
        }

        private void chkNoChlr_Click(object sender, RoutedEventArgs e) { chkNoChlr_Click(); }
        private void chkNoChlr_Click()
        {
            if (CurrSpec > -1)
            {
                TmpOpts.Specie(CurrSpec).NoChlr = false;
                if (chkNoChlr.value == 1)
                {
                    TmpOpts.Specie(CurrSpec).NoChlr = true;
                    TmpOpts.Specie(CurrSpec).Veg = false;
                    SpecVeg.value = 0;
                }
            }
        }

        private void CyclesHi_LostFocus(object sender, RoutedEventArgs e) { CyclesHi_LostFocus(); }
        private void CyclesHi_LostFocus()
        {
            int pass = 0;

            pass = val(CyclesHi.text);
            if (pass > 500000)
            {
                pass = 500000;
            }
            if (pass < 0)
            {
                pass = 0;
            }
            CyclesHi.Text = pass;
        }

        private void CyclesLo_LostFocus(object sender, RoutedEventArgs e) { CyclesLo_LostFocus(); }
        private void CyclesLo_LostFocus()
        {
            int pass = 0;

            pass = val(CyclesLo.text);
            if (pass > 500000)
            {
                pass = 500000;
            }
            if (pass < 0)
            {
                pass = 0;
            }
            CyclesLo.Text = pass;
        }

        private void Gradient_GotFocus(object sender, RoutedEventArgs e) { Gradient_GotFocus(); }
        private void Gradient_GotFocus()
        {
            tmrLight.IsEnabled = true;
        }

        private void LightText_GotFocus(object sender, RoutedEventArgs e) { LightText_GotFocus(); }
        private void LightText_GotFocus()
        {
            tmrLight.IsEnabled = true;
        }

        private void MaxCyclesText_LostFocus(object sender, RoutedEventArgs e) { MaxCyclesText_LostFocus(); }
        private void MaxCyclesText_LostFocus()
        {
            MaxCyclesText.text = MaxCycles;
        }

        private void MaxPopF1Text_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { MaxPopF1Text_Change(); }
        private void MaxPopF1Text_Change()
        {
            MaxPop = val(MaxPopF1Text.text);
            if (MaxPop != 0 & MaxPop < 10)
            {
                MaxPop = 10;
            }
        }

        private void MaxPopF1Text_LostFocus(object sender, RoutedEventArgs e) { MaxPopF1Text_LostFocus(); }
        private void MaxPopF1Text_LostFocus()
        {
            MaxPopF1Text.text = MaxPop;
        }

        private void MutOscillSin_Click(object sender, RoutedEventArgs e) { MutOscillSin_Click(); }
        private void MutOscillSin_Click()
        {
            TmpOpts.MutOscillSine = false;
            if (MutOscillSin.value == 1)
            {
                TmpOpts.MutOscillSine = true;
            }
            Label3.Content = IIf(MutOscillSin.value == 1, "Max at 20x", "Cycles at 16x");
            Label9.Content = IIf(MutOscillSin.value == 1, "Max at 1/20x", "Cycles at 1/16x");
        }

        private void tmrLight_Timer()
        {
            Static(dir(As(Boolean)));
            int n = 0;

            if (dir)
            {
                n = EnergyScalingFactor.Text + 5;
                if (n > 100)
                {
                    dir = false;
                    n = 100;
                }
            }
            else
            {
                n = EnergyScalingFactor.Text - 5;
                if (n < 2)
                {
                    dir = true;
                    n = 2;
                }
            }
            EnergyScalingFactor.Text = n;
        }

        private void txtMinRounds_LostFocus(object sender, RoutedEventArgs e) { txtMinRounds_LostFocus(); }
        private void txtMinRounds_LostFocus()
        {
            txtMinRounds.text = MinRounds;
            optMinRounds = MinRounds;
        }

        private void CorpseCheck_MouseUp(int Button_UNUSED, int Shift_UNUSED, decimal X_UNUSED, decimal Y_UNUSED)
        {
            if (CorpseCheck.value == 1)
            { //Botsareus 1/17/2013 set default values
                TmpOpts.DecayType = 3;
                TmpOpts.Decay = 75;
                TmpOpts.Decaydelay = 3;
                DispSettings();
            }
        }

        private void DisableDNACheck_Click(object sender, RoutedEventArgs e) { DisableDNACheck_Click(); }
        private void DisableDNACheck_Click()
        {
            if (CurrSpec > -1)
            {
                TmpOpts.Specie(CurrSpec).DisableDNA = false;
                if (DisableDNACheck.value == 1)
                {
                    TmpOpts.Specie(CurrSpec).DisableDNA = true;
                }
            }
        }

        private void DisableMovementSysvarsCheck_Click(object sender, RoutedEventArgs e) { DisableMovementSysvarsCheck_Click(); }
        private void DisableMovementSysvarsCheck_Click()
        {
            if (CurrSpec > -1)
            {
                TmpOpts.Specie(CurrSpec).DisableMovementSysvars = false;
                if (DisableMovementSysvarsCheck.value == 1)
                {
                    TmpOpts.Specie(CurrSpec).DisableMovementSysvars = true;
                }
            }
        }

        private void DisableMutationsCheck_Click(object sender, RoutedEventArgs e) { DisableMutationsCheck_Click(); }
        private void DisableMutationsCheck_Click()
        {
            TmpOpts.DisableMutations = DisableMutationsCheck.value * true;
        }

        private void DisableReproductionCheck_Click(object sender, RoutedEventArgs e) { DisableReproductionCheck_Click(); }
        private void DisableReproductionCheck_Click()
        {
            if (CurrSpec > -1)
            {
                TmpOpts.Specie(CurrSpec).CantReproduce = false;
                if (DisableReproductionCheck.value == 1)
                {
                    TmpOpts.Specie(CurrSpec).CantReproduce = true;
                }
            }
        }

        private void DisableVisionCheck_Click(object sender, RoutedEventArgs e) { DisableVisionCheck_Click(); }
        private void DisableVisionCheck_Click()
        {
            if (CurrSpec > -1)
            {
                TmpOpts.Specie(CurrSpec).CantSee = false;
                if (DisableVisionCheck.value == 1)
                {
                    TmpOpts.Specie(CurrSpec).CantSee = true;
                }
            }
        }

        private void Elasticity_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { Elasticity_Change(); }
        private void Elasticity_Change()
        {
            TmpOpts.CoefficientElasticity = Elasticity.value / 10;
            Elasticity.text = Elasticity.value / 10;
            // Elasticity.Refresh
        }

        private void Energy_Click(object sender, RoutedEventArgs e) { Energy_Click(); }
        private void Energy_Click()
        {
            EnergyForm.Show(vbModal);
            Update();
        }

        /*
        'Botsareus 9/1/2014 Update main presets
        */
        private void Update()
        {
            BrownianCombo.Visibility = TmpOpts.Tides == 0;
            GravityCombo.Visibility = TmpOpts.Tides == 0;
            Label7(2).Visibility = TmpOpts.Tides == 0;
            Label7(3).Visibility = TmpOpts.Tides == 0;
            Label47.Visibility = TmpOpts.Tides > 0;

            if (TmpOpts.PhysMoving < 0.33m)
            {
                EfficiencyCombo.text = EfficiencyCombo.list(2);
            }
            if (TmpOpts.PhysMoving > 0.33m && TmpOpts.PhysMoving < 0.66m)
            {
                EfficiencyCombo.text = EfficiencyCombo.list(1);
            }
            if (TmpOpts.PhysMoving > 0.66m)
            {
                EfficiencyCombo.text = EfficiencyCombo.list(0);
            }

            if (TmpOpts.PhysMoving == 0.33m)
            {
                EfficiencyCombo.text = EfficiencyCombo.list(2);
            }
            if (TmpOpts.PhysMoving == 0.66m)
            {
                EfficiencyCombo.text = EfficiencyCombo.list(1);
            }
            if (TmpOpts.PhysMoving == 1)
            {
                EfficiencyCombo.text = EfficiencyCombo.list(0);
            }


            if (TmpOpts.PhysBrown > 7)
            {
                BrownianCombo.text = BrownianCombo.list(0);
            }
            if (TmpOpts.PhysBrown > 0.5m && TmpOpts.PhysBrown < 7)
            {
                BrownianCombo.text = BrownianCombo.list(1);
            }
            if (TmpOpts.PhysBrown < 0.5m)
            {
                BrownianCombo.text = BrownianCombo.list(2);
            }

            if (TmpOpts.PhysBrown == 7)
            {
                BrownianCombo.text = BrownianCombo.list(0);
            }
            if (TmpOpts.PhysBrown == 0.5m)
            {
                BrownianCombo.text = BrownianCombo.list(1);
            }
            if (TmpOpts.PhysBrown == 0)
            {
                BrownianCombo.text = BrownianCombo.list(2);
            }


            if (TmpOpts.Ygravity < 0.1m)
            {
                GravityCombo.text = GravityCombo.list(0);
            }
            if (TmpOpts.Ygravity > 0.1m && TmpOpts.Ygravity < 0.3m)
            {
                GravityCombo.text = GravityCombo.list(1);
            }
            if (TmpOpts.Ygravity > 0.3m && TmpOpts.Ygravity < 0.9m)
            {
                GravityCombo.text = GravityCombo.list(2);
            }
            if (TmpOpts.Ygravity > 0.9m && TmpOpts.Ygravity < 9)
            {
                GravityCombo.text = GravityCombo.list(3);
            }
            if (TmpOpts.Ygravity > 6)
            {
                GravityCombo.text = GravityCombo.list(4);
            }

            if (TmpOpts.Ygravity == 0)
            {
                GravityCombo.text = GravityCombo.list(0);
            }
            if (TmpOpts.Ygravity == 0.1m)
            {
                GravityCombo.text = GravityCombo.list(1);
            }
            if (TmpOpts.Ygravity == 0.3m)
            {
                GravityCombo.text = GravityCombo.list(2);
            }
            if (TmpOpts.Ygravity == 0.9m)
            {
                GravityCombo.text = GravityCombo.list(3);
            }
            if (TmpOpts.Ygravity == 6)
            {
                GravityCombo.text = GravityCombo.list(4);
            }

        }

        private void FixBotRadius_Click(object sender, RoutedEventArgs e) { FixBotRadius_Click(); }
        private void FixBotRadius_Click()
        {
            TmpOpts.FixedBotRadii = FixBotRadius.value * true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) { int c = 0, u = 0; Form_QueryUnload(c, u); e.Cancel = c != 0; }//Botsareus 1/5/2013 moved cancel code here
        private void Form_QueryUnload(int Cancel_UNUSED, int UnloadMode_UNUSED)
        {
            Form1.show_graphs();
            Form1.camfix = false; //Botsareus 2/23/2013 re-normalize screen
            Canc = true;
        }

        private void FrequencyCheckUpDn_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { FrequencyCheckUpDn_Change(); }
        private void FrequencyCheckUpDn_Change()
        {
            txtMaxRounds.text = FrequencyCheckUpDn.value;
            Maxrounds = val(txtMaxRounds.text);
        }

        private void Gradient_LostFocus(object sender, RoutedEventArgs e) { Gradient_LostFocus(); }
        private void Gradient_LostFocus()
        {
            //Botsareus 3/26/2013 applyed caps to gradient
            Gradient.text = val(Gradient.text);
            if (Gradient.text > 200)
            {
                Gradient.text = 200;
            }
            if (Gradient.text < 0)
            {
                Gradient.text = 0;
            }
            tmrLight.IsEnabled = false;
        }

        private void gset_Click(object sender, RoutedEventArgs e) { gset_Click(); }
        private void gset_Click()
        {
            frmGset.instance.tb.Tab = 0;
            frmGset.Show(vbModal, this);
        }

        private void MaxCyclesText_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { MaxCyclesText_Change(); }
        private void MaxCyclesText_Change()
        {
            MaxCycles = val(MaxCyclesText.text);
            if (MaxCycles != 0 & MaxCycles < 2000)
            {
                MaxCycles = 2000;
            }
        }

        private void txtMaxRounds_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { txtMaxRounds_Change(); }
        private void txtMaxRounds_Change()
        {
            Maxrounds = val(txtMaxRounds.text);
            if (Maxrounds < 0)
            {
                Maxrounds = 0;
            }
        }

        private void txtMaxRounds_LostFocus(object sender, RoutedEventArgs e) { txtMaxRounds_LostFocus(); }
        private void txtMaxRounds_LostFocus()
        {
            txtMaxRounds.text = Maxrounds;
        }

        private void MutEnabledCheck_Click(object sender, RoutedEventArgs e) { MutEnabledCheck_Click(); }
        private void MutEnabledCheck_Click()
        {
            if (CurrSpec > -1)
            {
                if ((x_restartmode == 4 || x_restartmode == 5) && y_eco_im > 0 & !TmpOpts.Specie(CurrSpec).Veg)
                { //Botsareus 8/3/2014 At some cases mutations should be disabled
                    MutIsEnabledCheck.IsEnabled = false;
                }
                else
                {
                    MutIsEnabledCheck.IsEnabled = true;
                }

                if (MutEnabledCheck.value == 1)
                {
                    TmpOpts.Specie(CurrSpec).Mutables.Mutations = false;
                    MutRatesBut.IsEnabled = false;
                }
                else
                {
                    TmpOpts.Specie(CurrSpec).Mutables.Mutations = true;
                    MutRatesBut.IsEnabled = true;
                }
            }
        }

        public void PopulateSpeciesList()
        {
            int i = 0;


            SortSpecies();
            // If SpeciesToggle Then 'Botsareus 1/21/2013 Get non-native species via SnapShotSearch
            //    SpeciesLabel = "Native and Non-Native Species:"
            //    NativeSpeciesButton.Caption = "Show Native Species Only"
            //    SpecList.CLEAR
            //    For i = 0 To TmpOpts.SpeciesNum - 1
            //      SpecList.additem (TmpOpts.Specie(i).Name)
            //    Next i
            //  Else
            //    SpeciesLabel = "Native Species:"
            //    NativeSpeciesButton.Caption = "Show Non-Native Species"
            SpecList.CLEAR();
            for (i = 0; i < TmpOpts.SpeciesNum - 1; i++)
            {
                if (TmpOpts.Specie(i).Native)
                {
                    SpecList.additem((TmpOpts.Specie(i).Name));
                }
            }
            //  End If

            SpecList.Refresh();
        }

        private void NativeSpeciesButton_Click(object sender, RoutedEventArgs e) { NativeSpeciesButton_Click(); }//Botsareus 1/21/2013 new code for view non-native speices
        private void NativeSpeciesButton_Click()
        {
            int i = 0;

            string MSG = "";

            bool found = false;

            MSG = "The Non-Native Species are:" + vbCrLf + vbCrLf;
            for (i = 0; i < SimOpts.SpeciesNum - 1; i++)
            {
                if (!SimOpts.Specie(i).Native)
                {
                    MSG = MSG + "\"" + SimOpts.Specie(i).Name + "\", ";
                    found = true;
                }
            }
            MSG = MSG + vbCrLf + vbCrLf + "Save a snapshot and use snapshotsearch.exe to extract DNA. Press CTRL+C to copy this message.";
            if (found)
            {
                MsgBox(MSG);
            }
            else
            {
                MsgBox("There are no non-native species.");
            }
        }

        private void Newseed_Click(object sender, RoutedEventArgs e) { Newseed_Click(); }
        private void Newseed_Click()
        {
            UserSeedText.Text = Timer * 100;
        }

        private void RenameButton_Click(object sender, RoutedEventArgs e) { RenameButton_Click(); }
        private void RenameButton_Click()
        {
            int ind = 0;


            ind = OptionsForm.instance.SpecList.SelectedIndex;
            if (ind >= 0)
            {
                //RenameForm.Show vbModal
            }
            else
            {
                MsgBox(("No Species Selected."));
            }

            PopulateSpeciesList();
        }

        private void ToCosts_Click(object sender, RoutedEventArgs e) { ToCosts_Click(); }
        private void ToCosts_Click()
        {
            CostsForm.Show(vbModal);
            Unload(CostsForm.instance);
        }

        private void CostRadio_Click(object sender, RoutedEventArgs e) { CostRadio_Click(); }
        private void CostRadio_Click(int Index)
        {
            int k = 0;

            switch (Index)
            {
                case 0:  //No Costs
                    ToCosts.IsEnabled = false;
                    for (k = 0; k < 70; k++)
                    {
                        TmpOpts.Costs(k) = 0;
                    }
                    break;//F1 Costs
                case 1:
                    ToCosts.IsEnabled = false;
                    for (k = 0; k < 70; k++)
                    {
                        TmpOpts.Costs(k) = 0;
                    }
                    TmpOpts.Costs(5) = 0.004m;
                    TmpOpts.Costs(7) = 0.04m;
                    TmpOpts.Costs(20) = 0.05m;
                    TmpOpts.Costs(22) = 2;
                    TmpOpts.Costs(23) = 2;
                    TmpOpts.Costs(26) = 0.01m;
                    TmpOpts.Costs(27) = 0.01m;
                    TmpOpts.Costs(28) = 0.1m;
                    TmpOpts.Costs(29) = 0.1m;
                    TmpOpts.Costs(BODYUPKEEP) = 0.00001m;
                    TmpOpts.Costs(AGECOST) = 0.01m;
                    TmpOpts.Costs(COSTMULTIPLIER) = 1;
                    break;//Custom
                case 2:
                    ToCosts.IsEnabled = true;
                    break;
            }
            TmpOpts.CostRadioSetting = Index;
        }

        private void FluidSolidRadio_Click(object sender, RoutedEventArgs e) { FluidSolidRadio_Click(); }
        private void FluidSolidRadio_Click(int Index)
        {
            switch (Index)
            {
                case 0:  //Fluid
                    FrictionCombo.text = FrictionCombo.list(3);
                    FrictionCombo_Click();
                    FrictionCombo.IsEnabled = false;
                    DragCombo.IsEnabled = true;
                    ToPhysics.IsEnabled = false;
                    DragCombo_Click();
                    break;//Solid
                case 1:
                    DragCombo.text = DragCombo.list(3);
                    DragCombo_Click();
                    FrictionCombo.IsEnabled = true;
                    DragCombo.IsEnabled = false;
                    ToPhysics.IsEnabled = false;
                    FrictionCombo_Click();
                    break;//Custom
                case 2:
                    FrictionCombo.IsEnabled = false;
                    DragCombo.IsEnabled = false;
                    ToPhysics.IsEnabled = true;
                    break;
            }
            TmpOpts.FluidSolidCustom = Index;
        }

        private void MaxVelSlider_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { MaxVelSlider_Change(); }
        private void MaxVelSlider_Change()
        {
            TmpOpts.MaxVelocity = MaxVelSlider.value;
        }

        /*
        'Private Sub PauseButton_Click() 'Botsareus 1/5/2013 Can not control pause from settings
        '  Form1.Active = Not Form1.Active
        '  PauseButton.Caption = IIf(Form1.Active, "Unpaused", "Paused")
        'End Sub


        ''''''''''''''''''''''''''''''''

        'Species Tab

        ''''''''''''''''''''''''''''''''
        */
        private void SpecList_Click(object sender, RoutedEventArgs e) { SpecList_Click(); }
        private void SpecList_Click()
        {
            int k = 0;

            int cmaxx = 0;

            int cminx = 0;

            int cmaxy = 0;

            int cminy = 0;

            int w = 0;

            int h = 0;


            w = IPB.Width;
            h = IPB.Height;


            enprop();
            DragEnd();
            k = SpecList.SelectedIndex;
            CurrSpec = k;
            ShowSkin(k);
            speclistchecked = true;
            SpecCol.SelectedIndex = TmpOpts.Specie(k).Colind; //EricL 3/21/2006 - Set the Color Drop Down to match the selected species
            speclistchecked = false;
            Frame1.Caption = WSproperties + TmpOpts.Specie(k).Name;
            SpecQty.text = Str((TmpOpts.Specie(k).qty,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,, + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + +);
            SpecNrg.text = Str((TmpOpts.Specie(k).Stnrg,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,, + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + +);
            Cerchio.FillColor = TmpOpts.Specie(k).color;
            Cerchio.BorderColor = TmpOpts.Specie(k).color;
            Line7.BorderColor = TmpOpts.Specie(k).color;
            Line8.BorderColor = TmpOpts.Specie(k).color;
            Line9.BorderColor = TmpOpts.Specie(k).color;
            if (TmpOpts.Specie(k).Veg)
            {
                SpecVeg.value = 1;
            }
            else
            {
                SpecVeg.value = 0;
            }
            if (TmpOpts.Specie(k).NoChlr)
            { //Botsareus 3/28/2014 Disable chloroplasts
                chkNoChlr.value = 1;
            }
            else
            {
                chkNoChlr.value = 0;
            }

            if (TmpOpts.Specie(k).CantSee)
            {
                DisableVisionCheck.value = 1;
            }
            else
            {
                DisableVisionCheck.value = 0;
            }

            if (TmpOpts.Specie(k).DisableDNA)
            {
                DisableDNACheck.value = 1;
            }
            else
            {
                DisableDNACheck.value = 0;
            }

            if (TmpOpts.Specie(k).CantReproduce)
            {
                DisableReproductionCheck.value = 1;
            }
            else
            {
                DisableReproductionCheck.value = 0;
            }

            if (TmpOpts.Specie(k).DisableMovementSysvars)
            {
                DisableMovementSysvarsCheck.value = 1;
            }
            else
            {
                DisableMovementSysvarsCheck.value = 0;
            }

            if (TmpOpts.Specie(k).VirusImmune)
            {
                VirusImmuneCheck.value = 1;
            }
            else
            {
                VirusImmuneCheck.value = 0;
            }

            if (TmpOpts.Specie(k).Mutables.Mutations)
            {
                MutEnabledCheck.value = 0;
            }
            else
            {
                MutEnabledCheck.value = 1;
            }

            if (TmpOpts.Specie(k).Fixed)
            {
                BlockSpec.value = 1;
            }
            else
            {
                BlockSpec.value = 0;
            }


            CommentBox.text = TmpOpts.Specie(k).Comment;

            if (TmpOpts.Specie(k).Poslf < 0)
            {
                TmpOpts.Specie(k).Poslf = 0;
            }
            if (TmpOpts.Specie(k).Postp < 0)
            {
                TmpOpts.Specie(k).Postp = 0;
            }

            if (TmpOpts.Specie(k).Poslf > 1)
            {
                TmpOpts.Specie(k).Poslf = 0;
            }
            if (TmpOpts.Specie(k).Postp > 1)
            {
                TmpOpts.Specie(k).Postp = 0;
            }

            if (TmpOpts.Specie(k).Posrg > 1)
            {
                TmpOpts.Specie(k).Posrg = 1;
            }
            if (TmpOpts.Specie(k).Posdn > 1)
            {
                TmpOpts.Specie(k).Posdn = 1;
            }

            if (TmpOpts.Specie(k).Posrg < 0)
            {
                TmpOpts.Specie(k).Posrg = 1;
            }
            if (TmpOpts.Specie(k).Posdn < 0)
            {
                TmpOpts.Specie(k).Posdn = 1;
            }

            Initial_Position.Left = TmpOpts.Specie(k).Poslf * w;
            Initial_Position.Top = TmpOpts.Specie(k).Postp * h;

            Initial_Position.Width = (TmpOpts.Specie(k).Posrg * w) - Initial_Position.Left;
            Initial_Position.Height = (TmpOpts.Specie(k).Posdn * h) - Initial_Position.Top;
            Initial_Position.setVisible(true);

            Frame1.Refresh();

        }

        private void AddSpec_Click(object sender, RoutedEventArgs e) { AddSpec_Click(); }
        private void AddSpec_Click()
        {
            // TODO (not supported):   On Error GoTo fine
            CommonDialog1.FileName = "";
            CommonDialog1.Filter = "Dna file(*.txt)|*.txt"; //Botsareus 1/11/2013 DNA only
            CommonDialog1.InitDir = MDIForm1.instance.MainDir + "\\robots";
            CommonDialog1.DialogTitle = WSchoosedna;
            CommonDialog1.ShowOpen();
            if (CommonDialog1.FileName != "")
            { //Botsareus 1/11/2013 Do not insert robot if filename is blank
                additem(CommonDialog1.FileName);
                // TmpOpts.SpeciesNum = TmpOpts.SpeciesNum + 1
                PopulateSpeciesList();
            }
        fine:
}

        private void DuplicaButt_Click(object sender, RoutedEventArgs e) { DuplicaButt_Click(); }//Botsareus 4/30/2013 Fix for the duplicator
        private void DuplicaButt_Click()
        {
            // TODO (not supported):   On Error GoTo fine
            int ind = 0;

            ind = SpecList.SelectedIndex;
            if (ind >= 0 & TmpOpts.Specie(ind).Native)
            {
                //    CommonDialog1.FileName = ""
                //    CommonDialog1.Filter = "Dna file(*.txt)|*.txt"
                //    CommonDialog1.InitDir = MDIForm1.MainDir + "\robots"
                //    CommonDialog1.DialogTitle = WSchoosedna
                //    CommonDialog1.ShowOpen
                //    additem CommonDialog1.FileName
                TmpOpts.SpeciesNum = TmpOpts.SpeciesNum + 1;
                TmpOpts.Specie(TmpOpts.SpeciesNum - 1) == TmpOpts.Specie(ind);
                DispSettings();
            }
            else
            {
                MsgBox(("Sorry, but you can only duplicate bots that originated in this simulation."));
            }
            PopulateSpeciesList();
        fine:
}

        private void DelSpec_Click(object sender, RoutedEventArgs e) { DelSpec_Click(); }
        void DelSpec_Click()
        {
            int ind = 0;
            int k = 0;
            int t = 0;
            int l = 0;

            ind = SpecList.SelectedIndex;
            if (ind >= 0)
            {
                SpecList.RemoveItem(ind);
                k = ind;
                //l = SpecList.ListCount + 1
                for (t = ind; t < SpecList.Items.Count; t++)
                { //Listcount now has one fewer than it did before!!!
                    TmpOpts.Specie(t) = TmpOpts.Specie(t + 1);
                }
                TmpOpts.SpeciesNum = TmpOpts.SpeciesNum - 1;
                disprop();
                //Else
                // MsgBox ("Sorry, but you can only delete bots that originated in this simulation.")
                //Check to make sure our current slected index is still valid
                if (ind >= SpecList.Items.Count)
                {
                    ind = ind - 1;
                }
                if (SpecList.Items.Count > 0)
                {
                    SpecList.SelectedIndex = ind;
                }
            }
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

        void additem(string path)
        {
            int k = 0;
            int t = 0;

            SpecList.additem(extractname(path));
            //k = SpecList.ListCount - 1

            k = TmpOpts.SpeciesNum;
            TmpOpts.SpeciesNum = TmpOpts.SpeciesNum + 1;

            TmpOpts.Specie(k).Colind = 8; //Botsareus 5/8/2012 Changed color CamboBox default value to Random
            TmpOpts.Specie(k).Posrg = 1;
            TmpOpts.Specie(k).Posdn = 1;
            TmpOpts.Specie(k).Poslf = 0;
            TmpOpts.Specie(k).Postp = 0;
            TmpOpts.Specie(k).Name = extractname(path);
            TmpOpts.Specie(k).path = extractpath(path);
            ExtractComment(path, k);
            TmpOpts.Specie(k).path = relpath(TmpOpts.Specie(k).path);
            TmpOpts.Specie(k).Veg = false;
            TmpOpts.Specie(k).CantSee = false;
            TmpOpts.Specie(k).DisableMovementSysvars = false;
            TmpOpts.Specie(k).DisableDNA = false;
            TmpOpts.Specie(k).CantReproduce = false;
            TmpOpts.Specie(k).VirusImmune = false;

            Randomize(); //Botsareus 4/27/2013 Added randomize here so we have interesting colors
            TmpOpts.Specie(k).color = IIf(UseOldColor, RGB(Rnd * 200 + 55, Rnd * 200 + 55, Rnd * 255), RGB(Rnd * 255, Rnd * 255, Rnd * 255));

            //Special overwrites for unique robot names
            if (TmpOpts.Specie(k).Name == "Base.txt")
            {
                TmpOpts.Specie(k).color = vbBlue;
            }
            if (TmpOpts.Specie(k).Name == "Mutate.txt")
            {
                TmpOpts.Specie(k).color = vbRed;
            }
            if (TmpOpts.Specie(k).Name == "robotA.txt")
            {
                TmpOpts.Specie(k).color = RGB(255, 128, 0);
            }
            if (TmpOpts.Specie(k).Name == "robotB.txt")
            {
                TmpOpts.Specie(k).color = RGB(0, 128, 255);
            }
            if (TmpOpts.Specie(k).Name == "Test.txt")
            {
                TmpOpts.Specie(k).color = vbRed;
            }

            Cerchio.FillColor = TmpOpts.Specie(k).color; //Botsareus 4/27/2013 Update ze color on load speicies
            Cerchio.BorderColor = TmpOpts.Specie(k).color;
            Line7.BorderColor = TmpOpts.Specie(k).color;
            Line8.BorderColor = TmpOpts.Specie(k).color;
            Line9.BorderColor = TmpOpts.Specie(k).color;

            CurrSpec = k;
            SetDefaultMutationRates(TmpOpts.Specie(k).Mutables);
            //Botsareus 12/11/2013 Do we have an .mrate file assoicated with this robot?
            string mfname = "";

            mfname = TmpOpts.Specie(k).path + "\\" + extractexactname(TmpOpts.Specie(k).Name) + ".mrate";
            mfname = Replace(mfname, "&#", MDIForm1.instance.MainDir); //Botsareus 1/18/2014 Bugfix
            if (dir(mfname) != "")
            {
                TmpOpts.Specie(k).Mutables = LoadMutationRates(mfname);
            }

            TmpOpts.Specie(k).Mutables.Mutations = true;
            TmpOpts.Specie(k).qty = 5;
            TmpOpts.Specie(k).Stnrg = 3000;
            TmpOpts.Specie(k).Native = true;

            AssignSkin(k, path);
            ShowSkin(k);


        }

        void duplitem(int b, int a)
        {
            int t = 0;


            TmpOpts.Specie(a).Posrg = TmpOpts.Specie(b).Posrg;
            TmpOpts.Specie(a).Posdn = TmpOpts.Specie(b).Posdn;
            TmpOpts.Specie(a).Poslf = TmpOpts.Specie(b).Poslf;
            TmpOpts.Specie(a).Postp = TmpOpts.Specie(b).Postp;
            TmpOpts.Specie(a).Veg = TmpOpts.Specie(b).Veg;
            TmpOpts.Specie(a).Stnrg = TmpOpts.Specie(b).Stnrg;
            TmpOpts.Specie(a).qty = TmpOpts.Specie(b).qty;
            TmpOpts.Specie(a).Fixed = TmpOpts.Specie(b).Fixed;
            TmpOpts.Specie(a).Colind = TmpOpts.Specie(b).Colind;
            TmpOpts.Specie(a).CantSee = TmpOpts.Specie(b).CantSee;
            TmpOpts.Specie(a).DisableMovementSysvars = TmpOpts.Specie(b).DisableMovementSysvars;
            TmpOpts.Specie(a).DisableDNA = TmpOpts.Specie(b).DisableDNA;
            TmpOpts.Specie(a).CantReproduce = TmpOpts.Specie(b).CantReproduce;
            TmpOpts.Specie(a).VirusImmune = TmpOpts.Specie(b).VirusImmune;
            TmpOpts.Specie(a).Mutables = TmpOpts.Specie(b).Mutables;
            TmpOpts.Specie(a).Native = TmpOpts.Specie(b).Native;
        }

        private void ExtractComment(string path, int k)
        {
            // TODO (not supported):   On Error GoTo fine
            bool commend = false;

            string a = "";

            path = stringops.respath(path);
            TmpOpts.Specie(k).Comment = "";
            VBOpenFile(1, path); ;
            While(!EOF(1) && !commend);
            Line(Input(1), a);
            //Debug.Print a
            if (Left(a, 1) == "'" || Left(a, 1) == "/")
            {
                TmpOpts.Specie(k).Comment = TmpOpts.Specie(k).Comment + Mid(a, 2) + vbCrLf;
            }
            else
            {
                commend = true;
            }
            Wend();
            VBCloseFile(1); ();
            return;

        fine:
            TmpOpts.Specie(k).Comment = "ATTENTION!  NOT A VALID BOT FILE.";
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

        /*
        'Botsareus 4/37/2013 Do not need this one also
        'Private Sub ShowSkinO(k As Integer)
        '  Dim t As Integer
        '  Dim x As Long
        '  Dim y As Long
        '  x = Shape2.Left
        '  y = Shape2.Top
        '  multx = Shape2.Width / 120
        '  multy = Shape2.Height / 120
        '  Me.AutoRedraw = True
        '  Shape3.Left = x + TmpOpts.Specie(k).Skin(t) * multx
        '  Shape3.Top = y + TmpOpts.Specie(k).Skin(t + 1) * multy
        '  Shape3.Width = TmpOpts.Specie(k).Skin(t + 2) * multx
        '  Shape3.Height = TmpOpts.Specie(k).Skin(t + 3) * multy
        '  'Shape4.Left = x + specie(k).Skin(t + 4) * multx
        '  'Shape4.Top = Y + specie(k).Skin(t + 5) * multy
        '  'Shape4.Width = specie(k).Skin(t + 6) * multx
        '  'Shape4.Height = specie(k).Skin(t + 7) * multy
        'End Sub

        'Botsareus 4/37/2013 This code never runs anyway
        'Private Sub AssignSkinO(k As Integer)
        '  Dim i As Integer
        '  For i = 0 To 8 Step 4
        '    TmpOpts.Specie(k).Skin(i) = Random(0, 120)
        '    TmpOpts.Specie(k).Skin(i + 2) = Random(0, 120 - TmpOpts.Specie(k).Skin(i))
        '    TmpOpts.Specie(k).Skin(i + 1) = Random(0, 120)
        '    TmpOpts.Specie(k).Skin(i + 3) = Random(0, 120 - TmpOpts.Specie(k).Skin(i + 1))
        '  Next i
        'End Sub
        */
        void AssignSkin(int k, string path)
        { //The new skin engine requires path
          //Botsareus 4/27/2013 The new skin engine


            Randomize(0);

            string robname = "";

            robname = Replace(TmpOpts.Specie(k).Name, ".txt", "");

            decimal newR = 0;

            decimal nextR = 0;

            decimal nameR = 0;

            int X = 0;


            List<decimal> dbls = new List<decimal> { }; // TODO - Specified Minimum Array Boundary Not Supported: Dim dbls() As Double


            List<decimal> dbls_4593_tmp = new List<decimal>();
            for (int redim_iter_4873 = 0; i < 0; redim_iter_4873++) { dbls.Add(0); }
            for (X = 1; X < Len(robname); X++)
            {
                dbls[X - 1] == Rnd(-Asc(Mid(robname, X, 1)));
            } //pre seeds

            for (X = 1; X < Len(robname); X++)
            {
                newR = dbls[X - 1];
                nextR = Rnd(((angle(0, 0, nextR - 0.5, newR - 0.5),,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,, - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -));
            } //randomize by name

            nameR = nextR;
            newR = 0;
            nextR = 0;

            if (MaxRobs == 0)
            {
                List<> rob_9462_tmp = new List<>();
                for (int redim_iter_4015 = 0; i < 0; redim_iter_4015++) { rob.Add(null); }
            }
            if (LoadDNA(path, 0))
            {
                Randomize(0);

                List<decimal> dbls_2390_tmp = new List<decimal>();
                for (int redim_iter_8051 = 0; i < 0; redim_iter_8051++) { dbls.Add(0); }
                for (X = 0; X < UBound(rob(0).dna); X++)
                {
                    dbls[X] = Rnd(((angle(0, 0, Rnd(-rob(0).dna(X).value) - 0.5, Rnd(-rob(0).dna(X).tipo) - 0.5),,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,, - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -));
                } //pre seeds

                for (X = 0; X < UBound(rob(0).dna); X++)
                {
                    newR = dbls[X];
                    nextR = Rnd(((angle(0, 0, nextR - 0.5, newR - 0.5),,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,, - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -));
                } //randomize by dna

                if (MaxRobs == 0)
                {
                    List<> rob_9106_tmp = new List<>();
                    for (int redim_iter_5368 = 0; i < 0; redim_iter_5368++) { rob.Add(null); }
                }

            }

            Randomize(nextR * 1000);

            int i = 0;

            if (k > -1)
            {
                for (i = 0; i < 7 Step 2; i++) {
                    TmpOpts.Specie(k).Skin(i) = Int(Rnd * (half + 1));
                    if (i == 4)
                    {
                        Randomize(nameR * 1000);
                    }
                    TmpOpts.Specie(k).Skin(i + 1) == Int(Rnd * 629);
                }
                Randomize();
                TmpOpts.Specie(k).Skin(6) = (TmpOpts.Specie(k).Skin(6) + Int(Rnd * (half + 1)) * 2) / 3;
            }
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
            int i = 0;

            int xFudge = 0;
            int yFudge = 0;


            bool ShowHandlesBool = false;


            ShowHandlesBool = true;

            if (bShowHandles && !m_CurrCtl == null)
            {
                dynamic _WithVar_871;
                _WithVar_871 = m_DragRect;
                if (!(_WithVar_871.Width < 250 || _WithVar_871.Height < 250))
                {
                    //Save some calculations in variables for speed
                    xFudge = (2 * Screen.TwipsPerPixelX);
                    yFudge = (2 * Screen.TwipsPerPixelY);

                    //Top Left
                    picHandle(0).Move(_WithVar_871.Left - IPB.Left - Frame1.Left - SSTab1.Left) + xFudge,(_WithVar_871.Top - IPB.Top - Frame1.Top) + yFudge - SSTab1.Top;
                    //Top right
                    picHandle(2).Move _WithVar_871.Left - IPB.Left - Frame1.Left - SSTab1.Left + _WithVar_871.Width - picHandle(0).Width - xFudge, _WithVar_871.Top - IPB.Top - Frame1.Top + yFudge - SSTab1.Top;
                    //Bottom left
                    picHandle(6).Move _WithVar_871.Left - IPB.Left - Frame1.Left - SSTab1.Left + xFudge, _WithVar_871.Top + _WithVar_871.Height - picHandle(0).Height - yFudge - IPB.Top - Frame1.Top - SSTab1.Top;
                    //Bottom right
                    picHandle(4).Move(_WithVar_871.Left - IPB.Left - Frame1.Left - SSTab1.Left + _WithVar_871.Width) - picHandle(0).Width - xFudge, _WithVar_871.Top + _WithVar_871.Height - picHandle(0).Height - yFudge - IPB.Top - Frame1.Top - SSTab1.Top;
                }
                else
                {
                    ShowHandlesBool = false;
                }

                RobPlacLine(0).Move _WithVar_871.Left - IPB.Left - Frame1.Left - SSTab1.Left, _WithVar_871.Top - IPB.Top - Frame1.Top - SSTab1.Top, _WithVar_871.Width, 60;
                RobPlacLine(1).Move _WithVar_871.Left - IPB.Left - Frame1.Left - SSTab1.Left, _WithVar_871.Top + _WithVar_871.Height - IPB.Top - Frame1.Top - SSTab1.Top - 60, _WithVar_871.Width, 60;

                RobPlacLine(2).Move _WithVar_871.Left - IPB.Left - Frame1.Left - SSTab1.Left, _WithVar_871.Top - IPB.Top - Frame1.Top - SSTab1.Top, 60, _WithVar_871.Height;
                RobPlacLine(3).Move _WithVar_871.Left + _WithVar_871.Width - IPB.Left - Frame1.Left - SSTab1.Left - 60, _WithVar_871.Top - IPB.Top - Frame1.Top - SSTab1.Top, 60, _WithVar_871.Height;
            }
            //Show or hide each handle
            picHandle(0).Visibility = bShowHandles && ShowHandlesBool;
            picHandle(2).Visibility = bShowHandles && ShowHandlesBool;
            picHandle(6).Visibility = bShowHandles && ShowHandlesBool;
            picHandle(4).Visibility = bShowHandles && ShowHandlesBool;

            RobPlacLine(0).Visibility = bShowHandles;
            RobPlacLine(1).Visibility = bShowHandles;
            RobPlacLine(2).Visibility = bShowHandles;
            RobPlacLine(3).Visibility = bShowHandles;
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
            ActivForm.instance.Visible = false;
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
            xObstacle = Obstacles.Obstacles;
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
            IPB.Cls();
            int o = 0;

            for (o = 1; o < UBound(xObstacle); o++)
            {
                if (xObstacle(o).exist)
                {
                    dynamic _WithVar_1759;
                    _WithVar_1759 = xObstacle(o);
                    IPB.Line(_WithVar_1759.pos.X * IPB.ScaleWidth, _WithVar_1759.pos.Y * IPB.ScaleHeight) - ((_WithVar_1759.pos.X + _WithVar_1759.Width) * IPB.ScaleWidth, (_WithVar_1759.pos.Y + _WithVar_1759.Height) * IPB.ScaleHeight), _WithVar_1759.color, BF);
        }
    }

return;

fine:

  List<> xObstacle_4753_tmp = new List<>();
for (int redim_iter_1869=0;i<0;redim_iter_1869++) {xObstacle.Add(null);}
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

private void LightText_lostfocus()
{
    decimal a = 0;

    a = val(LightText.text);
    if (a < LightUpDn.Min)
    {
        a = LightUpDn.Min;
    }
    if (a > LightUpDn.Max)
    {
        a = LightUpDn.Max;
    }
    LightUpDn.value = a;
    TmpOpts.LightIntensity = a;
    tmrLight.IsEnabled = false;
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

private void DispSettings()
{
    //Botsareus 5/3/2013 No need to change seed if it is fully randomized in any condition
    if (chseedloadsim && chseedstartnew)
    {
        Newseed.IsEnabled = false;
        UserSeedText.IsEnabled = false;
    }

    int t = 0;


    //PauseButton.Caption = IIf(Form1.Active, "Unpaused", "Paused") 'Botsareus 1/5/2013 Can not control pause from settings


    FieldSizeSlide.value = TmpOpts.FieldSize;
    MaxPopText.text = TmpOpts.MaxPopulation;
    MinVegText.text = TmpOpts.MinVegs;
    RepopAmountText.text = TmpOpts.RepopAmount;
    RepopCooldownText.text = TmpOpts.RepopCooldown;
    DecayText.text = TmpOpts.Decay;
    LightText.text = TmpOpts.LightIntensity;
    Gradient.text = (TmpOpts.Gradient - 1) * 10; //Botsareus 12/12/2012 Fix for Gradient
    GradientUpDn.value = Gradient.text * 5;
    //  DNLength.text = TmpOpts.CycleLength
    UserSeedText.text = TmpOpts.UserSeedNumber;
    FrequencyText.text = TmpOpts.Decaydelay;

    FWidthLab.Content = TmpOpts.FieldWidth;
    FHeightLab.Content = TmpOpts.FieldHeight;

    //DisableTiesCheck.value = TmpOpts.DisableTies * True
    TopDownCheck.value = TmpOpts.Updnconnected * true;
    RightLeftCheck.value = TmpOpts.Dxsxconnected * true;

    Pondcheck.value = TmpOpts.Pondmode * true;
    Gradient.IsEnabled = TmpOpts.Pondmode * true;
    LightText.IsEnabled = TmpOpts.Pondmode * true;
    LightUpDn.IsEnabled = TmpOpts.Pondmode * true;
    GradientUpDn.IsEnabled = TmpOpts.Pondmode * true;

    CorpseCheck.value = TmpOpts.CorpseEnabled * true;
    DecayText.IsEnabled = TmpOpts.CorpseEnabled * true;
    DecayUpDn(0).IsEnabled = TmpOpts.CorpseEnabled * true;
    DecayUpDn(1).IsEnabled = TmpOpts.CorpseEnabled * true;

    //EricL 4/11/2006 Added these to initialize the control
    DecayOption(0).IsEnabled = TmpOpts.CorpseEnabled * true;
    DecayOption(1).IsEnabled = TmpOpts.CorpseEnabled * true;
    DecayOption(2).IsEnabled = TmpOpts.CorpseEnabled * true;
    DecayOption(0).value = false;
    DecayOption(1).value = false;
    DecayOption(2).value = false;

    //EricL 4/11/2006 Don't ask me why, but historically, DecayType was stored as DecayOption Index +1
    //So this IF statement is here to gaurd against the case where DecayType has never been set and has value 0 by default.
    if ((TmpOpts.DecayType > 0))
    {
        DecayOption(TmpOpts.DecayType - 1) == true;
    }
    else
    {
        DecayOption(0).IsChecked = true;
    }

    // DNLength.Enabled = TmpOpts.DayNight * True
    //  DNCycleUpDn.Enabled = TmpOpts.DayNight * True
    //  DNCheck.value = TmpOpts.DayNight * True

    //Botsareus 5/3/2013 Replaced by safe mode
    //  UserSeed.value = TmpOpts.UserSeedToggle * True
    //  UserSeedText.Enabled = TmpOpts.UserSeedToggle * True

    FrequencyText.text = TmpOpts.Decaydelay;

    Prop.text = Str(TmpOpts.EnergyProp * 100);
    Fixed.text = (TmpOpts.EnergyFix);
    FixUpDn.value = TmpOpts.EnergyFix;
    ExchangeProp.value = TmpOpts.EnergyExType;
    ExchangeFix.value = !TmpOpts.EnergyExType;

    if (TmpOpts.MutCurrMult <= 0)
    {
        TmpOpts.MutCurrMult = 1;
    }
    MutSlide.value = Log(TmpOpts.MutCurrMult) / Log(2);

    if (TmpOpts.MutCurrMult > 1)
    {
        MutLab.Content = CStr(TmpOpts.MutCurrMult) + " X";
    }
    else
    {
        MutLab.Content = "1/" + Str(2 ^ -MutSlide.value) + " X";
    }

    MutOscill.value = TmpOpts.MutOscill * true;
    MutOscillSin.value = TmpOpts.MutOscillSine * true;

    MutOscillSin.Visibility = MutOscill.value == 1;
    Label3.Visibility = MutOscill.value == 1;
    Label9.Visibility = MutOscill.value == 1;
    CyclesHi.Visibility = MutOscill.value == 1;
    CyclesLo.Visibility = MutOscill.value == 1;
    CycHiUpDn.Visibility = MutOscill.value == 1;
    CycLoUpDn.Visibility = MutOscill.value == 1;


    DisableMutationsCheck.value = TmpOpts.DisableMutations * true;

    CyclesHi.text = TmpOpts.MutCycMax;
    CyclesLo.text = TmpOpts.MutCycMin;


    IntName.text = IntOpts.IName;
    inboundPathText.text = IntOpts.InboundPath;
    outboundPathText.text = IntOpts.OutboundPath;
    txtInttServ.text = IntOpts.ServIP;
    txtInttPort.text = IntOpts.ServPort;
    if (inboundPathText.text == "")
    {
        inboundPathText.text = MDIForm1.instance.MainDir + "\\IM\\inbound";
    }
    if (outboundPathText.text == "")
    {
        outboundPathText.text = MDIForm1.instance.MainDir + "\\IM\\outbound";
    }

    MaxNRGText.text = TmpOpts.MaxEnergy;

    //display F1 page settings
    ContestMode = TmpOpts.F1;
    F1Check.value = ContestMode * true;
    RestartMode = TmpOpts.Restart;
    RestartSimCheck.value = RestartMode * true;


    txtMinRounds.text = optMinRounds;
    txtMaxRounds.text = Maxrounds;
    MaxCyclesText.text = MaxCycles;
    MaxPopF1Text.text = MaxPop;

    SampFreq = 10; //Botsareus 2/11/2014 Sample freq is always 10

    //EricL 5/7/2006 Initialize new UI
    FluidSolidRadio(TmpOpts.FluidSolidCustom).value = true;
    CostRadio(TmpOpts.CostRadioSetting).value = true;


    if (TmpOpts.CoefficientKinetic == 0.75m && TmpOpts.CoefficientStatic == 0.9m && TmpOpts.Zgravity == 4)
    {
        FrictionCombo.text = FrictionCombo.list(0);
    }
    else if (TmpOpts.CoefficientKinetic == 0.4m && TmpOpts.CoefficientStatic == 0.6m && TmpOpts.Zgravity == 2)
    {
        FrictionCombo.text = FrictionCombo.list(1);
    }
    else if (TmpOpts.CoefficientStatic == 0.05m && TmpOpts.CoefficientKinetic == 0.05m && TmpOpts.Zgravity == 1)
    {
        FrictionCombo.text = FrictionCombo.list(2);
    }
    else if (TmpOpts.CoefficientStatic == 0 & TmpOpts.CoefficientKinetic == 0 & TmpOpts.Zgravity == 0)
    {
        FrictionCombo.text = FrictionCombo.list(3);
    }
    else
    {
        FrictionCombo.text = "Custom";
    }

    //needs work
    if (TmpOpts.Viscosity == 0.01m && TmpOpts.Density == 0.0000001m)
    {
        DragCombo.text = DragCombo.list(0);
    }
    else if (TmpOpts.Viscosity == 0.0005m && TmpOpts.Density == 0.0000001m)
    {
        DragCombo.text = DragCombo.list(1);
    }
    else if (TmpOpts.Viscosity == 0.000025m && TmpOpts.Density == 0.0000001m)
    {
        DragCombo.text = DragCombo.list(2);
    }
    else if (TmpOpts.Viscosity == 0 & TmpOpts.Density == 0)
    {
        DragCombo.text = DragCombo.list(3);
    }
    else
    {
        DragCombo.text = "Custom";
    }

    MaxVelSlider.value = TmpOpts.MaxVelocity;
    BodyNrgDist.value = TmpOpts.VegFeedingToBody * 100;

    //EricL 4/1/2006 Added these to initialize values
    ChartInterval.text = TmpOpts.ChartingInterval;
    CustomWaste.text = TmpOpts.BadWastelevel;

    Elasticity.value = TmpOpts.CoefficientElasticity * 10;
    Elasticity.text = TmpOpts.CoefficientElasticity;

    FixBotRadius.value = TmpOpts.FixedBotRadii * true;

    // Botsareus 12/12/2012 SimOpts should never overwrite TmpOpts during display settings
    //Do this so the right CostX gets put back into SimOpts even when no Cost changes are made
    //TmpOpts.Costs(COSTMULTIPLIER) = SimOpts.Costs(COSTMULTIPLIER)

    // EricL Initialize that no species is selected
    CurrSpec = -1;
    SpecVeg.value = 0;
    BlockSpec.value = 0;
    DisableVisionCheck.value = 0;
    DisableMovementSysvarsCheck.value = 0;
    DisableReproductionCheck.value = 0;
    DisableDNACheck.value = 0;
    VirusImmuneCheck.value = 0;

    // Botsareus 12/12/2012 SimOpts should never overwrite TmpOpts during display settings
    //  'So the right value gets put back in when the dialog is closed and tmpopts is copied back into simopts...
    //  'TmpOpts.SpeciesNum = SimOpts.SpeciesNum
    //  For t = 0 To TmpOpts.SpeciesNum - 1
    //    TmpOpts.Specie(t) = SimOpts.Specie(t) ' Population
    //    'TmpOpts.Specie(t).SubSpeciesCounter = SimOpts.Specie(t).SubSpeciesCounter
    //    'TmpOpts.Specie(t).Native = SimOpts.Specie(t).Native
    //  Next t

    //display the scriptlist
    SpecList.Refresh();
    Update();


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
fine:
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
    skipthisspecie:
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
    skipthisspecie3:
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
    skipthisspecie4:
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
    if (!EOF(1) Then: Input 1, TmpOpts.VegFeedingToBody: Else: TmpOpts.VegFeedingToBody == 0.1m) {
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

/*
'  I N T E R N E T   O P T I O N S
'  -------------------------------
*/
private void SaveInternetSett()
{
    // TODO (not supported): On Error GoTo fine
    VBOpenFile(1, MDIForm1.MainDir + "\\intsett.ini"); ;

    //Breaking change as of 2.44.07 - Shasta
    VBWriteFile(1, IntName.text); ;
    VBWriteFile(1, inboundPathText.text); ;
    VBWriteFile(1, outboundPathText.text); ;
    VBWriteFile(1, txtInttServ.text); ;
    VBWriteFile(1, txtInttPort.text); ;
    VBCloseFile(1); ();
    return;

fine:
    MsgBox(("Unable to save internet settings: some error occurred"));
}

public void IntSettLoad()
{
    string a = "";

    a = dir(MDIForm1.instance.MainDir + "\\intsett.ini");
    if (a != "")
    {
        VBOpenFile(1, MDIForm1.MainDir + "\\intsett.ini"); ;

        //Breaking change as of 2.44.07 - Shasta
        Input(1, IntOpts.IName);
        Input(1, IntOpts.InboundPath);
        Input(1, IntOpts.OutboundPath);
        if (!EOF(1))
        {
            Input(1, IntOpts.ServIP);
        }
        if (!EOF(1))
        {
            Input(1, IntOpts.ServPort);
        }
        VBCloseFile(1); ();
    }
}


}
}
