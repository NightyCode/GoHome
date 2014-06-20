namespace MottoBeneApps.GoHome
{
    #region Namespace Imports

    using System.ComponentModel.Composition;
    using System.Reflection;
    using System.Windows;

    using Caliburn.Micro;

    using Gemini.Framework;
    using Gemini.Framework.Services;

    #endregion


    [Export(typeof(IModule))]
    public sealed class MainModule : ModuleBase
    {
        #region Properties

        [Export(typeof(IUserActivityTracker))]
        public IUserActivityTracker UserActivityTracker
        {
            get;
            private set;
        }

        #endregion


        #region Public Methods

        public override void Initialize()
        {
            base.Initialize();

            MainWindow.WindowState = WindowState.Maximized;
            MainWindow.Title = "Go Home";
            MainWindow.Icon = IoC.Get<IResourceManager>()
                .GetBitmap("Resources/GoHome.png", Assembly.GetExecutingAssembly().GetAssemblyName());

            UserActivityTracker = new UserActivityTracker();
            UserActivityTracker.Start();
        }

        #endregion
    }
}