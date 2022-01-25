using DarwinBots.ViewModels;

namespace DarwinBots.Forms
{
    internal partial class MutationsProbability
    {
        public MutationsProbability()
        {
            InitializeComponent();

            DataContext = ViewModel;
        }

        public MutationsProbabilitiesViewModel ViewModel { get; } = new();

        private void Okay_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
