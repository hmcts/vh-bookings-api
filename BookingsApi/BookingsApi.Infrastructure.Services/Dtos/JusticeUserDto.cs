﻿using System;
using System.Collections.Generic;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;

namespace BookingsApi.Infrastructure.Services.Dtos
{
    public class JusticeUserDto
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string[] UserRoles { get; set; }
    }
}