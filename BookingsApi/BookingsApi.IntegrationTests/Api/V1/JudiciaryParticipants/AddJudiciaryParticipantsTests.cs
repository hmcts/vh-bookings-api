using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Requests.Enums;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain.Validations;
using BookingsApi.Validations.V1;

namespace BookingsApi.IntegrationTests.Api.V1.JudiciaryParticipants
{
    public class AddJudiciaryParticipantsTests : ApiTest
    {
        private string _personalCodeJudge;
        private string _personalCodePanelMember;
        private string _personalCodeGenericPanelMember;

        [SetUp]
        public void Setup()
        {
            _personalCodeJudge = Guid.NewGuid().ToString();
            _personalCodePanelMember = Guid.NewGuid().ToString();
            _personalCodeGenericPanelMember = Guid.NewGuid().ToString();
        }
        
        [Test]
        public async Task Should_add_judiciary_participants()
        {
            // Arrange
            var seededHearing = await Hooks.SeedVideoHearingV2(configureOptions: options =>
            {
                options.AddJudge = false;
                options.AddPanelMember = false;
            });
            var judiciaryPersonJudge = await Hooks.AddJudiciaryPerson(personalCode: _personalCodeJudge);
            var judiciaryPersonPanelMember = await Hooks.AddJudiciaryPerson(personalCode: _personalCodePanelMember);
            var judiciaryPersonGenericPanelMember = await Hooks.AddGenericJudiciaryPerson(personalCode: _personalCodeGenericPanelMember);
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
            
            AssertJudiciaryJudgeParticipant(judiciaryParticipants[0], request[0], judiciaryPersonJudge.Id);
            AssertJudiciaryPanelMemberParticipant(judiciaryParticipants[1], request[1], judiciaryPersonPanelMember.Id);
            AssertJudiciaryPanelMemberParticipant(judiciaryParticipants[2], request[2], judiciaryPersonGenericPanelMember.Id);

            var response = await ApiClientResponse.GetResponses<List<JudiciaryParticipantResponse>>(result.Content);
            response.Count.Should().Be(request.Count);

            var judgeResponse = response.Find(x => x.PersonalCode == _personalCodeJudge);
            judgeResponse.DisplayName.Should().Be(request[0].DisplayName);
            judgeResponse.HearingRoleCode.Should().Be(JudiciaryParticipantHearingRoleCode.Judge);
            judgeResponse.Email.Should().Be(judiciaryPersonJudge.Email);
            judgeResponse.Title.Should().Be(judiciaryPersonJudge.Title);
            judgeResponse.FirstName.Should().Be(judiciaryPersonJudge.KnownAs);
            judgeResponse.LastName.Should().Be(judiciaryPersonJudge.Surname);
            judgeResponse.FullName.Should().Be(judiciaryPersonJudge.Fullname);
            judgeResponse.WorkPhone.Should().Be(judiciaryPersonJudge.WorkPhone);
            
            var panelMemberResponse = response.Find(x => x.PersonalCode == _personalCodePanelMember);
            panelMemberResponse.DisplayName.Should().Be(request[1].DisplayName);
            panelMemberResponse.HearingRoleCode.Should().Be(JudiciaryParticipantHearingRoleCode.PanelMember);
            panelMemberResponse.Email.Should().Be(judiciaryPersonPanelMember.Email);
            panelMemberResponse.Title.Should().Be(judiciaryPersonPanelMember.Title);
            panelMemberResponse.FirstName.Should().Be(judiciaryPersonPanelMember.KnownAs);
            panelMemberResponse.LastName.Should().Be(judiciaryPersonPanelMember.Surname);
            panelMemberResponse.FullName.Should().Be(judiciaryPersonPanelMember.Fullname);
            panelMemberResponse.WorkPhone.Should().Be(judiciaryPersonPanelMember.WorkPhone);
            
            var genericPanelMemberResponse = response.Find(x => x.PersonalCode == _personalCodeGenericPanelMember);
            genericPanelMemberResponse.DisplayName.Should().Be(request[2].DisplayName);
            genericPanelMemberResponse.HearingRoleCode.Should().Be(JudiciaryParticipantHearingRoleCode.PanelMember);
            genericPanelMemberResponse.Email.Should().Be(request[2].OptionalContactEmail);
            genericPanelMemberResponse.Title.Should().Be(judiciaryPersonGenericPanelMember.Title);
            genericPanelMemberResponse.FirstName.Should().Be(judiciaryPersonGenericPanelMember.KnownAs);
            genericPanelMemberResponse.LastName.Should().Be(judiciaryPersonGenericPanelMember.Surname);
            genericPanelMemberResponse.FullName.Should().Be(judiciaryPersonGenericPanelMember.Fullname);
            genericPanelMemberResponse.WorkPhone.Should().Be(request[2].OptionalContactTelephone);
        }

