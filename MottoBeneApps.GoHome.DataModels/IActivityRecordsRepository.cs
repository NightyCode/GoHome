namespace MottoBeneApps.GoHome.DataModels
{
    #region Namespace Imports

    using System;
    using System.Collections.Generic;

    #endregion


    public interface IActivityRecordsRepository
    {
        #region Public Methods

        void Add(ActivityRecord activityRecord);
        IEnumerable<ActivityRecord> GetActivityLog(DateTime date, TimeSpan minimumDuration);
        ActivityRecord GetLastRecord();
        IEnumerable<ActivityRecord> GetRecords();
        void Update(ActivityRecord activityRecord);

        #endregion
    }
}