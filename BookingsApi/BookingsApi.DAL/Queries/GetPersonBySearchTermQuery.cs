using BookingsApi.Common.Services;

namespace BookingsApi.DAL.Queries
{
    public class GetPersonBySearchTermQuery : IQuery
    {
        public string Term { get; }

        public GetPersonBySearchTermQuery(string term)
        {
            Term = term.ToLowerInvariant();
        }
    }

    public class GetPersonBySearchTermQueryHandler : IQueryHandler<GetPersonBySearchTermQuery, List<Person>>
    {
        private readonly BookingsDbContext _context;

        public GetPersonBySearchTermQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<Person>> Handle(GetPersonBySearchTermQuery query) =>
              await _context.Persons
                .Include(x => x.Organisation)
                .Where(x => x.ContactEmail.ToLower().Contains(query.Term.ToLower()))
                .ToListAsync();
        
    }
}
