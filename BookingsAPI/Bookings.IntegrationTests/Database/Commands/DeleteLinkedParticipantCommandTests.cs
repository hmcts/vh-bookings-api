using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL;
using Bookings.DAL.Commands;
using Bookings.DAL.Exceptions;
using Bookings.DAL.Queries;
using Bookings.Domain.Enumerations;
using Bookings.Domain.Participants;
using FluentAssertions;
using NUnit.Framework;

namespace Bookings.IntegrationTests.Database.Commands
{
    public class DeleteLinkedParticipantCommandTests : DatabaseTestsBase
    {
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;
        private DeleteLinkedParticipantCommandHandler _commandHandler;
        private Guid _hearingId;
        
        private List<Participant> _participantsWithLinks;
        
        [SetUp]
        public async Task SetUp()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new DeleteLinkedParticipantCommandHandler(context);
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(context);
            
            var seededHearing = await Hooks.SeedVideoHearing(
                null, 
                false, 
                BookingStatus.Created, 
                0, 
                false, 
                true);
            _hearingId = seededHearing.Id;
            TestContext.WriteLine($"New seeded video hearing id: {_hearingId}");
            
            _participantsWithLinks = new List<Participant>();
            _participantsWithLinks = seededHearing.Participants.Where(
                x => x.LinkedParticipants != null).ToList();
        }

        [Test]
        public async Task Should_Remove_Participants_Link()
        {
            var participant = _participantsWithLinks.First();
            var id = participant.LinkedParticipants.FirstOrDefault().Id;
            var command = new DeleteLinkedParticipantCommand(id);
            await _commandHandler.Handle(command);

            var returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(_hearingId));

            participant = returnedVideoHearing.Participants.Single(x => x.Id == participant.Id);

            participant.LinkedParticipants.Should().BeNullOrEmpty();
        }

        [Test]
        public void Should_Throw_Participant_Not_Found_Exception_When_Link_Not_Found()
        {
            var command = new DeleteLinkedParticipantCommand(Guid.NewGuid());
            _commandHandler.Invoking(async x => await x.Handle(command)).Should().Throw<ParticipantNotFoundException>();
        }
        
        [TearDown]
        public new async Task TearDown()
        {
            if (_hearingId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test hearing {_hearingId}");
                await Hooks.RemoveVideoHearing(_hearingId);
            }
        }
    }
}