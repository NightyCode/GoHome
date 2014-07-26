namespace MottoBeneApps.GoHome.SystemTray
{
    #region Namespace Imports

    using System;
    using System.Windows.Controls.Primitives;

    using Caliburn.Micro;

    #endregion


    public class TrayPopup : Screen
    {
        #region Properties

        internal Popup ParentPopup
        {
            get;
            set;
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Called to check whether or not this instance can close.
        /// </summary>
        /// <param name="callback">The implementor calls this action with the result of the close check.</param>
        public override sealed void CanClose(Action<bool> callback)
        {
            base.CanClose(callback);
        }


        public void Dismiss()
        {
            ParentPopup.IsOpen = false;
        }

        #endregion
    }
}