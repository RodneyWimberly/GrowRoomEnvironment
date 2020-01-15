using FluentValidation;

namespace GrowRoomEnvironment.Web.ViewModels.Validators
{
    public class RoleViewModelValidator : AbstractValidator<RoleViewModel>
    {
        public RoleViewModelValidator()
        {
            RuleFor(r => r.Name).Length(2, 200).WithMessage("Role name must be between 2 and 200 characters in length");
        }
    }
}
