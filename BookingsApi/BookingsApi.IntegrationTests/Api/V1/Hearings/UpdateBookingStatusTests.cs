using BookingsApi.Client;
using BookingsApi.Contract.V2.Enums;

namespace BookingsApi.IntegrationTests.Api.V1.Hearings
{
    public class UpdateBookingStatusTests : ApiTest
    {
        [Test]
        public async Task should_return_bad_request_when_hearing_id_is_invalid()
        {
            // arrange
            var hearingId = Guid.Empty;
            
            // act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpoints.UpdateBookingStatus(hearingId), null);

            // assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors.SelectMany(x => x.Value).Should()
                .Contain($"Please provide a valid {nameof(hearingId)}");
        }
        
        [Test]
        public async Task should_be_Created_when_requested_to_Created_with_judge_in_hearing()
        {
            // arrange
            var seededHearing = await Hooks.SeedVideoHearingV2(status: Domain.Enumerations.BookingStatus.Booked, configureOptions: options =>
            {
                options.ScheduledDate = DateTime.UtcNow;
            });

            var hearingId = seededHearing.Id;
            // act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpoints.UpdateBookingStatus(hearingId), null);

            // assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
            await GetHearingAndConfirmStatusIs(client, hearingId, BookingStatusV2.Created);
        }

        [Test]
        public async Task should_be_ConfirmedWithoutJudge_when_requested_to_Created_with_no_judge_in_hearing()
        {
            // arrange
            var seededHearing = await Hooks.SeedVideoHearingV2(status: Domain.Enumerations.BookingStatus.Booked, configureOptions: options =>
            {
                options.ScheduledDate = DateTime.UtcNow;
                options.AddJudge = false;
            });

            var hearingId = seededHearing.Id;
            // act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpoints.UpdateBookingStatus(hearingId), RequestBody.Set(string.Empty));

            // assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
            await GetHearingAndConfirmStatusIs(client, hearingId, BookingStatusV2.ConfirmedWithoutJudge);
        }

        private static async Task GetHearingAndConfirmStatusIs(HttpClient client, Guid hearingId, BookingStatusV2 expectedStatus)
        {
            var bookingsApiClient = BookingsApiClient.GetClient(client);
            var hearingResponse = await bookingsApiClient.GetHearingDetailsByIdV2Async(hearingId);
            hearingResponse.Status.Should().Be(expectedStatus);
            hearingResponse.UpdatedBy.Should().Be("System");
            hearingResponse.ConfirmedBy.Should().Be("System");
        }
    }
}
