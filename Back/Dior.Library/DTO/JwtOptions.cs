namespace Dior.Library.DTOs
{
    public sealed class JwtOptions
    {
        public string Issuer { get; set; } = "DiorServiceHost";
        public string Audience { get; set; } = "DiorClient";
        public int ExpirationMinutes { get; set; } = 60;
        // La clé NE DOIT PAS être commitée. Récupérée via secrets/env.
        public string? Secret { get; set; }
    }
}