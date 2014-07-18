namespace MottoBeneApps.GoHome.DataModels.SQLite
{
    #region Namespace Imports

    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Data.Entity;
    using System.Linq;

    #endregion


    [Export(typeof(IActivitiesRepository))]
    public sealed class ActivitiesRepository : IActivitiesRepository
    {
        #region Public Methods

        public IEnumerable<Activity> GetActivities()
        {
            using (var entities = new UserActivityLogEntities())
            {
                return entities.Activities.Include(a => a.Parent).ToList();
            }
        }

        #endregion
    }
}