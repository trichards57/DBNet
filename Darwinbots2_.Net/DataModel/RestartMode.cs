using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Iersera.DataModel
{
    internal class RestartMode
    {
        public int FileNumber { get; set; }
        public int Mode { get; set; }

        public static async Task Save(int mode, int fileNumber)
        {
            await File.WriteAllTextAsync("restartmode.gset", JsonSerializer.Serialize(new RestartMode
            {
                Mode = mode,
                FileNumber = fileNumber
            }));
        }
    }
}
