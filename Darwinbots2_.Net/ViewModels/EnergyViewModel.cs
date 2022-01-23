using Microsoft.Toolkit.Mvvm.ComponentModel;
using PostSharp.Patterns.Model;
using System;

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

        public bool EnableTides { get; set; }
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

        public void LoadFromOptions(OptionsViewModel options)
        {
            DayNightCyclePeriod = options.DayNightCyclePeriod;
            EnableDayNightCycles = options.EnableDayNightCycles;
            SunComesUpThreshold = options.SunComesUpThreshold;
            EnableSunComesUpThreshold = options.EnableSunComesUpThreshold;
            SunGoesDownThreshold = options.SunGoesDownThreshold;
            EnableSunGoesDownThreshold = options.EnableSunGoesDownThreshold;
            ThresholdSuspendsDayCycles = options.ThresholdSuspendsDayCycles;
            ThresholdAdvancesSun = options.ThresholdAdvancesSun;
            ThresholdTogglesSunState = options.ThresholdTogglesSunState;
            EnableWeather = options.EnableWeather;
            TidesCyclesOn = options.TidesCyclesOn;
            TidesCyclesOff = options.TidesCyclesOff;
            EnableTides = options.EnableTides;
        }

        public void SaveToOptions(OptionsViewModel options)
        {
            options.DayNightCyclePeriod = DayNightCyclePeriod;
            options.EnableDayNightCycles = EnableDayNightCycles;
            options.SunComesUpThreshold = SunComesUpThreshold;
            options.EnableSunComesUpThreshold = EnableSunComesUpThreshold;
            options.SunGoesDownThreshold = SunGoesDownThreshold;
            options.EnableSunGoesDownThreshold = EnableSunGoesDownThreshold;
            options.ThresholdSuspendsDayCycles = ThresholdSuspendsDayCycles;
            options.ThresholdAdvancesSun = ThresholdAdvancesSun;
            options.ThresholdTogglesSunState = ThresholdTogglesSunState;
            options.EnableWeather = EnableWeather;
            options.TidesCyclesOn = TidesCyclesOn;
            options.TidesCyclesOff = TidesCyclesOff;
            options.EnableTides = EnableTides;
        }
    }
}
