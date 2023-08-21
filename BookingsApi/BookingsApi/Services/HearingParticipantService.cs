namespace BookingsApi.Services;

public interface IHearingParticipantService
{
    public Task PublishEventForNewParticipantsAsync(Hearing hearing, IEnumerable<NewParticipant> newParticipants);
    public Task PublishEventForUpdateParticipantsAsync(
        Hearing hearing,
        List<ExistingParticipantDetails> existingParticipants,
        List<NewParticipant> newParticipants,
        List<Guid> removedParticipantIds,
        List<LinkedParticipantDto> linkedParticipants);

}

public class HearingParticipantService : IHearingParticipantService
{
    private readonly IEventPublisher _eventPublisher;
    private readonly ICommandHandler _commandHandler;

    public HearingParticipantService(ICommandHandler commandHandler, IEventPublisher eventPublisher)
    {
        _commandHandler = commandHandler;
        _eventPublisher = eventPublisher;
    }

    public async Task PublishEventForNewParticipantsAsync(Hearing hearing, IEnumerable<NewParticipant> newParticipants)
    {
        var participants = hearing.GetParticipants()
                    .Where(x => newParticipants.Any(y => y.Person.ContactEmail == x.Person.ContactEmail)).ToList();
        if (participants.Any())
        {
            if(hearing.Status == BookingStatus.Created) 
            {
                await _eventPublisher.PublishAsync(new ParticipantsAddedIntegrationEvent(hearing, participants));
            }
            else if (participants.Exists(x => x.HearingRole.UserRole.Name == "Judge"))
            {
                await _eventPublisher.PublishAsync(new HearingIsReadyForVideoIntegrationEvent(hearing, participants));
            }
            else
            {
                await _eventPublisher.PublishAsync(new CreateAndNotifyUserIntegrationEvent(hearing, participants));
                await _eventPublisher.PublishAsync(new HearingNotificationIntegrationEvent(hearing, participants));
            }
        }
    }
    
    public async Task PublishEventForUpdateParticipantsAsync(Hearing hearing, List<ExistingParticipantDetails> existingParticipants, List<NewParticipant> newParticipants, List<Guid> removedParticipantIds, List<LinkedParticipantDto> linkedParticipants)
    {
        var eventNewParticipants = hearing
            .GetParticipants()
            .Where(x => newParticipants.Exists(y => y.Person.ContactEmail == x.Person.ContactEmail))
            .ToList();
        var eventExistingParticipants = hearing
            .GetParticipants()
            .Where(x => existingParticipants.Exists(y => y.ParticipantId == x.Id))
            .ToList();
        
        if (eventNewParticipants.Any() || removedParticipantIds.Any())
        {
            if (hearing.Status == BookingStatus.Created)
            {
                var eventLinkedParticipants = new List<Infrastructure.Services.Dtos.LinkedParticipantDto>();
            
                foreach (var linkedParticipant in linkedParticipants)
                {
                    var primaryLinkedParticipant = hearing.GetParticipants().SingleOrDefault(x => x.Person.ContactEmail == linkedParticipant.ParticipantContactEmail);
                    var secondaryLinkedParticipant = hearing.GetParticipants().SingleOrDefault(x => x.Person.ContactEmail == linkedParticipant.LinkedParticipantContactEmail);
                    if (primaryLinkedParticipant != null && secondaryLinkedParticipant != null)
                    {
                        eventLinkedParticipants.Add(new Infrastructure.Services.Dtos.LinkedParticipantDto
                        {
                            LinkedId = secondaryLinkedParticipant.Id,
                            ParticipantId = primaryLinkedParticipant.Id,
                            Type = linkedParticipant.Type
                        });
                    }
                }
            
                var hearingParticipantsUpdatedIntegrationEvent = new HearingParticipantsUpdatedIntegrationEvent(hearing, eventExistingParticipants, eventNewParticipants, removedParticipantIds, eventLinkedParticipants);
                await _eventPublisher.PublishAsync(hearingParticipantsUpdatedIntegrationEvent);
            }
            else if (eventNewParticipants.Exists(x => x.HearingRole.UserRole.Name == "Judge"))
            {
                await UpdateHearingStatusAsync(hearing.Id, BookingStatus.Created, "System", string.Empty);
                await _eventPublisher.PublishAsync(new HearingIsReadyForVideoIntegrationEvent(hearing, eventNewParticipants));
            }
            else
            {
                if (eventNewParticipants.Any())
                {
                    await _eventPublisher.PublishAsync(new CreateAndNotifyUserIntegrationEvent(hearing, eventNewParticipants));
                    await _eventPublisher.PublishAsync(new HearingNotificationIntegrationEvent(hearing, eventNewParticipants));   
                }
            }
        }
        else
            foreach (var participant in eventExistingParticipants)
            {
                if (participant is Judge)
                {
                    var judgeRequest =
                        existingParticipants.First(e => e.ParticipantId == participant.Id);
                    participant.Person.ContactEmail = judgeRequest.Person.ContactEmail;
                    participant.Person.TelephoneNumber = judgeRequest.Person.TelephoneNumber;
                    await _eventPublisher.PublishAsync(new JudgeUpdatedIntegrationEvent(hearing, participant));  
                }
                else
                    await _eventPublisher.PublishAsync(new ParticipantUpdatedIntegrationEvent(hearing.Id, participant));  
            }
    }
    
    private async Task UpdateHearingStatusAsync(Guid hearingId, BookingStatus bookingStatus, string updatedBy, string cancelReason)
    {
        var command = new UpdateHearingStatusCommand(hearingId, bookingStatus, updatedBy, cancelReason);
        await _commandHandler.Handle(command);
    }
}