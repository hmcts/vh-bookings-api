using BookingsApi.Domain.Participants;

namespace BookingsApi.DAL.Commands;

public class AnonymiseCaseAndParticipantCommand : ICommand
{
    public List<Guid> HearingIds { get; init; }
}

public class AnonymiseCaseAndParticipantCommandHandler(BookingsDbContext context)
    : ICommandHandler<AnonymiseCaseAndParticipantCommand>
{
    public const string AnonymisedNameSuffix = "@email.net";

    public async Task Handle(AnonymiseCaseAndParticipantCommand command)
    {
        var hearingIds = command.HearingIds;

        var videoHearings = await context.VideoHearings
            .Include(vh => vh.HearingCases)
            .ThenInclude(vh => vh.Case)
            .Where(hearing => hearingIds.Contains(hearing.Id))
            .ToListAsync();

        videoHearings = videoHearings
            .Select(r => AnonymiseCaseName(r, hearingIds))
            .ToList();

        context.VideoHearings.UpdateRange(videoHearings);


        var participants = await context.Participants
            .Where(participant => hearingIds.Contains(participant.HearingId))
            .ToListAsync();

        participants = participants
            .Select(AnonymiseParticipantDisplayName)
            .ToList();

        context.Participants.UpdateRange(participants);

        await context.SaveChangesAsync();
    }

    private static Participant AnonymiseParticipantDisplayName(Participant participant)
    {
        var randomString = RandomStringGenerator.GenerateRandomString(9);

        if (!participant.DisplayName.Contains(AnonymisedNameSuffix, StringComparison.InvariantCultureIgnoreCase))
        {
            participant.DisplayName = $"{randomString}{AnonymisedNameSuffix}";
        }

        if (participant is Representative participantAsRepresentative &&
            !string.IsNullOrEmpty(participantAsRepresentative.Representee))
        {
            participantAsRepresentative.Representee = randomString;
        }

        return participant;
    }

    private static VideoHearing AnonymiseCaseName(VideoHearing hearing, IList<Guid> hearingIds)
    {
        hearing.HearingCases.ToList().ForEach(
            hearingCase =>
            {
                if (hearingIds.Any(r => r == hearingCase.HearingId)
                    && !hearingCase.Case.Name.ToLowerInvariant().Contains(AnonymisedNameSuffix))
                {
                    hearingCase.Case.Name =
                        $"{RandomStringGenerator.GenerateRandomString(9)}{AnonymisedNameSuffix}";
                }
            });

        return hearing;
    }
}