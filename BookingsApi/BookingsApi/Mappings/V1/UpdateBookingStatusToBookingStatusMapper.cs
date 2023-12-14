using BookingsApi.Contract.V1.Requests.Enums;

namespace BookingsApi.Mappings.V1
{
    public static class UpdateBookingStatusToBookingStatusMapper
    {
        public static BookingStatus Map(UpdateBookingStatus updateBookingStatus, VideoHearing videoHearing)
        {
            var bookingStatus = Enum.Parse<BookingStatus>(updateBookingStatus.ToString());
            
            if (videoHearing.Status == BookingStatus.BookedWithoutJudge && 
                bookingStatus == BookingStatus.Created)
            {
                bookingStatus = BookingStatus.ConfirmedWithoutJudge;
            }
            
            return bookingStatus;
        }
    }
}
