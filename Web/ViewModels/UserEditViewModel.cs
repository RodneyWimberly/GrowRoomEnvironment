﻿namespace GrowRoomEnvironment.Web.ViewModels
{
    public class UserEditViewModel : UserBaseViewModel
    {
        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }

        public string[] Roles { get; set; }
    }
}
