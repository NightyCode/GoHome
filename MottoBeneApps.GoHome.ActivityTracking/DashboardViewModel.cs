namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Windows;

    using Gemini.Framework;

    using MottoBeneApps.GoHome.ActivityTracking.Properties;
    using MottoBeneApps.GoHome.DataModels;

    #endregion


    [Export(typeof(IDashboard))]
    public sealed class DashboardViewModel : Document, IDashboard
    {
        #region Constants and Fields

        private readonly IActivityRecordsRepository _activityRecordsRepository;
        private readonly IUserActivityTracker _activityTracker;
        private readonly IActivityTrackingSettings _settings;
        private IEnumerable<IActivityChartPiece> _gaugeChartPieces;
        private IEnumerable<IActivityChartPiece> _pieChartPieces;

        private IActivityChartPiece _selectedActivity;
        private Window _window;
        private DateTime _workdayEndTime;
        private DateTime _workdayStartTime;

        #endregion


        #region Constructors and Destructors

        [ImportingConstructor]
        public DashboardViewModel(
            IActivityRecordsRepository activityRecordsRepository,
            IUserActivityTracker activityTracker,
            IActivityTrackingSettings settings)
        {
            _activityRecordsRepository = activityRecordsRepository;
            _activityTracker = activityTracker;
            _settings = settings;

            DisplayName = "Dashboard";
        }

        #endregion


        #region Properties

        public IEnumerable<IActivityChartPiece> GaugeChartPieces
        {
            get
            {
                return _gaugeChartPieces;
            }

            private set
            {
                if (Equals(value, _gaugeChartPieces))
                {
                    return;
                }

                _gaugeChartPieces = value;

                NotifyOfPropertyChange(() => GaugeChartPieces);
            }
        }

        public IEnumerable<IActivityChartPiece> PieChartPieces
        {
            get
            {
                return _pieChartPieces;
            }


            private set
            {
                if (Equals(value, _pieChartPieces))
                {
                    return;
                }

                _pieChartPieces = value;

                NotifyOfPropertyChange(() => PieChartPieces);
            }
        }

        public IActivityChartPiece SelectedActivity
        {
            get
            {
                return _selectedActivity;
            }

            set
            {
                if (Equals(value, _selectedActivity))
                {
                    return;
                }

                _selectedActivity = value;

                NotifyOfPropertyChange(() => SelectedActivity);
            }
        }

        public override bool ShouldReopenOnStart
        {
            get
            {
                return true;
            }
        }

        public DateTime WorkdayEndTime
        {
            get
            {
                return _workdayEndTime;
            }

            private set
            {
                if (value.Equals(_workdayEndTime))
                {
                    return;
                }

                _workdayEndTime = value;

                NotifyOfPropertyChange(() => WorkdayEndTime);
            }
        }


        public DateTime WorkdayStartTime
        {
            get
            {
                return _workdayStartTime;
            }

            private set
            {
                if (value.Equals(_workdayStartTime))
                {
                    return;
                }

                _workdayStartTime = value;

                NotifyOfPropertyChange(() => WorkdayStartTime);
            }
        }

        #endregion


        #region Methods

        /// <summary>
        /// Called when activating.
        /// </summary>
        protected override void OnActivate()
        {
            base.OnActivate();

            RefreshData();
        }


        /// <summary>
        /// Called when deactivating.
        /// </summary>
        /// <param name="close">Inidicates whether this instance will be closed.</param>
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (!close)
            {
                return;
            }

            if (_window != null)
            {
                _window.StateChanged -= OnWindowStateChanged;
            }

            Settings.Default.PropertyChanged -= OnSettingsPropertyChanged;
        }


        /// <summary>
        /// Called when an attached view's Loaded event fires.
        /// </summary>
        /// <param name="view"/>
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            _window = Window.GetWindow((DependencyObject)view);

            if (_window != null)
            {
                _window.StateChanged += OnWindowStateChanged;
            }

            Settings.Default.PropertyChanged += OnSettingsPropertyChanged;

            RefreshData();
        }


        private void OnSettingsPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            RefreshData();
        }


        private void OnWindowStateChanged(object sender, EventArgs e)
        {
            if (_window.WindowState != WindowState.Minimized)
            {
                RefreshData();
            }
        }


        private void RefreshData()
        {
            _activityTracker.UpdateUserActivityLog();

            List<ActivityRecord> records = _activityRecordsRepository.GetActivityLog(DateTime.Now).ToList();
            var workRecords = records.Where(r => r.Activity.IsWork).ToList();

            TimeSpan remainingTime = _settings.WorkDayDuration;
            TimeSpan extraTime = TimeSpan.Zero;
            TimeSpan workdayDuration = TimeSpan.Zero;

            var chartPieces = new List<IActivityChartPiece>();

            if (workRecords.Count == 0)
            {
                WorkdayStartTime = DateTime.Now;
            }
            else
            {
                var workdayDurationTicks = workRecords.Sum(r => r.DurationTicks);
                workdayDuration = TimeSpan.FromTicks(workdayDurationTicks);

                if (workdayDuration < _settings.WorkDayDuration)
                {
                    remainingTime = _settings.WorkDayDuration - workdayDuration;
                }
                else
                {
                    remainingTime = TimeSpan.Zero;
                    extraTime = workdayDuration - _settings.WorkDayDuration;
                }

                WorkdayStartTime = workRecords.Min(r => r.StartTime);
                WorkdayEndTime = DateTime.Now + remainingTime;

                var recordsByActivity = workRecords.GroupBy(r => r.Activity).ToList();

                foreach (var activityRecords in recordsByActivity)
                {
                    chartPieces.Add(
                        new ActivityRecordsLogChartPiece(activityRecords.Key, activityRecords.ToList(), workdayDuration));
                }
            }

            var gaugeChartPieces = chartPieces.ToList();

            gaugeChartPieces.Insert(
                0,
                new ActivityChartPiece("Total work time", workdayDuration, _settings.WorkDayDuration));

            GaugeChartPieces = gaugeChartPieces;

            if (remainingTime.TotalMinutes > 0)
            {
                chartPieces.Add(new ActivityChartPiece("Remaining time", remainingTime, _settings.WorkDayDuration));
            }

            if (extraTime.TotalMinutes > 0)
            {
                chartPieces.Add(new ActivityChartPiece("Extra time", extraTime, _settings.WorkDayDuration));
            }

            PieChartPieces = chartPieces;
        }

        #endregion
    }
}