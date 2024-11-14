using BookingsApi.Domain.Participants;

namespace BookingsApi.DAL.Queries;

public class GetHearingsByUsernameForDeletionQuery(string username) : IQuery
{
    public string Username { get; } = username;
}

public class
    GetHearingsByUsernameForDeletionQueryHandler(BookingsDbContext context)
    : IQueryHandler<GetHearingsByUsernameForDeletionQuery,
        List<VideoHearing>>
{
    public async Task<List<VideoHearing>> Handle(GetHearingsByUsernameForDeletionQuery query)
    {
        var username = query.Username.ToLower().Trim();

        var person = await context.Persons.SingleOrDefaultAsync(x => x.Username.ToLower().Trim() == username);

        if (person == null)
        {
            throw new PersonNotFoundException(username);
        }
            
        var allHearings = await context.VideoHearings
            .Include(x => x.Participants).ThenInclude(x => x.HearingRole).ThenInclude(x => x.UserRole)
            .Include(x => x.Participants).ThenInclude(x => x.Person)
            .Include(x => x.HearingCases).ThenInclude(x => x.Case)
            .Include(x => x.CaseType)
            .Include(x => x.HearingVenue)
            .Where(x => x.Participants.Any(p => p.Person.Username.ToLower().Trim() == username))
            .OrderByDescending(h => h.ScheduledDateTime)
            .AsNoTracking().ToListAsync();

        var filteredHearings = FilterHearingsWithRoleAsRepOrIndividual(allHearings, query.Username);
        return filteredHearings.ToList();
    }

    private static IEnumerable<VideoHearing> FilterHearingsWithRoleAsRepOrIndividual(List<VideoHearing> allHearings,
        string username)
    {
        foreach (var hearing in allHearings)
        {
            var p = hearing.GetParticipants().First(x =>
                x.Person.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase));
            if (p is Judge)
            {
                throw new PersonIsAJudgeException(username);
            }

            yield return hearing;
        }
    }
}