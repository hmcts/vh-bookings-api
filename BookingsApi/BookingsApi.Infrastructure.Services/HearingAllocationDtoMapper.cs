using System.Linq;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services
{
    public static class HearingAllocationDtoMapper
    {
        public static HearingAllocationDto MapToDto(Hearing hearing)
        {
            var hearingDto = HearingDtoMapper.MapToDto(hearing);
            var hearingAllocationDto = (HearingAllocationDto)hearingDto;
            hearingAllocationDto.JudgeDisplayName = hearing.Participants?.FirstOrDefault(p => p.HearingRole.UserRole.IsJudge)?.DisplayName;
            return hearingAllocationDto;
        }
    }
}