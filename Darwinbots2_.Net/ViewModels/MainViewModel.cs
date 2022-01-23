using AsyncAwaitBestPractices.MVVM;
using DarwinBots.Forms;
using DarwinBots.Model;
using DarwinBots.Modules;
using DarwinBots.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DarwinBots.ViewModels
{
    internal class MainViewModel : ObservableObject
    {
        private readonly DialogService _dialogService;

        public MainViewModel(DialogService dialogService = null)
        {
            NewSimulationCommand = new AsyncCommand(NewSimulation);
            _dialogService = dialogService ?? new DialogService(Application.Current?.MainWindow);
        }

        public ICommand NewSimulationCommand { get; }

        private async Task NewSimulation()
        {
            SimOpt.SimOpts ??= new SimOptions();

            var optionsForm = new OptionsForm()
            {
                Owner = _dialogService.Owner
            };
            await optionsForm.ViewModel.LoadFromOptions(SimOpt.SimOpts);

            var result = optionsForm.ShowDialog();
        }
    }
}
