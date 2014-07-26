namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System;

    #endregion


    public interface IUserActivityTracker
    {
        #region Events

        event EventHandler ActivityLogUpdated;
        event EventHandler<ActivityRecordEventArgs> UnknownActivityLogged;

        #endregion


        #region Properties

        TimeSpan ActiveTime
        {
            get;
        }

        TimeSpan IdleTime
        {
            get;
        }

        bool IsTracking
        {
            get;
        }

        #endregion


        #region Public Methods

        void Start();
        void Stop();
        void UpdateUserActivityLog();

        #endregion
    }
}