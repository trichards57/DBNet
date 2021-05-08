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
using static Buckets_Module;
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


static class varspecie {
//should the mutation even be triggered?
//probs of the different mutations
//Extras
public class mutationprobs {
 public bool Mutations = false;
 public double[] mutarray = new double[20];
 public double[] Mean = new double[20];
 public double[] StdDev = new double[20];
 public int PointWhatToChange = 0;
 public int CopyErrorWhatToChange = 0;
}
//Botsareus 3/28/2014 Prevents robots from evolving chloroplasts
// Flag indicating eyes should be turned off for this species
// Flag indicating DNA should not execute for this species
// Flag indicating movement sysvars should be disabled for this species
// Flag indicating whether reproduction has been disabled for this species.
// Flag indicating whether members of this species are suceptable to viruses.
// Number of this species in the sim.  Updated each cycle.
// Using to increment the per-bot subspecies.
// Indicates this species was first added locally to this sim and not teleported in from another
public class datispecie {
 public int[] Skin = new int[13];
 public string path = "";
 public string Name = "";
 public int Stnrg = 0;
 public bool Veg = false;
 public bool NoChlr = false;
 public bool Fixed = false;
 public int color = 0;
 public int Colind = 0;
 public decimal Postp = 0;
 public decimal Poslf = 0;
 public decimal Posdn = 0;
 public decimal Posrg = 0;
 public int qty = 0;
 public string Comment = "";
 public string Leaguefilecomment = "";
 public mutationprobs Mutables = null;
 public bool CantSee = false;
 public bool DisableDNA = false;
 public bool DisableMovementSysvars = false;
 public bool CantReproduce = false;
 public bool VirusImmune = false;
 public int population = 0;
 public int SubSpeciesCounter = 0;
 public bool Native = false;
 public Image DisplayImage = null;
 public bool kill_mb = false;
 public bool dq_kill = false;
}
}
