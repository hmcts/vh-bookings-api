using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class UpdateNonWorkingHoursCommandTests : DatabaseTestsBase
    {
        private UpdateNonWorkingHoursCommandHandler _commandHandler;
        private BookingsDbContext _context;
        private Dictionary<long, long> _hourIdMappings;
        private const string Username = "team.lead.1@hearings.reform.hmcts.net";
        
        [SetUp]
        public void Setup()
        {
            _context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new UpdateNonWorkingHoursCommandHandler(_context);
        }

        [Test]
        public async Task Should_update_non_working_hours()
        {
            // Arrange
            await SeedNonWorkingHours();
            
            // Hours to update
            var newHour1 = new
            {
                StartTime = new DateTime(2022, 2, 1, 6, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(2022, 2, 1, 10, 0, 0, DateTimeKind.Utc)
            };
            var newHour2 = new
            {
                StartTime = new DateTime(2022, 2, 2, 6, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(2022, 2, 2, 10, 0, 0, DateTimeKind.Utc)
            };
            
            var newHours = new List<NonWorkingHours>
            {
                new()
                {
                    Id = _hourIdMappings[1],
                    StartTime = newHour1.StartTime,
                    EndTime = newHour1.EndTime
                },
                new()
                {
                    Id = _hourIdMappings[2],
                    StartTime = newHour2.StartTime,
                    EndTime = newHour2.EndTime
                }
            };
            
            // Act
            await _commandHandler.Handle(new UpdateNonWorkingHoursCommand(Guid.NewGuid(), newHours));
            
            // Assert
            var nonWorkingHours = _context.VhoNonAvailabilities;
            
            var updatedHour1 = nonWorkingHours.FirstOrDefault(h => h.Id == _hourIdMappings[1]);
            updatedHour1.StartTime.Should().Be(newHour1.StartTime);
            updatedHour1.EndTime.Should().Be(newHour1.EndTime);
            
            var updatedHour2 = nonWorkingHours.FirstOrDefault(h => h.Id == _hourIdMappings[2]);
            updatedHour2.StartTime.Should().Be(newHour2.StartTime);
            updatedHour2.EndTime.Should().Be(newHour2.EndTime);
        }

        [Test]
        public async Task Should_add_non_working_hour_when_not_found()
        {
            // Arrange
            var justiceUserId = _context.JusticeUsers.First().Id;
            var originalNonWorkingHoursLength = _context.VhoNonAvailabilities.Where(x => x.JusticeUserId == justiceUserId).Count();

            var newHour1 = new
            {
                StartTime = new DateTime(2022, 2, 1, 6, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(2022, 2, 1, 10, 0, 0, DateTimeKind.Utc)
            };

            var newHours = new List<NonWorkingHours>
            {
                new()
                {
                    Id = 1,
                    StartTime = newHour1.StartTime,
                    EndTime = newHour1.EndTime
                }
            };

            // Act
            await _commandHandler.Handle(new UpdateNonWorkingHoursCommand(justiceUserId, newHours));
            var newNonWorkingHoursLength = _context.VhoNonAvailabilities.Where(x => x.JusticeUserId == justiceUserId).Count();

            // Assert
            Assert.AreEqual(originalNonWorkingHoursLength + 1, newNonWorkingHoursLength);
        }
        
        private async Task SeedNonWorkingHours()
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
