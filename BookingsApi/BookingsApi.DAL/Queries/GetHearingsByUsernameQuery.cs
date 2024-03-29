using BookingsApi.DAL.Queries.BaseQueries;

namespace BookingsApi.DAL.Queries
{
    public class GetHearingsByUsernameQuery : IQuery
    {
        public GetHearingsByUsernameQuery(string username)
        {
            Username = username;
        }

        public string Username { get; }
    }

    public class GetHearingsByUsernameQueryHandler : IQueryHandler<GetHearingsByUsernameQuery, List<VideoHearing>>
    {
        private readonly BookingsDbContext _context;

        public GetHearingsByUsernameQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<VideoHearing>> Handle(GetHearingsByUsernameQuery query)
        {
            var username = query.Username.ToLower().Trim();
            var videoHearing = VideoHearings.Get(_context);

            return await videoHearing
                .Where(x => x.Participants.Any(p => p.Person.Username.ToLower().Trim() == username))
                .ToListAsync();
        }
    }
}