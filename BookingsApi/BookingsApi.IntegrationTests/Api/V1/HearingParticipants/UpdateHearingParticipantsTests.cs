using BookingsApi.Contract.V1.Enums;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using BookingsApi.Validations.V1;

namespace BookingsApi.IntegrationTests.Api.V1.HearingParticipants;

public class UpdateHearingParticipantsTests : ApiTest
{
    [Test]
    public async Task should_update_an_existing_participant()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearing(options
            => { options.Case = new Case("UpdateParticipantJudge", "UpdateParticipantJudge"); }, Domain.Enumerations.BookingStatus.Created);
        var participant = hearing.Participants.First(e => e.HearingRole.Name == "Litigant in person");
        var request = new UpdateHearingParticipantsRequest
        {
            ExistingParticipants = new List<UpdateParticipantRequest> { new()
            {
                ParticipantId = participant.Id, 
                DisplayName = "NewDisplayName",
                ContactEmail = participant.Person?.ContactEmail,
                FirstName = participant.Person?.FirstName,
                LastName = participant.Person?.LastName,
                OrganisationName = participant.Person?.Organisation?.Name,
                TelephoneNumber = participant.Person?.TelephoneNumber,
                Title = participant.Person?.Title
            } }
        };

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpoints.UpdateHearingParticipants(hearing.Id),RequestBody.Set(request));
        var updatedHearing = await client.GetAsync(ApiUriFactory.HearingsEndpoints.GetHearingDetailsById(hearing.Id.ToString()));
        var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponse>(updatedHearing.Content);
        
        // assert
        hearingResponse.Participants.Should().Contain(p => p.DisplayName == "NewDisplayName");
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var serviceBusStub = Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
        var messages = serviceBusStub!.ReadAllMessagesFromQueue(hearing.Id);
        var participantMessages = messages
            .Where(x => x.IntegrationEvent is ParticipantUpdatedIntegrationEvent)
            .Select(x => x.IntegrationEvent as ParticipantUpdatedIntegrationEvent)
            .Where(x => x.HearingId == hearing.Id)
            .ToList();

        participantMessages.Count.Should().Be(1);
    }
    
    [Test]
    public async Task should_change_a_judge_on_a_confirmed_hearing()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearing(options
            => { options.Case = new Case("UpdateParticipantJudge", "UpdateParticipantJudge"); }, Domain.Enumerations.BookingStatus.Created);
        var judge = hearing.Participants.First(e => e.HearingRole.IsJudge());
        const string newDisplayName = "Judge";
        var request = new UpdateHearingParticipantsRequest
        {
            RemovedParticipantIds = new List<Guid>{ judge.Id },
            NewParticipants = new List<ParticipantRequest>
            {
                new()
                {
                    ContactEmail = GenericJudge.ContactEmail,
                    DisplayName = newDisplayName,  
                    Username = GenericJudge.Username,
                    HearingRoleName = "Judge",
                    CaseRoleName = "Judge",
                    FirstName = GenericJudge.FirstName,
                    LastName = GenericJudge.LastName,
                }
            }
        };

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpoints.UpdateHearingParticipants(hearing.Id),RequestBody.Set(request));
        var updatedHearing = await client.GetAsync(ApiUriFactory.HearingsEndpoints.GetHearingDetailsById(hearing.Id.ToString()));
        var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponse>(updatedHearing.Content);
        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        hearingResponse.Participants.Should().NotContain(p => p.Id == judge.Id);
        hearingResponse.Participants.Should().Contain(p => p.Username == GenericJudge.Username);
        hearingResponse.Participants.Should().Contain(p => p.DisplayName == newDisplayName);
        hearingResponse.Status.Should().Be(BookingStatus.Created);
    }
    
    [Test]
    public async Task should_remove_a_judge_from_the_confirmed_hearing()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearing(options
            => { options.Case = new Case("UpdateParticipantsRemoveParticipant", "UpdateParticipantsRemoveParticipant"); }, Domain.Enumerations.BookingStatus.Created);
        var judge = hearing.Participants.First(e => e.HearingRole.IsJudge());
        var request = new UpdateHearingParticipantsRequest { RemovedParticipantIds = new List<Guid>{ judge.Id } };

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpoints.UpdateHearingParticipants(hearing.Id),RequestBody.Set(request));
        var updatedHearing = await client.GetAsync(ApiUriFactory.HearingsEndpoints.GetHearingDetailsById(hearing.Id.ToString()));
        var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponse>(updatedHearing.Content);
        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        hearingResponse.Participants.Should().NotContain(p => p.Id == judge.Id);
        hearingResponse.Status.Should().Be(BookingStatus.ConfirmedWithoutJudge);
    }

    [Test]
    public async Task should_return_bad_request_when_hearing_id_is_invalid()
    {
        // arrange
        var hearingId = Guid.Empty;
        var request = new UpdateHearingParticipantsRequest { RemovedParticipantIds = new List<Guid>()};
        
        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpoints.UpdateHearingParticipants(hearingId),RequestBody.Set(request));
        
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
        var request = new UpdateHearingParticipantsRequest { RemovedParticipantIds = new List<Guid>()};
        
        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpoints.UpdateHearingParticipants(hearingId),RequestBody.Set(request));
        
        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors.SelectMany(x => x.Value).Should()
            .Contain(UpdateHearingParticipantsRequestValidation.NoParticipantsErrorMessage);
    }
}