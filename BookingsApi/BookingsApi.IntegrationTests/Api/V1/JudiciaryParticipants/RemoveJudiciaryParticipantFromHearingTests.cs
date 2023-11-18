using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Validations;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;

namespace BookingsApi.IntegrationTests.Api.V1.JudiciaryParticipants;

public class RemoveJudiciaryParticipantFromHearingTests : ApiTest
{
    [Test]
    public async Task Should_return_not_found_when_hearing_does_not_exist()
    {
        // Arrange
        var hearingId = Guid.NewGuid();
        var personalCode = Guid.NewGuid().ToString();

        // Act
        using var client = Application.CreateClient();
        var result =
            await client.DeleteAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.RemoveJudiciaryParticipantFromHearing(hearingId,
                    personalCode));
            
        // Assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var response = await ApiClientResponse.GetResponses<string>(result.Content);
        response.Should().Be($"Hearing {hearingId} does not exist");
    }

    [Test]
    public async Task should_return_not_found_when_judiciary_participant_does_not_exist()
    {
        // Arrange
        var seededHearing = await Hooks.SeedVideoHearing();
        var hearingId = seededHearing.Id;
        var personalCode = Guid.NewGuid().ToString();

        // Act
        using var client = Application.CreateClient();
        var result =
            await client.DeleteAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.RemoveJudiciaryParticipantFromHearing(hearingId,
                    personalCode));
            
        // Assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var response = await ApiClientResponse.GetResponses<string>(result.Content);
        response.Should().Be(DomainRuleErrorMessages.JudiciaryParticipantNotFound);
    }

    [Test]
    public async Task should_remove_judiciary_participant()
    {
        // Arrange
        var seededHearing = await Hooks.SeedVideoHearingV2(options =>
        {
            options.AddJudge = true;
            options.AddStaffMember = true;
        }, status:BookingStatus.Created);
        var hearingId = seededHearing.Id;
        var judiciaryParticipant = seededHearing.JudiciaryParticipants[0];

        // Act
        using var client = Application.CreateClient();
        var result =
            await client.DeleteAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.RemoveJudiciaryParticipantFromHearing(hearingId,
                    judiciaryParticipant.JudiciaryPerson.PersonalCode));

        // Assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);

        await using var db = new BookingsDbContext(BookingsDbContextOptions);
        var hearingFromDb = await db.VideoHearings.Include(x => x.JudiciaryParticipants)
            .ThenInclude(x => x.JudiciaryPerson).FirstAsync(x => x.Id == hearingId);


        hearingFromDb.GetJudiciaryParticipants()
            .Any(p => p.JudiciaryPerson.PersonalCode == judiciaryParticipant.JudiciaryPerson.PersonalCode)
            .Should()
            .BeFalse();
        
        var serviceBusStub = Application.Services
            .GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
        var message = serviceBusStub!
            .ReadMessageFromQueue();
        
        message.IntegrationEvent
            .Should()
            .BeEquivalentTo(new ParticipantRemovedIntegrationEvent(hearingId, judiciaryParticipant.Id));
    }
}