using Iersera.ViewModels;
using System.Windows;

namespace DBNet.Forms
{
    public partial class RestrictionOptionsForm : Window
    {
        public byte res_state = 0;

        public RestrictionOptionsForm()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        internal RestrictionOptionsViewModel ViewModel { get; } = new RestrictionOptionsViewModel();

        private void ApplyClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
