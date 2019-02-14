using System;
using System.Collections.Generic;
using FizzWare.NBuilder;
using VhListings.Api.Contract.Requests;
using VhListings.Domain.Enumerations;

namespace Testing.Common.Builders.Api.Request
{
    public class ParticipantRequestBuilder
    {
        private readonly ParticipantRequest _request;

        public ParticipantRequestBuilder()
        {
            _request = Builder<ParticipantRequest>.CreateNew()
                .WithFactory(() => new ParticipantRequest())
                .With(x => x.Role = Role.Citizen.ToString())
                .With(x => x.Title = Faker.Name.Prefix())
                .With(x => x.FirstName = Faker.Name.First())
                .With(x => x.LastName = Faker.Name.Last())
                .With(x => x.Username = $"{DateTime.Now:yyyy_MM_dd_HH_mm_ss_fff}_{Faker.Internet.Email()}")
                .With(x => x.ContactEmail = $"{DateTime.Now:yyyy_MM_dd_HH_mm_ss_fff}_{Faker.Internet.Email()}")
                .Build();
        }

        public ParticipantRequestBuilder WithEmail(string email)
        {
            _request.Username = email;
            return this;
        }
        
        public ParticipantRequestBuilder WithRole(Role role)
        {
            _request.Role = role.ToString();
            return this;
        }

        public ParticipantRequest Build()
        {
            return _request;
        }

        public static List<ParticipantRequest> BuildListOfValidParticipantsRequest()
        {
            return new List<ParticipantRequest>
            {
                new ParticipantRequestBuilder().WithRole(Role.Judge).Build(),
                new ParticipantRequestBuilder().WithRole(Role.Administrator).Build(),
                new ParticipantRequestBuilder().WithRole(Role.Citizen).Build(),
                new ParticipantRequestBuilder().WithRole(Role.Professional).Build()
            };
        }
    }
}