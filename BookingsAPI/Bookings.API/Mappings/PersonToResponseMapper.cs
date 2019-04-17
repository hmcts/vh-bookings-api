using Bookings.Api.Contract.Responses;
using Bookings.Domain;

namespace Bookings.API.Mappings
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
                HouseNumber = person.Address?.Postcode,
                Street = person.Address?.Street,
                City  = person.Address?.City,
                County = person.Address?.County,
                Postcode = person.Address?.Postcode
            };
        }
    }
}