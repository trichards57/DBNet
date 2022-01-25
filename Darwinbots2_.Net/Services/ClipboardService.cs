using System.Windows;

namespace DarwinBots.Services
{
    public interface IClipboardService
    {
        void CopyCsvToClipboard(string value);
    }

    public class ClipboardService : IClipboardService
    {
        public void CopyCsvToClipboard(string value)
        {
            Clipboard.SetText(value, TextDataFormat.CommaSeparatedValue);
        }
    }
}
