namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Windows.Input;

    using Gemini.Framework;

    using MottoBeneApps.GoHome.DataModels;
    using MottoBeneApps.GoHome.SystemTray;

    #endregion


    [Export(typeof(IUnknownActivityNotificationPopupViewModel))]
    public sealed class UnknownActivityNotificationPopupViewModel
        : TrayPopup, IUnknownActivityNotificationPopupViewModel
    {
        #region Constants and Fields

        private readonly IActivitiesRepository _activitiesRepository;
        private readonly ObservableCollection<ActivityRecord> _activityRecords;
        private readonly IActivityRecordsRepository _activityRecordsRepository;
        private IEnumerable<Activity> _activities;
        private ActivityRecord _currentActivityRecord;

        #endregion


        #region Constructors and Destructors

        [ImportingConstructor]
        public UnknownActivityNotificationPopupViewModel(
            IActivitiesRepository activitiesRepository,
            IActivityRecordsRepository activityRecordsRepository)
        {
            _activitiesRepository = activitiesRepository;
            _activityRecordsRepository = activityRecordsRepository;

            _activityRecords = new ObservableCollection<ActivityRecord>();

            OkCommand = new RelayCommand(UpdateActivityRecord, CanUpdateActivityRecord);
        }

        #endregion


        #region Events

        public event EventHandler AllRecordsUpdated;
        public event EventHandler<ActivityRecordEventArgs> RecordUpdated;

        #endregion


        #region Properties

        public IEnumerable<Activity> Activities
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

        public IEnumerable<ActivityRecord> ActivityRecords
        {
            get
            {
                return _activityRecords;
            }
        }

        public ActivityRecord CurrentActivityRecord
        {
            get
            {
                return _currentActivityRecord;
            }

            private set
            {
                if (Equals(value, _currentActivityRecord))
                {
                    return;
                }

                _currentActivityRecord = value;

                NotifyOfPropertyChange(() => CurrentActivityRecord);
            }
        }

        public ICommand OkCommand
        {
            get;
            private set;
        }

        public Activity SelectedActivity
        {
            get;
            set;
        }

        #endregion


        #region Public Methods

        public void AddRecord(ActivityRecord record)
        {
            if (_activityRecords.Any(r => r.ActivityRecordId == record.ActivityRecordId))
            {
                return;
            }

            _activityRecords.Add(record);
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

            Activities = _activitiesRepository.GetActivities().ToList();

            GetNextRecord();
        }


        private bool CanUpdateActivityRecord(object obj)
        {
            return SelectedActivity != null;
        }


        private void GetNextRecord()
        {
            CurrentActivityRecord = ActivityRecords.OrderBy(r => r.StartTime).First();
        }


        private void UpdateActivityRecord(object obj)
        {
            CurrentActivityRecord.Activity = SelectedActivity;
            _activityRecordsRepository.Update(CurrentActivityRecord);

            _activityRecords.Remove(CurrentActivityRecord);

            if (RecordUpdated != null)
            {
                RecordUpdated(this, new ActivityRecordEventArgs(CurrentActivityRecord));
            }

            if (_activityRecords.Count > 0)
            {
                GetNextRecord();
            }
            else
            {
                CurrentActivityRecord = null;

                TryClose();

                if (AllRecordsUpdated != null)
                {
                    AllRecordsUpdated(this, EventArgs.Empty);
                }
            }
        }

        #endregion
    }
}