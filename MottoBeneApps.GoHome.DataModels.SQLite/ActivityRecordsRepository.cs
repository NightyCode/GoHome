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
            using (var entities = new UserActivityLogEntities())
            {
                entities.ActivityRecords.Add(activityRecord);
                entities.SaveChanges();
            }
        }


        public IEnumerable<ActivityRecord> GetActivityLog(DateTime date, TimeSpan minimumDuration)
        {
            using (var entities = new UserActivityLogEntities())
            {
                long minimumDurationTicks = minimumDuration.Ticks;

                return
                    entities.ActivityRecords.Where(
                        s => (s.StartTime <= date || s.EndTime <= date) && s.DurationTicks >= minimumDurationTicks).ToList();
            }
        }


        public ActivityRecord GetLastRecord()
        {
            using (var entities = new UserActivityLogEntities())
            {
                return entities.ActivityRecords.OrderByDescending(s => s.EndTime).Take(1).ToList().FirstOrDefault();
            }
        }


        public IEnumerable<ActivityRecord> GetRecords()
        {
            using (var entities = new UserActivityLogEntities())
            {
                return entities.ActivityRecords.ToList();
            }
        }


        public void Update(ActivityRecord activityRecord)
        {
            using (var entities = new UserActivityLogEntities())
            {
                var existingState = entities.ActivityRecords.Single(s => s.ActivityRecordId == activityRecord.ActivityRecordId);
                existingState.StartTime = activityRecord.StartTime;
                existingState.EndTime = activityRecord.EndTime;
                existingState.Idle = activityRecord.Idle;

                entities.SaveChanges();
            }
        }

        #endregion
    }
}