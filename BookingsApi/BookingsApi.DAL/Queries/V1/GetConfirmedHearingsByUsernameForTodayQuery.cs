using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL.Queries.BaseQueries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries.V1
{
    public class GetConfirmedHearingsByUsernameForTodayQuery : IQuery
    {
        public GetConfirmedHearingsByUsernameForTodayQuery(string username)
        {
            Username = username;
        }

        public string Username { get; }
    }

    public class GetConfirmedHearingsByUsernameForTodayQueryHandler : IQueryHandler<GetConfirmedHearingsByUsernameForTodayQuery, List<VideoHearing>>
    {
        private readonly BookingsDbContext _context;

        public GetConfirmedHearingsByUsernameForTodayQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<VideoHearing>> Handle(GetConfirmedHearingsByUsernameForTodayQuery query)
        {
            var username = query.Username.ToLower().Trim();
            var videoHearing = VideoHearings.Get(_context);

            return await videoHearing
                .Where(x =>
                    x.ScheduledDateTime.Date == System.DateTime.UtcNow.Date &&
                    x.Status == Domain.Enumerations.BookingStatus.Created && // Only Confirmed
                    x.Participants.Any(p => p.Person.Username.ToLower().Trim() == username))
                .ToListAsync();
        }
    }
}