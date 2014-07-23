namespace MottoBeneApps.GoHome.UnitTests
{
    #region Namespace Imports

    using System;

    using FakeItEasy;

    using MottoBeneApps.GoHome.ActivityTracking;

    #endregion


    internal static class UserInputTrackerExtensions
    {
        #region Public Methods

        public static void RaiseUserInputDetectedEvent(
            this IUserInputTracker userInputTracker,
            ref DateTime timeStamp,
            TimeSpan delay)
        {
            timeStamp += delay;

            RaiseUserInputDetectedEvent(userInputTracker, timeStamp);
        }


        public static void RaiseUserInputDetectedEvent(this IUserInputTracker userInputTracker, DateTime timeStamp)
        {
            userInputTracker.UserInputDetected += Raise.With(new UserInputEventArgs(timeStamp)).Now;
        }

        #endregion
    }
}