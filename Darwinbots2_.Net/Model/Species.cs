using DarwinBots.Support;

namespace DarwinBots.Model
{
    internal class Species
    {
        public Species()
        {
        }

        public Species(string fileName)
        {
            Name = System.IO.Path.GetFileName(fileName);
            Posrg = 1;
            Posdn = 1;
            Poslf = 0;
            Postp = 0;
            Path = System.IO.Path.GetDirectoryName(fileName);
            Veg = false;
            CantSee = false;
            DisableMovementSysvars = false;
            DisableDna = false;
            CantReproduce = false;
            VirusImmune = false;
            Quantity = 5;
            Stnrg = 3000;

            Color = System.Windows.Media.Color.FromRgb((byte)ThreadSafeRandom.Local.Next(0, 256),
                (byte)ThreadSafeRandom.Local.Next(0, 256), (byte)ThreadSafeRandom.Local.Next(0, 256));

            Mutables = new MutationProbabilities()
            {
                EnableMutations = true
            };
        }

        public bool CantReproduce { get; set; }

        public bool CantSee { get; set; }

        public System.Windows.Media.Color Color { get; init; }

        public string Comment { get; set; }

        public bool DisableDna { get; set; }

        public bool DisableMovementSysvars { get; set; }

        public bool Fixed { get; set; }

        public bool kill_mb { get; set; }

        public MutationProbabilities Mutables { get; init; }

        public string Name { get; set; }

        public bool Native { get; set; }

        public bool NoChlr { get; set; }

        public string Path { get; set; }

        public int Population { get; set; }

        public decimal Posdn { get; init; }

        public decimal Poslf { get; init; }

        public decimal Posrg { get; init; }

        public decimal Postp { get; init; }

        public int Quantity { get; set; }

        public int[] Skin { get; } = new int[8];

        public int Stnrg { get; set; }

        public int SubSpeciesCounter { get; set; }

        public bool Veg { get; set; }

        public bool VirusImmune { get; set; }

        public void AssignSkin()
        {
            for (var i = 0; i < 8; i += 2)
            {
                Skin[i] = ThreadSafeRandom.Local.Next(0, Robot.RobSize / 2 + 1);
                Skin[i + 1] = ThreadSafeRandom.Local.Next(0, 629);
            }
        }
    }
}
