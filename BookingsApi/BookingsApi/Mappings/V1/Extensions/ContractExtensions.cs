using BookingStatus = BookingsApi.Contract.V1.Enums.BookingStatus;

namespace BookingsApi.Mappings.V1.Extensions
{
    internal static class ContractExtensions
    {
        public static BookingStatus MapToContractEnum(this Domain.Enumerations.BookingStatus status)
        {
            return Enum.Parse<BookingStatus>(status.ToString());
        }
    }
}