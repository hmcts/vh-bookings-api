using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL.Queries.Core;
using Bookings.Domain;
using Bookings.Domain.Enumerations;
using Microsoft.EntityFrameworkCore;

namespace Bookings.DAL.Queries
{
    public class GetHearingsForUsernameQuery : IQuery
    {
        public string Username { get; set; }

        public GetHearingsForUsernameQuery(string username)
        {
            Username = username;
        }
    }
    
    public class GetHearingForUsernameQueryHandler : IQueryHandler<GetHearingsForUsernameQuery, List<VideoHearing>>
    {
        private readonly BookingsDbContext _context;

        public GetHearingForUsernameQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<VideoHearing>> Handle(GetHearingsForUsernameQuery query)
        {
            var hearingsForUsername = await _context.VideoHearings
                .Where(x => 
                    x.Participants.Any(y => y.Person.Username == query.Username) &&
                    x.Status != HearingStatus.Closed
                    )
                .AsNoTracking().ToListAsync();

            return hearingsForUsername;
        }
    }
}