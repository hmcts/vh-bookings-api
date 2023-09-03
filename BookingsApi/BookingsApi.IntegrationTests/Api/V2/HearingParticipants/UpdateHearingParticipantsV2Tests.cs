using BookingsApi.Contract.V2.Requests;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Validations.V2;

namespace BookingsApi.IntegrationTests.Api.V2.HearingParticipants;

public class UpdateHearingParticipantsV2Tests : ApiTest
{
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
                },
            },
            NewParticipants = new List<ParticipantRequestV2>()
            {
                new ()
                {
                    Username = "newusername@test.email.com",
                    CaseRoleName = "Applicant",
                    DisplayName = "DisplayName",
                    FirstName = "NewFirstName",
                    HearingRoleName = "Applicant LIP",
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
        var hearing = await Hooks.SeedVideoHearing(options
            => { options.Case = new Case("Case1 Num", "Case1 Name"); },false, BookingStatus.Created);
        
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
        var hearing = await Hooks.SeedVideoHearing(options
            => { options.Case = new Case("Case1 Num", "Case1 Name"); },false, BookingStatus.Created);
        
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
                }
            },
            NewParticipants = new List<ParticipantRequestV2>
            {
                new ()
                {
                    Username = "username@test.email.com",
                    CaseRoleName = null,
                    DisplayName = "DisplayName",
                    FirstName = "FirstName",
                    HearingRoleName = "Applicant",
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
        validationProblemDetails.Errors["NewParticipants[0].ContactEmail"].Should().Contain(ParticipantRequestValidationV2.NoContactEmailErrorMessage);
        validationProblemDetails.Errors["NewParticipants[0].DisplayName"].Should().Contain(ParticipantRequestValidationV2.NoDisplayNameErrorMessage);
        validationProblemDetails.Errors["ExistingParticipants[0].ParticipantId"].Should().Contain(UpdateParticipantRequestValidationV2.NoParticipantIdErrorMessage);
        validationProblemDetails.Errors["ExistingParticipants[0].DisplayName"].Should().Contain(UpdateParticipantRequestValidationV2.NoDisplayNameErrorMessage);
        
    }
    
    [Test]
    public async Task should_return_validation_errors_when_hearing_role_not_found_from_case_role()
    {
        // arrange
        var hearingRoleName = "Invalid Role";
        var hearing = await Hooks.SeedVideoHearing(options
            => { options.Case = new Case("UpdateParticipantDataValidationFailure", "UpdateParticipantDataValidationFailure"); },false, BookingStatus.Created);
        
        var request = new UpdateHearingParticipantsRequestV2
        {
            NewParticipants = new List<ParticipantRequestV2>
            {
                new ()
                {
                    Username = "newusername@test.email.com",
                    CaseRoleName = "Applicant",
                    DisplayName = "DisplayName",
                    FirstName = "NewFirstName",
                    HearingRoleName = "Applicant LIP",
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
        validationProblemDetails.Errors[$"{nameof(request.NewParticipants )}[0]"].Should().Contain($"Invalid hearing role [{hearingRoleName}]");
    }

    [Test]
    public async Task should_return_validation_errors_when_flat_structure_hearing_role_not_found()
    {
        // arrange
        var hearingRoleName = "Invalid Role";
        var hearing = await Hooks.SeedVideoHearing(options
            => { options.Case = new Case("UpdateParticipantDataValidationFailure", "UpdateParticipantDataValidationFailure"); },false, BookingStatus.Created);
        
        var request = new UpdateHearingParticipantsRequestV2
        {
            NewParticipants = new List<ParticipantRequestV2>
            {
                new ()
                {
                    Username = "username@test.email.com",
                    CaseRoleName = null,
                    DisplayName = "DisplayName",
                    FirstName = "FirstName",
                    HearingRoleName = hearingRoleName,
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
        validationProblemDetails.Errors[$"{nameof(request.NewParticipants )}[0]"].Should().Contain($"Invalid hearing role [{hearingRoleName}]");
    }
}