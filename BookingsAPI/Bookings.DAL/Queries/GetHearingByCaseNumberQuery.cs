using Bookings.DAL.Queries.Core;
using Bookings.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Bookings.DAL.Queries
{
    public class GetHearingByCaseNumberQuery : IQuery
    {
        public string CaseNumber { get; set; }

        public GetHearingByCaseNumberQuery(string caseNumber)
        {
            CaseNumber = caseNumber;
        }
    }

    public class GetHearingByCaseNumberQueryHandler : IQueryHandler<GetHearingByCaseNumberQuery, VideoHearing>
    {
        private readonly BookingsDbContext _context;

        public GetHearingByCaseNumberQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<VideoHearing> Handle(GetHearingByCaseNumberQuery query)
        {
            return await _context.VideoHearings
                .Include("Participants.Person")
                .Include("Participants.Person.Address")
                .Include("HearingCases.Case")
                .Include("Participants.Person.Organisation")
                .Include(x => x.CaseType)
                .ThenInclude(x => x.CaseRoles)
                .ThenInclude(x => x.HearingRoles)
                .ThenInclude(x => x.UserRole)
                .SingleOrDefaultAsync(x => x.AudioRecordingRequired && x.HearingCases.Any(c => c.Case.Number.ToLower() == query.CaseNumber.ToLower()));
        }
    }
}