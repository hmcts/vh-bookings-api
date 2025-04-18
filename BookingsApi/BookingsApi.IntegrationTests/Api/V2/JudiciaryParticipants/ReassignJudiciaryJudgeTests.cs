using BookingsApi.Contract.V2.Requests;
using BookingsApi.Contract.V2.Responses;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Validations;
using BookingsApi.Mappings.Common;
using BookingsApi.Validations.V2;

namespace BookingsApi.IntegrationTests.Api.V2.JudiciaryParticipants
{
    public class ReassignJudiciaryJudgeTests : JudiciaryParticipantApiTest
    {
        [Test]
        public async Task Should_reassign_judiciary_judge_when_hearing_has_a_judge()
        {
            // Arrange
            var seededHearing = await Hooks.SeedVideoHearingV2(configureOptions: options =>
            {
                options.AddJudge = true;
            }, status: BookingStatus.Created);
            var personalCodeNewJudge = Guid.NewGuid().ToString();
            await Hooks.AddJudiciaryPerson(personalCode: personalCodeNewJudge);

            var request = new ReassignJudiciaryJudgeRequest
            {
                DisplayName = "DisplayName",
                PersonalCode = personalCodeNewJudge
            };
            
            // Act
            using var client = Application.CreateClient();
            var result = await client.PutAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.ReassignJudiciaryJudge(seededHearing.Id), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var hearing = await new GetHearingByIdQueryHandler(db).Handle(new GetHearingByIdQuery(seededHearing.Id));
            var judiciaryParticipant = hearing.JudiciaryParticipants.FirstOrDefault(x => x.JudiciaryPerson.PersonalCode == personalCodeNewJudge);
            judiciaryParticipant.Should().NotBeNull();
            
            var response = await ApiClientResponse.GetResponses<JudiciaryParticipantResponse>(result.Content);
            response.Should().BeEquivalentTo(new JudiciaryParticipantToResponseMapper().MapJudiciaryParticipantToResponse(judiciaryParticipant));
            
            hearing.Status.Should().Be(BookingStatus.Created);
            AssertEventsPublishedForNewJudiciaryParticipants(hearing, judiciaryParticipant);
        }

        [Test]
        public async Task Should_reassign_judiciary_judge_when_hearing_does_not_have_a_judge()
        {
            // Arrange
            var seededHearing = await Hooks.SeedVideoHearingV2(configureOptions: options =>
            {
                options.AddJudge = false;
            }, status: BookingStatus.ConfirmedWithoutJudge);
            var personalCodeNewJudge = Guid.NewGuid().ToString();
            await Hooks.AddJudiciaryPerson(personalCode: personalCodeNewJudge);

            var request = new ReassignJudiciaryJudgeRequest
            {
                DisplayName = "DisplayName",
                PersonalCode = personalCodeNewJudge
            };
            
            // Act
            using var client = Application.CreateClient();
            var result = await client.PutAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.ReassignJudiciaryJudge(seededHearing.Id), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var hearing = await new GetHearingByIdQueryHandler(db).Handle(new GetHearingByIdQuery(seededHearing.Id));
            var judiciaryParticipant = hearing.JudiciaryParticipants.FirstOrDefault(x => x.JudiciaryPerson.PersonalCode == personalCodeNewJudge);
            judiciaryParticipant.Should().NotBeNull();
            
            var response = await ApiClientResponse.GetResponses<JudiciaryParticipantResponse>(result.Content);
            response.Should().BeEquivalentTo(new JudiciaryParticipantToResponseMapper().MapJudiciaryParticipantToResponse(judiciaryParticipant));

            hearing.Status.Should().Be(BookingStatus.Created);
            AssertEventsPublishedForNewJudiciaryParticipants(hearing, judiciaryParticipant);
        }

        [Test]
        public async Task Should_reassign_judiciary_judge_when_new_judge_is_same_as_old_judge()
        {
            // Arrange
            var seededHearing = await Hooks.SeedVideoHearingV2(configureOptions: options =>
            {
                options.AddJudge = true;
            }, status: BookingStatus.Created);
            var oldJudge = seededHearing.GetJudge();
            var personalCodeNewJudge = oldJudge.JudiciaryPerson.PersonalCode;

            var request = new ReassignJudiciaryJudgeRequest
            {
                DisplayName = "DisplayName",
                PersonalCode = personalCodeNewJudge
            };
            
            // Act
            using var client = Application.CreateClient();
            var result = await client.PutAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.ReassignJudiciaryJudge(seededHearing.Id), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var hearing = await new GetHearingByIdQueryHandler(db).Handle(new GetHearingByIdQuery(seededHearing.Id));
            var judiciaryParticipant = hearing.JudiciaryParticipants.FirstOrDefault(x => x.JudiciaryPerson.PersonalCode == personalCodeNewJudge);
            judiciaryParticipant.Should().NotBeNull();
            
            var response = await ApiClientResponse.GetResponses<JudiciaryParticipantResponse>(result.Content);
            response.Should().BeEquivalentTo(new JudiciaryParticipantToResponseMapper().MapJudiciaryParticipantToResponse(judiciaryParticipant));
            
            hearing.Status.Should().Be(BookingStatus.Created);
            AssertEventsPublishedForNewJudiciaryParticipants(hearing, judiciaryParticipant);
        }

        [Test]
        public async Task Should_return_not_found_when_hearing_does_not_exist()
        {
            // Arrange
            var hearingId = Guid.NewGuid();
            var personalCodeNewJudge = Guid.NewGuid().ToString();
            await Hooks.AddJudiciaryPerson(personalCode: Guid.NewGuid().ToString());
            
            var request = new ReassignJudiciaryJudgeRequest
            {
                DisplayName = "DisplayName",
                PersonalCode = personalCodeNewJudge
            };

            // Act
            using var client = Application.CreateClient();
            var result = await client.PutAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.ReassignJudiciaryJudge(hearingId), 
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
                options.AddJudge = true;
            });
            var personalCodeNewJudge = Guid.NewGuid().ToString();

