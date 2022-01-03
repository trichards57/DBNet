using System.Windows;

namespace DarwinBots.Services
{
    public interface IDialogService
    {
        void ShowInfoMessageBox(string title, string message);
    }

    public class DialogService : IDialogService
    {
        private readonly Window _owner;

        public DialogService(Window owner)
        {
            _owner = owner;
        }

        public void ShowInfoMessageBox(string title, string message)
        {
            MessageBox.Show(_owner, message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
