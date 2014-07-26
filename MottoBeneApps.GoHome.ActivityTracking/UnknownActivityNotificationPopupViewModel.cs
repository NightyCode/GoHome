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

            ActivityRecords = new ObservableCollection<ActivityRecord>();

            OkCommand = new RelayCommand(UpdateActivityRecord, CanUpdateActivityRecord);
        }

        #endregion


        #region Events

        public event EventHandler RecordsUpdated;

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

        public ObservableCollection<ActivityRecord> ActivityRecords
        {
            get;
            private set;
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


        #region Methods

        /// <summary>
        /// Called when an attached view's Loaded event fires.
        /// </summary>
        /// <param name="view"/>
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            Activities = _activitiesRepository.GetActivities().ToList();

            CurrentActivityRecord = ActivityRecords.First();
        }


        private bool CanUpdateActivityRecord(object obj)
        {
            return SelectedActivity != null;
        }


        private void UpdateActivityRecord(object obj)
        {
            CurrentActivityRecord.Activity = SelectedActivity;
            _activityRecordsRepository.Update(CurrentActivityRecord);

            ActivityRecords.Remove(CurrentActivityRecord);

            if (ActivityRecords.Count > 0)
            {
                CurrentActivityRecord = ActivityRecords.First();
            }
            else
            {
                TryClose();

                if (RecordsUpdated != null)
                {
                    RecordsUpdated(this, EventArgs.Empty);
                }
            }
        }

        #endregion
    }
}