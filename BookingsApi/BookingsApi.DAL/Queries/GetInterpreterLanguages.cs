namespace BookingsApi.DAL.Queries;

public class GetInterpreterLanguages : IQuery
{
}

public class GetInterpreterLanguagesHandler(BookingsDbContext context)
    : IQueryHandler<GetInterpreterLanguages, List<InterpreterLanguage>>
{
    public async Task<List<InterpreterLanguage>> Handle(GetInterpreterLanguages query)
    {
        return await context.InterpreterLanguages.AsNoTracking().ToListAsync();
    }
}