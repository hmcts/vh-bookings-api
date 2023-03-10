using System.Linq;
using BookingsApi.Contract.Requests;
using FizzWare.NBuilder;

namespace BookingsApi.AcceptanceTests.Models
{
    internal static class AddParticipantRequest
    {
        public const string ParticipantRequestFirstName = "Automation_AddedParticipant";
        public static AddParticipantsToHearingRequest BuildRequest()
        {
            var participants = Builder<ParticipantRequest>.CreateListOfSize(1).All()
                .With(x => x.ContactEmail = $"Automation_{Faker.RandomNumber.Next()}@hmcts.net")
                .With(x => x.Username = $"Automation_{Faker.RandomNumber.Next()}@hmcts.net")
                .With(x => x.TelephoneNumber = "01234567890")
                .Build().ToList();
            participants[0].CaseRoleName = "Applicant";
            participants[0].HearingRoleName = "Litigant in person";
            participants[0].FirstName = ParticipantRequestFirstName;
            var request = new AddParticipantsToHearingRequest{Participants = participants};
            return request;
        }
    }
}
