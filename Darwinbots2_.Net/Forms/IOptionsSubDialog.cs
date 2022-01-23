using DarwinBots.ViewModels;
using System.Windows;

namespace DarwinBots.Forms
{
    public interface IOptionsSubDialog
    {
        Window Owner { get; set; }

        void LoadFromOptions(OptionsViewModel viewModel);

        void SaveToOptions(OptionsViewModel viewModel);

        bool? ShowDialog();
    }
}
