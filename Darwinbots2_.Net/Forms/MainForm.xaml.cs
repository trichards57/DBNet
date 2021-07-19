using DarwinBots.ViewModels;

namespace DarwinBots.Forms
{
    public partial class MainForm
    {
        private readonly MainViewModel _viewModel = new();

        public MainForm()
        {
            InitializeComponent();

            DataContext = _viewModel;
        }
    }
}
