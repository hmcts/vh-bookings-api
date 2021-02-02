using BookingsApi.Contract.Requests.Enums;

namespace BookingsApi.AcceptanceTests.Models
{
    internal static class UpdateBookingStatusRequest
    {
        public static BookingsApi.Contract.Requests.UpdateBookingStatusRequest BuildRequest(UpdateBookingStatus status)
        {
            return new BookingsApi.Contract.Requests.UpdateBookingStatusRequest
            {
                UpdatedBy = Faker.Internet.Email(),
                Status = status,
                CancelReason = "Judge decision"
            };
        }
    }
}