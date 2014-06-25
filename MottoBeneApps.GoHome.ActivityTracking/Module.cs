namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using Gemini.Framework;
    using Gemini.Framework.Results;
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