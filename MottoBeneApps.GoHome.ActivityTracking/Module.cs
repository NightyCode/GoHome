namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Caliburn.Micro;

    using Gemini.Framework;
    using Gemini.Framework.Results;
    using Gemini.Framework.Services;
    using Gemini.Modules.MainMenu.Models;

    #endregion


    [Export(typeof(IModule))]
    internal sealed class Module : ModuleBase
    {
        #region Properties

        [Import(typeof(INotificationManager))]
        public INotificationManager NotificationManager
        {
            get;
            private set;
        }

        [Import(typeof(IUserActivityTracker))]
        public IUserActivityTracker UserActivityTracker
        {
            get;
            private set;
        }

        #endregion


        #region Public Methods

        public override void Initialize()
        {
            base.Initialize();

            UserActivityTracker.Start();

            MainMenu.Find(KnownMenuItemNames.View)
                .Add(new MenuItem("Dashboard", OnOpenDashboardMenuItemClick, CanOpenDashboard));

            MainMenu.Find(KnownMenuItemNames.View).Add(new MenuItem("Activity Log", OnOpenActivityLogMenuItemClick));

            var shell = IoC.Get<IShell>() as IDeactivate;

            if (shell != null)
            {
                shell.Deactivated += OnShellDeactivated;
            }

            NotificationManager.CheckUnknownActivityRecords();
        }

        #endregion


        #region Methods

        private bool CanOpenDashboard()
        {
            return !IoC.Get<IShell>().Documents.Any(d => d is IDashboard);
        }

        private IEnumerable<IResult> OnOpenActivityLogMenuItemClick()
        {
            yield return Show.Document<IUserActivityLog>();
        }


        private IEnumerable<IResult> OnOpenDashboardMenuItemClick()
        {
            yield return Show.Document<IDashboard>();
        }


        private void OnShellDeactivated(object sender, DeactivationEventArgs e)
        {
            if (!e.WasClosed)
            {
                return;
            }

            UserActivityTracker.Stop();
        }

        #endregion
    }
}