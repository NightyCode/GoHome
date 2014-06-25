namespace MottoBeneApps.GoHome
{
    #region Namespace Imports

    using System;

    using Caliburn.Micro;

    using NLog;

    #endregion


    internal sealed class Logger : NLog.Logger, ILog
    {
        #region Methods

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        void ILog.Error(Exception exception)
        {
            Log(typeof(Logger), LogEventInfo.Create(LogLevel.Error, Name, "An exception occurred:", exception));
        }


        /// <summary>
        /// Logs the message as info.
        /// </summary>
        /// <param name="format">A formatted message.</param><param name="args">Parameters to be injected into the formatted message.</param>
        void ILog.Info(string format, params object[] args)
        {
            Log(typeof(Logger), LogEventInfo.Create(LogLevel.Info, Name, null, format, args));
        }


        /// <summary>
        /// Logs the message as a warning.
        /// </summary>
        /// <param name="format">A formatted message.</param><param name="args">Parameters to be injected into the formatted message.</param>
        void ILog.Warn(string format, params object[] args)
        {
            Log(typeof(Logger), LogEventInfo.Create(LogLevel.Warn, Name, null, format, args));
        }

        #endregion
    }
}