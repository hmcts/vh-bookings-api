using System.Collections.Generic;
using System.Threading.Tasks;
using Bookings.Domain;
using BookingsApi.DAL.Queries.Core;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries
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