using Locker.Api.Features.Auth.Models;
using Locker.Api.Features.Shared;
using Locker.Api.Web.Filters;
using Locker.Domain.Features.Auth.Entities;
using Locker.Domain.Features.Auth.Repositories;
using Locker.Domain.Features.Shared.ExecutionResult;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Locker.Api.Features.Auth
{
    [ValidateModelStateFilter]
    public class AuthController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;

        public AuthController(
            IUserRepository userRepository,
            IAuthService authService)
        {
            _userRepository = userRepository;
            _authService = authService;
        }

        private IEnumerable<Claim> GetUserClaims(User user) =>
            user.Permissions.GroupBy(it => it.Resource).Select(group =>
                    new Claim(@group.Key, string.Join(',', @group.Select(it => it.AccessRight))))
                .Concat(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id) });


        [AllowAnonymous, HttpPost("api/auth/token")]
        public Task<IActionResult> GetToken([BindRequired, FromBody] GetAuthTokenApiInput input)
        {
            if (input.GrantType == "password")
            {
                return GetTokenViaPasswordGrantType(input.UserName, input.Password);
            }

            if (input.GrantType == "refresh_token")
            {
                return GetTokenViaRefreshTokenGrantType(input.AccessToken, input.RefreshToken);
            }

            throw new NotSupportedException($"{input.GrantType} is not supported.");
        }

        [NonAction]
        public async Task<IActionResult> GetTokenViaPasswordGrantType(string userName, string password)
        {
            var user = await _userRepository.FindByUsername(userName).ConfigureAwait(false);

            if (user == null || !user.PasswordsMatch(password))
            {
                return new ExecutionResult
                {
                    ResultType = ExecutionResultType.ValidationError,
                    Message = "Invalid username or password."
                }.ToActionResult();
            }

            var accessToken = _authService.GenerateAccessToken(GetUserClaims(user));

            var refreshToken = _authService.GenerateRefreshToken();
            await _userRepository.AddRefreshToken(user.Id, refreshToken).ConfigureAwait(false);

            return new ExecutionResult<TokenApiOutput>
            {
                ResultType = ExecutionResultType.Ok,
                Value = new TokenApiOutput
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                }
            }.ToActionResult();
        }

        [NonAction]
        public async Task<IActionResult> GetTokenViaRefreshTokenGrantType(string accessToken, string refreshToken)
        {
            var claimsPrincipal = _authService.ReadClaimsPrincipalFromExpiredToken(accessToken);

            if (claimsPrincipal == null)
            {
                return new ExecutionResult
                {
                    ResultType = ExecutionResultType.ValidationError,
                    Message = "Invalid access token."
                }.ToActionResult();
            }

            var userId = claimsPrincipal.Claims.First(it => it.Type == ClaimTypes.NameIdentifier).Value;

            if (!await _userRepository.TryUseRefreshToken(userId, refreshToken).ConfigureAwait(false))
            {
                return new ExecutionResult
                {
                    ResultType = ExecutionResultType.ValidationError,
                    Message = "Invalid refresh token."
                }.ToActionResult();
            }

            var newRefreshToken = _authService.GenerateRefreshToken();
            await _userRepository.AddRefreshToken(userId, newRefreshToken).ConfigureAwait(false);

            var user = await _userRepository.Get(userId).ConfigureAwait(false);

            return new ExecutionResult<TokenApiOutput>
            {
                ResultType = ExecutionResultType.Ok,
                Value = new TokenApiOutput
                {
                    AccessToken = _authService.GenerateAccessToken(GetUserClaims(user)),
                    RefreshToken = newRefreshToken
                }
            }.ToActionResult();
        }
    }
}