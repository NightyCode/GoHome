namespace MottoBeneApps.GoHome.DataModels
{
    #region Namespace Imports

    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    #endregion


    public sealed class ActivityRecord
    {
        #region Constants and Fields

        private long _duration;

        #endregion


        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ActivityRecord()
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ActivityRecord(DateTime startTime, DateTime endTime, bool idle)
        {
            StartTime = startTime;
            EndTime = endTime;
            Idle = idle;
        }

        #endregion


        #region Properties

        public TimeSpan Duration
        {
            get
            {
                return EndTime - StartTime;
            }
        }

        [Column("Duration")]
        public long DurationTicks
        {
            get
            {
                long duration = (EndTime - StartTime).Ticks;

                if (_duration != duration)
                {
                    DurationTicks = duration;
                }

                return _duration;
            }

            private set
            {
                _duration = value;
            }
        }

        public DateTime EndTime
        {
            get;
            set;
        }

        public int Id
        {
            get;
            private set;
        }

        public bool Idle
        {
            get;
            set;
        }

        public DateTime StartTime
        {
            get;
            set;
        }

        #endregion
    }
}