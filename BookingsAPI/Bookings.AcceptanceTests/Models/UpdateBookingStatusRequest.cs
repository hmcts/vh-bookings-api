using Bookings.Api.Contract.Requests.Enums;

namespace Bookings.AcceptanceTests.Models
{
    internal static class UpdateBookingStatusRequest
    {
        public static Api.Contract.Requests.UpdateBookingStatusRequest BuildRequest(UpdateBookingStatus status)
        {
            return new Api.Contract.Requests.UpdateBookingStatusRequest
            {
                UpdatedBy = Faker.Internet.Email(),
                Status = status
            };
        }
    }
}