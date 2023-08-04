using BookingsApi.Domain.Participants;

namespace BookingsApi.DAL.Queries
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

        public async Task<List<Participant>> Handle(GetParticipantsInHearingQuery query)
        {
            var hearing = await _context.VideoHearings
                .Include("Participants.Person.Organisation")
                .Include("Participants.HearingRole.UserRole")
                .Include("Participants.CaseRole")
                .SingleOrDefaultAsync(x => x.Id == query.HearingId);
            if (hearing == null)
            {
                throw new HearingNotFoundException(query.HearingId);
            }    
            return hearing.GetParticipants().ToList();
        }
    }
}