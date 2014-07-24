namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System;
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using Gemini.Framework.Services;
    using Gemini.Modules.Settings;

    using MottoBeneApps.GoHome.ActivityTracking.Properties;

    #endregion


    [Export(typeof(IActivityTrackingSettings))]
    [Export(typeof(ISettingsEditor))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class ActivityTrackingSettingsViewModel
        : PropertyChangedBase, IActivityTrackingSettings, ISettingsEditor
    {
        #region Constants and Fields

        private TimeSpan _minimumActivityDuration;
        private TimeSpan _minimumIdleDuration;
        private TimeSpan _workDayDuration;

        #endregion


        #region Constructors and Destructors

        /// <summary>
        /// Creates an instance of <see cref="T:Caliburn.Micro.PropertyChangedBase"/>.
        /// </summary>
        public ActivityTrackingSettingsViewModel()
        {
            LoadSettings();

            var eventManager = new SettingsPropertyChangedEventManager<Settings>(Settings.Default);

            eventManager.AddListener(
                s => s.ActiveThreshold,
                i => MinimumActivityDuration = TimeSpan.FromMilliseconds(i));

            eventManager.AddListener(s => s.IdleThreshold, i => MinimumIdleDuration = TimeSpan.FromMilliseconds(i));
            eventManager.AddListener(s => s.WorkDayDuration, i => WorkDayDuration = TimeSpan.FromMinutes(i));
        }

        #endregion


        #region Properties

        public TimeSpan MinimumActivityDuration
        {
            get
            {
                return _minimumActivityDuration;
            }

            set
            {
                if (value.Equals(_minimumActivityDuration))
                {
                    return;
                }

                _minimumActivityDuration = value;

                NotifyOfPropertyChange(() => MinimumActivityDuration);
            }
        }

        public TimeSpan MinimumIdleDuration
        {
            get
            {
                return _minimumIdleDuration;
            }

            set
            {
                if (value.Equals(_minimumIdleDuration))
                {
                    return;
                }

                _minimumIdleDuration = value;

                NotifyOfPropertyChange(() => MinimumIdleDuration);
            }
        }

        public TimeSpan WorkDayDuration
        {
            get
            {
                return _workDayDuration;
            }

            set
            {
                if (value.Equals(_workDayDuration))
                {
                    return;
                }

                _workDayDuration = value;

                NotifyOfPropertyChange(() => WorkDayDuration);
            }
        }

        string ISettingsEditor.SettingsPageName
        {
            get
            {
                return "General";
            }
        }

        string ISettingsEditor.SettingsPagePath
        {
            get
            {
                return "Activity Tracking";
            }
        }

        #endregion


        #region Public Methods

        public void ApplyChanges()
        {
            Settings.Default.ActiveThreshold = (int)MinimumActivityDuration.TotalMilliseconds;
            Settings.Default.IdleThreshold = (int)MinimumIdleDuration.TotalMilliseconds;
            Settings.Default.WorkDayDuration = (int)WorkDayDuration.TotalMinutes;

            Settings.Default.Save();
        }

        #endregion


        #region Methods

        private void LoadSettings()
        {
            MinimumActivityDuration = TimeSpan.FromMilliseconds(Settings.Default.ActiveThreshold);
            MinimumIdleDuration = TimeSpan.FromMilliseconds(Settings.Default.IdleThreshold);
            WorkDayDuration = TimeSpan.FromMinutes(Settings.Default.WorkDayDuration);
        }

        #endregion
    }
}