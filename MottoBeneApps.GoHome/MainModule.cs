namespace MottoBeneApps.GoHome
{
    #region Namespace Imports

    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Reflection;
    using System.Windows;

    using Caliburn.Micro;

    using Gemini.Framework;
    using Gemini.Framework.Results;
    using Gemini.Framework.Services;
    using Gemini.Modules.MainMenu.Models;

    #endregion


    [Export(typeof(IModule))]
    public sealed class MainModule : ModuleBase
    {
        #region Properties

        [Import(typeof(IUserActivityTracker))]
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

            UserActivityTracker.Start();

            MainMenu.Find(KnownMenuItemNames.View).Add(new MenuItem("Activity Log", OpenActivityLogView));
        }

        #endregion


        #region Methods

        private IEnumerable<IResult> OpenActivityLogView()
        {
            yield return Show.Document<IUserActivityLog>();
        }

        #endregion
    }
}