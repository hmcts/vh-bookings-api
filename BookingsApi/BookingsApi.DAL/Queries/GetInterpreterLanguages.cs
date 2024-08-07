namespace BookingsApi.DAL.Queries;

public class GetInterpreterLanguages : IQuery
{
}

public class GetInterpreterLanguagesHandler : IQueryHandler<GetInterpreterLanguages, List<InterpreterLanguage>>
{
    private readonly BookingsDbContext _context;

    public GetInterpreterLanguagesHandler(BookingsDbContext context)
    {
        _context = context;
    }

    public async Task<List<InterpreterLanguage>> Handle(GetInterpreterLanguages query)
    {
        return await _context.InterpreterLanguages.AsNoTracking().ToListAsync();
    }
}