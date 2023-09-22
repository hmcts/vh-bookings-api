using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using BookingsApi.Validations.Common;
using BookingsApi.Validations.V1;

namespace BookingsApi.IntegrationTests.Api.V1.HearingParticipants;

public class UpdateParticipantDetailsTests : ApiTest
{
    [Test]
    public async Task should_return_validation_error_when_request_validation_fails()
    {
        // arrange
        var hearingId = Guid.NewGuid();
        var participantId = Guid.NewGuid();
        var request = new UpdateParticipantRequest()
        {
            ParticipantId = Guid.Empty,
            ContactEmail = null,
            DisplayName = null,
            OrganisationName = null,
            Representee = null,
            TelephoneNumber = null,
            Title = null,
            LinkedParticipants = new List<LinkedParticipantRequest>()
        };

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PutAsync(ApiUriFactory.ParticipantsEndpoints.UpdateParticipantDetails(hearingId, participantId),RequestBody.Set(request));


        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        var errorMessages = validationProblemDetails.Errors.SelectMany(e => e.Value).ToList();
        errorMessages.Should().Contain(x => x.Contains(UpdateParticipantRequestValidation.NoDisplayNameErrorMessage));
    }
    
    [Test]
    public async Task should_return_not_found_when_hearing_does_not_exist()
    {
        // arrange
        var hearingId = Guid.NewGuid();
        var participantId = Guid.NewGuid();
        var request = new UpdateParticipantRequest()
        {
            ParticipantId = participantId,
            ContactEmail = "random@test.com",
            DisplayName = "Foo",
            OrganisationName = "Bar",
            Representee = null,
            TelephoneNumber = "01234567890",
            Title = "Title",
            LinkedParticipants = new List<LinkedParticipantRequest>()
        };

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PutAsync(ApiUriFactory.ParticipantsEndpoints.UpdateParticipantDetails(hearingId, participantId),RequestBody.Set(request));


        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        ApiClientResponse.GetResponses<string>(result.Content).Result.Should().Be($"Video hearing {hearingId} not found");
    }
    
