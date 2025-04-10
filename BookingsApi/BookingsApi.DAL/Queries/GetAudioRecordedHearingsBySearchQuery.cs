﻿namespace BookingsApi.DAL.Queries
{
    public class GetAudioRecordedHearingsBySearchQuery : IQuery
    {
        public string CaseNumber { get; }
        public DateTime? Date { get;}

        public GetAudioRecordedHearingsBySearchQuery(string caseNumber, DateTime? date = null)
        {
            CaseNumber = caseNumber;
            Date = date;
        }
    }

    public class GetAudioRecordedHearingsBySearchQueryHandler : IQueryHandler<GetAudioRecordedHearingsBySearchQuery, List<VideoHearing>>
    {
        private readonly BookingsDbContext _context;

        public GetAudioRecordedHearingsBySearchQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<VideoHearing>> Handle(GetAudioRecordedHearingsBySearchQuery query)
        {
            var efQuery = _context.VideoHearings
                .Include(x=> x.Participants).ThenInclude(x=> x.Person).ThenInclude(x=> x.Organisation)
                .Include(x=> x.Participants).ThenInclude(x=> x.HearingRole).ThenInclude(x=> x.UserRole)
                .Include(x=> x.HearingCases).ThenInclude(x=> x.Case)
                .Include(x => x.CaseType)
                .Include(x => x.HearingVenue)
                .Where(x => x.AudioRecordingRequired && x.Status == BookingStatus.Created);

            if (!string.IsNullOrWhiteSpace(query.CaseNumber))
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