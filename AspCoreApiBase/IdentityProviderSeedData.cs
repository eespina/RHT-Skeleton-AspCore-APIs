using IdentityModel;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace AspCoreBase
{
    public class IdentityProviderSeedData
    {
        public static IEnumerable<ApiResource> GetApiResourceses() => new List<ApiResource>
            {
                new ApiResource()
                {
                    Name = "sample_api",
                    ApiSecrets = {new Secret("api_secret".Sha256())},
                    Scopes= new List<string>{"sample_api"}
                }
            };

        public static IEnumerable<ApiScope> GetApiScopes() => new List<ApiScope>()
            {
                new ApiScope("sample_api","Access Sample Api")
            };

        public static IEnumerable<Client> Clients => new List<Client>
            {
                new Client
                {
                    ClientId = "sample_client",
                    AllowedGrantTypes = {GrantType.ClientCredentials},
                    ClientSecrets = {new Secret("client_secret".Sha256())},
                    AllowedScopes = {"sample_api"},
                    AccessTokenType = AccessTokenType.Reference,
                    Claims = {new ClientClaim(JwtClaimTypes.Role, "sample_api.admin")}
                }
            };
    }
}