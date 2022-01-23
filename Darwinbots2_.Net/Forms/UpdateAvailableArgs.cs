using DarwinBots.Model.Display;
using System;

namespace DarwinBots.Forms
{
    internal class UpdateAvailableArgs : EventArgs
    {
        public DisplayUpdate Update { get; init; }
    }
}
