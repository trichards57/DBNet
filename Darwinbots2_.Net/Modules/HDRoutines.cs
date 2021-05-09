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
using System.IO;

static class HDRoutines
{
    // Option Explicit
    //   D I S K    O P E R A T I O N S


    public static void movetopos(string s, int pos)
    { //Botsareus 3/7/2014 Used in Stepladder to move files in specific order

        var files = getfiles(MDIForm1.instance.MainDir + "\\league\\stepladder");
        if (pos > files.Count)
        {
            //just put at end
            FileCopy(s, MDIForm1.instance.MainDir + "\\league\\stepladder\\" + (files.Count + 1) + "-" + extractname(s));
        }
        else
        {
            //move files first
            for (var i = files.Count; i > pos; i--)
            {
                //find a file prefixed i
                for (var j = 1; j < files.Count; j++)
                {
                    var tmpname = extractname(files[j]);
                    if (tmpname == CStr(i) + "-*")
                    {
                        FileCopy(files[j], MDIForm1.instance.MainDir + "\\league\\stepladder\\" + (i + 1) + "-" + Right(tmpname, Len(tmpname) - Len(CStr(i) + "-")));
                        File.Delete(files[j]);
                        break;
                    }
                }
            }
            FileCopy(s, MDIForm1.instance.MainDir + "\\league\\stepladder\\" + pos + "-" + extractname(ref s));
        }

        File.Delete(s);
    }

    public static void deseed(string s)
    { //Botsareus 2/25/2014 Used in Tournament to get back original names of the files and move to result folder
        var lastLine = "";

        var files = getfiles(s);

        for (var i = 1; i < files.Count; i++)
        {
            VBOpenFile(1, files[i]); ; // Open file for input.
            while (!EOF(1))
            { // Check for end of file.
                lastLine = LineInput(1);
            }
            VBCloseFile(1);
            lastLine = Replace(lastLine, "'#tag:", "");
            FileCopy(files[i], MDIForm1.instance.MainDir + "\\league\\Tournament_Results\\" + lastLine);
        }
    }

    public static string NamefileRecursive(string s)
    {//Botsareus 1/31/2014 .txt files only

        var i = Asc("a") - 1;
        var NamefileRecursive = s;
        while (Dir(NamefileRecursive) != "")
        {
            i = i + 1;
            if (Asc("z") < i)
            {
                NamefileRecursive = Replace(s, ".txt", "") + Chr((i - Asc("a")) / 26 + Asc("a") - 1) + Chr((i - Asc("a")) % 26 + Asc("a")) + ".txt";
            }
            else
            {
                NamefileRecursive = Replace(s, ".txt", "") + Chr(i) + ".txt";
            }
        }
        return NamefileRecursive;
    }

    public static void movefilemulti(string source, string Out, int count)
    { //Botsareus 2/18/2014 top/buttom pattern file move

        var last = false;


        for (var i = 1; i < count; i++)
        {
            var files = getfiles(source);
            SortCollection(files, "Moving " + i + " of " + count + " files");
            if (last)
            {
                FileCopy(files[1], Out + "\\" + extractname(files[1]));
                File.Delete(files[1]);
            }
            else
            {
                FileCopy(files[files.Count], Out + "\\" + extractname(files(files.Count)));
                File.Delete(files[files.Count]);
            }
            last = !last;
        }
    }

    public static bool FolderExists(string sFullPath)
    {
        return Directory.Exists(sFullPath);
    }

    /*
    'Botsareus 1/31/2014 Delete this directory and all the files it contains.
    */
    public static void RecursiveRmDir(string dir_name)
    {
        Directory.Delete(dir_name, true);
    }

    /*
    'Botsareus 1/31/2014  stores all files names in folder into Collection
    */
    static List<string> getfiles(string dir_name)
    {
        return Directory.GetFiles(dir_name).ToList();
    }

