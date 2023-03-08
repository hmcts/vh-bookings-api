using System;
using System.Collections.Generic;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;

namespace BookingsApi.Infrastructure.Services.Dtos
{
    public class UserDto
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string UserRoleName { get; set; }
    }
}