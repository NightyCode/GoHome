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
        private IEnumerable<ActivityLogViewModel> _activities;
        private ActivityLogViewModel _selectedActivity;
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

        public IEnumerable<ActivityLogViewModel> Activities
        {
            get
            {
                return _activities;
            }

            private set
            {
                if (Equals(value, _activities))
                {
                    return;
                }

                _activities = value;

                NotifyOfPropertyChange(() => Activities);
            }
        }

        public ActivityLogViewModel SelectedActivity
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

            var activities = new List<ActivityLogViewModel>();

            List<ActivityRecord> records = _activityRecordsRepository.GetActivityLog(DateTime.Now).ToList();
            var workRecords = records.Where(r => r.Activity.IsWork).ToList();

            TimeSpan remainingTime = _settings.WorkDayDuration;

            if (workRecords.Count == 0)
            {
                WorkdayStartTime = DateTime.Now;
            }
            else
            {
                var workdayDurationTicks = workRecords.Sum(r => r.DurationTicks);
                var workdayDuration = TimeSpan.FromTicks(workdayDurationTicks);
                remainingTime = _settings.WorkDayDuration - workdayDuration;

                WorkdayStartTime = workRecords.Min(r => r.StartTime);
                WorkdayEndTime = DateTime.Now + remainingTime;

                foreach (var activityRecords in records.GroupBy(r => r.Activity))
                {
                    string activityName = "Undefined";

                    if (activityRecords.Key != null)
                    {
                        activityName = activityRecords.Key.Name;
                    }

                    activities.Add(new ActivityLogViewModel(activityName, activityRecords.ToList()));
                }
            }

            activities.Add(new ActivityLogViewModel("Remaining time", (int)Math.Round(remainingTime.TotalMinutes)));

            Activities = activities;
        }

        #endregion
    }
}