    [Test]
    public async Task should_return_not_found_when_participant_does_not_exist()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearing();
        var hearingId = hearing.Id;
        var participantId = Guid.NewGuid();
        var request = new UpdateParticipantRequest()
        {
            ParticipantId = participantId,
            ContactEmail = "random@test.com",
            DisplayName = "Foo",
            OrganisationName = "Bar",
            Representee = null,
            TelephoneNumber = "01234567890",
            Title = "Title",
            LinkedParticipants = new List<LinkedParticipantRequest>()
        };

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PutAsync(ApiUriFactory.ParticipantsEndpoints.UpdateParticipantDetails(hearingId, participantId),RequestBody.Set(request));


        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        ApiClientResponse.GetResponses<string>(result.Content).Result.Should().Be($"Participant {participantId} not found for hearing {hearingId}");
    }

    [Test]
    public async Task
        should_return_validation_error_when_updating_a_representative_without_representee_or_organisation_name()
    {
        var hearing = await Hooks.SeedVideoHearing();
        var hearingId = hearing.Id;
        var participant = hearing.GetParticipants().First(x=> x is Representative);
        var participantId = participant.Id;
        var request = new UpdateParticipantRequest()
        {
            ParticipantId = participantId,
            ContactEmail = participant.Person.ContactEmail,
            DisplayName = "Foo",
            OrganisationName = null,
            Representee = null,
            TelephoneNumber = "01234567890",
            Title = participant.Person.Title,
            LinkedParticipants = new List<LinkedParticipantRequest>()
        };

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PutAsync(ApiUriFactory.ParticipantsEndpoints.UpdateParticipantDetails(hearingId, participantId),
                RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        var errorMessages = validationProblemDetails.Errors.SelectMany(e => e.Value).ToList();
        errorMessages.Should().Contain(x => x.Contains(RepresentativeValidation.NoOrganisation));
        errorMessages.Should().Contain(x => x.Contains(RepresentativeValidation.NoRepresentee));
    }

    [Test]
    public async Task should_update_a_participant_and_publish_event_when_hearing_is_confirmed()
    {
        var hearing = await Hooks.SeedVideoHearing(status:BookingStatus.Created);
        var hearingId = hearing.Id;
        var participant = hearing.GetParticipants().First(x=> x is Individual);
        var participantId = participant.Id;
        var request = new UpdateParticipantRequest()
        {
            ParticipantId = participantId,
            ContactEmail = participant.Person.ContactEmail,
            DisplayName = "New Display Name",
            OrganisationName = null,
            Representee = null,
            TelephoneNumber = "01526791027",
            Title = participant.Person.Title,
            LinkedParticipants = new List<LinkedParticipantRequest>()
        };

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PutAsync(ApiUriFactory.ParticipantsEndpoints.UpdateParticipantDetails(hearingId, participantId),
                RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var participantResponse = await ApiClientResponse.GetResponses<ParticipantResponse>(result.Content);
        participantResponse.Id.Should().Be(participantId);
        participantResponse.DisplayName.Should().Be(request.DisplayName);
        participantResponse.ContactEmail.Should().Be(request.ContactEmail);
        participantResponse.TelephoneNumber.Should().Be(request.TelephoneNumber);
        participantResponse.Title.Should().Be(request.Title);
        participantResponse.Representee.Should().BeNull();
        participantResponse.Organisation.Should().Be(participant.Person.Organisation?.Name);
        participantResponse.LinkedParticipants.Should().BeEmpty();
        
        var serviceBusStub = Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
        var message = serviceBusStub!.ReadMessageFromQueue();
        message.IntegrationEvent.Should().BeOfType<ParticipantUpdatedIntegrationEvent>();
        var integrationEvent = message.IntegrationEvent as ParticipantUpdatedIntegrationEvent;
        integrationEvent!.Participant.ParticipantId.Should().Be(participantId);
        integrationEvent!.Participant.DisplayName.Should().Be(request.DisplayName);
        integrationEvent!.Participant.ContactEmail.Should().Be(request.ContactEmail);
        integrationEvent!.Participant.ContactTelephone.Should().Be(request.TelephoneNumber);
        integrationEvent!.Participant.Representee.Should().BeEmpty();
    }
    
    [Test]
    public async Task should_update_a_participant_and_not_publish_event_when_hearing_is_not_confirmed()
    {
        var hearing = await Hooks.SeedVideoHearing(status:BookingStatus.Booked);
        var hearingId = hearing.Id;
        var participant = hearing.GetParticipants().First(x=> x is Individual);
        var participantId = participant.Id;
        var request = new UpdateParticipantRequest()
        {
            ParticipantId = participantId,
            ContactEmail = participant.Person.ContactEmail,
            DisplayName = "New Display Name",
            OrganisationName = null,
            Representee = null,
            TelephoneNumber = "01526791027",
            Title = participant.Person.Title,
            LinkedParticipants = new List<LinkedParticipantRequest>()
        };

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PutAsync(ApiUriFactory.ParticipantsEndpoints.UpdateParticipantDetails(hearingId, participantId),
                RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var participantResponse = await ApiClientResponse.GetResponses<ParticipantResponse>(result.Content);
        participantResponse.Id.Should().Be(participantId);
        participantResponse.DisplayName.Should().Be(request.DisplayName);
        participantResponse.ContactEmail.Should().Be(request.ContactEmail);
        participantResponse.TelephoneNumber.Should().Be(request.TelephoneNumber);
        participantResponse.Title.Should().Be(request.Title);
        participantResponse.Representee.Should().BeNull();
        participantResponse.Organisation.Should().Be(participant.Person.Organisation?.Name);
        participantResponse.LinkedParticipants.Should().BeEmpty();
        
        var serviceBusStub = Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
        serviceBusStub!.Count.Should().Be(0);
    }
}