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
            const string addText = "add";
            var addCommand = new AddJudiciaryPersonCommand(externalRefId, addText, addText, addText, addText, addText, addText, addText);
            await _commandHandler.Handle(addCommand);

            var addedPerson = await _getJudiciaryPersonByExternalRefIdQueryHandler.Handle(new GetJudiciaryPersonByExternalRefIdQuery(externalRefId));

            addedPerson.ExternalRefId.Should().Be(addCommand.ExternalRefId);
            addedPerson.Email.Should().Be(addText);
            addedPerson.Fullname.Should().Be(addText);
            addedPerson.Surname.Should().Be(addText);
            addedPerson.Title.Should().Be(addText);
            addedPerson.KnownAs.Should().Be(addText);
            addedPerson.PersonalCode.Should().Be(addText);
            addedPerson.PostNominals.Should().Be(addText);

            await Hooks.RemoveJudiciaryPersonAsync(addedPerson);
        }
    }
}