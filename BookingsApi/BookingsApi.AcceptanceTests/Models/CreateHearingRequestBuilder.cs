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

        public CreateHearingRequestBuilder(string caseName)
        {
            var participants = Builder<ParticipantRequest>.CreateListOfSize(5).All()
                .With(x => x.Title = "Mrs")
                .With(x => x.TelephoneNumber = "01234567890")
                .With(x => x.OrganisationName = "Organisation1")
                .Build().ToList();

            participants[0].CaseRoleName = "Applicant";
            participants[0].HearingRoleName = "Litigant in person";
            participants[0].Representee = null;
            participants[0].FirstName = "Automation_Applicant";
            participants[0].LastName = "LitigantInPerson_1";
            participants[0].ContactEmail = $"{participants[0].FirstName}_{participants[0].LastName}@hmcts.net";
            participants[0].Username = $"{participants[0].FirstName}_{participants[0].LastName}@hearings.reform.hmcts.net";
            participants[0].DisplayName = $"{participants[0].FirstName} {participants[0].LastName}";

            participants[1].CaseRoleName = "Applicant";
            participants[1].HearingRoleName = "Representative";
            participants[1].Representee = participants[0].DisplayName;
            participants[1].FirstName = "Automation_Applicant";
            participants[1].LastName = "Representative_1";
            participants[1].ContactEmail = $"{participants[1].FirstName}_{participants[1].LastName}@hmcts.net";
            participants[1].Username = $"{participants[1].FirstName}_{participants[1].LastName}@hearings.reform.hmcts.net";
            participants[1].DisplayName = $"{participants[1].FirstName} {participants[1].LastName}";

            participants[2].CaseRoleName = "Respondent";
            participants[2].HearingRoleName = "Litigant in person";
            participants[2].Representee = null;
            participants[2].FirstName = "Automation_Respondent";
            participants[2].LastName = "LitigantInPerson_1";
            participants[2].ContactEmail = $"{participants[2].FirstName}_{participants[2].LastName}@hmcts.net";
            participants[2].Username = $"{participants[2].FirstName}_{participants[2].LastName}@hearings.reform.hmcts.net";
            participants[2].DisplayName = $"{participants[2].FirstName} {participants[2].LastName}";

            participants[3].CaseRoleName = "Respondent";
            participants[3].HearingRoleName = "Representative";
            participants[3].Representee = participants[2].DisplayName;
            participants[3].FirstName = "Automation_Respondent";
            participants[3].LastName = "Representative_1";
            participants[3].ContactEmail = $"{participants[3].FirstName}_{participants[3].LastName}@hmcts.net";
            participants[3].Username = $"{participants[3].FirstName}_{participants[3].LastName}@hearings.reform.hmcts.net";
            participants[3].DisplayName = $"{participants[3].FirstName} {participants[3].LastName}";

            participants[4].CaseRoleName = "Judge";
            participants[4].HearingRoleName = "Judge";
            participants[4].Representee = null;
            participants[4].FirstName = "Automation_Judge";
            participants[4].LastName = "Judge_1";
            participants[4].ContactEmail = $"{participants[4].FirstName}_{participants[4].LastName}@hmcts.net";
            participants[4].Username = $"{participants[4].FirstName}_{participants[4].LastName}@hearings.reform.hmcts.net";
            participants[4].DisplayName = $"{participants[4].FirstName} {participants[4].LastName}";

            var cases = Builder<CaseRequest>.CreateListOfSize(1).Build().ToList();
            cases[0].IsLeadCase = false;
            cases[0].Name = $"{caseName} {Faker.RandomNumber.Next(0, 9999999)}";
            cases[0].Number = $"{Faker.RandomNumber.Next(0, 9999)}/{Faker.RandomNumber.Next(0, 9999)}";

            var endpoints = Builder<EndpointRequest>.CreateListOfSize(3)
                                .All()
                                .Build().ToList();

            const string createdBy = "caseAdmin@hmcts.net";

            _request = Builder<BookNewHearingRequest>.CreateNew()
                .With(x => x.CaseTypeName = "Generic")
                .With(x => x.HearingTypeName = "Automated Test")
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
                .With(x => x.HearingTypeCode = "AutomatedTest")
                .Build();
        }

        public CreateHearingRequestBuilder WithContext(TestContext context)
        {
            _context = context;
            return this;
        }

        public CreateHearingRequestBuilder WithParticipant(string caseRoleName, string hearingRoleName)
        {
            var firstName = $"Automation_{caseRoleName}";
            var lastName = $"{hearingRoleName}_1";
            const string organisationName = "Organisation1";
            
            var participant = Builder<ParticipantRequest>.CreateNew()
                .With(x => x.Title = "Mrs")
                .With(x => x.FirstName = firstName)
                .With(x => x.LastName = lastName)
                .With(x => x.ContactEmail = $"{firstName}_{lastName}@hmcts.net")
                .With(x => x.TelephoneNumber = "01234567890")
                .With(x => x.Username = $"{firstName}_{lastName}@hearings.reform.hmcts.net")
                .With(x => x.DisplayName = $"{firstName}_{lastName}")
                .With(x => x.CaseRoleName = caseRoleName)
                .With(x => x.HearingRoleName = hearingRoleName)
                .With(x => x.Representee = null)
                .With(x => x.OrganisationName = organisationName)
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
