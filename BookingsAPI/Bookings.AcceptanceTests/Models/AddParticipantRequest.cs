using System.Linq;
using Bookings.Api.Contract.Requests;
using FizzWare.NBuilder;

namespace Bookings.AcceptanceTests.Models
{
    internal static class AddParticipantRequest
    {
        public static AddParticipantsToHearingRequest BuildRequest()
        {
            var participants = Builder<ParticipantRequest>.CreateListOfSize(1).All()
                .With(x => x.ContactEmail = $"Automation_{Faker.Internet.Email()}")
                .With(x => x.Username = $"Automation_{Faker.Internet.Email()}")
                .Build().ToList();
            participants[0].CaseRoleName = "Claimant";
            participants[0].HearingRoleName = "Litigant in person";
            participants[0].FirstName = "Automation_Added Participant";
            var request = new AddParticipantsToHearingRequest{Participants = participants};
            return request;
        }
    }
}
