namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System;

    using Gemini.Framework;

    #endregion


    public interface IDashboard : IDocument
    {
        #region Properties

        DateTime WorkdayStartTime
        {
            get;
        }

        #endregion
    }
}