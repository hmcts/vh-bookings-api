using System.Diagnostics.CodeAnalysis;

namespace BookingsApi.DAL.Queries;
public class GetHearingRolesQuery : IQuery { }

[ExcludeFromCodeCoverage]
public class GetHearingRolesQueryHandler : IQueryHandler<GetHearingRolesQuery, List<HearingRole>>
{
    private readonly BookingsDbContext _context;

    public GetHearingRolesQueryHandler(BookingsDbContext context) => _context = context;

    public async Task<List<HearingRole>> Handle(GetHearingRolesQuery query)
    {
        var hearingRoles = await _context.HearingRoles
            .Include(hr => hr.UserRole)
            .Where(hr => hr.CaseRoleId == null)
            .ToListAsync();
        return hearingRoles;

    }
}
