namespace MottoBeneApps.GoHome
{
    #region Namespace Imports

    using System;
    using System.Runtime.InteropServices;

    #endregion


    internal delegate int HookCallback(int nCode, int wParam, IntPtr lParam);


    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern bool GetLastInputInfo(ref LastInputInfo lastInputInfo);


        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);


        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall,
            SetLastError = true)]
        public static extern int SetWindowsHookEx(int idHook, HookCallback lpfn, IntPtr hMod, int dwThreadId);


        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall,
            SetLastError = true)]
        public static extern int UnhookWindowsHookEx(int idHook);
    }
}