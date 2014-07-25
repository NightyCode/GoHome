namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System;
    using System.Collections.Generic;

    using MottoBeneApps.GoHome.DataModels;

    #endregion


    internal sealed class ActivityChartPiece : IActivityChartPiece
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ActivityChartPiece(string activityName, TimeSpan totalDuration, TimeSpan workdayDuration)
        {
            ActivityName = activityName;
            TotalDuration = (int)Math.Round(totalDuration.TotalMinutes);
            var totalWorkdayMinutes = (int)Math.Round(workdayDuration.TotalMinutes);
            WorkdayPercent = (int)(TotalDuration / (totalWorkdayMinutes / 100d));
        }

        #endregion


        #region Properties

        public string ActivityName
        {
            get;
            private set;
        }

        public bool HasRecords
        {
            get
            {
                return false;
            }
        }

        public IEnumerable<ActivityRecord> Records
        {
            get
            {
                return null;
            }
        }

        public int TotalDuration
        {
            get;
            private set;
        }

        public int WorkdayPercent
        {
            get;
            private set;
        }

        #endregion
    }
}