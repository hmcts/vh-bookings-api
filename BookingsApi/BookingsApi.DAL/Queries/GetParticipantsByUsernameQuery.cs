using BookingsApi.Domain.Participants;

namespace BookingsApi.DAL.Queries
{
    public class GetParticipantsByUsernameQuery : IQuery
    {
        public string Username { get; set; }

        public GetParticipantsByUsernameQuery(string username)
        {
            Username = username;
        }
    }
    
    public class GetParticipantsByUsernameQueryHandler : IQueryHandler<GetParticipantsByUsernameQuery, List<Participant>>
    {
        private readonly BookingsDbContext _context;

        public GetParticipantsByUsernameQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<Participant>> Handle(GetParticipantsByUsernameQuery query)
        {
            return await _context.Participants
                .Include(x => x.Person)
                .Include(x => x.HearingRole)
                .Include(x => x.HearingRole.UserRole)
                .Include(x => x.CaseRole)
                .Where(x => x.Person.Username == query.Username)
                .ToListAsync();   
        }
    }
}