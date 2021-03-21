using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using BookingsApi.DAL;
using BookingsApi.IntegrationTests.Helper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TechTalk.SpecFlow;
using Testing.Common.Configuration;
using static Testing.Common.Builders.Api.ApiUriFactory.JudiciaryPersonsEndpoints;
using TestContext = BookingsApi.IntegrationTests.Contexts.TestContext;

namespace BookingsApi.IntegrationTests.Steps
{
    [Binding]
    public class JudiciaryPersonsSteps : BaseSteps
    {
        private readonly TestContext _apiTestContext;
        private readonly List<Guid> _judiciaryPersonsExternalIds;
        
        public JudiciaryPersonsSteps(TestContext apiTestContext) : base(apiTestContext)
        {
            _apiTestContext = apiTestContext;
            _judiciaryPersonsExternalIds = new List<Guid>();
        }
        
        [Given(@"I have a post bulk judiciary persons request")]
        public void GivenIHaveAPostBulkJudiciaryPersonsRequest()
        {
            Context.Uri = BulkJudiciaryPersons();
            Context.HttpMethod = HttpMethod.Post;
            var request = GetBulkRequest();
            _apiTestContext.TestData.JudiciaryPersonsExternalIds = request.Select(x => x.Id).ToList();
            Context.HttpContent = new StringContent(RequestHelper.Serialise(request), Encoding.UTF8, "application/json");
        }
        
        [Then(@"the judiciary persons should be saved")]
        public async Task AndTheJudiciaryPersonsShouldBeSaved()
        {
            foreach (var id in _apiTestContext.TestData.JudiciaryPersonsExternalIds)
            {
                await using var db = new BookingsDbContext(Context.BookingsDbContextOptions);
                var jp = await db.JudiciaryPersons.SingleOrDefaultAsync(x => x.ExternalRefId == id);
                Assert.NotNull(jp);
            }
        }

        private List<JudiciaryPersonRequest> GetBulkRequest()
        {
            return new List<JudiciaryPersonRequest>
            {
                new JudiciaryPersonRequest {Id = Guid.NewGuid(), Email = "test.email.com", Fullname = "ccc ddd", KnownAs = "ccc", Surname = "ddd", Title = "mr", PersonalCode = "123", PostNominals = "postnom1"},
                new JudiciaryPersonRequest {Id = Guid.NewGuid(), Email = "test.email.com", Fullname = "eee fff", KnownAs = "eee", Surname = "fff", Title = "mr", PersonalCode = "123", PostNominals = "postnom1"},
                new JudiciaryPersonRequest {Id = Guid.NewGuid(), Email = "test.email.com", Fullname = "ggg hhh", KnownAs = "ggg", Surname = "hhh", Title = "mr", PersonalCode = "123", PostNominals = "postnom1"}
            };
        }
    }
}