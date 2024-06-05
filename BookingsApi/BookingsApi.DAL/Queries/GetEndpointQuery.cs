namespace BookingsApi.DAL.Queries
{
    public class GetEndpointQuery : IQuery
    {
        public string Sip { get; set; }

        public GetEndpointQuery(string sipAddress)
        {
            Sip = sipAddress;
        }
    }

    public class GetEndpointQueryHandler : IQueryHandler<GetEndpointQuery, Endpoint>
    {
        private readonly BookingsDbContext _context;

        public GetEndpointQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<Endpoint> Handle(GetEndpointQuery parameter)
        {
            var query = _context.VideoHearings
                .Include(x => x.Endpoints)
                .ThenInclude(x => x.EndpointParticipants)
                .ThenInclude(x => x.Participant)
                .ThenInclude(x => x.Person)
                .Where(vh => vh.Endpoints.Any(e => e.Sip == parameter.Sip));
            
            return await query.SelectMany(x => x.Endpoints).SingleOrDefaultAsync(e => e.Sip == parameter.Sip);
        }
    }
}