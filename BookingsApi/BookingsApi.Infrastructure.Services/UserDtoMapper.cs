using System.Linq;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services
{
    public static class UserDtoMapper
    {
        public static UserDto MapToDto(JusticeUser user)
        {
            return new UserDto
            {
                Username = user.Username,
                UserId = user.Id.ToString(),
                UserRoleName = user.UserRole.Name
            };
        }
    }
}