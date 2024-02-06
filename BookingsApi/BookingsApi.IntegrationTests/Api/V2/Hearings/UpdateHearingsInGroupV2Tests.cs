using BookingsApi.Contract.V2.Enums;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using FizzWare.NBuilder;

namespace BookingsApi.IntegrationTests.Api.V2.Hearings
{
    public class UpdateHearingsInGroupV2Tests : ApiTest
    {
        [Test]
        public async Task should_update_hearings_in_group()
        {
            // Arrange
            var dates = new List<DateTime>
            {
                DateTime.Today.AddDays(1).AddHours(10),
                DateTime.Today.AddDays(2).AddHours(10),
                DateTime.Today.AddDays(3).AddHours(10)
            };

            var hearings = await Hooks.SeedMultiDayHearingV2(dates, addPanelMember: true);
 
            var request = new UpdateHearingsInGroupRequestV2
            {
                Hearings = hearings.Select(MapHearingRequest).ToList()
            };

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
                
                // Add and remove a participant
                requestHearing.Participants.NewParticipants.Add(newParticipant);
                var participantToRemove = requestHearing.Participants.ExistingParticipants.First(p => p.ParticipantId != defenceAdvocateParticipant.Id);
                requestHearing.Participants.RemovedParticipantIds.Add(participantToRemove.ParticipantId);
                requestHearing.Participants.ExistingParticipants.Remove(participantToRemove);
            
                // Add and remove an endpoint
                requestHearing.Endpoints.NewEndpoints.Add(newEndpoint);
                var endpointToRemove = requestHearing.Endpoints.ExistingEndpoints.First(e => e.DefenceAdvocateContactEmail != defenceAdvocateEmail);
                requestHearing.Endpoints.RemovedEndpointIds.Add(endpointToRemove.Id);
                requestHearing.Endpoints.ExistingEndpoints.Remove(endpointToRemove);

                // Add and remove a judiciary participant
                requestHearing.JudiciaryParticipants.NewJudiciaryParticipants.Add(newJudiciaryPanelMember);
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

                AssertParticipantsUpdated(updatedHearing, requestHearing);
                AssertEndpointsUpdated(updatedHearing, requestHearing);
                AssertJudiciaryParticipantsUpdated(updatedHearing, requestHearing);
            }
        }

        [Test]
        public async Task should_return_not_found_when_no_hearings_found_for_group()
        {
            // Arrange
            var request = new UpdateHearingsInGroupRequestV2
            {
                Hearings = new List<HearingRequestV2>
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
                .PatchAsync(ApiUriFactory.HearingsEndpointsV2.UpdateHearingsInGroupId(groupId),RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.NotFound, result.Content.ReadAsStringAsync().Result);
        }

        [Test]
        public async Task should_return_bad_request_when_hearings_in_request_do_not_belong_to_group()
        {
            // Arrange
            var dates = new List<DateTime>
            {
                DateTime.Today.AddDays(1).AddHours(10),
                DateTime.Today.AddDays(2).AddHours(10),
                DateTime.Today.AddDays(3).AddHours(10)
            };

            var hearings = await Hooks.SeedMultiDayHearingV2(dates);

            var request = new UpdateHearingsInGroupRequestV2
            {
                Hearings = hearings.Select(MapHearingRequest).ToList()
            };

            var hearingsNotInGroup = new List<HearingRequestV2>
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
                .PatchAsync(ApiUriFactory.HearingsEndpointsV2.UpdateHearingsInGroupId(groupId),RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors["hearings[3].HearingId"][0].Should()
                .Be($"Hearing {hearingsNotInGroup[0].HearingId} does not belong to group {groupId}");
            validationProblemDetails.Errors["hearings[4].HearingId"][0].Should()
                .Be($"Hearing {hearingsNotInGroup[1].HearingId} does not belong to group {groupId}");
        }

        [Test]
        public async Task should_return_bad_request_when_duplicate_hearing_id_specified()
        {
            // Arrange
            var dates = new List<DateTime>
            {
                DateTime.Today.AddDays(1).AddHours(10),
                DateTime.Today.AddDays(2).AddHours(10),
                DateTime.Today.AddDays(3).AddHours(10)
            };

            var hearings = await Hooks.SeedMultiDayHearingV2(dates);

            var request = new UpdateHearingsInGroupRequestV2
            {
                Hearings = hearings.Select(MapHearingRequest).ToList()
            };

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
            validationProblemDetails.Errors["hearings[0].HearingId"][0].Should()
                .Be($"Duplicate hearing id {request.Hearings[0].HearingId}");
        }

        [Test]
        public async Task should_return_bad_request_when_empty_hearings_list_specified()
        {
            // Arrange
            var request = new UpdateHearingsInGroupRequestV2
            {
                Hearings = new List<HearingRequestV2>()
            };
            
            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpointsV2.UpdateHearingsInGroupId(Guid.NewGuid()),RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors["hearings"][0].Should().Be("Please provide at least one hearing");
        }
        
        [Test]
        public async Task should_return_bad_request_when_null_hearings_list_specified()
        {
            // Arrange
            var request = new UpdateHearingsInGroupRequestV2
            {
                Hearings = null
            };
            
            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpointsV2.UpdateHearingsInGroupId(Guid.NewGuid()),RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors["hearings"][0].Should().Be("Please provide at least one hearing");
        }

        private static HearingRequestV2 MapHearingRequest(Hearing hearing) =>
            new()
            {
                HearingId = hearing.Id,
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
    }
}
