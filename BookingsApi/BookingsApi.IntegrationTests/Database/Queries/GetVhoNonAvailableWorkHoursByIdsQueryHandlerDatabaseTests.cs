using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.DAL;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetVhoNonAvailableWorkHoursByIdsQueryHandlerDatabaseTests : DatabaseTestsBase
    {
        private GetVhoNonAvailableWorkHoursByIdsQueryHandler _queryHandler;
        private BookingsDbContext _context;
        private Dictionary<long, long> _hourIdMappings;
        private const string Username = "team.lead.1@hearings.reform.hmcts.net";

        [SetUp]
        public void Setup()
        {
            _context = new BookingsDbContext(BookingsDbContextOptions);
            _queryHandler = new GetVhoNonAvailableWorkHoursByIdsQueryHandler(_context);
        }
        
        [Test]
        public async Task Should_return_work_hours_when_found()
        {
            // Arrange
            await SeedVhoNonAvailableWorkHours();
            var workHourIds = _hourIdMappings.Values.ToList();
            var query = new GetVhoNonAvailableWorkHoursByIdsQuery(workHourIds);
            
            // Act
            var workHours = await _queryHandler.Handle(query);

            // Assert
            workHours.Count.Should().Be(2);
            
            workHours[0].Id.Should().Be(_hourIdMappings[1]);
            workHours[0].JusticeUser.Username.Should().Be(Username);
            workHours[0].StartTime.Should().Be(new DateTime(2022, 1, 1, 6, 0, 0, DateTimeKind.Utc));
            workHours[0].EndTime.Should().Be(new DateTime(2022, 1, 1, 10, 0, 0, DateTimeKind.Utc));
            
            workHours[1].Id.Should().Be(_hourIdMappings[2]);
            workHours[1].JusticeUser.Username.Should().Be(Username);
            workHours[1].StartTime.Should().Be(new DateTime(2022, 1, 2, 6, 0, 0, DateTimeKind.Utc));
            workHours[1].EndTime.Should().Be(new DateTime(2022, 1, 2, 10, 0, 0, DateTimeKind.Utc));
        }

        [Test]
        public async Task Should_return_empty_list_when_work_hours_not_found()
        {
            // Arrange
            await SeedVhoNonAvailableWorkHours();
            const int nonExistentId = 9999;
            var workHourIds = new List<long> { nonExistentId };
            var query = new GetVhoNonAvailableWorkHoursByIdsQuery(workHourIds);
            
            // Act
            var workHours = await _queryHandler.Handle(query);
            
            // Assert
            workHours.Should().BeEmpty();
        }

        private async Task SeedVhoNonAvailableWorkHours()
        {
            var user = await Hooks
                .SeedJusticeUser(Username, "firstName", "secondname", true);
            
            _hourIdMappings = new Dictionary<long, long>();
            
            var existingHours = new List<NonWorkingHours>
            {
                new()
                {
                    Id = 1,
                    StartTime = new DateTime(2022, 1, 1, 6, 0, 0, DateTimeKind.Utc),
                    EndTime = new DateTime(2022, 1, 1, 10, 0, 0, DateTimeKind.Utc)
                },
                new()
                {
                    Id = 2,
                    StartTime = new DateTime(2022, 1, 2, 6, 0, 0, DateTimeKind.Utc),
                    EndTime = new DateTime(2022, 1, 2, 10, 0, 0, DateTimeKind.Utc)
                }
            };

            foreach (var hour in existingHours)
            {
                var vhoNonWorkingHour = _context.VhoNonAvailabilities.Add(new VhoNonAvailability
                {
                    JusticeUser = _context.JusticeUsers.FirstOrDefault(u => u.Id == user.Id),
                    StartTime = hour.StartTime,
                    EndTime = hour.EndTime
                });

                await _context.SaveChangesAsync();

                _hourIdMappings[hour.Id] = vhoNonWorkingHour.Entity.Id;
            }
        }
    }
}
