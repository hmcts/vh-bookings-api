﻿using BookingsApi.Contract.Enums;
using BookingsApi.Contract.Requests;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain;
using BookingsApi.Domain.RefData;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Testing.Common.Assertions;
using Testing.Common.Builders.Domain;

namespace BookingsApi.UnitTests.Controllers.HearingParticipantsController
{
    public class UpdateHearingParticipantsTests : HearingParticipantsControllerTest
    {
        private UpdateHearingParticipantsRequest _request;

        private List<UpdateParticipantRequest> _existingParticipants { get; set; }
        private List<ParticipantRequest> _newParticipants { get; set; }
        private List<Guid> _removedParticipantIds { get; set; }
        private List<LinkedParticipantRequest> _linkedParticipants { get; set; }

        [SetUp]
        public void SetUp()
        {
            var existingParticipant = GetVideoHearing().Participants[0];

            _existingParticipants = new List<UpdateParticipantRequest>
            {
                new UpdateParticipantRequest
                {
                    DisplayName = "DisplayName",
                    OrganisationName = "OrganisationName",
                    ParticipantId = existingParticipant.Id,
                    Representee = "Representee",
                    TelephoneNumber = "07123456789",
                    Title = "Title"
                }
            };

            _newParticipants = new List<ParticipantRequest>
            {
                new ParticipantRequest
                {
                    CaseRoleName = "Generic",
                    ContactEmail = "contactme@dontcontactme.com",
                    DisplayName = "DisplayName",
                    FirstName = "FirstName",
                    HearingRoleName = "Litigant in person",
                    LastName = "LastName",
                    MiddleNames = "MiddleNames",
                    OrganisationName = "OrganisationName",
                    Representee = "Representee",
                    TelephoneNumber = "07123456789",
                    Title = "Title",
                    Username = "contactme@dontcontactme.com",
                }
            };

            _removedParticipantIds = new List<Guid> { Guid.NewGuid() };

            _linkedParticipants = new List<LinkedParticipantRequest>
            {
                new LinkedParticipantRequest
                {
                    LinkedParticipantContactEmail = "participant@notLinked.com",
                    ParticipantContactEmail = "participant@linked.com",
                    Type = LinkedParticipantType.Interpreter
                }
            };
        }

