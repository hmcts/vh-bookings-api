using System.Text;
using AcceptanceTests.Common.Api.Helpers;
using BookingsApi.Contract.V1.Requests;
using Microsoft.EntityFrameworkCore;
using TechTalk.SpecFlow;
using static Testing.Common.Builders.Api.ApiUriFactory.JudiciaryPersonsEndpoints;
using TestContext = BookingsApi.IntegrationTests.Contexts.TestContext;

namespace BookingsApi.IntegrationTests.Steps
{
    [Binding]
    public class JudiciaryPersonsSteps : BaseSteps
    {
        public JudiciaryPersonsSteps(TestContext apiTestContext) : base(apiTestContext)
        {
        }
        
        [Given(@"I have a post bulk judiciary persons request")]
        public void GivenIHaveAPostBulkJudiciaryPersonsRequest()
        {
            Context.Uri = BulkJudiciaryPersons();
            Context.HttpMethod = HttpMethod.Post;
            var request = GetBulkRequest();
            
            TestDataManager.AddJudiciaryPersonsForCleanup(request.Select(x => x.PersonalCode).ToArray());
            Context.HttpContent = new StringContent(RequestHelper.Serialise(request), Encoding.UTF8, "application/json");
        }
        
        [Then(@"the judiciary persons should be saved")]
        public async Task AndTheJudiciaryPersonsShouldBeSaved()
        {
            await using var db = new BookingsDbContext(Context.BookingsDbContextOptions);
            var jps = await db.JudiciaryPersons.Where(x => TestDataManager.JudiciaryPersons.Contains(x.PersonalCode)).ToListAsync();
            jps.ForEach(Assert.NotNull);
        }

        private static List<JudiciaryPersonRequest> GetBulkRequest()
        {
            return new List<JudiciaryPersonRequest>
            {
                new JudiciaryPersonRequest {Id = Guid.NewGuid().ToString(), Email = "test.email.com", Fullname = "ccc ddd", KnownAs = "ccc", Surname = "ddd", Title = "mr", PersonalCode = "123", PostNominals = "postnom1"},
                new JudiciaryPersonRequest {Id = Guid.NewGuid().ToString(), Email = "test.email.com", Fullname = "eee fff", KnownAs = "eee", Surname = "fff", Title = "mr", PersonalCode = "123", PostNominals = "postnom1"},
                new JudiciaryPersonRequest {Id = Guid.NewGuid().ToString(), Email = "test.email.com", Fullname = "ggg hhh", KnownAs = "ggg", Surname = "hhh", Title = "mr", PersonalCode = "123", PostNominals = "postnom1"}
            };
        }
    }
}