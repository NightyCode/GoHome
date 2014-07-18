namespace MottoBeneApps.GoHome.DataModels
{
    public class Activity
    {
        #region Properties

        public int ActivityId
        {
            get;
            private set;
        }

        public bool IsWork
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public virtual Activity Parent
        {
            get;
            set;
        }

        public int? ParentActivityId
        {
            get;
            private set;
        }

        #endregion
    }
}