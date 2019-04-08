using System;
using System.Collections.Generic;
using System.Text;

namespace Bookings.Api.Contract.Requests
{
    public interface IAddressRequest
    {

        /// <summary>
        /// House number of an Individual
        /// </summary>
        string HouseNumber { get; set; }

        /// <summary>
        /// Stree number of an Individual
        /// </summary>
        string Street { get; set; }

        /// <summary>
        /// Postcode of an Individual
        /// </summary>
        string Postcode { get; set; }

        /// <summary>
        /// City/Town of an Individual
        /// </summary>
        string City { get; set; }

        /// <summary>
        /// County of an Individual
        /// </summary>
        string County { get; set; }
    }
}
