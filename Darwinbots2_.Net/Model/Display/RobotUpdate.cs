using System.Windows;
using System.Windows.Media;

namespace DarwinBots.Model.Display
{
    internal record RobotUpdate
    {
        public Point Position { get; set; }
        public double Radius { get; set; }
        public Color Color { get; set; }
    }
}
