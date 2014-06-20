namespace MottoBeneApps.GoHome
{
    #region Namespace Imports

    using System.Collections.Generic;

    #endregion


    public interface IUserActivityTracker
    {
        #region Properties

        IEnumerable<UserActivityState> ActivityLog
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