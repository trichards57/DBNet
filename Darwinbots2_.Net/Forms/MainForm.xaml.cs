using DarwinBots.ViewModels;
using System;

namespace DarwinBots.Forms
{
    public partial class MainForm
    {
        private readonly MainViewModel _viewModel = new();

        public MainForm()
        {
            InitializeComponent();

            DataContext = _viewModel;
            _viewModel.Dispatcher = Dispatcher;
        }

        protected override void OnClosed(EventArgs e)
        {
            _viewModel.StopSimulation();
            base.OnClosed(e);
        }
    }
}
