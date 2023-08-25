using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Requests.Enums;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.DAL.Queries;
using BookingsApi.Validations.V1;
using FizzWare.NBuilder;

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
            var seededHearing = await Hooks.SeedVideoHearing(addJudge: false);
            var judiciaryPersonJudge = await Hooks.AddJudiciaryPerson(personalCode: _personalCodeJudge);
            var judiciaryPersonPanelMember = await Hooks.AddJudiciaryPerson(personalCode: _personalCodePanelMember);
            
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
            judiciaryParticipants.Count.Should().Be(request.Participants.Count);
            judiciaryParticipants[0].JudiciaryPersonId.Should().Be(judiciaryPersonJudge.Id);
            judiciaryParticipants[0].DisplayName.Should().Be(request.Participants[0].DisplayName);
            judiciaryParticipants[0].HearingRoleCode.Should().Be(Domain.Enumerations.JudiciaryParticipantHearingRoleCode.Judge);
            judiciaryParticipants[1].JudiciaryPersonId.Should().Be(judiciaryPersonPanelMember.Id);
            judiciaryParticipants[1].DisplayName.Should().Be(request.Participants[1].DisplayName);
            judiciaryParticipants[1].HearingRoleCode.Should().Be(Domain.Enumerations.JudiciaryParticipantHearingRoleCode.PanelMember);
            
            var response = await ApiClientResponse.GetResponses<IList<JudiciaryParticipantResponse>>(result.Content);
            response.Should().BeEquivalentTo(request.Participants);

            // TODO assert on participants added event being published
        }

        [Test]
        public async Task Should_only_return_participants_added_from_request()
        {
            // Arrange
            var seededHearing = await Hooks.SeedVideoHearing(addJudiciaryPanelMember: true);
            await Hooks.AddJudiciaryPerson(personalCode: _personalCodePanelMember);
            var judiciaryParticipantsCountBefore = seededHearing.JudiciaryParticipants.Count;
            
            var request = BuildValidAddJudiciaryParticipantsRequestWithoutJudge();

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
            judiciaryParticipants.Count.Should().Be(request.Participants.Count + judiciaryParticipantsCountBefore);
            
            var response = await ApiClientResponse.GetResponses<IList<JudiciaryParticipantResponse>>(result.Content);
            response.Should().BeEquivalentTo(request.Participants);
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
            validationProblemDetails.Errors["Participants[0].PersonalCode"][0].Should().Be(JudiciaryParticipantRequestValidation.NoPersonalCodeErrorMessage);
            validationProblemDetails.Errors["Participants[0].DisplayName"][0].Should().Be(JudiciaryParticipantRequestValidation.NoDisplayNameErrorMessage);
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
        public async Task Should_return_bad_request_when_request_contains_null_participants()
        {
            // Arrange
            var hearingId = Guid.NewGuid();
            
            var request = BuildAddJudiciaryParticipantsRequestWithNullParticipants();

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
            var newParticipant = request.Participants.FirstOrDefault(x => x.HearingRoleCode == JudiciaryParticipantHearingRoleCode.PanelMember);
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

        private AddJudiciaryParticipantsRequest BuildValidAddJudiciaryParticipantsRequest()
        {
            return Builder<AddJudiciaryParticipantsRequest>.CreateNew()
                .With(x => x.Participants = new List<JudiciaryParticipantRequest>
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
                })
                .Build();
        }
        
        private AddJudiciaryParticipantsRequest BuildValidAddJudiciaryParticipantsRequestWithoutJudge()
        {
            return Builder<AddJudiciaryParticipantsRequest>.CreateNew()
                .With(x => x.Participants = new List<JudiciaryParticipantRequest>
                {
                    new()
                    {
                        PersonalCode = _personalCodePanelMember,
                        DisplayName = "B Panel Member",
                        HearingRoleCode = JudiciaryParticipantHearingRoleCode.PanelMember
                    }
                })
                .Build();
        }
        
        private AddJudiciaryParticipantsRequest BuildInvalidAddJudiciaryParticipantsRequest()
        {
            return Builder<AddJudiciaryParticipantsRequest>.CreateNew()
                .With(x => x.Participants = new List<JudiciaryParticipantRequest>
                {
                    new()
                    {
                        PersonalCode = "",
                        DisplayName = "",
                        HearingRoleCode = JudiciaryParticipantHearingRoleCode.PanelMember
                    }
                })
                .Build();
        }

        private AddJudiciaryParticipantsRequest BuildAddJudiciaryParticipantsRequestWithEmptyParticipants()
        {
            return Builder<AddJudiciaryParticipantsRequest>.CreateNew()
                .With(x => x.Participants = new List<JudiciaryParticipantRequest>())
                .Build();
        }
        
        private AddJudiciaryParticipantsRequest BuildAddJudiciaryParticipantsRequestWithNullParticipants()
        {
            return Builder<AddJudiciaryParticipantsRequest>.CreateNew()
                .Build();
        }
    }
}
