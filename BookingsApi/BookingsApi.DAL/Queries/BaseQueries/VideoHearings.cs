using System.Linq;
using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries.BaseQueries
{
    public static class VideoHearings
    {
        public static IQueryable<VideoHearing> Get(BookingsDbContext context)
        {
            return context.VideoHearings
                .Include("Participants.Person")
                .Include("Participants.Questionnaire")
                .Include("Participants.Questionnaire.SuitabilityAnswers")
                .Include(x => x.Participants).ThenInclude(x => x.LinkedParticipants)
                .Include("HearingCases.Case")
                .Include("Participants.Person.Organisation")
                .Include(x => x.CaseType)
                .ThenInclude(x => x.CaseRoles)
                .ThenInclude(x => x.HearingRoles)
                .ThenInclude(x => x.UserRole)
                .Include(x => x.HearingType)
                .Include(x => x.HearingVenue)
                .Include(x => x.Endpoints);
        }
    }
}