        [Test]
        public async Task Should_return_bad_request_when_hearing_id_is_empty()
        {
            //Arrange
            _request = BuildRequest();

            //Act
            var response = await Controller.UpdateHearingParticipants(Guid.Empty, _request) as BadRequestObjectResult;

            //Assert
            response.Should().BeOfType<BadRequestObjectResult>();
            ((SerializableError)response.Value).ContainsKeyAndErrorMessage(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
        }

        [Test]
        public async Task Should_return_bad_request_when_request_validation_fails()
        {
            //Arrange

            //Act
            var response = await Controller.UpdateHearingParticipants(hearingId, new UpdateHearingParticipantsRequest()) as BadRequestObjectResult;

            //Assert
            response.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task Should_return_bad_request_for_given_invalid_representative_info()
        {
            //Arrange
            _request = BuildRequest();
            _request.NewParticipants[0].HearingRoleName = "Litigant in person";
            _request.NewParticipants[0].Representee = string.Empty;

            //Act
            var response = await Controller.UpdateHearingParticipants(hearingId, _request) as BadRequestObjectResult;

            response.Should().NotBeNull();
            response.Should().BeOfType<BadRequestObjectResult>();
            ((SerializableError)response.Value).ContainsKeyAndErrorMessage("Representee", "Representee is required");
        }

        [Test]
        public async Task Should_return_not_found_when_hearing_cannot_be_found()
        {
            //Arrange
            QueryHandler
             .Setup(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
             .ReturnsAsync((VideoHearing)null);
            _request = BuildRequest();

            //Act
            var response = await Controller.UpdateHearingParticipants(hearingId, _request) as NotFoundResult;

            //Assert
            response.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task Should_call_update_hearing_participants_command()
        {
            //Arrange
            var hearing = GetVideoHearing();
            hearing.Participants[0].Person.ContactEmail = "contactme@dontcontactme.com";
            hearing.Participants[1].Person.ContactEmail = "participant@notLinked.com";
            hearing.Participants[2].Person.ContactEmail = "participant@linked.com";
            hearing.UpdateStatus(BookingsApi.Domain.Enumerations.BookingStatus.Created, "test", "");
            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(hearing);

            _existingParticipants = new List<UpdateParticipantRequest>
            {
                new UpdateParticipantRequest
                {
                    DisplayName = "DisplayName",
                    OrganisationName = "OrganisationName",
                    ParticipantId = hearing.Participants[0].Id,
                    Representee = "Representee",
                    TelephoneNumber = "07123456789",
                    Title = "Title"
                }
            };
            _request = BuildRequest();

            //Act
            var response = await Controller.UpdateHearingParticipants(hearingId, _request) as OkObjectResult;

            //Assert
            CommandHandler.Verify(ch => ch.Handle(It.Is<UpdateHearingParticipantsCommand>(x =>
                x.HearingId == hearingId
                
                && x.ExistingParticipants[0].DisplayName == _request.ExistingParticipants[0].DisplayName
                && x.ExistingParticipants[0].OrganisationName == _request.ExistingParticipants[0].OrganisationName
                && x.ExistingParticipants[0].ParticipantId == _request.ExistingParticipants[0].ParticipantId
                && x.ExistingParticipants[0].RepresentativeInformation.Representee == _request.ExistingParticipants[0].Representee
                && x.ExistingParticipants[0].TelephoneNumber == _request.ExistingParticipants[0].TelephoneNumber
                && x.ExistingParticipants[0].Title == _request.ExistingParticipants[0].Title

                && x.NewParticipants[0].CaseRole.Name == _request.NewParticipants[0].CaseRoleName
                && x.NewParticipants[0].DisplayName == _request.NewParticipants[0].DisplayName
                && x.NewParticipants[0].HearingRole.Name == _request.NewParticipants[0].HearingRoleName
                && x.NewParticipants[0].Representee == _request.NewParticipants[0].Representee
                && x.NewParticipants[0].Person.ContactEmail == _request.NewParticipants[0].ContactEmail
                && x.NewParticipants[0].Person.FirstName == _request.NewParticipants[0].FirstName
                && x.NewParticipants[0].Person.LastName == _request.NewParticipants[0].LastName
                && x.NewParticipants[0].Person.MiddleNames == _request.NewParticipants[0].MiddleNames
                && x.NewParticipants[0].Person.TelephoneNumber == _request.NewParticipants[0].TelephoneNumber
                && x.NewParticipants[0].Person.Title == _request.NewParticipants[0].Title

                && x.RemovedParticipantIds[0] == _request.RemovedParticipantIds[0]

                && x.LinkedParticipants[0].LinkedParticipantContactEmail == _request.LinkedParticipants[0].LinkedParticipantContactEmail
                && x.LinkedParticipants[0].ParticipantContactEmail == _request.LinkedParticipants[0].ParticipantContactEmail
            )), Times.Once);
            response.Should().BeOfType<OkObjectResult>();

            EventPublisher.Verify(x => x.PublishAsync(It.IsAny<HearingParticipantsUpdatedIntegrationEvent>()), Times.Once);
        }

        [Test]
        public async Task Should_publish_CreateAndNotifyUser_and_HearingNotification_integration_events_when_no_judge()
        {
            //Arrange
            var hearing = GetVideoHearing();
            var part1 = hearing.Participants.FirstOrDefault(x => x.HearingRole.Name == "Name");
            part1.Person.ContactEmail = "contactme@dontcontactme.com";
            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(hearing);

            _existingParticipants = new List<UpdateParticipantRequest>
            {
                new UpdateParticipantRequest
                {
                    DisplayName = "DisplayName",
                    OrganisationName = "OrganisationName",
                    ParticipantId = hearing.Participants[0].Id,
                    Representee = "Representee",
                    TelephoneNumber = "07123456789",
                    Title = "Title"
                }
            };
            _request = BuildRequest();

            //Act
            await Controller.UpdateHearingParticipants(hearingId, _request);

            //Assert
            EventPublisher.Verify(x => x.PublishAsync(It.IsAny<CreateAndNotifyUserIntegrationEvent>()), Times.Once);
            EventPublisher.Verify(x => x.PublishAsync(It.IsAny<HearingNotificationIntegrationEvent>()), Times.Once);
        }

        [Test]
        public async Task Should_publish_HearingIsReadyForVideo_integration_events_when_judge_added()
        {
            //Arrange
            var hearing = GetVideoHearing();
            var part1 = hearing.Participants.FirstOrDefault(x => x.HearingRole.Name == "Name");
            part1.HearingRole = new HearingRole(102, "Judge") { UserRole = new UserRole(1, "Judge") };
            part1.Person.ContactEmail = "contactme@dontcontactme.com";
            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(hearing);

            _existingParticipants = new List<UpdateParticipantRequest>
            {
                new UpdateParticipantRequest
                {
                    DisplayName = "DisplayName",
                    OrganisationName = "OrganisationName",
                    ParticipantId = hearing.Participants[0].Id,
                    Representee = "Representee",
                    TelephoneNumber = "07123456789",
                    Title = "Title"
                }
            };
            _request = BuildRequest();

            //Act
            await Controller.UpdateHearingParticipants(hearingId, _request);

            //Assert
            EventPublisher.Verify(x => x.PublishAsync(It.IsAny<HearingIsReadyForVideoIntegrationEvent>()), Times.Once);
        }

        [Test]
        public async Task Should_publish_HearingParticipantsUpdated_integration_events_when_hearing_status_is_created()
        {
            //Arrange
            var hearing = GetVideoHearing();
            hearing.Participants[0].Person.ContactEmail = "contactme@dontcontactme.com";
            hearing.Participants[1].Person.ContactEmail = "participant@notLinked.com";
            hearing.Participants[2].Person.ContactEmail = "participant@linked.com";

            hearing.UpdateStatus(BookingsApi.Domain.Enumerations.BookingStatus.Created, "test", "");
            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(hearing);

            _existingParticipants = new List<UpdateParticipantRequest>
            {
                new UpdateParticipantRequest
                {
                    DisplayName = "DisplayName",
                    OrganisationName = "OrganisationName",
                    ParticipantId = hearing.Participants[0].Id,
                    Representee = "Representee",
                    TelephoneNumber = "07123456789",
                    Title = "Title"
                }
            };
            _request = BuildRequest();

            //Act
            await Controller.UpdateHearingParticipants(hearingId, _request);

            //Assert
            EventPublisher.Verify(x => x.PublishAsync(It.IsAny<HearingParticipantsUpdatedIntegrationEvent>()), Times.Once);
        }


        [Test]
        public async Task Should_add_given_participants_to_hearing_with_created_status_and_publishevent_if_several_matching_participant_with_contactemail()
        {
            var hearing = GetVideoHearing(true);
            hearing.Participants[0].HearingRole = new HearingRole(1, "Name") { UserRole = new UserRole(1, "User"), };
            
            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(hearing);
            _request = BuildRequest(withLinkedParticipants: false);
            _request.NewParticipants[0].ContactEmail = hearing.Participants[0].Person.ContactEmail;

            var response = await Controller.UpdateHearingParticipants(hearingId, _request);

            response.Should().NotBeNull();
            EventPublisher.Verify(e => e.PublishAsync(It.IsAny<HearingParticipantsUpdatedIntegrationEvent>()), Times.Once);
            CommandHandler.Verify(ch => ch.Handle(It.IsAny<UpdateHearingStatusCommand>()), Times.Never);
        }

        [Test]
        public async Task Should_add_given_participants_to_hearing_with_judge_and_publishevent_if_several_matching_participant_with_contactemail()
        {
            var hearing = GetVideoHearing(false);
            hearing.Participants[0].HearingRole = new HearingRole(1, "Generic") { UserRole = new UserRole(1, "Judge"), };
            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(hearing);
            _request = BuildRequest(withLinkedParticipants: false);
            _request.NewParticipants[0].ContactEmail = hearing.Participants[0].Person.ContactEmail;

            var response = await Controller.UpdateHearingParticipants(hearingId, _request);

            response.Should().NotBeNull();
            EventPublisher.Verify(e => e.PublishAsync(It.IsAny<HearingIsReadyForVideoIntegrationEvent>()), Times.Once);
            CommandHandler.Verify(ch => ch.Handle(It.IsAny<UpdateHearingStatusCommand>()), Times.Once);
        }

        [Test]
        public async Task Should_add_given_participants_to_hearing_without_judge_and_publishevent_if_several_matching_participant_with_contactemail()
        {
            var hearing = GetVideoHearing(false);
            hearing.Participants[0].HearingRole = new HearingRole(1, "Name") { UserRole = new UserRole(1, "User"), };
            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(hearing);
            _request = BuildRequest(withLinkedParticipants: false);
            _request.NewParticipants[0].ContactEmail = hearing.Participants[0].Person.ContactEmail;

            var response = await Controller.UpdateHearingParticipants(hearingId, _request);

            response.Should().NotBeNull();
            EventPublisher.Verify(e => e.PublishAsync(It.IsAny<CreateAndNotifyUserIntegrationEvent>()), Times.Once);
            CommandHandler.Verify(ch => ch.Handle(It.IsAny<UpdateHearingStatusCommand>()), Times.Never);
        }
        
        [Test]
        public async Task Should_publish_HearingParticipantsUpdated_integration_events_when_participant_is_removed()
        {
            //Arrange
            var hearing = GetVideoHearing();
            hearing.Participants[0].Person.ContactEmail = "contactme@dontcontactme.com";
            hearing.Participants[1].Person.ContactEmail = "participantToDelete@notLinked.com";

            hearing.UpdateStatus(BookingsApi.Domain.Enumerations.BookingStatus.Created, "test", "");
            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(hearing);

            _existingParticipants = new List<UpdateParticipantRequest>
            {
                new UpdateParticipantRequest
                {
                    DisplayName = "DisplayName",
                    OrganisationName = "OrganisationName",
                    ParticipantId = hearing.Participants[0].Id,
                    Representee = "Representee",
                    TelephoneNumber = "07123456789",
                    Title = "Title"
                }
            };
            _request = BuildRequest();
            _request.LinkedParticipants = new List<LinkedParticipantRequest>();
            _request.NewParticipants = new List<ParticipantRequest>();
            _request.RemovedParticipantIds = new List<Guid> { hearing.Participants[1].Id };

            //Act
            await Controller.UpdateHearingParticipants(hearingId, _request);

            //Assert
            EventPublisher.Verify(x => x.PublishAsync(It.IsAny<HearingParticipantsUpdatedIntegrationEvent>()), Times.Once);
        }  
        [Test]
        public async Task Should_publish_ParticipantUpdatedIntegrationEvent_on_each_participant_when_no_new_participants_added()
        {
            //Arrange
            var hearing = GetVideoHearing();
            hearing.UpdateStatus(BookingsApi.Domain.Enumerations.BookingStatus.Created, "test", "");
            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(hearing);

            _existingParticipants = new List<UpdateParticipantRequest>
            {
                new UpdateParticipantRequest
                {
                    DisplayName = "DisplayName",
                    OrganisationName = "OrganisationName",
                    ParticipantId = hearing.Participants[0].Id,
                    Representee = "Representee",
                    TelephoneNumber = "07123456789",
                    Title = "Title"
                }
            };
            _request = BuildRequest();
            _request.LinkedParticipants = new List<LinkedParticipantRequest>();
            _request.NewParticipants = new List<ParticipantRequest>();
            _request.RemovedParticipantIds = new List<Guid>();

            //Act
            await Controller.UpdateHearingParticipants(hearingId, _request);

            //Assert
            EventPublisher.Verify(x => x.PublishAsync(It.IsAny<ParticipantUpdatedIntegrationEvent>()), Times.Once);
        }

        private UpdateHearingParticipantsRequest BuildRequest(bool withLinkedParticipants = true)
        {
            return new UpdateHearingParticipantsRequest
            {
                ExistingParticipants = _existingParticipants,
                NewParticipants = _newParticipants,
                RemovedParticipantIds = _removedParticipantIds,
                LinkedParticipants = withLinkedParticipants ? _linkedParticipants : new List<LinkedParticipantRequest>()
            };
        }
    }
}
