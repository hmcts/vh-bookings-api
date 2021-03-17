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
    public class UpdateJudiciaryPersonByExternalRefIdCommandTests : DatabaseTestsBase
    {
        private UpdateJudiciaryPersonByExternalRefIdHandler _commandHandler;
        private GetJudiciaryPersonByExternalRefIdQueryHandler _getJudiciaryPersonByExternalRefIdQueryHandler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new UpdateJudiciaryPersonByExternalRefIdHandler(context);
            _getJudiciaryPersonByExternalRefIdQueryHandler = new GetJudiciaryPersonByExternalRefIdQueryHandler(context);
        }
        
        [Test]
        public void should_throw_exception_when_peron_does_not_exist()
        {
            var command = new UpdateJudiciaryPersonByExternalRefIdCommand(Guid.NewGuid(), "123", "Mr", "Steve", "Allen", "Steve Allen", "nom1", "email1@email.com");
            Assert.ThrowsAsync<JudiciaryPersonNotFoundException>(() => _commandHandler.Handle(command));
        }
        
        [Test]
        public async Task should_update_person()
        {
            var externalRefId = Guid.NewGuid();
            await Hooks.AddJudiciaryPerson(externalRefId);

            const string changedText = "changed";
            var updateCommand = new UpdateJudiciaryPersonByExternalRefIdCommand(externalRefId, changedText, changedText, changedText, changedText, changedText, changedText, changedText);
            await _commandHandler.Handle(updateCommand);

            var updatePerson = await _getJudiciaryPersonByExternalRefIdQueryHandler.Handle(new GetJudiciaryPersonByExternalRefIdQuery(externalRefId));

            updatePerson.ExternalRefId.Should().Be(updateCommand.ExternalRefId);
            updatePerson.Email.Should().Be(changedText);
            updatePerson.Fullname.Should().Be(changedText);
            updatePerson.Surname.Should().Be(changedText);
            updatePerson.Title.Should().Be(changedText);
            updatePerson.KnownAs.Should().Be(changedText);
            updatePerson.PersonalCode.Should().Be(changedText);
            updatePerson.PostNominals.Should().Be(changedText);

            await Hooks.RemoveJudiciaryPersonAsync(updatePerson);
        }
    }
}