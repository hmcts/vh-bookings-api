using System;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
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
        public async Task should_add_person()
        {
            var externalRefId = Guid.NewGuid();

            var insertCommand = new AddJudiciaryPersonByExternalRefIdCommand(externalRefId, "PersonalCode", "Title", "KnownAs", "Surname", "FullName", "PostNominals", "Email", true);
            await _commandHandler.Handle(insertCommand);

            var judiciaryPerson = await _getJudiciaryPersonByExternalRefIdQueryHandler.Handle(new GetJudiciaryPersonByExternalRefIdQuery(externalRefId));

            judiciaryPerson.ExternalRefId.Should().Be(externalRefId);
            judiciaryPerson.PersonalCode.Should().Be("PersonalCode");
            judiciaryPerson.Title.Should().Be("Title");
            judiciaryPerson.KnownAs.Should().Be("KnownAs");
            judiciaryPerson.Surname.Should().Be("Surname");
            judiciaryPerson.Fullname.Should().Be("FullName");
            judiciaryPerson.PostNominals.Should().Be("PostNominals");
            judiciaryPerson.Email.Should().Be("Email");
            judiciaryPerson.HasLeft.Should().BeTrue();
            judiciaryPerson.CreatedDate.Should().Be(judiciaryPerson.UpdatedDate);
        }
    }
}