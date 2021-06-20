using System;

namespace DarwinBots.Model
{
    [Flags]
    internal enum Direction
    {
        North = 1,
        East = 2,
        South = 4,
        West = 8,
        NorthEast = North | East,
        SouthEast = South | East,
        SouthWest = South | West,
        NorthWest = North | West
    }
}
