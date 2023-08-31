using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Requests.Enums;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.DAL.Queries;
using BookingsApi.Validations.V1;

namespace BookingsApi.IntegrationTests.Api.V1.JudiciaryParticipants
{
    public class AddJudiciaryParticipantsTests : ApiTest
    {
        private readonly string _personalCodeJudge;
        private readonly string _personalCodePanelMember;

        public AddJudiciaryParticipantsTests()
        {
            _personalCodeJudge = Guid.NewGuid().ToString();
            _personalCodePanelMember = Guid.NewGuid().ToString();
        }
        
        [Test]
        public async Task Should_add_judiciary_participants()
        {
            // Arrange
            var seededHearing = await Hooks.SeedVideoHearing(addJudge: false, addJudiciaryPanelMember: true);
            var judiciaryPersonJudge = await Hooks.AddJudiciaryPerson(personalCode: _personalCodeJudge);
            var judiciaryPersonPanelMember = await Hooks.AddJudiciaryPerson(personalCode: _personalCodePanelMember);
            var judiciaryParticipantsCountBefore = seededHearing.JudiciaryParticipants.Count;
            
            var request = BuildValidAddJudiciaryParticipantsRequest();

            // Act
            using var client = Application.CreateClient();
            var result = await client.PostAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.AddJudiciaryParticipantsToHearing(seededHearing.Id), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var hearing = await new GetHearingByIdQueryHandler(db).Handle(new GetHearingByIdQuery(seededHearing.Id));
            var judiciaryParticipants = hearing.JudiciaryParticipants.OrderBy(x => x.DisplayName).ToList();
            judiciaryParticipants.Count.Should().Be(request.Count + judiciaryParticipantsCountBefore);
            judiciaryParticipants[0].JudiciaryPersonId.Should().Be(judiciaryPersonJudge.Id);
            judiciaryParticipants[0].DisplayName.Should().Be(request[0].DisplayName);
            judiciaryParticipants[0].HearingRoleCode.Should().Be(Domain.Enumerations.JudiciaryParticipantHearingRoleCode.Judge);
            judiciaryParticipants[1].JudiciaryPersonId.Should().Be(judiciaryPersonPanelMember.Id);
            judiciaryParticipants[1].DisplayName.Should().Be(request[1].DisplayName);
            judiciaryParticipants[1].HearingRoleCode.Should().Be(Domain.Enumerations.JudiciaryParticipantHearingRoleCode.PanelMember);
            
            var response = await ApiClientResponse.GetResponses<IList<JudiciaryParticipantResponse>>(result.Content);
            response.Should().BeEquivalentTo(request);
        }

        [Test]
        public async Task Should_return_not_found_when_hearing_does_not_exist()
        {
            // Arrange
            var hearingId = Guid.NewGuid();
            await Hooks.AddJudiciaryPerson(personalCode: _personalCodeJudge);
            await Hooks.AddJudiciaryPerson(personalCode: _personalCodePanelMember);
            
            var request = BuildValidAddJudiciaryParticipantsRequest();

            // Act
            using var client = Application.CreateClient();
            var result = await client.PostAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.AddJudiciaryParticipantsToHearing(hearingId), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var response = await ApiClientResponse.GetResponses<string>(result.Content);
            response.Should().Be($"Hearing {hearingId} does not exist");
        }
        
        [Test]
        public async Task Should_return_not_found_when_judiciary_person_does_not_exist()
        {
            // Arrange
            var seededHearing = await Hooks.SeedVideoHearing(addJudge: false);
            await Hooks.AddJudiciaryPerson(personalCode: _personalCodePanelMember);
            
            var request = BuildValidAddJudiciaryParticipantsRequest();

            // Act
            using var client = Application.CreateClient();
            var result = await client.PostAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.AddJudiciaryParticipantsToHearing(seededHearing.Id), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var response = await ApiClientResponse.GetResponses<string>(result.Content);
            response.Should().Be($"Judiciary Person with personal code: {_personalCodeJudge} does not exist");
        }

        [Test]
        public async Task Should_return_bad_request_when_request_is_invalid()
        {
            // Arrange
            var hearingId = Guid.NewGuid();
            
            var request = BuildInvalidAddJudiciaryParticipantsRequest();

            // Act
            using var client = Application.CreateClient();
            var result = await client.PostAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.AddJudiciaryParticipantsToHearing(hearingId), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors["[0].PersonalCode"][0].Should().Be(JudiciaryParticipantRequestValidation.NoPersonalCodeErrorMessage);
            validationProblemDetails.Errors["[0].DisplayName"][0].Should().Be(JudiciaryParticipantRequestValidation.NoDisplayNameErrorMessage);
            validationProblemDetails.Errors["[1].DisplayName"][0].Should().Be(JudiciaryParticipantRequestValidation.NoDisplayNameErrorMessage);
        }
        
