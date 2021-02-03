using BookingsApi.Contract.Responses;
using BookingsApi.Domain;

namespace BookingsApi.Mappings
{
    public class PersonToResponseMapper
    {
        public PersonResponse MapPersonToResponse(Person person)
        {
            return new PersonResponse
            {
                Id = person.Id,
                Title = person.Title,
                FirstName = person.FirstName,
                LastName = person.LastName,
                MiddleNames = person.MiddleNames,
                Username = person.Username,
                ContactEmail = person.ContactEmail,
                TelephoneNumber = person.TelephoneNumber,
                Organisation = person.Organisation?.Name,
            }; 
        }
    }
}