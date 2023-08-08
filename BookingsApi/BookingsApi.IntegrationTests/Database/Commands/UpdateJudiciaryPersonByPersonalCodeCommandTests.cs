using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class UpdateJudiciaryPersonByPersonalCodeCommandTests : DatabaseTestsBase
    {
        private UpdateJudiciaryPersonByPersonalCodeHandler _commandHandler;
        private GetJudiciaryPersonByPersonalCodeQueryHandler _getJudiciaryPersonByPersonalCodeQueryHandler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new UpdateJudiciaryPersonByPersonalCodeHandler(context);
            _getJudiciaryPersonByPersonalCodeQueryHandler = new GetJudiciaryPersonByPersonalCodeQueryHandler(context);
        }
        
        [Test]
        public void should_throw_exception_when_peron_does_not_exist()
        {
            var command = new UpdateJudiciaryPersonByPersonalCodeCommand{PersonalCode = "asdasd"};
            Assert.ThrowsAsync<JudiciaryPersonNotFoundException>(() => _commandHandler.Handle(command));
        }
        
        [Test]
        public async Task should_update_person()
        {
            var externalRefId = Guid.NewGuid().ToString();
            var personalCode = Guid.NewGuid().ToString();
            await Hooks.AddJudiciaryPerson(personalCode);

            var updateCommand = new UpdateJudiciaryPersonByPersonalCodeCommand
            {
                ExternalRefId =externalRefId,
                PersonalCode =personalCode,
                Title ="Title",
                KnownAs ="KnownAs",
                Fullname ="Fullname",
                Surname ="Surname",
                PostNominals ="PostNominals",
                Email ="Email",
                Leaver = true,
                LeftOn ="LeftOn",
                    
            };
            await _commandHandler.Handle(updateCommand);

            var updatePerson = await _getJudiciaryPersonByPersonalCodeQueryHandler.Handle(new GetJudiciaryPersonByPersonalCodeQuery(personalCode));
            
            updatePerson.ExternalRefId.Should().Be(updateCommand.ExternalRefId);
            updatePerson.PersonalCode.Should().Be(updateCommand.PersonalCode);
            updatePerson.Title.Should().Be(updateCommand.Title);
            updatePerson.KnownAs.Should().Be(updateCommand.KnownAs);
            updatePerson.Fullname.Should().Be(updateCommand.Fullname);
            updatePerson.Surname.Should().Be(updateCommand.Surname);
            updatePerson.PostNominals.Should().Be(updateCommand.PostNominals);
            updatePerson.Email.Should().Be(updateCommand.Email);
            updatePerson.LeftOn.Should().Be(updateCommand.LeftOn);
            updatePerson.HasLeft.Should().BeTrue();
        }
    }
}