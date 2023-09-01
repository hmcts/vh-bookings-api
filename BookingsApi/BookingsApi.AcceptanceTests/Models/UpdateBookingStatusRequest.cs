using BookingsApi.Contract.V1.Requests.Enums;

namespace BookingsApi.AcceptanceTests.Models
{
    internal static class UpdateBookingStatusRequest
    {
        public static Contract.V1.Requests.UpdateBookingStatusRequest BuildRequest(UpdateBookingStatus status)
        {
            return new Contract.V1.Requests.UpdateBookingStatusRequest
            {
                UpdatedBy = $"{Faker.RandomNumber.Next()}@hmcts.net",
                Status = status,
                CancelReason = "Judge decision"
            };
        }
    }
}