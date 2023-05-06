using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services
{
    public static class JusticeUserDtoMapper
    {
        public static JusticeUserDto MapToDto(JusticeUser user)
        {
            return new JusticeUserDto
            {
                Username = user.Username,
                UserId = user.Id,
                UserRoleName = user.UserRole.Name
            };
        }
    }
}