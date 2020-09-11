using System;
using System.Collections.Generic;
using System.Linq;
using Bookings.Api.Contract.Requests;
using Bookings.Domain.Participants;

namespace Bookings.API.Helpers
{
    public static class DefenceAdvocateHelper
    {
        public static Participant CheckAndReturnDefenceAdvocate(EndpointRequest endpointRequest,
            IEnumerable<Participant> participants)
        {
            Participant defenceAdvocate = null;

            if (endpointRequest.DefenceAdvocateId != null && endpointRequest.DefenceAdvocateId != Guid.Empty)
            {
                defenceAdvocate = participants.First(x => x.Id == endpointRequest.DefenceAdvocateId);
            }

            return defenceAdvocate;
        }
    }
}