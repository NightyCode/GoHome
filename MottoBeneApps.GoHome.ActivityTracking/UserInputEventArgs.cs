namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System;

    #endregion


    public class UserInputEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.EventArgs"/> class.
        /// </summary>
        public UserInputEventArgs()
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.EventArgs"/> class.
        /// </summary>
        public UserInputEventArgs(DateTime timeStamp)
        {
            TimeStamp = timeStamp;
        }

        #endregion


        #region Properties

        public DateTime TimeStamp
        {
            get;
            private set;
        }

        #endregion
    }
}