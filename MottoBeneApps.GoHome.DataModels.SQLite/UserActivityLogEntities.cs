namespace MottoBeneApps.GoHome.DataModels.SQLite
{
    #region Namespace Imports

    using System.Data.Entity;

    #endregion


    internal sealed class UserActivityLogEntities : DbContext
    {
        #region Constructors and Destructors

        public UserActivityLogEntities()
            : base("name=UserActivityLogEntities")
        {
        }

        #endregion


        #region Properties

        public DbSet<UserActivityState> ActivityStates
        {
            get;
            set;
        }

        #endregion
    }
}