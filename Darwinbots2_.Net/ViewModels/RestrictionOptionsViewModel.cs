using AsyncAwaitBestPractices.MVVM;
using GalaSoft.MvvmLight;
using Iersera.DataModel;
using Microsoft.Win32;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Iersera.ViewModels
{
    internal class RestrictionOptionsViewModel : ViewModelBase
    {
        private RestrictionOptionsDialogState _dialogState;
        private bool _disableChloroplastsNonVeg;
        private bool _disableDnaNonVeg;
        private bool _disableDnaVeg;
        private bool _disableMotionNonVeg;
        private bool _disableMotionVeg;
        private bool _disableMutationsNonVeg;
        private bool _disableMutationsVeg;
        private bool _disableReproductionNonVeg;
        private bool _disableReproductionVeg;
        private bool _disableVisionNonVeg;
        private bool _disableVisionVeg;
        private bool _fixedInPlaceNonVeg;
        private bool _fixedInPlaceVeg;
        private bool _killNonMultibotNonVeg;
        private bool _killNonMultibotVeg;
        private bool _showNonVegetablePropertySettings;
        private bool _showNonVegetableSettings;
        private bool _showVegetablePropertySettings;
        private bool _showVegetableSettings;
        private string _title;
        private bool _virusImmuneNonVeg;
        private bool _virusImmuneVeg;
        private string speciesName;

        public RestrictionOptionsViewModel()
        {
            LoadPresetCommand = new AsyncCommand(LoadPreset, o => DialogState == RestrictionOptionsDialogState.ActiveSimulation);
            SavePresetCommand = new AsyncCommand(SavePreset, o => DialogState == RestrictionOptionsDialogState.ActiveSimulation);
        }

        public RestrictionOptionsDialogState DialogState
        {
            get => _dialogState;
            set
            {
                _dialogState = value;

                (LoadPresetCommand as AsyncCommand).RaiseCanExecuteChanged();
                (SavePresetCommand as AsyncCommand).RaiseCanExecuteChanged();

                UpdateDisplay();
            }
        }

        public bool DisableChloroplastsNonVeg { get => _disableChloroplastsNonVeg; set { _disableChloroplastsNonVeg = value; RaisePropertyChanged(); } }

        public bool DisableDnaNonVeg { get => _disableDnaNonVeg; set { _disableDnaNonVeg = value; RaisePropertyChanged(); } }

        public bool DisableDnaVeg { get => _disableDnaVeg; set { _disableDnaVeg = value; RaisePropertyChanged(); } }

        public bool DisableMotionNonVeg { get => _disableMotionNonVeg; set { _disableMotionNonVeg = value; RaisePropertyChanged(); } }

        public bool DisableMotionVeg { get => _disableMotionVeg; set { _disableMotionVeg = value; RaisePropertyChanged(); } }

        public bool DisableMutationsNonVeg { get => _disableMutationsNonVeg; set { _disableMutationsNonVeg = value; RaisePropertyChanged(); } }

        public bool DisableMutationsVeg { get => _disableMutationsVeg; set { _disableMutationsVeg = value; RaisePropertyChanged(); } }

        public bool DisableReproductionNonVeg { get => _disableReproductionNonVeg; set { _disableReproductionNonVeg = value; RaisePropertyChanged(); } }

        public bool DisableReproductionVeg { get => _disableReproductionVeg; set { _disableReproductionVeg = value; RaisePropertyChanged(); } }

        public bool DisableVisionNonVeg { get => _disableVisionNonVeg; set { _disableVisionNonVeg = value; RaisePropertyChanged(); } }

        public bool DisableVisionVeg { get => _disableVisionVeg; set { _disableVisionVeg = value; RaisePropertyChanged(); } }

        public bool FixedInPlaceNonVeg { get => _fixedInPlaceNonVeg; set { _fixedInPlaceNonVeg = value; RaisePropertyChanged(); } }

        public bool FixedInPlaceVeg { get => _fixedInPlaceVeg; set { _fixedInPlaceVeg = value; RaisePropertyChanged(); } }

        public bool KillNonMultibotNonVeg { get => _killNonMultibotNonVeg; set { _killNonMultibotNonVeg = value; RaisePropertyChanged(); } }

        public bool KillNonMultibotVeg { get => _killNonMultibotVeg; set { _killNonMultibotVeg = value; RaisePropertyChanged(); } }

        public ICommand LoadPresetCommand { get; }

        public ICommand SavePresetCommand { get; }

        public bool ShowNonVegetablePropertySettings { get => _showNonVegetablePropertySettings; set { _showNonVegetablePropertySettings = value; RaisePropertyChanged(); } }

        public bool ShowNonVegetableSettings { get => _showNonVegetableSettings; set { _showNonVegetableSettings = value; RaisePropertyChanged(); } }

        public bool ShowVegetablePropertySettings { get => _showVegetablePropertySettings; set { _showVegetablePropertySettings = value; RaisePropertyChanged(); } }

        public bool ShowVegetableSettings { get => _showVegetableSettings; set { _showVegetableSettings = value; RaisePropertyChanged(); } }

        public string SpeciesName
        {
            get => speciesName;
            set
            {
                speciesName = value;
                UpdateDisplay();
            }
        }

        public string Title { get => _title; set { _title = value; RaisePropertyChanged(); } }

        public bool VirusImmuneNonVeg { get => _virusImmuneNonVeg; set { _virusImmuneNonVeg = value; RaisePropertyChanged(); } }

        public bool VirusImmuneVeg { get => _virusImmuneVeg; set { _virusImmuneVeg = value; RaisePropertyChanged(); } }

        private async Task LoadPreset()
        {
            var dialog = new OpenFileDialog
            {
                InitialDirectory = Environment.CurrentDirectory,
                Filter = "Restrictions Preset File (*.resp)|*.resp|All Files (*.*)|*.*",
                CheckFileExists = true,
                Title = "Select Restrictions Preset File"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var fileContent = await File.ReadAllTextAsync(dialog.FileName);
                    var file = JsonSerializer.Deserialize<RestrictionsPresetFile>(fileContent);

                    DisableChloroplastsNonVeg = file.DisableChloroplastsNonVeg;
                    DisableDnaNonVeg = file.DisableDnaNonVeg;
                    DisableDnaVeg = file.DisableDnaVeg;
                    DisableMotionNonVeg = file.DisableMotionNonVeg;
                    DisableMotionVeg = file.DisableMotionVeg;
                    DisableMutationsNonVeg = file.DisableMutationsNonVeg;
                    DisableMutationsVeg = file.DisableMutationsVeg;
                    DisableReproductionNonVeg = file.DisableReproductionNonVeg;
                    DisableReproductionVeg = file.DisableReproductionVeg;
                    DisableVisionNonVeg = file.DisableVisionNonVeg;
                    DisableVisionVeg = file.DisableVisionVeg;
                    FixedInPlaceNonVeg = file.FixedInPlaceNonVeg;
                    FixedInPlaceVeg = file.FixedInPlaceVeg;
                    KillNonMultibotNonVeg = file.KillNonMultibotNonVeg;
                    KillNonMultibotVeg = file.KillNonMultibotVeg;
                    VirusImmuneNonVeg = file.VirusImmuneNonVeg;
                    VirusImmuneVeg = file.VirusImmuneVeg;
                }
                catch (IOException)
                {
                    MessageBox.Show("There was an error reading that file.", "Error Reading File", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (JsonException)
                {
                    MessageBox.Show("That was not a valid preset file.", "Error Reading File", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task SavePreset()
        {
            var dialog = new SaveFileDialog
            {
                InitialDirectory = Environment.CurrentDirectory,
                Filter = "Restrictions Preset File (*.resp)|*.resp|All Files (*.*)|*.*",
                ValidateNames = true,
                CheckPathExists = true,
                OverwritePrompt = true,
                Title = "Select Restrictions Preset File"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var file = new RestrictionsPresetFile
                    {
                        DisableChloroplastsNonVeg = DisableChloroplastsNonVeg,
                        DisableDnaNonVeg = DisableDnaNonVeg,
                        DisableDnaVeg = DisableDnaVeg,
                        DisableMotionNonVeg = DisableMotionNonVeg,
                        DisableMotionVeg = DisableMotionVeg,
                        DisableMutationsNonVeg = DisableMutationsNonVeg,
                        DisableMutationsVeg = DisableMutationsVeg,
                        DisableReproductionNonVeg = DisableReproductionNonVeg,
                        DisableReproductionVeg = DisableReproductionVeg,
                        DisableVisionNonVeg = DisableVisionNonVeg,
                        DisableVisionVeg = DisableVisionVeg,
                        FixedInPlaceNonVeg = FixedInPlaceNonVeg,
                        FixedInPlaceVeg = FixedInPlaceVeg,
                        KillNonMultibotNonVeg = KillNonMultibotNonVeg,
                        KillNonMultibotVeg = KillNonMultibotVeg,
                        VirusImmuneNonVeg = VirusImmuneNonVeg,
                        VirusImmuneVeg = VirusImmuneVeg
                    };

                    var fileContent = JsonSerializer.Serialize(file);

                    await File.WriteAllTextAsync(dialog.FileName, fileContent);
                }
                catch (IOException)
                {
                    MessageBox.Show("There was an error writing that file.", "Error Reading File", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UpdateDisplay()
        {
            ShowVegetableSettings = true;
            ShowVegetablePropertySettings = true;
            ShowNonVegetableSettings = true;
            ShowNonVegetablePropertySettings = true;

            switch (_dialogState)
            {
                case RestrictionOptionsDialogState.VegetableKillsOnly:
                    ShowNonVegetableSettings = false;
                    ShowVegetablePropertySettings = false;
                    Title = $"Restriction Options: {SpeciesName}";
                    break;

                case RestrictionOptionsDialogState.NonVegetableKillsOnly:
                    ShowVegetableSettings = false;
                    ShowNonVegetablePropertySettings = false;
                    Title = $"Restriction Options: {SpeciesName}";
                    break;

                case RestrictionOptionsDialogState.ActiveSimulation:
                    Title = $"Restriction Options: Active Simulation";
                    break;
            }
        }
    }
}
