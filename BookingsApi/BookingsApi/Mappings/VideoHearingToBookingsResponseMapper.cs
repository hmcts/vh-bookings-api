using BookingsApi.Contract.Responses;
using BookingsApi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.DAL.Helper;
using BookingsApi.Domain.RefData;
using BookingsApi.Extensions;

namespace BookingsApi.Mappings
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
            if (videoHearing.HearingType == null) throw new ArgumentException("Hearing is missing hearing type");
            
            var judgeParticipant = videoHearing.GetParticipants().FirstOrDefault(s => s.HearingRole?.UserRole != null && s.HearingRole.UserRole.Name == "Judge");
            var judgeName = judgeParticipant != null ? judgeParticipant.DisplayName : string.Empty;
            var courtRoomAccount = judgeParticipant != null ? judgeParticipant.Person.Username : string.Empty;
            var allocatedVho = "Not Allocated";
            var isScottishVenue =
                HearingScottishVenueNames.ScottishHearingVenuesList.Any(venueName =>
                    venueName == videoHearing.HearingVenueName);
            if (videoHearing.AllocatedTo == null) {
                if (isScottishVenue || videoHearing.CaseTypeId == 3) // not required if scottish venue or generic type
                {
                    allocatedVho = "Not Required";
                }
            } else {
                allocatedVho = videoHearing.AllocatedTo.ContactEmail;
            }

            var response = new BookingsHearingResponse
            {
                HearingId = videoHearing.Id,
                HearingNumber = @case.Number,
                HearingName = @case.Name,
                ScheduledDuration = videoHearing.ScheduledDuration,
                ScheduledDateTime = videoHearing.ScheduledDateTime,
                HearingTypeName = videoHearing.HearingType.Name,
                CaseTypeName = videoHearing.CaseType.Name,
                CourtAddress = videoHearing.HearingVenueName,
                CourtRoom = videoHearing.HearingRoomName,
                CreatedDate = videoHearing.CreatedDate,
                CreatedBy = videoHearing.CreatedBy,
                LastEditDate = videoHearing.UpdatedDate,
                LastEditBy = videoHearing.UpdatedBy,
                ConfirmedBy = videoHearing.ConfirmedBy,
                ConfirmedDate = videoHearing.ConfirmedDate,
                JudgeName = judgeName,
                Status = videoHearing.Status.MapToContractEnum(),
                QuestionnaireNotRequired = videoHearing.QuestionnaireNotRequired,
                AudioRecordingRequired = videoHearing.AudioRecordingRequired,
                CancelReason = videoHearing.CancelReason,
                GroupId = videoHearing.SourceId,
                CourtRoomAccount = courtRoomAccount,
                AllocatedVho = allocatedVho
            };

            return response;
        }
    }
}