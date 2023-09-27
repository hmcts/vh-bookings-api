using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Requests.Enums;
using BookingsApi.Contract.V2.Enums;
using BookingsApi.Contract.V2.Responses;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.DAL.Helper;
using BookingsApi.Domain.Constants;
using BookingsApi.Domain.Validations;
using BookingsApi.Validations.V2;
using Testing.Common.Builders.Api.V2;

namespace BookingsApi.IntegrationTests.Api.V2.Hearings;

public class BookNewHearingV2Tests : ApiTest
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
    public async Task should_book_a_hearing_with_codes_instead_of_names()
    {
        // arrange
        var request = CreateBookingRequestWithServiceIdsAndCodes();

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpointsV2.BookNewHearing, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue(result.Content.ReadAsStringAsync().Result);
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var getHearingUri = result.Headers.Location;
        var getResponse = await client.GetAsync(getHearingUri);
        var createdResponse = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(result.Content);
        var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(getResponse.Content);
        createdResponse.Should().BeEquivalentTo(hearingResponse);
        _hearingIds.Add(hearingResponse.Id);
    }
    
    [Test]
    public async Task should_book_a_hearing_with_codes_and_judiciary_judge()
    {
        // arrange
        var request = CreateBookingRequestWithServiceIdsAndCodes();
        request.Participants = request.Participants.Where(x => x.HearingRoleCode != HearingRoleCodes.Judge).ToList();
        var judiciaryPerson = await Hooks.AddJudiciaryPerson();
        var judiciaryJudgeRequest = new JudiciaryParticipantRequest()
        {
            PersonalCode = judiciaryPerson.PersonalCode,
            HearingRoleCode = JudiciaryParticipantHearingRoleCode.Judge,
            DisplayName = "Judiciary Judge"
        };
        request.JudiciaryParticipants = new List<JudiciaryParticipantRequest>() {judiciaryJudgeRequest};
        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpointsV2.BookNewHearing, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue(result.Content.ReadAsStringAsync().Result);
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var getHearingUri = result.Headers.Location;
        var getResponse = await client.GetAsync(getHearingUri);
        var createdResponse = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(result.Content);
        _hearingIds.Add(createdResponse.Id);
        createdResponse.JudiciaryParticipants.Should().Contain(x =>
            x.PersonalCode == judiciaryJudgeRequest.PersonalCode &&
            x.HearingRoleCode == judiciaryJudgeRequest.HearingRoleCode && 
            x.DisplayName == judiciaryJudgeRequest.DisplayName
            );
        var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(getResponse.Content);
        createdResponse.Should().BeEquivalentTo(hearingResponse);
        
    }

    [Test]
    public async Task should_book_a_hearing_without_case_roles()
    {
        // arrange
        var request = CreateBookingRequestWithServiceIdsAndCodes();

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpointsV2.BookNewHearing, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue(result.Content.ReadAsStringAsync().Result);
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var getHearingUri = result.Headers.Location;
        var getResponse = await client.GetAsync(getHearingUri);
        var createdResponse = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(result.Content);
        _hearingIds.Add(createdResponse.Id);
        var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(getResponse.Content);
        createdResponse.Should().BeEquivalentTo(hearingResponse);
    }
    
    [Test]
    public async Task should_return_validation_error_when_flat_structure_hearing_role_not_found()
    {
        // arrange
        var request = CreateBookingRequestWithServiceIdsAndCodes();
        var hearingRoleCode = "Invalid Code";
        request.Participants.ForEach(x =>
        {
            x.HearingRoleCode = hearingRoleCode;
        });

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpointsV2.BookNewHearing, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[$"{nameof(request.Participants)}[0]"].Should().Contain($"Invalid hearing role [{hearingRoleCode}]");
    }
    
    [Test]
    public async Task should_return_validation_error_when_validation_fails()
    {
        // arrange
        var request = CreateBookingRequestWithServiceIdsAndCodes();
        request.HearingVenueCode = null;
        request.ServiceId = null;
        request.HearingTypeCode = null;
        request.ScheduledDateTime = DateTime.UtcNow.AddDays(-1);

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpointsV2.BookNewHearing, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[nameof(request.HearingVenueCode)][0].Should()
            .Be(BookNewHearingRequestInputValidationV2.HearingVenueCodeErrorMessage);
        
        validationProblemDetails.Errors[nameof(request.ServiceId)][0].Should()
            .Be(BookNewHearingRequestInputValidationV2.CaseTypeServiceIdErrorMessage);
        
        validationProblemDetails.Errors[nameof(request.HearingTypeCode)][0].Should()
            .Be(BookNewHearingRequestInputValidationV2.HearingTypeCodeErrorMessage);
    }
    
    [Test]
    public async Task should_return_validation_error_when_case_type_service_id_is_not_found()
    {
        // arrange
        var request = CreateBookingRequestWithServiceIdsAndCodes();
        request.ServiceId = "999299292929";

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpointsV2.BookNewHearing, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[nameof(request.ServiceId)][0].Should()
            .Be("Case type does not exist");
    }
    
    [Test]
    public async Task should_return_validation_error_when_hearing_type_code_is_not_found()
    {
        // arrange
        var request = CreateBookingRequestWithServiceIdsAndCodes();
        request.HearingTypeCode = "999299292929";

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpointsV2.BookNewHearing, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        
        // regex to find the error message as the error message is not consistent
        // message should contain "Hearing type code [code] does not exist"
        // regex for message 'Hearing type code [0-9]+ does not exist'
        validationProblemDetails.Errors[nameof(request.HearingTypeCode)][0].Should()
            .MatchRegex("Hearing type code [A-Za-z0-9]+ does not exist");
    }
    
    [Test]
    public async Task should_return_validation_error_when_venue_code_is_not_found()
    {
        // arrange
        var request = CreateBookingRequestWithServiceIdsAndCodes();
        request.HearingVenueCode = "999299292929";

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpointsV2.BookNewHearing, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[nameof(request.HearingVenueCode)][0].Should()
            .MatchRegex("HearingVenueCode [A-Za-z0-9]+ does not exist");
    }

    [Test(Description = "To be backwards compatible, judges can be added via participants or judiciaryparticipants")]
    public async Task should_return_validation_error_when_booking_a_hearing_with_judge_and_a_judiciary_judge()
    {
        // arrange
        var personalCodeJudge = Guid.NewGuid().ToString("N");
        var personalCodePanelMember = Guid.NewGuid().ToString("N");
        var request = CreateBookingRequestWithServiceIdsAndCodes();
        var judiciaryPersonJudge = await Hooks.AddJudiciaryPerson(personalCode: personalCodeJudge);
        var judiciaryPersonPanelMember = await Hooks.AddJudiciaryPerson(personalCode: personalCodePanelMember);
        request.JudiciaryParticipants = new List<JudiciaryParticipantRequest>()
        {
            new ()
            {
                PersonalCode = judiciaryPersonJudge.PersonalCode,
                HearingRoleCode = JudiciaryParticipantHearingRoleCode.Judge,
                DisplayName = "Judiciary Judge"
            },
            new ()
            {
                PersonalCode = judiciaryPersonPanelMember.PersonalCode,
                HearingRoleCode = JudiciaryParticipantHearingRoleCode.PanelMember,
                DisplayName = "Judiciary Panel Member"
            }
        };

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpointsV2.BookNewHearing, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors["judiciaryPerson"][0].Should()
            .Be(DomainRuleErrorMessages.ParticipantWithJudgeRoleAlreadyExists);
    }
            
    [Test]
    public async Task should_book_a_hearing_without_a_judge()
    {
        // arrange
        var request = CreateBookingRequestWithServiceIdsAndCodes();
        var judge = request.Participants.Find(p => p.HearingRoleCode == HearingRoleCodes.Judge); 
        request.Participants.Remove(judge);
        
        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpointsV2.BookNewHearing, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var getHearingUri = result.Headers.Location;
        var getResponse = await client.GetAsync(getHearingUri);
        var createdResponse = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(result.Content);
        var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(getResponse.Content);
        createdResponse.Should().BeEquivalentTo(hearingResponse);
        hearingResponse.Status.Should().Be(BookingStatusV2.BookedWithoutJudge);
        _hearingIds.Add(hearingResponse.Id);
        
    }
    
    private static BookNewHearingRequestV2 CreateBookingRequestWithServiceIdsAndCodes()
    {
        var hearingSchedule = DateTime.UtcNow.AddMinutes(5);
        var caseName = "Bookings Api Integration Automated";
        var request = new SimpleBookNewHearingRequestV2(caseName, hearingSchedule).Build();
        request.ServiceId = "vhG1"; // intentionally incorrect case
        request.HearingTypeCode = "automatedtest"; // intentionally incorrect case
        request.HearingVenueCode = "231596";
        return request;
    }
}