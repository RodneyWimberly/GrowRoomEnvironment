using FluentValidation;

namespace GrowRoomEnvironment.Web.ViewModels.Validators
{
    public class UserEditViewModelValidator : AbstractValidator<UserEditViewModel>
    {
        public UserEditViewModelValidator()
        {
            RuleFor(user => user.Roles).Must(r => r.Length > 0).WithMessage("Must contain at least 1 role");
            RuleFor(user => user.NewPassword).Length(4, 20).WithMessage("Password must be between 4 and 20 characters in length");
        }
    }
}
