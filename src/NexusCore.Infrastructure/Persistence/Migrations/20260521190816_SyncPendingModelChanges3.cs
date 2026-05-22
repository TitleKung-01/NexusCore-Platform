// การย้ายข้อมูล: sync การเปลี่ยนแปลงโมเดลที่ค้าง (รอบที่ 3)
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusCore.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SyncPendingModelChanges3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoleDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CanApprove = table.Column<bool>(type: "boolean", nullable: false),
                    IsHrAccess = table.Column<bool>(type: "boolean", nullable: false),
                    IsBuiltIn = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleDefinitions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleDefinitions_Name",
                table: "RoleDefinitions",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleDefinitions");
        }
    }
}
