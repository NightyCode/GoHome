namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System;

    using MottoBeneApps.GoHome.DataModels;

    #endregion


    public class ActivityRecordEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.EventArgs"/> class.
        /// </summary>
        public ActivityRecordEventArgs(ActivityRecord activityRecord)
        {
            ActivityRecord = activityRecord;
        }

        #endregion


        #region Properties

        public ActivityRecord ActivityRecord
        {
            get;
            private set;
        }

        #endregion
    }
}