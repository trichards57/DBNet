using AutoFixture;
using DarwinBots.Model;
using DarwinBots.Modules;
using DarwinBots.Services;
using DarwinBots.ViewModels;
using FluentAssertions;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DBNetTests.ViewModels
{
    public class OptionsViewModelTests
    {
        private readonly Fixture _fixture;

        public OptionsViewModelTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void CyclesHigh_InputClamped()
        {
            var vm = new OptionsViewModel();
            vm.CyclesHigh = 700000;
            vm.CyclesHigh.Should().Be(500000);
            vm.CyclesHigh = -1;
            vm.CyclesHigh.Should().Be(0);
        }

        [Fact]
        public void CyclesLow_InputClamped()
        {
            var vm = new OptionsViewModel();
            vm.CyclesLow = 700000;
            vm.CyclesLow.Should().Be(500000);
            vm.CyclesLow = -1;
            vm.CyclesLow.Should().Be(0);
        }

        [Fact]
        public void EnableCorpseMode_UpdatesDecaySettings()
        {
            var vm = new OptionsViewModel();
            vm.EnableCorpseMode.Should().BeFalse();
            vm.DecayTypeNone.Should().BeTrue();
            vm.DecayTypeEnergy.Should().BeFalse();
            vm.DecayTypeWaste.Should().BeFalse();
            vm.DecayRate.Should().Be(0);
            vm.DecayPeriod.Should().Be(0);

            vm.EnableCorpseMode = true;
            vm.DecayTypeNone.Should().BeFalse();
            vm.DecayTypeEnergy.Should().BeTrue();
            vm.DecayTypeWaste.Should().BeFalse();
            vm.DecayRate.Should().Be(75);
            vm.DecayPeriod.Should().Be(3);

            vm.EnableCorpseMode = false;
            vm.DecayTypeNone.Should().BeFalse();
            vm.DecayTypeEnergy.Should().BeTrue();
            vm.DecayTypeWaste.Should().BeFalse();
            vm.DecayRate.Should().Be(75);
            vm.DecayPeriod.Should().Be(3);
        }

        [Fact]
        public void EnabledMutationSineWave_UpdatesMinMaxLabels()
        {
            var vm = new OptionsViewModel();
            vm.EnableMutationSineWave = true;
            vm.MaxCyclesLabel.Should().Be("Max at 20x");
            vm.MinCyclesLabel.Should().Be("Max at 1/20x");
            vm.EnableMutationSineWave = false;
            vm.MaxCyclesLabel.Should().Be("Cycles at 16x");
            vm.MinCyclesLabel.Should().Be("Cycles at 1/16x");
        }

        [Fact]
        public void ListNativeSpecies_ReportsNoneWhenAllFiltered()
        {
            var testSpecies = _fixture.Build<Species>()
                .Without(c => c.Color)
                .With(c => c.Native, true)
                .CreateMany().ToList();

            var dialogService = new Mock<IDialogService>();
            var clipboardService = new Mock<IClipboardService>();

            var vm = new OptionsViewModel(clipboardService.Object, dialogService.Object);

            vm.SpeciesList.Clear();
            foreach (var s in testSpecies)
            {
                vm.SpeciesList.Add(new SpeciesViewModel(s));
            }

            vm.ListNonNativeSpeciesCommand.Execute(null);

            dialogService.Verify(d => d.ShowInfoMessageBox("Non-Native Species Summary", "There are no non-native species."));
            clipboardService.VerifyNoOtherCalls();
        }

        [Fact]
        public void ListNativeSpecies_ReportsNoneWhenListEmpty()
        {
            var dialogService = new Mock<IDialogService>();
            var clipboardService = new Mock<IClipboardService>();

            var vm = new OptionsViewModel(clipboardService.Object, dialogService.Object);

            vm.SpeciesList.Clear();

            vm.ListNonNativeSpeciesCommand.Execute(null);

            dialogService.Verify(d => d.ShowInfoMessageBox("Non-Native Species Summary", "There are no non-native species."));
            clipboardService.VerifyNoOtherCalls();
        }

        [Fact]
        public void ListNativeSpecies_ReportsNonNative()
        {
            var testSpecies = _fixture.Build<Species>()
                .Without(c => c.Color)
                .With(c => c.Native, true)
                .CreateMany(5).ToList();

            testSpecies.First().Native = false;
            testSpecies.Last().Native = false;

            var dialogService = new Mock<IDialogService>();
            var clipboardService = new Mock<IClipboardService>();

            var vm = new OptionsViewModel(clipboardService.Object, dialogService.Object);

            vm.SpeciesList.Clear();
            foreach (var s in testSpecies)
            {
                vm.SpeciesList.Add(new SpeciesViewModel(s));
            }

            vm.ListNonNativeSpeciesCommand.Execute(null);

            var names = $"\"{testSpecies.First().Name}\", \"{testSpecies.Last().Name}\"";

            dialogService.Verify(d => d.ShowInfoMessageBox("Non-Native Species Summary", $"The non-native species are:\n\n{names}\n\nThese have been copied to the clipboard."));
            clipboardService.Verify(c => c.CopyCsvToClipboard(names));
        }

        [Fact]
        public async Task LoadFromOptions_HandlesBrownianMotion()
        {
            var options = _fixture.Create<SimOptions>();
            var vm = new OptionsViewModel();

            options.PhysBrown = 0.1;
            await vm.LoadFromOptions(options);

            vm.BrownianMotion.Should().Be(BrownianMotion.Animal);

            options.PhysBrown = 0.6;
            await vm.LoadFromOptions(options);

            vm.BrownianMotion.Should().Be(BrownianMotion.Bacterial);

            options.PhysBrown = 10;
            await vm.LoadFromOptions(options);

            vm.BrownianMotion.Should().Be(BrownianMotion.Molecular);
        }

        [Fact]
        public async Task LoadFromOptions_HandlesCosts()
        {
            var options = _fixture.Create<SimOptions>();
            var vm = new OptionsViewModel();

            options.Costs = Costs.ZeroCosts;
            await vm.LoadFromOptions(options);

            vm.CostsNoCosts.Should().BeTrue();
            vm.CostsCustom.Should().BeFalse();

            options.Costs = Costs.DefaultCosts;
            await vm.LoadFromOptions(options);

            vm.CostsNoCosts.Should().BeFalse();
            vm.CostsCustom.Should().BeTrue();
        }

        [Fact]
        public async Task LoadFromOptions_HandlesDecayType()
        {
            var options = _fixture.Create<SimOptions>();
            var vm = new OptionsViewModel();

            options.DecayType = DecayType.Waste;
            await vm.LoadFromOptions(options);

            vm.DecayTypeWaste.Should().BeTrue();
            vm.DecayTypeEnergy.Should().BeFalse();
            vm.DecayTypeNone.Should().BeFalse();

            options.DecayType = DecayType.None;
            await vm.LoadFromOptions(options);

            vm.DecayTypeWaste.Should().BeFalse();
            vm.DecayTypeEnergy.Should().BeFalse();
            vm.DecayTypeNone.Should().BeTrue();

            options.DecayType = DecayType.Energy;
            await vm.LoadFromOptions(options);

            vm.DecayTypeWaste.Should().BeFalse();
            vm.DecayTypeEnergy.Should().BeTrue();
            vm.DecayTypeNone.Should().BeFalse();
        }

        [Fact]
        public async Task LoadFromOptions_HandlesDragCoefficient()
        {
            var options = _fixture.Create<SimOptions>();
            var vm = new OptionsViewModel();

            options.Viscosity = 0.01;
            options.Density = 0.0000001;
            await vm.LoadFromOptions(options);

            vm.MovementDrag = DragPresets.ThickFluid;

            options.Viscosity = 0.0005;
            options.Density = 0.0000001;
            await vm.LoadFromOptions(options);

            vm.MovementDrag = DragPresets.Transitory;

            options.Viscosity = 0.000025;
            options.Density = 0.0000001;
            await vm.LoadFromOptions(options);

            vm.MovementDrag = DragPresets.ThinFluid;

            options.Viscosity = 0;
            options.Density = 0;
            await vm.LoadFromOptions(options);

            vm.MovementDrag = DragPresets.None;

            options.Viscosity = 1.23;
            options.Density = 4.56;
            await vm.LoadFromOptions(options);

            vm.MovementDrag = DragPresets.Custom;
        }

        [Fact]
        public async Task LoadFromOptions_HandlesFieldMode()
        {
            var options = _fixture.Create<SimOptions>();
            var vm = new OptionsViewModel();

            options.FluidSolidCustom = FieldMode.Solid;
            await vm.LoadFromOptions(options);

            vm.FieldModeSolid.Should().BeTrue();
            vm.FieldModeFluid.Should().BeFalse();
            vm.FieldModeCustom.Should().BeFalse();

            options.FluidSolidCustom = FieldMode.Custom;
            await vm.LoadFromOptions(options);

            vm.FieldModeSolid.Should().BeFalse();
            vm.FieldModeFluid.Should().BeFalse();
            vm.FieldModeCustom.Should().BeTrue();

            options.FluidSolidCustom = FieldMode.Fluid;
            await vm.LoadFromOptions(options);

            vm.FieldModeSolid.Should().BeFalse();
            vm.FieldModeFluid.Should().BeTrue();
            vm.FieldModeCustom.Should().BeFalse();
        }

        [Fact]
        public async Task LoadFromOptions_HandlesFrictionCoefficient()
        {
            var options = _fixture.Create<SimOptions>();
            var vm = new OptionsViewModel();

            options.CoefficientKinetic = 0.75;
            options.CoefficientStatic = 0.9;
            options.ZGravity = 4;
            await vm.LoadFromOptions(options);

            vm.MovementFriction = FrictionPresets.Sandpaper;

            options.CoefficientKinetic = 0.4;
            options.CoefficientStatic = 0.6;
            options.ZGravity = 2;
            await vm.LoadFromOptions(options);

            vm.MovementFriction = FrictionPresets.Metal;

            options.CoefficientKinetic = 0.05;
            options.CoefficientStatic = 0.05;
            options.ZGravity = 1;
            await vm.LoadFromOptions(options);

            vm.MovementFriction = FrictionPresets.Teflon;

            options.CoefficientKinetic = 0;
            options.CoefficientStatic = 0;
            options.ZGravity = 0;
            await vm.LoadFromOptions(options);

            vm.MovementFriction = FrictionPresets.None;

            options.CoefficientKinetic = 1.23;
            options.CoefficientStatic = 4.56;
            options.ZGravity = 7;
            await vm.LoadFromOptions(options);

            vm.MovementFriction = FrictionPresets.Custom;
        }

        [Fact]
        public async Task LoadFromOptions_HandlesGravity()
        {
            var options = _fixture.Create<SimOptions>();
            var vm = new OptionsViewModel();

            options.YGravity = 0.1;
            await vm.LoadFromOptions(options);

            vm.Gravity.Should().Be(VerticalGravity.None);

            options.YGravity = 0.2;
            await vm.LoadFromOptions(options);

            vm.Gravity.Should().Be(VerticalGravity.Moon);

            options.YGravity = 0.4;
            await vm.LoadFromOptions(options);

            vm.Gravity.Should().Be(VerticalGravity.Earth);

            options.YGravity = 5;
            await vm.LoadFromOptions(options);

            vm.Gravity.Should().Be(VerticalGravity.Jupiter);

            options.YGravity = 10;
            await vm.LoadFromOptions(options);

            vm.Gravity.Should().Be(VerticalGravity.Star);
        }

        [Fact]
        public async Task LoadFromOptions_HandlesMovementEfficiency()
        {
            var options = _fixture.Create<SimOptions>();
            var vm = new OptionsViewModel();

            options.PhysMoving = 0.1;
            await vm.LoadFromOptions(options);

            vm.MovementEfficiency.Should().Be(MovementEfficiency.Mechanical);

            options.PhysMoving = 0.5;
            await vm.LoadFromOptions(options);

            vm.MovementEfficiency.Should().Be(MovementEfficiency.Biological);

            options.PhysMoving = 0.9;
            await vm.LoadFromOptions(options);

            vm.MovementEfficiency.Should().Be(MovementEfficiency.Ideal);
        }

        [Fact]
        public async Task LoadFromOptions_HandlesShotModes()
        {
            var options = _fixture.Create<SimOptions>();
            var vm = new OptionsViewModel();

            options.EnergyExType = ShotMode.Proportional;
            await vm.LoadFromOptions(options);

            vm.ShotModeProportional.Should().BeTrue();
            vm.ShotModeFixedEnergy.Should().BeFalse();

            options.EnergyExType = ShotMode.Fixed;
            await vm.LoadFromOptions(options);

            vm.ShotModeFixedEnergy.Should().BeTrue();
            vm.ShotModeProportional.Should().BeFalse();
        }

        [Fact]
        public async Task LoadFromOptions_HandlesWasteThreshold()
        {
            var options = _fixture.Create<SimOptions>();
            var vm = new OptionsViewModel();

            options.BadWasteLevel = -1;
            await vm.LoadFromOptions(options);

            vm.WasteThreshold.Should().Be(0);

            options.BadWasteLevel = 20;
            await vm.LoadFromOptions(options);

            vm.WasteThreshold.Should().Be(20);
        }

        [Fact]
        public async Task LoadFromOptions_LoadsBasicDetails()
        {
            var options = _fixture.Create<SimOptions>();

            var vm = new OptionsViewModel();

            await vm.LoadFromOptions(options);

            vm.MaximumChloroplasts.Should().Be(options.MaxPopulation);
            vm.MinimumChloroplastsThreshold.Should().Be(options.MinVegs);
            vm.RobotsPerRepopulationEvent.Should().Be(options.RepopAmount);
            vm.RepopulationCooldownPeriod.Should().Be(options.RepopCooldown);
            vm.EnableCorpseMode.Should().Be(options.CorpseEnabled);
            vm.DecayRate.Should().Be(options.Decay);
            vm.DecayPeriod.Should().Be(options.DecayDelay);
            vm.ShotProportion.Should().Be(options.EnergyProp * 100);
            vm.ShotEnergy.Should().Be(options.EnergyFix);
            vm.MutationMultiplier.Should().Be(Math.Log(Math.Max(options.MutCurrMult, 0), 2));
            vm.EnableMutationCycling.Should().Be(options.MutOscill);
            vm.EnableMutationSineWave.Should().Be(options.MutOscillSine);
            vm.DisableMutations.Should().Be(options.DisableMutations);
            vm.CyclesHigh.Should().Be(options.MutCycMax);
            vm.CyclesLow.Should().Be(options.MutCycMin);
            vm.InitialLightEnergy.Should().Be(options.MaxEnergy);
            vm.MaxVelocity.Should().Be(options.MaxVelocity);
            vm.VegEnergyBodyDistribution.Should().Be(options.VegFeedingToBody * 100);
            vm.CollisionElasticity.Should().Be(options.CoefficientElasticity * 10);
            vm.FixBotRadii.Should().Be(options.FixedBotRadii);
            vm.SelectedSpecies.Should().BeNull();
        }

        [Fact]
        public async Task LoadFromOptions_ShouldLoadSpecies()
        {
            var options = _fixture.Create<SimOptions>();
            var testSpecies = _fixture.Build<Species>()
                .Without(c => c.Color)
                .With(c => c.Path, "TestSpecies")
                .With(c => c.Name, "Alga_Minimalis_Chloroplastus.txt")
                .CreateMany().ToList();

            var expectedSpecies = testSpecies.Select(s => new SpeciesViewModel(s));
            foreach (var s in expectedSpecies)
            {
                await s.LoadComment();
            }

            options.Specie.AddRange(testSpecies);

            var vm = new OptionsViewModel();

            await vm.LoadFromOptions(options);

            vm.SpeciesList.Should().BeEquivalentTo(expectedSpecies);
        }
    }
}
