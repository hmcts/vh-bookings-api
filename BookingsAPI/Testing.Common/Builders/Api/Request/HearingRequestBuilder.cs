using System;
using FizzWare.NBuilder;
using VhListings.Api.Contract.Requests;
using VhListings.Domain.Enumerations;

namespace Testing.Common.Builders.Api.Request
{
    public class HearingRequestBuilder
    {
        private readonly HearingRequest _request;

        public HearingRequestBuilder()
        {
            _request = Builder<HearingRequest>.CreateNew()
                .WithFactory(() => new HearingRequest())
                .With(x => x.ScheduledDateTime = DateTime.Today.AddHours(10))
                .With(x => x.ScheduledDuration = 120)
                .Build();
        }

        public HearingRequestBuilder WithAdministrator()
        {
            WithParticipant(Role.Administrator);
            return this;
        }

        public HearingRequestBuilder WithCitizen()
        {
            WithParticipant(Role.Citizen);
            return this;
        }
        
        public HearingRequestBuilder WithProfessional()
        {
            WithParticipant(Role.Professional);
            return this;
        }

        public HearingRequestBuilder WithJudge()
        {
            WithParticipant(Role.Judge);
            return this;
        }

        public HearingRequestBuilder WithCase()
        {
            _request.Cases.Add(Builder<CaseRequest>.CreateNew().Build());
            return this;
        }
        
        public HearingRequestBuilder WithCourt(int courtId)
        {
            _request.CourtId = courtId;
            return this;
        }
        
        private void WithParticipant(Role role)
        {
            var participant = new ParticipantRequestBuilder().WithRole(role).Build();
            _request.Participants.Add(participant);
        }

        public HearingRequest Build()
        {
            return _request;
        }

        public static HearingRequest BasicRequest()
        {
            return new HearingRequestBuilder()
                .WithCourt(1)
                .WithCase()
                .WithJudge()
                .Build();
        }
    }
}