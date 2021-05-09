using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Iersera.DataModel
{
    internal class AutoSaved
    {
        public bool State { get; set; }

        public static async Task Save(bool state)
        {
            await File.WriteAllTextAsync("safemode.gset", JsonSerializer.Serialize(new AutoSaved
            {
                State = state
            }));
        }
    }
}
