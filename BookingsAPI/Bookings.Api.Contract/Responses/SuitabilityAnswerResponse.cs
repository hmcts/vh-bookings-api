﻿namespace Bookings.Api.Contract.Responses
{
    /// <summary>
    /// Suitability Answer Response Object
    /// </summary>
    public class SuitabilityAnswerResponse
    {
        /// <summary>
        /// Key used to identify the question
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Answer the question
        /// </summary>
        /// 
        public string Answer { get; set; }
        /// <summary>
        /// Extended answer to the question
        /// </summary>
        public string ExtendedAnswer { get; set; }
    }
}
