using DarwinBots.Model;
using GalaSoft.MvvmLight;
using PostSharp.Patterns.Model;
using System;

namespace DarwinBots.ViewModels
{
    [NotifyPropertyChanged(ExcludeExplicitProperties = true)]
    public class EnergyViewModel : ViewModelBase
    {
        private int dayNightCyclePeriod;
        private bool thresholdAdvancesSun;
        private bool thresholdSuspendsDayCycles;
        private bool thresholdTogglesSunState;
        private int tidesCyclesOff;
        private int tidesCyclesOn;

        public int DayNightCyclePeriod
        {
            get => dayNightCyclePeriod;
            set
            {
                dayNightCyclePeriod = Math.Clamp(value, 0, 32000);
                RaisePropertyChanged();
            }
        }

        public bool EnableDayNightCycles { get; set; }
        public bool EnableSunComesUpThreshold { get; set; }
        public bool EnableSunGoesDownThreshold { get; set; }
        public bool EnableWeather { get; set; }
        public int SunComesUpThreshold { get; set; }
        public int SunGoesDownThreshold { get; set; }

        public bool ThresholdAdvancesSun
        {
            get => thresholdAdvancesSun;
            set
            {
                thresholdAdvancesSun = value;
                if (thresholdAdvancesSun)
                {
                    ThresholdSuspendsDayCycles = false;
                    ThresholdTogglesSunState = false;
                }
                RaisePropertyChanged();
            }
        }

        public bool ThresholdSuspendsDayCycles
        {
            get => thresholdSuspendsDayCycles;
            set
            {
                thresholdSuspendsDayCycles = value;
                if (thresholdSuspendsDayCycles)
                {
                    ThresholdAdvancesSun = false;
                    ThresholdTogglesSunState = false;
                }
                RaisePropertyChanged();
            }
        }

        public bool ThresholdTogglesSunState
        {
            get => thresholdTogglesSunState;
            set
            {
                thresholdTogglesSunState = value;
                if (thresholdTogglesSunState)
                {
                    ThresholdAdvancesSun = false;
                    thresholdSuspendsDayCycles = false;
                }
                RaisePropertyChanged();
            }
        }

        public int TidesCyclesOff
        {
            get => tidesCyclesOff;
            set
            {
                tidesCyclesOff = Math.Clamp(value, 0, 32000);
                RaisePropertyChanged();
            }
        }

        public int TidesCyclesOn
        {
            get => tidesCyclesOn;
            set
            {
                tidesCyclesOn = Math.Clamp(value, 0, 32000);
                RaisePropertyChanged();
            }
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
