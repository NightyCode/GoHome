namespace MottoBeneApps.GoHome.ActivityTracking
{
    public interface IUserActivityTracker
    {
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