using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.Controllers;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Controllers
{
    public class JudiciaryPersonControllerTests
    {
        protected JudiciaryPersonController Controller;
        protected Mock<IQueryHandler> QueryHandlerMock;
        protected Mock<ICommandHandler> CommandHandlerMock;
        protected Mock<ILogger<JudiciaryPersonController>> LoggerMock;

        [SetUp]
        public void Setup()
        {
            QueryHandlerMock = new Mock<IQueryHandler>();
            CommandHandlerMock = new Mock<ICommandHandler>();
            LoggerMock = new Mock<ILogger<JudiciaryPersonController>>();
            Controller = new JudiciaryPersonController(QueryHandlerMock.Object, CommandHandlerMock.Object, LoggerMock.Object);
        }

        [Test]
        public async Task Should_return_ok_result_with_empty_response_for_zero_bulk_inserts()
        {
            var request = new List<JudiciaryPersonRequest>();

            var result = await Controller.BulkJudiciaryPersonsAsync(request);

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
            var item1 = new JudiciaryPersonRequest{Id = Guid.NewGuid(), Email = "some@email.com", Fullname = "a", Surname = "b", Title = "c", KnownAs = "d", PersonalCode = "123", PostNominals = "nom1"};
            var request = new List<JudiciaryPersonRequest> { item1 };
            
            QueryHandlerMock
                .Setup(x => x.Handle<GetJudiciaryPersonByExternalRefIdQuery, JudiciaryPerson>(It.IsAny<GetJudiciaryPersonByExternalRefIdQuery>()))
                .ReturnsAsync((JudiciaryPerson) null);

            var result = await Controller.BulkJudiciaryPersonsAsync(request);

            result.Should().NotBeNull();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var data = objectResult.Value as BulkJudiciaryPersonResponse;
            data.Should().NotBeNull();
            data.ErroredRequests.Count.Should().Be(0);

            CommandHandlerMock.Verify(c => c.Handle(It.Is<AddJudiciaryPersonCommand>
            (
                c => c.Email == item1.Email && c.Fullname == item1.Fullname && c.Surname == item1.Surname && 
                     c.Title == item1.Title && c.KnownAs == item1.KnownAs && c.PersonalCode == item1.PersonalCode && 
                     c.PostNominals == item1.PostNominals && c.ExternalRefId == item1.Id
            )));
        }
        
        [Test]
        public async Task Should_return_ok_result_updating_item()
        {
            var item1 = new JudiciaryPersonRequest{Id = Guid.NewGuid(), Email = "some@email.com", Fullname = "a", Surname = "b", Title = "c", KnownAs = "d", PersonalCode = "123", PostNominals = "nom1"};
            var retrievedPerson1 = new JudiciaryPerson(item1.Id, item1.PersonalCode, item1.Title, item1.KnownAs, item1.Surname, item1.Fullname, item1.PostNominals, item1.Email);
            var request = new List<JudiciaryPersonRequest> { item1 };
            
            QueryHandlerMock
                .Setup(x => x.Handle<GetJudiciaryPersonByExternalRefIdQuery, JudiciaryPerson>(It.IsAny<GetJudiciaryPersonByExternalRefIdQuery>()))
                .ReturnsAsync(retrievedPerson1);

            var result = await Controller.BulkJudiciaryPersonsAsync(request);

            result.Should().NotBeNull();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var data = objectResult.Value as BulkJudiciaryPersonResponse;
            data.Should().NotBeNull();
            data.ErroredRequests.Count.Should().Be(0);

            CommandHandlerMock.Verify(c => c.Handle(It.Is<UpdateJudiciaryPersonByExternalRefIdCommand>
            (
                c => c.Email == item1.Email && c.Fullname == item1.Fullname && c.Surname == item1.Surname && 
                     c.Title == item1.Title && c.KnownAs == item1.KnownAs && c.PersonalCode == item1.PersonalCode && 
                     c.PostNominals == item1.PostNominals && c.ExternalRefId == item1.Id
            )));
        }
        
        [Test]
        public async Task Should_return_ok_result_adding_and_updating_item()
        {
            var item1 = new JudiciaryPersonRequest{Id = Guid.NewGuid(), Email = "some@email.com", Fullname = "a", Surname = "b", Title = "c", KnownAs = "d", PersonalCode = "123", PostNominals = "nom1"};
            var item2 = new JudiciaryPersonRequest{Id = Guid.NewGuid(), Email = "some2@email.com", Fullname = "a2", Surname = "b2", Title = "c2", KnownAs = "d2", PersonalCode = "456", PostNominals = "nom2"};
            var retrievedPerson1 = new JudiciaryPerson(item2.Id, item2.PersonalCode, item2.Title, item2.KnownAs, item2.Surname, item2.Fullname, item2.PostNominals, item2.Email);
            var request = new List<JudiciaryPersonRequest> { item1, item2 };
            
            QueryHandlerMock
                .Setup(x => x.Handle<GetJudiciaryPersonByExternalRefIdQuery, JudiciaryPerson>(It.IsAny<GetJudiciaryPersonByExternalRefIdQuery>()))
                .ReturnsAsync(retrievedPerson1);
            
            QueryHandlerMock
                .Setup(x => x.Handle<GetJudiciaryPersonByExternalRefIdQuery, JudiciaryPerson>(It.Is<GetJudiciaryPersonByExternalRefIdQuery>(x => x.ExternalRefId == item1.Id)))
                .ReturnsAsync((JudiciaryPerson) null);

            var result = await Controller.BulkJudiciaryPersonsAsync(request);

            result.Should().NotBeNull();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var data = objectResult.Value as BulkJudiciaryPersonResponse;
            data.Should().NotBeNull();
            data.ErroredRequests.Count.Should().Be(0);

            CommandHandlerMock.Verify(c => c.Handle(It.Is<AddJudiciaryPersonCommand>
            (
                c => c.Email == item1.Email && c.Fullname == item1.Fullname && c.Surname == item1.Surname && 
                     c.Title == item1.Title && c.KnownAs == item1.KnownAs && c.PersonalCode == item1.PersonalCode && 
                     c.PostNominals == item1.PostNominals && c.ExternalRefId == item1.Id
            )));
            
            CommandHandlerMock.Verify(c => c.Handle(It.Is<UpdateJudiciaryPersonByExternalRefIdCommand>
            (
                c => c.Email == item2.Email && c.Fullname == item2.Fullname && c.Surname == item2.Surname && 
                     c.Title == item2.Title && c.KnownAs == item2.KnownAs && c.PersonalCode == item2.PersonalCode && 
                     c.PostNominals == item2.PostNominals && c.ExternalRefId == item2.Id
            )));
        }

        [Test]
        public async Task Should_return_error_items_in_request_for_bad_request_no_id()
        {
            var requestNoEmail = new JudiciaryPersonRequest
            {
                Fullname = "a",
                Surname = "b",
                Title = "c",
                KnownAs = "d",
                PersonalCode = "123",
                PostNominals = "nom1",
                Email = "some@email.com"
            };
            
            var result = await Controller.BulkJudiciaryPersonsAsync(new List<JudiciaryPersonRequest> { requestNoEmail });

            result.Should().NotBeNull();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var data = objectResult.Value as BulkJudiciaryPersonResponse;
            data.Should().NotBeNull();
            data.ErroredRequests.Count.Should().Be(1);
            data.ErroredRequests[0].JudiciaryPersonRequest.Should().BeEquivalentTo(requestNoEmail);
        }
        
        [Test]
        public async Task Should_return_error_items_in_request_for_bad_request_no_knownas()
        {
            var requestNoEmail = new JudiciaryPersonRequest
            {
                Id = Guid.NewGuid(),
                Fullname = "a",
                Surname = "b",
                Title = "c",
                PersonalCode = "123",
                PostNominals = "nom1",
                Email = "some@email.com"
            };
            
            var result = await Controller.BulkJudiciaryPersonsAsync(new List<JudiciaryPersonRequest> { requestNoEmail });

            result.Should().NotBeNull();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var data = objectResult.Value as BulkJudiciaryPersonResponse;
            data.Should().NotBeNull();
            data.ErroredRequests.Count.Should().Be(1);
            data.ErroredRequests[0].JudiciaryPersonRequest.Should().BeEquivalentTo(requestNoEmail);
        }

        [Test]
        public async Task Should_return_error_items_in_request_for_bad_request_no_surname()
        {
            var requestNoEmail = new JudiciaryPersonRequest
            {
                Id = Guid.NewGuid(),
                Fullname = "a",
                Title = "c",
                KnownAs = "d",
                PersonalCode = "123",
                PostNominals = "nom1",
                Email = "some@email.com"
            };
            
            var result = await Controller.BulkJudiciaryPersonsAsync(new List<JudiciaryPersonRequest> { requestNoEmail });

            result.Should().NotBeNull();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var data = objectResult.Value as BulkJudiciaryPersonResponse;
            data.Should().NotBeNull();
            data.ErroredRequests.Count.Should().Be(1);
            data.ErroredRequests[0].JudiciaryPersonRequest.Should().BeEquivalentTo(requestNoEmail);
        }

        [Test]
        public async Task Should_return_error_items_in_request_for_bad_request_no_email()
        {
            var requestNoEmail = new JudiciaryPersonRequest
            {
                Id = Guid.NewGuid(),
                Fullname = "a",
                Surname = "b",
                Title = "c",
                KnownAs = "d",
                PersonalCode = "123",
                PostNominals = "nom1"
            };
            
            var result = await Controller.BulkJudiciaryPersonsAsync(new List<JudiciaryPersonRequest> { requestNoEmail });

            result.Should().NotBeNull();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var data = objectResult.Value as BulkJudiciaryPersonResponse;
            data.Should().NotBeNull();
            data.ErroredRequests.Count.Should().Be(1);
            data.ErroredRequests[0].JudiciaryPersonRequest.Should().BeEquivalentTo(requestNoEmail);
        }
        
        [Test]
        public async Task Should_return_error_items_in_request_exception()
        {
            var item1 = new JudiciaryPersonRequest{Id = Guid.NewGuid(), Email = "some@email.com", Fullname = "a", Surname = "b", Title = "c", KnownAs = "d", PersonalCode = "123", PostNominals = "nom1"};
            var request = new List<JudiciaryPersonRequest> { item1 };

            QueryHandlerMock
                .Setup(x => x.Handle<GetJudiciaryPersonByExternalRefIdQuery, JudiciaryPerson>(It.IsAny<GetJudiciaryPersonByExternalRefIdQuery>()))
                .ThrowsAsync(new Exception("Error"));

            var result = await Controller.BulkJudiciaryPersonsAsync(request);

            result.Should().NotBeNull();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var data = objectResult.Value as BulkJudiciaryPersonResponse;
            data.Should().NotBeNull();
            data.ErroredRequests.Count.Should().Be(1);
            data.ErroredRequests[0].JudiciaryPersonRequest.Should().BeEquivalentTo(item1);

            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<AddJudiciaryPersonCommand>()), Times.Never);
        }
    }
}