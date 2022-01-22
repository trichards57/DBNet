using DarwinBots.ViewModels;
using System.Windows;

namespace DarwinBots.Forms
{
    public partial class PhysicsOptions : Window
    {
        public PhysicsOptions()
        {
            DataContext = ViewModel;

            InitializeComponent();
        }

        internal PhysicsOptionsViewModel ViewModel { get; } = new();

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
