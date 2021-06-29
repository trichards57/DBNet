using DarwinBots.ViewModels;
using System.Windows;

namespace DarwinBots.Forms
{
    public partial class EnergyForm
    {
        public EnergyForm()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        public EnergyViewModel ViewModel { get; } = new();

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
