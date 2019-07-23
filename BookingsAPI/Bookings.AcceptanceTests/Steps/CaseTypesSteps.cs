using System.Collections.Generic;
using Bookings.AcceptanceTests.Contexts;
using Bookings.Api.Contract.Responses;
using FluentAssertions;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api;

namespace Bookings.AcceptanceTests.Steps
{
    [Binding]
    public sealed class CaseTypesSteps
    {
        private readonly TestContext _context;
        private readonly CaseTypesEndpoints _endpoints = new ApiUriFactory().CaseTypesEndpoints;

        public CaseTypesSteps(TestContext context)
        {
            _context = context;
        }

        [Given(@"I have a get available case types request")]
        public void GivenIHaveAGetAvailableCaseTypesRequest()
        {
            _context.Request = _context.Get(_endpoints.GetCaseTypes());
        }

        [Given(@"I have a get case roles for a case type of '(.*)' request")]
        public void GivenIHaveAGetAllHearingVenuesAvailableForBookingRequest(string caseType)
        {
            _context.Request = _context.Get(_endpoints.GetCaseRolesForCaseType(caseType));
        }

        [Given(@"I have a get hearing roles for a case role of '(.*)' and case type of '(.*)' request")]
        public void GivenIHaveAGetHearingRolesForCaseRoleOfCaseTypeRequest(string caseType, string caseRoleName)
        {
            _context.Request = _context.Get(_endpoints.GetHearingRolesForCaseRole(caseType, caseRoleName));
        }

        [Then(@"a list of case types should be retrieved")]
        public void ThenAListOfCaseTypesShouldBeRetrieved()
        {
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<CaseTypeResponse>>(_context.Json);
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
        [Then(@"a list of hearing roles should be retrieved")]
        public void ThenAListOfRolesShouldBeRetrieved()
        {
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<CaseRoleResponse>>(_context.Json);
            model.Should().NotBeEmpty();
            model[0].Name.IsNotNullOrEmpty();
        }
    }
}
