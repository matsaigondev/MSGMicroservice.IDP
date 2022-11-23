using System.Collections.Generic;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace MSGMicroservice.IDP
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(), //co the them
                new IdentityResources.Email(),
                new IdentityResource
                {
                    Name = "roles",
                    UserClaims = new List<string> {"roles"}
                },
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new("msg_microservice_api.read", "MSG Microservice API Read Scope"),
                new("msg_microservice_api.write", "MSG Microservice API Write Scope")
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource("msg_microservice_api", "MSG Microservice API")
                {
                    Scopes = new List<string> {"msg_microservice_api.read", "msg_microservice_api.write"},
                    UserClaims = new List<string> {"roles"}
                },
                //new ApiResource("msgApi", "MSG Microservices Web Client"),
                // new ApiResource("msg_microservice_api", "MSG Microservice API")
                // {
                //     Scopes = new List<string> { "msg_microservice_api.read" }
                // }
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new()
                {
                    ClientName = "MSG Microservices Swagger Client",
                    ClientId = "msg_microservice_swagger",

                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    AccessTokenLifetime = 60 * 60 * 2, //2h

                    RedirectUris = new List<string>()
                    {
                        "http://localhost:5001/swagger/oauth2-redirect.html",
                        "http://localhost:5006/swagger/oauth2-redirect.html",
                        "http://localhost:5002/swagger/oauth2-redirect.html",
                        "http://localhost:5020/swagger/oauth2-redirect.html",

                         "https://localhost:6001/swagger/oauth2-redirect.html",
                        //"https://localhost:6006/swagger/oauth2-redirect.html",
                        "https://localhost:6009/swagger/oauth2-redirect.html",
                        "https://10.120.2.44:6020/swagger/oauth2-redirect.html"
                    },
                    PostLogoutRedirectUris = new List<string>()
                    {
                        "http://localhost:5001/swagger/oauth2-redirect.html",
                        "http://localhost:5006/swagger/oauth2-redirect.html",
                        // "http://localhost:5002/swagger/oauth2-redirect.html",
                        // "http://localhost:5020/swagger/oauth2-redirect.html",
                        
                         "https://localhost:6001/swagger/oauth2-redirect.html",
                        //"https://localhost:6006/swagger/oauth2-redirect.html",
                        "https://localhost:6009/swagger/oauth2-redirect.html",
                        "https://10.120.2.44:6020/swagger/oauth2-redirect.html"
                    },
                    AllowedCorsOrigins = new List<string>()
                    {
                        "http://localhost:5001",
                        "http://localhost:5006",
                        // "http://localhost:5020",
                        
                         "https://localhost:6001",
                        "https://localhost:6006",
                        "https://localhost:6009",
                        "https://10.120.2.44:6020"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "roles",
                        "msg_microservice_api.read",
                        "msg_microservice_api.write",
                    }
                },
                new()
                {
                    ClientName = "MSG Microservices Postman Client",
                    ClientId = "msg_microservice_postman",
                    Enabled = true,
                    ClientUri = null,
                    RequireClientSecret = true,
                    ClientSecrets = new[]
                    {
                        new Secret("SuperStrongSecret".Sha512())
                    },
                    AllowedGrantTypes = new[]
                    {
                        GrantType.ClientCredentials,
                        GrantType.ResourceOwnerPassword
                    },
                    RequireConsent = false,
                    AccessTokenLifetime = 60 * 60 * 2, //2h
                    AllowOfflineAccess = true,
                    // RedirectUris = new List<string>()
                    // {
                    //     "https://www.getpostman.com/oauth2/callback"
                    // },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "roles",
                        "msg_microservice_api.read",
                        "msg_microservice_api.write",
                    }
                },
                
            };
    }
}