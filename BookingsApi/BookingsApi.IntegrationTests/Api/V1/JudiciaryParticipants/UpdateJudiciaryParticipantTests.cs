using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Validations;
using BookingsApi.Extensions;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using BookingsApi.Validations.V1;
using JudiciaryParticipantHearingRoleCode = BookingsApi.Contract.V1.Requests.Enums.JudiciaryParticipantHearingRoleCode;

namespace BookingsApi.IntegrationTests.Api.V1.JudiciaryParticipants
{
    public class UpdateJudiciaryParticipantTests : ApiTest
    {
        [Test]
        public async Task Should_update_judiciary_judge()
        {
            // Arrange
            var seededHearing = await Hooks.SeedVideoHearingV2(configureOptions: options =>
            {
                options.AddJudge = true;
            });
            var judiciaryJudge = seededHearing.JudiciaryParticipants
                .FirstOrDefault(x => x.HearingRoleCode == Domain.Enumerations.JudiciaryParticipantHearingRoleCode.Judge);
            var personalCode = judiciaryJudge.JudiciaryPerson.PersonalCode;
            var newDisplayName = "New Display Name";

            var request = new UpdateJudiciaryParticipantRequest
            {
                DisplayName = newDisplayName,
                HearingRoleCode = JudiciaryParticipantHearingRoleCode.Judge
            };
            
            // Act
            using var client = Application.CreateClient();
            var result = await client.PatchAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.UpdateJudiciaryParticipant(seededHearing.Id, personalCode), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var hearing = await new GetHearingByIdQueryHandler(db).Handle(new GetHearingByIdQuery(seededHearing.Id));
            var judiciaryParticipants = hearing.JudiciaryParticipants.OrderBy(x => x.DisplayName).ToList();
            judiciaryParticipants.Count.Should().Be(1);
            judiciaryParticipants[0].JudiciaryPersonId.Should().Be(judiciaryJudge.JudiciaryPersonId);
            judiciaryParticipants[0].DisplayName.Should().Be(newDisplayName);
            judiciaryParticipants[0].HearingRoleCode.Should().Be(JudiciaryParticipantHearingRoleCode.Judge.MapToDomainEnum());

            var response = await ApiClientResponse.GetResponses<JudiciaryParticipantResponse>(result.Content);
            response.Should().BeEquivalentTo(request);
            
            var serviceBusStub =
                Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
            var message = serviceBusStub!.ReadMessageFromQueue();
            message.IntegrationEvent.Should()
                .BeEquivalentTo(new ParticipantUpdatedIntegrationEvent(seededHearing.Id, judiciaryParticipants[0]));
        }
        
        [TestCase(JudiciaryParticipantHearingRoleCode.Judge)]
        [TestCase(JudiciaryParticipantHearingRoleCode.PanelMember)]
        public async Task Should_update_judiciary_panel_member(JudiciaryParticipantHearingRoleCode newHearingRoleCode)
        {
            // Arrange
            var seededHearing = await Hooks.SeedVideoHearingV2(configureOptions: options =>
            {
                options.AddJudge = false;
                options.AddPanelMember = true;
            }, status: BookingStatus.ConfirmedWithoutJudge);
            var judiciaryPanelMember = seededHearing.JudiciaryParticipants
                .FirstOrDefault(x => x.HearingRoleCode == Domain.Enumerations.JudiciaryParticipantHearingRoleCode.PanelMember);
            var personalCode = judiciaryPanelMember.JudiciaryPerson.PersonalCode;
            var newDisplayName = "New Display Name";

            var request = new UpdateJudiciaryParticipantRequest
            {
                DisplayName = newDisplayName,
                HearingRoleCode = newHearingRoleCode
            };
            
            // Act
            using var client = Application.CreateClient();
            var result = await client.PatchAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.UpdateJudiciaryParticipant(seededHearing.Id, personalCode), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var hearing = await new GetHearingByIdQueryHandler(db).Handle(new GetHearingByIdQuery(seededHearing.Id));
            var judiciaryParticipants = hearing.JudiciaryParticipants.OrderBy(x => x.DisplayName).ToList();
            judiciaryParticipants.Count.Should().Be(1);
            judiciaryParticipants[0].JudiciaryPersonId.Should().Be(judiciaryPanelMember.JudiciaryPersonId);
            judiciaryParticipants[0].DisplayName.Should().Be(newDisplayName);
            judiciaryParticipants[0].HearingRoleCode.Should().Be(newHearingRoleCode.MapToDomainEnum());

            var response = await ApiClientResponse.GetResponses<JudiciaryParticipantResponse>(result.Content);
            response.Should().BeEquivalentTo(request);
            
            var serviceBusStub =
                Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
            var message = serviceBusStub!.ReadMessageFromQueue();
            message.IntegrationEvent.Should()
                .BeEquivalentTo(new ParticipantUpdatedIntegrationEvent(seededHearing.Id, judiciaryParticipants[0]));
        }

