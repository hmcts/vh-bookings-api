namespace BookingsApi.DAL.Queries
{
    public class GetJudiciaryPersonByPersonalCodeQuery : IQuery
    {
        public string PersonalCode { get; set; }

        public GetJudiciaryPersonByPersonalCodeQuery(string personalCode)
        {
            PersonalCode = personalCode;
        }
    }
    
    public class GetJudiciaryPersonByPersonalCodeQueryHandler : IQueryHandler<GetJudiciaryPersonByPersonalCodeQuery, JudiciaryPerson>
    {
        private readonly BookingsDbContext _context;

        public GetJudiciaryPersonByPersonalCodeQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<JudiciaryPerson> Handle(GetJudiciaryPersonByPersonalCodeQuery query)
        {
            return await _context.JudiciaryPersons.SingleOrDefaultAsync(x => x.PersonalCode == query.PersonalCode);
        }
    }
}