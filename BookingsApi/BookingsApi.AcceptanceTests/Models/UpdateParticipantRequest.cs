using System.Collections.Generic;
using BookingsApi.Contract.Requests;

namespace BookingsApi.AcceptanceTests.Models
{
    internal static class UpdateParticipantRequest
    {
        public static BookingsApi.Contract.Requests.UpdateParticipantRequest BuildRequest()
        {
            return new BookingsApi.Contract.Requests.UpdateParticipantRequest
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
