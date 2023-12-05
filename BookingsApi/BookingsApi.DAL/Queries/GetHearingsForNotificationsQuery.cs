using BookingsApi.DAL.Dtos;
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

        public GetHearingsForNotificationsQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<HearingNotificationDto>> Handle(GetHearingsForNotificationsQuery query)
        {
            var videoHearing = VideoHearings.Get(_context);

            var startDate = DateTime.Today.AddDays(2);  // 2 days is 48 hrs
            var endDate = DateTime.Today.AddDays(3);    // 3 days is 72 hrs.

            // we are gathering all the hearings where the scheduled date and time is between 48 hrs and 72 hrs.
            var listHearings = await videoHearing.Where(x =>
                    (x.Status == BookingStatus.Created ||
                     x.Status == BookingStatus.ConfirmedWithoutJudge ||
                     x.Status == BookingStatus.BookedWithoutJudge ||
                     x.Status == BookingStatus.Booked)
                    && x.ScheduledDateTime >= startDate
                    && x.ScheduledDateTime < endDate)
                .ToListAsync();

            var sourceIds = listHearings.Select(x => x.SourceId).Where(x => x != null).Distinct().ToList();
            var sourceHearings = await videoHearing.Where(x => sourceIds.Contains(x.SourceId) && x.SourceId == x.Id).ToListAsync();

            // map all singleday hearings to dto with total day set to 1
            var singleDayHearings = listHearings.Where(x => x.SourceId == null).ToList();
            var singleDayHearingsDto = singleDayHearings.Select(hearing => new HearingNotificationDto(hearing, 1)).ToList();

            var multipleDayHearings = listHearings.Where(x => x.SourceId != null).ToList();
            var multiDayHearingDtos = multipleDayHearings
                .Select(hearing => new HearingNotificationDto(
                    hearing,
                    TotalDays: _context.VideoHearings.Count(x => x.SourceId == hearing.SourceId), 
                    SourceHearing: sourceHearings.Find(x => x.Id == hearing.SourceId)))
                .ToList();

            return singleDayHearingsDto.Concat(multiDayHearingDtos).ToList();
        }
    }
}
