namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System.Drawing;
    using System.IO;
    using System.Reflection;

    using Caliburn.Micro;

    using Gemini.Framework.Services;

    using MottoBeneApps.GoHome.DataModels;
    using MottoBeneApps.GoHome.SystemTray;

    #endregion


    internal sealed class UnknownActivityNotification : TaskbarIcon
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public UnknownActivityNotification()
        {
            Stream iconStream = IoC.Get<IResourceManager>()
                .GetStream(
                    "Resources/UnknownActivityNotification.ico",
                    Assembly.GetExecutingAssembly().GetAssemblyName());

            Icon = new Icon(iconStream);
        }

        #endregion


        #region Public Methods

        public void Show(ActivityRecord idleActivity)
        {

        }

        #endregion
    }
}