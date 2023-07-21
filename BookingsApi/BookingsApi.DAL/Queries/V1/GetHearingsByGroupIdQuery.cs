using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL.Queries.BaseQueries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries.V1
{
    public class GetHearingsByGroupIdQuery : IQuery
    {
        public GetHearingsByGroupIdQuery(Guid groupId)
        {
            GroupId = groupId;
        }

        public Guid GroupId { get; }
    }

    public class GetHearingsByGroupIdQueryHandler : IQueryHandler<GetHearingsByGroupIdQuery, List<VideoHearing>>
    {
        private readonly BookingsDbContext _context;

        public GetHearingsByGroupIdQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<VideoHearing>> Handle(GetHearingsByGroupIdQuery query)
        {
            var videoHearing = VideoHearings.Get(_context);

            return await videoHearing.Where(x => x.SourceId == query.GroupId)
                .OrderBy(x => x.ScheduledDateTime)
                .ToListAsync();
        }
    }
}