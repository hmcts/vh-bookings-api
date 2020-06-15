﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using Bookings.Api.Contract.Responses;
using Bookings.IntegrationTests.Contexts;
using Bookings.IntegrationTests.Models;
using FluentAssertions;
using TechTalk.SpecFlow;
using static Testing.Common.Builders.Api.ApiUriFactory.CaseTypesEndpoints;

namespace Bookings.IntegrationTests.Steps
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
            var model = RequestHelper.DeserialiseSnakeCaseJsonToResponse<List<CaseTypeResponse>>(json);
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
            var models = RequestHelper.DeserialiseSnakeCaseJsonToResponse<List<CaseTypeResponse>>(json);

            models.Select(x => x.Name).Should().Contain(caseHearingTypes.Keys);
            foreach (var model in models)
            {
                caseHearingTypes[model.Name].Should().Contain(model.HearingTypes.Select(x => x.Name));
                model.HearingTypes.Select(x => x.Name).Should().Contain(caseHearingTypes[model.Name]);
            }
        }

        [Then(@"a list of hearing roles should be retrieved")]
        [Then(@"a list of case roles should be retrieved")]
        public async Task ThenAListOfCaseRolesShouldBeRetrieved()
        {
            var json = await Context.Response.Content.ReadAsStringAsync();
            var model = RequestHelper.DeserialiseSnakeCaseJsonToResponse<List<CaseRoleResponse>>(json);
            model.Should().NotBeEmpty();
            model[0].Name.Should().NotBeNullOrEmpty();
        }
    }
}
