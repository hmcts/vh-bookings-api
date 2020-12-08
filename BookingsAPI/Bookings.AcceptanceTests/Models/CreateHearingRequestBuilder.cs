using System;
using System.Collections.Generic;
using System.Linq;
using Bookings.AcceptanceTests.Contexts;
using Bookings.Api.Contract.Requests;
using FizzWare.NBuilder;

namespace Bookings.AcceptanceTests.Models
{
    internal class CreateHearingRequestBuilder
    {
        private TestContext _context;
        private readonly BookNewHearingRequest _request;

        public CreateHearingRequestBuilder(string caseName)
        {
            var participants = Builder<ParticipantRequest>.CreateListOfSize(5).All()
                .With(x => x.Title = "Mrs")
                .With(x => x.FirstName = $"Automation_{Faker.Name.First()}")
                .With(x => x.LastName = $"Automation_{Faker.Name.Last()}")
                .With(x => x.ContactEmail = $"Automation_{Faker.Internet.Email()}")
                .With(x => x.TelephoneNumber = Faker.Phone.Number())
                .With(x => x.Username = $"Automation_{Faker.Internet.Email()}")
                .With(x => x.DisplayName = $"Automation_{Faker.Name.FullName()}")
                .With(x => x.OrganisationName = $"{Faker.Company.Name()}")
                .Build().ToList();

            participants[0].CaseRoleName = "Claimant";
            participants[0].HearingRoleName = "Litigant in person";
            participants[0].Representee = null;

            participants[1].CaseRoleName = "Claimant";
            participants[1].HearingRoleName = "Representative";
            participants[1].Representee = participants[0].DisplayName;

            participants[2].CaseRoleName = "Defendant";
            participants[2].HearingRoleName = "Litigant in person";
            participants[2].Representee = null;

            participants[3].CaseRoleName = "Defendant";
            participants[3].HearingRoleName = "Representative";
            participants[3].Representee = participants[2].DisplayName;

            participants[4].CaseRoleName = "Judge";
            participants[4].HearingRoleName = "Judge";
            participants[4].Representee = null;

            var cases = Builder<CaseRequest>.CreateListOfSize(1).Build().ToList();
            cases[0].IsLeadCase = false;
            cases[0].Name = $"{caseName} {Faker.RandomNumber.Next(0, 9999999)}";
            cases[0].Number = $"{Faker.RandomNumber.Next(0, 9999)}/{Faker.RandomNumber.Next(0, 9999)}";

            var endpoints = Builder<EndpointRequest>.CreateListOfSize(3)
                                .All()
                                .Build().ToList();

            const string createdBy = "caseAdmin@emailaddress.com";

            _request = Builder<BookNewHearingRequest>.CreateNew()
                .With(x => x.CaseTypeName = "Civil Money Claims")
                .With(x => x.HearingTypeName = "Application to Set Judgment Aside")
                .With(x => x.HearingVenueName = "Birmingham Civil and Family Justice Centre")
                .With(x => x.ScheduledDateTime = DateTime.Today.ToUniversalTime().AddDays(1).AddMinutes(-1))
                .With(x => x.ScheduledDuration = 5)
                .With(x => x.Participants = participants)
                .With(x => x.Endpoints = endpoints)
                .With(x => x.Cases = cases)
                .With(x => x.CreatedBy = createdBy)
                .With(x => x.QuestionnaireNotRequired = false)
                .With(x => x.AudioRecordingRequired = true)
                .With(x => x.Endpoints = new List<EndpointRequest> {new EndpointRequest{ DisplayName = "New Endpoint"}})
                .Build();
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
                .With(x => x.ContactEmail = $"Automation_{Faker.Internet.Email()}")
                .With(x => x.TelephoneNumber = Faker.Phone.Number())
                .With(x => x.Username = $"Automation_{Faker.Internet.Email()}")
                .With(x => x.DisplayName = $"Automation_{Faker.Name.FullName()}")
                .With(x => x.OrganisationName = $"{Faker.Company.Name()}")
                .Build();

            participant.CaseRoleName = "Claimant";
            participant.HearingRoleName = "Litigant in person";
            participant.Representee = null;

            _request.Participants.Add(participant);
            
            return this;
        }

        public BookNewHearingRequest Build()
        {
            _context.TestData.CreateHearingRequest = _request;
            return _request;
        }
    }
}
