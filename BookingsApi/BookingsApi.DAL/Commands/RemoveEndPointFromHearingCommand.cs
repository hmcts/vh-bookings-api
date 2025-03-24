namespace BookingsApi.DAL.Commands
{
    public class RemoveEndPointFromHearingCommand : ICommand
    {
        public RemoveEndPointFromHearingCommand(Guid hearingId, Guid endpointId)
        {
            HearingId = hearingId;
            EndpointId = endpointId;
        }

        public Guid HearingId { get; }
        public Guid EndpointId { get; }
    }

    public class RemoveEndPointFromHearingCommandHandler : ICommandHandler<RemoveEndPointFromHearingCommand>
    {
        private readonly BookingsDbContext _context;

        public RemoveEndPointFromHearingCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(RemoveEndPointFromHearingCommand command)
        {
            var hearing = await _context.VideoHearings
                .Include(h => h.Participants)
                    .ThenInclude(p => p.Screening)
                    .ThenInclude(s => s.ScreeningEntities)
                .Include(h => h.Participants)
                    .ThenInclude(x => x.Person)
                .Include(x=> x.Participants)
                    .ThenInclude(x=> x.InterpreterLanguage)
                .Include(h => h.Endpoints)
                    .ThenInclude(e => e.Screening)
                    .ThenInclude(s => s.ScreeningEntities)
                .Include(x => x.Endpoints)
                    .ThenInclude(x => x.ParticipantsLinked)
                    .ThenInclude(p => p.Person)
                .Include(x => x.Endpoints)
                    .ThenInclude(x=> x.InterpreterLanguage)
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);

            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }

            var endpoint = hearing.Endpoints.FirstOrDefault(e => e.Id == command.EndpointId);
            if (endpoint == null)
            {
                throw new EndPointNotFoundException(command.EndpointId);
            }

            hearing.RemoveEndpoint(endpoint);
            await _context.SaveChangesAsync();
        }
    }
}
