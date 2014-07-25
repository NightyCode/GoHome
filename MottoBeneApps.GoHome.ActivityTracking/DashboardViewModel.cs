namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Gemini.Framework;

    using MottoBeneApps.GoHome.DataModels;

    #endregion


    [Export(typeof(IDashboard))]
    public sealed class DashboardViewModel : Document, IDashboard
    {
        #region Constants and Fields

        private readonly IActivityRecordsRepository _activityRecordsRepository;
        private readonly IUserActivityTracker _activityTracker;
        private readonly IActivityTrackingSettings _settings;
        private IEnumerable<IActivityChartPiece> _pieChartPieces;

        private IActivityChartPiece _selectedActivity;
        private DateTime _workdayEndTime;
        private DateTime _workdayStartTime;
        private IEnumerable<IActivityChartPiece> _gaugeChartPieces;

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
        /// Called when an attached view's Loaded event fires.
        /// </summary>
        /// <param name="view"/>
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            RefreshData();
        }


        private void RefreshData()
        {
            _activityTracker.UpdateUserActivityLog();

            List<ActivityRecord> records = _activityRecordsRepository.GetActivityLog(DateTime.Now).ToList();
            var workRecords = records.Where(r => r.Activity.IsWork).ToList();

            TimeSpan remainingTime = _settings.WorkDayDuration;
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
                remainingTime = _settings.WorkDayDuration - workdayDuration;

                WorkdayStartTime = workRecords.Min(r => r.StartTime);
                WorkdayEndTime = DateTime.Now + remainingTime;

                var recordsByActivity = workRecords.GroupBy(r => r.Activity).ToList();

                foreach (var activityRecords in recordsByActivity)
                {
                    chartPieces.Add(
                        new ActivityRecordsLogChartPiece(
                            activityRecords.Key,
                            activityRecords.ToList(),
                            _settings.WorkDayDuration.TotalMinutes));
                }
            }

            var gaugeChartPieces = chartPieces.ToList();

            gaugeChartPieces.Insert(0, new ActivityChartPiece(
                "Total work time",
                (int)Math.Round(workdayDuration.TotalMinutes),
                _settings.WorkDayDuration.TotalMinutes));

            GaugeChartPieces = gaugeChartPieces;

            chartPieces.Add(new ActivityChartPiece(
                "Remaining time",
                (int)Math.Round(remainingTime.TotalMinutes),
                _settings.WorkDayDuration.TotalMinutes));

            PieChartPieces = chartPieces;
        }

        #endregion
    }
}