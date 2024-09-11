namespace BookingsApi.DAL.Queries;

public class GetHearingsForTodayQueryAllocatedToQuery(List<Guid> allocatedTo, bool? unallocated) : IQuery
{
    public List<Guid> AllocatedTo { get; } = allocatedTo;
    public bool? Unallocated { get; } = unallocated;
}

public class GetHearingsForTodayQueryAllocatedToQueryHandler : IQueryHandler<GetHearingsForTodayQueryAllocatedToQuery, List<VideoHearing>>
{
    private readonly BookingsDbContext _context;

    public GetHearingsForTodayQueryAllocatedToQueryHandler(BookingsDbContext context) => _context = context;

    public async Task<List<VideoHearing>> Handle(GetHearingsForTodayQueryAllocatedToQuery query)
    {
        var hearingQuery = _context.VideoHearings
            .Include(h => h.Allocations).ThenInclude(a => a.JusticeUser).ThenInclude(ju => ju.JusticeUserRoles).ThenInclude(jur => jur.UserRole)
            .Include(x => x.Participants).ThenInclude(x => x.Person).ThenInclude(x => x.Organisation)
            .Include(x => x.Participants).ThenInclude(x => x.CaseRole)
            .Include(x => x.Participants).ThenInclude(x => x.HearingRole).ThenInclude(x => x.UserRole)
            .Include(x => x.HearingCases).ThenInclude(x => x.Case)
            .Include(x => x.CaseType)
            .Include(x => x.HearingVenue)
            .Where(x => x.ScheduledDateTime.Date == DateTime.UtcNow.Date)
            .AsQueryable();

        if (query.Unallocated.HasValue && query.Unallocated.Value && query.AllocatedTo.Count == 0)
        {
            hearingQuery = hearingQuery
                .Where(hearing => !hearing.Allocations.Any());
        }
        
        if(query.AllocatedTo.Count > 0 && !query.Unallocated.HasValue)
        {
            hearingQuery = hearingQuery
                .Where(hearing => hearing.Allocations
                    .Any(a => query.AllocatedTo.Contains(a.JusticeUser.Id)));
        }
        
        if(query.AllocatedTo.Count > 0 && query.Unallocated.HasValue && query.Unallocated.Value)
        {
            hearingQuery = hearingQuery
                .Where(hearing => hearing.Allocations
                    .Any(a => query.AllocatedTo.Contains(a.JusticeUser.Id)) || !hearing.Allocations.Any());
        }

        return await hearingQuery.OrderBy(x => x.ScheduledDateTime).AsNoTracking().AsSplitQuery().ToListAsync();
    }
        
}