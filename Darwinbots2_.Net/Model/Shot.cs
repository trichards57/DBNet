using System.Collections.Generic;
using System.Windows.Media;

namespace DarwinBots.Model
{
    internal class Shot
    {
        public int age { get; set; }
        public Color color { get; set; }
        public List<DNABlock> dna { get; set; } = new();
        public bool exist { get; set; }
        public bool flash { get; set; }
        public string FromSpecie { get; set; }
        public bool fromveg { get; set; }
        public int genenum { get; set; }
        public int memloc { get; set; }
        public int Memval { get; set; }
        public double nrg { get; set; }
        public vector opos { get; set; }
        public robot parent { get; set; }
        public vector pos { get; set; }
        public double Range { get; set; }
        public int shottype { get; set; }
        public bool stored { get; set; }
        public int value { get; set; }
        public vector velocity { get; set; }
    }
}
