using System;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
using Bookings.DAL;
using Bookings.DAL.Commands;
using Bookings.DAL.Exceptions;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;

namespace Bookings.IntegrationTests.Database.Commands
{
    public class DeleteLinkedParticipantCommandTests : DatabaseTestsBase
    {
        private DeleteLinkedParticipantCommandHandler _commandHandler;
        private RemoveLinkedParticipantRequest _request;

        [SetUp]
        public async Task SetUp()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new DeleteLinkedParticipantCommandHandler(context);
        
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            
            _request = Builder<RemoveLinkedParticipantRequest>.CreateNew()
                .With(x => x.Id)
                .Build();
        }

        [Test]
        public async Task Should_Delete_LinkedParticipant_In_Database()
        {
            var command = new DeleteLinkedParticipantCommand(_request.Id);
            await _commandHandler.Handle(command);
            
            // Verify whether the command has been successful
        }
        
        [Test]
        public void Should_Throw_ParticipantNotFoundException_If_LinkedParticipant_Is_Null()
        {
            var command = new DeleteLinkedParticipantCommand(Guid.NewGuid());
            Assert.ThrowsAsync<ParticipantNotFoundException>(() => _commandHandler.Handle(command));
        }
    }
}