using BookingsApi.Contract.V1.Responses;

namespace BookingsApi.IntegrationTests.Api.V1.CaseTypes;

public class GetHearingRolesForCaseRoleTests : ApiTest
{
    [Test]
    public async Task should_return_hearing_roles_for_case_role()
    {
        // arrange
        const string caseType = "Generic";
        const string caseRole = "Applicant";
        using var client = Application.CreateClient();

        // act
        var result = await client.GetAsync(ApiUriFactory.CaseTypesEndpoints.GetHearingRolesForCaseRole(caseType, caseRole));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var caseRoleResponses = await ApiClientResponse.GetResponses<List<HearingRoleResponse>>(result.Content);
        caseRoleResponses.Should().NotBeEmpty();
        caseRoleResponses.Exists(x=> x.Name == "Litigant in person").Should().BeTrue();
    }

    [Test]
    public async Task should_return_not_found_when_case_role_does_not_exist()
    {
        // arrange
        const string caseType = "Made up for test";
        const string caseRole = "Applicant";
        using var client = Application.CreateClient();

        // act
        var result = await client.GetAsync(ApiUriFactory.CaseTypesEndpoints.GetHearingRolesForCaseRole(caseType, caseRole));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }
    
    [Test]
    public async Task should_return_not_found_when_hearing_role_does_not_exist()
    {
        // arrange
        const string caseType = "Generic";
        const string caseRole = "Made up case role";
        using var client = Application.CreateClient();

        // act
        var result = await client.GetAsync(ApiUriFactory.CaseTypesEndpoints.GetHearingRolesForCaseRole(caseType, caseRole));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }
}