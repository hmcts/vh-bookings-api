using BookingsApi.Common;
using BookingsApi.Contract.V1.Enums;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.Publishers;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using BookingsApi.Validations.Common;
using BookingsApi.Validations.V1;
using FizzWare.NBuilder;

namespace BookingsApi.IntegrationTests.Api.V1.Hearings
{
    public class UpdateHearingsInGroupTests : ApiTest
    {
        [Test]
        public async Task should_update_hearings_in_group()
        {
            // Arrange
            var hearings = await SeedHearingsInGroup();

            var request = BuildRequest();
            request.Hearings = hearings.Select(MapHearingRequest).ToList();
            foreach (var requestHearing in request.Hearings)
            {
                requestHearing.CaseNumber = "UpdatedCaseNumber";
                requestHearing.ScheduledDateTime = requestHearing.ScheduledDateTime.AddDays(1);
                requestHearing.ScheduledDuration = 90;
                requestHearing.HearingVenueName = "Manchester County and Family Court";
                requestHearing.HearingRoomName = "UpdatedRoomName";
                requestHearing.OtherInformation = "UpdatedOtherInformation";
                requestHearing.AudioRecordingRequired = true;
            }
        
            var newParticipant = new Builder(new BuilderSettings()).CreateNew<ParticipantRequest>()
                .With(p => p.ContactEmail, Faker.Internet.Email())
                .With(p => p.CaseRoleName, "Applicant")
                .With(p => p.HearingRoleName, "Litigant in person")
                .Build();
            var newEndpoint = new Builder(new BuilderSettings()).CreateNew<AddEndpointRequest>()
                .With(e => e.DefenceAdvocateContactEmail, null)
                .With(e => e.InterpreterLanguageCode, null)
                .With(e => e.OtherLanguage, null)
                .Build();

            foreach (var requestHearing in request.Hearings)
            {
                var hearing = hearings.First(h => h.Id == requestHearing.HearingId);
                
                var defenceAdvocateEmail = requestHearing.Endpoints.ExistingEndpoints
                    .First(e => e.DefenceAdvocateContactEmail != null).DefenceAdvocateContactEmail;
                var defenceAdvocateParticipant = hearing.Participants.First(p => p.Person.ContactEmail == defenceAdvocateEmail);
                
                // Add a participant
                requestHearing.Participants.NewParticipants.Add(newParticipant);
                
                // Remove a participant
                var participantToRemove = requestHearing.Participants.ExistingParticipants.First(p => p.ParticipantId != defenceAdvocateParticipant.Id);
                requestHearing.Participants.RemovedParticipantIds.Add(participantToRemove.ParticipantId);
                requestHearing.Participants.ExistingParticipants.Remove(participantToRemove);
                
                // Update a participant
                requestHearing.Participants.ExistingParticipants[0].DisplayName = "UpdatedDisplayName";
                
                // Add an endpoint
                requestHearing.Endpoints.NewEndpoints.Add(newEndpoint);
                
                // Remove an endpoint
                var endpointToRemove = requestHearing.Endpoints.ExistingEndpoints.First(e => e.DefenceAdvocateContactEmail != defenceAdvocateEmail);
                requestHearing.Endpoints.RemovedEndpointIds.Add(endpointToRemove.Id);
                requestHearing.Endpoints.ExistingEndpoints.Remove(endpointToRemove);
            }
            
            var groupId = hearings[0].SourceId.Value;
        
            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpoints.UpdateHearingsInGroupId(groupId),RequestBody.Set(request));
        
            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.NoContent, result.Content.ReadAsStringAsync().Result);
            
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var updatedHearings = await new GetHearingsByGroupIdQueryHandler(db).Handle(new GetHearingsByGroupIdQuery(groupId));
            foreach (var updatedHearing in updatedHearings)
            {
                var requestHearing = request.Hearings.First(x => x.HearingId == updatedHearing.Id);
                var originalHearing = hearings.First(x => x.Id == updatedHearing.Id);
                
                updatedHearing.UpdatedDate.Should().BeAfter(originalHearing.UpdatedDate);
                updatedHearing.UpdatedBy.Should().Be(request.UpdatedBy);
                var updatedCase = updatedHearing.GetCases().FirstOrDefault();
                updatedCase.Number.Should().Be(requestHearing.CaseNumber);
                var originalCase = originalHearing.GetCases().FirstOrDefault();
                updatedCase.Name.Should().Be(originalCase.Name);
                updatedHearing.ScheduledDateTime.Should().Be(requestHearing.ScheduledDateTime);
                updatedHearing.ScheduledDuration.Should().Be(requestHearing.ScheduledDuration);
                updatedHearing.HearingVenue.Name.Should().Be(requestHearing.HearingVenueName);
                updatedHearing.HearingRoomName.Should().Be(requestHearing.HearingRoomName);
                updatedHearing.OtherInformation.Should().Be(requestHearing.OtherInformation);
                updatedHearing.AudioRecordingRequired.Should().Be(requestHearing.AudioRecordingRequired);

                AssertParticipantsUpdated(updatedHearing, requestHearing);
                AssertEndpointsUpdated(updatedHearing, requestHearing);
                AssertEventsPublished(updatedHearing, requestHearing, existingParticipantsModified: 1);
            }

