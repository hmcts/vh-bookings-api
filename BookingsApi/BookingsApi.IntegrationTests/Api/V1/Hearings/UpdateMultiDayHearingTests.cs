using BookingsApi.Contract.V1.Requests;
using BookingsApi.Domain.Participants;

namespace BookingsApi.IntegrationTests.Api.V1.Hearings
{
    public class UpdateMultiDayHearingTests : ApiTest
    {
        [TestCase(false)]
        [TestCase(true)]
        public async Task should_update_multi_day_hearing(bool updateFutureDays)
        {
            // Arrange
            var dates = new List<DateTime>
            {
                DateTime.Today.AddDays(1).AddHours(10),
                DateTime.Today.AddDays(2).AddHours(10),
                DateTime.Today.AddDays(3).AddHours(10)
            };

            var hearings = await Hooks.SeedMultiDayHearing(dates);
            var hearingToUpdate = hearings[0];
            
            // TODO make this a mixture of existing, new and removed participants
            var request = new UpdateMultiDayHearingRequest
            {
                Participants = hearingToUpdate.Participants.Select(x => new EditableParticipantRequest
                {
                    Id = x.Id,
                    Title = x.Person.Title,
                    FirstName = x.Person.FirstName,
                    MiddleNames = x.Person.MiddleNames,
                    LastName = x.Person.LastName,
                    ContactEmail = x.Person.ContactEmail,
                    TelephoneNumber = x.Person.TelephoneNumber,
                    Username = x.Person.Username,
                    DisplayName = x.DisplayName,
                    CaseRoleName = x.CaseRole.Name,
                    HearingRoleName = x.HearingRole.Name,
                    Representee = x is Representative representative ? representative.Representee : null,
                    OrganisationName = x.Person.Organisation?.Name
                }).ToList(),
                UpdateFutureDays = updateFutureDays
            };
            
            // Act
            using var client = Application.CreateClient();
            var result = await client.PutAsync(ApiUriFactory.HearingsEndpoints.UpdateMultiDayHearing(hearingToUpdate.Id), RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            
            // TODO assert on the hearing, should only update future days if set in request
        }
    }
}
