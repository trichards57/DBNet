using System;
using System.Drawing;

namespace DarwinBots.Forms
{
    internal class ImageAvailableArgs : EventArgs
    {
        public Bitmap Image { get; set; }
    }
}
