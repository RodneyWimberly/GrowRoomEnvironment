using IdentityModel;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;

namespace GrowRoomEnvironment.Web.Helpers
{
    public static class Utilities
    {
          

        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirst(JwtClaimTypes.Subject)?.Value?.Trim();
        }

        public static string[] GetRoles(this ClaimsPrincipal identity)
        {
            return identity.Claims
                .Where(c => c.Type == JwtClaimTypes.Role)
                .Select(c => c.Value)
                .ToArray();
        }
    }
}
