using System;
using System.Collections.Generic;
using System.Linq;
using Bookings.Domain.Participants;
using Microsoft.EntityFrameworkCore;

namespace Bookings.DAL.Queries
{
    public class GetParticipantsInHearingQuery : IQuery
    {
        public Guid HearingId { get; set; }

        public GetParticipantsInHearingQuery(Guid hearingId)
        {
            HearingId = hearingId;
        }
    }
    
    public class GetParticipantsInHearingQueryHandler : IQueryHandler<GetParticipantsInHearingQuery, List<Participant>>
    {
        private readonly BookingsDbContext _context;

        public GetParticipantsInHearingQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public List<Participant> Handle(GetParticipantsInHearingQuery query)
        {
            return _context.VideoHearings
                .Include(x => x.Participants).ThenInclude(x => x.HearingRole)
                .Include(x => x.Participants).ThenInclude(x => x.CaseRole)
                .Include(x => x.Participants).ThenInclude(x => x.Person).ThenInclude(x=> x.Address)
                .Include(x => x.Participants).ThenInclude(x => x.Person).ThenInclude(x=> x.Organisation)
                .SingleOrDefault(x => x.Id == query.HearingId)?
                .GetParticipants().ToList();
        }
    }
}