        [Test]
        public async Task Should_return_not_found_when_hearing_does_not_exist()
        {
            // Arrange
            var hearingId = Guid.NewGuid();
            var personalCode = Guid.NewGuid().ToString();
            var newDisplayName = "New Display Name";
            var newHearingRole = JudiciaryParticipantHearingRoleCode.PanelMember;

            var request = new UpdateJudiciaryParticipantRequest
            {
                DisplayName = newDisplayName,
                HearingRoleCode = newHearingRole
            };

            // Act
            using var client = Application.CreateClient();
            var result = await client.PatchAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.UpdateJudiciaryParticipant(hearingId, personalCode), 
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
            var seededHearing = await Hooks.SeedVideoHearingV2();
            var personalCode = Guid.NewGuid().ToString();
            var newDisplayName = "New Display Name";
            var newHearingRole = JudiciaryParticipantHearingRoleCode.Judge;

            var request = new UpdateJudiciaryParticipantRequest
            {
                DisplayName = newDisplayName,
                HearingRoleCode = newHearingRole
            };
            
            // Act
            using var client = Application.CreateClient();
            var result = await client.PatchAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.UpdateJudiciaryParticipant(seededHearing.Id, personalCode), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var response = await ApiClientResponse.GetResponses<string>(result.Content);
            response.Should().Be(DomainRuleErrorMessages.JudiciaryParticipantNotFound);
        }

        [Test]
        public async Task Should_return_bad_request_when_request_is_invalid()
        {
            // Arrange
            var seededHearing = await Hooks.SeedVideoHearingV2();
            var personalCode = Guid.NewGuid().ToString();
            var newDisplayName = "";
            var newHearingRole = JudiciaryParticipantHearingRoleCode.PanelMember;

            var request = new UpdateJudiciaryParticipantRequest
            {
                DisplayName = newDisplayName,
                HearingRoleCode = newHearingRole
            };
            
            // Act
            using var client = Application.CreateClient();
            var result = await client.PatchAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.UpdateJudiciaryParticipant(seededHearing.Id, personalCode), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors["DisplayName"][0].Should().Be(UpdateJudiciaryParticipantRequestValidation.NoDisplayNameErrorMessage);
        }
        
        [Test]
        public async Task Should_return_bad_request_when_request_is_close_to_start_time_and_created()
        {
            // Arrange
            var seededHearing = await Hooks.SeedVideoHearingV2(configureOptions: options =>
            {
                options.ScheduledDate = DateTime.UtcNow.AddMinutes(5);
                options.AddJudge = false;
                options.AddPanelMember = true;
            }, BookingStatus.Created);
            var judiciaryPanelMember = seededHearing.JudiciaryParticipants
                .FirstOrDefault(x => x.HearingRoleCode == Domain.Enumerations.JudiciaryParticipantHearingRoleCode.PanelMember);
            var personalCode = judiciaryPanelMember.JudiciaryPerson.PersonalCode;
            var newDisplayName = "New Display Name";

            var request = new UpdateJudiciaryParticipantRequest
            {
                DisplayName = newDisplayName,
                HearingRoleCode = JudiciaryParticipantHearingRoleCode.Judge
            };
            
            // Act
            using var client = Application.CreateClient();
            var result = await client.PatchAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.UpdateJudiciaryParticipant(seededHearing.Id, personalCode), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors["Hearing"].Should().Contain(DomainRuleErrorMessages.CannotUpdateJudiciaryParticipantCloseToStartTime);
        }

