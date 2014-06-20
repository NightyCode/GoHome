namespace MottoBeneApps.GoHome
{
    #region Namespace Imports

    using System;

    #endregion


    public sealed class UserActivityState
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public UserActivityState(DateTime startTime, DateTime endTime, bool idle)
        {
            StartTime = startTime;
            EndTime = endTime;
            Idle = idle;
        }

        #endregion


        #region Properties

        public TimeSpan Duration
        {
            get
            {
                return EndTime - StartTime;
            }
        }

        public DateTime EndTime
        {
            get;
            private set;
        }

        public bool Idle
        {
            get;
            private set;
        }

        public DateTime StartTime
        {
            get;
            private set;
        }

        #endregion
    }
}