            var updateDateHearing = updatedHearings[0].UpdatedDate.TrimSeconds();
            var firstHearing = updatedHearings[0];
            var expectedExistingUser =
                PublisherHelper.GetExistingParticipantsSinceLastUpdate(firstHearing, updateDateHearing).ToList().Count;
            var expectedNewUser =
                PublisherHelper.GetNewParticipantsSinceLastUpdate(firstHearing, updateDateHearing).ToList().Count;
            var expectedWelcomeNewUser =
                PublisherHelper.GetNewParticipantsSinceLastUpdate(firstHearing, updateDateHearing).Where(x => x is not JudicialOfficeHolder)
                    .ToList().Count;
            AssertNotificationEvents(firstHearing, expectedExistingUser, expectedNewUser, expectedWelcomeNewUser);
        }
        
        [Test]
        public async Task should_return_not_found_when_no_hearings_found_for_group()
        {
            // Arrange
            var request = BuildRequest();
            request.Hearings = new List<HearingRequest>
            {
                BuildHearingRequest(DateTime.Today.AddDays(1).AddHours(10))
            };
            
            var groupId = Guid.NewGuid();
            
            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpoints.UpdateHearingsInGroupId(groupId),RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.NotFound, result.Content.ReadAsStringAsync().Result);
        }

        [Test]
        public async Task should_return_bad_request_when_hearings_in_request_do_not_belong_to_group()
        {
            // Arrange
            var hearings = await SeedHearingsInGroup();

            var request = BuildRequest();
            request.Hearings = hearings.Select(MapHearingRequest).ToList();

            var hearingsNotInGroup = new List<HearingRequest>
            {
                BuildHearingRequest(DateTime.Today.AddDays(1).AddHours(10)),
                BuildHearingRequest(DateTime.Today.AddDays(2).AddHours(10))
            };
            
            request.Hearings.AddRange(hearingsNotInGroup);

            var groupId = hearings[0].SourceId.Value;

            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpoints.UpdateHearingsInGroupId(groupId),RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors["Hearings[3]"][0].Should()
                .Be($"Hearing {hearingsNotInGroup[0].HearingId} does not belong to group {groupId}");
            validationProblemDetails.Errors["Hearings[4]"][0].Should()
                .Be($"Hearing {hearingsNotInGroup[1].HearingId} does not belong to group {groupId}");
        }

        [Test]
        public async Task should_return_bad_request_when_duplicate_hearing_ids_in_request()
        {
            // Arrange
            var hearings = await SeedHearingsInGroup();

            var request = BuildRequest();
            request.Hearings = hearings.Select(MapHearingRequest).ToList();

            request.Hearings[1].HearingId = request.Hearings[0].HearingId;

            var groupId = hearings[0].SourceId.Value;

            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpoints.UpdateHearingsInGroupId(groupId),RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors[nameof(request.Hearings)][0].Should()
                .Be(UpdateHearingsInGroupRequestInputValidation.DuplicateHearingIdsMessage);
        }

        [Test]
        public async Task should_return_bad_request_when_empty_hearings_list_in_request()
        {
            // Arrange
            var request = BuildRequest();
            request.Hearings = new List<HearingRequest>();
            
            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpoints.UpdateHearingsInGroupId(Guid.NewGuid()),RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors[nameof(request.Hearings)][0].Should().Be(
                UpdateHearingsInGroupRequestInputValidation.NoHearingsErrorMessage);
        }
        
        [Test]
        public async Task should_return_bad_request_when_null_hearings_list_in_request()
        {
            // Arrange
            var request = BuildRequest();
            request.Hearings = null;
            
            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpoints.UpdateHearingsInGroupId(Guid.NewGuid()),RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors[nameof(request.Hearings)][0].Should().Be(
                UpdateHearingsInGroupRequestInputValidation.NoHearingsErrorMessage);
        }
        
        [Test]
        public async Task should_return_bad_request_when_duplicate_scheduled_date_times_in_request()
        {
            // Arrange
            var hearings = await SeedHearingsInGroup();

            var request = BuildRequest();
            request.Hearings = hearings.Select(MapHearingRequest).ToList();

            request.Hearings[1].ScheduledDateTime = request.Hearings[0].ScheduledDateTime;

            var groupId = hearings[0].SourceId.Value;

            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpoints.UpdateHearingsInGroupId(groupId),RequestBody.Set(request));

            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors[nameof(request.Hearings)][0].Should()
                .Be(UpdateHearingsInGroupRequestInputValidation.DuplicateScheduledDateTimesMessage);
        }

        [Test]
        public async Task should_return_bad_request_when_invalid_participants_in_request()
        {
            // Arrange
            var hearings = await SeedHearingsInGroup();

            var request = BuildRequest();
            request.Hearings = hearings.Select(MapHearingRequest).ToList();

            foreach (var requestHearing in request.Hearings)
            {
                requestHearing.Participants.ExistingParticipants = new List<UpdateParticipantRequest>();
                requestHearing.Participants.NewParticipants = new List<ParticipantRequest>();
                requestHearing.Participants.LinkedParticipants = new List<LinkedParticipantRequest>();
                requestHearing.Participants.RemovedParticipantIds = new List<Guid>();
            }
            
            var groupId = hearings[0].SourceId.Value;

            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpoints.UpdateHearingsInGroupId(groupId),RequestBody.Set(request));

            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors["Hearings[0].Participants"][0].Should().Be(
                UpdateHearingParticipantsRequestValidation.NoParticipantsErrorMessage);
        }
        
        [Test]
        public async Task should_return_bad_request_when_invalid_participant_ref_data_in_request()
        {
            // Arrange
            var hearings = await SeedHearingsInGroup();

            var request = BuildRequest();
            request.Hearings = hearings.Select(MapHearingRequest).ToList();

            var newParticipant = new Builder(new BuilderSettings()).CreateNew<ParticipantRequest>()
                .With(p => p.ContactEmail, Faker.Internet.Email())
                .With(p => p.CaseRoleName, "Applicant")
                .With(p => p.HearingRoleName, "Representative")
                .With(p => p.OrganisationName, "Org1")
                .With(p => p.Representee, "") // Invalid representee
                .Build();
            
            foreach (var requestHearing in request.Hearings)
            {
                requestHearing.Participants.NewParticipants.Add(newParticipant);
            }
            
            var groupId = hearings[0].SourceId.Value;

            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpoints.UpdateHearingsInGroupId(groupId),RequestBody.Set(request));

            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors["NewParticipants[0].Representee"][0].Should().Be(
                RepresentativeValidation.NoRepresentee);
        }

        [Test]
        public async Task should_return_bad_request_when_invalid_endpoints_in_request()
        {
            // Arrange
            var hearings = await SeedHearingsInGroup();

            var request = BuildRequest();
            request.Hearings = hearings.Select(MapHearingRequest).ToList();

            var newEndpoint = new Builder(new BuilderSettings()).CreateNew<AddEndpointRequest>()
                .With(e => e.DefenceAdvocateContactEmail, null)
                .With(e => e.DisplayName, "") // Invalid display name
                .Build();
            
            foreach (var requestHearing in request.Hearings)
            {
                requestHearing.Endpoints.NewEndpoints.Add(newEndpoint);
            }
            
            var groupId = hearings[0].SourceId.Value;

            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpoints.UpdateHearingsInGroupId(groupId),RequestBody.Set(request));

            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors["Hearings[0].Endpoints.NewEndpoints[0].DisplayName"][0].Should().Be(
                AddEndpointRequestValidation.NoDisplayNameError);
        }

        [Test]
        public async Task should_return_bad_request_when_invalid_details_in_request()
        {
            // Arrange
            var request = new UpdateHearingsInGroupRequest();
            var groupId = Guid.NewGuid();

            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpoints.UpdateHearingsInGroupId(groupId),RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors[nameof(request.UpdatedBy)][0].Should().Be(
                UpdateHearingsInGroupRequestInputValidation.NoUpdatedByErrorMessage);
        }
        
        [Test]
        public async Task should_return_bad_request_when_invalid_hearing_details_in_request()
        {
            // Arrange
            var request = new UpdateHearingsInGroupRequest
            {
                Hearings = new List<HearingRequest>
                {
                    new()
                    {
                        HearingId = Guid.NewGuid(),
                        ScheduledDateTime = DateTime.Today.AddDays(-1).AddHours(10)
                    }
                }
            };
            var groupId = Guid.NewGuid();

            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpoints.UpdateHearingsInGroupId(groupId),RequestBody.Set(request));

            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors["Hearings[0].HearingVenueName"][0].Should().Be(
                UpdateHearingRequestValidation.NoHearingVenueNameErrorMessage);
            validationProblemDetails.Errors["Hearings[0].ScheduledDuration"][0].Should().Be(
                UpdateHearingRequestValidation.NoScheduleDurationErrorMessage);
            validationProblemDetails.Errors["Hearings[0].CaseNumber"][0].Should().Be(
                CaseRequestValidation.CaseNumberMessage);
            validationProblemDetails.Errors["Hearings[0].ScheduledDateTime"][0].Should().Be(
                UpdateHearingRequestValidation.ScheduleDateTimeInPastErrorMessage);
        }
        
        [Test]
        public async Task should_return_bad_request_when_hearing_venue_name_does_not_exist()
        {
            // Arrange
            var hearings = await SeedHearingsInGroup();

            var request = BuildRequest();
            request.Hearings = hearings.Select(MapHearingRequest).ToList();
            request.Hearings[0].HearingVenueName = "NonExistingVenueName";

            var groupId = hearings[0].SourceId.Value;

            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpoints.UpdateHearingsInGroupId(groupId),RequestBody.Set(request));

            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors["Hearings[0]"][0].Should()
                .Be($"Hearing venue name {request.Hearings[0].HearingVenueName} does not exist");
        }

        private async Task<List<VideoHearing>> SeedHearingsInGroup()
        {
            var dates = new List<DateTime>
            {
                DateTime.Today.AddDays(5).AddHours(10).ToUniversalTime(),
                DateTime.Today.AddDays(6).AddHours(10).ToUniversalTime(),
                DateTime.Today.AddDays(7).AddHours(10).ToUniversalTime()
            };

            var multiDayHearings = await Hooks.SeedMultiDayHearing(useV2: false, dates, addPanelMember: true);

            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            
            // Add a second panel member for test coverage
            var hearings = new List<VideoHearing>();
            foreach (var hearing in multiDayHearings)
            {
                await Hooks.AddPanelMember(hearing, hearing.CaseType);
                var hearingFromDb = await new GetHearingByIdQueryHandler(db).Handle(new GetHearingByIdQuery(hearing.Id));
                hearings.Add(hearingFromDb);
            }

            return hearings;
        }

        private static UpdateHearingsInGroupRequest BuildRequest() =>
            new()
            {
                UpdatedBy = "updatedBy@email.com"
            };
        
        private static HearingRequest BuildHearingRequest(DateTime scheduledDateTime) =>
            new()
            {
                HearingId = Guid.NewGuid(),
                HearingVenueName = "VenueName",
                ScheduledDateTime = scheduledDateTime,
                ScheduledDuration = 45,
                CaseNumber = "CaseNumber"
            };
        
        private static HearingRequest MapHearingRequest(Hearing hearing) =>
            new()
            {
                HearingId = hearing.Id,
                CaseNumber = hearing.GetCases().FirstOrDefault().Number,
                ScheduledDateTime = hearing.ScheduledDateTime,
                ScheduledDuration = hearing.ScheduledDuration,
                HearingVenueName = hearing.HearingVenue.Name,
                HearingRoomName = hearing.HearingRoomName,
                OtherInformation = hearing.OtherInformation,
                AudioRecordingRequired = hearing.AudioRecordingRequired,
                Participants = new UpdateHearingParticipantsRequest
                {
                    ExistingParticipants = hearing.Participants.Select(p => new UpdateParticipantRequest
                    {
                        ParticipantId = p.Id,
                        Title = p.Person.Title,
                        TelephoneNumber = p.Person.TelephoneNumber,
                        DisplayName = p.DisplayName,
                        OrganisationName = p.Person.Organisation?.Name,
                        Representee = p is Representative ? (p as Representative).Representee : null,
                        FirstName = p.Person.FirstName,
                        MiddleName = p.Person.MiddleNames,
                        LastName = p.Person.LastName,
                        ContactEmail = p.Person.ContactEmail,
                        LinkedParticipants = p.LinkedParticipants.Select(lp => new LinkedParticipantRequest
                        {
                            ParticipantContactEmail = lp.Participant.Person.ContactEmail,
                            LinkedParticipantContactEmail = lp.Linked.Person.ContactEmail,
                            Type = LinkedParticipantType.Interpreter
                        }).ToList()
                    }).ToList()
                },
                Endpoints = new UpdateHearingEndpointsRequest
                {
                    ExistingEndpoints = hearing.Endpoints.Select(e => new EditableEndpointRequest
                    {
                        Id = e.Id,
                        DisplayName = e.DisplayName,
                        DefenceAdvocateContactEmail = e.DefenceAdvocate?.Person.ContactEmail
                    }).ToList()
                }
            };
        
        private static void AssertParticipantsUpdated(Hearing hearing, HearingRequest requestHearing)
        {
            var expectedParticipantCount = requestHearing.Participants.NewParticipants.Count + 
                                           requestHearing.Participants.ExistingParticipants.Count;
            var participants = hearing.GetParticipants();
            participants.Count.Should().Be(expectedParticipantCount);
            foreach (var newParticipant in requestHearing.Participants.NewParticipants)
            {
                participants.Should().Contain(p => p.Person.ContactEmail == newParticipant.ContactEmail);
            }
            participants.Should().NotContain(p => requestHearing.Participants.RemovedParticipantIds.Contains(p.Id));
        }

        private static void AssertEndpointsUpdated(Hearing hearing, HearingRequest requestHearing)
        {
            var expectedEndpointCount = requestHearing.Endpoints.NewEndpoints.Count + 
                                        requestHearing.Endpoints.ExistingEndpoints.Count;
            var endpoints = hearing.GetEndpoints();
            endpoints.Count.Should().Be(expectedEndpointCount);
            foreach (var newEndpoint in requestHearing.Endpoints.NewEndpoints)
            {
                endpoints.Should().Contain(e => e.DisplayName == newEndpoint.DisplayName);
            }
            endpoints.Should().NotContain(e => requestHearing.Endpoints.RemovedEndpointIds.Contains(e.Id));
        }

        private void AssertEventsPublished(Hearing hearing, HearingRequest requestHearing, int existingParticipantsModified)
        {
            var serviceBusStub = Application.Services
                .GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
            var messages = serviceBusStub!
                .ReadAllMessagesFromQueue(hearing.Id);

            AssertParticipantEvents();
            AssertEndpointEvents();
            AssertHearingEvents();
            return;

            void AssertParticipantEvents()
            {
                var participantMessages = messages
                    .Where(x => x.IntegrationEvent is HearingParticipantsUpdatedIntegrationEvent)
                    .Select(x => x.IntegrationEvent as HearingParticipantsUpdatedIntegrationEvent)
                    .Where(x => x.Hearing.HearingId == hearing.Id)
                    .ToList();

                participantMessages.Count.Should().Be(1);
                var participantMessage = participantMessages.Single();

                var expectedAddedCount = requestHearing.Participants.NewParticipants.Count;
                var expectedRemovedCount = requestHearing.Participants.RemovedParticipantIds.Count;
                var expectedLinkedCount = requestHearing.Participants.LinkedParticipants.Count;

                participantMessage.NewParticipants.Count.Should().Be(expectedAddedCount);
                participantMessage.ExistingParticipants.Count.Should().Be(existingParticipantsModified);
                participantMessage.RemovedParticipants.Count.Should().Be(expectedRemovedCount);
                participantMessage.LinkedParticipants.Count.Should().Be(expectedLinkedCount);
            }

            void AssertEndpointEvents()
            {
                var expectedAddedCount = requestHearing.Endpoints.NewEndpoints.Count;

                var endpointAddedMessages = messages
                    .Where(x => x.IntegrationEvent is EndpointAddedIntegrationEvent)
                    .Select(x => x.IntegrationEvent as EndpointAddedIntegrationEvent)
                    .Where(x => x.HearingId == hearing.Id)
                    .ToList();

                endpointAddedMessages.Count.Should().Be(expectedAddedCount);

                var expectedUpdatedCount = requestHearing.Endpoints.ExistingEndpoints.Count;

                var endpointUpdatedMessages = messages
                    .Where(x => x.IntegrationEvent is EndpointUpdatedIntegrationEvent)
                    .Select(x => x.IntegrationEvent as EndpointUpdatedIntegrationEvent)
                    .Where(x => x.HearingId == hearing.Id)
                    .ToList();

                endpointUpdatedMessages.Count.Should().Be(expectedUpdatedCount);

                var expectedRemovedCount = requestHearing.Endpoints.RemovedEndpointIds.Count;

                var endpointRemovedMessages = messages
                    .Where(x => x.IntegrationEvent is EndpointRemovedIntegrationEvent)
                    .Select(x => x.IntegrationEvent as EndpointRemovedIntegrationEvent)
                    .Where(x => x.HearingId == hearing.Id)
                    .ToList();

                endpointRemovedMessages.Count.Should().Be(expectedRemovedCount);
            }
            
            void AssertHearingEvents()
            {
                const int expectedDetailsUpdatedCount = 6; // 3 hearings updated endpoint added, endpoint removed, and endpoint updated

                var hearingDetailsUpdatedMessages = messages
                    .Where(x => x.IntegrationEvent is HearingDetailsUpdatedIntegrationEvent)
                    .Select(x => x.IntegrationEvent as HearingDetailsUpdatedIntegrationEvent)
                    .Where(x => x.Hearing.HearingId == hearing.Id)
                    .ToList();

                hearingDetailsUpdatedMessages.Count.Should().Be(expectedDetailsUpdatedCount);

                if (requestHearing.ScheduledDateTime != hearing.ScheduledDateTime)
                {
                    var expectedHearingAmendmentCount = hearing.Participants.Count + hearing.JudiciaryParticipants.Count;

                    var hearingAmendmentMessages = messages
                        .Where(x => x.IntegrationEvent is HearingAmendmentNotificationEvent)
                        .Select(x => x.IntegrationEvent as HearingAmendmentNotificationEvent)
                        .Where(x => x.HearingConfirmationForParticipant.HearingId == hearing.Id)
                        .ToList();

                    hearingAmendmentMessages.Count.Should().Be(expectedHearingAmendmentCount);
                }
            }
        }
        
        private void AssertNotificationEvents(Hearing hearing, int expectedExistingUserMessages, int expectedNewUserMessages, int expectedNewUserWelcomeMessages)
        {
            var serviceBusStub = Application.Services
                .GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
            var messages = serviceBusStub!
                .ReadAllMessagesFromQueue(hearing.Id);
            
            
            var existingUserMessages = messages
                .Where(x => x.IntegrationEvent is ExistingParticipantMultidayHearingConfirmationEvent)
                .Select(x => x.IntegrationEvent as ExistingParticipantMultidayHearingConfirmationEvent)
                .Where(x => x.HearingConfirmationForParticipant.HearingId == hearing.Id)
                .ToList();
            
            var newUserMessages = messages
                .Where(x => x.IntegrationEvent is NewParticipantMultidayHearingConfirmationEvent)
                .Select(x => x.IntegrationEvent as NewParticipantMultidayHearingConfirmationEvent)
                .Where(x => x.HearingConfirmationForParticipant.HearingId == hearing.Id)
                .ToList();
            
            var newUserWelcomeMessages = messages
                .Where(x => x.IntegrationEvent is NewParticipantWelcomeEmailEvent)
                .Select(x => x.IntegrationEvent as NewParticipantWelcomeEmailEvent)
                .Where(x => x.WelcomeEmail.HearingId == hearing.Id)
                .ToList();
            
            existingUserMessages.Count.Should().Be(expectedExistingUserMessages);
            newUserMessages.Count.Should().Be(expectedNewUserMessages);
            newUserWelcomeMessages.Count.Should().Be(expectedNewUserWelcomeMessages);
            
        }
    }
}
