using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Locker.Api.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Locker.Api.Features.Auth
{
    public interface IAuthService
    {
        string GenerateAccessToken(IEnumerable<Claim> userClaims);

        string GenerateRefreshToken();

        ClaimsPrincipal ReadClaimsPrincipalFromExpiredToken(string token);
    }

    public class AuthService : IAuthService
    {
        private readonly AutSettings _autSettings;

        public AuthService(AutSettings autSettings)
        {
            _autSettings = autSettings;
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_autSettings.SigningKey));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _autSettings.Issuer,
                audience: _autSettings.Audience,
                expires: DateTime.Now.Add(TimeSpan.FromMinutes(_autSettings.AccessTokenLifetime)),
                claims: claims,
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal ReadClaimsPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = _autSettings.Audience,

                ValidateIssuer = true,
                ValidIssuer = _autSettings.Issuer,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_autSettings.SigningKey)),

                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal;
            SecurityToken securityToken;

            try
            {
                principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            }
            catch (Exception ex) when (ex is SecurityTokenValidationException || ex is ArgumentException)
            {
                return null;
            }

            if (!(securityToken is JwtSecurityToken jwtSecurityToken)
                || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                return null;

            return principal;
        }


    }
}
