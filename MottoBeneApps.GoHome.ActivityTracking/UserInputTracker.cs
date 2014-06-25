namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    #endregion


    internal static class UserInputTracker
    {
        #region Constants and Fields

        private const int _keyboardHookId = 13;
        private const int _mouseHookId = 14;
        private static HookCallback _keyboardEventCallback;
        private static int _keyboardHookHandle;
        private static HookCallback _mouseEventCallback;
        private static int _mouseHookHandle;

        #endregion


        #region Events

        /// <summary>
        /// Occurs when a keyboard input is registered.
        /// </summary>
        public static event EventHandler UserInputDetected;

        #endregion


        #region Public Methods

        public static void Start()
        {
            StartTrackingKeyboardEvents();
            StartTrackingMouseEvents();
        }


        public static void Stop()
        {
            StopTrackingKeyboardEvents();
            StopTrackingMouseEvents();
        }

        #endregion


        #region Methods

        private static int OnKeyboardInputEvent(int nCode, Int32 wParam, IntPtr lParam)
        {
            if (UserInputDetected != null)
            {
                UserInputDetected(null, EventArgs.Empty);
            }

            return NativeMethods.CallNextHookEx(_keyboardHookHandle, nCode, wParam, lParam);
        }


        private static int OnMouseInputEvent(int nCode, int wParam, IntPtr lParam)
        {
            if (UserInputDetected != null)
            {
                UserInputDetected(null, EventArgs.Empty);
            }

            return NativeMethods.CallNextHookEx(_mouseHookHandle, nCode, wParam, lParam);
        }


        private static void StartTrackingKeyboardEvents()
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


        private static void StartTrackingMouseEvents()
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


        private static void StopTrackingKeyboardEvents()
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


        private static void StopTrackingMouseEvents()
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