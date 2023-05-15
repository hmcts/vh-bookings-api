using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Contract.Requests;
using FizzWare.NBuilder;

namespace BookingsApi.AcceptanceTests.Models;

internal class SimpleBookNewHearingRequest
{
    private readonly BookNewHearingRequest _request;

    public SimpleBookNewHearingRequest(string caseName, DateTime scheduledDateTime, string emailDomain = "@hmcts.net")
    {
        var hearingScheduled = scheduledDateTime;
        var username1 = $"auto_vw.individual_60{emailDomain}";
        var username2 = $"auto_vw.representative_13{emailDomain}";
        var username3 = $"auto_vw.individual_134{emailDomain}";
        var username4 = $"auto_vw.representative_157{emailDomain}";
        var username5 = $"auto_aw.judge_02{emailDomain}";
        var participants = Builder<ParticipantRequest>.CreateListOfSize(5).All()
            .With(x => x.Title = "Mrs")
            .With(x => x.FirstName = $"Automation_{Faker.Name.First()}")
            .With(x => x.LastName = $"Automation_{Faker.Name.Last()}")
            .With(x => x.TelephoneNumber = Faker.Phone.Number())
            .With(x => x.DisplayName = $"Automation_{Faker.Name.FullName()}")
            .With(x => x.OrganisationName = $"{Faker.Company.Name()}")
            .Build().ToList();

        participants[0].CaseRoleName = "Applicant";
        participants[0].HearingRoleName = "Litigant in person";
        participants[0].Representee = null;
        participants[0].Username = username1;
        participants[0].ContactEmail = username1;

        participants[1].CaseRoleName = "Applicant";
        participants[1].HearingRoleName = "Representative";
        participants[1].Representee = participants[0].DisplayName;
        participants[1].Username = username2;
        participants[1].ContactEmail = username2;

        participants[2].CaseRoleName = "Respondent";
        participants[2].HearingRoleName = "Litigant in person";
        participants[2].Representee = null;
        participants[2].Username = username3;
        participants[2].ContactEmail = username3;

        participants[3].CaseRoleName = "Respondent";
        participants[3].HearingRoleName = "Representative";
        participants[3].Representee = participants[2].DisplayName;
        participants[3].Username = username4;
        participants[3].ContactEmail = username4;

        participants[4].CaseRoleName = "Judge";
        participants[4].HearingRoleName = "Judge";
        participants[4].Representee = null;
        participants[4].Username = username5;
        participants[4].ContactEmail = username5;

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