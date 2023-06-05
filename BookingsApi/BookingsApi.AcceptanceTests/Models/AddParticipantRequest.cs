using System.Linq;
using BookingsApi.Contract.Requests;
using FizzWare.NBuilder;
using Testing.Common.Configuration;

namespace BookingsApi.AcceptanceTests.Models
{
    internal static class AddParticipantRequest
    {
        public const string ParticipantRequestFirstName = TestUsers.ApplicantLitigant2.FirstName;
        private const string ParticipantRequestLastName = TestUsers.ApplicantLitigant2.LastName;
        public static AddParticipantsToHearingRequest BuildRequest()
        {
            var participants = Builder<ParticipantRequest>.CreateListOfSize(1).All()
                .With(x => x.ContactEmail = $"{ParticipantRequestFirstName}_{ParticipantRequestLastName}@hmcts.net")
                .With(x => x.Username = $"{ParticipantRequestFirstName}_{ParticipantRequestLastName}@hearings.reform.hmcts.net")
                .With(x => x.TelephoneNumber = "01234567890")
                .Build().ToList();
            participants[0].CaseRoleName = "Applicant";
            participants[0].HearingRoleName = "Litigant in person";
            participants[0].FirstName = ParticipantRequestFirstName;
            participants[0].LastName = ParticipantRequestLastName;
            var request = new AddParticipantsToHearingRequest{Participants = participants};
            return request;
        }
    }
}
