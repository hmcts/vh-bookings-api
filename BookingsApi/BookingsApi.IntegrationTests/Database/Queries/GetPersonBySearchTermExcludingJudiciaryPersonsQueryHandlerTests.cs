using BookingsApi.DAL;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetPersonBySearchTermExcludingJudiciaryPersonsQueryHandlerTests : DatabaseTestsBase
    {
        private GetPersonBySearchTermExcludingJudiciaryPersonsQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetPersonBySearchTermExcludingJudiciaryPersonsQueryHandler(context);
        }

        [Test]
        public async Task Returns_Person_List_Filtering_Out_JudiciaryPersons_With_Matching_Contact_Email()
        {
            var judiciaryPersonRefId = Guid.NewGuid();
            await Hooks.AddJudiciaryPersonWithoutAddingForCleanup(judiciaryPersonRefId);
            var judiciaryPersonFromDb = await Hooks.GetJudiciaryPerson(judiciaryPersonRefId);
            var judiciaryPersonEmail = judiciaryPersonFromDb.Email;
            await Hooks.AddPersonWithContactEmail(judiciaryPersonEmail);

            var matches = await _handler.Handle(new GetPersonBySearchTermExcludingJudiciaryPersonsQuery(judiciaryPersonEmail.Substring(0, 2), new List<string>()));

            matches.Select(m => m.ContactEmail).Should().NotContain(judiciaryPersonEmail);

            await Hooks.RemoveJudiciaryPersonFromPersonsTable(judiciaryPersonEmail.ToLowerInvariant());
            await Hooks.RemoveJudiciaryPersonsByExternalRefIdAsync(judiciaryPersonRefId);
        }

        [Test]
        public async Task Returns_Person_List_Filtering_Out_JudiciaryPersons_With_Matching_Username()
        {
            var judiciaryPersonRefId = Guid.NewGuid();
            await Hooks.AddJudiciaryPersonWithoutAddingForCleanup(judiciaryPersonRefId);
            var judiciaryPersonFromDb = await Hooks.GetJudiciaryPerson(judiciaryPersonRefId);
            var judiciaryPersonEmail = judiciaryPersonFromDb.Email;
            await Hooks.AddPersonWithUsernameAndDifferentContactEmail(judiciaryPersonEmail);

            var matches = await _handler.Handle(new GetPersonBySearchTermExcludingJudiciaryPersonsQuery(judiciaryPersonEmail.Substring(0, 2), new List<string>()));

            matches.Select(m => m.Username).Should().NotContain(judiciaryPersonEmail);

            await Hooks.RemoveJudiciaryPersonFromPersonsTable(judiciaryPersonEmail.ToLowerInvariant());
            await Hooks.RemoveJudiciaryPersonsByExternalRefIdAsync(judiciaryPersonRefId);
        }

        [Test]
        public async Task Finds_Contact_By_Search_Case_Insensitive_Term()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            var person = seededHearing.GetPersons().First();
            var contactEmail = person.ContactEmail;
            var twoCharactersLowercase = contactEmail.Substring(0, 2).ToLower();
            var twoCharactersUppercase = contactEmail.Substring(2, 2).ToUpper();
            var searchTerm = twoCharactersLowercase + twoCharactersUppercase;

            var matches = await _handler.Handle(new GetPersonBySearchTermExcludingJudiciaryPersonsQuery(searchTerm, new List<string>()));

            matches.Select(m => m.Id).Should().Contain(person.Id);
        }

        [Test]
        public async Task Returns_Person_List_Filtering_Out_Specified_AD_Users()
        {
            var judiciaryPersonEmail = "john.doe@judiciary.hmcts.net";
            await Hooks.AddPersonWithContactEmail(judiciaryPersonEmail);
            var adUsers = new List<string> { judiciaryPersonEmail };

            var matches = await _handler.Handle(new GetPersonBySearchTermExcludingJudiciaryPersonsQuery("joh", adUsers));

            matches.Select(m => m.ContactEmail).Should().NotContain(judiciaryPersonEmail);

            await Hooks.RemoveJudiciaryPersonFromPersonsTable(judiciaryPersonEmail.ToLowerInvariant());
        }

        [Test]
        public async Task Returns_Person_List_Filtering_Out_Specified_AD_Users_And_Judiciary_Persons()
        {

            //judiciary person for the test
            var judiciaryPersonRefId = Guid.NewGuid();
            await Hooks.AddJudiciaryPersonWithoutAddingForCleanup(judiciaryPersonRefId);
            var judiciaryPersonFromDb = await Hooks.GetJudiciaryPerson(judiciaryPersonRefId);
            var judiciaryPersonEmail = judiciaryPersonFromDb.Email;
            await Hooks.AddPersonWithContactEmail(judiciaryPersonEmail);

            //search term for the test
            var twoCharactersLowercase = judiciaryPersonEmail.Substring(0, 2).ToLower();
            var twoCharactersUppercase = judiciaryPersonEmail.Substring(2, 2).ToUpper();
            var searchTerm = twoCharactersLowercase + twoCharactersUppercase;

            //ad user for the test
            var judiciaryInADEmail = $"{searchTerm}_john.doe@judiciary.hmcts.net";
            await Hooks.AddPersonWithContactEmail(judiciaryInADEmail);
            var adUsers = new List<string> { judiciaryInADEmail };

            var matches = await _handler.Handle(new GetPersonBySearchTermExcludingJudiciaryPersonsQuery(searchTerm, adUsers));

            matches.Select(m => m.ContactEmail).Should().NotContain(judiciaryInADEmail);
            matches.Select(m => m.ContactEmail).Should().NotContain(judiciaryPersonEmail);

            await Hooks.RemoveJudiciaryPersonFromPersonsTable(judiciaryInADEmail.ToLowerInvariant());
            await Hooks.RemoveJudiciaryPersonFromPersonsTable(judiciaryPersonEmail.ToLowerInvariant());
            await Hooks.RemoveJudiciaryPersonsByExternalRefIdAsync(judiciaryPersonRefId);
        }
    }

}
