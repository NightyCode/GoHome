namespace MottoBeneApps.GoHome.DataModels.SQLite
{
    #region Namespace Imports

    using System;
    using System.Data.Entity;

    #endregion


    internal sealed class UserActivityLogEntities : DbContext
    {
        #region Constructors and Destructors

        public UserActivityLogEntities()
            : base("name=UserActivityLogEntities")
        {
            AppDomain.CurrentDomain.SetData(
                "DataDirectory",
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
        }

        #endregion


        #region Properties

        public DbSet<Activity> Activities
        {
            get;
            set;
        }

        public DbSet<ActivityRecord> ActivityRecords
        {
            get;
            set;
        }

        #endregion
    }
}