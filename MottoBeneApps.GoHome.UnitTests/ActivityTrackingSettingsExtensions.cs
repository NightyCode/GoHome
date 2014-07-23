namespace MottoBeneApps.GoHome.UnitTests
{
    #region Namespace Imports

    using System;

    using MottoBeneApps.GoHome.ActivityTracking;

    #endregion


    public static class ActivityTrackingSettingsExtensions
    {
        #region Public Methods

        public static TimeSpan GetShortActivityDuration(this IActivityTrackingSettings settings)
        {
            return settings.MinimumActivityDuration - TimeSpan.FromMilliseconds(2);
        }


        public static TimeSpan GetShortIdleDuration(this IActivityTrackingSettings settings)
        {
            return settings.MinimumIdleDuration - TimeSpan.FromMilliseconds(4);
        }

        #endregion
    }
}