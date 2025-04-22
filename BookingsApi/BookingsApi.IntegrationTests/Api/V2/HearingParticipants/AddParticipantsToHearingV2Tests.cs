using BookingsApi.Contract.V2.Enums;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Contract.V2.Responses;
using BookingsApi.Domain.Constants;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Validations.Common;
using BookingsApi.Validations.V2;
using ScreeningType = BookingsApi.Contract.V2.Enums.ScreeningType;

namespace BookingsApi.IntegrationTests.Api.V2.HearingParticipants;

public class AddParticipantsToHearingV2Tests : ApiTest
{
    [Test]
    public async Task should_add_participant_to_hearing_and_return_200()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(options =>
            {
                options.Case = new Case("Case1 Num", "Case1 Name");
            },
            BookingStatus.Created);
        
        var request = BuildRequestObject();

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpointsV2.AddParticipantsToHearing(hearing.Id),RequestBody.Set(request));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
    }

    [Test]
    public async Task should_return_400_when_input_validation_fails()
    {
        var request = BuildRequestObject();
        request.Participants = [];

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpointsV2.AddParticipantsToHearing(Guid.NewGuid()),RequestBody.Set(request));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest, result.Content.ReadAsStringAsync().Result);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors["Participants"][0].Should().Be(AddParticipantsToHearingRequestInputValidationV2.NoParticipantsErrorMessage);
    }

    [Test]
    public async Task should_return_400_representative_validation_fails()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(options =>
            {
                options.Case = new Case("Case1 Num", "Case1 Name");
            },
            BookingStatus.Created);
        
        var request = BuildRequestObject();
        request.Participants[0].HearingRoleCode = HearingRoleCodes.Representative;
        request.Participants[0].Representee = string.Empty;

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpointsV2.AddParticipantsToHearing(hearing.Id),RequestBody.Set(request));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest, result.Content.ReadAsStringAsync().Result);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors["Participants[0].Representee"][0].Should().Be(RepresentativeValidation.NoRepresentee);
    }
    
    [Test]
    public async Task should_return_404_when_hearing_does_not_exist()
    {
        var request = BuildRequestObject();

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpointsV2.AddParticipantsToHearing(Guid.NewGuid()),RequestBody.Set(request));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound, result.Content.ReadAsStringAsync().Result);
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
        
        var request = BuildRequestObject();
        var participant = hearing.Participants.First(x => x is Individual);
        request.Participants[0].HearingRoleCode = HearingRoleCodes.Interpreter;
        request.LinkedParticipants = new List<LinkedParticipantRequestV2>
        {
            new ()
            {
                Type = LinkedParticipantTypeV2.Interpreter,
                ParticipantContactEmail = request.Participants[0].ContactEmail,
                LinkedParticipantContactEmail = participant.Person.ContactEmail
            }
        };

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpointsV2.AddParticipantsToHearing(hearing.Id),RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
    }

    [Test]
    public async Task should_add_a_participant_with_screening_requirements_all()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(options =>
            {
                options.Case = new Case("Case1 Num", "Case1 Name");
            },
            BookingStatus.Created);
        
        var request = BuildRequestObject();
        request.Participants[0].Screening = new ScreeningRequest()
        {
           Type = ScreeningType.All
        };

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpointsV2.AddParticipantsToHearing(hearing.Id),RequestBody.Set(request));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var updatedHearing = await client.GetAsync(ApiUriFactory.HearingsEndpointsV2.GetHearingDetailsById(hearing.Id.ToString()));
        var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(updatedHearing.Content);

        var addedParticipant =
            hearingResponse.Participants.Find(x => x.ContactEmail == request.Participants[0].ContactEmail);
        
        addedParticipant.Should().NotBeNull();
        addedParticipant.Screening.Should().NotBeNull();
        addedParticipant.Screening.Type.Should().Be(ScreeningType.All);
    }
    
    [Test]
    public async Task should_add_a_participant_with_screening_requirements_specific()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(options =>
            {
                options.Case = new Case("Case1 Num", "Case1 Name");
            },
            BookingStatus.Created);
        
        var request = BuildRequestObject();
        request.Participants[0].Screening = new ScreeningRequest()
        {
            Type = ScreeningType.Specific,
            ProtectedFrom = [hearing.Endpoints[0].ExternalReferenceId]
        };

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpointsV2.AddParticipantsToHearing(hearing.Id),RequestBody.Set(request));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var updatedHearing = await client.GetAsync(ApiUriFactory.HearingsEndpointsV2.GetHearingDetailsById(hearing.Id.ToString()));
        var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(updatedHearing.Content);

        var addedParticipant =
            hearingResponse.Participants.Find(x => x.ContactEmail == request.Participants[0].ContactEmail);
        
        addedParticipant.Should().NotBeNull();
        addedParticipant.Screening.Should().NotBeNull();
        addedParticipant.Screening.Type.Should().Be(ScreeningType.Specific);
        addedParticipant.Screening.ProtectedFrom.Should().Contain(hearing.Endpoints[0].ExternalReferenceId);
    }
    
    private static AddParticipantsToHearingRequestV2 BuildRequestObject()
    {
        var request = new AddParticipantsToHearingRequestV2
        {
            Participants = new List<ParticipantRequestV2>
            {
                new ()
                {
                    DisplayName = "DisplayName Added",
                    FirstName = "FirstName",
                    HearingRoleCode = HearingRoleCodes.Applicant,
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
        return request;
    }
}