using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingsApi.DAL.Queries
{
    public class GetPersonBySearchTermAndAccountTypeQuery : IQuery
    {
        public string Term { get; }
        public List<string> AccountType { get; }

        public GetPersonBySearchTermAndAccountTypeQuery(string term, List<string> accountType = null)
        {
            Term = term.ToLowerInvariant();
            AccountType = accountType;
        }
    }

    public class GetPersonBySearchTermAndAccountTypeQueryHandler : IQueryHandler<GetPersonBySearchTermAndAccountTypeQuery, List<Person>>
    {
        private readonly BookingsDbContext _context;

        public GetPersonBySearchTermAndAccountTypeQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<Person>> Handle(GetPersonBySearchTermAndAccountTypeQuery query)
        {
            List<Person> persons;
            if (query.AccountType == null || query.AccountType.Count == 0)
            {
                persons = await _context.Persons
                    .Include(x => x.Organisation)
                    .Where(x => x.ContactEmail.ToLower().Contains(query.Term.ToLower()) && string.IsNullOrWhiteSpace(x.AccountType))
                    .ToListAsync();
            }
            else
            {
                persons = await _context.Persons
                    .Include(x => x.Organisation)
                    .Where(x => x.ContactEmail.ToLower().Contains(query.Term.ToLower()) &&
                                query.AccountType.Any(specifiedAccountType => specifiedAccountType == x.AccountType))
                    .ToListAsync();
            }
            return persons;
        }
    }
}
