using System;
using System.Linq;
using BookingsApi.DAL;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetVhoWorkHoursQueryHandlerDatabaseTests : DatabaseTestsBase
    {
        private GetVhoWorkHoursQueryHandler _handler;
        private const string UserWithRecords = "test.Integration.GetVhoWorkHoursQueryHandlerDatabase1@hearings.reform.hmcts.net";
        private const string UserWithoutRecords = "test.Integration.GetVhoWorkHoursQueryHandlerDatabase2@hearings.reform.hmcts.net";
        private const string DeletedUser = "test.Integration.GetVhoWorkHoursQueryHandlerDatabase3@hearings.reform.hmcts.net";
        private const string UserWithDeletedRecords = "test.Integration.GetVhoWorkHoursQueryHandlerDatabase4@hearings.reform.hmcts.net";

        [TearDown]
        public void DbCleanup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            context.VhoWorkHours.RemoveRange(context.VhoWorkHours.Where(e => e.JusticeUser.Username == UserWithRecords));
            context.VhoWorkHours.RemoveRange(context.VhoWorkHours.Where(e => e.JusticeUser.Username == UserWithoutRecords));
            context.VhoWorkHours.RemoveRange(context.VhoWorkHours.IgnoreQueryFilters().Where(e => e.JusticeUser.Username == DeletedUser));
            context.VhoWorkHours.RemoveRange(context.VhoWorkHours.IgnoreQueryFilters().Where(e => e.JusticeUser.Username == UserWithDeletedRecords));
            context.SaveChanges();
        }

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            var userRole = context.UserRoles.First(e => e.Id == (int)UserRoleId.Vho);
            var justiceUser = context.JusticeUsers.Add(new JusticeUser
            {
                ContactEmail = "contact@email.com",
                Username     = UserWithRecords,
                CreatedBy    = "integration.GetVhoWorkHoursQueryHandler.UnitTest",
                CreatedDate  = DateTime.Now,
                FirstName    = "test",
                Lastname     = "test"
            });
            context.JusticeUserRoles.Add(new JusticeUserRole(justiceUser.Entity, userRole));
            Hooks._seededJusticeUserIds.Add(justiceUser.Entity.Id);
            
            justiceUser = context.JusticeUsers.Add(new JusticeUser
            {
                ContactEmail = "contact@email.com",
                Username     = UserWithoutRecords,
                CreatedBy    = "integration.GetVhoWorkHoursQueryHandler.UnitTest",
                CreatedDate  = DateTime.Now,
                FirstName    = "test",
                Lastname     = "test"
            });
            context.JusticeUserRoles.Add(new JusticeUserRole(justiceUser.Entity, userRole));
            Hooks._seededJusticeUserIds.Add(justiceUser.Entity.Id);

            justiceUser = context.JusticeUsers.Add(new JusticeUser
            {
                ContactEmail = "contact@email.com",
                Username = DeletedUser,
                CreatedBy = "integration.GetVhoWorkHoursQueryHandler.UnitTest",
                CreatedDate = DateTime.Now,
                FirstName = "test",
                Lastname = "test"
            });
            context.JusticeUserRoles.Add(new JusticeUserRole(justiceUser.Entity, userRole));
            Hooks._seededJusticeUserIds.Add(justiceUser.Entity.Id);

            justiceUser = context.JusticeUsers.Add(new JusticeUser
            {
                ContactEmail = "contact@email.com",
                Username = UserWithDeletedRecords,
                CreatedBy = "integration.GetVhoWorkHoursQueryHandler.UnitTest",
                CreatedDate = DateTime.Now,
                FirstName = "test",
                Lastname = "test"
            });
            context.JusticeUserRoles.Add(new JusticeUserRole(justiceUser.Entity, userRole));
            Hooks._seededJusticeUserIds.Add(justiceUser.Entity.Id);

            context.SaveChanges();
            var vhoWorkHours1 = new VhoWorkHours
            {
                StartTime   = new TimeSpan(),
                EndTime     = new TimeSpan(),
                JusticeUser = context.JusticeUsers.First(e => e.Username == UserWithRecords),
                CreatedDate = DateTime.Now,
                CreatedBy   = "integration.GetVhoWorkHoursQueryHandler.UnitTest",
                DayOfWeek   = context.DaysOfWeek.First(e => e.Id == 1)
            };
            var vhoWorkHours2 = new VhoWorkHours
            {
                StartTime   = new TimeSpan(),
                EndTime     = new TimeSpan(),
                JusticeUser = context.JusticeUsers.First(e => e.Username == UserWithRecords),
                CreatedDate = DateTime.Now,
                CreatedBy   = "integration.GetVhoWorkHoursQueryHandler.UnitTest",
                DayOfWeek   = context.DaysOfWeek.First(e => e.Id == 2)
            };
            var deletedVhoWorkHours1 = new VhoWorkHours
            {
                StartTime = new TimeSpan(),
                EndTime = new TimeSpan(),
                JusticeUser = context.JusticeUsers.First(e => e.Username == DeletedUser),
                CreatedDate = DateTime.Now,
                CreatedBy = "integration.GetVhoWorkHoursQueryHandler.UnitTest",
                DayOfWeek = context.DaysOfWeek.First(e => e.Id == 1)
            };
            var deletedVhoWorkHours2 = new VhoWorkHours
            {
                StartTime = new TimeSpan(),
                EndTime = new TimeSpan(),
                JusticeUser = context.JusticeUsers.First(e => e.Username == UserWithDeletedRecords),
                CreatedDate = DateTime.Now,
                CreatedBy = "integration.GetVhoWorkHoursQueryHandler.UnitTest",
                DayOfWeek = context.DaysOfWeek.First(e => e.Id == 1)
            };
            context.VhoWorkHours.AddRange(vhoWorkHours1, vhoWorkHours2, deletedVhoWorkHours1, deletedVhoWorkHours2);
            var deletedUser = context.JusticeUsers.First(x => x.Username == DeletedUser);
            deletedUser.Delete();
            var userWithDeletedWorkHours = context.JusticeUsers.First(x => x.Username == UserWithDeletedRecords);
            userWithDeletedWorkHours.Delete(); // Delete the work hours
            userWithDeletedWorkHours.Restore(); // Undelete the user without undeleting the work hours
            context.SaveChanges();

            // Necessary for the user's work hours to get refreshed when we do the query
            context.Entry(userWithDeletedWorkHours).State = EntityState.Detached;

            _handler = new GetVhoWorkHoursQueryHandler(context);
        }

        [Test]
        public async Task Should_return_empty_list_when_no_user_is_found()
        {
            var query = new GetVhoWorkHoursQuery("doesnt.existatall@hmcts.net");
            var vhoWorkHours = await _handler.Handle(query);
            vhoWorkHours.Should().BeNull();
        }
        
        [Test]
        public async Task Should_return_VhoWorkHours_when_that_user_exists()
        {
            var query = new GetVhoWorkHoursQuery(UserWithRecords);
            var vhoWorkHours = await _handler.Handle(query);

            vhoWorkHours.Should().NotBeNull();
            vhoWorkHours[0].JusticeUser.Username.Should().Be(UserWithRecords);
            vhoWorkHours.Count.Should().Be(2);
        }
        
        [Test]
        public async Task Should_return_empty_list_when_user_exists_but_not_work_hours_exist()
        {
            var query        = new GetVhoWorkHoursQuery(UserWithoutRecords);
            var vhoWorkHours = await _handler.Handle(query);
            vhoWorkHours.Should().BeEmpty();
        }

        [Test]
        public async Task Should_return_null_when_user_is_deleted()
        {
            var query = new GetVhoWorkHoursQuery(DeletedUser);
            var vhoWorkHours = await _handler.Handle(query);
            vhoWorkHours.Should().BeNull();
        }

        [Test]
        public async Task Should_not_return_deleted_work_hours()
        {
            var query = new GetVhoWorkHoursQuery(UserWithDeletedRecords);
            var vhoWorkHours = await _handler.Handle(query);
            vhoWorkHours.Should().BeEmpty();
        }
    }
}