using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Domain;
using BookingsApi.DAL.Queries.Core;
using Microsoft.EntityFrameworkCore;
using BookingsApi.Common.Services;

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
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CaseNumber { get; set; }
        public IList<int> VenueIds { get; set; }
        public string LastName { get; set; }

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
        private readonly IFeatureToggles _featureToggles;

        public GetBookingsByCaseTypesQueryHandler(BookingsDbContext context, IFeatureToggles featureToggles)
        {
            _context = context;
            _featureToggles = featureToggles;
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
                hearings = hearings.Where(x => query.CaseTypes.Contains(x.CaseTypeId));
            }

            // Executes code block for new feature when AdminSearchToggle is ON 
            if (_featureToggles.AdminSearchToggle())
            {
                if (!string.IsNullOrWhiteSpace(query.CaseNumber))
                {
                    hearings = await FilterByCaseNumber(hearings, query);
                }

                if (query.VenueIds != null && query.VenueIds.Any())
                {
                    hearings = hearings.Where(x => query.VenueIds.Contains(x.HearingVenue.Id));
                }

                if (query.EndDate != null)
                {
                    hearings = hearings.Where(x => x.ScheduledDateTime <= query.EndDate);
                }

                if (!string.IsNullOrWhiteSpace(query.LastName))
                {
                    hearings = hearings.Where(h => h.Participants.Any(p => p.Person.LastName.Contains(query.LastName)));
                }
            }

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

            // Add one to the limit to know whether or not we have a next page
            var result = hearings.Take(query.Limit + 1).ToList();
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

        private async Task<IQueryable<VideoHearing>> FilterByCaseNumber(IQueryable<VideoHearing> hearings, GetBookingsByCaseTypesQuery query)
        {
            var cases = await _context.Cases.Where(x => x.Number.Contains(query.CaseNumber)).AsNoTracking().ToListAsync();

            hearings = hearings.Where(x => x.HearingCases.Any(y => cases.Contains(y.Case)));

            var caseNumbers = cases.Select(r => r.Number);

            var vhList = new List<VideoHearing>();

            foreach (var item in hearings)
            {
                var hearingCases = item.HearingCases.Where(y => caseNumbers.Contains(y.Case.Number)).ToList();

                if (hearingCases.Any())
                {
                    item.HearingCases = hearingCases;
                    vhList.Add(item);
                }

            }

            return vhList.AsQueryable();
        }
    }
}