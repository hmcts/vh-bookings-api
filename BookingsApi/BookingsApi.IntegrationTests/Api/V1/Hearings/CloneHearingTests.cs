using BookingsApi.Common.Services;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Infrastructure.Services;
using BookingsApi.Infrastructure.Services.Dtos;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using BookingsApi.Mappings.Common;
using BookingsApi.Validations.V1;
using Testing.Common.Stubs;
using Constants = BookingsApi.Contract.V1.Constants;

namespace BookingsApi.IntegrationTests.Api.V1.Hearings;

public class CloneHearingTests : ApiTest
{
    [Test]
    public async Task should_return_all_cloned_hearings_for_the_dates_with_unspecified_duration()
    {
        // arrange
        var startingDate = DateTime.UtcNow.AddMinutes(5);
        var hearing1 = await Hooks.SeedVideoHearing(isMultiDayFirstHearing:true, configureOptions: options =>
        {
            options.ScheduledDate = startingDate;
        });
        var groupId = hearing1.SourceId;

        var dates = new List<DateTime> {startingDate.AddDays(2), startingDate.AddDays(3)};

        // act
        using var client = Application.CreateClient();
        var request = new CloneHearingRequest { Dates = dates }; // No duration specified - should use the default
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.CloneHearing(hearing1.Id), RequestBody.Set(request));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var clonedHearingsList = await ApiClientResponse.GetResponses<List<HearingDetailsResponse>>(result.Content);
        clonedHearingsList.Count.Should().Be(dates.Count);
        var first = clonedHearingsList.Single(x => x.ScheduledDateTime == dates[0]);
        var second = clonedHearingsList.Single(x => x.ScheduledDateTime == dates[1]);
        first.Endpoints.Should().NotBeEquivalentTo(second.Endpoints);
        
        clonedHearingsList.TrueForAll(x => x.GroupId == groupId).Should().BeTrue();

