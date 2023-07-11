using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Domain;
using BookingsApi.DAL.Queries.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BookingsApi.DAL.Queries
{
    public class GetHearingsForTodayByVenuesQuery : IQuery
    {
        public GetHearingsForTodayByVenuesQuery(IEnumerable<string> hearingVenueNames)
        {
            HearingVenueNames = hearingVenueNames.ToList();
        }
        public List<string> HearingVenueNames { get; set; }
    }
    
    public class GetHearingsForTodayByVenuesQueryHandler : IQueryHandler<GetHearingsForTodayByVenuesQuery, List<VideoHearing>>
    {
        private readonly BookingsDbContext _context;

        public GetHearingsForTodayByVenuesQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<VideoHearing>> Handle(GetHearingsForTodayByVenuesQuery query)
        {
            var hearings = _context.VideoHearings
                .Include(h => h.Allocations)
                    .ThenInclude(a => a.JusticeUser)
                    .ThenInclude(ju => ju.JusticeUserRoles)
                    .ThenInclude(jur => jur.UserRole)
                .Where(x => x.ScheduledDateTime.Date == DateTime.Today);

            if (!query.HearingVenueNames.IsNullOrEmpty())
                hearings = hearings.Where(p => query.HearingVenueNames.Contains(p.HearingVenueName)).Distinct();
            
            return await hearings
                .OrderBy(x => x.HearingVenueName).ThenBy(x => x.ScheduledDateTime)
                .ToListAsync();
        }
    }
}