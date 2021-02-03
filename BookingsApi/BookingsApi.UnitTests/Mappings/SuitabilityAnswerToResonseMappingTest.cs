using BookingsApi.Mappings;
using BookingsApi.Domain;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using BookingsApi.UnitTests.Utilities;

namespace BookingsApi.UnitTests.Mappings
{
    public class SuitabilityAnswerToResonseMappingTest : TestBase
    {
        private IList<SuitabilityAnswer> suitabilityAnswers;
        private SuitabilityAnswerToResponseMapper mapper;

        [SetUp]
        public void SetUp()
        {
            mapper = new SuitabilityAnswerToResponseMapper();

            var answer1 = new SuitabilityAnswer("AboutYou", "Yes",null);
            var answer2 = new SuitabilityAnswer("AboutClient", "No", "note");
            var answer3 = new SuitabilityAnswer("AboutCpmputer", "Yes", "");

            suitabilityAnswers = new List<SuitabilityAnswer> { answer1, answer2, answer3 };
        }

        [Test]
        public void Should_map_suitability_answears_items_to_response_list()
        {
            var result = mapper.MapToResponses(suitabilityAnswers);
            result.Should().NotBeNull();
            result.Count.Should().Equals(3);
        }

        [Test]
        public void Should_map_suitability_answear_to_response_model()
        {
            var result = mapper.MapToResponses(suitabilityAnswers);
            result[0].Key.Should().Equals("AboutYou");
            result[0].Answer.Should().Equals("Yes");
            result[0].ExtendedAnswer.Should().BeNull();

            result[1].Key.Should().Equals("AboutClient");
            result[1].Answer.Should().Equals("No");
            result[1].ExtendedAnswer.Should().Equals("note");
        }

        [Test]
        public void Should_returns_empty_list_if_suitability_answears_is_empty_list()
        {
            suitabilityAnswers = new List<SuitabilityAnswer>();
            var result = mapper.MapToResponses(suitabilityAnswers);
            result.Should().NotBeNull();
            result.Count.Should().Equals(0);
        }

        [Test]
        public void Should_returns_empty_list_if_suitability_answears_is_null()
        {
            suitabilityAnswers = null;
            var result = mapper.MapToResponses(suitabilityAnswers);
            result.Should().NotBeNull();
            result.Count.Should().Equals(0);
        }
    }
}
