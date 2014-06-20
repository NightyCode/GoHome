namespace MottoBeneApps.GoHome.DataModels
{
    #region Namespace Imports

    using System.Collections.Generic;

    #endregion


    public interface IUserActivityStateRepository
    {
        #region Public Methods

        void Add(UserActivityState state);
        IEnumerable<UserActivityState> GetStates();
        void Update(UserActivityState state);

        #endregion
    }
}