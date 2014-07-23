namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System;

    #endregion


    public interface IActivityTrackingSettings
    {
        #region Properties

        TimeSpan MinimumActivityDuration
        {
            get;
        }

        TimeSpan MinimumIdleDuration
        {
            get;
        }

        TimeSpan WorkDayDuration
        {
            get;
        }

        #endregion
    }
}