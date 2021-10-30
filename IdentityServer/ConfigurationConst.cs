using System.Collections.Generic;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace IdentityServer
{
    public static class ConfigurationConst
    {
        public static IEnumerable<ApiScope> GetApis() =>
            new List<ApiScope>
            {
                new ApiScope("ApiOne", "Api One"),
                new ApiScope("ApiTwo", "Api Two"),
            };

        public static IEnumerable<IdentityResource> GetIdentityResources() =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        public static IEnumerable<Client> GetClients() =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "client_id",
                    ClientSecrets = { new Secret("client_secret".ToSha256()) },
                    // Machine to machine communication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    // The client can get only this API
                    AllowedScopes = { "ApiOne" },
                    RequireConsent = false
                },
                new Client
                {
                    ClientId = "client_id_mvc",
                    ClientSecrets = { new Secret("client_secret_mvc".ToSha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowOfflineAccess = true,
                    RedirectUris = { "https://localhost:44301/signin-oidc" },
                    // The client can get only these APIs
                    AllowedScopes = 
                    {
                        "ApiOne",
                        "ApiTwo",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,

                    },
                    RequireConsent = false
                }
            };
    }
}
