using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Contract.Requests;
using FizzWare.NBuilder;

namespace BookingsApi.AcceptanceTests.Models;

internal class SimpleBookNewHearingRequest
{
    private readonly BookNewHearingRequest _request;

    public SimpleBookNewHearingRequest(string caseName, DateTime scheduledDateTime)
    {
        var hearingScheduled = scheduledDateTime;
        var participants = Builder<ParticipantRequest>.CreateListOfSize(5).All()
            .With(x => x.Title = "Mrs")
            .With(x => x.FirstName = $"Automation_{Faker.Name.First()}")
            .With(x => x.LastName = $"Automation_{Faker.Name.Last()}")
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

        var cases = Builder<CaseRequest>.CreateListOfSize(1).Build().ToList();
        cases[0].IsLeadCase = false;
        cases[0].Name = $"{caseName} {Faker.RandomNumber.Next(0, 9999999)}";
        cases[0].Number = $"{Faker.RandomNumber.Next(0, 9999)}/{Faker.RandomNumber.Next(0, 9999)}";

        const string createdBy = "bookingsapi.acautomation@hmcts.net";
            
        _request = Builder<BookNewHearingRequest>.CreateNew()
            .With(x => x.CaseTypeName = "Generic")
            .With(x => x.HearingTypeName = "Automated Test")
            .With(x => x.HearingVenueName = "Birmingham Civil and Family Justice Centre")
            .With(x => x.ScheduledDateTime = hearingScheduled)
            .With(x => x.ScheduledDuration = 5)
            .With(x => x.Participants = participants)
            .With(x => x.Cases = cases)
            .With(x => x.CreatedBy = createdBy)
            .With(x => x.QuestionnaireNotRequired = false)
            .With(x => x.AudioRecordingRequired = true)
            .With(x => x.Endpoints = new List<EndpointRequest> {new EndpointRequest{ DisplayName = "New Endpoint"}})
            .With(x => x.HearingTypeCode = "AutomatedTest")
            .Build();
    }

    public BookNewHearingRequest Build()
    {
        return _request;
    }
}