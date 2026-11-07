using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Iperia.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateIpercTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "puestos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_puestos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tareas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    puesto_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tareas", x => x.id);
                    table.ForeignKey(
                        name: "FK_tareas_puestos_puesto_id",
                        column: x => x.puesto_id,
                        principalTable: "puestos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "actividades",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    tarea_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_actividades", x => x.id);
                    table.ForeignKey(
                        name: "FK_actividades_tareas_tarea_id",
                        column: x => x.tarea_id,
                        principalTable: "tareas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "peligros",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    actividad_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_peligros", x => x.id);
                    table.ForeignKey(
                        name: "FK_peligros_actividades_actividad_id",
                        column: x => x.actividad_id,
                        principalTable: "actividades",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "riesgos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    probabilidad = table.Column<int>(type: "integer", nullable: false),
                    severidad = table.Column<int>(type: "integer", nullable: false),
                    valor = table.Column<int>(type: "integer", nullable: false),
                    peligro_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_riesgos", x => x.id);
                    table.ForeignKey(
                        name: "FK_riesgos_peligros_peligro_id",
                        column: x => x.peligro_id,
                        principalTable: "peligros",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_actividades_tarea_id",
                table: "actividades",
                column: "tarea_id");

            migrationBuilder.CreateIndex(
                name: "IX_peligros_actividad_id",
                table: "peligros",
                column: "actividad_id");

            migrationBuilder.CreateIndex(
                name: "IX_riesgos_peligro_id",
                table: "riesgos",
                column: "peligro_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tareas_puesto_id",
                table: "tareas",
                column: "puesto_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "riesgos");

            migrationBuilder.DropTable(
                name: "peligros");

            migrationBuilder.DropTable(
                name: "actividades");

            migrationBuilder.DropTable(
                name: "tareas");

            migrationBuilder.DropTable(
                name: "puestos");
        }
    }
}
