using System;
using System.Collections.Generic;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using Moq;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace BookingsApi.UnitTests.Controllers.HearingParticipantsController
{
    public class HearingParticipantsControllerTest
    {
        protected Mock<IQueryHandler> QueryHandler;
        protected Mock<ICommandHandler> CommandHandler;
        protected Mock<IEventPublisher> EventPublisher;
        private List<Participant> participants;

        protected List<Participant> Participants
        {
            get
            {
                if (participants != null) return participants;

                participants = new ParticipantBuilder().Build();

                foreach(var participant in participants)
                {
                    participant.DisplayName = "Test Participant";
                    participant.CaseRole = new CaseRole(1, "TestCaseRole");

                    if(participant.HearingRole.UserRole.Name == "Representative")
                    {
                        var representative = (Representative)participant;
                        representative.Representee = "Representee";
                    }
                }

                return participants;
            }
        }

        protected BookingsApi.Controllers.HearingParticipantsController Controller;

        protected Guid hearingId;
        protected Guid participantId;
        protected VideoHearing videoHearing;

        protected VideoHearing GetVideoHearing(bool createdStatus = false)
        { 
            var hearing = new VideoHearingBuilder().Build();
            hearing.AddCase("123", "Case name", true);
            hearing.CaseType = CaseType;
            foreach (var participant in hearing.Participants)
            {
                participant.HearingRole = new HearingRole(1, "Name") { UserRole = new UserRole(1, "User"), };
                participant.CaseRole = new CaseRole(1, "Generic");
            }

            if(createdStatus)
                hearing.UpdateStatus(BookingsApi.Domain.Enumerations.BookingStatus.Created, "administrator", string.Empty);

            return hearing; 
        }

        private CaseType CaseType => new CaseType(1, "Civil") { CaseRoles = new List<CaseRole> { 
            CreateCaseAndHearingRoles(1, "Generic", "representative", new List<string> { "Litigant in person" }),
            CreateCaseAndHearingRoles(2, "Test", "Judge", new List<string> { "Judge" })
        } };

        [SetUp]
        public void Intialize()
        {
            QueryHandler = new Mock<IQueryHandler>();
            CommandHandler = new Mock<ICommandHandler>();
            EventPublisher = new Mock<IEventPublisher>();

            hearingId = Guid.NewGuid();
            participantId = Guid.NewGuid();
            videoHearing = GetVideoHearing();
            Controller = new BookingsApi.Controllers.HearingParticipantsController(QueryHandler.Object, CommandHandler.Object, EventPublisher.Object);
            
            QueryHandler.Setup(q => q.Handle<GetParticipantsInHearingQuery, List<Participant>>(It.IsAny<GetParticipantsInHearingQuery>()))
                .ReturnsAsync(Participants);

            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(videoHearing);
             
            QueryHandler.Setup(q => q.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>())).ReturnsAsync(CaseType);
        }

        protected CaseRole CreateCaseAndHearingRoles(int caseId, string caseRoleName,string userRole, List<string> roles)
        {
            var hearingRoles = new List<HearingRole>();

            foreach (var role in roles)
            {
                hearingRoles.Add(new HearingRole(1, role) { UserRole = new UserRole(1, userRole) });
            }

            var caseRole = new CaseRole(caseId, caseRoleName) { HearingRoles = hearingRoles };

            return caseRole;
        }
    }
}
