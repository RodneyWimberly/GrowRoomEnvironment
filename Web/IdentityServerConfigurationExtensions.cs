using System.Collections.Generic;
using System.Linq;
using GrowRoomEnvironment.DataAccess.Core.Constants;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GrowRoomEnvironment.Web
{
    public static class IdentityServerConfigurationExtensions
    {
        public const string ApiName = "growroom_api";
        public const string ApiFriendlyName = "GrowRoomEnvironment.Web API";
        public const string ClientAppClientID = "growroom_spa";
        public const string SwaggerClientID = "swaggerui";

        public static IApplicationBuilder InitializeIdentityServerDatabase(this IApplicationBuilder app)
        {
            using (IServiceScope serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                ConfigurationDbContext context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    context.Clients.AddRange(GetClients().Select(m => m.ToEntity()));
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    context.IdentityResources.AddRange(GetIdentityResources().Select(m => m.ToEntity()));
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    context.ApiResources.AddRange(GetApiResources().Select(m => m.ToEntity()));
                    context.SaveChanges();
                }
            }
            return app;
        }

        public static IIdentityServerBuilder UseInMemoryStore(this IIdentityServerBuilder builder)
        {
            builder.AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(GetIdentityResources())
                .AddInMemoryApiResources(GetApiResources())
                .AddInMemoryClients(GetClients());
            return builder;
        }

        // Identity resources (used by UserInfo endpoint).
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Phone(),
                new IdentityResources.Email(),
                new IdentityResource(Scopes.Roles, new List<string> { JwtClaimTypes.Role })
            };
        }

        // Api resources.
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource(ApiName) {
                    UserClaims = {
                        JwtClaimTypes.Name,
                        JwtClaimTypes.Email,
                        JwtClaimTypes.PhoneNumber,
                        JwtClaimTypes.Role,
                        Claims.Permission
                    }
                }
            };
        }

        // Clients want to access resources.
        public static IEnumerable<Client> GetClients()
        {
            // Clients credentials.
            return new List<Client>
            {
                // http://docs.identityserver.io/en/release/reference/client.html.
                new Client
                {
                    ClientId = IdentityServerConfigurationExtensions.ClientAppClientID,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword, // Resource Owner Password Credential grant.
                    AllowAccessTokensViaBrowser = true,
                    RequireClientSecret = false, // This client does not need a secret to request tokens from the token endpoint.
                    
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId, // For UserInfo endpoint.
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Phone,
                        IdentityServerConstants.StandardScopes.Email,
                        Scopes.Roles,
                        ApiName
                    },
                    AllowOfflineAccess = true, // For refresh token.
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    //AccessTokenLifetime = 900, // Lifetime of access token in seconds.
                    //AbsoluteRefreshTokenLifetime = 7200,
                    //SlidingRefreshTokenLifetime = 900,
                },

                new Client
                {
                    ClientId = SwaggerClientID,
                    ClientName = "Swagger UI",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowAccessTokensViaBrowser = true,
                    RequireClientSecret = false,

                    AllowedScopes = {
                        ApiName
                    }
                }
            };
        }
    }
}
