namespace MottoBeneApps.GoHome.DataModels.SQLite
{
    #region Namespace Imports

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;

    #endregion


    [Export(typeof(IActivityRecordsRepository))]
    public sealed class ActivityRecordsRepository : IActivityRecordsRepository
    {
        #region Public Methods

        public void Add(ActivityRecord activityRecord)
        {
            using (var entities = new UserActivityLogEntities())
            {
                if (activityRecord.Activity != null)
                {
                    activityRecord.Activity =
                        entities.Activities.Single(a => a.ActivityId == activityRecord.Activity.ActivityId);
                }

                var record = entities.ActivityRecords.Add(activityRecord);
                entities.SaveChanges();
                activityRecord.ActivityRecordId = record.ActivityRecordId;
            }
        }


        public IEnumerable<ActivityRecord> GetActivityLog(DateTime date)
        {
            using (var entities = new UserActivityLogEntities())
            {
                return GetActivityLog(entities, date).ToList();
            }
        }


        public IEnumerable<ActivityRecord> GetActivityLog(DateTime date, Expression<Func<ActivityRecord, bool>> filter)
        {
            using (var entities = new UserActivityLogEntities())
            {
                return GetActivityLog(entities, date).Where(filter).ToList();
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
                return entities.ActivityRecords.Include(r => r.Activity).Select(Clone).ToList();
            }
        }


        public TimeSpan GetRemainingWorkTime(TimeSpan workDayDuration)
        {
            using (var entities = new UserActivityLogEntities())
            {
                IEnumerable<ActivityRecord> activityLog =
                    GetActivityLog(entities, DateTime.Now).Where(r => r.Activity.IsWork);
                long duration = activityLog.Sum(r => r.DurationTicks);
                return workDayDuration - TimeSpan.FromTicks(duration);
            }
        }


        public IEnumerable<ActivityRecord> GetUnknownActivityRecords()
        {
            using (var entities = new UserActivityLogEntities())
            {
                return
                    entities.ActivityRecords.Include(r => r.Activity)
                        .Where(r => r.Activity == null)
                        .Select(Clone)
                        .ToList();
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

                if (activityRecord.Activity != null)
                {
                    existingState.Activity =
                        entities.Activities.Single(a => a.ActivityId == activityRecord.Activity.ActivityId);
                }

                entities.SaveChanges();
            }
        }

        #endregion


        #region Methods

        private static ActivityRecord Clone(ActivityRecord record)
        {
            return new ActivityRecord(record.ActivityRecordId)
            {
                Activity = Clone(record.Activity),
                EndTime = record.EndTime,
                Idle = record.Idle,
                StartTime = record.StartTime,
                ActivityRecordId = record.ActivityRecordId
            };
        }


        private static Activity Clone(Activity activity)
        {
            if (activity == null)
            {
                return null;
            }

            var clone = new Activity(activity.ActivityId)
            {
                IsWork = activity.IsWork,
                Name = activity.Name,
                ParentActivityId = activity.ParentActivityId,
            };

            return clone;
        }


        private static IQueryable<ActivityRecord> GetActivityLog(UserActivityLogEntities entities, DateTime date)
        {
            date = date.Date;

            return entities.ActivityRecords.Include(r => r.Activity)
                .Where(s => s.StartTime >= date || s.EndTime >= date);
        }

        #endregion
    }
}