        [Test]
        public async Task Should_return_bad_request_when_request_contains_empty_participants()
        {
            // Arrange
            var hearingId = Guid.NewGuid();
            
            var request = BuildAddJudiciaryParticipantsRequestWithEmptyParticipants();

            // Act
            using var client = Application.CreateClient();
            var result = await client.PostAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.AddJudiciaryParticipantsToHearing(hearingId), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors["Participants"][0].Should().Be(AddJudiciaryParticipantsToHearingRequestValidation.NoParticipantsErrorMessage);
        }

        [Test]
        public async Task Should_return_bad_request_when_participant_already_exists_in_the_hearing()
        {
            // Arrange
            var seededHearing = await Hooks.SeedVideoHearing(addJudge: false, addJudiciaryPanelMember: true);
            await Hooks.AddJudiciaryPerson(personalCode: _personalCodeJudge);
            var existingParticipant = seededHearing.JudiciaryParticipants.FirstOrDefault(x => x.HearingRoleCode == Domain.Enumerations.JudiciaryParticipantHearingRoleCode.PanelMember);

            var request = BuildValidAddJudiciaryParticipantsRequest();
            var newParticipant = request.Find(x => x.HearingRoleCode == JudiciaryParticipantHearingRoleCode.PanelMember);
            newParticipant.PersonalCode = existingParticipant.JudiciaryPerson.PersonalCode;

            // Act
            using var client = Application.CreateClient();
            var result = await client.PostAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.AddJudiciaryParticipantsToHearing(seededHearing.Id), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors["judiciaryPerson"][0].Should().Be("Judiciary participant already exists in the hearing");
        }

        [Test]
        public async Task Should_return_bad_request_when_non_judiciary_judge_already_exists_in_the_hearing()
        {
            // Arrange
            var seededHearing = await Hooks.SeedVideoHearing(addJudge: true);
            await Hooks.AddJudiciaryPerson(personalCode: _personalCodeJudge);
            await Hooks.AddJudiciaryPerson(personalCode: _personalCodePanelMember);

            var request = BuildValidAddJudiciaryParticipantsRequest();

            // Act
            using var client = Application.CreateClient();
            var result = await client.PostAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.AddJudiciaryParticipantsToHearing(seededHearing.Id), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors["judiciaryPerson"][0].Should().Be("A participant with Judge role already exists in the hearing");
        }

        private List<JudiciaryParticipantRequest> BuildValidAddJudiciaryParticipantsRequest()
        {
            return new List<JudiciaryParticipantRequest>
            {
                new()
                {
                    PersonalCode = _personalCodeJudge,
                    DisplayName = "A Judge",
                    HearingRoleCode = JudiciaryParticipantHearingRoleCode.Judge
                },
                new()
                {
                    PersonalCode = _personalCodePanelMember,
                    DisplayName = "B Panel Member",
                    HearingRoleCode = JudiciaryParticipantHearingRoleCode.PanelMember
                }
            };
        }
        
        private List<JudiciaryParticipantRequest> BuildValidAddJudiciaryParticipantsRequestWithoutJudge()
        {
            return new List<JudiciaryParticipantRequest>
            {
                new()
                {
                    PersonalCode = _personalCodePanelMember,
                    DisplayName = "B Panel Member",
                    HearingRoleCode = JudiciaryParticipantHearingRoleCode.PanelMember
                }
            };
        }
        
        private List<JudiciaryParticipantRequest> BuildInvalidAddJudiciaryParticipantsRequest()
        {
            return new List<JudiciaryParticipantRequest>
            {
                new()
                {
                    PersonalCode = "",
                    DisplayName = "",
                    HearingRoleCode = JudiciaryParticipantHearingRoleCode.PanelMember
                },
                new()
                {
                    PersonalCode = _personalCodePanelMember,
                    DisplayName = "",
                    HearingRoleCode = JudiciaryParticipantHearingRoleCode.PanelMember
                }
            };
        }

        private static List<JudiciaryParticipantRequest> BuildAddJudiciaryParticipantsRequestWithEmptyParticipants()
        {
            return new List<JudiciaryParticipantRequest>();
        }
    }
}
