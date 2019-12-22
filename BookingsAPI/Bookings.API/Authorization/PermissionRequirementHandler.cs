using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Bookings.API.Authorization
{
    public class PermissionRequirementHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            if (requirement.DelegatedPermissions.Any(p =>
                context.User.HasClaim(Constants.ScopeClaimType, p)))
            {
                if (requirement.UserRoles.Any(r => context.User.HasClaim(ClaimTypes.Role, r)))
                {
                    context.Succeed(requirement);
                }

                //hack since we don't currently support roles.
                context.Succeed(requirement);
            }
            else if (requirement.ApplicationPermissions.Any(p =>
                context.User.HasClaim(ClaimTypes.Role, p)))
            {
                //Caller has one of the allowed application permissions
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}