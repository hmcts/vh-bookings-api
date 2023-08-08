using BookingsApi.Contract.V1.Responses;

namespace BookingsApi.IntegrationTests.Api.V1.CaseTypes;

public class GetCaseRolesForCaseTypeTests : ApiTest
{
    [Test]
    public async Task should_get_all_case_roles_for_case_type()
    {
        // arrange
        const string caseType = "Generic";
        using var client = Application.CreateClient();

        // act
        var result = await client.GetAsync(ApiUriFactory.CaseTypesEndpoints.GetCaseRolesForCaseType(caseType));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var caseRoleResponses = await ApiClientResponse.GetResponses<List<CaseRoleResponse>>(result.Content);
        caseRoleResponses.Should().NotBeEmpty();
        caseRoleResponses.Exists(x=> x.Name == "Applicant").Should().BeTrue();
    }

    [Test]
    public async Task should_return_not_found_when_invalid_case_type_is_provided()
    {
        // arrange
        const string caseType = "Made Up Case Type";
        using var client = Application.CreateClient();

        // act
        var result = await client.GetAsync(ApiUriFactory.CaseTypesEndpoints.GetCaseRolesForCaseType(caseType));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}