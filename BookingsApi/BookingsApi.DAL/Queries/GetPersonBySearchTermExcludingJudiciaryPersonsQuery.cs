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
    public class GetPersonBySearchTermExcludingJudiciaryPersonsQuery : IQuery
    {
        public string Term { get; }

        public GetPersonBySearchTermExcludingJudiciaryPersonsQuery(string term)
        {
            Term = term;
        }

    }

    public class GetPersonBySearchTermExcludingJudiciaryPersonsQueryHandler : IQueryHandler<GetPersonBySearchTermExcludingJudiciaryPersonsQuery, List<Person>>
    {
        private readonly BookingsDbContext _context;

        public GetPersonBySearchTermExcludingJudiciaryPersonsQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }
        public async Task<List<Person>> Handle(GetPersonBySearchTermExcludingJudiciaryPersonsQuery query)
        {
            var judiciaryPersons = await _context.JudiciaryPersons.Select(x => x.Email.ToLowerInvariant()).ToListAsync();
            var persons = await _context.Persons.ToListAsync();

            //var filteredOutList = persons.Where(
            //    x => 
            //    (!string.IsNullOrEmpty(x.ContactEmail) && !judiciaryPersons.Contains(x.ContactEmail.ToLowerInvariant())) &&
            //    (!string.IsNullOrEmpty(x.Username) && !judiciaryPersons.Contains(x.Username.ToLowerInvariant()))
            //    ).ToList();

            var filteredOutList = persons.Where(
              person => person.ContactEmail.DoesJudidicaryPersonExistInPersons(judiciaryPersons) &&
                        person.Username.DoesJudidicaryPersonExistInPersons(judiciaryPersons))
                        .ToList();

            return filteredOutList;
        }

    }

    public static class Extension
    {
        public static bool DoesJudidicaryPersonExistInPersons(this string email, List<string> judiciaryPersons)
        {
            return (!string.IsNullOrEmpty(email) && !judiciaryPersons.Contains(email.ToLowerInvariant()));
        }

    }
}