        first.ScheduledDuration.Should().Be(Constants.CloneHearings.DefaultScheduledDuration);
        second.ScheduledDuration.Should().Be(Constants.CloneHearings.DefaultScheduledDuration);
    }

    [Test]
    public async Task should_return_all_cloned_hearings_for_the_dates_with_specified_duration()
    {
        // arrange
        var startingDate = DateTime.UtcNow.AddMinutes(5);
        var hearing1 = await Hooks.SeedVideoHearing(isMultiDayFirstHearing:true, configureOptions: options =>
        {
            options.ScheduledDate = startingDate;
        });
        var groupId = hearing1.SourceId;

        var dates = new List<DateTime> {startingDate.AddDays(2), startingDate.AddDays(3)};
        const int specifiedDuration = 120;

        // act
        using var client = Application.CreateClient();
        var request = new CloneHearingRequest
        {
            Dates = dates,
            ScheduledDuration = specifiedDuration // Duration specifeid
        };
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.CloneHearing(hearing1.Id), RequestBody.Set(request));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var clonedHearingsList = await ApiClientResponse.GetResponses<List<HearingDetailsResponse>>(result.Content);
        clonedHearingsList.Count.Should().Be(dates.Count);
        var first = clonedHearingsList.Single(x => x.ScheduledDateTime == dates[0]);
        var second = clonedHearingsList.Single(x => x.ScheduledDateTime == dates[1]);
        first.Endpoints.Should().NotBeEquivalentTo(second.Endpoints);
        
        clonedHearingsList.TrueForAll(x => x.GroupId == groupId).Should().BeTrue();

        first.ScheduledDuration.Should().Be(specifiedDuration);
        second.ScheduledDuration.Should().Be(specifiedDuration);
    }

    [Test]
    public async Task should_return_all_cloned_hearings_with_judiciary_participants_for_the_dates()
    {
        // arrange
        var startingDate = DateTime.UtcNow.AddMinutes(5);
        var hearing1 = await Hooks.SeedVideoHearingV2(isMultiDayFirstHearing:true, configureOptions: options =>
        {
            options.ScheduledDate = startingDate;
            options.AddJudge = true;
            options.AddPanelMember = true;
        });
        var groupId = hearing1.SourceId;

        var dates = new List<DateTime> {startingDate.AddDays(2), startingDate.AddDays(3)};
        
        // act
        using var client = Application.CreateClient();
        var request = new CloneHearingRequest { Dates = dates };
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.CloneHearing(hearing1.Id), RequestBody.Set(request));
        
        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var clonedHearingsList = await ApiClientResponse.GetResponses<List<HearingDetailsResponse>>(result.Content);
        clonedHearingsList.Count.Should().Be(dates.Count);
        clonedHearingsList.TrueForAll(x => x.GroupId == groupId).Should().BeTrue();

        foreach (var clonedHearing in clonedHearingsList)
        {
            AssertClonedJudiciaryParticipants(clonedHearing, hearing1.JudiciaryParticipants);
        }
    }

    [Test]
    public async Task should_clone_hearing_for_v1_and_new_notify_templates_feature_toggled_off()
    {
        // arrange
        var startingDate = DateTime.UtcNow.AddMinutes(5);
        var hearing1 = await Hooks.SeedVideoHearing(isMultiDayFirstHearing:true, configureOptions: options =>
        {
            options.ScheduledDate = startingDate;
        });

        var dates = new List<DateTime> {startingDate.AddDays(2), startingDate.AddDays(3)};
        
        var featureToggles = (FeatureTogglesStub)Application.Services.GetService(typeof(IFeatureToggles));
        featureToggles.NewTemplates = false;

        // act
        using var client = Application.CreateClient();
        var request = new CloneHearingRequest { Dates = dates }; // No duration specified - should use the default
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.CloneHearing(hearing1.Id), RequestBody.Set(request));
        
        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        AssertEventsPublishedForV1AndNotifyFeatureOff(request, hearing1);
    }

    [Test]
    public async Task should_return_validation_error_when_validation_fails()
    {
        // arrange
        var startingDate = DateTime.UtcNow.AddMinutes(5);
        var hearing1 = await Hooks.SeedVideoHearing(isMultiDayFirstHearing:true, configureOptions: options =>
        {
            options.ScheduledDate = startingDate;
        });

        var dates = new List<DateTime> {startingDate.AddDays(2), startingDate.AddDays(3)};
        const int specifiedDuration = -1; // Invalid value

        // act
        using var client = Application.CreateClient();
        var request = new CloneHearingRequest
        {
            Dates = dates,
            ScheduledDuration = specifiedDuration
        };
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.CloneHearing(hearing1.Id), RequestBody.Set(request));
        
        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[nameof(request.ScheduledDuration)][0].Should()
            .Be(CloneHearingRequestValidation.InvalidScheduledDuration);
    }

    private static void AssertClonedJudiciaryParticipants(
        HearingDetailsResponse cloneHearingResponse, 
        ICollection<JudiciaryParticipant> originalJudiciaryParticipants)
    {
        cloneHearingResponse.JudiciaryParticipants.Count.Should().Be(originalJudiciaryParticipants.Count);
            
        var mapper = new JudiciaryParticipantToResponseMapper();

        foreach (var clonedJudiciaryParticipant in cloneHearingResponse.JudiciaryParticipants)
        {
            var judiciaryParticipant = originalJudiciaryParticipants.FirstOrDefault(x => x.JudiciaryPerson.PersonalCode == clonedJudiciaryParticipant.PersonalCode);
            judiciaryParticipant.Should().NotBeNull();
            clonedJudiciaryParticipant.Should().BeEquivalentTo(mapper.MapJudiciaryParticipantToResponse(judiciaryParticipant));
        }
    }

    private void AssertEventsPublishedForV1AndNotifyFeatureOff(CloneHearingRequest request, Hearing hearing)
    {
        var serviceBusStub = Application.Services
            .GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
        var messages = serviceBusStub!
            .ReadAllMessagesFromQueue(hearing.Id);
            
        var expectedTotalMessageCount = hearing.Participants.Count;
        var totalDays = request.Dates.Count + 1;
        
        messages.Count(x => x.IntegrationEvent is MultiDayHearingIntegrationEvent).Should().Be(expectedTotalMessageCount);
        var hearingConfirmationDtos = GetHearingConfirmationDtos(hearing);
        var multiDayIntegrationEvents = messages
            .Where(x => x.IntegrationEvent is MultiDayHearingIntegrationEvent)
            .Select(x => x.IntegrationEvent as MultiDayHearingIntegrationEvent)
            .ToList();
        multiDayIntegrationEvents.TrueForAll(x => x.TotalDays == totalDays).Should().BeTrue();
        multiDayIntegrationEvents.Select(x => x.HearingConfirmationForParticipant)
            .Should().BeEquivalentTo(hearingConfirmationDtos);
    }
    
    private static IEnumerable<HearingConfirmationForParticipantDto> GetHearingConfirmationDtos(Hearing hearing)
    {
        var participantDtos = hearing.Participants
            .Select(p => ParticipantDtoMapper.MapToDto(p, hearing.OtherInformation))
            .ToList();
        var @case = hearing.GetCases()[0];
        var hearingConfirmationDtos = participantDtos
            .Select(p => EventDtoMappers.MapToHearingConfirmationDto(
                hearing.Id, hearing.ScheduledDateTime, p, @case))
            .ToList();

        return hearingConfirmationDtos;
    }
}