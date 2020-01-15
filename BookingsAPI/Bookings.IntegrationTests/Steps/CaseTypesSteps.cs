using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Bookings.Api.Contract.Responses;
using Bookings.IntegrationTests.Contexts;
using FluentAssertions;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api;

namespace Bookings.IntegrationTests.Steps
{
    [Binding]
    public sealed class CaseTypesSteps : StepsBase
    {
        private readonly CaseTypesEndpoints _endpoints = new ApiUriFactory().CaseTypesEndpoints;
        private readonly ScenarioContext _scenarioContext;

        public CaseTypesSteps(TestContext apiTestContext, ScenarioContext scenarioContext) : base(apiTestContext)
        {
            _scenarioContext = scenarioContext;
        }   

        [Given(@"I have a get available case types request")]
        public void GivenIHaveAGetAvailableCaseTypesRequest()
        {
            Context.Uri = _endpoints.GetCaseTypes();
            Context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a get case roles for a case type of (.*) request")]
        public void GivenIHaveAGetCaseRolesForACaseTypeRequest(string caseType)
        {
            Context.Uri = _endpoints.GetCaseRolesForCaseType(caseType);
            Context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a get hearing roles for a case type of '(.*)' and case role of '(.*)' request")]
        public void GivenIHaveAGetHearingRolesForCaseTypeOfCaseRoleRequest(string caseType, string caseRoleName)
        {          
            Context.Uri = _endpoints.GetHearingRolesForCaseRole(caseType, caseRoleName);
            Context.HttpMethod = HttpMethod.Get;
            _scenarioContext.Add("CaseTypeKey", caseType);
        }

        [Then(@"a list of case types should be retrieved")]
        public async Task ThenAListOfCaseTypesShouldBeRetrieved()
        {
            var json = await Context.ResponseMessage.Content.ReadAsStringAsync();
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<CaseTypeResponse>>(json);
            model.Should().NotBeEmpty();
            foreach (var caseType in model)
            {
                caseType.Name.Should().NotBeNullOrEmpty();
                caseType.Id.Should().BeGreaterThan(0);
                foreach (var hearingType in caseType.HearingTypes)
                {
                    hearingType.Id.Should().BeGreaterThan(0);
                    hearingType.Name.Should().NotBeNullOrEmpty();
                }
            }
        }
        [Then(@"a list of hearing roles should be retrieved")]
        [Then(@"a list of case roles should be retrieved")]
        public async Task ThenAListOfCaseRolesShouldBeRetrieved()
        {
            var json = await Context.ResponseMessage.Content.ReadAsStringAsync();
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<CaseRoleResponse>>(json);
            model.Should().NotBeEmpty();
            model[0].Name.Should().NotBeNullOrEmpty();
        }
    }
}
