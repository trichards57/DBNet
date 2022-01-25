using DarwinBots.Forms;
using DarwinBots.Model;
using DarwinBots.Modules;
using DarwinBots.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace DarwinBots.ViewModels
{
    internal class MainViewModel : ObservableObject
    {
        private readonly DialogService _dialogService;

        // TODO : Set up the add species combo box
        private MainEngine _engine;

        public MainViewModel(DialogService dialogService = null)
        {
            NewSimulationCommand = new AsyncRelayCommand(NewSimulation);
            _dialogService = dialogService ?? new DialogService(Application.Current?.MainWindow);
        }

        public event EventHandler<UpdateAvailableArgs> UpdateAvailable;

        public Dispatcher Dispatcher { get; internal set; }

        public IRelayCommand NewSimulationCommand { get; }

        public void StopSimulation()
        {
            _engine?.Stop();
        }

        private async Task NewSimulation()
        {
            SimOpt.SimOpts ??= new SimOptions();

            var optionsForm = new OptionsForm()
            {
                Owner = _dialogService.Owner
            };
            await optionsForm.ViewModel.LoadFromOptions(SimOpt.SimOpts);

            var result = optionsForm.ShowDialog();

            if (result != true)
                return;

            optionsForm.ViewModel.SaveToOptions(SimOpt.SimOpts);

            SimOpt.SimOpts.TotRunCycle = -1;

            foreach (var s in SimOpt.SimOpts.Specie)
            {
                s.SubSpeciesCounter = 0;
                s.Native = true;
            }

            if (_engine != null)
                _engine.UpdateAvailable -= OnUpdateAvailable;

            _engine = new MainEngine();
            _engine.UpdateAvailable += OnUpdateAvailable;

            _engine.StartSimulation();
        }

        private void OnUpdateAvailable(object sender, UpdateAvailableArgs e)
        {
            UpdateAvailable?.Invoke(sender, e);
        }
    }
}
