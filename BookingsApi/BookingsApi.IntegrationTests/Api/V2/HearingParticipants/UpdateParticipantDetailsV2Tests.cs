using BookingsApi.Contract.V2.Requests;
using BookingsApi.Contract.V2.Responses;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using BookingsApi.Validations.Common;
using BookingsApi.Validations.V2;
using ScreeningType = BookingsApi.Contract.V2.Enums.ScreeningType;

namespace BookingsApi.IntegrationTests.Api.V2.HearingParticipants;

public class UpdateParticipantDetailsV2Tests : ApiTest
{
    [Test]
    public async Task should_return_validation_error_when_request_validation_fails()
    {
        // arrange
        var hearingId = Guid.NewGuid();
        var participantId = Guid.NewGuid();
        var request = new UpdateParticipantRequestV2()
        {
            ParticipantId = Guid.Empty,
            DisplayName = null,
            OrganisationName = null,
            Representee = null,
            TelephoneNumber = null,
            Title = null,
            FirstName = null,
            MiddleNames = null,
            LastName = null,
            ContactEmail = "gsdgdsgfs",
            LinkedParticipants = new List<LinkedParticipantRequestV2>()
        };

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PatchAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateParticipantDetails(hearingId, participantId),RequestBody.Set(request));


        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        var errorMessages = validationProblemDetails.Errors.SelectMany(e => e.Value).ToList();
        errorMessages.Should().Contain(x => x.Contains(UpdateParticipantRequestValidationV2.NoDisplayNameErrorMessage));
        errorMessages.Should().Contain(x => x.Contains(UpdateParticipantRequestValidationV2.NoParticipantIdErrorMessage));
        errorMessages.Should().Contain(x => x.Contains(ParticipantValidationV2.NoFirstNameErrorMessage));
        errorMessages.Should().Contain(x => x.Contains(ParticipantValidationV2.NoLastNameErrorMessage));
        errorMessages.Should().Contain(x => x.Contains(ParticipantRequestValidationV2.InvalidContactEmailErrorMessage));
    }
    
    [Test]
    public async Task should_return_not_found_when_hearing_does_not_exist()
    {
        // arrange
        var hearingId = Guid.NewGuid();
        var participantId = Guid.NewGuid();
        var request = new UpdateParticipantRequestV2()
        {
            ParticipantId = participantId,
            DisplayName = "Foo",
            OrganisationName = "Bar",
            Representee = null,
            TelephoneNumber = "01234567890",
            Title = "Title",
            FirstName = "New First Name",
            LastName = "New Last Name",
            LinkedParticipants = new List<LinkedParticipantRequestV2>()
        };

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PatchAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateParticipantDetails(hearingId, participantId),RequestBody.Set(request));


        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        ApiClientResponse.GetResponses<string>(result.Content).Result.Should().Be($"Video hearing {hearingId} not found");
    }
    
    [Test]
    public async Task should_return_not_found_when_participant_does_not_exist()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2();
        var hearingId = hearing.Id;
        var participantId = Guid.NewGuid();
        var request = new UpdateParticipantRequestV2()
        {
            ParticipantId = participantId,
            DisplayName = "Foo",
            OrganisationName = "Bar",
            Representee = null,
            TelephoneNumber = "01234567890",
            Title = "Title",
            FirstName = "New First Name",
            LastName = "New Last Name",
            LinkedParticipants = new List<LinkedParticipantRequestV2>()
        };

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PatchAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateParticipantDetails(hearingId, participantId),RequestBody.Set(request));


        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        ApiClientResponse.GetResponses<string>(result.Content).Result.Should().Be($"Participant {participantId} not found for hearing {hearingId}");
    }

    [Test]
    public async Task
        should_return_validation_error_when_updating_a_representative_without_representee()
    {
        var hearing = await Hooks.SeedVideoHearingV2();
        var hearingId = hearing.Id;
        var participant = hearing.GetParticipants().First(x=> x is Representative);
        var participantId = participant.Id;
        var request = new UpdateParticipantRequestV2()
        {
            ParticipantId = participantId,
            DisplayName = "Foo",
            OrganisationName = null,
            Representee = null,
            TelephoneNumber = "01234567890",
            Title = participant.Person.Title,
            FirstName = "New First Name",
            LastName = "New Last Name",
            LinkedParticipants = new List<LinkedParticipantRequestV2>()
        };

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PatchAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateParticipantDetails(hearingId, participantId),
                RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        var errorMessages = validationProblemDetails.Errors.SelectMany(e => e.Value).ToList();
        errorMessages.Should().Contain(x => x.Contains(RepresentativeValidation.NoRepresentee));
    }
    
    [TestCase("wil.li_am." , false)]
    [TestCase("Cr.aig_1234", true)]
    [TestCase("I.", false)]
    [TestCase(".william1234", false)]
    [TestCase("_a", true)]
    [TestCase("Willi..amCraig1234", false)]
    [TestCase(" qweqwe ", false)]
    [TestCase("w.w", true)]
    [TestCase("XY", true)]
    [TestCase("bill e boy", true)]
    [TestCase("Test-Judge 1", true)]
    [TestCase("Test-Judge  1", false)]
    public async Task should_return_validation_error_when_first_last_names_are_not_valid(string testName, bool expectedResult)
    {
        var hearing = await Hooks.SeedVideoHearing(status:BookingStatus.Booked);
        var hearingId = hearing.Id;
        var participant = hearing.GetParticipants().First(x=> x is Individual);
        var participantId = participant.Id;
        var request = new UpdateParticipantRequestV2()
        {
            ParticipantId = participantId,
            DisplayName = "New Display Name",
            OrganisationName = null,
            Representee = null,
            TelephoneNumber = "01526791027",
            Title = participant.Person.Title,
            FirstName = testName,
            LastName = testName,
            LinkedParticipants = new List<LinkedParticipantRequestV2>()
        };
        
        // act
        using var client = Application.CreateClient();
        var result = await client
            .PatchAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateParticipantDetails(hearingId, participantId),
                RequestBody.Set(request));
        
        // assert
        result.IsSuccessStatusCode.Should().Be(expectedResult);
        if (!expectedResult)
        {
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            var errorMessages = validationProblemDetails.Errors.SelectMany(e => e.Value).ToList();
            errorMessages.Should().Contain(x => x.Contains(ParticipantValidationV2.FirstNameDoesntMatchRegex));
            errorMessages.Should().Contain(x => x.Contains(ParticipantValidationV2.LastNameDoesntMatchRegex));
        }
        else
        {
            var serviceBusStub = Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
            serviceBusStub!.ReadMessageFromQueue();
        }
    }

    [Test]
    public async Task should_not_update_middle_names_when_middle_names_not_provided()
    {
        var hearing = await Hooks.SeedVideoHearing(status:BookingStatus.Created);
        var hearingId = hearing.Id;
        var participant = hearing.GetParticipants().First(x=> x is Individual);
        var participantId = participant.Id;
        var request = new UpdateParticipantRequestV2()
        {
            ParticipantId = participantId,
            DisplayName = "New Display Name",
            OrganisationName = null,
            Representee = null,
            TelephoneNumber = "01526791027",
            Title = participant.Person.Title,
            FirstName = "New First Name",
            LastName = "New Last Name",
            LinkedParticipants = new List<LinkedParticipantRequestV2>()
        };
    
        // act
        using var client = Application.CreateClient();
        var result = await client
            .PatchAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateParticipantDetails(hearingId, participantId),
                RequestBody.Set(request));
    
        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var participantResponse = await ApiClientResponse.GetResponses<ParticipantResponseV2>(result.Content);
        participantResponse.MiddleNames.Should().Be(participant.Person.MiddleNames);
        
        var serviceBusStub = Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
        var message = serviceBusStub!.ReadMessageFromQueue();
        message.IntegrationEvent.Should().BeOfType<ParticipantUpdatedIntegrationEvent>();
    }

    [Test]
    public async Task should_update_a_participant_and_publish_event_when_hearing_is_confirmed()
    {
        var hearing = await Hooks.SeedVideoHearingV2(status: BookingStatus.Created);
        var hearingId = hearing.Id;
        var participant = hearing.GetParticipants().First(x=> x is Individual);
        var participantId = participant.Id;
        var request = new UpdateParticipantRequestV2()
        {
            ParticipantId = participantId,
            DisplayName = "New Display Name",
            ContactEmail = "contactEmailUpdated@email.com",
            OrganisationName = null,
            Representee = null,
            TelephoneNumber = "01526791027",
            Title = participant.Person.Title,
            FirstName = "New First Name",
            MiddleNames = "New Middle Names",
            LastName = "New Last Name",
            LinkedParticipants = new List<LinkedParticipantRequestV2>()
        };

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PatchAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateParticipantDetails(hearingId, participantId),
                RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var participantResponse = await ApiClientResponse.GetResponses<ParticipantResponseV2>(result.Content);
        participantResponse.Id.Should().Be(participantId);
        participantResponse.DisplayName.Should().Be(request.DisplayName);
        participantResponse.ContactEmail.Should().Be(request.ContactEmail);
        participantResponse.Username.Should().Be(participant.Person.Username);
        participantResponse.TelephoneNumber.Should().Be(request.TelephoneNumber);
        participantResponse.Title.Should().Be(request.Title);
        participantResponse.Representee.Should().BeNull();
        participantResponse.Organisation.Should().Be(participant.Person.Organisation?.Name);
        participantResponse.FirstName.Should().Be(request.FirstName);
        participantResponse.MiddleNames.Should().Be(request.MiddleNames);
        participantResponse.LastName.Should().Be(request.LastName);
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
        integrationEvent!.Participant.FirstName.Should().Be(request.FirstName);
        integrationEvent!.Participant.LastName.Should().Be(request.LastName);
    }

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    public async Task should_update_a_participant_contact_email_and_publish_event_when_hearing_is_confirmed_and_contact_email_already_exists_for_different_person(int testCase)
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(status: BookingStatus.Created);
        var hearingId = hearing.Id;
        var participant = hearing.GetParticipants().First(x=> x is Individual);
        var participantId = participant.Id;
        
        var hearing2 = await Hooks.SeedVideoHearingV2(status: BookingStatus.Created);
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
        
        var request = new UpdateParticipantRequestV2
        {
            ParticipantId = participantId,
            DisplayName = "New Display Name",
            ContactEmail = contactEmail,
            OrganisationName = null,
            Representee = null,
            TelephoneNumber = "01526791027",
            Title = participant.Person.Title,
            FirstName = "New First Name",
            MiddleNames = "New Middle Names",
            LastName = "New Last Name",
            LinkedParticipants = new List<LinkedParticipantRequestV2>()
        };
        
        // act
        using var client = Application.CreateClient();
        var result = await client
            .PatchAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateParticipantDetails(hearingId, participantId),
                RequestBody.Set(request));
        
        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var participantResponse = await ApiClientResponse.GetResponses<ParticipantResponseV2>(result.Content);
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
        var hearing = await Hooks.SeedVideoHearingV2(status: BookingStatus.Created);
        var hearingId = hearing.Id;
        var participant = hearing.GetParticipants().First(x=> x is Individual);
        var participantId = participant.Id;
        var request = new UpdateParticipantRequestV2()
        {
            ParticipantId = participantId,
            DisplayName = "New Display Name",
            OrganisationName = null,
            Representee = null,
            TelephoneNumber = "01526791027",
            Title = participant.Person.Title,
            FirstName = "New First Name",
            MiddleNames = "New Middle Names",
            LastName = "New Last Name",
            LinkedParticipants = new List<LinkedParticipantRequestV2>()
        };

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PatchAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateParticipantDetails(hearingId, participantId),
                RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var participantResponse = await ApiClientResponse.GetResponses<ParticipantResponseV2>(result.Content);
        participantResponse.Id.Should().Be(participantId);
        participantResponse.DisplayName.Should().Be(request.DisplayName);
        participantResponse.ContactEmail.Should().Be(participant.Person.ContactEmail);
        participantResponse.Username.Should().Be(participant.Person.Username);
        participantResponse.TelephoneNumber.Should().Be(request.TelephoneNumber);
        participantResponse.Title.Should().Be(request.Title);
        participantResponse.Representee.Should().BeNull();
        participantResponse.Organisation.Should().Be(participant.Person.Organisation?.Name);
        participantResponse.FirstName.Should().Be(request.FirstName);
        participantResponse.MiddleNames.Should().Be(request.MiddleNames);
        participantResponse.LastName.Should().Be(request.LastName);
        participantResponse.LinkedParticipants.Should().BeEmpty();
        
        var serviceBusStub = Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
        var message = serviceBusStub!.ReadAllMessagesFromQueue(hearingId)[0];
        message.IntegrationEvent.Should().BeOfType<ParticipantUpdatedIntegrationEvent>();
        var integrationEvent = message.IntegrationEvent as ParticipantUpdatedIntegrationEvent;
        integrationEvent!.Participant.ParticipantId.Should().Be(participantId);
        integrationEvent!.Participant.DisplayName.Should().Be(request.DisplayName);
        integrationEvent!.Participant.ContactEmail.Should().Be(participant.Person.ContactEmail);
        integrationEvent!.Participant.ContactTelephone.Should().Be(request.TelephoneNumber);
        integrationEvent!.Participant.Representee.Should().BeEmpty();
        integrationEvent!.Participant.FirstName.Should().Be(request.FirstName);
        integrationEvent!.Participant.LastName.Should().Be(request.LastName);
    }
    
    [Test]
    public async Task should_update_a_participant_and_publish_event_when_hearing_is_not_confirmed()
    {
        var hearing = await Hooks.SeedVideoHearingV2(status:BookingStatus.Booked);
        var hearingId = hearing.Id;
        var participant = hearing.GetParticipants().First(x=> x is Individual);
        var participantId = participant.Id;
        var request = new UpdateParticipantRequestV2()
        {
            ParticipantId = participantId,
            DisplayName = "New Display Name",
            OrganisationName = null,
            Representee = null,
            TelephoneNumber = "01526791027",
            Title = participant.Person.Title,
            FirstName = "New First Name",
            MiddleNames = "New Middle Names",
            LastName = "New Last Name",
            LinkedParticipants = new List<LinkedParticipantRequestV2>()
        };

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PatchAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateParticipantDetails(hearingId, participantId),
                RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var participantResponse = await ApiClientResponse.GetResponses<ParticipantResponseV2>(result.Content);
        participantResponse.Id.Should().Be(participantId);
        participantResponse.DisplayName.Should().Be(request.DisplayName);
        participantResponse.ContactEmail.Should().Be(participant.Person.ContactEmail);
        participantResponse.Username.Should().Be(participant.Person.Username);
        participantResponse.TelephoneNumber.Should().Be(request.TelephoneNumber);
        participantResponse.Title.Should().Be(request.Title);
        participantResponse.Representee.Should().BeNull();
        participantResponse.Organisation.Should().Be(participant.Person.Organisation?.Name);
        participantResponse.FirstName.Should().Be(request.FirstName);
        participantResponse.MiddleNames.Should().Be(request.MiddleNames);
        participantResponse.LastName.Should().Be(request.LastName);
        participantResponse.LinkedParticipants.Should().BeEmpty();
        
        var serviceBusStub = Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
        var message = serviceBusStub!.ReadAllMessagesFromQueue(hearingId)[0];
        message.IntegrationEvent.Should().BeOfType<ParticipantUpdatedIntegrationEvent>();
        var integrationEvent = message.IntegrationEvent as ParticipantUpdatedIntegrationEvent;
        integrationEvent!.Participant.ParticipantId.Should().Be(participantId);
        integrationEvent!.Participant.DisplayName.Should().Be(request.DisplayName);
        integrationEvent!.Participant.ContactEmail.Should().Be(participant.Person.ContactEmail);
        integrationEvent!.Participant.ContactTelephone.Should().Be(request.TelephoneNumber);
        integrationEvent!.Participant.Representee.Should().BeEmpty();
        integrationEvent!.Participant.FirstName.Should().Be(request.FirstName);
        integrationEvent!.Participant.LastName.Should().Be(request.LastName);
    }

    [Test]
    public async Task should_update_participant_to_set_interpreter_code()
    {
        var hearing = await Hooks.SeedVideoHearingV2(status: BookingStatus.Created);
        var hearingId = hearing.Id;
        var participant = hearing.GetParticipants().First(x=> x is Individual);
        var participantId = participant.Id;
        var request = new UpdateParticipantRequestV2()
        {
            ParticipantId = participantId,
            DisplayName = "New Display Name",
            ContactEmail = "contactEmailUpdated@email.com",
            OrganisationName = null,
            Representee = null,
            TelephoneNumber = "01526791027",
            Title = participant.Person.Title,
            FirstName = "New First Name",
            MiddleNames = "New Middle Names",
            LastName = "New Last Name",
            LinkedParticipants = new List<LinkedParticipantRequestV2>(),
            InterpreterLanguageCode = "fra"
        };
        
        // act
        using var client = Application.CreateClient();
        var result = await client
            .PatchAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateParticipantDetails(hearingId, participantId),
                RequestBody.Set(request));
        
        
        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var participantResponse = await ApiClientResponse.GetResponses<ParticipantResponseV2>(result.Content);
        participantResponse.InterpreterLanguage.Should().NotBeNull();
        participantResponse.InterpreterLanguage.Code.Should().Be(request.InterpreterLanguageCode);
    }

    [Test]
    public async Task should_update_participant_with_screening_added()
    {
        var hearing = await Hooks.SeedVideoHearingV2(status: BookingStatus.Created);
        var hearingId = hearing.Id;
        var individuals = hearing.GetParticipants().Where(x => x is Individual).ToList();
        var participant = individuals[0];
        var secondParticipant = individuals[1];
        var participantId = participant.Id;
        var endpointForScreening = hearing.Endpoints[0];
        
        var request = new UpdateParticipantRequestV2()
        {
            ParticipantId = participantId,
            DisplayName = "New Display Name",
            ContactEmail = "contactEmailUpdated@email.com",
            OrganisationName = null,
            Representee = null,
            TelephoneNumber = "01526791027",
            Title = participant.Person.Title,
            FirstName = "New First Name",
            MiddleNames = "New Middle Names",
            LastName = "New Last Name",
            LinkedParticipants = new List<LinkedParticipantRequestV2>(),
            Screening = new ScreeningRequest
            {
                Type = ScreeningType.Specific,
                ProtectFromParticipants = [secondParticipant.Person.ContactEmail],
                ProtectFromEndpoints = [endpointForScreening.DisplayName]
            }
        };
        
        // act
        using var client = Application.CreateClient();
        var result = await client
            .PatchAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateParticipantDetails(hearingId, participantId),
                RequestBody.Set(request));
        
        
        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var participantResponse = await ApiClientResponse.GetResponses<ParticipantResponseV2>(result.Content);
        participantResponse.Screening.Should().NotBeNull();
        participantResponse.Screening.Type.Should().Be(ScreeningType.Specific);
        participantResponse.Screening.ProtectFromParticipantsIds.Should().Contain(secondParticipant.Id);
        participantResponse.Screening.ProtectFromEndpointsIds.Should().Contain(endpointForScreening.Id);
    }
}