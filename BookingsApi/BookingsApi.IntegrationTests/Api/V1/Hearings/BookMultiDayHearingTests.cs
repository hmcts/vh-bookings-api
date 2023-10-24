using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using BookingsApi.Validations.V1;
using Testing.Common.Builders.Api.V1.Request;

namespace BookingsApi.IntegrationTests.Api.V1.Hearings;

public class BookMultiDayHearingTests : ApiTest
{
    [Test]
    public async Task should_return_not_found_when_hearing_does_not_exist()
    {
        // arrange
        var hearingId = Guid.NewGuid();
        var date = DateTime.UtcNow.AddDays(1);
        var request = new CloneHearingRequest()
        {
            Dates = new List<DateTime>()
            {
                date.AddDays(1),
                date.AddDays(2),
                date.AddDays(3)
            }
        };
        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.CloneHearing(hearingId), RequestBody.Set(request));


        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Test]
    public async Task should_return_bad_request_when_validation_fails()
    {
        // arrange
        var hearing = await BookHearingViaApi();
        var request = new CloneHearingRequest()
        {
            Dates = new List<DateTime>()
            {
                hearing.ScheduledDateTime.AddDays(-1),
                hearing.ScheduledDateTime.AddDays(-1)
            }
        };
        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.CloneHearing(hearing.Id), RequestBody.Set(request));


        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[nameof(request.Dates)].Should().Contain(CloneHearingRequestValidation.DuplicateDateErrorMessage);
        validationProblemDetails.Errors[nameof(request.Dates)].Should().Contain(CloneHearingRequestValidation.InvalidDateRangeErrorMessage);
    }
    
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
        
        await AssertClonedHearingsAreLinked(hearing, request);
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
        allMessages.Count(x=> x.IntegrationEvent is CreateAndNotifyUserIntegrationEvent).Should().Be(4);
        allMessages.Count(x=> x.IntegrationEvent is MultiDayHearingIntegrationEvent).Should().Be(1);
        allMessages.Should().NotBeEmpty();
        
        await AssertClonedHearingsAreLinked(hearing, request);
    }

    private async Task<HearingDetailsResponse> BookHearingViaApi(bool skipJudge = false)
    {
        var hearingSchedule = DateTime.UtcNow.AddMinutes(5);
        var caseName = "Bookings Api Integration Automated";
        var request = new SimpleBookNewHearingRequest(caseName, hearingSchedule).Build();
        request.IsMultiDayHearing = true;
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
    
    private async Task AssertClonedHearingsAreLinked(HearingDetailsResponse hearing, CloneHearingRequest request)
    {
        await using var db = new BookingsDbContext(BookingsDbContextOptions);
        var hearingsFromDb =
            await db.VideoHearings.Include(x => x.HearingCases).ThenInclude(h => h.Case).AsNoTracking()
                .Where(x => x.SourceId == hearing.Id)
                .OrderBy(x => x.ScheduledDateTime).ToListAsync();
        hearingsFromDb.Count.Should().Be(request.Dates.Count + 1); // +1 to include the original hearing

        var totalDays = hearingsFromDb.Count;
        for (var i = 0; i < hearingsFromDb.Count - 1; i++)
        {
            var hearingDay = i + 1;
            hearingsFromDb[i].GetCases()[0].Name.Should().EndWith($"Day {hearingDay} of {totalDays}");
        }
    }
}