using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Domain;
using BookingsApi.DAL.Queries.Core;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<Person>> Handle(GetPersonBySearchTermQuery query)
        {
            var excludeRoles = new List<string>() { "Judge", "JudicialOfficeHolder" };

            var results = await (from person in _context.Persons
             join participant in _context.Participants on person.Id equals participant.PersonId
             where !excludeRoles.Contains(participant.Discriminator) 
             && person.ContactEmail.ToLower().Contains(query.Term.ToLower())
             select person).Distinct().Include(x => x.Organisation).ToListAsync();

            return results;
        }
    }
}
