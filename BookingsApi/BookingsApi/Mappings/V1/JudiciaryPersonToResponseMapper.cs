﻿using BookingsApi.Contract.V1.Responses;

namespace BookingsApi.Mappings.V1
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
