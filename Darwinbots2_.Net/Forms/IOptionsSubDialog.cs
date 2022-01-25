using DarwinBots.ViewModels;
using System.Windows;

namespace DarwinBots.Forms
{
    internal interface IOptionsSubDialog
    {
        Window Owner { get; set; }

        void LoadFromOptions(OptionsViewModel viewModel);

        void SaveToOptions(OptionsViewModel viewModel);

        bool? ShowDialog();
    }
}
