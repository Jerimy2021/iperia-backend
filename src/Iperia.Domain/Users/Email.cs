using System.Text.RegularExpressions;
using Iperia.Domain.Common;
using Iperia.Domain.Exceptions;

namespace Iperia.Domain.Users;

/// <summary>
/// Email normalizado (sin espacios y en minúsculas) y con formato válido.
/// </summary>
public sealed partial class Email : ValueObject
{
    public const int MaxLength = 254;

    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string? value)
    {
        var normalized = value?.Trim().ToLowerInvariant() ?? string.Empty;

        if (!IsValid(normalized))
        {
            throw new DomainException("El email no tiene un formato válido.");
        }

        return new Email(normalized);
    }

    public static bool IsValid(string? value) =>
        !string.IsNullOrWhiteSpace(value)
        && value.Trim().Length <= MaxLength
        && EmailRegex().IsMatch(value.Trim());

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}
