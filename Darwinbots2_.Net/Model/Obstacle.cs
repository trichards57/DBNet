using System.Windows.Media;

namespace DarwinBots.Model
{
    internal class Obstacle
    {
        public Color Color { get; set; }
        public bool Exist { get; set; }
        public double Height { get; set; }
        public DoubleVector Position { get; set; }
        public DoubleVector Velocity { get; set; }
        public double Width { get; set; }
    }
}
