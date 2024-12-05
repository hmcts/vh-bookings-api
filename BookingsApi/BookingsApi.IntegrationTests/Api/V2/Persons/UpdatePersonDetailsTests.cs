using BookingsApi.Client;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BookingsApi.IntegrationTests.Api.V2.Persons
{
    public class UpdatePersonDetailsTests : ApiTest
    {
        [Test]
        public async Task should_return_bad_request_when_request_is_invalid()
        {
            // arrange
            var personId = Guid.NewGuid();
            var request = new UpdatePersonDetailsRequestV2();
            
            // act
            using var client = Application.CreateClient();
            var result = await client
                .PutAsync(ApiUriFactory.PersonEndpointsV2.UpdatePersonDetails(personId), RequestBody.Set(request));


            // assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors.SelectMany(x => x.Value).Should()
                .Contain($"Username is required");
        }

        [Test]
        public async Task should_update_a_person_details_and_publish_updates_to_all_booked_and_non_anonymised_hearings()
        {
            // arrange
            var bookingsApiClient = BookingsApiClient
                .GetClient(Application.CreateClient(new WebApplicationFactoryClientOptions()));
            var hearing1 = await Hooks.SeedVideoHearingV2(options => options.AddJudge = true, status:BookingStatus.Created);
            var participant = hearing1.Participants.First(x => x is Individual); 
            var person = participant.Person;
            
            var hearing2 = await Hooks.SeedVideoHearingV2(options => options.AddJudge = false, status:BookingStatus.Created);
            var added = await bookingsApiClient.AddParticipantsToHearingV2Async(hearing2.Id, new AddParticipantsToHearingRequestV2
            {
                Participants =
                [
                    new()
                    {
                        ContactEmail = person.ContactEmail,
                        FirstName = person.FirstName,
                        LastName = person.LastName,
                        DisplayName = participant.DisplayName,
                        HearingRoleCode = participant.HearingRole.Code,
                    }
                ]
            });
            added.Should().Contain(x=> x.ContactEmail == person.ContactEmail);
            var serviceBusStub =
                Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
            serviceBusStub!.ClearMessages();
            
            var request = new UpdatePersonDetailsRequestV2
            {
                Username = "new-username@test.com",
                LastName = "Updated Auto test",
                FirstName = "Updated Auto test",
            };
            
            // act
            await bookingsApiClient.UpdatePersonDetailsV2Async(person.Id, request);
            
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var updatedPerson = await db.Persons.FindAsync(person.Id);
            updatedPerson!.Username.Should().Be("new-username@test.com");
            updatedPerson.LastName.Should().Be("Updated Auto test");
            updatedPerson.FirstName.Should().Be("Updated Auto test");
            
            var hearing1Messages = serviceBusStub!.ReadAllMessagesFromQueue(hearing1.Id);
            Array.Exists(hearing1Messages, x => x.IntegrationEvent is ParticipantUpdatedIntegrationEvent).Should().BeTrue();
            
            var hearing2Messages = serviceBusStub!.ReadAllMessagesFromQueue(hearing2.Id);
            Array.Exists(hearing2Messages, x => x.IntegrationEvent is ParticipantUpdatedIntegrationEvent).Should()
                .BeFalse("Only confirmed hearings with a judge should publish updated");
        }
    }
}
