using BookingsApi.Contract.V1.Responses;

namespace BookingsApi.IntegrationTests.Api.V1.CaseTypes;

public class GetCaseTypeTests : ApiTest
{
    [Test]
    public async Task should_get_all_case_types()
    {
        // arrange
        using var client = Application.CreateClient();

        // act
        var result = await client.GetAsync(ApiUriFactory.CaseTypesEndpoints.GetCaseTypes);

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var caseTypeResponses = await ApiClientResponse.GetResponses<List<CaseTypeResponse>>(result.Content);
        caseTypeResponses.Should().NotBeEmpty();
        caseTypeResponses.Exists(x=> x.Name == "Generic").Should().BeTrue();
    }
}