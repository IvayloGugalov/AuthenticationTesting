using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Basic.Policies
{
    public static class FirmHierarchyPolicy
    {
        public static IEnumerable<string> Get()
        {
            yield return FirmHierarchy;
            yield return FirmRank;
        }
        public const string FirmHierarchy = "HierarchyLevel";
        public const string FirmRank = "Rank";
    }

    public static class FirmHierarchyAuthorizationPolicyFactory
    {
        // Create the Authorization policy for the specified claim
        public static AuthorizationPolicy Create(string policyName)
        {
            var parts = policyName.Split('.');
            var type = parts.First();
            var value = parts.Last();

            switch (type)
            {
                case FirmHierarchyPolicy.FirmRank:
                    return new AuthorizationPolicyBuilder()
                        .RequireClaim(nameof(FirmHierarchyPolicy.FirmRank), value)
                        .Build();

                case FirmHierarchyPolicy.FirmHierarchy:
                    return new AuthorizationPolicyBuilder()
                        .AddRequirements(new FirmHierarchyRequirement(value))
                        .Build();

                default:
                    return new AuthorizationPolicyBuilder().Build();
            }
        }
    }

    public class FirmHierarchyRequirement : IAuthorizationRequirement
    {
        public string LevelName { get; }

        public FirmHierarchyRequirement(string levelName)
        {
            this.LevelName = levelName;
        }
    }

    public class FirmHierarchyAttribute : AuthorizeAttribute
    {
        public FirmHierarchyAttribute(string levelName)
        {
            Policy = $"{FirmHierarchyPolicy.FirmHierarchy}.{levelName}";
        }
    }
}
