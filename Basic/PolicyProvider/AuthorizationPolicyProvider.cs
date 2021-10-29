using System.Threading.Tasks;
using Basic.Authorizations;
using Basic.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Basic.PolicyProvider
{
    public class AuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
        }

        // Providing extended logic over the BASIC DefaultAuthorizationPolicyProvider flow here
        // Called when me make a call to FirmController
        // policyName = {type}.{value}
        public override Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            foreach (var firmPolicy in FirmHierarchyPolicy.Get())
            {
                if (policyName.StartsWith(firmPolicy))
                {
                    var policy = FirmHierarchyAuthorizationPolicyFactory.Create(policyName);

                    return Task.FromResult(policy);
                }
            }

            return base.GetPolicyAsync(policyName);
        }
    }
}
