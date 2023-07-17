using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Domain;
using BookingsApi.DAL.Queries.Core;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries
{
    public class GetHearingsForTodayByVenuesQuery : IQuery
    {
        public GetHearingsForTodayByVenuesQuery(IEnumerable<string> hearingVenueNames)
        {
            HearingVenueNames = hearingVenueNames.ToList();
        }
        public List<string> HearingVenueNames { get; }
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
            var hearingQuery = _context.VideoHearings
                .Include(h => h.Allocations).ThenInclude(a => a.JusticeUser).ThenInclude(ju => ju.JusticeUserRoles).ThenInclude(jur => jur.UserRole)
                .Include(x => x.Participants).ThenInclude(x => x.Person).ThenInclude(x => x.Organisation)
                .Include(x => x.Participants).ThenInclude(x => x.CaseRole)
                .Include(x => x.Participants).ThenInclude(x => x.HearingRole).ThenInclude(x => x.UserRole)
                .Include(x => x.HearingCases).ThenInclude(x => x.Case)
                .Include(x => x.CaseType)
                .AsQueryable();
                
            return await hearingQuery
                .Where(x => x.ScheduledDateTime.Date == DateTime.Today.Date && query.HearingVenueNames.Contains(x.HearingVenueName))
                .OrderBy(x => x.HearingVenueName).ThenBy(x => x.ScheduledDateTime).ToListAsync();
        }
    }
}