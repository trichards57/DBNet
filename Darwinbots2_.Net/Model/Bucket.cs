using System.Collections.Generic;

namespace DarwinBots.Model
{
    internal class Bucket
    {
        public List<IntVector> AdjacentBuckets { get; } = new();
        public List<Robot> Robots { get; } = new();
    }
}
