namespace MottoBeneApps.GoHome.DataModels
{
    #region Namespace Imports

    using System.Collections.Generic;

    #endregion


    public interface IActivitiesRepository
    {
        #region Public Methods

        IEnumerable<Activity> GetActivities();

        #endregion
    }
}