using System;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class UpdateJudiciaryLeaverByExternalRefIdCommandTests : DatabaseTestsBase
    {
        private UpdateJudiciaryLeaverByExternalRefIdHandler _commandHandler;
        private GetJudiciaryPersonByExternalRefIdQueryHandler _getJudiciaryPersonByExternalRefIdQueryHandler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new UpdateJudiciaryLeaverByExternalRefIdHandler(context);
            _getJudiciaryPersonByExternalRefIdQueryHandler = new GetJudiciaryPersonByExternalRefIdQueryHandler(context);
        }
        
        [Test]
        public void should_throw_exception_when_peron_does_not_exist()
        {
            var command = new UpdateJudiciaryLeaverByExternalRefIdCommand(Guid.NewGuid(),true);
            Assert.ThrowsAsync<JudiciaryLeaverNotFoundException>(() => _commandHandler.Handle(command));
        }
        
        [Test]
        public async Task should_update_person()
        {
            var externalRefId = Guid.NewGuid();
            await Hooks.AddJudiciaryPerson(externalRefId);

            var updateCommand = new UpdateJudiciaryLeaverByExternalRefIdCommand(externalRefId, true);
            await _commandHandler.Handle(updateCommand);

            var updatePerson = await _getJudiciaryPersonByExternalRefIdQueryHandler.Handle(new GetJudiciaryPersonByExternalRefIdQuery(externalRefId));

            updatePerson.ExternalRefId.Should().Be(updateCommand.ExternalRefId);
            updatePerson.HasLeft.Should().BeTrue();
        }
    }
}