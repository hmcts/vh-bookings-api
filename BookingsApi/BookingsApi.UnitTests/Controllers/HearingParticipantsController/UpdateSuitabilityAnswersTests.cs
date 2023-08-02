using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Domain.Participants;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Testing.Common.Assertions;

namespace BookingsApi.UnitTests.Controllers.HearingParticipantsController
{
    public class UpdateSuitabilityAnswersTests : HearingParticipantsControllerTest
    {
        private List<SuitabilityAnswersRequest> answers;

        [SetUp]
        public void TestInitialize()
        {
            answers = new List<SuitabilityAnswersRequest> {
                                    new SuitabilityAnswersRequest { Key = "Test", Answer = "TestA" },
                                     new SuitabilityAnswersRequest { Key = "Tester", Answer = "TestB" }
                            };
        }

        [Test]
        public async Task Should_update_participant_in_hearing_for_given_hearing_and_participantid()
        {

            var response = await Controller.UpdateSuitabilityAnswers(hearingId, participantId, answers);

            response.Should().NotBeNull();
            var objectResult = (NoContentResult)response;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
            CommandHandler.Verify(c => c.Handle(It.IsAny<UpdateSuitabilityAnswersCommand>()), Times.Once);
        }

        [Test]
        public async Task Should_return_badrequest_for_given_invalid_hearingid()
        {
            hearingId = Guid.Empty;

            var result = await Controller.UpdateSuitabilityAnswers(hearingId, participantId, answers);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
            QueryHandler.Verify(q => q.Handle<GetParticipantsInHearingQuery, List<Participant>>(It.IsAny<GetParticipantsInHearingQuery>()), Times.Never);
        }

        [Test]
        public async Task Should_return_notfound_with_HearingNotFoundException()
        {
            CommandHandler.Setup(c => c.Handle(It.IsAny<UpdateSuitabilityAnswersCommand>())).ThrowsAsync(new HearingNotFoundException(Guid.NewGuid()));

            var result = await Controller.UpdateSuitabilityAnswers(hearingId, participantId, answers);

            result.Should().NotBeNull();
            var objectResult = (NotFoundObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            objectResult.Value.Should().Be("Hearing not found");
        }

        [Test]
        public async Task Should_return_notfound_with_ParticipantNotFoundException()
        {
            CommandHandler.Setup(c => c.Handle(It.IsAny<UpdateSuitabilityAnswersCommand>())).ThrowsAsync(new ParticipantNotFoundException(Guid.NewGuid()));

            var result = await Controller.UpdateSuitabilityAnswers(hearingId, participantId, answers);

            result.Should().NotBeNull();
            var objectResult = (NotFoundObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            objectResult.Value.Should().Be("Participant not found");
        }

        [Test]
        public async Task Should_return_badrequest_for_given_invalid_participantid()
        {
            participantId = Guid.Empty;

            var result = await Controller.UpdateSuitabilityAnswers(hearingId, participantId, answers);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(participantId), $"Please provide a valid {nameof(participantId)}");
            QueryHandler.Verify(q => q.Handle<GetParticipantsInHearingQuery, List<Participant>>(It.IsAny<GetParticipantsInHearingQuery>()), Times.Never);
        }

        [Test]
        public async Task Should_return_badrequest_for_given_duplicated_answers()
        {
            answers.Add(new SuitabilityAnswersRequest { Key = "Test", Answer = "Testb" });

            var result = await Controller.UpdateSuitabilityAnswers(hearingId, participantId, answers);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(participantId), $"Request '{nameof(answers)}' cannot contain duplicate keys.");
            QueryHandler.Verify(q => q.Handle<GetParticipantsInHearingQuery, List<Participant>>(It.IsAny<GetParticipantsInHearingQuery>()), Times.Never);
        }
    }
}
