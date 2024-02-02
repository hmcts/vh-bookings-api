using System;
using System.Collections.Generic;
using BookingsApi.Contract.V2.Enums;

namespace BookingsApi.Contract.V2.Requests
{
    public class UpdateMultiDayHearingRequestV2
    {
        public List<HearingRequestV2> Hearings { get; set; } = new();
    }

    public class HearingRequestV2
    {
        public Guid HearingId { get; set; }
        public UpdateHearingParticipantsRequestV2 Participants { get; set; }
        public UpdateHearingEndpointsRequestV2 Endpoints { get; set; }
        public UpdateJudiciaryParticipantsRequestV2 JudiciaryParticipants { get; set; }
    }

    public class UpdateHearingEndpointsRequestV2
    {
        /// <summary>
        ///     List of new endpoints
        /// </summary>
        public List<EndpointRequestV2> NewEndpoints { get; set; } = new();

        /// <summary>
        ///     List of existing endpoints
        /// </summary>
        public List<EndpointRequestV2> ExistingEndpoints { get; set; } = new();

        /// <summary>
        ///     List of removed endpoint Ids
        /// </summary>
        public List<Guid> RemovedEndpointIds { get; set; } = new();
    }

    public class UpdateJudiciaryParticipantsRequestV2
    {
        /// <summary>
        ///     List of new judiciary participants
        /// </summary>
        public List<JudiciaryParticipantRequestV2> NewJudiciaryParticipants { get; set; } = new();

        /// <summary>
        ///     List of existing judiciary participants
        /// </summary>
        public List<JudiciaryParticipantRequestV2> ExistingJudiciaryParticipants { get; set; } = new();

        /// <summary>
        ///     List of removed judiciary participant personal codes
        /// </summary>
        public List<string> RemovedJudiciaryParticipantPersonalCodes { get; set; } = new();
    }
    
    public class JudiciaryParticipantRequestV2
    {
        /// <summary>
        /// The participant's judicial personal code
        /// </summary>
        public string PersonalCode { get; set; }
        
        /// <summary>
        /// The participant's display name
        /// </summary>
        public string DisplayName { get; set; }
        
        /// <summary>
        /// The participant's hearing role code
        /// </summary>
        public JudiciaryParticipantHearingRoleCodeV2 HearingRoleCode { get; set; }
        
        /// <summary>
        /// Optional Contact Email
        /// </summary>
        public string ContactEmail { get; set; }
        
        /// <summary>
        /// Optional Contact Telephone
        /// </summary>
        public string ContactTelephone { get; set; }
    }
}
