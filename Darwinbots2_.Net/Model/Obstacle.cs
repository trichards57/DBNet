using System.Windows.Media;

namespace DarwinBots.Model
{
    internal class Obstacle
    {
        public Color Color { get; set; }
        public bool Exist { get; set; }
        public double Height { get => Size.Y; set => Size = new DoubleVector(Size.X, value); }
        public DoubleVector Position { get; set; }
        public DoubleVector Size { get; set; }
        public DoubleVector Velocity { get; set; }
        public double Width { get => Size.X; set => Size = new DoubleVector(value, Size.Y); }
    }
}
