using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingsApi.DAL.Queries
{
    public class GetJudiciaryPersonBySearchTermQuery : IQuery
    {
        public string Term { get; }

        public GetJudiciaryPersonBySearchTermQuery(string term)
        {
            Term = term.ToLowerInvariant();
        }
    }

    public class GetJudiciaryPersonBySearchTermQueryHandler : IQueryHandler<GetJudiciaryPersonBySearchTermQuery, List<JudiciaryPerson>>
    {
        private readonly BookingsDbContext _context;

        public GetJudiciaryPersonBySearchTermQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<JudiciaryPerson>> Handle(GetJudiciaryPersonBySearchTermQuery query)
        {
            return await _context.JudiciaryPersons
                .Where(x => x.Email.ToLower().Contains(query.Term.ToLower()) && !x.HasLeft)
                .ToListAsync();

        }
    }
}
