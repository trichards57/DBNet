using DarwinBots.ViewModels;
using System.Windows;

namespace DarwinBots.Forms
{
    public partial class RestrictionOptionsForm
    {
        // TODO : Implement validation on apply.

        public RestrictionOptionsForm()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        internal RestrictionOptionsViewModel ViewModel { get; } = new();

        private void ApplyClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
