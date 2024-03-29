using System;
using System.Linq;
using BookingsApi.Contract.V1.Requests;
using FizzWare.NBuilder;
using Testing.Common.Data;

namespace Testing.Common.Builders.Api.V1.Request;

public class SimpleBookNewHearingRequest
{
    private readonly BookNewHearingRequest _request;

    public SimpleBookNewHearingRequest(string caseName, DateTime scheduledDateTime)
    {
        var hearingScheduled = scheduledDateTime;
        var participants = Builder<ParticipantRequest>.CreateListOfSize(7).All()
            .With(x => x.Title = "Mrs")
            .With(x => x.TelephoneNumber = "01234567890")
            .With(x => x.OrganisationName = TestUsers.Organisation1)
            .Build().ToList();

        var participant0 = new TestUser(TestUsers.ApplicantLitigant1.FirstName, TestUsers.ApplicantLitigant1.LastName);
        participants[0].CaseRoleName = "Applicant";
        participants[0].HearingRoleName = "Litigant in person";
        participants[0].Representee = null;
        participants[0].FirstName = participant0.FirstName;
        participants[0].LastName = participant0.LastName;
        participants[0].ContactEmail = participant0.ContactEmail;
        participants[0].Username = participant0.Username;
        participants[0].DisplayName = participant0.DisplayName;

        var participant1 = new TestUser(TestUsers.ApplicantRepresentative1.FirstName, TestUsers.ApplicantRepresentative1.LastName);
        participants[1].CaseRoleName = "Applicant";
        participants[1].HearingRoleName = "Representative";
        participants[1].Representee = participant0.DisplayName;
        participants[1].FirstName = participant1.FirstName;
        participants[1].LastName = participant1.LastName;
        participants[1].ContactEmail = participant1.ContactEmail;
        participants[1].Username = participant1.Username;
        participants[1].DisplayName = participant1.DisplayName;

        var participant2 = new TestUser(TestUsers.RespondentLitigant1.FirstName, TestUsers.RespondentLitigant1.LastName);
        participants[2].CaseRoleName = "Respondent";
        participants[2].HearingRoleName = "Litigant in person";
        participants[2].Representee = null;
        participants[2].FirstName = participant2.FirstName;
        participants[2].LastName = participant2.LastName;
        participants[2].ContactEmail = participant2.ContactEmail;
        participants[2].Username = participant2.Username;
        participants[2].DisplayName = participant2.DisplayName;

        var participant3 = new TestUser(TestUsers.RespondentRepresentative1.FirstName, TestUsers.RespondentRepresentative1.LastName);
        participants[3].CaseRoleName = "Respondent";
        participants[3].HearingRoleName = "Representative";
        participants[3].Representee = participant2.DisplayName;
        participants[3].FirstName = participant3.FirstName;
        participants[3].LastName = participant3.LastName;
        participants[3].ContactEmail = participant3.ContactEmail;
        participants[3].Username = participant3.Username;
        participants[3].DisplayName = participant3.DisplayName;

        var participant4 = new TestUser(TestUsers.Judge1.FirstName, TestUsers.Judge1.LastName);
        participants[4].CaseRoleName = "Judge";
        participants[4].HearingRoleName = "Judge";
        participants[4].Representee = null;
        participants[4].FirstName = participant4.FirstName;
        participants[4].LastName = participant4.LastName;
        participants[4].ContactEmail = participant4.ContactEmail;
        participants[4].Username = participant4.Username;
        participants[4].DisplayName = participant4.DisplayName;

        var participant5 = new TestUser(TestUsers.PanelMember1.FirstName, TestUsers.PanelMember1.LastName);
        participants[5].CaseRoleName = "Panel Member";
        participants[5].HearingRoleName = "Panel Member";
        participants[5].Representee = null;
        participants[5].TelephoneNumber = null;
        participants[5].FirstName = participant5.FirstName;
        participants[5].LastName = participant5.LastName;
        participants[5].ContactEmail = participant5.ContactEmail;
        participants[5].Username = participant5.Username;
        participants[5].DisplayName = participant5.DisplayName;

        var participant6 = new TestUser("Panel_2", "Member_2");
        participants[6].CaseRoleName = "Panel member";
        participants[6].HearingRoleName = "Panel member";
        participants[6].Representee = null;
        participants[6].TelephoneNumber = null;
        participants[6].FirstName = participant6.FirstName;
        participants[6].LastName = participant6.LastName;
        participants[6].ContactEmail = participant6.ContactEmail;
        participants[6].Username = participant6.Username;
        participants[6].DisplayName = participant6.DisplayName;

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
            .With(x => x.AudioRecordingRequired = true)
            .Build();
    }

    public BookNewHearingRequest Build()
    {
        return _request;
    }
}