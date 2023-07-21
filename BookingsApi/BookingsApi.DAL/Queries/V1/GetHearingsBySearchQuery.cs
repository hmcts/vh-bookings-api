using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using Castle.Core.Internal;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries.V1
{
    public class GetHearingsBySearchQuery : IQuery
    {
        public string CaseNumber { get; }
        public DateTime? Date { get;}

        public GetHearingsBySearchQuery(string caseNumber, DateTime? date = null)
        {
            CaseNumber = caseNumber;
            Date = date;
        }
    }

    public class GetHearingsBySearchQueryHandler : IQueryHandler<GetHearingsBySearchQuery, List<VideoHearing>>
    {
        private readonly BookingsDbContext _context;

        public GetHearingsBySearchQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<VideoHearing>> Handle(GetHearingsBySearchQuery query)
        {
            var efQuery = _context.VideoHearings
                .Include(x=> x.Participants).ThenInclude(x=> x.Person).ThenInclude(x=> x.Organisation)
                .Include(x=> x.Participants).ThenInclude(x=> x.CaseRole)
                .Include(x=> x.Participants).ThenInclude(x=> x.HearingRole).ThenInclude(x=> x.UserRole)
                .Include(x=> x.HearingCases).ThenInclude(x=> x.Case)
                .Include(x => x.CaseType)
                .Where(x => x.AudioRecordingRequired && x.Status == BookingStatus.Created);

            if (!query.CaseNumber.IsNullOrEmpty())
            {
                var caseNumber  = query.CaseNumber.ToLower().Trim();
                efQuery = efQuery.Where(x =>
                    x.HearingCases.Any(c => c.Case.Number.ToLower().Trim().Contains(caseNumber)));
            }

            if (query.Date.HasValue)
            {
                var date = query.Date.Value;
                efQuery = efQuery.Where(x =>
                    x.ScheduledDateTime.DayOfYear == date.DayOfYear);
            }
            
            return await efQuery.AsNoTracking().Take(100).ToListAsync();
        }
    }
}