            var request = new ReassignJudiciaryJudgeRequest
            {
                DisplayName = "DisplayName",
                PersonalCode = personalCodeNewJudge
            };

            // Act
            using var client = Application.CreateClient();
            var result = await client.PutAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.ReassignJudiciaryJudge(seededHearing.Id), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var response = await ApiClientResponse.GetResponses<string>(result.Content);
            response.Should().Be($"Judiciary Person with personal code: {personalCodeNewJudge} does not exist");
        }

        [Test]
        public async Task Should_return_bad_request_when_request_is_invalid()
        {
            // Arrange
            var hearingId = Guid.NewGuid();
            var request = new ReassignJudiciaryJudgeRequest();
            
            // Act
            using var client = Application.CreateClient();
            var result = await client.PutAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.ReassignJudiciaryJudge(hearingId), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors[nameof(request.DisplayName)][0].Should().Be(ReassignJudiciaryJudgeRequestValidation.NoDisplayNameErrorMessage);
            validationProblemDetails.Errors[nameof(request.PersonalCode)][0].Should().Be(ReassignJudiciaryJudgeRequestValidation.NoPersonalCodeErrorMessage);
        }
        
        [Test]
        public async Task Should_reassign_judiciary_judge_with_interpreter_languages()
        {
            // Arrange
            var seededHearing = await Hooks.SeedVideoHearingV2(configureOptions: options =>
            {
                options.AddJudge = true;
            }, status: BookingStatus.Created);
            var personalCodeNewJudge = Guid.NewGuid().ToString();
            const string languageCode = "spa";
            await Hooks.AddJudiciaryPerson(personalCode: personalCodeNewJudge);

            var request = new ReassignJudiciaryJudgeRequest
            {
                DisplayName = "DisplayName",
                PersonalCode = personalCodeNewJudge,
                InterpreterLanguageCode = languageCode
            };
            
            // Act
            using var client = Application.CreateClient();
            var result = await client.PutAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.ReassignJudiciaryJudge(seededHearing.Id), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var response = await ApiClientResponse.GetResponses<JudiciaryParticipantResponse>(result.Content);
            response.Should().NotBeNull();
            response.InterpreterLanguage.Code.Should().Be(request.InterpreterLanguageCode);
        }

        [Test]
        public async Task Should_reassign_judiciary_judge_with_other_languages()
        {
            // Arrange
            var seededHearing = await Hooks.SeedVideoHearingV2(configureOptions: options =>
            {
                options.AddJudge = true;
            }, status: BookingStatus.Created);
            var personalCodeNewJudge = Guid.NewGuid().ToString();
            const string otherLanguage = "made up";
            await Hooks.AddJudiciaryPerson(personalCode: personalCodeNewJudge);

            var request = new ReassignJudiciaryJudgeRequest
            {
                DisplayName = "DisplayName",
                PersonalCode = personalCodeNewJudge,
                OtherLanguage = otherLanguage
            };
            
            // Act
            using var client = Application.CreateClient();
            var result = await client.PutAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.ReassignJudiciaryJudge(seededHearing.Id), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var response = await ApiClientResponse.GetResponses<JudiciaryParticipantResponse>(result.Content);
            response.Should().NotBeNull();
            response.OtherLanguage.Should().Be(request.OtherLanguage);
        }

        [Test]
        public async Task should_return_validation_error_when_interpreter_language_code_is_not_found()
        {
            // Arrange
            var seededHearing = await Hooks.SeedVideoHearingV2(configureOptions: options =>
            {
                options.AddJudge = true;
            }, status: BookingStatus.Created);
            var personalCodeNewJudge = Guid.NewGuid().ToString();
            const string languageCode = "non existing";
            await Hooks.AddJudiciaryPerson(personalCode: personalCodeNewJudge);

            var request = new ReassignJudiciaryJudgeRequest
            {
                DisplayName = "DisplayName",
                PersonalCode = personalCodeNewJudge,
                InterpreterLanguageCode = languageCode
            };
            
            // Act
            using var client = Application.CreateClient();
            var result = await client.PutAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.ReassignJudiciaryJudge(seededHearing.Id), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors["JudiciaryParticipant"][0].Should().Be($"Language code {languageCode} does not exist");
        }

        [Test]
        public async Task should_return_validation_error_when_both_interpreter_language_code_and_other_language_are_specified()
        {
            // Arrange
            var seededHearing = await Hooks.SeedVideoHearingV2(configureOptions: options =>
            {
                options.AddJudge = true;
            }, status: BookingStatus.Created);
            var personalCodeNewJudge = Guid.NewGuid().ToString();
            const string languageCode = "spa";
            const string otherLanguage = "made up";
            await Hooks.AddJudiciaryPerson(personalCode: personalCodeNewJudge);

            var request = new ReassignJudiciaryJudgeRequest
            {
                DisplayName = "DisplayName",
                PersonalCode = personalCodeNewJudge,
                InterpreterLanguageCode = languageCode,
                OtherLanguage = otherLanguage
            };
            
            // Act
            using var client = Application.CreateClient();
            var result = await client.PutAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.ReassignJudiciaryJudge(seededHearing.Id), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors["JudiciaryParticipant"][0].Should().Be(DomainRuleErrorMessages.LanguageAndOtherLanguageCannotBeSet);
        }
    }
}
