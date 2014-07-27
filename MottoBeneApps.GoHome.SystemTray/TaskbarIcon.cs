namespace MottoBeneApps.GoHome.SystemTray
{
    #region Namespace Imports

    using System;
    using System.Drawing;
    using System.Windows;
    using System.Windows.Controls.Primitives;

    using Caliburn.Micro;

    using Action = System.Action;

    #endregion


    public class TaskbarIcon : PropertyChangedBase
    {
        #region Constants and Fields

        private readonly Hardcodet.Wpf.TaskbarNotification.TaskbarIcon _icon;
        private object _popup;

        #endregion


        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public TaskbarIcon()
        {
            _icon = new Hardcodet.Wpf.TaskbarNotification.TaskbarIcon { Visibility = Visibility.Hidden };

            _icon.TrayMouseDoubleClick += OnTrayMouseDoubleClick;
            _icon.TrayLeftMouseDown += OnTrayLeftMouseDown;
            _icon.TrayLeftMouseUp += OnTrayLeftMouseUp;
            _icon.TrayMiddleMouseDown += OnTrayMiddleMouseDown;
            _icon.TrayMiddleMouseUp += OnTrayMiddleMouseUp;
            _icon.TrayRightMouseDown += OnTrayRightMouseDown;
            _icon.TrayRightMouseUp += OnTrayRightMouseUp;

            _icon.TrayPopupOpen += OnTrayPopupOpen;
        }

        #endregion


        #region Events

        public event EventHandler TrayLeftMouseDown;
        public event EventHandler TrayLeftMouseUp;
        public event EventHandler TrayMiddleMouseDown;
        public event EventHandler TrayMiddleMouseUp;
        public event EventHandler TrayMouseDoubleClick;
        public event EventHandler TrayRightMouseDown;
        public event EventHandler TrayRightMouseUp;

        #endregion


        #region Properties

        public Icon Icon
        {
            get
            {
                return Invoke(() => _icon.Icon);
            }

            set
            {
                Invoke(
                    () =>
                    {
                        if (Equals(_icon.Icon, value))
                        {
                            return;
                        }

                        _icon.Icon = value;

                        NotifyOfPropertyChange(() => Icon);
                    });
            }
        }

        public bool IsVisible
        {
            get
            {
                return Invoke(() => _icon.Visibility == Visibility.Visible);
            }

            set
            {
                Invoke(
                    () =>
                    {
                        var visibility = value ? Visibility.Visible : Visibility.Hidden;

                        if (Equals(_icon.Visibility, visibility))
                        {
                            return;
                        }

                        _icon.Visibility = visibility;

                        NotifyOfPropertyChange(() => IsVisible);
                    });
            }
        }

        public object Popup
        {
            get
            {
                return _popup;
            }
            set
            {
                if (Equals(value, _popup))
                {
                    return;
                }

                _popup = value;

                UpdateTrayPopup();

                NotifyOfPropertyChange(() => Popup);
            }
        }


        public PopupActivationMode PopupActivation
        {
            get
            {
                return
                    Invoke(
                        () =>
                            ConvertEnum<Hardcodet.Wpf.TaskbarNotification.PopupActivationMode, PopupActivationMode>(
                                _icon.PopupActivation));
            }

            set
            {
                Invoke(
                    () =>
                    {
                        var popupActivationMode =
                            ConvertEnum<PopupActivationMode, Hardcodet.Wpf.TaskbarNotification.PopupActivationMode>(
                                value);

                        if (Equals(_icon.PopupActivation, popupActivationMode))
                        {
                            return;
                        }

                        _icon.PopupActivation = popupActivationMode;

                        NotifyOfPropertyChange(() => PopupActivation);
                    });
            }
        }

        public string ToolTipText
        {
            get
            {
                return Invoke(() => _icon.ToolTipText);
            }
            set
            {
                Invoke(
                    () =>
                    {
                        if (Equals(_icon.ToolTipText, value))
                        {
                            return;
                        }

                        _icon.ToolTipText = value;
                        NotifyOfPropertyChange(() => ToolTipText);
                    });
            }
        }

        #endregion


        #region Public Methods

        public void ShowBalloonTip(string title, string mesasge, BalloonIcon icon)
        {
            _icon.ShowBalloonTip(
                title,
                mesasge,
                ConvertEnum<BalloonIcon, Hardcodet.Wpf.TaskbarNotification.BalloonIcon>(icon));
        }


        public void ShowBalloonTip(string title, string mesasge, Icon icon)
        {
            _icon.ShowBalloonTip(title, mesasge, icon);
        }

        #endregion


        #region Methods

        private TDestinationEnum ConvertEnum<TSourceEnum, TDestinationEnum>(TSourceEnum sourceValue)
        {
            return (TDestinationEnum)Enum.Parse(typeof(TDestinationEnum), sourceValue.ToString());
        }


        private void Invoke(Action action)
        {
            if (_icon.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                _icon.Dispatcher.Invoke(action);
            }
        }


        private TResult Invoke<TResult>(Func<TResult> action)
        {
            return _icon.Dispatcher.CheckAccess() ? action() : _icon.Dispatcher.Invoke(action);
        }


        private void OnTrayLeftMouseDown(object sender, RoutedEventArgs routedEventArgs)
        {
            if (TrayLeftMouseDown != null)
            {
                TrayLeftMouseDown(this, EventArgs.Empty);
            }
        }


        private void OnTrayLeftMouseUp(object sender, RoutedEventArgs routedEventArgs)
        {
            if (TrayLeftMouseUp != null)
            {
                TrayLeftMouseUp(this, EventArgs.Empty);
            }
        }


        private void OnTrayMiddleMouseDown(object sender, RoutedEventArgs routedEventArgs)
        {
            if (TrayMiddleMouseDown != null)
            {
                TrayMiddleMouseDown(this, EventArgs.Empty);
            }
        }


        private void OnTrayMiddleMouseUp(object sender, RoutedEventArgs routedEventArgs)
        {
            if (TrayMiddleMouseUp != null)
            {
                TrayMiddleMouseUp(this, EventArgs.Empty);
            }
        }


        private void OnTrayMouseDoubleClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (TrayMouseDoubleClick != null)
            {
                TrayMouseDoubleClick(this, EventArgs.Empty);
            }
        }


        private void OnTrayPopupOpen(object sender, RoutedEventArgs routedEventArgs)
        {
            Popup popup = _icon.TrayPopupResolved;

            var popupViewModel = _popup;
            UIElement popupView = _icon.TrayPopup;

            popup.SetValue(View.IsGeneratedProperty, true);
            ViewModelBinder.Bind(popupViewModel, popup, null);
            Caliburn.Micro.Action.SetTargetWithoutContext(popupView, popupViewModel);

            var activate = popupViewModel as IActivate;
            if (activate != null)
            {
                activate.Activate();
            }

            var deactivator = popupViewModel as IDeactivate;
            if (deactivator != null)
            {
                popup.Closed += (EventHandler)((param0, param1) => deactivator.Deactivate(true));
            }

            var trayPopup = popupViewModel as TrayPopup;

            if (trayPopup != null)
            {
                trayPopup.ParentPopup = popup;
            }
        }


        private void OnTrayRightMouseDown(object sender, RoutedEventArgs routedEventArgs)
        {
            if (TrayRightMouseDown != null)
            {
                TrayRightMouseDown(this, EventArgs.Empty);
            }
        }


        private void OnTrayRightMouseUp(object sender, RoutedEventArgs routedEventArgs)
        {
            if (TrayRightMouseUp != null)
            {
                TrayRightMouseUp(this, EventArgs.Empty);
            }
        }


        private void UpdateTrayPopup()
        {
            Invoke(
                () =>
                {
                    UIElement popupView = ViewLocator.LocateForModel(_popup, null, null);
                    _icon.TrayPopup = popupView;
                });
        }

        #endregion
    }
}