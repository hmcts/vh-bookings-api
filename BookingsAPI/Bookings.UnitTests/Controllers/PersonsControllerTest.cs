using Bookings.Api.Contract.Responses;
using Bookings.API.Controllers;
using Bookings.DAL.Queries;
using Bookings.DAL.Queries.Core;
using Bookings.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Testing.Common.Builders.Domain;

namespace Bookings.UnitTests.Controllers
{
    public class PersonsControllerTest
    {
        private PersonsController _controller;
        private Mock<IQueryHandler> _queryHandlerMock;

        [SetUp]
        public void Setup()
        {
            _queryHandlerMock = new Mock<IQueryHandler>();
            _controller = new PersonsController(_queryHandlerMock.Object);
        }

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
            var userName = "some@becker.ca";
            _queryHandlerMock
             .Setup(x => x.Handle<GetHearingsByUsernameQuery, List<VideoHearing>>(It.IsAny<GetHearingsByUsernameQuery>()))
             .ReturnsAsync(new List<VideoHearing> { TestData(false) });

            var result = await _controller.GetPersonSuitabilityAnswers(userName);

            result.Should().NotBeNull();

            var objectResult = result as ObjectResult;
            var data = (List<PersonSuitabilityAnswerResponse>)(objectResult.Value);
            data.Count.Should().Be(0);
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

        private VideoHearing TestData(bool addSuitability = true)
        {
            var builder = new VideoHearingBuilder();
            var hearing = builder.Build();
            if(addSuitability)
            {
                hearing.Participants[0].SuitabilityAnswers.Add(new SuitabilityAnswer("AboutYou", "Yes", ""));
                hearing.Participants[0].SuitabilityAnswers[0].UpdatedDate = DateTime.Now.AddDays(-2);
            }
            return hearing;
        }
    }
}
