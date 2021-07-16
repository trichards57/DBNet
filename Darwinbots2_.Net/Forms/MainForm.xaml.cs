using DarwinBots.ViewModels;
using System.Windows;

namespace DarwinBots.Forms
{
    public partial class MainForm : Window
    {
        private readonly MainViewModel _viewModel = new MainViewModel();

        public MainForm()
        {
            InitializeComponent();

            DataContext = _viewModel;
        }
    }
}
