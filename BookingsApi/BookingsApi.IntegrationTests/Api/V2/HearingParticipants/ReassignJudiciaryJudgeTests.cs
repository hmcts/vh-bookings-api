using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V2.Enums;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain.Constants;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.Validations;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;

namespace BookingsApi.IntegrationTests.Api.V2.HearingParticipants;

public class ReassignJudiciaryJudgeTests : ApiTest
{
    private string _personalCodeJudge;

    [SetUp]
    public void Setup()
    {
        _personalCodeJudge = Guid.NewGuid().ToString();
    }

    [Test]
    public async Task Should_change_judiciary_participants_and_notify_newjudge()
    {
        // Arrange
        var seededHearing = await Hooks.SeedVideoHearingV2(configureOptions: options =>
        {
            options.AddJudge = true;
            options.AddPanelMember = true;
        }, BookingStatus.Created);
        var judiciaryPersonJudge = await Hooks.AddJudiciaryPerson(personalCode: _personalCodeJudge);

        var originalJudge = seededHearing.JudiciaryParticipants
            .Where(x => x.HearingRoleCode == Domain.Enumerations.JudiciaryParticipantHearingRoleCode.Judge)
            .FirstOrDefault();

        var request = BuildReassignJudiciaryJudgeRequest(judiciaryPersonJudge);

        // Act
        using var client = Application.CreateClient();
        var result = await client.PutAsync(
            ApiUriFactory.JudiciaryParticipantEndpoints.ReassignJudiciaryJudge(seededHearing.Id),
            RequestBody.Set(request));

        // Assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        await using var db = new BookingsDbContext(BookingsDbContextOptions);
        var updatedHearing = await new GetHearingByIdQueryHandler(db).Handle(new GetHearingByIdQuery(seededHearing.Id));
        var newJudge = updatedHearing.JudiciaryParticipants.FirstOrDefault(x =>
            x.HearingRoleCode == Domain.Enumerations.JudiciaryParticipantHearingRoleCode.Judge);

        newJudge.JudiciaryPersonId.Should().NotBe(originalJudge.JudiciaryPersonId);
        newJudge.JudiciaryPersonId.Should().Be(judiciaryPersonJudge.Id);

        var serviceBusStub =
            Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
        var messages = serviceBusStub!.ReadAllMessagesFromQueue(updatedHearing.Id);
        messages.Length.Should().Be(4);
        var judgeMessage = messages.SingleOrDefault(x => x.IntegrationEvent is HearingNotificationIntegrationEvent &&
                                                         ((HearingNotificationIntegrationEvent) x.IntegrationEvent)
                                                         .HearingConfirmationForParticipant.UserRole == "Judge");
        judgeMessage.Should().NotBeNull();
    }
    private static ReassignJudiciaryJudgeRequest BuildReassignJudiciaryJudgeRequest(JudiciaryPerson judge)
    {
        return new ReassignJudiciaryJudgeRequest()
        {
            PersonalCode = judge.PersonalCode,
            DisplayName = $"{judge.KnownAs} {judge.Surname}",
            OptionalContactEmail = judge.Email,
            OptionalContactTelephone = judge.WorkPhone
        };
    }
}