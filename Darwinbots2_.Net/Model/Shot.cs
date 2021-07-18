using System.Collections.Generic;
using System.Windows.Media;

namespace DarwinBots.Model
{
    internal class Shot
    {
        public int Age { get; set; }
        public Color Color { get; set; }
        public List<DNABlock> Dna { get; set; } = new();
        public double Energy { get; set; }
        public bool Exist { get; set; }
        public bool Flash { get; set; }
        public string FromSpecie { get; init; }
        public int GeneNum { get; set; }
        public int MemoryLocation { get; init; }
        public int MemoryValue { get; set; }
        public DoubleVector OldPosition { get; set; }
        public robot Parent { get; init; }
        public DoubleVector Position { get; set; }
        public double Range { get; set; }
        public int ShotType { get; set; }
        public bool Stored { get; set; }
        public int Value { get; init; }
        public DoubleVector Velocity { get; set; }
    }
}
