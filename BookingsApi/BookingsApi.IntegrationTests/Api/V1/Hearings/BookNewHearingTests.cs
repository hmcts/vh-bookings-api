using BookingsApi.Common.Services;
using BookingsApi.Contract.V1.Enums;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using BookingsApi.Validations.V1;
using System.Threading;
using Testing.Common.Builders.Api.V1.Request;
using Testing.Common.Stubs;

namespace BookingsApi.IntegrationTests.Api.V1.Hearings;

public class BookNewHearingTests : ApiTest
{
    private readonly List<Guid> _hearingIds = new();

    [SetUp]
    public void Setup()
    {
        _hearingIds.Clear();
    }
    
    [TearDown]
    public new async Task TearDown()
    {
        foreach (var hearingId in _hearingIds)
        {
            await Hooks.RemoveVideoHearing(hearingId);
        }
    }
    
    [Test]
    public async Task should_book_a_hearing()
    {
        // arrange
        var request = CreateBookingRequest();
        
        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.BookNewHearing, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var getHearingUri = result.Headers.Location;
        var getResponse = await client.GetAsync(getHearingUri);
        var createdResponse = await ApiClientResponse.GetResponses<HearingDetailsResponse>(result.Content);
        var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponse>(getResponse.Content);
        createdResponse.Should().BeEquivalentTo(hearingResponse);
        _hearingIds.Add(hearingResponse.Id);
    }
    
    [Test]
    public async Task should_book_a_hearing_with_a_judge_only()
    {
        // arrange
        var request = CreateBookingRequest();
        var panelMember = request.Participants.SingleOrDefault(x => x.HearingRoleName == "Panel Member");
        request.Participants.Remove(panelMember);
        request.Participants = request.Participants.Where(x=> x.HearingRoleName == "Judge").ToList();
        var judge = request.Participants[0];
        var serviceBusStub = Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.BookNewHearing, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var getHearingUri = result.Headers.Location;
        var getResponse = await client.GetAsync(getHearingUri);
        var createdResponse = await ApiClientResponse.GetResponses<HearingDetailsResponse>(result.Content);
        var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponse>(getResponse.Content);
        createdResponse.Should().BeEquivalentTo(hearingResponse);
        _hearingIds.Add(hearingResponse.Id);

        var messages = serviceBusStub.ReadAllMessagesFromQueue(hearingResponse.Id);
        var message = messages.Single(x => x.IntegrationEvent.GetType() == typeof(HearingIsReadyForVideoIntegrationEvent));
        var integrationEvent = message.IntegrationEvent as HearingIsReadyForVideoIntegrationEvent;
        integrationEvent!.Participants.Should().Contain(x=> 
            x.ContactEmail == judge.ContactEmail && x.HearingRole == "Judge" && x.UserRole == "Judge");
    }

    [Test]
    public async Task should_book_a_hearing_with_a_panelmember_with_no_telepehone_number()
    {
        // arrange
        var request = CreateBookingRequest();

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.BookNewHearing, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Created);

        var getHearingUri = result.Headers.Location;
        var getResponse = await client.GetAsync(getHearingUri);
        var createdResponse = await ApiClientResponse.GetResponses<HearingDetailsResponse>(result.Content);
        var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponse>(getResponse.Content);
        createdResponse.Should().BeEquivalentTo(hearingResponse);

