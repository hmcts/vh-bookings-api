using BookingsApi.Contract.V2.Requests;
using BookingsApi.Contract.V2.Responses;
using BookingsApi.Domain.Constants;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using Testing.Common.Builders.Api.V2;

namespace BookingsApi.IntegrationTests.Api.V2;

public class ParticipantWithoutEmailTests : ApiTest
{
    private readonly List<Guid> _hearingIds = new();
    
    [SetUp]
    public void Setup()
    {
        _hearingIds.Clear();
    }

    [TearDown]
    public new async Task TearDown()
    {
        foreach (var hearingId in _hearingIds)
        {
            await Hooks.RemoveVideoHearing(hearingId);
        }
    }

    [Test]
    public async Task should_book_hearing_without_participant_contact_email()
    {
        // arrange
        var request = await CreateBookingRequestWithServiceIdsAndCodes();
        // remove email from a representative
        var rep = request.Participants.First(x => x.HearingRoleCode == HearingRoleCodes.Representative);
        rep.ContactEmail = null;

        var expectedParticipantInConferenceCount = 3; // 4 participants - 1 without email
        
        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpointsV2.BookNewHearing, RequestBody.Set(request));
        
        // assert
        // double-check the response from the post matches that in the GET
        result.IsSuccessStatusCode.Should().BeTrue(result.Content.ReadAsStringAsync().Result);
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var getHearingUri = result.Headers.Location;
        var getResponse = await client.GetAsync(getHearingUri);
        
        var createdResponse = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(result.Content);
        var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(getResponse.Content);
        _hearingIds.Add(hearingResponse.Id);
        
        createdResponse.Should().BeEquivalentTo(hearingResponse);
        
        // assert participants without a contact email are excluded from the integration events
        var serviceBusStub =
            Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
        
        var messages = serviceBusStub!.ReadAllMessagesFromQueue(hearingResponse.Id);
        Array.Exists(messages, x => x.IntegrationEvent is HearingIsReadyForVideoIntegrationEvent).Should().BeTrue();
        var message = messages.Single(x => x.IntegrationEvent is HearingIsReadyForVideoIntegrationEvent);
        var hearingIsReadyEvent = message.IntegrationEvent as HearingIsReadyForVideoIntegrationEvent;
        
        hearingIsReadyEvent!.Participants.Exists(x => x.ContactEmail == null).Should().BeFalse();
        
        Array.Exists(messages, x => x.IntegrationEvent is NewParticipantWelcomeEmailEvent).Should().BeTrue();


        var welcomeEmailEvents =
            messages.Select(x => x.IntegrationEvent).OfType<NewParticipantWelcomeEmailEvent>().ToList();
        welcomeEmailEvents.Exists(x => x.WelcomeEmail.ContactEmail == null).Should().BeFalse();
        welcomeEmailEvents.Count.Should().Be(expectedParticipantInConferenceCount);
        
        Array.Exists(messages, x => x.IntegrationEvent is NewParticipantHearingConfirmationEvent).Should().BeTrue();
        var newParticipantHearingConfirmationEvents = messages.Select(x => x.IntegrationEvent)
            .OfType<NewParticipantHearingConfirmationEvent>().ToList();

