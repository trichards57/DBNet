using System.Collections.Generic;

namespace DarwinBots.Model
{
    internal class Bucket
    {
        public List<IntVector> AdjacentBuckets { get; set; } = new();
        public List<robot> Robots { get; set; } = new();
    }
}
