using BookingsApi.Client;

namespace BookingsApi.IntegrationTests.Api.V2.CaseTypes;

public class GetCaseTypesV2Tests : ApiTest
{
    [Test]
    public async Task should_get_all_case_types_not_deleted()
    {
        // arrange
        const string caseType = "ZZY1";
        using var client = Application.CreateClient();
        var bookingsApiClient = BookingsApiClient.GetClient(client);

        // act
        var caseRoleResponses = await bookingsApiClient.GetCaseTypesV2Async(includeDeleted: false);

        // assert
        caseRoleResponses.Should().NotBeEmpty();

        caseRoleResponses.Should().Contain(x => x.ServiceId == caseType);
    }
}