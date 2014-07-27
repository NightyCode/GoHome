namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.Drawing;
    using System.IO;
    using System.Linq;
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
            _unknownActivityNotificationViewModel.AllRecordsUpdated += OnUnknownActivityAllRecordsUpdated;
            _unknownActivityNotificationViewModel.RecordUpdated += OnUnknownActivityRecordUpdated;

            Stream iconStream = IoC.Get<IResourceManager>()
                .GetStream(
                    "Resources/UnknownActivityNotification.ico",
                    Assembly.GetExecutingAssembly().GetAssemblyName());

            _unknownActivityNotification = new TaskbarIcon
            {
                Icon = new Icon(iconStream),
                Popup = _unknownActivityNotificationViewModel,
                PopupActivation = PopupActivationMode.All,
                ToolTipText = "You've been missing too long. What've you been up to?"
            };

            iconStream = IoC.Get<IResourceManager>()
                .GetStream("Resources/GoHome.ico", Assembly.GetExecutingAssembly().GetAssemblyName());

            _workdayEndedNotification = new TaskbarIcon
            {
                Icon = new Icon(iconStream),
                ToolTipText = "You can go home now!"
            };

            _userActivityTracker.UnknownActivityLogged += OnUnknownActivityLogged;

            Task.Run(() => CheckRemainingWorkTime());
        }

        #endregion


        #region Events

        public event EventHandler<ActivityRecordEventArgs> UnknownActivityRecordUpdated;

        #endregion


        #region Public Methods

        public void CheckUnknownActivityRecords()
        {
            Task.Run(
                () =>
                {
                    _userActivityTracker.UpdateUserActivityLog();

                    IEnumerable<ActivityRecord> unknownActivityRecords =
                        _activityRecordsRepository.GetUnknownActivityRecords();

                    foreach (var record in unknownActivityRecords)
                    {
                        _unknownActivityNotificationViewModel.AddRecord(record);
                    }

                    if (_unknownActivityNotificationViewModel.ActivityRecords.Any())
                    {
                        ShowUnknownActivitiesNotification();
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
                _workdayEndedNotification.ShowBalloonTip(
                    "Go Home",
                    "Your workday is over! You can go home now.",
                    BalloonIcon.Info);
            }
        }


        private void OnUnknownActivityAllRecordsUpdated(object sender, EventArgs eventArgs)
        {
            _unknownActivityNotification.IsVisible = false;
        }


        private void OnUnknownActivityLogged(object sender, ActivityRecordEventArgs e)
        {
            Application.Current.Dispatcher.InvokeAsync(
                () =>
                {
                    _unknownActivityNotificationViewModel.AddRecord(e.ActivityRecord);
                    ShowUnknownActivitiesNotification();
                });
        }


        private void OnUnknownActivityRecordUpdated(object sender, ActivityRecordEventArgs e)
        {
            if (UnknownActivityRecordUpdated != null)
            {
                UnknownActivityRecordUpdated(this, e);
            }
        }


        private void OnWorkDayEndTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            CheckRemainingWorkTime();
        }


        private void ShowUnknownActivitiesNotification()
        {
            _unknownActivityNotification.IsVisible = true;
            _unknownActivityNotification.ShowBalloonTip(
                "You've been missing too long",
                "What've you been up to?",
                BalloonIcon.Info);
        }

        #endregion
    }
}