using System;

namespace Bookings.Api.Contract.Responses
{
    /// <summary>
    /// Short summary about a hearing
    /// </summary>
    public class HearingSummaryResponse
    {
        /// <summary>
        ///     Hearing Id
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        ///     The date and time for a hearing
        /// </summary>
        public DateTime ScheduledDateTime { get; set; }
        
        /// <summary>
        ///     The duration of a hearing (number of minutes)
        /// </summary>
        public int ScheduledDuration { get; set; }
        
        /// <summary>
        /// The leading case number
        /// </summary>
        public string CaseNumber { get; set; }
        
        /// <summary>
        /// The leading case name
        /// </summary>
        public string CaseName { get; set; }
    }
}