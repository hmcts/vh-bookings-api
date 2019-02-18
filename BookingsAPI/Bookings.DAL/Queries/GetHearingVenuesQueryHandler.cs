using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookings.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bookings.DAL.Queries
{
    public class GetHearingVenuesQuery : IQuery
    {
    }
    
    public class GetHearingVenuesQueryHandler : IQueryHandler<GetHearingVenuesQuery, List<HearingVenue>>
    {
        private readonly BookingsDbContext _context;

        public GetHearingVenuesQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<HearingVenue>> Handle(GetHearingVenuesQuery query)
        {
            return await _context.Venues.ToListAsync();
        }
    }
}