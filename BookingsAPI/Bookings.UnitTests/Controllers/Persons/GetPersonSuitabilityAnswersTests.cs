using Bookings.Api.Contract.Responses;
using Bookings.DAL.Queries;
using Bookings.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Testing.Common.Assertions;
using System.Linq;
using Bookings.Domain.Participants;

namespace Bookings.UnitTests.Controllers
{
    public class GetPersonSuitabilityAnswersTests : PersonsControllerTest
    {

        [Test]
        public async Task Should_return_empty_list_of_suitability_answers_if_no_hearings()
        {
            var userName = "userName@hearings.reform.net";
            _queryHandlerMock
             .Setup(x => x.Handle<GetHearingsByUsernameQuery, List<VideoHearing>>(It.IsAny<GetHearingsByUsernameQuery>()))
             .ReturnsAsync(new List<VideoHearing>());

            var result = await _controller.GetPersonSuitabilityAnswers(userName);

            result.Should().NotBeNull();
            var objectResult = result as ObjectResult;
            var data = (List<PersonSuitabilityAnswerResponse>)(objectResult.Value);
            data.Count.Should().Be(0);
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Test]
        public async Task Should_return_empty_list_of_suitability_answers_if_not_found_username()
        {
            var videoHearing = TestData(false);
            _queryHandlerMock
             .Setup(x => x.Handle<GetHearingsByUsernameQuery, List<VideoHearing>>(It.IsAny<GetHearingsByUsernameQuery>()))
             .ReturnsAsync(new List<VideoHearing> { videoHearing });
            var userName = videoHearing.Participants.First(p => p is Individual).Person.Username;
            var result = await _controller.GetPersonSuitabilityAnswers(userName);

            result.Should().NotBeNull();

            var objectResult = result as ObjectResult;
            var data = (List<PersonSuitabilityAnswerResponse>)(objectResult.Value);
            data.Count.Should().BeGreaterThan(0);
            data[0].Answers.Count.Should().Be(0);
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Test]
        public async Task Should_return_list_of_suitability_answers()
        {
            var hearing = TestData();
            var userName = hearing.Participants[0].Person.Username;
            _queryHandlerMock
             .Setup(x => x.Handle<GetHearingsByUsernameQuery, List<VideoHearing>>(It.IsAny<GetHearingsByUsernameQuery>()))
             .ReturnsAsync(new List<VideoHearing> { hearing });

            var result = await _controller.GetPersonSuitabilityAnswers(userName);

            result.Should().NotBeNull();

            var objectResult = result as ObjectResult;
            var data = (List<PersonSuitabilityAnswerResponse>)(objectResult.Value);
            data.Count.Should().Be(1);
            data[0].Answers.Count.Should().Be(1);
            data[0].Answers[0].Key.Should().Be("AboutYou");
            data[0].UpdatedAt.Date.Should().Be(DateTime.Now.AddDays(-2).Date);
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Test]
        public async Task Should_return_badrequest_for_invalid_username()
        {
            var username = string.Empty;

            var result = await _controller.GetPersonSuitabilityAnswers(username);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(username), $"Please provide a valid {nameof(username)}");
        }

        
    }
}
