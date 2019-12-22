﻿using Microsoft.AspNetCore.Authorization;

 namespace Bookings.API.Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string[] DelegatedPermissions { get; set; }
        public string[] ApplicationPermissions { get; set; }
        public string[] UserRoles { get; set; }
    }
}