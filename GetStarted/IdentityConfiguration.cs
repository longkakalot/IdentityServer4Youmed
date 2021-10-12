using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GetStarted
{
    public class IdentityConfiguration
    {
        public static List<TestUser> TestUsers => new List<TestUser>
        {
            new TestUser
            {
                SubjectId = "1144",
                Username = "tmhtp-youmed",            
                Password = "tmhtp-youmed2021",
                Claims =
                {
                    new Claim(JwtClaimTypes.Name, "BV Tai Mũi Họng TPHCM"),                    
                    new Claim(JwtClaimTypes.WebSite, "http://taimuihongtphcm.vn"),
                }
            }
        };

        public static IEnumerable<IdentityResource> IdentityResources =>    new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

        public static IEnumerable<ApiScope> ApiScopes => new ApiScope[]
        {
            new ApiScope("myApi.read"),
            new ApiScope("myApi.write"),
            new ApiScope("ApiOne.read")
        };

        public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
        {
            new ApiResource("myApi")
            {
                Scopes = new List<string>{ "myApi.read","myApi.write" },
                ApiSecrets = new List<Secret>{ new Secret("supersecret".Sha256()) }
            },
            new ApiResource("ApiOne")
            {
                Scopes = new List<string>{ "ApiOne.read", "ApiOne.write" },
                ApiSecrets = new List<Secret>{ new Secret("supersecret".Sha256()) }
            }
        };

        //Trong ApiResourse sẽ bao gồm một số Scope đã được định nghĩa trong APIScope, nếu các scope trong 
        //ApiResource không giống các scope trong API Scope thì sẽ ko được authorize
        //Để ApiResource được authorize thì các scope trong apiresource phải được allow trong Client,
        //mục AllowScopes

        public static IEnumerable<Client> Clients => new Client[]
        {
            new Client
            {
                ClientId = "youmed_client",
                ClientName = "Youmed Credentials Client",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                ClientSecrets = { new Secret("youmed_secret".Sha256()) },
                AllowedScopes = { "myApi.read", "myApi.write", "ApiOne.read" }
            },
        };
    }
}
