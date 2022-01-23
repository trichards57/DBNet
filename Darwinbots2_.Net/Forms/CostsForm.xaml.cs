using DarwinBots.ViewModels;
using System.Windows;

namespace DarwinBots.Forms
{
    public partial class CostsForm : IOptionsSubDialog
    {
        // TODO : Set up some validation on the Okay button.

        public CostsForm()
        {
            DataContext = ViewModel;

            InitializeComponent();
        }

        internal CostsViewModel ViewModel { get; } = new();

        public void LoadFromOptions(OptionsViewModel viewModel)
        {
            ViewModel.LoadFromOptions(viewModel.Costs);
        }

        public void SaveToOptions(OptionsViewModel viewModel)
        {
            viewModel.Costs = ViewModel.SaveToOptions();
        }

        private void Okay_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
