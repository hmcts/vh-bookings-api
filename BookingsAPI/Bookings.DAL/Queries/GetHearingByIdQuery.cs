using System;
using System.Linq;
using Bookings.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bookings.DAL.Queries
{
    public class GetHearingByIdQuery : IQuery
    {
        public Guid HearingId { get; set; }

        public GetHearingByIdQuery(Guid hearingId)
        {
            HearingId = hearingId;
        }
    }

    public class GetHearingByIdQueryHandler : IQueryHandler<GetHearingByIdQuery, VideoHearing>
    {
        private readonly BookingsDbContext _context;

        public GetHearingByIdQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public VideoHearing Handle(GetHearingByIdQuery query)
        {
            return _context.VideoHearings
                .Include(x => x.Participants).ThenInclude(x => x.Person)
//                .Include("Participants.Person")
                .Include("HearingCases.Case")
                .Include(x => x.CaseType)
                .ThenInclude(x => x.CaseRoles)
                .ThenInclude(x => x.HearingRoles)
                .ThenInclude(x=>x.UserRole)
                .Include(x => x.HearingType)
                .Include(x => x.HearingVenue)
                .SingleOrDefault(x => x.Id == query.HearingId);
        }
    }
}