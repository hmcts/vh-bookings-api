using BookingsApi.Contract.V2.Enums;

namespace BookingsApi.Contract.V2.Requests
{
    public class LinkedParticipantRequestV2
    {
        /// <summary>
        /// The contact email for the participant to create a link for
        /// </summary>
        public string ParticipantContactEmail { get; set; }
        
        /// <summary>
        /// The contact email for the participant to create a link with
        /// </summary>
        public string LinkedParticipantContactEmail { get; set; }
        
        /// <summary>
        /// The type of link to create
        /// </summary>
        public LinkedParticipantTypeV2 Type { get; set; }
    }
}