using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bookings.Api.Contract.Responses;
using Bookings.IntegrationTests.Api;
using Bookings.IntegrationTests.Contexts;
using Bookings.IntegrationTests.Helper;
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

        [Given(@"I have a (.*) get case roles for a case type request")]
        [Given(@"I have a (.*) case type in a get case roles for a case type request")]
        public void GivenIHaveAGetCaseRolesForACaseTypeRequest(Scenario scenario)
        {
            string caseType;
            switch (scenario)
            {
                case Scenario.Valid: caseType = "Civil Money Claims"; break;
                case Scenario.Nonexistent: caseType = "Does not exist"; break;               
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }
            _apiTestContext.Uri = _endpoints.GetCaseRolesForCaseType(caseType);
            _apiTestContext.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a (.*) get hearing roles for a case role of a case type request")]
        [Given(@"I have a (.*) in a get hearing roles for a case role of a case type request")]
        public void GivenIHaveAGetHearingRolesForCaseRoleOfCaseTypeRequest(Scenario scenario)
        {
            var caseType = "Civil Money Claims";
            var roleName = "Claimant";
            switch (scenario)
            {
                case Scenario.Valid: caseType = "Civil Money Claims"; break;
                case Scenario.NonexistentCaseType: caseType = "Does not exist"; break;
                case Scenario.NonexistentRoleName: roleName = "Does not exist"; break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }
            _apiTestContext.Uri = _endpoints.GetHearingRolesForCaseRole(caseType, roleName);
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
