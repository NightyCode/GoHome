namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Timers;
    using System.Windows;

    using Caliburn.Micro;

    using Gemini.Framework.Services;

    using MottoBeneApps.GoHome.ActivityTracking.Properties;
    using MottoBeneApps.GoHome.DataModels;
    using MottoBeneApps.GoHome.SystemTray;

    #endregion


    [Export(typeof(INotificationManager))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal sealed class NotificationManager : INotificationManager
    {
        #region Constants and Fields

        private readonly IActivityRecordsRepository _activityRecordsRepository;
        private readonly TaskbarIcon _unknownActivityNotification;
        private readonly IUnknownActivityNotificationPopupViewModel _unknownActivityNotificationViewModel;
        private readonly IUserActivityTracker _userActivityTracker;
        private readonly TaskbarIcon _workdayEndedNotification;
        private Timer _workDayEndTimer;

        #endregion


        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        [ImportingConstructor]
        public NotificationManager(
            IActivityRecordsRepository activityRecordsRepository,
            IUserActivityTracker userActivityTracker,
            IUnknownActivityNotificationPopupViewModel unknownActivityNotificationPopupViewModel)
        {
            _activityRecordsRepository = activityRecordsRepository;
            _userActivityTracker = userActivityTracker;

            _unknownActivityNotificationViewModel = unknownActivityNotificationPopupViewModel;
            _unknownActivityNotificationViewModel.RecordsUpdated += OnUnknownActivityRecordsUpdated;

            Stream iconStream = IoC.Get<IResourceManager>()
                .GetStream(
                    "Resources/UnknownActivityNotification.ico",
                    Assembly.GetExecutingAssembly().GetAssemblyName());

            _unknownActivityNotification = new TaskbarIcon
            {
                Icon = new Icon(iconStream),
                Popup = _unknownActivityNotificationViewModel,
                PopupActivation = PopupActivationMode.All
            };

            iconStream = IoC.Get<IResourceManager>()
                .GetStream("Resources/GoHome.ico", Assembly.GetExecutingAssembly().GetAssemblyName());

            _workdayEndedNotification = new TaskbarIcon { Icon = new Icon(iconStream) };

            _userActivityTracker.UnknownActivityLogged += OnUnknownActivityLogged;

            Task.Run(() => CheckRemainingWorkTime());
        }

        #endregion


        #region Public Methods

        public void CkeckMissedNotifications()
        {
            Task.Run(
                () =>
                {
                    _userActivityTracker.UpdateUserActivityLog();

                    IEnumerable<ActivityRecord> unknownActivityRecords =
                        _activityRecordsRepository.GetUnknownActivityRecords();

                    foreach (var record in unknownActivityRecords)
                    {
                        _unknownActivityNotificationViewModel.ActivityRecords.Add(record);
                    }

                    if (_unknownActivityNotificationViewModel.ActivityRecords.Count > 0)
                    {
                        _unknownActivityNotification.IsVisible = true;
                    }
                });
        }

        #endregion


        #region Methods

        private void CheckRemainingWorkTime()
        {
            TimeSpan remainingWorkTime =
                _activityRecordsRepository.GetRemainingWorkTime(TimeSpan.FromMinutes(Settings.Default.WorkDayDuration));

            remainingWorkTime = remainingWorkTime - _userActivityTracker.ActiveTime;

            if (remainingWorkTime.TotalMilliseconds > 0)
            {
                _workDayEndTimer = new Timer { AutoReset = false, Interval = remainingWorkTime.TotalMilliseconds };
                _workDayEndTimer.Elapsed += OnWorkDayEndTimerElapsed;
                _workDayEndTimer.Start();
            }
            else
            {
                _workdayEndedNotification.IsVisible = true;
            }
        }


        private void OnUnknownActivityLogged(object sender, ActivityRecordEventArgs e)
        {
            Application.Current.Dispatcher.InvokeAsync(
                () =>
                {
                    _unknownActivityNotificationViewModel.ActivityRecords.Add(e.ActivityRecord);
                    _unknownActivityNotification.IsVisible = true;
                });
        }


        private void OnUnknownActivityRecordsUpdated(object sender, EventArgs eventArgs)
        {
            _unknownActivityNotification.IsVisible = false;
        }


        private void OnWorkDayEndTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            CheckRemainingWorkTime();
        }

        #endregion
    }
}