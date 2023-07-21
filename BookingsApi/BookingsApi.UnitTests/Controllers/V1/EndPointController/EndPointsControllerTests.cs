using System;
using System.Collections.Generic;
using BookingsApi.Common.Configuration;
using BookingsApi.Contract.Requests;
using BookingsApi.Controllers.V1;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using BookingsApi.Domain.RefData;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Testing.Common.Builders.Domain;
using IRandomGenerator = BookingsApi.Common.Services.IRandomGenerator;

namespace BookingsApi.UnitTests.Controllers.V1.EndPointController
{
    public class EndPointsControllerTests
    {
        protected AddEndpointRequest AddEndpointRequest;
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
            AddEndpointRequest = new AddEndpointRequest {DisplayName = "DisplayNameAdded"};

            QueryHandler = new Mock<IQueryHandler>();
            CommandHandlerMock = new Mock<ICommandHandler>();
            RandomGenerator = new Mock<IRandomGenerator>();
            EventPublisher = new Mock<IEventPublisher>();
            KinlyConfiguration = new KinlyConfiguration {SipAddressStem = "@hmcts.net"};

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

            if (createdStatus)
                Hearing.UpdateStatus(BookingsApi.Domain.Enumerations.BookingStatus.Created, "administrator", string.Empty);

            var endpoint = new Endpoint("one", $"{Guid.NewGuid().ToString()}{KinlyConfiguration.SipAddressStem}",
                "1234", null);
            Hearing.AddEndpoint(endpoint);
            return Hearing;
        }

        private CaseType CaseType => new CaseType(1, "Civil")
        {
            CaseRoles = new List<CaseRole>
            {
                CreateCaseAndHearingRoles(1, "Generic", "representative", new List<string> {"Litigant in person"})
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
