using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries
{
    public class GetVhoNonAvailableWorkHoursByIdsQuery : IQuery
    {
        public IList<long> Ids { get; set; }
        
        public GetVhoNonAvailableWorkHoursByIdsQuery(IList<long> ids)
        {
            Ids = ids;
        }
    }

    public class GetVhoNonAvailableWorkHoursByIdsQueryHandler : IQueryHandler<GetVhoNonAvailableWorkHoursByIdsQuery, List<VhoNonAvailability>>
    {
        private readonly BookingsDbContext _context;
        
        public GetVhoNonAvailableWorkHoursByIdsQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<VhoNonAvailability>> Handle(GetVhoNonAvailableWorkHoursByIdsQuery query)
        {
            return await _context.VhoNonAvailabilities.Where(a => query.Ids.Contains(a.Id))
                .ToListAsync();
        }
    }
}
