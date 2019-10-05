using System;
using System.Diagnostics;
using BCryptNet = BCrypt.Net.BCrypt;

namespace Locker.Domain.Features.Auth.Entities
{
    [DebuggerDisplay("{UserName} {Id}")]
    public class User
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string PasswordHash { get; set; }

        public UserPermission[] Permissions { get; set; } = new UserPermission[0];

        public UserRefreshToken[] RefreshTokens { get; set; } = new UserRefreshToken[0];

        public bool PasswordsMatch(string password)
        {
            if (string.IsNullOrEmpty(PasswordHash) || string.IsNullOrEmpty(password))
            {
                return false;
            }

            return BCryptNet.Verify(password, PasswordHash);
        }

        public void SetPassword(string password)
        {
            PasswordHash = BCryptNet.HashPassword(password);
        }
    }

    [DebuggerDisplay("{Resource} {AccessRight}")]
    public class UserPermission
    {
        public string Resource { get; set; }

        public ResourceAccessRight AccessRight { get; set; }
    }

    [Flags]
    public enum ResourceAccessRight
    {
        Create = 1,
        Read = 1 << 1,
        Update = 1 << 2,
        Delete = 1 << 3,
        All = 1 << 4
    }

    public class UserRefreshToken
    {
        public string Token { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}