namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System;

    #endregion


    public interface IUserActivityTracker
    {
        #region Properties

        TimeSpan ActiveTime
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

        #endregion
    }
}