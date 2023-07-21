using BookingsApi.Contract.V1.Responses;
using BookingsApi.Domain;

namespace BookingsApi.UnitTests.Mappings
{
    public class JudiciaryPersonToResponseMapper
    {
        public PersonResponse MapJudiciaryPersonToResponse(JudiciaryPerson person)
        {
            return new PersonResponse
            {
                Id = person.Id,
                Title = person.Title,
                FirstName = person.KnownAs,
                LastName = person.Surname,
                Username = person.Email,
            };
        }
    }
}
