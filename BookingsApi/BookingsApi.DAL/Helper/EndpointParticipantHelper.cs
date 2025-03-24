using BookingsApi.Domain.Participants;

namespace BookingsApi.DAL.Helper;

public static class EndpointParticipantHelper
{
    public static IEnumerable<Participant> CheckAndReturnParticipantsLinkedToEndpoint(List<string> linkedEmails, List<Participant> participants)
    {
        foreach (var email in linkedEmails)
        {
            var defenceAdvocate = participants.Find(x => x.Person.ContactEmail.Equals(email, StringComparison.CurrentCultureIgnoreCase));
            if (defenceAdvocate != null)
                yield return defenceAdvocate;
        }
    }
}