        [Test]
        public async Task Should_return_bad_request_when_changing_panel_member_to_judge_and_judge_already_exists_in_the_hearing()
        {
            // Arrange
            var seededHearing = await Hooks.SeedVideoHearingV2(configureOptions: options =>
            {
                options.AddJudge = true;
                options.AddPanelMember = true;
            });
            var judiciaryPanelMember = seededHearing.JudiciaryParticipants
                .FirstOrDefault(x => x.HearingRoleCode == Domain.Enumerations.JudiciaryParticipantHearingRoleCode.PanelMember);
            var personalCode = judiciaryPanelMember.JudiciaryPerson.PersonalCode;
            var newDisplayName = "New Display Name";
            var newHearingRole = JudiciaryParticipantHearingRoleCode.Judge;

            var request = new UpdateJudiciaryParticipantRequest
            {
                DisplayName = newDisplayName,
                HearingRoleCode = newHearingRole
            };
            
            // Act
            using var client = Application.CreateClient();
            var result = await client.PatchAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.UpdateJudiciaryParticipant(seededHearing.Id, personalCode), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var response = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            response.Errors["personalCode"].Should().Contain("A participant with Judge role already exists in the hearing");
        }

        [Test]
        public async Task Should_return_bad_request_when_changing_judge_to_panel_member_and_judge_is_only_host()
        {
            // Arrange
            var seededHearing = await Hooks.SeedVideoHearingV2(configureOptions: options =>
            {
                options.AddJudge = true;
            });
            var judiciaryJudge = seededHearing.JudiciaryParticipants
                .FirstOrDefault(x => x.HearingRoleCode == Domain.Enumerations.JudiciaryParticipantHearingRoleCode.Judge);
            var personalCode = judiciaryJudge.JudiciaryPerson.PersonalCode;
            var newDisplayName = "New Display Name";
            var newHearingRole = JudiciaryParticipantHearingRoleCode.PanelMember;

            var request = new UpdateJudiciaryParticipantRequest
            {
                DisplayName = newDisplayName,
                HearingRoleCode = newHearingRole
            };
            
            // Act
            using var client = Application.CreateClient();
            var result = await client.PatchAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.UpdateJudiciaryParticipant(seededHearing.Id, personalCode), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var response = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            response.Errors["Host"].Should().Contain(DomainRuleErrorMessages.HearingNeedsAHost);
        }
        
        [Test]
        public async Task Should_update_judiciary_participant_with_interpreter_languages()
        {
            // Arrange
            var seededHearing = await Hooks.SeedVideoHearingV2(configureOptions: options =>
            {
                options.AddJudge = true;
                options.AddPanelMember = true;
            });
            const JudiciaryParticipantHearingRoleCode hearingRoleCode = JudiciaryParticipantHearingRoleCode.Judge;
            var judiciaryParticipant = seededHearing.JudiciaryParticipants
                .FirstOrDefault(x => x.HearingRoleCode == hearingRoleCode.MapToDomainEnum());
            var personalCode = judiciaryParticipant.JudiciaryPerson.PersonalCode;
            const string languageCode = "spa";

            var request = new UpdateJudiciaryParticipantRequest
            {
                DisplayName = judiciaryParticipant.DisplayName,
                HearingRoleCode = hearingRoleCode,
                InterpreterLanguageCode = languageCode
            };
            
            // Act
            using var client = Application.CreateClient();
            var result = await client.PatchAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.UpdateJudiciaryParticipant(seededHearing.Id, personalCode), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var response = await ApiClientResponse.GetResponses<JudiciaryParticipantResponse>(result.Content);
            response.InterpreterLanguageCode.Should().Be(languageCode);
        }

