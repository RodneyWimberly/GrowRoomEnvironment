using FluentValidation;


namespace GrowRoomEnvironment.Web.ViewModels.Validators
{
    public class UserViewModelValidator : AbstractValidator<UserViewModel>
    {
        public UserViewModelValidator()
        {
            //Validation logic here
            RuleFor(user => user.UserName).MinimumLength(2).MaximumLength(200).WithMessage("Username must be between 2 and 200 characters");
            RuleFor(user => user.Email).EmailAddress().WithMessage("Invalid email address")
                .NotEmpty().MaximumLength(200).WithMessage("Email address must be between 1 and 200 characters");
            RuleFor(user => user.Roles).Must(r => r.Length > 0).WithMessage("Must contain at least 1 role");
        }
    }
}
