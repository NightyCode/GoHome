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
        private readonly ObservableCollection<ActivityRecord> _states = new ObservableCollection<ActivityRecord>();

        #endregion


        #region Constructors and Destructors

        [ImportingConstructor]
        public UserActivityLogViewModel(IActivityRecordsRepository activityRecordsRepository)
        {
            _activityRecordsRepository = activityRecordsRepository;
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

            foreach (var state in _activityRecordsRepository.GetActivityLog(DateTime.Now, TimeSpan.FromHours(4)))
            {
                _states.Add(state);
            }
        }

        #endregion
    }
}