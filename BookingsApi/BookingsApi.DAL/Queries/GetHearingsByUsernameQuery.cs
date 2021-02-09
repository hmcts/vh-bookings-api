using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Domain;
using BookingsApi.DAL.Queries.Core;
using Microsoft.EntityFrameworkCore;

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
            return await _context.VideoHearings
                .Include("Participants.Person")
                .Include("Participants.Questionnaire")
                .Include("Participants.Questionnaire.SuitabilityAnswers")
                .Include(x => x.Participants).ThenInclude(x => x.LinkedParticipants)
                .Include("HearingCases.Case")
                .Include("Participants.Person.Organisation")
                .Include(x => x.CaseType)
                .ThenInclude(x => x.CaseRoles)
                .ThenInclude(x => x.HearingRoles)
                .ThenInclude(x=>x.UserRole)
                .Include(x => x.HearingType)
                .Include(x => x.HearingVenue)
                .Include(x => x.Endpoints)
                .Where(x => x.Participants.Any(p => p.Person.Username.ToLower().Trim() == username))
                .ToListAsync();
        }
    }
}