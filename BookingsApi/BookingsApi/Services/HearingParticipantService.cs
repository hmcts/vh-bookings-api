using BookingsApi.Contract.Interfaces.Requests;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Infrastructure.Services.AsynchronousProcesses;
using BookingsApi.Mappings.V1;
using BookingsApi.Mappings.V2;
using BookingsApi.Validations.Common;
using FluentValidation.Results;

namespace BookingsApi.Services;

public interface IHearingParticipantService
{
    public Task PublishEventForNewParticipantsAsync(VideoHearing hearing, IEnumerable<NewParticipant> newParticipants);
    public Task PublishEventForUpdateParticipantsAsync(
        VideoHearing hearing,
        List<ExistingParticipantDetails> existingParticipants,
        List<NewParticipant> newParticipants,
        List<Guid> removedParticipantIds,
        List<LinkedParticipantDto> linkedParticipants, 
        bool sendNotification = true);
    public Task PublishEventForNewJudiciaryParticipantsAsync(Hearing hearing, IEnumerable<NewJudiciaryParticipant> newJudiciaryParticipants, bool sendNotification = true);
    public Task PublishEventForUpdateJudiciaryParticipantAsync(Hearing hearing, UpdatedJudiciaryParticipant updatedJudiciaryParticipant);
    
    public Task<ValidationResult> ValidateRepresentativeInformationAsync(IRepresentativeInfoRequest request);
    public Task<Participant> UpdateParticipantAndPublishEventAsync(Hearing videoHearing, UpdateParticipantCommand updateParticipantCommand);
    Task<VideoHearing> UpdateParticipants(UpdateHearingParticipantsRequest request, VideoHearing hearing, bool sendNotification = true);
    Task<VideoHearing> UpdateParticipantsV2(UpdateHearingParticipantsRequestV2 request, VideoHearing hearing, List<HearingRole> hearingRoles, bool sendNotification = true);
}

public class HearingParticipantService : IHearingParticipantService
{
    private readonly IEventPublisher _eventPublisher;
    private readonly IQueryHandler _queryHandler;
    private readonly ICommandHandler _commandHandler;
    private readonly IParticipantAddedToHearingAsynchronousProcess _participantAddedToHearingAsynchronousProcess;
    private readonly INewJudiciaryAddedAsynchronousProcesses _newJudiciaryAddedAsynchronousProcesses;
    public HearingParticipantService(ICommandHandler commandHandler, IEventPublisher eventPublisher, 
        IParticipantAddedToHearingAsynchronousProcess participantAddedToHearingAsynchronousProcess, INewJudiciaryAddedAsynchronousProcesses newJudiciaryAddedAsynchronousProcesses,
        IQueryHandler queryHandler)
    {
        _commandHandler = commandHandler;
        _eventPublisher = eventPublisher;
        _participantAddedToHearingAsynchronousProcess = participantAddedToHearingAsynchronousProcess;
        _newJudiciaryAddedAsynchronousProcesses = newJudiciaryAddedAsynchronousProcesses;
        _queryHandler = queryHandler;
    }

    public async Task PublishEventForNewParticipantsAsync(VideoHearing hearing, IEnumerable<NewParticipant> newParticipants)
    {
        var participants = hearing.GetParticipants()
                    .Where(x => newParticipants.Any(y => y.Person.ContactEmail == x.Person.ContactEmail)).ToList();
        if (participants.Any())
        {
            // Raising the below event here instead of in the async process to avoid the async process adding a duplicate participant to the conference
            // as the UpdateHearingParticipants (also inlcudes new participants) has a separate process to add participants
            await _eventPublisher.PublishAsync(new ParticipantsAddedIntegrationEvent(hearing, participants));
            await _participantAddedToHearingAsynchronousProcess.Start(hearing);
        }
    }

