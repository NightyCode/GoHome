namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Caliburn.Micro;

    using Microsoft.Win32;

    using MottoBeneApps.GoHome.DataModels;

    using Action = System.Action;

    #endregion


    [Export(typeof(IUserActivityTracker))]
    public sealed class UserActivityTracker : IUserActivityTracker
    {
        #region Constants and Fields

        private static readonly object _activityLoggingSyncRoot = new object();
        private static readonly object _activityTrackingSyncRoot = new object();

        private readonly IActivityRecordsRepository _activityRecordsRepository;
        private readonly Activity _breakActivity;
        private readonly Activity _defaultActivity;
        private readonly Activity _homeActivity;
        private readonly IUserInputTracker _inputTracker;
        private readonly ILog _log = LogManager.GetLog(typeof(UserActivityTracker));
        private readonly IActivityTrackingSettings _settings;
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
            IActivitiesRepository activitiesRepository,
            IActivityTrackingSettings settings,
            IUserInputTracker inputTracker)
        {
            _activityRecordsRepository = activityRecordsRepository;
            _settings = settings;
            _inputTracker = inputTracker;

            List<Activity> activities = activitiesRepository.GetActivities().ToList();
            _defaultActivity = activities[0];
            _breakActivity = activities[1];
            _homeActivity = activities[5];
        }

        #endregion


        #region Events

        public event EventHandler ActivityLogUpdated;
        public event EventHandler<ActivityRecordEventArgs> UnknownActivityLogged;

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

                _inputTracker.UserInputDetected += OnUserInput;
                _inputTracker.Start();

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

                _inputTracker.Stop();
                _inputTracker.UserInputDetected -= OnUserInput;

                LogUserActivity(false, true, DateTime.Now);

                _lastUserInputTime = DateTime.MinValue;
                _userInputStartTime = DateTime.MinValue;

                IsTracking = false;
            }
        }


        public void UpdateUserActivityLog()
        {
            LogUserActivity(false, true, DateTime.Now);
        }

        #endregion


        #region Methods

        internal void LogUserActivity(bool updateActivityTimeStamps, bool forceLog, DateTime currentTime)
        {
            lock (_activityLoggingSyncRoot)
            {
                DateTime lastUserInputTime;
                DateTime userInputStartTime;
                TimeSpan idleTime;

                lock (_activityTrackingSyncRoot)
                {
                    lastUserInputTime = _lastUserInputTime;
                    userInputStartTime = _userInputStartTime;

                    if (_lastUserInputTime == DateTime.MinValue)
                    {
                        idleTime = TimeSpan.Zero;
                    }
                    else
                    {
                        idleTime = currentTime - _lastUserInputTime;
                    }

                    bool longIdleTime = idleTime >= _settings.MinimumIdleDuration;

                    if (updateActivityTimeStamps)
                    {
                        if (_lastUserInputTime == DateTime.MinValue)
                        {
                            _lastUserInputTime = currentTime;
                            _userInputStartTime = currentTime;
                        }
                        else if (longIdleTime)
                        {
                            _lastUserInputTime = currentTime;
                            _userInputStartTime = currentTime;
                        }
                        else
                        {
                            _lastUserInputTime = currentTime;
                            lastUserInputTime = currentTime;
                            idleTime = TimeSpan.Zero;
                        }
                    }

                    if (longIdleTime)
                    {
                        forceLog = true;

                        if (!updateActivityTimeStamps)
                        {
                            _lastUserInputTime = DateTime.MinValue;
                            _userInputStartTime = DateTime.MinValue;
                        }
                    }
                }

                if (lastUserInputTime == DateTime.MinValue)
                {
                    if (forceLog)
                    {
                        lastUserInputTime = currentTime;
                        userInputStartTime = currentTime;
                    }
                    else
                    {
                        return;
                    }
                }

                if (!forceLog && idleTime < _settings.MinimumIdleDuration)
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
                    _settings,
                    OnActivityLogUpdated,
                    OnUnknownActivityLogged);
            }
        }


        private static void LogUserActivity(
            IActivityRecordsRepository activityRecordsRepository,
            Action<ActivityRecord> setIdleRecordActivity,
            Activity defaultActivity,
            DateTime userInputStartTime,
            DateTime lastUserInputTime,
            DateTime currentTime,
            IActivityTrackingSettings settings,
            Action onActivityLogUpdated,
            Action<ActivityRecord> onUnknownActivityLogged)
        {
            var activeTime = lastUserInputTime - userInputStartTime;
            var idleTime = currentTime - lastUserInputTime;

            bool logUpdated = false;

            Action<ActivityRecord, Action<ActivityRecord>> addOrUpdateRecord = (record, addOrUpdate) =>
            {
                if (record.Idle)
                {
                    setIdleRecordActivity(record);
                }

                addOrUpdate(record);

                if (record.Idle && record.Activity == null)
                {
                    onUnknownActivityLogged(record);
                }

                logUpdated = true;
            };

            Action<ActivityRecord> addRecord = record => addOrUpdateRecord(record, activityRecordsRepository.Add);
            Action<ActivityRecord> updateRecord = record => addOrUpdateRecord(record, activityRecordsRepository.Update);

            try
            {
                ActivityRecord lastRecord = activityRecordsRepository.GetLastRecord();

                if (lastRecord != null)
                {
                    TimeSpan idleTimeAfterLastRecord = userInputStartTime - lastRecord.EndTime;

                    if (lastRecord.Idle)
                    {
                        // Cannot log this yet. Not enough data.
                        if (activeTime < settings.MinimumActivityDuration && idleTime < settings.MinimumIdleDuration)
                        {
                            return;
                        }

                        if (activeTime < settings.MinimumActivityDuration)
                        {
                            lastRecord.EndTime = currentTime;
                            updateRecord(lastRecord);

                            return;
                        }

                        if (idleTimeAfterLastRecord.TotalMilliseconds > 0)
                        {
                            lastRecord.EndTime = userInputStartTime;
                            updateRecord(lastRecord);
                        }
                    }
                    else
                    {
                        if (idleTimeAfterLastRecord >= settings.MinimumIdleDuration)
                        {
                            var newIdleRecord = new ActivityRecord
                            {
                                StartTime = lastRecord.EndTime,
                                EndTime = userInputStartTime,
                                Idle = true,
                            };

                            addRecord(newIdleRecord);
                        }
                        else
                        {
                            lastRecord.EndTime = lastUserInputTime;
                            updateRecord(lastRecord);
                            activeTime = TimeSpan.Zero;
                        }
                    }
                }

                if (activeTime < settings.MinimumActivityDuration && idleTime < settings.MinimumIdleDuration)
                {
                    return;
                }

                if (activeTime >= settings.MinimumActivityDuration)
                {
                    addRecord(
                        new ActivityRecord
                        {
                            StartTime = userInputStartTime,
                            EndTime = lastUserInputTime,
                            Idle = false,
                            Activity = defaultActivity
                        });
                }
                else if (activeTime != TimeSpan.Zero)
                {
                    idleTime += activeTime;
                    lastUserInputTime = userInputStartTime;
                }

                if (idleTime < settings.MinimumIdleDuration)
                {
                    return;
                }

                var idleRecord = new ActivityRecord
                {
                    StartTime = lastUserInputTime,
                    EndTime = currentTime,
                    Idle = true,
                    Activity = defaultActivity
                };

                addRecord(idleRecord);
            }
            finally
            {
                if (logUpdated)
                {
                    onActivityLogUpdated();
                }
            }
        }


        private void OnActivityLogUpdated()
        {
            if (ActivityLogUpdated != null)
            {
                ActivityLogUpdated(this, EventArgs.Empty);
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
                    _inputTracker.Stop();
                    LogUserActivity(false, true, DateTime.Now);
                    break;

                case SessionSwitchReason.SessionUnlock:
                    _inputTracker.Start();
                    break;

                case SessionSwitchReason.SessionLogoff:
                    Stop();
                    break;

                case SessionSwitchReason.SessionLogon:
                    Start();
                    break;
            }
        }


        private void OnUnknownActivityLogged(ActivityRecord idleActivity)
        {
            if (!idleActivity.Idle)
            {
                _log.Warn("Unknown activity notification posted for non idle activity.");

                return;
            }

            if (UnknownActivityLogged != null)
            {
                UnknownActivityLogged(this, new ActivityRecordEventArgs(idleActivity));
            }
        }


        private void OnUserInput(object sender, UserInputEventArgs eventArgs)
        {
            LogUserActivity(true, false, eventArgs.TimeStamp);
        }


        private void SetIdleRecordActivity(ActivityRecord activityRecord)
        {
            if (activityRecord.StartTime.Date != activityRecord.EndTime.Date)
            {
                activityRecord.Activity = _homeActivity;
            }
            else if (activityRecord.Duration < _settings.MaximumBreakDuration)
            {
                activityRecord.Activity = _breakActivity;
            }
            else
            {
                activityRecord.Activity = null;
            }
        }

        #endregion
    }
}