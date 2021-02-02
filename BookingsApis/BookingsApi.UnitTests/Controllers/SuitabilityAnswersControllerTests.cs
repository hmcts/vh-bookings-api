using BookingsApi.Contract.Responses;
using BookingsApi.Controllers;
using Bookings.Domain;
using Bookings.Domain.Participants;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using Testing.Common.Builders.Domain;

namespace BookingsApi.UnitTests.Controllers
{
    public class SuitabilityAnswersControllerTests
    {
        private Mock<IQueryHandler> queryHandler;
        private SuitabilityAnswersController suitabilityAnswersController;

        [SetUp]
        public void TestInitialize()
        {
            var builder = new VideoHearingBuilder();
            var hearing = builder.Build();
            var participants = new List<Participant> { Create(hearing, "Test", "Tester", 1), Create(hearing, "Tester", "Test", 2) };

            queryHandler = new Mock<IQueryHandler>();
            queryHandler.Setup(q =>
                    q.Handle<GetParticipantWithSuitabilityAnswersQuery, CursorPagedResult<Participant, string>>(It.IsAny<GetParticipantWithSuitabilityAnswersQuery>()))
                    .ReturnsAsync(new CursorPagedResult<Participant, string>(participants, "next-cursor"));

            suitabilityAnswersController = new SuitabilityAnswersController(queryHandler.Object);
        }

        private Participant Create(VideoHearing hearing,string fName, string lName,int addHour)
        {
            var participant = new Mock<Participant>();
            participant.SetupGet(p => p.Hearing).Returns(hearing);
            participant.SetupGet(p => p.Questionnaire).Returns(new Questionnaire { UpdatedDate = DateTime.Now.AddHours(addHour) });
            participant.SetupGet(p => p.Person).Returns(new Person("Mr", fName, lName,  $"{fName} {lName}"));
            participant.SetupGet(p => p.HearingRole).Returns(new Bookings.Domain.RefData.HearingRole(1,"Test"));

            return participant.Object;
        }

        [Test]
        public async Task Should_return_SuitabilityAnswersResponse_without_cursor_for_given_query()
        {
            var result = await suitabilityAnswersController.GetSuitabilityAnswers("0",1);

            result.Should().NotBeNull();            
            var objectResult = (ObjectResult)result.Result;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var suitabilityAnswer = (SuitabilityAnswersResponse) objectResult.Value;
            suitabilityAnswer.PrevPageUrl.Should().Be("suitability-answers/?cursor=0&limit=1");
            suitabilityAnswer.NextPageUrl.Should().Be("suitability-answers/?cursor=next-cursor&limit=1");
            suitabilityAnswer.NextCursor.Should().Be("next-cursor");
            suitabilityAnswer.ParticipantSuitabilityAnswerResponse[0].FirstName.Should().Be("Tester");
            suitabilityAnswer.ParticipantSuitabilityAnswerResponse[0].CaseNumber.Should().BeNullOrEmpty();
            suitabilityAnswer.ParticipantSuitabilityAnswerResponse[0].Representee.Should().BeNullOrEmpty();
            suitabilityAnswer.ParticipantSuitabilityAnswerResponse[1].FirstName.Should().Be("Test");
            queryHandler.Verify(q =>
                    q.Handle<GetParticipantWithSuitabilityAnswersQuery, CursorPagedResult<Participant, string>>(It.Is<GetParticipantWithSuitabilityAnswersQuery>(g => g.Cursor == null)),
                    Times.Once);
        }

        [Test]
        public async Task Should_return_SuitabilityAnswersResponse_with_cursor_for_given_query()
        {
            var result = await suitabilityAnswersController.GetSuitabilityAnswers("2", 1);

            result.Should().NotBeNull();
            var objectResult = (ObjectResult)result.Result;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var suitabilityAnswer = (SuitabilityAnswersResponse)objectResult.Value;
            suitabilityAnswer.PrevPageUrl.Should().Be("suitability-answers/?cursor=2&limit=1");
            suitabilityAnswer.NextPageUrl.Should().Be("suitability-answers/?cursor=next-cursor&limit=1");
            suitabilityAnswer.NextCursor.Should().Be("next-cursor");
            queryHandler.Verify(q =>
                   q.Handle<GetParticipantWithSuitabilityAnswersQuery, CursorPagedResult<Participant, string>>(It.Is<GetParticipantWithSuitabilityAnswersQuery>(g => g.Cursor == "2")),
                   Times.Once);
        }
    }
}
