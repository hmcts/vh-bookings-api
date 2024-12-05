using BookingsApi.Contract.V2.Responses;

namespace BookingsApi.Mappings.V2
{
    public static class PersonToResponseV2Mapper
    {
        public static PersonResponseV2 MapPersonToResponse(Person person)
        {
            return new PersonResponseV2
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