using Bookings.Api.Contract.Requests;
using Faker;
using FizzWare.NBuilder;

namespace Testing.Common.Builders.Api.Request
{
    public class UpdateParticipantRequestBuilder
    {
        private readonly UpdateParticipantRequest _updateParticipantRequest;

        public UpdateParticipantRequestBuilder(string hearingRole)
        {
            _updateParticipantRequest = Builder<UpdateParticipantRequest>.CreateNew()
                .With(x => x.HearingRoleName = hearingRole)
                .With(x => x.Title = Name.Prefix())
                .With(x => x.TelephoneNumber = Phone.Number())
                .With(x => x.DisplayName = Name.FullName())
                .With(x => x.HouseNumber = Address.StreetAddress())
                .With(x => x.Street = Address.StreetName())
                .With(x => x.City = Address.City())
                .With(x => x.County = Address.Country())
                .With(x => x.Postcode = Address.UkPostCode())
                .Build();
        }

        public UpdateParticipantRequest Build()
        {
            return _updateParticipantRequest;
        }
    }
}