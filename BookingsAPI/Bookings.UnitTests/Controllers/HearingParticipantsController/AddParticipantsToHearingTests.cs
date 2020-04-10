﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
using Bookings.DAL.Commands;
using Bookings.DAL.Queries;
using Bookings.Domain;
using Bookings.Domain.RefData;
using Bookings.Infrastructure.Services.IntegrationEvents.Events;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Testing.Common.Assertions;

namespace Bookings.UnitTests.Controllers.HearingParticipantsController
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
        private List<ParticipantRequest> BuildParticipants(int listSize)
        {
            var participants = Builder<ParticipantRequest>.CreateListOfSize(listSize).All()
               .With(x => x.ContactEmail = $"Automation_{Faker.Internet.Email()}")
               .With(x => x.Username = $"Automation_{Faker.Internet.Email()}")
               .Build().ToList();
            participants.ForEach(x =>
            {
                x.CaseRoleName = "Civil Money Claims";
                x.HearingRoleName = "Claimant LIP";
                x.FirstName = "Automation_Added Participant";
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
            QueryHandler.Setup(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>())).ReturnsAsync(hearing);

            var response = await Controller.AddParticipantsToHearing(hearingId, request);

            response.Should().NotBeNull();
            QueryHandler.Verify(q => q.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()), Times.Exactly(2));
            QueryHandler.Verify(q => q.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>()), Times.Once);
            CommandHandler.Verify(c => c.Handle(It.IsAny<AddParticipantsToVideoHearingCommand>()), Times.Once);
            EventPublisher.Verify(e => e.PublishAsync(It.Is<ParticipantsAddedIntegrationEvent>(p => p.HearingId == hearing.Id && p.Participants[0].Username == request.Participants[0].Username)), Times.Once);
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
        public async Task Should_return_badrequest_for_given_invalid_participant_address()
        {
            var caseType = new CaseType(1, "Civil") { CaseRoles = new List<CaseRole> { CreateCaseAndHearingRoles(1, "Individual", "Individual", new List<string> { "Claimant LIP" }) } };
            QueryHandler.Setup(q => q.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>())).ReturnsAsync(caseType);

            request.Participants[0].Postcode = string.Empty;

            var result = await Controller.AddParticipantsToHearing(hearingId, request);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage("Postcode", "Postcode is required");
        }

        [Test]
        public async Task Should_return_badrequest_for_given_invalid_representative_info()
        {
            request.Participants[0].Reference = string.Empty;

            var result = await Controller.AddParticipantsToHearing(hearingId, request);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage("Reference", "Reference is required");
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
        public async Task Should_add_given_participants_to_hearing_and_PublishParticipantsAddedEvent_if_several_matching_participant_with_username()
        {
            var hearing = GetVideoHearing(true);
            var participants = BuildParticipants(3);
            participants[0].Username = hearing.Participants[0].Person.Username;
            participants[1].Username = hearing.Participants[1].Person.Username;

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
