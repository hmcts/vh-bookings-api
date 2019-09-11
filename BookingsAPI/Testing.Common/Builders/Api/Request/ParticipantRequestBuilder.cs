using Bookings.Api.Contract.Requests;
using Faker;
using FizzWare.NBuilder;

namespace Testing.Common.Builders.Api.Request
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
                .With(x => x.FirstName = $"Automation_{Name.First()}")
                .With(x => x.LastName = $"Automation_{Name.Last()}")
                .With(x => x.Username = $"Automation_{Internet.Email()}")
                .With(x => x.ContactEmail = $"Automation_{Internet.Email()}")
                .With(x => x.TelephoneNumber = Phone.Number())
                .Build();
        }

        public ParticipantRequestBuilder WithSolicitorDetails(string solicitorsReference, string representee)
        {
            _participantRequest.SolicitorsReference = solicitorsReference;
            _participantRequest.Representee = representee;
            return this;
        }

        public ParticipantRequest Build()
        {
            return _participantRequest;
        }
    }
}