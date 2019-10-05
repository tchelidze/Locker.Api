using FluentValidation;

namespace Locker.Api.Features.UserManagement.Models
{
    public class AddUserPermissionApiInput
    {
        public UserPermissionApiInput[] Permissions { get; set; }
    }

    public class AddUserPermissionApiInputValidator : AbstractValidator<AddUserPermissionApiInput>
    {
        public AddUserPermissionApiInputValidator()
        {
            RuleFor(it => it.Permissions)
                .NotNull().WithMessage("Permissions is required");

            RuleFor(it => it.Permissions)
                .Must(it => it?.Length > 0).WithMessage("At least one permission should be provided.");

            RuleForEach(it => it.Permissions)
                .SetValidator(new UserPermissionApiInputValidator());
        }
    }
}