        newParticipantHearingConfirmationEvents.Exists(x => x.HearingConfirmationForParticipant.ContactEmail == null)
            .Should().BeFalse();
        newParticipantHearingConfirmationEvents.Count.Should().Be(expectedParticipantInConferenceCount);
    }

    [Test]
    public async Task should_treat_participant_with_contact_email_added_as_a_new_participant_updateparticipants()
    {
        // arrange
        var serviceBusStub =
            Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;

        // set up a hearing with a participant without an email
        var bookHearingRequest = await CreateBookingRequestWithServiceIdsAndCodes();
        var rep = bookHearingRequest.Participants.First(x => x.HearingRoleCode == HearingRoleCodes.Representative);
        rep.ContactEmail = null;

        using var client = Application.CreateClient();
        var bookHearingResult = await client.PostAsync(ApiUriFactory.HearingsEndpointsV2.BookNewHearing,
            RequestBody.Set(bookHearingRequest));

        var hearing = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(bookHearingResult.Content);
        _hearingIds.Add(hearing.Id);

        foreach (var participant in hearing.Participants.Where(x=> x.ContactEmail is not null))
        {
            await client.PutAsync(
                ApiUriFactory.PersonEndpoints.UpdatePersonUsername(participant.ContactEmail,
                    $"{participant.Id}_user@test.com"), null);
        }
        
        // clear messages created from the booking flow
        serviceBusStub!.ClearMessages();
        
        var newEmail = "adding_an_email@test.com";
        var requests = hearing.Participants.Select(x =>
            new UpdateParticipantRequestV2
            {
                ParticipantId = x.Id,
                DisplayName = x.DisplayName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                OrganisationName = x.Organisation,
                TelephoneNumber = x.TelephoneNumber,
                Title = x.Title,
                ContactEmail = x.ContactEmail ?? newEmail,
                Representee = x.Representee,
                MiddleNames = x.MiddleNames
            }).ToList();

        var updateParticipantRequest = new UpdateHearingParticipantsRequestV2
        {
            ExistingParticipants = requests
        };
        
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateHearingParticipants(hearing.Id),
                RequestBody.Set(updateParticipantRequest));
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);


        var messages = serviceBusStub!.ReadAllMessagesFromQueue(hearing.Id);

        var participantsUpdatedEvents =
            messages.Select(x => x.IntegrationEvent).OfType<HearingParticipantsUpdatedIntegrationEvent>().ToList();
        participantsUpdatedEvents.Count.Should().Be(1);
        participantsUpdatedEvents.Exists(x => x.NewParticipants.Exists(p => p.ContactEmail == newEmail)).Should()
            .BeTrue();
        
        var welcomeEmailEvents = 
                messages.Select(x => x.IntegrationEvent).OfType<NewParticipantWelcomeEmailEvent>().ToList();
        welcomeEmailEvents.Count.Should().Be(1);
        welcomeEmailEvents.Exists(x=> x.WelcomeEmail.ContactEmail == newEmail).Should().BeTrue();
        
        
        var hearingConfirmationEvents = 
            messages.Select(x => x.IntegrationEvent).OfType<NewParticipantHearingConfirmationEvent>().ToList();
        hearingConfirmationEvents.Count.Should().Be(1);
        hearingConfirmationEvents.Exists(x=> x.HearingConfirmationForParticipant.ContactEmail == newEmail).Should().BeTrue();
    }
    
    
    [Test]
    public async Task should_treat_participant_with_contact_email_added_as_a_new_participant_updateparticipantsdetails()
    {
        // arrange
        var serviceBusStub =
            Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;

        // set up a hearing with a participant without an email
        var bookHearingRequest = await CreateBookingRequestWithServiceIdsAndCodes();
        var rep = bookHearingRequest.Participants.First(x => x.HearingRoleCode == HearingRoleCodes.Representative);
        rep.ContactEmail = null;

        using var client = Application.CreateClient();

        var bookHearingResult = await client.PostAsync(ApiUriFactory.HearingsEndpointsV2.BookNewHearing, RequestBody.Set(bookHearingRequest));
        var hearing = await ApiClientResponse.GetResponses<HearingDetailsResponseV2>(bookHearingResult.Content);
        _hearingIds.Add(hearing.Id);

        var participantWithoutEmail = hearing.Participants.First(x => x.ContactEmail == null);
        
        // make a request to update username so that they are not treated as a new person
        foreach (var participant in hearing.Participants.Where(x=> x.ContactEmail is not null))
        {
            await client
                .PutAsync(ApiUriFactory.PersonEndpoints.UpdatePersonUsername(participant.ContactEmail, $"{participant.Id}_user@test.com"), null);
        }
        
        // clear messages created from the booking flow
        serviceBusStub!.ClearMessages();
        
        var newEmail = "adding_an_email@test.com";
        var updateParticipantRequest =
            new UpdateParticipantRequestV2
            {
                ParticipantId = participantWithoutEmail.Id,
                DisplayName = participantWithoutEmail.DisplayName,
                FirstName = participantWithoutEmail.FirstName,
                LastName = participantWithoutEmail.LastName,
                OrganisationName = participantWithoutEmail.Organisation,
                TelephoneNumber = participantWithoutEmail.TelephoneNumber,
                Title = participantWithoutEmail.Title,
                ContactEmail = newEmail,
                Representee = participantWithoutEmail.Representee,
                MiddleNames = participantWithoutEmail.MiddleNames
            };

        await client
            .PatchAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateParticipantDetails(hearing.Id, participantWithoutEmail.Id),
                RequestBody.Set(updateParticipantRequest));

        var messages = serviceBusStub!.ReadAllMessagesFromQueue(hearing.Id);

        var participantsUpdatedEvents =
            messages.Select(x => x.IntegrationEvent).OfType<ParticipantsAddedIntegrationEvent>().ToList();
        participantsUpdatedEvents.Count.Should().Be(1);
        participantsUpdatedEvents.Exists(x => x.Participants.Any(p => p.ContactEmail == newEmail)).Should()
            .BeTrue();
        
        var welcomeEmailEvents = 
                messages.Select(x => x.IntegrationEvent).OfType<NewParticipantWelcomeEmailEvent>().ToList();
        welcomeEmailEvents.Count.Should().Be(1);
        welcomeEmailEvents.Exists(x=> x.WelcomeEmail.ContactEmail == newEmail).Should().BeTrue();
        
        
        var hearingConfirmationEvents = 
            messages.Select(x => x.IntegrationEvent).OfType<NewParticipantHearingConfirmationEvent>().ToList();
        hearingConfirmationEvents.Count.Should().Be(1);
        hearingConfirmationEvents.Exists(x=> x.HearingConfirmationForParticipant.ContactEmail == newEmail).Should().BeTrue();
    }

    private async Task<BookNewHearingRequestV2> CreateBookingRequestWithServiceIdsAndCodes()
    {
        var personalCode = Guid.NewGuid().ToString();
        await Hooks.AddJudiciaryPerson(personalCode);
        var hearingSchedule = DateTime.UtcNow.AddMinutes(5);
        var caseName = "Integration Automated - ParticipantWithoutEmailTests";
        var request = new SimpleBookNewHearingRequestV2(caseName, hearingSchedule, personalCode).Build();
        request.ServiceId = "ZZY1";
        request.HearingVenueCode = "231596";
        return request;
    }
}