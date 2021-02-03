using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Domain;
using BookingsApi.DAL.Queries.Core;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries
{
    public class GetBookingsByCaseTypesQuery : IQuery
    {
        private int _limit;

        public GetBookingsByCaseTypesQuery() : this(new List<int>())
        {
        }

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
                if (value <= 0) throw new ArgumentException("Limit must be one or more", nameof(value));
                _limit = value;
            }
        }
    }

    public class GetBookingsByCaseTypesQueryHandler :
        IQueryHandler<GetBookingsByCaseTypesQuery, CursorPagedResult<VideoHearing, string>>
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
                .Include(x => x.HearingVenue)
                .AsNoTracking();

            if (query.CaseTypes.Any())
            {
                var dayToday = DateTime.UtcNow;
                var dateNow = new DateTime(dayToday.Year, dayToday.Month, dayToday.Day);
                hearings = hearings.Where(x =>
                    x.ScheduledDateTime > dateNow && query.CaseTypes.Contains(x.CaseTypeId));
            }

            hearings = hearings.OrderBy(x => x.ScheduledDateTime).ThenBy(x => x.Id);
            if (!string.IsNullOrEmpty(query.Cursor))
            {
                TryParseCursor(query.Cursor, out var scheduledDateTime, out var id);

                // Because of the difference in ordering using ThenBy and the comparison available with Guid.CompareTo
                // we have to both sort and compare the guid as a string which will give us a consistent behavior
                hearings = hearings.Where(x => x.ScheduledDateTime > scheduledDateTime
                                               || x.ScheduledDateTime == scheduledDateTime
                                               && x.Id.CompareTo(id) > 0);
            }

            // Add one to the limit to know whether or not we have a next page
            var result = await hearings.Take(query.Limit + 1).ToListAsync();
            string nextCursor = null;
            if (result.Count > query.Limit)
            {
                // The next cursor should be built based on the last item in the list
                result = result.Take(query.Limit).ToList();
                var lastResult = result.Last();
                nextCursor = $"{lastResult.ScheduledDateTime.Ticks}_{lastResult.Id}";
            }

            return new CursorPagedResult<VideoHearing, string>(result, nextCursor);
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