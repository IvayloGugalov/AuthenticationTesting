using System.Linq;
using System.Threading.Tasks;
using Basic.Policies;
using Microsoft.AspNetCore.Authorization;

namespace Basic.Authorizations
{
    // Handles the authorization to the requested path
    public class FirmHierarchyAuthorizationHandler : AuthorizationHandler<FirmHierarchyRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, FirmHierarchyRequirement requirement)
        {
            var claimValue = context.User.Claims
                .FirstOrDefault(x => x.Type == FirmHierarchyPolicy.FirmHierarchy)?
                .Value ?? string.Empty;

            if (requirement.LevelName == claimValue)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
