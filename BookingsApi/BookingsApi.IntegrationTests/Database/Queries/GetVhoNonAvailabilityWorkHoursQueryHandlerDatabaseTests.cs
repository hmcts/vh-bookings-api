using System;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetVhoNonAvailabilityWorkHoursQueryHandlerDatabaseTests : DatabaseTestsBase
    {
        private GetVhoNonAvailableWorkHoursQueryHandler _handler;
        private const string UserWithRecords = "Test.Integration.GetVhoNonAvailabilityWorkHoursQueryHandlerDatabaseTests@hearings1.reform.hmcts.net";
        private const string UserWithoutRecords = "Test.Integration.GetVhoNonAvailabilityWorkHoursQueryHandlerDatabaseTests@hearings2.reform.hmcts.net";

        [TearDown]
        public void DbCleanup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            context.VhoNonAvailabilities.RemoveRange(context.VhoNonAvailabilities.Where(e => e.JusticeUser.Username == UserWithRecords));
            context.JusticeUsers.Remove(context.JusticeUsers.First(e => e.Username == UserWithRecords));
            context.JusticeUsers.Remove(context.JusticeUsers.First(e => e.Username == UserWithoutRecords));
            context.SaveChanges();
        }

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            context.JusticeUsers.Add(new JusticeUser
            {
                ContactEmail = UserWithRecords,
                Username     = UserWithRecords,
                CreatedBy    = "integration.GetVhoNonAvailabilityWorkHoursQueryHandlerDatabaseTests.UnitTest",
                CreatedDate  = DateTime.Now,
                FirstName    = "test",
                Lastname     = "test",
            });
            context.JusticeUsers.Add(new JusticeUser
            {
                ContactEmail = UserWithoutRecords,
                Username     = UserWithoutRecords,
                CreatedBy    = "integration.GetVhoNonAvailabilityWorkHoursQueryHandlerDatabaseTests.UnitTest",
                CreatedDate  = DateTime.Now,
                FirstName    = "test",
                Lastname     = "test",
            });
            context.SaveChanges();
            var vhoWorkHours1 = new VhoNonAvailability()
            {
                StartTime   = DateTime.Now,
                EndTime     = DateTime.Now,
                JusticeUser = context.JusticeUsers.First(e => e.Username == UserWithRecords),
                CreatedDate = DateTime.Now,
                CreatedBy   = "integration.GetVhoWorkHoursQueryHandler.UnitTest",
            };
            var vhoWorkHours2 = new VhoNonAvailability()
            {
                StartTime   = DateTime.Now,
                EndTime     = DateTime.Now,
                JusticeUser = context.JusticeUsers.First(e => e.Username == UserWithRecords),
                CreatedDate = DateTime.Now,
                CreatedBy   = "integration.GetVhoWorkHoursQueryHandler.UnitTest",
            };
            var vhoWorkHours3 = new VhoNonAvailability()
            {
                StartTime   = DateTime.Now,
                EndTime     = DateTime.Now,
                JusticeUser = context.JusticeUsers.First(e => e.Username == UserWithRecords),
                CreatedDate = DateTime.Now,
                CreatedBy   = "integration.GetVhoWorkHoursQueryHandler.UnitTest",
                Deleted = true
            };
            context.VhoNonAvailabilities.AddRange(vhoWorkHours1, vhoWorkHours2, vhoWorkHours3);
            context.SaveChanges();
            _handler = new GetVhoNonAvailableWorkHoursQueryHandler(context);
        }

        [Test]
        public async Task Should_return_null_when_no_user_is_found()
        {
            var query = new GetVhoNonAvailableWorkHoursQuery("doesnt.existatall@hmcts.net");
            var vhoWorkHours = await _handler.Handle(query);
            vhoWorkHours.Should().BeNull();
        }
        
        [Test]
        public async Task Should_return_VhoWorkHours_when_that_user_exists()
        {
            var query = new GetVhoNonAvailableWorkHoursQuery(UserWithRecords);
            var vhoWorkHours = await _handler.Handle(query);

            vhoWorkHours.Should().NotBeNull();
            vhoWorkHours[0].JusticeUser.Username.Should().Be(UserWithRecords);
            vhoWorkHours.Count.Should().Be(2);
        }
        
        [Test]
        public async Task Should_return_empty_list_when_user_exists_but_not_work_hours_exist()
        {
            var query = new GetVhoNonAvailableWorkHoursQuery(UserWithoutRecords);
            var vhoWorkHours = await _handler.Handle(query);
            vhoWorkHours.Should().BeEmpty();
        }
    }
}