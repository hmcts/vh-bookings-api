using System;
using System.Linq;
using BookingsApi.Controllers.V1;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace BookingsApi.UnitTests.Controllers.V1.Persons
{
    public class PersonsControllerTest
    {
        protected PersonsController Controller;
        protected Mock<IQueryHandler> QueryHandlerMock;
        protected Mock<ICommandHandler> CommandHandlerMock;
        protected Mock<IEventPublisher> EventPublisherMock;
        protected Mock<ILogger<PersonsController>> LoggerMock;

        [SetUp]
        public void Setup()
        {
            QueryHandlerMock = new Mock<IQueryHandler>();
            CommandHandlerMock = new Mock<ICommandHandler>();
            EventPublisherMock = new Mock<IEventPublisher>();
            LoggerMock = new Mock<ILogger<PersonsController>>();
            Controller = new PersonsController(QueryHandlerMock.Object, CommandHandlerMock.Object,
                EventPublisherMock.Object, LoggerMock.Object);
        }
        
        protected VideoHearing TestData(bool addSuitability = true)
        {
            var builder = new VideoHearingBuilder();
            var hearing = builder.Build();
            if (addSuitability)
            {
                var participant = hearing.Participants.FirstOrDefault(p => p is Individual);
                if (participant != null)
                {
                    var answer = new SuitabilityAnswer("AboutYou", "Yes", "")
                    {
                        UpdatedDate = DateTime.Now.AddDays(-2)
                    };

                    participant.Questionnaire = new Questionnaire
                    {
                        Participant = participant,
                        ParticipantId = participant.Id
                    };

                    participant.Questionnaire.SuitabilityAnswers.Add(answer);
                    participant.Questionnaire.UpdatedDate = DateTime.Now.AddDays(-2);
                }

            }
            return hearing;
        }
    }
}
