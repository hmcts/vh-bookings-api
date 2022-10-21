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
    public class GetVhoNonAvailabilityWorkHoursQueryHandlerDatabaseTests : DatabaseTestsBase
    {
        private GetVhoNonAvailableWorkHoursQueryHandler _handler;
        private const string Username = "TestGetVhoNonAvailabilityWorkHoursQueryHandlerDatabaseTests@hearings.reform.hmcts.net";

        [TearDown]
        public void DbCleanup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            context.VhoNonAvailabilities.RemoveRange(context.VhoNonAvailabilities.Where(e => e.JusticeUser.Username == Username));
            context.JusticeUsers.Remove(context.JusticeUsers.First(e => e.Username == Username));
            context.SaveChanges();
        }

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            context.JusticeUsers.Add(new JusticeUser
            {
                ContactEmail = "contact@email.com",
                Username = Username,
                UserRoleId = (int)UserRoleId.vho,
                CreatedBy = "integration.GetVhoNonAvailabilityWorkHoursQueryHandlerDatabaseTests.UnitTest",
                CreatedDate = DateTime.Now,
                FirstName = "test",
                Lastname = "test",
            });
            context.SaveChanges();
            var vhoWorkHours1 = new VhoNonAvailability()
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now,
                JusticeUser = context.JusticeUsers.First(e => e.Username == Username),
                CreatedDate = DateTime.Now,
                CreatedBy = "integration.GetVhoWorkHoursQueryHandler.UnitTest",
            };
            var vhoWorkHours2 = new VhoNonAvailability()
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now,
                JusticeUser = context.JusticeUsers.First(e => e.Username == Username),
                CreatedDate = DateTime.Now,
                CreatedBy = "integration.GetVhoWorkHoursQueryHandler.UnitTest",
            };
            context.VhoNonAvailabilities.AddRange(vhoWorkHours1, vhoWorkHours2);
            context.SaveChanges();
            _handler = new GetVhoNonAvailableWorkHoursQueryHandler(context);
        }

        [Test]
        public async Task Should_return_empty_list_when_no_user_is_found()
        {
            var query = new GetVhoNonAvailableWorkHoursQuery("doesnt.existatall@hmcts.net");
            var vhoWorkHours = await _handler.Handle(query);
            vhoWorkHours.Should().BeEmpty();
        }
        
        [Test]
        public async Task Should_return_VhoWorkHours_when_that_user_exists()
        {
            var query = new GetVhoNonAvailableWorkHoursQuery(Username);
            var vhoWorkHours = await _handler.Handle(query);

            vhoWorkHours.Should().NotBeNull();
            vhoWorkHours[0].JusticeUser.Username.Should().Be(Username);
            vhoWorkHours.Count.Should().Be(2);
        }
    }
}