using BookingsApi.Contract.V2.Requests;
using BookingsApi.Contract.V2.Responses;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Validations;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using BookingsApi.Validations.V2;
using FizzWare.NBuilder;

namespace BookingsApi.IntegrationTests.Api.V2.Hearings;

public class UpdateHearingV2Tests : ApiTest
{

    [Test]
    public async Task should_return_bad_request_and_validation_errors_when_payload_fails_validation()
    {
        // arrange
        var hearingId = Guid.NewGuid();
        var request = new UpdateHearingRequestV2 {ScheduledDateTime = DateTime.MinValue};

        // act
        using var client = Application.CreateClient();
        var result = await client.PutAsync(ApiUriFactory.HearingsEndpointsV2.UpdateHearingDetails(hearingId), RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[nameof(request.HearingVenueCode)][0].Should()
            .Be(UpdateHearingRequestValidationV2.NoHearingVenueCodeErrorMessage);
        
        validationProblemDetails.Errors[nameof(request.ScheduledDuration)][0].Should()
            .Be(UpdateHearingRequestValidationV2.NoScheduleDurationErrorMessage);
        
        validationProblemDetails.Errors[nameof(request.ScheduledDateTime)][0].Should()
            .Be(UpdateHearingRequestValidationV2.ScheduleDateTimeInPastErrorMessage);
        
        validationProblemDetails.Errors[nameof(request.UpdatedBy)][0].Should()
            .Be(UpdateHearingRequestValidationV2.NoUpdatedByErrorMessage);
    }
    
    [Test]
    public async Task should_return_not_found_when_attempting_update_a_hearing_that_does_not_exist()
    {
        // arrange
        var hearingId = Guid.NewGuid();
        var request = BuildRequest();

        // act
        using var client = Application.CreateClient();
        var result = await client.PutAsync(ApiUriFactory.HearingsEndpointsV2.UpdateHearingDetails(hearingId), RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Test]
    public async Task should_return_bad_request_and_validation_errors_when_venue_does_not_exist()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2();
        var hearingId = hearing.Id;
        var request = BuildRequest();
        request.HearingVenueCode = "ShouldNotExist";

        // act
        using var client = Application.CreateClient();
        var result = await client.PutAsync(ApiUriFactory.HearingsEndpointsV2.UpdateHearingDetails(hearingId), RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[nameof(request.HearingVenueCode)][0].Should()
            .Be($"Hearing venue code {request.HearingVenueCode} does not exist");
    }
    
    [Test]
    public async Task should_return_bad_request_and_validation_failure_when_editing_a_created_hearing_close_to_start_time()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(options => options.ScheduledDate = DateTime.UtcNow.AddMinutes(5), BookingStatus.Created);
        var hearingId = hearing.Id;
        var request = BuildRequest();
        
        // act
        using var client = Application.CreateClient();
        var result = await client.PutAsync(ApiUriFactory.HearingsEndpointsV2.UpdateHearingDetails(hearingId), RequestBody.Set(request));
        
        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors["Hearing"].Should().Contain(DomainRuleErrorMessages.CannotUpdateHearingDetailsCloseToStartTime);
    }

    [Test]
    public async Task should_return_bad_request_when_changing_hearing_scheduled_datetime_to_be_on_same_date_as_another_hearing_in_hearing_group()
    {
        // arrange
        var dates = new List<DateTime>
        {
            DateTime.Today.AddDays(5).AddHours(10).ToUniversalTime(),
            DateTime.Today.AddDays(6).AddHours(10).ToUniversalTime(),
            DateTime.Today.AddDays(7).AddHours(10).ToUniversalTime()
        };
        var hearingsInGroup = await Hooks.SeedMultiDayHearing(useV2: true, dates);
        var hearingToUpdate = hearingsInGroup[^1];
        var hearingId = hearingToUpdate.Id;
        var request = BuildRequest();
        request.ScheduledDateTime = hearingsInGroup[0].ScheduledDateTime.AddHours(1);

        // act
        using var client = Application.CreateClient();
        var result = await client.PutAsync(ApiUriFactory.HearingsEndpointsV2.UpdateHearingDetails(hearingId), RequestBody.Set(request));
        
        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[nameof(request.ScheduledDateTime)].Should().Contain(DomainRuleErrorMessages.CannotBeOnSameDateAsOtherHearingInGroup);
    }

    [Test]
    public async Task should_update_hearing_when_changing_hearing_scheduled_datetime_to_be_on_different_date_to_other_hearings_in_hearing_group()
    {
        // arrange
        var dates = new List<DateTime>
        {
            DateTime.Today.AddDays(5).AddHours(10).ToUniversalTime(),
            DateTime.Today.AddDays(6).AddHours(10).ToUniversalTime(),
            DateTime.Today.AddDays(7).AddHours(10).ToUniversalTime()
        };
        var hearingsInGroup = await Hooks.SeedMultiDayHearing(useV2: true, dates);
        var hearingToUpdate = hearingsInGroup[0];
        var hearingId = hearingToUpdate.Id;
        var request = BuildRequest();
        request.ScheduledDateTime = hearingsInGroup.Max(x => x.ScheduledDateTime).AddDays(1);
        
        // act
        using var client = Application.CreateClient();
        var result = await client.PutAsync(ApiUriFactory.HearingsEndpointsV2.UpdateHearingDetails(hearingId), RequestBody.Set(request));
        
        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var response = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(result.Content);
        response.ScheduledDateTime.Should().Be(request.ScheduledDateTime);
    }

    [Test]
    public async Task should_update_hearing_when_hearing_scheduled_datetime_date_is_unchanged_for_hearing_in_group()
    {
        // arrange
        var dates = new List<DateTime>
        {
            DateTime.Today.AddDays(5).AddHours(10).ToUniversalTime(),
            DateTime.Today.AddDays(6).AddHours(10).ToUniversalTime(),
            DateTime.Today.AddDays(7).AddHours(10).ToUniversalTime()
        };
        var hearingsInGroup = await Hooks.SeedMultiDayHearing(useV2: true, dates);
        var hearingToUpdate = hearingsInGroup[0];
        var hearingId = hearingToUpdate.Id;
        var request = BuildRequest();
        request.ScheduledDateTime = hearingToUpdate.ScheduledDateTime.AddHours(1);
        
        // act
        using var client = Application.CreateClient();
        var result = await client.PutAsync(ApiUriFactory.HearingsEndpointsV2.UpdateHearingDetails(hearingId), RequestBody.Set(request));
        
        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var response = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(result.Content);
        response.ScheduledDateTime.Should().Be(request.ScheduledDateTime);
    }

    [Test]
    public async Task should_update_hearing_and_publish_when_hearing_status_is_created()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(options => { options.Case = new Case("Case1 Num", "Case1 Name"); },
            BookingStatus.Created);
        var hearingId = hearing.Id;
        var request = BuildRequest();

        // act
        using var client = Application.CreateClient();
        var result = await client.PutAsync(ApiUriFactory.HearingsEndpointsV2.UpdateHearingDetails(hearingId),
            RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        await using var db = new BookingsDbContext(BookingsDbContextOptions);
        var hearingFromDb = await db.VideoHearings.Include(x => x.HearingVenue).FirstAsync(x => x.Id == hearingId);

        var serviceBusStub =
            Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
        var message = serviceBusStub!.ReadAllMessagesFromQueue(hearingId)[0];
        message.IntegrationEvent.Should().BeOfType<HearingDetailsUpdatedIntegrationEvent>();

        hearingFromDb.HearingVenue.VenueCode.Should().Be(request.HearingVenueCode);
        hearingFromDb.ScheduledDuration.Should().Be(request.ScheduledDuration);
        hearingFromDb.ScheduledDateTime.Should().Be(request.ScheduledDateTime!.Value.ToUniversalTime());
        hearingFromDb.UpdatedBy.Should().Be(request.UpdatedBy);
        hearingFromDb.HearingRoomName.Should().Be(request.HearingRoomName);
        hearingFromDb.OtherInformation.Should().Be(request.OtherInformation);

        var createdResponse = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(result.Content);
        createdResponse.HearingVenueCode.Should().Be(request.HearingVenueCode);
        createdResponse.ScheduledDuration.Should().Be(request.ScheduledDuration);
        createdResponse.ScheduledDateTime.Should().Be(request.ScheduledDateTime!.Value.ToUniversalTime());
        createdResponse.UpdatedBy.Should().Be(request.UpdatedBy);
        createdResponse.HearingRoomName.Should().Be(request.HearingRoomName);
        createdResponse.OtherInformation.Should().Be(request.OtherInformation);

        var integrationEvent = message.IntegrationEvent as HearingDetailsUpdatedIntegrationEvent;
        integrationEvent!.Hearing.HearingVenueName.Should().Be("Manchester County and Family Court");
        integrationEvent.Hearing.ScheduledDuration.Should().Be(request.ScheduledDuration);
        integrationEvent.Hearing.ScheduledDateTime.Should().Be(request.ScheduledDateTime!.Value.ToUniversalTime());
        integrationEvent.Hearing.CaseName.Should().Be(request.Cases[0].Name);
        integrationEvent.Hearing.CaseNumber.Should().Be(request.Cases[0].Number);

        //validate all participants are notified
        var notifications = serviceBusStub
            .ReadAllMessagesFromQueue(hearingId)
            .Where(x => x.IntegrationEvent is HearingAmendmentNotificationEvent)
            .Select(x => x.IntegrationEvent as HearingAmendmentNotificationEvent)
            .ToList();
        
        foreach (var participants in hearing.Participants)
            notifications.Should().Contain(x
                => x.HearingConfirmationForParticipant.ContactEmail == participants.Person.ContactEmail);
        foreach (var judiciaryParticipants in hearing.JudiciaryParticipants)
            notifications.Should().Contain(x
                => x.HearingConfirmationForParticipant.ContactEmail == judiciaryParticipants.GetEmail());
    }


    [Test]
    public async Task should_update_hearing_with_no_hearing_type_and_publish_when_hearing_status_is_created()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(options =>
        {
            options.Case = new Case("Case1 Num", "Case1 Name");
            options.ExcludeHearingType = true;
        }, BookingStatus.Created);
        var hearingId = hearing.Id;
        var request = BuildRequest();

        // act
        using var client = Application.CreateClient();
        var result = await client.PutAsync(ApiUriFactory.HearingsEndpointsV2.UpdateHearingDetails(hearingId),
            RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        await using var db = new BookingsDbContext(BookingsDbContextOptions);
        var hearingFromDb = await db.VideoHearings.Include(x => x.HearingVenue).FirstAsync(x => x.Id == hearingId);

        hearingFromDb.HearingVenue.VenueCode.Should().Be(request.HearingVenueCode);
        hearingFromDb.ScheduledDuration.Should().Be(request.ScheduledDuration);
        hearingFromDb.ScheduledDateTime.Should().Be(request.ScheduledDateTime!.Value.ToUniversalTime());
        hearingFromDb.UpdatedBy.Should().Be(request.UpdatedBy);
        hearingFromDb.HearingRoomName.Should().Be(request.HearingRoomName);
        hearingFromDb.OtherInformation.Should().Be(request.OtherInformation);
        
        var createdResponse = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(result.Content);
        createdResponse.HearingVenueCode.Should().Be(request.HearingVenueCode);
        createdResponse.ScheduledDuration.Should().Be(request.ScheduledDuration);
        createdResponse.ScheduledDateTime.Should().Be(request.ScheduledDateTime!.Value.ToUniversalTime());
        createdResponse.UpdatedBy.Should().Be(request.UpdatedBy);
        createdResponse.HearingRoomName.Should().Be(request.HearingRoomName);
        createdResponse.OtherInformation.Should().Be(request.OtherInformation);
    }
    
    [Test]
    public async Task should_update_hearing_and_not_publish_when_hearing_status_is_not_created()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2();
        var hearingId = hearing.Id;
        var request = BuildRequest();

        using var client = Application.CreateClient();
        var result = await client.PutAsync(ApiUriFactory.HearingsEndpointsV2.UpdateHearingDetails(hearingId),
            RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var serviceBusStub = Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
        var messages = serviceBusStub!.ReadAllMessagesFromQueue(hearingId);
        messages.Length.Should().Be(0);
    }
    
    [Test]
    public async Task should_update_hearing_when_details_are_the_same()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(options =>
        {
            options.Case = new Case("Case1 Num", "Case1 Name");
        }, BookingStatus.Created);
        var hearingId = hearing.Id;
        var request = BuildRequestWithSameDetailsAsExisting(hearing);

        var beforeUpdatedDate = hearing.UpdatedDate;
        
        // act
        using var client = Application.CreateClient();
        var result = await client.PutAsync(ApiUriFactory.HearingsEndpointsV2.UpdateHearingDetails(hearingId),
            RequestBody.Set(request));
        
        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var response = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(result.Content);
        response.UpdatedDate.Should().BeAfter(beforeUpdatedDate);
        response.UpdatedBy.Should().Be(request.UpdatedBy);
    }

