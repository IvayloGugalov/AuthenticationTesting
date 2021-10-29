using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Basic.Authorizations
{
    // Request to get authorized by the handler
    public class CustomRequirementClaim : IAuthorizationRequirement
    {
        public string ClaimType { get; }

        public CustomRequirementClaim(string claimType)
        {
            this.ClaimType = claimType;
        }
    }

    // Used by an authorization policy
    public class CustomRequirementClaimHandler : AuthorizationHandler<CustomRequirementClaim>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomRequirementClaim requirement)
        {
            var hasClaim =  context.User.Claims.Any(x => x.Type == requirement.ClaimType);

            if (hasClaim)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public static class CustomRequirementClaimExtensions
    {
        public static AuthorizationPolicyBuilder RequireCustomClaim(this AuthorizationPolicyBuilder builder, string claimType)
        {
            builder.AddRequirements(new CustomRequirementClaim(claimType));

            return builder;
        }
    }
}
