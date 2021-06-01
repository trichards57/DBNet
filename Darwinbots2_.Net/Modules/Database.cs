using DBNet.Forms;
using Iersera.Model;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

internal static class Database
{
    private static string SnapName = "";

    public static async Task AddRecord(robot rob)
    {
        var folder = "Autosave";

        Directory.CreateDirectory(folder);

        var path1 = $"{folder}\\DeadRobots.snp";
        var path2 = $"{folder}\\DeadRobots_Mutations.txt";

        var deadRobotsExists = File.Exists(path1);
        var deadRobotsMutationExists = File.Exists(path2);

        using var deadRobotsFile = new StreamWriter(path1);
        using var deadRobotsMutationFile = new StreamWriter(path2);

        if (!deadRobotsExists)
            await deadRobotsFile.WriteLineAsync("Rob id,Parent id,Founder name,Generation,Birth cycle,Age,Mutations,New mutations,Dna length,Offspring number,kills,Fitness,Energy,Chloroplasts");

        if (!deadRobotsMutationExists)
            await deadRobotsMutationFile.WriteLineAsync("Rob id,Mutation History");

        if (rob.dna.Count == 1)
            return;

        var fitness = GetFitness(rob);

        await deadRobotsMutationFile.WriteLineAsync($"{rob.AbsNum}, {rob.LastMutDetail}");

        await deadRobotsFile.WriteLineAsync($"{rob.AbsNum},{rob.parent},{rob.FName},{rob.generation},{rob.BirthCycle},{rob.age},{rob.Mutations},{rob.LastMut},{rob.dna.Count},{rob.SonNumber},{rob.Kills},{fitness},{rob.nrg + rob.body * 10},{rob.chloroplasts}");
        await deadRobotsFile.WriteLineAsync(DNATokenizing.DetokenizeDNA(rob).Trim());
    }

    public static async Task Snapshot()
    {
        var dialog = new SaveFileDialog
        {
            FileName = "",
            InitialDirectory = "database",
            Title = "Select a name for your snapshot file.",
            Filter = "Snapshot Database (*.snp)|*.snp"
        };

        var result = dialog.ShowDialog();

        if (!result != true)
            return;

        SnapName = dialog.FileName;

        using var snapFile = new StreamWriter(SnapName);

        await snapFile.WriteLineAsync("Rob id,Parent id,Founder name,Generation,Birth cycle,Age,Mutations,New mutations,Dna length,Offspring number,kills,Fitness,Energy,Chloroplasts");

        var m = MessageBox.Show("Do you want to generate mutation history file as well?", "Save Mutation History", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.Yes);

        StreamWriter mutationsFiles = null;

        try
        {
            if (m == MessageBoxResult.Yes)
            {
                mutationsFiles = new StreamWriter(Path.Join(Path.GetDirectoryName(SnapName), Path.GetFileNameWithoutExtension(SnapName) + "_Mutations.txt"));
                await mutationsFiles.WriteLineAsync("Rob id,Mutation History");
            }

            //records a snapshot of all living robots in a snapshot database
            Form1.instance.GraphLab.Visibility = Visibility.Visible;
            foreach (var rob in Robots.rob.Where(r => r.exist && r.dna.Count > 1))
            {
                await mutationsFiles?.WriteLineAsync($"{rob.AbsNum},{rob.LastMutDetail}");

                var fitness = GetFitness(rob);

                await snapFile.WriteLineAsync($"{rob.AbsNum},{rob.parent},{rob.FName},{rob.generation},{rob.BirthCycle},{rob.age},{rob.Mutations},{rob.LastMut},{rob.dna.Count},{rob.SonNumber},{rob.Kills},{fitness},{rob.nrg + rob.body * 10},{rob.chloroplasts}");
                await snapFile.WriteLineAsync(DNATokenizing.DetokenizeDNA(rob).Trim());
            }

            Form1.instance.GraphLab.Visibility = Visibility.Hidden;

            MessageBox.Show("Saved snapshot successfully.");
        }
        finally
        {
            await snapFile.FlushAsync();
            await mutationsFiles.FlushAsync();
            mutationsFiles?.Dispose();
        }
    }

    private static double GetFitness(robot rob)
    {
        var sEnergy = (Globals.intFindBestV2 > 100 ? 100 : Globals.intFindBestV2) / 100;
        var sPopulation = (Globals.intFindBestV2 < 100 ? 100 : 200 - Globals.intFindBestV2) / 100;
        Form1.instance.TotalOffspring = 1;
        var fitness = Form1.instance.score(rob, 1, 10, 0) + rob.nrg + rob.body * 10; //Botsareus 5/22/2013 Advanced fit test
        if (fitness < 0)
            fitness = 0; //Botsareus 9/23/2016 Bug fix

        fitness = Math.Pow(Form1.instance.TotalOffspring, sPopulation) * Math.Pow(fitness, sEnergy);
        return fitness;
    }
}