        private void AssertJudiciaryJudgeParticipant(JudiciaryParticipant judiciaryParticipant, JudiciaryParticipantRequest request, Guid id)
        {
            AssertJudiciaryParticipant(judiciaryParticipant, request, id);
            judiciaryParticipant.HearingRoleCode.Should().Be(Domain.Enumerations.JudiciaryParticipantHearingRoleCode.Judge);
        }
        
        private void AssertJudiciaryPanelMemberParticipant(JudiciaryParticipant judiciaryParticipant, JudiciaryParticipantRequest request, Guid id)
        {
            AssertJudiciaryParticipant(judiciaryParticipant, request, id);
            judiciaryParticipant.HearingRoleCode.Should().Be(Domain.Enumerations.JudiciaryParticipantHearingRoleCode.PanelMember);
        }
        
        private void AssertJudiciaryParticipant(JudiciaryParticipant judiciaryParticipant, JudiciaryParticipantRequest request, Guid id)
        {
            judiciaryParticipant.JudiciaryPersonId.Should().Be(id);
            judiciaryParticipant.DisplayName.Should().Be(request.DisplayName);
            judiciaryParticipant.ContactTelephone.Should().Be(request.OptionalContactTelephone);
            judiciaryParticipant.ContactEmail.Should().Be(request.OptionalContactEmail);
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
            var seededHearing = await Hooks.SeedVideoHearingV2(configureOptions: options =>
            {
                options.AddJudge = false;
                options.AddPanelMember = false;
            });
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
            validationProblemDetails.Errors["judiciaryParticipants[0].PersonalCode"][0].Should().Be(JudiciaryParticipantRequestValidation.NoPersonalCodeErrorMessage);
            validationProblemDetails.Errors["judiciaryParticipants[0].DisplayName"][0].Should().Be(JudiciaryParticipantRequestValidation.NoDisplayNameErrorMessage);
            validationProblemDetails.Errors["judiciaryParticipants[1].DisplayName"][0].Should().Be(JudiciaryParticipantRequestValidation.NoDisplayNameErrorMessage);
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
            validationProblemDetails.Errors[""][0].Should().Be(AddJudiciaryParticipantsToHearingRequestValidation.NoParticipantsErrorMessage);
        }

        [Test]
        public async Task Should_return_bad_request_when_participant_already_exists_in_the_hearing()
        {
            // Arrange
            var seededHearing = await Hooks.SeedVideoHearingV2(configureOptions: options =>
            {
                options.AddJudge = false;
                options.AddPanelMember = true;
            });
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
            validationProblemDetails.Errors["judiciaryPerson"][0].Should().Be(DomainRuleErrorMessages.JudiciaryPersonAlreadyExists(existingParticipant.JudiciaryPerson.PersonalCode));
        }

        [Test]
        public async Task Should_return_bad_request_when_non_judiciary_judge_already_exists_in_the_hearing()
        {
            // Arrange
            var seededHearing = await Hooks.SeedVideoHearing(configureOptions: options =>
            {
                options.AddJudge = true;
            });
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
                },
                new()
                {
                    PersonalCode = _personalCodeGenericPanelMember,
                    DisplayName = "Generic Panel Member",
                    HearingRoleCode = JudiciaryParticipantHearingRoleCode.PanelMember,
                    OptionalContactTelephone = "0123456789",
                    OptionalContactEmail = "generic-panel-member@email.com"
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
