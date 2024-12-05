using BookingsApi.Client;
using BookingsApi.Domain.Participants;

namespace BookingsApi.IntegrationTests.Api.V1.Participants;

public class RemoveParticipantFromHearingTests : ApiTest
{
    [Test]
    public async Task should_get_validation_problem_when_hearing_id_is_invalid()
    {
        var hearingId = Guid.Empty;
        var participantId = Guid.NewGuid();
        
        var client = GetBookingsApiClient();
        
        // act
        var act = async () => await client.RemoveParticipantFromHearingAsync(hearingId, participantId);
        
        // assert
        var exception = await act.Should().ThrowAsync<BookingsApiException>();
        exception.And.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        var bookingsApiException = exception.And.As<BookingsApiException<ValidationProblemDetails>>();
        var errors = bookingsApiException.Result.Errors;
        errors.Should().ContainKey("hearingId");
        errors["hearingId"].Should().Contain("Please provide a valid hearingId");
    }
    
    [Test]
    public async Task should_get_validation_problem_when_participant_id_is_invalid()
    {
        var hearingId = Guid.NewGuid();
        var participantId = Guid.Empty;
        
        var client = GetBookingsApiClient();
        
        // act
        var act = async () => await client.RemoveParticipantFromHearingAsync(hearingId, participantId);
        
        // assert
        var exception = await act.Should().ThrowAsync<BookingsApiException>();
        exception.And.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        var bookingsApiException = exception.And.As<BookingsApiException<ValidationProblemDetails>>();
        var errors = bookingsApiException.Result.Errors;
        errors.Should().ContainKey("participantId");
        errors["participantId"].Should().Contain("Please provide a valid participantId");
    }
    
    [Test]
    public async Task should_get_not_found_when_hearing_does_not_exist()
    {
        // arrange
        var hearingId = Guid.NewGuid();
        var participantId = Guid.NewGuid();
        
        var client = GetBookingsApiClient();
        
        // act
        var act = async () => await client.RemoveParticipantFromHearingAsync(hearingId, participantId);
        
        // assert
        var exception = await act.Should().ThrowAsync<BookingsApiException>();
        exception.And.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        exception.And.Response.Should().Be($"\"Hearing {hearingId} does not exist\"");
    }
    
    [Test]
    public async Task should_get_not_found_when_participant_does_not_exist()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2();
        var participantId = Guid.NewGuid();
        var client = GetBookingsApiClient();
        
        // act
        var act = async () => await client.RemoveParticipantFromHearingAsync(hearing.Id, participantId);
        
        // assert
        var exception = await act.Should().ThrowAsync<BookingsApiException>();
        exception.And.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }
    
    [Test]
    public async Task should_remove_participant_from_hearing()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2();
        var participant = hearing.Participants.First(x=> x is Individual);
        var client = GetBookingsApiClient();
        
        // act
        await client.RemoveParticipantFromHearingAsync(hearing.Id, participant.Id);
        
        // assert
        var updatedHearing = await client.GetHearingDetailsByIdV2Async(hearing.Id);
        updatedHearing.Participants.Should().NotContain(x => x.Id == participant.Id);
    }
}