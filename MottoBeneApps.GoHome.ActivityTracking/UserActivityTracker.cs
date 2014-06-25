namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System;
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using Microsoft.Win32;

    using MottoBeneApps.GoHome.ActivityTracking.Properties;
    using MottoBeneApps.GoHome.DataModels;

    #endregion


    [Export(typeof(IUserActivityTracker))]
    internal sealed class UserActivityTracker : IUserActivityTracker
    {
        #region Constants and Fields

        private readonly ILog _log = LogManager.GetLog(typeof(UserActivityTracker));

        private readonly IUserActivityStateRepository _stateRepository;
        private DateTime _inputSequenceStartTime = DateTime.MinValue;
        private DateTime _lastUserInputTime = DateTime.MinValue;

        #endregion


        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        [ImportingConstructor]
        public UserActivityTracker(IUserActivityStateRepository stateRepository)
        {
            _stateRepository = stateRepository;
        }

        #endregion


        #region Public Methods

        public void Start()
        {
            _log.Info("User activity tracking started.");

            SystemEvents.PowerModeChanged += OnPowerModeChanged;
            SystemEvents.SessionSwitch += OnSessionSwitch;
            SystemEvents.SessionEnding += OnSessionEnding;

            UserInputTracker.UserInputDetected += OnUserInput;
            UserInputTracker.Start();
        }


        public void Stop()
        {
            _log.Info("User activity tracking stopped.");

            SystemEvents.PowerModeChanged -= OnPowerModeChanged;
            SystemEvents.SessionSwitch -= OnSessionSwitch;
            SystemEvents.SessionEnding -= OnSessionEnding;

            UserInputTracker.Stop();
            UserInputTracker.UserInputDetected -= OnUserInput;
        }

        #endregion


        #region Methods

        private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            _log.Info("Power mode changed to '{0}'", e.Mode);

            switch (e.Mode)
            {
                case PowerModes.Resume:
                    break;
                case PowerModes.Suspend:
                    break;
            }
        }


        private void OnSessionEnding(object sender, SessionEndingEventArgs e)
        {
            _log.Info("Session ending with reason: '{0}'", e.Reason);

            switch (e.Reason)
            {
                case SessionEndReasons.Logoff:
                    break;
                case SessionEndReasons.SystemShutdown:
                    break;
            }
        }


        private void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            _log.Info("Session switched to: '{0}'", e.Reason);

            switch (e.Reason)
            {
                case SessionSwitchReason.SessionLock:
                    break;
                case SessionSwitchReason.SessionLogoff:
                    break;
                case SessionSwitchReason.SessionLogon:
                    break;
                case SessionSwitchReason.SessionUnlock:
                    break;
            }
        }


        private void OnUserInput(object sender, EventArgs eventArgs)
        {
            DateTime currentTime = DateTime.Now;

            if (_lastUserInputTime == DateTime.MinValue)
            {
                _inputSequenceStartTime = currentTime;
                _lastUserInputTime = currentTime;

                return;
            }

            TimeSpan idleTime = currentTime - _lastUserInputTime;

            if (idleTime.TotalMilliseconds > Settings.Default.IdleThreshold)
            {
                _log.Info("Added non idle activity period: {0} {1}.", _inputSequenceStartTime, _lastUserInputTime);
                _stateRepository.Add(new UserActivityState(_inputSequenceStartTime, _lastUserInputTime, false));

                _log.Info("Added idle activity period: {0} {1}.", _lastUserInputTime, currentTime);
                _stateRepository.Add(new UserActivityState(_lastUserInputTime, currentTime, true));

                _inputSequenceStartTime = currentTime;
            }

            _lastUserInputTime = currentTime;
        }

        #endregion
    }
}