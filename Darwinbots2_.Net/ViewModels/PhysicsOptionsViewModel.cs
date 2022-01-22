using DarwinBots.Model;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using PostSharp.Patterns.Model;
using System;

namespace DarwinBots.ViewModels
{
    [NotifyPropertyChanged(ExcludeExplicitProperties = true)]
    internal class PhysicsOptionsViewModel : ObservableObject
    {
        private double _density;
        private double _viscosity;

        public double BangEfficiency { get; private set; }
        public double BrownianMotion { get; set; }

        public double Density
        {
            get => _density;
            set
            {
                if (SetProperty(ref _density, value))
                    OnPropertyChanged(nameof(ReynoldsNumber));
            }
        }

        public bool EnableTides { get; private set; }
        public double KineticFrictionCoefficient { get; private set; }
        public bool PlanetEaters { get; set; }
        public double PlanetEatersG { get; private set; }

        public double ReynoldsNumber => Density * Robot.RobSize / Math.Max(Viscosity, 1e-7);

        public double StaticFrictionCoefficient { get; private set; }

        public double Viscosity
        {
            get => _viscosity;
            set
            {
                if (SetProperty(ref _viscosity, value))
                    OnPropertyChanged(nameof(ReynoldsNumber));
            }
        }

        public object YGravity { get; set; }
        public double ZAxisGravity { get; private set; }
        public bool ZeroMomentum { get; set; }

        public void LoadFromOptions(OptionsViewModel optionsViewModel)
        {
            ZeroMomentum = optionsViewModel.ZeroMomentum;
            PlanetEaters = optionsViewModel.PlanetEaters;
            PlanetEatersG = optionsViewModel.PlanetEatersG;
            EnableTides = optionsViewModel.EnableTides;

            if (optionsViewModel.EnableTides)
            {
                YGravity = optionsViewModel.YGravity;
                BrownianMotion = optionsViewModel.PhysBrown;
            }

            BangEfficiency = optionsViewModel.PhysMoving;
            ZAxisGravity = optionsViewModel.ZGravity;
            StaticFrictionCoefficient = optionsViewModel.CoefficientStatic;
            KineticFrictionCoefficient = optionsViewModel.CoefficientKinetic;
        }

        internal void SaveToOptions(OptionsViewModel optionsViewModel)
        {
            optionsViewModel.ZeroMomentum = ZeroMomentum;
            optionsViewModel.PlanetEaters = PlanetEaters;
            optionsViewModel.PlanetEatersG = PlanetEatersG;
            optionsViewModel.EnableTides = EnableTides;

            if (EnableTides)
            {
                optionsViewModel.YGravity = YGravity;
                optionsViewModel.PhysBrown = BrownianMotion;
            }

            optionsViewModel.PhysMoving = BangEfficiency;
            optionsViewModel.ZGravity = ZAxisGravity;
            optionsViewModel.CoefficientStatic = StaticFrictionCoefficient;
            optionsViewModel.CoefficientKinetic = KineticFrictionCoefficient;
        }
    }
}
