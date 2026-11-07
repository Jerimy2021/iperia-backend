using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Iperia.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateEvidencesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "evidences",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    storage_key = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    content_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    size_bytes = table.Column<long>(type: "bigint", nullable: false),
                    uploaded_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evidences", x => x.id);
                    table.ForeignKey(
                        name: "FK_evidences_users_uploaded_by_user_id",
                        column: x => x.uploaded_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_evidences_created_at",
                table: "evidences",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_evidences_storage_key",
                table: "evidences",
                column: "storage_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_evidences_uploaded_by_user_id",
                table: "evidences",
                column: "uploaded_by_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "evidences");
        }
    }
}
