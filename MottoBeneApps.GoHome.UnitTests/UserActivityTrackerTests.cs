namespace MottoBeneApps.GoHome.UnitTests
{
    #region Namespace Imports

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FakeItEasy;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using MottoBeneApps.GoHome.ActivityTracking;
    using MottoBeneApps.GoHome.DataModels;

    #endregion


    [TestClass]
    public sealed class UserActivityTrackerTests
    {
        #region Constants and Fields

        private static readonly List<Activity> _activityFakes = new List<Activity>
        {
            GetActivityFake(1, "Work", true),
            GetActivityFake(2, "Break", true),
            GetActivityFake(3, "Meeting", true),
            GetActivityFake(4, "Lunch", false),
            GetActivityFake(5, "Out of office", false),
            GetActivityFake(6, "Home", false),
        };

        #endregion


        #region Properties

        private static Activity BreakActivity
        {
            get
            {
                return _activityFakes[1];
            }
        }

        private static Activity WorkActivity
        {
            get
            {
                return _activityFakes[0];
            }
        }

        #endregion


        #region Public Methods

        [TestMethod]
        public void TestActivityThenForceLogThenIdleThenActivityThenIdleLoggingInEmptyLog()
        {
            // | Activity, Force Log, Idle, Activity, Idle
            var settings = GetActivityTrackingSettingsFake();
            var activitiesRepository = GetActivitiesRepositoryFake();
            var userInputTracker = GetUserInputTrackerFake();

            var records = new List<ActivityRecord>();
            var activityRecordsRepository = GetActivityRecordsRepositoryFake(records);

            var tracker = new UserActivityTracker(
                activityRecordsRepository,
                activitiesRepository,
                settings,
                userInputTracker);

            tracker.Start();

            DateTime timeStamp = DateTime.Now;
            userInputTracker.RaiseUserInputDetectedEvent(timeStamp);
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumActivityDuration);

            tracker.LogUserActivity(false, true, timeStamp);

            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumIdleDuration);
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumActivityDuration);
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumIdleDuration);

            records.Should().HaveCount(4);

            ActivityRecord activityRecord = records[0];
            activityRecord.Idle.Should().Be(false);
            activityRecord.Duration.Should().Be(settings.MinimumActivityDuration);
            activityRecord.Activity.Should().Be(WorkActivity);

            ActivityRecord idleRecord = records[1];
            idleRecord.Idle.Should().Be(true);
            idleRecord.Duration.Should().Be(settings.MinimumIdleDuration);
            idleRecord.Activity.Should().Be(BreakActivity);
            idleRecord.StartTime.Should().Be(activityRecord.EndTime);

            activityRecord = records[2];
            activityRecord.Idle.Should().Be(false);
            activityRecord.Duration.Should().Be(settings.MinimumActivityDuration);
            activityRecord.Activity.Should().Be(WorkActivity);
            activityRecord.StartTime.Should().Be(idleRecord.EndTime);

            idleRecord = records[3];
            idleRecord.Idle.Should().Be(true);
            idleRecord.Duration.Should().Be(settings.MinimumIdleDuration);
            idleRecord.Activity.Should().Be(BreakActivity);
            idleRecord.StartTime.Should().Be(activityRecord.EndTime);

            tracker.Stop();
        }


        [TestMethod]
        public void TestActivityThenForceLogThenShortIdleThenActivityThenIdleLoggingInEmptyLog()
        {
            // | Activity, Force Log, Short Idle, Activity, Idle
            var settings = GetActivityTrackingSettingsFake();
            var activitiesRepository = GetActivitiesRepositoryFake();
            var userInputTracker = GetUserInputTrackerFake();

            var records = new List<ActivityRecord>();
            var activityRecordsRepository = GetActivityRecordsRepositoryFake(records);

            var tracker = new UserActivityTracker(
                activityRecordsRepository,
                activitiesRepository,
                settings,
                userInputTracker);

            tracker.Start();

            DateTime timeStamp = DateTime.Now;
            userInputTracker.RaiseUserInputDetectedEvent(timeStamp);
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumActivityDuration);

            tracker.LogUserActivity(false, true, timeStamp);

            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.GetShortIdleDuration());
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumActivityDuration);
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumIdleDuration);

            records.Should().HaveCount(2);

            ActivityRecord activityRecord = records[0];
            activityRecord.Idle.Should().Be(false);
            activityRecord.Duration.Should()
                .Be(
                    settings.MinimumActivityDuration + settings.GetShortIdleDuration()
                    + settings.MinimumActivityDuration);
            activityRecord.Activity.Should().Be(WorkActivity);

            ActivityRecord idleRecord = records[1];
            idleRecord.Idle.Should().Be(true);
            idleRecord.Duration.Should().Be(settings.MinimumIdleDuration);
            idleRecord.Activity.Should().Be(BreakActivity);
            idleRecord.StartTime.Should().Be(activityRecord.EndTime);

            tracker.Stop();
        }


        [TestMethod]
        public void TestActivityThenIdleLoggingInEmptyLog()
        {
            // | Activity, Idle
            var settings = GetActivityTrackingSettingsFake();
            var activitiesRepository = GetActivitiesRepositoryFake();
            var userInputTracker = GetUserInputTrackerFake();

            var records = new List<ActivityRecord>();
            var activityRecordsRepository = GetActivityRecordsRepositoryFake(records);

            var tracker = new UserActivityTracker(
                activityRecordsRepository,
                activitiesRepository,
                settings,
                userInputTracker);

            tracker.Start();

            DateTime timeStamp = DateTime.Now;
            userInputTracker.RaiseUserInputDetectedEvent(timeStamp);
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumActivityDuration);
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumIdleDuration);

            records.Should().HaveCount(2);

            ActivityRecord activityRecord = records[0];
            activityRecord.Idle.Should().Be(false);
            activityRecord.Duration.Should().Be(settings.MinimumActivityDuration);
            activityRecord.Activity.Should().Be(WorkActivity);

            ActivityRecord idleRecord = records[1];
            idleRecord.Idle.Should().Be(true);
            idleRecord.Duration.Should().Be(settings.MinimumIdleDuration);
            idleRecord.Activity.Should().Be(BreakActivity);
            idleRecord.StartTime.Should().Be(activityRecord.EndTime);

            tracker.Stop();
        }


        [TestMethod]
        public void TestActivityThenIdleLoggingWithLastRecordIndicatingActivityInLog()
        {
            // Activity | Activity, Idle
            var settings = GetActivityTrackingSettingsFake();
            var activitiesRepository = GetActivitiesRepositoryFake();
            var userInputTracker = GetUserInputTrackerFake();

            var records = new List<ActivityRecord>();
            var activityRecordsRepository = GetActivityRecordsRepositoryFake(records);

            var tracker = new UserActivityTracker(
                activityRecordsRepository,
                activitiesRepository,
                settings,
                userInputTracker);

            tracker.Start();

            DateTime timeStamp = DateTime.Now;
            records.Add(
                new ActivityRecord(1)
                {
                    Activity = WorkActivity,
                    Idle = false,
                    StartTime = timeStamp - settings.MinimumActivityDuration,
                    EndTime = timeStamp
                });

            userInputTracker.RaiseUserInputDetectedEvent(timeStamp);
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumActivityDuration);
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumIdleDuration);

            records.Should().HaveCount(2);

            ActivityRecord activityRecord = records[0];
            activityRecord.Idle.Should().Be(false);
            activityRecord.Duration.Should().Be(settings.MinimumActivityDuration + settings.MinimumActivityDuration);

            ActivityRecord idleRecord = records[1];
            idleRecord.Idle.Should().Be(true);
            idleRecord.Duration.Should().Be(settings.MinimumIdleDuration);
            idleRecord.StartTime.Should().Be(activityRecord.EndTime);

            tracker.Stop();
        }


        [TestMethod]
        public void TestActivityThenIdleLoggingWithLastRecordIndicatingInactivityInLog()
        {
            // Idle | Activity, Idle
            var settings = GetActivityTrackingSettingsFake();
            var activitiesRepository = GetActivitiesRepositoryFake();
            var userInputTracker = GetUserInputTrackerFake();

            var records = new List<ActivityRecord>();
            var activityRecordsRepository = GetActivityRecordsRepositoryFake(records);

            var tracker = new UserActivityTracker(
                activityRecordsRepository,
                activitiesRepository,
                settings,
                userInputTracker);

            tracker.Start();

            DateTime timeStamp = DateTime.Now;
            records.Add(
                new ActivityRecord(1)
                {
                    Activity = WorkActivity,
                    Idle = true,
                    StartTime = timeStamp - settings.MinimumIdleDuration,
                    EndTime = timeStamp
                });

            userInputTracker.RaiseUserInputDetectedEvent(timeStamp);
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumActivityDuration);
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumIdleDuration);

            records.Should().HaveCount(3);

            ActivityRecord idleRecord = records[0];
            idleRecord.Idle.Should().Be(true);
            idleRecord.Duration.Should().Be(settings.MinimumIdleDuration);

            ActivityRecord activityRecord = records[1];
            activityRecord.Idle.Should().Be(false);
            activityRecord.Duration.Should().Be(settings.MinimumActivityDuration);
            activityRecord.StartTime.Should().Be(idleRecord.EndTime);

            ActivityRecord lastIdleRecord = records[2];
            lastIdleRecord.Idle.Should().Be(true);
            lastIdleRecord.Duration.Should().Be(settings.MinimumIdleDuration);
            lastIdleRecord.StartTime.Should().Be(activityRecord.EndTime);

            tracker.Stop();
        }


        [TestMethod]
        public void TestForceLogThenActivityThenIdleLoggingInEmptyLog()
        {
            // | Activity, Idle
            var settings = GetActivityTrackingSettingsFake();
            var activitiesRepository = GetActivitiesRepositoryFake();
            var userInputTracker = GetUserInputTrackerFake();

            var records = new List<ActivityRecord>();
            var activityRecordsRepository = GetActivityRecordsRepositoryFake(records);

            var tracker = new UserActivityTracker(
                activityRecordsRepository,
                activitiesRepository,
                settings,
                userInputTracker);

            tracker.Start();

            DateTime timeStamp = DateTime.Now;

            tracker.LogUserActivity(false, true, timeStamp);

            userInputTracker.RaiseUserInputDetectedEvent(timeStamp);
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumActivityDuration);
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumIdleDuration);

            records.Should().HaveCount(2);

            ActivityRecord activityRecord = records[0];
            activityRecord.Idle.Should().Be(false);
            activityRecord.Duration.Should().Be(settings.MinimumActivityDuration);
            activityRecord.Activity.Should().Be(WorkActivity);

            ActivityRecord idleRecord = records[1];
            idleRecord.Idle.Should().Be(true);
            idleRecord.Duration.Should().Be(settings.MinimumIdleDuration);
            idleRecord.Activity.Should().Be(BreakActivity);
            idleRecord.StartTime.Should().Be(activityRecord.EndTime);

            tracker.Stop();
        }


        [TestMethod]
        public void TestIdleThenActivityThenIdleLoggingWithLastRecordIndicatingActivityInLog()
        {
            // Activity | Idle, Activity, Idle
            var settings = GetActivityTrackingSettingsFake();
            var activitiesRepository = GetActivitiesRepositoryFake();
            var userInputTracker = GetUserInputTrackerFake();

            var records = new List<ActivityRecord>();
            var activityRecordsRepository = GetActivityRecordsRepositoryFake(records);

            var tracker = new UserActivityTracker(
                activityRecordsRepository,
                activitiesRepository,
                settings,
                userInputTracker);

            tracker.Start();

            DateTime timeStamp = DateTime.Now;
            records.Add(
                new ActivityRecord(1)
                {
                    Activity = WorkActivity,
                    Idle = false,
                    StartTime = timeStamp - settings.MinimumActivityDuration,
                    EndTime = timeStamp
                });

            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumIdleDuration);
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumActivityDuration);
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumIdleDuration);

            records.Should().HaveCount(4);

            ActivityRecord initialRecord = records[0];
            initialRecord.Idle.Should().Be(false);
            initialRecord.Duration.Should().Be(settings.MinimumActivityDuration);

            ActivityRecord idleRecord = records[1];
            idleRecord.Idle.Should().Be(true);
            idleRecord.Duration.Should().Be(settings.MinimumIdleDuration);
            idleRecord.StartTime.Should().Be(initialRecord.EndTime);

            ActivityRecord activityRecord = records[2];
            activityRecord.Idle.Should().Be(false);
            activityRecord.Duration.Should().Be(settings.MinimumActivityDuration);
            activityRecord.StartTime.Should().Be(idleRecord.EndTime);

            ActivityRecord lastIdleRecord = records[3];
            lastIdleRecord.Idle.Should().Be(true);
            lastIdleRecord.Duration.Should().Be(settings.MinimumIdleDuration);
            lastIdleRecord.StartTime.Should().Be(activityRecord.EndTime);

            tracker.Stop();
        }


        [TestMethod]
        public void TestRestartLoggingThenActivityThenIdleLoggingInEmptyLog()
        {
            // | Activity, Idle
            var settings = GetActivityTrackingSettingsFake();
            var activitiesRepository = GetActivitiesRepositoryFake();
            var userInputTracker = GetUserInputTrackerFake();

            var records = new List<ActivityRecord>();
            var activityRecordsRepository = GetActivityRecordsRepositoryFake(records);

            var tracker = new UserActivityTracker(
                activityRecordsRepository,
                activitiesRepository,
                settings,
                userInputTracker);

            tracker.Start();
            tracker.Stop();
            tracker.Start();

            DateTime timeStamp = DateTime.Now;
            userInputTracker.RaiseUserInputDetectedEvent(timeStamp);
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumActivityDuration);
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumIdleDuration);

            records.Should().HaveCount(2);

            ActivityRecord activityRecord = records[0];
            activityRecord.Idle.Should().Be(false);
            activityRecord.Duration.Should().Be(settings.MinimumActivityDuration);
            activityRecord.Activity.Should().Be(WorkActivity);

            ActivityRecord idleRecord = records[1];
            idleRecord.Idle.Should().Be(true);
            idleRecord.Duration.Should().Be(settings.MinimumIdleDuration);
            idleRecord.Activity.Should().Be(BreakActivity);
            idleRecord.StartTime.Should().Be(activityRecord.EndTime);

            tracker.Stop();
        }


        [TestMethod]
        public void TestShortActivityThenForceLogThenActivityThenIdleLoggingInEmptyLog()
        {
            // | Activity, Idle
            var settings = GetActivityTrackingSettingsFake();
            var activitiesRepository = GetActivitiesRepositoryFake();
            var userInputTracker = GetUserInputTrackerFake();

            var records = new List<ActivityRecord>();
            var activityRecordsRepository = GetActivityRecordsRepositoryFake(records);

            var tracker = new UserActivityTracker(
                activityRecordsRepository,
                activitiesRepository,
                settings,
                userInputTracker);

            tracker.Start();

            DateTime timeStamp = DateTime.Now;

            userInputTracker.RaiseUserInputDetectedEvent(timeStamp);
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.GetShortActivityDuration());

            tracker.LogUserActivity(false, true, timeStamp);

            userInputTracker.RaiseUserInputDetectedEvent(timeStamp);
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumActivityDuration);
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumIdleDuration);

            records.Should().HaveCount(2);

            ActivityRecord activityRecord = records[0];
            activityRecord.Idle.Should().Be(false);
            activityRecord.Duration.Should().Be(settings.MinimumActivityDuration + settings.GetShortActivityDuration());
            activityRecord.Activity.Should().Be(WorkActivity);

            ActivityRecord idleRecord = records[1];
            idleRecord.Idle.Should().Be(true);
            idleRecord.Duration.Should().Be(settings.MinimumIdleDuration);
            idleRecord.Activity.Should().Be(BreakActivity);
            idleRecord.StartTime.Should().Be(activityRecord.EndTime);

            tracker.Stop();
        }


        [TestMethod]
        public void TestShortActivityThenForceLogThenShortActivityThenForceLogThenActivityThenIdleLoggingInEmptyLog()
        {
            // | Short Activity, Force Log, Short activity, Force Log, Activity, Idle
            var settings = GetActivityTrackingSettingsFake();
            var activitiesRepository = GetActivitiesRepositoryFake();
            var userInputTracker = GetUserInputTrackerFake();

            var records = new List<ActivityRecord>();
            var activityRecordsRepository = GetActivityRecordsRepositoryFake(records);

            var tracker = new UserActivityTracker(
                activityRecordsRepository,
                activitiesRepository,
                settings,
                userInputTracker);

            tracker.Start();

            DateTime timeStamp = DateTime.Now;

            userInputTracker.RaiseUserInputDetectedEvent(timeStamp);
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.GetShortActivityDuration());

            tracker.LogUserActivity(false, true, timeStamp);

            userInputTracker.RaiseUserInputDetectedEvent(timeStamp);
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.GetShortActivityDuration());
            tracker.LogUserActivity(false, true, timeStamp);

            userInputTracker.RaiseUserInputDetectedEvent(timeStamp);
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumActivityDuration);
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumIdleDuration);

            records.Should().HaveCount(2);

            ActivityRecord activityRecord = records[0];
            activityRecord.Idle.Should().Be(false);
            activityRecord.Duration.Should()
                .Be(
                    settings.MinimumActivityDuration + settings.GetShortActivityDuration()
                    + settings.GetShortActivityDuration());
            activityRecord.Activity.Should().Be(WorkActivity);

            ActivityRecord idleRecord = records[1];
            idleRecord.Idle.Should().Be(true);
            idleRecord.Duration.Should().Be(settings.MinimumIdleDuration);
            idleRecord.Activity.Should().Be(BreakActivity);
            idleRecord.StartTime.Should().Be(activityRecord.EndTime);

            tracker.Stop();
        }


        [TestMethod]
        public void TestShortActivityThenIdleLoggingInEmptyLog()
        {
            // | Short Activity, Idle
            var settings = GetActivityTrackingSettingsFake();
            var activitiesRepository = GetActivitiesRepositoryFake();
            var userInputTracker = GetUserInputTrackerFake();

            var records = new List<ActivityRecord>();
            var activityRecordsRepository = GetActivityRecordsRepositoryFake(records);

            var tracker = new UserActivityTracker(
                activityRecordsRepository,
                activitiesRepository,
                settings,
                userInputTracker);

            tracker.Start();

            DateTime timeStamp = DateTime.Now;
            userInputTracker.RaiseUserInputDetectedEvent(timeStamp);
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.GetShortActivityDuration());
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumIdleDuration);

            records.Should().HaveCount(1);

            ActivityRecord idleRecord = records[0];
            idleRecord.Idle.Should().Be(true);
            idleRecord.Duration.Should().Be(settings.MinimumIdleDuration + settings.GetShortActivityDuration());
            idleRecord.Activity.Should().Be(BreakActivity);

            tracker.Stop();
        }


        [TestMethod]
        public void TestShortActivityThenIdleLoggingWithLastRecordIndicatingActivityInLog()
        {
            // Activity | Short Activity, Idle
            var settings = GetActivityTrackingSettingsFake();
            var activitiesRepository = GetActivitiesRepositoryFake();
            var userInputTracker = GetUserInputTrackerFake();

            var records = new List<ActivityRecord>();
            var activityRecordsRepository = GetActivityRecordsRepositoryFake(records);

            var tracker = new UserActivityTracker(
                activityRecordsRepository,
                activitiesRepository,
                settings,
                userInputTracker);

            tracker.Start();

            DateTime timeStamp = DateTime.Now;
            records.Add(
                new ActivityRecord(1)
                {
                    Activity = WorkActivity,
                    Idle = false,
                    StartTime = timeStamp - settings.MinimumActivityDuration,
                    EndTime = timeStamp
                });

            userInputTracker.RaiseUserInputDetectedEvent(timeStamp);
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.GetShortActivityDuration());
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumIdleDuration);

            records.Should().HaveCount(2);

            ActivityRecord record = records[0];
            record.Idle.Should().Be(false);
            record.Duration.Should().Be(settings.MinimumActivityDuration + settings.GetShortActivityDuration());

            record = records[1];
            record.Idle.Should().Be(true);
            record.Duration.Should().Be(settings.MinimumIdleDuration);
            record.StartTime.Should().Be(records[0].EndTime);

            tracker.Stop();
        }


        [TestMethod]
        public void TestShortActivityThenIdleWithLastRecordIndicatingInactivityInLog()
        {
            // Idle | Short Activity, Idle
            var settings = GetActivityTrackingSettingsFake();
            var activitiesRepository = GetActivitiesRepositoryFake();
            var userInputTracker = GetUserInputTrackerFake();

            var records = new List<ActivityRecord>();
            var activityRecordsRepository = GetActivityRecordsRepositoryFake(records);

            var tracker = new UserActivityTracker(
                activityRecordsRepository,
                activitiesRepository,
                settings,
                userInputTracker);

            tracker.Start();

            DateTime timeStamp = DateTime.Now;
            records.Add(
                new ActivityRecord(1)
                {
                    Activity = BreakActivity,
                    Idle = true,
                    StartTime = timeStamp - settings.MinimumIdleDuration,
                    EndTime = timeStamp
                });

            userInputTracker.RaiseUserInputDetectedEvent(timeStamp);
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.GetShortActivityDuration());
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumIdleDuration);

            records.Should().HaveCount(1);

            ActivityRecord idleRecord = records[0];
            idleRecord.Idle.Should().Be(true);
            idleRecord.Duration.Should()
                .Be(settings.MinimumIdleDuration + settings.GetShortActivityDuration() + settings.MinimumIdleDuration);

            tracker.Stop();
        }


        [TestMethod]
        public void TestShortIdleThenActivityThenIdleLoggingWithLastRecordIndicatingActivityInLog()
        {
            // Activity | Short Idle, Activity, Idle
            var settings = GetActivityTrackingSettingsFake();
            var activitiesRepository = GetActivitiesRepositoryFake();
            var userInputTracker = GetUserInputTrackerFake();

            var records = new List<ActivityRecord>();
            var activityRecordsRepository = GetActivityRecordsRepositoryFake(records);

            var tracker = new UserActivityTracker(
                activityRecordsRepository,
                activitiesRepository,
                settings,
                userInputTracker);

            tracker.Start();

            DateTime timeStamp = DateTime.Now;
            records.Add(
                new ActivityRecord(1)
                {
                    Activity = WorkActivity,
                    Idle = false,
                    StartTime = timeStamp - settings.MinimumActivityDuration,
                    EndTime = timeStamp
                });

            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.GetShortIdleDuration());
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumActivityDuration);
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumIdleDuration);

            records.Should().HaveCount(2);

            ActivityRecord initialRecord = records[0];
            initialRecord.Idle.Should().Be(false);
            initialRecord.Duration.Should()
                .Be(
                    settings.MinimumActivityDuration + settings.GetShortIdleDuration()
                    + settings.MinimumActivityDuration);

            ActivityRecord idleRecord = records[1];
            idleRecord.Idle.Should().Be(true);
            idleRecord.Duration.Should().Be(settings.MinimumIdleDuration);
            idleRecord.StartTime.Should().Be(initialRecord.EndTime);

            tracker.Stop();
        }


        [TestMethod]
        public void TestShortIdleThenActivityThenIdleLoggingWithLastRecordIndicatingInactivityInLog()
        {
            // Idle | Short Idle, Activity, Idle
            var settings = GetActivityTrackingSettingsFake();
            var activitiesRepository = GetActivitiesRepositoryFake();
            var userInputTracker = GetUserInputTrackerFake();

            var records = new List<ActivityRecord>();
            var activityRecordsRepository = GetActivityRecordsRepositoryFake(records);

            var tracker = new UserActivityTracker(
                activityRecordsRepository,
                activitiesRepository,
                settings,
                userInputTracker);

            tracker.Start();

            DateTime timeStamp = DateTime.Now;
            records.Add(
                new ActivityRecord(1)
                {
                    Activity = BreakActivity,
                    Idle = true,
                    StartTime = timeStamp - settings.MinimumIdleDuration,
                    EndTime = timeStamp
                });

            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.GetShortIdleDuration());
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumActivityDuration);
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumIdleDuration);

            records.Should().HaveCount(3);

            ActivityRecord initialRecord = records[0];
            initialRecord.Idle.Should().Be(true);
            initialRecord.Duration.Should().Be(settings.MinimumIdleDuration + settings.GetShortIdleDuration());

            ActivityRecord activityRecord = records[1];
            activityRecord.Idle.Should().Be(false);
            activityRecord.Duration.Should().Be(settings.MinimumActivityDuration);
            activityRecord.StartTime.Should().Be(initialRecord.EndTime);

            ActivityRecord idleRecord = records[2];
            idleRecord.Idle.Should().Be(true);
            idleRecord.Duration.Should().Be(settings.MinimumIdleDuration);
            idleRecord.StartTime.Should().Be(activityRecord.EndTime);

            tracker.Stop();
        }


        [TestMethod]
        public void TestShortIdleThenShortActivityThenIdleLoggingWithLastRecordIndicatingActivityInLog()
        {
            // Activity | Short Idle, Short Activity, Idle
            var settings = GetActivityTrackingSettingsFake();
            var activitiesRepository = GetActivitiesRepositoryFake();
            var userInputTracker = GetUserInputTrackerFake();

            var records = new List<ActivityRecord>();
            var activityRecordsRepository = GetActivityRecordsRepositoryFake(records);

            var tracker = new UserActivityTracker(
                activityRecordsRepository,
                activitiesRepository,
                settings,
                userInputTracker);

            tracker.Start();

            DateTime timeStamp = DateTime.Now;
            records.Add(
                new ActivityRecord(1)
                {
                    Activity = WorkActivity,
                    Idle = false,
                    StartTime = timeStamp - settings.MinimumActivityDuration,
                    EndTime = timeStamp
                });

            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.GetShortIdleDuration());
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.GetShortActivityDuration());
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumIdleDuration);

            records.Should().HaveCount(2);

            ActivityRecord initialRecord = records[0];
            initialRecord.Idle.Should().Be(false);
            initialRecord.Duration.Should()
                .Be(
                    settings.MinimumActivityDuration + settings.GetShortIdleDuration()
                    + settings.GetShortActivityDuration());

            ActivityRecord idleRecord = records[1];
            idleRecord.Idle.Should().Be(true);
            idleRecord.Duration.Should().Be(settings.MinimumIdleDuration);
            idleRecord.StartTime.Should().Be(initialRecord.EndTime);

            tracker.Stop();
        }


        [TestMethod]
        public void TestShortIdleThenShortActivityThenIdleLoggingWithLastRecordIndicatingInactivityInLog()
        {
            // Idle | Short Idle, Short Activity, Idle
            var settings = GetActivityTrackingSettingsFake();
            var activitiesRepository = GetActivitiesRepositoryFake();
            var userInputTracker = GetUserInputTrackerFake();

            var records = new List<ActivityRecord>();
            var activityRecordsRepository = GetActivityRecordsRepositoryFake(records);

            var tracker = new UserActivityTracker(
                activityRecordsRepository,
                activitiesRepository,
                settings,
                userInputTracker);

            tracker.Start();

            DateTime timeStamp = DateTime.Now;
            records.Add(
                new ActivityRecord(1)
                {
                    Activity = WorkActivity,
                    Idle = true,
                    StartTime = timeStamp - settings.MinimumIdleDuration,
                    EndTime = timeStamp
                });

            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.GetShortIdleDuration());
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.GetShortActivityDuration());
            userInputTracker.RaiseUserInputDetectedEvent(ref timeStamp, settings.MinimumIdleDuration);

            records.Should().HaveCount(1);

            ActivityRecord initialRecord = records[0];
            initialRecord.Idle.Should().Be(true);
            initialRecord.Duration.Should()
                .Be(
                    settings.MinimumIdleDuration + settings.GetShortIdleDuration() + settings.GetShortActivityDuration()
                    + settings.MinimumIdleDuration);

            tracker.Stop();
        }

        #endregion


        #region Methods

        private static IActivitiesRepository GetActivitiesRepositoryFake()
        {
            var activitiesRepository = A.Fake<IActivitiesRepository>();

            A.CallTo(() => activitiesRepository.GetActivities()).Returns(_activityFakes);

            return activitiesRepository;
        }


        private static Activity GetActivityFake(int id, string name, bool isWork)
        {
            return new Activity(id) { Name = name, IsWork = isWork };
        }


        private static IActivityRecordsRepository GetActivityRecordsRepositoryFake(ICollection<ActivityRecord> records)
        {
            var activityRecordsRepository = A.Fake<IActivityRecordsRepository>();

            A.CallTo(() => activityRecordsRepository.GetLastRecord()).ReturnsLazily(records.LastOrDefault);

            A.CallTo(() => activityRecordsRepository.Add(A<ActivityRecord>._))
                .Invokes(call => records.Add(call.Arguments.Get<ActivityRecord>(0)));

            A.CallTo(() => activityRecordsRepository.Update(A<ActivityRecord>._)).Invokes(
                call =>
                {
                    var record = call.Arguments.Get<ActivityRecord>(0);
                    var existingRecord = records.Single(r => r.ActivityRecordId == record.ActivityRecordId);

                    existingRecord.Activity = record.Activity;
                    existingRecord.StartTime = record.StartTime;
                    existingRecord.EndTime = record.EndTime;
                    existingRecord.Idle = record.Idle;
                });

            return activityRecordsRepository;
        }


        private static IActivityTrackingSettings GetActivityTrackingSettingsFake(
            int activityThreshold = 100,
            int idleThreshold = 200,
            int workdayDuration = 10000)
        {
            var activityTrackingSettings = A.Fake<IActivityTrackingSettings>();
            A.CallTo(() => activityTrackingSettings.WorkDayDuration).Returns(TimeSpan.FromSeconds(workdayDuration));
            A.CallTo(() => activityTrackingSettings.MinimumActivityDuration)
                .Returns(TimeSpan.FromMilliseconds(activityThreshold));
            A.CallTo(() => activityTrackingSettings.MinimumIdleDuration)
                .Returns(TimeSpan.FromMilliseconds(idleThreshold));
            return activityTrackingSettings;
        }


        private static IUserInputTracker GetUserInputTrackerFake()
        {
            var userInputTracker = A.Fake<IUserInputTracker>();
            bool isTrackingUserInput = false;
            A.CallTo(() => userInputTracker.IsTracking).ReturnsLazily(() => isTrackingUserInput);
            A.CallTo(() => userInputTracker.Start()).Invokes(() => isTrackingUserInput = true);
            A.CallTo(() => userInputTracker.Stop()).Invokes(() => isTrackingUserInput = false);
            return userInputTracker;
        }

        #endregion
    }
}