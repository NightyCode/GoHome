namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System;
    using System.Collections.Generic;

    using MottoBeneApps.GoHome.DataModels;

    #endregion


    public interface IUnknownActivityNotificationPopupViewModel
    {
        #region Events

        event EventHandler AllRecordsUpdated;
        event EventHandler<ActivityRecordEventArgs> RecordUpdated;

        #endregion


        #region Properties

        IEnumerable<ActivityRecord> ActivityRecords
        {
            get;
        }

        #endregion


        #region Public Methods

        void AddRecord(ActivityRecord record);

        #endregion
    }
}