using BookingsApi.Controllers;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using BookingsApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.V1.Configuration;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Controllers.V1;

namespace BookingsApi.UnitTests.Controllers
{
    public class JudiciaryPersonControllerTests
    {
        private JudiciaryPersonController _controller;
        private Mock<IQueryHandler> _queryHandlerMock;
        private Mock<ICommandHandler> _commandHandlerMock;
        private Mock<ILogger<JudiciaryPersonController>> _loggerMock;
        private Mock<IFeatureFlagService> _featureFlagsService;

        [SetUp]
        public void Setup()
        {
            _queryHandlerMock = new Mock<IQueryHandler>();
            _commandHandlerMock = new Mock<ICommandHandler>();
            _loggerMock = new Mock<ILogger<JudiciaryPersonController>>();
            _featureFlagsService = new Mock<IFeatureFlagService>();
   
            _controller = new JudiciaryPersonController(_queryHandlerMock.Object, _commandHandlerMock.Object, _loggerMock.Object, _featureFlagsService.Object);
        }

        [Test]
        public async Task Should_return_ok_result_with_empty_response_for_zero_bulk_inserts()
        {
            var request = new List<JudiciaryPersonRequest>();

            var result = await _controller.BulkJudiciaryPersonsAsync(request);

            result.Should().NotBeNull();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var data = objectResult.Value as BulkJudiciaryPersonResponse;
            data.Should().NotBeNull();
            data.ErroredRequests.Count.Should().Be(0);
        }

        [Test]
        public async Task Should_return_ok_result_adding_item()
        {
            var item1 = new JudiciaryPersonRequest { Id = Guid.NewGuid().ToString(), Email = "some@email.com", Fullname = "a", Surname = "b", Title = "c", KnownAs = "d", PersonalCode = "123", PostNominals = "nom1" };
            var request = new List<JudiciaryPersonRequest> { item1 };

            _queryHandlerMock
                .Setup(x => x.Handle<GetJudiciaryPersonByPersonalCodeQuery, JudiciaryPerson>(It.IsAny<GetJudiciaryPersonByPersonalCodeQuery>()))
                .ReturnsAsync((JudiciaryPerson)null);

            var result = await _controller.BulkJudiciaryPersonsAsync(request);

            result.Should().NotBeNull();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var data = objectResult.Value as BulkJudiciaryPersonResponse;
            data.Should().NotBeNull();
            data.ErroredRequests.Count.Should().Be(0);

            _commandHandlerMock.Verify(c => c.Handle(It.Is<AddJudiciaryPersonByPersonalCodeCommand>
            (
                c => c.Email == item1.Email && c.Fullname == item1.Fullname && c.Surname == item1.Surname &&
                     c.Title == item1.Title && c.KnownAs == item1.KnownAs && c.PersonalCode == item1.PersonalCode &&
                     c.PostNominals == item1.PostNominals && c.ExternalRefId == item1.Id
            )));
        }
        
        [Test]
        public async Task Should_return_ok_result_adding_leaver_person()
        {
            var item1 = new JudiciaryPersonRequest { Id = Guid.NewGuid().ToString(), Leaver = true, LeftOn = "2022-06-08", PersonalCode = "PersonalCode" };
            var request = new List<JudiciaryPersonRequest> { item1 };

            _queryHandlerMock
                .Setup(x => x.Handle<GetJudiciaryPersonByPersonalCodeQuery, JudiciaryPerson>(It.IsAny<GetJudiciaryPersonByPersonalCodeQuery>()))
                .ReturnsAsync((JudiciaryPerson)null);

            var result = await _controller.BulkJudiciaryPersonsAsync(request);

            result.Should().NotBeNull();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var data = objectResult.Value as BulkJudiciaryPersonResponse;
            data.Should().NotBeNull();
            data.ErroredRequests.Count.Should().Be(0);

            _commandHandlerMock.Verify(c => c.Handle(It.Is<AddJudiciaryPersonByPersonalCodeCommand>
            (
                c => c.Leaver == item1.Leaver && c.LeftOn == item1.LeftOn && c.ExternalRefId == item1.Id && c.PersonalCode == item1.PersonalCode
            )));
        }

