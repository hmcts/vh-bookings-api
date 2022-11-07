using System;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain;
using BookingsApi.IntegrationTests.Constants;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetVhoWorkHoursQueryHandlerDatabaseTests : DatabaseTestsBase
    {
        private GetVhoWorkHoursQueryHandler _handler;
        private const string UserWithRecords = "test.Integration.GetVhoWorkHoursQueryHandlerDatabase1@hearings.reform.hmcts.net";
        private const string UserWithoutRecords = "test.Integration.GetVhoWorkHoursQueryHandlerDatabase2@hearings.reform.hmcts.net";

        [TearDown]
        public void DbCleanup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            context.VhoWorkHours.RemoveRange(context.VhoWorkHours.Where(e => e.JusticeUser.Username == UserWithRecords));
            context.JusticeUsers.Remove(context.JusticeUsers.First(e => e.Username == UserWithRecords));
            context.SaveChanges();
        }

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            context.JusticeUsers.Add(new JusticeUser
            {
                ContactEmail = "contact@email.com",
                Username     = UserWithRecords,
                UserRoleId   = (int)UserRoleId.vho,
                CreatedBy    = "integration.GetVhoWorkHoursQueryHandler.UnitTest",
                CreatedDate  = DateTime.Now,
                FirstName    = "test",
                Lastname     = "test",
            });
            context.JusticeUsers.Add(new JusticeUser
            {
                ContactEmail = "contact@email.com",
                Username     = UserWithoutRecords,
                UserRoleId   = (int)UserRoleId.vho,
                CreatedBy    = "integration.GetVhoWorkHoursQueryHandler.UnitTest",
                CreatedDate  = DateTime.Now,
                FirstName    = "test",
                Lastname     = "test",
            });
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
            context.VhoWorkHours.AddRange(vhoWorkHours1, vhoWorkHours2);
            context.SaveChanges();
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
    }
}