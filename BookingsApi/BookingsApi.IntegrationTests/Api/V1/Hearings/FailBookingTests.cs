using BookingsApi.Client;
using BookingsApi.Contract.V2.Enums;

namespace BookingsApi.IntegrationTests.Api.V1.Hearings
{
    public class FailBookingTests : ApiTest
    {
        [Test]
        public async Task should_fail_booking()
        {
            // Arrange
            var seededHearing = await Hooks.SeedVideoHearingV2(status: Domain.Enumerations.BookingStatus.Booked, configureOptions: options =>
            {
                options.ScheduledDate = DateTime.UtcNow;
                options.AddJudge = false;
            });

            var hearingId = seededHearing.Id;
            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpoints.FailBookingUri(hearingId), RequestBody.Set(string.Empty));

            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
            
            
            var bookingsApiClient = BookingsApiClient.GetClient(client);
            var getHearing = await bookingsApiClient.GetHearingDetailsByIdV2Async(hearingId);
            getHearing.Status.Should().Be(BookingStatusV2.Failed);
            getHearing.UpdatedBy.Should().Be("System");
            getHearing.ConfirmedBy.Should().BeNull();
        }
    }
}
