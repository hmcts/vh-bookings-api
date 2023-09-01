using System.Collections.Generic;
using BookingsApi.Contract.V1.Requests;

namespace BookingsApi.AcceptanceTests.Models
{
    internal static class UpdateParticipantRequest
    {
        public static Contract.V1.Requests.UpdateParticipantRequest BuildRequest()
        {
            return new Contract.V1.Requests.UpdateParticipantRequest
            {
                Title = "Mr",
                DisplayName="Update Display Name",
                TelephoneNumber="11112222333",
                OrganisationName = "OrgName",
                Representee = "Rep",
                LinkedParticipants = new List<LinkedParticipantRequest>()
            };
        }

        public static Contract.V1.Requests.UpdateParticipantRequest WithLinkedParticipants(this Contract.V1.Requests.UpdateParticipantRequest updateParticipantRequest, string participantEmail, string interpreterEmail)
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
