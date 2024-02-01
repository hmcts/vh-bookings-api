using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;

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

        [Test]
        public async Task should_be_Created_when_requested_to_Created_with_judge_in_hearing()
        {
            // arrange
            var seededHearing = await Hooks.SeedVideoHearing(status: Domain.Enumerations.BookingStatus.Booked, configureOptions: options =>
            {
                options.ScheduledDate = DateTime.UtcNow;
            });

            var hearingId = seededHearing.Id;
            var request = new UpdateBookingStatusRequest { Status = Contract.V1.Requests.Enums.UpdateBookingStatus.Created, UpdatedBy = "test" };

            // act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpoints.UpdateBookingStatus(hearingId), RequestBody.Set(request));

            var getHearing = await client.GetAsync(ApiUriFactory.HearingsEndpoints.GetHearingDetailsById(hearingId.ToString()));

            // assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponse>(getHearing.Content);

            hearingResponse.Status.Should().Be(Contract.V1.Enums.BookingStatus.Created);
        }

        [Test]
        public async Task should_be_ConfirmedWithoutJudge_when_requested_to_Created_with_no_judge_in_hearing()
        {
            // arrange
            var seededHearing = await Hooks.SeedVideoHearing(status: Domain.Enumerations.BookingStatus.Booked, configureOptions: options =>
            {
                options.ScheduledDate = DateTime.UtcNow;
                options.AddJudge = false;
            });

            var hearingId = seededHearing.Id;
            var request = new UpdateBookingStatusRequest { Status = Contract.V1.Requests.Enums.UpdateBookingStatus.Created, UpdatedBy = "test" };

            // act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpoints.UpdateBookingStatus(hearingId), RequestBody.Set(request));

            var getHearing = await client.GetAsync(ApiUriFactory.HearingsEndpoints.GetHearingDetailsById(hearingId.ToString()));

            // assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponse>(getHearing.Content);

            hearingResponse.Status.Should().Be(Contract.V1.Enums.BookingStatus.ConfirmedWithoutJudge);
        }
    }
}
