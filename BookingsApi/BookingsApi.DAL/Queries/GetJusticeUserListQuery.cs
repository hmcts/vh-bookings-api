namespace BookingsApi.DAL.Queries;

public class GetJusticeUserListQuery(string term, bool includeDeleted = false) : IQuery
{
    public string Term { get; set; } = term;
    public bool IncludeDeleted { get; set; } = includeDeleted;
}
    
public class GetJusticeUserListQueryHandler(BookingsDbContext context)
    : IQueryHandler<GetJusticeUserListQuery, List<JusticeUser>>
{
    public async Task<List<JusticeUser>> Handle(GetJusticeUserListQuery query)
    {
        var term = query.Term;
            
        var users = context.JusticeUsers.IgnoreQueryFilters()
            .Where(x => query.IncludeDeleted.Equals(true) || x.Deleted.Equals(false))
            .OrderBy(x => x.Lastname).ThenBy(x => x.FirstName)
            .Include(x => x.JusticeUserRoles).ThenInclude(x => x.UserRole)
            .AsQueryable();

        if (!string.IsNullOrEmpty(term))
        {
            users = users
                .Where(u =>
                    u.Lastname.Contains(term) ||
                    u.FirstName.Contains(term) ||
                    u.ContactEmail.Contains(term) ||
                    u.Username.Contains(term));
        }

        return await users.ToListAsync();
    }
}