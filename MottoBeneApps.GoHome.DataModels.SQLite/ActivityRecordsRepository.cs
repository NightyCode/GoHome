namespace MottoBeneApps.GoHome.DataModels.SQLite
{
    #region Namespace Imports

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Data.Entity;
    using System.Linq;

    #endregion


    [Export(typeof(IActivityRecordsRepository))]
    public sealed class ActivityRecordsRepository : IActivityRecordsRepository
    {
        #region Public Methods

        public void Add(ActivityRecord activityRecord)
        {
            using (var entities = new UserActivityLogEntities())
            {
                entities.ActivityRecords.Add(activityRecord);
                entities.SaveChanges();
            }
        }


        public IEnumerable<ActivityRecord> GetActivityLog(DateTime date)
        {
            using (var entities = new UserActivityLogEntities())
            {
                return
                    entities.ActivityRecords.Include(r => r.Activity)
                        .Where(s => s.StartTime <= date || s.EndTime <= date)
                        .ToList();
            }
        }


        public ActivityRecord GetLastRecord()
        {
            using (var entities = new UserActivityLogEntities())
            {
                return
                    entities.ActivityRecords.Include(r => r.Activity)
                        .OrderByDescending(s => s.EndTime)
                        .Take(1)
                        .ToList()
                        .FirstOrDefault();
            }
        }


        public IEnumerable<ActivityRecord> GetRecords()
        {
            using (var entities = new UserActivityLogEntities())
            {
                return entities.ActivityRecords.Include(r => r.Activity).ToList();
            }
        }


        public void Update(ActivityRecord activityRecord)
        {
            using (var entities = new UserActivityLogEntities())
            {
                var existingState =
                    entities.ActivityRecords.Single(s => s.ActivityRecordId == activityRecord.ActivityRecordId);

                existingState.StartTime = activityRecord.StartTime;
                existingState.EndTime = activityRecord.EndTime;
                existingState.Idle = activityRecord.Idle;

                entities.SaveChanges();
            }
        }

        #endregion
    }
}