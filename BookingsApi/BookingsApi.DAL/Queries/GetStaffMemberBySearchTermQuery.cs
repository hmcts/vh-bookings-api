using BookingsApi.Domain.Participants;

namespace BookingsApi.DAL.Queries
{
    public class GetStaffMemberBySearchTermQuery : IQuery
    {
        public string Term { get; }

        public GetStaffMemberBySearchTermQuery(string term)
        {
            Term = term.ToLowerInvariant();
        }
    }

    public class GetStaffMemberBySearchTermQueryHandler : IQueryHandler<GetStaffMemberBySearchTermQuery, List<Person>>
    {
        private readonly BookingsDbContext _context;

        public GetStaffMemberBySearchTermQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }
        
        public async Task<List<Person>> Handle(GetStaffMemberBySearchTermQuery query)
        {
            var results = await (from person in _context.Persons
                join participant in _context.Participants.OfType<Participant>() on person.Id equals participant.PersonId
                where person.ContactEmail.ToLower().Contains(query.Term.ToLower())
                select person).Distinct().Include(x => x.Organisation).ToListAsync();

            return results;
        }
    }
}
