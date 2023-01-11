using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.DAL.Queries.Core;
using Castle.Core.Internal;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries
{
    public class GetAllocationHearingsBySearchQuery : IQuery
    {
        public string CaseNumber { get; }
        public string CaseType { get; }
        public DateTime? FromDate { get;}
        public DateTime? ToDate { get;}
        public string CsoName { get;}

        public GetAllocationHearingsBySearchQuery(
            string caseNumber = null, 
            string caseType = null,
            DateTime? fromDate = null, 
            DateTime? toDate = null, 
            string csoUserName = null)
        {
            CaseNumber = caseNumber;
            CaseType = caseType;
            FromDate = fromDate;
            ToDate = toDate;
            CsoName = csoUserName;
        }
    }

    public class GetAllocationHearingsBySearchQueryHandler : IQueryHandler<GetAllocationHearingsBySearchQuery, List<VideoHearing>>
    {
        private readonly BookingsDbContext _context;

        public GetAllocationHearingsBySearchQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<VideoHearing>> Handle(GetAllocationHearingsBySearchQuery query)
        {
            var hearings =  _context.VideoHearings
                .Include(h => h.CaseType)
                .Include(h => h.HearingCases).ThenInclude(hc => hc.Case)
                .Include(h => h.Allocations).ThenInclude(a => a.JusticeUser)
                .Where(x => (x.Status == BookingStatus.Created || x.Status == BookingStatus.Booked) && x.Status != BookingStatus.Cancelled) 
                .AsQueryable();

            if (query.FromDate != null)
            {
                hearings = query.ToDate != null 
                    ? hearings.Where(e => e.ScheduledDateTime >= query.FromDate && e.ScheduledDateTime <= query.ToDate)
                    : hearings.Where(e => e.ScheduledDateTime.Date == query.FromDate.Value.Date);
            }
            if (!query.CaseNumber.IsNullOrEmpty())
            {
                var caseNumber  = query.CaseNumber.ToLower().Trim();
                hearings = hearings
                    .Where(x => x.HearingCases
                        .Any(c => c.Case.Number.ToLower().Trim().Contains(caseNumber)));
            }
            if (!query.CaseType.IsNullOrEmpty())
            {
                var caseType  = query.CaseType.ToLower().Trim();
                hearings = hearings
                    .Where(x => x.CaseType.Name.ToLower().Trim().Contains(caseType));
            }
            if (!query.CsoName.IsNullOrEmpty())
            {
                var csoName  = query.CsoName.ToLower().Trim();
                hearings = hearings
                    .Where(x => x.Allocations
                        .Any(e => e.JusticeUser.Username.ToLower().Trim().Contains(csoName)));
            }

            return await hearings.AsNoTracking().ToListAsync();
        }
    }
}