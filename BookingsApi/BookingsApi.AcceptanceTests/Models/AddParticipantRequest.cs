using System.Linq;
using BookingsApi.Contract.V1.Requests;
using FizzWare.NBuilder;
using Testing.Common.Configuration;
using Testing.Common.Data;

namespace BookingsApi.AcceptanceTests.Models
{
    internal static class AddParticipantRequest
    {
        public static readonly ParticipantRequest User = new TestUser(ParticipantRequestFirstName, ParticipantRequestLastName);
        public const string ParticipantRequestFirstName = TestUsers.ApplicantLitigant2.FirstName;
        private const string ParticipantRequestLastName = TestUsers.ApplicantLitigant2.LastName;
        public static AddParticipantsToHearingRequest BuildRequest()
        {
            var participants = Builder<ParticipantRequest>.CreateListOfSize(1).All()
                .With(x => x.ContactEmail = User.ContactEmail)
                .With(x => x.Username = User.Username)
                .With(x => x.TelephoneNumber = "01234567890")
                .Build().ToList();
            participants[0].CaseRoleName = "Applicant";
            participants[0].HearingRoleName = "Litigant in person";
            participants[0].FirstName = User.FirstName;
            participants[0].LastName = User.LastName;
            var request = new AddParticipantsToHearingRequest{Participants = participants};
            return request;
        }
    }
}
