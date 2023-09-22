using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using Testing.Common.Builders.Api.V1.Request;

namespace BookingsApi.IntegrationTests.Api.V1.Hearings;

public class BookMultiDayHearingTests : ApiTest
{
    [Test]
    public async Task should_book_a_multi_day_with_consecutive_days()
    {
        // arrange
        var hearing = await BookHearingViaApi();
        var request = new CloneHearingRequest()
        {
            Dates = new List<DateTime>()
            {
                hearing.ScheduledDateTime.AddDays(1),
                hearing.ScheduledDateTime.AddDays(2),
                hearing.ScheduledDateTime.AddDays(3)
            }
        };

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.CloneHearing(hearing.Id), RequestBody.Set(request));


        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        var serviceBusStub = Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
        var allMessages = serviceBusStub!.ReadAllMessagesFromQueue();
        allMessages.Count(x=> x.IntegrationEvent is HearingIsReadyForVideoIntegrationEvent).Should().Be(4);
        allMessages.Count(x=> x.IntegrationEvent is MultiDayHearingIntegrationEvent).Should().Be(1);
        allMessages.Should().NotBeEmpty();
    }
    
    [Test]
    public async Task should_book_a_multi_day_with_consecutive_days_without_a_judge()
    {
        // arrange
        var hearing = await BookHearingViaApi(skipJudge: true);
        var request = new CloneHearingRequest()
        {
            Dates = new List<DateTime>()
            {
                hearing.ScheduledDateTime.AddDays(1),
                hearing.ScheduledDateTime.AddDays(2),
                hearing.ScheduledDateTime.AddDays(3)
            }
        };

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.CloneHearing(hearing.Id), RequestBody.Set(request));


        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        var serviceBusStub = Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
        var allMessages = serviceBusStub!.ReadAllMessagesFromQueue();
        allMessages.Count(x=> x.IntegrationEvent is HearingNotificationIntegrationEvent).Should().Be(1);
        allMessages.Count(x=> x.IntegrationEvent is CreateAndNotifyUserIntegrationEvent).Should().Be(4);
        allMessages.Count(x=> x.IntegrationEvent is MultiDayHearingIntegrationEvent).Should().Be(1);
        allMessages.Should().NotBeEmpty();
    }

    private async Task<HearingDetailsResponse> BookHearingViaApi(bool skipJudge = false)
    {
        var hearingSchedule = DateTime.UtcNow.AddMinutes(5);
        var caseName = "Bookings Api Integration Automated";
        var request = new SimpleBookNewHearingRequest(caseName, hearingSchedule).Build();
        if (skipJudge)
        {
            request.Participants = request.Participants.Where(x => x.HearingRoleName != "Judge").ToList();
        }

        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.BookNewHearing, RequestBody.Set(request));
        var hearing =  await ApiClientResponse.GetResponses<HearingDetailsResponse>(result.Content);
        Hooks.AddHearingForCleanup(hearing.Id);
        return hearing;
    }
}