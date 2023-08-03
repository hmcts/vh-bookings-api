using System;
using BookingsApi.Contract.V2.Enums;

namespace BookingsApi.Mappings.V2.Extensions
{
    public static class ContractExtensions
    {
        public static BookingStatusV2 MapToContractEnum(this Domain.Enumerations.BookingStatus status)
        {
            return Enum.Parse<BookingStatusV2>(status.ToString());
        }
        
        public static LinkedParticipantTypeV2 MapToContractEnum(this Domain.Enumerations.LinkedParticipantType type)
        {
            return Enum.Parse<LinkedParticipantTypeV2>(type.ToString());
        }
        
        public static Domain.Enumerations.LinkedParticipantType MapToDomainEnum(this LinkedParticipantTypeV2 typeV2)
        {
            return Enum.Parse<Domain.Enumerations.LinkedParticipantType>(typeV2.ToString());
        }
    }
}