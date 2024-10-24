using BookingsApi.Contract.V2.Enums;
using ScreeningType = BookingsApi.Contract.V2.Enums.ScreeningType;

namespace BookingsApi.Mappings.V2.Extensions
{
    /// <summary>
    /// Extensions for mapping between domain and contract enums
    /// </summary>
    public static class ContractExtensions
    {
        /// <summary>
        /// Maps a booking status to a contract enum
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static BookingStatusV2 MapToContractEnum(this BookingStatus status)
        {
            return Enum.Parse<BookingStatusV2>(status.ToString());
        }
        
        internal static LinkedParticipantTypeV2 MapToContractEnum(this LinkedParticipantType type)
        {
            return Enum.Parse<LinkedParticipantTypeV2>(type.ToString());
        }
        
        internal static ScreeningType MapToContractEnum(this Domain.Enumerations.ScreeningType type)
        {
            return Enum.Parse<ScreeningType>(type.ToString());
        }
        
        internal static BookingSupplier MapToContractEnum(this VideoSupplier supplier)
        {
            return Enum.Parse<BookingSupplier>(supplier.ToString());
        }
        
        internal static LinkedParticipantType MapToDomainEnum(this LinkedParticipantTypeV2 typeV2)
        {
            return Enum.Parse<LinkedParticipantType>(typeV2.ToString());
        }
        
        internal static Domain.Enumerations.ScreeningType MapToDomainEnum(this ScreeningType type)
        {
            return Enum.Parse<Domain.Enumerations.ScreeningType>(type.ToString());
        }
        
        internal static VideoSupplier MapToDomainEnum(this BookingSupplier supplier)
        {
            return Enum.Parse<VideoSupplier>(supplier.ToString());
        }
    }
}