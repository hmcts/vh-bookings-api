﻿using BookingsApi.Contract.V1.Requests;
using Faker;
using FizzWare.NBuilder;

namespace Testing.Common.Builders.Api.V1.Request
{
    public class UpdateParticipantRequestBuilder
    {
        private readonly UpdateParticipantRequest _updateParticipantRequest;

        public UpdateParticipantRequestBuilder()
        {
            _updateParticipantRequest = Builder<UpdateParticipantRequest>.CreateNew()
                .With(x => x.Title = Name.Prefix())
                .With(x => x.TelephoneNumber = Phone.Number())
                .With(x => x.DisplayName = Name.FullName())
                .With(x=>x.OrganisationName = Company.Name())
                .With(x=>x.Representee = Name.FullName())
                .With(x =>  x.ContactEmail = "")
                .Build();
        }

        public UpdateParticipantRequest Build()
        {
            return _updateParticipantRequest;
        }
    }
}