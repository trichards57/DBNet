using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace DarwinBots.DataModel
{
    public class EvoData
    {
        private const string FileName = @"evolution\data.gset";

        public int curr_dna_size { get; internal set; }
        public int hidePredCycl { get; internal set; }
        public int Init_hidePredCycl { get; internal set; }
        public double LFOR { get; internal set; }
        public double LFORcorr { get; internal set; }
        public bool LFORdir { get; internal set; }
        public int target_dna_size { get; internal set; }
        public int y_Stgwins { get; internal set; }

        public static async Task<EvoData> Load()
        {
            if (!File.Exists(FileName))
                return null;

            try
            {
                var input = await File.ReadAllTextAsync(FileName);
                return JsonSerializer.Deserialize<EvoData>(input);
            }
            catch (JsonException)
            {
                return null;
            }
            catch (IOException)
            {
                return null;
            }
        }

        public static async Task Save(EvoData settings)
        {
            await File.WriteAllTextAsync(FileName, JsonSerializer.Serialize(settings));
        }
    }
}
