using System.Threading.Tasks;
using Bookings.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bookings.DAL.Queries
{
    public class GetPersonByUsernameQuery : IQuery
    {
        public string Username { get; set; }

        public GetPersonByUsernameQuery(string username)
        {
            Username = username;
        }
    }
    
    public class GetPersonByUsernameQueryHandler : IQueryHandler<GetPersonByUsernameQuery, Person>
    {
        private readonly BookingsDbContext _context;

        public GetPersonByUsernameQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<Person> Handle(GetPersonByUsernameQuery query)
        {
            return await _context.Persons
                .Include(x => x.Address)
                .Include(x => x.Organisation)
                .SingleOrDefaultAsync(x => x.Username == query.Username);   
            
        }
    }
}