        [Test]
        public async Task Should_update_judiciary_participant_with_other_languages()
        {
            // Arrange
            var seededHearing = await Hooks.SeedVideoHearingV2(configureOptions: options =>
            {
                options.AddJudge = true;
                options.AddPanelMember = true;
            });
            const JudiciaryParticipantHearingRoleCode hearingRoleCode = JudiciaryParticipantHearingRoleCode.Judge;
            var judiciaryParticipant = seededHearing.JudiciaryParticipants
                .FirstOrDefault(x => x.HearingRoleCode == hearingRoleCode.MapToDomainEnum());
            var personalCode = judiciaryParticipant.JudiciaryPerson.PersonalCode;
            const string otherLanguage = "made up";

            var request = new UpdateJudiciaryParticipantRequest
            {
                DisplayName = judiciaryParticipant.DisplayName,
                HearingRoleCode = hearingRoleCode,
                OtherLanguage = otherLanguage
            };
            
            // Act
            using var client = Application.CreateClient();
            var result = await client.PatchAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.UpdateJudiciaryParticipant(seededHearing.Id, personalCode), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var response = await ApiClientResponse.GetResponses<JudiciaryParticipantResponse>(result.Content);
            response.OtherLanguage.Should().Be(otherLanguage);
        }

        [Test]
        public async Task Should_return_validation_error_when_interpreter_language_code_is_not_found()
        {
            // Arrange
            var seededHearing = await Hooks.SeedVideoHearingV2(configureOptions: options =>
            {
                options.AddJudge = true;
                options.AddPanelMember = true;
            });
            const JudiciaryParticipantHearingRoleCode hearingRoleCode = JudiciaryParticipantHearingRoleCode.Judge;
            var judiciaryParticipant = seededHearing.JudiciaryParticipants
                .FirstOrDefault(x => x.HearingRoleCode == hearingRoleCode.MapToDomainEnum());
            var personalCode = judiciaryParticipant.JudiciaryPerson.PersonalCode;
            const string languageCode = "non existing";

            var request = new UpdateJudiciaryParticipantRequest
            {
                DisplayName = judiciaryParticipant.DisplayName,
                HearingRoleCode = hearingRoleCode,
                InterpreterLanguageCode = languageCode
            };
            
            // Act
            using var client = Application.CreateClient();
            var result = await client.PatchAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.UpdateJudiciaryParticipant(seededHearing.Id, personalCode), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors["JudiciaryParticipant"][0].Should().Be($"Language code {languageCode} does not exist");
        }

        [Test]
        public async Task Should_return_validation_error_when_both_interpreter_language_code_and_other_language_are_specified()
        {
            // Arrange
            var seededHearing = await Hooks.SeedVideoHearingV2(configureOptions: options =>
            {
                options.AddJudge = true;
                options.AddPanelMember = true;
            });
            const JudiciaryParticipantHearingRoleCode hearingRoleCode = JudiciaryParticipantHearingRoleCode.Judge;
            var judiciaryParticipant = seededHearing.JudiciaryParticipants
                .FirstOrDefault(x => x.HearingRoleCode == hearingRoleCode.MapToDomainEnum());
            var personalCode = judiciaryParticipant.JudiciaryPerson.PersonalCode;
            const string languageCode = "spa";
            const string otherLanguage = "made up";

            var request = new UpdateJudiciaryParticipantRequest
            {
                DisplayName = judiciaryParticipant.DisplayName,
                HearingRoleCode = hearingRoleCode,
                InterpreterLanguageCode = languageCode,
                OtherLanguage = otherLanguage
            };
            
            // Act
            using var client = Application.CreateClient();
            var result = await client.PatchAsync(
                ApiUriFactory.JudiciaryParticipantEndpoints.UpdateJudiciaryParticipant(seededHearing.Id, personalCode), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors["JudiciaryParticipant"][0].Should().Be(DomainRuleErrorMessages.LanguageAndOtherLanguageCannotBeSet);
        }
    }
}
