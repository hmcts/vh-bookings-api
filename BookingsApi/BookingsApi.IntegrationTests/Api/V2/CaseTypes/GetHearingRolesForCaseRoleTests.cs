using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.V2.Responses;
using BookingsApi.IntegrationTests.Helper;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Api;

namespace BookingsApi.IntegrationTests.Api.V2.CaseTypes;

public class GetHearingRolesForCaseRoleTests : ApiTest
{
    [Test]
    public async Task should_return_hearing_roles_for_case_role()
    {
        // arrange
        const string serviceId = "vhG1";
        const string caseRole = "Applicant";
        using var client = Application.CreateClient();

        // act
        var result = await client.GetAsync(ApiUriFactory.CaseTypesEndpointsV2.GetHearingRolesForCaseRole(serviceId, caseRole));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var caseRoleResponses = await ApiClientResponse.GetResponses<List<HearingRoleResponseV2>>(result.Content);
        caseRoleResponses.Should().NotBeEmpty();
        caseRoleResponses.Exists(x=> x.Name == "Litigant in person").Should().BeTrue();
    }

    [Test]
    public async Task should_return_not_found_when_case_role_does_not_exist()
    {
        // arrange
        const string serviceId = "Made up for test";
        const string caseRole = "Applicant";
        using var client = Application.CreateClient();

        // act
        var result = await client.GetAsync(ApiUriFactory.CaseTypesEndpointsV2.GetHearingRolesForCaseRole(serviceId, caseRole));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }
    
    [Test]
    public async Task should_return_not_found_when_hearing_role_does_not_exist()
    {
        // arrange
        const string serviceId = "vhG1";
        const string caseRole = "Made up case role";
        using var client = Application.CreateClient();

        // act
        var result = await client.GetAsync(ApiUriFactory.CaseTypesEndpointsV2.GetHearingRolesForCaseRole(serviceId, caseRole));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }
}