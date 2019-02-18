using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Bookings.Api.Contract.Responses;
using Bookings.IntegrationTests.Api;
using Bookings.IntegrationTests.Contexts;
using FluentAssertions;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api;

namespace Bookings.IntegrationTests.Steps
{
    [Binding]
    public sealed class CaseTypesSteps : ControllerTestsBase
    {
        private readonly ApiTestContext _apiTestContext;
        private readonly CaseTypesEndpoints _endpoints = new ApiUriFactory().CaseTypesEndpoints;

        public CaseTypesSteps(ApiTestContext apiTestContext)
        {
            _apiTestContext = apiTestContext;
        }

        [Given(@"I have a get case roles for a case type of '(.*)' request")]
        public void GivenIHaveAGetCaseRolesForACaseTypeRequest(string caseType)
        {
            _apiTestContext.Uri = _endpoints.GetCaseRolesForCaseType(caseType);
            _apiTestContext.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a get hearing roles for a case role of '(.*)' and case type of '(.*)' request")]
        public void GivenIHaveAGetHearingRolesForCaseRoleOfCaseTypeRequest(string caseType, string caseRoleName)
        {          
            _apiTestContext.Uri = _endpoints.GetHearingRolesForCaseRole(caseType, caseRoleName);
            _apiTestContext.HttpMethod = HttpMethod.Get;
        }

        [Then(@"a list of case roles should be retrieved")]
        [Then(@"a list of hearing roles should be retrieved")]
        public async Task ThenAListOfCaseRolesShouldBeRetrieved()
        {
            var json = await _apiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<CaseRoleResponse>>(json);
            model.Should().NotBeEmpty();
            model[0].Name.IsNotNullOrEmpty();
        }
    }
}
