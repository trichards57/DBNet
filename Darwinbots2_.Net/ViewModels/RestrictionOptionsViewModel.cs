using AsyncAwaitBestPractices.MVVM;
using DarwinBots.DataModel;
using DarwinBots.Model;
using DarwinBots.Modules;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
using PostSharp.Patterns.Model;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DarwinBots.ViewModels
{
    [NotifyPropertyChanged]
    internal class RestrictionOptionsViewModel : ObservableObject
    {
        private RestrictionOptionsDialogState _dialogState;
        private string _speciesName;

        public RestrictionOptionsViewModel()
        {
            LoadPresetCommand = new AsyncCommand(LoadPreset, _ => DialogState == RestrictionOptionsDialogState.ActiveSimulation);
            SavePresetCommand = new AsyncCommand(SavePreset, _ => DialogState == RestrictionOptionsDialogState.ActiveSimulation);
        }

        public RestrictionOptionsDialogState DialogState
        {
            get => _dialogState;
            set
            {
                _dialogState = value;

                (LoadPresetCommand as AsyncCommand)?.RaiseCanExecuteChanged();
                (SavePresetCommand as AsyncCommand)?.RaiseCanExecuteChanged();

                UpdateDisplay();
            }
        }

        public bool DisableChloroplastsNonVeg { get; set; }
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
        public bool KillNonMultibotNonVeg { get; set; }
        public bool KillNonMultibotVeg { get; set; }
        public ICommand LoadPresetCommand { get; }
        public ICommand SavePresetCommand { get; }
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

        public void LoadFromSpecies(SpeciesViewModel species)
        {
            switch (DialogState)
            {
                case RestrictionOptionsDialogState.VegetableKillsOnly:
                    KillNonMultibotVeg = species.KillNonMultibot;
                    break;

                case RestrictionOptionsDialogState.NonVegetableKillsOnly:
                    KillNonMultibotNonVeg = species.KillNonMultibot;
                    break;

                case RestrictionOptionsDialogState.ActiveSimulation:
                    throw new NotImplementedException();
            }
        }

        public void SaveToAllRobs(IRobotManager robotManager)
        {
            foreach (var rob in robotManager.Robots.Where(r => r.Exists))
            {
                if (rob.IsVegetable)
                {
                    rob.MultibotTimer = KillNonMultibotVeg ? 210 : 0;
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
                    rob.MultibotTimer = KillNonMultibotNonVeg ? 210 : 0;
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

        public void SaveToSpecies(SpeciesViewModel species)
        {
            switch (DialogState)
            {
                case RestrictionOptionsDialogState.VegetableKillsOnly:
                    species.KillNonMultibot = KillNonMultibotVeg;
                    break;

                case RestrictionOptionsDialogState.NonVegetableKillsOnly:
                    species.KillNonMultibot = KillNonMultibotNonVeg;
                    break;

                case RestrictionOptionsDialogState.ActiveSimulation:
                    throw new NotImplementedException();
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
                    Title = "Restriction Options: Active Simulation";
                    break;
            }
        }
    }
}
