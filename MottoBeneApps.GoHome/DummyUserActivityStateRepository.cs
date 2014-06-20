namespace MottoBeneApps.GoHome
{
    #region Namespace Imports

    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    using MottoBeneApps.GoHome.DataModels;

    #endregion


    [Export(typeof(IUserActivityStateRepository))]
    public sealed class DummyUserActivityStateRepository : IUserActivityStateRepository
    {
        #region Constants and Fields

        private readonly List<UserActivityState> _states = new List<UserActivityState>();

        #endregion


        #region Public Methods

        public void Add(UserActivityState state)
        {
            _states.Add(state);
        }


        public IEnumerable<UserActivityState> GetStates()
        {
            return _states;
        }


        public void Update(UserActivityState state)
        {
        }

        #endregion
    }
}