namespace BookingsApi.DAL.Queries;

/// <summary>
/// Get all standard hearing roles (i.e. not linked to a case role)
/// </summary>
public class GetHearingRolesQuery : IQuery
{
    
}

public class GetHearingRolesQueryHandler : IQueryHandler<GetHearingRolesQuery, List<HearingRole>>
{
    private readonly BookingsDbContext _context;

    public GetHearingRolesQueryHandler(BookingsDbContext context)
    {
        _context = context;
    }

    public async Task<List<HearingRole>> Handle(GetHearingRolesQuery query)
    {
        return await _context.HearingRoles.Include(x=> x.UserRole)
            .Where(hr => hr.CaseRoleId == null)
            .ToListAsync();
    }
}