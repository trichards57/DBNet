using DarwinBots.ViewModels;
using System.Windows;

namespace DarwinBots.Forms
{
    public partial class OptionsForm
    {
        public OptionsForm()
        {
            InitializeComponent();

            DataContext = ViewModel;
            ViewModel.ParentForm = this;
        }

        internal OptionsViewModel ViewModel { get; } = new();

        private void StartNew(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
