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
        private int _limit;

        public GetBookingsByCaseTypesQuery() : this(new List<int>())
        {}
        
        public GetBookingsByCaseTypesQuery(IList<int> types)
        {
            CaseTypes = types;
            Limit = 100;
        }

        public IList<int> CaseTypes { get; }

        public string Cursor { get; set; }

        public int Limit
        {
            get => _limit;
            set
            {
                if (value <= 0) throw new ArgumentException("Limit must be one or more");
                _limit = value;
            }
        }
    }

    public class GetBookingsByCaseTypesQueryHandler : IQueryHandler<GetBookingsByCaseTypesQuery, CursorPagedResult<VideoHearing, string>>
    {
        private readonly BookingsDbContext _context;

        public GetBookingsByCaseTypesQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<CursorPagedResult<VideoHearing, string>> Handle(GetBookingsByCaseTypesQuery query)
        {               
            IQueryable<VideoHearing> hearings = _context.VideoHearings
                .Include("Participants.Person")
                .Include("Participants.HearingRole.UserRole")
                .Include("Participants.CaseRole")
                .Include("HearingCases.Case")
                .Include(x => x.HearingType)
                .Include(x => x.CaseType)
                .Include(x => x.HearingVenue);
            
            if (query.CaseTypes.Any())
            {
                hearings = hearings.Where(x =>
                    x.ScheduledDateTime > DateTime.Now && query.CaseTypes.Contains(x.CaseTypeId));
            }

            hearings = hearings.OrderBy(x => x.ScheduledDateTime).ThenBy(x => x.Id);
            if (!string.IsNullOrEmpty(query.Cursor))
            {
                TryParseCursor(query.Cursor, out var scheduledDateTime, out var id);
                hearings = hearings.Where(x =>
                    x.ScheduledDateTime > scheduledDateTime || x.ScheduledDateTime == scheduledDateTime && x.Id.CompareTo(id) > 0);
            }

            var result = await hearings.Take(query.Limit).ToListAsync();
            var lastResult = result.Last();
            var nextCursor = $"{lastResult.ScheduledDateTime.Date.Ticks}_{lastResult.Id}";
            return new CursorPagedResult<VideoHearing, string>(result, query.Cursor, nextCursor);
        }

        private void TryParseCursor(string cursor, out DateTime scheduledDateTime, out Guid id)
        {
            try
            {
                var parts = cursor.Split('_');
                scheduledDateTime = new DateTime(long.Parse(parts[0]));
                id = Guid.Parse(parts[1]);
            }
            catch (Exception e)
            {
                throw new FormatException($"Unexpected cursor format [{cursor}]", e);
            }
        }
    }
}