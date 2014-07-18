namespace MottoBeneApps.GoHome.DataModels.SQLite
{
    #region Namespace Imports

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    #endregion


    [Export(typeof(IActivityRecordsRepository))]
    public sealed class ActivityRecordsRepository : IActivityRecordsRepository
    {
        #region Public Methods

        public void Add(ActivityRecord activityRecord)
        {
            var entities = new UserActivityLogEntities();
            entities.ActivityRecords.Add(activityRecord);
            entities.SaveChanges();
        }


        public IEnumerable<ActivityRecord> GetActivityLog(DateTime date)
        {
            var entities = new UserActivityLogEntities();
            return entities.ActivityRecords.Where(s => s.StartTime <= date || s.EndTime <= date).ToList();
        }


        public ActivityRecord GetLastRecord()
        {
            var entities = new UserActivityLogEntities();
            return entities.ActivityRecords.OrderByDescending(s => s.EndTime).Take(1).ToList().FirstOrDefault();
        }


        public IEnumerable<ActivityRecord> GetRecords()
        {
            var entities = new UserActivityLogEntities();
            return entities.ActivityRecords.ToList();
        }


        public void Update(ActivityRecord activityRecord)
        {
            var entities = new UserActivityLogEntities();
            var existingState =
                entities.ActivityRecords.Single(s => s.ActivityRecordId == activityRecord.ActivityRecordId);
            existingState.StartTime = activityRecord.StartTime;
            existingState.EndTime = activityRecord.EndTime;
            existingState.Idle = activityRecord.Idle;

            entities.SaveChanges();
        }

        #endregion
    }
}