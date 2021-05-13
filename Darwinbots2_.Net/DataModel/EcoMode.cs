﻿using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Iersera.DataModel
{
    internal class EcoMode
    {
        private const string FileName = "im.gset";
        public int EcoIm { get; set; }

        public static async Task<int> Load()
        {
            if (!File.Exists(FileName))
                return 0;

            try
            {
                var input = await File.ReadAllTextAsync(FileName);
                var data = JsonSerializer.Deserialize<EcoMode>(input);

                return data.EcoIm;
            }
            catch (JsonException)
            {
                return 0;
            }
            catch (IOException)
            {
                return 0;
            }
        }

        public static async Task Save(int ecoIm)
        {
            await File.WriteAllTextAsync(FileName, JsonSerializer.Serialize(new EcoMode
            {
                EcoIm = ecoIm
            }));
        }
    }
}
