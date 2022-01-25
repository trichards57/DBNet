namespace DarwinBots.Model
{
    internal record DnaBlock
    {
        public DnaBlock() { }

        public DnaBlock(int type, int value)
        {
            Type = type;
            Value = value;
        }

        public int Type { get; init; }
        public int Value { get; init; }
    }
}
