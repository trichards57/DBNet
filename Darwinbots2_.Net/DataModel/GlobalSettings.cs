using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace DarwinBots.DataModel
{
    internal class GlobalSettings
    {
        private const string FileName = "global.gset";

        public int BodyFix { get; internal set; }
        public bool BoyLablDisp { get; internal set; }
        public bool ChSeedLoadSim { get; internal set; }
        public bool ChSeedStartNew { get; internal set; }
        public bool Delta2 { get; internal set; }
        public int DeltaDevChance { get; internal set; }
        public double DeltaDevExp { get; internal set; }
        public double DeltaDevLn { get; internal set; }
        public int DeltaMainChance { get; internal set; }
        public double DeltaMainExp { get; internal set; }
        public double DeltaMainLn { get; internal set; }
        public int DeltaPM { get; internal set; }
        public int DeltaWTC { get; internal set; }
        public int Disqualify { get; internal set; }
        public bool EpiReset { get; internal set; }
        public int EpiResetOP { get; internal set; }
        public double EpiResetTemp { get; internal set; }
        public bool GraphUp { get; internal set; }
        public bool HideDB { get; internal set; }
        public int IntFindBestV2 { get; internal set; }
        public string LeagueSourceDir { get; internal set; }
        public bool NormMut { get; internal set; }
        public bool ReproFix { get; internal set; }
        public bool ScreenRatioFix { get; internal set; }
        public int StartChlr { get; internal set; }
        public bool StartNovId { get; internal set; }
        public bool SunBelt { get; internal set; }
        public bool UseEpiGene { get; internal set; }
        public bool UseIntRnd { get; internal set; }
        public bool UseOldColor { get; internal set; }
        public bool UseSafeMode { get; internal set; }
        public bool UseStepladder { get; internal set; }
        public int ValMaxNormMut { get; internal set; }
        public int ValNormMut { get; internal set; }
        public int XFudge { get; internal set; }
        public int XResKillChlr { get; internal set; }
        public bool XResKillMb { get; internal set; }
        public bool XResKillMbVeg { get; internal set; }
        public int XResOther { get; internal set; }
        public int XResOtherVeg { get; internal set; }
        public bool YGraphs { get; internal set; }
        public int YHidePredCycl { get; internal set; }
        public double YLFOR { get; internal set; }
        public bool YNormSize { get; internal set; }
        public int YResKillChlr { get; internal set; }
        public bool YResKillDq { get; internal set; }
        public bool YResKillDqVeg { get; internal set; }
        public bool YResKillMb { get; internal set; }
        public bool YResKillMbVeg { get; internal set; }
        public int YResOther { get; internal set; }
        public int YResOtherVeg { get; internal set; }
        public string YRobDir { get; internal set; }
        public int YZblen { get; internal set; }

        public static async Task<GlobalSettings> Load()
        {
            if (!File.Exists(FileName))
                return null;

            try
            {
                var input = await File.ReadAllTextAsync(FileName);
                return JsonSerializer.Deserialize<GlobalSettings>(input);
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

        public static async Task Save(GlobalSettings settings)
        {
            await File.WriteAllTextAsync(FileName, JsonSerializer.Serialize(settings));
        }
    }
}
