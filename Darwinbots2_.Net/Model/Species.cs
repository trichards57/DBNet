using System.Drawing;

namespace Iersera.Model
{
    public class Species
    {
        public bool CantReproduce { get; set; }
        public bool CantSee { get; set; }
        public int Colind { get; set; }
        public System.Windows.Media.Color color { get; set; }
        public string Comment { get; set; }
        public bool DisableDNA { get; set; }
        public bool DisableMovementSysvars { get; set; }
        public Image DisplayImage { get; set; }
        public bool dq_kill { get; set; }
        public bool Fixed { get; set; }
        public bool kill_mb { get; set; }
        public string Leaguefilecomment { get; set; }
        public MutationProbabilities Mutables { get; set; }
        public string Name { get; set; }
        public bool Native { get; set; }
        public bool NoChlr { get; set; }
        public string path { get; set; }
        public int population { get; set; }
        public decimal Posdn { get; set; }
        public decimal Poslf { get; set; }
        public decimal Posrg { get; set; }
        public int qty { get; set; }
        public int[] Skin { get; set; } = new int[13];
        public int Stnrg { get; set; }
        public int SubSpeciesCounter { get; set; }
        public bool Veg { get; set; }
        public bool VirusImmune { get; set; }
    }
}
