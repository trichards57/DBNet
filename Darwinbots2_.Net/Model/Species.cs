using DarwinBots.Modules;
using DarwinBots.Support;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace DarwinBots.Model
{
    public class Species
    {
        public Species()
        {
        }

        public Species(string fileName)
        {
            Name = Path.GetFileName(fileName);
            Colind = 8;
            Posrg = 1;
            Posdn = 1;
            Poslf = 0;
            Postp = 0;
            path = Path.GetDirectoryName(fileName);
            Veg = false;
            CantSee = false;
            DisableMovementSysvars = false;
            DisableDNA = false;
            CantReproduce = false;
            VirusImmune = false;

            var nameLower = Name.ToLowerInvariant();

            color = nameLower switch
            {
                "base.txt" => System.Windows.Media.Colors.Blue,
                "mutate.txt" => System.Windows.Media.Colors.Red,
                "robota.txt" => System.Windows.Media.Colors.Red,
                "robotb.txt" => System.Windows.Media.Colors.Red,
                "test.txt" => System.Windows.Media.Colors.Red,
                _ => System.Windows.Media.Color.FromRgb((byte)ThreadSafeRandom.Local.Next(0, 256), (byte)ThreadSafeRandom.Local.Next(0, 256), (byte)ThreadSafeRandom.Local.Next(0, 256)),
            };
        }

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

        public decimal Postp { get; set; }

        public int qty { get; set; }

        public int[] Skin { get; set; } = new int[13];

        public int Stnrg { get; set; }

        public int SubSpeciesCounter { get; set; }

        public bool Veg { get; set; }

        public bool VirusImmune { get; set; }

        public void AssignSkin()
        {
            Skin = new int[8];

            for (var i = 0; i < 8; i += 2)
            {
                Skin[i] = ThreadSafeRandom.Local.Next(0, Robots.half + 1);
                Skin[i + 1] = ThreadSafeRandom.Local.Next(0, 629);
            }
        }

        public async Task ResetMutationsToDefault(bool skipNorm = false)
        {
            var len = 0;

            if (Globals.NormMut && !skipNorm)
            {
                var robot = new robot();

                var pth = Path.Combine(path, Name);

                if (!File.Exists(pth))
                    pth = Path.Combine("Robots", Name);

                if (await DNATokenizing.LoadDNA(pth, robot))
                    len = DNAManipulations.DnaLen(robot.dna);
            }

            for (var a = 0; a < 20; a++)
            {
                Mutables.mutarray[a] = Globals.NormMut && !skipNorm ? len * Globals.valNormMut : 5000;
                Mutables.Mean[a] = 1;
                Mutables.StdDev[a] = 0;
            }

            if (skipNorm)
                Mutables.mutarray[NeoMutations.P2UP] = 0; //Botsareus 2/21/2014 Might as well disable p2 mutations if loading from the net

            NeoMutations.SetDefaultLengths(Mutables);
        }
    }
}
