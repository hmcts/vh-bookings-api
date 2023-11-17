using BookingsApi.Contract.V1.Responses;
using BookingsApi.Domain.Helper;
using BookingsApi.Mappings.V1.Extensions;

namespace BookingsApi.Mappings.V1
{
    public class VideoHearingsToBookingsResponseMapper
    {
        public List<BookingsByDateResponse> MapHearingResponses(IEnumerable<VideoHearing> videoHearings)
        {
            var mapped = videoHearings.Select(MapHearingResponse);

            return mapped
                .GroupBy(hearing => hearing.ScheduledDateTime.Date)
                .Select(group => new BookingsByDateResponse
                {
                    Hearings = group.ToList(),
                    ScheduledDate = group.Key
                })
                .OrderBy(x => x.ScheduledDate)
                .ToList();
        }

        public BookingsHearingResponse MapHearingResponse(VideoHearing videoHearing)
        {   
            var @case = videoHearing.GetCases().FirstOrDefault();
            if (@case == null) throw new ArgumentException("Hearing is missing case");
            
            if (videoHearing.CaseType == null) throw new ArgumentException("Hearing is missing case type");

            var judge = videoHearing.GetJudge();
            var courtRoomAccount = string.Empty;
            if (judge is Judge judgeParticipant)
            {
                courtRoomAccount = judgeParticipant.Person.Username;
            }
            var judgeName = judge?.DisplayName ?? string.Empty;
            var allocatedVho = VideoHearingHelper.AllocatedVho(videoHearing);

            var response = new BookingsHearingResponse
            {
                HearingId = videoHearing.Id,
                HearingNumber = @case.Number,
                HearingName = @case.Name,
                ScheduledDuration = videoHearing.ScheduledDuration,
                ScheduledDateTime = videoHearing.ScheduledDateTime,
                HearingTypeName = videoHearing.HearingType?.Name,
                CaseTypeName = videoHearing.CaseType.Name,
                CourtAddress = videoHearing.HearingVenue.Name,
                CourtRoom = videoHearing.HearingRoomName,
                CreatedDate = videoHearing.CreatedDate,
                CreatedBy = videoHearing.CreatedBy,
                LastEditDate = videoHearing.UpdatedDate,
                LastEditBy = videoHearing.UpdatedBy,
                ConfirmedBy = videoHearing.ConfirmedBy,
                ConfirmedDate = videoHearing.ConfirmedDate,
                JudgeName = judgeName,
                Status = videoHearing.Status.MapToContractEnum(),
                AudioRecordingRequired = videoHearing.AudioRecordingRequired,
                CancelReason = videoHearing.CancelReason,
                GroupId = videoHearing.SourceId,
                CourtRoomAccount = courtRoomAccount,
                AllocatedTo = allocatedVho
            };

            return response;
        }
    }
}