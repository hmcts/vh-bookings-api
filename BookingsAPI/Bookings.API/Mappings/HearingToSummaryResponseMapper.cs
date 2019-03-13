using System.Linq;
using Bookings.Api.Contract.Responses;
using Bookings.Domain;

namespace Bookings.API.Mappings
{
    public class HearingToSummaryResponseMapper
    {
        public HearingSummaryResponse MapHearingToSummaryResponse(Hearing hearing)
        {
            var leadingCase = hearing.GetCases().FirstOrDefault();
            var response = new HearingSummaryResponse
            {
                Id = hearing.Id,
                ScheduledDateTime = hearing.ScheduledDateTime,
                ScheduledDuration = hearing.ScheduledDuration,
                CaseName = leadingCase?.Name,
                CaseNumber = leadingCase?.Number
            };

            return response;
        }
    }
}