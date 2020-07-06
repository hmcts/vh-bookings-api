using Bookings.DAL.Queries.Core;
using Bookings.Domain;
using Bookings.Domain.Enumerations;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookings.DAL.Queries
{
    public class GetHearingsByCaseNumberQuery : IQuery
    {
        public string CaseNumber { get; set; }

        public GetHearingsByCaseNumberQuery(string caseNumber)
        {
            CaseNumber = caseNumber;
        }
    }

    public class GetHearingsByCaseNumberQueryHandler : IQueryHandler<GetHearingsByCaseNumberQuery, List<VideoHearing>>
    {
        private readonly BookingsDbContext _context;

        public GetHearingsByCaseNumberQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<VideoHearing>> Handle(GetHearingsByCaseNumberQuery query)
        {
            var caseNumber  = query.CaseNumber.ToLower().Trim();
            return  await _context.VideoHearings
                .Include("Participants.Person")
                .Include("Participants.Person.Address")
                .Include("HearingCases.Case")
                .Include("Participants.Person.Organisation")
                .Include(x => x.CaseType)
                .ThenInclude(x => x.CaseRoles)
                .ThenInclude(x => x.HearingRoles)
                .ThenInclude(x => x.UserRole)
                .Where(x => x.AudioRecordingRequired && x.Status == BookingStatus.Created 
                    && x.HearingCases.Any(c => c.Case.Number.ToLower().Trim() == caseNumber.ToLower().Trim()))
                .ToListAsync();
        }
    }
}