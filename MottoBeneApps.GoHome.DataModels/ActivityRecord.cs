namespace MottoBeneApps.GoHome.DataModels
{
    #region Namespace Imports

    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    #endregion


    public class ActivityRecord
    {
        #region Constants and Fields

        private long _duration;

        #endregion


        #region Properties

        public virtual Activity Activity
        {
            get;
            set;
        }

        public int? ActivityId
        {
            get;
            private set;
        }

        public int ActivityRecordId
        {
            get;
            private set;
        }


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