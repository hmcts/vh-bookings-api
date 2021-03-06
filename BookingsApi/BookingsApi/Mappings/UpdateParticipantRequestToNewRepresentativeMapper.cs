﻿using BookingsApi.Contract.Requests;
using BookingsApi.DAL.Commands;

namespace BookingsApi.Mappings
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