using System;
using FluentValidation;
using Locker.Domain.Features.LockManagement.Entities;

namespace Locker.Api.Features.LockManagement.Models
{
    public class ChangeLockStateApiInput
    {
        public LockingState State { get; set; }
    }

    public class ChangeLockStateApiInputValidator : AbstractValidator<ChangeLockStateApiInput>
    {
        public ChangeLockStateApiInputValidator()
        {
            RuleFor(it => it.State).Must(it => Enum.IsDefined(typeof(LockingState), it)).WithMessage("Invalid state");
        }
    }
}
