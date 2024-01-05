using System.Collections.Generic;
using System.Net;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Domain;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Queries;
using Microsoft.AspNetCore.Mvc;
using Testing.Common.Assertions;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Domain.Participants;
using BookingsApi.Contract.V1.Responses;

namespace BookingsApi.UnitTests.Controllers.HearingsController
{
    public class CloneHearingTests : HearingsControllerTests
    {

        [Test]
        public async Task Should_return_not_clone_without_videohearing()
        {
            QueryHandlerMock
              .Setup(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
              .ReturnsAsync((VideoHearing)null);

            var result = await Controller.CloneHearing(Guid.NewGuid(), new CloneHearingRequest());

            result.Should().NotBeNull();
            var objectResult = (NotFoundResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Should_return_badrequest_without_valid_videohearing()
        {
            var hearing = GetHearing("123");
            QueryHandlerMock
                .Setup(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
                .ReturnsAsync(hearing);

            var result = await Controller.CloneHearing(Guid.NewGuid(), new CloneHearingRequest { Dates = new List<DateTime> { DateTime.Now.AddYears(-1) } });

            result.Should().NotBeNull();
            var objectResult = (ObjectResult)result;
            ((ValidationProblemDetails)objectResult.Value).ContainsKeyAndErrorMessage("Dates", "Dates cannot be before original hearing");
        }

        [Test]
        public async Task Should_successfully_clone_hearing_with_judge()
        {
            var hearingId = Guid.NewGuid();
            var request = new CloneHearingRequest { Dates = new List<DateTime> { DateTime.Now.AddDays(2), DateTime.Now.AddDays(3) } };
            var hearing = GetHearing("123");
            var judgeAsExistingParticipantCount = 1;
            var caseName = $"{hearing.GetCases().First().Name} Day {1} of 3";
            QueryHandlerMock
                .Setup(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
                .ReturnsAsync(hearing);

            var result = await Controller.CloneHearing(hearingId, request);
            
            result.Should().NotBeNull();
            var objectResult = (OkObjectResult)result;
            objectResult.Should().NotBeNull();
            objectResult.Value.Should().BeOfType<List<HearingDetailsResponse>>();
            CommandHandlerMock.Verify(c => c.Handle(It.Is<CreateVideoHearingCommand>(c => c.ScheduledDateTime == request.Dates[0] && c.Cases[0].Name == "Case name Day 2 of 3")), Times.Once);
            CommandHandlerMock.Verify(c => c.Handle(It.Is<CreateVideoHearingCommand>(c => c.ScheduledDateTime == request.Dates[1] && c.Cases[0].Name == "Case name Day 3 of 3")), Times.Once);
            HearingServiceMock.Verify(h => h.UpdateHearingCaseName(It.Is<Guid>(g => g == hearingId), It.Is<string>(x => x == caseName)), Times.Once);

            EventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<NewParticipantMultidayHearingConfirmationEvent>()), Times.Exactly(hearing.Participants.Count(x => x is not JudicialOfficeHolder)));
            EventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<ExistingParticipantMultidayHearingConfirmationEvent>()), Times.Exactly(judgeAsExistingParticipantCount));
        }

        [Test]
        public async Task Should_successfully_clone_hearing_without_judge()
        {
            var hearingId = Guid.NewGuid();
            var request = new CloneHearingRequest { Dates = new List<DateTime> { DateTime.Now.AddDays(2), DateTime.Now.AddDays(3) } };
            var hearing = GetHearing("123");
            var caseName = $"{hearing.GetCases().First().Name} Day {1} of 3";

            hearing.Participants.Remove(hearing.Participants.Single(x => x.HearingRole.Name == "Judge"));
            QueryHandlerMock
                .Setup(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
                .ReturnsAsync(hearing);

            var result = await Controller.CloneHearing(hearingId, request);

            result.Should().NotBeNull();
            var objectResult = (OkObjectResult)result;
            objectResult.Should().NotBeNull();
            objectResult.Value.Should().BeOfType<List<HearingDetailsResponse>>();
            CommandHandlerMock.Verify(c => c.Handle(It.Is<CreateVideoHearingCommand>(c => c.ScheduledDateTime == request.Dates[0] && c.Cases[0].Name == "Case name Day 2 of 3")), Times.Once);
            CommandHandlerMock.Verify(c => c.Handle(It.Is<CreateVideoHearingCommand>(c => c.ScheduledDateTime == request.Dates[1] && c.Cases[0].Name == "Case name Day 3 of 3")), Times.Once);
            HearingServiceMock.Verify(h => h.UpdateHearingCaseName(It.Is<Guid>(g => g == hearingId), It.Is<string>(x => x == caseName)), Times.Once);

            EventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<HearingIsReadyForVideoIntegrationEvent>()), Times.Exactly(request.Dates.Count));
            EventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<NewParticipantMultidayHearingConfirmationEvent>()), Times.Exactly(
                    hearing.Participants.Count(x => x is not Judge)));

            CommandHandlerMock.Verify(x => x.Handle(It.IsAny<UpdateHearingStatusCommand>()), Times.Never);
        }

    }
}
