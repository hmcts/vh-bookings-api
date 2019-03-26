using Bookings.Api.Contract.Responses;
using Bookings.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bookings.API.Mappings
{
    public class VideoHearingsToBookingsResponseMapper
    {
        public BookingsResponse MapHearingResponses(List<VideoHearing> videoHearings)
        {
            var hearings = videoHearings.Select(MapHearingResponse).ToList();
            var lastItem = hearings != null && hearings.Any() ? hearings.Last() : null;
            var last_cursor = lastItem != null ? lastItem.CreatedDate.Ticks : 0;

            return new BookingsResponse
            {
                Hearings = GroupBookingsByDate(hearings),
                NextCursor = last_cursor > 0 ? last_cursor.ToString() : "0"
            };
        }

        private BookingsHearingResponse MapHearingResponse(VideoHearing videoHearing)
        {
            var cases = videoHearing.GetCases().FirstOrDefault();
            var participant = videoHearing.GetParticipants().FirstOrDefault(s => s.HearingRole != null && s.HearingRole.UserRole != null && s.HearingRole.UserRole.Name == "Judge");
            var judgeName = participant != null ? participant.DisplayName : "";

            var response = new BookingsHearingResponse
            {
                HearingId = videoHearing.Id,
                HearingNumber = cases != null ? cases.Number : "",
                HearingName = cases != null ? cases.Name : "",
                ScheduledDuration = videoHearing.ScheduledDuration,
                ScheduledDateTime = videoHearing.ScheduledDateTime,
                HearingTypeName = videoHearing.HearingType != null ? videoHearing.HearingType.Name : "",
                CaseTypeName = videoHearing.CaseType != null ? videoHearing.CaseType.Name : "",
                CourtAddress = videoHearing.HearingVenueName,
                CourtRoom = videoHearing.HearingRoomName,
                CreatedDate = videoHearing.CreatedDate,
                CreatedBy = videoHearing.CreatedBy,
                LastEditDate = videoHearing.UpdatedDate,
                LastEditBy = videoHearing.UpdatedBy,
                JudgeName = judgeName,
            };
            return response;
        }

        private List<BookingsByDateResponse> GroupBookingsByDate(List<BookingsHearingResponse> bookingsHearings)
        {
            var result = new List<BookingsByDateResponse>();
            IEnumerable<IGrouping<DateTime, BookingsHearingResponse>> groups = bookingsHearings.GroupBy(p => p.HearingDate).ToList();
            foreach (var groupingBy in groups)
            {
                var booking = new BookingsByDateResponse
                {
                    ScheduledDate = groupingBy.Key,
                    Hearings = new List<BookingsHearingResponse>()
                };
                //iterating through values
                foreach (var item in groupingBy)
                {
                    booking.Hearings.Add(item);
                }
                result.Add(booking);
            }

            return result.OrderBy(x => x.ScheduledDate).ToList();
        }
    }
}