using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;

namespace BookingsApi.AcceptanceTests.Models
{
    internal static class UpdateSuitabilityAnswersRequest
    {
        public static List<BookingsApi.Contract.V1.Requests.SuitabilityAnswersRequest> BuildRequest()
        {
            return Builder<BookingsApi.Contract.V1.Requests.SuitabilityAnswersRequest>
                    .CreateListOfSize(2).All()
                    .With(x => x.Key = Faker.RandomNumber.Next().ToString())
                    .With(x => x.Answer = Faker.Lorem.Sentence(10))
                    .With(x => x.ExtendedAnswer = Faker.Lorem.Sentence(30))
                    .Build()
                    .ToList();
        }
    }
}