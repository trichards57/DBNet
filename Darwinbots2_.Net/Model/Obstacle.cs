using System.Windows.Media;

namespace DarwinBots.Model
{
    internal class Obstacle
    {
        public Color color { get; set; }
        public bool exist { get; set; }
        public double Height { get; set; }
        public DoubleVector pos { get; set; }
        public DoubleVector vel { get; set; }
        public double Width { get; set; }
    }
}
