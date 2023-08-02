using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.IntegrationTests.Contexts;
using FluentAssertions;
using TechTalk.SpecFlow;
using static Testing.Common.Builders.Api.ApiUriFactory.CaseTypesEndpoints;

namespace BookingsApi.IntegrationTests.Steps
{
    [Binding]
    public sealed class CaseTypesBaseSteps : BaseSteps
    {
        private readonly ScenarioContext _scenarioContext;

        public CaseTypesBaseSteps(TestContext apiTestContext, ScenarioContext scenarioContext) : base(apiTestContext)
        {
            _scenarioContext = scenarioContext;
        }   

        [Given(@"I have a get available case types request")]
        public void GivenIHaveAGetAvailableCaseTypesRequest()
        {
            Context.Uri = GetCaseTypes;
            Context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a get case roles for a case type of (.*) request")]
        public void GivenIHaveAGetCaseRolesForACaseTypeRequest(string caseType)
        {
            Context.Uri = GetCaseRolesForCaseType(caseType);
            Context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a get hearing roles for a case type of '(.*)' and case role of '(.*)' request")]
        public void GivenIHaveAGetHearingRolesForCaseTypeOfCaseRoleRequest(string caseType, string caseRoleName)
        {          
            Context.Uri = GetHearingRolesForCaseRole(caseType, caseRoleName);
            Context.HttpMethod = HttpMethod.Get;
            _scenarioContext.Add("CaseTypeKey", caseType);
        }

        [Then(@"a list of case types should be retrieved")]
        public async Task ThenAListOfCaseTypesShouldBeRetrieved()
        {
            var json = await Context.Response.Content.ReadAsStringAsync();
            var model = RequestHelper.Deserialise<List<CaseTypeResponse>>(json);
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

        [Then(@"a list of case types should be contain")]
        public async Task ThenAListOfCaseTypesShouldContain(IDictionary<string, IEnumerable<string>> caseHearingTypes)
        {
            var json = await Context.Response.Content.ReadAsStringAsync();
            var models = RequestHelper.Deserialise<List<CaseTypeResponse>>(json);
            foreach (var (key, value) in caseHearingTypes)
            {
                var caseType = models.FirstOrDefault(x => x.Name.Equals(key, StringComparison.CurrentCultureIgnoreCase));
                caseType.Should().NotBeNull($"{key} should be in the list of case types");
                caseType.HearingTypes.Select(x => x.Name).Should().Contain(value);
            }
        }

        [Then(@"a list of case roles should be retrieved")]
        public async Task ThenAListOfCaseRolesShouldBeRetrieved()
        {
            var json = await Context.Response.Content.ReadAsStringAsync();
            var model = RequestHelper.Deserialise<List<CaseRoleResponse>>(json);
            model.Should().NotBeEmpty();
            model[0].Name.Should().NotBeNullOrEmpty();
        }
        
        [Then(@"a list of hearing roles should be retrieved")]
        public async Task ThenAListOfHearingRolesShouldBeRetrieved()
        {
            var json = await Context.Response.Content.ReadAsStringAsync();
            var model = RequestHelper.Deserialise<List<CaseRoleResponse>>(json);
            model.Should().NotBeEmpty();
            model[0].Name.Should().NotBeNullOrEmpty();
            model[0].UserRole.Should().NotBeNullOrEmpty();
        }
    }
}
