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
    public class UpdateJudiciaryLeaverByPersonalCodeCommandTests : DatabaseTestsBase
    {
        private UpdateJudiciaryLeaverByPersonalCodeHandler _commandHandler;
        private GetJudiciaryPersonByPersonalCodeQueryHandler _getJudiciaryPersonByPersonalCodeQueryHandler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new UpdateJudiciaryLeaverByPersonalCodeHandler(context);
            _getJudiciaryPersonByPersonalCodeQueryHandler = new GetJudiciaryPersonByPersonalCodeQueryHandler(context);
        }
        
        [Test]
        public void should_throw_exception_when_peron_does_not_exist()
        {
            var command = new UpdateJudiciaryLeaverByPersonalCodeCommand(Guid.NewGuid().ToString(),true);
            Assert.ThrowsAsync<JudiciaryLeaverNotFoundException>(() => _commandHandler.Handle(command));
        }
        
        [Test]
        public async Task should_update_person()
        {
            var personalCode = Guid.NewGuid().ToString();
            await Hooks.AddJudiciaryPerson(personalCode);

            var updateCommand = new UpdateJudiciaryLeaverByPersonalCodeCommand(personalCode, true);
            await _commandHandler.Handle(updateCommand);

            var updatePerson = await _getJudiciaryPersonByPersonalCodeQueryHandler.Handle(new GetJudiciaryPersonByPersonalCodeQuery(personalCode));

            updatePerson.PersonalCode.Should().Be(updateCommand.PersonalCode);
            updatePerson.HasLeft.Should().BeTrue();
        }
    }
}