using System;
using System.Linq;
using Bookings.Domain.Participants;
    using Bookings.Domain;
using Bookings.Domain.RefData;
using FizzWare.NBuilder;
using System.Collections.Generic;

namespace Testing.Common.Builders.Domain
{
    public class ParticipantBuilder
    {
        private readonly Participant _individualParticipant1;
        private readonly Participant _individualParticipant2;
        private readonly Participant _representativeParticipant;

        private readonly List<Participant> participants= new List<Participant>();
       

        public ParticipantBuilder()
        {
            var claimantCaseRole = new CaseRole(1, "Claimant");
            var defendantCaseRole = new CaseRole(2, "Defendant");
            var claimantLipHearingRole = new HearingRole(1, "Claimant LIP");
            claimantLipHearingRole.UserRole = new UserRole(5, "Individual");
            var defendantSolicitorHearingRole =  new HearingRole(5, "Solicitor");
            defendantSolicitorHearingRole.UserRole = new UserRole(6, "Representative");
            var defendantLipHearingRole =  new HearingRole(4, "Defendant LIP");
            defendantLipHearingRole.UserRole = new UserRole(5, "Individual");
            var person1 = new PersonBuilder(true).WithAddress().Build();
            var person2 = new PersonBuilder(true).WithAddress().Build();
            var person3 = new PersonBuilder(true).Build();

            _individualParticipant1 = new Individual(person1, claimantLipHearingRole, claimantCaseRole);
            _individualParticipant1.HearingRole = claimantLipHearingRole;
            _individualParticipant2 = new Individual(person2, defendantLipHearingRole, defendantCaseRole);
            _individualParticipant2.HearingRole = defendantLipHearingRole;
            _representativeParticipant = new Representative(person3, defendantSolicitorHearingRole, defendantCaseRole);
            _representativeParticipant.HearingRole = defendantSolicitorHearingRole;

            participants.Add(_individualParticipant1);
            participants.Add(_individualParticipant2);
            participants.Add(_representativeParticipant);


        }

        public Participant IndividualPrticipantClaimant => _individualParticipant1;
        public Participant IndividualPrticipantDefendant => _individualParticipant2;
        public Participant RepresentativePrticipantDefendant => _representativeParticipant;
        public List<Participant> Build() => participants;
    }
}