using BookingsApi.Contract.V2.Enums;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.Validations;

namespace BookingsApi.IntegrationTests.Api.V2.HearingParticipants;

public class AddParticipantsToHearingV2Tests : ApiTest
{
    [Test]
    public async Task should_add_participant_to_hearing_and_return_200()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearing(options => { options.Case = new Case("Case1 Num", "Case1 Name"); },
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
    public async Task should_add_participant_to_hearing_without_case_role_and_return_200()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearing(options => { options.Case = new Case("Case1 Num", "Case1 Name"); },
            BookingStatus.Created);
        
        var request = BuildRequestObject();
        request.Participants.ForEach(x =>
        {
            x.CaseRoleName = null;
            if (x.HearingRoleName.Contains("LIP"))
                x.HearingRoleName = "Applicant";
            if (x.HearingRoleName == "Representative")
                x.HearingRoleName = "Legal Representative";
        });

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpointsV2.AddParticipantsToHearing(hearing.Id),RequestBody.Set(request));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
    }
    
    [Test]
    public async Task should_fail_to_add_an_interpreter_participant_to_hearing_when_hearing_is_close_to_start_time_and_return_400()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearing(options =>
            {
                options.Case = new Case("Case1 Num", "Case1 Name");
                options.ScheduledDate = DateTime.UtcNow.AddMinutes(25);
            },
            BookingStatus.Created);
        
        var request = BuildRequestObject();
        var participant = hearing.Participants.First(x => x is Individual);
        request.Participants[0].HearingRoleName = "Interpreter";
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
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors.SelectMany(x => x.Value).Should()
            .Contain(DomainRuleErrorMessages.CannotAddInterpreterToHearingCloseToStartTime);
    }
    
    private static AddParticipantsToHearingRequestV2 BuildRequestObject()
    {
        var request = new AddParticipantsToHearingRequestV2
        {
            Participants = new List<ParticipantRequestV2>
            {
                new ()
                {
                    Username = "username@test.email.com",
                    CaseRoleName = "Applicant",
                    DisplayName = "DisplayName",
                    FirstName = "FirstName",
                    HearingRoleName = "Applicant LIP",
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