    private static void SortCollection(Collection ColVar, string moredata)
    { //special code to reorder by name
        Collection oCol = null;

        int i = 0;

        int i2 = 0;

        int iBefore = 0;

        if (!(ColVar == null))
        {
            if (ColVar.Count > 0)
            {
                oCol = new Collection(); ;
                for (i = 1; i < ColVar.Count; i++)
                {
                    if (oCol.Count == 0)
                    {
                        oCol.Add(ColVar[i]);
                    }
                    else
                    {
                        iBefore = 0;
                        for (i2 = oCol.Count; i2 > 1; i2++)
                        {
                            if (Val(extractexactname(extractname(ColVar[i]))) < Val(extractexactname(extractname(oCol(i2)))))
                            {
                                iBefore = i2;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (iBefore == 0)
                        {
                            oCol.Add(ColVar[i]);
                        }
                        else
                        {
                            oCol.Add(ColVar[i]);
                        }
                    }
                    MDIForm1.instance.Caption = "Moving files " + Int(i / ColVar.count * 100) + "% " + moredata;
                    DoEvents();
                }
                ColVar = oCol;
                oCol = null;
            }
        }
    }

    public static bool RecursiveMkDir(string destDir)
    {
        Directory.CreateDirectory(destDir);
    }

    /*
    ' inserts organism file in the simulation
    ' remember that organisms could be made of more than one robot
    */
    public static void InsertOrganism(string path)
    {

        var X = Random(60, SimOpts.FieldWidth - 60); //Botsareus 2/24/2013 bug fix: robots location within screen limits
        var Y = Random(60, SimOpts.FieldHeight - 60);
        LoadOrganism(path, X, Y);
    }

    /*
    ' saves organism file
    */
    public static void SaveOrganism(ref string path, ref int r)
    {
        var clist = new int[51];

        int k = 0;
        int cnum = 0;

        k = 0;
        clist[0] = r;
        ListCells(clist);
        while (clist[cnum] > 0)
            cnum = cnum + 1;

        VBCloseFile(401);
        VBOpenFile(401, path); ;
        FilePut(401, cnum);
        for (k = 0; k < cnum - 1; k++)
        {
            rob[clist[k]].LastOwner = IntOpts.IName;
            SaveRobotBody(401, clist[k]);
        }
        VBCloseFile(401);
        return;

    }

    /*
    'Adds a record to the species array when a bot with a new species is loaded or teleported in
    */
    public static int AddSpecie(ref int n, ref bool IsNative)
    {
        int AddSpecie = 0;
        int k = 0;

        if (rob[n].Corpse || rob[n].FName == "Corpse" || rob[n].exist == false)
        {
            AddSpecie = 0;
            return AddSpecie;

        }

        k = SimOpts.SpeciesNum;
        if (k < MAXNATIVESPECIES)
        {
            SimOpts.SpeciesNum = SimOpts.SpeciesNum + 1;
        }

        SimOpts.Specie[k].Name = rob[n].FName;
        SimOpts.Specie[k].Veg = rob[n].Veg;
        SimOpts.Specie[k].CantSee = rob[n].CantSee;
        SimOpts.Specie[k].DisableMovementSysvars = rob[n].DisableMovementSysvars;
        SimOpts.Specie[k].DisableDNA = rob[n].DisableDNA;
        SimOpts.Specie[k].CantReproduce = rob[n].CantReproduce;
        SimOpts.Specie[k].VirusImmune = rob[n].VirusImmune;
        SimOpts.Specie[k].population = 1;
        SimOpts.Specie[k].SubSpeciesCounter = 0;
        //If rob[n].FName = "Corpse" Then
        //  SimOpts.Specie[k].color = vbBlack
        //Else
        SimOpts.Specie[k].color = rob[n].color;
        //End If
        SimOpts.Specie[k].Comment = "Species arrived from the Internet";
        SimOpts.Specie[k].Posrg = 1;
        SimOpts.Specie[k].Posdn = 1;
        SimOpts.Specie[k].Poslf = 0;
        SimOpts.Specie[k].Postp = 0;

        SetDefaultMutationRates(SimOpts.Specie[k].Mutables);
        SimOpts.Specie[k].Mutables.Mutations = rob[n].Mutables.Mutations;
        SimOpts.Specie[k].qty = 5;
        SimOpts.Specie[k].Stnrg = 3000;
        SimOpts.Specie[k].Native = IsNative;
        //  On Error GoTo bypass
        //  Set robotFile = fso.GetFile(MDIForm1.MainDir + "\robots\" + rob[n].FName)
        // If robotFile.size > 0 Then
        //   SimOpts.Specie[k].Native = True
        //    MDIForm1.Combo1.additem rob[n].FName
        //  End If
        //bypass:
        SimOpts.Specie[k].path = MDIForm1.instance.MainDir + "\\robots";

        // Have to do this becuase of the crazy way SimOpts is NOT copied into TmpOpts when the options dialog is opened
        // TmpOpts.Specie[k] = SimOpts.Specie[k]
        // TmpOpts.SpeciesNum = SimOpts.SpeciesNum

        AddSpecie = k;

        return AddSpecie;
    }

    /*
    ' loads an organism file
    */
    public static int LoadOrganism(ref string path, ref decimal X, ref decimal Y)
    {
        int LoadOrganism = 0;
        var clist = new int[51];

        var OList = new int[51];

        int k = 0;
        int cnum = 0;

        int i = 0;

        int nuovo = 0;

        bool foundSpecies = false;


        // TODO (not supported):   On Error GoTo problem
        VBCloseFile(402);
        VBOpenFile(402, path);
        FileGet(402, ref cnum);
        for (k = 0; k < cnum - 1; k++)
        {
            nuovo = posto();
            clist[k] = nuovo;
            LoadRobot(402, nuovo);
            LoadOrganism = nuovo;
            i = SimOpts.SpeciesNum;
            foundSpecies = false;
            while (i > 0)
            {
                i = i - 1;
                if (rob(nuovo).FName == SimOpts.Specie(i).Name)
                {
                    foundSpecies = true;
                    i = 0;
                }
            }
            if (!foundSpecies)
            {
                AddSpecie(nuovo, false);
            }

        }
        VBCloseFile(402);
        if (X > -1 && Y > -1)
        {
            PlaceOrganism(clist, X, Y);
        }
        RemapTies(clist, OList, cnum);

        return LoadOrganism;
    }

    /*
    ' places an organism (made of robots listed in clist())
    ' in the specified x,y position
    */
    public static void PlaceOrganism(int[] clist, double x, double y)
    {
        int k = 0;

        decimal dx = 0;
        decimal dy = 0;

        k = 0;

        dx = x - rob[clist[0]].pos.X;
        dy = y - rob[clist[0]].pos.Y;
        while (clist[k] > 0)
        {
            rob[clist[k]].pos.X = rob[clist[k]].pos.X + dx;
            rob[clist[k]].pos.Y = rob[clist[k]].pos.Y + dy;
            rob[clist[k]].BucketPos.X = -2;
            rob[clist[k]].BucketPos.Y = -2;
            UpdateBotBucket(clist[k]);
            k = k + 1;
        }
    }

    /*
    ' remaps ties from the old index numbers - those the robots had
    ' when saved to disk- to the new indexes assigned in this simulation
    */
    public static void RemapTies(int[] clist, int[] olist, int cnum)
    {
        int t = 0;
        int ind = 0;
        int j = 0;
        int k = 0;

        bool TiePointsToNode = false;


        for (t = 0; t < cnum - 1; t++)
        { // Loop through each cell
            ind = rob(clist(t)).oldBotNum;
            for (k = 0; k < cnum - 1; k++)
            { // Loop through each cell
                j = 1;
                while (rob[clist[k]].Ties(j).pnt > 0)
                { // Loop through each tie
                    if (rob[clist[k]].Ties(j).pnt == ind)
                    {
                        rob[clist[k]].Ties(j).pnt = clist(t);
                    }
                    j = j + 1;
                }
            }
        }

        for (k = 0; k < cnum - 1; k++)
        { // All cells
            j = 1;
            while (rob[clist[k]].Ties(j).pnt > 0)
            { //All Ties
                TiePointsToNode = false;
                for (t = 0; t < cnum - 1; t++)
                {
                    ind = clist(t);
                    if (rob[clist[k]].Ties(j).pnt == ind)
                    {
                        TiePointsToNode = true;
                    }
                }
                if (!TiePointsToNode)
                {
                    rob[clist[k]].Ties(j).pnt = 0;
                }
                j = j + 1;
            }
        }
    }

    public static dynamic RemapAllTies(ref int numOfBots_UNUSED)
    {
        dynamic RemapAllTies = null;
        int i = 0;

        int j = 0;

        int k = 0;


        for (i = 1; i < numOfBots; i++)
        {
            j = 1;
            while (rob(i).Ties(j).pnt > 0)
            { // Loop through each tie
                for (k = 1; k < numOfBots; k++)
                {
                    if (rob(i).Ties(j).pnt == rob(k).oldBotNum)
                    {
                        rob(i).Ties(j).pnt = k;
                        goto ;
                    }
                }
            nexttie:
                j = j + 1;
            }
        }
        return RemapAllTies;
    }

    public static dynamic RemapAllShots(int numOfShots)
    {
        dynamic RemapAllShots = null;
        int i = 0;

        int j = 0;


        for (i = 1; i < numOfShots; i++)
        {
            if (Shots[i].exist)
            {
                for (j = 1; j < MaxRobs; j++)
                {
                    if (rob[j].exist)
                    {
                        if (Shots[i].parent == rob[j].oldBotNum)
                        {
                            Shots[i].parent = j;
                            if (Shots[i].stored)
                            {
                                rob[j].virusshot = i;
                            }
                            goto nextshot;
                        }
                    }
                }
                Shots[i].stored = false; // Could not find parent.  Should probalby never happen but if it does, release the shot
            }
        nextshot:;
        }
        return RemapAllShots;
    }

    /*
    'Saves a small file with per species population informaton
    'Used for aggregating the population stats from multiple connected sims
    */
    public static void SaveSimPopulation(ref string path)
    {
        int X = 0;

        int numSpecies = 0;

        const byte Fe = 254;


        Form1.instance.MousePointer = vbHourglass;
        // TODO (not supported):   On Error GoTo bypass

        VBOpenFile(10, path); ;

        FilePut(10, Len(IntOpts.IName));
        FilePut(10, IntOpts.IName);

        numSpecies = 0;
        for (X = 0; X < SimOpts.SpeciesNum - 1; X++)
        {
            if (SimOpts.Specie[x].population > 0)
            {
                numSpecies = numSpecies + 1;
            }
        }

        FilePut(10, numSpecies); // Only save non-zero populations


        for (X = 0; X < SimOpts.SpeciesNum - 1; X++)
        {
            if (SimOpts.Specie[x].population > 0)
            {
                FilePut(10, Len(SimOpts.Specie[x].Name));
                FilePut(10, SimOpts.Specie[x].Name);
                FilePut(10, SimOpts.Specie[x].population);
                FilePut(10, SimOpts.Specie[x].Veg);
                FilePut(10, SimOpts.Specie[x].color);

                //write any future data here

                //Record ending bytes
                FilePut(10, Fe);
                FilePut(10, Fe);
                FilePut(10, Fe);
            }

        }


        VBCloseFile(10);
        Form1.instance.MousePointer = vbArrow;

    }

    public static string GetFilePath(ref string FileName)
    {
        var fi = new FileInfo(FileName);
        return fi.Name;
    }

    /*
    ' saves a whole simulation
    */
    public static void SaveSimulation(ref string path)
    {
        // TODO (not supported): On Error GoTo tryagain
        int t = 0;

        int n = 0;

        int X = 0;

        int j = 0;

        string s2 = "";

        string temp = "";

        int numOfExistingBots = 0;


        Form1.MousePointer = vbHourglass;

        numOfExistingBots = 0;

        for (X = 1; X < MaxRobs; X++)
        {
            if (rob(X).exist)
            {
                numOfExistingBots = numOfExistingBots + 1;
            }
            Next(X);

            string justPath = "";

            justPath = GetFilePath(ref path);

            RecursiveMkDir((justPath));

            VBCloseFile(1); ();
            VBOpenFile(1, path); ;

            Put(#1);

    Form1.lblSaving.Visible = true; //Botsareus 1/14/2014 New code to display save status

            for (t = 1; t < MaxRobs; t++)
            {
                if (rob(t).exist)
                {
                    SaveRobotBody(1, t);
                }
                if (t % 20 == 0)
                {
                    Form1.lblSaving.Caption = "Saving... (" + Int(t / MaxRobs * 100) + "%)"; //Botsareus 1/14/2014
                    DoEvents();
                }
                Next(t);

                Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);

//new stuff

      Put(#1);

//Put #1, , SimOpts.KineticEnergy
      Put(#1);

      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);

//new new stuff

      Put(#1);
      Put(#1);

//obsolete
      Put(#1);

      Put(#1);
      Put(#1);

//even even newer newer stuff
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);
      Put(#1);

      Put(#1);

      int k = 0;

                for (k = 0; k < SimOpts.SpeciesNum - 1; k++)
                {
                    Put(#1);
        Put(#1);
        Put(#1);
        Put(#1);
        Put(#1);
        Put(#1);
        Put(#1);

//obsolete, so we do this instead
//Put #1, , SimOpts.Specie[k].omnifeed
        Put(#1);

        Put(#1);
        Put(#1);

        Put(#1);
        Put(#1);
        Put(#1);
        Put(#1);

//Put #1, , SimOpts.Specie[k].Posdn
//Put #1, , SimOpts.Specie[k].Poslf
//Put #1, , SimOpts.Specie[k].Posrg
//Put #1, , SimOpts.Specie[k].Postp

        Put(#1);
        Put(#1);
        Put(#1);
        Put(#1);
        Next(k);

                    Put(#1);
        Put(#1);
        Put(#1);
        Put(#1);
        Put(#1);
        Put(#1);
        Put(#1);
        Put(#1);

//New for 2.4:
        for (k = 0; k < SimOpts.SpeciesNum - 1; k++)
                    {
                        Put(#1);
          Put(#1);

          int h = 0;


                        for (h = 0; h < 20; h++)
                        {
                            Put(#1);
            Put(#1);
            Next(h);

                            //Put #1, , SimOpts.p
                            Next(k);

                            for (k = 0; k < 70; k++)
                            {
                                Put(#1);
              Next(k);

                                //EricL 4/1/2006 Fixed bug below by added -1.  Loop was executing one too many times...
                                for (k = 0; k < SimOpts.SpeciesNum - 1; k++)
                                {
                                    Put(#1);
                Put(#1);
                Put(#1);
                Put(#1);
                Next(k);

                                    Put(#1); //EricL 4/1/2006 Added this
                Put(#1); //EricL 4/1/2006 Added this
                Put(#1); //EricL 4/29/2006 Added this
                Put(#1); // EricL 5/7/2006
                Put(#1); // EricL 5/7/2006
                Put(#1); // EricL 5/15/2006
                Put(#1); // EricL 6/8/2006
                Put(#1); //EricL 6/8/2006 Added this
                Put(#1); //EricL 6/8/2006 Added this
                Put(#1); //EricL 6/8/2006 Added this
                Put(#1); //EricL 6/8/2006 Added this
                Put(#1);
                Put(#1);
                Put(#1);
                Put(#1);
                Put(#1);
                Put(#1);

                Put(#1);

                for (X = 1; X < numTeleporters; X++)
                                    {
                                        SaveTeleporter(1, X);
                                        Next(X);

                                        Put(#1);

                  for (X = 1; X < numObstacles; X++)
                                        {
                                            SaveObstacle(1, X);
                                            Next(X);

                                            Put(#1);

                    for (k = 0; k < SimOpts.SpeciesNum - 1; k++)
                                            {
                                                Put(#1);
                      Put(#1);
                      Put(#1);
                      Next(k);

                                                Put(#1);
                      Put(#1);
                      Put(#1);
                      Put(#1);
                      Put(#1);
                      Put(#1);
                      Put(#1);
                      Put(#1);

                      for (k = 0; k < SimOpts.SpeciesNum - 1; k++)
                                                {
                                                    Put(#1);
                        Next(k);

                                                    Put(#1);

                        for (j = 1; j < maxshotarray; j++)
                                                    {
                                                        SaveShot(1, j);
                                                        Next(j);

                                                        Put(#1);

                          for (k = 0; k < SimOpts.SpeciesNum - 1; k++)
                                                        {
                                                            Put(#1);
                            Next(k);

                                                            for (k = 0; k < SimOpts.SpeciesNum - 1; k++)
                                                            {
                                                                Put(#1);
                              Put(#1);
                              Next(k);

                                                                for (k = 0; k < SimOpts.SpeciesNum - 1; k++)
                                                                {
                                                                    Put(#1);
                                Next(k);

                                                                    Put(#1);
                                Put(#1);
                                Put(#1);
                                Put(#1);
                                Put(#1);
                                Put(#1);
                                Put(#1);
                                Put(#1);
                                Put(#1);
                                Put(#1);

//Botsareus 4/17/2013
                                Put(#1);

//Botsareus 5/31/2013 Save all graph data
//strings
                                Put(#1);
                                Put(#1);
                                Put(#1);
                                Put(#1);
                                Put(#1);
                                Put(#1);
                                Put(#1);
                                Put(#1);

//the graphs themselfs
                                for (k = 1; k < NUMGRAPHS; k++)
                                                                    {
                                                                        Put(#1);
                                  Put(#1);
                                  Put(#1);
                                  Put(#1);
                                  Put(#1);
                                  Next(k);

                                                                        Put(#1); //Botsareus 9/28/2013

//evo stuff
                                  Put(#1);
                                  Put(#1);
                                  Put(#1);
                                  Put(#1);
                                  Put(#1);
                                  Put(#1);
                                  Put(#1);
                                  Put(#1);
                                  Put(#1);

//some mor simopts stuff
                                  Put(#1);

//Botsareus 8/5/2014
                                  Put(#1);

//Botsareus 8/16/2014
                                  Put(#1);
                                  Put(#1);
                                  Put(#1);

//Botsareus 10/13/2014
                                  Put(#1);
                                  Put(#1);

//Botsareus 10/8/2015
                                  Put(#1);

//Botsareus 10/20/2015
                                  Put(#1);

                                  Form1.lblSaving.Visible = false; //Botsareus 1/14/2014

                                                                        VBCloseFile(1); ();
                                                                        Form1.MousePointer = vbArrow;
                                                                        return;

                                                                    tryagain:
                                                                        SaveSimulation(path);
                                                                    }

                                                                    /*
                                                                    'Botsareus 3/15/2013 load global settings
                                                                    */
                                                                    public static void LoadGlobalSettings()
                                                                    {
                                                                        //defaults
                                                                        bodyfix = 32100;
                                                                        chseedstartnew = true;
                                                                        chseedloadsim = true;
                                                                        GraphUp = false;
                                                                        HideDB = false;
                                                                        MDIForm1.instance.MainDir = App.path;
                                                                        UseSafeMode = true; //Botsareus 10/5/2015
                                                                        UseEpiGene = false; //Botsareus 10/8/2015
                                                                        UseIntRnd = false; //Botsareus 10/8/2015
                                                                        intFindBestV2 = 100;
                                                                        UseOldColor = true;
                                                                        //mutations tab
                                                                        epiresetemp = 1.3m;
                                                                        epiresetOP = 17;
                                                                        //Delta2
                                                                        Delta2 = false;
                                                                        DeltaMainExp = 1;
                                                                        DeltaMainLn = 0;
                                                                        DeltaDevExp = 7;
                                                                        DeltaDevLn = 1;
                                                                        DeltaPM = 3000;
                                                                        DeltaWTC = 15;
                                                                        DeltaMainChance = 100;
                                                                        DeltaDevChance = 30;
                                                                        //Normailize mutation rates
                                                                        NormMut = false;
                                                                        valNormMut = 1071;
                                                                        valMaxNormMut = 1071;
                                                                        string holdmaindir = "";


                                                                        y_hidePredCycl = 1500;
                                                                        y_LFOR = 10;

                                                                        y_zblen = 255;

                                                                        //see if maindir overwrite exisits
                                                                        if (dir(App.path + "\\Maindir.gset") != "")
                                                                        {
                                                                            //load the new maindir
                                                                            VBOpenFile(1, App.path + "\\Maindir.gset"); ;
                                                                            Input(#1, holdmaindir);
    VBCloseFile(1); ();
                                                                            if (dir(holdmaindir + "\\", vbDirectory) != "")
                                                                            { //Botsareus 6/11/2013 small bug fix to do with no longer finding a main directory
                                                                                MDIForm1.instance.MainDir = holdmaindir;
                                                                            }
                                                                        }

                                                                        leagueSourceDir = MDIForm1.instance.MainDir + "\\Robots\\F1league";

                                                                        //see if eco exsists
                                                                        y_eco_im = 0;
                                                                        if (dir(App.path + "\\im.gset") != "")
                                                                        {
                                                                            VBOpenFile(1, App.path + "\\im.gset"); ;
                                                                            Input(#1, y_eco_im);
    VBCloseFile(1); ();
                                                                            y_eco_im = y_eco_im + 1;
                                                                        }

                                                                        //see if restartmode exisit

                                                                        if (dir(App.path + "\\restartmode.gset") != "")
                                                                        {
                                                                            VBOpenFile(1, App.path + "\\restartmode.gset"); ;
                                                                            Input(#1, x_restartmode);
    Input(#1, x_filenumber);
    VBCloseFile(1); ();
                                                                        }

                                                                        //see if settings exsist
                                                                        if (dir(MDIForm1.instance.MainDir + "\\Global.gset") != "")
                                                                        {
                                                                            //load all settings
                                                                            VBOpenFile(1, MDIForm1.MainDir + "\\Global.gset"); ;
                                                                            Input(#1, screenratiofix);
    if (!EOF(1))
                                                                            {
                                                                                Input(#1, bodyfix);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, reprofix);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, chseedstartnew);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, chseedloadsim);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, UseSafeMode);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, intFindBestV2);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, UseOldColor);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, boylabldisp);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, startnovid);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, epireset);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, epiresetemp);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, epiresetOP);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, sunbelt);
    }

                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, Delta2);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, DeltaMainExp);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, DeltaMainLn);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, DeltaDevExp);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, DeltaDevLn);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, DeltaPM);
    }

                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, NormMut);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, valNormMut);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, valMaxNormMut);
    }

                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, DeltaWTC);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, DeltaMainChance);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, DeltaDevChance);
    }

                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, leagueSourceDir);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, UseStepladder);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, x_fudge);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, StartChlr);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, Disqualify);
    }

                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, y_robdir);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, y_graphs);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, y_normsize);
    }
                                                                            //Botsareus 10/6/2015 Overwrite y_normsize
                                                                            if (x_restartmode < 4 || x_restartmode == 10)
                                                                            {
                                                                                y_normsize = false;
                                                                            }

                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, y_hidePredCycl);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, y_LFOR);
    }

                                                                            bool unused = false;

                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, unused);
    }

                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, y_zblen);
    }

                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, x_res_kill_chlr);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, x_res_kill_mb);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, x_res_other);
    }

                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, y_res_kill_chlr);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, y_res_kill_mb);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, y_res_kill_dq);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, y_res_other);
    }

                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, x_res_kill_mb_veg);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, x_res_other_veg);
    }

                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, y_res_kill_mb_veg);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, y_res_kill_dq_veg);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, y_res_other_veg);
    }

                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, GraphUp);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, HideDB);
    }

                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, UseEpiGene);
    }

                                                                            if (!EOF(1))
                                                                            {
                                                                                Input(#1, UseIntRnd);
    }

                                                                            VBCloseFile(1); ();
                                                                        }

                                                                        //some global settings change during simulation (copy is here)
                                                                        loadboylabldisp = boylabldisp;
                                                                        loadstartnovid = startnovid;

                                                                        //see if safemode settings exisit
                                                                        if (dir(App.path + "\\Safemode.gset") != "")
                                                                        {
                                                                            //load all settings
                                                                            VBOpenFile(1, App.path + "\\Safemode.gset"); ;
                                                                            Input(#1, simalreadyrunning);
    VBCloseFile(1); ();
                                                                        }


                                                                        //see if autosaved file exisit
                                                                        if (dir(App.path + "\\autosaved.gset") != "")
                                                                        {
                                                                            //load all settings
                                                                            VBOpenFile(1, App.path + "\\autosaved.gset"); ;
                                                                            Input(#1, autosaved);
    VBCloseFile(1); ();
                                                                        }

                                                                        //Botsareus  10/31/2015 Moved for bug fix
                                                                        //If we are not using safe mode assume simulation is not runnin'
                                                                        if (UseSafeMode == false)
                                                                        {
                                                                            simalreadyrunning = false;
                                                                        }

                                                                        if (simalreadyrunning == false)
                                                                        {
                                                                            autosaved = false;
                                                                        }

                                                                        //Botsareus 3/16/2014 If autosaved, we change restartmode, this forces system to run in diagnostic mode
                                                                        //The difference between x_restartmode 0 and 5 is that 5 uses hidepred settings
                                                                        if (autosaved && x_restartmode == 4)
                                                                        {
                                                                            x_restartmode = 5;
                                                                            MDIForm1.instance.y_info.setVisible(true);
                                                                        }
                                                                        if (autosaved && x_restartmode == 7)
                                                                        {
                                                                            x_restartmode = 8; //Botsareus 4/14/2014 same deal for zb evo
                                                                            intFindBestV2 = 20 + Rnd(-(x_filenumber + 1)) * 40; //Botsareus 10/26/2015 Value more interesting
                                                                        }

                                                                        //Botsareus 3/19/2014 Load data for evo mode
                                                                        if (x_restartmode == 4 || x_restartmode == 5 || x_restartmode == 6)
                                                                        {
                                                                            VBOpenFile(1, MDIForm1.MainDir + "\\evolution\\data.gset"); ;
                                                                            Input(#1, LFOR); //LFOR init
    Input(#1, LFORdir); //dir
    Input(#1, LFORcorr); //corr

    Input(#1, hidePredCycl); //hidePredCycl

    Input(#1, curr_dna_size); //curr_dna_size
    Input(#1, target_dna_size); //target_dna_size

    Input(#1, Init_hidePredCycl);

    Input(#1, y_Stgwins);
    VBCloseFile(1); ();
                                                                        }
                                                                        else
                                                                        {
                                                                            y_eco_im = 0;
                                                                        }

                                                                        //Botsareus 3/22/2014 Initial hidepred offset is normal

                                                                        hidePredOffset = hidePredCycl / 6;

                                                                        if (UseIntRnd)
                                                                        {
                                                                            //Use pictures from internet as randomizer
                                                                            cprndy = 0;
                                                                            List<> rndylist_4922_tmp = new List<>();
                                                                            for (int redim_iter_4002 = 0; i < 3999; redim_iter_4002++) { rndylist.Add(null); }
                                                                            MDIForm1.instance.grabfile();
                                                                        }

                                                                    }

                                                                    /*
                                                                    ' loads a whole simulation
                                                                    */
                                                                    public static void LoadSimulation(ref string path)
                                                                    {
                                                                        Form1.camfix = false; //Botsareus 2/23/2013 When simulation starts the screen is normailized

                                                                        //Because of the way that loadrobot and saverobot work, all save and load
                                                                        //sim routines are backwards and forwards compatible after 2.37.2
                                                                        //(not 2.37.2, but everything that comes after)
                                                                        int j = 0;

                                                                        int k = 0;

                                                                        int X = 0;

                                                                        int t = 0;

                                                                        decimal s = 0;//EricL 4/1/2006 Use this to read in single values

                                                                        bool tempbool = false;

                                                                        int tempint = 0;

                                                                        string temp = "";

                                                                        string s2 = "";


                                                                        Form1.MousePointer = vbHourglass;

                                                                        //For k = 0 To MaxRobs
                                                                        //  Erase rob(k).DNA()
                                                                        //  ReDim rob(k).DNA(1)
                                                                        //  rob(k).exist = False
                                                                        //Next k
                                                                        //Erase rob()
                                                                        //Init_Buckets

                                                                        VBOpenFile(1, path); ;

                                                                        //As of 2.42.8, indicates a value less than the "real" MaxRobs, not a high water mark, since only existing bots are stored post 2.42.8
                                                                        Get(#1);

//Round up to the next multiple of 500
  List <> rob_973_tmp = new List<>();
                                                                        for (int redim_iter_6126 = 0; i < 0; redim_iter_6126++) { rob.Add(null); }

                                                                        Form1.lblSaving.Visible = true; //Botsareus 1/14/2014 New code to display load status
                                                                        Form1.Visible = true;

                                                                        for (k = 1; k < MaxRobs; k++)
                                                                        {
                                                                            LoadRobot(1, k);
                                                                            if (k % 20 == 0)
                                                                            {
                                                                                Form1.lblSaving.Caption = "Loading... (" + Int(k / MaxRobs * 100) + "%)"; //Botsareus 1/14/2014
                                                                                DoEvents();
                                                                            }
                                                                            Next(k);

                                                                            // As of 2.42.8, the sim file is packed.  Every bot stored is guarenteed to exist, yet their bot numbers, when loaded, may be
                                                                            // different from the sim they came from.  Thus, we remap all the ties from all the loaded bots.
                                                                            RemapAllTies(MaxRobs);


                                                                            Get(#1);
    temp = Space(k);
                                                                            Get(#1);
    Get(#1);
    Get(#1);
    temp = Space(k);
                                                                            Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    temp = Space(k);
                                                                            Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    SimOpts.SimName = Space(Abs(k));
                                                                            Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
//Get #1, , SimOpts.KineticEnergy
    Get(#1); //dummy variable
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);
    Get(#1);

//obsolete
    Get(#1);

    Get(#1);
    Get(#1);

//newer stuff
    if (!EOF(1))
                                                                            {
                                                                                Get(#1);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Get(#1);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Get(#1);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Get(#1);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Get(#1);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Get(#1);
    }
                                                                            if (!EOF(1))
                                                                            {
                                                                                Get(#1);
    }

                                                                            if (!EOF(1))
                                                                            {
                                                                                Get(#1);
    }

                                                                            for (k = 0; k < SimOpts.SpeciesNum - 1; k++)
                                                                            {
                                                                                if (!EOF(1))
                                                                                {
                                                                                    Get(#1);
      }
                                                                                if (!EOF(1))
                                                                                {
                                                                                    Get(#1);
      }
                                                                                if (!EOF(1))
                                                                                {
                                                                                    Get(#1);
      }
                                                                                if (!EOF(1))
                                                                                {
                                                                                    Get(#1);
      }
                                                                                if (!EOF(1))
                                                                                {
                                                                                    Get(#1);
      }
                                                                                if (!EOF(1))
                                                                                {
                                                                                    Get(#1);
        SimOpts.Specie[k].Name = Space(Abs(j));
                                                                                }
                                                                                if (!EOF(1))
                                                                                {
                                                                                    Get(#1);
      }

                                                                                //obsolete
                                                                                //If Not EOF(1) Then Get #1, , SimOpts.Specie[k].omnifeed
                                                                                if (!EOF(1))
                                                                                {
                                                                                    Get(#1);
      }

                                                                                if (!EOF(1))
                                                                                {
                                                                                    Get(#1);
        SimOpts.Specie[k].path = Space(j);
                                                                                }
                                                                                if (!EOF(1))
                                                                                {
                                                                                    Get(#1);

//Botsareus 8/21/2012 Had to dump this, VERY BUGY!
//        'New for 2.42.5.  Insure the path points to our main directory. It might be a sim that was saved before hand on a different machine.
//        'First, we strip off the working directory portion of the robot path
//        'We have to do it this way since the sim could have come from a different machine with a different install directory
//        temp = SimOpts.Specie[k].path
//        s2 = Left(temp, 7)
//        While s2 <> "\Robots" And Len(temp) > 7
//          temp = Right(temp, Len(temp) - 1)
//          s2 = Left(temp, 7)
//        Wend
//        SimOpts.Specie[k].path = temp

                                                                                    //        'Now we add on the main directory to get the full path.  The sim may have come from a different machine, but at least
                                                                                    //        'now the path points to the right main directory...
                                                                                    //        SimOpts.Specie[k].path = MDIForm1.MainDir + SimOpts.Specie[k].path
                                                                                }

                                                                                if (!EOF(1))
                                                                                {
                                                                                    Get(#1); //SimOpts.Specie[k].Posdn 'EricL 4/1/06 Changed these to use the variable s
      }
                                                                                if (!EOF(1))
                                                                                {
                                                                                    Get(#1); //SimOpts.Specie[k].Poslf
      }
                                                                                if (!EOF(1))
                                                                                {
                                                                                    Get(#1); //SimOpts.Specie[k].Posrg
      }
                                                                                if (!EOF(1))
                                                                                {
                                                                                    Get(#1); //SimOpts.Specie[k].Postp
      }

                                                                                SimOpts.Specie[k].Posdn = 1;
                                                                                SimOpts.Specie[k].Posrg = 1;
                                                                                SimOpts.Specie[k].Poslf = 0;
                                                                                SimOpts.Specie[k].Postp = 0;

                                                                                if (!EOF(1))
                                                                                {
                                                                                    Get(#1);
      }
                                                                                if (!EOF(1))
                                                                                {
                                                                                    Get(#1);
      }
                                                                                if (!EOF(1))
                                                                                {
                                                                                    Get(#1);
      }
                                                                                if (!EOF(1))
                                                                                {
                                                                                    Get(#1);
      }
                                                                                Next(k);

                                                                                if (!EOF(1))
                                                                                {
                                                                                    Get(#1);
      }
                                                                                if (!EOF(1))
                                                                                {
                                                                                    Get(#1);
      }

                                                                                //New for 2.4
                                                                                if (!EOF(1))
                                                                                {
                                                                                    Get(#1);
      }
                                                                                if (!EOF(1))
                                                                                {
                                                                                    Get(#1);
      }
                                                                                if (!EOF(1))
                                                                                {
                                                                                    Get(#1);
      }
                                                                                if (!EOF(1))
                                                                                {
                                                                                    Get(#1);
      }
                                                                                if (!EOF(1))
                                                                                {
                                                                                    Get(#1);
      }
                                                                                if (!EOF(1))
                                                                                {
                                                                                    Get(#1);
      }

                                                                                //EricL - 4/1/06 Fixed bug by adding -1.  Loop was executing one too many times...
                                                                                for (k = 0; k < SimOpts.SpeciesNum - 1; k++)
                                                                                {
                                                                                    if (!EOF(1))
                                                                                    {
                                                                                        Get(#1);
        }
                                                                                    if (!EOF(1))
                                                                                    {
                                                                                        Get(#1);
        }

                                                                                    for (j = 0; j < 20; j++)
                                                                                    {
                                                                                        if (!EOF(1))
                                                                                        {
                                                                                            Get(#1);
          }
                                                                                        if (!EOF(1))
                                                                                        {
                                                                                            Get(#1);
          }
                                                                                        Next(j);
                                                                                        Next(k);

                                                                                        for (k = 0; k < 70; k++)
                                                                                        {
                                                                                            if (!EOF(1))
                                                                                            {
                                                                                                Get(#1);
            }
                                                                                            Next(k);

                                                                                            for (k = 0; k < SimOpts.SpeciesNum - 1; k++)
                                                                                            {
                                                                                                if (!EOF(1))
                                                                                                {
                                                                                                    Get(#1);
              }
                                                                                                if (!EOF(1))
                                                                                                {
                                                                                                    Get(#1);
              }
                                                                                                if (!EOF(1))
                                                                                                {
                                                                                                    Get(#1);
              }
                                                                                                if (!EOF(1))
                                                                                                {
                                                                                                    Get(#1);
              }
                                                                                                Next(k);

                                                                                                if (!EOF(1))
                                                                                                {
                                                                                                    Get(#1); //EricL 4/1/2006 Added this
              }
                                                                                                //EricL 4/1/2006 Default value so as to avoid divide by zero problems when loading older saved sim files
                                                                                                if (SimOpts.BadWastelevel == 0)
                                                                                                {
                                                                                                    SimOpts.BadWastelevel = 400;
                                                                                                }

                                                                                                if (!EOF(1))
                                                                                                {
                                                                                                    Get(#1); //EricL 4/1/2006 Added this
              }
                                                                                                //EricL May be cases where 0 is read from old format files which can cause divide by 0 problems later
                                                                                                if (SimOpts.chartingInterval <= 0 || SimOpts.chartingInterval > 32000)
                                                                                                {
                                                                                                    SimOpts.chartingInterval = 200;
                                                                                                }

                                                                                                SimOpts.CoefficientElasticity = 0; //Set a reasonable value for older saved sim files
                                                                                                if (!EOF(1))
                                                                                                {
                                                                                                    Get(#1); //EricL 4/29/2006 Added this
              }

                                                                                                SimOpts.FluidSolidCustom = 2; //Set to custom as a default value for older saved sim files
                                                                                                if (!EOF(1))
                                                                                                {
                                                                                                    Get(#1); //EricL 5/7/2006 Added this for UI initialization
              }
                                                                                                if (SimOpts.FluidSolidCustom < 0 || SimOpts.FluidSolidCustom > 2)
                                                                                                {
                                                                                                    SimOpts.FluidSolidCustom = 2;
                                                                                                }

                                                                                                SimOpts.CostRadioSetting = 2; //Set to custom as a default value for older saved sim files
                                                                                                if (!EOF(1))
                                                                                                {
                                                                                                    Get(#1); //EricL 5/7/2006 Added this for UI initialization
              }
                                                                                                if (SimOpts.CostRadioSetting < 0 || SimOpts.CostRadioSetting > 2)
                                                                                                {
                                                                                                    SimOpts.CostRadioSetting = 2;
                                                                                                }

                                                                                                SimOpts.MaxVelocity = 40; //Set to a reasonable default value for older saved sim files
                                                                                                if (!EOF(1))
                                                                                                {
                                                                                                    Get(#1); //EricL 5/16/2006 Added this - was not saved before
              }
                                                                                                if (SimOpts.MaxVelocity <= 0 || SimOpts.MaxVelocity > 200)
                                                                                                {
                                                                                                    SimOpts.MaxVelocity = 40;
                                                                                                }

                                                                                                SimOpts.NoShotDecay = false; //Set to a reasonable default value for older saved sim files
                                                                                                if (!EOF(1))
                                                                                                {
                                                                                                    Get(#1); //EricL 6/8/2006 Added this
              }

                                                                                                SimOpts.SunUpThreshold = 500000; //Set to a reasonable default value for older saved sim files
                                                                                                if (!EOF(1))
                                                                                                {
                                                                                                    Get(#1); //EricL 6/8/2006 Added this
              }

                                                                                                SimOpts.SunUp = false; //Set to a reasonable default value for older saved sim files
                                                                                                if (!EOF(1))
                                                                                                {
                                                                                                    Get(#1); //EricL 6/8/2006 Added this
              }

                                                                                                SimOpts.SunDownThreshold = 1000000; //Set to a reasonable default value for older saved sim files
                                                                                                if (!EOF(1))
                                                                                                {
                                                                                                    Get(#1); //EricL 6/8/2006 Added this
              }

                                                                                                SimOpts.SunDown = false; //Set to a reasonable default value for older saved sim files
                                                                                                if (!EOF(1))
                                                                                                {
                                                                                                    Get(#1); //EricL 6/8/2006 Added this
              }

                                                                                                if (!EOF(1))
                                                                                                {
                                                                                                    Get(#1);
              }
                                                                                                if (!EOF(1))
                                                                                                {
                                                                                                    Get(#1);
              }

                                                                                                SimOpts.FixedBotRadii = false;
                                                                                                if (!EOF(1))
                                                                                                {
                                                                                                    Get(#1);
              }

                                                                                                SimOpts.DayNightCycleCounter = 0;
                                                                                                if (!EOF(1))
                                                                                                {
                                                                                                    Get(#1);
              }

                                                                                                SimOpts.Daytime = true;
                                                                                                if (!EOF(1))
                                                                                                {
                                                                                                    Get(#1);
              }

                                                                                                SimOpts.SunThresholdMode = 0;
                                                                                                if (!EOF(1))
                                                                                                {
                                                                                                    Get(#1);
              }

                                                                                                numTeleporters = 0;
                                                                                                if (!EOF(1))
                                                                                                {
                                                                                                    Get(#1);
              }

                                                                                                t = numTeleporters;

                                                                                                for (X = 1; X < numTeleporters; X++)
                                                                                                {
                                                                                                    LoadTeleporter(1, X);
                                                                                                    Next(X);

                                                                                                    for (X = 1; X < numTeleporters; X++)
                                                                                                    {
                                                                                                        if (Teleporters(X).Internet)
                                                                                                        {
                                                                                                            DeleteTeleporter((X));
                                                                                                        }
                                                                                                        Next(X);

                                                                                                        numObstacles = 0;
                                                                                                        if (!EOF(1))
                                                                                                        {
                                                                                                            Get(#1);
                  }

                                                                                                        for (X = 1; X < numObstacles; X++)
                                                                                                        {
                                                                                                            LoadObstacle(1, X);
                                                                                                            Next(X);

                                                                                                            if (!EOF(1))
                                                                                                            {
                                                                                                                Get(#1);
                    }

                                                                                                            for (k = 0; k < SimOpts.SpeciesNum - 1; k++)
                                                                                                            {
                                                                                                                SimOpts.Specie[k].CantSee = false;
                                                                                                                SimOpts.Specie[k].DisableDNA = false;
                                                                                                                SimOpts.Specie[k].DisableMovementSysvars = false;

                                                                                                                if (!EOF(1))
                                                                                                                {
                                                                                                                    Get(#1);
                      }
                                                                                                                if (!EOF(1))
                                                                                                                {
                                                                                                                    Get(#1);
                      }
                                                                                                                if (!EOF(1))
                                                                                                                {
                                                                                                                    Get(#1);
                      }
                                                                                                                Next(k);

                                                                                                                SimOpts.shapesAreVisable = false;
                                                                                                                if (!EOF(1))
                                                                                                                {
                                                                                                                    Get(#1);
                      }

                                                                                                                SimOpts.allowVerticalShapeDrift = false;
                                                                                                                if (!EOF(1))
                                                                                                                {
                                                                                                                    Get(#1);
                      }

                                                                                                                SimOpts.allowHorizontalShapeDrift = false;
                                                                                                                if (!EOF(1))
                                                                                                                {
                                                                                                                    Get(#1);
                      }

                                                                                                                SimOpts.shapesAreSeeThrough = false;
                                                                                                                if (!EOF(1))
                                                                                                                {
                                                                                                                    Get(#1);
                      }

                                                                                                                SimOpts.shapesAbsorbShots = false;
                                                                                                                if (!EOF(1))
                                                                                                                {
                                                                                                                    Get(#1);
                      }

                                                                                                                SimOpts.shapeDriftRate = 0;
                                                                                                                if (!EOF(1))
                                                                                                                {
                                                                                                                    Get(#1);
                      }

                                                                                                                SimOpts.makeAllShapesTransparent = false;
                                                                                                                if (!EOF(1))
                                                                                                                {
                                                                                                                    Get(#1);
                      }

                                                                                                                SimOpts.makeAllShapesBlack = false;
                                                                                                                if (!EOF(1))
                                                                                                                {
                                                                                                                    Get(#1);
                      }

                                                                                                                for (k = 0; k < SimOpts.SpeciesNum - 1; k++)
                                                                                                                {
                                                                                                                    SimOpts.Specie[k].CantReproduce = false;
                                                                                                                    if (!EOF(1))
                                                                                                                    {
                                                                                                                        Get(#1);
                        }
                                                                                                                    Next(k);

                                                                                                                    maxshotarray = 0;
                                                                                                                    if (!EOF(1))
                                                                                                                    {
                                                                                                                        Get(#1);
                        }

                                                                                                                    if (maxshotarray != 0 & maxshotarray > 0 & maxshotarray < 1000000)
                                                                                                                    {
                                                                                                                        List<> Shots_5059_tmp = new List<>();
                                                                                                                        for (int redim_iter_8156 = 0; i < 0; redim_iter_8156++) { Shots.Add(null); }

                                                                                                                        for (j = 1; j < maxshotarray; j++)
                                                                                                                        {
                                                                                                                            LoadShot(1, j);
                                                                                                                            Next(j);
                                                                                                                            RemapAllShots(maxshotarray);
                                                                                                                        } else
                                                                                                                        {
                                                                                                                            // Old sim with no saved shots
                                                                                                                            // Init the shots array (this used to be done in StartLoaded
                                                                                                                            maxshotarray = 100;
                                                                                                                            List<> Shots_3552_tmp = new List<>();
                                                                                                                            for (int redim_iter_4086 = 0; i < 0; redim_iter_4086++) { Shots.Add(null); }
                                                                                                                            for (j = 1; j < maxshotarray; j++)
                                                                                                                            {
                                                                                                                                Shots(j).stored = false;
                                                                                                                                Shots(j).exist = false;
                                                                                                                                Shots(j).parent = 0;
                                                                                                                                Next(j);
                                                                                                                            }

                                                                                                                            SimOpts.MaxAbsNum = MaxRobs;
                                                                                                                            if (!EOF(1))
                                                                                                                            {
                                                                                                                                Get(#1);
                            }

                                                                                                                            for (k = 0; k < SimOpts.SpeciesNum - 1; k++)
                                                                                                                            {
                                                                                                                                SimOpts.Specie[k].VirusImmune = false;
                                                                                                                                if (!EOF(1))
                                                                                                                                {
                                                                                                                                    Get(#1);
                              }
                                                                                                                                Next(k);

                                                                                                                                for (k = 0; k < SimOpts.SpeciesNum - 1; k++)
                                                                                                                                {
                                                                                                                                    SimOpts.Specie[k].population = 0;
                                                                                                                                    if (!EOF(1))
                                                                                                                                    {
                                                                                                                                        Get(#1);
                                }

                                                                                                                                    SimOpts.Specie[k].SubSpeciesCounter = 0;
                                                                                                                                    if (!EOF(1))
                                                                                                                                    {
                                                                                                                                        Get(#1);
                                }
                                                                                                                                    Next(k);

                                                                                                                                    for (k = 0; k < SimOpts.SpeciesNum - 1; k++)
                                                                                                                                    {
                                                                                                                                        SimOpts.Specie[k].Native = true; // Default
                                                                                                                                        if (!EOF(1))
                                                                                                                                        {
                                                                                                                                            Get(#1);
                                  }
                                                                                                                                        Next(k);

                                                                                                                                        if (!EOF(1))
                                                                                                                                        {
                                                                                                                                            Get(#1);
                                  }

                                                                                                                                        SimOpts.EGridEnabled = false;
                                                                                                                                        if (!EOF(1))
                                                                                                                                        {
                                                                                                                                            Get(#1);
                                  }

                                                                                                                                        if (!EOF(1))
                                                                                                                                        {
                                                                                                                                            Get(#1);
                                  }

                                                                                                                                        SimOpts.DisableMutations = false;
                                                                                                                                        if (!EOF(1))
                                                                                                                                        {
                                                                                                                                            Get(#1);
                                  }
                                                                                                                                        if (CInt(SimOpts.DisableMutations) > 1 || CInt(SimOpts.DisableMutations) < 0)
                                                                                                                                        {
                                                                                                                                            SimOpts.DisableMutations = false;
                                                                                                                                        }

                                                                                                                                        SimOpts.SimGUID = CLng(Rnd);
                                                                                                                                        if (!EOF(1))
                                                                                                                                        {
                                                                                                                                            Get(#1);
                                  }
                                                                                                                                        if (!EOF(1))
                                                                                                                                        {
                                                                                                                                            Get(#1);
                                  }
                                                                                                                                        if (!EOF(1))
                                                                                                                                        {
                                                                                                                                            Get(#1);
                                  }
                                                                                                                                        if (!EOF(1))
                                                                                                                                        {
                                                                                                                                            Get(#1);
                                  }
                                                                                                                                        if (!EOF(1))
                                                                                                                                        {
                                                                                                                                            Get(#1);
                                  }

                                                                                                                                        SimOpts.SpeciationForkInterval = 5000;
                                                                                                                                        if (!EOF(1))
                                                                                                                                        {
                                                                                                                                            Get(#1);
                                  }

                                                                                                                                        //Botsareus 4/17/2013
                                                                                                                                        SimOpts.DisableTypArepro = false;
                                                                                                                                        if (!EOF(1))
                                                                                                                                        {
                                                                                                                                            Get(#1);
                                  }

                                                                                                                                        //Botsareus 5/31/2013 Load all graph data
                                                                                                                                        //strings
                                                                                                                                        if (!EOF(1))
                                                                                                                                        {
                                                                                                                                            Get(#1);
                                    strGraphQuery1 = Space(j);
                                                                                                                                        }
                                                                                                                                        if (!EOF(1))
                                                                                                                                        {
                                                                                                                                            Get(#1);
                                  }
                                                                                                                                        if (!EOF(1))
                                                                                                                                        {
                                                                                                                                            Get(#1);
                                    strGraphQuery2 = Space(j);
                                                                                                                                        }
                                                                                                                                        if (!EOF(1))
                                                                                                                                        {
                                                                                                                                            Get(#1);
                                  }
                                                                                                                                        if (!EOF(1))
                                                                                                                                        {
                                                                                                                                            Get(#1);
                                    strGraphQuery3 = Space(j);
                                                                                                                                        }
                                                                                                                                        if (!EOF(1))
                                                                                                                                        {
                                                                                                                                            Get(#1);
                                  }
                                                                                                                                        if (!EOF(1))
                                                                                                                                        {
                                                                                                                                            Get(#1);
                                    strSimStart = Space(j);
                                                                                                                                        }
                                                                                                                                        if (!EOF(1))
                                                                                                                                        {
                                                                                                                                            Get(#1);
                                  }
                                                                                                                                        //the graphs themselfs
                                                                                                                                        for (k = 1; k < NUMGRAPHS; k++)
                                                                                                                                        {
                                                                                                                                            if (!EOF(1))
                                                                                                                                            {
                                                                                                                                                Get(#1);
                                    }
                                                                                                                                            if (!EOF(1))
                                                                                                                                            {
                                                                                                                                                Get(#1);
                                    }
                                                                                                                                            if (!EOF(1))
                                                                                                                                            {
                                                                                                                                                Get(#1);
                                    }
                                                                                                                                            if (!EOF(1))
                                                                                                                                            {
                                                                                                                                                Get(#1);
                                    }
                                                                                                                                            if (!EOF(1))
                                                                                                                                            {
                                                                                                                                                Get(#1);
                                    }
                                                                                                                                            if (graphvisible(k))
                                                                                                                                            {
                                                                                                                                                switch (k)
                                                                                                                                                {
                                                                                                                                                    case 1:
                                                                                                                                                        Form1.NewGraph(POPULATION_GRAPH, "Populations");
                                                                                                                                                        break;
                                                                                                                                                    case 2:
                                                                                                                                                        Form1.NewGraph(MUTATIONS_GRAPH, "Average_Mutations");
                                                                                                                                                        break;
                                                                                                                                                    case 3:
                                                                                                                                                        Form1.NewGraph(AVGAGE_GRAPH, "Average_Age");
                                                                                                                                                        break;
                                                                                                                                                    case 4:
                                                                                                                                                        Form1.NewGraph(OFFSPRING_GRAPH, "Average_Offspring");
                                                                                                                                                        break;
                                                                                                                                                    case 5:
                                                                                                                                                        Form1.NewGraph(ENERGY_GRAPH, "Average_Energy");
                                                                                                                                                        break;
                                                                                                                                                    case 6:
                                                                                                                                                        Form1.NewGraph(DNALENGTH_GRAPH, "Average_DNA_length");
                                                                                                                                                        break;
                                                                                                                                                    case 7:
                                                                                                                                                        Form1.NewGraph(DNACOND_GRAPH, "Average_DNA_Cond_statements");
                                                                                                                                                        break;
                                                                                                                                                    case 8:
                                                                                                                                                        Form1.NewGraph(MUT_DNALENGTH_GRAPH, "Average_Mutations_per_DNA_length_x1000-");
                                                                                                                                                        break;
                                                                                                                                                    case 9:
                                                                                                                                                        Form1.NewGraph(ENERGY_SPECIES_GRAPH, "Total_Energy_per_Species_x1000-");
                                                                                                                                                        break;
                                                                                                                                                    case 10:
                                                                                                                                                        Form1.NewGraph(DYNAMICCOSTS_GRAPH, "Dynamic_Costs");
                                                                                                                                                        break;
                                                                                                                                                    case 11:
                                                                                                                                                        Form1.NewGraph(SPECIESDIVERSITY_GRAPH, "Species_Diversity");
                                                                                                                                                        break;
                                                                                                                                                    case 12:
                                                                                                                                                        Form1.NewGraph(AVGCHLR_GRAPH, "Average_Chloroplasts");
                                                                                                                                                        break;
                                                                                                                                                    case 13:
                                                                                                                                                        Form1.NewGraph(GENETIC_DIST_GRAPH, "Genetic_Distance_x1000-");
                                                                                                                                                        break;
                                                                                                                                                    case 14:
                                                                                                                                                        Form1.NewGraph(GENERATION_DIST_GRAPH, "Max_Generational_Distance");
                                                                                                                                                        break;
                                                                                                                                                    case 15:
                                                                                                                                                        Form1.NewGraph(GENETIC_SIMPLE_GRAPH, "Simple_Genetic_Distance_x1000-");
                                                                                                                                                        break;
                                                                                                                                                    case 16:
                                                                                                                                                        Form1.NewGraph(CUSTOM_1_GRAPH, "Customizable_Graph_1-");
                                                                                                                                                        break;
                                                                                                                                                    case 17:
                                                                                                                                                        Form1.NewGraph(CUSTOM_2_GRAPH, "Customizable_Graph_2-");
                                                                                                                                                        break;
                                                                                                                                                    case 18:
                                                                                                                                                        Form1.NewGraph(CUSTOM_3_GRAPH, "Customizable_Graph_3-");
                                                                                                                                                        break;
                                                                                                                                                }
                                                                                                                                            }
                                                                                                                                            Next(k);

                                                                                                                                            SimOpts.NoWShotDecay = false; //Load information about not decaying waste shots
                                                                                                                                            if (!EOF(1))
                                                                                                                                            {
                                                                                                                                                Get(#1); //EricL 6/8/2006 Added this
                                    }

                                                                                                                                            //evo stuff
                                                                                                                                            if (!EOF(1))
                                                                                                                                            {
                                                                                                                                                Get(#1);
                                    }
                                                                                                                                            if (!EOF(1))
                                                                                                                                            {
                                                                                                                                                Get(#1);
                                    }
                                                                                                                                            if (!EOF(1))
                                                                                                                                            {
                                                                                                                                                Get(#1);
                                    }
                                                                                                                                            if (!EOF(1))
                                                                                                                                            {
                                                                                                                                                Get(#1);
                                    }
                                                                                                                                            if (!EOF(1))
                                                                                                                                            {
                                                                                                                                                Get(#1);
                                    }
                                                                                                                                            if (!EOF(1))
                                                                                                                                            {
                                                                                                                                                Get(#1);
                                    }
                                                                                                                                            if (!EOF(1))
                                                                                                                                            {
                                                                                                                                                Get(#1);
                                    }
                                                                                                                                            if (!EOF(1))
                                                                                                                                            {
                                                                                                                                                Get(#1);
                                    }
                                                                                                                                            if (!EOF(1))
                                                                                                                                            {
                                                                                                                                                Get(#1);
                                    }

                                                                                                                                            //some more simopts stuff
                                                                                                                                            if (!EOF(1))
                                                                                                                                            {
                                                                                                                                                Get(#1);
                                    }

                                                                                                                                            SimOpts.DisableFixing = false;
                                                                                                                                            if (!EOF(1))
                                                                                                                                            {
                                                                                                                                                Get(#1);
                                    }

                                                                                                                                            if (!EOF(1))
                                                                                                                                            {
                                                                                                                                                Get(#1);
                                    }
                                                                                                                                            if (!EOF(1))
                                                                                                                                            {
                                                                                                                                                Get(#1);
                                    }
                                                                                                                                            if (!EOF(1))
                                                                                                                                            {
                                                                                                                                                Get(#1);
                                    }

                                                                                                                                            //Botsareus 10/13/2014
                                                                                                                                            if (!EOF(1))
                                                                                                                                            {
                                                                                                                                                Get(#1);
                                    }
                                                                                                                                            if (!EOF(1))
                                                                                                                                            {
                                                                                                                                                Get(#1);
                                    }

                                                                                                                                            //Botsareus 10/8/2015
                                                                                                                                            if (!EOF(1))
                                                                                                                                            {
                                                                                                                                                Get(#1);
                                    }

                                                                                                                                            //Botsareus 10/20/2015
                                                                                                                                            if (!EOF(1))
                                                                                                                                            {
                                                                                                                                                Get(#1);
                                    }

                                                                                                                                            Form1.lblSaving.Visible = false; //Botsareus 1/14/2014

                                                                                                                                            VBCloseFile(1); ();

                                                                                                                                            if (SimOpts.Costs(DYNAMICCOSTSENSITIVITY) == 0)
                                                                                                                                            {
                                                                                                                                                SimOpts.Costs(DYNAMICCOSTSENSITIVITY) = 500;
                                                                                                                                            }

                                                                                                                                            //EricL 3/28/2006 This line insures that all the simulation dialog options get set to match the loaded sim
                                                                                                                                            TmpOpts = SimOpts;

                                                                                                                                            Form1.MousePointer = vbArrow;
                                                                                                                                        }

                                                                                                                                        /*
                                                                                                                                        ' loads a single robot
                                                                                                                                        */
                                                                                                                                        public static void LoadRobot(ref int fnum, int n)
                                                                                                                                        {
                                                                                                                                            LoadRobotBody(fnum, n);
                                                                                                                                            if (rob[n].exist)
                                                                                                                                            {
                                                                                                                                                GiveAbsNum(n);
                                                                                                                                                insertsysvars(n);
                                                                                                                                                ScanUsedVars(n);
                                                                                                                                                makeoccurrlist(n);
                                                                                                                                                rob[n].DnaLen = DnaLen(ref rob[n].dna());
                                                                                                                                                rob[n].genenum = CountGenes(ref rob[n].dna());
                                                                                                                                                rob[n].mem(DnaLenSys) = rob[n].DnaLen;
                                                                                                                                                rob[n].mem(GenesSys) = rob[n].genenum;
                                                                                                                                                // UpdateBotBucket n
                                                                                                                                            }
                                                                                                                                        }

                                                                                                                                        /*
                                                                                                                                        ' assignes a robot his unique code
                                                                                                                                        */
                                                                                                                                        public static void GiveAbsNum(ref int k)
                                                                                                                                        {
                                                                                                                                            // Dim n As Integer, Max As Long
                                                                                                                                            //For n = 1 To MaxRobs
                                                                                                                                            //  If Max < rob[n].AbsNum Then
                                                                                                                                            //    Max = rob[n].AbsNum
                                                                                                                                            //  End If
                                                                                                                                            //Next n
                                                                                                                                            //rob(k).AbsNum = Max + 1
                                                                                                                                            if (rob(k).AbsNum == 0)
                                                                                                                                            {
                                                                                                                                                SimOpts.MaxAbsNum = SimOpts.MaxAbsNum + 1;
                                                                                                                                                rob(k).AbsNum = SimOpts.MaxAbsNum;
                                                                                                                                            }
                                                                                                                                        }

                                                                                                                                        /*
                                                                                                                                        ' loads the body of the robot
                                                                                                                                        */
                                                                                                                                        private static void LoadRobotBody(ref int n, ref int r)
                                                                                                                                        {
                                                                                                                                            //robot r
                                                                                                                                            //file #n,
                                                                                                                                            int t = 0;
                                                                                                                                            int k = 0;
                                                                                                                                            int ind = 0;
                                                                                                                                            byte Fe = 0;
                                                                                                                                            int L1 = 0;
                                                                                                                                            int inttmp = 0;

                                                                                                                                            bool MessedUpMutations = false;

                                                                                                                                            int longtmp = 0;//Botsareus 10/5/2015 freeing up memory from Eric's obsolete ancestors code


                                                                                                                                            MessedUpMutations = false;
                                                                                                                                            dynamic _WithVar_2822;
                                                                                                                                            _WithVar_2822 = rob(r);
                                                                                                                                            Get(#n);
    Get(#n);
    Get(#n);

    Get(#n);
    Get(#n);
    Get(#n);
    Get(#n);
    Get(#n);
    Get(#n); //momento angolare
    Get(#n); //momento torcente

    _WithVar_2822.BucketPos.X = -2;
                                                                                                                                            _WithVar_2822.BucketPos.Y = -2;

                                                                                                                                            //ties
                                                                                                                                            for (t = 0; t < MAXTIES; t++)
                                                                                                                                            {
                                                                                                                                                Get(#n);
      Get(#n);
      Get(#n);
      Get(#n);
      Get(#n);
      Get(#n);
      Get(#n);
      Get(#n);
      Get(#n);
      Get(#n);
      Get(#n);
      Get(#n);
      Get(#n);
      Get(#n);
      Get(#n);
      Next(t);

                                                                                                                                                Get(#n);

      for (t = 1; t < 50; t++)
                                                                                                                                                {
                                                                                                                                                    Get(#n);
        _WithVar_2822.vars(t).Name = Space(k);
                                                                                                                                                    Get(#n); //|
        Get(#n);
        Next(t);
                                                                                                                                                    Get(#n); //| variabili private

// macchina virtuale
        Get(#n); // memoria dati
        Get(#n);
        List <> dna_4061_tmp = new List<>();
                                                                                                                                                    for (int redim_iter_8013 = 0; i < 0; redim_iter_8013++) { dna.Add(null); }

                                                                                                                                                    for (t = 1; t < k; t++)
                                                                                                                                                    {
                                                                                                                                                        Get(#n);
          Get(#n);
          Next(t);

                                                                                                                                                        //Force an end base pair to protect against DNA corruption
                                                                                                                                                        _WithVar_2822.dna(k).tipo = 10;
                                                                                                                                                        _WithVar_2822.dna(k).value = 1;


                                                                                                                                                        //EricL Set reasonable default values to protect against corrupted sims that don't read these values
                                                                                                                                                        SetDefaultMutationRates(_WithVar_2822.Mutables, true);

                                                                                                                                                        for (t = 0; t < 20; t++)
                                                                                                                                                        {
                                                                                                                                                            Get(#n);
            Next(t);

                                                                                                                                                            // informative
                                                                                                                                                            Get(#n);
            Get(#n);
            _WithVar_2822.Mutations = inttmp;
                                                                                                                                                            Get(#n);
            _WithVar_2822.LastMut = inttmp;
                                                                                                                                                            Get(#n);
            Get(#n);
            Get(#n);
            Get(#n);
            Get(#n);
            Get(#n);

// aspetto
            Get(#n);
            Get(#n);

//new stuff using FileContinue conditions for backward and forward compatability
            if (FileContinue(n))
                                                                                                                                                            {
                                                                                                                                                                Get(#n);
              _WithVar_2822.radius = FindRadius(r);
                                                                                                                                                            }
                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                            {
                                                                                                                                                                Get(#n);
            }
                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                            {
                                                                                                                                                                Get(#n);
            }
                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                            {
                                                                                                                                                                Get(#n);
            }
                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                            {
                                                                                                                                                                Get(#n);
            }
                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                            {
                                                                                                                                                                Get(#n);
            }
                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                            {
                                                                                                                                                                Get(#n);
            }
                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                            {
                                                                                                                                                                Get(#n);
            }
                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                            {
                                                                                                                                                                Get(#n);
            }
                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                            {
                                                                                                                                                                Get(#n);
            }

                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                            {
                                                                                                                                                                Get(#n);
              _WithVar_2822.FName = Space(k);
                                                                                                                                                            }
                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                            {
                                                                                                                                                                Get(#n);
            }

                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                            {
                                                                                                                                                                Get(#n);
              _WithVar_2822.LastOwner = Space(k);
                                                                                                                                                            }
                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                            {
                                                                                                                                                                Get(#n);
            }
                                                                                                                                                            if (_WithVar_2822.LastOwner == "")
                                                                                                                                                            {
                                                                                                                                                                _WithVar_2822.LastOwner = "Local";
                                                                                                                                                            }

                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                            {
                                                                                                                                                                Get(#n);
            }

                                                                                                                                                            //EricL 5/2/2006  This needs some explaining.  The length of the mutation details can exceed 2^15 -1 for bots with lots
                                                                                                                                                            //of mutations.  If we are reading an old file, the length could be negative in which case we read what we can and then punt and skip the
                                                                                                                                                            //rest of the bot.  We will miss some stuff, like the mutation settings, but at least the sim will load.
                                                                                                                                                            //If the sim file was stored with 2.42.4 or later and this bot has a ton of mutation details, then an Int value of 1
                                                                                                                                                            //indicates the actual length of the mutation details is stored as a Long in which case we read that and continue.
                                                                                                                                                            if (k < 0)
                                                                                                                                                            {
                                                                                                                                                                //Its an old corrupted file with > 2^15 worth of mutation details.  Bail.
                                                                                                                                                                _WithVar_2822.LastMutDetail = "Problem reading mutation details.  May be a very old sim.  Please tell the developers.  Mutation Details deleted.";

                                                                                                                                                                //EricL Set reasonable default values for everything read from this point on.
                                                                                                                                                                _WithVar_2822.Mutables.Mutations = true;

                                                                                                                                                                SetDefaultMutationRates(_WithVar_2822.Mutables, true);

                                                                                                                                                                _WithVar_2822.View = true;
                                                                                                                                                                _WithVar_2822.NewMove = false;
                                                                                                                                                                _WithVar_2822.oldBotNum = 0;
                                                                                                                                                                _WithVar_2822.CantSee = false;
                                                                                                                                                                _WithVar_2822.DisableDNA = false;
                                                                                                                                                                _WithVar_2822.DisableMovementSysvars = false;
                                                                                                                                                                _WithVar_2822.CantReproduce = false;
                                                                                                                                                                _WithVar_2822.VirusImmune = false;
                                                                                                                                                                _WithVar_2822.shell = 0;
                                                                                                                                                                _WithVar_2822.Slime = 0;

                                                                                                                                                                goto ;
                                                                                                                                                            }
                                                                                                                                                            if (k == 1)
                                                                                                                                                            {
                                                                                                                                                                //Its a new file with lots of mutations.  Read the actual length stored as a Long
                                                                                                                                                                Get(#n);
            }
                                                                                                                                                            else
                                                                                                                                                            {
                                                                                                                                                                //Not that many mutations for this bot (It's possible its an old file with lots of mutations and the len wrapped.
                                                                                                                                                                //If so, we just read the postiive len and keep going.  Everything following this will be wrong, but the sim should
                                                                                                                                                                //still load.  It's a corner case.  The alternative is to try to parse the mutation details strings directly.  No thanks.
                                                                                                                                                                L1 = CLng(k);
                                                                                                                                                            }

                                                                                                                                                            if (Form1.lblSaving.Visible)
                                                                                                                                                            { //Botsareus 4/18/2016 Bug fix to prevent string buffer overflow
                                                                                                                                                                _WithVar_2822.LastMutDetail = Space(L1);
                                                                                                                                                                if (FileContinue(n))
                                                                                                                                                                {
                                                                                                                                                                    Get(#n);
              }
                                                                                                                                                            }
                                                                                                                                                            else
                                                                                                                                                            {
                                                                                                                                                                if (L1 > (100000000 / TotalRobotsDisplayed))
                                                                                                                                                                {
                                                                                                                                                                    Seek(#n, L1 + Seek(n));
              }
                                                                                                                                                                else
                                                                                                                                                                {
                                                                                                                                                                    _WithVar_2822.LastMutDetail = Space(L1);
                                                                                                                                                                    if (FileContinue(n))
                                                                                                                                                                    {
                                                                                                                                                                        Get(#n);
                }
                                                                                                                                                                }
                                                                                                                                                            }

                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                            {
                                                                                                                                                                Get(#n);
            }

                                                                                                                                                            for (t = 0; t < 20; t++)
                                                                                                                                                            {
                                                                                                                                                                if (FileContinue(n))
                                                                                                                                                                {
                                                                                                                                                                    Get(#n);
              }
                                                                                                                                                                if (FileContinue(n))
                                                                                                                                                                {
                                                                                                                                                                    Get(#n);
              }
                                                                                                                                                                Next(t);

                                                                                                                                                                for (t = 0; t < 20; t++)
                                                                                                                                                                {
                                                                                                                                                                    if (_WithVar_2822.Mutables.Mean(t) < 0 || _WithVar_2822.Mutables.Mean(t) > 32000 || _WithVar_2822.Mutables.StdDev(t) < 0 || _WithVar_2822.Mutables.StdDev(t) > 32000)
                                                                                                                                                                    {
                                                                                                                                                                        MessedUpMutations = true;
                                                                                                                                                                    }
                                                                                                                                                                    Next(t);

                                                                                                                                                                    if (FileContinue(n))
                                                                                                                                                                    {
                                                                                                                                                                        Get(#n);
                }
                                                                                                                                                                    if (FileContinue(n))
                                                                                                                                                                    {
                                                                                                                                                                        Get(#n);
                }

                                                                                                                                                                    if (_WithVar_2822.Mutables.CopyErrorWhatToChange < 0 || _WithVar_2822.Mutables.CopyErrorWhatToChange > 32000 || _WithVar_2822.Mutables.PointWhatToChange < 0 || _WithVar_2822.Mutables.PointWhatToChange > 32000)
                                                                                                                                                                    {
                                                                                                                                                                        MessedUpMutations = true;
                                                                                                                                                                    }

                                                                                                                                                                    //If we read wacky values, the file was saved with an older version which messed these up.  Set the defaults.
                                                                                                                                                                    if (MessedUpMutations)
                                                                                                                                                                    {
                                                                                                                                                                        SetDefaultMutationRates(_WithVar_2822.Mutables, true);
                                                                                                                                                                    }

                                                                                                                                                                    if (FileContinue(n))
                                                                                                                                                                    {
                                                                                                                                                                        Get(#n);
                }
                                                                                                                                                                    if (FileContinue(n))
                                                                                                                                                                    {
                                                                                                                                                                        Get(#n);
                }

                                                                                                                                                                    _WithVar_2822.oldBotNum = 0;
                                                                                                                                                                    if (FileContinue(n))
                                                                                                                                                                    {
                                                                                                                                                                        Get(#n);
                }

                                                                                                                                                                    _WithVar_2822.CantSee = false;
                                                                                                                                                                    if (FileContinue(n))
                                                                                                                                                                    {
                                                                                                                                                                        Get(#n);
                }
                                                                                                                                                                    if (CInt(_WithVar_2822.CantSee) > 0 || CInt(_WithVar_2822.CantSee) < -1)
                                                                                                                                                                    {
                                                                                                                                                                        _WithVar_2822.CantSee = false; // Protection against corrpt sim files.
                                                                                                                                                                    }

                                                                                                                                                                    _WithVar_2822.DisableDNA = false;
                                                                                                                                                                    if (FileContinue(n))
                                                                                                                                                                    {
                                                                                                                                                                        Get(#n);
                }
                                                                                                                                                                    if (CInt(_WithVar_2822.DisableDNA) > 0 || CInt(_WithVar_2822.DisableDNA) < -1)
                                                                                                                                                                    {
                                                                                                                                                                        _WithVar_2822.DisableDNA = false; // Protection against corrpt sim files.
                                                                                                                                                                    }

                                                                                                                                                                    _WithVar_2822.DisableMovementSysvars = false;
                                                                                                                                                                    if (FileContinue(n))
                                                                                                                                                                    {
                                                                                                                                                                        Get(#n);
                }
                                                                                                                                                                    if (CInt(_WithVar_2822.DisableMovementSysvars) > 0 || CInt(_WithVar_2822.DisableMovementSysvars) < -1)
                                                                                                                                                                    {
                                                                                                                                                                        _WithVar_2822.DisableMovementSysvars = false; // Protection against corrpt sim files.
                                                                                                                                                                    }

                                                                                                                                                                    _WithVar_2822.CantReproduce = false;
                                                                                                                                                                    if (FileContinue(n))
                                                                                                                                                                    {
                                                                                                                                                                        Get(#n);
                }
                                                                                                                                                                    if (CInt(_WithVar_2822.CantReproduce) > 0 || CInt(_WithVar_2822.CantReproduce) < -1)
                                                                                                                                                                    {
                                                                                                                                                                        _WithVar_2822.CantReproduce = false; // Protection against corrpt sim files.
                                                                                                                                                                    }

                                                                                                                                                                    _WithVar_2822.shell = 0;
                                                                                                                                                                    if (FileContinue(n))
                                                                                                                                                                    {
                                                                                                                                                                        Get(#n);
                }

                                                                                                                                                                    if (_WithVar_2822.shell > 32000)
                                                                                                                                                                    {
                                                                                                                                                                        _WithVar_2822.shell = 32000;
                                                                                                                                                                    }
                                                                                                                                                                    if (_WithVar_2822.shell < 0)
                                                                                                                                                                    {
                                                                                                                                                                        _WithVar_2822.shell = 0;
                                                                                                                                                                    }

                                                                                                                                                                    _WithVar_2822.Slime = 0;
                                                                                                                                                                    if (FileContinue(n))
                                                                                                                                                                    {
                                                                                                                                                                        Get(#n);
                }

                                                                                                                                                                    if (_WithVar_2822.Slime > 32000)
                                                                                                                                                                    {
                                                                                                                                                                        _WithVar_2822.Slime = 32000;
                                                                                                                                                                    }
                                                                                                                                                                    if (_WithVar_2822.Slime < 0)
                                                                                                                                                                    {
                                                                                                                                                                        _WithVar_2822.Slime = 0;
                                                                                                                                                                    }

                                                                                                                                                                    _WithVar_2822.VirusImmune = false;
                                                                                                                                                                    if (FileContinue(n))
                                                                                                                                                                    {
                                                                                                                                                                        Get(#n);
                }
                                                                                                                                                                    if (CInt(_WithVar_2822.VirusImmune) > 0 || CInt(_WithVar_2822.VirusImmune) < -1)
                                                                                                                                                                    {
                                                                                                                                                                        _WithVar_2822.VirusImmune = false; // Protection against corrpt sim files.
                                                                                                                                                                    }

                                                                                                                                                                    _WithVar_2822.SubSpecies = 0; // For older sims saved before this was implemented, set the sup species to be the bot's number.  Every bot is a sub species.
                                                                                                                                                                    if (FileContinue(n))
                                                                                                                                                                    {
                                                                                                                                                                        Get(#n);
                }

                                                                                                                                                                    _WithVar_2822.spermDNAlen = 0;
                                                                                                                                                                    if (FileContinue(n))
                                                                                                                                                                    {
                                                                                                                                                                        Get(#n);
                  List <> spermDNA_1155_tmp = new List<>();
                                                                                                                                                                        for (int redim_iter_9379 = 0; i < 0; redim_iter_9379++) { spermDNA.Add(null); }
                                                                                                                                                                    }
                                                                                                                                                                    for (t = 1; t < _WithVar_2822.spermDNAlen; t++)
                                                                                                                                                                    {
                                                                                                                                                                        if (FileContinue(n))
                                                                                                                                                                        {
                                                                                                                                                                            Get(#n);
                  }
                                                                                                                                                                        if (FileContinue(n))
                                                                                                                                                                        {
                                                                                                                                                                            Get(#n);
                  }
                                                                                                                                                                        Next(t);

                                                                                                                                                                        _WithVar_2822.fertilized = -1;
                                                                                                                                                                        if (FileContinue(n))
                                                                                                                                                                        {
                                                                                                                                                                            Get(#n);
                  }

                                                                                                                                                                        //Botsareus 10/5/2015 freeing up memory from Eric's obsolete ancestors code
                                                                                                                                                                        if (FileContinue(n))
                                                                                                                                                                        {
                                                                                                                                                                            Get(#n);
                  }
                                                                                                                                                                        for (t = 0; t < 500; t++)
                                                                                                                                                                        {
                                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                                            {
                                                                                                                                                                                Get(#n);
                    }
                                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                                            {
                                                                                                                                                                                Get(#n);
                    }
                                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                                            {
                                                                                                                                                                                Get(#n);
                    }
                                                                                                                                                                            Next(t);

                                                                                                                                                                            _WithVar_2822.sim = 0;
                                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                                            {
                                                                                                                                                                                Get(#n);
                    }
                                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                                            {
                                                                                                                                                                                Get(#n);
                    }

                                                                                                                                                                            //Botsareus 2/23/2013 Rest of tie data
                                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                                            {
                                                                                                                                                                                Get(#n);
                    }
                                                                                                                                                                            for (t = 0; t < MAXTIES; t++)
                                                                                                                                                                            {
                                                                                                                                                                                if (FileContinue(n))
                                                                                                                                                                                {
                                                                                                                                                                                    Get(#n);
                      }
                                                                                                                                                                                if (FileContinue(n))
                                                                                                                                                                                {
                                                                                                                                                                                    Get(#n);
                      }
                                                                                                                                                                                if (FileContinue(n))
                                                                                                                                                                                {
                                                                                                                                                                                    Get(#n);
                      }
                                                                                                                                                                                if (FileContinue(n))
                                                                                                                                                                                {
                                                                                                                                                                                    Get(#n);
                      }
                                                                                                                                                                                //Botsareus 4/18/2016 Protection against currupt file
                                                                                                                                                                                if (_WithVar_2822.Ties(t).NaturalLength < 0)
                                                                                                                                                                                {
                                                                                                                                                                                    _WithVar_2822.Ties(t).NaturalLength = 0;
                                                                                                                                                                                }
                                                                                                                                                                                if (_WithVar_2822.Ties(t).NaturalLength > 1500)
                                                                                                                                                                                {
                                                                                                                                                                                    _WithVar_2822.Ties(t).NaturalLength = 1500;
                                                                                                                                                                                }
                                                                                                                                                                            }

                                                                                                                                                                            //Botsareus 4/9/2013 For genetic distance graph
                                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                                            {
                                                                                                                                                                                Get(#n);
                    }
                                                                                                                                                                            _WithVar_2822.GenMut = _WithVar_2822.DnaLen / GeneticSensitivity;

                                                                                                                                                                            //Panda 2013/08/11 chloroplasts
                                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                                            {
                                                                                                                                                                                Get(#n);
                    }
                                                                                                                                                                            //Botsareus 4/18/2016 Protection against currupt file
                                                                                                                                                                            if (_WithVar_2822.chloroplasts < 0)
                                                                                                                                                                            {
                                                                                                                                                                                _WithVar_2822.chloroplasts = 0;
                                                                                                                                                                            }
                                                                                                                                                                            if (_WithVar_2822.chloroplasts > 32000)
                                                                                                                                                                            {
                                                                                                                                                                                _WithVar_2822.chloroplasts = 32000;
                                                                                                                                                                            }

                                                                                                                                                                            //Botsareus 12/3/2013 Read epigenetic information

                                                                                                                                                                            for (t = 0; t < 14; t++)
                                                                                                                                                                            {
                                                                                                                                                                                if (FileContinue(n))
                                                                                                                                                                                {
                                                                                                                                                                                    Get(#n);
                      }
                                                                                                                                                                            }

                                                                                                                                                                            //Botsareus 1/28/2014 Read robot tag

                                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                                            {
                                                                                                                                                                                Get(#n);
                    }

                                                                                                                                                                            //Read if robot is using sunbelt

                                                                                                                                                                            bool usesunbelt = false;//sunbelt mutations


                                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                                            {
                                                                                                                                                                                Get(#n);
                    }

                                                                                                                                                                            //Botsareus 3/28/2014 Read if disable chloroplasts

                                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                                            {
                                                                                                                                                                                Get(#n);
                    }

                                                                                                                                                                            //Botsareus 3/28/2014 Read kill resrictions

                                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                                            {
                                                                                                                                                                                Get(#n);
                    }
                                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                                            {
                                                                                                                                                                                Get(#n);
                    }
                                                                                                                                                                            if (_WithVar_2822.Chlr_Share_Delay > 8)
                                                                                                                                                                            {
                                                                                                                                                                                _WithVar_2822.Chlr_Share_Delay = 8; //Botsareus 4/18/2016 Protection against currupt file
                                                                                                                                                                            }
                                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                                            {
                                                                                                                                                                                Get(#n);
                    }
                                                                                                                                                                            if (_WithVar_2822.dq > 3)
                                                                                                                                                                            {
                                                                                                                                                                                _WithVar_2822.dq = 3; //Botsareus 4/18/2016 Protection against currupt file
                                                                                                                                                                            }

                                                                                                                                                                            //Botsareus 10/8/2015 Keep track of mutations from old dna file
                                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                                            {
                                                                                                                                                                                Get(#n);
                    }

                                                                                                                                                                            //Botsareus 6/22/2016 Actual velocity

                                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                                            {
                                                                                                                                                                                Get(#n);
                    }
                                                                                                                                                                            if (FileContinue(n))
                                                                                                                                                                            {
                                                                                                                                                                                Get(#n);
                    }

                                                                                                                                                                            _WithVar_2822.dq = _WithVar_2822.dq - IIf(_WithVar_2822.dq > 1, 2, 0);

                                                                                                                                                                            if (!.Veg)
                                                                                                                                                                            {
                                                                                                                                                                                if (y_eco_im > 0 & Form1.lblSaving.Visible == false)
                                                                                                                                                                                {
                                                                                                                                                                                    if (Trim(Right(_WithVar_2822.tag, 5)) != Trim(Left(_WithVar_2822.nrg + _WithVar_2822.nrg, 5)))
                                                                                                                                                                                    {
                                                                                                                                                                                        _WithVar_2822.dq = 2 + (_WithVar_2822.dq == 1) * true;
                                                                                                                                                                                    }
                                                                                                                                                                                    if (_WithVar_2822.FName != "Mutate.txt" && _WithVar_2822.FName != "Base.txt" && _WithVar_2822.FName != "Corpse")
                                                                                                                                                                                    {
                                                                                                                                                                                        _WithVar_2822.dq = 2 + (_WithVar_2822.dq == 1) * true;
                                                                                                                                                                                    }
                                                                                                                                                                                }
                                                                                                                                                                            }
                                                                                                                                                                            else
                                                                                                                                                                            {
                                                                                                                                                                                if (y_eco_im > 0 & _WithVar_2822.chloroplasts < 2000)
                                                                                                                                                                                {
                                                                                                                                                                                    _WithVar_2822.Dead = true;
                                                                                                                                                                                }
                                                                                                                                                                                if (TotalChlr > SimOpts.MaxPopulation)
                                                                                                                                                                                {
                                                                                                                                                                                    _WithVar_2822.Dead = true;
                                                                                                                                                                                }
                                                                                                                                                                            }
                                                                                                                                                                            if (_WithVar_2822.FName == "Corpse")
                                                                                                                                                                            {
                                                                                                                                                                                _WithVar_2822.nrg = 0;
                                                                                                                                                                            }

                                                                                                                                                                        //Botsareus 10/5/2015 Replaced with something better
                                                                                                                                                                        //Botsareus 9/16/2014 Read gene kill resrictions
                                                                                                                                                                        //    ReDim .delgenes(0)
                                                                                                                                                                        //    ReDim .delgenes(0).dna(0)
                                                                                                                                                                        //    Dim x As Integer
                                                                                                                                                                        //    Dim y As Integer
                                                                                                                                                                        //    Dim poz As Long
                                                                                                                                                                        //    poz = Seek(n)
                                                                                                                                                                        //    Get #n, , x
                                                                                                                                                                        //    If x < 0 Then
                                                                                                                                                                        //        Get #n, poz - 1, Fe
                                                                                                                                                                        //        If y_eco_im > 0 And Form1.lblSaving.Visible = False Then
                                                                                                                                                                        //            .dq = 2 + (.dq = 1) * True
                                                                                                                                                                        //        End If
                                                                                                                                                                        //        GoTo OldFile
                                                                                                                                                                        //    End If
                                                                                                                                                                        //    ReDim .delgenes(x)
                                                                                                                                                                        //    For y = 0 To x
                                                                                                                                                                        //        Get #n, , .delgenes(y).position
                                                                                                                                                                        //        Get #n, , k
                                                                                                                                                                        //        ReDim .delgenes(y).dna(k)
                                                                                                                                                                        //        For t = 0 To k
                                                                                                                                                                        //          Get #n, , .delgenes(y).dna(t).tipo
                                                                                                                                                                        //          Get #n, , .delgenes(y).dna(t).value
                                                                                                                                                                        //        Next t
                                                                                                                                                                        //    Next

                                                                                                                                                                        //read in any future data here

                                                                                                                                                                        OldFile:
                                                                                                                                                                            //burn through any new data from a different version
                                                                                                                                                                            While(FileContinue(n));
                                                                                                                                                                            Get(#n);
                    Wend();

                                                                                                                                                                            //grab these three FE codes
                                                                                                                                                                            Get(#n);
                    Get(#n);
                    Get(#n);

//don't you dare put anything after this!
//except some initialization stuff
                    _WithVar_2822.Vtimer = 0;
                                                                                                                                                                            _WithVar_2822.virusshot = 0;

                                                                                                                                                                            //Botsareus 2/21/2014 Special case reset sunbelt mutations

                                                                                                                                                                            if (!usesunbelt)
                                                                                                                                                                            {
                                                                                                                                                                                _WithVar_2822.Mutables.mutarray(P2UP) = 0;
                                                                                                                                                                                _WithVar_2822.Mutables.mutarray(CE2UP) = 0;
                                                                                                                                                                                _WithVar_2822.Mutables.mutarray(AmplificationUP) = 0;
                                                                                                                                                                                _WithVar_2822.Mutables.mutarray(TranslocationUP) = 0;
                                                                                                                                                                            }
                                                                                                                                                                        }

                                                                                                                                                                        private static bool FileContinue(ref int filenumber)
                                                                                                                                                                        {
                                                                                                                                                                            bool FileContinue = false;
                                                                                                                                                                            //three FE bytes (ie: 254) means we are at the end of the record

                                                                                                                                                                            byte Fe = 0;

                                                                                                                                                                            int Position = 0;

                                                                                                                                                                            int k = 0;


                                                                                                                                                                            FileContinue = false;
                                                                                                                                                                            Position() = Seek(filenumber);

                                                                                                                                                                            do
                                                                                                                                                                            {
                                                                                                                                                                                if (!EOF(filenumber))
                                                                                                                                                                                {
                                                                                                                                                                                    Get(#filenumber);
    }
                                                                                                                                                                                else
                                                                                                                                                                                {
                                                                                                                                                                                    FileContinue = false;
                                                                                                                                                                                    Fe = 254;
                                                                                                                                                                                }

                                                                                                                                                                                k = k + 1;

                                                                                                                                                                                if (Fe != 254)
                                                                                                                                                                                {
                                                                                                                                                                                    FileContinue = true;
                                                                                                                                                                                    //exit immediatly, we are done
                                                                                                                                                                                }
                                                                                                                                                                            } while (!(!FileContinue && k < 3);

                                                                                                                                                                            //reset position
                                                                                                                                                                            Get(#filenumber, Position() - 1, Fe);
  return FileContinue;
                                                                                                                                                                        }

                                                                                                                                                                        /*
                                                                                                                                                                        ' saves the body of the robot
                                                                                                                                                                        */
                                                                                                                                                                        private static void SaveRobotBody(ref int n, ref int r)
                                                                                                                                                                        {
                                                                                                                                                                            int t = 0;
                                                                                                                                                                            int k = 0;

                                                                                                                                                                            string s = "";

                                                                                                                                                                            string s2 = "";

                                                                                                                                                                            string temp = "";

                                                                                                                                                                            int longtmp = 0;


                                                                                                                                                                            const byte Fe = 254;
                                                                                                                                                                            // Dim space As Integer

                                                                                                                                                                            s = "Mutation Details removed in last save.";

                                                                                                                                                                            dynamic _WithVar_7882;
                                                                                                                                                                            _WithVar_7882 = rob(r);

                                                                                                                                                                            Put(#n);
    Put(#n);
    Put(#n);

// fisiche
    Put(#n);
    Put(#n);
    Put(#n);
    Put(#n);
    Put(#n);
    Put(#n); //momento angolare
    Put(#n); //momento torcente

    for (t = 0; t < MAXTIES; t++)
                                                                                                                                                                            {
                                                                                                                                                                                Put(#n);
      Put(#n);
      Put(#n);
      Put(#n);
      Put(#n);
      Put(#n);
      Put(#n);
      Put(#n);
      Put(#n);
      Put(#n);
      Put(#n);
      Put(#n);
      Put(#n);
      Put(#n);
      Put(#n);
      Next(t);

                                                                                                                                                                                // biologiche
                                                                                                                                                                                Put(#n);

//custom variables we're saving
      for (t = 1; t < 50; t++)
                                                                                                                                                                                {
                                                                                                                                                                                    Put(#n);
        Put(#n); //|
        Put(#n);
        Next(t);

                                                                                                                                                                                    Put(#n); //| variabili private

// macchina virtuale
        Put(#n);
        k = DnaLen(ref rob(r).dna());
                                                                                                                                                                                    Put(#n);
        for (t = 1; t < k; t++)
                                                                                                                                                                                    {
                                                                                                                                                                                        Put(#n);
          Put(#n);
          Next(t);

                                                                                                                                                                                        for (t = 0; t < 20; t++)
                                                                                                                                                                                        {
                                                                                                                                                                                            Put(#n);
            Next(t);

                                                                                                                                                                                            // informative
                                                                                                                                                                                            Put(#n);
            Put(#n);
            Put(#n);
            Put(#n);
            Put(#n);
            Put(#n);
            Put(#n);
            Put(#n);
            Put(#n);

// aspetto

            Put(#n);
            Put(#n);

// new features
            Put(#n);
            Put(#n);
            Put(#n);
            Put(#n);
            Put(#n);
            Put(#n);
            Put(#n);
            Put(#n);
            Put(#n);
            Put(#n);

            Put(#n);
            Put(#n);

            Put(#n);
            Put(#n);

//EricL 5/8/2006 New feature allows for saving sims without all the mutations details
            if (MDIForm1.instance.SaveWithoutMutations)
                                                                                                                                                                                            {
                                                                                                                                                                                                Put(#n);
              Put(#n);
            }
                                                                                                                                                                                            else
                                                                                                                                                                                            {
                                                                                                                                                                                                //EricL 5/3/2006  This needs some explaining.  It's all about backward compatability.  The length of the mutation details
                                                                                                                                                                                                //was stored as an Integer in older sim file versions.  It can overflow and go negative or even wrap positive
                                                                                                                                                                                                //again in sims with lots of mutations.  So, we test to see if it would have overflowed and it so, we write
                                                                                                                                                                                                //the interger 1 there instead of the actual length.  Since the actual details, being string descriptions,
                                                                                                                                                                                                //should never have length 1, this is a signal to the sim file read routine that the real length is a Long
                                                                                                                                                                                                //stored right after the Int.
                                                                                                                                                                                                if (CLng(Len(_WithVar_7882.LastMutDetail)) > CLng((2 ^ 15 - 1)))
                                                                                                                                                                                                {
                                                                                                                                                                                                    // Lots of mutations.  Tell the read routine that the real length is Long valued and coming up next.
                                                                                                                                                                                                    Put(#n);
                Put(#n); // The real length
              }
                                                                                                                                                                                                else
                                                                                                                                                                                                {
                                                                                                                                                                                                    //Not so many mutation details.  Leave the length as an Int for backward compatability
                                                                                                                                                                                                    Put(#n);
              }
                                                                                                                                                                                                Put(#n);
            }

                                                                                                                                                                                            //EricL 3/30/2006 Added the following line.  Looks like it was just missing.  Mutations were turned off after loading save...
                                                                                                                                                                                            Put(#n);

            for (t = 0; t < 20; t++)
                                                                                                                                                                                            {
                                                                                                                                                                                                Put(#n);
              Put(#n);
              Next(t);

                                                                                                                                                                                                Put(#n);
              Put(#n);

              Put(#n);
              Put(#n);
              Put(#n); //EricL  New for 2.42.8.  Save Robot number for use in re-mapping ties and shots when re-loaded

              Put(#n);
              Put(#n);
              Put(#n);
              Put(#n);
              Put(#n);
              Put(#n);
              Put(#n);
              Put(#n);

              if (_WithVar_7882.fertilized < 0)
                                                                                                                                                                                                {
                                                                                                                                                                                                    _WithVar_7882.spermDNAlen = 0;
                                                                                                                                                                                                }

                                                                                                                                                                                                Put(#n);
              for (t = 1; t < _WithVar_7882.spermDNAlen; t++)
                                                                                                                                                                                                {
                                                                                                                                                                                                    Put(#n);
                Put(#n);
                Next(t);
                                                                                                                                                                                                    Put(#n);

//Botsareus 10/5/2015 freeing up memory from Eric's obsolete ancestors code
                Put(#n);
                for (t = 0; t < 500; t++)
                                                                                                                                                                                                    {
                                                                                                                                                                                                        Put(#n);
                  Put(#n);
                  Put(#n);
                  Next(t);

                                                                                                                                                                                                        Put(#n);
                  Put(#n);

//Botsareus 2/23/2013 Rest of tie data
                  Put(#n);
                  for (t = 0; t < MAXTIES; t++)
                                                                                                                                                                                                        {
                                                                                                                                                                                                            Put(#n);
                    Put(#n);
                    Put(#n);
                    Put(#n);
                  }

                                                                                                                                                                                                        //Botsareus 4/9/2013 For genetic distance graph
                                                                                                                                                                                                        Put(#n);

//Panda 8/13/2013 Write chloroplasts
                  Put(#n);

//Botsareus 12/3/2013 Write epigenetic information
                  for (t = 0; t < 14; t++)
                                                                                                                                                                                                        {
                                                                                                                                                                                                            Put(#n);
                  }

                                                                                                                                                                                                        //Botsareus 1/28/2014 Write robot tag

                                                                                                                                                                                                        string blank = "";


                                                                                                                                                                                                        if (!.Veg)
                                                                                                                                                                                                        {
                                                                                                                                                                                                            if (y_eco_im > 0 & Form1.lblSaving.Visible == false && _WithVar_7882.dq < 2)
                                                                                                                                                                                                            {
                                                                                                                                                                                                                if (Left(_WithVar_7882.tag, 45) == Left(blank, 45))
                                                                                                                                                                                                                {
                                                                                                                                                                                                                    _WithVar_7882.tag = _WithVar_7882.FName;
                                                                                                                                                                                                                }
                                                                                                                                                                                                                _WithVar_7882.tag = Left(_WithVar_7882.tag, 45) + Left(_WithVar_7882.nrg + _WithVar_7882.nrg, 5);
                                                                                                                                                                                                            }
                                                                                                                                                                                                        }

                                                                                                                                                                                                        Put(#n);

//Botsareus 1/28/2014 Write if robot is using sunbelt

                  Put(#n);

//Botsareus 3/28/2014 Write if disable chloroplasts

                  Put(#n);

//Botsareus 3/28/2014 Read kill resrictions

                  Put(#n);
                  Put(#n);
                  Put(#n);

//Botsareus 10/8/2015 Keep track of mutations from old dna file

                  Put(#n);

//Botsareus 6/22/2016 Actual velocity

                  Put(#n);
                  Put(#n);


//Botsareus 10/5/2015 Replaced with something better
//    'Botsareus 9/16/2014 Write gene kill resrictions

//    Dim x As Integer
//    Dim y As Integer
//    x = UBound(.delgenes): Put #n, , x
//    For y = 0 To x
//        Put #n, , .delgenes(y).position
//        k = UBound(.delgenes(y).dna): Put #n, , k
//        For t = 0 To k
//          Put #n, , .delgenes(y).dna(t).tipo
//          Put #n, , .delgenes(y).dna(t).value
//        Next t
//    Next

//write any future data here

                  Put(#n);
                  Put(#n);
                  Put(#n);
              }

                                                                                                                                                                                                    /*
                                                                                                                                                                                                    ' saves a robot dna     !!!New routine from Carlo!!!
                                                                                                                                                                                                    'Botsareus 10/8/2015 Code simplification
                                                                                                                                                                                                    */
                                                                                                                                                                                                    static void salvarob(ref int n, ref string path)
                                                                                                                                                                                                    {
                                                                                                                                                                                                        string hold = "";

                                                                                                                                                                                                        string hashed = "";


                                                                                                                                                                                                        int a = 0;

                                                                                                                                                                                                        string epigene = "";


                                                                                                                                                                                                        VBCloseFile(1); ();
                                                                                                                                                                                                        VBOpenFile(1, path); ;
                                                                                                                                                                                                        hold = SaveRobHeader(ref n);

                                                                                                                                                                                                        //Botsareus 10/8/2015 New code to save epigenetic memory as gene

                                                                                                                                                                                                        if (UseEpiGene)
                                                                                                                                                                                                        {
                                                                                                                                                                                                            for (a = 971; a < 990; a++)
                                                                                                                                                                                                            {
                                                                                                                                                                                                                if (rob[n].mem(a) != 0)
                                                                                                                                                                                                                {
                                                                                                                                                                                                                    epigene = epigene + rob[n].mem(a) + " " + a + " store" + vbCrLf;
                                                                                                                                                                                                                }
                                                                                                                                                                                                            }

                                                                                                                                                                                                            if (epigene != "")
                                                                                                                                                                                                            {
                                                                                                                                                                                                                epigene = "start" + vbCrLf + epigene + "*.thisgene .delgene store" + vbCrLf + "stop";

                                                                                                                                                                                                                hold = hold + epigene;

                                                                                                                                                                                                            }

                                                                                                                                                                                                        }

                                                                                                                                                                                                        savingtofile = true; //Botsareus 2/28/2014 when saving to file the def sysvars should not save
                                                                                                                                                                                                        hold = hold + DetokenizeDNA(ref n, ref 0);
                                                                                                                                                                                                        savingtofile = false;
                                                                                                                                                                                                        hashed = Hash(ref hold, ref 20);
                                                                                                                                                                                                        VBWriteFile(1, hold); ;
                                                                                                                                                                                                        VBWriteFile(1, ""); ;
                                                                                                                                                                                                        VBWriteFile(1, "'#hash: " + hashed); ;
                                                                                                                                                                                                        string blank = "";

                                                                                                                                                                                                        if (Left(rob[n].tag, 45) != Left(blank, 45))
                                                                                                                                                                                                        {
                                                                                                                                                                                                            VBWriteFile(1, "'#tag:" + Left(rob[n].tag, 45) + vbCrLf); ;
                                                                                                                                                                                                        }
                                                                                                                                                                                                        VBCloseFile(1); ();

                                                                                                                                                                                                        //Botsareus 12/11/2013 Save mrates file
                                                                                                                                                                                                        Save_mrates(rob[n].Mutables, extractpath(ref path) + "\\" + extractexactname(ref extractname(ref path)) + ".mrate");

                                                                                                                                                                                                        if (x_restartmode > 0)
                                                                                                                                                                                                        {
                                                                                                                                                                                                            return;//Botsareus 10/8/2015 Can not rename robot in any special restart mode

                                                                                                                                                                                                        }

                                                                                                                                                                                                        if (MsgBox("Do you want to change robot's name to " + extractname(ref path) + " ?", vbYesNo, "Robot DNA saved") == vbYes)
                                                                                                                                                                                                        {
                                                                                                                                                                                                            rob[n].FName = extractname(ref path);
                                                                                                                                                                                                        }
                                                                                                                                                                                                    }

                                                                                                                                                                                                    /*
                                                                                                                                                                                                    ' saves a Teleporter
                                                                                                                                                                                                    */
                                                                                                                                                                                                    private static void SaveTeleporter(ref int n, ref int t)
                                                                                                                                                                                                    {
                                                                                                                                                                                                        const byte Fe = 254;

                                                                                                                                                                                                        dynamic _WithVar_7039;
                                                                                                                                                                                                        _WithVar_7039 = Teleporters(t);
                                                                                                                                                                                                        Put(#n);
    Put(#n);
    Put(#n);
    Put(#n);
    Put(#n);
    Put(#n);
    Put(#n);
    Put(#n);
    Put(#n);
    Put(#n);
    Put(#n);
    Put(#n);
    Put(#n);
    Put(#n);
    Put(#n);
    Put(#n);
    Put(#n);
    Put(#n);
    Put(#n);
    Put(#n);
    Put(#n);
    Put(#n);

//write any future data here

    Put(#n);
    Put(#n);
    Put(#n);
//don't you dare put anything after this!

}

                                                                                                                                                                                                    /*
                                                                                                                                                                                                    ' loads a Teleporter
                                                                                                                                                                                                    */
                                                                                                                                                                                                    private static void LoadTeleporter(ref int n, ref int t)
                                                                                                                                                                                                    {
                                                                                                                                                                                                        int k = 0;

                                                                                                                                                                                                        byte Fe = 0;


                                                                                                                                                                                                        dynamic _WithVar_6152;
                                                                                                                                                                                                        _WithVar_6152 = Teleporters(t);
                                                                                                                                                                                                        Get(#n);
    Get(#n);
    Get(#n);
    Get(#n);
    Get(#n);
    Get(#n);
    _WithVar_6152.path = Space(k);
                                                                                                                                                                                                        Get(#n);
    Get(#n);
    Get(#n);
    Get(#n);
    Get(#n);
    Get(#n);
    Get(#n);
    Get(#n);
    Get(#n);
    Get(#n);
    Get(#n);

    _WithVar_6152.teleportHeterotrophs = true;
                                                                                                                                                                                                        _WithVar_6152.InboundPollCycles = 10;
                                                                                                                                                                                                        _WithVar_6152.BotsPerPoll = 10;
                                                                                                                                                                                                        _WithVar_6152.PollCountDown = 10;

                                                                                                                                                                                                        if (FileContinue(n))
                                                                                                                                                                                                        {
                                                                                                                                                                                                            Get(#n);
    }
                                                                                                                                                                                                        if (FileContinue(n))
                                                                                                                                                                                                        {
                                                                                                                                                                                                            Get(#n);
    }
                                                                                                                                                                                                        if (FileContinue(n))
                                                                                                                                                                                                        {
                                                                                                                                                                                                            Get(#n);
    }
                                                                                                                                                                                                        if (FileContinue(n))
                                                                                                                                                                                                        {
                                                                                                                                                                                                            Get(#n);
    }
                                                                                                                                                                                                        if (FileContinue(n))
                                                                                                                                                                                                        {
                                                                                                                                                                                                            Get(#n);
    }


                                                                                                                                                                                                        //burn through any new data from a newer version
                                                                                                                                                                                                        While(FileContinue(n));
                                                                                                                                                                                                        Get(#n);
    Wend();

                                                                                                                                                                                                        //grab these three FE codes
                                                                                                                                                                                                        Get(#n);
    Get(#n);
    Get(#n);
//don't you dare put anything after this!

}

                                                                                                                                                                                                    /*
                                                                                                                                                                                                    ' saves a Obstacle
                                                                                                                                                                                                    */
                                                                                                                                                                                                    private static void SaveObstacle(ref int n, ref int t)
                                                                                                                                                                                                    {
                                                                                                                                                                                                        const byte Fe = 254;

                                                                                                                                                                                                        dynamic _WithVar_5847;
                                                                                                                                                                                                        _WithVar_5847 = Obstacles.Obstacles(t);
                                                                                                                                                                                                        Put(#n);
    Put(#n);
    Put(#n);
    Put(#n);
    Put(#n);
    Put(#n);

//write any future data here

    Put(#n);
    Put(#n);
    Put(#n);
//don't you dare put anything after this!

}

                                                                                                                                                                                                    /*
                                                                                                                                                                                                    ' loads an Obstacle
                                                                                                                                                                                                    */
                                                                                                                                                                                                    private static void LoadObstacle(ref int n, ref int t)
                                                                                                                                                                                                    {
                                                                                                                                                                                                        int k = 0;

                                                                                                                                                                                                        byte Fe = 0;


                                                                                                                                                                                                        dynamic _WithVar_9798;
                                                                                                                                                                                                        _WithVar_9798 = Obstacles.Obstacles(t);
                                                                                                                                                                                                        Get(#n);
    Get(#n);
    Get(#n);
    Get(#n);
    Get(#n);
    Get(#n);

//burn through any new data from a different version
    While(FileContinue(n));
                                                                                                                                                                                                        Get(#n);
    Wend();

                                                                                                                                                                                                        //grab these three FE codes
                                                                                                                                                                                                        Get(#n);
    Get(#n);
    Get(#n);

//don't you dare put anything after this!
}

                                                                                                                                                                                                    /*
                                                                                                                                                                                                    'Saves a Shot
                                                                                                                                                                                                    'New routine by EricL
                                                                                                                                                                                                    */
                                                                                                                                                                                                    private static void SaveShot(ref int n, ref int t)
                                                                                                                                                                                                    {
                                                                                                                                                                                                        int k = 0;

                                                                                                                                                                                                        int X = 0;


                                                                                                                                                                                                        const byte Fe = 254;

                                                                                                                                                                                                        dynamic _WithVar_7715;
                                                                                                                                                                                                        _WithVar_7715 = Shots(t);
                                                                                                                                                                                                        Put(#n); // exists?
    Put(#n); // position vector
    Put(#n); // old position vector
    Put(#n); // velocity vector
    Put(#n); // who shot it?
    Put(#n); // shot age
    Put(#n); // energy carrier
    Put(#n); // shot range (the maximum .nrg ever was)
    Put(#n); // power of shot for negative shots (or amt of shot, etc.), value to write for > 0
    Put(#n); // colour
    Put(#n); // carried location/value couple
    Put(#n); // does shot come from veg?
    Put(#n);
    Put(#n); // Which species fired the shot
    Put(#n); // Memory location for custom poison and venom
    Put(#n); // Value to insert into custom venom location

// Somewhere to store genetic code for a virus or sperm
    if ((_WithVar_7715.shottype == -7 || _WithVar_7715.shottype == -8) && _WithVar_7715.exist && _WithVar_7715.DnaLen > 0)
                                                                                                                                                                                                        {
                                                                                                                                                                                                            Put(#n);
      for (X = 1; X < _WithVar_7715.DnaLen; X++)
                                                                                                                                                                                                            {
                                                                                                                                                                                                                Put(#n);
        Put(#n);
        Next(X);
                                                                                                                                                                                                            } else
                                                                                                                                                                                                            {
                                                                                                                                                                                                                k = 0;
                                                                                                                                                                                                                Put(#n);
      }

                                                                                                                                                                                                            Put(#n); // which gene to copy in host bot
      Put(#n); // for virus shots (and maybe future types) this shot is stored inside the bot until it's ready to be launched


//write any future data here

      Put(#n);
      Put(#n);
      Put(#n);
//don't you dare put anything after this!

  }

                                                                                                                                                                                                        /*
                                                                                                                                                                                                        'Loads a Shot
                                                                                                                                                                                                        'New routine from EricL
                                                                                                                                                                                                        */
                                                                                                                                                                                                        private static void LoadShot(ref int n, ref int t)
                                                                                                                                                                                                        {
                                                                                                                                                                                                            int k = 0;

                                                                                                                                                                                                            int X = 0;

                                                                                                                                                                                                            byte Fe = 0;


                                                                                                                                                                                                            dynamic _WithVar_2687;
                                                                                                                                                                                                            _WithVar_2687 = Shots(t);
                                                                                                                                                                                                            Get(#n); // exists?
    Get(#n); // position vector
    Get(#n); // old position vector
    Get(#n); // velocity vector
    Get(#n); // who shot it?
    Get(#n); // shot age
    Get(#n); // energy carrier
    Get(#n); // shot range (the maximum .nrg ever was)
    Get(#n); // power of shot for negative shots (or amt of shot, etc.), value to write for > 0
    Get(#n); // colour
    Get(#n); // carried location/value couple
    Get(#n); // does shot come from veg?

    Get(#n);
    _WithVar_2687.FromSpecie = Space(k);
                                                                                                                                                                                                            Get(#n); // Which species fired the shot

    Get(#n); // Memory location for custom poison and venom
    Get(#n); // Value to insert into custom venom location


// Somewhere to store genetic code for a virus
    Get(#n);
    if (k > 0)
                                                                                                                                                                                                            {
                                                                                                                                                                                                                List<> dna_4226_tmp = new List<>();
                                                                                                                                                                                                                for (int redim_iter_3051 = 0; i < 0; redim_iter_3051++) { dna.Add(null); }
                                                                                                                                                                                                                for (X = 1; X < k; X++)
                                                                                                                                                                                                                {
                                                                                                                                                                                                                    Get(#n);
        Get(#n);
        Next(X);
                                                                                                                                                                                                                }

                                                                                                                                                                                                                _WithVar_2687.DnaLen = k;

                                                                                                                                                                                                                Get(#n); // which gene to copy in host bot
      Get(#n); // for virus shots (and maybe future types) this shot is stored inside the bot until it's ready to be launched

//burn through any new data from a different version
      While(FileContinue(n));
                                                                                                                                                                                                                Get(#n);
      Wend();

                                                                                                                                                                                                                //grab these three FE codes
                                                                                                                                                                                                                Get(#n);
      Get(#n);
      Get(#n);

//don't you dare put anything after this!
  }

                                                                                                                                                                                                            /*
                                                                                                                                                                                                            'M U T A T I O N  F I L E Botsareus 12/11/2013

                                                                                                                                                                                                            'generate mrates file
                                                                                                                                                                                                            */
                                                                                                                                                                                                            static void Save_mrates(ref mutationprobs mut, ref string FName)
                                                                                                                                                                                                            {
                                                                                                                                                                                                                byte m = 0;

                                                                                                                                                                                                                VBOpenFile(1, FName); ;
                                                                                                                                                                                                                mutationprobs _WithVar_mut;
                                                                                                                                                                                                                _WithVar_mut = mut;
                                                                                                                                                                                                                Write(#1, _WithVar_mut.PointWhatToChange);
    Write(#1, _WithVar_mut.CopyErrorWhatToChange);
    for (m = 0; m < 10; m++)
                                                                                                                                                                                                                { //Need to change this if adding more mutation types (Trying to keep some backword compatability here)
                                                                                                                                                                                                                    Write(#1, _WithVar_mut.mutarray(m));
      Write(#1, _WithVar_mut.Mean(m));
      Write(#1, _WithVar_mut.StdDev(m));
    }
                                                                                                                                                                                                                mut = _WithVar_mut;
                                                                                                                                                                                                                VBCloseFile(1); ();
                                                                                                                                                                                                            }

                                                                                                                                                                                                            /*
                                                                                                                                                                                                            'load mrates file
                                                                                                                                                                                                            */
                                                                                                                                                                                                            public static mutationprobs Load_mrates(ref string FName)
                                                                                                                                                                                                            {
                                                                                                                                                                                                                mutationprobs Load_mrates = null;
                                                                                                                                                                                                                byte m = 0;

                                                                                                                                                                                                                VBOpenFile(1, FName); ;
                                                                                                                                                                                                                mutationprobs _WithVar_Load_mrates;
                                                                                                                                                                                                                _WithVar_Load_mrates = Load_mrates;
                                                                                                                                                                                                                Input(#1, _WithVar_Load_mrates.PointWhatToChange);
    Input(#1, _WithVar_Load_mrates.CopyErrorWhatToChange);
    for (m = 0; m < 10; m++)
                                                                                                                                                                                                                { //Need to change this if adding more mutation types (needs to have eofs if more then 10 for backword compatability)
                                                                                                                                                                                                                    Input(#1, _WithVar_Load_mrates.mutarray(m));
      Input(#1, _WithVar_Load_mrates.Mean(m));
      Input(#1, _WithVar_Load_mrates.StdDev(m));
    }
                                                                                                                                                                                                                Load_mrates = _WithVar_Load_mrates;
                                                                                                                                                                                                                VBCloseFile(1); ();
                                                                                                                                                                                                                return Load_mrates;
                                                                                                                                                                                                            }

                                                                                                                                                                                                            /*
                                                                                                                                                                                                            'D A T A C O N V E R S I O N S Botsareus 12/18/2013
                                                                                                                                                                                                            */
                                                                                                                                                                                                            private static int sint(int lval)
                                                                                                                                                                                                            {
                                                                                                                                                                                                                int sint = 0;
                                                                                                                                                                                                                lval = lval % 32000;
                                                                                                                                                                                                                sint = lval;
                                                                                                                                                                                                                return sint;
                                                                                                                                                                                                            }
                                                                                                                                                                                                        }
