using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries.V1
{
    public class GetJusticeUserListQuery : IQuery
    {
        public string Term { get; set; }
        public bool IncludeDeleted { get; set; }
        public GetJusticeUserListQuery(string term, bool includeDeleted = false)
        {
            Term = term;
            IncludeDeleted = includeDeleted;
        }
    }
    
    public class GetJusticeUserListQueryHandler : IQueryHandler<GetJusticeUserListQuery, List<JusticeUser>>
    {
        private readonly BookingsDbContext _context;

        public GetJusticeUserListQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<JusticeUser>> Handle(GetJusticeUserListQuery query)
        {
            var term = query.Term;
            
            var users = _context.JusticeUsers.IgnoreQueryFilters()
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
}