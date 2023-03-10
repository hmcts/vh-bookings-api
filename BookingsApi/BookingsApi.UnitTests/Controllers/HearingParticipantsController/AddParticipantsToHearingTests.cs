using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.Domain;
using BookingsApi.Domain.RefData;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Testing.Common.Assertions;

namespace BookingsApi.UnitTests.Controllers.HearingParticipantsController
{
    public class AddParticipantsToHearingTests : HearingParticipantsControllerTest
    {
        private AddParticipantsToHearingRequest request;

        [SetUp]
        public void TestInitialize()
        {
            var participants = BuildParticipants(1);
            request = new AddParticipantsToHearingRequest { Participants = participants };

        }
        private static List<ParticipantRequest> BuildParticipants(int listSize)
        {
            var participants = Builder<ParticipantRequest>.CreateListOfSize(listSize).All()
                .With(x => x.ContactEmail = $"Automation_{Faker.RandomNumber.Next()}@hmcts.net")
                .With(x => x.Username = $"Automation_{Faker.RandomNumber.Next()}@hmcts.net")
                .With(x=> x.TelephoneNumber, "01234567890")
                .Build().ToList();
            participants.ForEach(x =>
            {
                x.CaseRoleName = "Generic";
                x.HearingRoleName = "Litigant in person";
                x.FirstName = "Automation_AddedParticipant";
                x.DisplayName = "DisplayName";
            });

            return participants;
        }

