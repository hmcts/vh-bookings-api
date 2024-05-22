using BookingsApi.Domain.Participants;

namespace BookingsApi.DAL.Commands
{
    public class UpdateEndPointOfHearingCommand : ICommand
    {
        public UpdateEndPointOfHearingCommand(Guid hearingId, Guid endpointId, string displayName, params (Participant, LinkedParticipantType)[] endpointParticipants)
        {
            HearingId = hearingId;
            EndpointId = endpointId;
            DisplayName = displayName;
            EndpointParticipants = endpointParticipants ?? Array.Empty<(Participant, LinkedParticipantType)>();
        }

        public Guid HearingId { get; }
        public Guid EndpointId { get;  }
        public string DisplayName { get;  }
        public (Participant, LinkedParticipantType)[] EndpointParticipants { get; }
}

    public class UpdateEndPointOfHearingCommandHandler : ICommandHandler<UpdateEndPointOfHearingCommand>
    {
        private readonly BookingsDbContext _context;

        public UpdateEndPointOfHearingCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateEndPointOfHearingCommand command)
        {
            var hearing = await _context.VideoHearings
                .Include(h => h.Participants).ThenInclude(x => x.Person)
                .Include(h => h.Endpoints).ThenInclude(e => e.EndpointParticipants)
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);

            if (hearing == null)
                throw new HearingNotFoundException(command.HearingId);

            var endpoint = hearing.Endpoints.SingleOrDefault(e => e.Id == command.EndpointId);
            if (endpoint == null)
                throw new EndPointNotFoundException(command.EndpointId);

            if (!string.IsNullOrWhiteSpace(endpoint.DisplayName)) 
                endpoint.UpdateDisplayName(command.DisplayName);
            
            foreach ((Participant participant, LinkedParticipantType type) endpointParticipant in command.EndpointParticipants)
            {
                var participantToAddToEndpoint = hearing.GetParticipants().Single(p => p.Id == endpointParticipant.participant.Id);
                endpoint.LinkParticipantToEndpoint(participantToAddToEndpoint, endpointParticipant.type);
            }
            var participantToRemove = (from endpointParticipant in endpoint.EndpointParticipants where command.EndpointParticipants.All(x => x.Item1.Id != endpointParticipant.ParticipantId) select endpointParticipant.Participant).ToList();
            participantToRemove.ForEach(p => endpoint.RemoveLinkedParticipant(p));
          
            await _context.SaveChangesAsync();
        }
    }
}
