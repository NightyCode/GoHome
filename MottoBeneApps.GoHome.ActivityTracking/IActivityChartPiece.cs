namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System.Collections.Generic;

    using MottoBeneApps.GoHome.DataModels;

    #endregion


    public interface IActivityChartPiece
    {
        #region Properties

        string ActivityName
        {
            get;
        }

        bool HasRecords
        {
            get;
        }

        IEnumerable<ActivityRecord> Records
        {
            get;
        }

        int TotalDuration
        {
            get;
        }

        int WorkdayPercent
        {
            get;
        }

        #endregion
    }
}