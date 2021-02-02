using System.Collections.Generic;

namespace BookingsApi.Contract.Responses
{
    /// <summary>
    ///     A case type and the hearing types possible
    /// </summary>
    public class CaseTypeResponse
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
        ///     The hearing types
        /// </summary>
        public List<HearingTypeResponse> HearingTypes { get; set; }
    }
}