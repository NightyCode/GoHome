namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System;

    #endregion


    public interface INotificationManager
    {
        #region Events

        event EventHandler<ActivityRecordEventArgs> UnknownActivityRecordUpdated;

        #endregion


        #region Public Methods

        void CheckUnknownActivityRecords();

        #endregion
    }
}