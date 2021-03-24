using System;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class AddJudiciaryPersonCommandTests : DatabaseTestsBase
    {
        private AddJudiciaryPersonCommandHandler _commandHandler;
        private GetJudiciaryPersonByExternalRefIdQueryHandler _getJudiciaryPersonByExternalRefIdQueryHandler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new AddJudiciaryPersonCommandHandler(context);
            _getJudiciaryPersonByExternalRefIdQueryHandler = new GetJudiciaryPersonByExternalRefIdQueryHandler(context);
        }
        
        [Test]
        public async Task should_add_person()
        {
            var externalRefId = Guid.NewGuid();
            var addCommand = new AddJudiciaryPersonCommand(externalRefId, "PersonalCode", "Title", "KnownAs", "Surname", "FullName", "PostNominals", "Email");
            
            await _commandHandler.Handle(addCommand);
            Hooks.AddJudiciaryPersonsForCleanup(externalRefId);
            
            var addedPerson = await _getJudiciaryPersonByExternalRefIdQueryHandler.Handle(new GetJudiciaryPersonByExternalRefIdQuery(externalRefId));

            addedPerson.ExternalRefId.Should().Be(addCommand.ExternalRefId);
            addedPerson.PersonalCode.Should().Be("PersonalCode");
            addedPerson.Title.Should().Be("Title");
            addedPerson.KnownAs.Should().Be("KnownAs");
            addedPerson.Surname.Should().Be("Surname");
            addedPerson.Fullname.Should().Be("FullName");
            addedPerson.PostNominals.Should().Be("PostNominals");
            addedPerson.Email.Should().Be("Email");
        }
    }
}