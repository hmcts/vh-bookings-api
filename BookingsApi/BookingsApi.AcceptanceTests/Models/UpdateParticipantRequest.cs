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

        public static BookingsApi.Contract.Requests.UpdateParticipantRequest WithLinkedParticipants(this BookingsApi.Contract.Requests.UpdateParticipantRequest updateParticipantRequest, string participantEmail, string interpreterEmail)
        {
            updateParticipantRequest.LinkedParticipants.Add(CreateLinkedParticipantRequest(participantEmail, interpreterEmail));
            updateParticipantRequest.LinkedParticipants.Add(CreateLinkedParticipantRequest(interpreterEmail, participantEmail));
            return updateParticipantRequest;
        }

        private static LinkedParticipantRequest CreateLinkedParticipantRequest(string participantEmail, string interpreterEmail)
        {
            return new LinkedParticipantRequest
            {
                ParticipantContactEmail = participantEmail,
                LinkedParticipantContactEmail = interpreterEmail
            };
        }
    }
}
