namespace Locker.Api.Configuration
{
    public class AutSettings
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string SigningKey { get; set; }

        public int AccessTokenLifetime { get; set; }
    }
}