using BookingsApi.Common.Services;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Services;
using BookingsApi.Domain.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using DayOfWeek = System.DayOfWeek;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class UpdateHearingCommandDatabaseTests : DatabaseTestsBase
    {
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;
        private GetHearingVenuesQueryHandler _getHearingVenuesQueryHandler;
        private UpdateHearingCommandHandler _commandHandler;
        private Guid _newHearingId;
        private BookingsDbContext _context;

        [SetUp]
        public void Setup()
        {
            _context = new BookingsDbContext(BookingsDbContextOptions);
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(_context);
            _getHearingVenuesQueryHandler = new GetHearingVenuesQueryHandler(_context);
            var randomNumberGenerator = new RandomNumberGenerator();
            var allocationConfiguration = GetDefaultAllocationSettings();
            var hearingAllocationService = new HearingAllocationService(_context, 
                randomNumberGenerator, 
                new OptionsWrapper<AllocateHearingConfiguration>(allocationConfiguration),
                new NullLogger<HearingAllocationService>());
            _commandHandler = new UpdateHearingCommandHandler(_context, hearingAllocationService);
            _newHearingId = Guid.Empty;
        }
        
        [Test]
        public async Task Should_be_able_to_update_video_hearing()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;

            var allVenues = await _getHearingVenuesQueryHandler.Handle(new GetHearingVenuesQuery());
            var newVenue = allVenues.Last();
            var newDuration = seededHearing.ScheduledDuration + 10;
            var newDateTime = seededHearing.ScheduledDateTime.AddDays(1);
            var newHearingRoomName = "Room02 edit";
            var newOtherInformation = "OtherInformation02 edit";
            var updatedBy = "testuser";
            var casesToUpdate = new List<Case>();
            var caseName = "CaseName Update";
            var caseNumber = "CaseNumber Update";
            casesToUpdate.Add(new Case(caseNumber, caseName));
            const bool audioRecordingRequired = true;

            await _commandHandler.Handle(new UpdateHearingCommand(_newHearingId, newDateTime, newDuration, 
                        newVenue, newHearingRoomName, newOtherInformation, updatedBy, casesToUpdate, audioRecordingRequired));
            
            var returnedVideoHearing = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));

            returnedVideoHearing.HearingVenue.Name.Should().Be(newVenue.Name);
            returnedVideoHearing.ScheduledDuration.Should().Be(newDuration);
            returnedVideoHearing.ScheduledDateTime.Should().Be(newDateTime);
            returnedVideoHearing.HearingRoomName.Should().Be(newHearingRoomName);
            returnedVideoHearing.OtherInformation.Should().Be(newOtherInformation);
            returnedVideoHearing.GetCases().First().Name.Should().Be(caseName);
            returnedVideoHearing.GetCases().First().Number.Should().Be(caseNumber);
            returnedVideoHearing.AudioRecordingRequired.Should().BeTrue();
            returnedVideoHearing.UpdatedDate.Should().BeAfter(returnedVideoHearing.CreatedDate);
        }

        [Test]
        public async Task Should_deallocate_when_scheduled_datetime_changes_and_user_is_not_available_due_to_work_hours()
        {
            var daysOfWeek = await _context.DaysOfWeek.ToListAsync();
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;
            var allocatedUser = await Hooks.SeedJusticeUser("cso@email.com", "Cso", "Test");
            await Hooks.AddAllocation(seededHearing, allocatedUser);

            _context.Attach(allocatedUser); // re-attach to avoid another query to db
            for (var i = 1; i <= 7; i++)
            {
                var dayOfWeek = daysOfWeek.First(x => x.Id == i);
                allocatedUser.AddOrUpdateWorkHour(dayOfWeek, new TimeSpan(1, 0, 0), new TimeSpan(2, 0, 0));
            }

            await _context.SaveChangesAsync();
            var hearing = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            hearing.AllocatedTo.Should().NotBeNull();
            hearing.AllocatedTo.Id.Should().Be(allocatedUser.Id);

            var allVenues = await _getHearingVenuesQueryHandler.Handle(new GetHearingVenuesQuery());
            var newVenue = allVenues.Last();
            var newDateTime = seededHearing.ScheduledDateTime.AddDays(1);
            var updatedBy = "testuser";
            var casesToUpdate = new List<Case>();
            
            await _commandHandler.Handle(new UpdateHearingCommand(_newHearingId, newDateTime, seededHearing.ScheduledDuration, 
                newVenue, seededHearing.HearingRoomName, seededHearing.OtherInformation, updatedBy, casesToUpdate, seededHearing.AudioRecordingRequired));
            
            var returnedVideoHearing = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));

            returnedVideoHearing.AllocatedTo.Should().BeNull();
        }

        [Test]
        public async Task Should_deallocate_when_scheduled_datetime_changes_and_user_is_not_available_due_to_non_availability()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;
            var allocatedUser = await Hooks.SeedJusticeUser("cso@email.com", "Cso", "Test", initWorkHours: false);
            await Hooks.AddAllocation(seededHearing, allocatedUser);
            allocatedUser.AddOrUpdateNonAvailability(
                new DateTime(seededHearing.ScheduledDateTime.Year, seededHearing.ScheduledDateTime.Month, seededHearing.ScheduledDateTime.Day, 0, 0, 0),
                new DateTime(seededHearing.ScheduledDateTime.Year, seededHearing.ScheduledDateTime.Month, seededHearing.ScheduledDateTime.Day, 23, 59, 59)
                );

            await _context.SaveChangesAsync();
            var hearing = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            hearing.AllocatedTo.Should().NotBeNull();
            hearing.AllocatedTo.Id.Should().Be(allocatedUser.Id);

            var allVenues = await _getHearingVenuesQueryHandler.Handle(new GetHearingVenuesQuery());
            var newVenue = allVenues.Last();
            var newDateTime = seededHearing.ScheduledDateTime.AddDays(1);
            var updatedBy = "testuser";
            var casesToUpdate = new List<Case>();
            
            await _commandHandler.Handle(new UpdateHearingCommand(_newHearingId, newDateTime, seededHearing.ScheduledDuration, 
                newVenue, seededHearing.HearingRoomName, seededHearing.OtherInformation, updatedBy, casesToUpdate, seededHearing.AudioRecordingRequired));
            
            var returnedVideoHearing = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));

            returnedVideoHearing.AllocatedTo.Should().BeNull();
        }

        [Test]
        public async Task Should_not_deallocate_when_scheduled_datetime_changes_and_user_is_available_due_to_work_hours()
        {
            var daysOfWeek = await _context.DaysOfWeek.ToListAsync();
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;
            var allocatedUser = await Hooks.SeedJusticeUser("cso@email.com", "Cso", "Test");
            await Hooks.AddAllocation(seededHearing, allocatedUser);
            
            for (var i = 1; i <= 7; i++)
            {
                var dayOfWeek = daysOfWeek.First(x => x.Id == i);
                allocatedUser.AddOrUpdateWorkHour(dayOfWeek, new TimeSpan(0, 0, 0), new TimeSpan(23, 59, 59));
            }
            allocatedUser.AddOrUpdateNonAvailability(
                new DateTime(seededHearing.ScheduledDateTime.Year, seededHearing.ScheduledDateTime.Month, seededHearing.ScheduledDateTime.Day, 1, 0, 0),
                new DateTime(seededHearing.ScheduledDateTime.Year, seededHearing.ScheduledDateTime.Month, seededHearing.ScheduledDateTime.Day, 2, 0, 0)
            );
            
            await _context.SaveChangesAsync();
            var hearing = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            hearing.AllocatedTo.Should().NotBeNull();
            hearing.AllocatedTo.Id.Should().Be(allocatedUser.Id);

            var allVenues = await _getHearingVenuesQueryHandler.Handle(new GetHearingVenuesQuery());
            var newVenue = allVenues.Last();
            var newDateTime = GetNextWorkingDay(seededHearing.ScheduledDateTime);
            var updatedBy = "testuser";
            var casesToUpdate = new List<Case>();
            
            await _commandHandler.Handle(new UpdateHearingCommand(_newHearingId, newDateTime, seededHearing.ScheduledDuration, 
                newVenue, seededHearing.HearingRoomName, seededHearing.OtherInformation, updatedBy, casesToUpdate, seededHearing.AudioRecordingRequired));
            
            var returnedVideoHearing = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));

            returnedVideoHearing.AllocatedTo.Should().NotBeNull();
            returnedVideoHearing.AllocatedTo.Id.Should().Be(allocatedUser.Id);
        }

        [Test]
        public async Task Should_not_deallocate_when_scheduled_datetime_has_not_changed()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;
            var allocatedUser = await Hooks.SeedJusticeUser("cso@email.com", "Cso", "Test");
            await Hooks.AddAllocation(seededHearing, allocatedUser);
            var hearing = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            hearing.AllocatedTo.Should().NotBeNull();
            hearing.AllocatedTo.Id.Should().Be(allocatedUser.Id);
            
            var allVenues = await _getHearingVenuesQueryHandler.Handle(new GetHearingVenuesQuery());
            var newVenue = allVenues.Last();
            var updatedBy = "testuser";
            var casesToUpdate = new List<Case>();
            
            await _commandHandler.Handle(new UpdateHearingCommand(_newHearingId, seededHearing.ScheduledDateTime, seededHearing.ScheduledDuration, 
                newVenue, seededHearing.HearingRoomName, seededHearing.OtherInformation, updatedBy, casesToUpdate, seededHearing.AudioRecordingRequired));
            
            var returnedVideoHearing = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));

            returnedVideoHearing.AllocatedTo.Should().NotBeNull();
            returnedVideoHearing.AllocatedTo.Id.Should().Be(allocatedUser.Id);
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
        
        private static DateTime GetNextWorkingDay(DateTime startingDateTime)
        {
            var newDateTime = startingDateTime.AddDays(1);

            if (newDateTime.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            {
                newDateTime = newDateTime.AddDays(2);
            }
            return newDateTime;
        }
    }
}