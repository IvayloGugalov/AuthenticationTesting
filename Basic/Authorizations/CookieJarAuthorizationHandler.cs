using System.Threading.Tasks;
using Basic.Controllers;
using Basic.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Basic.Authorizations
{
    public class CookieJarAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement>
    {
        // Handle the cookie jar authorization based on the specified requirement OperationAuthorizationRequirement
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement)
        {
            if (requirement.Name == CookieJarOperations.Open)
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    context.Succeed(requirement);
                }
            }
            else if (requirement.Name == CookieJarOperations.NotAllowed)
            {
                if (!context.User.HasClaim(CookieJarPolicy.MonsterClaim, CookieJarPolicy.CookieMonsterValue))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
