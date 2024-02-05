using BookingsApi.Contract.V2.Enums;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;

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

            var hearings = await Hooks.SeedMultiDayHearingV2(dates);

            var hearingRequests = hearings.Select(h => new HearingRequestV2
            {
                HearingId = h.Id,
                Participants = new UpdateHearingParticipantsRequestV2
                {
                    ExistingParticipants = h.Participants.Select(p => new UpdateParticipantRequestV2
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
                    ExistingEndpoints = h.Endpoints.Select(e => new UpdateEndpointRequestV2
                    {
                        Id = e.Id,
                        DisplayName = e.DisplayName,
                        DefenceAdvocateContactEmail = e.DefenceAdvocate?.Person.ContactEmail
                    }).ToList()
                },
                JudiciaryParticipants = new UpdateJudiciaryParticipantsRequestV2
                {
                    ExistingJudiciaryParticipants = h.JudiciaryParticipants.Select(jp => new EditableUpdateJudiciaryParticipantRequestV2
                    {
                        DisplayName = jp.DisplayName,
                        PersonalCode = jp.JudiciaryPerson.PersonalCode,
                        HearingRoleCode = jp.HearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge ?
                            JudiciaryParticipantHearingRoleCodeV2.Judge
                            : JudiciaryParticipantHearingRoleCodeV2.PanelMember
                    }).ToList()
                }
            }).ToList();

            var request = new UpdateHearingsInGroupRequestV2
            {
                Hearings = hearingRequests
            };

            var groupId = hearings[0].SourceId.Value;

            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.HearingsEndpointsV2.UpdateHearingsInGroupId(groupId),RequestBody.Set(request));

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NoContent, result.Content.ReadAsStringAsync().Result);
        }
    }
}
