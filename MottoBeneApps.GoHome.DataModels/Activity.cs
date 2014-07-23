namespace MottoBeneApps.GoHome.DataModels
{
    public class Activity
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Activity()
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Activity(int activityId)
        {
            ActivityId = activityId;
        }

        #endregion


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