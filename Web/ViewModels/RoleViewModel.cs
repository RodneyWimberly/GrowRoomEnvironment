﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace GrowRoomEnvironment.Web.ViewModels
{
    public class RoleViewModel : AuditableViewModelBase
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Role name is required"), StringLength(200, MinimumLength = 2, ErrorMessage = "Role name must be between 2 and 200 characters")]
        public string Name { get; set; }

        public string Description { get; set; }

        public int UsersCount { get; set; }

        public ICollection<PermissionViewModel> Permissions { get; set; }
    }
}
