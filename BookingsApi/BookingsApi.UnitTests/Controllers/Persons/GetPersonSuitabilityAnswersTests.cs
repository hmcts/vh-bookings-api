using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using BookingsApi.DAL.Queries;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Testing.Common.Assertions;

namespace BookingsApi.UnitTests.Controllers.Persons
{
    public class GetPersonSuitabilityAnswersTests : PersonsControllerTest
    {

        [Test]
        public async Task Should_return_empty_list_of_suitability_answers_if_no_hearings()
        {
            var userName = "userName@hmcts.net";
            QueryHandlerMock
             .Setup(x => x.Handle<GetHearingsByUsernameQuery, List<VideoHearing>>(It.IsAny<GetHearingsByUsernameQuery>()))
             .ReturnsAsync(new List<VideoHearing>());

            var result = await Controller.GetPersonSuitabilityAnswers(userName);

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
            QueryHandlerMock
             .Setup(x => x.Handle<GetHearingsByUsernameQuery, List<VideoHearing>>(It.IsAny<GetHearingsByUsernameQuery>()))
             .ReturnsAsync(new List<VideoHearing> { videoHearing });
            var userName = videoHearing.Participants.First(p => p is Individual).Person.Username;
            var result = await Controller.GetPersonSuitabilityAnswers(userName);

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
            QueryHandlerMock
             .Setup(x => x.Handle<GetHearingsByUsernameQuery, List<VideoHearing>>(It.IsAny<GetHearingsByUsernameQuery>()))
             .ReturnsAsync(new List<VideoHearing> { hearing });

            var result = await Controller.GetPersonSuitabilityAnswers(userName);

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

            var result = await Controller.GetPersonSuitabilityAnswers(username);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(username), $"Please provide a valid {nameof(username)}");
        }

        [Test]
        public async Task Should_return_empty_list_of_suitability_answers_if_no_matched_participant_with_username()
        {
            var hearing = TestData();
            var userName = "some@hmcts.net";
            
            QueryHandlerMock
             .Setup(x => x.Handle<GetHearingsByUsernameQuery, List<VideoHearing>>(It.IsAny<GetHearingsByUsernameQuery>()))
             .ReturnsAsync(new List<VideoHearing> { hearing });

            var result = await Controller.GetPersonSuitabilityAnswers(userName);

            result.Should().NotBeNull();

            var objectResult = result as ObjectResult;
            var data = (List<PersonSuitabilityAnswerResponse>)(objectResult.Value);
            data.Count.Should().Be(0);
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Test]
        public async Task Should_return_list_of_usernames_for_old_hearings()
        {
            var usernameList = new List<string> { "testUser1@hmcts.net", "testUser2@hmcts.net", "testUser3@hmcts.net" };
            QueryHandlerMock
                .Setup(x => x.Handle<GetPersonsByClosedHearingsQuery, List<string>>(It.IsAny<GetPersonsByClosedHearingsQuery>()))
                .ReturnsAsync(usernameList);

            var result = await Controller.GetPersonByClosedHearings();
            result.Should().NotBeNull();
            var objectResult = result as ObjectResult;
            var response = (UserWithClosedConferencesResponse)(objectResult.Value);
            response.Usernames.Count.Should().Be(3);
            QueryHandlerMock
                .Verify(x => x.Handle<GetPersonsByClosedHearingsQuery, List<string>>(It.IsAny<GetPersonsByClosedHearingsQuery>()), Times.Once);
        }
    }
}
