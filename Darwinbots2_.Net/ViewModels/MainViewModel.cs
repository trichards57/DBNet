using AsyncAwaitBestPractices.MVVM;
using DarwinBots.Forms;
using DarwinBots.Model;
using DarwinBots.Modules;
using DarwinBots.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace DarwinBots.ViewModels
{
    internal class MainViewModel : ObservableObject
    {
        private readonly DialogService _dialogService;

        // TODO : Set up the add species combo box
        private MainEngine _engine;

        public MainViewModel(DialogService dialogService = null)
        {
            NewSimulationCommand = new AsyncCommand(NewSimulation);
            _dialogService = dialogService ?? new DialogService(Application.Current?.MainWindow);
        }

        public BitmapSource FieldRender { get; set; }

        public ICommand NewSimulationCommand { get; }

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        private void ImageAvailable(object sender, ImageAvailableArgs e)
        {
            var ip = e.Image.GetHbitmap();

            try
            {
                FieldRender = Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(ip);
            }
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

            _engine = new MainEngine();
            _engine.ImageAvailable += ImageAvailable;
            await _engine.StartSimulation();
        }
    }
}
