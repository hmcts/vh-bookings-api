using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Common.Services;
using BookingsApi.Contract.Requests;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Services;
using BookingsApi.Domain;
using BookingsApi.Domain.Configuration;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class UpdateNonWorkingHoursCommandTests : DatabaseTestsBase
    {
        private UpdateNonWorkingHoursCommandHandler _commandHandler;
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;
        private BookingsDbContext _context;
        private Dictionary<long, long> _hourIdMappings;
        private const string Username = "team.lead.1@hearings.reform.hmcts.net";
        private Guid _justiceUserId;
        
        [SetUp]
        public void Setup()
        {
            _context = new BookingsDbContext(BookingsDbContextOptions);
            var randomNumberGenerator = new RandomNumberGenerator();
            var allocationConfiguration = GetDefaultAllocationSettings();
            var hearingAllocationService = new HearingAllocationService(_context, 
                randomNumberGenerator, 
                new OptionsWrapper<AllocateHearingConfiguration>(allocationConfiguration),
                new NullLogger<HearingAllocationService>());
            _commandHandler = new UpdateNonWorkingHoursCommandHandler(_context, hearingAllocationService);
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(_context);
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
            await _commandHandler.Handle(new UpdateNonWorkingHoursCommand(_justiceUserId, newHours));
            
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
            await SeedNonWorkingHours();
            
            var justiceUserId = _justiceUserId;
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
                    Id = 3,
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
        
        [Test]
        public async Task Should_deallocate_hearings_when_users_no_longer_available()
        {
            // Arrange
            await SeedNonWorkingHours();
            var userId = _justiceUserId;
            var seededHearing = await Hooks.SeedVideoHearing();
            _context.Allocations.Add(new Allocation
            {
                HearingId = seededHearing.Id,
                JusticeUserId = userId
            });
            await _context.SaveChangesAsync();
            var hearing = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            hearing.AllocatedTo.Should().NotBeNull();
            hearing.AllocatedTo.Id.Should().Be(userId);

            // Hours to update
            var newHour1 = new
            {
                StartTime = seededHearing.ScheduledDateTime.Date.AddHours(1),
                EndTime = seededHearing.ScheduledDateTime.Date.AddHours(12)
            };
            var newHour2 = new
            {
                StartTime = seededHearing.ScheduledDateTime.Date.AddHours(12),
                EndTime = seededHearing.ScheduledDateTime.Date.AddHours(23)
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
            await _commandHandler.Handle(new UpdateNonWorkingHoursCommand(userId, newHours));
            
            // Assert
            hearing = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            hearing.AllocatedTo.Should().BeNull();
        }
        
        private async Task SeedNonWorkingHours()
        {
            var user = await Hooks
                .SeedJusticeUser(Username, "firstName", "secondname", true);
            _justiceUserId = user.Id;
            
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
        
        private static AllocateHearingConfiguration GetDefaultAllocationSettings()
        {
            return new AllocateHearingConfiguration
            {
                AllowHearingToStartBeforeWorkStartTime = false,
                AllowHearingToEndAfterWorkEndTime = false,
                MinimumGapBetweenHearingsInMinutes = 30,
                MaximumConcurrentHearings = 3
            };
        }
    }
}
