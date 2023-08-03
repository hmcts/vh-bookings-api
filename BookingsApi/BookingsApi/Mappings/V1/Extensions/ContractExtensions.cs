using System;
using BookingsApi.Contract.V1.Enums;

namespace BookingsApi.Mappings.V1.Extensions
{
    public static class ContractExtensions
    {
        public static BookingStatus MapToContractEnum(this Domain.Enumerations.BookingStatus status)
        {
            return Enum.Parse<BookingStatus>(status.ToString());
        }
        
        public static LinkedParticipantType MapToContractEnum(this Domain.Enumerations.LinkedParticipantType type)
        {
            return Enum.Parse<LinkedParticipantType>(type.ToString());
        }
        
        public static Domain.Enumerations.LinkedParticipantType MapToDomainEnum(this LinkedParticipantType type)
        {
            return Enum.Parse<Domain.Enumerations.LinkedParticipantType>(type.ToString());
        }
    }
}