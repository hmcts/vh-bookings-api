using System;

namespace BookingsApi.Extensions
{
    public static class ContractExtensions
    {
        public static Contract.Enums.BookingStatus MapToContractEnum(this Domain.Enumerations.BookingStatus status)
        {
            return Enum.Parse<Contract.Enums.BookingStatus>(status.ToString());
        }
        
        public static Contract.Enums.LinkedParticipantType MapToContractEnum(this Domain.Enumerations.LinkedParticipantType type)
        {
            return Enum.Parse<Contract.Enums.LinkedParticipantType>(type.ToString());
        }
        
        public static Domain.Enumerations.LinkedParticipantType MapToDomainEnum(this Contract.Enums.LinkedParticipantType type)
        {
            return Enum.Parse<Domain.Enumerations.LinkedParticipantType>(type.ToString());
        }
    }
}