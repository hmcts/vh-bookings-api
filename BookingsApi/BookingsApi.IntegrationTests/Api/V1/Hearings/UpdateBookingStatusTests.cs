using BookingsApi.Contract.V1.Requests;

namespace BookingsApi.IntegrationTests.Api.V1.Hearings
{
    public class UpdateBookingStatusTests : ApiTest
    {
        [Test]
        public async Task should_return_bad_request_when_hearing_id_is_invalid()
        {
            // arrange
            var hearingId = Guid.Empty;
            var request = new UpdateBookingStatusRequest();
            
            // act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpoints.UpdateBookingStatus(hearingId), RequestBody.Set(request));

            // assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors.SelectMany(x => x.Value).Should()
                .Contain($"Please provide a valid {nameof(hearingId)}");
        }
        
        [Test]
        public async Task should_return_bad_request_when_request_is_invalid()
        {
            // arrange
            var hearingId = Guid.NewGuid();
            var request = new UpdateBookingStatusRequest();
            
            // act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpoints.UpdateBookingStatus(hearingId), RequestBody.Set(request));

            // assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors.SelectMany(x => x.Value).Should()
                .Contain("The booking status is not recognised");
        }
    }
}
