using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusCore.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SyncPendingModelChanges2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PerformanceReviews");

            migrationBuilder.DropTable(
                name: "ReviewCycles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReviewCycles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsOpen = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewCycles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PerformanceReviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CycleId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ManagerComment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ManagerId = table.Column<Guid>(type: "uuid", nullable: true),
                    ManagerScore = table.Column<int>(type: "integer", nullable: true),
                    SelfComment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    SelfScore = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerformanceReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerformanceReviews_EmployeeProfiles_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "EmployeeProfiles",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PerformanceReviews_ReviewCycles_CycleId",
                        column: x => x.CycleId,
                        principalTable: "ReviewCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceReviews_CycleId_EmployeeId",
                table: "PerformanceReviews",
                columns: new[] { "CycleId", "EmployeeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceReviews_EmployeeId",
                table: "PerformanceReviews",
                column: "EmployeeId");
        }
    }
}
