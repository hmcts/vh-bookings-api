using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.AcceptanceTests.Contexts;
using BookingsApi.Contract.Requests;
using FizzWare.NBuilder;

namespace BookingsApi.AcceptanceTests.Models
{
    internal class CreateHearingRequestBuilder
    {
        private TestContext _context;
        private readonly BookNewHearingRequest _request;

        public CreateHearingRequestBuilder(string caseName, DateTime? scheduledDateTime = null)
        {
            var hearingScheduled = scheduledDateTime ?? DateTime.Today.ToUniversalTime().AddDays(1).AddMinutes(-1);
            var request = new SimpleBookNewHearingRequest(caseName, hearingScheduled);
            _request = request.Build();
            
            var endpoints = Builder<EndpointRequest>.CreateListOfSize(3)
                .All()
                .Build().ToList();

            _request.Endpoints = endpoints;
        }

        public CreateHearingRequestBuilder WithContext(TestContext context)
        {
            _context = context;
            return this;
        }

        public CreateHearingRequestBuilder WithParticipant(string caseRoleName, string hearingRoleName)
        {
            var participant = Builder<ParticipantRequest>.CreateNew()
                .With(x => x.Title = "Mrs")
                .With(x => x.FirstName = $"Automation_{Faker.Name.First()}")
                .With(x => x.LastName = $"Automation_{Faker.Name.Last()}")
                .With(x => x.ContactEmail = $"Automation_{Faker.RandomNumber.Next()}@hmcts.net")
                .With(x => x.TelephoneNumber = Faker.Phone.Number())
                .With(x => x.Username = hearingRoleName == "Judge" ? $"Automation_{Faker.RandomNumber.Next()}@hmcts.net" : String.Empty)
                .With(x => x.DisplayName = $"Automation_{Faker.Name.FullName()}")
                .With(x => x.CaseRoleName = caseRoleName)
                .With(x => x.HearingRoleName = hearingRoleName)
                .With(x => x.Representee = null)
                .With(x => x.OrganisationName = $"{Faker.Company.Name()}")
                .Build();

            _request.Participants.Add(participant);
            
            return this;
        }

        public CreateHearingRequestBuilder WithLinkedParticipants()
        {
            var individualEmail = _request.Participants.FirstOrDefault(p => p.HearingRoleName == "Litigant in person").ContactEmail;
            var interpreterEmail = _request.Participants.FirstOrDefault(p => p.HearingRoleName == "Interpreter").ContactEmail;

            _request.LinkedParticipants = new List<LinkedParticipantRequest>();
            _request.LinkedParticipants.Add(AddLinkedParticipant(individualEmail, interpreterEmail));
            _request.LinkedParticipants.Add(AddLinkedParticipant(interpreterEmail, individualEmail));
            return this;
        }

        private LinkedParticipantRequest AddLinkedParticipant(string participantEmail, string linkedParticipantEmail)
        {
            var linkeParticipantRequest = Builder<LinkedParticipantRequest>.CreateNew()
                                    .With(x => x.LinkedParticipantContactEmail = linkedParticipantEmail)
                                    .With(x => x.ParticipantContactEmail = participantEmail)
                                    .Build();

            return linkeParticipantRequest;
        }

        public BookNewHearingRequest Build()
        {
            _context.TestData.CreateHearingRequest = _request;
            return _request;
        }
    }
}
