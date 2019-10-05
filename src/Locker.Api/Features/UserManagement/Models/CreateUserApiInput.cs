using FluentValidation;

namespace Locker.Api.Features.UserManagement.Models
{
    public class CreateUserApiInput
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public UserPermissionApiInput[] Permissions { get; set; }
    }

    public class CreateUserApiInputValidator : AbstractValidator<CreateUserApiInput>
    {
        public CreateUserApiInputValidator()
        {
            RuleFor(it => it.UserName)
                .NotNull()
                .NotEmpty()
                .WithMessage("Username is required");

            RuleFor(it => it.Password)
                .NotNull()
                .NotEmpty()
                .WithMessage("Password is required")
                .Length(6, int.MaxValue)
                .WithMessage("Password must be at least 6 character long");

            RuleFor(it => it.Permissions)
                .NotNull().WithMessage("Permissions is required");

            RuleForEach(it => it.Permissions)
                .SetValidator(new UserPermissionApiInputValidator());
        }
    }
}
