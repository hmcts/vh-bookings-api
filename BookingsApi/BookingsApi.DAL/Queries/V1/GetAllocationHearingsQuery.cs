﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries.V1
{
    public class GetAllocationHearingsQuery : IQuery
    {
        public Guid[] HearingIds { get;}

        public GetAllocationHearingsQuery(Guid[] hearingId)
        {
            HearingIds = hearingId;
        }
    }

    public class GetAllocationHearingsQueryHandler : IQueryHandler<GetAllocationHearingsQuery, List<VideoHearing>>
    {
        private readonly BookingsDbContext _context;

        public GetAllocationHearingsQueryHandler(BookingsDbContext context) => _context = context;
        
        public async Task<List<VideoHearing>> Handle(GetAllocationHearingsQuery query)
            => await _context.VideoHearings
                .Include(h => h.HearingVenue)
                .Include(h => h.Allocations)
                    .ThenInclude(a => a.JusticeUser)
                    .ThenInclude(ju => ju.JusticeUserRoles)
                    .ThenInclude(jur => jur.UserRole)
                .Where(x => query.HearingIds.Contains(x.Id))
                .AsNoTracking().ToListAsync();
    }
}