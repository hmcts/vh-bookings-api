using BookingsApi.Contract.V2.Responses;

namespace BookingsApi.IntegrationTests.Api.V2.CaseTypes;

public class GetCaseRolesForCaseTypeTestsV2 : ApiTest
{
    [Test]
    public async Task should_get_all_case_roles_for_case_type()
    {
        // arrange
        const string caseType = "ZZY1";
        using var client = Application.CreateClient();

        // act
        var result = await client.GetAsync(ApiUriFactory.CaseTypesEndpointsV2.GetCaseRolesForCaseType(caseType));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var caseRoleResponses = await ApiClientResponse.GetResponses<List<CaseRoleResponseV2>>(result.Content);
        caseRoleResponses.Should().NotBeEmpty();
        caseRoleResponses.Exists(x=> x.Name == "Applicant").Should().BeTrue();
    }

    [Test]
    public async Task should_return_not_found_when_invalid_case_type_is_provided()
    {
        // arrange
        const string caseType = "Made Up Case Service";
        using var client = Application.CreateClient();

        // act
        var result = await client.GetAsync(ApiUriFactory.CaseTypesEndpointsV2.GetCaseRolesForCaseType(caseType));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}