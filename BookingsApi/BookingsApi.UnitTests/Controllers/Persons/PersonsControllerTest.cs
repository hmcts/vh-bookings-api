﻿using System;
using System.Linq;
using BookingsApi.Controllers;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using Bookings.Infrastructure.Services.IntegrationEvents;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Queries.Core;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace BookingsApi.UnitTests.Controllers.Persons
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
