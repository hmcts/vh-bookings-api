﻿using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V2.Requests;
using Faker;
using FizzWare.NBuilder;

namespace Testing.Common.Builders.Api.V2
{
    public class ParticipantRequestBuilderV2
    {
        private readonly ParticipantRequestV2 _participantRequest;

        public ParticipantRequestBuilderV2(string caseRole, string hearingRole)
        {
            _participantRequest = Builder<ParticipantRequestV2>.CreateNew()
                .With(x => x.Title = Name.Prefix())
                .With(x => x.FirstName = "Automation_FirstName")
                .With(x => x.LastName = "Automation_LastName")
                .With(x => x.ContactEmail = $"Automation_{RandomNumber.Next()}@hmcts.net")
                .With(x => x.TelephoneNumber = "01234567890")
                .Build();
        }

        public ParticipantRequestBuilderV2 WithRepresentativeDetails(string representee)
        {
            _participantRequest.Representee = representee;
            return this;
        }

        public ParticipantRequestV2 Build()
        {
            return _participantRequest;
        }
    }
}