        _hearingIds.Add(hearingResponse.Id);
        var panel1 = hearingResponse.Participants.First(p => p.HearingRoleName.ToLowerInvariant() == "Panel Member".ToLowerInvariant());
        panel1.UserRoleName.Should().Be("Judicial Office Holder");
        panel1.TelephoneNumber.Should().BeNullOrEmpty();
    }

    [Test]
    public async Task should_return_validation_error_when_validation_fails()
    {
        // arrange
        var request = CreateBookingRequest();
        request.HearingVenueName = null;
        request.CaseTypeName = null;
        request.HearingTypeName = null;
        request.Cases = new List<CaseRequest>();
        request.Participants = new List<ParticipantRequest>();
        request.ScheduledDuration = -100;
        request.ScheduledDateTime = DateTime.Today.AddDays(-5);

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.BookNewHearing, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[nameof(request.HearingVenueName)][0].Should()
            .Be(BookNewHearingRequestValidation.HearingVenueErrorMessage);
        
        validationProblemDetails.Errors[nameof(request.CaseTypeName)][0].Should()
            .Be(BookNewHearingRequestValidation.CaseTypeNameErrorMessage);
        
        validationProblemDetails.Errors[nameof(request.HearingTypeName)][0].Should()
            .Be(BookNewHearingRequestValidation.HearingTypeNameErrorMessage);
        
        validationProblemDetails.Errors[nameof(request.Cases)][0].Should()
            .Be("'Cases' must not be empty.");
        
        validationProblemDetails.Errors[nameof(request.Cases)][1].Should()
            .Be(BookNewHearingRequestValidation.CasesErrorMessage);
        
        validationProblemDetails.Errors[nameof(request.Participants)][0].Should()
            .Be("'Participants' must not be empty.");
        
        validationProblemDetails.Errors[nameof(request.Participants)][1].Should()
            .Be(BookNewHearingRequestValidation.ParticipantsErrorMessage);
        
        validationProblemDetails.Errors[nameof(request.ScheduledDuration)][0].Should()
            .Be(BookNewHearingRequestValidation.ScheduleDurationErrorMessage);
        
        validationProblemDetails.Errors[nameof(request.ScheduledDateTime)][0].Should()
            .Be(BookNewHearingRequestValidation.ScheduleDateTimeInPastErrorMessage);
    }
    
    [Test]
    public async Task should_return_validation_error_when_case_type_name_is_not_found()
    {
        // arrange
        var request = CreateBookingRequest();
        request.CaseTypeName = "doesnotexist";

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.BookNewHearing, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[nameof(request.CaseTypeName)][0].Should()
            .Be("Case type does not exist");
    }
    
    [Test]
    public async Task should_return_validation_error_when_hearing_type_name_is_not_found()
    {
        // arrange
        var request = CreateBookingRequest();
        request.HearingTypeName = "doesnotexist";

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.BookNewHearing, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[nameof(request.HearingTypeName)][0].Should()
            .Be("Hearing type does not exist");
    }
    
    [Test]
    public async Task should_return_validation_error_when_venue_name_is_not_found()
    {
        // arrange
        var request = CreateBookingRequest();
        request.HearingVenueName = "doesnotexist";

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.BookNewHearing, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[nameof(request.HearingVenueName)][0].Should()
            .Be("Hearing venue does not exist");
    }
            
    [Test]
    public async Task should_book_a_hearing_without_a_judge()
    {
        // arrange
        var request = CreateBookingRequest();
        var judge = request.Participants.Find(p => p.HearingRoleName == "Judge"); 
        request.Participants.Remove(judge);
        
        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.BookNewHearing, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var getHearingUri = result.Headers.Location;
        var getResponse = await client.GetAsync(getHearingUri);
        var createdResponse = await ApiClientResponse.GetResponses<HearingDetailsResponse>(result.Content);
        var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponse>(getResponse.Content);
        createdResponse.Should().BeEquivalentTo(hearingResponse);
        hearingResponse.Status.Should().Be(BookingStatus.BookedWithoutJudge);
        _hearingIds.Add(hearingResponse.Id);
        
    }

    [Test]
    public async Task should_have_sent_relevant_judge_message_to_the_queue_when_a_hearing_booked_with_judge()
    {
        // arrange
        var request = CreateBookingRequest();

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.BookNewHearing, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Created);

        var response = await ApiClientResponse.GetResponses<HearingDetailsResponse>(result.Content);
        var serviceBusStub = Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
        var messages = serviceBusStub!.ReadAllMessagesFromQueue(response.Id);
        var judgeMessage = messages.SingleOrDefault(x => x.IntegrationEvent is ExistingParticipantHearingConfirmationEvent &&
            ((ExistingParticipantHearingConfirmationEvent)x.IntegrationEvent).HearingConfirmationForParticipant.UserRole == "Judge");

        judgeMessage.Should().NotBeNull();
        _hearingIds.Add(response.Id);
    }

    [Test]
    public async Task should_not_send_judge_message_to_the_queue_when_a_new_participant_added_to_existing_hearing()
    {
        // arrange
        var request = CreateBookingRequest();
        var featureToggles = (FeatureTogglesStub)Application.Services.GetService(typeof(IFeatureToggles));
        featureToggles.NewTemplates = false;

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.BookNewHearing, RequestBody.Set(request));
        var response = await ApiClientResponse.GetResponses<HearingDetailsResponse>(result.Content);
        var serviceBusStub = Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
        serviceBusStub.ReadAllMessagesFromQueue(response.Id);

        var participantContactEmail = "lit.one@lit.com";
        var updateRequest = new UpdateHearingParticipantsRequest
        {
            NewParticipants = new List<ParticipantRequest> {
            new ParticipantRequest
            {
                CaseRoleName = "Applicant",
                HearingRoleName = "Litigant in person",
                Representee = null,
                FirstName = "Lit",
                LastName = "One",
                TelephoneNumber = "12222222222",
                ContactEmail = participantContactEmail,
                DisplayName = "Lit One"
            } }
        };

        var currentTimeStamp = DateTime.UtcNow;
        var updatedResult = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpoints.UpdateHearingParticipants(response.Id), RequestBody.Set(updateRequest));

        // assert
        updatedResult.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var messages = serviceBusStub.ReadAllMessagesFromQueue(response.Id);

        messages.Should().Contain(x => x.IntegrationEvent is HearingParticipantsUpdatedIntegrationEvent);
        messages.Should().Contain(x => x.IntegrationEvent is HearingNotificationIntegrationEvent);
        var participantMessage = messages.SingleOrDefault(x => x.IntegrationEvent is HearingNotificationIntegrationEvent &&
            ((HearingNotificationIntegrationEvent)x.IntegrationEvent).HearingConfirmationForParticipant.UserRole == "Individual" &&
            ((HearingNotificationIntegrationEvent)x.IntegrationEvent).HearingConfirmationForParticipant.ContactEmail == participantContactEmail);

        participantMessage.Should().NotBeNull();

        var judgeMessage = messages.SingleOrDefault(x => x.IntegrationEvent is HearingNotificationIntegrationEvent &&
            ((HearingNotificationIntegrationEvent)x.IntegrationEvent).HearingConfirmationForParticipant.UserRole == "Judge" && x.Timestamp <= response.CreatedDate &&
            x.Timestamp >= currentTimeStamp);

        judgeMessage.Should().BeNull();
        _hearingIds.Add(response.Id);
    }


    [Test]
    public async Task should_have_sent_relevant_judge_message_to_the_queue_when_a_judge_updated_to_the_existing_booking()
    {
        // arrange
        var request = CreateBookingRequest();
        var featureToggles = (FeatureTogglesStub)Application.Services.GetService(typeof(IFeatureToggles));
        featureToggles.NewTemplates = false;

        await Hooks.SeedJudgePerson("Judge", "auto_aw.judge_update@hearings.reform.hmcts.net", "auto_aw", "judge_update", "auto_aw.judge_update@hmcts.net", "1233333333");

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.BookNewHearing, RequestBody.Set(request));
        var response = await ApiClientResponse.GetResponses<HearingDetailsResponse>(result.Content);

        var judge = response.Participants.First(e => e.UserRoleName == "Judge");
        var otherParticipant = response.Participants.Last(p => p.Username != "Judge");
        var newJudgeUsername = "auto_aw.judge_update@hearings.reform.hmcts.net";
        var updateRequest = new UpdateHearingParticipantsRequest
        {
            RemovedParticipantIds = new List<Guid> { judge.Id },
            NewParticipants = new List<ParticipantRequest> {
            new ParticipantRequest
            {
                CaseRoleName = "Judge",
                HearingRoleName = "Judge",
                Representee = null,
                FirstName = "Automation_Judge",
                LastName = "Judge_1",
                ContactEmail = "auto_aw.judge_update@hmcts.net",
                Username = newJudgeUsername,
                DisplayName = "Auto Judge_01"
            } }, 
            ExistingParticipants = new List<UpdateParticipantRequest>
            {
                new UpdateParticipantRequest
                {
                    ParticipantId = otherParticipant.Id,
                    Title = otherParticipant.Title,
                    TelephoneNumber = otherParticipant.TelephoneNumber,
                    ContactEmail = otherParticipant.ContactEmail,
                    DisplayName = otherParticipant.DisplayName,
                }
            }
        };

        var currentTimeStamp = DateTime.UtcNow;
        var updatedResult = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpoints.UpdateHearingParticipants(response.Id), RequestBody.Set(updateRequest));

        // assert
        updatedResult.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var serviceBusStub = Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
        var messages = serviceBusStub!.ReadAllMessagesFromQueue(response.Id);

        messages.Should().Contain(x => x.IntegrationEvent is HearingParticipantsUpdatedIntegrationEvent);
        messages.Should().Contain(x => x.IntegrationEvent is HearingNotificationIntegrationEvent);

        var judgeMessage = messages.SingleOrDefault(x => x.IntegrationEvent is HearingNotificationIntegrationEvent &&
            ((HearingNotificationIntegrationEvent)x.IntegrationEvent).HearingConfirmationForParticipant.UserRole == "Judge" &&
            ((HearingNotificationIntegrationEvent)x.IntegrationEvent).HearingConfirmationForParticipant.Username == newJudgeUsername &&
            x.Timestamp >= currentTimeStamp);

        judgeMessage.Should().NotBeNull();
        _hearingIds.Add(response.Id);
    }

    private BookNewHearingRequest CreateBookingRequest()
    {
        var hearingSchedule = DateTime.UtcNow.AddMinutes(5);
        var caseName = "Bookings Api Integration Automated";
        var request = new SimpleBookNewHearingRequest(caseName, hearingSchedule).Build();
        request.CaseTypeName = "Generic";
        request.HearingTypeName = "Automated Test";
        request.HearingVenueName = "Birmingham Civil and Family Justice Centre";
        return request;
    }
}