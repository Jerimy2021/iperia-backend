using Iperia.Domain.Common;
using Iperia.Domain.Exceptions;

namespace Iperia.Domain.Users;

public sealed class User : AggregateRoot
{
    public const int FullNameMaxLength = 150;

    public string FullName { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public Role Role { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Requerido por EF Core.
    private User()
    {
    }

    private User(Guid id, string fullName, Email email, string passwordHash, Role role, DateTime createdAt)
        : base(id)
    {
        FullName = fullName;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// Crea un usuario nuevo. El password llega ya hasheado: el dominio nunca ve el texto plano.
    /// </summary>
    public static User Register(string? fullName, Email email, string passwordHash, Role role = Role.Worker)
    {
        var name = fullName?.Trim() ?? string.Empty;

        if (name.Length == 0)
        {
            throw new DomainException("El nombre es obligatorio.");
        }

        if (name.Length > FullNameMaxLength)
        {
            throw new DomainException($"El nombre no puede superar {FullNameMaxLength} caracteres.");
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new DomainException("El hash del password es obligatorio.");
        }

        return new User(Guid.NewGuid(), name, email, passwordHash, role, DateTime.UtcNow);
    }

    public void ChangeRole(Role newRole)
    {
        if (!Enum.IsDefined(newRole))
        {
            throw new DomainException("El rol indicado no existe.");
        }

        Role = newRole;
    }
}
