using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace DarwinBots.DataModel
{
    internal class RestartMode
    {
        private const string FileName = "restartmode.gset";
        public int FileNumber { get; set; }
        public int Mode { get; set; }

        public static async Task<RestartMode> Load()
        {
            if (!File.Exists(FileName))
                return new RestartMode { FileNumber = 0, Mode = 0 };

            try
            {
                var input = await File.ReadAllTextAsync(FileName);
                var data = JsonSerializer.Deserialize<RestartMode>(input);

                return data ?? new RestartMode { FileNumber = 0, Mode = 0 };
            }
            catch (JsonException)
            {
                return new RestartMode { FileNumber = 0, Mode = 0 };
            }
            catch (IOException)
            {
                return new RestartMode { FileNumber = 0, Mode = 0 };
            }
        }

        public static async Task Save(int mode, int fileNumber)
        {
            await File.WriteAllTextAsync(FileName, JsonSerializer.Serialize(new RestartMode
            {
                Mode = mode,
                FileNumber = fileNumber
            }));
        }
    }
}
