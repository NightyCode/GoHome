namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System;
    using System.Collections.ObjectModel;

    using MottoBeneApps.GoHome.DataModels;

    #endregion


    public interface IUnknownActivityNotificationPopupViewModel
    {
        #region Events

        event EventHandler RecordsUpdated;

        #endregion


        #region Properties

        ObservableCollection<ActivityRecord> ActivityRecords
        {
            get;
        }

        #endregion
    }
}