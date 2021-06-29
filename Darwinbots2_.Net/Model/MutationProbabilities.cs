using DarwinBots.Modules;

namespace DarwinBots.Model
{
    public class MutationProbabilities
    {
        public int CopyErrorWhatToChange { get; set; }
        public double[] Mean { get; set; } = new double[20];
        public double[] mutarray { get; set; } = new double[20];
        public bool Mutations { get; set; }
        public int PointWhatToChange { get; set; }
        public double[] StdDev { get; set; } = new double[20];

        public void ResetToDefault()
        {
            var len = 0;

            for (var a = 0; a < 20; a++)
            {
                mutarray[a] = Globals.NormMut ? len * Globals.valNormMut : 5000;
                Mean[a] = 1;
                StdDev[a] = 0;
            }

            NeoMutations.SetDefaultLengths(this);
        }
    }
}
