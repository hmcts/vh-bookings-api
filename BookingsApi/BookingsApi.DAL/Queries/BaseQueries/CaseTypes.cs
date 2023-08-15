namespace BookingsApi.DAL.Queries.BaseQueries;

public static class CaseTypes
{
    public static IQueryable<CaseType> Get(BookingsDbContext context)
    {
        return context.CaseTypes
            .Include(x => x.CaseRoles)
            .ThenInclude(x => x.HearingRoles)
            .ThenInclude(x => x.UserRole)
            .Include(x => x.HearingTypes)
            .AsQueryable();
    }
    
    public static void PopulateCaseRoles(this CaseType caseType)
    {
        caseType.CaseRoles = caseType.CaseRoles
            .OrderBy(x => x.Name)
            .ToList();
        
        foreach (var caseRole in caseType.CaseRoles)
            caseRole.HearingRoles = caseRole.HearingRoles.OrderBy(x => x.Name).ToList();
    }
}