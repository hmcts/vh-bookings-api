using System;
using System.Collections.Generic;
using System.Text;

namespace Bookings.Api.Contract.Requests
{
    public interface IRepresentativeInfoRequest
    {
        /// <summary>
        ///     Participant Organisation
        /// </summary>
        string Organisation { get; set; }
        /// <summary>
        ///     Solicitor Reference
        /// </summary>
        string SolicitorReference { get; set; }

        /// <summary>
        ///     Representee
        /// </summary>
        string Representee { get; set; }        
    }
}
