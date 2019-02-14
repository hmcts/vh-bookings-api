using System.Collections.Generic;
using System.Linq;
using Bookings.Domain;

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

        public List<HearingVenue> Handle(GetHearingVenuesQuery query)
        {
            return _context.Venues.ToList();
        }
    }
}