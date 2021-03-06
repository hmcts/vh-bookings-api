﻿using BookingsApi.Contract.Responses;
using BookingsApi.Domain;

namespace BookingsApi.Mappings
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
