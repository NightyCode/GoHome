namespace MottoBeneApps.GoHome
{
    #region Namespace Imports

    using System;
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using Gemini;

    using LogManager = NLog.LogManager;

    #endregion


    internal sealed class Bootstrapper : AppBootstrapper
    {
        #region Constants and Fields

        private static ILog _logger;

        #endregion


        #region Constructors and Destructors

        static Bootstrapper()
        {
            Caliburn.Micro.LogManager.GetLog = type => Logger;
        }

        #endregion


        #region Properties

        [Export]
        public static ILog Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = (ILog)LogManager.GetLogger(Guid.NewGuid().ToString("B"), typeof(Logger));
                }

                return _logger;
            }
        }

        #endregion
    }
}