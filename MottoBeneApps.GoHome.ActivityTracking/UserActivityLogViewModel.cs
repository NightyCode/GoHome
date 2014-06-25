namespace MottoBeneApps.GoHome.ActivityTracking
{
    #region Namespace Imports

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;

    using Gemini.Framework;

    using MottoBeneApps.GoHome.DataModels;

    #endregion


    [Export(typeof(IUserActivityLog))]
    internal sealed class UserActivityLogViewModel : Document, IUserActivityLog
    {
        #region Constants and Fields

        private readonly IUserActivityStateRepository _stateRepository;
        private readonly ObservableCollection<UserActivityState> _states = new ObservableCollection<UserActivityState>();

        #endregion


        #region Constructors and Destructors

        [ImportingConstructor]
        public UserActivityLogViewModel(IUserActivityStateRepository stateRepository)
        {
            _stateRepository = stateRepository;
            DisplayName = "Activity Log";
        }

        #endregion


        #region Properties

        public IEnumerable<UserActivityState> ActivityLog
        {
            get
            {
                return _states;
            }
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

            foreach (var state in _stateRepository.GetStates())
            {
                _states.Add(state);
            }
        }

        #endregion
    }
}