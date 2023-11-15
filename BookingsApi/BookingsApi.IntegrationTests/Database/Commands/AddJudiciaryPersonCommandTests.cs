using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Queries;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class AddJudiciaryPersonCommandTests : DatabaseTestsBase
    {
        private AddJudiciaryPersonCommandHandler _commandHandler;
        private GetJudiciaryPersonByPersonalCodeQueryHandler _getJudiciaryPersonByPersonalCodeQueryHandler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new AddJudiciaryPersonCommandHandler(context);
            _getJudiciaryPersonByPersonalCodeQueryHandler = new GetJudiciaryPersonByPersonalCodeQueryHandler(context);
        }
        
        [Test]
        public async Task should_add_non_leaver_person()
        {
            var externalRefId = Guid.NewGuid().ToString();
            var personalCode = Guid.NewGuid().ToString();
            var addCommand = new AddJudiciaryPersonCommand(externalRefId, personalCode, "Title", "KnownAs", "Surname",
                "FullName", "PostNominals", "Email", "01234567890", false, false, string.Empty);
            
            await _commandHandler.Handle(addCommand);
            Hooks.AddJudiciaryPersonsForCleanup(externalRefId);
            
            var addedPerson = await _getJudiciaryPersonByPersonalCodeQueryHandler.Handle(new GetJudiciaryPersonByPersonalCodeQuery(personalCode));

            addedPerson.ExternalRefId.Should().Be(addCommand.ExternalRefId);
            addedPerson.PersonalCode.Should().Be(addCommand.PersonalCode);
            addedPerson.Title.Should().Be("Title");
            addedPerson.KnownAs.Should().Be("KnownAs");
            addedPerson.Surname.Should().Be("Surname");
            addedPerson.Fullname.Should().Be("FullName");
            addedPerson.PostNominals.Should().Be("PostNominals");
            addedPerson.Email.Should().Be("Email");
            addedPerson.WorkPhone.Should().Be("01234567890");
            addedPerson.HasLeft.Should().BeFalse();
            addedPerson.Leaver.Should().BeFalse();
            addedPerson.LeftOn.Should().Be(string.Empty);
        }
        
        [Test]
        public async Task should_add_leaver_person()
        {
            var externalRefId = Guid.NewGuid().ToString();
            var personalCode = Guid.NewGuid().ToString();
            var addCommand = new AddJudiciaryPersonCommand(externalRefId, personalCode, string.Empty, string.Empty,
                string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, false, true, "2022-06-08");
            
            await _commandHandler.Handle(addCommand);
            Hooks.AddJudiciaryPersonsForCleanup(externalRefId);
            
            var addedPerson = await _getJudiciaryPersonByPersonalCodeQueryHandler.Handle(new GetJudiciaryPersonByPersonalCodeQuery(personalCode));

            addedPerson.ExternalRefId.Should().Be(addCommand.ExternalRefId);
            addedPerson.PersonalCode.Should().Be(personalCode);
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