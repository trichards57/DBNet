using DarwinBots.DataModel;
using DarwinBots.Model;
using DarwinBots.Modules;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using PostSharp.Patterns.Model;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace DarwinBots.ViewModels
{
    [NotifyPropertyChanged]
    internal class RestrictionOptionsViewModel : ObservableObject
    {
        private RestrictionOptionsDialogState _dialogState;
        private string _speciesName;

        public RestrictionOptionsViewModel()
        {
            LoadPresetCommand = new AsyncRelayCommand(LoadPreset, () => DialogState == RestrictionOptionsDialogState.ActiveSimulation);
            SavePresetCommand = new AsyncRelayCommand(SavePreset, () => DialogState == RestrictionOptionsDialogState.ActiveSimulation);
        }

        public RestrictionOptionsDialogState DialogState
        {
            get => _dialogState;
            set
            {
                _dialogState = value;

                LoadPresetCommand.NotifyCanExecuteChanged();
                SavePresetCommand.NotifyCanExecuteChanged();

                UpdateDisplay();
            }
        }

        public bool DisableDnaNonVeg { get; set; }
        public bool DisableDnaVeg { get; set; }
        public bool DisableMotionNonVeg { get; set; }
        public bool DisableMotionVeg { get; set; }
        public bool DisableMutationsNonVeg { get; set; }
        public bool DisableMutationsVeg { get; set; }
        public bool DisableReproductionNonVeg { get; set; }
        public bool DisableReproductionVeg { get; set; }
        public bool DisableVisionNonVeg { get; set; }
        public bool DisableVisionVeg { get; set; }
        public bool FixedInPlaceNonVeg { get; set; }
        public bool FixedInPlaceVeg { get; set; }
        public IRelayCommand LoadPresetCommand { get; }
        public IRelayCommand SavePresetCommand { get; }
        public bool ShowNonVegetablePropertySettings { get; set; }
        public bool ShowNonVegetableSettings { get; set; }
        public bool ShowVegetablePropertySettings { get; set; }
        public bool ShowVegetableSettings { get; set; }

        public string SpeciesName
        {
            get => _speciesName;
            set
            {
                _speciesName = value;
                UpdateDisplay();
            }
        }

        public string Title { get; set; }
        public bool VirusImmuneNonVeg { get; set; }
        public bool VirusImmuneVeg { get; set; }

        public void SaveToAllRobs(IRobotManager robotManager)
        {
            foreach (var rob in robotManager.Robots.Where(r => r.Exists))
            {
                if (rob.IsVegetable)
                {
                    rob.IsFixed = FixedInPlaceVeg;

                    if (rob.IsFixed)
                    {
                        rob.Memory[216] = 1;
                        rob.Velocity = new DoubleVector(0, 0);
                    }

                    rob.CantSee = DisableVisionVeg;
                    rob.DnaDisabled = DisableDnaVeg;
                    rob.CantReproduce = DisableReproductionVeg;
                    rob.IsVirusImmune = VirusImmuneVeg;
                    rob.MutationProbabilities.EnableMutations = !DisableMutationsVeg;
                    rob.MovementSysvarsDisabled = DisableMotionVeg;
                }
                else
                {
                    rob.IsFixed = FixedInPlaceNonVeg;

                    if (rob.IsFixed)
                    {
                        rob.Memory[216] = 1;
                        rob.Velocity = new DoubleVector(0, 0);
                    }

                    rob.CantSee = DisableVisionNonVeg;
                    rob.DnaDisabled = DisableDnaNonVeg;
                    rob.CantReproduce = DisableReproductionNonVeg;
                    rob.IsVirusImmune = VirusImmuneNonVeg;
                    rob.MutationProbabilities.EnableMutations = !DisableMutationsNonVeg;
                    rob.MovementSysvarsDisabled = DisableMotionNonVeg;
                }
            }
        }

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

                    if (file == null)
                        return;

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
                    Title = "Restriction Options: Active Simulation";
                    break;
            }
        }
    }
}
