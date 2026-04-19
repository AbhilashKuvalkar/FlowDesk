using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowDesk.TicketService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSlaPolicy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SlaPolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ResponseTimeMinutes = table.Column<int>(type: "int", nullable: false),
                    ResolutionTimeMinutes = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlaPolicies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SlaPolicies_TenantId_Priority",
                table: "SlaPolicies",
                columns: new[] { "TenantId", "Priority" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SlaPolicies");
        }
    }
}
