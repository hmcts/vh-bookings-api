using BookingsApi.Client;
using BookingsApi.Contract.V2.Enums;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Contract.V2.Responses;
using BookingsApi.DAL.Queries;
using Testing.Common.Builders.Api.V2;

namespace BookingsApi.IntegrationTests.Api.V2.Hearings;

/// <summary>
/// Although Clone hearing is a V1 endpoint, we need to test its ability to clone V2 hearings
/// </summary>
public class CloneHearingTests : ApiTest
{
    [Test]
    public async Task should_clone_hearing_with_screening_requirements()
    {
        // arrange
        using var client = Application.CreateClient();
        var hearing = await BookHearingWithScreening(client);
        var participantWithScreening = hearing.Participants.Find(x => x.Screening != null);
        
        var groupId = hearing.GroupId.GetValueOrDefault();

        var dates = new List<DateTime> {hearing.ScheduledDateTime.AddDays(2), hearing.ScheduledDateTime.AddDays(3)};
        
        // act
        var request = new CloneHearingRequestV2 { Dates = dates }; // No duration specified - should use the default
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpointsV2.CloneHearing(hearing.Id), RequestBody.Set(request));
        
        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var bookingsApiClient = BookingsApiClient.GetClient(client);
        
        var groupedHearing = await bookingsApiClient.GetHearingsByGroupIdV2Async(groupId);
        
        groupedHearing.Count.Should().Be(dates.Count + 1);

        // verify that the participant with screening is cloned correctly
        foreach (var h in groupedHearing.Where(x=> x.Id != hearing.Id))
        {
            // get hearings by group does not include screening
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var hearingFromQuery = await new GetHearingByIdQueryHandler(db).Handle(new GetHearingByIdQuery(h.Id));
            // find by external reference id
            var clonedParticipant = hearingFromQuery.Participants.First(x => x.ExternalReferenceId == participantWithScreening.ExternalReferenceId);
            clonedParticipant.Should().NotBeNull();
            
            // verify that the screening is cloned correctly
            clonedParticipant.Screening.Should().NotBeNull();
            clonedParticipant.Screening.Type.Should().Be(Domain.Enumerations.ScreeningType.Specific);

            var participantExternalRefs = clonedParticipant.Screening.GetParticipants().Select(x => x.Participant.ExternalReferenceId);
            var endpointExternalRefs = clonedParticipant.Screening.GetEndpoints().Select(x => x.Endpoint.ExternalReferenceId);
            var combinedExternalRefs = participantExternalRefs.Concat(endpointExternalRefs).ToList();
            
            combinedExternalRefs.Should().BeEquivalentTo(participantWithScreening.Screening.ProtectedFrom);
        }
        
        
        
        groupedHearing.All(x => x.GroupId == groupId).Should().BeTrue();
    }
    
    [Test(Description = "Copy of test above using the route used by S&L")]
    public async Task should_clone_hearing_with_screening_requirements_for_sanl()
    {
        // arrange
        using var client = Application.CreateClient();
        var hearing = await BookHearingWithScreening(client);
        var participantWithScreening = hearing.Participants.Find(x => x.Screening != null);
        
        var groupId = hearing.GroupId.GetValueOrDefault();

        var dates = new List<DateTime> {hearing.ScheduledDateTime.AddDays(2), hearing.ScheduledDateTime.AddDays(3)};
        
        // act
        var request = new CloneHearingRequestV2 { Dates = dates }; // No duration specified - should use the default
        var result = await client.PostAsync($"hearings/{hearing.Id}/clone", RequestBody.Set(request));
        
        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var bookingsApiClient = BookingsApiClient.GetClient(client);
        
        var groupedHearing = await bookingsApiClient.GetHearingsByGroupIdV2Async(groupId);
        
        groupedHearing.Count.Should().Be(dates.Count + 1);

        // verify that the participant with screening is cloned correctly
        foreach (var h in groupedHearing.Where(x=> x.Id != hearing.Id))
        {
            // get hearings by group does not include screening
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var hearingFromQuery = await new GetHearingByIdQueryHandler(db).Handle(new GetHearingByIdQuery(h.Id));
            // find by external reference id
            var clonedParticipant = hearingFromQuery.Participants.First(x => x.ExternalReferenceId == participantWithScreening.ExternalReferenceId);
            clonedParticipant.Should().NotBeNull();
            
            // verify that the screening is cloned correctly
            clonedParticipant.Screening.Should().NotBeNull();
            clonedParticipant.Screening.Type.Should().Be(Domain.Enumerations.ScreeningType.Specific);

            var participantExternalRefs = clonedParticipant.Screening.GetParticipants().Select(x => x.Participant.ExternalReferenceId);
            var endpointExternalRefs = clonedParticipant.Screening.GetEndpoints().Select(x => x.Endpoint.ExternalReferenceId);
            var combinedExternalRefs = participantExternalRefs.Concat(endpointExternalRefs).ToList();
            
            combinedExternalRefs.Should().BeEquivalentTo(participantWithScreening.Screening.ProtectedFrom);
        }
        
        groupedHearing.All(x => x.GroupId == groupId).Should().BeTrue();
    }

    private async Task<HearingDetailsResponseV2> BookHearingWithScreening(HttpClient client)
    {
        var request = await CreateBookingRequestWithServiceIdsAndCodes();
        request.IsMultiDayHearing = true;
        request.Participants = request.Participants.Take(2).ToList();
        
        var endpoint = new EndpointRequestV2
        {
            DisplayName = "Endpoint A",
            ExternalParticipantId = Guid.NewGuid().ToString()
        };
        request.Endpoints.Add(endpoint);
        
        var participantWithSpecificScreening = request.Participants[0];
        participantWithSpecificScreening.DisplayName = "Screen Specific Protected 1";
        var protectedFrom = request.Participants[1];
        
        participantWithSpecificScreening.Screening = new ScreeningRequest
        {
            Type = ScreeningType.Specific,
            ProtectedFrom = [protectedFrom.ExternalParticipantId, endpoint.ExternalParticipantId]
        };
        
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpointsV2.BookNewHearing, RequestBody.Set(request));
        
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdResponse = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(result.Content);
        Hooks.AddHearingForCleanup(createdResponse.Id);
        return createdResponse;
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