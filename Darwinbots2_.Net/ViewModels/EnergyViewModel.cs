using System;
using DarwinBots.Model;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using PostSharp.Patterns.Model;

namespace DarwinBots.ViewModels
{
    [NotifyPropertyChanged(ExcludeExplicitProperties = true)]
    public class EnergyViewModel : ObservableObject
    {
        private int _dayNightCyclePeriod;
        private bool _thresholdAdvancesSun;
        private bool _thresholdSuspendsDayCycles;
        private bool _thresholdTogglesSunState;
        private int _tidesCyclesOff;
        private int _tidesCyclesOn;

        public int DayNightCyclePeriod
        {
            get => _dayNightCyclePeriod;
            set => SetProperty(ref _dayNightCyclePeriod, Math.Clamp(value, 0, 32000));
        }

        public bool EnableDayNightCycles { get; set; }
        public bool EnableSunComesUpThreshold { get; set; }
        public bool EnableSunGoesDownThreshold { get; set; }
        public bool EnableWeather { get; set; }
        public int SunComesUpThreshold { get; set; }
        public int SunGoesDownThreshold { get; set; }

        public bool ThresholdAdvancesSun
        {
            get => _thresholdAdvancesSun;
            set
            {
                if (SetProperty(ref _thresholdAdvancesSun, value) && _thresholdAdvancesSun)
                {
                    ThresholdSuspendsDayCycles = false;
                    ThresholdTogglesSunState = false;
                }
            }
        }

        public bool ThresholdSuspendsDayCycles
        {
            get => _thresholdSuspendsDayCycles;
            set
            {
                if (SetProperty(ref _thresholdSuspendsDayCycles, value) && _thresholdSuspendsDayCycles)
                {
                    ThresholdAdvancesSun = false;
                    ThresholdTogglesSunState = false;
                }
            }
        }

        public bool ThresholdTogglesSunState
        {
            get => _thresholdTogglesSunState;
            set
            {
                if (SetProperty(ref _thresholdTogglesSunState, value) && _thresholdTogglesSunState)
                {
                    ThresholdAdvancesSun = false;
                    _thresholdSuspendsDayCycles = false;
                }
            }
        }

        public int TidesCyclesOff
        {
            get => _tidesCyclesOff;
            set => SetProperty(ref _tidesCyclesOff, Math.Clamp(value, 0, 32000));
        }

        public int TidesCyclesOn
        {
            get => _tidesCyclesOn;
            set => SetProperty(ref _tidesCyclesOn, Math.Clamp(value, 0, 32000));
        }

        public void LoadFromOptions(SimOptions options)
        {
            DayNightCyclePeriod = options.CycleLength;
            EnableDayNightCycles = options.DayNight;
            SunComesUpThreshold = options.SunUpThreshold;
            EnableSunComesUpThreshold = options.SunUp;
            SunGoesDownThreshold = options.SunDownThreshold;
            EnableSunGoesDownThreshold = options.SunDown;

            switch (options.SunThresholdMode)
            {
                case SunThresholdMode.TemporarilySuspend:
                    ThresholdSuspendsDayCycles = true;
                    break;

                case SunThresholdMode.AdvanceToDawnDusk:
                    ThresholdAdvancesSun = true;
                    break;

                case SunThresholdMode.PermanentlyToggle:
                    ThresholdTogglesSunState = true;
                    break;
            }

            EnableWeather = options.SunOnRnd;

            TidesCyclesOn = options.Tides;
            TidesCyclesOff = options.TidesOf;
        }

        public void SaveToOptions(SimOptions options)
        {
            options.CycleLength = DayNightCyclePeriod;
            options.DayNight = EnableDayNightCycles;
            options.SunUpThreshold = SunComesUpThreshold;
            options.SunUp = EnableSunComesUpThreshold;
            options.SunDownThreshold = SunGoesDownThreshold;
            options.SunDown = EnableSunGoesDownThreshold;

            if (ThresholdSuspendsDayCycles)
                options.SunThresholdMode = SunThresholdMode.TemporarilySuspend;
            else if (ThresholdAdvancesSun)
                options.SunThresholdMode = SunThresholdMode.AdvanceToDawnDusk;
            else
                options.SunThresholdMode = SunThresholdMode.PermanentlyToggle;

            options.SunOnRnd = EnableWeather;

            options.Tides = TidesCyclesOn;
            options.TidesOf = TidesCyclesOff;
        }
    }
}
