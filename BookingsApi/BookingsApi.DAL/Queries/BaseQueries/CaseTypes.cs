namespace BookingsApi.DAL.Queries.BaseQueries;

public static class CaseTypes
{
    public static IQueryable<CaseType> Get(BookingsDbContext context)
    {
        return context.CaseTypes.AsQueryable();
    }
}