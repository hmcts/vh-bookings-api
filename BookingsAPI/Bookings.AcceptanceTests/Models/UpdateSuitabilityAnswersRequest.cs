using System;
using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;

namespace Bookings.AcceptanceTests.Models
{
    internal static class UpdateSuitabilityAnswersRequest
    {
        public static List<Api.Contract.Requests.SuitabilityAnswersRequest> BuildRequest()
        {
            return Builder<Api.Contract.Requests.SuitabilityAnswersRequest>
                    .CreateListOfSize(2).All()
                    .With(x => x.Key = Faker.Lorem.GetFirstWord())
                    .With(x => x.Answer = Faker.Lorem.Sentence(10))
                    .With(x => x.ExtendedAnswer = Faker.Lorem.Sentence(30))
                    .Build()
                    .ToList();
        }
    }
}
