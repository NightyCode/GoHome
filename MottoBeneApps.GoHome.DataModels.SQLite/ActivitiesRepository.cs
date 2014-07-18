namespace MottoBeneApps.GoHome.DataModels.SQLite
{
    #region Namespace Imports

    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    #endregion


    [Export(typeof(IActivitiesRepository))]
    public sealed class ActivitiesRepository : IActivitiesRepository
    {
        #region Public Methods

        public IEnumerable<Activity> GetActivities()
        {
            var entities = new UserActivityLogEntities();
            return entities.Activities.ToList();
        }

        #endregion
    }
}