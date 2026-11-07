using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Iperia.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameWorkerRoleToOperario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // El rol Worker pasó a llamarse Operario; el rol se guarda como texto.
            migrationBuilder.Sql("UPDATE users SET role = 'Operario' WHERE role = 'Worker';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Los roles nuevos no existen en la versión anterior: se degradan a Worker.
            migrationBuilder.Sql("UPDATE users SET role = 'Worker' WHERE role IN ('Operario', 'GerenciaEhs', 'Brigadista');");
        }
    }
}