    public async Task PublishEventForUpdateParticipantsAsync(VideoHearing hearing, List<ExistingParticipantDetails> existingParticipants, List<NewParticipant> newParticipants, 
        List<Guid> removedParticipantIds, List<LinkedParticipantDto> linkedParticipants, bool sendNotification = true)
    {
        var eventNewParticipants = hearing
            .GetParticipants()
            .Where(x => newParticipants.Exists(y => string.Equals(y.Person.ContactEmail, x.Person.ContactEmail, StringComparison.CurrentCultureIgnoreCase)))
            .ToList();
        var eventExistingParticipants = hearing
            .GetParticipants()
            .Where(x => existingParticipants.Exists(y => y.ParticipantId == x.Id))
            .ToList();
        
        if (eventNewParticipants.Any() || removedParticipantIds.Any())
        {
            await ProcessParticipantListChange(hearing, removedParticipantIds, linkedParticipants, eventExistingParticipants, eventNewParticipants, sendNotification);
        }
        else
        {
            await PublishExistingParticipantUpdatedEvent(hearing, existingParticipants, eventExistingParticipants, sendNotification);
        }
    }
    
    public async Task PublishEventForNewJudiciaryParticipantsAsync(Hearing hearing, IEnumerable<NewJudiciaryParticipant> newJudiciaryParticipants, bool sendNotification = true)
    {
        var participants = hearing.GetJudiciaryParticipants()
            .Where(x => newJudiciaryParticipants.Any(y => y.PersonalCode == x.JudiciaryPerson.PersonalCode))
            .ToList();

        switch (hearing.Status)
        {
            case BookingStatus.Created or BookingStatus.ConfirmedWithoutJudge:
                await _newJudiciaryAddedAsynchronousProcesses.Start((VideoHearing) hearing, participants, sendNotification);
                break;
            case BookingStatus.Booked or BookingStatus.BookedWithoutJudge when participants.Exists(p => p.HearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge):
                await _eventPublisher.PublishAsync(new HearingIsReadyForVideoIntegrationEvent(hearing, hearing.GetParticipants()));
                break;
        }
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
        await _eventPublisher.PublishAsync(new ParticipantUpdatedIntegrationEvent(updateParticipantCommand.HearingId, updatedParticipant));
        return updatedParticipant;
    }

    public async Task<VideoHearing> UpdateParticipants(UpdateHearingParticipantsRequest request, VideoHearing hearing, bool sendNotification = true)
    {
        var newParticipants = request.NewParticipants
                .Select(x => ParticipantRequestToNewParticipantMapper.Map(x, hearing.CaseType)).ToList();
        
        var existingParticipants = hearing.Participants
            .Where(x => request.ExistingParticipants.Select(ep => ep.ParticipantId).Contains(x.Id))
            .ToList();
        
        var existingParticipantDetails = new List<ExistingParticipantDetails>();

        foreach (var existingParticipantRequest in request.ExistingParticipants)
        {
            var updatedParticipantRequest = UpdateExistingParticipantDetailsFromRequest(existingParticipants, existingParticipantRequest);
            if (updatedParticipantRequest != null)
            {
                updatedParticipantRequest.Person.ContactEmail = existingParticipantRequest.ContactEmail ??  existingParticipants.First(ep => ep.Id == existingParticipantRequest.ParticipantId).Person.ContactEmail;
                existingParticipantDetails.Add(updatedParticipantRequest);
            }
        }

        var linkedParticipants =
            LinkedParticipantRequestToLinkedParticipantDtoMapper.MapToDto(request.LinkedParticipants);

        var command = new UpdateHearingParticipantsCommand(hearing.Id, existingParticipantDetails, newParticipants, request.RemovedParticipantIds, linkedParticipants);

        await _commandHandler.Handle(command);

        var getHearingByIdQuery = new GetHearingByIdQuery(hearing.Id);
        var updatedHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);
        await PublishEventForUpdateParticipantsAsync(updatedHearing, existingParticipantDetails, newParticipants, request.RemovedParticipantIds, linkedParticipants, sendNotification);

