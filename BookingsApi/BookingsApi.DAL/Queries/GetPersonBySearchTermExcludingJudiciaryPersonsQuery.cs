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
        public List<string> JudiciaryUsersFromAD { get; }

        public GetPersonBySearchTermExcludingJudiciaryPersonsQuery(string term, List<string> judiciaryUsersFromAD)
        {
            Term = term;
            JudiciaryUsersFromAD = judiciaryUsersFromAD;
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
            var persons = await _context.Persons
                                .Include(x => x.Organisation)
                                .Where(x => x.ContactEmail.ToLower().Contains(query.Term.ToLowerInvariant()))
                                .ToListAsync();

            var judiciaryPersons = await _context.JudiciaryPersons.Select(x => x.Email.ToLowerInvariant()).ToListAsync();
            var accountsToFilter = judiciaryPersons.Concat(query.JudiciaryUsersFromAD).ToList();
            //var filteredOutList = persons.Where(
            //    x => 
            //    (!string.IsNullOrEmpty(x.ContactEmail) && !judiciaryPersons.Contains(x.ContactEmail.ToLowerInvariant())) &&
            //    (!string.IsNullOrEmpty(x.Username) && !judiciaryPersons.Contains(x.Username.ToLowerInvariant()))
            //    ).ToList();

            var filteredOutList = persons.Where(
              person => DoesJudiciaryPersonExistInPersons(person.ContactEmail, accountsToFilter) &&
                        DoesJudiciaryPersonExistInPersons(person.Username, accountsToFilter))
                        .ToList();

            return filteredOutList;
        }

        private bool DoesJudiciaryPersonExistInPersons(string email, List<string> judiciaryPersons)
        {
            return (!string.IsNullOrEmpty(email) && !judiciaryPersons.Contains(email.ToLowerInvariant()));
        }

    }

    //public static class Extension
    //{
    //    public static bool DoesJudiciaryPersonExistInPersons(this string email, List<string> judiciaryPersons)
    //    {
    //        return (!string.IsNullOrEmpty(email) && !judiciaryPersons.Contains(email.ToLowerInvariant()));
    //    }

    //}
}
