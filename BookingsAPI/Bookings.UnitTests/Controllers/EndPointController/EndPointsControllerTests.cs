using Bookings.Api.Contract.Requests;
using Bookings.API.Controllers;
using Bookings.Common.Configuration;
using Bookings.DAL.Commands.Core;
using Bookings.Infrastructure.Services.IntegrationEvents;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Bookings.DAL.Queries;
using Bookings.DAL.Queries.Core;
using Bookings.Domain;
using Bookings.Domain.RefData;
using FizzWare.NBuilder;
using Testing.Common.Builders.Domain;
using IRandomGenerator = Bookings.Common.Services.IRandomGenerator;

namespace Bookings.UnitTests.Controllers.EndPointController
{
    public class EndPointsControllerTests
    {
        protected AddEndpointRequest Request;
        protected Guid HearingId;
        protected VideoHearing Hearing;
        protected Guid EndpointId;

        protected Mock<IQueryHandler> QueryHandler;
        protected Mock<ICommandHandler> CommandHandlerMock;
        protected Mock<IRandomGenerator> RandomGenerator;
        protected Mock<IEventPublisher> EventPublisher;
        protected KinlyConfiguration KinlyConfiguration;

        protected EndPointsController Controller;

        [SetUp]
        public void TestInitialize()
        {
            HearingId = Guid.NewGuid();
            EndpointId = Guid.NewGuid();
            Request = new AddEndpointRequest {DisplayName = "DisplayNameAdded"};

            QueryHandler = new Mock<IQueryHandler>();
            CommandHandlerMock = new Mock<ICommandHandler>();
            RandomGenerator = new Mock<IRandomGenerator>();
            EventPublisher = new Mock<IEventPublisher>();
            KinlyConfiguration = new KinlyConfiguration {SipAddressStem = "@videohearings.com"};

            Controller = new EndPointsController(
                CommandHandlerMock.Object,
                RandomGenerator.Object,
                new OptionsWrapper<KinlyConfiguration>(KinlyConfiguration),
                EventPublisher.Object, QueryHandler.Object);

            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
                .ReturnsAsync(GetVideoHearing(true));
        }

        protected VideoHearing GetVideoHearing(bool createdStatus = false)
        {
            Hearing = new VideoHearingBuilder().Build();
            Hearing.AddCase("123", "Case name", true);
            Hearing.CaseType = CaseType;
            foreach (var participant in Hearing.Participants)
            {
                participant.HearingRole = new HearingRole(1, "Name") {UserRole = new UserRole(1, "User"),};
                participant.CaseRole = new CaseRole(1, "Civil Money Claims");
            }

            if (createdStatus)
                Hearing.UpdateStatus(Bookings.Domain.Enumerations.BookingStatus.Created, "administrator", string.Empty);

            var endpoint = new Endpoint("one", Guid.NewGuid().ToString(), "1234", null);
            Hearing.AddEndpoint(endpoint);
            return Hearing;
        }

        private CaseType CaseType => new CaseType(1, "Civil")
        {
            CaseRoles = new List<CaseRole>
            {
                CreateCaseAndHearingRoles(1, "Civil Money Claims", "representative", new List<string> {"Claimant LIP"})
            }
        };

        protected CaseRole CreateCaseAndHearingRoles(int caseId, string caseRoleName, string userRole,
            List<string> roles)
        {
            var hearingRoles = new List<HearingRole>();

            foreach (var role in roles)
            {
                hearingRoles.Add(new HearingRole(1, role) {UserRole = new UserRole(1, userRole)});
            }

            var caseRole = new CaseRole(caseId, caseRoleName) {HearingRoles = hearingRoles};

            return caseRole;
        }
    }
}
