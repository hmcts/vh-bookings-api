namespace BookingsApi.DAL.Queries
{
    public class GetAllCaseTypesQuery : IQuery
    {
        public GetAllCaseTypesQuery(bool hideExpired)
        {
            HideExpired = hideExpired;
        }

        public bool HideExpired { get; }
    }
    
    public class GetAllCaseTypesQueryHandler : IQueryHandler<GetAllCaseTypesQuery, List<CaseType>>
    {
        private readonly BookingsDbContext _context;

        public GetAllCaseTypesQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<CaseType>> Handle(GetAllCaseTypesQuery query)
        {
            var dbQuery = _context.CaseTypes.Include(x=> x.HearingTypes).AsNoTracking();

            if (!query.HideExpired)
            {
                return await dbQuery.ToListAsync();
            }
            
            // exclude case and hearing types that have expired
            dbQuery = dbQuery.Where(x => x.ExpirationDate == null || x.ExpirationDate >= DateTime.Today);

            var caseTypes = await dbQuery.ToListAsync();
            foreach (var caseType in caseTypes)
            {
                caseType.HearingTypes = caseType.HearingTypes.Where(x => !x.HasExpired()).ToList();
            }

            return caseTypes;
        }
    }
}