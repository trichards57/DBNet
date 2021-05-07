using DBNet.Forms;
using static DNATokenizing;
using static Globals;
using static Microsoft.VisualBasic.Constants;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.FileSystem;
using static Microsoft.VisualBasic.Interaction;
using static Microsoft.VisualBasic.Strings;
using static Robots;
using static System.Math;
using static VBExtension;

internal static class Database
{
    private static string SnapName = "";

    public static void AddRecord(int rn)
    {
        var v = ",";
        var path1 = MDIForm1.instance.MainDir + "\\Autosave\\DeadRobots.snp";
        var path2 = MDIForm1.instance.MainDir + "\\Autosave\\DeadRobots_Mutations.txt";
        // TODO (not supported): On Error GoTo getout
        if (Dir(path1) == "")
        { //write first line if no data
            VBOpenFile(177, path1); 
            VBWriteFile(177, "Rob id,Parent id,Founder name,Generation,Birth cycle,Age,Mutations,New mutations,Dna length,Offspring number,kills,Fitness,Energy,Chloroplasts"); ;
            VBCloseFile(177);
        }
        if (Dir(path2) == "")
        { //write first line if no data
            VBOpenFile(178, path2); 
            VBWriteFile(178, "Rob id,Mutation History"); 
            VBCloseFile(178);
        }
        //write the record
        VBOpenFile(177, path1); 
        VBOpenFile(178, path2); 

        var rob = Robots.rob[rn];

        if (rob.DnaLen == 1)
        {
            VBCloseFile(177);
            VBCloseFile(178);
            return;
        }

        VBWriteFile(178, vbCrLf + CStr(rob.AbsNum) + v);
        VBWriteFile(178, vbCrLf + rob.LastMutDetail);

        VBWriteFile(177, vbCrLf + vbCrLf + CStr(rob.AbsNum) + v + CStr(rob.parent) + v + rob.FName + v + CStr(rob.generation) + v + CStr(rob.BirthCycle) + v + CStr(rob.age) + v + CStr(rob.Mutations) + v);
        VBWriteFile(177, CStr(rob.LastMut) + v + CStr(rob.DnaLen) + v + CStr(rob.SonNumber) + v + CStr(rob.Kills) + v);
        //lets figureout fitness

        var sEnergy = IIf(intFindBestV2 > 100, 100, intFindBestV2) / 100;
        var sPopulation = IIf(intFindBestV2 < 100, 100, 200 - intFindBestV2) / 100;
        Form1.instance.TotalOffspring = 1;
        var s = Form1.instance.score(rn, 1, 10, 0) + Robots.rob[rn].nrg + Robots.rob[rn].body * 10; //Botsareus 5/22/2013 Advanced fit test
        if (s < 0)
            s = 0; //Botsareus 9/23/2016 Bug fix

        s = Pow(Form1.instance.TotalOffspring, sPopulation) * Pow(s, sEnergy);
        VBWriteFile(177, CStr(s) + v + CStr(Robots.rob[rn].nrg + Robots.rob[rn].body * 10) + v + rob.chloroplasts + vbCrLf); ;
        savingtofile = true;

        var d = DetokenizeDNA(rn) + vbCrLf;
        if (Mid(d, Len(d) - 3, 2) == vbCrLf)
        {
            d = Left(d, Len(d) - 2); //Borsareus 7/22/2014 a bug fix
        }
        savingtofile = false;
        VBWriteFile(177, d); ;

        VBCloseFile(177);
        VBCloseFile(178);
    }

    public static void Snapshot()
    {
        Form1.instance.CommonDialog1.FileName = "";
        SnapBrowse();
        if (Form1.instance.CommonDialog1.FileName == "")
        {
            return;
        }
        var m = MsgBox("Do you want to generate mutation history file as well?", vbYesNo | vbInformation) == vbYes;
        VBOpenFile(3, SnapName);
        if (m)
        {
            VBOpenFile(5, extractexactname(SnapName) + "_Mutations.txt"); ;
        }
        VBWriteFile(3, "Rob id,Parent id,Founder name,Generation,Birth cycle,Age,Mutations,New mutations,Dna length,Offspring number,kills,Fitness,Energy,Chloroplasts" + vbCrLf,); ;
        if (m)
        {
            VBWriteFile(5, "Rob id,Mutation History"); ;
        }
        //records a snapshot of all living robots in a snapshot database
        var v = ",";
        Form1.instance.GraphLab.Visible = true;
        var rn = 0;
        for (rn = 1; rn < MaxRobs; rn++)
        {
            var d = "";
            if (rob[rn].exist && rob[rn].DnaLen > 1)
            {
                var rob = Robots.rob[rn];

                if (m)
                {
                    VBWriteFile(5, vbCrLf + CStr(rob.AbsNum), v,); ;
                    VBWriteFile(5, vbCrLf + rob.LastMutDetail); ;
                }

                VBWriteFile(3, vbCrLf + vbCrLf + CStr(rob.AbsNum) + v + CStr(rob.parent) + v + rob.FName + v + CStr(rob.generation) + v + CStr(rob.BirthCycle) + v + CStr(rob.age) + v + CStr(rob.Mutations) + v);
                VBWriteFile(3, CStr(rob.LastMut) + v + CStr(rob.DnaLen) + v + CStr(rob.SonNumber) + v + CStr(rob.Kills) + v);
                //lets figureout fitness

                var sEnergy = IIf(intFindBestV2 > 100, 100, intFindBestV2) / 100;
                var sPopulation = IIf(intFindBestV2 < 100, 100, 200 - intFindBestV2) / 100;
                Form1.instance.TotalOffspring = 1;
                var s = Form1.instance.score(rn, 1, 10, 0) + Robots.rob[rn].nrg + Robots.rob[rn].body * 10; //Botsareus 5/22/2013 Advanced fit test
                if (s < 0)
                    s = 0; //Botsareus 9/23/2016 Bug fix

                s = Pow(Form1.instance.TotalOffspring, sPopulation) * Pow(s, sEnergy);
                VBWriteFile(3, CStr(s) + v + CStr(Robots.rob[rn].nrg + Robots.rob[rn].body * 10) + v + rob.chloroplasts + vbCrLf);
                d = "";
                savingtofile = true;
                d = DetokenizeDNA(rn) + vbCrLf;
                if (Mid(d, Len(d) - 3, 2) == vbCrLf)
                {
                    d = Left(d, Len(d) - 2); //Borsareus 7/22/2014 a bug fix
                }
                savingtofile = false;
                VBWriteFile(3, d);
            }
            Form1.instance.GraphLab.Caption = "Calculating a snapshot: " + Int(rn / MaxRobs * 100) + "%";
            DoEvents();
        }
        Form1.instance.GraphLab.Visible = false;
        VBCloseFile(3);
        if (m)
        {
            VBCloseFile(5);
        }
        MsgBox("Saved snapshot successfully.");
    }

    private static void SnapBrowse()
    {
        Form1.instance.CommonDialog1.InitDir = MDIForm1.instance.MainDir + "/database";
        Form1.instance.CommonDialog1.DialogTitle = "Select a name for your snapshot file.";
        Form1.instance.CommonDialog1.Filter = "Snapshot Database (*.snp)|*.snp";
        Form1.instance.CommonDialog1.ShowSave();
        SnapName = Form1.instance.CommonDialog1.FileName;
    }
}
