using Bookings.API.Controllers;
using Bookings.DAL.Queries.Core;
using Bookings.Domain;
using Moq;
using NUnit.Framework;
using System;
using Testing.Common.Builders.Domain;
using System.Linq;
using Bookings.Domain.Participants;

namespace Bookings.UnitTests.Controllers
{
    public class PersonsControllerTest
    {
        protected PersonsController _controller;
        protected Mock<IQueryHandler> _queryHandlerMock;

        [SetUp]
        public void Setup()
        {
            _queryHandlerMock = new Mock<IQueryHandler>();
            _controller = new PersonsController(_queryHandlerMock.Object);
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
