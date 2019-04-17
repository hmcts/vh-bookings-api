using Bookings.Api.Contract.Requests;
using Faker;
using FizzWare.NBuilder;

namespace Testing.Common.Builders.Api.Request
{
    public class UpdateParticipantRequestBuilder
    {
        private readonly UpdateParticipantRequest _updateParticipantRequest;

        public UpdateParticipantRequestBuilder()
        {
            _updateParticipantRequest = Builder<UpdateParticipantRequest>.CreateNew()
                .With(x => x.Title = Name.Prefix())
                .With(x => x.TelephoneNumber = Phone.Number())
                .With(x => x.DisplayName = Name.FullName())
                .With(x => x.HouseNumber = Address.StreetAddress())
                .With(x => x.Street = Address.StreetName())
                .With(x => x.City = Address.City())
                .With(x => x.County = Address.Country())
                .With(x => x.Postcode = Address.UkPostCode())
                .With(x=>x.OrganisationName = Company.Name())
                .With(x=>x.Representee = Name.FullName())
                .With(x=>x.SolicitorReference = Company.Name())
                .Build();
        }

        public UpdateParticipantRequest Build()
        {
            return _updateParticipantRequest;
        }
    }
}