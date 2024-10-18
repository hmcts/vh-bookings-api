using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.DAL.Queries;
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
            ContactEmail = "gsdgdsgfs",
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
        errorMessages.Should().Contain(x => x.Contains(ParticipantRequestValidation.InvalidContactEmailErrorMessage));
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
        should_return_validation_error_when_updating_a_representative_without_representee()
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
        errorMessages.Should().Contain(x => x.Contains(RepresentativeValidation.NoRepresentee));
    }

    [Test]
    public async Task should_update_a_participant_contact_email_and_publish_event_when_hearing_is_confirmed()
    {
        var hearing = await Hooks.SeedVideoHearing(status:BookingStatus.Created);
        var hearingId = hearing.Id;
        var participant = hearing.GetParticipants().First(x=> x is Individual);
        var participantPersonalDetails = participant.Person;
        var participantId = participant.Id;
        var request = new UpdateParticipantRequest()
        {
            ParticipantId = participantId,
            ContactEmail = "contactEmailUpdated@email.com",
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
        var message = serviceBusStub!.ReadAllMessagesFromQueue(hearingId)[0];
        message.IntegrationEvent.Should().BeOfType<ParticipantUpdatedIntegrationEvent>();
        var integrationEvent = message.IntegrationEvent as ParticipantUpdatedIntegrationEvent;
        integrationEvent!.Participant.ParticipantId.Should().Be(participantId);
        integrationEvent!.Participant.DisplayName.Should().Be(request.DisplayName);
        integrationEvent!.Participant.ContactEmail.Should().Be(request.ContactEmail);
        integrationEvent!.Participant.ContactTelephone.Should().Be(request.TelephoneNumber);
        integrationEvent!.Participant.Representee.Should().BeEmpty();
        participantResponse.FirstName.Should().Be(participantPersonalDetails.FirstName);
        participantResponse.LastName.Should().Be(participantPersonalDetails.LastName);
        participantResponse.MiddleNames.Should().Be(participantPersonalDetails.MiddleNames);
    }
    
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    public async Task should_update_a_participant_contact_email_and_publish_event_when_hearing_is_confirmed_and_contact_email_already_exists_for_different_person(int testCase)
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearing(status:BookingStatus.Created);
        var hearingId = hearing.Id;
        var participant = hearing.GetParticipants().First(x=> x is Individual);
        var participantId = participant.Id;

        var hearing2 = await Hooks.SeedVideoHearing(status: BookingStatus.Created);
        var hearing2Participant = hearing2.GetParticipants().First(x => x is Individual);
        var contactEmail = hearing2Participant.Person.ContactEmail;
        
        switch (testCase)
        {
            case 1:
                // Identical contact emails
                break;
            case 2:
                // Upper case
                contactEmail = contactEmail.ToUpper();
                break;
            case 3:
                // Lower case
                contactEmail = contactEmail.ToLower();
                break;
        }
        
        var request = new UpdateParticipantRequest
        {
            ParticipantId = participantId,
            ContactEmail = contactEmail,
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
        participantResponse.ContactEmail.Should().Be(request.ContactEmail.Trim());
        
        var serviceBusStub = Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
        var message = serviceBusStub!.ReadAllMessagesFromQueue(hearingId)[0];
        message.IntegrationEvent.Should().BeOfType<ParticipantUpdatedIntegrationEvent>();
        var integrationEvent = message.IntegrationEvent as ParticipantUpdatedIntegrationEvent;
        integrationEvent!.Participant.ParticipantId.Should().Be(participantId);
        integrationEvent!.Participant.ContactEmail.Should().Be(request.ContactEmail.Trim());
        
        await using var db = new BookingsDbContext(BookingsDbContextOptions);
        var updatedHearing = await new GetHearingByIdQueryHandler(db).Handle(new GetHearingByIdQuery(hearingId));
        var updatedParticipant = updatedHearing.GetParticipants().First(x => x.Id == participantId);
        updatedParticipant.PersonId.Should().Be(hearing2Participant.PersonId);
    }

    [Test]
    public async Task should_update_a_participant_and_publish_event_when_hearing_is_confirmed_and_optional_fields_are_null()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearing(status:BookingStatus.Created);
        var hearingId = hearing.Id;
        var participant = hearing.GetParticipants().First(x=> x is Individual);
        var participantPersonalDetails = participant.Person;
        var participantId = participant.Id;
        var request = new UpdateParticipantRequest
        {
            ParticipantId = participantId,
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
        participantResponse.FirstName.Should().Be(participant.Person.FirstName);
        participantResponse.LastName.Should().Be(participant.Person.LastName);
        participantResponse.MiddleNames.Should().Be(participant.Person.MiddleNames);
        participantResponse.ContactEmail.Should().Be(participant.Person.ContactEmail);
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
        integrationEvent!.Participant.ContactEmail.Should().Be(participant.Person.ContactEmail);
        integrationEvent!.Participant.ContactTelephone.Should().Be(request.TelephoneNumber);
        integrationEvent!.Participant.Representee.Should().BeEmpty();
        participantResponse.FirstName.Should().Be(participantPersonalDetails.FirstName);
        participantResponse.LastName.Should().Be(participantPersonalDetails.LastName);
        participantResponse.MiddleNames.Should().Be(participantPersonalDetails.MiddleNames);
    }
    
    [Test]
    public async Task should_update_a_participants_personal_info_and_publish_event_when_hearing_is_confirmed()
    {
        var hearing = await Hooks.SeedVideoHearing(status:BookingStatus.Created);
        var hearingId = hearing.Id;
        var participant = hearing.GetParticipants().First(x=> x is Individual);
        var participantId = participant.Id;
        
        var newFirstName = "Alpha";
        var newLastName = "Beta";
        var newMiddleName = "Theta";
        
        var request = new UpdateParticipantRequest
        {
            ParticipantId = participantId,
            ContactEmail = participant.Person.ContactEmail,
            DisplayName = "New Display Name",
            OrganisationName = null,
            Representee = null,
            TelephoneNumber = "01526791027",
            Title = participant.Person.Title,
            LinkedParticipants = new List<LinkedParticipantRequest>(),
            FirstName = newFirstName,
            LastName = newLastName,
            MiddleName = newMiddleName
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
        var message = serviceBusStub!.ReadAllMessagesFromQueue(hearingId).FirstOrDefault();
        message.IntegrationEvent.Should().BeOfType<ParticipantUpdatedIntegrationEvent>();
        var integrationEvent = message.IntegrationEvent as ParticipantUpdatedIntegrationEvent;
        integrationEvent!.Participant.ParticipantId.Should().Be(participantId);
        integrationEvent!.Participant.DisplayName.Should().Be(request.DisplayName);
        integrationEvent!.Participant.ContactEmail.Should().Be(request.ContactEmail);
        integrationEvent!.Participant.ContactTelephone.Should().Be(request.TelephoneNumber);
        integrationEvent!.Participant.Representee.Should().BeEmpty();
        participantResponse.FirstName.Should().Be(newFirstName);
        participantResponse.LastName.Should().Be(newLastName);
        participantResponse.MiddleNames.Should().Be(newMiddleName);
    }
    
    [Test]
    public async Task should_update_a_participant_and_publish_event_when_hearing_is_not_confirmed()
    {
        var hearing = await Hooks.SeedVideoHearing(status:BookingStatus.Booked);
        var hearingId = hearing.Id;
        var participant = hearing.GetParticipants().First(x=> x is Individual);
        var participantPersonalDetails = participant.Person;
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
        participantResponse.FirstName.Should().Be(participantPersonalDetails.FirstName);
        participantResponse.LastName.Should().Be(participantPersonalDetails.LastName);
        participantResponse.MiddleNames.Should().Be(participantPersonalDetails.MiddleNames);
        
        var serviceBusStub = Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
        var messages = serviceBusStub!.ReadAllMessagesFromQueue(hearingId);
        Array.Exists(messages, x => x.IntegrationEvent is ParticipantUpdatedIntegrationEvent).Should().BeTrue();
        Array.Exists(messages, x => x.IntegrationEvent is HearingDetailsUpdatedIntegrationEvent).Should().BeTrue();
    }
}