using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL.Queries.Core;
using Bookings.Domain.Participants;
using Microsoft.EntityFrameworkCore;

namespace Bookings.DAL.Queries
{
    public class GetParticipantsByUsernameQuery : IQuery
    {
        public string Username { get; set; }

        public GetParticipantsByUsernameQuery(string username)
        {
            Username = username;
        }
    }
    
    public class GetParticipantsByUsernameQueryHandler : IQueryHandler<GetParticipantsByUsernameQuery, IEnumerable<Participant>>
    {
        private readonly BookingsDbContext _context;

        public GetParticipantsByUsernameQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Participant>> Handle(GetParticipantsByUsernameQuery query)
        {
            return await _context.Participants
                .Include(x => x.Person)
                .Include(x => x.HearingRole)
                .Include(x => x.CaseRole)
                .Where(x => x.Person.Username == query.Username)
                .ToListAsync();   
        }
    }
}