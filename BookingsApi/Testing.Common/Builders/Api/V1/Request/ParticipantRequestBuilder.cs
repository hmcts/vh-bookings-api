using BookingsApi.Contract.V1.Requests;
using Faker;
using FizzWare.NBuilder;

namespace Testing.Common.Builders.Api.V1.Request
{
    public class ParticipantRequestBuilder
    {
        private readonly ParticipantRequest _participantRequest;

        public ParticipantRequestBuilder(string caseRole, string hearingRole)
        {
            _participantRequest = Builder<ParticipantRequest>.CreateNew()
                .With(x => x.CaseRoleName = caseRole)
                .With(x => x.HearingRoleName = hearingRole)
                .With(x => x.Title = Name.Prefix())
                .With(x => x.FirstName = "Automation_FirstName")
                .With(x => x.LastName = "Automation_LastName")
                .With(x => x.Username = $"Automation_{RandomNumber.Next()}@hmcts.net")
                .With(x => x.ContactEmail = $"Automation_{RandomNumber.Next()}@hmcts.net")
                .With(x => x.TelephoneNumber = "01234567890")
                .Build();
        }

        public ParticipantRequestBuilder WithRepresentativeDetails(string representee)
        {
            _participantRequest.Representee = representee;
            return this;
        }

        public ParticipantRequest Build()
        {
            return _participantRequest;
        }
    }
}