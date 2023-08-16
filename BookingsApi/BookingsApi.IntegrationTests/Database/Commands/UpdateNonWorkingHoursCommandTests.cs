using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Queries;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class UpdateNonWorkingHoursCommandDatabaseTests : AllocationDatabaseTestsBase
    {
        private UpdateNonWorkingHoursCommandHandler _commandHandler;
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;
        private BookingsDbContext _context;
        private Dictionary<long, long> _hourIdMappings;
        private const string Username = "team.lead.1@hearings.reform.hmcts.net";
        private JusticeUser _justiceUser;
        
        [SetUp]
        public void Setup()
        {
            _context = Context;
            _commandHandler = new UpdateNonWorkingHoursCommandHandler(_context, HearingAllocationService);
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
            
            var newHours = new List<NonWorkHoursDto>
            {
                new(_hourIdMappings[1],newHour1.StartTime, newHour1.EndTime),
                new(_hourIdMappings[2],newHour2.StartTime, newHour2.EndTime)
            };

            // Act
            await _commandHandler.Handle(new UpdateNonWorkingHoursCommand(_justiceUser.Id, newHours));
            
            // Assert
            var nonWorkingHours = _context.JusticeUsers.Include(x => x.VhoNonAvailability).First(x =>x.Id == _justiceUser.Id).VhoNonAvailability;
            
            var updatedHour1 = nonWorkingHours.First(h => h.Id == _hourIdMappings[1]);
            updatedHour1.StartTime.Should().Be(newHour1.StartTime);
            updatedHour1.EndTime.Should().Be(newHour1.EndTime);
            
            var updatedHour2 = nonWorkingHours.First(h => h.Id == _hourIdMappings[2]);
            updatedHour2.StartTime.Should().Be(newHour2.StartTime);
            updatedHour2.EndTime.Should().Be(newHour2.EndTime);
        }

        [Test]
        public async Task Should_add_non_working_hour_when_not_found()
        {
            // Arrange
            await SeedNonWorkingHours();

            var justiceUserId = _justiceUser.Id;
            var originalNonWorkingHoursLength = _context.JusticeUsers.Include(x => x.VhoNonAvailability)
                .First(x => x.Id == _justiceUser.Id).VhoNonAvailability.Count();

            var newHour1 = new
            {
                StartTime = new DateTime(2022, 2, 1, 6, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(2022, 2, 1, 10, 0, 0, DateTimeKind.Utc)
            };

            var newHours = new List<NonWorkHoursDto>
            {
                new(-1, newHour1.StartTime, newHour1.EndTime)
            };

            // Act
            await _commandHandler.Handle(new UpdateNonWorkingHoursCommand(justiceUserId, newHours));
            var newNonWorkingHoursLength = _context.JusticeUsers.Include(x => x.VhoNonAvailability)
                .First(x => x.Id == _justiceUser.Id).VhoNonAvailability.Count(x => x.JusticeUserId == justiceUserId);

            // Assert
            Assert.AreEqual(originalNonWorkingHoursLength + 1, newNonWorkingHoursLength);
        }

        [Test]
        public async Task Should_deallocate_hearings_when_users_no_longer_available()
        {
            // Arrange
            await SeedNonWorkingHours();
            var userId = _justiceUser.Id;
            var seededHearing = await Hooks.SeedVideoHearing();
            await Hooks.AddAllocation(seededHearing, _justiceUser);
            
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
            
            var newHours = new List<NonWorkHoursDto>
            {
                new(_hourIdMappings[1],newHour1.StartTime, newHour1.EndTime),
                new(_hourIdMappings[2],newHour2.StartTime, newHour2.EndTime)
            };
            
            // Act
            await _commandHandler.Handle(new UpdateNonWorkingHoursCommand(userId, newHours));
            
            // Assert
            hearing = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            hearing.AllocatedTo.Should().BeNull();
        }
        
        private async Task SeedNonWorkingHours()
        {
            _justiceUser = await Hooks
                .SeedJusticeUser(Username, "firstName", "secondname", true);

            _hourIdMappings = new Dictionary<long, long>();
            
            var existingHours = new List<NonWorkHoursDto>
            {
                new(1, new DateTime(2022, 1, 1, 6, 0, 0, DateTimeKind.Utc), new DateTime(2022, 1, 1, 10, 0, 0, DateTimeKind.Utc)),
                new(2, new DateTime(2022, 1, 2, 6, 0, 0, DateTimeKind.Utc), new DateTime(2022, 1, 2, 10, 0, 0, DateTimeKind.Utc))
            };

            _context.Attach(_justiceUser);
            foreach (var hour in existingHours)
            {
                _justiceUser.AddOrUpdateNonAvailability(hour.StartTime, hour.EndTime);

                await _context.SaveChangesAsync();

                _hourIdMappings[hour.Id] = _justiceUser.VhoNonAvailability.First(x => x.StartTime == hour.StartTime && x.EndTime == hour.EndTime).Id;
            }
        }
    }
}
