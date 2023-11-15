using BookingsApi.Contract.V1.Requests.Enums;
using BookingsApi.Contract.V2.Enums;
using BookingsApi.Contract.V2.Responses;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
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
        var request = await CreateBookingRequestWithServiceIdsAndCodes();

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
        _hearingIds.Add(hearingResponse.Id);

        createdResponse.Should().BeEquivalentTo(hearingResponse);
        var judiciaryJudgeRequest = request.JudiciaryParticipants[0];
        createdResponse.JudiciaryParticipants.Should().Contain(x =>
            x.PersonalCode == judiciaryJudgeRequest.PersonalCode &&
            x.HearingRoleCode == judiciaryJudgeRequest.HearingRoleCode &&
            x.DisplayName == judiciaryJudgeRequest.DisplayName
        );
        
        var serviceBusStub =
            Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
        var message = serviceBusStub!.ReadMessageFromQueue();
        message.IntegrationEvent.Should().BeOfType<HearingIsReadyForVideoIntegrationEvent>();
    }

    [Test]
    public async Task should_return_validation_error_when_flat_structure_hearing_role_not_found()
    {
        // arrange
        var request = await CreateBookingRequestWithServiceIdsAndCodes();
        var hearingRoleCode = "Invalid Code";
        request.Participants.ForEach(x => { x.HearingRoleCode = hearingRoleCode; });

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpointsV2.BookNewHearing, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[$"{nameof(request.Participants)}[0]"].Should()
            .Contain($"Invalid hearing role [{hearingRoleCode}]");
    }

    [Test]
    public async Task should_return_validation_error_when_validation_fails()
    {
        // arrange
        var request = await CreateBookingRequestWithServiceIdsAndCodes();
        request.HearingVenueCode = null;
        request.ServiceId = null;
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
    }

    [Test]
    public async Task should_return_validation_error_when_case_type_service_id_is_not_found()
    {
        // arrange
        var request = await CreateBookingRequestWithServiceIdsAndCodes();
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
    public async Task should_return_validation_error_when_venue_code_is_not_found()
    {
        // arrange
        var request = await CreateBookingRequestWithServiceIdsAndCodes();
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

    [Test]
    public async Task should_book_a_hearing_without_a_judge()
    {
        // arrange
        var request = await CreateBookingRequestWithServiceIdsAndCodes();
        var judge = request.JudiciaryParticipants.Find(p =>
            p.HearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge);
        request.JudiciaryParticipants.Remove(judge);

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

    private async Task<BookNewHearingRequestV2> CreateBookingRequestWithServiceIdsAndCodes()
    {
        var personalCode = Guid.NewGuid().ToString();
        await Hooks.AddJudiciaryPerson(personalCode);
        var hearingSchedule = DateTime.UtcNow.AddMinutes(5);
        var caseName = "Bookings Api Integration Automated";
        var request = new SimpleBookNewHearingRequestV2(caseName, hearingSchedule, personalCode).Build();
        request.ServiceId = "ZZY1"; // intentionally incorrect case
        request.HearingVenueCode = "231596";
        return request;
    }
}