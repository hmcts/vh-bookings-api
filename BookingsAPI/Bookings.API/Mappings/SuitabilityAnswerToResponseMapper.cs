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
            var responses = answers.Select(s => Map(s)).ToList();
            return responses;
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
