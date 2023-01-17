using System;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class AddJudiciaryPersonByPersonalCodeCommandTests : DatabaseTestsBase
    {
        private AddJudiciaryPersonByPersonalCodeHandler _commandHandler;
        private GetJudiciaryPersonByPersonalCodeQueryHandler _getJudiciaryPersonByPersonalCodeQueryHandler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new AddJudiciaryPersonByPersonalCodeHandler(context);
            _getJudiciaryPersonByPersonalCodeQueryHandler = new GetJudiciaryPersonByPersonalCodeQueryHandler(context);
        }

        [Test]
        public async Task should_add_person()
        {
            var externalRefId = Guid.NewGuid().ToString();
            var personalCode = Guid.NewGuid().ToString();

            var insertCommand = new AddJudiciaryPersonByPersonalCodeCommand(externalRefId, personalCode, "Title", "KnownAs", "Surname", "FullName", "PostNominals", "Email", true, true, "2022-06-08");
            await _commandHandler.Handle(insertCommand);

            var judiciaryPerson = await _getJudiciaryPersonByPersonalCodeQueryHandler.Handle(new GetJudiciaryPersonByPersonalCodeQuery(personalCode));

            judiciaryPerson.ExternalRefId.Should().Be(externalRefId);
            judiciaryPerson.PersonalCode.Should().Be(personalCode);
            judiciaryPerson.Title.Should().Be("Title");
            judiciaryPerson.KnownAs.Should().Be("KnownAs");
            judiciaryPerson.Surname.Should().Be("Surname");
            judiciaryPerson.Fullname.Should().Be("FullName");
            judiciaryPerson.PostNominals.Should().Be("PostNominals");
            judiciaryPerson.Email.Should().Be("Email");
            judiciaryPerson.HasLeft.Should().BeTrue();
            judiciaryPerson.CreatedDate.Should().Be(judiciaryPerson.UpdatedDate);
            judiciaryPerson.Leaver.Should().BeTrue();
            judiciaryPerson.LeftOn.Should().Be("2022-06-08");
        }
    }
}