using System.Windows;
using System.Windows.Media;

namespace DarwinBots.Model.Display
{
    internal record TieUpdate
    {
        public Point StartPoint { get; init; }
        public Point EndPoint { get; init; }
        public double Width { get; init; }
        public Color Color { get; init; }
    }
}
