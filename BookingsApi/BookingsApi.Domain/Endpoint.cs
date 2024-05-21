using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;

namespace BookingsApi.Domain
{
    public class Endpoint : TrackableEntity<Guid>
    {
        public string DisplayName { get; set; }
        public string Sip { get; set; }
        public string Pin { get; set; }
        public Participant DefenceAdvocate { get; set; }
        public virtual IList<EndpointParticipant> EndpointParticipants { get; }
        public Guid HearingId { get; set; }
        public virtual Hearing Hearing { get; protected set; }
        protected Endpoint(){}

        public Endpoint(string displayName, string sip, string pin)
        {
            DisplayName = displayName;
            Sip = sip;
            Pin = pin;
            EndpointParticipants = new List<EndpointParticipant>();
        }

        public void AssignDefenceAdvocate(Participant defenceAdvocate)
        {
            if(EndpointParticipants.Any(x => x.Type == LinkedParticipantType.DefenceAdvocate))
                EndpointParticipants.Remove(EndpointParticipants.First(x => x.Type == LinkedParticipantType.DefenceAdvocate));

            EndpointParticipants.Add(
                new EndpointParticipant(this, defenceAdvocate, LinkedParticipantType.DefenceAdvocate));
        }
        
        public Participant GetDefenceAdvocate()
        {
            return EndpointParticipants.FirstOrDefault(x => x.Type == LinkedParticipantType.DefenceAdvocate)?.Participant;
        }
        
        public void AssignIntermediary(Participant intermediary)
        {
            if(EndpointParticipants.Any(x => x.Type == LinkedParticipantType.Intermediary))
                EndpointParticipants.Remove(EndpointParticipants.First(x => x.Type == LinkedParticipantType.Intermediary));

            EndpointParticipants.Add(
                new EndpointParticipant(this, intermediary, LinkedParticipantType.Intermediary));
        }
        
        public Participant GetIntermediary()
        {
            return EndpointParticipants.FirstOrDefault(x => x.Type == LinkedParticipantType.Intermediary)?.Participant;
        }
        
        public void RemoveLinkedParticipant(Participant participant)
        {
            var linkedParticipant = EndpointParticipants.FirstOrDefault(x => x.ParticipantId == participant.Id);
            if(linkedParticipant != null)
                EndpointParticipants.Remove(linkedParticipant);
        }
        
        public void UpdateDisplayName(string displayName)
        {
            if (string.IsNullOrEmpty(displayName))
            {
                throw new ArgumentNullException(nameof(displayName));
            }

            DisplayName = displayName;
        }
    }
}