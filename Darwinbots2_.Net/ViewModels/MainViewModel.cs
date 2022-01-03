using System.Threading.Tasks;
using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;
using DarwinBots.Forms;
using DarwinBots.Model;
using DarwinBots.Modules;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace DarwinBots.ViewModels
{
    internal class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            NewSimulationCommand = new AsyncCommand(NewSimulation);
        }

        public ICommand NewSimulationCommand { get; }

        private async Task NewSimulation()
        {
            SimOpt.SimOpts ??= new SimOptions();

            var optionsForm = new OptionsForm();
            await optionsForm.ViewModel.LoadFromOptions(SimOpt.SimOpts);

            var result = optionsForm.ShowDialog();
        }
    }
}
