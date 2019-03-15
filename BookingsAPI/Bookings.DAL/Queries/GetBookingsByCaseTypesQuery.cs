using Bookings.DAL.Queries.Core;
using Bookings.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookings.DAL.Queries
{
    public class GetBookingsByCaseTypesQuery : IQuery
    {
        public GetBookingsByCaseTypesQuery(IList<int> types, string cursor, int limit)
        {
            CaseTypes = types;
            Cursor = cursor;
            Limit = limit;
        }

        public IList<int> CaseTypes { get; set; }

        public string Cursor { get; set; }

        public int Limit { get; set; }

        public long CursorCreatedTime
        {
            get
            {
                if (!string.IsNullOrEmpty(Cursor) && Cursor != "0")
                {
                    long.TryParse(Cursor, out long createdTime);
                    return createdTime;
                }
                return 0;
            }
        }
    }

    public class GetBookingsByCaseTypesQueryHandler : IQueryHandler<GetBookingsByCaseTypesQuery, IList<VideoHearing>>
    {
        private readonly BookingsDbContext _context;

        public GetBookingsByCaseTypesQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<IList<VideoHearing>> Handle(GetBookingsByCaseTypesQuery query)
        {
            IQueryable<VideoHearing> allList;
            IList<VideoHearing> response;
            if (query.CaseTypes.Any())
            {
                allList = _context.VideoHearings
                    .Include("Participants.Person")
                    .Include("Participants.HearingRole.UserRole")
                    .Include("Participants.CaseRole")
                    .Include("HearingCases.Case")
                    .Include(x => x.HearingType)
                    .Include(x => x.CaseType)
                    .Include(x => x.HearingVenue)
                    .Where(x => x.ScheduledDateTime > DateTime.Now && query.CaseTypes.Any(s => s == x.HearingTypeId))
                    .OrderBy(d => d.CreatedDate).AsQueryable();
            }
            else
            {
                allList =  _context.VideoHearings
                    .Include("Participants.Person")
                    .Include("Participants.HearingRole.UserRole")
                    .Include("Participants.CaseRole")
                    .Include("HearingCases.Case")
                    .Include(x => x.HearingType)
                    .Include(x => x.CaseType)
                    .Include(x => x.HearingVenue)
                    .Where(x => x.ScheduledDateTime > DateTime.Now)
                    .OrderBy(d => d.CreatedDate).AsQueryable();
            }

            if (!string.IsNullOrEmpty(query.Cursor) && query.Cursor != "0")
            {
                var createdTime = query.CursorCreatedTime;
                if (createdTime > 0)
                {
                    response = await allList
                        .Where(x => x.CreatedDate.Ticks > createdTime)
                       .Select(x => x)
                       .Take(query.Limit).ToListAsync(new System.Threading.CancellationToken());
                }
                else
                {
                    //no data
                    response = Enumerable.Empty<VideoHearing>().ToList();
                }
            }
            else
            {
                // first request, the next cursor is not defined
                response = await allList.Take(query.Limit).ToListAsync(new System.Threading.CancellationToken());
            }

            return response;
        }
    }
}
