using BookingsApi.Contract.Interfaces.Requests;
using BookingsApi.Validations.Common;
using FluentValidation.Results;

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
    public Task PublishEventForNewJudiciaryParticipantsAsync(Hearing hearing, IEnumerable<NewJudiciaryParticipant> newJudiciaryParticipants);
    public Task PublishEventForUpdateJudiciaryParticipantAsync(Hearing hearing, UpdatedJudiciaryParticipant updatedJudiciaryParticipant);
    
    public Task<ValidationResult> ValidateRepresentativeInformationAsync(IRepresentativeInfoRequest request);
    public Task<Participant> UpdateParticipantAndPublishEventAsync(Hearing videoHearing, UpdateParticipantCommand updateParticipantCommand);
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
            if(hearing.Status == BookingStatus.Created || hearing.Status == BookingStatus.ConfirmedWithoutJudge) 
                await _eventPublisher.PublishAsync(new ParticipantsAddedIntegrationEvent(hearing, participants));
            else if (participants.Exists(x => x.HearingRole.UserRole.Name == "Judge"))
                await _eventPublisher.PublishAsync(new HearingIsReadyForVideoIntegrationEvent(hearing, participants));
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
            await ProcessParticipantListChange(hearing, removedParticipantIds, linkedParticipants, eventExistingParticipants, eventNewParticipants);
        }
        else
        {
            await PublishExistingParticipantUpdatedEvent(hearing, existingParticipants, eventExistingParticipants);
        }
    }
    
    public async Task PublishEventForNewJudiciaryParticipantsAsync(Hearing hearing, IEnumerable<NewJudiciaryParticipant> newJudiciaryParticipants)
    {
        var participants = hearing.GetJudiciaryParticipants()
            .Where(x => newJudiciaryParticipants.Any(y => y.PersonalCode == x.JudiciaryPerson.PersonalCode))
            .ToList();
        
        await _eventPublisher.PublishAsync(new ParticipantsAddedIntegrationEvent(hearing, participants));
    }
    public async Task PublishEventForUpdateJudiciaryParticipantAsync(Hearing hearing, UpdatedJudiciaryParticipant updatedJudiciaryParticipant)
    {
        var participant = hearing.GetJudiciaryParticipants()
            .FirstOrDefault(x => x.JudiciaryPerson.PersonalCode == updatedJudiciaryParticipant.PersonalCode);
        
        await _eventPublisher.PublishAsync(new ParticipantUpdatedIntegrationEvent(hearing.Id, participant));
    }

    public async Task<ValidationResult> ValidateRepresentativeInformationAsync(IRepresentativeInfoRequest request)
    {
        var test = new RepresentativeValidation();
        await test.ValidateAsync(request);
        return await new RepresentativeValidation().ValidateAsync(request);
    }

    public async Task<Participant> UpdateParticipantAndPublishEventAsync(Hearing videoHearing, UpdateParticipantCommand updateParticipantCommand)
    {
        await _commandHandler.Handle(updateParticipantCommand);

        var updatedParticipant = updateParticipantCommand.UpdatedParticipant;
        
        // ONLY publish this event when Hearing is set for ready for video
        if (videoHearing.Status == BookingStatus.Created)
        { 
            await _eventPublisher.PublishAsync(new ParticipantUpdatedIntegrationEvent(updateParticipantCommand.HearingId, updatedParticipant));
        }

        return updatedParticipant;
    }

    private async Task ProcessParticipantListChange(Hearing hearing, List<Guid> removedParticipantIds, List<LinkedParticipantDto> linkedParticipants,
        List<Participant> eventExistingParticipants, List<Participant> eventNewParticipants)
    {
        if (hearing.Status == BookingStatus.Created || hearing.Status == BookingStatus.ConfirmedWithoutJudge)
            await PublishHearingParticipantListUpdatedEvent(hearing, 
                removedParticipantIds, 
                linkedParticipants, 
                eventExistingParticipants, 
                eventNewParticipants);
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

    private async Task PublishHearingParticipantListUpdatedEvent(Hearing hearing, List<Guid> removedParticipantIds,
        List<LinkedParticipantDto> linkedParticipants, List<Participant> eventExistingParticipants, List<Participant> eventNewParticipants)
    {
        var eventLinkedParticipants = new List<Infrastructure.Services.Dtos.LinkedParticipantDto>();

        foreach (var linkedParticipant in linkedParticipants)
        {
            var primaryLinkedParticipant = hearing.GetParticipants()
                .SingleOrDefault(x => x.Person.ContactEmail == linkedParticipant.ParticipantContactEmail);
            var secondaryLinkedParticipant = hearing.GetParticipants()
                .SingleOrDefault(x => x.Person.ContactEmail == linkedParticipant.LinkedParticipantContactEmail);
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

        var hearingParticipantsUpdatedIntegrationEvent = new HearingParticipantsUpdatedIntegrationEvent(hearing,
            eventExistingParticipants, eventNewParticipants, removedParticipantIds, eventLinkedParticipants);
        await _eventPublisher.PublishAsync(hearingParticipantsUpdatedIntegrationEvent);
    }

    private async Task PublishExistingParticipantUpdatedEvent(Hearing hearing, List<ExistingParticipantDetails> existingParticipants,
        List<Participant> eventExistingParticipants)
    {
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