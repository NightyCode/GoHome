namespace MottoBeneApps.GoHome.DataModels
{
    #region Namespace Imports

    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    #endregion


    public interface IActivityRecordsRepository
    {
        #region Public Methods

        void Add(ActivityRecord activityRecord);
        IEnumerable<ActivityRecord> GetActivityLog(DateTime date);
        IEnumerable<ActivityRecord> GetActivityLog(DateTime date, Expression<Func<ActivityRecord, bool>> filter);
        ActivityRecord GetLastRecord();
        TimeSpan GetRemainingWorkTime(TimeSpan workDayDuration);
        IEnumerable<ActivityRecord> GetUnknownActivityRecords();
        void Update(ActivityRecord activityRecord);

        #endregion
    }
}