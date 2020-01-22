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
                HouseNumber="Update 1",
                Street="Update Street",
                City="Update City",
                County="Update County",
                Postcode="ED1 5NR",
                OrganisationName = "OrgName",
                Representee = "Rep",
                SolicitorsReference = "SolRef"
            };
        }
    }
}
