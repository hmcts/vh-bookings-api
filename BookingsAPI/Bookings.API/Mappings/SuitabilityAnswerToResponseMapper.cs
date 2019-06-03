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
            return answers != null ? answers.Select(Map).ToList() : new List<SuitabilityAnswerResponse>();
        }

        private static SuitabilityAnswerResponse Map(SuitabilityAnswer answer)
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
