using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Domain;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Testing.Common.Assertions;

namespace BookingsApi.UnitTests.Controllers.EndPointController
{
    public class AddEndPointToHearingTests : EndPointsControllerTests
    {
        [Test]
        public async Task Should_add_endpoint_to_hearing()
        {
            var updatedHearing = GetVideoHearing(true);
            updatedHearing.AddEndpoint(new Endpoint(AddEndpointRequest.DisplayName, "sip", "pin", null));
            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
                .ReturnsAsync(updatedHearing);
            
            var response = await Controller.AddEndPointToHearingAsync(HearingId, AddEndpointRequest);

            response.Should().NotBeNull();
            var result = (OkObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<AddEndPointToHearingCommand>()), Times.Once);

            EventPublisher.Verify(x => x.PublishAsync(
                    It.Is<EndpointAddedIntegrationEvent>(r =>
                        r.HearingId == HearingId && r.Endpoint.Pin == "pin" && r.Endpoint.Sip == "sip" &&
                        r.Endpoint.DisplayName == AddEndpointRequest.DisplayName && r.Endpoint.DefenceAdvocateContactEmail == null)),
                Times.Once);
        }

        [Test]
        public async Task Should_add_endpoint_without_matching_advocate_and_no_send_event()
        {
            var updatedHearing = GetVideoHearing(true); 
            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
                .ReturnsAsync(updatedHearing);

            var response = await Controller.AddEndPointToHearingAsync(HearingId, AddEndpointRequest);

            response.Should().NotBeNull();
            var result = (NoContentResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<AddEndPointToHearingCommand>()), Times.Once);

            EventPublisher.Verify(x => x.PublishAsync(It.IsAny<EndpointAddedIntegrationEvent>()), Times.Never);
        }

        [Test]
        public async Task Should_add_endpoint_wth_defence_advocate_and_send_event()
        {
            var updatedHearing = GetVideoHearing(true);
            var rep = updatedHearing.Participants.First(x => x.HearingRole.UserRole.IsRepresentative);
            AddEndpointRequest.DefenceAdvocateContactEmail = rep.Person.ContactEmail;
            
            updatedHearing.AddEndpoint(new Endpoint(AddEndpointRequest.DisplayName, "sip", "pin", rep));
            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
                .ReturnsAsync(updatedHearing);

            var response = await Controller.AddEndPointToHearingAsync(HearingId, AddEndpointRequest);

            response.Should().NotBeNull();
            var result = (OkObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<AddEndPointToHearingCommand>()), Times.Once);
            EventPublisher.Verify(x => x.PublishAsync(
                    It.Is<EndpointAddedIntegrationEvent>(r =>
                        r.HearingId == HearingId && r.Endpoint.Pin == "pin" && r.Endpoint.Sip == "sip" &&
                        r.Endpoint.DisplayName == AddEndpointRequest.DisplayName && r.Endpoint.DefenceAdvocateContactEmail == AddEndpointRequest.DefenceAdvocateContactEmail)),
                Times.Once);
        }

        [Test]
        public async Task Returns_Endpoint_Response()
        {
            var updatedHearing = GetVideoHearing(true);
            var rep = updatedHearing.Participants.First(x => x.HearingRole.UserRole.IsRepresentative);
            AddEndpointRequest.DefenceAdvocateContactEmail = rep.Person.ContactEmail;
            var endpoint = new Endpoint(AddEndpointRequest.DisplayName, "sip", "pin", rep);
            
            updatedHearing.AddEndpoint(endpoint);
            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
                .ReturnsAsync(updatedHearing);
            
            var response = await Controller.AddEndPointToHearingAsync(HearingId, AddEndpointRequest);
            
            response.Should().NotBeNull();
            var result = (OkObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var resultObject = result.Value as EndpointResponse;

            resultObject.Id.Should().Be(endpoint.Id);
            resultObject.DisplayName.Should().Be(endpoint.DisplayName);
            resultObject.Sip.Should().Be(endpoint.Sip);
            resultObject.Pin.Should().Be(endpoint.Pin);
            resultObject.DefenceAdvocateId.Should().Be(endpoint.DefenceAdvocate.Id);
        }
        
        [Test]
        public async Task Should_return_badrequest_for_given_invalid_hearingid()
        {
            var hearingId = Guid.Empty;
            var result = await Controller.AddEndPointToHearingAsync(hearingId, AddEndpointRequest);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
        }

        [Test]
        public async Task Should_return_badrequest_for_given_invalid_request()
        {
            var result = await Controller.AddEndPointToHearingAsync(HearingId, new AddEndpointRequest());

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage("DisplayName", "DisplayName is required");
        }

        [Test]
        public async Task Should_return_notfound_for_given_invalid_hearingId()
        {
            CommandHandlerMock.Setup(c => c.Handle(It.IsAny<AddEndPointToHearingCommand>())).ThrowsAsync(new HearingNotFoundException(Guid.NewGuid()));

            var result = await Controller.AddEndPointToHearingAsync(HearingId, AddEndpointRequest);

            result.Should().NotBeNull();
            var objectResult = (NotFoundObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