        [Test]
        public async Task Should_return_ok_result_updating_leaver_item()
        {
            var id = Guid.NewGuid().ToString();
            var judiciaryPerson = new JudiciaryPerson(id, "some@email.com", "a", "b", "c", "d", "123", "nom1", false, false, string.Empty);
            var item1 = new JudiciaryLeaverRequest { Id = id.ToString(), Leaver = true, LeftOn = DateTime.Now.AddDays(-100).ToLongDateString(), PersonalCode = "some@email.com" };

            var request = new List<JudiciaryLeaverRequest> { item1 };

            _queryHandlerMock
                .Setup(x => x.Handle<GetJudiciaryPersonByPersonalCodeQuery, JudiciaryPerson>(It.IsAny<GetJudiciaryPersonByPersonalCodeQuery>()))
                .ReturnsAsync(judiciaryPerson);

            var result = await _controller.BulkJudiciaryLeaversAsync(request);

            result.Should().NotBeNull();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var data = objectResult.Value as BulkJudiciaryLeaverResponse;
            data.Should().NotBeNull();
            data.ErroredRequests.Count.Should().Be(0);

            _commandHandlerMock.Verify(c => c.Handle(It.Is<UpdateJudiciaryLeaverByPersonalCodeCommand>
            (
                c => c.PersonalCode.ToString() == item1.PersonalCode && c.HasLeft == item1.Leaver
            )), Times.Once);
        }

        [Test]
        public async Task Should_return_ok_result_updating_leaver_item_which_does_not_exist()
        {
            var item1 = new JudiciaryLeaverRequest { Id = Guid.NewGuid().ToString(), PersonalCode = "PersonalCode", Leaver = true, LeftOn = DateTime.Now.AddDays(-100).ToLongDateString() };

            var request = new List<JudiciaryLeaverRequest> { item1 };

            _queryHandlerMock
                .Setup(x => x.Handle<GetJudiciaryPersonByPersonalCodeQuery, JudiciaryPerson>(It.IsAny<GetJudiciaryPersonByPersonalCodeQuery>()))
                .ReturnsAsync((JudiciaryPerson)null);

            var result = await _controller.BulkJudiciaryLeaversAsync(request);

            result.Should().NotBeNull();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var data = objectResult.Value as BulkJudiciaryLeaverResponse;
            data.Should().NotBeNull();
            data.ErroredRequests.Count.Should().Be(1);
            _commandHandlerMock.Verify(c => c.Handle(It.Is<UpdateJudiciaryLeaverByPersonalCodeCommand>
            (
                c => c.PersonalCode.ToString() == item1.Id && c.HasLeft == item1.Leaver
            )), Times.Never());
        }

        [Test]
        public async Task Should_return_ok_result_with_empty_response_for_zero_bulk_leaver_update()
        {
            var request = new List<JudiciaryLeaverRequest>();

            var result = await _controller.BulkJudiciaryLeaversAsync(request);

            result.Should().NotBeNull();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var data = objectResult.Value as BulkJudiciaryLeaverResponse;
            data.Should().NotBeNull();
            data.ErroredRequests.Count.Should().Be(0);
        }

        [Test]
        public async Task Should_return_an_errored_RequestCount_One()
        {
            var requests = new List<JudiciaryLeaverRequest>
            {
                 new JudiciaryLeaverRequest
                 {
                     Leaver = true
                 }
            };
            var result = await _controller.BulkJudiciaryLeaversAsync(requests);

            result.Should().NotBeNull();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var data = objectResult.Value as BulkJudiciaryLeaverResponse;
            data.Should().NotBeNull();
            data.ErroredRequests.Count.Should().Be(1);
        }

        [Test]
        public async Task Should_return_an_errored_RequestCount_One_on_Exception()
        {
            var id = Guid.NewGuid();
            var item1 = new JudiciaryLeaverRequest { Id = id.ToString(), Leaver = true, LeftOn = DateTime.Now.AddDays(-100).ToLongDateString() };
            var request = new List<JudiciaryLeaverRequest> { item1 };

            _queryHandlerMock
                .Setup(x => x.Handle<GetJudiciaryPersonByPersonalCodeQuery, JudiciaryPerson>(It.IsAny<GetJudiciaryPersonByPersonalCodeQuery>()))
                .ThrowsAsync(new Exception("Unhandled exception"));

            var result = await _controller.BulkJudiciaryLeaversAsync(request);

            result.Should().NotBeNull();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var data = objectResult.Value as BulkJudiciaryLeaverResponse;
            data.Should().NotBeNull();
            data.ErroredRequests.Count.Should().Be(1);
        }

