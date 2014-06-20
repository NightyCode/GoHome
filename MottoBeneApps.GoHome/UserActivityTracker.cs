namespace MottoBeneApps.GoHome
{
    #region Namespace Imports

    using System;
    using System.ComponentModel.Composition;

    using Microsoft.Win32;

    using MottoBeneApps.GoHome.DataModels;
    using MottoBeneApps.GoHome.Properties;

    #endregion


    [Export(typeof(IUserActivityTracker))]
    internal sealed class UserActivityTracker : IUserActivityTracker
    {
        #region Constants and Fields

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
            SystemEvents.PowerModeChanged += OnPowerModeChanged;
            SystemEvents.SessionSwitch += OnSessionSwitch;
            SystemEvents.SessionEnding += OnSessionEnding;

            UserInputTracker.UserInputDetected += OnUserInput;
            UserInputTracker.Start();
        }


        public void Stop()
        {
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
                _stateRepository.Add(new UserActivityState(_inputSequenceStartTime, _lastUserInputTime, false));
                _stateRepository.Add(new UserActivityState(_lastUserInputTime, currentTime, true));

                _inputSequenceStartTime = currentTime;
            }

            _lastUserInputTime = currentTime;
        }

        #endregion
    }
}