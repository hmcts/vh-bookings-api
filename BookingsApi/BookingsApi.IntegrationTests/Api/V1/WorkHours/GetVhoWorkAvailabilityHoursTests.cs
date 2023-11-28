namespace BookingsApi.IntegrationTests.Api.V1.WorkHours
{
    public class GetVhoWorkAvailabilityHoursTests : ApiTest
    {
        [Test]
        public async Task should_return_bad_request_when_username_is_invalid()
        {
            // arrange
            var username = "invalid-username";
            
            // act
            using var client = Application.CreateClient();
            var result = await client
                .GetAsync(ApiUriFactory.WorkHoursEndpoints.GetVhoWorkAvailabilityHours(username));

            // assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors.SelectMany(x => x.Value).Should()
                .Contain($"Please provide a valid {nameof(username)}");
        }
    }
}
