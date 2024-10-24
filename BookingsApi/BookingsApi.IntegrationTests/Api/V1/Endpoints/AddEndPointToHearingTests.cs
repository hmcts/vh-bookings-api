using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Validations;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using BookingsApi.Validations.V1;

namespace BookingsApi.IntegrationTests.Api.V1.Endpoints;

public class AddEndPointToHearingTests : ApiTest
{
    
    [Test]
    public async Task should_return_bad_request_when_an_invalid_hearing_id_is_provided()
    {
        // arrange
        var hearingId = Guid.Empty;
        var request = new AddEndpointRequest()
        {
            DisplayName = "Add Endpoint Test",
            DefenceAdvocateContactEmail = null
        };
        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.JVEndPointEndpoints.AddEndpointToHearing(hearingId), RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors["hearingId"][0].Should().Be($"Please provide a valid hearingId");
    }
    
    [Test]
    public async Task should_return_bad_request_when_an_invalid_payload_is_provided()
    {
        // arrange
        var hearingId = Guid.NewGuid();
        var request = new AddEndpointRequest()
        {
            DisplayName = string.Empty,
            DefenceAdvocateContactEmail = null
        };
        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.JVEndPointEndpoints.AddEndpointToHearing(hearingId), RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[nameof(request.DisplayName)][0].Should()
            .Be(AddEndpointRequestValidation.NoDisplayNameError);
    }

    [Test]
    public async Task should_return_not_found_when_adding_an_endpoint_to_a_hearing_that_does_not_exist()
    {
        // arrange
        var hearingId = Guid.NewGuid();
        var request = new AddEndpointRequest()
        {
            DisplayName = "Auto Add Endpoint",
            DefenceAdvocateContactEmail = null
        };
        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.JVEndPointEndpoints.AddEndpointToHearing(hearingId), RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var response = await ApiClientResponse.GetResponses<string>(result.Content);
        response.Should().Be($"Hearing {hearingId} does not exist");
    }

    [Test]
    public async Task should_add_endpoint_with_a_defence_advocate_and_publish_message_when_hearing_is_created()
    {
        // arrange
        var seededHearing = await Hooks.SeedVideoHearing(null, BookingStatus.Created);
        var rep = seededHearing.GetParticipants().First(x => x.HearingRole.UserRole.IsRepresentative);
        var hearingId = seededHearing.Id;
        var request = new AddEndpointRequest()
        {
            DisplayName = "Auto Add Endpoint",
            DefenceAdvocateContactEmail = rep.Person.ContactEmail
        };

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.JVEndPointEndpoints.AddEndpointToHearing(hearingId), RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        
        
        await using var db = new BookingsDbContext(BookingsDbContextOptions);
        var hearingFromDb = db.VideoHearings.Include(x => x.Endpoints).ThenInclude(x => x.DefenceAdvocate)
            .ThenInclude(x => x.Person).Include(x => x.Participants).AsNoTracking().First(x => x.Id == hearingId);
        var endpoint = hearingFromDb.Endpoints.First(x=>x.DisplayName == request.DisplayName);

        var createdResponse = await ApiClientResponse.GetResponses<EndpointResponse>(result.Content);
        createdResponse.Should().BeEquivalentTo(new EndpointResponse
        {
            DisplayName = endpoint.DisplayName,
            DefenceAdvocateId = rep.Id,
            Id = endpoint.Id,
            Pin = endpoint.Pin,
            Sip = endpoint.Sip
        });
        var serviceBusStub = Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
        var message = serviceBusStub!.ReadMessageFromQueue();
        message.IntegrationEvent.Should().BeEquivalentTo(new EndpointAddedIntegrationEvent(seededHearing,endpoint));
    }

    [Test]
    public async Task should_add_endpoint_with_a_defence_advocate_and_not_publish_message_when_hearing_is_not_created()
    {
        // arrange
        var seededHearing = await Hooks.SeedVideoHearing(status: BookingStatus.Booked);
        var rep = seededHearing.GetParticipants().First(x => x.HearingRole.UserRole.IsRepresentative);
        var hearingId = seededHearing.Id;
        var request = new AddEndpointRequest()
        {
            DisplayName = "Auto Add Endpoint",
            DefenceAdvocateContactEmail = rep.Person.ContactEmail
        };

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.JVEndPointEndpoints.AddEndpointToHearing(hearingId), RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var serviceBusStub = Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
        var message = serviceBusStub!.ReadMessageFromQueue();
        message.Should().BeNull();
    }
    
    [Test]
    public async Task should_add_endpoint_with_interpreter_languages()
    {
        // arrange
        var seededHearing = await Hooks.SeedVideoHearing(null, BookingStatus.Created);
        var hearingId = seededHearing.Id;
        const string languageCode = "spa";
        var request = new AddEndpointRequest
        {
            DisplayName = "Auto Add Endpoint",
            InterpreterLanguageCode = languageCode
        };
        
        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.JVEndPointEndpoints.AddEndpointToHearing(hearingId), RequestBody.Set(request));
        
        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var response = await ApiClientResponse.GetResponses<EndpointResponse>(result.Content);
        response.Should().NotBeNull();
        response.InterpreterLanguageCode.Should().Be(languageCode);
    }

    [Test]
    public async Task should_add_endpoint_with_other_languages()
    {
        // arrange
        var seededHearing = await Hooks.SeedVideoHearing(null, BookingStatus.Created);
        var hearingId = seededHearing.Id;
        const string otherLanguage = "made up";
        var request = new AddEndpointRequest
        {
            DisplayName = "Auto Add Endpoint",
            OtherLanguage = otherLanguage
        };
        
        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.JVEndPointEndpoints.AddEndpointToHearing(hearingId), RequestBody.Set(request));
        
        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var response = await ApiClientResponse.GetResponses<EndpointResponse>(result.Content);
        response.Should().NotBeNull();
        response.OtherLanguage.Should().Be(otherLanguage);
    }

    [Test]
    public async Task should_return_validation_error_when_interpreter_language_code_is_not_found()
    {
        // arrange
        var seededHearing = await Hooks.SeedVideoHearing(null, BookingStatus.Created);
        var hearingId = seededHearing.Id;
        const string languageCode = "non existing";
        var request = new AddEndpointRequest
        {
            DisplayName = "Auto Add Endpoint",
            InterpreterLanguageCode = languageCode
        };
        
        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.JVEndPointEndpoints.AddEndpointToHearing(hearingId), RequestBody.Set(request));
        
        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);

        validationProblemDetails.Errors["Endpoint"][0].Should()
            .Be($"Language code {languageCode} does not exist");
    }

    [Test]
    public async Task should_return_validation_error_when_both_interpreter_language_code_and_other_language_are_specified()
    {
        // arrange
        var seededHearing = await Hooks.SeedVideoHearing(null, BookingStatus.Created);
        var hearingId = seededHearing.Id;
        var request = new AddEndpointRequest
        {
            DisplayName = "Auto Add Endpoint",
            InterpreterLanguageCode = "fra",
            OtherLanguage = "French"
        };
        
        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.JVEndPointEndpoints.AddEndpointToHearing(hearingId), RequestBody.Set(request));
        
        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);

        validationProblemDetails.Errors["Endpoint"][0].Should()
            .Be(DomainRuleErrorMessages.LanguageAndOtherLanguageCannotBeSet);
    }
}