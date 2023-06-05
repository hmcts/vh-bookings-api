using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Contract.Requests;
using FizzWare.NBuilder;
using Testing.Common.Configuration;

namespace BookingsApi.AcceptanceTests.Models;

internal class SimpleBookNewHearingRequest
{
    private readonly BookNewHearingRequest _request;

    public SimpleBookNewHearingRequest(string caseName, DateTime scheduledDateTime, string emailDomain = "@hmcts.net")
    {
        var hearingScheduled = scheduledDateTime;
        var participants = Builder<ParticipantRequest>.CreateListOfSize(5).All()
            .With(x => x.Title = "Mrs")
            .With(x => x.TelephoneNumber = "01234567890")
            .With(x => x.OrganisationName = TestUsers.Organisation1)
            .Build().ToList();

        participants[0].CaseRoleName = "Applicant";
        participants[0].HearingRoleName = "Litigant in person";
        participants[0].Representee = null;
        participants[0].FirstName = TestUsers.ApplicantLitigant1.FirstName;
        participants[0].LastName = TestUsers.ApplicantLitigant1.LastName;
        participants[0].ContactEmail = $"{participants[0].FirstName}_{participants[0].LastName}@hmcts.net";
        participants[0].Username = $"{participants[0].FirstName}_{participants[0].LastName}@hmcts.net";
        participants[0].DisplayName = $"{participants[0].FirstName} {participants[0].LastName}";

        participants[1].CaseRoleName = "Applicant";
        participants[1].HearingRoleName = "Representative";
        participants[1].Representee = participants[0].DisplayName;
        participants[1].FirstName = TestUsers.ApplicantRepresentative1.FirstName;
        participants[1].LastName = TestUsers.ApplicantRepresentative1.LastName;
        participants[1].ContactEmail = $"{participants[1].FirstName}_{participants[1].LastName}@hmcts.net";
        participants[1].Username = $"{participants[1].FirstName}_{participants[1].LastName}@hmcts.net";
        participants[1].DisplayName = $"{participants[1].FirstName} {participants[1].LastName}";

        participants[2].CaseRoleName = "Respondent";
        participants[2].HearingRoleName = "Litigant in person";
        participants[2].Representee = null;
        participants[2].FirstName = TestUsers.RespondentLitigant1.FirstName;
        participants[2].LastName = TestUsers.RespondentLitigant1.LastName;
        participants[2].ContactEmail = $"{participants[2].FirstName}_{participants[2].LastName}@hmcts.net";
        participants[2].Username = $"{participants[2].FirstName}_{participants[2].LastName}@hmcts.net";
        participants[2].DisplayName = $"{participants[2].FirstName} {participants[2].LastName}";

        participants[3].CaseRoleName = "Respondent";
        participants[3].HearingRoleName = "Representative";
        participants[3].Representee = participants[2].DisplayName;
        participants[3].FirstName = TestUsers.RespondentRepresentative1.FirstName;
        participants[3].LastName = TestUsers.RespondentRepresentative1.LastName;
        participants[3].ContactEmail = $"{participants[3].FirstName}_{participants[3].LastName}@hmcts.net";
        participants[3].Username = $"{participants[3].FirstName}_{participants[3].LastName}@hmcts.net";
        participants[3].DisplayName = $"{participants[3].FirstName} {participants[3].LastName}";

        participants[4].CaseRoleName = "Judge";
        participants[4].HearingRoleName = "Judge";
        participants[4].Representee = null;
        participants[4].FirstName = TestUsers.Judge1.FirstName;
        participants[4].LastName = TestUsers.Judge1.LastName;
        participants[4].ContactEmail = $"{participants[4].FirstName}_{participants[4].LastName}@hmcts.net";
        participants[4].Username = $"{participants[4].FirstName}_{participants[4].LastName}@hmcts.net";
        participants[4].DisplayName = $"{participants[4].FirstName} {participants[4].LastName}";

        var cases = Builder<CaseRequest>.CreateListOfSize(1).Build().ToList();
        cases[0].IsLeadCase = false;
        cases[0].Name = $"{caseName} {Faker.RandomNumber.Next(0, 9999999)}";
        cases[0].Number = $"{Faker.RandomNumber.Next(0, 9999)}/{Faker.RandomNumber.Next(0, 9999)}";

        const string createdBy = "bookingsapi.acautomation@hmcts.net";
            
        _request = Builder<BookNewHearingRequest>.CreateNew()
            .With(x => x.CaseTypeName = "Tribunal")
            .With(x => x.HearingTypeName = "Hearing")
            .With(x => x.HearingVenueName = "Birmingham Civil and Family Justice Centre")
            .With(x => x.ScheduledDateTime = hearingScheduled)
            .With(x => x.ScheduledDuration = 5)
            .With(x => x.Participants = participants)
            .With(x => x.Cases = cases)
            .With(x => x.CreatedBy = createdBy)
            .With(x => x.QuestionnaireNotRequired = false)
            .With(x => x.AudioRecordingRequired = true)
            .With(x => x.HearingTypeCode = "AutomatedTest")
            .Build();
    }

    public BookNewHearingRequest Build()
    {
        return _request;
    }
}