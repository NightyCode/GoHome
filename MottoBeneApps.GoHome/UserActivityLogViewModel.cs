namespace MottoBeneApps.GoHome
{
    #region Namespace Imports

    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    using Gemini.Framework;

    #endregion


    [Export(typeof(IUserActivityLog))]
    public sealed class UserActivityLogViewModel : Document, IUserActivityLog
    {
        #region Constants and Fields

        private readonly IUserActivityTracker _activityTracker;

        #endregion


        #region Constructors and Destructors

        [ImportingConstructor]
        public UserActivityLogViewModel(IUserActivityTracker activityTracker)
        {
            _activityTracker = activityTracker;
            DisplayName = "Activity Log";
        }

        #endregion


        #region Properties

        public IEnumerable<UserActivityState> ActivityLog
        {
            get
            {
                return _activityTracker.ActivityLog;
            }
        }

        #endregion
    }
}