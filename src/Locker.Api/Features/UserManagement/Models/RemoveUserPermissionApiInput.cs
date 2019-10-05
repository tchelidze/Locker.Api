using FluentValidation;

namespace Locker.Api.Features.UserManagement.Models
{
    public class RemoveUserPermissionApiInput
    {
        public string[] Resources { get; set; }
    }

    public class RemoveUserPermissionApiInputValidator : AbstractValidator<RemoveUserPermissionApiInput>
    {
        public RemoveUserPermissionApiInputValidator()
        {
            RuleFor(it => it.Resources)
                .NotNull().WithMessage("Resources is required")
                .Must(it => it?.Length > 0).WithMessage("At least one resource should be provided.");
        }
    }
}