using DarwinBots.ViewModels;
using System.Windows;

namespace DarwinBots.Forms
{
    public partial class OptionsForm
    {
        public OptionsForm()
        {
            InitializeComponent();

            DataContext = ViewModel;
            ViewModel.ParentForm = this;
        }

        internal OptionsViewModel ViewModel { get; } = new();

        private void LightBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ViewModel.StartLightTimer();
        }

        // TODO : Set up the gradient display
        private void LightBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ViewModel.StopLightTimer();
        }

        private void Pondcheck_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SetPondMode(!ViewModel.EnablePondMode);
        }

        private void StartNew(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
