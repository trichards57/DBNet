using System.Drawing;

namespace Iersera.Model
{
    public class Species
    {
        public bool CantReproduce = false;
        public bool CantSee = false;
        public int Colind = 0;
        public int color = 0;
        public string Comment = "";
        public bool DisableDNA = false;
        public bool DisableMovementSysvars = false;
        public Image DisplayImage = null;
        public bool dq_kill = false;
        public bool Fixed = false;
        public bool kill_mb = false;
        public string Leaguefilecomment = "";
        public MutationProbabilities Mutables = null;
        public string Name = "";
        public bool Native = false;
        public bool NoChlr = false;
        public string path = "";
        public int population = 0;
        public decimal Posdn = 0;
        public decimal Poslf = 0;
        public decimal Posrg = 0;
        public decimal Postp = 0;
        public int qty = 0;
        public int[] Skin = new int[13];
        public int Stnrg = 0;
        public int SubSpeciesCounter = 0;
        public bool Veg = false;
        public bool VirusImmune = false;
    }
}
