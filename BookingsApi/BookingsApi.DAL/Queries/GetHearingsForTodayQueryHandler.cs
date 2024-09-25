namespace BookingsApi.DAL.Queries
{
    public class GetHearingsForTodayQuery : IQuery
    {
        public GetHearingsForTodayQuery(IEnumerable<string> hearingVenueNames = null)
        {
            HearingVenueNames = hearingVenueNames?.ToList();
        }
        
        public List<string> HearingVenueNames { get; }
    }
    
    public class GetHearingsForTodayQueryHandler : IQueryHandler<GetHearingsForTodayQuery, List<VideoHearing>>
    {
        private readonly BookingsDbContext _context;

        public GetHearingsForTodayQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<VideoHearing>> Handle(GetHearingsForTodayQuery query)
        {
            var hearingQuery = _context.VideoHearings
                .Include(h => h.Allocations).ThenInclude(a => a.JusticeUser).ThenInclude(ju => ju.JusticeUserRoles).ThenInclude(jur => jur.UserRole)
                .Include(x => x.Participants).ThenInclude(x => x.Person).ThenInclude(x => x.Organisation)
                .Include(x => x.Participants).ThenInclude(x => x.CaseRole)
                .Include(x => x.Participants).ThenInclude(x => x.HearingRole).ThenInclude(x => x.UserRole)
                .Include(x => x.HearingCases).ThenInclude(x => x.Case)
                .Include(x => x.Endpoints)
                .Include(x => x.CaseType)
                .Include(x => x.HearingVenue)
                .Where(x => x.ScheduledDateTime.Date == DateTime.Today.Date)
                .AsQueryable();

            if (query.HearingVenueNames != null && query.HearingVenueNames.Count != 0)
                hearingQuery = hearingQuery
                    .Where(x => query.HearingVenueNames.Contains(x.HearingVenue.Name))
                    .OrderBy(x => x.HearingVenue.Name).ThenBy(x => x.ScheduledDateTime);
                
            return await hearingQuery
                .Where(x => x.Status == BookingStatus.Created || x.Status == BookingStatus.Booked)
                .ToListAsync();
        }
    }
}