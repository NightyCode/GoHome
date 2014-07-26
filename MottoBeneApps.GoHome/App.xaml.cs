namespace MottoBeneApps.GoHome
{
    #region Namespace Imports

    using System;
    using System.Threading.Tasks;
    using System.Windows.Threading;

    using Caliburn.Micro;

    #endregion


    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    internal sealed partial class App
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Application"/> class.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">More than one instance of the <see cref="T:System.Windows.Application"/> class is created per <see cref="T:System.AppDomain"/>.</exception>
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;

            DispatcherUnhandledException += OnDispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += OnTaskSchedulerUnobservedTaskException;
        }

        #endregion


        #region Methods

        private void LogException(Exception exception)
        {
            if (exception == null)
            {
                return;
            }

            LogManager.GetLog(GetType()).Error(exception);
        }


        private void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;

            LogException(exception);
        }


        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogException(e.Exception);
        }


        private void OnTaskSchedulerUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            LogException(e.Exception);

            foreach (var exception in e.Exception.InnerExceptions)
            {
                LogException(exception);
            }
        }

        #endregion
    }
}