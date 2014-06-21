namespace MottoBeneApps.GoHome.DataModels.SQLite
{
    #region Namespace Imports

    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    #endregion


    [Export(typeof(IUserActivityStateRepository))]
    public sealed class UserActivityStateRepository : IUserActivityStateRepository
    {
        #region Public Methods

        public void Add(UserActivityState state)
        {
            var entities = new UserActivityLogEntities();
            entities.ActivityStates.Add(state);
            entities.SaveChanges();
        }


        public IEnumerable<UserActivityState> GetStates()
        {
            var entities = new UserActivityLogEntities();
            return entities.ActivityStates;
        }


        public void Update(UserActivityState state)
        {
            var entities = new UserActivityLogEntities();

            var existingState = entities.ActivityStates.Single(s => s.Id == state.Id);
            existingState.StartTime = state.StartTime;
            existingState.StartTime = state.StartTime;
            existingState.Idle = state.Idle;

            entities.SaveChanges();
        }

        #endregion
    }
}