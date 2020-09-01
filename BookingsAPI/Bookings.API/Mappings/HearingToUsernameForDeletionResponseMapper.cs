using System.Linq;
using Bookings.Api.Contract.Responses;
using Bookings.Domain;

namespace Bookings.API.Mappings
{
    public static class HearingToUsernameForDeletionResponseMapper
    {
        public static HearingsByUsernameForDeletionResponse MapToDeletionResponse(Hearing hearing)
        {
            var leadCase = hearing.GetCases().FirstOrDefault(x => x.IsLeadCase) ?? hearing.GetCases().First();
            return new HearingsByUsernameForDeletionResponse
            {
                HearingId = hearing.Id,
                CaseName = leadCase.Name,
                CaseNumber = leadCase.Number,
                ScheduledDateTime = hearing.ScheduledDateTime,
                Venue = hearing.HearingVenueName
            };

        }
    }
}