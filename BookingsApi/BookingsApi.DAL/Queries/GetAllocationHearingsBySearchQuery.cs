﻿namespace BookingsApi.DAL.Queries;

public class GetAllocationHearingsBySearchQuery : IQuery
{
    public string CaseNumber { get; }
    public string[] CaseType { get; }
    public DateTime? FromDate { get;}
    public DateTime? ToDate { get;}
    public Guid[] Cso { get;}
    public bool IsUnallocated { get; }
    public bool IncludeWorkHours { get; }
    public bool ExcludeDurationsThatSpanMultipleDays { get; }

    public GetAllocationHearingsBySearchQuery(
        string caseNumber = null, 
        IEnumerable<string> caseType = null,
        DateTime? fromDate = null, 
        DateTime? toDate = null, 
        IEnumerable<Guid> cso = null,
        bool isUnallocated = false,
        bool includeWorkHours = false,
        bool excludeDurationsThatSpanMultipleDays = false)
    {
        CaseNumber = caseNumber?.ToLower().Trim();
        CaseType = caseType?.Select(s => s.ToLower().Trim()).ToArray() ?? [];
        FromDate = fromDate;
        ToDate = toDate;
        Cso = cso?.ToArray() ?? [];
        IsUnallocated = isUnallocated;
        IncludeWorkHours = includeWorkHours;
        ExcludeDurationsThatSpanMultipleDays = excludeDurationsThatSpanMultipleDays;
    }
}

public class GetAllocationHearingsBySearchQueryHandler(BookingsDbContext context, bool isTest = false)
    : IQueryHandler<GetAllocationHearingsBySearchQuery, List<VideoHearing>>
{
    public async Task<List<VideoHearing>> Handle(GetAllocationHearingsBySearchQuery query)
    {
        var hearings =  context.VideoHearings
            .Include(h => h.HearingVenue)
            .Include(h => h.CaseType)
            .Include(h => h.HearingCases).ThenInclude(hc => hc.Case)
            .Include(h => h.Allocations).ThenInclude(a => a.JusticeUser)
            .Where(x 
                => (x.Status == BookingStatus.Created || 
                    x.Status == BookingStatus.Booked ||
                    x.Status == BookingStatus.ConfirmedWithoutJudge || 
                    x.Status == BookingStatus.BookedWithoutJudge) 
                   && x.Status != BookingStatus.Cancelled
                   && x.HearingVenue.IsWorkAllocationEnabled)
            .AsSplitQuery()
            .AsQueryable();
            
        hearings = FilterQueryableHearings(query, hearings);

        return await hearings.OrderBy(x=>x.ScheduledDateTime).AsNoTracking().AsSplitQuery().ToListAsync();
    }

    private IQueryable<VideoHearing> FilterQueryableHearings(GetAllocationHearingsBySearchQuery query, IQueryable<VideoHearing> hearings)
    {
        if (!isTest)
            hearings = hearings.Where(h1 => h1.CaseTypeId != 3); //exclude generic type test cases from prod

        if (query.IsUnallocated)
            hearings = hearings.Where(x => !x.Allocations.Any());

        if (query.IncludeWorkHours)
        {
            hearings = hearings.Include(h => h.Allocations).ThenInclude(a => a.JusticeUser)
                .ThenInclude(x => x.VhoWorkHours)
                .Include(h => h.Allocations).ThenInclude(a => a.JusticeUser).ThenInclude(x => x.VhoNonAvailability);
        }

        if (!string.IsNullOrWhiteSpace(query.CaseNumber))
            hearings = hearings
                .Where(h3 => h3.HearingCases
                    .Any(c => c.Case.Number.ToLower().Trim().Contains(query.CaseNumber)));

        if (query.CaseType.Length != 0)
            hearings = hearings
                .Where(h4 => query.CaseType.Contains(h4.CaseType.Name.ToLower().Trim()));

        if (query.Cso.Length != 0)
            hearings = hearings
                .Where(h5 => h5.Allocations
                    .Any(a => query.Cso.Contains(a.JusticeUser.Id)));
            
        if (query.FromDate != null)
        {
            hearings = query.ToDate != null 
                ? hearings.Where(h6 => h6.ScheduledDateTime.Date >= query.FromDate.Value.Date && h6.ScheduledDateTime.Date <= query.ToDate.Value.Date)
                : hearings.Where(h6 => h6.ScheduledDateTime.Date == query.FromDate.Value.Date);
        }

        if (query.ExcludeDurationsThatSpanMultipleDays)
            hearings = hearings.Where(x => x.ScheduledDateTime.AddMinutes(x.ScheduledDuration).Date == x.ScheduledDateTime.Date);
       
        return hearings;
    }
}