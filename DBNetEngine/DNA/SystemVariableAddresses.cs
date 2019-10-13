using System.Linq;

namespace DBNetEngine.DNA
{
    public static class SystemVariableAddresses
    {
        public static int Aim { get; } = GetAddress("aim");

        public static int AimLeft { get; } = GetAddress("aimleft");

        public static int AimRight { get; } = GetAddress("aimright");

        public static int Body { get; } = GetAddress("body");

        public static int Chloroplasts { get; } = GetAddress("chlr");

        public static int DeleteGene { get; } = GetAddress("delgene");

        public static int DirDown { get; } = GetAddress("dirdn");

        public static int DirDx { get; } = GetAddress("dirdx");

        public static int DirSx { get; } = GetAddress("dirsx");

        public static int DirUp { get; } = GetAddress("dirup");

        public static int DNALength { get; } = GetAddress("dnalen");

        public static int Energy { get; } = GetAddress("nrg");

        public static int EyeEnd { get; } = GetAddress("eye9");

        public static int EyeStart { get; } = GetAddress("eye1");

        public static int FeedBody { get; } = GetAddress("fdbody");

        public static int Fertilized { get; } = GetAddress("fertilized");

        public static int FixedAngle { get; } = GetAddress("fixang");

        public static int FixedPosition { get; } = GetAddress("fixpos");

        public static int Genes { get; } = GetAddress("genes");

        public static int Light { get; } = GetAddress("light");

        public static int MakeChloroplasts { get; } = GetAddress("mkchlr");

        public static int MakePoison { get; } = GetAddress("mkpoison");

        public static int MakeShell { get; } = GetAddress("mkshell");

        public static int MakeSlime { get; } = GetAddress("mkslime");

        public static int MakeTie { get; } = GetAddress("mtie");

        public static int MakeVenom { get; } = GetAddress("mkvenom");

        public static int MakeVirus { get; } = GetAddress("mkvirus");

        public static int Mass { get; } = GetAddress("mass");

        public static int MaxVelocity { get; } = GetAddress("maxvel");

        public static int MutatingReproduce { get; } = GetAddress("mrepro");

        public static int PermamentWaste { get; } = GetAddress("pwaste");

        public static int Poison { get; } = GetAddress("poison");

        public static int Poisoned { get; } = GetAddress("paralyzed");

        public static int ReadBouyancy { get; } = GetAddress("rdboy");

        public static int RemoveChloroplasts { get; } = GetAddress("rmchlr");

        public static int Reproduce { get; } = GetAddress("repro");

        public static int RobotAge { get; } = GetAddress("robage");

        public static int SetAim { get; } = GetAddress("setaim");

        public static int SetBouyancy { get; } = GetAddress("setboy");

        public static int SexualReproduce { get; } = GetAddress("sexrepro");

        public static int ShareChloroplasts { get; } = GetAddress("sharechlr");

        public static int ShareEnergy { get; } = GetAddress("shareenergy");

        public static int ShareShell { get; } = GetAddress("shareshell");

        public static int ShareSlime { get; } = GetAddress("shareslime");

        public static int ShareWaste { get; } = GetAddress("sharewaste");

        public static int Shell { get; } = GetAddress("shell");

        public static int Shoot { get; } = GetAddress("shoot");

        public static int ShootValue { get; } = GetAddress("shootval");

        public static int Slime { get; } = GetAddress("slime");

        public static int StoreBody { get; } = GetAddress("strbody");

        public static int StorePoison { get; } = GetAddress("strpoison");

        public static int StoreVenom { get; } = GetAddress("strvenom");

        public static int Timer { get; } = GetAddress("timer");

        public static int VelocityDown { get; } = GetAddress("veldn");

        public static int VelocityLeft { get; } = GetAddress("velsx");

        public static int VelocityRight { get; } = GetAddress("veldx");

        public static int VelocityScalar { get; } = GetAddress("velscalar");

        public static int VelocityUp { get; } = GetAddress("velup");

        public static int Venom { get; } = GetAddress("venom");

        public static int VirusShoot { get; } = GetAddress("vshoot");

        public static int VirusTimer { get; } = GetAddress("vtimer");

        public static int Waste { get; } = GetAddress("waste");

        public static int GetAddress(string name)
        {
            return Tokenizer.SystemVariables.First(t => t.Name == name).Address;
        }
    }
}