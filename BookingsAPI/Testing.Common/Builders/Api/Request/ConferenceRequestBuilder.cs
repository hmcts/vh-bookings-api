using VhListings.Api.Contract.Requests;

namespace Testing.Common.Builders.Api.Request
{
    public static class ConferenceRequestBuilder
    {
        public static AddConferenceDetailsRequest BuildRequest()
        {
            return new AddConferenceDetailsRequest
            {
                MeetingUrl = "https://meet.resources.lync.com/056ruHudmeetings/056ruHud",
                JoinMeetingUrl = "sip://lync.meeting/056ruHudmeetings/056ruHud"
            };
        }
    }
}