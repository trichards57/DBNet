using DarwinBots.ViewModels;
using System.Windows;

namespace DarwinBots.Forms
{
    internal partial class PhysicsOptions : IOptionsSubDialog
    {
        public PhysicsOptions()
        {
            DataContext = ViewModel;

            InitializeComponent();
        }

        internal PhysicsOptionsViewModel ViewModel { get; } = new();

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
