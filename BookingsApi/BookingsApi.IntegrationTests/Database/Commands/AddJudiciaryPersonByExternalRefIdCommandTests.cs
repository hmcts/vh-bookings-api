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
    public class AddJudiciaryPersonByExternalRefIdCommandTests : DatabaseTestsBase
    {
        private AddJudiciaryPersonByExternalRefIdHandler _commandHandler;
        private GetJudiciaryPersonByExternalRefIdQueryHandler _getJudiciaryPersonByExternalRefIdQueryHandler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new AddJudiciaryPersonByExternalRefIdHandler(context);
            _getJudiciaryPersonByExternalRefIdQueryHandler = new GetJudiciaryPersonByExternalRefIdQueryHandler(context);
        }
        
        [Test]
        public void should_throw_exception_when_peron_does_not_exist()
        {
            var command = new AddJudiciaryPersonByExternalRefIdCommand(Guid.NewGuid(), "123", "Mr", "Steve", "Allen", "Steve Allen", "nom1", "email1@email.com", false);
            Assert.ThrowsAsync<JudiciaryPersonNotFoundException>(() => _commandHandler.Handle(command));
        }
        
        [Test]
        public async Task should_update_person()
        {
            var externalRefId = Guid.NewGuid();
            await Hooks.AddJudiciaryPerson(externalRefId);

            var updateCommand = new AddJudiciaryPersonByExternalRefIdCommand(externalRefId, "PersonalCode", "Title", "KnownAs", "Surname", "FullName", "PostNominals", "Email", true);
            await _commandHandler.Handle(updateCommand);

            var updatePerson = await _getJudiciaryPersonByExternalRefIdQueryHandler.Handle(new GetJudiciaryPersonByExternalRefIdQuery(externalRefId));

            updatePerson.ExternalRefId.Should().Be(updateCommand.ExternalRefId);
            updatePerson.PersonalCode.Should().Be("PersonalCode");
            updatePerson.Title.Should().Be("Title");
            updatePerson.KnownAs.Should().Be("KnownAs");
            updatePerson.Surname.Should().Be("Surname");
            updatePerson.Fullname.Should().Be("FullName");
            updatePerson.PostNominals.Should().Be("PostNominals");
            updatePerson.Email.Should().Be("Email");
            updatePerson.HasLeft.Should().BeTrue();
        }
    }
}