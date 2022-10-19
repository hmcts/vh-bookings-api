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
        private const string Username = "integrationtest@hearings.reform.hmcts.net";

        [TearDown]
        public void DbCleanup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            context.VhoWorkHours.RemoveRange(context.VhoWorkHours.Where(e => e.JusticeUser.Username == Username));
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
                CreatedBy = "integration.GetVhoWorkHoursQueryHandler.UnitTest",
                CreatedDate = DateTime.Now,
                FirstName = "test",
                Lastname = "test",
            });
            context.SaveChanges();
            var vhoWorkHours1 = new VhoWorkHours
            {
                StartTime = new TimeSpan(),
                EndTime = new TimeSpan(),
                JusticeUser = context.JusticeUsers.First(e => e.Username == Username),
                CreatedDate = DateTime.Now,
                CreatedBy = "integration.GetVhoWorkHoursQueryHandler.UnitTest",
                DayOfWeek = context.DaysOfWeek.First(e => e.Id == 1)
            };
            var vhoWorkHours2 = new VhoWorkHours
            {
                StartTime = new TimeSpan(),
                EndTime = new TimeSpan(),
                JusticeUser = context.JusticeUsers.First(e => e.Username == Username),
                CreatedDate = DateTime.Now,
                CreatedBy = "integration.GetVhoWorkHoursQueryHandler.UnitTest",
                DayOfWeek = context.DaysOfWeek.First(e => e.Id == 2)
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
            vhoWorkHours.Should().BeEmpty();
        }
        
        [Test]
        public async Task Should_return_VhoWorkHours_when_that_user_exists()
        {
            var query = new GetVhoWorkHoursQuery(Username);
            var vhoWorkHours = await _handler.Handle(query);

            vhoWorkHours.Should().NotBeNull();
            vhoWorkHours[0].JusticeUser.Username.Should().Be(Username);
            vhoWorkHours.Count.Should().Be(2);
        }
    }
}