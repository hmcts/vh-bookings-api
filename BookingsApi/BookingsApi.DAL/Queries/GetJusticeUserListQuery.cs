using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Domain;
using BookingsApi.DAL.Queries.Core;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries
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
            string term = query.Term;

            if (string.IsNullOrEmpty(term))
            {
                return await _context.JusticeUsers.IgnoreQueryFilters()
                    .Where(x => query.IncludeDeleted.Equals(true) || x.Deleted.Equals(false))
                    .Include(x => x.JusticeUserRoles).ThenInclude(jur => jur.UserRole).ToListAsync();
            }
            else
            {
                return await _context.JusticeUsers.IgnoreQueryFilters()
                    .Where(u=>
                        (query.IncludeDeleted.Equals(true) || u.Deleted.Equals(false)) &&
                        u.Lastname.Contains(term) || 
                        u.FirstName.Contains(term) ||
                        u.ContactEmail.Contains(term) ||
                        u.Username.Contains(term)
                    )
                    .Include(x => x.JusticeUserRoles).ThenInclude(jur => jur.UserRole)
                    .ToListAsync();
            }
            
        }
    }
}