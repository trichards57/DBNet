using System.Collections.Generic;
using System.Windows;

namespace DarwinBots.Model.Display
{
    internal record DisplayUpdate
    {
        public Size FieldSize { get; init; }
        public IReadOnlyList<RobotUpdate> RobotUpdates { get; init; }
    }
}
