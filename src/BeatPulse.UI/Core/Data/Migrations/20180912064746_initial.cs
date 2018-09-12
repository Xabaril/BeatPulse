using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BeatPulse.UI.Core.Data.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LivenessConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LivenessUri = table.Column<string>(maxLength: 500, nullable: false),
                    LivenessName = table.Column<string>(maxLength: 500, nullable: false),
                    DiscoveryService = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LivenessConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LivenessExecutions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Status = table.Column<string>(maxLength: 50, nullable: false),
                    IsHealthy = table.Column<bool>(nullable: false),
                    OnStateFrom = table.Column<DateTime>(nullable: false),
                    LastExecuted = table.Column<DateTime>(nullable: false),
                    LivenessUri = table.Column<string>(maxLength: 500, nullable: false),
                    LivenessName = table.Column<string>(maxLength: 500, nullable: false),
                    LivenessResult = table.Column<string>(maxLength: 2000, nullable: false),
                    DiscoveryService = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LivenessExecutions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LivenessFailuresNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LivenessName = table.Column<string>(maxLength: 500, nullable: false),
                    LastNotified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LivenessFailuresNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LivenessExecutionHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Status = table.Column<string>(maxLength: 50, nullable: false),
                    On = table.Column<DateTime>(nullable: false),
                    LivenessExecutionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LivenessExecutionHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LivenessExecutionHistory_LivenessExecutions_LivenessExecutionId",
                        column: x => x.LivenessExecutionId,
                        principalTable: "LivenessExecutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LivenessExecutionHistory_LivenessExecutionId",
                table: "LivenessExecutionHistory",
                column: "LivenessExecutionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LivenessConfigurations");

            migrationBuilder.DropTable(
                name: "LivenessExecutionHistory");

            migrationBuilder.DropTable(
                name: "LivenessFailuresNotifications");

            migrationBuilder.DropTable(
                name: "LivenessExecutions");
        }
    }
}
