namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;

    using Gemini.Framework;

    using MottoBeneApps.GoHome.DataModels;

    #endregion


    [Export(typeof(IUserActivityLog))]
    internal sealed class UserActivityLogViewModel : Document, IUserActivityLog
    {
        #region Constants and Fields

        private readonly IActivityRecordsRepository _activityRecordsRepository;
        private readonly IUserActivityTracker _activityTracker;
        private readonly ObservableCollection<ActivityRecord> _states = new ObservableCollection<ActivityRecord>();

        #endregion


        #region Constructors and Destructors

        [ImportingConstructor]
        public UserActivityLogViewModel(
            IActivityRecordsRepository activityRecordsRepository,
            IUserActivityTracker activityTracker)
        {
            _activityRecordsRepository = activityRecordsRepository;
            _activityTracker = activityTracker;
            DisplayName = "Activity Log";
        }

        #endregion


        #region Properties

        public IEnumerable<ActivityRecord> ActivityLog
        {
            get
            {
                return _states;
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

            _activityTracker.UpdateUserActivityLog();

            _states.Clear();

            foreach (var state in _activityRecordsRepository.GetActivityLog(DateTime.Now))
            {
                _states.Add(state);
            }
        }

        #endregion
    }
}