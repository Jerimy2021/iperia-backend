namespace Iperia.Domain.Users;

public enum Role
{
    /// <summary>Administrador del sistema: tiene los permisos de todos los demás roles.</summary>
    Admin,
    Operario,
    Supervisor,
    GerenciaEhs,
    Brigadista
}
