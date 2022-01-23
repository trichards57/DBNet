using System.Windows;
using System.Windows.Media;

namespace DarwinBots.Model.Display
{
    internal record ShotUpdate
    {
        public Point Position { get; init; }
        public Color Color { get; init; }
    }
}