        [Test]
        public async Task Should_return_an_errored_RequestCount_One_on_When__record_not_in_DB() {
            var id = Guid.NewGuid();
            var item1 = new JudiciaryLeaverRequest { Id = id.ToString(), Leaver = true, LeftOn = DateTime.Now.AddDays(-100).ToLongDateString() };
            var request = new List<JudiciaryLeaverRequest> { item1 };

            _queryHandlerMock
               .Setup(x => x.Handle<GetJudiciaryPersonByPersonalCodeQuery, JudiciaryPerson>(It.IsAny<GetJudiciaryPersonByPersonalCodeQuery>()))
               .ReturnsAsync(null as JudiciaryPerson);

            var result = await _controller.BulkJudiciaryLeaversAsync(request);

            result.Should().NotBeNull();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var data = objectResult.Value as BulkJudiciaryLeaverResponse;
            data.Should().NotBeNull();
            data.ErroredRequests.Count.Should().Be(1);
        }

        [Test]
        public async Task Should_return_ok_result_updating_item()
        {
            var item1 = new JudiciaryPersonRequest { Id = Guid.NewGuid().ToString(), Email = "some@email.com", Fullname = "a", Surname = "b", Title = "c", KnownAs = "d", PersonalCode = "123", PostNominals = "nom1", HasLeft = true };
            var retrievedPerson1 = new JudiciaryPerson(item1.Id, item1.PersonalCode, item1.Title, item1.KnownAs, item1.Surname, item1.Fullname, item1.PostNominals, item1.Email, false, false, string.Empty);
            var request = new List<JudiciaryPersonRequest> { item1 };

            _queryHandlerMock
                .Setup(x => x.Handle<GetJudiciaryPersonByPersonalCodeQuery, JudiciaryPerson>(It.IsAny<GetJudiciaryPersonByPersonalCodeQuery>()))
                .ReturnsAsync(retrievedPerson1);

            var result = await _controller.BulkJudiciaryPersonsAsync(request);

            result.Should().NotBeNull();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var data = objectResult.Value as BulkJudiciaryPersonResponse;
            data.Should().NotBeNull();
            data.ErroredRequests.Count.Should().Be(0);

            _commandHandlerMock.Verify(c => c.Handle(It.Is<UpdateJudiciaryPersonByPersonalCodeCommand>
            (
                c => c.Email == item1.Email && c.Fullname == item1.Fullname && c.Surname == item1.Surname &&
                     c.Title == item1.Title && c.KnownAs == item1.KnownAs && c.PersonalCode == item1.PersonalCode &&
                     c.PostNominals == item1.PostNominals && c.ExternalRefId == item1.Id
            )));
        }

        [Test]
        public async Task Should_return_ok_result_adding_and_updating_item()
        {
            var item1 = new JudiciaryPersonRequest { Id = Guid.NewGuid().ToString(), Email = "some@email.com", Fullname = "a", Surname = "b", Title = "c", KnownAs = "d", PersonalCode = "123", PostNominals = "nom1", HasLeft = false };
            var item2 = new JudiciaryPersonRequest { Id = Guid.NewGuid().ToString(), Email = "some2@email.com", Fullname = "a2", Surname = "b2", Title = "c2", KnownAs = "d2", PersonalCode = "456", PostNominals = "nom2", HasLeft = true };
            var retrievedPerson1 = new JudiciaryPerson(item2.Id, item2.PersonalCode, item2.Title, item2.KnownAs, item2.Surname, item2.Fullname, item2.PostNominals, item2.Email, false, false, string.Empty);
            var request = new List<JudiciaryPersonRequest> { item1, item2 };

            _queryHandlerMock
                .Setup(x => x.Handle<GetJudiciaryPersonByPersonalCodeQuery, JudiciaryPerson>(It.IsAny<GetJudiciaryPersonByPersonalCodeQuery>()))
                .ReturnsAsync(retrievedPerson1);

            _queryHandlerMock
                .Setup(x => x.Handle<GetJudiciaryPersonByPersonalCodeQuery, JudiciaryPerson>(It.Is<GetJudiciaryPersonByPersonalCodeQuery>(x => x.PersonalCode == item1.PersonalCode)))
                .ReturnsAsync((JudiciaryPerson)null);

            var result = await _controller.BulkJudiciaryPersonsAsync(request);

            result.Should().NotBeNull();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var data = objectResult.Value as BulkJudiciaryPersonResponse;
            data.Should().NotBeNull();
            data.ErroredRequests.Count.Should().Be(0);

            _commandHandlerMock.Verify(c => c.Handle(It.Is<AddJudiciaryPersonByPersonalCodeCommand>
            (
                c => c.Email == item1.Email && c.Fullname == item1.Fullname && c.Surname == item1.Surname &&
                     c.Title == item1.Title && c.KnownAs == item1.KnownAs && c.PersonalCode == item1.PersonalCode &&
                     c.PostNominals == item1.PostNominals && c.ExternalRefId == item1.Id
            )));

            _commandHandlerMock.Verify(c => c.Handle(It.Is<UpdateJudiciaryPersonByPersonalCodeCommand>
            (
                c => c.Email == item2.Email && c.Fullname == item2.Fullname && c.Surname == item2.Surname &&
                     c.Title == item2.Title && c.KnownAs == item2.KnownAs && c.PersonalCode == item2.PersonalCode &&
                     c.PostNominals == item2.PostNominals && c.ExternalRefId == item2.Id
            )));
        }

