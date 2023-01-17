﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL.Helper;
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
        public string[] CaseType { get; }
        public DateTime? FromDate { get;}
        public DateTime? ToDate { get;}
        public Guid[] Cso { get;}
        public bool IsUnallocated { get; }

        public GetAllocationHearingsBySearchQuery(
            string caseNumber = null, 
            IEnumerable<string> caseType = null,
            DateTime? fromDate = null, 
            DateTime? toDate = null, 
            IEnumerable<Guid> cso = null,
            bool isUnallocated = false)
        {
            CaseNumber = caseNumber?.ToLower().Trim();
            CaseType = caseType?.Select(s => s.ToLower().Trim()).ToArray() ?? Array.Empty<string>();
            FromDate = fromDate;
            ToDate = toDate;
            Cso = cso?.ToArray() ?? Array.Empty<Guid>();
            IsUnallocated = isUnallocated;
        }

    }

    public class GetAllocationHearingsBySearchQueryHandler : IQueryHandler<GetAllocationHearingsBySearchQuery, List<VideoHearing>>
    {
        private readonly BookingsDbContext _context;
        private readonly bool _isTest;

        public GetAllocationHearingsBySearchQueryHandler(BookingsDbContext context, bool isTest = false)
        {
            _context = context;
            _isTest = isTest;
        }
        
        public async Task<List<VideoHearing>> Handle(GetAllocationHearingsBySearchQuery query)
        {
            var hearings =  _context.VideoHearings
                .Include(h => h.CaseType)
                .Include(h => h.HearingType)
                .Include(h => h.HearingCases).ThenInclude(hc => hc.Case)
                .Include(h => h.Allocations).ThenInclude(a => a.JusticeUser)
                .Where(x 
                    => (x.Status == BookingStatus.Created || x.Status == BookingStatus.Booked) 
                         && x.Status != BookingStatus.Cancelled
                         && x.ScheduledDateTime >= DateTime.UtcNow  
                         && HearingScottishVenueNames.ScottishHearingVenuesList.All(venueName => venueName != x.HearingVenueName))
                .AsQueryable();

            if (!_isTest)
                hearings = hearings.Where(x => x.CaseTypeId != 3); //exclude generic type test cases from prod
            
            if (query.IsUnallocated)
                hearings = hearings.Where(x => x.Allocations.FirstOrDefault(a => a.HearingId == x.Id) == null);

            if (!query.CaseNumber.IsNullOrEmpty())
                hearings = hearings
                    .Where(x => x.HearingCases
                        .Any(c => c.Case.Number.ToLower().Trim().Contains(query.CaseNumber)));

            if (query.CaseType.Any())
                hearings = hearings
                    .Where(x => query.CaseType.Contains(x.CaseType.Name.ToLower().Trim()));

            if (query.Cso.Any())
                hearings = hearings
                    .Where(x => x.Allocations
                        .Any(a => query.Cso.Contains(a.JusticeUser.Id)));
            
            if (query.FromDate != null)
            {
                hearings = query.ToDate != null 
                    ? hearings.Where(e => e.ScheduledDateTime >= query.FromDate && e.ScheduledDateTime <= query.ToDate)
                    : hearings.Where(e => e.ScheduledDateTime.Date == query.FromDate.Value.Date);
            }
            
            return await hearings.OrderBy(x=>x.ScheduledDateTime).AsNoTracking().ToListAsync();
        }
    }
}