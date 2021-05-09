using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Iersera.DataModel
{
    internal class SafeMode
    {
        public bool Enable { get; set; }

        public static async Task Save(bool enable)
        {
            await File.WriteAllTextAsync("safemode.gset", JsonSerializer.Serialize(new SafeMode
            {
                Enable = enable
            }));
        }
    }
}
