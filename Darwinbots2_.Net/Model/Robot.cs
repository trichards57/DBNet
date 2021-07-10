using System.Collections.Generic;
using System.Windows.Media;

namespace DarwinBots.Model
{
    internal class robot
    {
        public int AbsNum { get; set; }
        public DoubleVector actvel { get; set; }
        public double AddedMass { get; set; }
        public int age { get; set; }
        public double aim { get; set; }
        public DoubleVector aimvector { get; set; }
        public int BirthCycle { get; set; }
        public double body { get; set; }
        public double Bouyancy { get; set; }
        public IntVector BucketPos { get; set; }
        public bool CantReproduce { get; set; }
        public bool CantSee { get; set; }
        public double chloroplasts { get; set; }
        public byte Chlr_Share_Delay { get; set; }
        public Color color { get; set; }
        public int condnum { get; set; }
        public bool Corpse { get; set; }
        public bool Dead { get; set; }
        public int DecayTimer { get; set; }
        public bool DisableDNA { get; set; }
        public bool DisableMovementSysvars { get; set; }
        public List<DNABlock> dna { get; set; } = new();
        public int[] epimem { get; set; } = new int[14];
        public bool exist { get; set; }
        public int fertilized { get; set; }
        public bool Fixed { get; set; }
        public string FName { get; set; }
        public int genenum { get; set; }
        public int generation { get; set; }
        public decimal GenMut { get; set; }
        public bool highlight { get; set; }
        public DoubleVector ImpulseInd { get; set; }
        public DoubleVector ImpulseRes { get; set; }
        public double ImpulseStatic { get; set; }
        public int Kills { get; set; }
        public int lastdown { get; set; }
        public int lastleft { get; set; }
        public int LastMut { get; set; }
        public string LastMutDetail { get; set; }
        public object lastopp { get; set; }
        public DoubleVector lastopppos { get; set; }
        public string LastOwner { get; set; }
        public int lastright { get; set; }
        public robot lasttch { get; set; }
        public int lastup { get; set; }
        public double ma { get; set; }
        public double mass { get; set; }
        public int[] mem { get; set; } = new int[1000];
        public decimal mt { get; set; }
        public bool Multibot { get; set; }
        public int multibot_time { get; set; }
        public MutationProbabilities Mutables { get; set; }
        public int Mutations { get; set; }
        public double MutEpiReset { get; set; }
        public int newage { get; set; }
        public bool NewMove { get; set; }
        public bool NoChlr { get; set; }
        public double nrg { get; set; }
        public decimal oaim { get; set; }
        public double obody { get; set; }
        public int[] occurr { get; set; } = new int[20];
        public int oldBotNum { get; set; }
        public float OldGD { get; set; }
        public int OldMutations { get; set; }
        public double onrg { get; set; }
        public DoubleVector opos { get; set; }
        public int order { get; set; }
        public int[] OSkin { get; set; } = new int[13];
        public double Paracount { get; set; }
        public bool Paralyzed { get; set; }
        public robot parent { get; set; }
        public int Ploc { get; set; }
        public int Point2MutCycle { get; set; }
        public int PointMutBP { get; set; }
        public int PointMutCycle { get; set; }
        public double poison { get; set; }
        public double Poisoncount { get; set; }
        public bool Poisoned { get; set; }
        public DoubleVector pos { get; set; }
        public int Pval { get; set; }
        public double Pwaste { get; set; }
        public double radius { get; set; }
        public int reproTimer { get; set; }
        public double shell { get; set; }
        public int sim { get; set; }
        public int[] Skin { get; set; } = new int[13];
        public double Slime { get; set; }
        public int SonNumber { get; set; }
        public List<DNABlock> spermDNA { get; set; } = new();
        public int SubSpecies { get; set; }
        public string tag { get; set; }
        public bool[] TieAngOverwrite { get; set; } = new bool[3];
        public bool[] TieLenOverwrite { get; set; } = new bool[3];
        public List<Tie> Ties { get; set; } = new();
        public List<Variable> vars { get; set; } = new();
        public double vbody { get; set; }
        public bool Veg { get; set; }
        public DoubleVector vel { get; set; }
        public double venom { get; set; }
        public bool View { get; set; }
        public bool VirusImmune { get; set; }
        public Shot virusshot { get; set; }
        public int Vloc { get; set; }
        public int Vtimer { get; set; }
        public int Vval { get; set; }
        public bool wall { get; set; }
        public double Waste { get; set; }
    }
}
