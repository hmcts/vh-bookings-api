using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain.Participants;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries.V1
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