using Bookings.Api.Contract.Requests;
using Bookings.DAL.Commands;

namespace Bookings.API.Mappings
{
    /// <summary>
    /// This class is used to map a participant request object to the NewAddress model
    /// used by the UpdateParticipantCommand.
    /// </summary>
    public class ParticipantRequestToNewAddressMapper
    {
        public NewAddress MapRequestToNewAddress(UpdateParticipantRequest requestParticipant)
        {
            return new NewAddress
            {
                HouseNumber = requestParticipant.HouseNumber,
                Street = requestParticipant.Street,
                City = requestParticipant.City,
                County = requestParticipant.County,
                Postcode = requestParticipant.Postcode,
            };
        }
    }
}
