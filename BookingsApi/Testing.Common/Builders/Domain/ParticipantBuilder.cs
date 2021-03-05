using BookingsApi.Domain.Participants;
using BookingsApi.Domain;
using BookingsApi.Domain.RefData;
using System.Collections.Generic;

namespace Testing.Common.Builders.Domain
{
    public class ParticipantBuilder
    {
        private readonly Participant _individualParticipant1;
        private readonly Participant _individualParticipant2;
        private readonly Participant _representativeParticipant;

        private readonly List<Participant> _participants = new List<Participant>();


        public ParticipantBuilder()
        {
            var applicantCaseRole = new CaseRole(1, "Applicant");
            var respondentCaseRole = new CaseRole(2, "Respondent");
            var applicantLipHearingRole = new HearingRole(1, "Litigant in person");
            applicantLipHearingRole.UserRole = new UserRole(5, "Individual");
            var respondentRepresentativeHearingRole = new HearingRole(5, "Representative");
            respondentRepresentativeHearingRole.UserRole = new UserRole(6, "Representative");
            var respondentLipHearingRole = new HearingRole(4, "Litigant in person");
            respondentLipHearingRole.UserRole = new UserRole(5, "Individual");
            var person1 = new PersonBuilder(true).Build();
            var person2 = new PersonBuilder(true).Build();
            var person3 = new PersonBuilder(true).Build();

            _individualParticipant1 = new Individual(person1, applicantLipHearingRole, applicantCaseRole);
            _individualParticipant1.HearingRole = applicantLipHearingRole;
            _individualParticipant1.Questionnaire = new Questionnaire
            {
                Participant = _individualParticipant1,
                ParticipantId = _individualParticipant1.Id
            };
            _individualParticipant1.Questionnaire.AddSuitabilityAnswers(ListOfSuitabilityAnswers());

            _individualParticipant2 = new Individual(person2, respondentLipHearingRole, respondentCaseRole);
            _individualParticipant2.HearingRole = respondentLipHearingRole;
            _individualParticipant2.Questionnaire = new Questionnaire
            {
                Participant = _individualParticipant2,
                ParticipantId = _individualParticipant2.Id
            };
            _individualParticipant2.Questionnaire.AddSuitabilityAnswers(ListOfSuitabilityAnswers());

            _representativeParticipant = new Representative(person3, respondentRepresentativeHearingRole, respondentCaseRole);
            _representativeParticipant.HearingRole = respondentRepresentativeHearingRole;
            _representativeParticipant.Questionnaire = new Questionnaire
            {
                Participant = _representativeParticipant,
                ParticipantId = _representativeParticipant.Id
            };
            _representativeParticipant.Questionnaire.AddSuitabilityAnswers(ListOfSuitabilityAnswers());

            _participants.Add(_individualParticipant1);
            _participants.Add(_individualParticipant2);
            _participants.Add(_representativeParticipant);
        }

        private List<SuitabilityAnswer> ListOfSuitabilityAnswers()
        {
            var answer1 = new SuitabilityAnswer("ABOUT_YOU", "No", "");
            var answer2 = new SuitabilityAnswer("ABOUT_YOUR_COMPUTER", "No", "Note");
            var answer3 = new SuitabilityAnswer("CONSENT", "YES", "");

            return new List<SuitabilityAnswer> { answer1, answer2, answer3 };
        }

        public Participant IndividualParticipantApplicant => _individualParticipant1;
        public Participant IndividualParticipantRespondent => _individualParticipant2;
        public Participant RepresentativeParticipantRespondent => _representativeParticipant;
        public List<Participant> Build() => _participants;
    }
}