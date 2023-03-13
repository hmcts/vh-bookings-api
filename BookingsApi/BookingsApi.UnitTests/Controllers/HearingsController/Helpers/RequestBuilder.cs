using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Contract.Enums;
using BookingsApi.Contract.Requests;
using FizzWare.NBuilder;

namespace BookingsApi.UnitTests.Controllers.HearingsController.Helpers
{
    public static class RequestBuilder
    {
        public static BookNewHearingRequest Build()
        {
            var participants = ParticipantsBuilder();
            var cases = CasesBuilder();
            var linkedParticipants = LinkedParticipantsBuilder(participants);

            const string createdBy = "caseAdmin@hmcts.net";

            var request = Builder<BookNewHearingRequest>.CreateNew()
                .With(x => x.CaseTypeName = "Generic")
                .With(x => x.HearingTypeName = "Automated Test")
                .With(x => x.HearingVenueName = "Birmingham Civil and Family Justice Centre")
                .With(x => x.ScheduledDateTime = DateTime.Today.ToUniversalTime().AddDays(1).AddMinutes(-1))
                .With(x => x.ScheduledDuration = 5)
                .With(x => x.Participants = participants)
                .With(x => x.LinkedParticipants = linkedParticipants)
                .With(x => x.Cases = cases)
                .With(x => x.CreatedBy = createdBy)
                .With(x => x.QuestionnaireNotRequired = false)
                .With(x => x.Endpoints = new List<EndpointRequest> { new EndpointRequest { DisplayName = "Cool endpoint 1" } })
                .With(x => x.CaseTypeServiceId = "TestServiceId")
                .With(x => x.HearingTypeCode = "AutomatedTest")
                .Build();

            return request;
        }

        private static List<LinkedParticipantRequest> LinkedParticipantsBuilder(List<ParticipantRequest> participants)
        {
            var applicants = participants.Where(x => x.CaseRoleName == "Applicant").ToList();
            var request = new LinkedParticipantRequest
            {
                ParticipantContactEmail = applicants[0].ContactEmail,
                LinkedParticipantContactEmail = applicants[1].ContactEmail,
                Type = LinkedParticipantType.Interpreter
            };

            return new List<LinkedParticipantRequest> {request};
        }

        private static List<ParticipantRequest> ParticipantsBuilder()
        {
            var participants = Builder<ParticipantRequest>.CreateListOfSize(6).All()
                .With(x => x.Title = "Mrs")
                .With(x => x.FirstName = $"Automation_testName")
                .With(x => x.LastName = $"Automation_LastName")
                .With(x => x.ContactEmail = $"Automation_{Faker.RandomNumber.Next()}@hmcts.net")
                .With(x => x.TelephoneNumber = Faker.Phone.Number())
                .With(x => x.Username = $"Automation_{Faker.RandomNumber.Next()}@hmcts.net")
                .With(x => x.DisplayName = $"Automation_{Faker.Name.FullName()}")
                .With(x => x.OrganisationName = $"{Faker.Company.Name()}")
                .Build().ToList();
            
            participants[0].CaseRoleName = "Applicant";
            participants[0].HearingRoleName = "Litigant in person";
            participants[0].Representee = null;

            participants[1].CaseRoleName = "Applicant";
            participants[1].HearingRoleName = "Representative";
            participants[1].Representee = participants[0].DisplayName;

            participants[2].CaseRoleName = "Respondent";
            participants[2].HearingRoleName = "Litigant in person";
            participants[2].Representee = null;

            participants[3].CaseRoleName = "Respondent";
            participants[3].HearingRoleName = "Representative";
            participants[3].Representee = participants[2].DisplayName;

            participants[4].CaseRoleName = "Judge";
            participants[4].HearingRoleName = "Judge";
            participants[4].Representee = null;
            
            participants[5].CaseRoleName = "Judicial Office Holder";
            participants[5].HearingRoleName = "Judicial Office Holder";
            participants[5].Representee = participants[5].DisplayName;

            return participants;
        }

        private static List<CaseRequest> CasesBuilder()
        {
            var cases = Builder<CaseRequest>.CreateListOfSize(1).Build().ToList();
            cases[0].IsLeadCase = false;
            cases[0].Name = $"Bookings Api Automated Test {Faker.RandomNumber.Next(0, 9999999)}";
            cases[0].Number = $"{Faker.RandomNumber.Next(0, 9999)}/{Faker.RandomNumber.Next(0, 9999)}";

            return cases;
        }
    }
}