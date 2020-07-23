using Bookings.Domain.Participants;
using Bookings.Domain;
using Bookings.Domain.RefData;
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
            var claimantCaseRole = new CaseRole(1, "Claimant");
            var defendantCaseRole = new CaseRole(2, "Defendant");
            var claimantLipHearingRole = new HearingRole(1, "Claimant LIP");
            claimantLipHearingRole.UserRole = new UserRole(5, "Individual");
            var defendantRepresentativeHearingRole = new HearingRole(5, "Representative");
            defendantRepresentativeHearingRole.UserRole = new UserRole(6, "Representative");
            var defendantLipHearingRole = new HearingRole(4, "Defendant LIP");
            defendantLipHearingRole.UserRole = new UserRole(5, "Individual");
            var person1 = new PersonBuilder(true).Build();
            var person2 = new PersonBuilder(true).Build();
            var person3 = new PersonBuilder(true).Build();

            _individualParticipant1 = new Individual(person1, claimantLipHearingRole, claimantCaseRole);
            _individualParticipant1.HearingRole = claimantLipHearingRole;
            _individualParticipant1.Questionnaire = new Questionnaire
            {
                Participant = _individualParticipant1,
                ParticipantId = _individualParticipant1.Id
            };
            _individualParticipant1.Questionnaire.AddSuitabilityAnswers(ListOfSuitabilityAnswers());

            _individualParticipant2 = new Individual(person2, defendantLipHearingRole, defendantCaseRole);
            _individualParticipant2.HearingRole = defendantLipHearingRole;
            _individualParticipant2.Questionnaire = new Questionnaire
            {
                Participant = _individualParticipant2,
                ParticipantId = _individualParticipant2.Id
            };
            _individualParticipant2.Questionnaire.AddSuitabilityAnswers(ListOfSuitabilityAnswers());

            _representativeParticipant = new Representative(person3, defendantRepresentativeHearingRole, defendantCaseRole);
            _representativeParticipant.HearingRole = defendantRepresentativeHearingRole;
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

        public Participant IndividualPrticipantClaimant => _individualParticipant1;
        public Participant IndividualPrticipantDefendant => _individualParticipant2;
        public Participant RepresentativeParticipantDefendant => _representativeParticipant;
        public List<Participant> Build() => _participants;
    }
}