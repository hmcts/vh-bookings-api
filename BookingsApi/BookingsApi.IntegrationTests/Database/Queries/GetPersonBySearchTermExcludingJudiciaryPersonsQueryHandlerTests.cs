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

            var matches = await _handler.Handle(new GetPersonBySearchTermExcludingJudiciaryPersonsQuery(judiciaryPersonEmail.Substring(0, 2)));

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

            var matches = await _handler.Handle(new GetPersonBySearchTermExcludingJudiciaryPersonsQuery(judiciaryPersonEmail.Substring(0, 2)));

            matches.Select(m => m.Username).Should().NotContain(judiciaryPersonEmail);

            await Hooks.RemoveJudiciaryPersonFromPersonsTable(judiciaryPersonEmail.ToLowerInvariant());
            await Hooks.RemoveJudiciaryPersonsByExternalRefIdAsync(judiciaryPersonRefId);
        }

        //[Test]
        //public async Task Returns_Person_List_Filtering_Out_AD_Users()
        //{
        //}
    }

}
