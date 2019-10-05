using FluentValidation;
using Locker.Domain.Features.Auth.Entities;

namespace Locker.Api.Features.UserManagement.Models
{
    public class UserPermissionApiInput
    {
        public string Resource { get; set; }

        public ResourceAccessRight AccessRight { get; set; }
    }

    public class UserPermissionApiInputValidator : AbstractValidator<UserPermissionApiInput>
    {
        public UserPermissionApiInputValidator()
        {
            RuleFor(it => it.Resource).NotNull().NotEmpty();
        }
    }
}
