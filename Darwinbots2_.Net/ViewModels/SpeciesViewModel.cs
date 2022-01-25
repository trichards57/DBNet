using DarwinBots.Forms;
using DarwinBots.Model;
using DarwinBots.Services;
using DarwinBots.Support;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using PostSharp.Patterns.Model;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace DarwinBots.ViewModels
{
    [NotifyPropertyChanged(ExcludeExplicitProperties = true)]
    internal class SpeciesViewModel : ObservableObject
    {
        // TODO : Sort out initial position
        // TODO : Sort out displaying skin
        // TODO : Sort out colour picking

        private readonly DialogService _dialogService;
        private readonly Species _species;
        private bool _disableChloroplasts;
        private bool _enableRepopulation;
        private int _initialEnergy;

        public SpeciesViewModel(Species species, DialogService dialogService = null)
        {
            _species = species;
            _dialogService = dialogService ?? new DialogService(Application.Current?.MainWindow);

            DisplayFatalRestrictionsCommand = new RelayCommand(DisplayFatalRestrictions);
            DisplayMutationRatesCommand = new RelayCommand(DisplayMutationRates);
            ChangeSkinCommand = new RelayCommand(ChangeSkin);
            SetInitialEnergyCommand = new RelayCommand<string>(SetInitialEnergy);
            SetInitialIndividualsCommand = new RelayCommand<string>(SetInitialIndividuals);

            DisableChloroplasts = _species.NoChlr;
            DisableDna = _species.DisableDna;
            DisableMovement = species.DisableMovementSysvars;
            DisableMutations = !species.Mutables.EnableMutations;
            DisableReproduction = species.CantReproduce;
            DisableVision = species.CantSee;
            EnableRepopulation = species.Veg;
            InitialEnergy = species.Stnrg;
            InitialIndividuals = species.Quantity;
            IsFixedInPlace = species.Fixed;
            IsVirusImmune = species.VirusImmune;
            KillNonMultibot = species.kill_mb;
            Name = species.Name;
            Comments = species.Comment;

            for (var i = 0; i < species.Skin.Length; i++)
                Skin[i] = species.Skin[i];
        }

        public ICommand ChangeSkinCommand { get; }
        public Color Color { get; set; }
        public string Comments { get; set; }

        public bool DisableChloroplasts
        {
            get => _disableChloroplasts;
            set
            {
                if (SetProperty(ref _disableChloroplasts, value) && _disableChloroplasts)
                {
                    EnableRepopulation = false;
                }
            }
        }

        public bool DisableDna { get; set; }
        public bool DisableMovement { get; set; }
        public bool DisableMutations { get; set; }
        public bool DisableReproduction { get; set; }
        public bool DisableVision { get; set; }
        public ICommand DisplayFatalRestrictionsCommand { get; }
        public ICommand DisplayMutationRatesCommand { get; }

        public bool EnableRepopulation
        {
            get => _enableRepopulation;
            set
            {
                if (SetProperty(ref _enableRepopulation, value) && _enableRepopulation)
                {
                    DisableChloroplasts = false;
                }
            }
        }

        public int InitialEnergy
        {
            get => _initialEnergy;
            set => SetProperty(ref _initialEnergy, value % 32000);
        }

        public int InitialIndividuals { get; set; }
        public bool IsFixedInPlace { get; set; }
        public bool IsVeg => EnableRepopulation;
        public bool IsVirusImmune { get; set; }
        public bool KillNonMultibot { get; set; }
        public string Name { get; set; }
        public bool Native => _species.Native;
        public OptionsForm ParentForm { get; set; }
        public ICommand PickColourCommand { get; }
        public ICommand ResetPositionCommand { get; }
        public ICommand SetInitialEnergyCommand { get; }
        public ICommand SetInitialIndividualsCommand { get; }
        public ObservableCollection<int> Skin { get; set; } = new(new int[8]);
        public Species Species => _species;

        public SpeciesViewModel Duplicate()
        {
            var species = new Species
            {
                Posrg = _species.Posrg,
                Posdn = _species.Posdn,
                Poslf = _species.Poslf,
                Postp = _species.Postp,
                Veg = _species.Veg,
                Stnrg = _species.Stnrg,
                Quantity = _species.Quantity,
                Fixed = _species.Fixed,
                CantSee = _species.CantSee,
                DisableMovementSysvars = _species.DisableMovementSysvars,
                DisableDna = _species.DisableDna,
                CantReproduce = _species.CantReproduce,
                VirusImmune = _species.VirusImmune,
                Mutables = _species.Mutables,
                Native = _species.Native,
            };

            return new SpeciesViewModel(species);
        }

        public async Task LoadComment()
        {
            var path = Path.Combine(_species.Path, _species.Name);
            var lines = await File.ReadAllLinesAsync(path);

            var topComment = lines.Select(s => s.Trim()).TakeWhile(s => s.StartsWith("'") || s.StartsWith("/"));
            var actualComment = string.Join("\n", topComment.Select(s => s[1..].Trim())).Trim();

            _species.Comment = actualComment;
            Comments = _species.Comment.Trim();
        }

        public void Save()
        {
            _species.NoChlr = DisableChloroplasts;

            _species.DisableDna = DisableDna;
            _species.DisableMovementSysvars = DisableMovement;
            _species.Mutables.EnableMutations = !DisableMutations;
            _species.CantReproduce = DisableReproduction;
            _species.CantSee = DisableVision;
            _species.Veg = EnableRepopulation;
            _species.Stnrg = InitialEnergy;
            _species.Quantity = InitialIndividuals;
            _species.Fixed = IsFixedInPlace;
            _species.VirusImmune = IsVirusImmune;
            _species.kill_mb = KillNonMultibot;
            _species.Name = Name;

            if (DisableChloroplasts)
                _species.Veg = false;
        }

        private void ChangeSkin()
        {
            Skin[6] = (Skin[6] + ThreadSafeRandom.Local.Next(0, Robot.RobSize / 2 + 1)) * 2 / 3;
        }

        private void DisplayFatalRestrictions()
        {
            var dialog = new RestrictionOptionsForm()
            {
                Owner = ParentForm
            };
            dialog.ViewModel.DialogState = IsVeg ? RestrictionOptionsDialogState.VegetableKillsOnly : RestrictionOptionsDialogState.NonVegetableKillsOnly;
            dialog.ViewModel.LoadFromSpecies(this);
            var res = dialog.ShowDialog();

            if (res == true)
                dialog.ViewModel.SaveToSpecies(this);
        }

        private void DisplayMutationRates()
        {
            var dialog = new MutationsProbability()
            {
                Owner = ParentForm
            };
            dialog.ViewModel.LoadFromProbabilities(_species.Mutables);
            var res = dialog.ShowDialog();

            if (res == true)
                dialog.ViewModel.SaveToProbabilities(_species.Mutables);
        }

        private void SetInitialEnergy(string value)
        {
            InitialEnergy = int.Parse(value);
        }

        private void SetInitialIndividuals(string value)
        {
            InitialIndividuals = int.Parse(value);
        }
    }
}