        return updatedHearing;
    }

    public async Task<VideoHearing> UpdateParticipantsV2(UpdateHearingParticipantsRequestV2 request, VideoHearing hearing, List<HearingRole> hearingRoles, bool sendNotification = true)
    {
        var newParticipants = request.NewParticipants
            .Select(x => ParticipantRequestV2ToNewParticipantMapper.Map(x, hearingRoles)).ToList();

        var existingParticipants = hearing.Participants
            .Where(x => request.ExistingParticipants.Select(ep => ep.ParticipantId).Contains(x.Id)).ToList();
        
        var existingParticipantDetails = new List<ExistingParticipantDetails>();

        foreach (var existingParticipantRequest in request.ExistingParticipants)
        {
            var updatedParticipantRequest = UpdateExistingParticipantDetailsFromRequest(existingParticipants, existingParticipantRequest);
            if (updatedParticipantRequest != null)
            {
                updatedParticipantRequest.Person.ContactEmail = existingParticipants.First(ep => ep.Id == existingParticipantRequest.ParticipantId).Person.ContactEmail;
                existingParticipantDetails.Add(updatedParticipantRequest);
            }
        }
        
        var linkedParticipants =
            LinkedParticipantRequestV2ToLinkedParticipantDtoMapper.MapToDto(request.LinkedParticipants);

        var command = new UpdateHearingParticipantsCommand(hearing.Id, existingParticipantDetails, newParticipants, request.RemovedParticipantIds, linkedParticipants);

        await _commandHandler.Handle(command);

        var getHearingByIdQuery = new GetHearingByIdQuery(hearing.Id);
        var updatedHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);
        await PublishEventForUpdateParticipantsAsync(updatedHearing, existingParticipantDetails, newParticipants, request.RemovedParticipantIds, linkedParticipants, sendNotification);

        return updatedHearing;
    }
    
    private async Task ProcessParticipantListChange(VideoHearing hearing, List<Guid> removedParticipantIds, List<LinkedParticipantDto> linkedParticipants,
        List<Participant> eventExistingParticipants, List<Participant> eventNewParticipants, bool sendNotification = true)
    {
        await PublishHearingParticipantListUpdatedEvent(hearing, 
            removedParticipantIds, 
            linkedParticipants, 
            eventExistingParticipants, 
            eventNewParticipants);

        
        if (eventNewParticipants.Exists(x => x.HearingRole.UserRole.Name == "Judge") && hearing.Status != BookingStatus.Created)
            await UpdateHearingStatusAsync(hearing.Id, BookingStatus.Created, "System", string.Empty);

        if (sendNotification)
        {
            await _participantAddedToHearingAsynchronousProcess.Start(hearing);
        }
    }

    private async Task PublishHearingParticipantListUpdatedEvent(Hearing hearing, List<Guid> removedParticipantIds,
        List<LinkedParticipantDto> linkedParticipants, List<Participant> eventExistingParticipants, List<Participant> eventNewParticipants, bool sendNotification = true)
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
        List<Participant> eventExistingParticipants, bool sendNotification = true)
    {
        foreach (var participant in eventExistingParticipants)
        {
            if (participant is Judge)
            {
                var judgeRequest =
                    existingParticipants.First(e => e.ParticipantId == participant.Id);
                participant.Person.ContactEmail = judgeRequest.Person.ContactEmail;
                participant.Person.TelephoneNumber = judgeRequest.Person.TelephoneNumber;
                await _eventPublisher.PublishAsync(new JudgeUpdatedIntegrationEvent(hearing, participant, sendNotification));
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
    
    private static ExistingParticipantDetails UpdateExistingParticipantDetailsFromRequest<T>(List<Participant> existingParticipants, T existingParticipantRequest) where T : IUpdateParticipantRequest
    {
        var existingParticipant = existingParticipants.SingleOrDefault(ep => ep.Id == existingParticipantRequest.ParticipantId);
        var isNotBeingUpdated = !CheckParticipantIsBeingUpdated(existingParticipant, existingParticipantRequest);
            
        if (existingParticipant == null || isNotBeingUpdated)
            return null;
            
        return new ExistingParticipantDetails
        {
            DisplayName = existingParticipantRequest.DisplayName,
            OrganisationName = existingParticipantRequest.OrganisationName,
            ParticipantId = existingParticipantRequest.ParticipantId,
            Person = existingParticipant.Person,
            RepresentativeInformation = new RepresentativeInformation { Representee = existingParticipantRequest.Representee },
            TelephoneNumber = existingParticipantRequest.TelephoneNumber,
            Title = existingParticipantRequest.Title
        };
    }
    
    //Check the only properties that can be updated on an existing Participant have been edited, otherwise should be excluded from updates to the video-api
    private static bool CheckParticipantIsBeingUpdated(Participant existingParticipant, IUpdateParticipantRequest requestChanges)
    {
        return existingParticipant?.Person?.Title != requestChanges.Title ||
               existingParticipant?.Person?.Organisation?.Name != requestChanges.OrganisationName ||
               existingParticipant?.Person?.TelephoneNumber != requestChanges.TelephoneNumber ||
               existingParticipant?.DisplayName != requestChanges.DisplayName;
    }
        
}