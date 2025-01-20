using System.Collections.Generic;

namespace BookingsApi.Contract.V2.Responses
{
    /// <summary>
    ///     A case type and the hearing types possible
    /// </summary>
    public class CaseTypeResponseV2
    {
        /// <summary>
        ///     The case type display name
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        ///     Unique identifier
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// The service identifier
        /// </summary>
        public string ServiceId { get; set; }

        /// <summary>
        /// Whether audio recording is allowed for the case type
        /// </summary>
        public bool IsAudioRecordingAllowed { get; set; }
    }
}