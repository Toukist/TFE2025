namespace Dior.Library.DTOs
{
    public sealed class JwtOptions
    {
        public string Issuer { get; set; } = "DiorServiceHost";
        public string Audience { get; set; } = "DiorClient";
        public int ExpirationMinutes { get; set; } = 60;
        // La cl� NE DOIT PAS �tre commit�e. R�cup�r�e via secrets/env.
        public string? Secret { get; set; }
    }
}