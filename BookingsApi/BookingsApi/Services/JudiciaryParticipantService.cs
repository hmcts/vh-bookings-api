namespace BookingsApi.Services
{
    public interface IJudiciaryParticipantService
    {
        Task<IList<JudiciaryParticipant>> AddJudiciaryParticipants(List<NewJudiciaryParticipant> newJudiciaryParticipants, Guid hearingId);
        Task<JudiciaryParticipant> UpdateJudiciaryParticipant(UpdatedJudiciaryParticipant judiciaryParticipant, Guid hearingId);
        Task RemoveJudiciaryParticipant(string personalCode, Guid hearingId);
        Task<JudiciaryParticipant> ReassignJudiciaryJudge(Guid hearingId, NewJudiciaryJudge newJudiciaryJudge);
    }
    
    public class JudiciaryParticipantService : IJudiciaryParticipantService
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;
        private readonly IHearingParticipantService _hearingParticipantService;
        private readonly IEventPublisher _eventPublisher;

        public JudiciaryParticipantService(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IHearingParticipantService hearingParticipantService, IEventPublisher eventPublisher)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _hearingParticipantService = hearingParticipantService;
            _eventPublisher = eventPublisher;
        }
        
        public async Task<IList<JudiciaryParticipant>> AddJudiciaryParticipants(List<NewJudiciaryParticipant> newJudiciaryParticipants, Guid hearingId)
        {
            var command = new AddJudiciaryParticipantsToHearingCommand(hearingId, newJudiciaryParticipants);
            await _commandHandler.Handle(command);
            
            var updatedHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
            await PublishEventsForJudiciaryParticipantsAdded(updatedHearing, newJudiciaryParticipants);
            
            var addedParticipants = updatedHearing.JudiciaryParticipants
                .Where(x => newJudiciaryParticipants.Select(p => p.PersonalCode).Contains(x.JudiciaryPerson.PersonalCode))
                .ToList();

            return addedParticipants;
        }

        public async Task<JudiciaryParticipant> UpdateJudiciaryParticipant(UpdatedJudiciaryParticipant judiciaryParticipant, Guid hearingId)
        {
            var command = new UpdateJudiciaryParticipantCommand(hearingId, judiciaryParticipant);
            await _commandHandler.Handle(command);
            
            var hearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
            await _hearingParticipantService.PublishEventForUpdateJudiciaryParticipantAsync(hearing, judiciaryParticipant);
            
            var updatedParticipant = hearing.JudiciaryParticipants
                .FirstOrDefault(x => x.JudiciaryPerson.PersonalCode == judiciaryParticipant.PersonalCode);

            return updatedParticipant;
        }

        public async Task RemoveJudiciaryParticipant(string personalCode, Guid hearingId)
        {
            var command = new RemoveJudiciaryParticipantFromHearingCommand(hearingId, personalCode);
            await _commandHandler.Handle(command);

            // ONLY publish this event when Hearing is set for ready for video
            var videoHearing =
                await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
            await PublishEventForJudiciaryParticipantRemoved(videoHearing, command.RemovedParticipantId.Value);
        }
        
        public async Task<JudiciaryParticipant> ReassignJudiciaryJudge(Guid hearingId, NewJudiciaryJudge newJudiciaryJudge)
        {
            var hearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));

            var oldJudge = (JudiciaryParticipant)hearing.GetJudge();
            
            var command = new ReassignJudiciaryJudgeCommand(hearingId, newJudiciaryJudge);
            await _commandHandler.Handle(command);
            
            hearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
            
            var newJudge = (JudiciaryParticipant)hearing.GetJudge();

            await PublishEventsForJudiciaryJudgeReassigned(hearing, oldJudge?.Id, newJudge);

            return newJudge;
        }

        private async Task PublishEventsForJudiciaryJudgeReassigned(Hearing hearing, Guid? oldJudgeId, JudiciaryParticipant newJudge)
        {
            if (oldJudgeId != null)
            {
                await PublishEventForJudiciaryParticipantRemoved(hearing, oldJudgeId.Value);
            }
            
            await PublishEventsForJudiciaryParticipantsAdded(hearing, new List<NewJudiciaryParticipant>
            {
                new()
                {
                    DisplayName = newJudge.DisplayName,
                    PersonalCode = newJudge.JudiciaryPerson.PersonalCode,
                    HearingRoleCode = newJudge.HearingRoleCode
                }
            });
        }
        
        private async Task PublishEventForJudiciaryParticipantRemoved(Hearing hearing, Guid removedJudiciaryParticipantId)
        {
            if (hearing.Status is BookingStatus.Created or BookingStatus.ConfirmedWithoutJudge)
            {
                await _eventPublisher.PublishAsync(
                    new ParticipantRemovedIntegrationEvent(hearing.Id, removedJudiciaryParticipantId));
            }
        }

        private async Task PublishEventsForJudiciaryParticipantsAdded(Hearing hearing, IEnumerable<NewJudiciaryParticipant> participants)
        {
            await _hearingParticipantService.PublishEventForNewJudiciaryParticipantsAsync(hearing, participants);
        }
    }
}
