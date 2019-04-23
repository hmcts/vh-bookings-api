using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL.Queries.Core;
using Bookings.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bookings.DAL.Queries
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
            return await _context.VideoHearings
                .Include("Participants.Person")
                .Include("Participants.Person.Address")
                .Include("HearingCases.Case")
                .Include("Participants.Person.Organisation")
                .Include(x => x.CaseType)
                .ThenInclude(x => x.CaseRoles)
                .ThenInclude(x => x.HearingRoles)
                .ThenInclude(x=>x.UserRole)
                .Include(x => x.HearingType)
                .Include(x => x.HearingVenue)
                .Where(x => x.Participants.Any(p => p.Person.Username.ToLower().Trim() == username))
                .ToListAsync();
        }
    }
}