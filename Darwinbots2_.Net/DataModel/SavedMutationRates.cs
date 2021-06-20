namespace DarwinBots.DataModel
{
    internal class SavedMutationRates
    {
        public int CopyErrorWhatToChange { get; internal set; }
        public double[] MutationMeans { get; internal set; }
        public double[] MutationProbabilities { get; internal set; }
        public double[] MutationStdDevs { get; internal set; }
        public int PointWhatToChange { get; internal set; }
    }
}
