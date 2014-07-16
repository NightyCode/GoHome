namespace MottoBeneApps.GoHome
{
    #region Namespace Imports

    using System.ComponentModel.Composition;
    using System.Reflection;

    using Caliburn.Micro;

    using Gemini.Modules.Settings;

    using Microsoft.Win32;

    #endregion


    [Export(typeof(ISettingsEditor))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class ApplicationSettingsViewModel : PropertyChangedBase, ISettingsEditor
    {
        #region Constants and Fields

        private readonly RegistryKey _registryKey =
            Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        private bool _startWhenWindowsStarts;

        #endregion


        #region Constructors and Destructors

        /// <summary>
        /// Creates an instance of <see cref="T:Caliburn.Micro.PropertyChangedBase"/>.
        /// </summary>
        public ApplicationSettingsViewModel()
        {
            _startWhenWindowsStarts = StartWhenWindowsStartsKeyValue;
        }

        #endregion


        #region Properties

        public string SettingsPageName
        {
            get
            {
                return "General";
            }
        }

        public string SettingsPagePath
        {
            get
            {
                return "Environment";
            }
        }


        public bool StartWhenWindowsStarts
        {
            get
            {
                return _startWhenWindowsStarts;
            }

            set
            {
                if (value.Equals(_startWhenWindowsStarts))
                {
                    return;
                }
                _startWhenWindowsStarts = value;
                NotifyOfPropertyChange(() => StartWhenWindowsStarts);
            }
        }

        private static string AppName
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Name;
            }
        }


        private bool StartWhenWindowsStartsKeyValue
        {
            get
            {
                return _registryKey.GetValue(AppName) != null;
            }
        }

        #endregion


        #region Public Methods

        public void ApplyChanges()
        {
            if (StartWhenWindowsStarts != StartWhenWindowsStartsKeyValue)
            {
                if (StartWhenWindowsStarts)
                {
                    _registryKey.SetValue(AppName, Assembly.GetExecutingAssembly().Location);
                }
                else
                {
                    _registryKey.DeleteValue(AppName, false);
                }
            }
        }

        #endregion
    }
}