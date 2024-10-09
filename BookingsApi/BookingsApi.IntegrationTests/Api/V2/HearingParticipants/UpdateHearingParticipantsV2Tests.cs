using BookingsApi.Contract.V2.Enums;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Contract.V2.Responses;
using BookingsApi.Domain.Constants;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.Validations;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using BookingsApi.Validations.V2;
using ScreeningType = BookingsApi.Contract.V2.Enums.ScreeningType;

namespace BookingsApi.IntegrationTests.Api.V2.HearingParticipants;

public class UpdateHearingParticipantsV2Tests : ApiTest
{
        
    [Test]
    public async Task should_update_an_existing_participant()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearing(options
            => { options.Case = new Case("UpdateParticipantJudge", "UpdateParticipantJudge"); }, Domain.Enumerations.BookingStatus.Created);
        var participant = hearing.Participants.First(e => e.HearingRole.Name == "Litigant in person");
        var request = new UpdateHearingParticipantsRequestV2
        {
            ExistingParticipants = new List<UpdateParticipantRequestV2> { new()
            {
                ParticipantId = participant.Id, 
                DisplayName = "NewDisplayName",
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
            .PostAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateHearingParticipants(hearing.Id),RequestBody.Set(request));
        var updatedHearing = await client.GetAsync(ApiUriFactory.HearingsEndpointsV2.GetHearingDetailsById(hearing.Id.ToString()));
        var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(updatedHearing.Content);
        
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
    public async Task should_update_individual_participant_in_a_hearing_and_return_200()
    {
        // arrange
        var request = new UpdateHearingParticipantsRequestV2
        {
            ExistingParticipants = new List<UpdateParticipantRequestV2>()
            {
                new ()
                {
                    ParticipantId = Guid.NewGuid(),
                    DisplayName = "DisplayName",
                    Title = "New Title",
                    FirstName = "New First Name",
                    LastName = "New Last Name"
                },
            },
            NewParticipants = new List<ParticipantRequestV2>()
            {
                new ()
                {
                    DisplayName = "DisplayName",
                    FirstName = "NewFirstName",
                    HearingRoleCode = HearingRoleCodes.Applicant,
                    LastName = "NewLastName",
                    MiddleNames = "NewMiddleNames",
                    OrganisationName = "OrganisationName",
                    ContactEmail = "newcontact@test.email.com",
                    TelephoneNumber = "0123456789",
                    Title = "Title",
                    Representee = "Representee",
                }
            }
        };
        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateHearingParticipants(Guid.NewGuid()),RequestBody.Set(request));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound, result.Content.ReadAsStringAsync().Result);
    }
    
    [Test]
    public async Task should_return_not_found_when_video_hearing_is_not_found()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(options =>
        {
            options.Case = new Case("Case1 Num", "Case1 Name");
        }, BookingStatus.Created);
        
