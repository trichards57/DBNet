using System.Collections.Generic;
using System.Windows;

namespace DarwinBots.Model.Display
{
    internal record DisplayUpdate
    {
        public Size FieldSize { get; init; }
        public IReadOnlyList<RobotUpdate> RobotUpdates { get; init; }
        public IReadOnlyList<TieUpdate> TieUpdates { get; init; }
        public IReadOnlyList<ShotUpdate> ShotUpdates { get; internal set; }
    }
}
