using BookingsApi.Contract.V1.Responses;

namespace BookingsApi.Mappings.V1
{
    public class AudioRecordedHearingsBySearchResponseMapper
    {
        private const string ErrorMessage = "Hearing is missing case";

        public List<AudioRecordedHearingsBySearchResponse> MapHearingToDetailedResponse(IEnumerable<Hearing> videoHearing,
            string caseNumber)
        {
            if (videoHearing == null || !videoHearing.Any()) return new List<AudioRecordedHearingsBySearchResponse>();

            var response = new List<AudioRecordedHearingsBySearchResponse>();
            foreach (var hearing in videoHearing)
            {
                var judgeParticipant = hearing.GetParticipants()
                    .FirstOrDefault(s => s is Judge);

                var courtroomAccountName = judgeParticipant != null 
                    ? judgeParticipant.DisplayName 
                    : string.Empty;
                var courtroomAccount = (judgeParticipant?.Person != null) 
                    ? judgeParticipant.Person.Username 
                    : string.Empty;

                var @case = string.IsNullOrEmpty(caseNumber)
                    ? hearing.GetCases().FirstOrDefault()
                    : hearing.GetCases()
                        .FirstOrDefault(c => c.Number.ToLower().Trim().Contains(caseNumber.ToLower().Trim()));

                if (@case == null) throw new ArgumentException(ErrorMessage);

                var hearingByCaseNumber = new AudioRecordedHearingsBySearchResponse()
                {
                    Id = hearing.Id,
                    ScheduledDateTime = hearing.ScheduledDateTime,
                    HearingVenueName = hearing.HearingVenue.Name,
                    HearingRoomName = hearing.HearingRoomName,
                    CourtroomAccount = courtroomAccount,
                    CourtroomAccountName = courtroomAccountName,
                    CaseName = @case.Name,
                    CaseNumber = @case.Number,
                    GroupId = hearing.SourceId
                };
                hearingByCaseNumber.TrimAllStringsRecursively();
                response.Add(hearingByCaseNumber);
            }

            return response;
        }
    }
}
