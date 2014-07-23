namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System;

    #endregion


    public interface IUserInputTracker
    {
        #region Events

        /// <summary>
        /// Occurs when a keyboard input is registered.
        /// </summary>
        event EventHandler<UserInputEventArgs> UserInputDetected;

        #endregion


        #region Properties

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