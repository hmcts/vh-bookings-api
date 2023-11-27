namespace BookingsApi.IntegrationTests.Api.V1.Hearings
{
    public class RemoveHearingTests : ApiTest
    {
        [Test]
        public async Task should_return_bad_request_when_hearing_id_is_invalid()
        {
            // arrange
            var hearingId = Guid.Empty;
            
            // act
            using var client = Application.CreateClient();
            var result = await client
                .DeleteAsync(ApiUriFactory.HearingsEndpoints.RemoveHearing(hearingId));


            // assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors.SelectMany(x => x.Value).Should()
                .Contain($"Please provide a valid {nameof(hearingId)}");
        }
    }
}
