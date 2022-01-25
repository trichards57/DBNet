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

        public double BangEfficiency { get; set; }
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

        public double KineticFrictionCoefficient { get; set; }

        public double ReynoldsNumber => Density * Robot.RobSize / Math.Max(Viscosity, 1e-7);

        public double StaticFrictionCoefficient { get; set; }

        public double Viscosity
        {
            get => _viscosity;
            set
            {
                if (SetProperty(ref _viscosity, value))
                    OnPropertyChanged(nameof(ReynoldsNumber));
            }
        }

        public double YGravity { get; set; }
        public double ZAxisGravity { get; set; }
        public bool ZeroMomentum { get; set; }

        public void LoadFromOptions(OptionsViewModel optionsViewModel)
        {
            ZeroMomentum = optionsViewModel.ZeroMomentum;
            YGravity = optionsViewModel.YGravity;
            BrownianMotion = optionsViewModel.PhysBrown;
            BangEfficiency = optionsViewModel.PhysMoving;
            ZAxisGravity = optionsViewModel.ZGravity;
            StaticFrictionCoefficient = optionsViewModel.CoefficientStatic;
            KineticFrictionCoefficient = optionsViewModel.CoefficientKinetic;
            Viscosity = optionsViewModel.Viscosity;
            Density = optionsViewModel.Density;
        }

        internal void SaveToOptions(OptionsViewModel optionsViewModel)
        {
            optionsViewModel.ZeroMomentum = ZeroMomentum;
            optionsViewModel.PhysMoving = BangEfficiency;
            optionsViewModel.ZGravity = ZAxisGravity;
            optionsViewModel.CoefficientStatic = StaticFrictionCoefficient;
            optionsViewModel.CoefficientKinetic = KineticFrictionCoefficient;
            optionsViewModel.YGravity = YGravity;
            optionsViewModel.PhysBrown = BrownianMotion;
            optionsViewModel.Viscosity = Viscosity;
            optionsViewModel.Density = Density;
        }
    }
}
