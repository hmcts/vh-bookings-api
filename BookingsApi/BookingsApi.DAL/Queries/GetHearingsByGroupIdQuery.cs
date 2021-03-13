using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries
{
    public class GetHearingsByGroupIdQuery : IQuery
    {
        public GetHearingsByGroupIdQuery(Guid groupId)
        {
            GroupId = groupId;
        }

        public Guid GroupId { get; }
    }

    public class GetHearingsByGroupIdQueryHandler : IQueryHandler<GetHearingsByGroupIdQuery, List<VideoHearing>>
    {
        private readonly BookingsDbContext _context;

        public GetHearingsByGroupIdQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<VideoHearing>> Handle(GetHearingsByGroupIdQuery query)
        {
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
                .Where(x => x.SourceId == query.GroupId)
                .OrderBy(x => x.ScheduledDateTime)
                .ToListAsync();
        }
    }
}