        [Test]
        public async Task Should_return_error_items_in_request_for_bad_request_no_personal_code()
        {
            var requestNoPersonalCode = new JudiciaryPersonRequest
            {
                Fullname = "a",
                Surname = "b",
                Title = "c",
                KnownAs = "d",
                PostNominals = "nom1",
                Email = "some@email.com"
            };

            var result = await _controller.BulkJudiciaryPersonsAsync(new List<JudiciaryPersonRequest> { requestNoPersonalCode });

            result.Should().NotBeNull();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var data = objectResult.Value as BulkJudiciaryPersonResponse;
            data.Should().NotBeNull();
            data.ErroredRequests.Count.Should().Be(1);
            data.ErroredRequests[0].JudiciaryPersonRequest.Should().BeEquivalentTo(requestNoPersonalCode);
            AssertErrorMessageContainsIdentifier(data.ErroredRequests[0].Message, requestNoPersonalCode.PersonalCode);
        }

        [Test]
        public async Task Should_return_error_items_in_request_for_bad_request_no_knownas()
        {
            var requestNoKnownAs = new JudiciaryPersonRequest
            {
                Id = Guid.NewGuid().ToString(),
                Fullname = "a",
                Surname = "b",
                Title = "c",
                PersonalCode = "123",
                PostNominals = "nom1",
                Email = "some@email.com"
            };

            var result = await _controller.BulkJudiciaryPersonsAsync(new List<JudiciaryPersonRequest> { requestNoKnownAs });

            result.Should().NotBeNull();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var data = objectResult.Value as BulkJudiciaryPersonResponse;
            data.Should().NotBeNull();
            data.ErroredRequests.Count.Should().Be(1);
            AssertErrorMessageContainsIdentifier(data.ErroredRequests[0].Message, requestNoKnownAs.PersonalCode);
        }

        [Test]
        public async Task Should_return_error_items_in_request_for_bad_request_no_surname()
        {
            var requestNoSurname = new JudiciaryPersonRequest
            {
                Id = Guid.NewGuid().ToString(),
                Fullname = "a",
                Title = "c",
                KnownAs = "d",
                PersonalCode = "123",
                PostNominals = "nom1",
                Email = "some@email.com"
            };

            var result = await _controller.BulkJudiciaryPersonsAsync(new List<JudiciaryPersonRequest> { requestNoSurname });

            result.Should().NotBeNull();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var data = objectResult.Value as BulkJudiciaryPersonResponse;
            data.Should().NotBeNull();
            data.ErroredRequests.Count.Should().Be(1);
            data.ErroredRequests[0].JudiciaryPersonRequest.Should().BeEquivalentTo(requestNoSurname);
            AssertErrorMessageContainsIdentifier(data.ErroredRequests[0].Message, requestNoSurname.PersonalCode);
        }

        [Test]
        public async Task Should_return_error_items_in_request_for_bad_request_no_email()
        {
            var requestNoEmail = new JudiciaryPersonRequest
            {
                Id = Guid.NewGuid().ToString(),
                Fullname = "a",
                Surname = "b",
                Title = "c",
                KnownAs = "d",
                PersonalCode = "123",
                PostNominals = "nom1"
            };

            var result = await _controller.BulkJudiciaryPersonsAsync(new List<JudiciaryPersonRequest> { requestNoEmail });

            result.Should().NotBeNull();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var data = objectResult.Value as BulkJudiciaryPersonResponse;
            data.Should().NotBeNull();
            data.ErroredRequests.Count.Should().Be(1);
            data.ErroredRequests[0].JudiciaryPersonRequest.Should().BeEquivalentTo(requestNoEmail);
            AssertErrorMessageContainsIdentifier(data.ErroredRequests[0].Message, requestNoEmail.PersonalCode);
        }

