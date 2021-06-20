using System.Windows.Media;

namespace DarwinBots.Modules
{
    internal static class stuffcolors
    {
        public static int backgcolor { get; set; }

        public static int chartcolor { get; set; }

        public static Color HslToColor(double h, double s, double l)
        {
            var hsltorgb = HueToColor(h);
            var c = Color.FromRgb(127, 127, 127);
            hsltorgb = MixColor(hsltorgb, c, 1 - (s / 240));

            if (l < 120)
                hsltorgb = MixColor(Color.FromRgb(0, 0, 0), hsltorgb, l / 120);
            else
                hsltorgb = MixColor(hsltorgb, Color.FromRgb(255, 255, 255), (l - 120) / 120);

            return hsltorgb;
        }

        private static Color HueToColor(double h)
        {
            Color huetorgb;

            var Delta = (byte)((int)h % 40);
            h -= Delta;
            Delta = (byte)(255 / 40 * Delta);
            if (h < 240)
                huetorgb = Color.FromRgb(255, 0, (byte)(255 - Delta));

            if (h < 200)
                huetorgb = Color.FromRgb(Delta, 0, 255);

            if (h < 160)
                huetorgb = Color.FromRgb(0, (byte)(255 - Delta), 255);

            if (h < 120)
                huetorgb = Color.FromRgb(0, 255, Delta);

            if (h < 80)
                huetorgb = Color.FromRgb((byte)(255 - Delta), 255, 0);

            if (h < 40)
                huetorgb = Color.FromRgb(255, Delta, 0);

            return huetorgb;
        }

        private static Color MixColor(Color c1, Color c2, dynamic factor)
        {
            return Color.FromRgb(c1.R * (1 - factor) + c2.R * factor, c1.G * (1 - factor) + c2.G * factor, c1.B * (1 - factor) + c2.B * factor);
        }
    }
}
