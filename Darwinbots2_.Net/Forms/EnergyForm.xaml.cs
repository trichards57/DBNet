using DarwinBots.ViewModels;
using System.Windows;

namespace DBNet.Forms
{
    public partial class EnergyForm : Window
    {
        public EnergyForm()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        public EnergyViewModel ViewModel { get; } = new EnergyViewModel();

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
