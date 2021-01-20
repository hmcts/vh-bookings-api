using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL;
using Bookings.DAL.Commands;
using Bookings.DAL.Queries;
using Bookings.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace Bookings.IntegrationTests.Database.Commands
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
        }
        
        [TearDown]
        public new async Task TearDown()
        {
            if (_newHearingId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test hearing {_newHearingId}");
                await Hooks.RemoveVideoHearing(_newHearingId);
            }
        }
    }
}