        [Test]
        public async Task Should_return_error_items_in_request_exception()
        {
            var item1 = new JudiciaryPersonRequest { Id = Guid.NewGuid().ToString(), Email = "some@email.com", Fullname = "a", Surname = "b", Title = "c", KnownAs = "d", PersonalCode = "123", PostNominals = "nom1" };
            var request = new List<JudiciaryPersonRequest> { item1 };

            _queryHandlerMock
                .Setup(x => x.Handle<GetJudiciaryPersonByPersonalCodeQuery, JudiciaryPerson>(It.IsAny<GetJudiciaryPersonByPersonalCodeQuery>()))
                .ThrowsAsync(new Exception("Error"));

            var result = await _controller.BulkJudiciaryPersonsAsync(request);

            result.Should().NotBeNull();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var data = objectResult.Value as BulkJudiciaryPersonResponse;
            data.Should().NotBeNull();
            data.ErroredRequests.Count.Should().Be(1);
            data.ErroredRequests[0].JudiciaryPersonRequest.Should().BeEquivalentTo(item1);

            _commandHandlerMock.Verify(c => c.Handle(It.IsAny<AddJudiciaryPersonCommand>()), Times.Never);
        }
        
        [Test]
        public async Task Should_return_error_items_in_request_exception_for_leavers()
        {
            var item1 = new JudiciaryLeaverRequest { Id = Guid.NewGuid().ToString(), PersonalCode = "PersonalCode", Leaver = true, LeftOn = DateTime.Now.AddDays(-100).ToLongDateString() };
            var request = new List<JudiciaryLeaverRequest> { item1 };

            _queryHandlerMock
                .Setup(x => x.Handle<GetJudiciaryPersonByPersonalCodeQuery, JudiciaryPerson>(It.IsAny<GetJudiciaryPersonByPersonalCodeQuery>()))
                .ThrowsAsync(new Exception("Error"));

            var result = await _controller.BulkJudiciaryLeaversAsync(request);

            result.Should().NotBeNull();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var data = objectResult.Value as BulkJudiciaryLeaverResponse;
            data.Should().NotBeNull();
            data.ErroredRequests.Count.Should().Be(1);
            data.ErroredRequests[0].JudiciaryLeaverRequest.Should().BeEquivalentTo(item1);

            _commandHandlerMock.Verify(c => c.Handle(It.IsAny<AddJudiciaryPersonCommand>()), Times.Never);
        }

        [Test]
        public async Task Should_return_list_of_PersonResponse_matching_serch_term_successfully()
        {
            var searchTermRequest = new SearchTermRequest("test");
            var persons = new List<JudiciaryPerson> {
                                new JudiciaryPerson(Guid.NewGuid().ToString(),"CODE1","Mr", "Test", "Tester", "T Tester", "N", "test@hmcts.net", false, false, string.Empty),
                                new JudiciaryPerson(Guid.NewGuid().ToString(), "CODE", "Mr", "Tester", "Test", "T Test", "n1", "atest@hmcts.net", false, false, string.Empty)
            };
            _queryHandlerMock
           .Setup(x => x.Handle<GetJudiciaryPersonBySearchTermQuery, List<JudiciaryPerson>>(It.IsAny<GetJudiciaryPersonBySearchTermQuery>()))
           .ReturnsAsync(persons);

            _featureFlagsService.Setup(p => p.GetFeatureFlag(It.Is<string>(p => p == nameof(FeatureFlags.EJudFeature)))).Returns(true);
            var result = await _controller.PostJudiciaryPersonBySearchTerm(searchTermRequest);

            result.Should().NotBeNull();
            var objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var personResponses = (List<PersonResponse>)objectResult.Value;
            personResponses.Count.Should().Be(2);
        }

        [Test]
        public void PostJudiciaryPersonBySearchTerm_Should_Return_EmptyList_When_EJudFlag_Is_False()
        {
            var searchTermRequest = new SearchTermRequest("test");
            _featureFlagsService.Setup(p => p.GetFeatureFlag(It.Is<string>(p => p == nameof(FeatureFlags.EJudFeature)))).Returns(false);
            var result = _controller.PostJudiciaryPersonBySearchTerm(searchTermRequest);

            result.Should().NotBeNull();
            var objectResult = (ObjectResult)result.Result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var personResponses = (List<PersonResponse>)objectResult.Value;
            personResponses.Count.Should().Be(0);
        }
        
        private static void AssertErrorMessageContainsIdentifier(string errorMessage, string identifier)
        {
            var expectedErrorMessage = $"Could not add or update external Judiciary user with Personal Code: {identifier}";
            errorMessage.Should().StartWith($"{expectedErrorMessage} - ");
        }
    }
}