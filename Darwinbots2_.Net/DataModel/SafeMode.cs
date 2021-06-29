using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace DarwinBots.DataModel
{
    internal class SafeMode
    {
        private const string FileName = "safemode.gset";
        public bool Enable { get; set; }

        public static async Task<bool> Load()
        {
            if (!File.Exists(FileName))
                return false;

            try
            {
                var input = await File.ReadAllTextAsync(FileName);
                var data = JsonSerializer.Deserialize<SafeMode>(input);

                return data?.Enable ?? false;
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

        public static async Task Save(bool enable)
        {
            await File.WriteAllTextAsync(FileName, JsonSerializer.Serialize(new SafeMode
            {
                Enable = enable
            }));
        }
    }
}
