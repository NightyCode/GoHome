namespace MottoBeneApps.GoHome
{
    #region Namespace Imports

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Windows;

    using Caliburn.Micro;

    using Gemini.Framework.Services;
    using Gemini.Modules.Shell.Views;

    using MottoBeneApps.GoHome.Properties;
    using MottoBeneApps.GoHome.SystemTray;

    #endregion


    [Export(typeof(IShell))]
    public sealed class ShellViewModel : Gemini.Modules.Shell.ViewModels.ShellViewModel
    {
        #region Constants and Fields

        private WindowState _previousWindowState;
        private TaskbarIcon _taskbarIcon;
        private Window _window;

        #endregion


        #region Constructors and Destructors

        static ShellViewModel()
        {
            ViewLocator.AddNamespaceMapping(typeof(ShellViewModel).Namespace, typeof(ShellView).Namespace);
        }

        #endregion


        #region Properties

        private bool StartHiddenToTray
        {
            get
            {
                return Settings.Default.StartHiddenToTray;
            }

            set
            {
                if (StartHiddenToTray == value)
                {
                    return;
                }

                Settings.Default.StartHiddenToTray = value;
                Settings.Default.Save();
            }
        }

        #endregion


        #region Public Methods

        public override void CanClose(Action<bool> callback)
        {
            Coroutine.BeginExecute(CanClose().GetEnumerator(), null, (s, e) => callback(!e.WasCancelled));
        }

        #endregion


        #region Methods

        /// <summary>
        /// Called when an attached view's Loaded event fires.
        /// </summary>
        /// <param name="view"/>
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            _window = Window.GetWindow((DependencyObject)view);

            if (_window == null)
            {
                return;
            }

            _window.StateChanged += OnWindowStateChanged;
            _previousWindowState = _window.WindowState;

            Stream iconStream = IoC.Get<IResourceManager>()
                .GetStream("Resources/GoHome.ico", Assembly.GetExecutingAssembly().GetAssemblyName());

            _taskbarIcon = new TaskbarIcon { Icon = new Icon(iconStream) };

            _taskbarIcon.TrayMouseDoubleClick += OnTaskbarIconTrayMouseDoubleClick;

            if (Settings.Default.StartHiddenToTray)
            {
                _window.WindowState = WindowState.Minimized;
            }
        }


        private IEnumerable<IResult> CanClose()
        {
            yield return new MessageBoxResult();
        }


        private void OnTaskbarIconTrayMouseDoubleClick(object sender, EventArgs eventArgs)
        {
            if (_window.IsVisible)
            {
                return;
            }

            _window.Show();
            _window.WindowState = _previousWindowState;
            _taskbarIcon.IsVisible = false;
            StartHiddenToTray = false;
        }


        private void OnWindowStateChanged(object sender, EventArgs eventArgs)
        {
            var window = (Window)sender;

            if (window.WindowState == WindowState.Minimized)
            {
                _taskbarIcon.IsVisible = true;
                window.Hide();
                StartHiddenToTray = true;
            }
            else
            {
                _previousWindowState = _window.WindowState;
            }
        }

        #endregion


        private sealed class MessageBoxResult : IResult
        {
            #region Events

            public event EventHandler<ResultCompletionEventArgs> Completed;

            #endregion


            #region Public Methods

            public void Execute(ActionExecutionContext context)
            {
                System.Windows.MessageBoxResult result = MessageBox.Show(
                    "Are you sure you want to exit?",
                    "Confirm",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning,
                    System.Windows.MessageBoxResult.No);

                Completed(
                    this,
                    new ResultCompletionEventArgs { WasCancelled = (result != System.Windows.MessageBoxResult.Yes) });
            }

            #endregion
        }
    }
}