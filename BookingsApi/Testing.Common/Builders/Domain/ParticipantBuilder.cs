using System;
using BookingsApi.Domain.Participants;
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


        public ParticipantBuilder(bool treatPersonAsNew = true)
        {
            var applicantLipHearingRole = new HearingRole(1, "Litigant in person")
            {
                UserRole = new UserRole(5, "Individual")
            };
            var respondentRepresentativeHearingRole = new HearingRole(5, "Representative")
            {
                UserRole = new UserRole(6, "Representative")
            };
            var respondentLipHearingRole = new HearingRole(4, "Litigant in person")
            {
                UserRole = new UserRole(5, "Individual")
            };
            var person1 = new PersonBuilder(true, treatPersonAsNew:treatPersonAsNew).Build();
            var person2 = new PersonBuilder(true, treatPersonAsNew:treatPersonAsNew).Build();
            var person3 = new PersonBuilder(true, treatPersonAsNew:treatPersonAsNew).Build();

            _individualParticipant1 = new Individual(Guid.NewGuid().ToString(), person1, applicantLipHearingRole, "Individual1");
            _individualParticipant1.HearingRole = applicantLipHearingRole;
            
            _individualParticipant2 = new Individual(Guid.NewGuid().ToString(), person2, respondentLipHearingRole, "Individual2");
            _individualParticipant2.HearingRole = respondentLipHearingRole;
            
            _representativeParticipant = new Representative(Guid.NewGuid().ToString(), person3, respondentRepresentativeHearingRole, "Representative1", "Representee1");
            _representativeParticipant.HearingRole = respondentRepresentativeHearingRole;
            
            _participants.Add(_individualParticipant1);
            _participants.Add(_individualParticipant2);
            _participants.Add(_representativeParticipant);
        }

        public Participant IndividualParticipantApplicant => _individualParticipant1;
        public Participant IndividualParticipantRespondent => _individualParticipant2;
        public Participant RepresentativeParticipantRespondent => _representativeParticipant;
        public List<Participant> Build() => _participants;
    }
}