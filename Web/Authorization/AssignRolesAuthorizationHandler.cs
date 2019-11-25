using GrowRoomEnvironment.DataAccess.Core;
using GrowRoomEnvironment.DataAccess.Core.Constants;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GrowRoomEnvironment.Web.Authorization
{
    public class AssignRolesAuthorizationHandler : AuthorizationHandler<AssignRolesAuthorizationRequirement, (string[] newRoles, string[] currentRoles)>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AssignRolesAuthorizationRequirement requirement, (string[] newRoles, string[] currentRoles) roles)
        {
            if (!GetIsRolesChanged(roles.newRoles, roles.currentRoles))
            {
                context.Succeed(requirement);
            }
            else if (context.User.HasClaim(Claims.Permission, ApplicationPermissions.AssignRoles))
            {
                if (context.User.HasClaim(Claims.Permission, ApplicationPermissions.ViewRoles)) // If user has ViewRoles permission, then he can assign any roles
                    context.Succeed(requirement);

                else if (GetIsUserInAllAddedRoles(context.User, roles.newRoles, roles.currentRoles)) // Else user can only assign roles they're part of
                    context.Succeed(requirement);
            }


            return Task.CompletedTask;
        }


        private bool GetIsRolesChanged(string[] newRoles, string[] currentRoles)
        {
            if (newRoles == null)
                newRoles = new string[] { };

            if (currentRoles == null)
                currentRoles = new string[] { };


            bool roleAdded = newRoles.Except(currentRoles).Any();
            bool roleRemoved = currentRoles.Except(newRoles).Any();

            return roleAdded || roleRemoved;
        }


        private bool GetIsUserInAllAddedRoles(ClaimsPrincipal contextUser, string[] newRoles, string[] currentRoles)
        {
            if (newRoles == null)
                newRoles = new string[] { };

            if (currentRoles == null)
                currentRoles = new string[] { };


            System.Collections.Generic.IEnumerable<string> addedRoles = newRoles.Except(currentRoles);

            return addedRoles.All(role => contextUser.IsInRole(role));
        }
    }
}