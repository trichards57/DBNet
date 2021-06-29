using DarwinBots.ViewModels;
using System.Windows;

namespace DarwinBots.Forms
{
    public partial class CostsForm
    {
        // TODO : Set up some validation on the Okay button.

        public CostsForm()
        {
            DataContext = ViewModel;

            InitializeComponent();
        }

        internal CostsViewModel ViewModel { get; } = new();

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Okay_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
