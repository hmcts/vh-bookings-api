﻿using BookingsApi.DAL.Queries.BaseQueries;

namespace BookingsApi.DAL.Queries
{
    public class GetHearingsForNotificationsQuery : IQuery
    {
        public GetHearingsForNotificationsQuery()
        {
        }

    }

    public class GetHearingsForNotificationsQueryHandler : IQueryHandler<GetHearingsForNotificationsQuery, List<VideoHearing>>
    {
        private readonly BookingsDbContext _context;

        public GetHearingsForNotificationsQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<VideoHearing>> Handle(GetHearingsForNotificationsQuery query)
        {
            var videoHearing = VideoHearings.Get(_context);

            var startDate = DateTime.Today.AddDays(2);  // 2 days is 48 hrs
            var endDate = DateTime.Today.AddDays(3);    // 3 days is 72 hrs.

            return await videoHearing.Where(x =>
                    (x.Status == BookingStatus.Created ||
                     x.Status == Domain.Enumerations.BookingStatus.ConfirmedWithoutJudge ||
                     x.Status == Domain.Enumerations.BookingStatus.BookedWithoutJudge ||
                     x.Status == Domain.Enumerations.BookingStatus.Booked)
                     && x.ScheduledDateTime >= startDate
                     && x.ScheduledDateTime < endDate)
                .ToListAsync();
        }
    }
}