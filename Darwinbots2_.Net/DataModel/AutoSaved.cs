using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Iersera.DataModel
{
    internal class AutoSaved
    {
        private const string FileName = "autosaved.gset";
        public bool State { get; set; }

        public static async Task<bool> Load()
        {
            if (!File.Exists(FileName))
                return false;

            try
            {
                var input = await File.ReadAllTextAsync(FileName);
                var data = JsonSerializer.Deserialize<AutoSaved>(input);

                return data.State;
            }
            catch (JsonException)
            {
                return false;
            }
            catch (IOException)
            {
                return false;
            }
        }

        public static async Task Save(bool state)
        {
            await File.WriteAllTextAsync(FileName, JsonSerializer.Serialize(new AutoSaved
            {
                State = state
            }));
        }
    }
}
