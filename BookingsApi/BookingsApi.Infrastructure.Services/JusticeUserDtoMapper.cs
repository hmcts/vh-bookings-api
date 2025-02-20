using System.Linq;
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
                FullName = $"{user.FirstName} {user.Lastname}",
                UserId = user.Id.ToString(),
                UserRoles = user.JusticeUserRoles.Select(jur => jur.UserRole.Name).ToArray()
            };
        }
    }
}