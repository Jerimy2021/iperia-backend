using Iperia.Application.Common.Exceptions;

namespace Iperia.Application.UseCases.Auth;

internal static class PasswordPolicy
{
    public const int MinLength = 8;
    // BCrypt solo considera los primeros 72 bytes.
    public const int MaxLength = 72;

    public static void EnsureIsValid(string? password)
    {
        if (string.IsNullOrEmpty(password)
            || password.Length < MinLength
            || password.Length > MaxLength
            || !password.Any(char.IsUpper)
            || !password.Any(char.IsLower)
            || !password.Any(char.IsDigit))
        {
            throw new ValidationException(
                $"El password debe tener entre {MinLength} y {MaxLength} caracteres e incluir mayúsculas, minúsculas y números.");
        }
    }
}
