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
    }
}
