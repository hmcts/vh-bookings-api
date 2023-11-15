using BookingsApi.Contract.V1.Responses;

namespace BookingsApi.Mappings.V1
{
    public class JudiciaryPersonToResponseMapper
    {
        public JudiciaryPersonResponse MapJudiciaryPersonToResponse(JudiciaryPerson person)
        {
            return new JudiciaryPersonResponse()
            {
                Title = person.Title,
                FirstName = person.KnownAs,
                LastName = person.Surname,
                FullName = person.Fullname,
                PersonalCode = person.PersonalCode,
                Email = person.Email.ToLower(),
            };
        }
    }
}
