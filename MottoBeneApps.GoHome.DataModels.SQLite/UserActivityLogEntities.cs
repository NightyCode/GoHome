namespace MottoBeneApps.GoHome.DataModels.SQLite
{
    #region Namespace Imports

    using System.Data.Entity;

    #endregion


    public class UserActivityLogEntities : DbContext
    {
        #region Constructors and Destructors

        public UserActivityLogEntities()
            : base("name=UserActivityLogEntities")
        {
        }

        #endregion


        #region Properties

        public virtual DbSet<UserActivityState> ActivityStates
        {
            get;
            set;
        }

        #endregion
    }
}