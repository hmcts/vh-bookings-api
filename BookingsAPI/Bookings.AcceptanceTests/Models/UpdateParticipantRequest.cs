using System.Collections.Generic;
using Bookings.Api.Contract.Requests;

namespace Bookings.AcceptanceTests.Models
{
    internal static class UpdateParticipantRequest
    {
        public static Api.Contract.Requests.UpdateParticipantRequest BuildRequest()
        {
            return new Api.Contract.Requests.UpdateParticipantRequest
            {
                Title = "Mr",
                DisplayName="Update Display Name",
                TelephoneNumber="11112222333",
                OrganisationName = "OrgName",
                Representee = "Rep",
                LinkedParticipants = new List<LinkedParticipantRequest>()
            };
        }
    }
}
