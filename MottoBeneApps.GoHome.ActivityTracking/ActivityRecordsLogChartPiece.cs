namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Caliburn.Micro;

    using MottoBeneApps.GoHome.DataModels;

    #endregion


    internal sealed class ActivityRecordsLogChartPiece : PropertyChangedBase, IActivityChartPiece
    {
        #region Constants and Fields

        private readonly Activity _activity;

        #endregion


        #region Constructors and Destructors

        /// <summary>
        /// Creates an instance of <see cref="T:Caliburn.Micro.PropertyChangedBase"/>.
        /// </summary>
        public ActivityRecordsLogChartPiece(
            Activity activity,
            IEnumerable<ActivityRecord> records,
            TimeSpan workdayDuration)
        {
            _activity = activity;
            Records = records.ToList();

            if (Records == null || !Records.Any())
            {
                return;
            }

            TotalDuration = (int)Math.Round(TimeSpan.FromTicks(Records.Sum(r => r.DurationTicks)).TotalMinutes);
            var totalWorkdayMinutes = (int)Math.Round(workdayDuration.TotalMinutes);
            WorkdayPercent = (int)(TotalDuration / (totalWorkdayMinutes / 100d));
        }

        #endregion


        #region Properties

        public string ActivityName
        {
            get
            {
                return _activity == null ? "Undefined" : _activity.Name;
            }
        }

        public bool HasRecords
        {
            get
            {
                return Records != null && Records.Any();
            }
        }

        public IEnumerable<ActivityRecord> Records
        {
            get;
            private set;
        }

        public int TotalDuration
        {
            get;
            private set;
        }

        public int WorkdayPercent
        {
            get;
            set;
        }

        #endregion
    }
}