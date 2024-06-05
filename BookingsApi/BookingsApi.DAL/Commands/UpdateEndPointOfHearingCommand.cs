using BookingsApi.Domain.Dtos;

namespace BookingsApi.DAL.Commands
{
    public class UpdateEndPointOfHearingCommand : ICommand
    {
        public UpdateEndPointOfHearingCommand(Guid hearingId, Guid endpointId, string displayName, List<NewEndpointParticipantDto> endpointParticipants)
        {
            HearingId = hearingId;
            EndpointId = endpointId;
            DisplayName = displayName;
            EndpointParticipants = endpointParticipants ?? new List<NewEndpointParticipantDto>();
        }

        public Guid HearingId { get; }
        public Guid EndpointId { get;  }
        public string DisplayName { get;  }
        public List<NewEndpointParticipantDto> EndpointParticipants { get; } 
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
            
            foreach (var endpointParticipant in command.EndpointParticipants)
            {
                var participant = hearing.GetParticipants().SingleOrDefault(x => x.Person.ContactEmail == endpointParticipant.ContactEmail);
                if (participant != null)
                    endpoint.LinkParticipantToEndpoint(participant, endpointParticipant.Type);
                
            }
            
            var participantToRemove = endpoint
                .GetLinkedParticipants()
                .Where(p => !command.EndpointParticipants.Exists(ep => ep.ContactEmail == p.Person.ContactEmail))
                .ToList();
            
            participantToRemove.ForEach(p => endpoint.RemoveLinkedParticipant(p));
          
            await _context.SaveChangesAsync();
        }
    }
}
