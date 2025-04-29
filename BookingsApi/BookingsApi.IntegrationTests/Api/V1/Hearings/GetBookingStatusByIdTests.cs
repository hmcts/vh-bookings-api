using BookingsApi.Contract.V1.Enums;

namespace BookingsApi.IntegrationTests.Api.V1.Hearings
{
    public class GetBookingStatusByIdTests : ApiTest
    {
        [Test]
        public async Task should_return_bad_request_when_hearing_id_is_invalid()
        {
            // arrange
            var hearingId = Guid.Empty;
            
            // act
            using var client = Application.CreateClient();
            var result = await client
                .GetAsync(ApiUriFactory.HearingsEndpoints.GetBookingStatusById(hearingId));


            // assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors.SelectMany(x => x.Value).Should()
                .Contain($"Please provide a valid {nameof(hearingId)}");
        }

        [Test]
        public async Task should_return_not_found_when_hearing_does_not_exist()
        {
            // arrange
            var hearingId = Guid.NewGuid();
            
            // act
            using var client = Application.CreateClient();
            var result = await client
                .GetAsync(ApiUriFactory.HearingsEndpoints.GetBookingStatusById(hearingId));


            // assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task should_return_okay_and_hearing_status()
        {
            // arrange
            var hearing = await Hooks.SeedVideoHearingV2();
            
            // act
            using var client = Application.CreateClient();
            var result = await client
                .GetAsync(ApiUriFactory.HearingsEndpoints.GetBookingStatusById(hearing.Id));
            
            result.IsSuccessStatusCode.Should().BeTrue();
            var bookingStatus = await ApiClientResponse.GetResponses<Contract.V1.Enums.BookingStatus>(result.Content);
            bookingStatus.Should().Be((BookingStatus)hearing.Status);
        }
    }
}
