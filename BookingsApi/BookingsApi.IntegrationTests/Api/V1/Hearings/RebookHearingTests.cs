using BookingsApi.Client;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;

namespace BookingsApi.IntegrationTests.Api.V1.Hearings
{
    public class RebookHearingTests: ApiTest
    {
        [Test]
        public async Task should_return_not_found_when_hearing_does_not_exist()
        {
            // arrange
            var hearingId = Guid.NewGuid();
            
            // act
            var client = GetBookingsApiClient();
            var act = async () => await client.RebookHearingAsync(hearingId);
            
            // assert
            var exception = await act.Should().ThrowAsync<BookingsApiException>();
            exception.And.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
        
        [Test]
        public async Task should_return_bad_request_when_hearing_status_is_not_failed()
        {
            // arrange
            var hearing = await Hooks.SeedVideoHearingV2(status: BookingStatus.Created);
            
            // act
            using var client = Application.CreateClient();
            var result = await client
                .PostAsync(ApiUriFactory.HearingsEndpoints.RebookHearing(hearing.Id), null);
            
            // assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors.SelectMany(x => x.Value).Should()
                .Contain($"Hearing must have a status of {nameof(BookingStatus.Failed)}");
        }

        [Test]
        public async Task should_rebook_failed_single_day_hearing()
        {
            // arrange
            var hearing = await Hooks.SeedVideoHearingV2(status: BookingStatus.Failed);
            
            // act
            var client = GetBookingsApiClient();
            await client.RebookHearingAsync(hearing.Id);
            
            // assert
            var serviceBus = Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
            var messages = serviceBus!.ReadAllMessagesFromQueue(hearing.Id);
            Array.Exists(messages, x => x.IntegrationEvent is HearingIsReadyForVideoIntegrationEvent).Should().BeTrue();
        }
    }
}
