using Bookings.Api.Contract.Requests;
using Bookings.DAL.Commands;

namespace Bookings.API.Mappings
{
    /// <summary>
    /// This class is used to map a participant request object to the RepresentativeInfo model
    /// used by the UpdateParticipantCommand.
    /// </summary>
    public static class UpdateParticipantRequestToNewRepresentativeMapper
    {
        public static RepresentativeInformation MapRequestToNewRepresentativeInfo(UpdateParticipantRequest requestParticipant)
        {
            return new RepresentativeInformation
            {
                Representee = requestParticipant.Representee
            };
        }
    }
}