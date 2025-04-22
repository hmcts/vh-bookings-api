using BookingsApi.Domain.Enumerations;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;

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

        [Test]
        public async Task should_return_not_found_when_hearing_does_not_exist()
        {
            // arrange
            var hearingId = Guid.NewGuid();

            // act
            using var client = Application.CreateClient();
            var result = await client
                .DeleteAsync(ApiUriFactory.HearingsEndpoints.RemoveHearing(hearingId));
            
            // assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Test]
        public async Task should_return_No_Content_when_hearing_is_removed_and_publish_cancelled_if_created()
        {
            // arrange
            var hearing = await Hooks.SeedVideoHearingV2(status:BookingStatus.Created);
            var hearingId = hearing.Id;

            // act
            using var client = Application.CreateClient();
            var result = await client
                .DeleteAsync(ApiUriFactory.HearingsEndpoints.RemoveHearing(hearingId));
            
            // assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
            
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var removedHearing = await db.VideoHearings
                .Include(x => x.Participants)
                .Include(x => x.JudiciaryParticipants)
                .FirstOrDefaultAsync(x => x.Id == hearingId);
            
            removedHearing.Should().BeNull();
            
            // check of the HearingCancelledIntegrationEvent has been published
            var serviceBusStub =
                Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
            var message = serviceBusStub!.ReadMessageFromQueue();
            message.Should().NotBeNull();
            message.IntegrationEvent.Should()
                .BeEquivalentTo(new HearingCancelledIntegrationEvent(hearing.Id));
        }
    }
}