        [Test]
        public async Task Should_add_given_participants_to_hearing()
        {
            var response = await Controller.AddParticipantsToHearing(hearingId, request);

            response.Should().NotBeNull();
            QueryHandler.Verify(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()), Times.Exactly(2));
            QueryHandler.Verify(q => q.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>()), Times.Once);
            CommandHandler.Verify(c => c.Handle(It.IsAny<AddParticipantsToVideoHearingCommand>()), Times.Once);
            EventPublisher.Verify(e => e.PublishAsync(It.IsAny<ParticipantsAddedIntegrationEvent>()), Times.Never);
        }

        [Test]
        public async Task Should_add_given_participants_to_hearing_and_PublishParticipantsAddedEvent()
        {
            var hearing = GetVideoHearing(true);
            request.Participants[0].Username = hearing.Participants[0].Person.Username;
            request.Participants[0].FirstName = hearing.Participants[0].Person.FirstName;
            request.Participants[0].LastName = hearing.Participants[0].Person.LastName;
            request.Participants[0].ContactEmail = hearing.Participants[0].Person.ContactEmail;
            request.Participants[0].TelephoneNumber = hearing.Participants[0].Person.TelephoneNumber;
            request.Participants[0].DisplayName = hearing.Participants[0].Person.FirstName;
            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(hearing);

            var response = await Controller.AddParticipantsToHearing(hearingId, request);

            response.Should().NotBeNull();
            QueryHandler.Verify(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()), Times.Exactly(2));
            QueryHandler.Verify(q => q.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>()), Times.Once);
            CommandHandler.Verify(c => c.Handle(It.IsAny<AddParticipantsToVideoHearingCommand>()), Times.Once);
            EventPublisher.Verify(e => e.PublishAsync(It.Is<ParticipantsAddedIntegrationEvent>
            (
                p => p.Hearing.HearingId == hearing.Id 
                                               && p.Participants[0].FirstName == request.Participants[0].FirstName
                                               && p.Participants[0].LastName == request.Participants[0].LastName
                                               && p.Participants[0].ContactEmail == request.Participants[0].ContactEmail
                                               && p.Participants[0].ContactTelephone == request.Participants[0].TelephoneNumber
            )), Times.Once);
        }

        [Test]
        public async Task Should_add_given_participants_to_hearing_without_judge_and_CreateAndNotifyUser_and_HearingNotification_IntegrationEvent()
        {
            var hearing = GetVideoHearing();
            request.Participants[0].Username = hearing.Participants[0].Person.Username;
            request.Participants[0].FirstName = hearing.Participants[0].Person.FirstName;
            request.Participants[0].LastName = hearing.Participants[0].Person.LastName;
            request.Participants[0].ContactEmail = hearing.Participants[0].Person.ContactEmail;
            request.Participants[0].TelephoneNumber = hearing.Participants[0].Person.TelephoneNumber;
            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(hearing);

            var response = await Controller.AddParticipantsToHearing(hearingId, request);

            response.Should().NotBeNull();
            QueryHandler.Verify(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()), Times.Exactly(2));
            QueryHandler.Verify(q => q.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>()), Times.Once);
            CommandHandler.Verify(c => c.Handle(It.IsAny<AddParticipantsToVideoHearingCommand>()), Times.Once);
            CommandHandler.Verify(c => c.Handle(It.IsAny<UpdateHearingStatusCommand>()), Times.Never);
            EventPublisher.Verify(e => e.PublishAsync(It.IsAny<CreateAndNotifyUserIntegrationEvent>()), Times.Once);
            EventPublisher.Verify(e => e.PublishAsync(It.IsAny<HearingNotificationIntegrationEvent>()), Times.Once);

        }

        [Test]
        public async Task Should_add_given_participants_to_hearing_with_judge_but_not_created_and_CreateAndNotifyUserIntegrationEvent()
        {
            var hearing = GetVideoHearing();
            request.Participants[0].Username = hearing.Participants[0].Person.Username;
            request.Participants[0].FirstName = hearing.Participants[0].Person.FirstName;
            request.Participants[0].LastName = hearing.Participants[0].Person.LastName;
            request.Participants[0].ContactEmail = hearing.Participants[0].Person.ContactEmail;
            request.Participants[0].TelephoneNumber = hearing.Participants[0].Person.TelephoneNumber;
            hearing.Participants.First(e => e.HearingRole.Name == "Judge").Person.ContactEmail = "judge@me.com";
            request.Participants.Add(new ParticipantRequest
            {
                CaseRoleName = "Test",
                ContactEmail = "judge@me.com",
                HearingRoleName = "Judge",
                FirstName = "Judge",
                LastName = "One",
                DisplayName = "One",
                Username = "judge@me.com",
                TelephoneNumber = "(+44)123 456"
            });
            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(hearing);

            var response = await Controller.AddParticipantsToHearing(hearingId, request);

            response.Should().NotBeNull();
            QueryHandler.Verify(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()), Times.Exactly(2));
            QueryHandler.Verify(q => q.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>()), Times.Once);
            CommandHandler.Verify(c => c.Handle(It.IsAny<AddParticipantsToVideoHearingCommand>()), Times.Once);
            EventPublisher.Verify(e => e.PublishAsync(It.IsAny<HearingIsReadyForVideoIntegrationEvent>()), Times.Once);
        }

        [Test]
        public async Task Should_return_badrequest_for_given_invalid_hearingid()
        {
            hearingId = Guid.Empty;

            var result = await Controller.AddParticipantsToHearing(hearingId, request);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
        }

        [Test]
        public async Task Should_return_badrequest_for_given_invalid_request()
        {
            var result = await Controller.AddParticipantsToHearing(hearingId, new AddParticipantsToHearingRequest());

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage("Participants", "Please provide at least one participant");
        }

        [Test]
        public async Task Should_return_notfound_with_no_matching_videohearing()
        {
            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync((VideoHearing)null);

            var result = await Controller.AddParticipantsToHearing(hearingId, request);

            result.Should().NotBeNull();
            var objectResult = (NotFoundResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            QueryHandler.Verify(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()), Times.Once);
        }

        [Test]
        public async Task Should_return_badrequest_for_given_invalid_representative_info()
        {

            request.Participants[0].Representee = string.Empty;
            var result = await Controller.AddParticipantsToHearing(hearingId, request);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage("Representee", "Representee is required");
        }

        [Test]
        public async Task Should_add_given_participants_to_hearing_and_not_PublishParticipantsAddedEvent_if_no_matching_participant_with_username()
        {
            var hearing = GetVideoHearing(true);
            var participants = BuildParticipants(3);
            request = new AddParticipantsToHearingRequest { Participants = participants };
            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(hearing);

            var response = await Controller.AddParticipantsToHearing(hearingId, request);

            response.Should().NotBeNull();
            QueryHandler.Verify(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()), Times.Exactly(2));
            QueryHandler.Verify(q => q.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>()), Times.Once);
            CommandHandler.Verify(c => c.Handle(It.IsAny<AddParticipantsToVideoHearingCommand>()), Times.Once);
            EventPublisher.Verify(e => e.PublishAsync(It.IsAny<ParticipantsAddedIntegrationEvent>()), Times.Never);
        }

        [Test]
        public async Task Should_add_given_participants_to_hearing_and_PublishParticipantsAddedEvent_if_several_matching_participant_with_contactemail()
        {
            var hearing = GetVideoHearing(true);
            var participants = BuildParticipants(3);
            participants[0].ContactEmail = hearing.Participants[0].Person.ContactEmail;
            participants[1].ContactEmail = hearing.Participants[1].Person.ContactEmail;

            request = new AddParticipantsToHearingRequest { Participants = participants };
            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(hearing);

            var response = await Controller.AddParticipantsToHearing(hearingId, request);

            response.Should().NotBeNull();
            QueryHandler.Verify(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()), Times.Exactly(2));
            QueryHandler.Verify(q => q.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>()), Times.Once);
            CommandHandler.Verify(c => c.Handle(It.IsAny<AddParticipantsToVideoHearingCommand>()), Times.Once);
            EventPublisher.Verify(e => e.PublishAsync(It.IsAny<ParticipantsAddedIntegrationEvent>()), Times.Once);
        }

    }
}
