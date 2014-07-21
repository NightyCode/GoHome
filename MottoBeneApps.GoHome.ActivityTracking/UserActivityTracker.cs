namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Threading.Tasks;

    using Caliburn.Micro;

    using Microsoft.Win32;

    using MottoBeneApps.GoHome.ActivityTracking.Properties;
    using MottoBeneApps.GoHome.DataModels;

    #endregion


    [Export(typeof(IUserActivityTracker))]
    internal sealed class UserActivityTracker : IUserActivityTracker
    {
        #region Constants and Fields

        private static readonly object _activityLoggingSyncRoot = new object();
        private static readonly object _activityTrackingSyncRoot = new object();

        private readonly IActivityRecordsRepository _activityRecordsRepository;
        private readonly Activity _defaultActivity;
        private readonly Activity _defaultIdleActivity;
        private readonly Activity _homeActivity;
        private readonly ILog _log = LogManager.GetLog(typeof(UserActivityTracker));
        private DateTime _lastUserInputTime = DateTime.MinValue;
        private DateTime _userInputStartTime = DateTime.MinValue;

        #endregion


        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        [ImportingConstructor]
        public UserActivityTracker(
            IActivityRecordsRepository activityRecordsRepository,
            IActivitiesRepository activitiesRepository)
        {
            _activityRecordsRepository = activityRecordsRepository;

            List<Activity> activities = activitiesRepository.GetActivities().ToList();
            _defaultActivity = activities[0];
            _defaultIdleActivity = activities[1];
            _homeActivity = activities[5];
        }

        #endregion


        #region Properties

        public TimeSpan ActiveTime
        {
            get
            {
                return _lastUserInputTime - _userInputStartTime;
            }
        }

        public TimeSpan IdleTime
        {
            get
            {
                return DateTime.Now - _lastUserInputTime;
            }
        }


        public bool IsTracking
        {
            get;
            private set;
        }

        #endregion


        #region Public Methods

        public void Start()
        {
            lock (_activityLoggingSyncRoot)
            {
                if (IsTracking)
                {
                    return;
                }

                _log.Info("User activity tracking started.");

                SystemEvents.PowerModeChanged += OnPowerModeChanged;
                SystemEvents.SessionSwitch += OnSessionSwitch;
                SystemEvents.SessionEnded += OnSessionEnded;

                UserInputTracker.UserInputDetected += OnUserInput;
                UserInputTracker.Start();

                IsTracking = true;
            }
        }


        public void Stop()
        {
            lock (_activityLoggingSyncRoot)
            {
                if (!IsTracking)
                {
                    return;
                }

                _log.Info("User activity tracking stopped.");

                SystemEvents.PowerModeChanged -= OnPowerModeChanged;
                SystemEvents.SessionSwitch -= OnSessionSwitch;
                SystemEvents.SessionEnded -= OnSessionEnded;

                UserInputTracker.Stop();
                UserInputTracker.UserInputDetected -= OnUserInput;

                LogUserActivity(false, true);

                _lastUserInputTime = DateTime.MinValue;
                _userInputStartTime = DateTime.MinValue;

                IsTracking = false;
            }
        }


        public void UpdateUaserActivityLog()
        {
            LogUserActivity(false, true);
        }

        #endregion


        #region Methods

        private static void LogUserActivity(
            IActivityRecordsRepository activityRecordsRepository,
            Action<ActivityRecord> setIdleRecordActivity,
            Activity defaultActivity,
            DateTime userInputStartTime,
            DateTime lastUserInputTime,
            DateTime currentTime,
            ILog log)
        {
            var activeTime = lastUserInputTime - userInputStartTime;
            var idleTime = currentTime - lastUserInputTime;

            ActivityRecord lastRecord = activityRecordsRepository.GetLastRecord();

            TimeSpan timeSpan = userInputStartTime - lastRecord.EndTime;

            if (!lastRecord.Idle && timeSpan.TotalMilliseconds > 1
                && timeSpan.TotalMilliseconds < Settings.Default.IdleThreshold)
            {
                log.Info(
                    "The time between last activity record and current user activity start time is too small. Treating it as user activity time.");
                userInputStartTime = lastRecord.StartTime;
                activeTime = lastUserInputTime - lastRecord.StartTime;
            }

            if (activeTime.TotalMilliseconds < Settings.Default.ActiveThreshold)
            {
                log.Info("The registered active time is too small. Nothing to log yet.");
                return;
            }

            if (lastRecord.Idle)
            {
                if (lastRecord.EndTime != userInputStartTime)
                {
                    log.Info("Updating last idle record with new end time ({0}).", userInputStartTime);
                    lastRecord.EndTime = userInputStartTime;

                    setIdleRecordActivity(lastRecord);

                    activityRecordsRepository.Update(lastRecord);
                }

                log.Info("Adding new activity record ({0} - {1}).", userInputStartTime, lastUserInputTime);
                activityRecordsRepository.Add(
                    new ActivityRecord
                    {
                        StartTime = userInputStartTime,
                        EndTime = lastUserInputTime,
                        Idle = false,
                        Activity = defaultActivity
                    });
            }
            else
            {
                log.Info("Updating last activity record with new end time ({0}).", lastUserInputTime);
                lastRecord.EndTime = lastUserInputTime;

                activityRecordsRepository.Update(lastRecord);
            }

            if (idleTime.TotalMilliseconds < Settings.Default.IdleThreshold)
            {
                log.Info("Idle time is too small. Cannot log it yet.");
                return;
            }

            log.Info("Adding new idle record ({0} - {1}).", lastUserInputTime, currentTime);
            var idleRecord = new ActivityRecord
            {
                StartTime = lastUserInputTime,
                EndTime = currentTime,
                Idle = true,
                Activity = defaultActivity
            };

            setIdleRecordActivity(idleRecord);

            activityRecordsRepository.Add(idleRecord);
        }


        private void LogUserActivity(bool updateActivityTimeStamps, bool forceLog)
        {
            lock (_activityLoggingSyncRoot)
            {
                DateTime currentTime;
                DateTime lastUserInputTime;
                DateTime userInputStartTime;
                TimeSpan idleTime;

                lock (_activityTrackingSyncRoot)
                {
                    currentTime = DateTime.Now;

                    if (updateActivityTimeStamps)
                    {
                        if (_lastUserInputTime == DateTime.MinValue)
                        {
                            _lastUserInputTime = currentTime;
                            _userInputStartTime = currentTime;
                        }
                        else
                        {
                            _lastUserInputTime = currentTime;
                        }
                    }

                    lastUserInputTime = _lastUserInputTime;
                    userInputStartTime = _userInputStartTime;
                    idleTime = currentTime - lastUserInputTime;

                    if (updateActivityTimeStamps && idleTime.TotalMilliseconds >= Settings.Default.IdleThreshold)
                    {
                        _log.Info("The user was idle for too long. Resetting activity time stamps.");

                        _lastUserInputTime = currentTime;
                        _userInputStartTime = currentTime;
                    }
                }

                if (lastUserInputTime == DateTime.MinValue)
                {
                    _log.Info("No user activity registered since app start. Nothing to log yet.");
                    return;
                }

                if (!forceLog && idleTime.TotalMilliseconds < Settings.Default.IdleThreshold)
                {
                    return;
                }

                LogUserActivity(
                    _activityRecordsRepository,
                    SetIdleRecordActivity,
                    _defaultActivity,
                    userInputStartTime,
                    lastUserInputTime,
                    currentTime,
                    _log);
            }
        }


        private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            _log.Info("Power mode changed to '{0}'", e.Mode);
        }


        private void OnSessionEnded(object sender, SessionEndedEventArgs e)
        {
            _log.Info("Session ended with reason: '{0}'", e.Reason);

            Stop();
        }


        private void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            _log.Info("Session switched to: '{0}'", e.Reason);

            switch (e.Reason)
            {
                case SessionSwitchReason.SessionLock:
                    UserInputTracker.Stop();
                    LogUserActivity(false, true);
                    break;

                case SessionSwitchReason.SessionUnlock:
                    UserInputTracker.Start();
                    break;

                case SessionSwitchReason.SessionLogoff:
                    Stop();
                    break;

                case SessionSwitchReason.SessionLogon:
                    Start();
                    break;
            }
        }


        private void OnUserInput(object sender, EventArgs eventArgs)
        {
            Task.Run(() => LogUserActivity(true, false));
        }


        private void SetIdleRecordActivity(ActivityRecord activityRecord)
        {
            if (activityRecord.StartTime.Date != activityRecord.EndTime.Date)
            {
                activityRecord.Activity = _homeActivity;
            }
            else
            {
                activityRecord.Activity = _defaultIdleActivity;
            }

            // TODO request activity from user.
        }

        #endregion
    }
}