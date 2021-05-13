using Iersera.Model;

namespace Iersera.DataModel
{
    internal class SavedShot
    {
        public int Age { get; internal set; }
        public int Color { get; internal set; }
        public DNAExecution.block[] Dna { get; internal set; }
        public decimal Energy { get; internal set; }
        public bool Exists { get; internal set; }
        public string FromSpecies { get; internal set; }
        public bool FromVeg { get; internal set; }
        public int GeneNumber { get; internal set; }
        public int MemoryLocation { get; internal set; }
        public int MemoryValue { get; internal set; }
        public vector OPosition { get; internal set; }
        public int Parent { get; internal set; }
        public vector Position { get; internal set; }
        public decimal Range { get; internal set; }
        public int ShotType { get; internal set; }
        public bool Stored { get; internal set; }
        public int Value { get; internal set; }
        public vector Velocity { get; internal set; }
    }
}
