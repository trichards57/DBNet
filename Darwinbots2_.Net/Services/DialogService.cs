using DarwinBots.Forms;
using DarwinBots.ViewModels;
using System.Windows;

namespace DarwinBots.Services
{
    public interface IDialogService
    {
        void ShowInfoMessageBox(string title, string message);

        void ShowOptionsSubDialog<TForm>(OptionsViewModel optionsViewModel, Window parentForm = null)
           where TForm : IOptionsSubDialog, new();
    }

    public class DialogService : IDialogService
    {
        public DialogService(Window owner)
        {
            Owner = owner;
        }

        public Window Owner { get; }

        public void ShowInfoMessageBox(string title, string message)
        {
            MessageBox.Show(Owner, message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowOptionsSubDialog<TForm>(OptionsViewModel optionsViewModel, Window parentForm = null)
            where TForm : IOptionsSubDialog, new()
        {
            var form = new TForm
            {
                Owner = parentForm ?? Owner
            };

            form.LoadFromOptions(optionsViewModel);

            var res = form.ShowDialog();

            if (res == true)
                form.SaveToOptions(optionsViewModel);
        }
    }
}
