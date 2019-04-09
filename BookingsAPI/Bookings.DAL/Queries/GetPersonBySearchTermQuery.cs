using System.Collections.Generic;
using System.Threading.Tasks;
using Bookings.DAL.Queries.Core;
using Bookings.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Bookings.DAL.Queries
{
    public class GetPersonBySearchTermQuery : IQuery
    {
        public string Term { get; set; }

        public GetPersonBySearchTermQuery(string term)
        {
            Term = term;
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
            return await _context.Persons
                .Include(x => x.Address)
                .Include(x => x.Organisation)
                .Where(x => x.ContactEmail.Contains(query.Term))
                .ToListAsync();

        }
    }
}
