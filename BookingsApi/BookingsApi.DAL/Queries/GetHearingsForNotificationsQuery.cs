using BookingsApi.DAL.Queries.BaseQueries;

namespace BookingsApi.DAL.Queries
{
    public class GetHearingsForNotificationsQuery : IQuery
    {
        public GetHearingsForNotificationsQuery()
        {
        }

    }

    public class GetHearingsForNotificationsQueryHandler : IQueryHandler<GetHearingsForNotificationsQuery, List<HearingNotificationDto>>
    {
        private readonly BookingsDbContext _context;
        private IQueryable<VideoHearing> _videoHearing;

        public GetHearingsForNotificationsQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<HearingNotificationDto>> Handle(GetHearingsForNotificationsQuery query)
        {
            _videoHearing = VideoHearings.Get(_context);

            var startDate = DateTime.Today.AddDays(2);  // 2 days is 48 hrs
            var endDate = DateTime.Today.AddDays(3);    // 3 days is 72 hrs.

            // we are gathering all the hearings where the scheduled date and time is between 48 hrs and 72 hrs.
            // all the hearings must be single day and only the first day of the multiday.
            var listHearings = await _videoHearing.Where(x =>
                    (x.Status == BookingStatus.Created ||
                     x.Status == Domain.Enumerations.BookingStatus.ConfirmedWithoutJudge ||
                     x.Status == Domain.Enumerations.BookingStatus.BookedWithoutJudge ||
                     x.Status == Domain.Enumerations.BookingStatus.Booked)
                    && x.ScheduledDateTime >= startDate
                    && x.ScheduledDateTime < endDate
                    && (x.SourceId == x.Id || x.SourceId == null))
                .ToListAsync();

            // map all singleday hearings to dto with total day set to 1
            var singleDayHearings = listHearings.Where(x => x.SourceId == null).ToList();
            var singleDayHearingsDto = singleDayHearings.Select(hearing => new HearingNotificationDto(hearing, 1)).ToList();

            var multipleDayHearings = listHearings.Where(x => x.SourceId != null).ToList();
            var multiDayIds = multipleDayHearings.Select(x => x.Id).ToList();

            var multiDayHearingList = await _context.VideoHearings
                .Where(x => x.SourceId != null && multiDayIds.Contains(x.SourceId.Value))
                .ToListAsync();
    
            // map all multiday hearings to dto with total day set to group count
            var groupedHearings = multiDayHearingList
                .GroupBy(x => x.SourceId).Select(grouping =>
                    new HearingNotificationDto(listHearings.Find(x => x.SourceId == grouping.Key), grouping.Count()))
                .ToList();

            return singleDayHearingsDto.Concat(groupedHearings).ToList();
        }
        
    }

    /// <summary>
    /// Extended class of the original dto with an additional property of TotalDays
    /// </summary>
    /// <param name="Hearing"></param>
    /// <param name="TotalDays"></param>
    public record HearingNotificationDto(VideoHearing Hearing, int TotalDays);
}
