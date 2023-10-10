using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Validations;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using BookingsApi.Validations.V1;
using FizzWare.NBuilder;

namespace BookingsApi.IntegrationTests.Api.V1.Hearings;

public class UpdateHearingTests : ApiTest
{
    [Test]
    public async Task should_return_bad_request_and_validation_errors_when_payload_fails_validation()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearing();
        var hearingId = hearing.Id;
        var request = new UpdateHearingRequest();

        // act
        using var client = Application.CreateClient();
        var result = await client.PutAsync(ApiUriFactory.HearingsEndpoints.UpdateHearingDetails(hearingId), RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[nameof(request.HearingVenueName)][0].Should()
            .Be(UpdateHearingRequestValidation.NoHearingVenueNameErrorMessage);
        
        validationProblemDetails.Errors[nameof(request.ScheduledDuration)][0].Should()
            .Be(UpdateHearingRequestValidation.NoScheduleDurationErrorMessage);
        
        validationProblemDetails.Errors[nameof(request.ScheduledDateTime)][0].Should()
            .Be(UpdateHearingRequestValidation.ScheduleDateTimeInPastErrorMessage);
        
        validationProblemDetails.Errors[nameof(request.UpdatedBy)][0].Should()
            .Be(UpdateHearingRequestValidation.NoUpdatedByErrorMessage);
    }
    
    [Test]
    public async Task should_return_not_found_when_attempting_update_a_hearing_that_does_not_exist()
    {
        // arrange
        var hearingId = Guid.NewGuid();
        var request = BuildRequest();

        // act
        using var client = Application.CreateClient();
        var result = await client.PutAsync(ApiUriFactory.HearingsEndpoints.UpdateHearingDetails(hearingId), RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Test]
    public async Task should_return_bad_request_and_validation_errors_when_venue_does_not_exist()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearing();
        var hearingId = hearing.Id;
        var request = BuildRequest();
        request.HearingVenueName = "shouldnotexist";

        // act
        using var client = Application.CreateClient();
        var result = await client.PutAsync(ApiUriFactory.HearingsEndpoints.UpdateHearingDetails(hearingId), RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[nameof(request.HearingVenueName)][0].Should()
            .Be($"Hearing venue does not exist");
    }

    [Test]
    public async Task should_return_bad_request_and_validation_failure_when_editing_a_created_hearing_close_to_start_time()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearing(options => options.ScheduledDate = DateTime.UtcNow.AddMinutes(5), BookingStatus.Created);
        var hearingId = hearing.Id;
        var request = BuildRequest();
        
        // act
        using var client = Application.CreateClient();
        var result = await client.PutAsync(ApiUriFactory.HearingsEndpoints.UpdateHearingDetails(hearingId), RequestBody.Set(request));
        
        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors["Hearing"].Should().Contain(DomainRuleErrorMessages.CannotUpdateHearingDetailsCloseToStartTime);
    }

    [Test]
    public async Task should_update_hearing_and_publish_when_hearing_status_is_created()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearing(options =>
        {
            options.Case = new Case("Case1 Num", "Case1 Name");
        }, BookingStatus.Created);
        var hearingId = hearing.Id;
        var request = BuildRequest();

        // act
        using var client = Application.CreateClient();
        var result = await client.PutAsync(ApiUriFactory.HearingsEndpoints.UpdateHearingDetails(hearingId),
            RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        await using var db = new BookingsDbContext(BookingsDbContextOptions);
        var hearingFromDb = await db.VideoHearings.Include(x => x.HearingVenue).FirstAsync(x => x.Id == hearingId);

        var serviceBusStub =
            Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
        var message = serviceBusStub!.ReadMessageFromQueue();
        message.IntegrationEvent.Should().BeOfType<HearingDetailsUpdatedIntegrationEvent>();
        
        hearingFromDb.HearingVenueName.Should().Be("Manchester County and Family Court");
        hearingFromDb.ScheduledDuration.Should().Be(request.ScheduledDuration);
        hearingFromDb.ScheduledDateTime.Should().Be(request.ScheduledDateTime.ToUniversalTime());
        hearingFromDb.UpdatedBy.Should().Be(request.UpdatedBy);
        hearingFromDb.HearingRoomName.Should().Be(request.HearingRoomName);
        hearingFromDb.OtherInformation.Should().Be(request.OtherInformation);
        
        var createdResponse = await ApiClientResponse.GetResponses<HearingDetailsResponse>(result.Content);
        createdResponse.HearingVenueName.Should().Be("Manchester County and Family Court");
        createdResponse.ScheduledDuration.Should().Be(request.ScheduledDuration);
        createdResponse.ScheduledDateTime.Should().Be(request.ScheduledDateTime.ToUniversalTime());
        createdResponse.UpdatedBy.Should().Be(request.UpdatedBy);
        createdResponse.HearingRoomName.Should().Be(request.HearingRoomName);
        createdResponse.OtherInformation.Should().Be(request.OtherInformation);
        
        var integrationEvent = message.IntegrationEvent as HearingDetailsUpdatedIntegrationEvent;
        integrationEvent!.Hearing.HearingVenueName.Should().Be("Manchester County and Family Court");
        integrationEvent.Hearing.ScheduledDuration.Should().Be(request.ScheduledDuration);
        integrationEvent.Hearing.ScheduledDateTime.Should().Be(request.ScheduledDateTime.ToUniversalTime());
        integrationEvent.Hearing.CaseName.Should().Be(request.Cases[0].Name);
        integrationEvent.Hearing.CaseNumber.Should().Be(request.Cases[0].Number);
    }

    [Test]
    public async Task should_update_hearing_and_not_publish_when_hearing_status_is_not_created()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearing(status: BookingStatus.Booked);
        var hearingId = hearing.Id;
        var request = BuildRequest();

        using var client = Application.CreateClient();
        var result = await client.PutAsync(ApiUriFactory.HearingsEndpoints.UpdateHearingDetails(hearingId),
            RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var serviceBusStub = Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
        var message = serviceBusStub!.ReadMessageFromQueue();
        message.Should().BeNull();
    }
    
    private static UpdateHearingRequest BuildRequest()
    {
        var cases = Builder<CaseRequest>.CreateListOfSize(1).Build().ToList();
        cases[0].IsLeadCase = false;
        cases[0].Name = $"auto test validation {Faker.RandomNumber.Next(0, 9999999)}";
        cases[0].Number = $"{Faker.RandomNumber.Next(0, 9999)}/{Faker.RandomNumber.Next(0, 9999)}";
        return new UpdateHearingRequest
        {
            ScheduledDuration = 60,
            ScheduledDateTime = DateTime.Today.AddDays(5).AddHours(10).AddMinutes(30),
            HearingRoomName = "RoomUpdate",
            OtherInformation = "OtherInformationUpdate",
            UpdatedBy = "test@hmcts.net",
            Cases = cases,
            AudioRecordingRequired = false,
            HearingVenueName = "Manchester County and Family Court"
        };
    }
}