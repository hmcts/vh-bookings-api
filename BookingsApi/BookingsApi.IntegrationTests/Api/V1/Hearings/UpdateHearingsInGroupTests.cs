using BookingsApi.Contract.V1.Enums;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
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
            
            var request = new UpdateHearingsInGroupRequest
            {
                Hearings = hearings.Select(MapHearingRequest).ToList()
            };
        
            var newParticipant = new Builder(new BuilderSettings()).CreateNew<ParticipantRequest>()
                .With(p => p.ContactEmail, Faker.Internet.Email())
                .With(p => p.CaseRoleName, "Applicant")
                .With(p => p.HearingRoleName, "Litigant in person")
                .Build();
            var newEndpoint = new Builder(new BuilderSettings()).CreateNew<AddEndpointRequest>()
                .With(e => e.DefenceAdvocateContactEmail, null)
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
        
                AssertParticipantsUpdated(updatedHearing, requestHearing);
                AssertEndpointsUpdated(updatedHearing, requestHearing);
                AssertEventsPublished(updatedHearing, requestHearing);
            }
        }
        
        [Test]
        public async Task should_return_not_found_when_no_hearings_found_for_group()
        {
            // Arrange
            var request = new UpdateHearingsInGroupRequest
            {
                Hearings = new List<HearingRequest>
                {
                    new()
                    {
                        HearingId = Guid.NewGuid()
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
            result.StatusCode.Should().Be(HttpStatusCode.NotFound, result.Content.ReadAsStringAsync().Result);
        }

        [Test]
        public async Task should_return_bad_request_when_hearings_in_request_do_not_belong_to_group()
        {
            // Arrange
            var hearings = await SeedHearingsInGroup();

            var request = new UpdateHearingsInGroupRequest
            {
                Hearings = hearings.Select(MapHearingRequest).ToList()
            };

            var hearingsNotInGroup = new List<HearingRequest>
            {
                new()
                {
                    HearingId = Guid.NewGuid()
                },
                new()
                {
                    HearingId = Guid.NewGuid()
                }
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

            var request = new UpdateHearingsInGroupRequest
            {
                Hearings = hearings.Select(MapHearingRequest).ToList()
            };

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
            var request = new UpdateHearingsInGroupRequest
            {
                Hearings = new List<HearingRequest>()
            };
            
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
            var request = new UpdateHearingsInGroupRequest
            {
                Hearings = null
            };
            
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
        public async Task should_return_bad_request_when_invalid_participants_in_request()
        {
            // Arrange
            var hearings = await SeedHearingsInGroup();

            var request = new UpdateHearingsInGroupRequest
            {
                Hearings = hearings.Select(MapHearingRequest).ToList()
            };

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

            var request = new UpdateHearingsInGroupRequest
            {
                Hearings = hearings.Select(MapHearingRequest).ToList()
            };

            const string invalidHearingRoleCode = "INVALID_CODE";

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
 
            var request = new UpdateHearingsInGroupRequest
            {
                Hearings = hearings.Select(MapHearingRequest).ToList()
            };

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

        private async Task<List<VideoHearing>> SeedHearingsInGroup()
        {
            var dates = new List<DateTime>
            {
                DateTime.Today.AddDays(1).AddHours(10),
                DateTime.Today.AddDays(2).AddHours(10),
                DateTime.Today.AddDays(3).AddHours(10)
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
        
        private static HearingRequest MapHearingRequest(Hearing hearing) =>
            new()
            {
                HearingId = hearing.Id,
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

        private void AssertEventsPublished(Hearing hearing, HearingRequest requestHearing)
        {
            var serviceBusStub = Application.Services
                .GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
            var messages = serviceBusStub!
                .ReadAllMessagesFromQueue(hearing.Id);

            AssertParticipantEvents();
            AssertEndpointEvents();
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
                var expectedUpdatedCount = requestHearing.Participants.ExistingParticipants.Count;
                var expectedRemovedCount = requestHearing.Participants.RemovedParticipantIds.Count;
                var expectedLinkedCount = requestHearing.Participants.LinkedParticipants.Count;

                participantMessage.NewParticipants.Count.Should().Be(expectedAddedCount);
                participantMessage.ExistingParticipants.Count.Should().Be(expectedUpdatedCount);
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
        }
    }
}
