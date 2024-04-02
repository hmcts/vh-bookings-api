namespace BookingsApi.DAL.Queries;
public class GetHearingsByJudiciaryPersonQuery : IQuery
{
    public string JudiciaryEmail { get; }
    public GetHearingsByJudiciaryPersonQuery(string email) => JudiciaryEmail = email.ToLowerInvariant();
}

public class GetHearingsByJudiciaryPersonQueryHandler : IQueryHandler<GetHearingsByJudiciaryPersonQuery, List<VideoHearing>>
{
    private readonly BookingsDbContext _context;

    public GetHearingsByJudiciaryPersonQueryHandler(BookingsDbContext context) => _context = context;

    public async Task<List<VideoHearing>> Handle(GetHearingsByJudiciaryPersonQuery query)
    {
        var judiciaryPerson = await _context.JudiciaryPersons
            .Where(x => x.Email.ToLower().Contains(query.JudiciaryEmail.ToLower()) && !x.HasLeft)
            .FirstOrDefaultAsync();

        if (judiciaryPerson == null)
            throw new JudiciaryPersonNotFoundException(query.JudiciaryEmail);

        return await _context.VideoHearings
            .Include(x => x.JudiciaryParticipants)
            .Where(x => x.ScheduledDateTime.Date == DateTime.Today.Date &&
                        x.JudiciaryParticipants.Any(p => p.JudiciaryPersonId == judiciaryPerson.Id) && 
                        x.Status == BookingStatus.Created)
            .ToListAsync();
    }
}

