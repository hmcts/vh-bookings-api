using System.Linq;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Domain;

namespace BookingsApi.Mappings.V1
{
    public static class HearingToUsernameForDeletionResponseMapper
    {
        public static HearingsByUsernameForDeletionResponse MapToDeletionResponse(Hearing hearing)
        {
            var leadCase = hearing.GetCases().FirstOrDefault(x => x.IsLeadCase) ?? hearing.GetCases().FirstOrDefault();
            return new HearingsByUsernameForDeletionResponse
            {
                HearingId = hearing.Id,
                CaseName = leadCase?.Name,
                CaseNumber = leadCase?.Number,
                ScheduledDateTime = hearing.ScheduledDateTime,
                Venue = hearing.HearingVenueName
            };

        }
    }
}