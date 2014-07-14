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
            using (var entities = new UserActivityLogEntities())
            {
                entities.ActivityStates.Add(state);
                entities.SaveChanges();
            }
        }


        public UserActivityState GetLastUserActivityState()
        {
            using (var entities = new UserActivityLogEntities())
            {
                return entities.ActivityStates.OrderByDescending(s => s.EndTime).Take(1).ToList().FirstOrDefault();
            }
        }


        public IEnumerable<UserActivityState> GetStates()
        {
            using (var entities = new UserActivityLogEntities())
            {
                return entities.ActivityStates.ToList();
            }
        }


        public void Update(UserActivityState state)
        {
            using (var entities = new UserActivityLogEntities())
            {
                var existingState = entities.ActivityStates.Single(s => s.Id == state.Id);
                existingState.StartTime = state.StartTime;
                existingState.EndTime = state.EndTime;
                existingState.Idle = state.Idle;

                entities.SaveChanges();
            }
        }

        #endregion
    }
}