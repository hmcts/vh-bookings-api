using Bookings.Api.Contract.Responses;
using Bookings.Domain;
using System.Collections.Generic;
using System.Linq;

namespace Bookings.API.Mappings
{
    public class SuitabilityAnswerToResponseMapper
    {
        public IList<SuitabilityAnswerResponse> MapToResponses(IList<SuitabilityAnswer> answers)
        {
            return answers != null ? answers.Select(s => Map(s)).ToList() : new List<SuitabilityAnswerResponse>();
        }

        private SuitabilityAnswerResponse Map(SuitabilityAnswer answer)
        {
            return new SuitabilityAnswerResponse
            {
                Key = answer.Key,
                Answer = answer.Data,
                ExtendedAnswer = answer.ExtendedData
            };
        }
    }
}