        var participantToUpdate = hearing.Participants.First(e => e.HearingRole.UserRole.IsIndividual);
        var request = new UpdateHearingParticipantsRequestV2
        {
            ExistingParticipants = new List<UpdateParticipantRequestV2>()
            {
                new ()
                {
                    ParticipantId = participantToUpdate.Id,
                    DisplayName = participantToUpdate.DisplayName,
                    Title = participantToUpdate.Person.Title,
                    FirstName = participantToUpdate.Person.FirstName,
                    LastName = participantToUpdate.Person.LastName
                }
            }
        };
        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateHearingParticipants(hearing.Id),RequestBody.Set(request));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
    }
    
    [Test]
    public async Task should_update_individual_participant_and_add_a_new_individual_without_a_case_role_in_a_hearing_and_return_200()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(options =>
        {
            options.Case = new Case("Case1 Num", "Case1 Name");
        }, BookingStatus.Created);
        
        var participantToUpdate = hearing.Participants.First(e => e.HearingRole.UserRole.IsIndividual);
        var request = new UpdateHearingParticipantsRequestV2
        {
            ExistingParticipants = new List<UpdateParticipantRequestV2>()
            {
                new ()
                {
                    ParticipantId = participantToUpdate.Id,
                    DisplayName = participantToUpdate.DisplayName,
                    Title = participantToUpdate.Person.Title,
                    FirstName = participantToUpdate.Person.FirstName,
                    LastName = participantToUpdate.Person.LastName
                }
            },
            NewParticipants = new List<ParticipantRequestV2>
            {
                new ()
                {
                    DisplayName = "DisplayName",
                    FirstName = "FirstName",
                    HearingRoleCode = "APPL",
                    LastName = "LastName",
                    MiddleNames = "MiddleNames",
                    OrganisationName = "OrganisationName",
                    ContactEmail = "contact@test.email.com",
                    TelephoneNumber = "0123456789",
                    Title = "Title",
                    Representee = "Representee",
                }
            }
        };
        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateHearingParticipants(hearing.Id),RequestBody.Set(request));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
    }

    [Test]
    public async Task should_return_validation_errors_when_input_is_incorrect()
    {
        // arrange
        var request = new UpdateHearingParticipantsRequestV2
        {
            ExistingParticipants = new List<UpdateParticipantRequestV2>()
            {
                new ()
                { }
            },
            NewParticipants = new List<ParticipantRequestV2>()
            {
                new ()
                { }
            }
        };

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateHearingParticipants(Guid.NewGuid()),RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors["NewParticipants[0].DisplayName"].Should().Contain(ParticipantRequestValidationV2.NoDisplayNameErrorMessage);
        validationProblemDetails.Errors["ExistingParticipants[0].ParticipantId"].Should().Contain(UpdateParticipantRequestValidationV2.NoParticipantIdErrorMessage);
        validationProblemDetails.Errors["ExistingParticipants[0].DisplayName"].Should().Contain(UpdateParticipantRequestValidationV2.NoDisplayNameErrorMessage);
        
    }
    
    [Test]
    public async Task should_return_validation_errors_when_hearing_role_not_found_from_case_role()
    {
        // arrange
        var hearingRoleCode = "Invalid Role";
        var hearing = await Hooks.SeedVideoHearingV2(options =>
        {
            options.Case = new Case("UpdateParticipantDataValidationFailure", "UpdateParticipantDataValidationFailure"); 
        }, BookingStatus.Created);
        
        var request = new UpdateHearingParticipantsRequestV2
        {
            NewParticipants = new List<ParticipantRequestV2>
            {
                new ()
                {
                    DisplayName = "DisplayName",
                    FirstName = "NewFirstName",
                    HearingRoleCode = hearingRoleCode,
                    LastName = "NewLastName",
                    MiddleNames = "NewMiddleNames",
                    OrganisationName = "OrganisationName",
                    ContactEmail = "newcontact@test.email.com",
                    TelephoneNumber = "0123456789",
                    Title = "Title",
                    Representee = "Representee"
                }
            }
        };

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateHearingParticipants(hearing.Id),RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[$"{nameof(request.NewParticipants )}[0]"].Should().Contain($"Invalid hearing role [{hearingRoleCode}]");
    }

    [Test]
    public async Task should_return_validation_errors_when_flat_structure_hearing_role_not_found()
    {
        // arrange
        var hearingRoleCode = "Invalid Code";
        var hearing = await Hooks.SeedVideoHearingV2(options =>
        {
            options.Case = new Case("UpdateParticipantDataValidationFailure", "UpdateParticipantDataValidationFailure");
        }, BookingStatus.Created);
        
        var request = new UpdateHearingParticipantsRequestV2
        {
            NewParticipants = new List<ParticipantRequestV2>
            {
                new ()
                {
                    DisplayName = "DisplayName",
                    FirstName = "FirstName",
                    HearingRoleCode = hearingRoleCode,
                    LastName = "LastName",
                    MiddleNames = "MiddleNames",
                    OrganisationName = "OrganisationName",
                    ContactEmail = "contact@test.email.com",
                    TelephoneNumber = "0123456789",
                    Title = "Title",
                    Representee = "Representee",
                }
            }
        };

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateHearingParticipants(hearing.Id),RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[$"{nameof(request.NewParticipants )}[0]"].Should().Contain($"Invalid hearing role [{hearingRoleCode}]");
    }
    
    [Test]
    public async Task should_remove_a_participant_from_the_confirmed_hearing()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearing(options
            => { options.Case = new Case("UpdateParticipantsRemoveParticipant", "UpdateParticipantsRemoveParticipant"); }, BookingStatus.Created);
        var participantBeingRemoved = hearing.Participants.First(x=> !x.HearingRole.IsJudge());
        var request = new UpdateHearingParticipantsRequestV2 { RemovedParticipantIds = new List<Guid>{ participantBeingRemoved.Id } };

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateHearingParticipants(hearing.Id),RequestBody.Set(request));
        var updatedHearing = await client
            .GetAsync(ApiUriFactory.HearingsEndpointsV2.GetHearingDetailsById(hearing.Id.ToString()));
        var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(updatedHearing.Content);
        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        hearingResponse.Participants.Should().NotContain(p => p.Id == participantBeingRemoved.Id);
        hearingResponse.Status.Should().Be(BookingStatusV2.Created);
    }
    
    [Test]
    public async Task should_add_an_interpreter_participant_to_hearing_when_hearing_is_close_to_start_time_and_return_200()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(options =>
            {
                options.Case = new Case("Case1 Num", "Case1 Name");
                options.ScheduledDate = DateTime.UtcNow.AddMinutes(25);
                options.AudioRecordingRequired = false;
            },
            BookingStatus.Created);

        var request = new UpdateHearingParticipantsRequestV2()
        {
            NewParticipants = new List<ParticipantRequestV2>
            {
                new()
                {
                    DisplayName = "Add Interpreter Participant DisplayName",
                    Title = "Mr",
                    FirstName = "FirstName",
                    HearingRoleCode = HearingRoleCodes.Interpreter,
                    LastName = "LastName",
                    MiddleNames = "MiddleNames",
                    OrganisationName = "OrganisationName",
                    ContactEmail = "int@testadded.com",
                    TelephoneNumber = "01234567890"
                }
            },
            LinkedParticipants = new()
            {
                new LinkedParticipantRequestV2
                {
                    Type = LinkedParticipantTypeV2.Interpreter,
                    ParticipantContactEmail = "int@testadded.com",
                    LinkedParticipantContactEmail =
                        hearing.GetParticipants().First(x => x is Individual).Person.ContactEmail
                }
            }
        };

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateHearingParticipants(hearing.Id),RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue(result.Content.ReadAsStringAsync().Result);
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Test]
    public async Task should_remove_a_judge_from_the_confirmed_hearing()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearing(options
            => { options.Case = new Case("UpdateParticipantsRemoveParticipant", "UpdateParticipantsRemoveParticipant"); }, BookingStatus.Created);
        var judge = hearing.Participants.First(e => e.HearingRole.IsJudge());
        var request = new UpdateHearingParticipantsRequestV2 { RemovedParticipantIds = new List<Guid>{ judge.Id } };

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateHearingParticipants(hearing.Id),RequestBody.Set(request));
        var updatedHearing = await client.GetAsync(ApiUriFactory.HearingsEndpointsV2.GetHearingDetailsById(hearing.Id.ToString()));
        var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(updatedHearing.Content);
        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        hearingResponse.Participants.Should().NotContain(p => p.Id == judge.Id);
        hearingResponse.Status.Should().Be(BookingStatusV2.ConfirmedWithoutJudge);
    }
    
    [Test]
    public async Task should_update_hearing_participants_with_interpreter_languages()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearing(options
            => { options.Case = new Case("UpdateParticipantJudge", "UpdateParticipantJudge"); }, Domain.Enumerations.BookingStatus.Created);
        var existingParticipant = hearing.Participants.First(e => e.HearingRole.Name == "Litigant in person");
        var newParticipant = new { ContactEmail = "newcontact@test.email.com" };
        const string languageCode = "spa";
        var request = new UpdateHearingParticipantsRequestV2
        {
            ExistingParticipants =
            [
                new UpdateParticipantRequestV2
                {
                    ParticipantId = existingParticipant.Id, 
                    DisplayName = "NewDisplayName",
                    FirstName = existingParticipant.Person?.FirstName,
                    LastName = existingParticipant.Person?.LastName,
                    OrganisationName = existingParticipant.Person?.Organisation?.Name,
                    TelephoneNumber = existingParticipant.Person?.TelephoneNumber,
                    Title = existingParticipant.Person?.Title,
                    InterpreterLanguageCode = languageCode
                }
            ],
            NewParticipants =
            [
                new ParticipantRequestV2
                {
                    DisplayName = "DisplayName",
                    FirstName = "NewFirstName",
                    HearingRoleCode = HearingRoleCodes.Applicant,
                    LastName = "NewLastName",
                    MiddleNames = "NewMiddleNames",
                    OrganisationName = "OrganisationName",
                    ContactEmail = newParticipant.ContactEmail,
                    TelephoneNumber = "0123456789",
                    Title = "Title",
                    Representee = "Representee",
                    InterpreterLanguageCode = languageCode
                }
            ]
        };
        
        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateHearingParticipants(hearing.Id),RequestBody.Set(request));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var updatedHearing = await client.GetAsync(ApiUriFactory.HearingsEndpointsV2.GetHearingDetailsById(hearing.Id.ToString()));
        var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(updatedHearing.Content);
        var updatedParticipant = hearingResponse.Participants.Find(x => x.Id == existingParticipant.Id);
        updatedParticipant.Should().NotBeNull();
        updatedParticipant.InterpreterLanguage.Code.Should().Be(languageCode);
        var addedParticipant = hearingResponse.Participants.Find(x => x.ContactEmail == newParticipant.ContactEmail);
        addedParticipant.Should().NotBeNull();
        addedParticipant.InterpreterLanguage.Code.Should().Be(languageCode);
    }

    [Test]
    public async Task should_update_hearing_participants_with_other_languages()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearing(options
            => { options.Case = new Case("UpdateParticipantJudge", "UpdateParticipantJudge"); }, Domain.Enumerations.BookingStatus.Created);
        var existingParticipant = hearing.Participants.First(e => e.HearingRole.Name == "Litigant in person");
        var newParticipant = new { ContactEmail = "newcontact@test.email.com" };
        const string otherLanguage = "made up";
        var request = new UpdateHearingParticipantsRequestV2
        {
            ExistingParticipants =
            [
                new UpdateParticipantRequestV2
                {
                    ParticipantId = existingParticipant.Id, 
                    DisplayName = "NewDisplayName",
                    FirstName = existingParticipant.Person?.FirstName,
                    LastName = existingParticipant.Person?.LastName,
                    OrganisationName = existingParticipant.Person?.Organisation?.Name,
                    TelephoneNumber = existingParticipant.Person?.TelephoneNumber,
                    Title = existingParticipant.Person?.Title,
                    OtherLanguage = otherLanguage
                }
            ],
            NewParticipants =
            [
                new ParticipantRequestV2
                {
                    DisplayName = "DisplayName",
                    FirstName = "NewFirstName",
                    HearingRoleCode = HearingRoleCodes.Applicant,
                    LastName = "NewLastName",
                    MiddleNames = "NewMiddleNames",
                    OrganisationName = "OrganisationName",
                    ContactEmail = newParticipant.ContactEmail,
                    TelephoneNumber = "0123456789",
                    Title = "Title",
                    Representee = "Representee",
                    OtherLanguage = otherLanguage
                }
            ]
        };
        
        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateHearingParticipants(hearing.Id),RequestBody.Set(request));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var updatedHearing = await client.GetAsync(ApiUriFactory.HearingsEndpointsV2.GetHearingDetailsById(hearing.Id.ToString()));
        var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(updatedHearing.Content);
        var updatedParticipant = hearingResponse.Participants.Find(x => x.Id == existingParticipant.Id);
        updatedParticipant.Should().NotBeNull();
        updatedParticipant.OtherLanguage.Should().Be(otherLanguage);
        var addedParticipant = hearingResponse.Participants.Find(x => x.ContactEmail == newParticipant.ContactEmail);
        addedParticipant.Should().NotBeNull();
        addedParticipant.OtherLanguage.Should().Be(otherLanguage);
    }
    
    [Test]
    public async Task should_return_validation_error_when_interpreter_language_code_is_not_found()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearing(options
            => { options.Case = new Case("UpdateParticipantJudge", "UpdateParticipantJudge"); }, Domain.Enumerations.BookingStatus.Created);
        var existingParticipant = hearing.Participants.First(e => e.HearingRole.Name == "Litigant in person");
        var newParticipant = new { ContactEmail = "newcontact@test.email.com" };
        const string languageCode = "non existing";
        var request = new UpdateHearingParticipantsRequestV2
        {
            ExistingParticipants =
            [
                new UpdateParticipantRequestV2
                {
                    ParticipantId = existingParticipant.Id, 
                    DisplayName = "NewDisplayName",
                    FirstName = existingParticipant.Person?.FirstName,
                    LastName = existingParticipant.Person?.LastName,
                    OrganisationName = existingParticipant.Person?.Organisation?.Name,
                    TelephoneNumber = existingParticipant.Person?.TelephoneNumber,
                    Title = existingParticipant.Person?.Title,
                    InterpreterLanguageCode = languageCode
                }
            ],
            NewParticipants =
            [
                new ParticipantRequestV2
                {
                    DisplayName = "DisplayName",
                    FirstName = "NewFirstName",
                    HearingRoleCode = HearingRoleCodes.Applicant,
                    LastName = "NewLastName",
                    MiddleNames = "NewMiddleNames",
                    OrganisationName = "OrganisationName",
                    ContactEmail = newParticipant.ContactEmail,
                    TelephoneNumber = "0123456789",
                    Title = "Title",
                    Representee = "Representee",
                    InterpreterLanguageCode = languageCode
                }
            ]
        };
        
        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateHearingParticipants(hearing.Id),RequestBody.Set(request));
        
        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors["Participant"][0].Should().Contain($"Language code {languageCode} does not exist");
    }

    [Test]
    public async Task should_return_validation_error_when_both_interpreter_language_code_and_other_language_are_specified()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearing(options
            => { options.Case = new Case("UpdateParticipantJudge", "UpdateParticipantJudge"); }, Domain.Enumerations.BookingStatus.Created);
        var existingParticipant = hearing.Participants.First(e => e.HearingRole.Name == "Litigant in person");
        var newParticipant = new { ContactEmail = "newcontact@test.email.com" };
        const string languageCode = "spa";
        const string otherLanguage = "made up";
        var request = new UpdateHearingParticipantsRequestV2
        {
            ExistingParticipants =
            [
                new UpdateParticipantRequestV2
                {
                    ParticipantId = existingParticipant.Id, 
                    DisplayName = "NewDisplayName",
                    FirstName = existingParticipant.Person?.FirstName,
                    LastName = existingParticipant.Person?.LastName,
                    OrganisationName = existingParticipant.Person?.Organisation?.Name,
                    TelephoneNumber = existingParticipant.Person?.TelephoneNumber,
                    Title = existingParticipant.Person?.Title,
                    InterpreterLanguageCode = languageCode,
                    OtherLanguage = otherLanguage
                }
            ],
            NewParticipants =
            [
                new ParticipantRequestV2
                {
                    DisplayName = "DisplayName",
                    FirstName = "NewFirstName",
                    HearingRoleCode = HearingRoleCodes.Applicant,
                    LastName = "NewLastName",
                    MiddleNames = "NewMiddleNames",
                    OrganisationName = "OrganisationName",
                    ContactEmail = newParticipant.ContactEmail,
                    TelephoneNumber = "0123456789",
                    Title = "Title",
                    Representee = "Representee",
                    InterpreterLanguageCode = languageCode,
                    OtherLanguage = otherLanguage
                }
            ]
        };
        
        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateHearingParticipants(hearing.Id),RequestBody.Set(request));
        
        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors["Participant"][0].Should()
            .Be(DomainRuleErrorMessages.LanguageAndOtherLanguageCannotBeSet);
    }

    [Test]
    public async Task should_update_participants_with_screening()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(options
            => { options.Case = new Case("UpdateParticipantJudge", "UpdateParticipantJudge"); }, BookingStatus.Created);
        var existingIndividual = hearing.Participants.First(e => e is Individual);
        var newParticipant = new { ContactEmail = "newcontact@test.email.com" };

        var endpointToScreenFrom = hearing.GetEndpoints()[0];
        
        var request = new UpdateHearingParticipantsRequestV2
        {
            ExistingParticipants =
            [
                new UpdateParticipantRequestV2
                {
                    ParticipantId = existingIndividual.Id, 
                    DisplayName = "NewDisplayName",
                    FirstName = existingIndividual.Person?.FirstName,
                    LastName = existingIndividual.Person?.LastName,
                    OrganisationName = existingIndividual.Person?.Organisation?.Name,
                    TelephoneNumber = existingIndividual.Person?.TelephoneNumber,
                    Title = existingIndividual.Person?.Title,
                    Screening = new ScreeningRequest
                    {
                        Type = ScreeningType.Specific,
                        ProtectFromEndpoints = [endpointToScreenFrom.DisplayName]
                    }
                    
                }
            ],
            NewParticipants =
            [
                new ParticipantRequestV2
                {
                    DisplayName = "DisplayName",
                    FirstName = "NewFirstName",
                    HearingRoleCode = HearingRoleCodes.Applicant,
                    LastName = "NewLastName",
                    MiddleNames = "NewMiddleNames",
                    OrganisationName = "OrganisationName",
                    ContactEmail = newParticipant.ContactEmail,
                    TelephoneNumber = "0123456789",
                    Title = "Title",
                    Representee = "Representee",
                    Screening = new ScreeningRequest
                    {
                        Type = ScreeningType.All
                    }
                }
            ]
        };
        
        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateHearingParticipants(hearing.Id),RequestBody.Set(request));
        
        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var updatedHearing = await client.GetAsync(ApiUriFactory.HearingsEndpointsV2.GetHearingDetailsById(hearing.Id.ToString()));
        var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(updatedHearing.Content);
        
        var updatedParticipant = hearingResponse.Participants.Find(x => x.Id == existingIndividual.Id);
        updatedParticipant.Should().NotBeNull();
        updatedParticipant.Screening.Should().NotBeNull("Screening should have been assigned");
        updatedParticipant.Screening.Type.Should().Be(ScreeningType.Specific);
        updatedParticipant.Screening.ProtectFromEndpointsIds.Should().Contain(endpointToScreenFrom.Id);
        
        var addedParticipant = hearingResponse.Participants.Find(x => x.ContactEmail == newParticipant.ContactEmail);
        addedParticipant.Should().NotBeNull();
        addedParticipant.Screening.Should().NotBeNull("Screening should have been assigned");
        addedParticipant.Screening.Type.Should().Be(ScreeningType.All);

    }

    [Test(Description = "Participant A is screened from Participant B. Participant B is then removed from the hearing")]
    public async Task should_update_screening_when_a_participant_is_removed()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(options
            =>
        {
            options.AddScreening = true;
            options.Case = new Case("UpdateParticipantJudge", "UpdateParticipantJudge");
        }, BookingStatus.Created);
        var individuals = hearing.Participants.Where(x => x is Individual).ToList();
        var participantA = individuals.Find(i => i.Screening != null);
        var participantB = individuals.Find(i => i.Screening == null);
        
        var request = new UpdateHearingParticipantsRequestV2
        {
            ExistingParticipants =
            [
                new UpdateParticipantRequestV2
                {
                    ParticipantId = participantA.Id, 
                    DisplayName = "NewDisplayName",
                    FirstName = participantA.Person?.FirstName,
                    LastName = participantA.Person?.LastName,
                    OrganisationName = participantA.Person?.Organisation?.Name,
                    TelephoneNumber = participantA.Person?.TelephoneNumber,
                    Title = participantA.Person?.Title,
                    Screening = new ScreeningRequest()
                    {
                        Type = ScreeningType.Specific,
                        ProtectFromEndpoints = participantA.Screening.GetEndpoints().Select(x=> x.Endpoint.DisplayName).ToList()
                    }
                }
            ],
            RemovedParticipantIds = [participantB.Id]
        };
        
        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateHearingParticipants(hearing.Id),RequestBody.Set(request));
        
        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var updatedHearing = await client.GetAsync(ApiUriFactory.HearingsEndpointsV2.GetHearingDetailsById(hearing.Id.ToString()));
        
        var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(updatedHearing.Content);
        var updatedParticipant = hearingResponse.Participants.Find(x => x.Id == participantA.Id);
        updatedParticipant.Should().NotBeNull();
        updatedParticipant.Screening.Should().NotBeNull();
        updatedParticipant.Screening.Type.Should().Be(ScreeningType.Specific);
        updatedParticipant.Screening.ProtectFromParticipantsIds.Should().BeEmpty();
    }
    
    [Test(Description = "Participant A is screened from Participant B. Participant A no longer requires screening")]
    public async Task should_update_screening_to_none()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(options
            =>
        {
            options.AddScreening = true;
            options.Case = new Case("UpdateParticipantJudge", "UpdateParticipantJudge");
        }, BookingStatus.Created);
        var individuals = hearing.Participants.Where(x => x is Individual).ToList();
        var participantA = individuals.Find(i => i.Screening != null);
        
        var request = new UpdateHearingParticipantsRequestV2
        {
            ExistingParticipants =
            [
                new UpdateParticipantRequestV2
                {
                    ParticipantId = participantA.Id, 
                    DisplayName = "NewDisplayName",
                    FirstName = participantA.Person?.FirstName,
                    LastName = participantA.Person?.LastName,
                    OrganisationName = participantA.Person?.Organisation?.Name,
                    TelephoneNumber = participantA.Person?.TelephoneNumber,
                    Title = participantA.Person?.Title,
                    Screening = null
                }
            ]
        };
        
        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateHearingParticipants(hearing.Id),RequestBody.Set(request));
        
        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var updatedHearing = await client.GetAsync(ApiUriFactory.HearingsEndpointsV2.GetHearingDetailsById(hearing.Id.ToString()));
        
        var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(updatedHearing.Content);
        var updatedParticipant = hearingResponse.Participants.Find(x => x.Id == participantA.Id);
        updatedParticipant.Should().NotBeNull();
        updatedParticipant.Screening.Should().BeNull();
    }
    
    [Test(Description = "Participant A is screened from Participant B. Participant A is not screened from all")]
    public async Task should_update_screening_to_all()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(options
            =>
        {
            options.AddScreening = true;
            options.Case = new Case("UpdateParticipantJudge", "UpdateParticipantJudge");
        }, BookingStatus.Created);
        var individuals = hearing.Participants.Where(x => x is Individual).ToList();
        var participantA = individuals.Find(i => i.Screening != null);
        
        var request = new UpdateHearingParticipantsRequestV2
        {
            ExistingParticipants =
            [
                new UpdateParticipantRequestV2
                {
                    ParticipantId = participantA.Id, 
                    DisplayName = "NewDisplayName",
                    FirstName = participantA.Person?.FirstName,
                    LastName = participantA.Person?.LastName,
                    OrganisationName = participantA.Person?.Organisation?.Name,
                    TelephoneNumber = participantA.Person?.TelephoneNumber,
                    Title = participantA.Person?.Title,
                    Screening = new ScreeningRequest()
                    {
                        Type = ScreeningType.All
                    }
                }
            ]
        };
        
        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateHearingParticipants(hearing.Id),RequestBody.Set(request));
        
        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var updatedHearing = await client.GetAsync(ApiUriFactory.HearingsEndpointsV2.GetHearingDetailsById(hearing.Id.ToString()));
        
        var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(updatedHearing.Content);
        var updatedParticipant = hearingResponse.Participants.Find(x => x.Id == participantA.Id);
        updatedParticipant.Should().NotBeNull();
        updatedParticipant.Screening.Type.Should().Be(ScreeningType.All);
        updatedParticipant.Screening.ProtectFromParticipantsIds.Should().BeEmpty();
        updatedParticipant.Screening.ProtectFromEndpointsIds.Should().BeEmpty();
    }
}