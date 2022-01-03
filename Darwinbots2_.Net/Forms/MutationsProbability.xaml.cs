using DarwinBots.ViewModels;

namespace DarwinBots.Forms
{
    public partial class MutationsProbability
    {
        public MutationsProbability()
        {
            InitializeComponent();

            DataContext = ViewModel;
        }

        public MutationsProbabilitiesViewModel ViewModel { get; } = new();

        private void OkayClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
