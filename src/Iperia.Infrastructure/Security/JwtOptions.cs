namespace Iperia.Infrastructure.Security;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    // HMAC-SHA256 requiere al menos 32 caracteres (256 bits).
    public string SecretKey { get; init; } = string.Empty;
    public int ExpirationMinutes { get; init; } = 60;

    public bool IsValid() =>
        !string.IsNullOrWhiteSpace(Issuer)
        && !string.IsNullOrWhiteSpace(Audience)
        && SecretKey.Length >= 32
        && ExpirationMinutes > 0;
}
