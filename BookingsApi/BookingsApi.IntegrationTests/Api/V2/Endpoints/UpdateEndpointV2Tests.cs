using BookingsApi.Client;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.Dtos;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using ScreeningType = BookingsApi.Contract.V2.Enums.ScreeningType;

namespace BookingsApi.IntegrationTests.Api.V2.Endpoints;

public class UpdateEndpointV2Tests : ApiTest
{
    [Test]
    public async Task should_return_validation_problem_when_updating_with_invalid_data()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(options => { options.Case = new Case("Case1 Num", "Case1 Name"); },
            BookingStatus.Created);

        var endpoint = hearing.Endpoints[0];
        var request = new UpdateEndpointRequestV2()
        {
            DisplayName = ""
        };
        using var client = Application.CreateClient();
        var bookingsApiClient = BookingsApiClient.GetClient(client);
        
        // act
        Func<Task> act = async () => await bookingsApiClient.UpdateEndpointV2Async(hearing.Id, endpoint.Id, request);
        
        // assert
        var exception = await act.Should().ThrowAsync<BookingsApiException>();
        exception.And.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        var bookingsApiException = exception.And.As<BookingsApiException<ValidationProblemDetails>>();
        var errors = bookingsApiException.Result.Errors;
        errors.Should().ContainKey(nameof(request.DisplayName));
        errors[nameof(request.DisplayName)].Should().Contain("'Display Name' must not be empty.");
    }

    [Test]
    public async Task should_return_not_found_when_hearing_does_not_exist()
    {
        // arrange
        var hearingId = Guid.NewGuid();
        var endpointId = Guid.NewGuid();
        var request = new UpdateEndpointRequestV2()
        {
            DisplayName = "Update"
        };
        using var client = Application.CreateClient();
        var bookingsApiClient = BookingsApiClient.GetClient(client);
        
        // act
        Func<Task> act = async () => await bookingsApiClient.UpdateEndpointV2Async(hearingId, endpointId, request);
        
        // assert
        var exception = await act.Should().ThrowAsync<BookingsApiException>();
        exception.And.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        var bookingsApiException = exception.And.As<BookingsApiException>();
        bookingsApiException.Response.Should().Contain($"Hearing {hearingId} does not exist");
    }
    
    [Test]
    public async Task should_return_not_found_when_endpoint_does_not_exist()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(options => { options.Case = new Case("Case1 Num", "Case1 Name"); },
            BookingStatus.Created);

        var endpointId = Guid.NewGuid();
        var request = new UpdateEndpointRequestV2()
        {
            DisplayName = "Update"
        };
        using var client = Application.CreateClient();
        var bookingsApiClient = BookingsApiClient.GetClient(client);
        
        // act
        Func<Task> act = async () => await bookingsApiClient.UpdateEndpointV2Async(hearing.Id, endpointId, request);
        
        // assert
        var exception = await act.Should().ThrowAsync<BookingsApiException>();
        exception.And.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        var bookingsApiException = exception.And.As<BookingsApiException>();
        bookingsApiException.Response.Should().Contain($"Endpoint {endpointId} does not exist");
    }

    [Test]
    public async Task should_publish_roles_for_all_endpoints_when_screening_is_added_for_only_one()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(options => { options.Case = new Case("Case1 Num", "Case1 Name"); },
            BookingStatus.Created);

        var endpoint1 = hearing.Endpoints[0]; // screening added - to have a role of guest
        var endpoint2 = hearing.Endpoints[1]; // no screening - to have role of host
        var screenFrom = hearing.GetParticipants().First(x=> x is Individual);
        
        // admin web will update the endpoint with screening
        var request1 = new UpdateEndpointRequestV2()
        {
            DisplayName = endpoint1.DisplayName,
            Screening = new ScreeningRequest()
            {
                Type = ScreeningType.Specific,
                ProtectedFrom = [screenFrom.ExternalReferenceId]
            }
        };
        
        // admin web will invoke update as well despite no screening or changes
        var request2 = new UpdateEndpointRequestV2()
        {
            DisplayName = endpoint1.DisplayName,
        };
        using var client = Application.CreateClient();
        var bookingsApiClient = BookingsApiClient.GetClient(client);
        
        // act
        await bookingsApiClient.UpdateEndpointV2Async(hearing.Id, endpoint1.Id, request1);
        await bookingsApiClient.UpdateEndpointV2Async(hearing.Id, endpoint2.Id, request2);
        
        var response = await bookingsApiClient.GetHearingDetailsByIdV2Async(hearing.Id);
        response.Should().NotBeNull();
        
        var updatedEndpoint = response.Endpoints.First(x => x.Id == endpoint1.Id);
        updatedEndpoint.DisplayName.Should().Be(request1.DisplayName);
        updatedEndpoint.Screening.Should().NotBeNull();
        
        var serviceBusStub =
            Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
        var messages = serviceBusStub!.ReadAllMessagesFromQueue(hearing.Id);
        Array.Exists(messages, x => x.IntegrationEvent is EndpointUpdatedIntegrationEvent).Should().BeTrue();


        var updateEndpointEvents = messages.Where(x => x.IntegrationEvent is EndpointUpdatedIntegrationEvent)
            .Select(x => x.IntegrationEvent.As<EndpointUpdatedIntegrationEvent>()).ToList();


        var endpoint1UpdatedEvent = updateEndpointEvents.Find(x => x.Sip == endpoint1.Sip);
        endpoint1UpdatedEvent.Role.Should().Be(ConferenceRole.Guest, "Endpoint is screened from a participant");
        
        var endpoint2UpdatedEvent = updateEndpointEvents.Find(x => x.Sip == endpoint2.Sip);
        endpoint2UpdatedEvent.Role.Should().Be(ConferenceRole.Host, "Endpoint is not screened from any participant");
    }
    
    [Test]
    public async Task should_update_endpoint_and_remove_screening()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(options => { options.Case = new Case("Case1 Num", "Case1 Name"); },
            BookingStatus.Created);

        var endpoint = hearing.Endpoints[0];
        var screenFrom = hearing.GetParticipants().First(x=> x is Individual);
        
        var request = new UpdateEndpointRequestV2()
        {
            DisplayName = "Updated",
            Screening = new ScreeningRequest()
            {
                Type = ScreeningType.Specific,
                ProtectedFrom = [screenFrom.ExternalReferenceId]
            }
        };
        using var client = Application.CreateClient();
        var bookingsApiClient = BookingsApiClient.GetClient(client);
        
        // act
        await bookingsApiClient.UpdateEndpointV2Async(hearing.Id, endpoint.Id, request);
        
        var response = await bookingsApiClient.GetHearingDetailsByIdV2Async(hearing.Id);
        response.Should().NotBeNull();
        var updatedEndpoint = response.Endpoints.First(x => x.Id == endpoint.Id);
        updatedEndpoint.DisplayName.Should().Be(request.DisplayName);
        updatedEndpoint.Screening.Should().NotBeNull();
        
        
        // act part 2 (remove screening)
        request.Screening = null;
        await bookingsApiClient.UpdateEndpointV2Async(hearing.Id, endpoint.Id, request);
        
        // assert
        response = await bookingsApiClient.GetHearingDetailsByIdV2Async(hearing.Id);
        response.Should().NotBeNull();
        updatedEndpoint = response.Endpoints.First(x => x.Id == endpoint.Id);
        updatedEndpoint.DisplayName.Should().Be(request.DisplayName);
        updatedEndpoint.Screening.Should().BeNull();
        
        var serviceBusStub =
            Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
        var messages = serviceBusStub!.ReadAllMessagesFromQueue(hearing.Id);
        Array.Exists(messages, x => x.IntegrationEvent is EndpointUpdatedIntegrationEvent).Should().BeTrue();
        
        
        var updateEndpointEvents = messages.Where(x=> x. IntegrationEvent is EndpointUpdatedIntegrationEvent).ToList();
        var firstUpdateEvent = updateEndpointEvents[0].IntegrationEvent as EndpointUpdatedIntegrationEvent;
        firstUpdateEvent!.Role.Should().Be(ConferenceRole.Guest, "Endpoint is screened from a participant");
        
        var secondUpdateEvent = updateEndpointEvents[1].IntegrationEvent as EndpointUpdatedIntegrationEvent;
        secondUpdateEvent!.Role.Should().Be(ConferenceRole.Host, "Endpoint is not screened from any participant");
        
        Array.Exists(messages, x => x.IntegrationEvent is HearingDetailsUpdatedIntegrationEvent).Should().BeTrue();

    }

    [Test(Description = "Ensures that the S&L route matches, rather than repeat the whole test suite")]
    public async Task should_return_validation_problem_when_updating_with_invalid_data_old_route()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(options => { options.Case = new Case("Case1 Num", "Case1 Name"); },
            BookingStatus.Created);

        var endpoint = hearing.Endpoints[0];
        
        var request = new UpdateEndpointRequestV2()
        {
            DisplayName = ""
        };
        using var client = Application.CreateClient();
        
        // act
        var result =
            await client.PatchAsync($"hearings/{hearing.Id}/endpoints/{endpoint.Id}", RequestBody.Set(request));
        
        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await result.Content.ReadAsStringAsync();
        content.Should().Contain("'Display Name' must not be empty.");
    }
}