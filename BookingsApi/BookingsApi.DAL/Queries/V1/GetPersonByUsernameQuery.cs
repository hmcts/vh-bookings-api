using System.Threading.Tasks;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries.V1
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
                .Include(x => x.Organisation)
                .SingleOrDefaultAsync(x => x.Username == query.Username);   
            
        }
    }
}