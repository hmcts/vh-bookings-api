namespace BookingsApi.IntegrationTests.Api.V1.StaffMembers
{
    public class GetStaffMemberBySearchTermTests : ApiTest
    {
        [Test]
        public async Task should_return_bad_request_when_search_term_is_invalid()
        {
            // arrange
            var searchTerm = "a";
            
            // act
            using var client = Application.CreateClient();
            var result = await client
                .GetAsync(ApiUriFactory.StaffMemberEndpoints.GetStaffMemberBySearchTerm(searchTerm));

            // assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors.SelectMany(x => x.Value).Should()
                .Contain($"Search term must be at least 3 characters.");
        }
    }
}