    [Test]
    public async Task should_update_case_when_case_info_has_changed_and_all_other_details_are_the_same()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(options =>
        {
            options.Case = new Case("Case1 Num", "Case1 Name");
        }, BookingStatus.Created);
        var hearingId = hearing.Id;
        var request = BuildRequestWithSameDetailsAsExisting(hearing);
        
        var newCase = new
        {
            Number = "Updated Case Number",
            Name = "Updated Case Name"
        };
        request.Cases[0].Number = newCase.Number;
        request.Cases[0].Name = newCase.Name;

        // act
        using var client = Application.CreateClient();
        var result = await client.PutAsync(ApiUriFactory.HearingsEndpointsV2.UpdateHearingDetails(hearingId),
            RequestBody.Set(request));
        
        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var response = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(result.Content);
        response.Cases[0].Number.Should().Be(newCase.Number);
        response.Cases[0].Name.Should().Be(newCase.Name);
    }
    
    private static UpdateHearingRequestV2 BuildRequest()
    {
        var cases = Builder<CaseRequestV2>.CreateListOfSize(1).Build().ToList();
        cases[0].IsLeadCase = false;
        cases[0].Name = $"auto test validation {Faker.RandomNumber.Next(0, 9999999)}";
        cases[0].Number = $"{Faker.RandomNumber.Next(0, 9999)}/{Faker.RandomNumber.Next(0, 9999)}";
        return new UpdateHearingRequestV2
        {
            ScheduledDuration = 60,
            ScheduledDateTime = DateTime.Today.AddDays(5).AddHours(10).AddMinutes(30).ToUniversalTime(),
            HearingRoomName = "RoomUpdate",
            OtherInformation = "OtherInformationUpdate",
            UpdatedBy = "test@hmcts.net",
            Cases = cases,
            HearingVenueCode = "701411", // Manchester County and Family Court
            AudioRecordingRequired = false
        };
    }

    private static UpdateHearingRequestV2 BuildRequestWithSameDetailsAsExisting(Hearing hearing)
    {
        var request = BuildRequest();
        request.HearingVenueCode = hearing.HearingVenue.VenueCode;
        request.ScheduledDateTime = hearing.ScheduledDateTime;
        request.ScheduledDuration = hearing.ScheduledDuration;
        request.HearingRoomName = hearing.HearingRoomName;
        request.OtherInformation = hearing.OtherInformation;
        request.AudioRecordingRequired = hearing.AudioRecordingRequired;
        request.UpdatedBy = "UpdatedByUserName";
        request.Cases[0].Number = hearing.GetCases()[0].Number;
        request.Cases[0].Name = hearing.GetCases()[0].Name;
        return request;
    }
}