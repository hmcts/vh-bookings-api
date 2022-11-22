using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class UpdateHearingCommandDatabaseTests : DatabaseTestsBase
    {
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;
        private GetHearingVenuesQueryHandler _getHearingVenuesQueryHandler;
        private UpdateHearingCommandHandler _commandHandler;
        private Guid _newHearingId;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(context);
            _getHearingVenuesQueryHandler = new GetHearingVenuesQueryHandler(context);
            _commandHandler = new UpdateHearingCommandHandler(context);
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
            const bool questionnaireNotRequired = false;
            const bool audioRecordingRequired = true;

            await _commandHandler.Handle(new UpdateHearingCommand(_newHearingId, newDateTime, newDuration, 
                        newVenue, newHearingRoomName, newOtherInformation, updatedBy, casesToUpdate,
                        questionnaireNotRequired, audioRecordingRequired));
            
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
        public async Task Should_deallocate_when_scheduled_datetime_changes()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;
            var allocatedUser = await Hooks.SeedJusticeUser("cso@email.com", "Cso", "Test");
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            db.Allocations.Add(new Allocation
            {
                HearingId = seededHearing.Id,
                JusticeUserId = allocatedUser.Id
            });
            await db.SaveChangesAsync();
            var hearing = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            hearing.AllocatedTo.Should().NotBeNull();
            hearing.AllocatedTo.Id.Should().Be(allocatedUser.Id);

            var allVenues = await _getHearingVenuesQueryHandler.Handle(new GetHearingVenuesQuery());
            var newVenue = allVenues.Last();
            var newDateTime = seededHearing.ScheduledDateTime.AddDays(1);
            var updatedBy = "testuser";
            var casesToUpdate = new List<Case>();
            
            await _commandHandler.Handle(new UpdateHearingCommand(_newHearingId, newDateTime, seededHearing.ScheduledDuration, 
                newVenue, seededHearing.HearingRoomName, seededHearing.OtherInformation, updatedBy, casesToUpdate,
                seededHearing.QuestionnaireNotRequired, seededHearing.AudioRecordingRequired));
            
            var returnedVideoHearing = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));

            returnedVideoHearing.AllocatedTo.Should().BeNull();
        }

        [Test]
        public async Task Should_not_deallocate_when_scheduled_datetime_has_not_changed()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;
            var allocatedUser = await Hooks.SeedJusticeUser("cso@email.com", "Cso", "Test");
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            db.Allocations.Add(new Allocation
            {
                HearingId = seededHearing.Id,
                JusticeUserId = allocatedUser.Id
            });
            await db.SaveChangesAsync();
            var hearing = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            hearing.AllocatedTo.Should().NotBeNull();
            hearing.AllocatedTo.Id.Should().Be(allocatedUser.Id);
            
            var allVenues = await _getHearingVenuesQueryHandler.Handle(new GetHearingVenuesQuery());
            var newVenue = allVenues.Last();
            var updatedBy = "testuser";
            var casesToUpdate = new List<Case>();
            
            await _commandHandler.Handle(new UpdateHearingCommand(_newHearingId, seededHearing.ScheduledDateTime, seededHearing.ScheduledDuration, 
                newVenue, seededHearing.HearingRoomName, seededHearing.OtherInformation, updatedBy, casesToUpdate,
                seededHearing.QuestionnaireNotRequired, seededHearing.AudioRecordingRequired));
            
            var returnedVideoHearing = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));

            returnedVideoHearing.AllocatedTo.Should().NotBeNull();
            returnedVideoHearing.AllocatedTo.Id.Should().Be(allocatedUser.Id);
        }
    }
}