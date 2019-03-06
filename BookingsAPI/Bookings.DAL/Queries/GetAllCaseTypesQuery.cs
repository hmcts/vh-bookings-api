using System.Collections.Generic;
using System.Threading.Tasks;
using Bookings.DAL.Queries.Core;
using Bookings.Domain.RefData;
using Microsoft.EntityFrameworkCore;

namespace Bookings.DAL.Queries
{
    public class GetAllCaseTypesQuery : IQuery
    {
    }
    
    public class GetAllCaseTypesQueryHandler : IQueryHandler<GetAllCaseTypesQuery, List<CaseType>>
    {
        private readonly BookingsDbContext _context;

        public GetAllCaseTypesQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<CaseType>> Handle(GetAllCaseTypesQuery query)
        {
        return await _context.CaseTypes
                .Include(x => x.HearingTypes)
                .ToListAsync();
        }
    }
}