namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Timers;
    using System.Windows;

    using Caliburn.Micro;

    using Gemini.Framework;
    using Gemini.Framework.Results;
    using Gemini.Framework.Services;
    using Gemini.Modules.MainMenu.Models;

    using MottoBeneApps.GoHome.ActivityTracking.Properties;
    using MottoBeneApps.GoHome.DataModels;

    #endregion


    [Export(typeof(IModule))]
    internal sealed class Module : ModuleBase
    {
        #region Constants and Fields

        private Timer _workDayEndTimer;

        #endregion


        #region Properties

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

            Task.Run(() => CheckRemainingWorkTime());
        }

        #endregion


        #region Methods

        private bool CanOpenDashboard()
        {
            return !IoC.Get<IShell>().Documents.Any(d => d is IDashboard);
        }


        private void CheckRemainingWorkTime()
        {
            var recordsRepository = IoC.Get<IActivityRecordsRepository>();

            TimeSpan remainingWorkTime =
                recordsRepository.GetRemainingWorkTime(TimeSpan.FromMinutes(Settings.Default.WorkDayDuration));

            remainingWorkTime = remainingWorkTime - UserActivityTracker.ActiveTime;

            if (remainingWorkTime.TotalMilliseconds > 0)
            {
                _workDayEndTimer = new Timer { AutoReset = false, Interval = remainingWorkTime.TotalMilliseconds };
                _workDayEndTimer.Elapsed += OnWorkDayEndTimerElapsed;
                _workDayEndTimer.Start();
            }
            else
            {
                MessageBox.Show(
                    "It's time to go home! Good job!",
                    "Go home",
                    MessageBoxButton.OK,
                    MessageBoxImage.Asterisk,
                    MessageBoxResult.OK);
            }
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


        private void OnWorkDayEndTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            CheckRemainingWorkTime();
        }

        #endregion
    }
}