using BookingsApi.Contract.V2.Enums;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Contract.V2.Requests.Enums;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using BookingsApi.Validations.V2;
using FizzWare.NBuilder;

namespace BookingsApi.IntegrationTests.Api.V2.Hearings
{
    public class UpdateHearingsInGroupV2Tests : ApiTest
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
                requestHearing.HearingVenueCode = "701411"; // Manchester County and Family Court
                requestHearing.HearingRoomName = "UpdatedRoomName";
                requestHearing.OtherInformation = "UpdatedOtherInformation";
                requestHearing.AudioRecordingRequired = true;
            }

            var newParticipant = new Builder(new BuilderSettings()).CreateNew<ParticipantRequestV2>()
                .With(p => p.ContactEmail, Faker.Internet.Email())
                .With(p => p.HearingRoleCode, "APPL")
                .Build();
            var newEndpoint = new Builder(new BuilderSettings()).CreateNew<EndpointRequestV2>()
                .With(e => e.DefenceAdvocateContactEmail, null)
                .Build();
            var newJudiciaryPanelMemberPerson = await Hooks.AddJudiciaryPerson(personalCode: Guid.NewGuid().ToString());
            var newJudiciaryPanelMember = new Builder(new BuilderSettings()).CreateNew<JudiciaryParticipantRequestV2>()
                .With(x => x.HearingRoleCode, JudiciaryParticipantHearingRoleCodeV2.PanelMember)
                .With(x => x.ContactEmail, newJudiciaryPanelMemberPerson.Email)
                .With(x => x.PersonalCode, newJudiciaryPanelMemberPerson.PersonalCode)
                .Build();
            var newJudiciaryJudgePerson = await Hooks.AddJudiciaryPerson(personalCode: Guid.NewGuid().ToString());
            var newJudiciaryJudge = new Builder(new BuilderSettings()).CreateNew<JudiciaryParticipantRequestV2>()
                .With(x => x.HearingRoleCode, JudiciaryParticipantHearingRoleCodeV2.Judge)
                .With(x => x.ContactEmail, newJudiciaryJudgePerson.Email)
                .With(x => x.PersonalCode, newJudiciaryJudgePerson.PersonalCode)
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

                // Add a judiciary participant
                requestHearing.JudiciaryParticipants.NewJudiciaryParticipants.Add(newJudiciaryPanelMember);
                
                // Remove a judiciary participant
                var judiciaryPanelMemberToRemove = requestHearing.JudiciaryParticipants.ExistingJudiciaryParticipants.First(jp => jp.HearingRoleCode == JudiciaryParticipantHearingRoleCodeV2.PanelMember);
                requestHearing.JudiciaryParticipants.RemovedJudiciaryParticipantPersonalCodes.Add(judiciaryPanelMemberToRemove.PersonalCode);
                requestHearing.JudiciaryParticipants.ExistingJudiciaryParticipants.Remove(judiciaryPanelMemberToRemove);

                // Reassign a judge
                requestHearing.JudiciaryParticipants.NewJudiciaryParticipants.Add(newJudiciaryJudge);
                var judiciaryJudgeToReassign = requestHearing.JudiciaryParticipants.ExistingJudiciaryParticipants.First(jp => jp.HearingRoleCode == JudiciaryParticipantHearingRoleCodeV2.Judge);
                requestHearing.JudiciaryParticipants.RemovedJudiciaryParticipantPersonalCodes.Add(judiciaryJudgeToReassign.PersonalCode);
                requestHearing.JudiciaryParticipants.ExistingJudiciaryParticipants.Remove(judiciaryJudgeToReassign);
            }
            
            var groupId = hearings[0].SourceId.Value;

            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpointsV2.UpdateHearingsInGroupId(groupId),RequestBody.Set(request));

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
                updatedHearing.HearingVenue.VenueCode.Should().Be(requestHearing.HearingVenueCode);
                updatedHearing.HearingRoomName.Should().Be(requestHearing.HearingRoomName);
                updatedHearing.OtherInformation.Should().Be(requestHearing.OtherInformation);
                updatedHearing.AudioRecordingRequired.Should().Be(requestHearing.AudioRecordingRequired);

                AssertParticipantsUpdated(updatedHearing, requestHearing);
                AssertEndpointsUpdated(updatedHearing, requestHearing);
                AssertJudiciaryParticipantsUpdated(updatedHearing, requestHearing);
                AssertEventsPublished(updatedHearing, requestHearing, existingParticipantsUpdated: 1);
            }
        }
        
        [Test]
        public async Task should_return_not_found_when_no_hearings_found_for_group()
        {
            // Arrange
            var request = BuildRequest();
            request.Hearings = new List<HearingRequestV2>
            {
                BuildHearingRequest(DateTime.Today.AddDays(1).AddHours(10))
            };
            
            var groupId = Guid.NewGuid();
            
            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpointsV2.UpdateHearingsInGroupId(groupId),RequestBody.Set(request));
            
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

            var hearingsNotInGroup = new List<HearingRequestV2>
            {
                BuildHearingRequest(DateTime.Today.AddDays(1).AddHours(10)),
                BuildHearingRequest(DateTime.Today.AddDays(2).AddHours(10))
            };

            request.Hearings.AddRange(hearingsNotInGroup);

            var groupId = hearings[0].SourceId.Value;

            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpointsV2.UpdateHearingsInGroupId(groupId),RequestBody.Set(request));
            
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
                .PatchAsync(ApiUriFactory.HearingsEndpointsV2.UpdateHearingsInGroupId(groupId),RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors[nameof(request.Hearings)][0].Should()
                .Be(UpdateHearingsInGroupRequestInputValidationV2.DuplicateHearingIdsMessage);
        }

        [Test]
        public async Task should_return_bad_request_when_empty_hearings_list_in_request()
        {
            // Arrange
            var request = BuildRequest();
            request.Hearings = new List<HearingRequestV2>();
            
            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpointsV2.UpdateHearingsInGroupId(Guid.NewGuid()),RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors[nameof(request.Hearings)][0].Should().Be(
                UpdateHearingsInGroupRequestInputValidationV2.NoHearingsErrorMessage);
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
                .PatchAsync(ApiUriFactory.HearingsEndpointsV2.UpdateHearingsInGroupId(Guid.NewGuid()),RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors[nameof(request.Hearings)][0].Should().Be(
                UpdateHearingsInGroupRequestInputValidationV2.NoHearingsErrorMessage);
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
                .PatchAsync(ApiUriFactory.HearingsEndpointsV2.UpdateHearingsInGroupId(groupId),RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors[nameof(request.Hearings)][0].Should()
                .Be(UpdateHearingsInGroupRequestInputValidationV2.DuplicateScheduledDateTimesMessage);
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
                requestHearing.Participants.ExistingParticipants = new List<UpdateParticipantRequestV2>();
                requestHearing.Participants.NewParticipants = new List<ParticipantRequestV2>();
                requestHearing.Participants.LinkedParticipants = new List<LinkedParticipantRequestV2>();
                requestHearing.Participants.RemovedParticipantIds = new List<Guid>();
            }
            
            var groupId = hearings[0].SourceId.Value;

            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpointsV2.UpdateHearingsInGroupId(groupId),RequestBody.Set(request));

            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors["Hearings[0].Participants"][0].Should().Be(
                UpdateHearingParticipantsRequestInputValidationV2.NoParticipantsErrorMessage);
        }

        [Test]
        public async Task should_return_bad_request_when_invalid_participant_ref_data_in_request()
        {
            // Arrange
            var hearings = await SeedHearingsInGroup();

            var request = BuildRequest();
            request.Hearings = hearings.Select(MapHearingRequest).ToList();

            const string invalidHearingRoleCode = "INVALID_CODE";

            var newParticipant = new Builder(new BuilderSettings()).CreateNew<ParticipantRequestV2>()
                .With(p => p.ContactEmail, Faker.Internet.Email())
                .With(p => p.HearingRoleCode, invalidHearingRoleCode)
                .Build();
            
            foreach (var requestHearing in request.Hearings)
            {
                requestHearing.Participants.NewParticipants.Add(newParticipant);
            }
            
            var groupId = hearings[0].SourceId.Value;

            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpointsV2.UpdateHearingsInGroupId(groupId),RequestBody.Set(request));

            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors["Hearings[0].Participants.NewParticipants[0]"][0].Should().Be(
                $"Invalid hearing role [{invalidHearingRoleCode}]");
        }

        [Test]
        public async Task should_return_bad_request_when_invalid_endpoints_in_request()
        {
            // Arrange
            var hearings = await SeedHearingsInGroup();

            var request = BuildRequest();
            request.Hearings = hearings.Select(MapHearingRequest).ToList();

            var newEndpoint = new Builder(new BuilderSettings()).CreateNew<EndpointRequestV2>()
                .With(e => e.DefenceAdvocateContactEmail, null)
                .With(e => e.DisplayName, "**") // Invalid display name
                .Build();
            
            foreach (var requestHearing in request.Hearings)
            {
                requestHearing.Endpoints.NewEndpoints.Add(newEndpoint);
            }
            
            var groupId = hearings[0].SourceId.Value;

            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpointsV2.UpdateHearingsInGroupId(groupId),RequestBody.Set(request));

            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors["Hearings[0].Endpoints.NewEndpoints[0].DisplayName"][0].Should().Be(
                EndpointRequestValidationV2.InvalidDisplayNameErrorMessage);
        }

        [Test]
        public async Task should_return_bad_request_when_invalid_judiciary_participants_in_request()
        {
            // Arrange
            var hearings = await SeedHearingsInGroup();

            var request = BuildRequest();
            request.Hearings = hearings.Select(MapHearingRequest).ToList();

            var newJudiciaryPanelMemberPerson = await Hooks.AddJudiciaryPerson(personalCode: Guid.NewGuid().ToString());
            var newJudiciaryPanelMember = new Builder(new BuilderSettings()).CreateNew<JudiciaryParticipantRequestV2>()
                .With(x => x.HearingRoleCode, JudiciaryParticipantHearingRoleCodeV2.PanelMember)
                .With(x => x.ContactEmail, newJudiciaryPanelMemberPerson.Email)
                .With(x => x.PersonalCode, "") // Invalid personal code
                .Build();
            
            foreach (var requestHearing in request.Hearings)
            {
                requestHearing.JudiciaryParticipants.NewJudiciaryParticipants.Add(newJudiciaryPanelMember);
            }
            
            var groupId = hearings[0].SourceId.Value;

            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpointsV2.UpdateHearingsInGroupId(groupId),RequestBody.Set(request));

            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors["Hearings[0].JudiciaryParticipants.NewJudiciaryParticipants[0].PersonalCode"][0].Should().Be(
                JudiciaryParticipantRequestValidationV2.NoPersonalCodeErrorMessage);
        }
        
        [Test]
        public async Task should_return_bad_request_when_invalid_details_in_request()
        {
            // Arrange
            var request = new UpdateHearingsInGroupRequestV2();
            var groupId = Guid.NewGuid();

            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpointsV2.UpdateHearingsInGroupId(groupId),RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors[nameof(request.UpdatedBy)][0].Should().Be(
                UpdateHearingsInGroupRequestInputValidationV2.NoUpdatedByErrorMessage);
        }

        [Test]
        public async Task should_return_bad_request_when_invalid_hearing_details_in_request()
        {
            // Arrange
            var request = new UpdateHearingsInGroupRequestV2
            {
                Hearings = new List<HearingRequestV2>
                {
                    new()
                    {
                        HearingId = Guid.NewGuid(),
                        ScheduledDateTime = new DateTime(2020, 1, 1, 10, 0, 0, DateTimeKind.Utc)
                    }
                }
            };
            var groupId = Guid.NewGuid();

            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpointsV2.UpdateHearingsInGroupId(groupId),RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors["Hearings[0].HearingVenueCode"][0].Should().Be(
                UpdateHearingRequestValidationV2.NoHearingVenueCodeErrorMessage);
            validationProblemDetails.Errors["Hearings[0].ScheduledDuration"][0].Should().Be(
                UpdateHearingRequestValidationV2.NoScheduleDurationErrorMessage);
            validationProblemDetails.Errors["Hearings[0].CaseNumber"][0].Should().Be(
                CaseRequestValidationV2.CaseNumberMessage);
            validationProblemDetails.Errors["Hearings[0].ScheduledDateTime"][0].Should().Be(
                UpdateHearingRequestValidationV2.ScheduleDateTimeInPastErrorMessage);
        }

        [Test]
        public async Task should_return_bad_request_when_hearing_venue_code_does_not_exist()
        {
            // Arrange
            var hearings = await SeedHearingsInGroup();
            
            var request = BuildRequest();
            request.Hearings = hearings.Select(MapHearingRequest).ToList();
            request.Hearings[0].HearingVenueCode = "NonExistingVenueCode";
            
            var groupId = hearings[0].SourceId.Value;
            
            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpointsV2.UpdateHearingsInGroupId(groupId),RequestBody.Set(request));

            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors["Hearings[0]"][0].Should()
                .Be($"Hearing venue code {request.Hearings[0].HearingVenueCode} does not exist");
        }
        
        private static UpdateHearingsInGroupRequestV2 BuildRequest() =>
            new()
            {
                UpdatedBy = "updatedBy@email.com"
            };

        private static HearingRequestV2 BuildHearingRequest(DateTime scheduledDateTime) =>
            new()
            {
                HearingId = Guid.NewGuid(),
                HearingVenueCode = "VenueCode",
                ScheduledDateTime = scheduledDateTime,
                ScheduledDuration = 45,
                CaseNumber = "CaseNumber"
            };

        private async Task<List<VideoHearing>> SeedHearingsInGroup()
        {
            var dates = new List<DateTime>
            {
                DateTime.Today.AddDays(5).AddHours(10),
                DateTime.Today.AddDays(6).AddHours(10),
                DateTime.Today.AddDays(7).AddHours(10)
            };

            var multiDayHearings = await Hooks.SeedMultiDayHearing(useV2: true, dates, addPanelMember: true);

            var judiciaryPersonPanelMember = await Hooks.AddJudiciaryPerson(personalCode: Guid.NewGuid().ToString());
            
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            
            // Add a second panel member for test coverage
            var hearings = new List<VideoHearing>();
            foreach (var hearing in multiDayHearings)
            {
                await Hooks.AddJudiciaryPanelMember(hearing, judiciaryPersonPanelMember, "Additional Panel Member");
                var hearingFromDb = await new GetHearingByIdQueryHandler(db).Handle(new GetHearingByIdQuery(hearing.Id));
                hearings.Add(hearingFromDb);
            }

            return hearings;
        }

        private static HearingRequestV2 MapHearingRequest(Hearing hearing) =>
            new()
            {
                HearingId = hearing.Id,
                CaseNumber = hearing.GetCases().FirstOrDefault().Number,
                ScheduledDateTime = hearing.ScheduledDateTime,
                ScheduledDuration = hearing.ScheduledDuration,
                HearingVenueCode = hearing.HearingVenue.VenueCode,
                HearingRoomName = hearing.HearingRoomName,
                OtherInformation = hearing.OtherInformation,
                AudioRecordingRequired = hearing.AudioRecordingRequired,
                Participants = new UpdateHearingParticipantsRequestV2
                {
                    ExistingParticipants = hearing.Participants.Select(p => new UpdateParticipantRequestV2
                    {
                        ParticipantId = p.Id,
                        Title = p.Person.Title,
                        TelephoneNumber = p.Person.TelephoneNumber,
                        DisplayName = p.DisplayName,
                        OrganisationName = p.Person.Organisation?.Name,
                        Representee = p is Representative ? (p as Representative).Representee : null,
                        FirstName = p.Person.FirstName,
                        MiddleNames = p.Person.MiddleNames,
                        LastName = p.Person.LastName,
                        LinkedParticipants = p.LinkedParticipants.Select(lp => new LinkedParticipantRequestV2
                        {
                            ParticipantContactEmail = lp.Participant.Person.ContactEmail,
                            LinkedParticipantContactEmail = lp.Linked.Person.ContactEmail,
                            Type = LinkedParticipantTypeV2.Interpreter
                        }).ToList()
                    }).ToList()
                },
                Endpoints = new UpdateHearingEndpointsRequestV2
                {
                    ExistingEndpoints = hearing.Endpoints.Select(e => new UpdateEndpointRequestV2
                    {
                        Id = e.Id,
                        DisplayName = e.DisplayName,
                        DefenceAdvocateContactEmail = e.DefenceAdvocate?.Person.ContactEmail
                    }).ToList()
                },
                JudiciaryParticipants = new UpdateJudiciaryParticipantsRequestV2
                {
                    ExistingJudiciaryParticipants = hearing.JudiciaryParticipants.Select(jp => new EditableUpdateJudiciaryParticipantRequestV2
                    {
                        DisplayName = jp.DisplayName,
                        PersonalCode = jp.JudiciaryPerson.PersonalCode,
                        HearingRoleCode = jp.HearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge ?
                            JudiciaryParticipantHearingRoleCodeV2.Judge
                            : JudiciaryParticipantHearingRoleCodeV2.PanelMember
                    }).ToList()
                }
            };

        private static void AssertParticipantsUpdated(Hearing hearing, HearingRequestV2 requestHearing)
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

        private static void AssertEndpointsUpdated(Hearing hearing, HearingRequestV2 requestHearing)
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

        private static void AssertJudiciaryParticipantsUpdated(Hearing hearing, HearingRequestV2 requestHearing)
        {
            var expectedJudiciaryParticipantCount = requestHearing.JudiciaryParticipants.NewJudiciaryParticipants.Count + 
                                                    requestHearing.JudiciaryParticipants.ExistingJudiciaryParticipants.Count;
            var judiciaryParticipants = hearing.GetJudiciaryParticipants();
            judiciaryParticipants.Count.Should().Be(expectedJudiciaryParticipantCount);
            foreach (var newJudiciaryParticipant in requestHearing.JudiciaryParticipants.NewJudiciaryParticipants)
            {
                judiciaryParticipants.Should().Contain(p => p.JudiciaryPerson.PersonalCode == newJudiciaryParticipant.PersonalCode);
            }
            judiciaryParticipants.Should().NotContain(p => requestHearing.JudiciaryParticipants.RemovedJudiciaryParticipantPersonalCodes.Contains(p.JudiciaryPerson.PersonalCode));
            var newJudge = requestHearing.JudiciaryParticipants.NewJudiciaryParticipants.Find(jp => jp.HearingRoleCode == JudiciaryParticipantHearingRoleCodeV2.Judge);
            if (newJudge != null)
            {
                ((JudiciaryParticipant)hearing.GetJudge()).JudiciaryPerson.PersonalCode.Should().Be(newJudge.PersonalCode);
            }
        }

        private void AssertEventsPublished(Hearing hearing, HearingRequestV2 requestHearing, int existingParticipantsUpdated)
        {
            var serviceBusStub = Application.Services
                .GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
            var messages = serviceBusStub!
                .ReadAllMessagesFromQueue(hearing.Id);
            
            AssertParticipantEvents();
            AssertJudiciaryParticipantEvents();
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
                participantMessage.ExistingParticipants.Count.Should().Be(existingParticipantsUpdated);
                participantMessage.RemovedParticipants.Count.Should().Be(expectedRemovedCount);
                participantMessage.LinkedParticipants.Count.Should().Be(expectedLinkedCount);
            }
            
            void AssertJudiciaryParticipantEvents()
            {
                var expectedAddedCount = requestHearing.JudiciaryParticipants.NewJudiciaryParticipants.Count;
            
                var participantAddedMessages = messages
                    .Where(x => x.IntegrationEvent is ParticipantsAddedIntegrationEvent)
                    .Select(x => x.IntegrationEvent as ParticipantsAddedIntegrationEvent)
                    .Where(x => x.Hearing.HearingId == hearing.Id)
                    .ToList();

                participantAddedMessages.Count.Should().Be(expectedAddedCount);

                var expectedRemovedCount = requestHearing.JudiciaryParticipants.RemovedJudiciaryParticipantPersonalCodes.Count;
            
                var participantRemovedMessages = messages
                    .Where(x => x.IntegrationEvent is ParticipantRemovedIntegrationEvent)
                    .Select(x => x.IntegrationEvent as ParticipantRemovedIntegrationEvent)
                    .Where(x => x.HearingId == hearing.Id)
                    .ToList();

                participantRemovedMessages.Count.Should().Be(expectedRemovedCount);
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
                const int expectedDetailsUpdatedCount = 1;
                
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
    }
}
