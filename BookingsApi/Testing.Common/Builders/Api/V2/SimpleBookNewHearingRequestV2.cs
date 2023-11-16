using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Requests.Enums;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Domain.Constants;
using FizzWare.NBuilder;
using Testing.Common.Data;

namespace Testing.Common.Builders.Api.V2;

public class SimpleBookNewHearingRequestV2
{
    private readonly BookNewHearingRequestV2 _requestV2;

    public SimpleBookNewHearingRequestV2(
        string caseName, 
        DateTime scheduledDateTime, 
        string judiciaryJudgePersonCode, 
        List<string> judiciaryPanelMemberPersonCodes = null, 
        bool useGenericParticipants = false)
    {
        var hearingScheduled = scheduledDateTime;
        var participants = Builder<ParticipantRequestV2>.CreateListOfSize(4).All()
            .With(x => x.Title = "Mrs")
            .With(x => x.TelephoneNumber = "01234567890")
            .With(x => x.OrganisationName = TestUsers.Organisation1)
            .Build().ToList();

        var participant0 = new TestUser(TestUsers.ApplicantLitigant1.FirstName, TestUsers.ApplicantLitigant1.LastName);
        participants[0].HearingRoleCode = HearingRoleCodes.Applicant;
        participants[0].Representee = null;
        participants[0].FirstName = participant0.FirstName;
        participants[0].LastName = participant0.LastName;
        participants[0].ContactEmail = participant0.ContactEmail;
        participants[0].DisplayName = participant0.DisplayName;

        var participant1 = new TestUser(TestUsers.ApplicantRepresentative1.FirstName, TestUsers.ApplicantRepresentative1.LastName);
        participants[1].HearingRoleCode = HearingRoleCodes.Representative;
        participants[1].Representee = participant0.DisplayName;
        participants[1].FirstName = participant1.FirstName;
        participants[1].LastName = participant1.LastName;
        participants[1].ContactEmail = participant1.ContactEmail;
        participants[1].DisplayName = participant1.DisplayName;

        var participant2 = new TestUser(TestUsers.RespondentLitigant1.FirstName, TestUsers.RespondentLitigant1.LastName);
        participants[2].HearingRoleCode = HearingRoleCodes.Respondent;
        participants[2].Representee = null;
        participants[2].FirstName = participant2.FirstName;
        participants[2].LastName = participant2.LastName;
        participants[2].ContactEmail = participant2.ContactEmail;
        participants[2].DisplayName = participant2.DisplayName;

        var participant3 = new TestUser(TestUsers.RespondentRepresentative1.FirstName, TestUsers.RespondentRepresentative1.LastName);
        participants[3].HearingRoleCode = HearingRoleCodes.Representative;
        participants[3].Representee = participant2.DisplayName;
        participants[3].FirstName = participant3.FirstName;
        participants[3].LastName = participant3.LastName;
        participants[3].ContactEmail = participant3.ContactEmail;
        participants[3].DisplayName = participant3.DisplayName;

        var judiciaryParticipants = new List<JudiciaryParticipantRequest>
        {
            new()
            {
                DisplayName = "Judiciary Judge",
                HearingRoleCode = JudiciaryParticipantHearingRoleCode.Judge,
                PersonalCode = judiciaryJudgePersonCode,
                OptionalContactTelephone = useGenericParticipants ? Faker.Phone.Number() : null,
                OptionalContactEmail = useGenericParticipants ? $"Automation_{Faker.RandomNumber.Next()}@hmcts.net" : null
            }
        };

        if (judiciaryPanelMemberPersonCodes != null)
        {
            var i = 1;
            
            foreach (var personCode in judiciaryPanelMemberPersonCodes)
            {
                judiciaryParticipants.Add(new JudiciaryParticipantRequest
                {
                    DisplayName = $"Judiciary Panel Member {i}",
                    HearingRoleCode = JudiciaryParticipantHearingRoleCode.PanelMember,
                    PersonalCode = personCode,
                    OptionalContactTelephone = useGenericParticipants ? Faker.Phone.Number() : null,
                    OptionalContactEmail = useGenericParticipants ? $"Automation_{Faker.RandomNumber.Next()}@hmcts.net" : null
                });

                i++;
            }
        }

        var cases = Builder<CaseRequestV2>.CreateListOfSize(1).Build().ToList();
        cases[0].IsLeadCase = false;
        cases[0].Name = $"{caseName} {Faker.RandomNumber.Next(0, 9999999)}";
        cases[0].Number = $"{Faker.RandomNumber.Next(0, 9999)}/{Faker.RandomNumber.Next(0, 9999)}";

        const string createdBy = "bookingsapi.acautomation@hmcts.net";
            
        _requestV2 = Builder<BookNewHearingRequestV2>.CreateNew()
            .With(x => x.ServiceId = "AAA6") // Civil Money Claims
            .With(x => x.HearingVenueCode = "231596") // Birmingham Civil and Family Justice Centre
            .With(x => x.ScheduledDateTime = hearingScheduled)
            .With(x => x.ScheduledDuration = 5)
            .With(x => x.Participants = participants)
            .With(x=> x.JudiciaryParticipants = judiciaryParticipants)
            .With(x => x.Cases = cases)
            .With(x => x.CreatedBy = createdBy)
            .With(x => x.AudioRecordingRequired = true)
            .Build();
    }
    
    public BookNewHearingRequestV2 Build()
    {
        return _requestV2;
    }
}