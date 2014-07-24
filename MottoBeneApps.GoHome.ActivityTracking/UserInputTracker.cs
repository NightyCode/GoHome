namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    #endregion


    [Export(typeof(IUserInputTracker))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal sealed class UserInputTracker : IUserInputTracker
    {
        #region Constants and Fields

        private const int _keyboardHookId = 13;
        private const int _mouseHookId = 14;
        private HookCallback _keyboardEventCallback;
        private int _keyboardHookHandle;
        private HookCallback _mouseEventCallback;
        private int _mouseHookHandle;

        #endregion


        #region Events

        /// <summary>
        /// Occurs when a keyboard input is registered.
        /// </summary>
        public event EventHandler<UserInputEventArgs> UserInputDetected;

        #endregion


        #region Properties

        public bool IsTracking
        {
            get;
            private set;
        }

        #endregion


        #region Public Methods

        public void Start()
        {
            if (IsTracking)
            {
                return;
            }

            StartTrackingKeyboardEvents();
            StartTrackingMouseEvents();

            IsTracking = true;
        }


        public void Stop()
        {
            if (!IsTracking)
            {
                return;
            }

            StopTrackingKeyboardEvents();
            StopTrackingMouseEvents();

            IsTracking = false;
        }

        #endregion


        #region Methods

        private int OnKeyboardInputEvent(int nCode, Int32 wParam, IntPtr lParam)
        {
            OnUserInputDetected();

            return NativeMethods.CallNextHookEx(_keyboardHookHandle, nCode, wParam, lParam);
        }


        private int OnMouseInputEvent(int nCode, int wParam, IntPtr lParam)
        {
            OnUserInputDetected();

            return NativeMethods.CallNextHookEx(_mouseHookHandle, nCode, wParam, lParam);
        }


        private void OnUserInputDetected()
        {
            EventHandler<UserInputEventArgs> onUserInputDetected = UserInputDetected;

            if (onUserInputDetected == null)
            {
                return;
            }

            var eventArgs = new UserInputEventArgs();
            Task.Run(() => onUserInputDetected(this, eventArgs));
        }


        private void StartTrackingKeyboardEvents()
        {
            if (_keyboardHookHandle != 0)
            {
                return;
            }

            _keyboardEventCallback = OnKeyboardInputEvent;

            _keyboardHookHandle = NativeMethods.SetWindowsHookEx(
                _keyboardHookId,
                _keyboardEventCallback,
                IntPtr.Zero,
                0);

            if (_keyboardHookHandle != 0)
            {
                return;
            }

            throw new Win32Exception(Marshal.GetLastWin32Error());
        }


        private void StartTrackingMouseEvents()
        {
            if (_mouseHookHandle != 0)
            {
                return;
            }

            _mouseEventCallback = OnMouseInputEvent;

            _mouseHookHandle = NativeMethods.SetWindowsHookEx(_mouseHookId, _mouseEventCallback, IntPtr.Zero, 0);

            if (_mouseHookHandle != 0)
            {
                return;
            }

            throw new Win32Exception(Marshal.GetLastWin32Error());
        }


        private void StopTrackingKeyboardEvents()
        {
            if (_keyboardHookHandle == 0)
            {
                return;
            }

            int result = NativeMethods.UnhookWindowsHookEx(_keyboardHookHandle);

            _keyboardHookHandle = 0;
            _keyboardEventCallback = null;

            if (result != 0)
            {
                return;
            }

            throw new Win32Exception(Marshal.GetLastWin32Error());
        }


        private void StopTrackingMouseEvents()
        {
            if (_mouseHookHandle == 0)
            {
                return;
            }

            int result = NativeMethods.UnhookWindowsHookEx(_mouseHookHandle);

            _mouseHookHandle = 0;
            _mouseEventCallback = null;

            if (result != 0)
            {
                return;
            }

            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        #endregion
    }
}