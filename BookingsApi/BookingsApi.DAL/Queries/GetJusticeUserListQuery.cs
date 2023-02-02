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
        public GetJusticeUserListQuery(string term)
        {
            Term = term;
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
                return await _context.JusticeUsers.Include(x => x.UserRole).ToListAsync();
            }
            else
            {
                return await _context.JusticeUsers
                    .Where(u=> 
                        u.Lastname.Contains(term) || 
                        u.FirstName.Contains(term) ||
                        u.ContactEmail.Contains(term) ||
                        u.Username.Contains(term)
                        )
                    .Include(x => x.UserRole).ToListAsync();
            }
            
        }
    }
}