namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MottoBeneApps.GoHome.DataModels;

    #endregion


    public class ActivityLogViewModel
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ActivityLogViewModel(string name, int duration)
            : this(name, null)
        {
            Duration = duration;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ActivityLogViewModel(string name, IEnumerable<ActivityRecord> records)
        {
            Name = name;

            if (records == null)
            {
                Records = new List<ActivityRecord>();
            }
            else
            {
                Records = records.ToList();

                var ticks = Records.Sum(r => r.DurationTicks);
                var totalDuration = TimeSpan.FromTicks(ticks);
                Duration = (int)Math.Round(totalDuration.TotalMinutes);
            }
        }

        #endregion


        #region Properties

        public int Duration
        {
            get;
            private set;
        }

        public bool HasRecords
        {
            get
            {
                return Records.Any();
            }
        }

        public string Name
        {
            get;
            private set;
        }

        public IEnumerable<ActivityRecord> Records
        {
            get;
            private set;
        }

        #endregion
    }
}