namespace BookingsApi.DAL.Queries;

public class GetJudiciaryPersonBySearchTermQuery(string term) : IQuery
{
    public string Term { get; } = term.ToLowerInvariant();
}


#pragma warning disable CA1862 // Disabled as must be SQL compliant
public class GetJudiciaryPersonBySearchTermQueryHandler(BookingsDbContext context)
    : IQueryHandler<GetJudiciaryPersonBySearchTermQuery, List<JudiciaryPerson>>
{
    public async Task<List<JudiciaryPerson>> Handle(GetJudiciaryPersonBySearchTermQuery query)
    {
        return await context.JudiciaryPersons
            .Where(x => x.Email.ToLower().Contains(query.Term.ToLower()) && !x.HasLeft && !x.Deleted)
            .ToListAsync();

    }
}
#pragma warning restore CA1862