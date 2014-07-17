namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using Gemini.Framework;
    using Gemini.Framework.Results;
    using Gemini.Framework.Services;
    using Gemini.Modules.MainMenu.Models;

    #endregion


    [Export(typeof(IModule))]
    internal sealed class Module : ModuleBase
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

            UserActivityTracker.Start();

            MainMenu.Find(KnownMenuItemNames.View).Add(new MenuItem("Activity Log", OpenActivityLogView));

            var shell = IoC.Get<IShell>() as IDeactivate;

            if (shell != null)
            {
                shell.Deactivated += OnShellDeactivated;
            }
        }


        private void OnShellDeactivated(object sender, DeactivationEventArgs e)
        {
            if (!e.WasClosed)
            {
                return;
            }

            UserActivityTracker.Stop();
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