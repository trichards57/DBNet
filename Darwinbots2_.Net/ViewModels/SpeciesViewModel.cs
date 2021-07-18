using DarwinBots.Forms;
using DarwinBots.Model;
using DarwinBots.Modules;
using DarwinBots.Support;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PostSharp.Patterns.Model;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace DarwinBots.ViewModels
{
    [NotifyPropertyChanged]
    internal class SpeciesViewModel : ViewModelBase
    {
        // TODO : Sort out initial position
        // TODO : Sort out displaying skin
        // TODO : Sort out displaying mutation rates dialog
        // TODO : Sort out colour picking

        private readonly Species _species;
        private bool _disableChloroplasts;
        private bool enableRepopulation;
        private int initialEnergy;

        public SpeciesViewModel(Species species)
        {
            _species = species;

            DisplayFatalRestrictionsCommand = new RelayCommand(DisplayFatalRestrictions);
            ChangeSkinCommand = new RelayCommand(ChangeSkin);
            SetInitialEnergyCommand = new RelayCommand<int>(SetInitialEnergy);
            SetInitialIndividualsCommand = new RelayCommand<int>(SetInitialIndividuals);

            DisableChloroplasts = _species.NoChlr;
            DisableDna = _species.DisableDNA;
            DisableMovement = species.DisableMovementSysvars;
            DisableMutations = !species.Mutables.EnableMutations;
            DisableReproduction = species.CantReproduce;
            DisableVision = species.CantSee;
            EnableRepopulation = species.Veg;
            InitialEnergy = species.Stnrg;
            InitialIndividuals = species.qty;
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
                _disableChloroplasts = value;
                RaisePropertyChanged();

                if (DisableChloroplasts)
                    EnableRepopulation = false;
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
            get => enableRepopulation;
            set
            {
                enableRepopulation = value;

                if (enableRepopulation)
                    DisableChloroplasts = false;

                RaisePropertyChanged();
            }
        }

        public int InitialEnergy
        {
            get => initialEnergy;
            set
            {
                initialEnergy = value % 32000;
                RaisePropertyChanged();
            }
        }

        public int InitialIndividuals { get; set; }
        public bool IsFixedInPlace { get; set; }
        public bool IsVeg => EnableRepopulation;
        public bool IsVirusImmune { get; set; }
        public bool KillNonMultibot { get; set; }
        public string Name { get; set; }
        public bool Native => _species.Native;
        public ICommand PickColourCommand { get; }
        public ICommand ResetPositionCommand { get; }
        public ICommand SetInitialEnergyCommand { get; }
        public ICommand SetInitialIndividualsCommand { get; }
        public ObservableCollection<int> Skin { get; set; } = new(new int[8]);

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
                qty = _species.qty,
                Fixed = _species.Fixed,
                Colind = _species.Colind,
                CantSee = _species.CantSee,
                DisableMovementSysvars = _species.DisableMovementSysvars,
                DisableDNA = _species.DisableDNA,
                CantReproduce = _species.CantReproduce,
                VirusImmune = _species.VirusImmune,
                Mutables = _species.Mutables,
                Native = _species.Native,
            };

            return new SpeciesViewModel(species);
        }

        public async Task LoadComment()
        {
            var lines = await File.ReadAllLinesAsync(Path.Combine(_species.path, _species.Name));

            var topComment = lines.Select(s => s.Trim()).TakeWhile(s => s.StartsWith("'") || s.StartsWith("/"));

            _species.Comment = string.Join("\n", topComment.Select(s => s[1..].Trim()));
        }

        public void Save()
        {
            _species.NoChlr = DisableChloroplasts;

            _species.DisableDNA = DisableDna;
            _species.DisableMovementSysvars = DisableMovement;
            _species.Mutables.EnableMutations = !DisableMutations;
            _species.CantReproduce = DisableReproduction;
            _species.CantSee = DisableVision;
            _species.Veg = EnableRepopulation;
            _species.Stnrg = InitialEnergy;
            _species.qty = InitialIndividuals;
            _species.Fixed = IsFixedInPlace;
            _species.VirusImmune = IsVirusImmune;
            _species.kill_mb = KillNonMultibot;
            _species.Name = Name;

            if (DisableChloroplasts)
                _species.Veg = false;
        }

        private void ChangeSkin()
        {
            Skin[6] = (Skin[6] + ThreadSafeRandom.Local.Next(0, Robots.half + 1)) * 2 / 3;
        }

        private void DisplayFatalRestrictions()
        {
            var vm = new RestrictionOptionsViewModel
            {
                DialogState = IsVeg ? RestrictionOptionsDialogState.VegetableKillsOnly : RestrictionOptionsDialogState.NonVegetableKillsOnly
            };

            vm.LoadFromSpecies(this);

            var dialog = new RestrictionOptionsForm();
            var res = dialog.ShowDialog();

            if (res == true)
                vm.SaveToSpecies(this);
        }

        private void SetInitialEnergy(int value)
        {
            InitialIndividuals = value;
        }

        private void SetInitialIndividuals(int value)
        {
            InitialIndividuals = value;
        }
    }
}
