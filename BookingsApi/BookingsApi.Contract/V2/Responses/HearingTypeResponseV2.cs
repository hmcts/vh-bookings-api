namespace BookingsApi.Contract.V2.Responses
{
    /// <summary>
    ///     A hearing type
    /// </summary>
    public class HearingTypeResponseV2
    {
        /// <summary>
        ///     The hearing type display name
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        ///     Unique identifier for the hearing type
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     The hearing type code
        /// </summary>
        public string Code { get; set; }
    }
}