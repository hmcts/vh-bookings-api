namespace BookingsApi.Contract.V2.Responses
{
    /// <summary>
    /// Case Information
    /// </summary>
    public class CaseResponseV2
    {
        /// <summary>
        ///     The case number
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        ///     The case name
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        ///     Is lead case for hearing
        /// </summary>
        public bool IsLeadCase { get; set; }
    }
}