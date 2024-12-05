namespace BookingsApi.DAL.Queries.BaseQueries
{
    public static class VideoHearings
    {
        public static IQueryable<VideoHearing> Get(BookingsDbContext context)
        {
            return context.VideoHearings
                .Include(x => x.Participants).ThenInclude(x => x.Person).ThenInclude(x => x.Organisation)
                .Include(x => x.Participants)
                .Include(x => x.Participants).ThenInclude(x => x.HearingRole).ThenInclude(x => x.UserRole)
                .Include(x => x.Participants).ThenInclude(x => x.LinkedParticipants)
                .Include(x => x.JudiciaryParticipants).ThenInclude(x => x.JudiciaryPerson)
                .Include(x=> x.HearingCases).ThenInclude(x=> x.Case)
                .Include(x => x.CaseType)
                .Include(x => x.HearingVenue)
                .Include(x => x.Endpoints).ThenInclude(x => x.DefenceAdvocate).ThenInclude(x => x.Person)
                .Include(x => x.Allocations).ThenInclude(x => x.JusticeUser)
                .AsNoTracking()
                .AsSplitQuery();
        }
    }
}