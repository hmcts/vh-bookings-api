namespace BookingsApi.DAL.Queries
{
    public class GetHearingVenuesQuery : IQuery
    {
        public bool ExcludeExpiredVenue { get;}
        public GetHearingVenuesQuery(bool excludeExpiredVenue = false)
        {
            ExcludeExpiredVenue = excludeExpiredVenue;
        }
    }
    
    public class GetHearingVenuesQueryHandler : IQueryHandler<GetHearingVenuesQuery, List<HearingVenue>>
    {
        private readonly BookingsDbContext _context;

        public GetHearingVenuesQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<HearingVenue>> Handle(GetHearingVenuesQuery query)
        {
            var venues = _context.Venues.AsQueryable();
            
            if(query.ExcludeExpiredVenue)
                venues = venues.Where(venue => venue.ExpirationDate == null || venue.ExpirationDate.Value.Date > DateTime.Today);

            venues = venues.OrderBy(x => x.Name);
            
            return await venues.ToListAsync();
        }
    }
}