using FluentValidation;

namespace Locker.Api.Features.Auth.Models
{
    public class GetAuthTokenApiInput
    {
        public string GrantType { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }

    public class GetAuthTokenApiInputValidator : AbstractValidator<GetAuthTokenApiInput>
    {
        public GetAuthTokenApiInputValidator()
        {
            RuleFor(it => it.GrantType)
                .Must(type => type == "password" || type == "refresh_token")
                .WithMessage("Only password and refresh token grant types are supported.");

            RuleFor(it => it.UserName)
                .NotNull()
                .NotEmpty()
                .WithMessage("Username is required")
                .When(it => it.GrantType == "password");

            RuleFor(it => it.Password)
                .NotNull()
                .NotEmpty()
                .WithMessage("Password is required")
                .When(it => it.GrantType == "password");

            RuleFor(it => it.AccessToken)
                .NotNull()
                .NotEmpty()
                .WithMessage("AccessToken is required")
                .When(it => it.GrantType == "refresh_token");

            RuleFor(it => it.RefreshToken)
                .NotNull()
                .NotEmpty()
                .WithMessage("RefreshToken is required")
                .When(it => it.GrantType == "refresh_token");
        }
    }

    public class TokenApiOutput
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
