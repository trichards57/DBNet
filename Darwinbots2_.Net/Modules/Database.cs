using DarwinBots.Model;
using Microsoft.Win32;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DarwinBots.Modules
{
    internal static class Database
    {
        private static string _snapName = "";

        public static async Task AddRecord(Robot rob)
        {
            const string folder = "Autosave";

            Directory.CreateDirectory(folder);

            var path1 = $"{folder}\\DeadRobots.snp";
            var path2 = $"{folder}\\DeadRobots_Mutations.txt";

            var deadRobotsExists = File.Exists(path1);
            var deadRobotsMutationExists = File.Exists(path2);

            await using var deadRobotsFile = new StreamWriter(path1);
            await using var deadRobotsMutationFile = new StreamWriter(path2);

            if (!deadRobotsExists)
                await deadRobotsFile.WriteLineAsync("Rob id,Parent id,Founder name,Generation,Birth cycle,Age,Mutations,New mutations,Dna length,Offspring number,kills,Energy,Chloroplasts");

            if (!deadRobotsMutationExists)
                await deadRobotsMutationFile.WriteLineAsync("Rob id,Mutation History");

            if (rob.Dna.Count == 1)
                return;

            await deadRobotsMutationFile.WriteLineAsync($"{rob.AbsNum}, {rob.LastMutationDetail}");

            await deadRobotsFile.WriteLineAsync($"{rob.AbsNum},{rob.Parent},{rob.FName},{rob.Generation},{rob.BirthCycle},{rob.Age},{rob.Mutations},{rob.LastMutation},{rob.Dna.Count},{rob.SonNumber},{rob.Kills},{rob.Energy + rob.Body * 10},{rob.Chloroplasts}");
            await deadRobotsFile.WriteLineAsync(DnaTokenizing.DetokenizeDna(rob).Trim());
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

            _snapName = dialog.FileName;

            await using var snapFile = new StreamWriter(_snapName);

            await snapFile.WriteLineAsync("Rob id,Parent id,Founder name,Generation,Birth cycle,Age,Mutations,New mutations,Dna length,Offspring number,kills,Fitness,Energy,Chloroplasts");

            var m = MessageBox.Show("Do you want to generate mutation history file as well?", "Save Mutation History", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.Yes);

            StreamWriter mutationsFiles = null;

            try
            {
                if (m == MessageBoxResult.Yes)
                {
                    mutationsFiles = new StreamWriter(Path.Join(Path.GetDirectoryName(_snapName), Path.GetFileNameWithoutExtension(_snapName) + "_Mutations.txt"));
                    await mutationsFiles.WriteLineAsync("Rob id,Mutation History");
                }

                // Records a snapshot of all living robots in a snapshot database
                foreach (var rob in Globals.RobotsManager.Robots.Where(r => r.Exists && r.Dna.Count > 1))
                {
                    if (mutationsFiles != null)
                        await mutationsFiles.WriteLineAsync($"{rob.AbsNum},{rob.LastMutationDetail}");

                    await snapFile.WriteLineAsync($"{rob.AbsNum},{rob.Parent},{rob.FName},{rob.Generation},{rob.BirthCycle},{rob.Age},{rob.Mutations},{rob.LastMutation},{rob.Dna.Count},{rob.SonNumber},{rob.Kills},{rob.Energy + rob.Body * 10},{rob.Chloroplasts}");
                    await snapFile.WriteLineAsync(DnaTokenizing.DetokenizeDna(rob).Trim());
                }

                MessageBox.Show("Saved snapshot successfully.");
            }
            finally
            {
                await snapFile.FlushAsync();
                if (mutationsFiles != null)
                {
                    await mutationsFiles.FlushAsync();
                    await mutationsFiles.DisposeAsync();
                }
            }
        }
    }
}
