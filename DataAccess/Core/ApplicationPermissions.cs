using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GrowRoomEnvironment.DataAccess.Core
{
    public static class ApplicationPermissions
    {
        public static ReadOnlyCollection<ApplicationPermission> AllPermissions;

        public const string LogsPermissionGroupName = "Log Permissions";
        public static ApplicationPermission ViewLogs = new ApplicationPermission("View Events", "logs.view", LogsPermissionGroupName, "Permission to view log details");
        public static ApplicationPermission ManageLogs = new ApplicationPermission("Manage Events", "logs.manage", LogsPermissionGroupName, "Permission to create, delete and modify log details");

        public const string EventsPermissionGroupName = "Event Permissions";
        public static ApplicationPermission ViewEvents = new ApplicationPermission("View Events", "events.view", EventsPermissionGroupName, "Permission to view event details");
        public static ApplicationPermission ManageEvents = new ApplicationPermission("Manage Events", "events.manage", EventsPermissionGroupName, "Permission to create, delete and modify event details");
        public static ApplicationPermission ExecuteEvents = new ApplicationPermission("Execute Events", "events.execute", EventsPermissionGroupName, "Permission to execute events");

        public const string UsersPermissionGroupName = "User Permissions";
        public static ApplicationPermission ViewUsers = new ApplicationPermission("View Users", "users.view", UsersPermissionGroupName, "Permission to view other users account details");
        public static ApplicationPermission ManageUsers = new ApplicationPermission("Manage Users", "users.manage", UsersPermissionGroupName, "Permission to create, delete and modify other users account details");

        public const string RolesPermissionGroupName = "Role Permissions";
        public static ApplicationPermission ViewRoles = new ApplicationPermission("View Roles", "roles.view", RolesPermissionGroupName, "Permission to view available roles");
        public static ApplicationPermission ManageRoles = new ApplicationPermission("Manage Roles", "roles.manage", RolesPermissionGroupName, "Permission to create, delete and modify roles");
        public static ApplicationPermission AssignRoles = new ApplicationPermission("Assign Roles", "roles.assign", RolesPermissionGroupName, "Permission to assign roles to users");

        static ApplicationPermissions()
        {
            List<ApplicationPermission> allPermissions = new List<ApplicationPermission>()
            {
                ViewLogs,
                ManageLogs,

                ViewEvents,
                ManageEvents,
                ExecuteEvents,

                ViewUsers,
                ManageUsers,

                ViewRoles,
                ManageRoles,
                AssignRoles
            };

            AllPermissions = allPermissions.AsReadOnly();
        }

        public static ApplicationPermission GetPermissionByName(string permissionName)
        {
            return AllPermissions.Where(p => p.Name == permissionName).SingleOrDefault();
        }

        public static ApplicationPermission GetPermissionByValue(string permissionValue)
        {
            return AllPermissions.Where(p => p.Value == permissionValue).SingleOrDefault();
        }

        public static string[] GetAllPermissionValues()
        {
            return AllPermissions.Select(p => p.Value).ToArray();
        }

        public static string[] GetAdministrativePermissionValues()
        {
            return new string[] { ManageLogs, ManageEvents, ExecuteEvents, ManageUsers, ManageRoles, AssignRoles };
        }
    }
}
