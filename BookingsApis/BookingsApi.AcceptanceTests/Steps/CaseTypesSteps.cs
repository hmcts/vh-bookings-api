using System.Collections.Generic;
using AcceptanceTests.Common.Api.Helpers;
using BookingsApi.AcceptanceTests.Contexts;
using BookingsApi.Contract.Responses;
using FluentAssertions;
using TechTalk.SpecFlow;
using static Testing.Common.Builders.Api.ApiUriFactory.CaseTypesEndpoints;

namespace BookingsApi.AcceptanceTests.Steps
{
    [Binding]
    public sealed class CaseTypesSteps
    {
        private readonly TestContext _context;

        public CaseTypesSteps(TestContext context)
        {
            _context = context;
        }

        [Given(@"I have a get available case types request")]
        public void GivenIHaveAGetAvailableCaseTypesRequest()
        {
            _context.Request = _context.Get(GetCaseTypes);
        }

        [Given(@"I have a get case roles for a case type of '(.*)' request")]
        public void GivenIHaveAGetAllHearingVenuesAvailableForBookingRequest(string caseType)
        {
            _context.Request = _context.Get(GetCaseRolesForCaseType(caseType));
        }

        [Given(@"I have a get hearing roles for a case type of '(.*)' and case role of '(.*)' request")]
        public void GivenIHaveAGetHearingRolesForCaseTypeOfCaseRoleRequest(string caseType, string caseRoleName)
        {
            _context.Request = _context.Get(GetHearingRolesForCaseRole(caseType, caseRoleName));
        }

        [Then(@"a list of case types should be retrieved")]
        public void ThenAListOfCaseTypesShouldBeRetrieved()
        {
            var model = RequestHelper.Deserialise<List<CaseTypeResponse>>(_context.Response.Content);
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

        [Then(@"a list of case roles should be retrieved")]
        public void ThenAListOfCaseRolesShouldBeRetrieved()
        {
            var model = RequestHelper.Deserialise<List<CaseRoleResponse>>(_context.Response.Content);
            model.Should().NotBeEmpty();
            model[0].Name.Should().NotBeNullOrEmpty();
        }
        
        [Then(@"a list of hearing roles should be retrieved")]
        public void ThenAListOfHearingRolesShouldBeRetrieved()
        {
            var model = RequestHelper.Deserialise<List<CaseRoleResponse>>(_context.Response.Content);
            model.Should().NotBeEmpty();
            model[0].Name.Should().NotBeNullOrEmpty();
            model[0].UserRole.Should().NotBeNullOrEmpty();
        }
    }
}
