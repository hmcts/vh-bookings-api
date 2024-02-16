using BookingsApi.Contract.V1.Responses;

namespace BookingsApi.IntegrationTests.Api.V1.Hearings
{
    public class FailBookingTests : ApiTest
    {
        [Test]
        public async Task should_fail_booking()
        {
            // Arrange
            var seededHearing = await Hooks.SeedVideoHearing(status: Domain.Enumerations.BookingStatus.Booked, configureOptions: options =>
            {
                options.ScheduledDate = DateTime.UtcNow;
                options.AddJudge = false;
            });

            var hearingId = seededHearing.Id;
            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpoints.FailBookingUri(hearingId), RequestBody.Set(string.Empty));

            var getHearing = await client.GetAsync(ApiUriFactory.HearingsEndpoints.GetHearingDetailsById(hearingId.ToString()));

            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponse>(getHearing.Content);

            hearingResponse.Status.Should().Be(Contract.V1.Enums.BookingStatus.Failed);
            AssertCommon(hearingResponse);
        }
        
        private static void AssertCommon(HearingDetailsResponse hearingResponse)
        {
            hearingResponse.UpdatedBy.Should().Be("System");
            hearingResponse.ConfirmedBy.Should().BeNull();
        }
    }
}
