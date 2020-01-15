namespace GrowRoomEnvironment.Web.ViewModels
{
    public class UserViewModel : UserBaseViewModel
    {
        public bool IsLockedOut { get; set; }

        public string[] Roles { get; set; }
    }
}
