using System.Linq;
using Bookings.Api.Contract.Requests;
using FizzWare.NBuilder;

namespace Bookings.AcceptanceTests.Models
{
    internal static class CreateHearingRequest
    {
        public static BookNewHearingRequest BuildRequest()
        {
            var participants = Builder<ParticipantRequest>.CreateListOfSize(5).All()
                .With(x => x.ContactEmail = Faker.Internet.Email())
                .With(x => x.Username = Faker.Internet.Email())
                .Build().ToList();
            participants[0].CaseRoleName = "Claimant";
            participants[0].HearingRoleName = "Claimant LIP";

            participants[1].CaseRoleName = "Claimant";
            participants[1].HearingRoleName = "Solicitor";

            participants[2].CaseRoleName = "Defendant";
            participants[2].HearingRoleName = "Defendant LIP";

            participants[3].CaseRoleName = "Defendant";
            participants[3].HearingRoleName = "Solicitor";

            participants[4].CaseRoleName = "Judge";
            participants[4].HearingRoleName = "Judge";           

            var cases = Builder<CaseRequest>.CreateListOfSize(2).Build().ToList();

            return Builder<BookNewHearingRequest>.CreateNew()
                .With(x => x.CaseTypeName = "Civil Money Claims")
                .With(x => x.HearingTypeName = "Application to Set Judgment Aside")
                .With(x => x.HearingVenueName = "Birmingham Civil and Family Justice Centre")
                .With(x => x.Participants = participants)
                .With(x => x.Cases = cases)
                .With(x => x.OtherInformation = "OtherInformation 01")
                .With(x => x.HearingRoomName = "Room 01")
                .Build();
        }
    }
}
