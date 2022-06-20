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
        public async Task should_add_non_leaver_person()
        {
            var externalRefId = Guid.NewGuid().ToString();
            var addCommand = new AddJudiciaryPersonCommand(externalRefId, "PersonalCode", "Title", "KnownAs", "Surname", "FullName", "PostNominals", "Email", false, false, string.Empty);
            
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
            addedPerson.HasLeft.Should().BeFalse();
            addedPerson.Leaver.Should().BeFalse();
            addedPerson.LeftOn.Should().Be(string.Empty);
        }
        
        [Test]
        public async Task should_add_leaver_person()
        {
            var externalRefId = Guid.NewGuid().ToString();
            var addCommand = new AddJudiciaryPersonCommand(externalRefId, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, false, true, "2022-06-08");
            
            await _commandHandler.Handle(addCommand);
            Hooks.AddJudiciaryPersonsForCleanup(externalRefId);
            
            var addedPerson = await _getJudiciaryPersonByExternalRefIdQueryHandler.Handle(new GetJudiciaryPersonByExternalRefIdQuery(externalRefId));

            addedPerson.ExternalRefId.Should().Be(addCommand.ExternalRefId);
            addedPerson.PersonalCode.Should().Be(string.Empty);
            addedPerson.Title.Should().Be(string.Empty);
            addedPerson.KnownAs.Should().Be(string.Empty);
            addedPerson.Surname.Should().Be(string.Empty);
            addedPerson.Fullname.Should().Be(string.Empty);
            addedPerson.PostNominals.Should().Be(string.Empty);
            addedPerson.Email.Should().Be(string.Empty);
            addedPerson.HasLeft.Should().BeFalse();
            addedPerson.Leaver.Should().BeTrue();
            addedPerson.LeftOn.Should().Be("2022-06-08");
        }
    }
}