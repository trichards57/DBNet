using System.Linq;

namespace DBNetModel.DNA
{
    public class SystemVariableAddresses
    {
        public static int Aim { get; } = Tokenizer.SystemVariables.First(t => t.Name == "aim").Address;
        public static int AimLeft { get; } = Tokenizer.SystemVariables.First(t => t.Name == "aimleft").Address;
        public static int AimRight { get; } = Tokenizer.SystemVariables.First(t => t.Name == "aimright").Address;
        public static int Body { get; } = Tokenizer.SystemVariables.First(t => t.Name == "body").Address;
        public static int Chloroplasts { get; } = Tokenizer.SystemVariables.First(t => t.Name == "chlr").Address;
        public static int DeleteGene { get; } = Tokenizer.SystemVariables.First(t => t.Name == "delgene").Address;
        public static int DirDown { get; } = Tokenizer.SystemVariables.First(t => t.Name == "dirdn").Address;
        public static int DirDx { get; } = Tokenizer.SystemVariables.First(t => t.Name == "dirdx").Address;
        public static int DirSx { get; } = Tokenizer.SystemVariables.First(t => t.Name == "dirsx").Address;
        public static int DirUp { get; } = Tokenizer.SystemVariables.First(t => t.Name == "dirup").Address;
        public static int DNALength { get; } = Tokenizer.SystemVariables.First(t => t.Name == "dnalen").Address;
        public static int FeedBody { get; } = Tokenizer.SystemVariables.First(t => t.Name == "fdbody").Address;
        public static int Genes { get; } = Tokenizer.SystemVariables.First(t => t.Name == "genes").Address;
        public static int Light { get; } = Tokenizer.SystemVariables.First(t => t.Name == "light").Address;
        public static int MakeChloroplasts { get; } = Tokenizer.SystemVariables.First(t => t.Name == "mkchlr").Address;
        public static int MakePoison { get; } = Tokenizer.SystemVariables.First(t => t.Name == "mkpoison").Address;
        public static int MakeShell { get; } = Tokenizer.SystemVariables.First(t => t.Name == "mkshell").Address;
        public static int MakeSlime { get; } = Tokenizer.SystemVariables.First(t => t.Name == "mkslime").Address;
        public static int MakeVenom { get; } = Tokenizer.SystemVariables.First(t => t.Name == "mkvenom").Address;
        public static int MakeVirus { get; } = Tokenizer.SystemVariables.First(t => t.Name == "mkvirus").Address;
        public static int Mass { get; } = Tokenizer.SystemVariables.First(t => t.Name == "mass").Address;
        public static int MaxVelocity { get; } = Tokenizer.SystemVariables.First(t => t.Name == "maxvel").Address;
        public static int PermamentWaste { get; } = Tokenizer.SystemVariables.First(t => t.Name == "pwaste").Address;
        public static int Poison { get; } = Tokenizer.SystemVariables.First(t => t.Name == "poison").Address;
        public static int Poisoned { get; } = Tokenizer.SystemVariables.First(t => t.Name == "paralyzed").Address;
        public static int RemoveChloroplasts { get; } = Tokenizer.SystemVariables.First(t => t.Name == "rmchlr").Address;
        public static int RobotAge { get; } = Tokenizer.SystemVariables.First(t => t.Name == "robage").Address;
        public static int SetAim { get; } = Tokenizer.SystemVariables.First(t => t.Name == "setaim").Address;
        public static int Shell { get; } = Tokenizer.SystemVariables.First(t => t.Name == "shell").Address;
        public static int Shoot { get; } = Tokenizer.SystemVariables.First(t => t.Name == "shoot").Address;
        public static int Slime { get; } = Tokenizer.SystemVariables.First(t => t.Name == "slime").Address;
        public static int StoreBody { get; } = Tokenizer.SystemVariables.First(t => t.Name == "strbody").Address;
        public static int Timer { get; } = Tokenizer.SystemVariables.First(t => t.Name == "timer").Address;
        public static int VelocityDown { get; } = Tokenizer.SystemVariables.First(t => t.Name == "veldn").Address;
        public static int VelocityLeft { get; } = Tokenizer.SystemVariables.First(t => t.Name == "velsx").Address;
        public static int VelocityRight { get; } = Tokenizer.SystemVariables.First(t => t.Name == "veldx").Address;
        public static int VelocityScalar { get; } = Tokenizer.SystemVariables.First(t => t.Name == "velscalar").Address;
        public static int VelocityUp { get; } = Tokenizer.SystemVariables.First(t => t.Name == "velup").Address;
        public static int VirusShoot { get; } = Tokenizer.SystemVariables.First(t => t.Name == "vshoot").Address;
        public static int VirusTimer { get; } = Tokenizer.SystemVariables.First(t => t.Name == "vtimer").Address;
        public static int Waste { get; } = Tokenizer.SystemVariables.First(t => t.Name == "waste").Address;
        public static int EyeStart { get; } = Tokenizer.SystemVariables.First(t => t.Name == "eye1").Address;
        public static int EyeEnd { get; } = Tokenizer.SystemVariables.First(t => t.Name == "eye9").Address;
        public static int SetBouyancy { get; } = Tokenizer.SystemVariables.First(t => t.Name == "setboy").Address;
        public static int ReadBouyancy { get; } = Tokenizer.SystemVariables.First(t => t.Name == "rdboy").Address;
        public static int FixedPosition { get; } = Tokenizer.SystemVariables.First(t => t.Name == "fixpos").Address;
        public static int Fertilized { get; } = Tokenizer.SystemVariables.First(t => t.Name == "fertilized").Address;
        public static int Reproduce { get; } = Tokenizer.SystemVariables.First(t => t.Name == "repro").Address;
        public static int MutatingReproduce { get; } = Tokenizer.SystemVariables.First(t => t.Name == "mrepro").Address;
        public static int SexualReproduce { get; } = Tokenizer.SystemVariables.First(t => t.Name == "sexrepro").Address;
        public static int MakeTie { get; } = Tokenizer.SystemVariables.First(t => t.Name == "mtie").Address;
    }
}