﻿namespace BookingsApi.DAL.Queries
{
    public class GetBookingsByCaseTypesQuery : IQuery
    {
        private int _limit;

        public GetBookingsByCaseTypesQuery(IList<int> types)
        {
            CaseTypes = types;
            Limit = 100;
            VenueIds = new List<int>();
            SelectedUsers = new List<Guid>();
        }

        public IList<int> CaseTypes { get; set; }
        
        public IList<Guid> SelectedUsers { get; set; }
        public string Cursor { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CaseNumber { get; set; }
        public IList<int> VenueIds { get; set; }
        public string LastName { get; set; }
        public bool NoJudge { get; set; }
        
        public bool Unallocated { get; set; }

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
            var hearings = _context.VideoHearings
                .Include(x=> x.Participants).ThenInclude(x=> x.Person)
                .Include(x=> x.Participants).ThenInclude(x=> x.HearingRole).ThenInclude(x=> x.UserRole)
                .Include(x=> x.HearingCases).ThenInclude(x=> x.Case)
                .Include(x => x.JudiciaryParticipants).ThenInclude(x => x.JudiciaryPerson)
                .Include(x => x.CaseType)
                .Include(x => x.HearingVenue)
                .Include(x => x.Allocations).ThenInclude(x => x.JusticeUser)
                .AsNoTracking();

            hearings = ApplyOptionalFilters(query, hearings);


            hearings = hearings.Where(x => x.ScheduledDateTime > query.StartDate)
                .OrderBy(x => x.ScheduledDateTime)
                .ThenBy(x => x.Id);

            if (!string.IsNullOrEmpty(query.Cursor))
            {
                TryParseCursor(query.Cursor, out var scheduledDateTime, out var id);

                // Because of the difference in ordering using ThenBy and the comparison available with Guid.CompareTo
                // we have to both sort and compare the guid as a string which will give us a consistent behavior
                hearings = hearings.Where(x => x.ScheduledDateTime > scheduledDateTime
                                               || x.ScheduledDateTime == scheduledDateTime
                                               && x.Id.CompareTo(id) > 0);
            }

            // Add one to the limit to know whether we have a next page
            var result = await hearings.Take(query.Limit + 1).ToListAsync();
            string nextCursor = null;
            if (result.Count > query.Limit)
            {
                // The next cursor should be built based on the last item in the list
                result = result.Take(query.Limit).ToList();
                var lastResult = result[result.Count-1];
                nextCursor = $"{lastResult.ScheduledDateTime.Ticks}_{lastResult.Id}";
            }

            return new CursorPagedResult<VideoHearing, string>(result, nextCursor);
        }

        private static IQueryable<VideoHearing> ApplyOptionalFilters(GetBookingsByCaseTypesQuery query, IQueryable<VideoHearing> hearings)
        {
            if (query.CaseTypes.Any())
            {
                hearings = hearings.Where(x => query.CaseTypes.Contains(x.CaseTypeId));
            }

            // Executes code block for new feature when AdminSearchToggle is ON 

            if (!string.IsNullOrWhiteSpace(query.CaseNumber))
            {
                hearings = hearings.Where(x => x.HearingCases.Any(hc => hc.Case.Number.Contains(query.CaseNumber)));
            }

            if (query.VenueIds.Any())
            {
                hearings = hearings.Where(x => query.VenueIds.Contains(x.HearingVenue.Id));
            }

            if (query.EndDate != null)
            {
                hearings = hearings.Where(x => x.ScheduledDateTime <= query.EndDate);
            }

            if (!string.IsNullOrWhiteSpace(query.LastName))
            {
                hearings = hearings
                    .Where(h => h.Participants
                        .Any(p => p.Person.LastName.Contains(query.LastName)));
            }

            if (query.NoJudge)
            {
                hearings = hearings.Where(x => x.JudiciaryParticipants.All(y => y.HearingRoleCode != JudiciaryParticipantHearingRoleCode.Judge));
            }

            if (query.Unallocated)
            {
                hearings = hearings.Where(h => !h.Allocations.Any());
            }

            if (query.SelectedUsers.Any())
            {
                hearings = hearings.Where(x =>
                    x.Allocations.Any(a => query.SelectedUsers.Contains(a.JusticeUserId)));
            }

            return hearings;
        }
    
        private static void TryParseCursor(string cursor, out DateTime scheduledDateTime, out Guid id)
        {
            try
            {
                var parts = cursor.Split('_');
                scheduledDateTime = new DateTime(long.Parse(parts[0]), DateTimeKind.Utc);
                id = Guid.Parse(parts[1]);
            }
            catch (Exception e)
            {
                throw new FormatException($"Unexpected cursor format [{cursor}]", e);
            }
        }
    }
}