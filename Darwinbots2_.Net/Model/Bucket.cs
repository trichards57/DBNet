using System.Collections.Generic;

namespace DarwinBots.Model
{
    internal class Bucket
    {
        public List<Bucket> Adjacent { get; } = new();
        public List<Robot> Robots { get; } = new();
    }
}
