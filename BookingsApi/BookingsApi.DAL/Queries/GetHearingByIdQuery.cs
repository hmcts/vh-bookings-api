namespace BookingsApi.DAL.Queries
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

        public async Task<VideoHearing> Handle(GetHearingByIdQuery query)
        {
            return await _context.VideoHearings
                .Include(x => x.Participants).ThenInclude(x=> x.Person).ThenInclude(x => x.Organisation)
                .Include(x => x.Participants).ThenInclude(x=> x.CaseRole)
                .Include(x => x.Participants).ThenInclude(x=> x.HearingRole).ThenInclude(x => x.UserRole)
                .Include(x => x.Participants).ThenInclude(x => x.LinkedParticipants)
                .Include(x=> x.Participants).ThenInclude(x=> x.InterpreterLanguage)
                .Include("HearingCases.Case")
                .Include(x => x.CaseType)
                .ThenInclude(x => x.CaseRoles)
                .ThenInclude(x => x.HearingRoles)
                .ThenInclude(x=>x.UserRole)
                .Include(x => x.HearingType)
                .Include(x => x.HearingVenue)
                .Include(x => x.Endpoints).ThenInclude(x => x.DefenceAdvocate).ThenInclude(x => x.Person)
                .Include(x=> x.Endpoints).ThenInclude(x=> x.InterpreterLanguage)
                .Include(x => x.Allocations).ThenInclude(x => x.JusticeUser)
                .Include(x => x.JudiciaryParticipants).ThenInclude(x => x.JudiciaryPerson)
                .Include(x=> x.JudiciaryParticipants).ThenInclude(x=> x.InterpreterLanguage)
                
                // keep the following includes for the screening entities - cannot auto include due to cyclic dependency
                .Include(x => x.Participants).ThenInclude(x => x.Screening).ThenInclude(x=> x.ScreeningEntities).ThenInclude(x=> x.Participant)
                .Include(x => x.Participants).ThenInclude(x => x.Screening).ThenInclude(x=> x.ScreeningEntities).ThenInclude(x=> x.Endpoint)
                .Include(x => x.Endpoints).ThenInclude(x => x.Screening).ThenInclude(x=> x.ScreeningEntities).ThenInclude(x=> x.Participant)
                .Include(x => x.Endpoints).ThenInclude(x => x.Screening).ThenInclude(x=> x.ScreeningEntities).ThenInclude(x=> x.Endpoint)
                .AsNoTracking()
                .AsSplitQuery()
                .SingleOrDefaultAsync(x => x.Id == query.HearingId);
        }
    }
}