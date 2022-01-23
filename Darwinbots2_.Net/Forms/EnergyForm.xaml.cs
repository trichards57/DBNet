using DarwinBots.ViewModels;
using System.Windows;

namespace DarwinBots.Forms
{
    public partial class EnergyForm : IOptionsSubDialog
    {
        public EnergyForm()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        public EnergyViewModel ViewModel { get; } = new();

        public void LoadFromOptions(OptionsViewModel viewModel)
        {
            ViewModel.LoadFromOptions(viewModel);
        }

        public void SaveToOptions(OptionsViewModel viewModel)
        {
            ViewModel.SaveToOptions(viewModel);
        }

        private void Okay_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
